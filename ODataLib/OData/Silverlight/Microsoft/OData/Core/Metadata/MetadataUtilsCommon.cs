//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Metadata
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Core.UriParser.Semantic;
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
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            return type.TypeKind == EdmTypeKind.Complex;
        }

        /// <summary>
        /// Checks whether a type reference refers to an OData enumeration type.
        /// </summary>
        /// <param name="typeReference">The (non-null) <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is an OData enumeration type reference; otherwise false.</returns>
        internal static bool IsODataEnumTypeKind(this IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(typeReference.Definition, "typeReference.Definition");

            return typeReference.Definition.IsODataEnumTypeKind();
        }

        /// <summary>
        /// Checks whether a type refers to an OData Enumeration type
        /// </summary>
        /// <param name="type">The (non-null) <see cref="IEdmType"/> to check.</param>
        /// <returns>true if the <paramref name="type"/> is an OData enumeration type; otherwise false.</returns>
        internal static bool IsODataEnumTypeKind(this IEdmType type)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            EdmTypeKind typeKind = type.TypeKind;
            return typeKind == EdmTypeKind.Enum;
        }

        /// <summary>
        /// Checks whether a type reference refers to an OData entity type.
        /// </summary>
        /// <param name="typeReference">The (non-null) <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is an OData entity type reference; otherwise false.</returns>
        internal static bool IsODataEntityTypeKind(this IEdmTypeReference typeReference)
        {
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
            IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitiveOrNull();
            if (primitiveTypeReference == null)
            {
                return false;
            }

            switch (primitiveTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Decimal:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Duration:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether a type reference refers to a OData collection value type of non-entity elements.
        /// </summary>
        /// <param name="typeReference">The (non-null) <see cref="IEdmType"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is a non-entity OData collection value type; otherwise false.</returns>
        internal static bool IsNonEntityCollectionType(this IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(typeReference.Definition, "typeReference.Definition");

            return typeReference.Definition.IsNonEntityCollectionType();
        }

        /// <summary>
        /// Checks whether a type refers to a OData collection value type of non-entity elements.
        /// </summary>
        /// <param name="type">The (non-null) <see cref="IEdmType"/> to check.</param>
        /// <returns>true if the <paramref name="type"/> is a non-entity OData collection value type; otherwise false.</returns>
        internal static bool IsNonEntityCollectionType(this IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            IEdmCollectionType collectionType = type as IEdmCollectionType;

            // Return false if this is not a collection type, or if it's a collection of entity types (i.e., a navigation property)
            if (collectionType == null || (collectionType.ElementType != null && collectionType.ElementType.TypeKind() == EdmTypeKind.Entity))
            {
                return false;
            }

            Debug.Assert(collectionType.TypeKind == EdmTypeKind.Collection, "Expected collection type kind.");
            return true;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmPrimitiveTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmPrimitiveTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmPrimitiveTypeReference AsPrimitiveOrNull(this IEdmTypeReference typeReference)
        {
            if (typeReference == null)
            {
                return null;
            }

            return typeReference.TypeKind() == EdmTypeKind.Primitive ? typeReference.AsPrimitive() : null;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmEntityTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmComplexTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmEntityTypeReference AsEntityOrNull(this IEdmTypeReference typeReference)
        {
            if (typeReference == null)
            {
                return null;
            }

            return typeReference.TypeKind() == EdmTypeKind.Entity ? typeReference.AsEntity() : null;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmStructuredTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmStructuredTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmStructuredTypeReference AsStructuredOrNull(this IEdmTypeReference typeReference)
        {
            if (typeReference == null)
            {
                return null;
            }

            return typeReference.IsStructured() ? typeReference.AsStructured() : null;
        }

        /// <summary>
        /// Determines if a <paramref name="sourcePrimitiveType"/> is convertibale according to OData rules to the
        /// <paramref name="targetPrimitiveType"/>.
        /// </summary>
        /// <param name="sourceNodeOrNull">The node which is to be converted.</param>
        /// <param name="sourcePrimitiveType">The type which is to be converted.</param>
        /// <param name="targetPrimitiveType">The type to which we want to convert.</param>
        /// <returns>true if the source type is convertible to the target type; otherwise false.</returns>
        internal static bool CanConvertPrimitiveTypeTo(
            SingleValueNode sourceNodeOrNull,
            IEdmPrimitiveType sourcePrimitiveType,
            IEdmPrimitiveType targetPrimitiveType)
        {
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

                        // allow single constant->decimal in order to made L,M,D,F optional
                        case EdmPrimitiveTypeKind.Decimal:
                            object tmp;
                            return TryGetConstantNodePrimitiveLDMF(sourceNodeOrNull, out tmp);
                    }

                    break;
                case EdmPrimitiveTypeKind.Double:
                    switch (targetPrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Double:
                            return true;

                        // allow double constant->decimal in order to made L,M,D,F optional
                        // (if the double value actually has overflowed decimal.MaxValue, exception will occur later. more details in ExpressionLexer.cs)
                        case EdmPrimitiveTypeKind.Decimal:
                            object tmp;
                            return TryGetConstantNodePrimitiveLDMF(sourceNodeOrNull, out tmp);
                    }

                    break;
                default:
                    return sourcePrimitiveKind == targetPrimitiveKind || targetPrimitiveType.IsAssignableFrom(sourcePrimitiveType);
            }

            return false;
        }

        /// <summary>
        /// Tries getting the constant node's primitive L M D F value (which can be converted to other primitive type while primitive AccessPropertyNode can't).
        /// </summary>
        /// <param name="sourceNodeOrNull">The Node</param>
        /// <param name="primitiveValue">THe out parameter if succeeds</param>
        /// <returns>true if the constant node is for long, float, double or decimal type</returns>
        internal static bool TryGetConstantNodePrimitiveLDMF(SingleValueNode sourceNodeOrNull, out object primitiveValue)
        {
            primitiveValue = null;
            if ((sourceNodeOrNull != null) && (sourceNodeOrNull is ConstantNode))
            {
                ConstantNode tmp = (ConstantNode)sourceNodeOrNull;
                IEdmPrimitiveType primitiveType = tmp.TypeReference.AsPrimitiveOrNull().Definition as IEdmPrimitiveType;
                if (primitiveType != null)
                {
                    switch (primitiveType.PrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Decimal:
                            primitiveValue = tmp.Value;
                            return true;
                        default:
                            return false;
                    }
                }
            }

            return false;
        }
    }
}
