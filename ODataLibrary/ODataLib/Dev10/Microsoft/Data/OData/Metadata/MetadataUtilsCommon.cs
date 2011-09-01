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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for dealing with OData metadata that are shared with the OData.Query project.
    /// </summary>
    internal static class MetadataUtilsCommon
    {
        /// <summary>
        /// Checks whether a type reference refers to an OData primitive type (i.e., a primitive, non-stream type).
        /// </summary>
        /// <param name="typeReference">The (non-null) <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is an OData primitive type reference; otherwise false.</returns>
        internal static bool IsODataPrimitiveTypeKind(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(typeReference.Definition, "typeReference.Definition");

            return typeReference.Definition.IsODataPrimitiveTypeKind();
        }

        /// <summary>
        /// Checks whether a type refers to an OData primitive type (i.e., a primitive, non-stream type).
        /// </summary>
        /// <param name="type">The (non-null) <see cref="IEdmType"/> to check.</param>
        /// <returns>true if the <paramref name="type"/> is an OData primitive type; otherwise false.</returns>
        internal static bool IsODataPrimitiveTypeKind(this IEdmType type)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            EdmTypeKind typeKind = type.TypeKind;
            if (typeKind != EdmTypeKind.Primitive)
            {
                return false;
            }

            // also make sure it is not a stream
            return !type.IsStream();
        }

        /// <summary>
        /// Checks whether a type reference refers to an OData complex type.
        /// </summary>
        /// <param name="typeReference">The (non-null) <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is an OData complex type reference; otherwise false.</returns>
        internal static bool IsODataComplexTypeKind(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(typeReference.Definition, "typeReference.Definition");

            return typeReference.Definition.IsODataComplexTypeKind();
        }

        /// <summary>
        /// Checks whether a type refers to an OData complex type.
        /// </summary>
        /// <param name="type">The (non-null) <see cref="IEdmType"/> to check.</param>
        /// <returns>true if the <paramref name="type"/> is an OData complex type; otherwise false.</returns>
        internal static bool IsODataComplexTypeKind(this IEdmType type)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            return type.TypeKind == EdmTypeKind.Complex;
        }

        /// <summary>
        /// Checks whether a type reference refers to an OData entity type.
        /// </summary>
        /// <param name="typeReference">The (non-null) <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is an OData entity type reference; otherwise false.</returns>
        internal static bool IsODataEntityTypeKind(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(typeReference.Definition, "typeReference.Definition");

            return typeReference.Definition.IsODataEntityTypeKind();
        }

        /// <summary>
        /// Checks whether a type refers to an OData entity type.
        /// </summary>
        /// <param name="type">The (non-null) <see cref="IEdmType"/> to check.</param>
        /// <returns>true if the <paramref name="type"/> is an OData entity type; otherwise false.</returns>
        internal static bool IsODataEntityTypeKind(this IEdmType type)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            return type.TypeKind == EdmTypeKind.Entity;
        }

        /// <summary>
        /// Checks whether a type reference is considered a value type in OData.
        /// </summary>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is considered a value type; otherwise false.</returns>
        /// <remarks>
        /// The notion of value type in the OData space is driven by the IDSMP requirements where 
        /// Clr types denote the primitive types.
        /// </remarks>
        internal static bool IsODataValueType(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitiveOrNull();
            if (primitiveTypeReference == null)
            {
                return false;
            }

            switch (primitiveTypeReference.PrimitiveKind())
            {
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
                case EdmPrimitiveTypeKind.Time:
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
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether a type reference refers to an OData multi value type (i.e., an unordered collection type).
        /// </summary>
        /// <param name="typeReference">The (non-null) <see cref="IEdmType"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is an OData multi value type; otherwise false.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "MultiValue is a valid term.")]
        internal static bool IsODataMultiValueTypeKind(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(typeReference.Definition, "typeReference.Definition");

            return typeReference.Definition.IsODataMultiValueTypeKind();
        }

        /// <summary>
        /// Checks whether a type refers to an OData multi value type (i.e., an unordered collection type).
        /// </summary>
        /// <param name="type">The (non-null) <see cref="IEdmType"/> to check.</param>
        /// <returns>true if the <paramref name="type"/> is an OData multi value type; otherwise false.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "MultiValue is a valid term.")]
        internal static bool IsODataMultiValueTypeKind(this IEdmType type)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(type != null, "type != null");

            IEdmCollectionType multiValueType = type as IEdmCollectionType;
            if (multiValueType == null  || !multiValueType.IsAtomic)
            {
                return false;
            }

            Debug.Assert(multiValueType.TypeKind == EdmTypeKind.Collection, "Expected multi value type kind.");
            return true;
        }

        /// <summary>
        /// Gets the full name of the definition referred to by the type reference.
        /// </summary>
        /// <param name="typeReference">The type reference to get the full name for.</param>
        /// <returns>The full name of this <paramref name="typeReference"/>.</returns>
        /// <remarks>
        /// Note that this method is different from the EdmLib FullName extension method in that it also returns
        /// names for multi value types. For EdmLib, multi value types are functions and thus don't have a full name.
        /// The name/string they use in CSDL is just shorthand for them.
        /// </remarks>
        internal static string ODataFullName(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            return typeReference.Definition.ODataFullName();
        }

        /// <summary>
        /// Gets the full name of the type.
        /// </summary>
        /// <param name="type">The type to get the full name for.</param>
        /// <returns>The full name of the <paramref name="type"/>.</returns>
        /// <remarks>
        /// Note that this method is different from the EdmLib FullName extension method in that it also returns
        /// names for multi value types. For EdmLib, multi value types are functions and thus don't have a full name.
        /// The name/string they use in CSDL is just shorthand for them.
        /// </remarks>
        internal static string ODataFullName(this IEdmType type)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(type != null, "type != null");

            // Handle multi value type names here since for EdmLib multi values are functions
            // that do not have a full name
            IEdmCollectionType collectionType = type as IEdmCollectionType;
            if (collectionType != null)
            {
                string elementTypeName = collectionType.ElementType.ODataFullName();
                if (elementTypeName == null)
                {
                    return null;
                }

                if (collectionType.IsAtomic)
                {
                    return "MultiValue(" + elementTypeName + ")";
                }

                return "Collection(" + elementTypeName + ")";
            }

            var namedDefinition = type as IEdmSchemaElement;
            return namedDefinition != null ? namedDefinition.FullName() : null;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmPrimitiveTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmPrimitiveTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmPrimitiveTypeReference AsPrimitiveOrNull(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference == null)
            {
                return null;
            }

            IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitive();
            return primitiveTypeReference.IsBad() ? null : primitiveTypeReference;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmComplexTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmComplexTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmEntityTypeReference AsEntityOrNull(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference == null)
            {
                return null;
            }

            IEdmEntityTypeReference entityTypeReference = typeReference.AsEntity();
            return entityTypeReference.IsBad() ? null : entityTypeReference;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmStructuredTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmStructuredTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmStructuredTypeReference AsStructuredOrNull(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference == null)
            {
                return null;
            }

            IEdmStructuredTypeReference structuredTypeReference = typeReference.AsStructured();
            return structuredTypeReference.IsBad() ? null : structuredTypeReference;
        }

        /// <summary>
        /// Determines if a <paramref name="sourcePrimitiveType"/> is convertibale according to OData rules to the
        /// <paramref name="targetPrimitiveType"/>.
        /// </summary>
        /// <param name="sourcePrimitiveType">The type which is to be converted.</param>
        /// <param name="targetPrimitiveType">The type to which we want to convert.</param>
        /// <returns>true if the source type is convertible to the target type; otherwise false.</returns>
        internal static bool CanConvertPrimitiveTypeTo(
            IEdmPrimitiveType sourcePrimitiveType,
            IEdmPrimitiveType targetPrimitiveType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(sourcePrimitiveType != null, "sourcePrimitiveType != null");
            Debug.Assert(targetPrimitiveType != null, "targetPrimitiveType != null");

            EdmPrimitiveTypeKind sourcePrimitiveKind = sourcePrimitiveType.PrimitiveKind;
            EdmPrimitiveTypeKind targetPrimitiveKind = targetPrimitiveType.PrimitiveKind;
            switch (sourcePrimitiveKind)
            {
                case EdmPrimitiveTypeKind.SByte:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.SByte:
                        case EdmPrimitiveTypeKind.Int16:
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Decimal:
                            return true;
                    }

                    break;
                case EdmPrimitiveTypeKind.Byte:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Byte:
                        case EdmPrimitiveTypeKind.Int16:
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Decimal:
                            return true;
                    }

                    break;
                case EdmPrimitiveTypeKind.Int16:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int16:
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Decimal:
                            return true;
                    }

                    break;
                case EdmPrimitiveTypeKind.Int32:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Decimal:
                            return true;
                    }

                    break;
                case EdmPrimitiveTypeKind.Int64:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Decimal:
                            return true;
                    }

                    break;
                case EdmPrimitiveTypeKind.Single:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Double:
                            return true;
                    }

                    break;
                default:
                    if (sourcePrimitiveKind == targetPrimitiveKind)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }
    }
}
