//---------------------------------------------------------------------
// <copyright file="QueryConstantExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    using System.Globalization;

    /// <summary>
    /// Expression node representing a constant expression in queries.
    /// </summary>
    public class QueryConstantExpression : QueryExpression
    {
        internal QueryConstantExpression(QueryScalarValue primitiveValue)
            : base(primitiveValue.Type)
        {
            this.ScalarValue = primitiveValue;
        }

        /// <summary>
        /// Gets the value of the constant expression.
        /// </summary>
        public QueryScalarValue ScalarValue { get; private set; }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public new QueryScalarType ExpressionType
        {
            get { return this.ScalarValue.Type; }
        }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", this.ScalarValue.Value);
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
