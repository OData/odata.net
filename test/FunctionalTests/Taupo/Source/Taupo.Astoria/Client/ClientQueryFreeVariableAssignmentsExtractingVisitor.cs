//---------------------------------------------------------------------
// <copyright file="ClientQueryFreeVariableAssignmentsExtractingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Extracts free variable assignments from query.
    /// </summary>
    public class ClientQueryFreeVariableAssignmentsExtractingVisitor : LinqToAstoriaExpressionReplacingVisitor
    {
        private Dictionary<string, ReadOnlyCollection<QueryExpression>> freeVariableAssignments;

        /// <summary>
        /// Replaces the query.
        /// </summary>
        /// <param name="queryExpression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public new QueryExpression ReplaceExpression(QueryExpression queryExpression)
        {
            this.freeVariableAssignments = new Dictionary<string, ReadOnlyCollection<QueryExpression>>();
            return queryExpression.Accept(this);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqFreeVariableExpression. Free variable information is extracted.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public override QueryExpression Visit(LinqFreeVariableExpression expression)
        {
            this.freeVariableAssignments.Add(expression.Name, expression.Values);

            return base.Visit(expression);
        }

        /// <summary>
        /// Extracts free variable assignments information.
        /// </summary>
        /// <param name="expression">Expression to extract information from.</param>
        /// <returns>Dictionary of free variable names with their coresponding values.</returns>
        public Dictionary<string, ReadOnlyCollection<QueryExpression>> ExtractFreeVariableAssignments(QueryExpression expression)
        {
            this.ReplaceExpression(expression);

            return this.freeVariableAssignments;
        }    
    }
}