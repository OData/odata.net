//---------------------------------------------------------------------
// <copyright file="QueryTypeOperationExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.CommonExpressions
{
    /// <summary>
    /// Expression node representing an Abstract Type expression in a Query
    /// </summary>
    public abstract class QueryTypeOperationExpression : QueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the QueryTypeOperationExpression class.
        /// </summary>
        /// <param name="source">The root argument of the expression.</param>
        /// <param name="typeToOperateAgainst">The type to covert to.</param>
        /// <param name="type">The type of expression.</param>
        protected QueryTypeOperationExpression(QueryExpression source, QueryType typeToOperateAgainst, QueryType type)
            : base(type)
        {
            this.Source = source;
            this.TypeToOperateAgainst = typeToOperateAgainst;
        }

        /// <summary>
        /// Gets the argument operand of the Type expression
        /// </summary>
        public QueryExpression Source { get; private set; }

        /// <summary>
        /// Gets the type operand of the Type expression
        /// </summary>
        public QueryType TypeToOperateAgainst { get; private set; }
    }
}
