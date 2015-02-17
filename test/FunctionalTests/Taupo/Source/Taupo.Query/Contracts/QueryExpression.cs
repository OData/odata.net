//---------------------------------------------------------------------
// <copyright file="QueryExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Diagnostics;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Base node class for expression trees used to generate, execute, and verify queries.
    /// </summary>
    public abstract class QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the QueryExpression class.
        /// </summary>
        /// <param name="type">The type of expression.</param>
        protected QueryExpression(QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            this.ExpressionType = type;
        }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public QueryType ExpressionType { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public abstract TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor);
    }
}
