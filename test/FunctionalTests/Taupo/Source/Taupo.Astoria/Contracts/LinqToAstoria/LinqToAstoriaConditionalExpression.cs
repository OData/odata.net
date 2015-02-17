//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaConditionalExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Expression node representing the ternary operator expression in a Linq query.
    /// e.g.: condition ? true : false
    /// </summary>
    public class LinqToAstoriaConditionalExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the LinqToAstoriaConditionalExpression class.
        /// </summary>
        /// <param name="testCondition">The condition to check for.</param>
        /// <param name="ifTrue">The value if the condition is true.</param>
        /// <param name="ifFalse">The value if the condition is false.</param>
        /// <param name="type">The type of the expression's result.</param>
        internal LinqToAstoriaConditionalExpression(QueryExpression testCondition, QueryExpression ifTrue, QueryExpression ifFalse, QueryType type)
            : base(type)
        {
            this.Condition = testCondition;
            this.IfTrue = ifTrue;
            this.IfFalse = ifFalse;
        }

        /// <summary>
        /// Gets the condition of the expression.
        /// </summary>
        public QueryExpression Condition { get; private set; }

        /// <summary>
        /// Gets the expression for the true side of the conditional.
        /// </summary>
        public QueryExpression IfTrue { get; private set; }

        /// <summary>
        /// Gets the expression for the false side of the conditional.
        /// </summary>
        public QueryExpression IfFalse { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            string conditionString = string.Join("{0} ? {1} : {2}", this.Condition.ToString(), this.IfTrue.ToString(), this.IfFalse.ToString());
            return conditionString;
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
