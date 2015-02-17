//---------------------------------------------------------------------
// <copyright file="IQueryExpressionReplacingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    /// <summary>
    /// Query expression replacing visitor.
    /// </summary>
    public interface IQueryExpressionReplacingVisitor : ICommonExpressionVisitor<QueryExpression>
    {
        /// <summary>
        /// Replaces the expression.
        /// </summary>
        /// <param name="queryExpression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        QueryExpression ReplaceExpression(QueryExpression queryExpression);
    }
}
