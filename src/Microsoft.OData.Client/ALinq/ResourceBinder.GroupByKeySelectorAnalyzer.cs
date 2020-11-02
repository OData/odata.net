//---------------------------------------------------------------------
// <copyright file="ResourceBinder.GroupByKeySelectorAnalyzer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    #endregion Namespaces

    internal partial class ResourceBinder
    {
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
            /// <param name="paramExpr">The parameter expression.</param>
            private GroupByKeySelectorAnalyzer(QueryableResourceExpression source, ParameterExpression paramExpr)
            {
                this.input = source;
                this.lambdaParameter = paramExpr;
            }

            /// <summary>
            /// Analyzes a GroupBy key selector for property or properties that the input sequence is grouped by. 
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            /// <param name="keySelector">Key selector expression to analyze.</param>
            internal static void Analyze(QueryableResourceExpression input, LambdaExpression keySelector)
            {
                Debug.Assert(input != null, "input != null");
                Debug.Assert(keySelector != null, "keySelector != null");

                GroupByKeySelectorAnalyzer analyzer = new GroupByKeySelectorAnalyzer(input, keySelector.Parameters[0]);

                MemberInitExpression memberInitExpr = keySelector.Body as MemberInitExpression;

                if (memberInitExpr != null)
                {
                    analyzer.Visit(memberInitExpr);
                }
                else
                {
                    analyzer.Visit(keySelector.Body);
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

                // Validate the grouping expression
                ValidationRules.ValidateGroupingExpression(memberExpr);

                List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
                Expression boundExpression = InputBinder.Bind(memberExpr, this.input, this.lambdaParameter, referencedInputs);

                if (referencedInputs.Count == 1 && referencedInputs[0] == this.input)
                {
                    EnsureApplyInitialized(this.input);
                    Debug.Assert(input.Apply != null, "input.Apply != null");

                    this.input.Apply.GroupingExpressions.Add(boundExpression);
                }

                return m;
            }
        }
    }
}
