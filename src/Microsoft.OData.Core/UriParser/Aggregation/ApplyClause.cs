//---------------------------------------------------------------------
// <copyright file="ApplyClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Aggregation
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents the set of transformations to perform as part of $apply.
    /// </summary>
    public sealed class ApplyClause
    {
        private readonly IEnumerable<TransformationNode> transformations;

        private readonly IEnumerable<AggregateExpression> lastAggregateExpressions;

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
                    lastAggregateExpressions = (transformations[i] as AggregateTransformationNode).Expressions;
                    break;
                }
                else if (transformations[i].Kind == TransformationNodeKind.GroupBy)
                {
                    var groupByTransformationNode =
                        transformations[i] as GroupByTransformationNode;
                    lastGroupByPropertyNodes = groupByTransformationNode.GroupingProperties;
                    var childTransformation = groupByTransformationNode.ChildTransformations;
                    if (childTransformation != null && childTransformation.Kind == TransformationNodeKind.Aggregate)
                    {
                        lastAggregateExpressions = (childTransformation as AggregateTransformationNode).Expressions;
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
            return CreatePropertiesUriSegment(lastGroupByPropertyNodes, lastAggregateExpressions);
        }

        internal List<string> GetLastAggregatedPropertyNames()
        {
            if (lastAggregateExpressions != null)
            {
                return lastAggregateExpressions.Select(statement => statement.Alias).ToList();
            }

            return null;
        }

        private string CreatePropertiesUriSegment(
            IEnumerable<GroupByPropertyNode> groupByPropertyNodes, 
            IEnumerable<AggregateExpression> aggregateExpressions)
        {
            string result = string.Empty;
            if (groupByPropertyNodes != null)
            {
                var groupByPropertyArray =
                    groupByPropertyNodes.Select(prop => prop.Name + CreatePropertiesUriSegment(prop.ChildTransformations, null))
                        .ToArray();
                result = string.Join(",", groupByPropertyArray);
                result = aggregateExpressions == null
                    ? result
                    : string.Format(CultureInfo.InvariantCulture, "{0},{1}", result, CreateAggregatePropertiesUriSegment(aggregateExpressions));
            }
            else
            {
                result = aggregateExpressions == null
                    ? string.Empty
                    : CreateAggregatePropertiesUriSegment(aggregateExpressions);
            }

            return string.IsNullOrEmpty(result)
                ? result
                : ODataConstants.ContextUriProjectionStart + result + ODataConstants.ContextUriProjectionEnd;
        }

        private static string CreateAggregatePropertiesUriSegment(IEnumerable<AggregateExpression> aggregateExpressions)
        {
            if (aggregateExpressions != null)
            {
                return string.Join(",", aggregateExpressions.Select(statement => statement.Alias).ToArray());
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
