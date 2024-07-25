//---------------------------------------------------------------------
// <copyright file="ApplyClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents the set of transformations to perform as part of $apply.
    /// </summary>
    public sealed class ApplyClause
    {
        private readonly IEnumerable<TransformationNode> transformations;

        private readonly IEnumerable<AggregateExpressionBase> lastAggregateExpressions;

        private readonly IEnumerable<GroupByPropertyNode> lastGroupByPropertyNodes;

        private readonly List<ComputeExpression> lastComputeExpressions;

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
                    lastAggregateExpressions = (transformations[i] as AggregateTransformationNode).AggregateExpressions;
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
                        lastAggregateExpressions = (childTransformation as AggregateTransformationNode).AggregateExpressions;
                    }

                    break;
                }
                else if (transformations[i].Kind == TransformationNodeKind.Compute)
                {
                    lastComputeExpressions = lastComputeExpressions ?? new List<ComputeExpression>();
                    lastComputeExpressions.AddRange((transformations[i] as ComputeTransformationNode).Expressions);
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
            IEnumerable<ComputeExpression> computeExpressions = transformations.OfType<ComputeTransformationNode>().SelectMany(n => n.Expressions);
            return CreatePropertiesUriSegment(lastGroupByPropertyNodes, lastAggregateExpressions, computeExpressions);
        }

        internal HashSet<EndPathToken> GetLastAggregatedPropertyNames()
        {
            if (lastAggregateExpressions == null && lastComputeExpressions == null && lastGroupByPropertyNodes == null)
            {
                return null;
            }

            HashSet<EndPathToken> result = new HashSet<EndPathToken>();
            if (lastAggregateExpressions != null)
            {
                result.UnionWith(lastAggregateExpressions.Select(statement => new EndPathToken(statement.Alias, null)));
            }

            if (lastComputeExpressions != null)
            {
                result.UnionWith(lastComputeExpressions.Select(statement => new EndPathToken(statement.Alias, null)));
            }

            if (lastGroupByPropertyNodes != null)
            {
                result.UnionWith(GetGroupByPaths(lastGroupByPropertyNodes, null));
            }

            return result;
        }

        private static IEnumerable<EndPathToken> GetGroupByPaths(IEnumerable<GroupByPropertyNode> nodes, EndPathToken token)
        {
            foreach (var node in nodes)
            {
                var nodeToken = new EndPathToken(node.Name, token);
                if (node.ChildTransformations == null || !node.ChildTransformations.Any())
                {
                    yield return nodeToken;
                }
                else
                {
                    foreach (var child in GetGroupByPaths(node.ChildTransformations, nodeToken))
                    {
                        yield return child;
                    }
                }
            }
        }

        private static string CreatePropertiesUriSegment(
            IEnumerable<GroupByPropertyNode> groupByPropertyNodes,
            IEnumerable<AggregateExpressionBase> aggregateExpressions,
            IEnumerable<ComputeExpression> computeExpressions)
        {
            Func<GroupByPropertyNode, string> func = (prop) =>
           {
               var children = CreatePropertiesUriSegment(prop.ChildTransformations, null, null);

               return string.IsNullOrEmpty(children)
                      ? prop.Name
                      : prop.Name + ODataConstants.ContextUriProjectionStart + children + ODataConstants.ContextUriProjectionEnd;
           };

            string result = string.Empty;
            if (groupByPropertyNodes != null)
            {
                var groupByPropertyArray =
                    groupByPropertyNodes.Select(prop => func(prop))
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

            if (computeExpressions != null && !string.IsNullOrEmpty(result) /* don't add compute if only compute() is present */)
            {
                string computeProperties = string.Join(",", computeExpressions.Select(e => e.Alias).ToArray());
                if (!string.IsNullOrEmpty(computeProperties))
                {
                    result = string.Format(CultureInfo.InvariantCulture, "{0},{1}", result, computeProperties);
                }
            }

            return result;
        }

        private static string CreateAggregatePropertiesUriSegment(IEnumerable<AggregateExpressionBase> aggregateExpressions)
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
