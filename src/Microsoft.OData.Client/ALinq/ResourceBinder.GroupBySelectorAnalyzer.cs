//---------------------------------------------------------------------
// <copyright file="ResourceBinder.GroupBySelectorAnalyzer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;

    #endregion Namespaces

    internal partial class ResourceBinder
    {
        /// <summary>
        /// Analyzes a GroupBy key selector for property or properties that the input sequence is grouped by.
        /// </summary>
        private sealed class GroupBySelectorAnalyzer : DataServiceALinqExpressionVisitor
        {
            /// <summary>The input resource, as a queryable resource</summary>
            private readonly QueryableResourceExpression input;

            /// <summary>
            /// Creates an <see cref="GroupBySelectorAnalyzer"/> expression.
            /// </summary>
            /// <param name="source">The source expression.</param>
            private GroupBySelectorAnalyzer(QueryableResourceExpression source)
            {
                this.input = source;
            }

            /// <summary>
            /// Analyzes a GroupBy key selector for property or properties that the input sequence is grouped by. 
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            /// <param name="lambdaExpr">Lambda expression to analyze.</param>
            internal static void Analyze(QueryableResourceExpression input, LambdaExpression lambdaExpr)
            {
                GroupBySelectorAnalyzer analyzer = new GroupBySelectorAnalyzer(input);

                MemberInitExpression memberInitExpr = lambdaExpr.Body as MemberInitExpression;

                if (memberInitExpr != null)
                {
                    analyzer.Visit(memberInitExpr);
                }
                else
                {
                    analyzer.Visit(lambdaExpr.Body);
                }
            }

            /// <inheritdoc/>
            internal override NewExpression VisitNew(NewExpression nex)
            {
                // Maintain a mapping of grouping expression and respective member
                // The mapping is cross-referenced if any of the grouping expression 
                // is referenced in result selector
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    this.input.Apply.GroupingExpressionsMap.Add(nex.Members[i].Name, nex.Arguments[i]);
                }

                return base.VisitNew(nex);
            }

            /// <inheritdoc/>
            internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
            {
                // Maintain a mapping of grouping expression and respective member
                // The mapping is cross-referenced if any of the grouping expression 
                // is referenced in result selector
                this.input.Apply.GroupingExpressionsMap.Add(assignment.Member.Name, assignment.Expression);

                return base.VisitMemberAssignment(assignment);
            }

            /// <inheritdoc/>
            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                MemberExpression memberExpr = StripTo<MemberExpression>(m);

                MemberExpression mExpr = memberExpr;
                
                while (true)
                {
                    if (mExpr.Expression.NodeType == ExpressionType.MemberAccess)
                    {
                        mExpr = StripTo<MemberExpression>(mExpr.Expression);
                    }
                    else
                    {
                        break;
                    }
                }

                ParameterExpression parameterExpr = StripTo<ParameterExpression>(mExpr.Expression);

                if (parameterExpr != null)
                {
                    List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
                    Expression boundExpression = InputBinder.Bind(memberExpr, this.input, parameterExpr, referencedInputs);

                    if (referencedInputs.Count == 1 && referencedInputs[0] == this.input)
                    {
                        EnsureApplyInitialized(this.input);
                        Debug.Assert(input.Apply != null, "input.Apply != null");

                        this.input.Apply.GroupingExpressions.Add(boundExpression);
                    }
                }

                return m;
            }
        }
    }
}
