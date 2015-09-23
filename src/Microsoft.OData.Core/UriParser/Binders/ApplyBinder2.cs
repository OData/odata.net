//---------------------------------------------------------------------
// <copyright file="ApplyBinder2.cs" company="Microsoft">
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

    internal sealed class ApplyBinder2
    {
        private readonly MetadataBinder.QueryTokenVisitor _bindMethod;

        private readonly BindingState _state;

        private readonly FilterBinder _filterBinder;
     
        internal ApplyBinder2(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
        {
            this._bindMethod = bindMethod;
            this._state = state;
            this._filterBinder = new FilterBinder(bindMethod, state);
        }
       
        internal ApplyClause2 BindApply(IEnumerable<QueryToken> tokens)
        {
            ExceptionUtils.CheckArgumentNotNull(tokens, "applyTokens");

            List<QueryNode> transformations = new List<QueryNode>();

            IEdmTypeReference resultType = null;
            foreach (var token in tokens)
            {
                switch (token.Kind)
                {
                    case TreeNodeKinds.QueryTokenKind.Aggregate:
                        var aggregate = BindAggregateToken((AggregateToken)(token));
                        resultType = aggregate.TypeReference;
                        transformations.Add(aggregate);
                        break;
                    case TreeNodeKinds.QueryTokenKind.GroupBy:
                        var groupBy = BindGroupByToken((GroupByToken)(token));
                        resultType = groupBy.TypeReference;
                        transformations.Add(groupBy);
                        break;
                    default: //assumes filter
                        var filterClause = this._filterBinder.BindFilter(token);
                        resultType = filterClause.Expression.TypeReference;
                        transformations.Add(filterClause);
                        break;
                }
            }

            return new ApplyClause2(transformations, resultType);
        }

        private AggregateNode2 BindAggregateToken(AggregateToken token)
        {
            var statements = new List<AggregateStatementNode2>();
            foreach (var statementToken in token.Statements)
            {
                statements.Add(BindAggregateStatementToken(statementToken));
            }

            var typeReference = CreateAggregateTypeReference(statements);

            return new AggregateNode2(statements, typeReference);
        }

        private AggregateStatementNode2 BindAggregateStatementToken(AggregateStatementToken token)
        {
            var expression = this._bindMethod(token.Expression) as SingleValueNode;

            if (expression == null)
            {
                throw new ODataException(ODataErrorStrings.ApplyBinder_AggregateStatementNotSingleValue(token.Expression));
            }

            var typeReference = CreateAggregateStatementTypeReference(expression, token.WithVerb);

            // TODO: Determine source
            return new AggregateStatementNode2(expression, token.WithVerb, null, token.AsAlias, typeReference, null);
        }

        private static IEdmTypeReference CreateAggregateTypeReference(IEnumerable<AggregateStatementNode2> statements)
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
                    return expressionType;
                case AggregationVerb.Min:
                    return expressionType;
                case AggregationVerb.Sum:
                    return expressionType;
                default:                    
                    throw new ODataException(ODataErrorStrings.ApplyBinder_UnsupportedAggregateVerb(withVerb));
            }
        }

        private GroupByNode2 BindGroupByToken(GroupByToken token)
        {
            var groupingType = new EdmEntityType(string.Empty, "DynamicTypeWrapper", baseType: null, isAbstract: false, isOpen: true);
            var resultType = new EdmEntityType(string.Empty, "DynamicTypeWrapper", baseType: null, isAbstract: false, isOpen: true);

            var properties = new List<SingleValuePropertyAccessNode>();

            foreach (var propertyToken in token.Properties)
            {
                var property = this._bindMethod(propertyToken) as SingleValuePropertyAccessNode;

                if (property == null)
                {
                    throw new ODataException(ODataErrorStrings.ApplyBinder_GroupByPropertyNotPropertyAccessValue(propertyToken.Identifier));
                }

                properties.Add(property);

                groupingType.AddStructuralProperty(property.Property.Name, property.GetEdmTypeReference());
                resultType.AddStructuralProperty(property.Property.Name, property.GetEdmTypeReference());
            }

            var groupingTypeReference = (IEdmTypeReference)ToEdmTypeReference(groupingType, true);


            AggregateNode2 aggregate = null;
            if (token.Aggregate != null)
            {
                aggregate = BindAggregateToken(token.Aggregate);
                

                foreach(var property in (aggregate.TypeReference.Definition as IEdmStructuredType).Properties())
                {
                    resultType.AddStructuralProperty(property.Name, property.Type);
                }
            }

            var resultingTypeReference = (IEdmTypeReference)ToEdmTypeReference(resultType, true);

            // TODO: Determine source
            return new GroupByNode2(properties, groupingTypeReference, aggregate, resultingTypeReference, null);

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
