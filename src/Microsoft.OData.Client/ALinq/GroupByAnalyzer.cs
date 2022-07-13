//---------------------------------------------------------------------
// <copyright file="GroupByAnalyzer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.UriParser.Aggregation;

    #endregion Namespaces

    /// <summary>
    /// Analyzes an expression to check whether it can be satisfied with $apply.
    /// </summary>
    internal static class GroupByAnalyzer
    {
        /// <summary>
        /// Analyzes <paramref name="methodCallExpr"/> to check whether it can be satisfied with $apply.
        /// The Projection property of <paramref name="resourceExpr"/> is set
        /// when <paramref name="methodCallExpr"/> is analyzed successfully.
        /// </summary>
        /// <param name="methodCallExpr">Expression to analyze.</param>
        /// <param name="source">The resource set expression (source) for the GroupBy.</param>
        /// <param name="resourceExpr">The resource expression in scope.</param>
        /// <returns>true if <paramref name="methodCallExpr"/> can be satisfied with $apply; false otherwise.</returns>
        internal static bool Analyze(
            MethodCallExpression methodCallExpr,
            QueryableResourceExpression source,
            ResourceExpression resourceExpr)
        {
            Debug.Assert(source != null, $"{nameof(source)} != null");
            Debug.Assert(resourceExpr != null, $"{nameof(resourceExpr)} != null");
            Debug.Assert(methodCallExpr != null, $"{nameof(methodCallExpr)} != null");
            Debug.Assert(methodCallExpr.Arguments.Count == 3, $"{nameof(methodCallExpr)}.Arguments.Count == 3");

            LambdaExpression keySelector;
            // Key selector should be a single argument lambda: d1 => ...
            if (!ResourceBinder.PatternRules.MatchSingleArgumentLambda(methodCallExpr.Arguments[1], out keySelector))
            {
                return false;
            }

            LambdaExpression resultSelector;
            // Result selector should be a double argument lambda: (d2, d3) => ...
            if (!ResourceBinder.PatternRules.MatchDoubleArgumentLambda(methodCallExpr.Arguments[2], out resultSelector))
            {
                return false;
            }

            // Analyze result selector - Must be a simple instantiation.
            // A result selector where the body is a single argument lambda is also valid: (d2, d3) => d3.Average(d4 => d4.Amount)
            // However, this presents a challenge since an alias is required in the aggregate transformation: aggregate(Amount with average as [alias])
            // We could circumvent this by defaulting to [aggregate method]+[aggregate property], e.g., AverageAmount
            // but this is deferred to a future release
            if (resultSelector.Body.NodeType != ExpressionType.MemberInit && resultSelector.Body.NodeType != ExpressionType.New)
            {
                return false;
            }

            // Analyze key selector
            if (!AnalyzeGroupByKeySelector(source, keySelector))
            {
                return false;
            }

            // Analyze result selector
            GroupByResultSelectorAnalyzer.Analyze(source, resultSelector);

            resourceExpr.Projection = new ProjectionQueryOptionExpression(resultSelector.Body.Type, resultSelector, new List<string>());

            return true;
        }

        /// <summary>
        /// Analyzes a GroupBy key selector for property/ies that the input sequence is grouped by.
        /// </summary>
        /// <param name="input">The resource set expression (source) for the GroupBy.</param>
        /// <param name="keySelector">Key selector expression to analyze.</param>
        /// <returns>true if analyzed successfully; false otherwise</returns>
        private static bool AnalyzeGroupByKeySelector(QueryableResourceExpression input, LambdaExpression keySelector)
        {
            Debug.Assert(input != null, $"{nameof(input)} != null");
            Debug.Assert(keySelector != null, $"{nameof(keySelector)} != null");

            EnsureApplyInitialized(input);
            Debug.Assert(input.Apply != null, $"{nameof(input)}.Apply != null");

            // Scenario 1: GroupBy(d1 => d1.Property) and GroupBy(d1 = d1.NavProperty...Property)
            if (keySelector.Body is MemberExpression memberExpr)
            {
                // Validate grouping expression
                ResourceBinder.ValidationRules.ValidateGroupingExpression(memberExpr);

                Expression boundExpression;
                if (!TryBindToInput(input, keySelector, out boundExpression))
                {
                    return false;
                }

                input.Apply.GroupingExpressions.Add(boundExpression);
                input.Apply.KeySelectorMap.Add(memberExpr.Member.Name, memberExpr);

                return true;
            }

            // Scenario 2: GroupBy(d1 => [Constant])
            if (keySelector.Body is ConstantExpression constExpr)
            {
                input.Apply.KeySelectorMap.Add(constExpr.Value.ToString(), constExpr);

                return true;
            }

            // Check whether the key selector body is a simple instantiation
            if (keySelector.Body.NodeType != ExpressionType.MemberInit && keySelector.Body.NodeType != ExpressionType.New)
            {
                return false;
            }

            // Scenario 3: GroupBy(d1 => new { d1.Property1, ..., d1.PropertyN })
            // Scenario 4: GroupBy(d1 => new Cls { ClsProperty1 = d1.Property1, ..., ClsPropertyN = d1.PropertyN }) - not common but possible
            GroupByKeySelectorAnalyzer.Analyze(input, keySelector);

            return true;
        }

        /// <summary>
        /// Ensure apply query option for the resource set is initialized
        /// </summary>
        /// <param name="input">The resource set expression (source) for the GroupBy.</param>
        private static void EnsureApplyInitialized(QueryableResourceExpression input)
        {
            Debug.Assert(input != null, $"{nameof(input)} != null");

            if (input.Apply == null)
            {
                ResourceBinder.AddSequenceQueryOption(input, new ApplyQueryOptionExpression(input.Type));
            }
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

        /// <summary>
        /// Analyzes a GroupBy key selector for property or properties that the input sequence is grouped by.
        /// </summary>
        private sealed class GroupByKeySelectorAnalyzer : DataServiceALinqExpressionVisitor
        {
            /// <summary>The input resource, as a queryable resource.</summary>
            private readonly QueryableResourceExpression input;

            /// <summary>The key selector lambda parameter.</summary>
            private readonly ParameterExpression lambdaParameter;

            /// <summary>
            /// Creates an <see cref="GroupByKeySelectorAnalyzer"/> expression.
            /// </summary>
            /// <param name="source">The source expression.</param>
            /// <param name="paramExpr">The parameter of the key selector expression, e.g., d1 => new { d1.ProductId, d1.CustomerId }.</param>
            private GroupByKeySelectorAnalyzer(QueryableResourceExpression source, ParameterExpression paramExpr)
            {
                this.input = source;
                this.lambdaParameter = paramExpr;
            }

            /// <summary>
            /// Analyzes a GroupBy key selector for property or properties that the input sequence is grouped by. 
            /// </summary>
            /// <param name="input">The resource set expression (source) for the GroupBy.</param>
            /// <param name="keySelector">Key selector expression to analyze.</param>
            internal static void Analyze(QueryableResourceExpression input, LambdaExpression keySelector)
            {
                Debug.Assert(input != null, $"{nameof(input)} != null");
                Debug.Assert(keySelector != null, $"{nameof(keySelector)} != null");
                Debug.Assert(keySelector.Parameters.Count == 1, $"{nameof(keySelector)}.Parameters.Count == 1");

                GroupByKeySelectorAnalyzer keySelectorAnalyzer = new GroupByKeySelectorAnalyzer(input, keySelector.Parameters[0]);

                keySelectorAnalyzer.Visit(keySelector);
            }

            /// <inheritdoc/>
            internal override NewExpression VisitNew(NewExpression nex)
            {
                // Maintain a mapping of grouping expression and respective member
                // The mapping is cross-referenced if any of the grouping expressions
                // is referenced in result selector
                if (nex.Members != null && nex.Members.Count == nex.Arguments.Count)
                {
                    // This block caters for the following scenario:
                    //
                    // dataServiceContext.Sales.GroupBy(
                    //     d1 => new { d1.ProductId },
                    //     (d2, d3) => new { AverageAmount = d3.Average(d4 => d4.Amount) })
                    for (int i = 0; i < nex.Arguments.Count; i++)
                    {
                        this.input.Apply.KeySelectorMap.Add(nex.Members[i].Name, nex.Arguments[i]);
                    }
                }
                else if (nex.Arguments.Count > 0 && nex.Constructor.GetParameters().Length > 0)
                {
                    // Constructor initialization in key selector not supported
                    throw new NotSupportedException(Strings.ALinq_InvalidGroupByKeySelector(nex));
                }

                return base.VisitNew(nex);
            }

            /// <inheritdoc/>
            internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
            {
                // Maintain a mapping of grouping expression and respective member
                // The mapping is cross-referenced if any of the grouping expressions
                // is referenced in result selector
                this.input.Apply.KeySelectorMap.Add(assignment.Member.Name, assignment.Expression);

                return base.VisitMemberAssignment(assignment);
            }

            /// <inheritdoc/>
            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                // Validate the grouping expression
                ResourceBinder.ValidationRules.ValidateGroupingExpression(m);

                List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
                Expression boundExpression = InputBinder.Bind(m, this.input, this.lambdaParameter, referencedInputs);

                if (referencedInputs.Count == 1 && referencedInputs[0] == this.input)
                {
                    EnsureApplyInitialized(this.input);
                    Debug.Assert(input.Apply != null, $"{nameof(input)}.Apply != null");

                    this.input.Apply.GroupingExpressions.Add(boundExpression);
                }

                return m;
            }
        }

        /// <summary>
        /// Analyzes expression for aggregate expressions in the GroupBy result selector.
        /// </summary>
        private sealed class GroupByResultSelectorAnalyzer : DataServiceALinqExpressionVisitor
        {
            /// <summary>The input resource, as a queryable resource</summary>
            private readonly QueryableResourceExpression input;

            /// <summary>
            /// Mapping of expressions in the GroupBy result selector to a key-value pair
            /// of the respective member name and type.
            /// </summary>
            /// <remarks>
            /// This property will contain a mapping of the expression {d3.Average(d4 => d4.Amount)} to a key-value pair
            /// of the respective member name {AverageAmount} and type {decimal} given the following GroupBy expression:
            /// dsc.CreateQuery&lt;Sale&gt;("Sales").GroupBy(d1 => d1.Product.Category.Name, (d2, d3) => new { CategoryName = d2, AverageAmount = d3.Average(d4 => d4.Amount) })
            /// </remarks>
            private readonly IDictionary<Expression, KeyValuePair<string, Type>> resultSelectorMap;

            /// <summary>Tracks the member a method call is mapped to when analyzing the GroupBy result selector.</summary>
            private readonly Stack<string> memberInScope;

            /// <summary>
            /// Creates an <see cref="GroupByResultSelectorAnalyzer"/> expression.
            /// </summary>
            /// <param name="input">The resource set expression (source) for the GroupBy.</param>
            private GroupByResultSelectorAnalyzer(QueryableResourceExpression input)
            {
                this.input = input;
                this.resultSelectorMap = new Dictionary<Expression, KeyValuePair<string, Type>>(ReferenceEqualityComparer<Expression>.Instance);
                this.memberInScope = new Stack<string>();
            }

            /// <summary>
            /// Analyzes expression for aggregate expressions in the GroupBy result selector.
            /// </summary>
            /// <param name="input">The resource set expression (source) for the GroupBy.</param>
            /// <param name="resultSelector">Result selector expression to analyze.</param>
            internal static void Analyze(QueryableResourceExpression input, LambdaExpression resultSelector)
            {
                Debug.Assert(input != null, $"{nameof(input)} != null");
                Debug.Assert(resultSelector != null, $"{nameof(resultSelector)} != null");

                GroupByResultSelectorAnalyzer resultSelectorAnalyzer = new GroupByResultSelectorAnalyzer(input);

                resultSelectorAnalyzer.Visit(resultSelector);
            }

            /// <inheritdoc/>
            internal override NewExpression VisitNew(NewExpression nex)
            {
                ParameterInfo[] parameters;

                // Maintain a mapping of expression argument and respective member
                if (nex.Members != null && nex.Members.Count == nex.Arguments.Count)
                {
                    // This block caters for the following scenario:
                    //
                    // dataServiceContext.Sales.GroupBy(
                    //     d1 => d1.ProductId,
                    //     (d2, d3) => new { AverageAmount = d3.Average(d4 => d4.Amount) })

                    for (int i = 0; i < nex.Arguments.Count; i++)
                    {
                        this.resultSelectorMap.Add(
                            nex.Arguments[i],
                            new KeyValuePair<string, Type>(nex.Members[i].Name, (nex.Members[i] as PropertyInfo).PropertyType));
                    }
                }
                else if (nex.Arguments.Count > 0 && (parameters = nex.Constructor.GetParameters()).Length >= nex.Arguments.Count)
                {
                    // Use of >= in the above if statement caters for optional parameters

                    // Given the following class definition:
                    //
                    // class GroupedResult
                    // {
                    //     public GroupedResult(decimal averageAmount) { AverageAmount = averageAmount }
                    //     public decimal AverageAmount { get; }
                    // }
                    //
                    // this block caters for the following scenario:
                    //
                    // dataServiceContext.Sales.GroupBy(
                    //     d1 => d1.ProductId,
                    //     (d2, d3) => new GroupedResult(d3.Average(d4 => d4.Amount)))

                    for (int i = 0; i < nex.Arguments.Count; i++)
                    {
                        this.resultSelectorMap.Add(
                            nex.Arguments[i],
                            new KeyValuePair<string, Type>(parameters[i].Name, parameters[i].ParameterType));
                    }
                }

                return base.VisitNew(nex);
            }

            /// <inheritdoc/>
            internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
            {
                // Maintain a mapping of expression argument and respective member
                this.resultSelectorMap.Add(
                    assignment.Expression,
                    new KeyValuePair<string, Type>(assignment.Member.Name, (assignment.Member as PropertyInfo).PropertyType));

                return base.VisitMemberAssignment(assignment);
            }

            /// <inheritdoc/>
            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (this.resultSelectorMap.TryGetValue(m, out KeyValuePair<string, Type> member))
                {
                    this.memberInScope.Push(member.Key);
                }

                // Caters for the following scenarios:
                // 1).
                // dataServiceContext.Sales.GroupBy(
                //     d1 => d1.Time.Year,
                //     (d2, d3) => new { YearStr = d2.ToString() });
                // 2).
                // dataServiceContext.Sales.GroupBy(
                //     d1 => d1.ProductId,
                //     (d2, d3) => new { AverageAmount = d3.Average(d4 => d4.Amount).ToString() });
                if (m.Method.Name == "ToString")
                {
                    if (m.Object is MethodCallExpression)
                    {
                        return Expression.Call(this.VisitMethodCall(m.Object as MethodCallExpression), m.Method, m.Arguments);
                    }

                    return base.VisitMethodCall(m);
                }

                SequenceMethod sequenceMethod;
                Expression result;
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
                        result = this.AnalyzeAggregation(m, AggregationMethod.Sum);

                        break;
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
                        result = this.AnalyzeAggregation(m, AggregationMethod.Average);

                        break;
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
                        result = this.AnalyzeAggregation(m, AggregationMethod.Min);

                        break;
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
                        result = this.AnalyzeAggregation(m, AggregationMethod.Max);

                        break;
                    case SequenceMethod.Count:
                    case SequenceMethod.LongCount:
                        // Add aggregation to $apply aggregations
                        AddAggregation(null, AggregationMethod.VirtualPropertyCount);
                        result = m;

                        break;
                    case SequenceMethod.CountDistinctSelector:
                        result = this.AnalyzeAggregation(m, AggregationMethod.CountDistinct);

                        break;
                    default:
                        throw Error.MethodNotSupported(m);
                };

                if (this.resultSelectorMap.ContainsKey(m))
                {
                    this.memberInScope.Pop();
                }

                return result;
            }

            /// <summary>
            /// Analyzes an aggregation expression.
            /// </summary>
            /// <param name="methodCallExpr">Expression to analyze.</param>
            /// <param name="aggregationMethod">The aggregation method.</param>
            /// <returns>The analyzed aggregate expression.</returns>
            private Expression AnalyzeAggregation(MethodCallExpression methodCallExpr, AggregationMethod aggregationMethod)
            {
                Debug.Assert(methodCallExpr != null, $"{nameof(methodCallExpr)} != null");

                if (methodCallExpr.Arguments.Count != 2)
                {
                    return methodCallExpr;
                }

                LambdaExpression lambdaExpr = ResourceBinder.StripTo<LambdaExpression>(methodCallExpr.Arguments[1]);
                if (lambdaExpr == null)
                {
                    return methodCallExpr;
                }

                ResourceBinder.ValidationRules.DisallowExpressionEndWithTypeAs(lambdaExpr.Body, methodCallExpr.Method.Name);

                Expression selector;
                if (!TryBindToInput(input, lambdaExpr, out selector))
                {
                    // UNSUPPORTED: Lambda should reference the input, and only the input
                    return methodCallExpr;
                }

                // Add aggregation to $apply aggregations
                AddAggregation(selector, aggregationMethod);

                return methodCallExpr;
            }

            /// <summary>
            /// Adds aggregation to $apply aggregations.
            /// </summary>
            /// <param name="selector">The selector.</param>
            /// <param name="aggregationMethod">The aggregation method.</param>
            private void AddAggregation(Expression selector, AggregationMethod aggregationMethod)
            {
                if (this.memberInScope.Count > 0)
                {
                    this.input.AddAggregation(selector, aggregationMethod, this.memberInScope.Peek());
                }
            }
        }
    }
}
