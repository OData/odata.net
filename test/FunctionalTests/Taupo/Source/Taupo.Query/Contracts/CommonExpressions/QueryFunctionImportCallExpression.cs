//---------------------------------------------------------------------
// <copyright file="QueryFunctionImportCallExpression.cs" company="Microsoft">
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
    /// Expression node representing a composable function import call in a query.
    /// </summary>
    public class QueryFunctionImportCallExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the QueryFunctionImportCallExpression class.
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="functionImport">The function to call.</param>
        /// <param name="isRoot">The value indicating whether it's a root query.</param>
        /// <param name="arguments">Arguments for the function call.</param>
        public QueryFunctionImportCallExpression(QueryType resultType, FunctionImport functionImport, bool isRoot, params QueryExpression[] arguments)
            : base(resultType)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionImport, "functionImport");
            ExceptionUtilities.CheckArgumentNotNull(arguments, "arguments");
            ExceptionUtilities.Assert(functionImport.IsComposable, "Only composable function imports are allowed.");

            this.FunctionImport = functionImport;
            this.Arguments = new ReadOnlyCollection<QueryExpression>(arguments.ToList());
            this.IsRoot = isRoot;
        }

        /// <summary>
        /// Gets a value indicating whether it's a root expression.
        /// </summary>
        public bool IsRoot { get; private set; }

        /// <summary>
        /// Gets the function.
        /// </summary>
        public FunctionImport FunctionImport { get; private set; }

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
            return this.FunctionImport.Name + "(" + string.Join(", ", this.Arguments.Select(x => x.ToString()).ToArray()) + ")";
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
