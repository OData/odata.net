//---------------------------------------------------------------------
// <copyright file="LinqBuiltInFunctionCallExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions
{
    using System.Collections.ObjectModel;
    using System.Linq;
    
    /// <summary>
    /// Expression node that represents linq canonical or provider built-in function call expression.
    /// </summary>
    public class LinqBuiltInFunctionCallExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the LinqBuiltInFunctionCallExpression class.
        /// </summary>
        /// <param name="resultType">The result type.</param>
        /// <param name="linqBuiltInFunction">The built-in function.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        internal LinqBuiltInFunctionCallExpression(QueryType resultType, LinqBuiltInFunction linqBuiltInFunction, params QueryExpression[] arguments)
            : base(resultType)
        {
            this.Arguments = new ReadOnlyCollection<QueryExpression>(arguments.ToList());
            this.LinqBuiltInFunction = linqBuiltInFunction;
        }

        /// <summary>
        /// Gets built-in function.
        /// </summary>
        public LinqBuiltInFunction LinqBuiltInFunction { get; private set; }

        /// <summary>
        /// Gets the arguments for the function call.
        /// </summary>
        public ReadOnlyCollection<QueryExpression> Arguments { get; private set; }

        /// <summary>
        /// Returns string that represents the expression.
        /// </summary>
        /// <returns>String representation for the expression.</returns>
        /// <remarks>This functionality is for debug purposes only.</remarks>
        public override string ToString()
        {
            return this.LinqBuiltInFunction.ClassName + "." + LinqBuiltInFunction.MethodName
                + "("
                + string.Join(", ", this.Arguments.Select(x => x.ToString()).ToArray())
                + ")";
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
