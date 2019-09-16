//---------------------------------------------------------------------
// <copyright file="ResourceBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Client.ALinq.UriParser;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using PathSegmentToken = Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken;
    using NonSystemToken = Microsoft.OData.Client.ALinq.UriParser.NonSystemToken;
    #endregion Namespaces

    /// <summary>
    /// This class provides a Bind method that analyzes an input <see cref="Expression"/> and returns a bound tree.
    /// </summary>
    internal class ResourceBinder : DataServiceALinqExpressionVisitor
    {
        private const string AddQueryOptionMethodName = "AddQueryOption";

        private const string ExpandMethodName = "Expand";

        private const string IncludeTotalCountMethodName = "IncludeTotalCount";

        /// <summary>
        /// The associated DataServiceContext instance. DevNote(shank): this is used for determining
        /// the fully-qualified name of types when TypeAs converts are processed (C# "as", VB "TryCast").
        /// Ideally the FQN is only required during URI translation, not during analysis. However,
        /// the current code constructs the $select and $expand parts of the URI during analysis. This
        /// could be refactored in the future to defer the $select and $expand URI construction until
        /// the URI translation phase.
        /// </summary>
        private readonly DataServiceContext context;

        /// <summary>Convenience property: model.</summary>
        private ClientEdmModel Model
        {
            get
            {
                return this.context.Model;
            }
        }


        private ResourceBinder(DataServiceContext context)
        {
            this.context = context;
        }

        /// <summary>Analyzes and binds the specified expression.</summary>
        /// <param name="e">Expression to bind.</param>
        /// <param name="context">The context of the expression used for bind.</param>
        /// <returns>
        /// The expression with bound nodes (annotated expressions used by
        /// the Expression-to-URI translator).
        /// </returns>
        internal static Expression Bind(Expression e, DataServiceContext context)
        {
            Debug.Assert(e != null, "e != null");

            ResourceBinder rb = new ResourceBinder(context);
            Expression boundExpression = rb.Visit(e);
            VerifyKeyPredicates(boundExpression);
            VerifyNotSelectManyProjection(boundExpression);
            return boundExpression;
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression"/> is
        /// missing necessary key predicates.
        /// </summary>
        /// <param name="expression">Expression to check.</param>
        /// <returns>
        /// true if the expression is a navigation expression and doesn't
        /// have the necessary key predicates on the associated resource
        /// expression; false otherwise.
        /// </returns>
        internal static bool IsMissingKeyPredicates(Expression expression)
        {
            ResourceExpression re = expression as ResourceExpression;
            if (re != null)
            {
                if (IsMissingKeyPredicates(re.Source))
                {
                    return true;
                }

                if (re.Source != null && !re.IsOperationInvocation)
                {
                    ResourceSetExpression rse = re.Source as ResourceSetExpression;
                    if ((rse != null) && !rse.HasKeyPredicate)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Verifies that all key predicates are assigned to the specified expression.
        /// </summary>
        /// <param name="e">Expression to verify.</param>
        internal static void VerifyKeyPredicates(Expression e)
        {
            if (IsMissingKeyPredicates(e))
            {
                throw new NotSupportedException(Strings.ALinq_CantNavigateWithoutKeyPredicate);
            }
        }

        /// <summary>Verifies that the specified <paramref name="expression"/> is not a projection based on SelectMany.</summary>
        /// <param name="expression">Expression to check.</param>
        internal static void VerifyNotSelectManyProjection(Expression expression)
        {
            Debug.Assert(expression != null, "expression != null");

            // Check that there isn't a SelectMany projection (or if there is one,
            // that there isn't an associated transparent scope for the resource
            // set reference).
            QueryableResourceExpression resourceExpression = expression as QueryableResourceExpression;
            if (resourceExpression != null)
            {
                ProjectionQueryOptionExpression projection = resourceExpression.Projection;
                if (projection != null)
                {
                    Debug.Assert(projection.Selector != null, "projection.Selector != null -- otherwise incorrectly constructed");
                    MethodCallExpression call = StripTo<MethodCallExpression>(projection.Selector.Body);
                    if (call != null && call.Method.Name == "SelectMany")
                    {
                        throw new NotSupportedException(Strings.ALinq_UnsupportedExpression(call));
                    }
                }
                else if (resourceExpression.HasTransparentScope)
                {
                    throw new NotSupportedException(Strings.ALinq_UnsupportedExpression(resourceExpression));
                }
            }
        }

        /// <summary>Analyzes a predicate (Where clause).</summary>
        /// <param name="mce"><see cref="MethodCallExpression"/> for a Where call.</param>
        /// <param name="model">The model.</param>
        /// <returns>
        /// An equivalent expression to <paramref name="mce"/>, possibly a different one with additional annotations.
        /// </returns>
        private static Expression AnalyzePredicate(MethodCallExpression mce, ClientEdmModel model)
        {
            Debug.Assert(mce != null, "mce != null -- caller couldn't have know the expression kind otherwise");
            Debug.Assert(mce.Method.Name == "Where", "mce.Method.Name == 'Where' -- otherwise this isn't a predicate");

            // Validate that the input is a resource set and retrieve the Lambda that defines the predicate
            QueryableResourceExpression input;
            LambdaExpression le;
            if (!TryGetResourceSetMethodArguments(mce, out input, out le))
            {
                // might have failed because of singleton, so throw better error if so.
                ValidationRules.RequireNonSingleton(mce.Arguments[0]);
                return mce;
            }

            ValidationRules.CheckPredicate(le.Body, model);

            //
            // Valid predicate patterns are as follows:
            // 1. A URI-compatible filter applied to the input resource set
            // 2. A key-predicate filter applied to the input resource set
            // - Additionally, key-predicate filters may be applied to any resource path component
            //   that does not already have a key-predicate filter, regardless of any filter applied
            //   to the current input resource set, if transparent scopes are present.
            // - It is not valid to apply a key-predicate or a filter to a resource set
            //   for which key-predicate is already present.
            // - It is valid to apply a filter to a resource set for which a filter already exists;
            //   such filters are AND'd together.
            //
            // [Key-predicate that targets a path component AND]* [Key-predicate over input | URI-compatible filter over input]+

            List<Expression> conjuncts = new List<Expression>();
            AddConjuncts(le.Body, conjuncts);

            Dictionary<QueryableResourceExpression, List<Expression>> predicatesByTarget = new Dictionary<QueryableResourceExpression, List<Expression>>(ReferenceEqualityComparer<QueryableResourceExpression>.Instance);
            List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
            foreach (Expression e in conjuncts)
            {
                ValidateFilter(e, mce.Method.Name);
                Expression reboundPredicate = InputBinder.Bind(e, input, le.Parameters[0], referencedInputs);
                if (referencedInputs.Count > 1)
                {
                    // UNSUPPORTED: A single clause cannot refer to more than one resource set
                    return mce;
                }

                QueryableResourceExpression boundTarget = (referencedInputs.Count == 0 ? input : referencedInputs[0] as QueryableResourceExpression);
                if (boundTarget == null)
                {
                    // UNSUPPORTED: Each clause must refer to a path component that is a resource set, not a singleton navigation property
                    return mce;
                }

                List<Expression> targetPredicates = null;
                if (!predicatesByTarget.TryGetValue(boundTarget, out targetPredicates))
                {
                    targetPredicates = new List<Expression>();
                    predicatesByTarget[boundTarget] = targetPredicates;
                }

                targetPredicates.Add(reboundPredicate);
                referencedInputs.Clear();
            }

            conjuncts = null;
            List<Expression> inputPredicates;
            if (predicatesByTarget.TryGetValue(input, out inputPredicates))
            {
                predicatesByTarget.Remove(input);
            }
            else
            {
                inputPredicates = null;
            }

            foreach (KeyValuePair<QueryableResourceExpression, List<Expression>> predicates in predicatesByTarget)
            {
                QueryableResourceExpression target = predicates.Key;
                List<Expression> clauses = predicates.Value;
                List<Expression> nonKeyPredicates;
                List<Expression> keyPredicates = ExtractKeyPredicate(target, clauses, model, out nonKeyPredicates);
                if (keyPredicates == null ||
                    (nonKeyPredicates != null && nonKeyPredicates.Count > 0))
                {
                    // UNSUPPORTED: Only key predicates may be applied to earlier path components
                    return mce;
                }

                // Earlier path components must be navigation sources, and navigation sources cannot have query options.
                Debug.Assert(!target.HasQueryOptions, "Navigation source had query options?");

                target.SetKeyPredicate(keyPredicates);
            }

            if (inputPredicates != null)
            {
                List<Expression> keyPredicates = null;
                List<Expression> nonKeyPredicates = null;

                // Get the current key predicates
                List<Expression> currentPredicates = input.KeyPredicateConjuncts.ToList();

                if (input.Filter != null && input.Filter.PredicateConjuncts.Count > 0)
                {
                    // Get the filter predicates
                    currentPredicates = currentPredicates.Union(input.Filter.PredicateConjuncts.Union(inputPredicates)).ToList();
                }
                else
                {
                    currentPredicates = currentPredicates.Union(inputPredicates).ToList();
                }

                if (!input.UseFilterAsPredicate)
                {
                    keyPredicates = ExtractKeyPredicate(input, currentPredicates, model, out nonKeyPredicates);
                }

                if (keyPredicates != null)
                {
                    input.SetKeyPredicate(keyPredicates);
                    input.RemoveFilterExpression();
                }

                // A key predicate cannot be applied if query options other than 'Expand' are present,
                // so merge the key predicate into the filter query option instead.
                if (keyPredicates != null && input.HasSequenceQueryOptions)
                {
                    input.ConvertKeyToFilterExpression();
                }

                if (keyPredicates == null)
                {
                    input.ConvertKeyToFilterExpression();
                    input.AddFilter(inputPredicates);
                }
            }

            return input; // No need to adjust this.currentResource - filters are merged in all cases
        }

        /// <summary>
        /// Validates neither operands of the binary expression filter ends with TypeAs
        /// </summary>
        /// <param name="exp">filter expression</param>
        /// <param name="method">method name where this filter is passed to</param>
        private static void ValidateFilter(Expression exp, string method)
        {
            Debug.Assert(exp != null && exp.Type == typeof(Boolean), "exp != null && exp.Type == typeof(Boolean)");
            BinaryExpression e = ResourceBinder.StripTo<BinaryExpression>(exp) as BinaryExpression;
            if (e != null)
            {
                ValidationRules.DisallowExpressionEndWithTypeAs(e.Left, method);
                ValidationRules.DisallowExpressionEndWithTypeAs(e.Right, method);
            }
        }

        /// <summary>
        /// Given a list of predicates, extracts key values for the specified <paramref name="target"/>.
        /// </summary>
        /// <param name="target">Target set.</param>
        /// <param name="predicates">Candidate predicates.</param>
        /// <param name="nonKeyPredicates">Returns list of non-key predicates.</param>
        /// <returns>List of key predicates if found, otherwise null</returns>
        private static List<Expression> ExtractKeyPredicate(
            QueryableResourceExpression target,
            List<Expression> predicates,
            ClientEdmModel edmModel,
            out List<Expression> nonKeyPredicates)
        {
            Debug.Assert(target != null, "target != null");
            Debug.Assert(predicates != null, "predicates != null");

            Dictionary<PropertyInfo, ConstantExpression> keyValuesFromPredicates = null;
            nonKeyPredicates = null;
            List<Expression> keyPredicates = null;

            foreach (Expression predicate in predicates)
            {
                PropertyInfo property;
                ConstantExpression constantValue;
                if (PatternRules.MatchKeyComparison(predicate, out property, out constantValue))
                {
                    if (keyValuesFromPredicates == null)
                    {
                        keyValuesFromPredicates = new Dictionary<PropertyInfo, ConstantExpression>(EqualityComparer<PropertyInfo>.Default);
                        keyPredicates = new List<Expression>();
                    }
                    else if (keyValuesFromPredicates.ContainsKey(property))
                    {
                        // UNSUPPORTED: <property1> = <value1> AND <property1> = <value2> are multiple key predicates and
                        // cannot be represented as a resource path.
                        throw Error.NotSupported(Strings.ALinq_CanOnlyApplyOneKeyPredicate);
                    }

                    keyValuesFromPredicates.Add(property, constantValue);
                    keyPredicates.Add(predicate);
                }
                else
                {
                    if (nonKeyPredicates == null)
                    {
                        nonKeyPredicates = new List<Expression>();
                    }

                    nonKeyPredicates.Add(predicate);
                }
            }

            if (nonKeyPredicates != null && nonKeyPredicates.Count > 0)
            {
                // If there is any non-key predicate, everything must go in filter statement,
                target.UseFilterAsPredicate = true;
                keyPredicates = null;
            }
            else
            {
                Debug.Assert(keyValuesFromPredicates != null || nonKeyPredicates != null, "No key predicates or non-key predicates found?");
                if (keyValuesFromPredicates != null)
                {
                    // Get the EDM schema type for target's Resource Type
                    var schemaType = (IEdmEntityType)edmModel.GetOrCreateEdmType(target.CreateReference().Type);

                    Debug.Assert(schemaType != null, "SchemaType can not be null.");

                    // Obtain the key properties from EdmEntityType, and compare them with keys obtained from predicates
                    // By this time we are sure that keyValuesFromPredicates has unique set (i.e. no duplicates), and has all the keys in it.
                    // So just comparing count with EDMModel's keys is enough for validation
                    bool allKeysPresent = schemaType.Key().Count() == keyValuesFromPredicates.Keys.Count;

                    if (!allKeysPresent)
                    {
                        keyValuesFromPredicates = null;
                        keyPredicates = null;
                    }
                }
            }

            return keyPredicates;
        }

        /// <summary>Adds all AND'ed expressions to the specified <paramref name="conjuncts"/> list.</summary>
        /// <param name="e">Expression to recursively add conjuncts from.</param>
        /// <param name="conjuncts">Target list of conjucts.</param>
        private static void AddConjuncts(Expression e, List<Expression> conjuncts)
        {
            Debug.Assert(conjuncts != null, "conjuncts != null");
            if (PatternRules.MatchAnd(e))
            {
                BinaryExpression be = (BinaryExpression)e;
                AddConjuncts(be.Left, conjuncts);
                AddConjuncts(be.Right, conjuncts);
            }
            else
            {
                conjuncts.Add(e);
            }
        }

        /// <summary>
        /// Analyzes the specified call to see whether it is recognized as a
        /// projection that is satisfied with $select usage.
        /// </summary>
        /// <param name="mce">Call expression to analyze.</param>
        /// <param name="sequenceMethod">Kind of sequence method to analyze.</param>
        /// <param name="e">Resulting expression.</param>
        /// <returns>true if <paramref name="mce"/> is a projection that can be satisfied with $select; false otherwise.</returns>
        internal bool AnalyzeProjection(MethodCallExpression mce, SequenceMethod sequenceMethod, out Expression e)
        {
            Debug.Assert(mce != null, "mce != null");
            Debug.Assert(
                sequenceMethod == SequenceMethod.Select || sequenceMethod == SequenceMethod.SelectManyResultSelector,
                "sequenceMethod == SequenceMethod.Select(ManyResultSelector)");

            e = mce;

            bool matchMembers = sequenceMethod == SequenceMethod.SelectManyResultSelector;
            ResourceExpression source = this.Visit(mce.Arguments[0]) as ResourceExpression;
            if (source == null)
            {
                return false;
            }

            if (sequenceMethod == SequenceMethod.SelectManyResultSelector)
            {
                // The processing for SelectMany for a projection is similar to that
                // of a regular Select as a projection, however in the latter the
                // signature is .Select(source, selector), whereas in the former
                // the signature is .SelectMany(source, collector, selector), where
                // the selector's source is the result of the collector.
                //
                // Only simple collectors (single member access) are supported.
                Expression collectionSelector = mce.Arguments[1];
                if (!PatternRules.MatchParameterMemberAccess(collectionSelector))
                {
                    return false;
                }

                Expression resultSelector = mce.Arguments[2];
                LambdaExpression resultLambda;
                if (!PatternRules.MatchDoubleArgumentLambda(resultSelector, out resultLambda))
                {
                    return false;
                }

                if (ExpressionPresenceVisitor.IsExpressionPresent(resultLambda.Parameters[0], resultLambda.Body))
                {
                    return false;
                }

                // Build a version of the collection body that has transparent identifiers
                // resolved and create a new resource reference for the navigation collection;
                // this is the source for the selector.
                List<ResourceExpression> referencedExpressions = new List<ResourceExpression>();
                LambdaExpression collectionLambda = StripTo<LambdaExpression>(collectionSelector);
                Expression collectorReference = InputBinder.Bind(collectionLambda.Body, source, collectionLambda.Parameters[0], referencedExpressions);
                collectorReference = StripCastMethodCalls(collectorReference);
                MemberExpression setNavigationMember;
                if (!PatternRules.MatchPropertyProjectionRelatedSet(source, collectorReference, this.context, out setNavigationMember))
                {
                    return false;
                }

                ResourceExpression resultSelectorSource = CreateResourceSetExpression(mce.Method.ReturnType, source, setNavigationMember, TypeSystem.GetElementType(setNavigationMember.Type));

                if (!PatternRules.MatchMemberInitExpressionWithDefaultConstructor(resultSelectorSource, resultLambda) &&
                    !PatternRules.MatchNewExpression(resultSelectorSource, resultLambda))
                {
                    return false;
                }

                resultLambda = Expression.Lambda(resultLambda.Body, new ParameterExpression[] { resultLambda.Parameters[1] });

                // Ideally, the projection analyzer would return true/false and an
                // exception with the explanation of the results if it failed;
                // however to minimize churn we will use a try/catch on
                // NotSupportedException instead.
                ResourceExpression resultWithProjection = resultSelectorSource.CreateCloneWithNewType(mce.Type);
                bool isProjection;
                try
                {
                    isProjection = ProjectionAnalyzer.Analyze(resultLambda, resultWithProjection, false, this.context);
                }
                catch (NotSupportedException)
                {
                    isProjection = false;
                }

                if (!isProjection)
                {
                    return false;
                }

                e = resultWithProjection;
                ValidationRules.RequireCanProject(resultSelectorSource);
            }
            else
            {
                LambdaExpression lambda;
                if (!PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out lambda))
                {
                    return false;
                }

                // the projection might be over a transparent identifier, so first try to rewrite if that is the case
                lambda = ProjectionRewriter.TryToRewrite(lambda, source);

                ResourceExpression re = source.CreateCloneWithNewType(mce.Type);

                // See whether the lambda matches a projection that is satisfied with $select usage.
                if (!ProjectionAnalyzer.Analyze(lambda, re, matchMembers, this.context))
                {
                    return false;
                }

                // Defer validating until after the projection has been analyzed since the lambda could represent a
                // navigation that we do not want to turn into a projection.
                ValidationRules.RequireCanProject(source);
                e = re;
            }

            return true;
        }

        /// <summary>
        /// Analyzes the specified method call as a WCF Data
        /// Services navigation operation.
        /// </summary>
        /// <param name="mce">Expression to analyze.</param>
        /// <param name="context">Data service context instance</param>
        /// <returns>An expression that represents the potential navigation.</returns>
        internal static Expression AnalyzeNavigation(MethodCallExpression mce, DataServiceContext context)
        {
            Debug.Assert(mce != null, "mce != null");
            Expression input = mce.Arguments[0];
            LambdaExpression le;
            ResourceExpression navSource;
            Expression boundProjection;
            MemberExpression navigationMember;

            if (!PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out le))
            {
                return mce;
            }

            ValidationRules.DisallowExpressionEndWithTypeAs(le.Body, mce.Method.Name);
            if (PatternRules.MatchIdentitySelector(le))
            {
                return input;
            }
            else if (PatternRules.MatchTransparentIdentitySelector(input, le, context))
            {
                return RemoveTransparentScope(mce.Method.ReturnType, (QueryableResourceExpression)input);
            }
            else if (IsValidNavigationSource(input, out navSource) &&
                TryBindToInput(navSource, le, out boundProjection) &&
                PatternRules.MatchPropertyProjectionSingleton(navSource, boundProjection, context, out navigationMember))
            {
                ValidationRules.DisallowMemberAccessInNavigation(navigationMember, context.Model);
                boundProjection = navigationMember;
                return CreateNavigationPropertySingletonExpression(mce.Method.ReturnType, navSource, boundProjection);
            }

            return mce;
        }

        private static bool IsValidNavigationSource(Expression input, out ResourceExpression sourceExpression)
        {
            ValidationRules.RequireCanNavigate(input);
            sourceExpression = input as ResourceExpression;
            return sourceExpression != null;
        }

        /// <summary>
        /// Analyzes a .Select or .SelectMany method call to determine
        /// whether it's allowed, to identify transparent identifiers
        /// in appropriate .SelectMany() cases, returning the method
        /// call or a resource set expression.
        /// </summary>
        /// <param name="mce">Expression to analyze.</param>
        /// <param name="context">Data service context instance</param>
        /// <returns>
        /// <paramref name="mce"/>, or a new resource set expression for
        /// the target resource in the method call for navigation properties.
        /// </returns>
        internal static Expression AnalyzeSelectMany(MethodCallExpression mce, DataServiceContext context)
        {
            Debug.Assert(mce != null, "mce != null");

            if (mce.Arguments.Count != 2 && mce.Arguments.Count != 3)
            {
                return mce;
            }

            ResourceExpression input;
            if (!IsValidNavigationSource(mce.Arguments[0], out input))
            {
                return mce;
            }

            LambdaExpression collectorSelector;
            if (!PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out collectorSelector))
            {
                return mce;
            }

            ValidationRules.DisallowExpressionEndWithTypeAs(collectorSelector.Body, mce.Method.Name);
            List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
            Expression navPropRef = InputBinder.Bind(collectorSelector.Body, input, collectorSelector.Parameters[0], referencedInputs);

            QueryableResourceExpression rse;
            if (!TryAnalyzeSelectManyCollector(input, navPropRef, context, out rse))
            {
                return mce;
            }
            else if (rse.Type != mce.Method.ReturnType)
            {
                rse = rse.CreateCloneForTransparentScope(mce.Method.ReturnType);
            }

            if (mce.Arguments.Count == 3)
            {
                return AnalyzeSelectManySelector(mce, rse, context);
            }
            else
            {
                return rse;
            }
        }

        /// <summary>
        /// Analyzes the nav prop reference for a SelectMany method call
        /// building the appropriate ResourceSetExpression if one can be
        /// created.
        /// </summary>
        /// <param name="input">Input resource expression to the collector</param>
        /// <param name="navPropRef">The navigation property reference to analyze</param>
        /// <param name="context">Data service context instance</param>
        /// <param name="result">The resource set expression</param>
        /// <returns>true if succesful, else false</returns>
        private static bool TryAnalyzeSelectManyCollector(ResourceExpression input, Expression navPropRef, DataServiceContext context, out QueryableResourceExpression result)
        {
            MethodCallExpression call = StripTo<MethodCallExpression>(navPropRef);

            SequenceMethod sequenceMethod;

            if (call != null && ReflectionUtil.TryIdentifySequenceMethod(call.Method, out sequenceMethod) &&
                (sequenceMethod == SequenceMethod.Cast || sequenceMethod == SequenceMethod.OfType))
            {
                // Recursively analyze Cast<> and OfType<> inputs
                if (TryAnalyzeSelectManyCollector(input, call.Arguments[0], context, out result))
                {
                    call = Expression.Call(call.Method, result);
                    if (sequenceMethod == SequenceMethod.Cast)
                    {
                        result = AnalyzeCast(call) as QueryableResourceExpression;
                    }
                    else
                    {
                        result = AnalyzeOfType(call) as QueryableResourceExpression;
                    }
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                MemberExpression setNavigationMember;
                if (PatternRules.MatchPropertyProjectionRelatedSet(input, navPropRef, context, out setNavigationMember))
                {
                    // Ground case for success - we have a member access from the input resource set
                    Type memberType = TypeSystem.GetElementType(setNavigationMember.Type);
                    result = CreateResourceSetExpression(setNavigationMember.Type, input, setNavigationMember, memberType);
                }
                else
                {
                    result = null;
                }
            }
            return result != null;
        }

        /// <summary>
        /// Analyzes a SelectMany method call that ranges over a resource set and
        /// returns the same method or the annotated resource set.
        /// </summary>
        /// <param name="selectManyCall">SelectMany method to analyze.</param>
        /// <param name="sourceResource">Source resource set for SelectMany result.</param>
        /// <param name="context">Context of expression to analyze.</param>
        /// <returns>The visited expression.</returns>
        /// <remarks>
        /// The <paramref name="sourceResource"/> expression represents the
        /// navigation produced by the collector of the SelectMany() method call.
        /// </remarks>
        private static Expression AnalyzeSelectManySelector(MethodCallExpression selectManyCall, QueryableResourceExpression sourceResource, DataServiceContext context)
        {
            Debug.Assert(selectManyCall != null, "selectManyCall != null");

            LambdaExpression selector = StripTo<LambdaExpression>(selectManyCall.Arguments[2]);

            // Check for transparent scope result - projects the input and the selector
            Expression result;
            QueryableResourceExpression.TransparentAccessors transparentScope;
            if (PatternRules.MatchTransparentScopeSelector(sourceResource, selector, out transparentScope))
            {
                sourceResource.TransparentScope = transparentScope;
                result = sourceResource;
            }
            else if (PatternRules.MatchIdentityProjectionResultSelector(selector))
            {
                result = sourceResource;
            }
            else if (PatternRules.MatchMemberInitExpressionWithDefaultConstructor(sourceResource, selector) || PatternRules.MatchNewExpression(sourceResource, selector))
            {
                // Projection analyzer will throw if it selector references first ParamExpression, so this is safe to do here.
                selector = Expression.Lambda(selector.Body, new ParameterExpression[] { selector.Parameters[1] });

                if (!ProjectionAnalyzer.Analyze(selector, sourceResource, false, context))
                {
                    result = selectManyCall;
                }
                else
                {
                    result = sourceResource;
                }
            }
            else
            {
                result = selectManyCall;
            }

            return result;
        }

        internal static Expression ApplyOrdering(MethodCallExpression mce, bool descending, bool thenBy, ClientEdmModel model)
        {
            ValidationRules.CheckOrderBy(mce, model);

            QueryableResourceExpression input;
            LambdaExpression le;
            if (!TryGetResourceSetMethodArguments(mce, out input, out le))
            {
                // UNSUPPORTED: Expected LambdaExpression as second argument to sequence method
                return mce;
            }

            ValidationRules.DisallowExpressionEndWithTypeAs(le.Body, mce.Method.Name);
            Expression selector;
            if (!TryBindToInput(input, le, out selector))
            {
                // UNSUPPORTED: Lambda should reference the input, and only the input
                return mce;
            }
            List<OrderByQueryOptionExpression.Selector> selectors;
            if (!thenBy)
            {
                selectors = new List<OrderByQueryOptionExpression.Selector>();
                AddSequenceQueryOption(input, new OrderByQueryOptionExpression(mce.Type, selectors));
            }
            else
            {
                Debug.Assert(input.OrderBy != null, "input.OrderBy != null");
                selectors = input.OrderBy.Selectors;
            }

            selectors.Add(new OrderByQueryOptionExpression.Selector(selector, descending));

            // The ResourceSetExpression 'input' can be of type IQueryable<T> (for example after OfType<T>() is called). Since OrderBy returns
            // IOrderedQueryable<T>, we need to change the ResourceSetExpression to IOrderedQueryable<T> so that ThenBy can compose correctly after this.
            if (input.Type != mce.Method.ReturnType)
            {
                return input.CreateCloneWithNewType(mce.Method.ReturnType);
            }

            return input;
        }

        /// <summary>Ensures that there's a limit on the cardinality of a query.</summary>
        /// <param name="mce"><see cref="MethodCallExpression"/> for the method to limit First/Single(OrDefault).</param>
        /// <param name="maxCardinality">Maximum cardinality to allow.</param>
        /// <returns>
        /// An expression that limits <paramref name="mce"/> to no more than <paramref name="maxCardinality"/> elements.
        /// </returns>
        /// <remarks>This method is used by .First(OrDefault) and .Single(OrDefault) to limit cardinality.</remarks>
        private static Expression LimitCardinality(MethodCallExpression mce, int maxCardinality)
        {
            Debug.Assert(mce != null, "mce != null");
            Debug.Assert(maxCardinality > 0, "Cardinality must be at least 1");

            if (mce.Arguments.Count != 1)
            {
                // Don't support methods with predicates.
                return mce;
            }

            ResourceSetExpression rse = mce.Arguments[0] as ResourceSetExpression;
            if (rse != null)
            {
                if (!rse.HasKeyPredicate && // no-op, already returns a singleton
                    (ResourceExpressionType)rse.NodeType != ResourceExpressionType.ResourceNavigationProperty)
                {
                    if (rse.Take == null || (int)rse.Take.TakeAmount.Value > maxCardinality)
                    {
                        AddSequenceQueryOption(rse, new TakeQueryOptionExpression(mce.Type, Expression.Constant(maxCardinality)));
                    }
                }
                return mce.Arguments[0];
            }
            else if (mce.Arguments[0] is NavigationPropertySingletonExpression || mce.Arguments[0] is SingletonResourceExpression)
            {
                // no-op, already returns a singleton
                return mce.Arguments[0];
            }

            return mce;
        }

        private static Expression AnalyzeCast(MethodCallExpression mce)
        {
            ResourceExpression re = mce.Arguments[0] as ResourceExpression;
            if (re != null)
            {
                return re.CreateCloneWithNewType(mce.Method.ReturnType);
            }

            return mce;
        }

        /// <summary>
        /// Analyzes the OfType method call
        /// </summary>
        /// <param name="mce">The OfType method call expression</param>
        /// <returns><paramref name="mce"/>, or a resource set expression with ResourceTypeAs set.</returns>
        private static Expression AnalyzeOfType(MethodCallExpression mce)
        {
            QueryableResourceExpression rse = mce.Arguments[0] as QueryableResourceExpression;
            if (rse != null)
            {
                Type filteredType;
                filteredType = mce.Method.GetGenericArguments().SingleOrDefault();

                if (filteredType == null)
                {
                    throw new InvalidOperationException(Strings.ALinq_OfTypeArgumentNotAvailable);
                }

                if (filteredType.IsAssignableFrom(rse.ResourceType))
                {
                    return rse;
                }
                else if (rse.ResourceType.IsAssignableFrom(filteredType))
                {
                    if (rse.ResourceTypeAs != null)
                    {
                        throw new NotSupportedException(Strings.ALinq_CannotUseTypeFiltersMultipleTimes);
                    }

                    // only apply the OfType filter if it is not redundant
                    rse.ResourceTypeAs = filteredType;
                    return rse.CreateCloneWithNewType(mce.Method.ReturnType);
                }
            }

            return mce;
        }

        private static bool IsTypeOfGenericBaseType(Type baseType, Type derivedType)
        {
            while (derivedType != null && derivedType != typeof(object))
            {
                var currentType = derivedType.IsGenericType() ? derivedType.GetGenericTypeDefinition() : derivedType;
                if (currentType == baseType)
                {
                    return true;
                }

                derivedType = derivedType.GetBaseType();
            }

            return false;
        }

        private static Expression StripConvert(Expression e, Type selfDefinedConvertType)
        {
            UnaryExpression ue = e as UnaryExpression;

            // TODO: We are going to allow either of DataServiceQuery or DataServiceOrderedQuery
            // to be the type of ResourceExpression in the cast parameter. Although this might be considered
            // overly relaxed we want to avoid causing breaking changes by just having the Ordered version
            if (ue != null && ue.NodeType == ExpressionType.Convert && ue.Type.GetBaseType() == typeof(DataServiceContext))
            {
                e = ue.Operand;
            }
            else
            {
                if (ue != null && ue.NodeType == ExpressionType.Convert &&
                    IsTypeOfGenericBaseType(typeof(DataServiceQuery<>), ue.Type))
                {
                    e = ue.Operand;
                    ResourceExpression re = e as ResourceExpression;
                    if (re != null && selfDefinedConvertType != null)
                    {
                        e = re.CreateCloneWithNewType(selfDefinedConvertType);
                    }
                    else if (re != null)
                    {
                        e = re.CreateCloneWithNewType(ue.Type);
                    }
                }
            }

            return e;
        }

        private static Expression AnalyzeExpand(MethodCallExpression mce, DataServiceContext context)
        {
            Expression obj = StripConvert(mce.Object, null);
            ResourceExpression re = obj as ResourceExpression;
            if (re == null)
            {
                return mce;
            }

            ValidationRules.RequireCanExpand(re);
            Expression arg = StripTo<Expression>(mce.Arguments[0]);
            string path = null;
            if (arg.NodeType == ExpressionType.Constant)
            {
                path = (string)((ConstantExpression)arg).Value;
            }
            else if (arg.NodeType == ExpressionType.Lambda)
            {
                Version uriVersion;
                ValidationRules.ValidateExpandPath(arg, context, out path, out uriVersion);
                re.RaiseUriVersion(uriVersion);
            }
            else
            {
                Debug.Assert(false, "Unexpected NodeType: " + arg.NodeType);
            }

            if (!re.ExpandPaths.Contains(path))
            {
                re.ExpandPaths.Add(path);
            }

            return re;
        }

        private static Expression AnalyzeFunc(MethodCallExpression mce, bool isExtensionMethod)
        {
            Expression obj;
            if (isExtensionMethod)
            {
                obj = StripConvert(mce.Arguments[0], mce.Method.ReturnType);
            }
            else
            {
                obj = StripConvert(mce.Object, mce.Method.ReturnType);
            }

            ResourceExpression re = obj as ResourceExpression;
            if (re == null)
            {
                return mce;
            }

            if (re.OperationName == null)
            {
                re.OperationName = mce.Method.Name;
            }

            // for now there's just functionimport that use this function, so it's always false.
            re.IsAction = false;

            string[] names = mce.Method.GetParameters().Select(p => p.Name).ToArray();
            for (int i = isExtensionMethod ? 1 : 0; i < mce.Arguments.Count - 1; ++i)
            {
                Expression arg = StripTo<Expression>(mce.Arguments[i]);
                string value = null;
                if (arg.NodeType == ExpressionType.Constant)
                {
                    ConstantExpression constantArgs = (ConstantExpression)arg;
                    value = ODataUriUtils.ConvertToUriLiteral(constantArgs.Value, ODataVersion.V4);
                }
                else
                {
                    Debug.Assert(false, "Unexpected NodeType: " + arg.NodeType);
                }

                KeyValuePair<string, string> parameters = new KeyValuePair<string, string>(names[i], value);
                if (!re.OperationParameters.Contains(parameters))
                {
                    re.OperationParameters.Add(names[i], value);
                }
            }

            return re;
        }

        private static Expression AnalyzeAddCustomQueryOption(MethodCallExpression mce)
        {
            Expression obj = StripConvert(mce.Object, null);
            ResourceExpression re = obj as ResourceExpression;
            if (re == null)
            {
                return mce;
            }

            ValidationRules.RequireCanAddCustomQueryOption(re);

            ConstantExpression name = StripTo<ConstantExpression>(mce.Arguments[0]);
            ConstantExpression value = StripTo<ConstantExpression>(mce.Arguments[1]);

            if (((string)name.Value).Trim() == UriHelper.DOLLARSIGN + UriHelper.OPTIONEXPAND)
            {
                // if the user is setting $expand option, need to merge with other existing expand paths that may have been alredy added
                // check for allow expansion
                ValidationRules.RequireCanExpand(re);
                re.ExpandPaths = re.ExpandPaths.Union(new string[] { (string)value.Value }, StringComparer.Ordinal).ToList();
            }
            else
            {
                ValidationRules.RequireLegalCustomQueryOption(mce.Arguments[0], re);
                re.CustomQueryOptions.Add(name, value);
            }

            return re;
        }

        private static Expression AnalyzeAddCountOption(MethodCallExpression mce, CountOption countOption)
        {
            Expression obj = StripConvert(mce.Object, null);
            QueryableResourceExpression rse = obj as QueryableResourceExpression;
            if (rse == null)
            {
                return mce;
            }

            ValidationRules.RequireCanAddCount(rse);
            rse.ConvertKeyToFilterExpression();
            rse.CountOption = countOption;

            return rse;
        }

        /// <summary>Creates a new resource set as produced by a navigation.</summary>
        /// <param name="type">
        /// The type of the expression as it appears in the tree (possibly
        /// with transparent scopes).
        /// </param>
        /// <param name="source">The source of the set.</param>
        /// <param name="memberExpression">The member access on <paramref name="source"/> that yields the set.</param>
        /// <param name="resourceType">The resource type on the set.</param>
        /// <returns>A new <see cref="QueryableResourceExpression"/> instance.</returns>
        private static QueryableResourceExpression CreateResourceSetExpression(Type type, ResourceExpression source, Expression memberExpression, Type resourceType)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(source != null, "source != null");
            Debug.Assert(memberExpression != null, "memberExpression != null");
            Debug.Assert(resourceType != null, "resourceType != null");

            // ResourceSetExpressions can always have order information,
            // so return them as IOrderedQueryable<> always. Necessary to allow
            // OrderBy results that get aliased to a previous expression work
            // with ThenBy.
            Type elementType = TypeSystem.GetElementType(type);
            Debug.Assert(elementType != null, "elementType != null -- otherwise the set isn't going to act like a collection");
            Type expressionType = typeof(IOrderedQueryable<>).MakeGenericType(elementType);

            // TODO: Should we create SingletonExpression here?
            ResourceSetExpression newResource = new ResourceSetExpression(expressionType, source, memberExpression, resourceType, source.ExpandPaths.ToList(), source.CountOption, source.CustomQueryOptions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), null /*projection*/, null /*resourceTypeAs*/, source.UriVersion);
            source.ExpandPaths.Clear();
            source.CountOption = CountOption.None;
            source.CustomQueryOptions.Clear();
            return newResource;
        }

        /// <summary>Creates a new resource singleton as produced by a navigation.</summary>
        /// <param name="type">
        /// The type of the expression as it appears in the tree (possibly
        /// with transparent scopes).
        /// </param>
        /// <param name="source">The source of the singleton.</param>
        /// <param name="memberExpression">The member access on <paramref name="source"/> that yields the singleton.</param>
        /// <returns>A new <see cref="NavigationPropertySingletonExpression"/> instance.</returns>
        private static NavigationPropertySingletonExpression CreateNavigationPropertySingletonExpression(Type type, ResourceExpression source, Expression memberExpression)
        {
            NavigationPropertySingletonExpression newResource = new NavigationPropertySingletonExpression(type, source, memberExpression, memberExpression.Type, source.ExpandPaths.ToList(), source.CountOption, source.CustomQueryOptions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), null /*projection*/, null /*resourceTypeAs*/, source.UriVersion);
            source.ExpandPaths.Clear();
            source.CountOption = CountOption.None;
            source.CustomQueryOptions.Clear();
            return newResource;
        }

        /// <summary>
        /// Produces a new <see cref="QueryableResourceExpression"/> that is a clone of <paramref name="input"/> in all respects,
        /// other than its result type - which will be <paramref name="expectedResultType"/> - and its transparent scope,
        /// which will be <c>null</c>. This is a shallow clone operation - sequence query options, key predicate, etc are
        /// not cloned, but are reassigned to the new instance. The <paramref name="input"/> resource expression should be
        /// discarded after being used with this method.
        /// </summary>
        /// <param name="expectedResultType">The result type - <see cref="Expression.Type"/> - that the new resource set expression should have.</param>
        /// <param name="input">The resource set expression from which the transparent scope is being removed</param>
        /// <returns>A new resource set expression without an enclosing transparent scope and with the specified result type.</returns>
        private static QueryableResourceExpression RemoveTransparentScope(Type expectedResultType, QueryableResourceExpression input)
        {
            // Create a new resource set expression based on the input
            ResourceSetExpression newResource = new ResourceSetExpression(expectedResultType, input.Source, input.MemberExpression, input.ResourceType, input.ExpandPaths, input.CountOption, input.CustomQueryOptions, input.Projection, input.ResourceTypeAs, input.UriVersion);

            // Reassign state items that are not constructor arguments - query options + key predicate
            newResource.SetKeyPredicate(input.KeyPredicateConjuncts);
            foreach (QueryOptionExpression queryOption in input.SequenceQueryOptions)
            {
                newResource.AddSequenceQueryOption(queryOption);
            }

            // Instruct the new resource set expression to use input's Input Reference instead of creating its own.
            // This will also update the Input Reference to specify the new resource set expression as it's target,
            // so that any usage of it in query option expressions is consistent with the new resource set expression.
            newResource.OverrideInputReference(input);

            return newResource;
        }

        /// <summary>Returns the specified expression, stripping redundant converts.</summary>
        /// <param name="e">Expression to return.</param>
        /// <returns>e, or the underlying expression for redundant converts.</returns>
        internal static Expression StripConvertToAssignable(Expression e)
        {
            Debug.Assert(e != null, "e != null");

            Expression result;
            UnaryExpression unary = e as UnaryExpression;
            if (unary != null && PatternRules.MatchConvertToAssignable(unary))
            {
                result = unary.Operand;
            }
            else
            {
                result = e;
            }

            return result;
        }

        /// <summary>
        /// Strips the specifed <paramref name="expression"/> of intermediate
        /// expression (unnecessary converts and quotes) and returns
        /// the underlying expression of type T (or null if it's not of that type).
        /// </summary>
        /// <typeparam name="T">Type of expression to return.</typeparam>
        /// <param name="expression">Expression to consider.</param>
        /// <returns>The underlying expression for <paramref name="expression"/>.</returns>
        internal static T StripTo<T>(Expression expression) where T : Expression
        {
            Debug.Assert(expression != null, "expression != null");

            Expression result;
            do
            {
                result = expression;
                expression = expression.NodeType == ExpressionType.Quote ? ((UnaryExpression)expression).Operand : expression;
                expression = StripConvertToAssignable(expression);
            }
            while (result != expression);

            return result as T;
        }

        /// <summary>
        /// Strips the specifed <paramref name="expression"/> of intermediate unnecessary converts
        /// and quotes, and returns the underlying expression of type T (or null if it's not of that type).
        /// </summary>
        /// <typeparam name="T">Type of expression to return.</typeparam>
        /// <param name="expression">Expression to consider.</param>
        /// <param name="convertedType">The converted type for typeAs expressions.</param>
        /// <returns>The underlying expression for <paramref name="expression"/>.</returns>
        internal static T StripTo<T>(Expression expression, out Type convertedType) where T : Expression
        {
            Debug.Assert(expression != null, "expression != null");

            Expression result;
            convertedType = null;
            do
            {
                result = expression;
                expression = expression.NodeType == ExpressionType.Quote ? ((UnaryExpression)expression).Operand : expression;

                if (expression.NodeType == ExpressionType.Convert ||
                    expression.NodeType == ExpressionType.ConvertChecked ||
                    expression.NodeType == ExpressionType.TypeAs)
                {
                    UnaryExpression ue = expression as UnaryExpression;
                    if (ue != null)
                    {
                        if (PatternRules.MatchConvertToAssignable(ue))
                        {
                            expression = ue.Operand; // remove redundant casts (equivalent and upcasts)
                        }
                        else if (expression.NodeType == ExpressionType.TypeAs)
                        {
                            if (convertedType != null)
                            {
                                // We do not support more than one instance of a necessary TypeAs conversion,
                                //   e.g., ((p as Employee) as Customer)
                                // Note 1: this also means that we do not support multiple downcasts as these are redundant,
                                //   e.g., ((p as Employee) as Manager)
                                //         from e in p.OfType<Employee>() select e as Manager
                                throw new NotSupportedException(Strings.ALinq_CannotUseTypeFiltersMultipleTimes);
                            }
                            else
                            {
                                // Note 2: no check if this is a downcast (see note 1 above).
                                expression = ue.Operand;
                                convertedType = ue.Type;
                            }
                        }
                    }
                }
            }
            while (result != expression);

            return result as T;
        }

        internal override Expression VisitQueryableResourceExpression(QueryableResourceExpression rse)
        {
            Debug.Assert(rse != null, "rse != null");

            if ((ResourceExpressionType)rse.NodeType == ResourceExpressionType.RootResourceSet || (ResourceExpressionType)rse.NodeType == ResourceExpressionType.RootSingleResource)
            {
                // Actually, the user may already have composed an expansion, so
                // we'll find a query option here.
                // Debug.Assert(!rse.HasQueryOptions, "!rse.HasQueryOptions");

                // since we could be adding query options to the root, create a new one which can be mutable.
                return QueryableResourceExpression.CreateNavigationResourceExpression(rse.NodeType, rse.Type, rse.Source, rse.MemberExpression, rse.ResourceType, null /*expandPaths*/, CountOption.None, null /*customQueryOptions*/, null /*projection*/, rse.ResourceTypeAs, rse.UriVersion, rse.OperationName, rse.OperationParameters);
            }
            return rse;
        }

        private static bool TryGetResourceSetMethodArguments(MethodCallExpression mce, out QueryableResourceExpression input, out LambdaExpression lambda)
        {
            input = null;
            lambda = null;

            input = mce.Arguments[0] as QueryableResourceExpression;
            if (input != null &&
                PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out lambda))
            {
                return true;
            }

            return false;
        }

        private static bool TryBindToInput(ResourceExpression input, LambdaExpression le, out Expression bound)
        {
            List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
            bound = InputBinder.Bind(le.Body, input, le.Parameters[0], referencedInputs);
            if (referencedInputs.Count > 1 || (referencedInputs.Count == 1 && referencedInputs[0] != input))
            {
                bound = null;
            }

            return bound != null;
        }

        private static Expression AnalyzeResourceSetConstantMethod(MethodCallExpression mce, Func<MethodCallExpression, ResourceExpression, ConstantExpression, Expression> constantMethodAnalyzer)
        {
            ResourceExpression input = (ResourceExpression)mce.Arguments[0];
            ConstantExpression constantArg = StripTo<ConstantExpression>(mce.Arguments[1]);
            if (null == constantArg)
            {
                // UNSUPPORTED: A ConstantExpression is expected
                return mce;
            }

            return constantMethodAnalyzer(mce, input, constantArg);
        }

        private static Expression AnalyzeCountMethod(MethodCallExpression mce)
        {
            // [Res].LongCount()
            // [Res].Count()
            // It is either a ResourceExpression or a collection property.
            QueryableResourceExpression rse = mce.Arguments[0] as QueryableResourceExpression;
            if (rse == null)
            {
                return mce;
            }

            ValidationRules.RequireCanAddCount(rse);
            rse.ConvertKeyToFilterExpression();
            rse.CountOption = CountOption.CountSegment;

            return rse;
        }

        private static void AddSequenceQueryOption(ResourceExpression target, QueryOptionExpression qoe)
        {
            QueryableResourceExpression rse = (QueryableResourceExpression)target;
            rse.ConvertKeyToFilterExpression();

            // Validation that can add option
            switch (qoe.NodeType)
            {
                case (ExpressionType)ResourceExpressionType.FilterQueryOption:
                    if (rse.Skip != null)
                    {
                        throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("filter", "skip"));
                    }
                    else if (rse.Take != null)
                    {
                        throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("filter", "top"));
                    }
                    else if (rse.Projection != null)
                    {
                        throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("filter", "select"));
                    }
                    break;
                case (ExpressionType)ResourceExpressionType.OrderByQueryOption:
                    if (rse.Skip != null)
                    {
                        throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("orderby", "skip"));
                    }
                    else if (rse.Take != null)
                    {
                        throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("orderby", "top"));
                    }
                    else if (rse.Projection != null)
                    {
                        throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("orderby", "select"));
                    }
                    break;
                case (ExpressionType)ResourceExpressionType.SkipQueryOption:
                    if (rse.Take != null)
                    {
                        throw new NotSupportedException(Strings.ALinq_QueryOptionOutOfOrder("skip", "top"));
                    }
                    break;
                default:
                    break;
            }

            rse.AddSequenceQueryOption(qoe);
        }

        internal override Expression VisitBinary(BinaryExpression b)
        {
            Expression e = base.VisitBinary(b);
            if (PatternRules.MatchStringAddition(e))
            {
                BinaryExpression be = StripTo<BinaryExpression>(e);
                MethodInfo mi = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
                return Expression.Call(mi, new Expression[] { be.Left, be.Right });
            }

            return e;
        }

        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            Expression e = base.VisitMemberAccess(m);
            MemberExpression me = StripTo<MemberExpression>(e);
            PropertyInfo pi;
            Expression boundTarget;
            MethodInfo mi;
            if (me != null &&
                PatternRules.MatchNonPrivateReadableProperty(me, out pi, out boundTarget) &&
                TypeSystem.TryGetPropertyAsMethod(pi, out mi))
            {
                return Expression.Call(me.Expression, mi);
            }

            return e;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Large switch is necessary")]
        internal override Expression VisitMethodCall(MethodCallExpression mce)
        {
            Expression e;

            // check first to see if projection (not a navigation) so that func does recursively analyze selector
            // Currently the patterns being looked for in the selector are mutually exclusive from naviagtion patterns looked at later.
            SequenceMethod sequenceMethod;
            if (ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out sequenceMethod))
            {
                // The leaf projection can be one of Select(source, selector) or
                // SelectMany(source, collectionSelector, resultSelector).
                if (sequenceMethod == SequenceMethod.Select ||
                    sequenceMethod == SequenceMethod.SelectManyResultSelector)
                {
                    if (this.AnalyzeProjection(mce, sequenceMethod, out e))
                    {
                        return e;
                    }
                }
            }

            e = base.VisitMethodCall(mce);
            mce = e as MethodCallExpression;

            if (mce != null)
            {
                if (ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out sequenceMethod))
                {
                    switch (sequenceMethod)
                    {
                        case SequenceMethod.Where:
                            return AnalyzePredicate(mce, this.Model);
                        case SequenceMethod.Select:
                            return AnalyzeNavigation(mce, this.context);
                        case SequenceMethod.SelectMany:
                        case SequenceMethod.SelectManyResultSelector:
                            {
                                Expression result = AnalyzeSelectMany(mce, this.context);
                                return result;
                            }

                        case SequenceMethod.Take:
                            return AnalyzeResourceSetConstantMethod(mce, (callExp, resource, takeCount) => { AddSequenceQueryOption(resource, new TakeQueryOptionExpression(callExp.Type, takeCount)); return resource; });
                        case SequenceMethod.Skip:
                            return AnalyzeResourceSetConstantMethod(mce, (callExp, resource, skipCount) => { AddSequenceQueryOption(resource, new SkipQueryOptionExpression(callExp.Type, skipCount)); return resource; });
                        case SequenceMethod.OrderBy:
                            return ApplyOrdering(mce, /*descending=*/false, /*thenBy=*/false, this.Model);
                        case SequenceMethod.ThenBy:
                            return ApplyOrdering(mce, /*descending=*/false, /*thenBy=*/true, this.Model);
                        case SequenceMethod.OrderByDescending:
                            return ApplyOrdering(mce, /*descending=*/true, /*thenBy=*/false, this.Model);
                        case SequenceMethod.ThenByDescending:
                            return ApplyOrdering(mce, /*descending=*/true, /*thenBy=*/true, this.Model);
                        case SequenceMethod.First:
                        case SequenceMethod.FirstOrDefault:
                            return LimitCardinality(mce, 1);
                        case SequenceMethod.Single:
                        case SequenceMethod.SingleOrDefault:
                            return LimitCardinality(mce, 2);
                        case SequenceMethod.Cast:
                            return AnalyzeCast(mce);
                        case SequenceMethod.OfType:
                            return AnalyzeOfType(mce);
                        case SequenceMethod.Any:
                        case SequenceMethod.All:
                        case SequenceMethod.AnyPredicate:
                            return mce;
                        case SequenceMethod.LongCount:
                        case SequenceMethod.Count:
                            return AnalyzeCountMethod(mce);
                        default:
                            throw Error.MethodNotSupported(mce);
                    }
                }
                else if ((mce.Method.DeclaringType.IsGenericType() &&
                    IsTypeOfGenericBaseType(typeof(DataServiceQuery<>), mce.Method.DeclaringType))
                    || (mce.Method.GetParameters().Any() && IsTypeOfGenericBaseType(typeof(DataServiceQuery<>), mce.Method.GetParameters()[0].ParameterType)))
                {
                    Type t;
                    if (mce.Method.DeclaringType.IsGenericType() && IsTypeOfGenericBaseType(typeof(DataServiceQuery<>), mce.Method.DeclaringType))
                    {
                        t = typeof(DataServiceQuery<>).MakeGenericType(mce.Method.DeclaringType.GetGenericArguments()[0]);
                    }
                    else
                    {
                        t = typeof(DataServiceQuery<>).MakeGenericType(mce.Method.GetParameters()[0].ParameterType.GetGenericArguments()[0]);
                    }

                    if (mce.Method.Name == ExpandMethodName && mce.Method.DeclaringType == t)
                    {
                        return AnalyzeExpand(mce, this.context);
                    }
                    if (mce.Method.GetParameters().Any() && mce.Method.GetParameters()[0].ParameterType == t)
                    {
                        return AnalyzeFunc(mce, true);
                    }
                    if (mce.Method.Name == AddQueryOptionMethodName && mce.Method.DeclaringType == t)
                    {
                        return AnalyzeAddCustomQueryOption(mce);
                    }
                    if (mce.Method.Name == IncludeTotalCountMethodName && mce.Method.DeclaringType == t)
                    {
                        return AnalyzeAddCountOption(mce, CountOption.CountQuery);
                    }
                    else
                    {
                        throw Error.MethodNotSupported(mce);
                    }
                }
                else if (mce.Method.DeclaringType != null && mce.Method.DeclaringType.GetBaseType() == typeof(DataServiceContext))
                {
                    return AnalyzeFunc(mce, false);
                }

                return mce;
            }

            return e;
        }

        /// <summary>Strips calls to .Cast() methods, returning the underlying expression.</summary>
        /// <param name="expression">Expression to strip calls from.</param>
        /// <returns>The underlying expression.</returns>
        /// <remarks>
        /// Note that this method drops information about what the casts were,
        /// and is only supported for collector selectors in SelectMany() calls,
        /// to enable scenarios such as from t in ctx.Tables from Customer c in t.Items...
        /// </remarks>
        private static Expression StripCastMethodCalls(Expression expression)
        {
            Debug.Assert(expression != null, "expression != null");

            MethodCallExpression call = StripTo<MethodCallExpression>(expression);
            while (call != null && ReflectionUtil.IsSequenceMethod(call.Method, SequenceMethod.Cast))
            {
                expression = call.Arguments[0];
                call = StripTo<MethodCallExpression>(expression);
            }

            return expression;
        }

        /// <summary>Use this class to perform pattern-matching over expression trees.</summary>
        /// <remarks>
        /// Following these guidelines simplifies usage:
        ///
        /// - Return true/false for matches, and interesting matched information in out parameters.
        ///
        /// - If one of the inputs to be matched undergoes "skipping" for unnecesary converts,
        ///   return the same member as an out parameter. This forces callers to be aware that
        ///   they should use the more precise representation for computation (without having
        ///   to rely on a normalization step).
        /// </remarks>
        internal static class PatternRules
        {
            /// <summary>
            /// Checks whether the <paramref name="expression"/> is a convert that
            /// always succeeds because it converts to the same target type or a
            /// base type.
            /// </summary>
            /// <param name="expression">Expression to match.</param>
            /// <returns>
            /// true if <paramref name="expression"/> is a convert to same or base type; false otherwise.
            /// </returns>
            internal static bool MatchConvertToAssignable(UnaryExpression expression)
            {
                Debug.Assert(expression != null, "expression != null");

                if (expression.NodeType != ExpressionType.Convert &&
                    expression.NodeType != ExpressionType.ConvertChecked &&
                    expression.NodeType != ExpressionType.TypeAs)
                {
                    return false;
                }

                return expression.Type.IsAssignableFrom(expression.Operand.Type);
            }

            /// <summary>
            /// Checks whether <paramref name="expression"/> is a lambda of the
            /// form (p) => p.member[.member]* (possibly quoted).
            /// </summary>
            /// <param name="expression">Expression to match.</param>
            /// <returns>true if the expression is a match; false otherwise.</returns>
            /// <remarks>
            /// This method strip .Call methods because it's currently used only
            /// to supporte .SelectMany() collector selectors. If this method
            /// is reused for other purposes, this behavior should be made
            /// conditional or factored out.
            /// </remarks>
            internal static bool MatchParameterMemberAccess(Expression expression)
            {
                Debug.Assert(expression != null, "lambda != null");

                LambdaExpression lambda = StripTo<LambdaExpression>(expression);
                if (lambda == null || lambda.Parameters.Count != 1)
                {
                    return false;
                }

                ParameterExpression parameter = lambda.Parameters[0];
                Expression body = StripCastMethodCalls(lambda.Body);
                MemberExpression memberAccess = StripTo<MemberExpression>(body);
                while (memberAccess != null)
                {
                    if (memberAccess.Expression == parameter)
                    {
                        return true;
                    }

                    memberAccess = StripTo<MemberExpression>(memberAccess.Expression);
                }

                return false;
            }

            /// <summary>
            /// Checks whether the specified expression is a path of member
            /// access expressions.
            /// </summary>
            /// <param name="e">Expression to match.</param>
            /// <param name="context">Data service context instance</param>
            /// <param name="member">Expression equivalent to <paramref name="e"/>, without unnecessary converts.</param>
            /// <param name="instance">Expression from which the path starts.</param>
            /// <param name="propertyPath">Path of member names from <paramref name="instance"/>.</param>
            /// <param name="uriVersion">Uri version.</param>
            /// <returns>true if there is at least one property in the path; false otherwise.</returns>
            internal static bool MatchPropertyAccess(Expression e, DataServiceContext context, out MemberExpression member, out Expression instance, out PathSegmentToken propertyPath, out Version uriVersion)
            {
                instance = null;
                propertyPath = null;
                uriVersion = Util.ODataVersion4;

                MemberExpression me = StripTo<MemberExpression>(e);
                member = me;
                while (me != null)
                {
                    PropertyInfo pi;
                    Expression boundTarget;
                    if (MatchNonPrivateReadableProperty(me, out pi, out boundTarget))
                    {
                        string propertyName = ClientTypeUtil.GetServerDefinedName(pi);

                        NonSystemToken newPropertyToAdd = new NonSystemToken(propertyName, null, null);
                        if (propertyPath == null)
                        {
                            propertyPath = newPropertyToAdd;
                        }
                        else
                        {
                            newPropertyToAdd.NextToken = propertyPath;
                            propertyPath = newPropertyToAdd;
                        }
                        e = me.Expression;
                        Type convertedType;
                        me = ResourceBinder.StripTo<MemberExpression>(e, out convertedType);
                        if (convertedType != null)
                        {
                            NonSystemToken subPropertyToAdd = new NonSystemToken(UriHelper.GetEntityTypeNameForUriAndValidateMaxProtocolVersion(convertedType, context, ref uriVersion), null, null);
                            subPropertyToAdd.NextToken = propertyPath;
                            propertyPath = subPropertyToAdd;
                        }
                    }
                    else
                    {
                        me = null;
                    }
                }

                if (propertyPath != null)
                {
                    instance = e;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Checks whether the specified expression is a path of member
            /// access expressions.
            /// </summary>
            /// <param name="e">Expression to match.</param>
            /// <param name="context">Data service context instance</param>
            /// <param name="member">Expression equivalent to <paramref name="e"/>, without unnecessary converts.</param>
            /// <param name="instance">Expression from which the path starts.</param>
            /// <param name="propertyPath">Path of member names from <paramref name="instance"/>.</param>
            /// <param name="uriVersion">Uri version.</param>
            /// <returns>true if there is at least one property in the path; false otherwise.</returns>
            internal static bool MatchPropertyAccess(Expression e, DataServiceContext context, out MemberExpression member, out Expression instance, out List<string> propertyPath, out Version uriVersion)
            {
                instance = null;
                propertyPath = null;
                uriVersion = Util.ODataVersion4;

                MemberExpression me = StripTo<MemberExpression>(e);
                member = me;
                while (me != null)
                {
                    PropertyInfo pi;
                    Expression boundTarget;
                    if (MatchNonPrivateReadableProperty(me, out pi, out boundTarget))
                    {
                        if (propertyPath == null)
                        {
                            propertyPath = new List<string>();
                        }

                        propertyPath.Insert(0, pi.Name);
                        e = me.Expression;
                        Type convertedType;
                        me = ResourceBinder.StripTo<MemberExpression>(e, out convertedType);
                        if (convertedType != null)
                        {
                            propertyPath.Insert(0, UriHelper.GetTypeNameForUri(convertedType, context));
                        }
                    }
                    else
                    {
                        me = null;
                    }
                }

                if (propertyPath != null)
                {
                    instance = e;
                    return true;
                }

                return false;
            }

            // is constant
            internal static bool MatchConstant(Expression e, out ConstantExpression constExpr)
            {
                constExpr = e as ConstantExpression;
                return constExpr != null;
            }

            internal static bool MatchAnd(Expression e)
            {
                BinaryExpression be = e as BinaryExpression;
                return (be != null && (be.NodeType == ExpressionType.And || be.NodeType == ExpressionType.AndAlso));
            }

            /// <summary>
            /// Checks whether the specified member expression refers to a member
            /// that is a non-private property (readable and with getter and/or setter).
            /// </summary>
            /// <param name="e">Expression to check.</param>
            /// <param name="propInfo">Non-null property info when result is true.</param>
            /// <param name="target">The property access target.</param>
            /// <returns>Whether the member refers to a non-private, readable property.</returns>
            internal static bool MatchNonPrivateReadableProperty(Expression e, out PropertyInfo propInfo, out Expression target)
            {
                Debug.Assert(e != null, "e != null");

                propInfo = null;
                target = null;
                // must be member expression
                MemberExpression me = e as MemberExpression;
                if (me == null)
                {
                    return false;
                }

                // must be property access
                if (PlatformHelper.IsProperty(me.Member))
                {
                    PropertyInfo pi = (PropertyInfo)me.Member;
                    // must be readable and non-private
                    if (pi.CanRead && !TypeSystem.IsPrivate(pi))
                    {
                        propInfo = pi;
                        target = me.Expression;
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Checks whether the specified <paramref name="expression"/> is a member access to a key.
            /// </summary>
            /// <param name="expression">Expression to check.</param>
            /// <param name="property">If this is a key access, the property for the key.</param>
            /// <returns>true if <paramref name="expression"/> is a member access to a key; false otherwise.</returns>
            internal static bool MatchKeyProperty(Expression expression, out PropertyInfo property)
            {
                property = null;

                // make sure member is property, it is public and has a Getter.
                PropertyInfo pi;
                Expression boundTarget;
                if (!PatternRules.MatchNonPrivateReadableProperty(expression, out pi, out boundTarget))
                {
                    return false;
                }

                // DEVNOTE(pqian):
                // we must check whether the property access is:
                // 1. a Key property on the resource's type
                // 2. bound to the input ref expression
                // (binding to the root IRE means that the property access must be on the lambda's parameter
                // i.e., e => e.ID, rather than e.OtherProperty.ID), note that "OtherProperty" may be the same type as e.
                // Call Order: Do not short circuit the get key properties call
                // in V1 we will always call this for memberaccess expr, and thus always create a client type.
                // There are certain types that we don't support and this could be throwing.
#if PORTABLELIB
                Type resourceType = pi.DeclaringType;
#else
                Type resourceType = pi.ReflectedType;
#endif
                if ((ClientTypeUtil.GetKeyPropertiesOnType(resourceType) ?? ClientTypeUtil.EmptyPropertyInfoArray).Contains(pi, PropertyInfoEqualityComparer.Instance) && boundTarget is InputReferenceExpression)
                {
                    property = pi;
                    return true;
                }

                return false;
            }

            internal static bool MatchKeyComparison(Expression e, out PropertyInfo keyProperty, out ConstantExpression keyValue)
            {
                if (PatternRules.MatchBinaryEquality(e))
                {
                    BinaryExpression be = (BinaryExpression)e;
                    if ((PatternRules.MatchKeyProperty(be.Left, out keyProperty) && PatternRules.MatchConstant(be.Right, out keyValue)) ||
                        (PatternRules.MatchKeyProperty(be.Right, out keyProperty) && PatternRules.MatchConstant(be.Left, out keyValue)))
                    {
                        // if property is compared to null, expression is not key predicate comparison
                        return keyValue.Value != null;
                    }
                }

                keyProperty = null;
                keyValue = null;
                return false;
            }

            /// <summary>
            /// Checks whether the specified <paramref name="expression"/> matches
            /// a call to System.Object.ReferenceEquals.
            /// </summary>
            /// <param name="expression">Expression to check.</param>
            /// <returns>true if the expression matches; false otherwise.</returns>
            internal static bool MatchReferenceEquals(Expression expression)
            {
                Debug.Assert(expression != null, "expression != null");
                MethodCallExpression call = expression as MethodCallExpression;
                if (call == null)
                {
                    return false;
                }

                return call.Method == typeof(object).GetMethod("ReferenceEquals");
            }

            /// <summary>
            /// Checks whether the specifed <paramref name="expression"/> refers to a resource.
            /// </summary>
            /// <param name="expression">Expression to check.</param>
            /// <param name="resource">Resource expression if successful.</param>
            /// <returns>true if the expression is a resource expression; false otherwise.</returns>
            internal static bool MatchResource(Expression expression, out ResourceExpression resource)
            {
                resource = expression as ResourceExpression;
                return resource != null;
            }

            /// <summary>
            /// Checks whether the specified expression is a lambda with a two parameters
            /// (possibly quoted).
            /// </summary>
            /// <param name="expression">Expression to match.</param>
            /// <param name="lambda">If the expression matches, the lambda with the two parameters.</param>
            /// <returns>true if the expression is a lambda with two parameters.</returns>
            internal static bool MatchDoubleArgumentLambda(Expression expression, out LambdaExpression lambda)
            {
                return MatchNaryLambda(expression, 2, out lambda);
            }

            /// <summary>
            /// Checks whether the specified <paramref name="lambda"/> is a selector
            /// of the form (p) =&gt; p.
            /// </summary>
            /// <param name="lambda">Expression to check.</param>
            /// <returns>true if the lambda is an identity selector; false otherwise.</returns>
            internal static bool MatchIdentitySelector(LambdaExpression lambda)
            {
                Debug.Assert(lambda != null, "lambda != null");

                ParameterExpression parameter = lambda.Parameters[0];
                return parameter == StripTo<ParameterExpression>(lambda.Body);
            }

            /// <summary>
            /// Checks whether the specified expression is a lambda with a single parameter
            /// (possibly quoted).
            /// </summary>
            /// <param name="expression">Expression to match.</param>
            /// <param name="lambda">If the expression matches, the lambda with the single parameter.</param>
            /// <returns>true if the expression is a lambda with a single argument.</returns>
            internal static bool MatchSingleArgumentLambda(Expression expression, out LambdaExpression lambda)
            {
                return MatchNaryLambda(expression, 1, out lambda);
            }

            /// <summary>
            /// Checked whether the specified <paramref name="selector"/> has the
            /// form [input's transparent scope].[accessor].
            /// </summary>
            /// <param name="input">Input expression (source) for the selector.</param>
            /// <param name="selector">Selector lambda.</param>
            /// <param name="context">Data service context</param>
            /// <returns>true if the selector's body looks like [input's transparent scope].[accesor].</returns>
            internal static bool MatchTransparentIdentitySelector(Expression input, LambdaExpression selector, DataServiceContext context)
            {
                if (selector.Parameters.Count != 1)
                {
                    return false;
                }

                QueryableResourceExpression rse = input as QueryableResourceExpression;
                if (rse == null || rse.TransparentScope == null)
                {
                    return false;
                }

                Expression potentialRef = selector.Body;
                ParameterExpression expectedTarget = selector.Parameters[0];

                MemberExpression propertyMember;
                Expression paramRef;
                List<string> refPath;
                Version uriVersion;
                if (!MatchPropertyAccess(potentialRef, context, out propertyMember, out paramRef, out refPath, out uriVersion))
                {
                    return false;
                }

                Debug.Assert(refPath != null, "refPath != null -- otherwise MatchPropertyAccess should not have returned true");
                return paramRef == expectedTarget && refPath.Count == 1 && refPath[0] == rse.TransparentScope.Accessor;
            }

            internal static bool MatchIdentityProjectionResultSelector(Expression e)
            {
                LambdaExpression le = (LambdaExpression)e;
                return (le.Body == le.Parameters[1]);
            }

            /// <summary>
            /// Checks wheter the specified lambda matches a selector that yields
            /// a transparent identifier.
            /// </summary>
            /// <param name="input">
            /// The input expression for the lambda, used to set up the
            /// references from the transparent scope if one is produced.
            /// </param>
            /// <param name="resultSelector">Lambda expression to match.</param>
            /// <param name="transparentScope">
            /// After invocation, information on the accessors if the result
            /// is true; null otherwise.
            /// </param>
            /// <returns>
            /// true if <paramref name="input"/> is a selector for a transparent
            /// identifier; false otherwise.
            /// </returns>
            /// <remarks>
            /// Note that C# and VB.NET have different patterns for accumulating
            /// parameters.
            ///
            /// C# uses a two-member anonymous type with "everything so far"
            /// plus the newly introduced range variable.
            ///
            /// VB.NET uses an n-member anonymous type by pulling range variables
            /// from a previous anonymous type (or the first range variable),
            /// plus the newly introduced range variable.
            ///
            /// For additional background, see:
            /// Transparent Identifiers - http://blogs.msdn.com/wesdyer/archive/2006/12/22/transparent-identifiers.aspx
            /// http://msdn.microsoft.com/en-us/library/bb308966.aspx
            /// In particular:
            /// - 26.7.1.4 From, let, where, join and orderby clauses
            /// - 26.7.1.7 Transparent identifiers
            ///
            /// <paramref name="input"/> is the expression that represents the
            /// navigation resulting from the collector selector in the
            /// SelectMany() call under analysis.
            /// </remarks>
            internal static bool MatchTransparentScopeSelector(QueryableResourceExpression input, LambdaExpression resultSelector, out QueryableResourceExpression.TransparentAccessors transparentScope)
            {
                transparentScope = null;

                // Constructing transparent identifiers must be a simple instantiation.
                if (resultSelector.Body.NodeType != ExpressionType.New)
                {
                    return false;
                }

                // Less than two arguments implies there's no new range variable introduced.
                NewExpression ne = (NewExpression)resultSelector.Body;
                if (ne.Arguments.Count < 2)
                {
                    return false;
                }

                // Transparent identifier must not be part of hierarchies.
                if (ne.Type.GetBaseType() != typeof(object))
                {
                    return false;
                }

                // Transparent identifiers have a public property per constructor,
                // matching the parameter name.
                ParameterInfo[] constructorParams = ne.Constructor.GetParameters();
                if (ne.Members.Count != constructorParams.Length)
                {
                    return false;
                }

                // Every argument to the constructor should be a lambdaparameter or
                // a member access off one. The introduced range variable is always
                // a standalone parameter (note that for the first transparent
                // identifier, both are; we pick it by convention in that case).
                QueryableResourceExpression inputSource = input.Source as QueryableResourceExpression;
                int introducedMemberIndex = -1;
                ParameterExpression collectorSourceParameter = resultSelector.Parameters[0];
                ParameterExpression introducedRangeParameter = resultSelector.Parameters[1];
                MemberInfo[] memberProperties = new MemberInfo[ne.Members.Count];
                IEnumerable<PropertyInfo> properties = ne.Type.GetPublicProperties(true /*instanceOnly*/);
                Dictionary<string, Expression> sourceAccessors = new Dictionary<string, Expression>(constructorParams.Length - 1, StringComparer.Ordinal);
                for (int i = 0; i < ne.Arguments.Count; i++)
                {
                    Expression argument = ne.Arguments[i];
                    MemberInfo member = ne.Members[i];

                    if (!ExpressionIsSimpleAccess(argument, resultSelector.Parameters))
                    {
                        return false;
                    }

                    // Transparent identifiers have a property that matches the parameter
                    // name. The Members collection contains the get_Foo methods.
                    if (PlatformHelper.IsMethod(member))
                    {
                        member = properties.Where(property => PlatformHelper.AreMembersEqual(member, property.GetGetMethod())).FirstOrDefault();
                        if (member == null)
                        {
                            return false;
                        }
                    }

                    if (member.Name != constructorParams[i].Name)
                    {
                        return false;
                    }

                    memberProperties[i] = member;

                    ParameterExpression argumentAsParameter = StripTo<ParameterExpression>(argument);
                    if (introducedRangeParameter == argumentAsParameter)
                    {
                        if (introducedMemberIndex != -1)
                        {
                            return false;
                        }

                        introducedMemberIndex = i;
                    }
                    else if (collectorSourceParameter == argumentAsParameter)
                    {
                        sourceAccessors[member.Name] = inputSource.CreateReference();
                    }
                    else
                    {
                        List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
                        InputBinder.Bind(argument, inputSource, resultSelector.Parameters[0], referencedInputs);
                        if (referencedInputs.Count != 1)
                        {
                            return false;
                        }

                        sourceAccessors[member.Name] = referencedInputs[0].CreateReference();
                    }
                }

                // Transparent identifers should add at least one new range variable.
                if (introducedMemberIndex == -1)
                {
                    return false;
                }

                string resultAccessor = memberProperties[introducedMemberIndex].Name;
                transparentScope = new QueryableResourceExpression.TransparentAccessors(resultAccessor, sourceAccessors);

                return true;
            }

            /// <summary>
            /// Checks whether the specified <paramref name="input"/> is a member access
            /// that references <paramref name="potentialPropertyRef"/>.
            /// </summary>
            /// <param name="input">Expression to check.</param>
            /// <param name="potentialPropertyRef">InputReferenceExpression to consider as source.</param>
            /// <param name="context">Data service context instance.</param>
            /// <param name="setNavigationMember">Navigation member, equivalent to <paramref name="potentialPropertyRef"/> without unnecessary casts.</param>
            /// <returns>
            /// true if <paramref name="input"/> is a property collection that originates in
            /// <paramref name="potentialPropertyRef"/>; false otherwise.
            /// </returns>
            internal static bool MatchPropertyProjectionRelatedSet(ResourceExpression input, Expression potentialPropertyRef, DataServiceContext context, out MemberExpression setNavigationMember)
            {
                return MatchPropertyProjection(input, potentialPropertyRef, true, context, out setNavigationMember);
            }

            /// <summary>
            /// Checks whether the specified <paramref name="input"/> is a member access
            /// that references <paramref name="potentialPropertyRef"/>.
            /// </summary>
            /// <param name="input">Expression to check.</param>
            /// <param name="potentialPropertyRef">InputReferenceExpression to consider as source.</param>
            /// <param name="context">Data service context instance</param>
            /// <param name="propertyMember">Member expression, equivalent to <paramref name="potentialPropertyRef"/> without unnecessary casts.</param>
            /// <returns>
            /// true if <paramref name="input"/> is a scalar property or singleton navigation property that originates in
            /// <paramref name="potentialPropertyRef"/>; false otherwise.
            /// </returns>
            internal static bool MatchPropertyProjectionSingleton(ResourceExpression input, Expression potentialPropertyRef, DataServiceContext context, out MemberExpression propertyMember)
            {
                return MatchPropertyProjection(input, potentialPropertyRef, false, context, out propertyMember);
            }

            /// <summary>
            /// Checks whether the specified <paramref name="input"/> is a member access with the specified cardinality (per <paramref name="matchSetNavigationProperty"/>)
            /// that references <paramref name="potentialPropertyRef"/>.
            /// </summary>
            /// <param name="input">Expression to check.</param>
            /// <param name="potentialPropertyRef">InputReferenceExpression to consider as source.</param>
            /// <param name="matchSetNavigationProperty">
            /// Whether the match should be for a set of related entities or a singleton property.
            /// Singleton properties can be scalar types (including collection and other enumerable types like byte[]), complex types, or singleton navigation properties.
            /// Related set properties are only navigation properties.
            /// </param>
            /// <param name="context">Data service context instance.</param>
            /// <param name="propertyMember">Member expression for accessing the property, equivalent to <paramref name="potentialPropertyRef"/> without unnecessary casts.</param>
            /// <returns>
            /// true if <paramref name="input"/> is a property that originates in <paramref name="potentialPropertyRef"/>
            /// and is the expected cardinality (per <paramref name="matchSetNavigationProperty"/>); false otherwise.
            /// </returns>
            private static bool MatchPropertyProjection(ResourceExpression input, Expression potentialPropertyRef, bool matchSetNavigationProperty, DataServiceContext context, out MemberExpression propertyMember)
            {
                Expression foundInstance;
                List<string> propertyPath;
                Version uriVersion;
                if (MatchPropertyAccess(potentialPropertyRef, context, out propertyMember, out foundInstance, out propertyPath, out uriVersion))
                {
                    UnaryExpression unaryInstance = foundInstance as UnaryExpression;
                    if (unaryInstance != null && unaryInstance.NodeType == ExpressionType.TypeAs)
                    {
                        foundInstance = unaryInstance.Operand;
                    }

                    if (foundInstance == input.CreateReference())
                    {
                        if (PatternRules.MatchSetNavigationProperty(propertyMember, context.Model) == matchSetNavigationProperty)
                        {
                            return true;
                        }
                    }
                }

                propertyMember = null;
                return false;
            }

            internal static bool MatchMemberInitExpressionWithDefaultConstructor(Expression source, LambdaExpression e)
            {
                MemberInitExpression mie = StripTo<MemberInitExpression>(e.Body);
                ResourceExpression resource;
                return MatchResource(source, out resource) && (mie != null) && (mie.NewExpression.Arguments.Count == 0);
            }

            internal static bool MatchNewExpression(Expression source, LambdaExpression e)
            {
                ResourceExpression resource;
                return MatchResource(source, out resource) && (e.Body is NewExpression);
            }

            /// <summary>
            /// Checks whether <paramref name="expression"/> is a logical negation
            /// expression.
            /// </summary>
            /// <param name="expression">Expression to check.</param>
            /// <returns>true if expression is a Not expression; false otherwise.</returns>
            internal static bool MatchNot(Expression expression)
            {
                Debug.Assert(expression != null, "expression != null");
                return expression.NodeType == ExpressionType.Not;
            }

            /// <summary>Checks whether the type of the specified expression could represent a set of related resources.</summary>
            /// <param name="e">Expression to check.</param>
            /// <param name="model">The model that the client uses.</param>
            /// <returns>true if the type of the expression could represent a set of related resources; false otherwise.</returns>
            internal static bool MatchSetNavigationProperty(Expression e, ClientEdmModel model)
            {
                // Even though collection, byte[], and char[] are enumerable types, these properties are treated as scalars
                // for the purposes of projection and should not be considered as set navigation properties
                return (TypeSystem.FindIEnumerable(e.Type) != null) &&
                    e.Type != typeof(char[]) &&
                    e.Type != typeof(byte[]) &&
                    !WebUtil.IsCLRTypeCollection(e.Type, model);
            }

            /// <summary>
            /// Checks whether <paramref name="entityInScope"/> is a conditional expression
            /// that checks whether a navigation property (reference or collection) is
            /// null before proceeding.
            /// </summary>
            /// <param name="entityInScope">Entity in scope to be checked.</param>
            /// <param name="conditional">Expression to check.</param>
            /// <returns>Check results.</returns>
            internal static MatchNullCheckResult MatchNullCheck(Expression entityInScope, ConditionalExpression conditional)
            {
                Debug.Assert(conditional != null, "conditional != null");

                MatchNullCheckResult result = new MatchNullCheckResult();
                MatchEqualityCheckResult equalityCheck = MatchEquality(conditional.Test);
                if (!equalityCheck.Match)
                {
                    return result;
                }

                Expression assignedCandidate;
                if (equalityCheck.EqualityYieldsTrue)
                {
                    // Pattern: memberCandidate EQ null ? null : memberCandidate.Something
                    if (!MatchNullConstant(conditional.IfTrue))
                    {
                        return result;
                    }

                    assignedCandidate = conditional.IfFalse;
                }
                else
                {
                    // Pattern: memberCandidate NEQ null ? memberCandidate.Something : null
                    if (!MatchNullConstant(conditional.IfFalse))
                    {
                        return result;
                    }

                    assignedCandidate = conditional.IfTrue;
                }

                // Pattern can be one memberCandidate OP null or null OP memberCandidate.
                Expression memberCandidate;
                if (MatchNullConstant(equalityCheck.TestLeft))
                {
                    memberCandidate = equalityCheck.TestRight;
                }
                else if (MatchNullConstant(equalityCheck.TestRight))
                {
                    memberCandidate = equalityCheck.TestLeft;
                }
                else
                {
                    return result;
                }

                Debug.Assert(assignedCandidate != null, "assignedCandidate != null");
                Debug.Assert(memberCandidate != null, "memberCandidate != null");

                // Verify that the member expression is a prefix path of the assigned expressions.
                MemberAssignmentAnalysis assignedAnalysis = MemberAssignmentAnalysis.Analyze(entityInScope, assignedCandidate);
                if (assignedAnalysis.MultiplePathsFound)
                {
                    return result;
                }

                MemberAssignmentAnalysis memberAnalysis = MemberAssignmentAnalysis.Analyze(entityInScope, memberCandidate);
                if (memberAnalysis.MultiplePathsFound)
                {
                    return result;
                }

                Expression[] assignedExpressions = assignedAnalysis.GetExpressionsToTargetEntity();
                Expression[] memberExpressions = memberAnalysis.GetExpressionsToTargetEntity();
                if (memberExpressions.Length > assignedExpressions.Length)
                {
                    return result;
                }

                // The access form we're interested in is [param].member0.member1...
                for (int i = 0; i < memberExpressions.Length; i++)
                {
                    Expression assigned = assignedExpressions[i];
                    Expression member = memberExpressions[i];
                    if (assigned == member)
                    {
                        continue;
                    }

                    if (assigned.NodeType != member.NodeType || assigned.NodeType != ExpressionType.MemberAccess)
                    {
                        return result;
                    }

                    if (((MemberExpression)assigned).Member != ((MemberExpression)member).Member)
                    {
                        return result;
                    }
                }

                result.AssignExpression = assignedCandidate;
                result.Match = true;
                result.TestToNullExpression = memberCandidate;
                return result;
            }

            /// <summary>Checks whether the specified <paramref name="expression"/> is a null constant.</summary>
            /// <param name="expression">Expression to check.</param>
            /// <returns>true if <paramref name="expression"/> is a constant null value; false otherwise.</returns>
            internal static bool MatchNullConstant(Expression expression)
            {
                Debug.Assert(expression != null, "expression != null");
                ConstantExpression constant = expression as ConstantExpression;
                if (constant != null && constant.Value == null)
                {
                    return true;
                }

                return false;
            }

            internal static bool MatchBinaryExpression(Expression e)
            {
                return (e is BinaryExpression);
            }

            internal static bool MatchBinaryEquality(Expression e)
            {
                return (PatternRules.MatchBinaryExpression(e) && ((BinaryExpression)e).NodeType == ExpressionType.Equal);
            }

            internal static bool MatchStringAddition(Expression e)
            {
                if (e.NodeType == ExpressionType.Add)
                {
                    BinaryExpression be = e as BinaryExpression;
                    return be != null &&
                        be.Left.Type == typeof(string) &&
                        be.Right.Type == typeof(string);
                }
                return false;
            }

            /// <summary>
            /// Checks whether <paramref name="nex"/> is a "new DataServiceCollection of T".
            /// </summary>
            /// <param name="nex">The expression to match</param>
            /// <returns>true if the expression matches the "new DataServiceCollection of T" or false otherwise.</returns>
            internal static bool MatchNewDataServiceCollectionOfT(NewExpression nex)
            {
                return nex.Type.IsGenericType() && WebUtil.IsDataServiceCollectionType(nex.Type.GetGenericTypeDefinition());
            }

            /// <summary>
            /// Checks whether <paramref name="nex"/> is a "new ICollection of T".
            /// </summary>
            /// <param name="nex">The expression to match</param>
            /// <returns></returns>
            internal static bool MatchNewCollectionOfT(NewExpression nex)
            {
                Type type = nex.Type;
                return type.GetInterfaces().Any(t => t.GetGenericTypeDefinition() == typeof(ICollection<>));
            }

            /// <summary>
            /// Checks whether <paramref name="expression"/> is a check for
            /// equality on two expressions.
            /// </summary>
            /// <param name="expression">Expression to match.</param>
            /// <returns>
            /// A structure describing whether the expression is a match,
            /// whether it yields true on equality (ie, '==' as opposed to '!='),
            /// and the compared expressions.
            /// </returns>
            /// <remarks>
            /// This pattern recognizes the following:
            /// - Calls to object.ReferenceEquals
            /// - Equality checks (ExpressionNodeType.Equals and ExpressionNodeType.NotEquals)
            /// - Negation (ExpressionNodeType.Not)
            /// </remarks>
            internal static MatchEqualityCheckResult MatchEquality(Expression expression)
            {
                Debug.Assert(expression != null, "expression != null");

                // Before starting the pattern match, assume that we will not
                // find one, and if we do, that it's a simple '==' check. The
                // implementation needs to update these values as it traverses
                // down the tree for nesting in expression such as !(a==b).
                MatchEqualityCheckResult result = new MatchEqualityCheckResult();
                result.Match = false;
                result.EqualityYieldsTrue = true;

                while (true)
                {
                    if (MatchReferenceEquals(expression))
                    {
                        MethodCallExpression call = (MethodCallExpression)expression;
                        result.Match = true;
                        result.TestLeft = call.Arguments[0];
                        result.TestRight = call.Arguments[1];
                        break;
                    }
                    else if (MatchNot(expression))
                    {
                        result.EqualityYieldsTrue = !result.EqualityYieldsTrue;
                        expression = ((UnaryExpression)expression).Operand;
                    }
                    else
                    {
                        BinaryExpression test = expression as BinaryExpression;
                        if (test == null)
                        {
                            break;
                        }

                        if (test.NodeType == ExpressionType.NotEqual)
                        {
                            result.EqualityYieldsTrue = !result.EqualityYieldsTrue;
                        }
                        else if (test.NodeType != ExpressionType.Equal)
                        {
                            break;
                        }

                        result.TestLeft = test.Left;
                        result.TestRight = test.Right;
                        result.Match = true;
                        break;
                    }
                }

                return result;
            }

            /// <summary>
            /// Checks whether the <paramref name="argument"/> expression is a
            /// simple access (standalone or member-access'ed) on one of the
            /// parameter <paramref name="expressions"/>.
            /// </summary>
            /// <param name="argument">Argument to match.</param>
            /// <param name="expressions">Candidate parameters.</param>
            /// <returns>
            /// true if the argument is a parmater or a member from a
            /// parameter; false otherwise.
            /// </returns>
            private static bool ExpressionIsSimpleAccess(Expression argument, ReadOnlyCollection<ParameterExpression> expressions)
            {
                Debug.Assert(argument != null, "argument != null");
                Debug.Assert(expressions != null, "expressions != null");

                Expression source = argument;
                MemberExpression member;
                do
                {
                    member = source as MemberExpression;
                    if (member != null)
                    {
                        source = member.Expression;
                    }
                }
                while (member != null);

                ParameterExpression parameter = source as ParameterExpression;
                if (parameter == null)
                {
                    return false;
                }

                return expressions.Contains(parameter);
            }

            /// <summary>
            /// Checks whether the specified expression is a lambda with a parameterCount parameters
            /// (possibly quoted).
            /// </summary>
            /// <param name="expression">Expression to match.</param>
            /// <param name="parameterCount">Expected number of parametrs.</param>
            /// <param name="lambda">If the expression matches, the lambda with the two parameters.</param>
            /// <returns>true if the expression is a lambda with parameterCount parameters.</returns>
            private static bool MatchNaryLambda(Expression expression, int parameterCount, out LambdaExpression lambda)
            {
                lambda = null;

                LambdaExpression le = StripTo<LambdaExpression>(expression);
                if (le != null && le.Parameters.Count == parameterCount)
                {
                    lambda = le;
                }

                return lambda != null;
            }

            /// <summary>
            /// Use this class to represent the results of a match on an expression
            /// that does a null check before accessing a property.
            /// </summary>
            internal struct MatchNullCheckResult
            {
                /// <summary>Expression used to assign a value when the reference is not null.</summary>
                internal Expression AssignExpression;

                /// <summary>Whether the expression analyzed matches a null check pattern.</summary>
                internal bool Match;

                /// <summary>Expression being checked againt null.</summary>
                internal Expression TestToNullExpression;
            }

            /// <summary>
            /// Use this class to represent the results of a match on an expression
            /// that checks for equality .
            /// </summary>
            internal struct MatchEqualityCheckResult
            {
                /// <summary>Whether a positive equality yields 'true' (ie, is this '==' as opposed to '!=').</summary>
                internal bool EqualityYieldsTrue;

                /// <summary>Whether the expression analyzed matches an equality check pattern.</summary>
                internal bool Match;

                /// <summary>The left-hand side of the check.</summary>
                internal Expression TestLeft;

                /// <summary>The right-hand side of the check.</summary>
                internal Expression TestRight;
            }
        }

        internal static class ValidationRules
        {
            internal static void RequireCanNavigate(Expression e)
            {
                QueryableResourceExpression resourceExpression = e as QueryableResourceExpression;
                if (resourceExpression != null && resourceExpression.HasSequenceQueryOptions)
                {
                    throw new NotSupportedException(Strings.ALinq_QueryOptionsOnlyAllowedOnLeafNodes);
                }

                ResourceExpression resource;
                if (PatternRules.MatchResource(e, out resource) && resource.Projection != null)
                {
                    throw new NotSupportedException(Strings.ALinq_ProjectionOnlyAllowedOnLeafNodes);
                }
            }

            internal static void RequireCanProject(Expression e)
            {
                ResourceExpression re = (ResourceExpression)e;
                if (!PatternRules.MatchResource(e, out re))
                {
                    throw new NotSupportedException(Strings.ALinq_CanOnlyProjectTheLeaf);
                }

                if (re.Projection != null)
                {
                    throw new NotSupportedException(Strings.ALinq_ProjectionCanOnlyHaveOneProjection);
                }

                if (re.ExpandPaths.Count > 0)
                {
                    throw new NotSupportedException(Strings.ALinq_CannotProjectWithExplicitExpansion);
                }
            }

            internal static void RequireCanExpand(Expression e)
            {
                ResourceExpression re = (ResourceExpression)e;
                if (!PatternRules.MatchResource(e, out re))
                {
                    throw new NotSupportedException(Strings.ALinq_CantExpand);
                }

                if (re.Projection != null)
                {
                    throw new NotSupportedException(Strings.ALinq_CannotProjectWithExplicitExpansion);
                }
            }

            internal static void RequireCanAddCount(Expression e)
            {
                ResourceExpression re = (ResourceExpression)e;
                if (!PatternRules.MatchResource(e, out re))
                {
                    throw new NotSupportedException(Strings.ALinq_CannotAddCountOption);
                }

                // do we already have a count option?
                if (re.CountOption != CountOption.None)
                {
                    throw new NotSupportedException(Strings.ALinq_CannotAddCountOptionConflict);
                }
            }

            internal static void RequireCanAddCustomQueryOption(Expression e)
            {
                ResourceExpression re = (ResourceExpression)e;
                if (!PatternRules.MatchResource(e, out re))
                {
                    throw new NotSupportedException(Strings.ALinq_CantAddQueryOption);
                }
            }

            internal static void RequireNonSingleton(Expression e)
            {
                ResourceExpression re = e as ResourceExpression;
                if (re != null && re.IsSingleton)
                {
                    throw new NotSupportedException(Strings.ALinq_QueryOptionsOnlyAllowedOnSingletons);
                }
            }

            internal static void RequireLegalCustomQueryOption(Expression e, ResourceExpression target)
            {
                string name = ((string)(e as ConstantExpression).Value).Trim();

                if (name[0] == UriHelper.DOLLARSIGN)
                {
                    if (target.CustomQueryOptions.Any(c => (string)c.Key.Value == name))
                    {
                        // don't allow dups in Astoria $ namespace.
                        throw new NotSupportedException(Strings.ALinq_CantAddDuplicateQueryOption(name));
                    }

                    QueryableResourceExpression rse = target as QueryableResourceExpression;
                    if (rse != null)
                    {
                        switch (name.Substring(1))
                        {
                            case UriHelper.OPTIONFILTER:
                                if (rse.Filter != null)
                                {
                                    throw new NotSupportedException(Strings.ALinq_CantAddAstoriaQueryOption(name));
                                }
                                break;
                            case UriHelper.OPTIONORDERBY:
                                if (rse.OrderBy != null)
                                    throw new NotSupportedException(Strings.ALinq_CantAddAstoriaQueryOption(name));
                                break;
                            case UriHelper.OPTIONEXPAND:
                                // how did we get here?
                                Debug.Assert(false, "$expand should not walk this path");
                                break;
                            case UriHelper.OPTIONSKIP:
                                if (rse.Skip != null)
                                    throw new NotSupportedException(Strings.ALinq_CantAddAstoriaQueryOption(name));
                                break;
                            case UriHelper.OPTIONTOP:
                                if (rse.Take != null)
                                    throw new NotSupportedException(Strings.ALinq_CantAddAstoriaQueryOption(name));
                                break;
                            case UriHelper.OPTIONCOUNT:
                                // cannot add count if any counting already exists
                                if (rse.CountOption != CountOption.None)
                                    throw new NotSupportedException(Strings.ALinq_CantAddAstoriaQueryOption(name));
                                break;
                            case UriHelper.OPTIONSELECT:
                                if (rse.Projection != null)
                                {
                                    throw new NotSupportedException(Strings.ALinq_CantAddAstoriaQueryOption(name));
                                }
                                // DEVNOTE(pqian):
                                // while the normal projection analyzer does not allow expansions, we must allow it here
                                // since we do not automatically include the expansions in the URI. Users who wishes to
                                // project into navigation properties must manually expand those that are been selected.
                                break;
                            case UriHelper.OPTIONFORMAT:
                                ThrowNotSupportedExceptionForTheFormatOption();
                                break;
                            default:
                                throw new NotSupportedException(Strings.ALinq_CantAddQueryOptionStartingWithDollarSign(name));
                        }
                    }
                }
            }

            /// <summary>
            /// Throws the NotSupportedException for the $format query option.
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DataServiceContext", Justification = "The spelling is correct.")]
            private static void ThrowNotSupportedExceptionForTheFormatOption()
            {
                throw new NotSupportedException(Strings.ALinq_FormatQueryOptionNotSupported);
            }

            /// <summary>
            /// Detect and disallow member access for certain known types, in 'where' and 'orderby' requests.
            /// </summary>
            internal class WhereAndOrderByChecker : DataServiceALinqExpressionVisitor
            {
                private readonly SequenceMethod checkedMethod;
                private readonly ClientEdmModel model;

                internal WhereAndOrderByChecker(ClientEdmModel model, SequenceMethod checkedMethod)
                {
                    Debug.Assert(checkedMethod == SequenceMethod.Where || checkedMethod == SequenceMethod.OrderBy);
                    this.model = model;
                    this.checkedMethod = checkedMethod;
                }

                internal override Expression VisitMethodCall(MethodCallExpression mce)
                {
                    SequenceMethod sequenceMethod;
                    if (ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out sequenceMethod) &&
                        ReflectionUtil.IsAnyAllMethod(sequenceMethod))
                    {
                        if (this.checkedMethod == SequenceMethod.OrderBy)
                        {
                            throw new NotSupportedException(Strings.ALinq_AnyAllNotSupportedInOrderBy(mce.Method.Name));
                        }

                        Type filteredType = mce.Method.GetGenericArguments().SingleOrDefault();
                        if (!ClientTypeUtil.TypeOrElementTypeIsEntity(filteredType))
                        {
                            Expression source = mce.Arguments[0];
                            MemberExpression me = StripTo<MemberExpression>(source);
                            PropertyInfo pi;
                            Expression boundTarget;
                            if (me == null ||
                                !PatternRules.MatchNonPrivateReadableProperty(me, out pi, out boundTarget) ||
                                !WebUtil.IsCLRTypeCollection(pi.PropertyType, this.model))
                            {
                                throw new NotSupportedException(Strings.ALinq_InvalidSourceForAnyAll(mce.Method.Name));
                            }
                        }

                        // visit lambda for any/all
                        if (mce.Arguments.Count == 2)
                        {
                            base.Visit(mce.Arguments[1]);
                        }

                        return mce;
                    }

                    return base.VisitMethodCall(mce);
                }

                internal override Expression VisitMemberAccess(MemberExpression m)
                {
                    if (PlatformHelper.IsProperty(m.Member))
                    {
                        PropertyInfo pi = (PropertyInfo)m.Member;

                        // For member like "c.Trips.Count", when Count is visited, no future visit if declare type is a Collection
                        if (pi.Name.Equals(ReflectionUtil.COUNTPROPNAME))
                        {
                            MemberExpression me = StripTo<MemberExpression>(m.Expression);
                            if (me != null && !PrimitiveType.IsKnownNullableType(me.Type))
                            {
                                Type collectionType = ClientTypeUtil.GetImplementationType(me.Type, typeof(ICollection<>));
                                if (collectionType != null)
                                {
                                    return m;
                                }
                            }
                        }

                        // Continue the check if it is not a count segment
                        if (WebUtil.IsCLRTypeCollection(pi.PropertyType, this.model))
                        {
                            if (this.checkedMethod == SequenceMethod.Where)
                            {
                                throw new NotSupportedException(Strings.ALinq_CollectionPropertyNotSupportedInWhere(pi.Name));
                            }
                            else
                            {
                                throw new NotSupportedException(Strings.ALinq_CollectionPropertyNotSupportedInOrderBy(pi.Name));
                            }
                        }

                        if (typeof(DataServiceStreamLink).IsAssignableFrom(pi.PropertyType))
                        {
                            throw new NotSupportedException(Strings.ALinq_LinkPropertyNotSupportedInExpression(pi.Name));
                        }
                    }

                    return base.VisitMemberAccess(m);
                }
            }

            /// <summary>
            /// Detect and disallow member access for certain known types, in 'where' requests.
            /// </summary>
            internal static void CheckPredicate(Expression e, ClientEdmModel model)
            {
                WhereAndOrderByChecker predicateVisitor = new WhereAndOrderByChecker(model, SequenceMethod.Where);
                predicateVisitor.Visit(e);
            }

            /// <summary>
            /// Detect and disallow member access for certain known types, in 'orderby' requests.
            /// </summary>
            internal static void CheckOrderBy(Expression e, ClientEdmModel model)
            {
                WhereAndOrderByChecker orderByVisitor = new WhereAndOrderByChecker(model, SequenceMethod.OrderBy);
                orderByVisitor.Visit(e);
            }

            /// <summary>
            /// Used to identify and block navigation to members of collection property in select statement.
            /// e.g. from c in ctx.Customers select c.CollectionProperty.member
            /// </summary>
            internal static void DisallowMemberAccessInNavigation(Expression e, ClientEdmModel model)
            {
                MemberExpression me = StripTo<MemberExpression>(e);

                while (me != null)
                {
                    if (WebUtil.IsCLRTypeCollection(me.Expression.Type, model))
                    {
                        throw new NotSupportedException(Strings.ALinq_CollectionMemberAccessNotSupportedInNavigation(me.Member.Name));
                    }

                    me = StripTo<MemberExpression>(me.Expression);
                }
            }

            /// <summary>
            /// We do not support type identifier at the end of a path.
            /// For example, these would throw:
            /// select p as Employee
            /// select p.BestFriend as Employee
            /// Expand(p => p.BestFriend as Employee)
            /// </summary>
            /// <param name="exp">expression passed to validate</param>
            /// <param name="method">The context of where <paramref name="exp"/> is applied</param>
            internal static void DisallowExpressionEndWithTypeAs(Expression exp, string method)
            {
                Expression e = ResourceBinder.StripTo<UnaryExpression>(exp);
                if (e != null && e.NodeType == ExpressionType.TypeAs)
                {
                    throw new NotSupportedException(Strings.ALinq_ExpressionCannotEndWithTypeAs(exp.ToString(), method));
                }
            }

            /// <summary>
            /// Checks whether the specified <paramref name="input"/> is a valid expand path.
            /// </summary>
            /// <param name="input">The lambda expression for the expand path</param>
            /// <param name="context">Data service context instance.</param>
            /// <param name="expandPath">Expand path</param>
            /// <param name="uriVersion">Uri version</param>
            internal static void ValidateExpandPath(Expression input, DataServiceContext context, out string expandPath, out Version uriVersion)
            {
                expandPath = null;
                uriVersion = Util.ODataVersion4;
                LambdaExpression le;
                if (PatternRules.MatchSingleArgumentLambda(input, out le))
                {
                    Expression foundInstance;
                    PathSegmentToken propertyPath;
                    MemberExpression propertyMember;
                    MemberExpression pathExpression = ResourceBinder.StripTo<MemberExpression>(le.Body);

                    if (pathExpression != null && PatternRules.MatchPropertyAccess(pathExpression, context, out propertyMember, out foundInstance, out propertyPath, out uriVersion))
                    {
                        UnaryExpression unaryInstance = foundInstance as UnaryExpression;
                        if (unaryInstance != null && unaryInstance.NodeType == ExpressionType.TypeAs)
                        {
                            foundInstance = unaryInstance.Operand;
                        }
#if PORTABLELIB
                        Type resourceType = propertyMember.Member.DeclaringType;
#else
                        Type resourceType = propertyMember.Member.ReflectedType;
#endif
                        if (foundInstance == le.Parameters[0] && ClientTypeUtil.TypeOrElementTypeIsEntity(resourceType))
                        {
                            Debug.Assert(propertyPath != null, "propertyPath != null");
                            ExpandOnlyPathToStringVisitor expandPathVisitor = new ExpandOnlyPathToStringVisitor();
                            expandPath = propertyPath.Accept(expandPathVisitor);
                            return;
                        }
                    }

                }

                throw new NotSupportedException(Strings.ALinq_InvalidExpressionInNavigationPath(input));
            }
        }

        // TODO: By default, C#/Vb compilers uses declaring type for property expression when
        // we pass property name while creating the property expression. But its totally fine to use
        // property info reflected from any subtype while creating property expressions.
        // The problem is when one creates the property expression from a property info reflected from one
        // of the subtype, then we don't recognize the key properties and instead of generating a key predicate, we generate
        // a filter predicate. This limits a bunch of scenarios, since we can't navigate further once
        // we generate a filter predicate.
        // To fix this issue, we use a PropertyInfoEqualityComparer, which checks for the name and DeclaringType
        // of the property and if they are the same, then it considers them equal.

        /// <summary>
        /// Equality and comparison implementation for PropertyInfo.
        /// </summary>
        private sealed class PropertyInfoEqualityComparer : IEqualityComparer<PropertyInfo>
        {
            /// <summary>
            /// private constructor for the singleton pattern
            /// </summary>
            private PropertyInfoEqualityComparer() { }

            /// <summary>
            /// Static property which returns the single instance of the EqualityComparer
            /// </summary>
            internal static readonly PropertyInfoEqualityComparer Instance = new PropertyInfoEqualityComparer();

            #region IEqualityComparer<TypeUsage> Members

            /// <summary>
            /// Checks whether the given property info's refers to the same property or not.
            /// </summary>
            /// <param name="left">first property info</param>
            /// <param name="right">second property info</param>
            /// <returns>true if they refer to the same property, otherwise returns false.</returns>
            public bool Equals(PropertyInfo left, PropertyInfo right)
            {
                // Short circuit the comparison if we know the other reference is equivalent
                if (object.ReferenceEquals(left, right)) { return true; }

                // If either side is null, return false order (both can't be null because of
                // the previous check)
                if (null == left || null == right) { return false; }

                // If the declaring type and the name of the property are the same,
                // both the property infos refer to the same property.
                return object.ReferenceEquals(left.DeclaringType, right.DeclaringType) && left.Name.Equals(right.Name);
            }

            /// <summary>
            /// Computes the hashcode for the given property info
            /// </summary>
            /// <param name="obj">property info whose hash code needs to be computed.</param>
            /// <returns>the hashcode for the given property info.</returns>
            public int GetHashCode(PropertyInfo obj)
            {
                Debug.Assert(obj != null, "obj != null");
                return (null != obj) ? obj.GetHashCode() : 0;
            }

            #endregion
        }

        /// <summary>
        /// Use this visitor to detect whether an Expression is found in an
        /// Expression tree.
        /// </summary>
        private sealed class ExpressionPresenceVisitor : DataServiceALinqExpressionVisitor
        {
            #region Private fields

            /// <summary>Target expression being sought.</summary>
            private readonly Expression target;

            /// <summary>Whether the target has been found.</summary>
            private bool found;

            #endregion Private fields

            /// <summary>
            /// Initializes a new <see cref="ExpressionPresenceVisitor"/> that
            /// searches for the given <paramref name="target"/>.
            /// </summary>
            /// <param name="target">Target expression to look for.</param>
            private ExpressionPresenceVisitor(Expression target)
            {
                Debug.Assert(target != null, "target != null");
                this.target = target;
            }

            /// <summary>
            /// Checks whether the specified <paramref name="target"/> can
            /// be found in the given <paramref name="tree"/>.
            /// </summary>
            /// <param name="target">Expression sought.</param>
            /// <param name="tree">Expression tree to look into.</param>
            /// <returns>true if target is found at least once; false otherwise.</returns>
            internal static bool IsExpressionPresent(Expression target, Expression tree)
            {
                Debug.Assert(target != null, "target != null");
                Debug.Assert(tree != null, "tree != null");

                ExpressionPresenceVisitor visitor = new ExpressionPresenceVisitor(target);
                visitor.Visit(tree);
                return visitor.found;
            }

            /// <summary>Visits the specified expression.</summary>
            /// <param name="exp">Expression to visit.</param>
            /// <returns>The visited expression (<paramref name="exp"/>).</returns>
            internal override Expression Visit(Expression exp)
            {
                Expression result;

                // After finding the node, there is no need to visit any further.
                if (this.found || object.ReferenceEquals(this.target, exp))
                {
                    this.found = true;
                    result = exp;
                }
                else
                {
                    result = base.Visit(exp);
                }

                return result;
            }
        }
    }
}
