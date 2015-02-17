//---------------------------------------------------------------------
// <copyright file="QueryPropertyExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    using System.Globalization;

    /// <summary>
    /// Expression node representing the property expression in a query.
    /// </summary>
    public class QueryPropertyExpression : QueryExpression
    {
        internal QueryPropertyExpression(QueryExpression instance, string name, QueryType type)
            : base(type)
        {
            this.Instance = instance;
            this.Name = name;
        }

        /// <summary>
        /// Gets the instance that declares the property.
        /// </summary>
        public QueryExpression Instance { get; private set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", this.Instance, this.Name);
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