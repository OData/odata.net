//---------------------------------------------------------------------
// <copyright file="LinqSingleExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Globalization;

    /// <summary>
    /// Expression node representing the Single expression in a Linq query.
    /// Note that LINQ to Entities does not support Single, but customers can create queries like
    ///     var q = Context.Products.Where(p => p.ProductId == 1).Single()
    /// with Single being the LINQ to Objects method.
    /// </summary>
    public class LinqSingleExpression : LinqQueryMethodWithLambdaExpression
    {
        internal LinqSingleExpression(QueryExpression source, LinqLambdaExpression predicate, QueryType type)
            : base(source, predicate, type)
        {
        }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.Single({1})", this.Source, this.Lambda);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return ((ILinqExpressionVisitor<TResult>)visitor).Visit(this);
        }
    }
}
