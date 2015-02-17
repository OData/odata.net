//---------------------------------------------------------------------
// <copyright file="LinqSelectManyExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Expression node representing the SelectMany expression in a Linq query.
    /// </summary>
    public class LinqSelectManyExpression : LinqQueryMethodExpression
    {
        internal LinqSelectManyExpression(QueryExpression source, LinqLambdaExpression collectionSelector, LinqLambdaExpression resultSelector, QueryType type)
            : base(source, type)
        {
            this.CollectionSelector = collectionSelector;
            this.ResultSelector = resultSelector;
        }

        /// <summary>
        /// Gets the collection selector
        /// </summary>
        public LinqLambdaExpression CollectionSelector { get; private set; }

        /// <summary>
        /// Gets the result selector
        /// </summary>
        public LinqLambdaExpression ResultSelector { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            var argumentString = string.Join(", ", new List<LinqLambdaExpression>(new[] { this.CollectionSelector, this.ResultSelector }).Where(a => a != null));
            
            return string.Format(CultureInfo.InvariantCulture, "{0}.SelectMany({1})", this.Source, argumentString);
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
