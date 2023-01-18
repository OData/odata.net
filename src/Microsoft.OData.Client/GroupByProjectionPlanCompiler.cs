//---------------------------------------------------------------------
// <copyright file="GroupByProjectionPlanCompiler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Use this class to create a <see cref="ProjectionPlan"/> for a given projection lambda.
    /// </summary>
    internal class GroupByProjectionPlanCompiler : ALinqExpressionVisitor
    {
        #region Private fields

        /// <summary>Creates dynamic methods that wrap calls to internal methods.</summary>
        private static readonly DynamicProxyMethodGenerator dynamicProxyMethodGenerator = new DynamicProxyMethodGenerator();

        /// <summary>Annotations being tracked on this tree.</summary>
        private readonly Dictionary<Expression, ExpressionAnnotation> annotations;

        /// <summary>Expression that refers to the materializer.</summary>
        private readonly ParameterExpression materializerExpression;

        /// <summary>Tracks rewrite-to-source rewrites introduced by expression normalizer.</summary>
        private readonly Dictionary<Expression, Expression> normalizerRewrites;

        /// <summary>Number to suffix to identifiers to help with debugging.</summary>
        private int identifierId;

        /// <summary>Path builder used to help with tracking state while compiling.</summary>
        private GroupByProjectionPathBuilder pathBuilder;

        /// <summary>Whether the top level projection has been found.</summary>
        private bool topLevelProjectionFound;

        /// <summary>Mapping of expressions in the GroupBy result selector to info required during projection plan compilation.</summary>
        private Dictionary<Expression, MappingInfo> resultSelectorMap;

        /// <summary>Mapping of member names in the GroupBy key selector to their respective expressions.</summary>
        private readonly Dictionary<string, Expression> keySelectorMap;

        #endregion Private fields

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="GroupByProjectionPlanCompiler"/> instance.
        /// </summary>
        /// <param name="normalizerRewrites">Rewrites introduced by normalizer.</param>
        /// <param name="keySelectorMap">Mapping of member names in the GroupBy key selector to their respective expressions.</param>
        private GroupByProjectionPlanCompiler(
            Dictionary<Expression, Expression> normalizerRewrites,
            Dictionary<string, Expression> keySelectorMap)
        {
            this.annotations = new Dictionary<Expression, ExpressionAnnotation>(ReferenceEqualityComparer<Expression>.Instance);
            this.materializerExpression = Expression.Parameter(typeof(object), "mat");
            this.normalizerRewrites = normalizerRewrites;
            this.pathBuilder = new GroupByProjectionPathBuilder();
            this.resultSelectorMap = new Dictionary<Expression, MappingInfo>(ReferenceEqualityComparer<Expression>.Instance);
            this.keySelectorMap = keySelectorMap;
        }

        #endregion Constructors

        #region Internal methods.

        /// <summary>Creates a projection plan from the specified <paramref name="projection"/>.</summary>
        /// <param name="projection">Projection expression.</param>
        /// <param name="normalizerRewrites">Tracks rewrite-to-source rewrites introduced by expression normalizer.</param>
        /// <param name="keySelectorMap">Mapping of member names in the GroupBy key selector to their respective expressions.</param>
        /// <returns>A new <see cref="ProjectionPlan"/> instance.</returns>
        internal static ProjectionPlan CompilePlan(
            LambdaExpression projection,
            Dictionary<Expression, Expression> normalizerRewrites,
            Dictionary<string, Expression> keySelectorMap)
        {
            Debug.Assert(projection != null, "projection != null");
            Debug.Assert(projection.Parameters.Count >= 1, "projection.Parameters.Count >= 1");
            Debug.Assert(
                projection.Body.NodeType == ExpressionType.Constant ||
                projection.Body.NodeType == ExpressionType.MemberInit ||
                projection.Body.NodeType == ExpressionType.MemberAccess ||
                projection.Body.NodeType == ExpressionType.Convert ||
                projection.Body.NodeType == ExpressionType.ConvertChecked ||
                projection.Body.NodeType == ExpressionType.New,
                "projection.Body.NodeType == Constant, MemberInit, MemberAccess, Convert(Checked) New");

            GroupByProjectionPlanCompiler rewriter = new GroupByProjectionPlanCompiler(normalizerRewrites, keySelectorMap);
            GroupByProjectionAnalyzer.Analyze(rewriter, projection);

            Expression plan = rewriter.Visit(projection);

            ProjectionPlan result = new ProjectionPlan();
            result.Plan = (Func<object, object, Type, object>)((LambdaExpression)plan).Compile();
            result.ProjectedType = projection.Body.Type;
#if DEBUG
            result.SourceProjection = projection;
            result.TargetProjection = plan;
#endif
            return result;
        }

        /// <summary>
        /// MemberExpression visit method
        /// </summary>
        /// <param name="m">The MemberExpression expression to visit</param>
        /// <returns>The visited MemberExpression expression </returns>
        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            Debug.Assert(m != null, "m != null");

            Expression baseSourceExpression = m.Expression;

            // If primitive or nullable primitive, allow member access... i.e. calling Value on nullable<int>
            if (PrimitiveType.IsKnownNullableType(baseSourceExpression.Type))
            {
                return base.VisitMemberAccess(m);
            }

            Expression baseTargetExpression = this.Visit(baseSourceExpression);
            ExpressionAnnotation annotation;
            if (this.annotations.TryGetValue(baseTargetExpression, out annotation))
            {
                return this.RebindMemberAccess(m, annotation);
            }

            if (this.resultSelectorMap.TryGetValue(m, out MappingInfo mappingInfo))
            {
                this.BindProjectedExpression(mappingInfo.GroupingExpression);

                return this.Visit(mappingInfo.GroupingExpression);
            }

            return Expression.MakeMemberAccess(baseTargetExpression, m.Member);
        }

        /// <summary>Parameter visit method.</summary>
        /// <param name="p">Parameter to visit.</param>
        /// <returns>Resulting expression.</returns>
        /// <remarks>
        /// The parameter may get rewritten as a materializing projection if
        /// it refers to an entity outside of member binding. In this case,
        /// it becomes a standalone tracked entity.
        /// </remarks>
        internal override Expression VisitParameter(ParameterExpression p)
        {
            Debug.Assert(p != null, "p != null");

            // If this parameter isn't interesting, we're not doing any rewrites.
            ExpressionAnnotation annotation;
            if (this.annotations.TryGetValue(p, out annotation))
            {
                return this.RebindParameter(p, annotation);
            }

            // Scenario: GroupBy(d1 => d1.Prop, (d2, d3) => new { ProductId = d2, ... })
            if (this.resultSelectorMap.TryGetValue(p, out MappingInfo mappingInfo))
            {
                Debug.Assert(mappingInfo.GroupingExpression != null, "mappingInfo.GroupingExpression != null");

                this.BindProjectedExpression(mappingInfo.GroupingExpression);

                return this.Visit(mappingInfo.GroupingExpression);
            }

            return base.VisitParameter(p);
        }

        /// <summary>Visits a method call expression.</summary>
        /// <param name="m">Expression to visit.</param>
        /// <returns>A (possibly rewritten) expression for <paramref name="m"/>.</returns>
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            Debug.Assert(m != null, "m != null");

            Expression original = this.GetExpressionBeforeNormalization(m);
            if (original != m)
            {
                return this.Visit(original);
            }

            SequenceMethod sequenceMethod;
            ReflectionUtil.TryIdentifySequenceMethod(m.Method, out sequenceMethod);

            switch (sequenceMethod)
            {
                case SequenceMethod.SumIntSelector:
                case SequenceMethod.SumDoubleSelector:
                case SequenceMethod.SumDecimalSelector:
                case SequenceMethod.SumLongSelector:
                case SequenceMethod.SumSingleSelector:
                case SequenceMethod.SumNullableIntSelector:
                case SequenceMethod.SumNullableDoubleSelector:
                case SequenceMethod.SumNullableDecimalSelector:
                case SequenceMethod.SumNullableLongSelector:
                case SequenceMethod.SumNullableSingleSelector:
                case SequenceMethod.AverageIntSelector:
                case SequenceMethod.AverageDoubleSelector:
                case SequenceMethod.AverageDecimalSelector:
                case SequenceMethod.AverageLongSelector:
                case SequenceMethod.AverageSingleSelector:
                case SequenceMethod.AverageNullableIntSelector:
                case SequenceMethod.AverageNullableDoubleSelector:
                case SequenceMethod.AverageNullableDecimalSelector:
                case SequenceMethod.AverageNullableLongSelector:
                case SequenceMethod.AverageNullableSingleSelector:
                case SequenceMethod.MinIntSelector:
                case SequenceMethod.MinDoubleSelector:
                case SequenceMethod.MinDecimalSelector:
                case SequenceMethod.MinLongSelector:
                case SequenceMethod.MinSingleSelector:
                case SequenceMethod.MinNullableIntSelector:
                case SequenceMethod.MinNullableDoubleSelector:
                case SequenceMethod.MinNullableDecimalSelector:
                case SequenceMethod.MinNullableLongSelector:
                case SequenceMethod.MinNullableSingleSelector:
                case SequenceMethod.MaxIntSelector:
                case SequenceMethod.MaxDoubleSelector:
                case SequenceMethod.MaxDecimalSelector:
                case SequenceMethod.MaxLongSelector:
                case SequenceMethod.MaxSingleSelector:
                case SequenceMethod.MaxNullableIntSelector:
                case SequenceMethod.MaxNullableDoubleSelector:
                case SequenceMethod.MaxNullableDecimalSelector:
                case SequenceMethod.MaxNullableLongSelector:
                case SequenceMethod.MaxNullableSingleSelector:
                case SequenceMethod.Count:
                case SequenceMethod.LongCount:
                case SequenceMethod.CountDistinctSelector:
                    return this.RebindMethodCallForAggregationMethod(m);
                default:
                    return base.VisitMethodCall(m);
            }
        }

        /// <summary>
        /// Visit
        /// </summary>
        /// <param name="exp">Expression to visit</param>
        /// <returns>an expression </returns>
        internal override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                // Parent expression may contain null children, in this case just return
                return exp;
            }

            return base.Visit(exp);
        }

        /// <summary>LambdaExpression visit method.</summary>
        /// <param name="lambda">The LambdaExpression to visit</param>
        /// <returns>The visited LambdaExpression</returns>
        internal override Expression VisitLambda(LambdaExpression lambda)
        {
            Debug.Assert(lambda != null, "lambda != null");

            Expression result;
            ParameterExpression lambdaParameter = lambda.Parameters[lambda.Parameters.Count - 1];
            if (!this.topLevelProjectionFound || lambda.Parameters.Count >= 1 && ClientTypeUtil.TypeOrElementTypeIsEntity(lambdaParameter.Type))
            {
                this.topLevelProjectionFound = true;

                ParameterExpression expectedTypeParameter = Expression.Parameter(typeof(Type), "type" + this.identifierId.ToString(CultureInfo.InvariantCulture));
                ParameterExpression entryParameter = Expression.Parameter(typeof(object), "entry" + this.identifierId.ToString(CultureInfo.InvariantCulture));
                this.identifierId++;

                this.pathBuilder.EnterLambdaScope(lambda, entryParameter, expectedTypeParameter);
                ProjectionPath parameterPath = new ProjectionPath(lambdaParameter, expectedTypeParameter, entryParameter);
                ProjectionPathSegment parameterSegment = new ProjectionPathSegment(parameterPath, null, null);
                parameterPath.Add(parameterSegment);
                this.annotations[lambdaParameter] = new ExpressionAnnotation { Segment = parameterSegment };

                Expression body = this.Visit(lambda.Body);

                // Value types must be boxed explicitly; the lambda initialization
                // won't do it for us (type-compatible types still work, so all
                // references will work fine with System.Object).
                if (body.Type.IsValueType())
                {
                    body = Expression.Convert(body, typeof(object));
                }

                result = Expression.Lambda<Func<object, object, Type, object>>(
                    body,
                    this.materializerExpression,
                    entryParameter,
                    expectedTypeParameter);

                this.pathBuilder.LeaveLambdaScope();
            }
            else
            {
                result = base.VisitLambda(lambda);
            }

            return result;
        }

        #endregion Internal methods.

        #region Private methods.

        /// <summary>Generates a call to a static method on AtomMaterializer.</summary>
        /// <param name="methodName">Name of method to invoke.</param>
        /// <param name="arguments">Arguments to pass to method.</param>
        /// <returns>The constructed expression.</returns>
        /// <remarks>
        /// There is no support for overload resolution - method names in AtomMaterializer
        /// must be unique.
        /// </remarks>
        private static Expression CallMaterializer(string methodName, params Expression[] arguments)
        {
            return CallMaterializerWithType(methodName, null, arguments);
        }

        /// <summary>Generates a call to a static method on AtomMaterializer.</summary>
        /// <param name="methodName">Name of method to invoke.</param>
        /// <param name="typeArguments">Type arguments for method (possibly null).</param>
        /// <param name="arguments">Arguments to pass to method.</param>
        /// <returns>The constructed expression.</returns>
        /// <remarks>
        /// There is no support for overload resolution - method names in AtomMaterializer
        /// must be unique.
        /// </remarks>
        private static Expression CallMaterializerWithType(string methodName, Type[] typeArguments, params Expression[] arguments)
        {
            Debug.Assert(methodName != null, "methodName != null");
            Debug.Assert(arguments != null, "arguments != null");

            MethodInfo method = typeof(ODataEntityMaterializerInvoker).GetMethod(methodName, false /*isPublic*/, true /*isStatic*/);
            Debug.Assert(method != null, "method != null - found " + methodName);
            if (typeArguments != null)
            {
                method = method.MakeGenericMethod(typeArguments);
            }

            return dynamicProxyMethodGenerator.GetCallWrapper(method, arguments);
        }

        /// <summary>Creates an expression that calls ProjectionValueForPath.</summary>
        /// <param name="entry">Expression for root entry for paths.</param>
        /// <param name="entryType">Expression for expected type for entry.</param>
        /// <param name="path">Path to pull value from.</param>
        /// <returns>A new expression with the call instance.</returns>
        private Expression CallValueForPath(Expression entry, Expression entryType, ProjectionPath path)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(path != null, "path != null");

            Expression result = CallMaterializer(
                nameof(ODataEntityMaterializerInvoker.ProjectionValueForPath),
                this.materializerExpression,
                entry, 
                entryType,
                Expression.Constant(path, typeof(object)));
            this.annotations.Add(result, new ExpressionAnnotation() { Segment = path[path.Count - 1] });
            return result;
        }

        /// <summary>Creates an expression that calls ProjectionValueForPath.</summary>
        /// <param name="entry">Expression for root entry for paths.</param>
        /// <param name="entryType">Expression for expected type for entry.</param>
        /// <param name="path">Path to pull value from.</param>
        /// <param name="type">Path to convert result for.</param>
        /// <returns>A new expression with the call instance.</returns>
        private Expression CallValueForPathWithType(Expression entry, Expression entryType, ProjectionPath path, Type type)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(path != null, "path != null");

            Expression value = this.CallValueForPath(entry, entryType, path);
            Expression result = Expression.Convert(value, type);
            this.annotations.Add(result, new ExpressionAnnotation() { Segment = path[path.Count - 1] });
            return result;
        }

        /// <summary>Gets an expression before its rewrite.</summary>
        /// <param name="expression">Expression to check.</param>
        /// <returns>The expression before normalization.</returns>
        private Expression GetExpressionBeforeNormalization(Expression expression)
        {
            Debug.Assert(expression != null, "expression != null");
            if (this.normalizerRewrites != null)
            {
                Expression original;
                if (this.normalizerRewrites.TryGetValue(expression, out original))
                {
                    expression = original;
                }
            }

            return expression;
        }

        /// <summary>Rebinds the specified parameter expression as a path-based access.</summary>
        /// <param name="expression">Expression to rebind.</param>
        /// <param name='annotation'>Annotation for the expression to rebind.</param>
        /// <returns>The rebound expression.</returns>
        private Expression RebindParameter(Expression expression, ExpressionAnnotation annotation)
        {
            Debug.Assert(expression != null, "expression != null");
            Debug.Assert(annotation != null, "annotation != null");

            Expression result;
            result = this.CallValueForPathWithType(
                annotation.Segment.StartPath.RootEntry,
                annotation.Segment.StartPath.ExpectedRootType,
                annotation.Segment.StartPath,
                expression.Type);

            // Refresh the annotation so the next one that comes along
            // doesn't start off with an already-written path.
            ProjectionPath parameterPath = new ProjectionPath(
                annotation.Segment.StartPath.Root,
                annotation.Segment.StartPath.ExpectedRootType,
                annotation.Segment.StartPath.RootEntry);
            ProjectionPathSegment parameterSegment = new ProjectionPathSegment(parameterPath, null, null);
            parameterPath.Add(parameterSegment);
            this.annotations[expression] = new ExpressionAnnotation() { Segment = parameterSegment };

            return result;
        }

        /// <summary>Rebinds the specified member access expression into a path-based value retrieval method call.</summary>
        /// <param name='m'>Member expression.</param>
        /// <param name='baseAnnotation'>Annotation for the base portion of the expression.</param>
        /// <returns>A rebound expression.</returns>
        private Expression RebindMemberAccess(MemberExpression m, ExpressionAnnotation baseAnnotation)
        {
            Debug.Assert(m != null, "m != null");
            Debug.Assert(baseAnnotation != null, "baseAnnotation != null");

            // This actually modifies the path for the underlying
            // segments, but that shouldn't be a problem. Actually
            // we should be able to remove it from the dictionary.
            // There should be no aliasing problems, because
            // annotations always come from target expression
            // that are generated anew (except parameters,
            // but those)
            ProjectionPathSegment memberSegment = new ProjectionPathSegment(baseAnnotation.Segment.StartPath, m);
            baseAnnotation.Segment.StartPath.Add(memberSegment);
            return this.CallValueForPathWithType(
                baseAnnotation.Segment.StartPath.RootEntry,
                baseAnnotation.Segment.StartPath.ExpectedRootType,
                baseAnnotation.Segment.StartPath,
                m.Type);
        }

        private void BindProjectedExpression(Expression expr)
        {
            ParameterExpression paramExpr;

            switch (expr.NodeType)
            {
                case ExpressionType.Parameter:
                    paramExpr = expr as ParameterExpression;
                    break;
                case ExpressionType.MemberAccess:
                    MemberExpression memberExpr = expr as MemberExpression;
                    while (memberExpr.Expression is MemberExpression)
                    {
                        memberExpr = memberExpr.Expression as MemberExpression;
                    }

                    paramExpr = memberExpr.Expression as ParameterExpression;
                    break;
                case ExpressionType.Constant:
                    paramExpr = null;
                    break;
                default:
                    throw Error.NotSupported();
            }

            if (paramExpr != null && !this.annotations.TryGetValue(paramExpr, out _))
            {
                ProjectionPath parameterPath = new ProjectionPath(paramExpr, this.pathBuilder.ExpectedParamTypeInScope, this.pathBuilder.ParameterEntryInScope);
                ProjectionPathSegment parameterSegment = new ProjectionPathSegment(parameterPath, null, null);
                parameterPath.Add(parameterSegment);
                this.annotations[paramExpr] = new ExpressionAnnotation { Segment = parameterSegment };
            }
        }

        private Expression RebindMethodCallForAggregationMethod(MethodCallExpression methodCallExpr)
        {
            Debug.Assert(methodCallExpr != null, $"{nameof(methodCallExpr)} != null");
            Debug.Assert(methodCallExpr.Arguments.Count == 2 || methodCallExpr.Method.Name == "Count" || methodCallExpr.Method.Name == "LongCount",
                $"{methodCallExpr.Method.Name} is not a supported aggregation method");
            Debug.Assert(methodCallExpr.Arguments.Count == 1 || !(methodCallExpr.Method.Name == "Count" || methodCallExpr.Method.Name == "LongCount"),
                $"{methodCallExpr.Method.Name} is not a supported aggregation method");

            if (!this.resultSelectorMap.TryGetValue(methodCallExpr, out MappingInfo mappingInfo))
            {
                return methodCallExpr;
            }

            Debug.Assert(mappingInfo.Member != null, "mappingInfo.Member != null");
            Debug.Assert(mappingInfo.Type != null, "mappingInfo.Type != null");
            string targetName = mappingInfo.Member;
            Type targetType = mappingInfo.Type;

            Expression baseSourceExpression = methodCallExpr.Arguments[0];
            Expression baseTargetExpression = this.Visit(baseSourceExpression);

            ExpressionAnnotation annotation;
            if (!this.annotations.TryGetValue(baseTargetExpression, out annotation))
            {
                return methodCallExpr;
            }

            ProjectionPathSegment memberSegment = new ProjectionPathSegment(annotation.Segment.StartPath, targetName, targetType);
            annotation.Segment.StartPath.Add(memberSegment);

            Expression value = CallMaterializer(
                nameof(ODataEntityMaterializerInvoker.ProjectionDynamicValueForPath),
                this.materializerExpression,
                annotation.Segment.StartPath.RootEntry,
                Expression.Constant(targetType, typeof(Type)),
                Expression.Constant(annotation.Segment.StartPath, typeof(object)));
            this.annotations.Add(value, new ExpressionAnnotation { Segment = annotation.Segment.StartPath[annotation.Segment.StartPath.Count - 1] });
            Expression result = Expression.Convert(value, targetType);
            this.annotations.Add(result, new ExpressionAnnotation { Segment = annotation.Segment.StartPath[annotation.Segment.StartPath.Count - 1] });

            return result;
        }

        #endregion Private methods.

        #region Inner Types

        /// <summary>Annotates an expression, typically from the target tree.</summary>
        private class ExpressionAnnotation
        {
            /// <summary>Segment that marks the path found to an expression.</summary>
            internal ProjectionPathSegment Segment
            {
                get;
                set;
            }
        }

        private class MappingInfo
        {
            public string Member { get; set; }
            public Type Type { get; set; }
            public Expression GroupingExpression { get; set; }
        }

        /// <summary>
        /// This class analyzes the GroupBy result selector to establish a mapping between 
        /// the expressions and the property they correspond to in the JSON response.
        /// </summary>
        /// <remarks>
        /// For example,
        ///     CategoryName corresponds to Product/Category/Name,
        ///     YearStr corresponds to Time/Year,
        ///     d3.Average(d4 => d4.Amount) corresponds to AverageAmount,
        ///     d3.Sum(d4 => d4.Amount) corresponds to SumAmount,
        /// in the following GroupBy expression:
        ///     dataServiceContext.Sales.GroupBy(
        ///         d1 => new { CategoryName = d1.Product.Category.Name, d1.Time.Year },
        ///         (d2, d3) => new
        ///         {
        ///             CategoryName = d2.CategoryName,
        ///             YearStr = d2.Year.ToString(),
        ///             AverageAmount = d3.Average(d4 => d4.Amount).ToString(),
        ///             SumAmount = d4.Sum(d4 => d4.Amount)
        ///         });
        /// </remarks>
        private class GroupByProjectionAnalyzer : ALinqExpressionVisitor
        {
            private GroupByProjectionPlanCompiler compiler;
            private readonly Dictionary<Expression, KeyValuePair<string, Type>> memberMap;
            private readonly Stack<string> memberInScope;

            /// <summary>
            /// Initializes a new <see cref="GroupByProjectionAnalyzer"/> instance.
            /// </summary>
            private GroupByProjectionAnalyzer()
            {
                this.memberMap = new Dictionary<Expression, KeyValuePair<string, Type>>(ReferenceEqualityComparer<Expression>.Instance);
                this.memberInScope = new Stack<string>();
            }

            /// <summary>
            /// Analyzes the GroupBy result selector to establish a mapping between 
            /// the expressions and the property they correspond to in the JSON response.
            /// </summary>
            /// <param name="compiler">The parent <see cref="GroupByProjectionPlanCompiler"/> instance.</param>
            /// <param name="resultSelector">The lambda expression to analyze.</param>
            internal static void Analyze(GroupByProjectionPlanCompiler compiler, LambdaExpression resultSelector)
            {
                GroupByProjectionAnalyzer analyzer = new GroupByProjectionAnalyzer();
                analyzer.compiler = compiler;

                analyzer.Visit(resultSelector.Body);
            }

            /// <inheritdoc/>
            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                Debug.Assert(m != null, "m != null");

                Expression baseSourceExpression = m.Expression;

                // if primitive or nullable primitive, allow member access... i.e. calling Value on nullable<int>
                if (PrimitiveType.IsKnownNullableType(baseSourceExpression.Type))
                {
                    return base.VisitMemberAccess(m);
                }

                if (compiler.keySelectorMap.TryGetValue(m.Member.Name, out Expression groupingExpression))
                {
                    compiler.resultSelectorMap.Add(
                        m,
                        new MappingInfo
                        {
                            GroupingExpression = groupingExpression
                        });
                }

                return m;
            }

            /// <inheritdoc/>
            internal override Expression VisitParameter(ParameterExpression p)
            {
                if (compiler.keySelectorMap.Count == 1)
                {
                    compiler.resultSelectorMap.Add(
                        p,
                        new MappingInfo
                        {
                            GroupingExpression = compiler.keySelectorMap.ElementAt(0).Value 
                        });
                }

                return p;
            }

            /// <inheritdoc/>
            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
                Debug.Assert(m != null, "m != null");

                if (this.memberMap.TryGetValue(m, out KeyValuePair<string, Type> member))
                {
                    this.memberInScope.Push(member.Key);
                }

                Expression original = compiler.GetExpressionBeforeNormalization(m);
                if (original != m)
                {
                    return this.Visit(original);
                }

                Expression result;

                SequenceMethod sequenceMethod;
                ReflectionUtil.TryIdentifySequenceMethod(m.Method, out sequenceMethod);

                switch (sequenceMethod)
                {
                    case SequenceMethod.SumIntSelector:
                    case SequenceMethod.SumDoubleSelector:
                    case SequenceMethod.SumDecimalSelector:
                    case SequenceMethod.SumLongSelector:
                    case SequenceMethod.SumSingleSelector:
                    case SequenceMethod.SumNullableIntSelector:
                    case SequenceMethod.SumNullableDoubleSelector:
                    case SequenceMethod.SumNullableDecimalSelector:
                    case SequenceMethod.SumNullableLongSelector:
                    case SequenceMethod.SumNullableSingleSelector:
                    case SequenceMethod.AverageIntSelector:
                    case SequenceMethod.AverageDoubleSelector:
                    case SequenceMethod.AverageDecimalSelector:
                    case SequenceMethod.AverageLongSelector:
                    case SequenceMethod.AverageSingleSelector:
                    case SequenceMethod.AverageNullableIntSelector:
                    case SequenceMethod.AverageNullableDoubleSelector:
                    case SequenceMethod.AverageNullableDecimalSelector:
                    case SequenceMethod.AverageNullableLongSelector:
                    case SequenceMethod.AverageNullableSingleSelector:
                    case SequenceMethod.MinIntSelector:
                    case SequenceMethod.MinDoubleSelector:
                    case SequenceMethod.MinDecimalSelector:
                    case SequenceMethod.MinLongSelector:
                    case SequenceMethod.MinSingleSelector:
                    case SequenceMethod.MinNullableIntSelector:
                    case SequenceMethod.MinNullableDoubleSelector:
                    case SequenceMethod.MinNullableDecimalSelector:
                    case SequenceMethod.MinNullableLongSelector:
                    case SequenceMethod.MinNullableSingleSelector:
                    case SequenceMethod.MaxIntSelector:
                    case SequenceMethod.MaxDoubleSelector:
                    case SequenceMethod.MaxDecimalSelector:
                    case SequenceMethod.MaxLongSelector:
                    case SequenceMethod.MaxSingleSelector:
                    case SequenceMethod.MaxNullableIntSelector:
                    case SequenceMethod.MaxNullableDoubleSelector:
                    case SequenceMethod.MaxNullableDecimalSelector:
                    case SequenceMethod.MaxNullableLongSelector:
                    case SequenceMethod.MaxNullableSingleSelector:
                    case SequenceMethod.Count:
                    case SequenceMethod.LongCount:
                    case SequenceMethod.CountDistinctSelector:
                        compiler.resultSelectorMap.Add(
                            m,
                            new MappingInfo
                            {
                                GroupingExpression = null,
                                Member = this.memberInScope.Peek(),
                                Type = m.Type
                            });
                        result = m;

                        break;
                    default:
                        result = base.VisitMethodCall(m);

                        break;
                }

                if (this.memberMap.ContainsKey(m))
                {
                    this.memberInScope.Pop();
                }

                return result;
            }

            /// <inheritdoc/>
            internal override NewExpression VisitNew(NewExpression nex)
            {
                ParameterInfo[] parameters;
                if (nex.Members != null && nex.Members.Count == nex.Arguments.Count)
                {
                    for (int i = 0; i < nex.Arguments.Count; i++)
                    {
                        PropertyInfo property = nex.Members[i] as PropertyInfo;
                        Debug.Assert(property != null, $"property != null");

                        this.memberMap.Add(nex.Arguments[i], new KeyValuePair<string, Type>(property.Name, property.PropertyType));
                    }
                }
                else if (nex.Arguments.Count > 0 && (parameters = nex.Constructor.GetParameters()).Length >= nex.Arguments.Count)
                {
                    for (int i = 0; i < nex.Arguments.Count; i++)
                    {
                        this.memberMap.Add(nex.Arguments[i], new KeyValuePair<string, Type>(parameters[i].Name, parameters[i].ParameterType));
                    }
                }

                return base.VisitNew(nex);
            }

            /// <inheritdoc/>
            internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
            {
                // Maintain a mapping of expression argument and respective member
                PropertyInfo property = assignment.Member as PropertyInfo;
                Debug.Assert(property != null, $"property != null");

                this.memberMap.Add(assignment.Expression, new KeyValuePair<string, Type>(property.Name, property.PropertyType));

                return base.VisitMemberAssignment(assignment);
            }
        }

        /// <summary>
        /// Use this class to help keep track of projection paths built
        /// while compiling a projection-based materialization plan.
        /// </summary>
        private class GroupByProjectionPathBuilder
        {
            /// <summary>Stack of lambda expressions in scope.</summary>
            private readonly Stack<ParameterExpression> parameterExpressions;

            /// <summary>
            /// Stack of expected type expression for <fieldref name="parameterExpressions"/>.
            /// </summary>
            private readonly Stack<Expression> parameterExpressionTypes;

            /// <summary>Stack of 'entry' parameter expressions.</summary>
            private readonly Stack<Expression> parameterEntries;

            /// <summary>Stack of projection (target-tree) types for parameters.</summary>
            private readonly Stack<Type> parameterProjectionTypes;

            /// <summary>Initializes a new <see cref="GroupByProjectionPathBuilder"/> instance.</summary>
            internal GroupByProjectionPathBuilder()
            {
                this.parameterExpressions = new Stack<ParameterExpression>();
                this.parameterExpressionTypes = new Stack<Expression>();
                this.parameterEntries = new Stack<Expression>();
                this.parameterProjectionTypes = new Stack<Type>();
            }

            /// <summary>Expression for the expected type parameter.</summary>
            internal Expression ExpectedParamTypeInScope
            {
                get
                {
                    Debug.Assert(this.parameterExpressionTypes.Count > 0, "this.parameterExpressionTypes.Count > 0");
                    return this.parameterExpressionTypes.Peek();
                }
            }

            /// <summary>Expression for the entity parameter in the source tree lambda.</summary>
            internal Expression LambdaParameterInScope
            {
                get
                {
                    return this.parameterExpressions.Peek();
                }
            }

            /// <summary>Expression for the entry parameter in the target tree.</summary>
            internal Expression ParameterEntryInScope
            {
                get
                {
                    return this.parameterEntries.Peek();
                }
            }

            /// <summary>Provides a string representation of this object.</summary>
            /// <returns>String representation of this object.</returns>
            public override string ToString()
            {
                string result = "GroupByProjectionPathBuilder: ";
                if (this.parameterExpressions.Count == 0)
                {
                    result += "(empty)";
                }
                else
                {
                    result += " param:" + this.ParameterEntryInScope;
                }

                return result;
            }

            /// <summary>Records that a lambda scope has been entered when visiting a projection.</summary>
            /// <param name="lambda">Lambda being visited.</param>
            /// <param name="entry">Expression to the entry parameter from the target tree.</param>
            /// <param name="expectedType">Expression to the entry-expected-type from the target tree.</param>
            internal void EnterLambdaScope(LambdaExpression lambda, Expression entry, Expression expectedType)
            {
                Debug.Assert(lambda != null, "lambda != null");
                Debug.Assert(lambda.Parameters.Count == 2, "lambda.Parameters.Count == 2");

                ParameterExpression param = lambda.Parameters[1];
                Type projectionType = lambda.Body.Type;

                this.parameterExpressions.Push(param);
                this.parameterExpressionTypes.Push(expectedType);
                this.parameterEntries.Push(entry);
                this.parameterProjectionTypes.Push(projectionType);
            }

            /// <summary>Records that a lambda scope has been left when visiting a projection.</summary>
            internal void LeaveLambdaScope()
            {
                this.parameterExpressions.Pop();
                this.parameterExpressionTypes.Pop();
                this.parameterEntries.Pop();
                this.parameterProjectionTypes.Pop();
            }
        }

        #endregion Inner Types
    }
}
