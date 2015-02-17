//---------------------------------------------------------------------
// <copyright file="LinqEvaluatingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Evaluates Linq-specific expression trees.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is temptorarily allowed until further refactoring of current design.")]
    public abstract class LinqEvaluatingVisitor : CommonExpressionEvaluatingVisitor, ILinqExpressionVisitor<QueryValue>
    {
        private Dictionary<string, QueryExpression> freeVariableAssignments;
        private Dictionary<LinqParameterExpression, QueryValue> lambdaParameterAssignments;

        /// <summary>
        /// Initializes a new instance of the LinqEvaluatingVisitor class.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="freeVariableAssignments">Free variable assignments.</param>
        protected LinqEvaluatingVisitor(IQueryDataSet dataSet, IDictionary<string, QueryExpression> freeVariableAssignments)
            : base(dataSet)
        {
            this.lambdaParameterAssignments = new Dictionary<LinqParameterExpression, QueryValue>();
            this.freeVariableAssignments = new Dictionary<string, QueryExpression>(freeVariableAssignments);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqAllExpression expression)
        {
            return this.EvaluateQueryMethodWithLambdaExpression<QueryScalarValue>(
                expression,
                null,
                (c, lambda) => c.All(lambda));
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqAnyExpression expression)
        {
            return this.EvaluateQueryMethodWithLambdaExpression<QueryScalarValue>(
                expression,
                c => c.Any(),
                (c, lambda) => c.Any(lambda));
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqAsEnumerableExpression expression)
        {
            var result = this.Evaluate(expression.Source);

            return result;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqBitwiseAndExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.BitwiseAnd(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqBitwiseOrExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.BitwiseOr(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqBuiltInFunctionCallExpression expression)
        {
            return this.EvaluateBuiltInFunction(expression.ExpressionType, expression.LinqBuiltInFunction.BuiltInFunction, expression.Arguments);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqCastExpression expression)
        {
            var source = this.EvaluateCollection(expression.Source);
            return source.Cast(expression.TypeToOperateAgainst);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqConcatExpression expression)
        {
            var outer = this.EvaluateCollection(expression.Outer);
            var inner = this.EvaluateCollection(expression.Inner);

            return outer.Concat(inner);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqContainsExpression expression)
        {
            var source = this.EvaluateCollection(expression.Source);
            var value = this.Evaluate(expression.Value);

            foreach (var sourceElement in source.Elements)
            {
                var sourceElementStructualValue = sourceElement as QueryStructuralValue;
                var sourceElementReferenceValue = sourceElement as QueryReferenceValue;
                var sourceElementPrimitiveValue = sourceElement as QueryScalarValue;

                QueryScalarValue result;
                if (sourceElementStructualValue != null)
                {
                    var structuralValue = (QueryStructuralValue)value;
                    result = sourceElementStructualValue.EqualTo(structuralValue);
                }
                else if (sourceElementReferenceValue != null)
                {
                    var referenceValue = (QueryReferenceValue)value;
                    result = sourceElementReferenceValue.EqualTo(referenceValue);
                }
                else if (sourceElementPrimitiveValue != null)
                {
                    var primitiveValue = (QueryScalarValue)value;
                    result = sourceElementPrimitiveValue.EqualTo(primitiveValue);
                    
                    // When running someCollection.Contains(null), simple equality comparison is not enough
                    // since the comparison in the evaluation strategy will always return Unknown due to the null
                    // value. In EF the null comparison for the purposes of Contains() will return true if the source
                    // collection has a null element.
                    if (sourceElementPrimitiveValue.IsNull && primitiveValue.IsNull)
                    {
                        result = result.Type.CreateValue(true);
                    }
                }
                else
                {
                    throw new TaupoNotSupportedException("Contains operation on type" + sourceElement.Type.ToString() + " is not supported.");
                }

                if ((bool?)result.Value == true)
                {
                    return result;
                }
            }

            return source.Type.EvaluationStrategy.BooleanType.CreateValue(false);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqCountExpression expression)
        {
            return this.EvaluateQueryMethodWithLambdaExpression<QueryScalarValue>(
                expression,
                c => c.Count(),
                (c, lambda) => c.Count(lambda));
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqDefaultIfEmptyExpression expression)
        {
            var source = this.EvaluateCollection(expression.Source);

            if (source.Elements.Count == 0)
            {
                var elementType = source.Type.ElementType;
                var queryClrType = elementType as IQueryClrType;
                var queryScalarType = elementType as QueryScalarType;

                QueryValue defaultValue;

                if (expression.DefaultValue == null)
                {
                    if (queryScalarType != null && queryClrType != null && typeof(ValueType).IsAssignableFrom(queryClrType.ClrType))
                    {
                        defaultValue = queryScalarType.CreateValue(Activator.CreateInstance(queryClrType.ClrType));
                    }
                    else
                    {
                        defaultValue = elementType.NullValue;
                    }
                }
                else
                {
                    defaultValue = this.Evaluate(expression.DefaultValue);
                }

                return source.Type.CreateCollectionWithValues(new QueryValue[] { defaultValue });
            }

            return source;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqDistinctExpression expression)
        {
            var source = this.EvaluateCollection(expression.Source);

            return source.Distinct();
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqExceptExpression expression)
        {
            var outer = this.EvaluateCollection(expression.Outer);
            var inner = this.EvaluateCollection(expression.Inner);

            return outer.Except(inner);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqExclusiveOrExpression expression)
        {
            QueryScalarValue left, right;

            this.EvaluateBinaryArguments(expression, out left, out right);
            return left.ExclusiveOr(right);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqFirstExpression expression)
        {
            return this.VisitFirstExpression(expression, false);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqFirstOrDefaultExpression expression)
        {
            return this.VisitFirstExpression(expression, true);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqFreeVariableExpression expression)
        {
            QueryExpression freeVariable;

            if (!this.freeVariableAssignments.TryGetValue(expression.Name, out freeVariable))
            {
                freeVariable = expression.Values.First();
            }

            return this.Evaluate(freeVariable);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqGroupByExpression expression)
        {
            var source = this.EvaluateCollection(expression.Source);

            Func<QueryCollectionValue, Func<QueryValue, QueryValue>, QueryValue> keySelectorEvaluator = (collection, keySelector) => collection.GroupBy(keySelector);
            var result = (QueryCollectionValue)keySelectorEvaluator(source, v => this.EvaluateLambda<QueryScalarValue>(expression.KeySelector, v));

            if (expression.ElementSelector != null)
            {
                result = this.RewriteGroupingElements(result, expression.ElementSelector);
            }

            if (expression.ResultSelector != null)
            {
                var rewrittenResultElements = result.Elements.Cast<QueryStructuralValue>().Select(e => this.EvaluateLambda<QueryValue>(expression.ResultSelector, e.GetValue("Key"), e.GetValue("Elements")));
                result = expression.ResultSelector.Body.ExpressionType.CreateCollectionType().CreateCollectionWithValues(rewrittenResultElements);
            }

            return result;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqGroupJoinExpression expression)
        {
            return this.EvaluateJoinExpression(expression, true);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqJoinExpression expression)
        {
            return this.EvaluateJoinExpression(expression, false);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqLambdaExpression expression)
        {
            throw new TaupoNotSupportedException("Lambdas cannot be evaluated directly, but only through query operators that use them.");
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqLengthPropertyExpression expression)
        {
            var instance = (QueryScalarValue)this.Evaluate(expression.Instance);
            var evaluationStrategy = instance.Type.EvaluationStrategy;

            // TODO: is this abstracted enough?
            return evaluationStrategy.EvaluateBuiltInFunction(evaluationStrategy.IntegerType, "Edm", "Length", instance);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqLongCountExpression expression)
        {
            return this.EvaluateQueryMethodWithLambdaExpression<QueryScalarValue>(
                expression,
                c => c.LongCount(),
                (c, lambda) => c.LongCount(lambda));
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqMaxExpression expression)
        {
            return this.EvaluateQueryMethodWithLambdaExpression<QueryScalarValue>(
            expression,
            c => c.Max(),
            (c, lambda) => c.Max(lambda));
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqMemberMethodExpression expression)
        {
            return this.EvaluateMemberMethod(expression.Source, expression.ExpressionType, expression.MemberMethod, expression.Arguments);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqMinExpression expression)
        {
            return this.EvaluateQueryMethodWithLambdaExpression<QueryScalarValue>(
            expression,
            c => c.Min(),
            (c, lambda) => c.Min(lambda));
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqNewArrayExpression expression)
        {
            List<QueryValue> queryValues = new List<QueryValue>();
            foreach (var childExpression in expression.Expressions)
            {
                queryValues.Add(this.Evaluate(childExpression));
            }

            return expression.ExpressionType.CreateCollectionType().CreateCollectionWithValues(queryValues);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqNewExpression expression)
        {
            QueryValue instance = ((QueryStructuralType)expression.ExpressionType).CreateNewInstance();
            for (int i = 0; i < expression.Members.Count; ++i)
            {
                ((QueryStructuralValue)instance).SetValue(expression.MemberNames[i], this.Evaluate(expression.Members[i]));
            }
         
            return instance;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqNewInstanceExpression expression)
        {
            if (expression.ExpressionType is QueryStructuralType)
            {
                QueryStructuralType queryType = (QueryStructuralType)expression.ExpressionType;
                QueryStructuralValue instance = queryType.CreateNewInstance();

                foreach (var property in queryType.Properties.Where(p => !(p.PropertyType is QueryScalarType)))
                {
                    instance.SetValue(property.Name, property.PropertyType.DefaultValue);
                }

                for (int i = 0; i < expression.Members.Count; ++i)
                {
                   instance.SetValue(expression.MemberNames[i], this.Evaluate(expression.Members[i]));
                }
                
                return instance;
            }
            else if (expression.ExpressionType is QueryCollectionType)
            {
                // for QueryCollectionTypes we only support constructor arguments, hence we will only be evaluating constructor arguments.
                QueryCollectionValue instance = ((QueryCollectionType)expression.ExpressionType).CreateCollectionWithValues(expression.ConstructorArguments.Select(arg => this.Evaluate(arg)));

                return instance;
            }
            else
            {
                var scalarType = expression.ExpressionType as QueryScalarType;
                ExceptionUtilities.CheckObjectNotNull(scalarType, "QueryType is not a supported type");
                ExceptionUtilities.Assert(expression.ConstructorArguments.Count == 1, "Cannot pass multiple arguments to PrimitiveType constructor");
                var constructorArgument = expression.ConstructorArguments.Select(this.Evaluate).Single();
                QueryScalarValue instance = scalarType.CreateValue(constructorArgument);

                return instance;
            }
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqOrderByExpression expression)
        {
            var keySelectors = expression.KeySelectors;
            var areDescending = expression.AreDescending;

            var source = this.Evaluate(expression.Source) as QueryCollectionValue;

            var ordering = new QueryOrdering();

            for (int i = 0; i < keySelectors.Count; i++)
            {
                Func<object, QueryScalarValue> selector = this.CreateKeySelector(keySelectors[i]);

                if (areDescending[i])
                {
                    ordering = ordering.Descending(selector);
                }
                else
                {
                    ordering = ordering.Ascending(selector);
                }
            }

            return source.OrderBy(ordering);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Visit(LinqParameterExpression expression)
        {
            return this.lambdaParameterAssignments[expression];
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqSelectExpression expression)
        {
            return this.EvaluateQueryMethodWithLambdaExpression<QueryValue>(
                expression,
                null,
                (c, l) => c.Select(l, (QueryCollectionType)expression.ExpressionType));
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqSelectManyExpression expression)
        {
            var source = this.EvaluateCollection(expression.Source);
            var collectionElementType = ((QueryCollectionType)expression.CollectionSelector.Body.ExpressionType).ElementType;
            var collections = source.Elements.Select(v => new { Element = v, Collection = (QueryCollectionValue)this.EvaluateLambda<QueryValue>(expression.CollectionSelector, v) });

            var flattenedStructure = new List<KeyValuePair<QueryValue, QueryValue>>();
            foreach (var collectionStructure in collections)
            {
                foreach (var innerCollectionElement in collectionStructure.Collection.Elements)
                {
                    flattenedStructure.Add(new KeyValuePair<QueryValue, QueryValue>(collectionStructure.Element, innerCollectionElement));
                }
            }

            if (expression.ResultSelector != null)
            {
                var results = flattenedStructure.Select(v => this.EvaluateLambda<QueryValue>(expression.ResultSelector, v.Key, v.Value));

                return expression.ResultSelector.Body.ExpressionType.CreateCollectionType().CreateCollectionWithValues(results);
            }
            else
            {
                return collectionElementType.CreateCollectionType().CreateCollectionWithValues(flattenedStructure.Select(s => s.Value));
            }
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqSingleExpression expression)
        {
            return this.VisitSingleExpression(expression, false);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqSingleOrDefaultExpression expression)
        {
            return this.VisitSingleExpression(expression, true);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqSkipExpression expression)
        {
            var source = this.EvaluateCollection(expression.Source);

            var skipCount = this.Evaluate(expression.SkipCount) as QueryScalarValue;
            if (skipCount == null)
            {
                throw new TaupoInvalidOperationException("Skip count expression must be a primitive.");
            }

            return source.Skip(skipCount);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqTakeExpression expression)
        {
            var source = this.EvaluateCollection(expression.Source);

            var takeCount = this.Evaluate(expression.TakeCount) as QueryScalarValue;
            if (takeCount == null)
            {
                throw new TaupoInvalidOperationException("Take count expression must be a primitive.");
            }

            return source.Take(takeCount);
        }

        /// <summary>
        /// Evalutes the specified expression
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression</returns>
        public QueryValue Visit(LinqUnionExpression expression)
        {
            var firstSource = this.EvaluateCollection(expression.FirstSource);
            var secondSource = this.EvaluateCollection(expression.SecondSource);

            return firstSource.Union(secondSource);
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public virtual QueryValue Visit(LinqWhereExpression expression)
        {
            return this.EvaluateQueryMethodWithLambdaExpression<QueryScalarValue>(
                expression,
                null,
                (c, l) => c.Where(l));
        }

        /// <summary>
        /// Evaluates a member method.
        /// </summary>
        /// <param name="sourceExpression">The query source expression</param>
        /// <param name="resultType">The method result type.</param>
        /// <param name="memberMethod">The member method name.</param>
        /// <param name="arguments">The arguments for the method call.</param>
        /// <returns>QueryValue which is the result of method evaluation.</returns>
        protected QueryValue EvaluateMemberMethod(QueryExpression sourceExpression, QueryType resultType, Function memberMethod, IEnumerable<QueryExpression> arguments)
        {
            var argumentValues = this.EvaluateArguments(arguments);
            var sourceValue = this.Evaluate(sourceExpression);
            var result = resultType.EvaluationStrategy.EvaluateMemberMethod(sourceValue, resultType, memberMethod.Name, argumentValues);
            return result;
        }

        private TResult EvaluateLambda<TResult>(LinqLambdaExpression lambda, params QueryValue[] currentParameterValues)
            where TResult : QueryValue
        {
            ExceptionUtilities.Assert(lambda.Parameters.Count == currentParameterValues.Length, "LambdaExpression parameter count must match length of currentParameterValues");

            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                LinqParameterExpression lambdaParameter = lambda.Parameters[i];
                ExceptionUtilities.Assert(!this.lambdaParameterAssignments.ContainsKey(lambdaParameter), "Attempt to recursively evaluate lambda.");

                this.lambdaParameterAssignments[lambdaParameter] = currentParameterValues[i];
            }

            try
            {
                return (TResult)this.Evaluate(lambda.Body);
            }
            finally
            {
                foreach (var p in lambda.Parameters)
                {
                    this.lambdaParameterAssignments.Remove(p);
                }
            }
        }

        private QueryValue EvaluateQueryMethodWithLambdaExpression<TResult>(
            LinqQueryMethodWithLambdaExpression expression,
            Func<QueryCollectionValue, QueryValue> nolambdaEvaluator,
            Func<QueryCollectionValue, Func<QueryValue, TResult>, QueryValue> lambdaEvaluator)
            where TResult : QueryValue
        {
            var source = this.EvaluateCollection(expression.Source);

            LinqLambdaExpression lambda = expression.Lambda;
            if (lambda == null)
            {
                ExceptionUtilities.Assert(nolambdaEvaluator != null, "noLambdaEvaluator must not be null.");
                return nolambdaEvaluator(source);
            }
            else
            {
                return lambdaEvaluator(source, v => this.EvaluateLambda<TResult>(lambda, v));
            }
        }

        private Func<object, QueryScalarValue> CreateKeySelector(LinqLambdaExpression keySelectorLambda)
        {
            return e => this.EvaluateLambda<QueryScalarValue>(keySelectorLambda, e as QueryValue);
        }

        private QueryCollectionValue RewriteGroupingElements(QueryCollectionValue groupings, LinqLambdaExpression elementSelectorLambda)
        {
            var groupingElementType = (QueryGroupingType)groupings.Type.ElementType;
            var rewrittenGroupingType = new QueryGroupingType(groupingElementType.Key.PropertyType, elementSelectorLambda.Body.ExpressionType, groupingElementType.EvaluationStrategy);

            var rewrittenGroupings = new List<QueryValue>();
            foreach (var grouping in groupings.Elements.Cast<QueryStructuralValue>())
            {
                QueryCollectionValue elements = grouping.GetCollectionValue("Elements");
                var rewrittenElements = elements.Select(e => this.EvaluateLambda<QueryValue>(elementSelectorLambda, e));
                var rewrittenGrouping = rewrittenGroupingType.CreateNewInstance();
                rewrittenGrouping.SetValue("Key", grouping.GetValue("Key"));
                rewrittenGrouping.SetValue("Elements", rewrittenElements);
                rewrittenGroupings.Add(rewrittenGrouping);
            }

            var result = rewrittenGroupingType.CreateCollectionType().CreateCollectionWithValues(rewrittenGroupings);

            return result;
        }

        private QueryValue EvaluateJoinExpression(LinqJoinExpressionBase expression, bool isGroupJoin)
        {
            var outer = this.EvaluateCollection(expression.Outer);
            var inner = this.EvaluateCollection(expression.Inner);

            var outerKeys = this.EvaluateKeysForJoin(outer, expression.OuterKeySelector);
            var innerKeys = this.EvaluateKeysForJoin(inner, expression.InnerKeySelector);

            // Dictionary: inner key -> a list of inner values with this key
            Dictionary<QueryScalarValue, IList<QueryValue>> innerKeyLookup = this.BuildInnerKeyLookup(inner, innerKeys);

            List<QueryValue> joinResultElements = new List<QueryValue>();
            for (int outerIndex = 0; outerIndex < outer.Elements.Count; outerIndex++)
            {
                ExceptionUtilities.Assert(outerKeys.Elements[outerIndex] is QueryScalarValue, "For now we only support join on primitive keys.");
                var outerKey = (QueryScalarValue)outerKeys.Elements[outerIndex];

                IList<QueryValue> innerMatches;
                if (!innerKeyLookup.TryGetValue(outerKey, out innerMatches))
                {
                    innerMatches = new List<QueryValue>();
                }

                if (isGroupJoin)
                {
                    QueryValue result = this.EvaluateLambda<QueryValue>(expression.ResultSelector, outer.Elements[outerIndex], inner.Type.CreateCollectionWithValues(innerMatches));
                    joinResultElements.Add(result);
                }
                else
                {
                    foreach (var innerValue in innerMatches)
                    {
                        QueryValue result = this.EvaluateLambda<QueryValue>(expression.ResultSelector, outer.Elements[outerIndex], innerValue);
                        joinResultElements.Add(result);
                    }
                }
            }

            var resultType = expression.ResultSelector.Body.ExpressionType;

            return new QueryCollectionValue(resultType.CreateCollectionType(), resultType.EvaluationStrategy, QueryError.GetErrorFromValues(joinResultElements), joinResultElements);
        }

        private QueryCollectionValue EvaluateKeysForJoin(QueryCollectionValue inputCollection, LinqLambdaExpression keySelector)
        {
            return inputCollection.Select(e => this.EvaluateLambda<QueryValue>(keySelector, e));
        }

        // Builds a lookup dictionary for inner value collection
        // Dictionary: inner key -> a list of inner values with this key
        private Dictionary<QueryScalarValue, IList<QueryValue>> BuildInnerKeyLookup(QueryCollectionValue inner, QueryCollectionValue innerKeys)
        {
            // Note: this dictionary uses a customized equality comparer
            // TODO: we should consider push this up into QueryValue.Equals()
            var innerKeyLookup = new Dictionary<QueryScalarValue, IList<QueryValue>>(new QueryScalarValueEqualityComparer());
            for (int innerIndex = 0; innerIndex < inner.Elements.Count; innerIndex++)
            {
                ExceptionUtilities.Assert(innerKeys.Elements[innerIndex] is QueryScalarValue, "For now we only support join on scalar keys.");
                var innerKey = (QueryScalarValue)innerKeys.Elements[innerIndex];

                IList<QueryValue> innerValues;
                if (!innerKeyLookup.TryGetValue(innerKey, out innerValues))
                {
                    innerValues = new List<QueryValue>();
                    innerKeyLookup.Add(innerKey, innerValues);
                }

                innerValues.Add(inner.Elements[innerIndex]);
            }

            return innerKeyLookup;
        }

        private QueryValue VisitFirstExpression(LinqQueryMethodWithLambdaExpression expression, bool isOrDefault)
        {
            var source = this.EvaluateCollection(expression.Source);

            IEnumerable<QueryValue> elements = source.Elements;
            if (expression.Lambda != null)
            {
                // Note: cannot directly use souce.Where(...) since underlying store semantics might change order!
                elements = source.Elements.Where(v =>
                {
                    QueryScalarValue predicate = this.EvaluateLambda<QueryScalarValue>(expression.Lambda, v);
                    return !predicate.IsNull && (bool)predicate.Value;
                }).ToArray();
            }

            if (elements.Count() == 0)
            {
                if (isOrDefault)
                {
                    return source.Type.ElementType.NullValue;
                }
                else
                {
                    var expectedException = new ExpectedExceptions(new ExpectedExceptionTypeMessageVerifier<InvalidOperationException>(null, "Sequence contains no elements"));
                    return source.Type.ElementType.CreateErrorValue(expectedException);
                }
            }
            else
            {
                return elements.First();
            }
        }

        private QueryValue VisitSingleExpression(LinqQueryMethodWithLambdaExpression expression, bool isOrDefault)
        {
            var source = this.EvaluateCollection(expression.Source);

            QueryCollectionValue collection = source;
            if (expression.Lambda != null)
            {
                collection = source.Where(v => this.EvaluateLambda<QueryScalarValue>(expression.Lambda, v));
            }

            if (collection.Elements.Count == 0)
            {
                if (isOrDefault)
                {
                    return collection.Type.ElementType.NullValue;
                }
                else
                {
                    var expectedException = new ExpectedExceptions(new ExpectedExceptionTypeMessageVerifier<InvalidOperationException>(null, "Sequence contains no elements"));
                    return source.Type.ElementType.CreateErrorValue(expectedException);
                }
            }
            else if (collection.Elements.Count == 1)
            {
                return collection.Elements[0];
            }
            else
            {
                var expectedException = new ExpectedExceptions(new ExpectedExceptionTypeMessageVerifier<InvalidOperationException>(null, "Sequence contains more than one element"));
                return collection.Type.ElementType.CreateErrorValue(expectedException);
            }
        }

        /// <summary>
        /// Equality comparer for query primitive values
        /// </summary>
        private class QueryScalarValueEqualityComparer : EqualityComparer<QueryScalarValue>
        {
            /// <summary>
            /// Determines whether two QueryScalarValues are equal
            /// </summary>
            /// <param name="x">The first query scalar value</param>
            /// <param name="y">The second query scalar value</param>
            /// <returns>True if equals, false otherwise</returns>
            public override bool Equals(QueryScalarValue x, QueryScalarValue y)
            {
                if (x.IsNull || y.IsNull)
                {
                    if (x.IsNull && y.IsNull)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var comparisonResult = x.EqualTo(y);
                    return (bool)comparisonResult.Value;
                }
            }

            /// <summary>
            /// Get the Hashcode of QueryPrimitive value
            /// </summary>
            /// <param name="queryScalarValue">The query scalar value</param>
            /// <returns>An int for hashcode</returns>
            public override int GetHashCode(QueryScalarValue queryScalarValue)
            {
                if (queryScalarValue.IsNull)
                {
                    return 0;
                }

                // for byte arrays we should not rely on hash code, since it's reference and not value based. 
                // by returning constant hash we always delegate to Equals method which handles these values correctly
                // for strings we need to handle them via Equals method, because of padding and other sql server specific issues around strings
                if (queryScalarValue.Value is byte[] || queryScalarValue.Value is string)
                {
                    return 123;
                }

                return queryScalarValue.Value.GetHashCode();
            }
        }
    }
}
