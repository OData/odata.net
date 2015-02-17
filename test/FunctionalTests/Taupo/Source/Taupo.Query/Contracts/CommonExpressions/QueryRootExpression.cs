//---------------------------------------------------------------------
// <copyright file="QueryRootExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Expression node representing a root of a query.
    /// </summary>
    public class QueryRootExpression : QueryExpression
    {
        internal QueryRootExpression(string name, QueryType expressionType)
            : base(expressionType)
        {
            this.Name = name;
        }

        internal QueryRootExpression(EntitySet entitySet, QueryType expressionType)
            : base(expressionType)
        {
            this.Name = entitySet.Name;
            this.UnderlyingEntitySet = entitySet;
        }

        /// <summary>
        /// Gets the unqualified name of the source query.
        /// </summary>
        public string Name { get; private set; }

        internal EntitySet UnderlyingEntitySet { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Query({0})", this.Name);
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
