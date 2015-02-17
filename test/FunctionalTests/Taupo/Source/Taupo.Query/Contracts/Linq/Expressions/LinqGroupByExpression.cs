//---------------------------------------------------------------------
// <copyright file="LinqGroupByExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Expression node representing the GroupBy expression in a Linq query.
    /// </summary>
    public class LinqGroupByExpression : LinqQueryMethodExpression
    {
        internal LinqGroupByExpression(
            QueryExpression source, 
            LinqLambdaExpression keySelector,
            LinqLambdaExpression elementSelector,
            LinqLambdaExpression resultSelector,
            QueryType type)
            : base(source, type)
        {
            this.KeySelector = keySelector;
            this.ElementSelector = elementSelector;
            this.ResultSelector = resultSelector;
        }

        /// <summary>
        /// Gets the lambda expression that selects keys to use in the grouping
        /// </summary>
        public LinqLambdaExpression KeySelector { get; private set; }

        /// <summary>
        /// Gets the lambda expression that selects elements to use in the grouping
        /// </summary>
        public LinqLambdaExpression ElementSelector { get; private set; }

        /// <summary>
        /// Gets the lambda expression that specifies result of the grouping
        /// </summary>
        public LinqLambdaExpression ResultSelector { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            var arguments = new List<string>();
            arguments.Add(this.KeySelector.ToString());

            if (this.ElementSelector != null)
            {
                arguments.Add(this.ElementSelector.ToString());
            }

            if (this.ResultSelector != null)
            {
                arguments.Add(this.ResultSelector.ToString());
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}.GroupBy({1})", this.Source, string.Join(", ", arguments));
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
