//---------------------------------------------------------------------
// <copyright file="LinqJoinExpressionBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    /// <summary>
    /// Base class for Linq Join expression
    /// </summary>
    public abstract class LinqJoinExpressionBase : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the LinqJoinExpressionBase class.
        /// </summary>
        /// <param name="outer">First input collection.</param>
        /// <param name="inner">Second input collection.</param>
        /// <param name="outerKeySelector">First key selector to the join condition.</param>
        /// <param name="innerKeySelector">Second key selector to the join condition.</param>
        /// <param name="resultSelector">Result selector</param>
        /// <param name="type">The query type of the expression</param>
        protected LinqJoinExpressionBase(
            QueryExpression outer, 
            QueryExpression inner, 
            LinqLambdaExpression outerKeySelector, 
            LinqLambdaExpression innerKeySelector, 
            LinqLambdaExpression resultSelector, 
            QueryType type)
            : base(type)
        {
            this.Outer = outer;
            this.Inner = inner;
            this.OuterKeySelector = outerKeySelector;
            this.InnerKeySelector = innerKeySelector;
            this.ResultSelector = resultSelector;
        }

        /// <summary>
        /// Gets the first input collection to the join.
        /// </summary>
        public QueryExpression Outer { get; private set; }

        /// <summary>
        /// Gets the second input collection to the join.
        /// </summary>
        public QueryExpression Inner { get; private set; }

        /// <summary>
        /// Gets the first key selector for the join condition
        /// </summary>
        public LinqLambdaExpression OuterKeySelector { get; private set; }

        /// <summary>
        /// Gets the second key selector for the join condition
        /// </summary>
        public LinqLambdaExpression InnerKeySelector { get; private set; }

        /// <summary>
        /// Gets the result selector
        /// </summary>
        public LinqLambdaExpression ResultSelector { get; private set; }
    }
}
