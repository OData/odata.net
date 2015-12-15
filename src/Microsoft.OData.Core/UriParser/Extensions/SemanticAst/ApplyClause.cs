//---------------------------------------------------------------------
// <copyright file="ApplyClause2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// Represents the set of transformations to perform as part of $apply.
    /// </summary>
    public sealed class ApplyClause
    {
        private readonly IEnumerable<TransformationNode> _transformations;

        private readonly IEnumerable<AggregateStatement> _lastAggregateStatements;

        private readonly IEnumerable<GroupByPropertyNode> _lastGroupByPropertyNodes;

        public ApplyClause(IEnumerable<TransformationNode> transformations)
        {
            ExceptionUtils.CheckArgumentNotNull(transformations, "transformations");

            this._transformations = transformations;
        }

        public ApplyClause(
            IEnumerable<TransformationNode> transformations, 
            IEnumerable<AggregateStatement> aggregateStatements, 
            IEnumerable<GroupByPropertyNode> groupByPropertyNodes) 
            :this(transformations)
        {
            this._lastAggregateStatements = aggregateStatements;
            this._lastGroupByPropertyNodes = groupByPropertyNodes;
        }

        /// <summary>
        /// The collection of transformations to perform.
        /// </summary>
        public IEnumerable<TransformationNode> Transformations
        {
            get
            {
                return this._transformations;
            }
        }

        internal string GetContextUri()
        {
            return CreatePropertiesUriSegment(_lastGroupByPropertyNodes, _lastAggregateStatements);
        }

        private string CreatePropertiesUriSegment(IEnumerable<GroupByPropertyNode> groupByPropertyNodes, IEnumerable<AggregateStatement> aggregateStatements)
        {
            string result = string.Empty;
            if (groupByPropertyNodes != null)
            {
                result = string.Join(",",
                    groupByPropertyNodes
                        .Select(prop => prop.Name + CreatePropertiesUriSegment(prop.Children, null))
                        .ToArray());
                result = aggregateStatements == null
                    ? result
                    : string.Format("{0},{1}", result, CreateAggregatePropertiesUriSegment(aggregateStatements));
            }
            else
            {
                result = aggregateStatements == null
                    ? string.Empty
                    : CreateAggregatePropertiesUriSegment(aggregateStatements);
            }

            return string.IsNullOrEmpty(result)
                ? result
                : ODataConstants.ContextUriProjectionStart + result + ODataConstants.ContextUriProjectionEnd;
        }

        private string CreateAggregatePropertiesUriSegment(IEnumerable<AggregateStatement> aggregateStatements)
        {
            if (aggregateStatements != null)
            {
                return string.Join(",", aggregateStatements.Select(statement => statement.AsAlias).ToArray());
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
