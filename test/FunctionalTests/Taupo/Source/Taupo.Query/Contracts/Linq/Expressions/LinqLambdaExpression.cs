//---------------------------------------------------------------------
// <copyright file="LinqLambdaExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Expression node representing the Lambda expression in a Linq query.
    /// </summary>
    public class LinqLambdaExpression : QueryExpression
    {
        internal LinqLambdaExpression(QueryExpression body, IEnumerable<LinqParameterExpression> parameters, QueryType type)
            : base(type)
        {
            this.Body = body;
            this.Parameters = parameters.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the body of the lambda expression.
        /// </summary>
        public QueryExpression Body { get; private set; }

        /// <summary>
        /// Gets the read-only collection of parameter for the lambda expression.
        /// </summary>
        public ReadOnlyCollection<LinqParameterExpression> Parameters { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}) => {1}", string.Join(", ", this.Parameters.Select(p => p.ToString()).ToArray()), this.Body);
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
