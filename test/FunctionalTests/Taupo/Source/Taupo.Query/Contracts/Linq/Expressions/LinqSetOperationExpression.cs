//---------------------------------------------------------------------
// <copyright file="LinqSetOperationExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    /// <summary>
    /// Abstract Expression node representing query method expression in a Linq query.
    /// </summary>
    public abstract class LinqSetOperationExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the LinqSetOperationExpression class.
        /// </summary>
        /// <param name="firstSource">The First input collection of the expression</param>
        /// <param name="secondSource">The Second input collection of the expression</param>
        /// <param name="type">The type of expression.</param>
        protected LinqSetOperationExpression(QueryExpression firstSource, QueryExpression secondSource, QueryType type)
            : base(type)
        {
            this.FirstSource = firstSource;
            this.SecondSource = secondSource;
        }

        /// <summary>
        /// Gets the first source expression.
        /// </summary>
        public QueryExpression FirstSource { get; private set; }

        /// <summary>
        /// Gets the second source expression.
        /// </summary>
        public QueryExpression SecondSource { get; private set; }
    }
}
