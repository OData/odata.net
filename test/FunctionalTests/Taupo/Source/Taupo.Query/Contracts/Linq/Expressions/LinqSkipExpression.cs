//---------------------------------------------------------------------
// <copyright file="LinqSkipExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Globalization;

    /// <summary>
    /// Expression node representing the Skip expression in a Linq query.
    /// </summary>
    public class LinqSkipExpression : LinqQueryMethodExpression
    {
        internal LinqSkipExpression(QueryExpression source, QueryExpression skipCount, QueryType type)
            : base(source, type)
        {
            this.SkipCount = skipCount;
        }

        /// <summary>
        /// Gets the skip count.
        /// </summary>
        public QueryExpression SkipCount { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.Skip({1})", this.Source, this.SkipCount);
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
