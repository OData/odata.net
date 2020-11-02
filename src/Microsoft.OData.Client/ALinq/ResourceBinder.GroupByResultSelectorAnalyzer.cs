//---------------------------------------------------------------------
// <copyright file="ResourceBinder.GroupByResultSelectorAnalyzer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.UriParser.Aggregation;

    #endregion Namespaces

    internal partial class ResourceBinder
    {
        /// <summary>
        /// Analyzes expression for aggregate expresssions in the GroupBy result selector.
        /// </summary>
        private sealed class GroupByResultSelectorAnalyzer : DataServiceALinqExpressionVisitor
        {
            /// <summary>The input resource, as a queryable resource</summary>
            private readonly QueryableResourceExpression input;

            /// <summary>Expression to member mapping.</summary>
            private readonly IDictionary<Expression, MemberInfo> expressionMap;

            /// <summary>
            /// Creates an <see cref="GroupByResultSelectorAnalyzer"/> expression.
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            private GroupByResultSelectorAnalyzer(QueryableResourceExpression input)
            {
                this.input = input;
                this.expressionMap = new Dictionary<Expression, MemberInfo>(ReferenceEqualityComparer<Expression>.Instance);
            }

            /// <summary>
            /// Analyzes expression for aggregate expressions in the GroupBy result selector.
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            /// <param name="resultSelector">Result selector expression to analyze.</param>
            internal static void Analyze(QueryableResourceExpression input, LambdaExpression resultSelector)
            {
                Debug.Assert(input != null, "input != null");
                Debug.Assert(resultSelector != null, "resultSelector != null");

                GroupByResultSelectorAnalyzer analyzer = new GroupByResultSelectorAnalyzer(input);

                MemberInitExpression memberInitExpr = resultSelector.Body as MemberInitExpression;

                if (memberInitExpr != null)
                {
                    analyzer.Visit(memberInitExpr);
                }
                else
                {
                    analyzer.Visit(resultSelector.Body);
                }
            }

            /// <inheritdoc/>
            internal override NewExpression VisitNew(NewExpression nex)
            {
                // Maintain a mapping of expression argument and respective member
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    this.expressionMap.Add(nex.Arguments[i], nex.Members[i]);
                }

                return base.VisitNew(nex);
            }

            /// <inheritdoc/>
            internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
            {
                // Maintain a mapping of expression argument and respective member
                this.expressionMap.Add(assignment.Expression, assignment.Member);

                return base.VisitMemberAssignment(assignment);
            }

            /// <inheritdoc/>
            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
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
                        return this.AnalyzeAggregation(m, AggregationMethod.Sum);
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
                        return this.AnalyzeAggregation(m, AggregationMethod.Average);
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
                        return this.AnalyzeAggregation(m, AggregationMethod.Min);
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
                        return this.AnalyzeAggregation(m, AggregationMethod.Max);
                    case SequenceMethod.Count:
                    case SequenceMethod.LongCount:
                        return this.AnalyzeCount(m);
                    case SequenceMethod.CountDistinctSelector:
                        return this.AnalyzeAggregation(m, AggregationMethod.CountDistinct);
                    default:
                        throw Error.MethodNotSupported(m);
                };
            }

            /// <summary>
            /// Analyzes an aggregation expression.
            /// </summary>
            /// <param name="methodCallExpr">Expression to analyze.</param>
            /// <param name="aggregationMethod">The aggregation method.</param>
            /// <returns>The analyzed aggregate expression.</returns>
            private Expression AnalyzeAggregation(MethodCallExpression methodCallExpr, AggregationMethod aggregationMethod)
            {
                Debug.Assert(methodCallExpr != null, "methodCallExpr != null");
                if (methodCallExpr.Arguments.Count != 2)
                {
                    return methodCallExpr;
                }

                LambdaExpression lambdaExpr = StripTo<LambdaExpression>(methodCallExpr.Arguments[1]);
                if (lambdaExpr == null)
                {
                    return methodCallExpr;
                }

                ValidationRules.DisallowExpressionEndWithTypeAs(lambdaExpr.Body, methodCallExpr.Method.Name);

                Expression selector;
                if (!TryBindToInput(input, lambdaExpr, out selector))
                {
                    // UNSUPPORTED: Lambda should reference the input, and only the input
                    return methodCallExpr;
                }

                // Add aggregation to $apply aggregations
                AddAggregation(methodCallExpr, selector, aggregationMethod);

                return methodCallExpr;
            }

            /// <summary>
            /// Analyzes count expression within a GroupBy result selector.
            /// </summary>
            /// <param name="methodCallExpr">Expression to analyze.</param>
            /// <returns>The analyzed count expression.</returns>
            private Expression AnalyzeCount(MethodCallExpression methodCallExpr)
            {
                Debug.Assert(methodCallExpr != null, "methodCallExpr != null");

                // Add aggregation to $apply aggregations
                AddAggregation(methodCallExpr, null, AggregationMethod.VirtualPropertyCount);

                return methodCallExpr;
            }

            /// <summary>
            /// Adds aggregation to $apply aggregations.
            /// </summary>
            /// <param name="methodCallExpr">The <see cref="MethodCallExpression"/> expression.</param>
            /// <param name="selector">The selector.</param>
            /// <param name="aggregationMethod">The aggregation method.</param>
            private void AddAggregation(MethodCallExpression methodCallExpr, Expression selector, AggregationMethod aggregationMethod)
            {
                MemberInfo memberInfo;

                if (expressionMap.TryGetValue(methodCallExpr, out memberInfo))
                {
                    EnsureApplyInitialized(input);
                    Debug.Assert(input.Apply != null, "input.Apply != null");

                    this.input.Apply.Aggregations.Add(new ApplyQueryOptionExpression.Aggregation(selector, aggregationMethod, memberInfo.Name));
                }
            }
        }
    }
}
