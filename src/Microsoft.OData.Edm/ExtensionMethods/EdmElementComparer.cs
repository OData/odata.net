//---------------------------------------------------------------------
// <copyright file="EdmElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Contains IsEquivalentTo() extension methods.
    /// </summary>
    public static class EdmElementComparer
    {
        /// <summary>
        /// Returns true if the compared type is semantically equivalent to this type.
        /// Schema types (<see cref="IEdmSchemaType"/>) are compared by their object refs.
        /// </summary>
        /// <param name="thisType">Type being compared.</param>
        /// <param name="otherType">Type being compared to.</param>
        /// <returns>Equivalence of the two types.</returns>
        public static bool IsEquivalentTo(this IEdmType thisType, IEdmType otherType)
        {
            if (thisType == otherType)
            {
                return true;
            }

            if (thisType == null || otherType == null)
            {
                return false;
            }

            thisType = thisType.AsActualType();
            otherType = otherType.AsActualType();

            if (thisType.TypeKind != otherType.TypeKind)
            {
                return false;
            }

            switch (thisType.TypeKind)
            {
                case EdmTypeKind.Primitive:
                    return ((IEdmPrimitiveType)thisType).IsEquivalentTo((IEdmPrimitiveType)otherType);
                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                case EdmTypeKind.Enum:
                    return ((IEdmSchemaType)thisType).IsEquivalentTo((IEdmSchemaType)otherType);
                case EdmTypeKind.Collection:
                    return ((IEdmCollectionType)thisType).IsEquivalentTo((IEdmCollectionType)otherType);
                case EdmTypeKind.EntityReference:
                    return ((IEdmEntityReferenceType)thisType).IsEquivalentTo((IEdmEntityReferenceType)otherType);
                case EdmTypeKind.None:
                    return otherType.TypeKind == EdmTypeKind.None;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_TypeKind(thisType.TypeKind));
            }
        }

        /// <summary>
        /// Returns true if the compared type reference is semantically equivalent to this type reference.
        /// Schema types (<see cref="IEdmSchemaType"/>) are compared by their object refs.
        /// </summary>
        /// <param name="thisType">Type reference being compared.</param>
        /// <param name="otherType">Type referenced being compared to.</param>
        /// <returns>Equivalence of the two type references.</returns>
        public static bool IsEquivalentTo(this IEdmTypeReference thisType, IEdmTypeReference otherType)
        {
            if (thisType == otherType)
            {
                return true;
            }

            if (thisType == null || otherType == null)
            {
                return false;
            }

            thisType = thisType.AsActualTypeReference();
            otherType = otherType.AsActualTypeReference();

            EdmTypeKind typeKind = thisType.TypeKind();
            if (typeKind != otherType.TypeKind())
            {
                return false;
            }

            if (typeKind == EdmTypeKind.Primitive)
            {
                return ((IEdmPrimitiveTypeReference)thisType).IsEquivalentTo((IEdmPrimitiveTypeReference)otherType);
            }
            else
            {
                return thisType.IsNullable == otherType.IsNullable &&
                       thisType.Definition.IsEquivalentTo(otherType.Definition);
            }
        }

        private static bool IsEquivalentTo(this IEdmPrimitiveType thisType, IEdmPrimitiveType otherType)
        {
            // ODataLib creates one-off instances of primitive type definitions that match by name and kind, but have different object refs.
            // So we can't use object ref comparison here like for other IEdmSchemaType objects.
            // See "src\Web\Client\System\Data\Services\Client\Serialization\PrimitiveType.cs:CreateEdmPrimitiveType()" for more info.
            return thisType.PrimitiveKind == otherType.PrimitiveKind &&
                   thisType.FullName() == otherType.FullName();
        }

        private static bool IsEquivalentTo(this IEdmSchemaType thisType, IEdmSchemaType otherType)
        {
            return Object.ReferenceEquals(thisType, otherType);
        }

        private static bool IsEquivalentTo(this IEdmCollectionType thisType, IEdmCollectionType otherType)
        {
            return thisType.ElementType.IsEquivalentTo(otherType.ElementType);
        }

        private static bool IsEquivalentTo(this IEdmEntityReferenceType thisType, IEdmEntityReferenceType otherType)
        {
            return thisType.EntityType.IsEquivalentTo(otherType.EntityType);
        }

        private static bool IsEquivalentTo(this IEdmPrimitiveTypeReference thisType, IEdmPrimitiveTypeReference otherType)
        {
            EdmPrimitiveTypeKind thisTypePrimitiveKind = thisType.PrimitiveKind();
            if (thisTypePrimitiveKind != otherType.PrimitiveKind())
            {
                return false;
            }

            switch (thisTypePrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    return (thisType as IEdmBinaryTypeReference).IsEquivalentTo(otherType as IEdmBinaryTypeReference);
                case EdmPrimitiveTypeKind.Decimal:
                    return (thisType as IEdmDecimalTypeReference).IsEquivalentTo(otherType as IEdmDecimalTypeReference);
                case EdmPrimitiveTypeKind.String:
                    return (thisType as IEdmStringTypeReference).IsEquivalentTo(otherType as IEdmStringTypeReference);
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return (thisType as IEdmTemporalTypeReference).IsEquivalentTo(otherType as IEdmTemporalTypeReference);
                default:
                    if (thisTypePrimitiveKind.IsSpatial())
                    {
                        return (thisType as IEdmSpatialTypeReference).IsEquivalentTo(otherType as IEdmSpatialTypeReference);
                    }
                    else
                    {
                        return thisType.IsNullable == otherType.IsNullable &&
                                thisType.Definition.IsEquivalentTo(otherType.Definition);
                    }
            }
        }

        private static bool IsEquivalentTo(this IEdmBinaryTypeReference thisType, IEdmBinaryTypeReference otherType)
        {
            return thisType != null && otherType != null &&
                   thisType.IsNullable == otherType.IsNullable &&
                   thisType.IsUnbounded == otherType.IsUnbounded &&
                   thisType.MaxLength == otherType.MaxLength;
        }

        private static bool IsEquivalentTo(this IEdmDecimalTypeReference thisType, IEdmDecimalTypeReference otherType)
        {
            return thisType != null && otherType != null &&
                   thisType.IsNullable == otherType.IsNullable &&
                   thisType.Precision == otherType.Precision &&
                   thisType.Scale == otherType.Scale;
        }

        private static bool IsEquivalentTo(this IEdmTemporalTypeReference thisType, IEdmTemporalTypeReference otherType)
        {
            return thisType != null && otherType != null &&
                   thisType.TypeKind() == otherType.TypeKind() &&
                   thisType.IsNullable == otherType.IsNullable &&
                   thisType.Precision == otherType.Precision;
        }

        private static bool IsEquivalentTo(this IEdmStringTypeReference thisType, IEdmStringTypeReference otherType)
        {
            return thisType != null && otherType != null &&
                   thisType.IsNullable == otherType.IsNullable &&
                   thisType.IsUnbounded == otherType.IsUnbounded &&
                   thisType.MaxLength == otherType.MaxLength &&
                   thisType.IsUnicode == otherType.IsUnicode;
        }

        private static bool IsEquivalentTo(this IEdmSpatialTypeReference thisType, IEdmSpatialTypeReference otherType)
        {
            return thisType != null && otherType != null &&
                   thisType.IsNullable == otherType.IsNullable &&
                   thisType.SpatialReferenceIdentifier == otherType.SpatialReferenceIdentifier;
        }
    }
}
