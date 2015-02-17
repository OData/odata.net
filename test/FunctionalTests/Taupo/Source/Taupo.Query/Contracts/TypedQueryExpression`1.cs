//---------------------------------------------------------------------
// <copyright file="TypedQueryExpression`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Helper class to improve filtering of root queries based on type.
    /// </summary>
    /// <typeparam name="TQueryType">Type of the query type.</typeparam>
    public class TypedQueryExpression<TQueryType>
        where TQueryType : QueryType
    {
        /// <summary>
        /// Initializes a new instance of the TypedQueryExpression class.
        /// </summary>
        /// <param name="expression">Wrapped query expression, whose type must be TQueryType.</param>
        public TypedQueryExpression(QueryExpression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");

            this.Expression = expression;
            this.ExpressionType = (TQueryType)expression.ExpressionType;
        }

        /// <summary>
        /// Gets the type of the query expression.
        /// </summary>
        public TQueryType ExpressionType { get; private set; }

        /// <summary>
        /// Gets the wrapped expression.
        /// </summary>
        public QueryExpression Expression { get; private set; }
    }
}
