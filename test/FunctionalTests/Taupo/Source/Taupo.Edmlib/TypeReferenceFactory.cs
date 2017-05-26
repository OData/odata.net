//---------------------------------------------------------------------
// <copyright file="TypeReferenceFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Factory (convenience methods) to construct TypeReference from Type definition
    /// </summary>
    /// <remarks>
    /// Ideally, this should be part of EdmLib itself (remove this if it becomes true)
    /// </remarks>
    public static class TypeReferenceFactory
    {
        /// <summary>
        /// Constructs a Binary type refrence with default facets
        /// </summary>
        /// <returns>Binary type refrence with default facets</returns>
        public static IEdmPrimitiveTypeReference Binary()
        {
            return Binary(false, null);
        }

        /// <summary>
        /// Constructs a Binary type refrence with specified facets
        /// </summary>
        /// <param name="isUnbounded">Whether it is of "max" length</param>
        /// <param name="maxLength">The max length</param>
        /// <returns>Binary type reference with specified facets</returns>
        public static IEdmPrimitiveTypeReference Binary(bool isUnbounded, int? maxLength)
        {
            return new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, isUnbounded, maxLength);
        }

        /// <summary>
        /// Constructs a Boolean type refrence
        /// </summary>
        /// <returns>Boolean type refrence</returns>
        public static IEdmPrimitiveTypeReference Boolean()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), false);
        }

        /// <summary>
        /// Constructs a Byte type refrence
        /// </summary>
        /// <returns>Byte type refrence</returns>
        public static IEdmPrimitiveTypeReference Byte()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), false);
        }

        /// <summary>
        /// Constructs a DateTimeOffset type refrence with default facets
        /// </summary>
        /// <returns>DateTimeOffset type refrence with default facets</returns>
        public static IEdmPrimitiveTypeReference DateTimeOffset()
        {
            return DateTimeOffset(null);
        }

        /// <summary>
        /// Constructs a DateTimeOffset type refrence with specified facets
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>DateTimeOffset type refrence with specified facets</returns>
        public static IEdmPrimitiveTypeReference DateTimeOffset(int? precision)
        {
            return new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false, precision);
        }

        /// <summary>
        /// Constructs a Decimal type refrence with default facets
        /// </summary>
        /// <returns>Decimal type refrence with default facets</returns>
        public static IEdmPrimitiveTypeReference Decimal()
        {
            return Decimal(null, null);
        }

        /// <summary>
        /// Constructs a Decimal type refrence with specified facets
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>Decimal type refrence with specified facets</returns>
        public static IEdmPrimitiveTypeReference Decimal(int? precision, int? scale)
        {
            return new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false, precision, scale);
        }

        /// <summary>
        /// Constructs a Double type refrence
        /// </summary>
        /// <returns>Double type refrence</returns>
        public static IEdmPrimitiveTypeReference Double()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), false);
        }

        /// <summary>
        /// Constructs a Guid type refrence
        /// </summary>
        /// <returns>Guid type refrence</returns>
        public static IEdmPrimitiveTypeReference Guid()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), false);
        }

        /// <summary>
        /// Constructs a Int16 type refrence
        /// </summary>
        /// <returns>Int16 type refrence</returns>
        public static IEdmPrimitiveTypeReference Int16()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), false);
        }

        /// <summary>
        /// Constructs a Int32 type refrence
        /// </summary>
        /// <returns>Int32 type refrence</returns>
        public static IEdmPrimitiveTypeReference Int32()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false);
        }

        /// <summary>
        /// Constructs a Int64 type refrence
        /// </summary>
        /// <returns>Int64 type refrence</returns>
        public static IEdmPrimitiveTypeReference Int64()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), false);
        }

        /// <summary>
        /// Constructs a SByte type refrence
        /// </summary>
        /// <returns>SByte type refrence</returns>
        public static IEdmPrimitiveTypeReference SByte()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), false);
        }

        /// <summary>
        /// Constructs a Single type refrence
        /// </summary>
        /// <returns>Single type refrence</returns>
        public static IEdmPrimitiveTypeReference Single()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), false);
        }

        /// <summary>
        /// Constructs a Stream type refrence with default facets
        /// </summary>
        /// <returns>Stream type refrence with default facets</returns>
        public static IEdmPrimitiveTypeReference Stream()
        {
            return new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), false);
        }

        /// <summary>
        /// Constructs a String type refrence with default facets
        /// </summary>
        /// <returns>String type refrence with default facets</returns>
        public static IEdmPrimitiveTypeReference String()
        {
            return String(false, null, true);
        }

        /// <summary>
        /// Constructs a String type refrence with specified facets
        /// </summary>
        /// <param name="isUnbounded">Whether it is of "max" length</param>
        /// <param name="maxLength">The max length</param>
        /// <param name="isUnicode">Whether it is unicode</param>
        /// <returns>String type refrence with specified facets</returns>
        public static IEdmPrimitiveTypeReference String(bool isUnbounded, int? maxLength, bool isUnicode)
        {
            return new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false, isUnbounded, maxLength, isUnicode);
        }

        /// <summary>
        /// Constructs a Time type refrence with default facets
        /// </summary>
        /// <returns>Time type refrence with default facets</returns>
        public static IEdmPrimitiveTypeReference Time()
        {
            return Time(null);
        }

        /// <summary>
        /// Constructs a Time type refrence with specified facets
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>Time type refrence with specified facets</returns>
        public static IEdmPrimitiveTypeReference Time(int? precision)
        {
            return new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), false, precision);
        }

        /// <summary>
        /// Constructs a Geography type refrence
        /// </summary>
        /// <returns>Geography type reference</returns>
        public static IEdmPrimitiveTypeReference Geography()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Geography, true);
        }

        /// <summary>
        /// Constructs a GeographyPoint type refrence
        /// </summary>
        /// <returns>GeographyPoint type reference</returns>
        public static IEdmPrimitiveTypeReference GeographyPoint()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeographyPoint, true);
        }

        /// <summary>
        /// Constructs a GeographyLineString type refrence
        /// </summary>
        /// <returns>GeographyLineString type reference</returns>
        public static IEdmPrimitiveTypeReference GeographyLineString()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeographyLineString, true);
        }

        /// <summary>
        /// Constructs a GeographyPolygon type refrence
        /// </summary>
        /// <returns>GeographyPolygon type reference</returns>
        public static IEdmPrimitiveTypeReference GeographyPolygon()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeographyPolygon, true);
        }

        /// <summary>
        /// Constructs a GeographyCollection type refrence
        /// </summary>
        /// <returns>GeographyCollection type reference</returns>
        public static IEdmPrimitiveTypeReference GeographyCollection()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeographyCollection, true);
        }

        /// <summary>
        /// Constructs a GeographyMultiPolygon type refrence
        /// </summary>
        /// <returns>GeographyMultiPolygon type reference</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "GeographyMultiPolygon is the accepted name")]
        public static IEdmPrimitiveTypeReference GeographyMultiPolygon()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeographyMultiPolygon, true);
        }

        /// <summary>
        /// Constructs a GeographyMultiLineString type refrence
        /// </summary>
        /// <returns>GeographyMultiLineString type reference</returns>
        public static IEdmPrimitiveTypeReference GeographyMultilineString()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeographyMultiLineString, true);
        }

        /// <summary>
        /// Constructs a GeographyMultiPoint type refrence
        /// </summary>
        /// <returns>GeographyMultiPoint type reference</returns>
        public static IEdmPrimitiveTypeReference GeographyMultipoint()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeographyMultiPoint, true);
        }

        /// <summary>
        /// Constructs a Geometry type refrence
        /// </summary>
        /// <returns>Geometry type reference</returns>
        public static IEdmPrimitiveTypeReference Geometry()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Geometry, true);
        }

        /// <summary>
        /// Constructs a GeometryPoint type refrence
        /// </summary>
        /// <returns>GeometryPoint type reference</returns>
        public static IEdmPrimitiveTypeReference GeometryPoint()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeometryPoint, true);
        }

        /// <summary>
        /// Constructs a GeometryLineString type refrence
        /// </summary>
        /// <returns>GeometryLineString type reference</returns>
        public static IEdmPrimitiveTypeReference GeometryLineString()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeometryLineString, true);
        }

        /// <summary>
        /// Constructs a GeometryPolygon type refrence
        /// </summary>
        /// <returns>GeometryPolygon type reference</returns>
        public static IEdmPrimitiveTypeReference GeometryPolygon()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeometryPolygon, true);
        }

        /// <summary>
        /// Constructs a GeometryCollection type refrence
        /// </summary>
        /// <returns>GeometryCollection type reference</returns>
        public static IEdmPrimitiveTypeReference GeometryCollection()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeometryCollection, true);
        }

        /// <summary>
        /// Constructs a GeometryMultiPolygon type refrence
        /// </summary>
        /// <returns>GeometryMultiPolygon type reference</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi",
             Justification = "GeometryMultiPolygon is the accepted name")]
        public static IEdmPrimitiveTypeReference GeometryMultiPolygon()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeometryMultiPolygon, true);
        }

        /// <summary>
        /// Constructs a GeometryMultiLineString type refrence
        /// </summary>
        /// <returns>GeometryMultiLineString type reference</returns>
        public static IEdmPrimitiveTypeReference GeometryMultilineString()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeometryMultiLineString, true);
        }

        /// <summary>
        /// Constructs a GeometryMultiPoint type refrence
        /// </summary>
        /// <returns>GeometryMultiPoint type reference</returns>
        public static IEdmPrimitiveTypeReference GeometryMultipoint()
        {
            return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.GeometryMultiPoint, true);
        }

        /// <summary>
        /// Constructs a Complex type reference from definition
        /// </summary>
        /// <param name="definition">The Complex type definition</param>
        /// <returns>The Complex type reference</returns>
        public static IEdmComplexTypeReference ComplexTypeReference(IEdmComplexType definition)
        {
            return new EdmComplexTypeReference(definition, true);
        }

        /// <summary>
        /// Constructs an Enum type reference from definition
        /// </summary>
        /// <param name="definition">The Enum type definition</param>
        /// <returns>The Enum type reference</returns>
        public static IEdmEnumTypeReference EnumTypeReference(IEdmEnumType definition)
        {
            return new EdmEnumTypeReference(definition, true);
        }

        /// <summary>
        /// Constructs a Entity type reference from definition
        /// </summary>
        /// <param name="definition">The Entity type definition</param>
        /// <returns>The Entity type reference</returns>
        public static IEdmEntityTypeReference EntityTypeReference(IEdmEntityType definition)
        {
            return new EdmEntityTypeReference(definition, true);
        }

        /// <summary>
        /// Constructs a EntityReference type reference from definition
        /// </summary>
        /// <param name="definition">The EntityReference type definition</param>
        /// <returns>The EntityReference type reference</returns>
        public static IEdmEntityReferenceTypeReference EntityReferenceTypeReference(IEdmEntityReferenceType definition)
        {
            return new EdmEntityReferenceTypeReference(definition, true);
        }

        /// <summary>
        /// Constructs a Collection type reference from definition
        /// </summary>
        /// <param name="definition">The Collection type definition</param>
        /// <returns>The Collection type reference</returns>
        public static IEdmCollectionTypeReference CollectionTypeReference(IEdmCollectionType definition)
        {
            return new EdmCollectionTypeReference(definition);
        }

        /// <summary>
        /// Constructs a new type reference with the specified nullable value
        /// </summary>
        /// <param name="typeReference">The original type reference</param>
        /// <param name="isNullable">The nullable value</param>
        /// <returns>A new type reference, with the specified nullable value</returns>
        public static IEdmTypeReference Nullable(this IEdmTypeReference typeReference, bool isNullable)
        {
            switch (typeReference.TypeKind())
            {
                case EdmTypeKind.Collection:
                    var collection = typeReference.AsCollection();
                    return new EdmCollectionTypeReference(collection.CollectionDefinition());

                case EdmTypeKind.Complex:
                    var complex = typeReference.AsComplex();
                    return new EdmComplexTypeReference(complex.ComplexDefinition(), isNullable);

                case EdmTypeKind.Entity:
                    var entity = typeReference.AsEntity();
                    return new EdmEntityTypeReference(entity.EntityDefinition(), isNullable);

                case EdmTypeKind.EntityReference:
                    var entityRef = typeReference.AsEntityReference();
                    return new EdmEntityReferenceTypeReference(entityRef.EntityReferenceDefinition(), isNullable);

                case EdmTypeKind.Primitive:
                    var primitive = (EdmPrimitiveTypeReference)typeReference.AsPrimitive();
                    return primitive.Nullable(isNullable);

                default:
                    throw new TaupoInvalidOperationException("Unexpected Edm Type Kind: " + typeReference.TypeKind());
            }
        }

        /// <summary>
        /// Convert to TypeReference from definition
        /// </summary>
        /// <param name="definition">The type definition</param>
        /// <returns>The type reference</returns>
        public static IEdmCollectionTypeReference ToTypeReference(this IEdmCollectionType definition)
        {
            return CollectionTypeReference(definition);
        }

        /// <summary>
        /// Convert to TypeReference from definition
        /// </summary>
        /// <param name="definition">The type definition</param>
        /// <returns>The type reference</returns>
        public static IEdmComplexTypeReference ToTypeReference(this IEdmComplexType definition)
        {
            return ComplexTypeReference(definition);
        }

        /// <summary>
        /// Convert to TypeReference from definition
        /// </summary>
        /// <param name="definition">The type definition</param>
        /// <returns>The type reference</returns>
        public static IEdmEntityTypeReference ToTypeReference(this IEdmEntityType definition)
        {
            return EntityTypeReference(definition);
        }

        /// <summary>
        /// Convert to TypeReference from definition
        /// </summary>
        /// <param name="definition">The type definition</param>
        /// <returns>The type reference</returns>
        public static IEdmEntityReferenceTypeReference ToTypeReference(this IEdmEntityReferenceType definition)
        {
            return EntityReferenceTypeReference(definition);
        }

        /// <summary>
        /// Convert to TypeReference from definition
        /// </summary>
        /// <param name="definition">The type definition</param>
        /// <returns>The type reference</returns>
        public static IEdmPrimitiveTypeReference ToTypeReference(this IEdmPrimitiveType definition)
        {
            switch (definition.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    return new EdmBinaryTypeReference(definition, false);

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
                    return new EdmPrimitiveTypeReference(definition, false);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return new EdmTemporalTypeReference(definition, false, null);

                case EdmPrimitiveTypeKind.Duration:
                    // Taupo Time to EdmLib Duration has not been done, just throwing instead.
                    // return new EdmTemporalTypeReference(definition, false, null);
                    throw new TaupoNotSupportedException("Taupo doesn't support EdmLib.Duration");
                case EdmPrimitiveTypeKind.Decimal:
                    return new EdmDecimalTypeReference(definition, false, null, null);

                case EdmPrimitiveTypeKind.String:
                    return new EdmStringTypeReference(definition, false);

                default:
                    throw new TaupoInvalidOperationException("Unexpected Edm Primitive Type Kind: " + definition.PrimitiveKind);
            }
        }

        /// <summary>
        /// Convert to EntityReference type reference from entity type definition
        /// </summary>
        /// <param name="entityDefinition">The entity type definition</param>
        /// <returns>The EntityReference type reference</returns>
        public static IEdmEntityReferenceTypeReference ToEntityReferenceTypeReference(this IEdmEntityType entityDefinition)
        {
            var definition = new EdmEntityReferenceType(entityDefinition);
            return definition.ToTypeReference();
        }

        /// <summary>
        /// Convert to collection type reference from element type reference
        /// </summary>
        /// <param name="elementTypeReference">The element type reference</param>
        /// <returns>The colleciton type reference</returns>
        public static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmTypeReference elementTypeReference)
        {
            var collectionDefinition = new EdmCollectionType(elementTypeReference);
            return collectionDefinition.ToTypeReference();
        }

        /// <summary>
        /// Convert to collection type reference from element type definition
        /// </summary>
        /// <param name="elementDefinition">The element type definiton</param>
        /// <returns>The collection type reference</returns>
        public static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmType elementDefinition)
        {
            var elementTypeReference = elementDefinition.ToTypeReference();
            return elementTypeReference.ToCollectionTypeReference();
        }

        private static IEdmPrimitiveTypeReference Nullable(this IEdmPrimitiveTypeReference typeReference, bool isNullable)
        {
            if (typeReference.IsBinary())
            {
                var binary = typeReference.AsBinary();
                return new EdmBinaryTypeReference(typeReference.PrimitiveDefinition(), isNullable, binary.IsUnbounded, binary.MaxLength);
            }
            else if (typeReference.IsDecimal())
            {
                var decimalRef = typeReference.AsDecimal();
                return new EdmDecimalTypeReference(typeReference.PrimitiveDefinition(), isNullable, decimalRef.Precision, decimalRef.Scale);
            }
            else if (typeReference.IsTemporal())
            {
                var temporal = typeReference.AsTemporal();
                return new EdmTemporalTypeReference(typeReference.PrimitiveDefinition(), isNullable, temporal.Precision);
            }
            else if (typeReference.IsString())
            {
                var stringRef = typeReference.AsString();
                return new EdmStringTypeReference(typeReference.PrimitiveDefinition(), isNullable, stringRef.IsUnbounded, stringRef.MaxLength, stringRef.IsUnicode);
            }

            return new EdmPrimitiveTypeReference(typeReference.PrimitiveDefinition(), isNullable);
        }

        private static IEdmTypeReference ToTypeReference(this IEdmType definition)
        {
            switch (definition.TypeKind)
            {
                case EdmTypeKind.Collection:
                    return ((IEdmCollectionType)definition).ToTypeReference();

                case EdmTypeKind.Complex:
                    return ((IEdmComplexType)definition).ToTypeReference();

                case EdmTypeKind.Entity:
                    return ((IEdmEntityType)definition).ToTypeReference();

                case EdmTypeKind.EntityReference:
                    return ((IEdmEntityReferenceType)definition).ToTypeReference();

                case EdmTypeKind.Primitive:
                    return ((IEdmPrimitiveType)definition).ToTypeReference();

                default:
                    throw new TaupoInvalidOperationException("Unexpected Edm Type Kind: " + definition.TypeKind);
            }
        }
    }
}
