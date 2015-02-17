//---------------------------------------------------------------------
// <copyright file="QueryBinaryExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    /// <summary>
    /// Abstract Expression node representing binary expression in queries.
    /// </summary>
    public abstract class QueryBinaryExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the QueryBinaryExpression class.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <param name="type">The type of expression.</param>
        protected QueryBinaryExpression(QueryExpression left, QueryExpression right, QueryType type)
            : base(type)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Gets the left operand of the expression.
        /// </summary>
        public QueryExpression Left { get; private set; }

        /// <summary>
        /// Gets the right operand of the expression.
        /// </summary>
        public QueryExpression Right { get; private set; }
    }
}
