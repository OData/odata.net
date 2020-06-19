//---------------------------------------------------------------------
// <copyright file="ApplyBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser.Aggregation
{
    internal sealed class ApplyBinder
    {
        private MetadataBinder.QueryTokenVisitor bindMethod;

        private BindingState state;

        private FilterBinder filterBinder;
        private ODataUriParserConfiguration configuration;
        private ODataPathInfo odataPathInfo;

        private IEnumerable<AggregateExpressionBase> aggregateExpressionsCache;

        public ApplyBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
            : this(bindMethod, state, null, null)
        {
        }

        public ApplyBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state, ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo)
        {
            this.bindMethod = bindMethod;
            this.state = state;
            this.filterBinder = new FilterBinder(bindMethod, state);
            this.configuration = configuration;
            this.odataPathInfo = odataPathInfo;
        }

        public ApplyClause BindApply(IEnumerable<QueryToken> tokens)
        {
            ExceptionUtils.CheckArgumentNotNull(tokens, "tokens");

            List<TransformationNode> transformations = new List<TransformationNode>();
            foreach (QueryToken token in tokens)
            {
                switch (token.Kind)
                {
                    case QueryTokenKind.Aggregate:
                        AggregateTransformationNode aggregate = BindAggregateToken((AggregateToken)(token));
                        transformations.Add(aggregate);
                        aggregateExpressionsCache = aggregate.AggregateExpressions;
                        state.AggregatedPropertyNames = new HashSet<EndPathToken>(aggregate.AggregateExpressions.Select(statement => new EndPathToken(statement.Alias, null)));
                        state.IsCollapsed = true;
                        break;
                    case QueryTokenKind.AggregateGroupBy:
                        GroupByTransformationNode groupBy = BindGroupByToken((GroupByToken)(token));
                        transformations.Add(groupBy);
                        state.IsCollapsed = true;
                        break;
                    case QueryTokenKind.Compute:
                        var compute = BindComputeToken((ComputeToken)token);
                        transformations.Add(compute);
                        state.AggregatedPropertyNames = new HashSet<EndPathToken>(compute.Expressions.Select(statement => new EndPathToken(statement.Alias, null)));
                        break;
                    case QueryTokenKind.Expand:
                        SelectExpandClause expandClause = SelectExpandSemanticBinder.Bind(this.odataPathInfo, (ExpandToken)token, null, this.configuration, null);
                        ExpandTransformationNode expandNode = new ExpandTransformationNode(expandClause);
                        transformations.Add(expandNode);
                        break;
                    default:
                        FilterClause filterClause = this.filterBinder.BindFilter(token);
                        FilterTransformationNode filterNode = new FilterTransformationNode(filterClause);
                        transformations.Add(filterNode);
                        break;
                }
            }

            return new ApplyClause(transformations);
        }

        private AggregateTransformationNode BindAggregateToken(AggregateToken token)
        {
            IEnumerable<AggregateTokenBase> aggregateTokens = MergeEntitySetAggregates(token.AggregateExpressions);
            List<AggregateExpressionBase> statements = new List<AggregateExpressionBase>();

            foreach (AggregateTokenBase statementToken in aggregateTokens)
            {
                statements.Add(BindAggregateExpressionToken(statementToken));
            }

            return new AggregateTransformationNode(statements);
        }

        private static IEnumerable<AggregateTokenBase> MergeEntitySetAggregates(IEnumerable<AggregateTokenBase> tokens)
        {
            List<AggregateTokenBase> mergedTokens = new List<AggregateTokenBase>();
            Dictionary<string, AggregateTokenBase> entitySetTokens = new Dictionary<string, AggregateTokenBase>();

            foreach (AggregateTokenBase token in tokens)
            {
                switch (token.Kind)
                {
                    case QueryTokenKind.EntitySetAggregateExpression:
                        {
                            AggregateTokenBase currentValue;
                            EntitySetAggregateToken entitySetToken = token as EntitySetAggregateToken;
                            string key = entitySetToken.Path();

                            if (entitySetTokens.TryGetValue(key, out currentValue))
                            {
                                entitySetTokens.Remove(key);
                            }

                            entitySetTokens.Add(key, EntitySetAggregateToken.Merge(entitySetToken, currentValue as EntitySetAggregateToken));
                            break;
                        }

                    case QueryTokenKind.AggregateExpression:
                        {
                            mergedTokens.Add(token);
                            break;
                        }
                }
            }

            return mergedTokens.Concat(entitySetTokens.Values).ToList();
        }

        private AggregateExpressionBase BindAggregateExpressionToken(AggregateTokenBase aggregateToken)
        {
            switch (aggregateToken.Kind)
            {
                case QueryTokenKind.AggregateExpression:
                    {
                        AggregateExpressionToken token = aggregateToken as AggregateExpressionToken;
                        SingleValueNode expression = this.bindMethod(token.Expression) as SingleValueNode;
                        IEdmTypeReference typeReference = CreateAggregateExpressionTypeReference(expression, token.MethodDefinition);

                        // TODO: Determine source
                        return new AggregateExpression(expression, token.MethodDefinition, token.Alias, typeReference);
                    }

                case QueryTokenKind.EntitySetAggregateExpression:
                    {
                        EntitySetAggregateToken token = aggregateToken as EntitySetAggregateToken;
                        QueryNode boundPath = this.bindMethod(token.EntitySet);

                        state.InEntitySetAggregation = true;
                        IEnumerable<AggregateExpressionBase> children = token.Expressions.Select(x => BindAggregateExpressionToken(x)).ToList();
                        state.InEntitySetAggregation = false;
                        return new EntitySetAggregateExpression((CollectionNavigationNode)boundPath, children);
                    }

                default:
                    throw new ODataException(ODataErrorStrings.ApplyBinder_UnsupportedAggregateKind(aggregateToken.Kind));
            }
        }

        private IEdmTypeReference CreateAggregateExpressionTypeReference(SingleValueNode expression, AggregationMethodDefinition method)
        {
            IEdmTypeReference expressionType = expression.TypeReference;
            if (expressionType == null && aggregateExpressionsCache != null)
            {
                SingleValueOpenPropertyAccessNode openProperty = expression as SingleValueOpenPropertyAccessNode;
                if (openProperty != null)
                {
                    expressionType = GetTypeReferenceByPropertyName(openProperty.Name);
                }
            }

            switch (method.MethodKind)
            {
                case AggregationMethod.Average:
                    EdmPrimitiveTypeKind expressionPrimitiveKind = expressionType.PrimitiveKind();
                    switch (expressionPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.Double:
                            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, expressionType.IsNullable);
                        case EdmPrimitiveTypeKind.Decimal:
                            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Decimal, expressionType.IsNullable);
                        case EdmPrimitiveTypeKind.Single:
                            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Single, expressionType.IsNullable);
                        case EdmPrimitiveTypeKind.None:
                            return expressionType;
                        default:
                            throw new ODataException(
                                ODataErrorStrings.ApplyBinder_AggregateExpressionIncompatibleTypeForMethod(expression,
                                    expressionPrimitiveKind));
                    }

                case AggregationMethod.VirtualPropertyCount:
                case AggregationMethod.CountDistinct:
                    // Issue #758: CountDistinct and $Count should return type Edm.Decimal with Scale="0" and sufficient Precision.
                    return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int64, false);
                case AggregationMethod.Max:
                case AggregationMethod.Min:
                case AggregationMethod.Sum:
                    return expressionType;
                default:
                    // Only the EdmModel knows which type the custom aggregation methods returns.
                    // Since we do not have a reference for it, right now we are assuming that all custom aggregation methods returns Doubles
                    // TODO: find a appropriate way of getting the return type.
                    return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, expressionType.IsNullable);
            }
        }

        private IEdmTypeReference GetTypeReferenceByPropertyName(string name)
        {
            if (aggregateExpressionsCache != null)
            {
                AggregateExpression expression = aggregateExpressionsCache.OfType<AggregateExpression>()
                    .FirstOrDefault(statement => statement.AggregateKind == AggregateExpressionKind.PropertyAggregate && statement.Alias.Equals(name, StringComparison.Ordinal));
                if (expression != null)
                {
                    return expression.TypeReference;
                }
            }

            return null;
        }

        private GroupByTransformationNode BindGroupByToken(GroupByToken token)
        {
            List<GroupByPropertyNode> properties = new List<GroupByPropertyNode>();

            foreach (EndPathToken propertyToken in token.Properties)
            {
                QueryNode bindResult = this.bindMethod(propertyToken);
                SingleValuePropertyAccessNode property = bindResult as SingleValuePropertyAccessNode;
                SingleComplexNode complexProperty = bindResult as SingleComplexNode;

                if (property != null)
                {
                    RegisterProperty(properties, ReversePropertyPath(property));
                }
                else if (complexProperty != null)
                {
                    RegisterProperty(properties, ReversePropertyPath(complexProperty));
                }
                else
                {
                    SingleValueOpenPropertyAccessNode openProperty = bindResult as SingleValueOpenPropertyAccessNode;
                    if (openProperty != null)
                    {
                        IEdmTypeReference type = GetTypeReferenceByPropertyName(openProperty.Name);
                        properties.Add(new GroupByPropertyNode(openProperty.Name, openProperty, type));
                    }
                    else
                    {
                        throw new ODataException(
                            ODataErrorStrings.ApplyBinder_GroupByPropertyNotPropertyAccessValue(propertyToken.Identifier));
                    }
                }
            }

            var newProperties = new HashSet<EndPathToken>(((GroupByToken)token).Properties);

            TransformationNode aggregate = null;
            if (token.Child != null)
            {
                if (token.Child.Kind == QueryTokenKind.Aggregate)
                {
                    aggregate = BindAggregateToken((AggregateToken)token.Child);
                    aggregateExpressionsCache = ((AggregateTransformationNode)aggregate).AggregateExpressions;
                    newProperties.UnionWith(aggregateExpressionsCache.Select(statement => new EndPathToken(statement.Alias, null)));
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.ApplyBinder_UnsupportedGroupByChild(token.Child.Kind));
                }
            }

            state.AggregatedPropertyNames = newProperties;

            // TODO: Determine source
            return new GroupByTransformationNode(properties, aggregate, null);
        }

        private static bool IsPropertyNode(SingleValueNode node)
        {
            return node.Kind == QueryNodeKind.SingleValuePropertyAccess ||
                   node.Kind == QueryNodeKind.SingleComplexNode ||
                   node.Kind == QueryNodeKind.SingleNavigationNode;
        }

        private static Stack<SingleValueNode> ReversePropertyPath(SingleValueNode node)
        {
            Stack<SingleValueNode> result = new Stack<SingleValueNode>();
            do
            {
                result.Push(node);
                if (node.Kind == QueryNodeKind.SingleValuePropertyAccess)
                {
                    node = ((SingleValuePropertyAccessNode)node).Source;
                }
                else if (node.Kind == QueryNodeKind.SingleComplexNode)
                {
                    node = (SingleValueNode)((SingleComplexNode)node).Source;
                }
                else if (node.Kind == QueryNodeKind.SingleNavigationNode)
                {
                    node = ((SingleNavigationNode)node).Source as SingleValueNode;
                }
            }
            while (node != null && IsPropertyNode(node));

            return result;
        }

        private static void RegisterProperty(IList<GroupByPropertyNode> properties, Stack<SingleValueNode> propertyStack)
        {
            SingleValueNode property = propertyStack.Pop();
            string propertyName = GetNodePropertyName(property);

            if (propertyStack.Count != 0)
            {
                // Not at the leaf, let's add to the container.
                GroupByPropertyNode containerProperty = properties.FirstOrDefault(p => p.Name == propertyName);
                if (containerProperty == null)
                {
                    // We do not have container yet. Create it.
                    containerProperty = new GroupByPropertyNode(propertyName, null);
                    properties.Add(containerProperty);
                }

                RegisterProperty(containerProperty.ChildTransformations, propertyStack);
            }
            else
            {
                // It's the leaf just add.
                properties.Add(new GroupByPropertyNode(propertyName, property, property.TypeReference));
            }
        }

        private static string GetNodePropertyName(SingleValueNode property)
        {
            if (property.Kind == QueryNodeKind.SingleValuePropertyAccess)
            {
                return ((SingleValuePropertyAccessNode)property).Property.Name;
            }
            else if (property.Kind == QueryNodeKind.SingleComplexNode)
            {
                return ((SingleComplexNode)property).Property.Name;
            }
            else if (property.Kind == QueryNodeKind.SingleNavigationNode)
            {
                return ((SingleNavigationNode)property).NavigationProperty.Name;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private ComputeTransformationNode BindComputeToken(ComputeToken token)
        {
            var statements = new List<ComputeExpression>();
            foreach (ComputeExpressionToken statementToken in token.Expressions)
            {
                var singleValueNode = (SingleValueNode)bindMethod(statementToken.Expression);
                statements.Add(new ComputeExpression(singleValueNode, statementToken.Alias, singleValueNode.TypeReference));
            }

            return new ComputeTransformationNode(statements);
        }
    }
}
