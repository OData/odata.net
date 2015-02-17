//---------------------------------------------------------------------
// <copyright file="QueryNotExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    /// <summary>
    /// Expression node representing a Not expression in an Entity SQL query
    /// </summary>
    public class QueryNotExpression : QueryExpression
    {
        internal QueryNotExpression(QueryExpression argument, QueryType type)
            : base(type)
        {
            this.Argument = argument;
        }

        /// <summary>
        /// Gets the argument operand of the Not expression
        /// </summary>
        public QueryExpression Argument { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "(!" + this.Argument + ")";
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
