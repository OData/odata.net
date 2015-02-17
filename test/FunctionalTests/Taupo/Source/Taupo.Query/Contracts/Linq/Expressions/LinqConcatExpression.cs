//---------------------------------------------------------------------
// <copyright file="LinqConcatExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Globalization;

    /// <summary>
    /// Expression node representing the Concat expression in a Linq query.
    /// </summary>
    public class LinqConcatExpression : QueryExpression
    {
        internal LinqConcatExpression(QueryExpression outer, QueryExpression inner, QueryType type)
            : base(type)
        {
            this.Outer = outer;
            this.Inner = inner;
        }

        /// <summary>
        /// Gets the outer input collection to the expression.
        /// </summary>
        public QueryExpression Outer { get; private set; }

        /// <summary>
        /// Gets the inner input collection to the expression.
        /// </summary>
        public QueryExpression Inner { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.Concat({1})", this.Outer, this.Inner);
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
