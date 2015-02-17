//---------------------------------------------------------------------
// <copyright file="LinqQueryMethodWithLambdaExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    /// <summary>
    /// Abstract Expression node representing query method expression with a predicate in a Linq query.
    /// </summary>
    public abstract class LinqQueryMethodWithLambdaExpression : LinqQueryMethodExpression
    {
        /// <summary>
        /// Initializes a new instance of the LinqQueryMethodWithLambdaExpression class.
        /// </summary>
        /// <param name="source">The source expression.</param>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="type">The type of expression.</param>
        protected LinqQueryMethodWithLambdaExpression(QueryExpression source, LinqLambdaExpression lambda, QueryType type)
            : base(source, type)
        {
            this.Lambda = lambda;
        }

        /// <summary>
        /// Gets the predicate expression.
        /// </summary>
        public LinqLambdaExpression Lambda { get; private set; }
    }
}
