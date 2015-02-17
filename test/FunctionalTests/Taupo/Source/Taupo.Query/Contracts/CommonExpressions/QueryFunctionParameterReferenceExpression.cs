//---------------------------------------------------------------------
// <copyright file="QueryFunctionParameterReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Expression node representing a Parameter Reference expression in a query
    /// </summary>
    public class QueryFunctionParameterReferenceExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the QueryFunctionParameterReferenceExpression class.
        /// </summary>
        /// <param name="parameterName">the name of the parameter</param>
        /// <param name="type">the query type of the parameter</param>
        public QueryFunctionParameterReferenceExpression(string parameterName, QueryType type)
            : base(type)
        {
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the name of the function parameter
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ParameterName;
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor</typeparam>
        /// <param name="visitor">The visitor that is visiting this expression</param>
        /// <returns>The result of visiting this expression</returns>
        public override TResult Accept<TResult>(ICommonExpressionVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
