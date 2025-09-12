//---------------------------------------------------------------------
// <copyright file="ApplyBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

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

                        QueryNode expression = this.bindMethod(token.Expression);

                        if (expression is SingleValueNode singleValueNode)
                        {
                            IEdmTypeReference typeReference = CreateAggregateExpressionTypeReference(singleValueNode, token.MethodDefinition);
                            return new AggregateExpression(singleValueNode, token.MethodDefinition, token.Alias, typeReference);
                        }
                        else if (expression is CollectionNode collectionNode)
                        {
                            IEdmTypeReference typeReference = CreateAggregateExpressionTypeReference(collectionNode, token.MethodDefinition);
                            return new AggregateCollectionExpression(collectionNode, token.MethodDefinition, token.Alias, typeReference);
                        }
                        else
                        {
                            throw new ODataException(Error.Format(SRResources.ApplyBinder_AggregateExpressionNodeNotSupported, aggregateToken.Kind));
                        }
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
                    throw new ODataException(Error.Format(SRResources.ApplyBinder_UnsupportedAggregateKind, aggregateToken.Kind));
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
                        case EdmPrimitiveTypeKind.Int16:
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
                                Error.Format(SRResources.ApplyBinder_AggregateExpressionIncompatibleTypeForMethod, expression,
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
                    if (method.MethodKind == AggregationMethod.Custom)
                    {
                        IEdmTypeReference returnType = GetCustomMethodReturnType(expressionType, method, configuration == null ? false : configuration.EnableCaseInsensitiveUriFunctionIdentifier);
                        if (returnType != null)
                        {
                            return returnType;
                        }
                    }

                    // Only the EdmModel knows which type the custom aggregation methods returns.
                    // Since we do not have a reference for it, right now we are assuming that all custom aggregation methods returns Doubles
                    // TODO: find a appropriate way of getting the return type.
                    return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, expressionType.IsNullable);
            }
        }

        private IEdmTypeReference CreateAggregateExpressionTypeReference(CollectionNode expression, AggregationMethodDefinition method)
        {
            IEdmTypeReference expressionType = expression.CollectionType;
            if (expressionType == null && aggregateExpressionsCache != null)
            {
                CollectionOpenPropertyAccessNode openProperty = expression as CollectionOpenPropertyAccessNode;
                if (openProperty != null)
                {
                    expressionType = GetTypeReferenceByPropertyName(openProperty.Name);
                }
            }

            IEdmTypeReference returnType = null;
            if (method.MethodKind == AggregationMethod.Custom)
            {
                returnType = GetCustomMethodReturnType(expressionType, method, configuration == null ? false : configuration.EnableCaseInsensitiveUriFunctionIdentifier);
            }
            else
            {
                throw new ODataException(SRResources.ApplyBinder_AggregateCollectionExpressionOnlySupportCustomMethod);
            }

            if (returnType == null)
            {
                // Only the EdmModel knows which type the custom aggregation methods returns.
                // Since we do not have a reference for it, right now we are assuming that all custom aggregation methods returns Collection(Double)?
                return new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, expressionType.IsNullable)));
            }

            return returnType;
        }

        private static IEdmTypeReference GetCustomMethodReturnType(IEdmTypeReference expressionType, AggregationMethodDefinition method, bool enableCaseInsensitive)
        {
            Debug.Assert(method != null);
            Debug.Assert(method.MethodKind == AggregationMethod.Custom);

            // So far, it seems the 'custom' function is out of edm model defined? Why?
            // Later, we can consider the 'unbound' functions in the Edm model?
            bool customFound = CustomUriFunctions.TryGetCustomFunction(method.MethodLabel, out IList<KeyValuePair<string, FunctionSignatureWithReturnType>> customUriFunctionsNameSignatures, enableCaseInsensitive);
            if (!customFound)
            {
                return null;
            }

            // Find the best custom function.
            foreach (KeyValuePair<string, FunctionSignatureWithReturnType> candidate in customUriFunctionsNameSignatures)
            {
                // The custom function accepts the query node as input (parameter), so, the length should be 1.
                if (candidate.Value.ArgumentTypes.Length != 1)
                {
                    continue;
                }

                IEdmTypeReference parameterType = candidate.Value.ArgumentTypes[0];
                if (CanPromoteParameterTo(expressionType, parameterType))
                {
                    // First match wins.
                    return candidate.Value.ReturnType;
                }
            }

            return null;
        }

        /// <summary>Promotes the specified expression to the given type if necessary.</summary>
        /// <param name="sourceType">The actual argument type.</param>
        /// <param name="targetType">The required type to promote to.</param>
        /// <returns>True if the <paramref name="sourceType"/> could be promoted; otherwise false.</returns>
        private static bool CanPromoteParameterTo(IEdmTypeReference sourceType, IEdmTypeReference targetType)
        {
            Debug.Assert(sourceType != null, "sourceType != null");
            Debug.Assert(targetType != null, "targetType != null");

            EdmTypeKind sourceTypeKind = sourceType.TypeKind();
            EdmTypeKind targetTypeKind = targetType.TypeKind();
            if (sourceTypeKind == EdmTypeKind.Collection && targetTypeKind == EdmTypeKind.Collection)
            {
                sourceType = sourceType.GetCollectionItemType();
                targetType = targetType.GetCollectionItemType();
            }
            else if (sourceTypeKind == EdmTypeKind.Collection || targetTypeKind == EdmTypeKind.Collection)
            {
                return false;
            }

            if (sourceType.IsEquivalentTo(targetType))
            {
                return true;
            }

            if (TypePromotionUtils.CanConvertTo(null, sourceType, targetType)) // sourceNodeOrNull is not needed here, so input 'null'
            {
                return true;
            }

            // Allow promotion from nullable<T> to non-nullable by directly accessing underlying value.
            if (sourceType.IsNullable && targetType.IsODataValueType())
            {
                // COMPAT 40: Type promotion in the product allows promotion from a nullable type to arbitrary value types
                IEdmTypeReference nonNullableSourceType = sourceType.Definition.ToTypeReference(false);
                if (TypePromotionUtils.CanConvertTo(null, nonNullableSourceType, targetType))
                {
                    return true;
                }
            }

            return false;
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

                if (bindResult is SingleValuePropertyAccessNode property)
                {
                    RegisterProperty(properties, ReversePropertyPath(property));
                }
                else if (bindResult is SingleComplexNode complexProperty)
                {
                    RegisterProperty(properties, ReversePropertyPath(complexProperty));
                }
                else if (bindResult is SingleValueOpenPropertyAccessNode openProperty)
                {
                    RegisterProperty(properties, ReversePropertyPath(openProperty));
                }
                else
                {
                    throw new ODataException(
                        Error.Format(SRResources.ApplyBinder_GroupByPropertyNotPropertyAccessValue, propertyToken.Identifier));
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
                    throw new ODataException(Error.Format(SRResources.ApplyBinder_UnsupportedGroupByChild, token.Child.Kind));
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
                   node.Kind == QueryNodeKind.SingleNavigationNode ||
                   node.Kind == QueryNodeKind.SingleResourceCast;
        }

        private static Stack<SingleValueNode> ReversePropertyPath(SingleValueNode node)
        {
            Stack<SingleValueNode> result = new Stack<SingleValueNode>();
            do
            {
                if (node.Kind == QueryNodeKind.SingleResourceCast)
                {
                    node = ((SingleResourceCastNode)node).Source;
                    continue;
                }

                result.Push(node);
                if (node.Kind == QueryNodeKind.SingleValuePropertyAccess)
                {
                    node = ((SingleValuePropertyAccessNode)node).Source;
                }
                else if (node.Kind == QueryNodeKind.SingleComplexNode)
                {
                    node = ((SingleComplexNode)node).Source;
                }
                else if (node.Kind == QueryNodeKind.SingleNavigationNode)
                {
                    node = ((SingleNavigationNode)node).Source;
                }
                else if (node.Kind == QueryNodeKind.SingleValueOpenPropertyAccess)
                {
                    node = ((SingleValueOpenPropertyAccessNode)node).Source;
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
            else if (property.Kind == QueryNodeKind.SingleValueOpenPropertyAccess)
            {
                return ((SingleValueOpenPropertyAccessNode)property).Name;
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
