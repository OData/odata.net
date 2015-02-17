//---------------------------------------------------------------------
// <copyright file="QueryStreamType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a stream type in a QueryType hierarchy.
    /// </summary>
    public abstract class QueryStreamType : QueryType
    {
        /// <summary>
        /// Initializes a new instance of the QueryStreamType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        protected QueryStreamType(IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
        }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "StreamType";
            }
        }

        /// <summary>
        /// Determines whether the type can be assigned from another.
        /// </summary>
        /// <param name="queryType">Type to assign from.</param>
        /// <returns>True if assignment is possible, false otherwise.</returns>
        public override bool IsAssignableFrom(QueryType queryType)
        {
            return queryType is QueryStreamType;
        }
    }
}