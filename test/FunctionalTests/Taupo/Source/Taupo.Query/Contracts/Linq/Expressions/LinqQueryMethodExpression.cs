//---------------------------------------------------------------------
// <copyright file="LinqQueryMethodExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    /// <summary>
    /// Abstract Expression node representing query method expression in a Linq query.
    /// </summary>
    public abstract class LinqQueryMethodExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the LinqQueryMethodExpression class.
        /// </summary>
        /// <param name="source">The source expression.</param>
        /// <param name="type">The type of expression.</param>
        protected LinqQueryMethodExpression(QueryExpression source, QueryType type)
            : base(type)
        {
            this.Source = source;
        }

        /// <summary>
        /// Gets the source expression.
        /// </summary>
        public QueryExpression Source { get; private set; }
    }
}
