//---------------------------------------------------------------------
// <copyright file="ApplyBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    using System.Collections.Generic;
    using Edm.Library;

    internal sealed class ApplyBinder
    {
        private MetadataBinder.QueryTokenVisitor _bindMethod;

        private BindingState _state;

        private FilterBinder _filterBinder;

        internal ApplyBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
        {
            this._bindMethod = bindMethod;
            this._state = state;
            this._filterBinder = new FilterBinder(bindMethod, state);
        }

        internal ApplyClause BindApply(IEnumerable<QueryToken> tokens)
        {
            ExceptionUtils.CheckArgumentNotNull(tokens, "tokens");

            List<TransformationNode> transformations = new List<TransformationNode>();

            IEdmTypeReference resultType = null;
            foreach (var token in tokens)
            {
                switch (token.Kind)
                {
                    case TreeNodeKinds.QueryTokenKind.Aggregate:
                        var aggregate = BindAggregateToken((AggregateToken)(token));
                        resultType = aggregate.ItemType;
                        transformations.Add(aggregate);
                        RefreshState(resultType);
                        break;
                    case TreeNodeKinds.QueryTokenKind.GroupBy:
                        var groupBy = BindGroupByToken((GroupByToken)(token));
                        resultType = groupBy.ItemType;
                        transformations.Add(groupBy);
                        RefreshState(resultType);
                        break;
                    default: //assumes filter
                        var filterClause = this._filterBinder.BindFilter(token);
                        var filterNode = new FilterTransformationNode(filterClause);
                        resultType = filterNode.ItemType;
                        transformations.Add(filterNode);
                        break;
                }
            }

            return new ApplyClause(transformations, resultType);
        }

        /// <summary>
        /// Updates bingingState, methoid and filter binder to refrect changed type.
        /// </summary>
        /// <remarks>
        /// Need to call that method after each transformation that chnages shape: groupby, aggregate etc.
        /// </remarks>
        /// <param name="resultType"></param>
        private void RefreshState(IEdmTypeReference resultType)
        {
            var state = new BindingState(_state.Configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(resultType, null);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            _state = state;

            this._bindMethod = (new MetadataBinder(state)).Bind;
            this._filterBinder = new FilterBinder(_bindMethod, state);
        }

        private AggregateTransformationNode BindAggregateToken(AggregateToken token)
        {
            var statements = new List<AggregateStatement>();
            foreach (var statementToken in token.Statements)
            {
                statements.Add(BindAggregateStatementToken(statementToken));
            }

            var typeReference = CreateAggregateTypeReference(statements);

            return new AggregateTransformationNode(statements, typeReference);
        }

        private AggregateStatement BindAggregateStatementToken(AggregateStatementToken token)
        {
            var expression = this._bindMethod(token.Expression) as SingleValueNode;

            if (expression == null)
            {
                throw new ODataException(ODataErrorStrings.ApplyBinder_AggregateStatementNotSingleValue(token.Expression));
            }

            var typeReference = CreateAggregateStatementTypeReference(expression, token.WithVerb);

            // TODO: Determine source
            return new AggregateStatement(expression, token.WithVerb, null, token.AsAlias, typeReference);
        }

        private static IEdmTypeReference CreateAggregateTypeReference(IEnumerable<AggregateStatement> statements)
        {
            var type = new EdmEntityType(string.Empty, "DynamicTypeWrapper", baseType: null, isAbstract: false, isOpen: true);
            foreach (var statement in statements)
            {
                type.AddStructuralProperty(statement.AsAlias, statement.TypeReference);
            }

            return (IEdmTypeReference)ToEdmTypeReference(type, true);
        }

        private static IEdmTypeReference CreateAggregateStatementTypeReference(SingleValueNode expression, AggregationVerb withVerb)
        {
            var expressionType = expression.TypeReference;

            switch (withVerb)
            {
                case AggregationVerb.Average:
                    var expressionPrimitiveKind = expressionType.PrimitiveKind();
                    switch (expressionPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.Double:
                            return Microsoft.OData.Edm.Library.EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, expressionType.IsNullable);
                        case EdmPrimitiveTypeKind.Decimal:
                            return Microsoft.OData.Edm.Library.EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Decimal, expressionType.IsNullable);
                        default:
                            throw new ODataException(ODataErrorStrings.ApplyBinder_AggregateStatementIncompatibleTypeForVerb(expression, expressionPrimitiveKind));
                    }
                case AggregationVerb.CountDistinct:
                    return Microsoft.OData.Edm.Library.EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int64, false);
                case AggregationVerb.Max:
                case AggregationVerb.Min:
                    return expressionType;
                case AggregationVerb.Sum:
                    return expressionType;
                default:
                    throw new ODataException(ODataErrorStrings.ApplyBinder_UnsupportedAggregateVerb(withVerb));
            }
        }

        private GroupByTransformationNode BindGroupByToken(GroupByToken token)
        {
            var properties = new List<GroupByPropertyNode>();

            foreach (var propertyToken in token.Properties)
            {
                var property = this._bindMethod(propertyToken) as SingleValuePropertyAccessNode;

                if (property == null)
                {
                    throw new ODataException(ODataErrorStrings.ApplyBinder_GroupByPropertyNotPropertyAccessValue(propertyToken.Identifier));
                }

                RegisterProperty(properties, ReverseAccessNode(property));
            }

            var groupingType = ToGroupingType(properties, "GroupingDynamicTypeWrapper");
            var resultType = new EdmEntityType(string.Empty, "ResultDynamicTypeWrapper", baseType: groupingType, isAbstract: false, isOpen: true);
            var groupingTypeReference = (IEdmTypeReference)ToEdmTypeReference(groupingType, true);

            AggregateTransformationNode aggregate = null;
            if (token.Aggregate != null)
            {
                aggregate = BindAggregateToken(token.Aggregate);

                foreach (var property in (aggregate.ItemType.Definition as IEdmStructuredType).Properties())
                {
                    resultType.AddStructuralProperty(property.Name, property.Type);
                }
            }

            var resultingTypeReference = (IEdmTypeReference)ToEdmTypeReference(resultType, true);

            // TODO: Determine source
            return new GroupByTransformationNode(properties, groupingTypeReference, aggregate, resultingTypeReference, null);

        }

        private Stack<SingleValueNode> ReverseAccessNode(SingleValueNode node)
        {
            var result = new Stack<SingleValueNode>();
            do
            {
                result.Push(node);
                if (node.Kind == TreeNodeKinds.QueryNodeKind.SingleValuePropertyAccess)
                {
                    node = ((SingleValuePropertyAccessNode)node).Source;

                }
                else if (node.Kind == TreeNodeKinds.QueryNodeKind.SingleNavigationNode)
                {
                    node = ((SingleNavigationNode)node).NavigationSource as SingleValueNode;
                }
            } while (node != null && (node.Kind == TreeNodeKinds.QueryNodeKind.SingleValuePropertyAccess || node.Kind == TreeNodeKinds.QueryNodeKind.SingleNavigationNode));

            return result;
        }

        private EdmEntityType ToGroupingType(IList<GroupByPropertyNode> properties, string typeName)
        {
            var groupingType = new EdmEntityType(string.Empty, typeName, baseType: null, isAbstract: false, isOpen: true);
            AddPropertiesToEdmType(properties, typeName, groupingType);

            return groupingType;
        }

        private EdmComplexType ToComplexType(IList<GroupByPropertyNode> properties, string typeName)
        {
            var groupingType = new EdmComplexType(string.Empty, typeName, baseType: null, isAbstract: false, isOpen: true);
            AddPropertiesToEdmType(properties, typeName, groupingType);

            return groupingType;
        }

        private void AddPropertiesToEdmType(IList<GroupByPropertyNode> properties, string typeName, EdmStructuredType groupingType)
        {
            foreach (var property in properties)
            {
                if (property.Accessor != null)
                {
                    groupingType.AddStructuralProperty(property.Name, property.Accessor.GetEdmTypeReference());
                }
                else
                {
                    var containerType = ToComplexType(property.Children, typeName + property.Name);
                    groupingType.AddStructuralProperty(property.Name, containerType.ToTypeReference());
                }
            }
        }

        private void RegisterProperty(IList<GroupByPropertyNode> properties, Stack<SingleValueNode> propertyStack)
        {
            var property = propertyStack.Pop();
            string propertyName = GetNodePropertyName(property);

            if (propertyStack.Count != 0)
            {
                // Not at the leaf, let's add to the container 

                var containerProperty = properties.FirstOrDefault(p => p.Name == propertyName);
                if (containerProperty == null)
                {
                    // We do not have container yet. Create it
                    containerProperty = new GroupByPropertyNode(propertyName, (SingleValuePropertyAccessNode)null);
                    properties.Add(containerProperty);
                }

                RegisterProperty(containerProperty.Children, propertyStack);
            }
            else
            {
                // It's the leaf just add
                properties.Add(new GroupByPropertyNode(propertyName, property as SingleValuePropertyAccessNode));
            }
        }

        private static string GetNodePropertyName(SingleValueNode property)
        {
            string propertyName = null;
            if (property.Kind == TreeNodeKinds.QueryNodeKind.SingleValuePropertyAccess)
            {
                propertyName = ((SingleValuePropertyAccessNode)property).Property.Name;
            }
            else if (property.Kind == TreeNodeKinds.QueryNodeKind.SingleNavigationNode)
            {
                propertyName = ((SingleNavigationNode)property).NavigationProperty.Name;
            }

            else
            {
                throw new NotSupportedException();
            }

            return propertyName;
        }

        private static IEdmTypeReference ToEdmTypeReference(IEdmType edmType, bool isNullable)
        {
            switch (edmType.TypeKind)
            {
                case EdmTypeKind.Collection:
                    return new EdmCollectionTypeReference(edmType as IEdmCollectionType);
                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference(edmType as IEdmComplexType, isNullable);
                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference(edmType as IEdmEntityType, isNullable);
                case EdmTypeKind.EntityReference:
                    return new EdmEntityReferenceTypeReference(edmType as IEdmEntityReferenceType, isNullable);
                case EdmTypeKind.Enum:
                    return new EdmEnumTypeReference(edmType as IEdmEnumType, isNullable);
                case EdmTypeKind.Primitive:
                    return Microsoft.OData.Edm.Library.EdmCoreModel.Instance.GetPrimitive((edmType as IEdmPrimitiveType).PrimitiveKind, isNullable);
                default:
                    throw new ODataException(ODataErrorStrings.ApplyBinder_UnsupportedType(edmType.TypeKind));
            }
        }
    }
}
