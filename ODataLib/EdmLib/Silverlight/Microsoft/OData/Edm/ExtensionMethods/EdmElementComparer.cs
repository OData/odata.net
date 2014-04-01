//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Expressions;

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
                case EdmTypeKind.Row:
                    return ((IEdmRowType)thisType).IsEquivalentTo((IEdmRowType)otherType);
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

        /// <summary>
        /// Returns true if function signatures are semantically equivalent.
        /// Signature includes function name (<see cref="IEdmNamedElement"/>) and its parameter types.
        /// </summary>
        /// <param name="thisOperation">Reference to the calling object.</param>
        /// <param name="otherOperation">Function being compared to.</param>
        /// <returns>Equivalence of signatures of the two functions.</returns>
        internal static bool IsFunctionSignatureEquivalentTo(this IEdmFunctionBase thisOperation, IEdmFunctionBase otherOperation)
        {
            if (thisOperation == otherOperation)
            {
                return true;
            }
            
            if (thisOperation.Name != otherOperation.Name)
            {
                return false;
            }
            
            if (!thisOperation.ReturnType.IsEquivalentTo(otherOperation.ReturnType))
            {
                return false;
            }

            IEnumerator<IEdmOperationParameter> otherOperationParameter = otherOperation.Parameters.GetEnumerator();
            foreach (IEdmOperationParameter parameter in thisOperation.Parameters)
            {
                otherOperationParameter.MoveNext();
                if (!parameter.IsEquivalentTo(otherOperationParameter.Current))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the compared operation parameter is semantically equivalent to this operation parameter.
        /// </summary>
        /// <param name="thisParameter">Reference to the calling object.</param>
        /// <param name="otherParameter">Operation parameter being compared to.</param>
        /// <returns>Equivalence of the two operation parameters.</returns>
        private static bool IsEquivalentTo(this IEdmOperationParameter thisParameter, IEdmOperationParameter otherParameter)
        {
            if (thisParameter == otherParameter)
            {
                return true;
            }
            
            if (thisParameter == null || otherParameter == null)
            {
                return false;
            }

            return thisParameter.Name == otherParameter.Name &&
                   thisParameter.Type.IsEquivalentTo(otherParameter.Type);
        }

        private static bool IsEquivalentTo(this IEdmPrimitiveType thisType, IEdmPrimitiveType otherType)
        {
            // ODataLib creates one-off instances of primitive type definitions that match by name and kind, but have different object refs.
            // So we can't use object ref comparison here like for other IEdmSchemaType objects.
            // See "src\DataWeb\Client\System\Data\Services\Client\Serialization\PrimitiveType.cs:CreateEdmPrimitiveType()" for more info.
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

        private static bool IsEquivalentTo(this IEdmRowType thisType, IEdmRowType otherType)
        {
            if (thisType.DeclaredProperties.Count() != otherType.DeclaredProperties.Count())
            {
                return false;
            }

            IEnumerator<IEdmProperty> thisTypePropertyEnumerator = thisType.DeclaredProperties.GetEnumerator();
            foreach (IEdmProperty otherTypeProperty in otherType.DeclaredProperties)
            {
                thisTypePropertyEnumerator.MoveNext();
                if (!((IEdmStructuralProperty)thisTypePropertyEnumerator.Current).IsEquivalentTo((IEdmStructuralProperty)otherTypeProperty))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsEquivalentTo(this IEdmStructuralProperty thisProp, IEdmStructuralProperty otherProp)
        {
            if (thisProp == otherProp)
            {
                return true;
            }

            if (thisProp == null || otherProp == null)
            {
                return false;
            }

            return thisProp.Name == otherProp.Name &&
                   thisProp.Type.IsEquivalentTo(otherProp.Type);
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
                    return ((IEdmBinaryTypeReference)thisType).IsEquivalentTo((IEdmBinaryTypeReference)otherType);
                case EdmPrimitiveTypeKind.Decimal:
                    return ((IEdmDecimalTypeReference)thisType).IsEquivalentTo((IEdmDecimalTypeReference)otherType);
                case EdmPrimitiveTypeKind.String:
                    return ((IEdmStringTypeReference)thisType).IsEquivalentTo((IEdmStringTypeReference)otherType);
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return ((IEdmTemporalTypeReference)thisType).IsEquivalentTo((IEdmTemporalTypeReference)otherType);
                default:
                    if (thisTypePrimitiveKind.IsSpatial())
                    {
                        return ((IEdmSpatialTypeReference)thisType).IsEquivalentTo((IEdmSpatialTypeReference)otherType);
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
            return thisType.IsNullable == otherType.IsNullable &&
                   thisType.IsFixedLength == otherType.IsFixedLength &&
                   thisType.IsUnbounded == otherType.IsUnbounded &&
                   thisType.MaxLength == otherType.MaxLength;
        }

        private static bool IsEquivalentTo(this IEdmDecimalTypeReference thisType, IEdmDecimalTypeReference otherType)
        {
            return thisType.IsNullable == otherType.IsNullable &&
                   thisType.Precision == otherType.Precision &&
                   thisType.Scale == otherType.Scale;
        }

        private static bool IsEquivalentTo(this IEdmTemporalTypeReference thisType, IEdmTemporalTypeReference otherType)
        {
            return thisType.TypeKind() == otherType.TypeKind() &&
                   thisType.IsNullable == otherType.IsNullable &&
                   thisType.Precision == otherType.Precision;
        }

        private static bool IsEquivalentTo(this IEdmStringTypeReference thisType, IEdmStringTypeReference otherType)
        {
            return thisType.IsNullable == otherType.IsNullable &&
                   thisType.IsFixedLength == otherType.IsFixedLength &&
                   thisType.IsUnbounded == otherType.IsUnbounded &&
                   thisType.MaxLength == otherType.MaxLength &&
                   thisType.IsUnicode == otherType.IsUnicode &&
                   thisType.Collation == otherType.Collation;
        }

        private static bool IsEquivalentTo(this IEdmSpatialTypeReference thisType, IEdmSpatialTypeReference otherType)
        {
            return thisType.IsNullable == otherType.IsNullable &&
                   thisType.SpatialReferenceIdentifier == otherType.SpatialReferenceIdentifier;
        }
    }
}
