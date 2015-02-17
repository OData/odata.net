//---------------------------------------------------------------------
// <copyright file="LinqExceptExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Expression node representing a Cast Linq method in a query
    /// </summary>
    public class LinqExceptExpression : QueryExpression
    {
        internal LinqExceptExpression(QueryExpression outer, QueryExpression inner, QueryType type)
            : base(type)
        {
            this.Outer = outer;
            this.Inner = inner;
        }

        /// <summary>
        /// Gets the first input collection to the expression.
        /// </summary>
        public QueryExpression Outer { get; private set; }

        /// <summary>
        /// Gets the second input collection to the expression.
        /// </summary>
        public QueryExpression Inner { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.Except({1})", this.Outer, this.Inner);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression</param>
        /// <returns>The result of visiting this expression</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return ((ILinqExpressionVisitor<TResult>)visitor).Visit(this);
        }
    }
}
