//---------------------------------------------------------------------
// <copyright file="LinqBitwiseAndExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;

    /// <summary>
    /// Expression node representing the BitwiseAnd expression in queries.
    /// </summary>
    public class LinqBitwiseAndExpression : QueryBinaryExpression
    {
        internal LinqBitwiseAndExpression(QueryExpression left, QueryExpression right, QueryType type)
            : base(left, right, type)
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "(" + this.Left + " & " + this.Right + ")";
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
