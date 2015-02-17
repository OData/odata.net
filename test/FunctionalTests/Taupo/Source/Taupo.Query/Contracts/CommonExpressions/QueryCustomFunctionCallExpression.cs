//---------------------------------------------------------------------
// <copyright file="QueryCustomFunctionCallExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Expression node representing a custom function call in a query.
    /// </summary>
    public class QueryCustomFunctionCallExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the QueryCustomFunctionCallExpression class.
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="function">The function to call.</param>
        /// <param name="functionBody">The query expression for the function body.</param>
        /// <param name="isRoot">The value indicating whether it's a root query.</param>
        /// <param name="calledByNameOnly">The value indicating whether the function should be called by name only.</param>
        /// <param name="arguments">Arguments for the function call.</param>
        public QueryCustomFunctionCallExpression(QueryType resultType, Function function, QueryExpression functionBody, bool isRoot, bool calledByNameOnly, params QueryExpression[] arguments)
            : base(resultType)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            ExceptionUtilities.CheckArgumentNotNull(arguments, "arguments");

            this.Function = function;
            this.FunctionBody = functionBody;
            this.Arguments = new ReadOnlyCollection<QueryExpression>(arguments.ToList());
            this.IsRoot = isRoot;
            this.IsCalledByNameOnly = calledByNameOnly;
        }

        /// <summary>
        /// Gets a value indicating whether it's a root expression.
        /// </summary>
        public bool IsRoot { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the function is called by name only without namespace qualifier.
        /// </summary>
        public bool IsCalledByNameOnly { get; private set; }

        /// <summary>
        /// Gets the function.
        /// </summary>
        public Function Function { get; private set; }

        /// <summary>
        /// Gets esql expression representing the function body.
        /// </summary>
        public QueryExpression FunctionBody { get; private set; }

        /// <summary>
        /// Gets the arguments for the function call.
        /// </summary>
        public ReadOnlyCollection<QueryExpression> Arguments { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string name = this.IsCalledByNameOnly ? this.Function.Name : this.Function.FullName;
            return name
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
            return visitor.Visit(this);
        }
    }
}
