//---------------------------------------------------------------------
// <copyright file="ApplyClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Extensions.TreeNodeKinds;

    /// <summary>
    /// Represents the set of transformations to perform as part of $apply.
    /// </summary>
    public sealed class ApplyClause
    {
        private readonly IEnumerable<TransformationNode> transformations;

        private readonly IEnumerable<AggregateStatement> lastAggregateStatements;

        private readonly IEnumerable<GroupByPropertyNode> lastGroupByPropertyNodes;

        /// <summary>
        /// Create a ApplyClause.
        /// </summary>
        /// <param name="transformations">The <see cref="TransformationNode"/>s.</param>
        public ApplyClause(IList<TransformationNode> transformations)
        {
            ExceptionUtils.CheckArgumentNotNull(transformations, "transformations");

            this.transformations = transformations;

            for (int i = transformations.Count - 1; i >= 0; i--)
            {
                if (transformations[i].Kind == TransformationNodeKind.Aggregate)
                {
                    lastAggregateStatements = (transformations[i] as AggregateTransformationNode).Statements;
                    break;
                }
                else if (transformations[i].Kind == TransformationNodeKind.GroupBy)
                {
                    var groupByTransformationNode =
                        transformations[i] as GroupByTransformationNode;
                    lastGroupByPropertyNodes = groupByTransformationNode.GroupingProperties;
                    var childTransformation = groupByTransformationNode.ChildTransformation;
                    if (childTransformation != null && childTransformation.Kind == TransformationNodeKind.Aggregate)
                    {
                        lastAggregateStatements = (childTransformation as AggregateTransformationNode).Statements;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// The collection of transformations to perform.
        /// </summary>
        public IEnumerable<TransformationNode> Transformations
        {
            get
            {
                return this.transformations;
            }
        }

        internal string GetContextUri()
        {
            return CreatePropertiesUriSegment(lastGroupByPropertyNodes, lastAggregateStatements);
        }

        internal List<string> GetLastAggregatedPropertyNames()
        {
            if (lastAggregateStatements != null)
            {
                return lastAggregateStatements.Select(statement => statement.AsAlias).ToList();
            }

            return null;
        }

        private string CreatePropertiesUriSegment(
            IEnumerable<GroupByPropertyNode> groupByPropertyNodes, 
            IEnumerable<AggregateStatement> aggregateStatements)
        {
            string result = string.Empty;
            if (groupByPropertyNodes != null)
            {
                var groupByPropertyArray =
                    groupByPropertyNodes.Select(prop => prop.Name + CreatePropertiesUriSegment(prop.Children, null))
                        .ToArray();
                result = string.Join(",", groupByPropertyArray);
                result = aggregateStatements == null
                    ? result
                    : string.Format(CultureInfo.InvariantCulture, "{0},{1}", result, CreateAggregatePropertiesUriSegment(aggregateStatements));
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

        private static string CreateAggregatePropertiesUriSegment(IEnumerable<AggregateStatement> aggregateStatements)
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
