//---------------------------------------------------------------------
// <copyright file="EdmTypeHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Builder
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// Complex, Entity, Enum, TypeDefinition -> IEdmSchemaType
    /// </summary>
    internal static class EdmTypeHelper
    {
        public static IEdmTypeReference BuildEdmTypeReference(
            string typeName, // if it's collection, it's element type string
            bool isCollection,
            bool isNullable,
            IDictionary<string, IEdmSchemaType> allSchemaTypes)
        {
            return BuildEdmTypeReference(typeName, isCollection, isNullable, false, null, null, null, null, null, allSchemaTypes);
        }

        public static IEdmTypeReference BuildEdmTypeReference(
            string typeName, // if it's collection, it's element type string
            bool isCollection,
            bool isNullable,
            bool isUnbounded,
            int? maxLength,
            bool? unicode,
            int? precision,
            int? scale,
            int? srid,
            IDictionary<string, IEdmSchemaType> allSchemaTypes)
        {
            IEdmTypeReference type = ParseNamedTypeReference(typeName, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid, allSchemaTypes);
            if (isCollection)
            {
                type = new EdmCollectionTypeReference(new EdmCollectionType(type));
            }

            return type;
        }

        private static IEdmTypeReference ParseNamedTypeReference(
            string typeName, // if it's collection, it's element type string
            bool isNullable,
            bool isUnbounded,
            int? maxLength,
            bool? unicode,
            int? precision,
            int? scale,
            int? srid,
            IDictionary<string, IEdmSchemaType> allSchemaTypes)
        {
            // Process the primitive type
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

            // Process the "Path" Edm Type.
            EdmPathTypeKind pathTypeKind = EdmCoreModel.Instance.GetPathTypeKind(typeName);
            if (pathTypeKind != EdmPathTypeKind.None)
            {
                return EdmCoreModel.Instance.GetPathType(pathTypeKind, isNullable);
            }

            IEdmSchemaType schemaType;
            if (allSchemaTypes.TryGetValue(typeName, out schemaType))
            {
                return GetSchemaTypeReference(schemaType, isNullable, unicode, maxLength, precision, scale, srid);
            }

            return null;
        }

        private static IEdmTypeReference GetSchemaTypeReference(
            IEdmSchemaType edmType,
            bool isNullable,
            bool? unicode,
            int? maxLength,
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
                    return new EdmTypeDefinitionReference((IEdmTypeDefinition)edmType, isNullable, false, maxLength, unicode, precision, scale, srid);
            }

            throw new CsdlParseException();
        }
    }
}
