//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

    using Strings = Microsoft.Data.OData.Strings;    
    #endregion Namespaces

    /// <summary>
    /// Class with code that will eventually live in EdmLib.
    /// </summary>
    /// <remarks>This class should go away completely when the EdmLib integration is fully done.</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Edm", Justification = "Following EdmLib standards.")]
    public static class EdmLibraryExtensions
    {
        /// <summary>
        /// Returns all the entity types in a model.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to get the entity types for (must not be null).</param>
        /// <returns>An enumerable of all <see cref="IEdmEntityType"/> instances in the <paramref name="model"/>.</returns>
        public static IEnumerable<IEdmEntityType> EntityTypes(this IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            IEnumerable<IEdmSchemaElement> schemaElements = model.SchemaElements;
            if (schemaElements != null)
            {
                return schemaElements.OfType<IEdmEntityType>();
            }

            return null;
        }

        /// <summary>
        /// Turns a <see cref="IEdmType"/> into the corresponding non-nullable <see cref="IEdmTypeReference"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>A non-nullable type reference for the <paramref name="type"/>.</returns>
        public static IEdmTypeReference ToTypeReference(this IEdmType type)
        {
            return ToTypeReference(type, false /*nullable*/);
        }

        /// <summary>
        /// Turns a <see cref="IEdmType"/> into the corresponding <see cref="IEdmTypeReference"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <param name="nullable">true if the returned type reference should be nullable; otherwise false.</param>
        /// <returns>A type reference for the <paramref name="type"/>.</returns>
        public static IEdmTypeReference ToTypeReference(this IEdmType type, bool nullable)
        {
            if (type == null)
            {
                return null;
            }

            switch (type.TypeKind)
            {
                case EdmTypeKind.Primitive:
                    return PrimitiveTypeReference((IEdmPrimitiveType)type, nullable);
                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference((IEdmComplexType)type, nullable);
                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference((IEdmEntityType)type, nullable);
                case EdmTypeKind.Collection:
                    return new EdmCollectionTypeReference((IEdmCollectionType)type, nullable);
                case EdmTypeKind.Row:
                    return new EdmRowTypeReference((IEdmRowType)type, nullable);
                case EdmTypeKind.EntityReference:
                    return new EdmEntityReferenceTypeReference((IEdmEntityReferenceType)type, nullable);
                case EdmTypeKind.None:
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EdmLibraryExtensions_ToTypeReference));
            }
        }

        /// <summary>
        /// Creates a multi value type for the specified <paramref name="itemTypeReference"/>.
        /// </summary>
        /// <param name="itemTypeReference">The <see cref="IEdmPrimitiveTypeReference"/> for the item type.</param>
        /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for primitive type references only.")]
        public static IEdmCollectionTypeReference ToMultiValueTypeReference(this IEdmPrimitiveTypeReference itemTypeReference)
        {
            IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference, true /*IsAtomic*/);
            return (IEdmCollectionTypeReference)ToTypeReference(collectionType);
        }

        /// <summary>
        /// Creates a multi value type for the specified <paramref name="itemTypeReference"/>.
        /// </summary>
        /// <param name="itemTypeReference">The <see cref="IEdmComplexTypeReference"/> for the item type.</param>
        /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for complex type references only.")]
        public static IEdmCollectionTypeReference ToMultiValueTypeReference(this IEdmComplexTypeReference itemTypeReference)
        {
            IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference, true /*IsAtomic*/);
            return (IEdmCollectionTypeReference)ToTypeReference(collectionType);
        }

        /// <summary>
        /// Try to determine the primitive type of the <paramref name="value"/> argument and return the name of the primitive type.
        /// </summary>
        /// <param name="value">The value to determine the type for.</param>
        /// <param name="typeName">The name of the primitive type of the <paramref name="value"/>.</param>
        /// <returns>True if the value is of a known primitive type; otherwise false.</returns>
        public static bool TryGetPrimitiveTypeName(object value, out string typeName)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            typeName = null;

            TypeCode typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    typeName = EdmConstants.EdmBooleanTypeName;
                    break;

                case TypeCode.Byte:
                    typeName = EdmConstants.EdmByteTypeName;
                    break;

                case TypeCode.DateTime:
                    typeName = EdmConstants.EdmDateTimeTypeName;
                    break;

                case TypeCode.Decimal:
                    typeName = EdmConstants.EdmDecimalTypeName;
                    break;

                case TypeCode.Double:
                    typeName = EdmConstants.EdmDoubleTypeName;
                    break;

                case TypeCode.Int16:
                    typeName = EdmConstants.EdmInt16TypeName;
                    break;

                case TypeCode.Int32:
                    typeName = EdmConstants.EdmInt32TypeName;
                    break;

                case TypeCode.Int64:
                    typeName = EdmConstants.EdmInt64TypeName;
                    break;

                case TypeCode.SByte:
                    typeName = EdmConstants.EdmSByteTypeName;
                    break;

                case TypeCode.String:
                    typeName = EdmConstants.EdmStringTypeName;
                    break;

                case TypeCode.Single:
                    typeName = EdmConstants.EdmSingleTypeName;
                    break;

                default:
                    byte[] bytes = value as byte[];
                    if (bytes != null)
                    {
                        typeName = EdmConstants.EdmBinaryTypeName;
                        break;
                    }

                    if (value is DateTimeOffset)
                    {
                        typeName = EdmConstants.EdmDateTimeOffsetTypeName;
                        break;
                    }

                    if (value is Guid)
                    {
                        typeName = EdmConstants.EdmGuidTypeName;
                        break;
                    }

                    if (value is TimeSpan)
                    {
                        // Edm.Time
                        typeName = EdmConstants.EdmTimeTypeName;
                        break;
                    }

                    return false;
            }

            Debug.Assert(typeName != null, "typeName != null");
            return true;
        }

        /// <summary>
        /// Checks if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for structured types only.")]
        public static bool IsAssignableFrom(this IEdmStructuredType baseType, IEdmStructuredType subtype)
        {
            ExceptionUtils.CheckArgumentNotNull(baseType, "baseType");
            ExceptionUtils.CheckArgumentNotNull(subtype, "subtype");

            if (baseType.TypeKind != subtype.TypeKind)
            {
                return false;
            }
            
            if (!baseType.IsODataEntityTypeKind() && !baseType.IsODataComplexTypeKind())
            {
                // we only support complex and entity type inheritance.
                return false;
            }

            IEdmStructuredType structuredSubType = subtype;
            while (structuredSubType != null)
            {
                if (structuredSubType.IsEquivalentTo(baseType))
                {
                    return true;
                }

                structuredSubType = structuredSubType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Checks if the primitive type is a geography type.
        /// </summary>
        /// <param name="primitiveType">The type to check.</param>
        /// <returns>true, if the <paramref name="primitiveType"/> is a geography type.</returns>
        public static bool IsGeographyType(this IEdmPrimitiveType primitiveType)
        {
            ExceptionUtils.CheckArgumentNotNull(primitiveType, "primitiveType");

            switch (primitiveType.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Point:
                case EdmPrimitiveTypeKind.LineString:
                    return true;

                // these types are geography types but not yet supported.
                case EdmPrimitiveTypeKind.Polygon:
                case EdmPrimitiveTypeKind.MultiPolygon:
                case EdmPrimitiveTypeKind.MultiLineString:
                case EdmPrimitiveTypeKind.MultiPoint:
                    return false;
                case EdmPrimitiveTypeKind.None:
                case EdmPrimitiveTypeKind.Binary:
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Decimal:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.String:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Time:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometricPoint:
                case EdmPrimitiveTypeKind.GeometricLineString:
                case EdmPrimitiveTypeKind.GeometricPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeometricMultiPolygon:
                case EdmPrimitiveTypeKind.GeometricMultiLineString:
                case EdmPrimitiveTypeKind.GeometricMultiPoint:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns the primitive type reference for the given Clr type.
        /// </summary>
        /// <param name="clrType">The Clr type to resolve.</param>
        /// <returns>The primitive type reference for the given Clr type.</returns>
        public static IEdmPrimitiveTypeReference GetPrimitiveTypeReference(Type clrType)
        {
            Debug.Assert(clrType != null, "type != null");

            Type targetType = TypeUtils.GetNonNullableType(clrType);
            TypeCode typeCode = Type.GetTypeCode(targetType);
            bool isNullable = TypeUtils.TypeAllowsNull(clrType);

            switch (typeCode)
            {
                case TypeCode.Boolean: return EdmCoreModel.Instance.GetBoolean(isNullable);
                case TypeCode.Byte: return EdmCoreModel.Instance.GetByte(isNullable);
                case TypeCode.DateTime: return EdmCoreModel.Instance.GetTemporalType(EdmPrimitiveTypeKind.DateTime, isNullable);
                case TypeCode.Decimal: return EdmCoreModel.Instance.GetDecimal(isNullable);
                case TypeCode.Double: return EdmCoreModel.Instance.GetDouble(isNullable);
                case TypeCode.Int16: return EdmCoreModel.Instance.GetInt16(isNullable);
                case TypeCode.Int32: return EdmCoreModel.Instance.GetInt32(isNullable);
                case TypeCode.Int64: return EdmCoreModel.Instance.GetInt64(isNullable);
                case TypeCode.SByte: return EdmCoreModel.Instance.GetSByte(isNullable);

                // NOTE: OData only supports nullable strings right now
                case TypeCode.String: return EdmCoreModel.Instance.GetString(true);
                case TypeCode.Single: return EdmCoreModel.Instance.GetSingle(isNullable);
                default:
                    if (targetType == typeof(byte[]))
                    {
                        // NOTE: OData only supports nullable binary values right now
                        return EdmCoreModel.Instance.GetBinary(true);
                    }

                    if (targetType == typeof(Stream))
                    {
                        // stream properties are non-nullable
                        return EdmCoreModel.Instance.GetStream(false);
                    }
                    
                    if (targetType == typeof(DateTimeOffset))
                    {
                        return EdmCoreModel.Instance.GetTemporalType(EdmPrimitiveTypeKind.DateTimeOffset, isNullable);
                    }
                    
                    if (targetType == typeof(Guid))
                    {
                        return EdmCoreModel.Instance.GetGuid(isNullable);
                    }
                    
                    if (targetType == typeof(TimeSpan))
                    {
                        return EdmCoreModel.Instance.GetTemporalType(EdmPrimitiveTypeKind.Time, isNullable);
                    }
                    
                    break;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the provided <paramref name="type"/> is a stream.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the <paramref name="type"/> represents a stream; otherwise false.</returns>
        public static bool IsStream(this IEdmType type)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                Debug.Assert(type.TypeKind != EdmTypeKind.Primitive, "Invalid type kind.");
                return false;
            }

            Debug.Assert(primitiveType.TypeKind == EdmTypeKind.Primitive, "Expected primitive type kind.");
            return primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Stream;
        }

        /// <summary>
        /// Determines whether the provided <paramref name="type"/> is an open type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the <paramref name="type"/> is an open type; otherwise false.</returns>
        public static bool IsOpenType(this IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            IEdmStructuredType structuredType = type as IEdmStructuredType;
            if (structuredType != null)
            {
                return structuredType.IsOpen;
            }

            return false;
        }

        /// <summary>
        /// Get the <see cref="IEdmEntityTypeReference"/> of the item type of the <paramref name="typeReference"/>.
        /// </summary>
        /// <param name="typeReference">The multi value type to get the item type for.</param>
        /// <returns>The item type of the <paramref name="typeReference"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "MultiValue is a valid term.")]
        public static IEdmTypeReference GetMultiValueItemType(this IEdmTypeReference typeReference)
        {
            Debug.Assert(typeReference != null, "typeReference != null");

            IEdmCollectionTypeReference multiValueType = typeReference.ValidateMultiValueType();
            return multiValueType.ElementType();
        }

        /// <summary>
        /// Returns the IEdmCollectionType implementation with the given IEdmTypeReference as element type.
        /// </summary>
        /// <param name="itemType">IEdmTypeReference instance which is the element type.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance using the <paramref name="itemType"/> as MultiValue item type.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "MultiValue is a Name")]
        public static IEdmCollectionType GetMultiValueType(IEdmType itemType)
        {
            ExceptionUtils.CheckArgumentNotNull(itemType, "itemType");

            IEdmTypeReference itemTypeReference;
            if (itemType.IsODataPrimitiveTypeKind())
            {
                IEdmPrimitiveType primitiveType = (IEdmPrimitiveType)itemType;
                if (primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.String || primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Binary)
                {
                    // OData only supports the nullable variants of these types
                    itemTypeReference = itemType.ToTypeReference(true);
                }
                else
                {
                    itemTypeReference = itemType.ToTypeReference();
                }
            }
            else if (itemType.IsODataComplexTypeKind())
            {
                itemTypeReference = itemType.ToTypeReference();
            }
            else
            {
                throw new ArgumentException(Strings.EdmLibraryExtensions_MultiValueItemCanBeOnlyPrimitiveOrComplex);
            }

            return new EdmCollectionType(itemTypeReference, true /*IsAtomic*/);
        }

        /// <summary>
        /// Checks whether the specified <paramref name="property"/> is defined for the type <paramref name="typeReference"/>.
        /// </summary>
        /// <param name="typeReference">The type to check the properties on.</param>
        /// <param name="property">The property to check for.</param>
        /// <returns>true if the <paramref name="property"/> is defined for the <paramref name="typeReference"/>; otherwise false.</returns>
        public static bool ContainsProperty(this IEdmTypeReference typeReference, IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            IEdmStructuredTypeReference structuredTypeReference = typeReference.AsStructuredOrNull();
            if (structuredTypeReference == null)
            {
                return false;
            }

            return ContainsProperty(structuredTypeReference.Definition, property);
        }

        /// <summary>
        /// Checks whether the specified <paramref name="property"/> is defined for the type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to check the properties on.</param>
        /// <param name="property">The property to check for.</param>
        /// <returns>true if the <paramref name="property"/> is defined for the <paramref name="type"/>; otherwise false.</returns>
        public static bool ContainsProperty(this IEdmType type, IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            IEdmComplexType complexType = type as IEdmComplexType;
            if (complexType != null)
            {
                // NOTE: using Any() instead of Contains() since Contains() does not exist on all platforms
                return complexType.Properties().Any(p => p == property);
            }

            IEdmEntityType entityType = type as IEdmEntityType;
            if (entityType != null)
            {
                // NOTE: using Any() instead of Contains() since Contains() does not exist on all platforms
                return entityType.Properties().Any(p => p == property) || 
                       entityType.NavigationProperties().Any(p => p == property);
            }

            // we only support complex and entity types with properties so far
            return false;
        }

        /// <summary>
        /// Gets a reference to a primitive kind definition of the appropriate kind.
        /// </summary>
        /// <param name="primitiveType">Primitive type to create a reference for.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        private static EdmPrimitiveTypeReference PrimitiveTypeReference(IEdmPrimitiveType primitiveType, bool isNullable)
        {
            EdmPrimitiveTypeKind kind = primitiveType.PrimitiveKind;
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
                    return new EdmPrimitiveTypeReference(primitiveType, isNullable);
                case EdmPrimitiveTypeKind.Binary:
                    return new EdmBinaryTypeReference(primitiveType, isNullable);
                case EdmPrimitiveTypeKind.String:
                    return new EdmStringTypeReference(primitiveType, isNullable);
                case EdmPrimitiveTypeKind.Decimal:
                    return new EdmDecimalTypeReference(primitiveType, isNullable);
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Time:
                    return new EdmTemporalTypeReference(primitiveType, isNullable);
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Point:
                case EdmPrimitiveTypeKind.LineString:
                case EdmPrimitiveTypeKind.Polygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.MultiPolygon:
                case EdmPrimitiveTypeKind.MultiLineString:
                case EdmPrimitiveTypeKind.MultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometricPoint:
                case EdmPrimitiveTypeKind.GeometricLineString:
                case EdmPrimitiveTypeKind.GeometricPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometricMultiPolygon:
                case EdmPrimitiveTypeKind.GeometricMultiLineString:
                case EdmPrimitiveTypeKind.GeometricMultiPoint:
                    return new EdmSpatialTypeReference(primitiveType, isNullable);
                default:
                    throw new InvalidOperationException(Strings.General_InternalError(InternalErrorCodes.EdmLibraryExtensions_PrimitiveTypeReference));
            }
        }
    }
}
