//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaAddQueryOptionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Expression node representing the Expand expression in a Linq query.
    /// </summary>
    public class LinqToAstoriaAddQueryOptionExpression : LinqQueryMethodExpression
    {
        internal LinqToAstoriaAddQueryOptionExpression(QueryExpression source, string queryOption, object queryValue, QueryType type)
            : base(source, type)
        {
            this.QueryOption = queryOption;
            this.QueryValue = queryValue;
        }

        /// <summary>
        /// Gets the Query option
        /// </summary>
        public string QueryOption { get; private set; }

        /// <summary>
        /// Gets the Query value
        /// </summary>
        public object QueryValue { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.AddQueryOption({1}, {2})", this.Source, this.QueryOption, this.QueryValue);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return ((ILinqToAstoriaExpressionVisitor<TResult>)visitor).Visit(this);
        }
    }
}
