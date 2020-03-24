//---------------------------------------------------------------------
// <copyright file="SchemaTypeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.OData.Edm.Csdl.Json.Ast;
using Microsoft.OData.Edm.Csdl.Json.Value;

namespace Microsoft.OData.Edm.Csdl.Json.Builder
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// Complex, Entity, Enum, TypeDefinition -> IEdmSchemaType
    /// </summary>
    internal class SchemaTypeBuilder
    {
        private IDictionary<CsdlJsonSchemaTypeItem, CsdlJsonModel> _schemaTypeItemToModelsMapping;

        private IDictionary<string, CsdlJsonSchemaTypeItem> _schemaTypeItems = new Dictionary<string, CsdlJsonSchemaTypeItem>();

        private readonly IDictionary<string, IEdmSchemaType> _schemaTypes = new Dictionary<string, IEdmSchemaType>();

      //  private CsdlSerializerOptions _options;

        internal SchemaTypeBuilder(IDictionary<CsdlJsonSchemaTypeItem, CsdlJsonModel> schemaTypeItemToModelMapping, CsdlSerializerOptions options)
        {
     //       _options = options;

            _schemaTypeItemToModelsMapping = schemaTypeItemToModelMapping;

            schemaTypeItemToModelMapping.ForEach(k => _schemaTypeItems[k.Key.FullName] = k.Key);
        }

        internal string ReplaceAlias(CsdlJsonSchemaTypeItem item, string name)
        {
            CsdlJsonModel declaredModel = _schemaTypeItemToModelsMapping[item];
            return declaredModel.ReplaceAlias(name);
        }

        public void BuildSchemaTypes()
        {
            // Create headers to allow CreateEdmTypeBody to blindly references other things.
            foreach (var item in _schemaTypeItems)
            {
                BuildSchemaTypeHeader(item.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullQualifiedName">namespace</param>
        /// <returns></returns>
        private IEdmSchemaType GetSchemaType(string fullQualifiedName)
        {
            IEdmSchemaType schemaType;
            if (_schemaTypes.TryGetValue(fullQualifiedName, out schemaType))
            {
                return schemaType;
            }

            return null;
        }

        private CsdlJsonSchemaTypeItem GetSchemaTypeItem(string fullQualifiedName)
        {
            CsdlJsonSchemaTypeItem baseItem;
            _schemaTypeItems.TryGetValue(fullQualifiedName, out baseItem);
            return baseItem;
        }

        //private EdmModel GetEdmModel(CsdlJsonSchemaTypeItem schemaItem)
        //{
        //    return _schemaTypeItemToModelsMapping[schemaItem].EdmModel;
        //}

        private void BuildSchemaTypeHeader(CsdlJsonSchemaTypeItem schemaItem)
        {
            IEdmSchemaType schemaType = GetSchemaType(schemaItem.FullName);
            if (schemaType != null)
            {
                // created before
                return;
            }

            switch (schemaItem.Kind)
            {
                case SchemaMemberKind.Complex:
                    CsdlJsonSchemaComplexItem complex = (CsdlJsonSchemaComplexItem)schemaItem;
                    IEdmComplexType baseComplexType = null;
                    if (complex.BaseType != null)
                    {
                        string replacedBaseTypeName = ReplaceAlias(complex, complex.BaseType);

                        CsdlJsonSchemaTypeItem baseItem = GetSchemaTypeItem(replacedBaseTypeName);

                        BuildSchemaTypeHeader(baseItem);

                        baseComplexType = GetSchemaType(replacedBaseTypeName) as IEdmComplexType;

                        Contract.Assert(baseComplexType != null);
                    }

                    EdmComplexType complexType = new EdmComplexType(schemaItem.Namespace, schemaItem.Name,
                        baseComplexType, complex.IsAbstract, complex.IsOpen);

                    _schemaTypes.Add(schemaItem.FullName, complexType);

                    _schemaTypeItemToModelsMapping[schemaItem].EdmModel.AddElement(complexType);
                    break;

                case SchemaMemberKind.Entity:
                    CsdlJsonSchemaEntityItem entity = (CsdlJsonSchemaEntityItem)schemaItem;

                    IEdmEntityType baseEntityType = null;
                    if (entity.BaseType != null)
                    {
                        string replacedBaseTypeName = ReplaceAlias(entity, entity.BaseType);

                        CsdlJsonSchemaTypeItem baseItem = GetSchemaTypeItem(replacedBaseTypeName);

                        BuildSchemaTypeHeader(baseItem);

                        baseEntityType = GetSchemaType(replacedBaseTypeName) as IEdmEntityType;

                        Contract.Assert(baseEntityType != null);
                    }

                    EdmEntityType entityType = new EdmEntityType(schemaItem.Namespace, schemaItem.Name, baseEntityType,
                        entity.IsAbstract, entity.IsOpen, entity.HasStream);

                    _schemaTypes.Add(schemaItem.FullName, entityType);

                    _schemaTypeItemToModelsMapping[schemaItem].EdmModel.AddElement(entityType);
                    break;

                case SchemaMemberKind.Enum:
                    CsdlJsonSchemaEnumItem enumItem = (CsdlJsonSchemaEnumItem)schemaItem;

                    EdmEnumType enumType = new EdmEnumType(enumItem.Namespace, enumItem.Name,
                            GetEnumUnderlyingType(enumItem.UnderlyingTypeName), enumItem.IsFlags);

                    _schemaTypes.Add(enumItem.FullName, enumType);
                    _schemaTypeItemToModelsMapping[schemaItem].EdmModel.AddElement(enumType);
                    break;

                case SchemaMemberKind.TypeDefinition:
                    CsdlJsonSchemaTypeDefinitionItem typeDefinitionItem = (CsdlJsonSchemaTypeDefinitionItem)schemaItem;

                    EdmTypeDefinition typeDefinition = new EdmTypeDefinition(typeDefinitionItem.Namespace, typeDefinitionItem.Name,
                        GetUnderlyingType(typeDefinitionItem));

                    _schemaTypes.Add(typeDefinitionItem.FullName, typeDefinition);

                    _schemaTypeItemToModelsMapping[schemaItem].EdmModel.AddElement(typeDefinition);
                    break;

                case SchemaMemberKind.EntityContainer:
                case SchemaMemberKind.Action:
                case SchemaMemberKind.Function:
                case SchemaMemberKind.Term:
                    // Don't build action, function, term until all types are built
                    break;
            }
        }

        private IEdmPrimitiveType GetUnderlyingType(CsdlJsonSchemaTypeDefinitionItem typeDefinitionItem)
        {
            IEdmTypeReference underlyingTypeRef = BuildTypeReference(typeDefinitionItem.UnderlyingTypeName,
                false,
                true,
                false,
                typeDefinitionItem.MaxLength,
                typeDefinitionItem.Unicode,
                typeDefinitionItem.Precision,
                typeDefinitionItem.Scale,
                typeDefinitionItem.Srid);

            IEdmType underlyingType = underlyingTypeRef.Definition;
            if (underlyingType.TypeKind != EdmTypeKind.Primitive)
            {
                throw new Exception();
            }

            return (IEdmPrimitiveType)underlyingType;
        }

        private static IEdmPrimitiveType GetEnumUnderlyingType(string underlyingType)
        {
            if (underlyingType != null)
            {
                var underlyingTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(underlyingType);
                if ( underlyingTypeKind != EdmPrimitiveTypeKind.None)
                {
                    return EdmCoreModel.Instance.GetPrimitiveType(underlyingTypeKind);
                }

                throw new Exception();
            }

            return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
        }

        public IEdmTypeReference BuildTypeReference(string typeString, // if it's collection, it's element type string
            bool isCollection,
            bool isNullable,
            bool isUnbounded,
            int? maxLength,
            bool? unicode,
            int? precision,
            int? scale,
            int? srid)
        {
            IEdmTypeReference type = ParseNamedTypeReference(typeString, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
            if (isCollection)
            {
                type = new EdmCollectionTypeReference(new EdmCollectionType(type));
            }

            return type;
        }

        private IEdmTypeReference ParseNamedTypeReference(string typeName, bool isNullable,
             bool isUnbounded,
             int? maxLength,
             bool? unicode,
             int? precision,
             int? scale,
             int? srid)
        {
            IEdmPrimitiveTypeReference primitiveType;
            EdmPrimitiveTypeKind kind = EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName);
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.PrimitiveType:
                    return EdmCoreModel.Instance.GetPrimitive(kind, isNullable);

                case EdmPrimitiveTypeKind.Binary:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmBinaryTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, isUnbounded, maxLength);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmTemporalTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, precision);

                case EdmPrimitiveTypeKind.Decimal:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmDecimalTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, precision, scale);

                case EdmPrimitiveTypeKind.String:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmStringTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, isUnbounded, maxLength, unicode);

                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmSpatialTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, srid);

                case EdmPrimitiveTypeKind.None:
                    break;
            }

            EdmPathTypeKind pathTypeKind = EdmCoreModel.Instance.GetPathTypeKind(typeName);
            if (pathTypeKind != EdmPathTypeKind.None)
            {
                return EdmCoreModel.Instance.GetPathType(pathTypeKind, isNullable);
            }

            IEdmSchemaType schemaType;
            if (_schemaTypes.TryGetValue(typeName, out schemaType))
            {
                return GetEdmTypeReference(schemaType as IEdmType, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
            }

            // If we can't find the type, find it from referenced model.
            // IEdmType edmType = _edmModel.FindType(typeName);
            return null;
            //return GetEdmTypeReference(edmType, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
        }

        private static IEdmTypeReference GetEdmTypeReference(IEdmType edmType, bool isNullable,
             bool isUnbounded,
             int? maxLength,
             bool? unicode,
             int? precision,
             int? scale,
             int? srid)
        {
            switch (edmType.TypeKind)
            {
                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference((IEdmComplexType)edmType, isNullable);

                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference((IEdmEntityType)edmType, isNullable);

                case EdmTypeKind.Enum:
                    return new EdmEnumTypeReference((IEdmEnumType)edmType, isNullable);

                case EdmTypeKind.TypeDefinition:
                    return new EdmTypeDefinitionReference((IEdmTypeDefinition)edmType, isNullable, isUnbounded, maxLength, isUnbounded, precision, scale, srid);
            }

            throw new CsdlParseException();
        }
    }
}
