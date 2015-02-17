//---------------------------------------------------------------------
// <copyright file="QueryCastExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Expression node representing a Singleton Cast expression in a query
    /// </summary>
    public class QueryCastExpression : QueryTypeOperationExpression
    {
        internal QueryCastExpression(QueryExpression source, QueryType typeToCastTo)
            : base(source, typeToCastTo, typeToCastTo)
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
            return "cast(" + this.Source + " as " + this.TypeToOperateAgainst.StringRepresentation + ")";
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
