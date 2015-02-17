//---------------------------------------------------------------------
// <copyright file="QueryNullExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    /// <summary>
    /// Expression node representing a Null value in a query.
    /// </summary>
    public class QueryNullExpression : QueryExpression
    {
        internal QueryNullExpression(QueryType type)
            : base(type)
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
            return "null";
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression</param>
        /// <returns>The result of visiting this expression</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
