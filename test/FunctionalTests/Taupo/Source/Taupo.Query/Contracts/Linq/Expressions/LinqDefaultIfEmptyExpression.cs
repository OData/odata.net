//---------------------------------------------------------------------
// <copyright file="LinqDefaultIfEmptyExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Globalization;

    /// <summary>
    /// Expression node representing the DefaultIfEmpty expression in a Linq query.
    /// </summary>
    public class LinqDefaultIfEmptyExpression : LinqQueryMethodExpression
    {
        internal LinqDefaultIfEmptyExpression(QueryExpression source, QueryType type) 
            : this(source, type, null)
        {
        }

        internal LinqDefaultIfEmptyExpression(QueryExpression source, QueryType type, QueryExpression defaultValue)
            : base(source, type)
        {
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets the value to return if the source collection is empty.
        /// </summary>
        public QueryExpression DefaultValue { get; set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            var defaultValue = this.DefaultValue == null ? string.Empty : this.DefaultValue.ToString();

            return string.Format(CultureInfo.InvariantCulture, "{0}.DefaultIfEmpty({1})", this.Source, defaultValue);
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
