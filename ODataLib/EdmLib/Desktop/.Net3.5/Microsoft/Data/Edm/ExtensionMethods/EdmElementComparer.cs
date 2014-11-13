//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm
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
        /// <param name="thisFunction">Reference to the calling object.</param>
        /// <param name="otherFunction">Function being compared to.</param>
        /// <returns>Equivalence of signatures of the two functions.</returns>
        internal static bool IsFunctionSignatureEquivalentTo(this IEdmFunctionBase thisFunction, IEdmFunctionBase otherFunction)
        {
            if (thisFunction == otherFunction)
            {
                return true;
            }
            
            if (thisFunction.Name != otherFunction.Name)
            {
                return false;
            }
            
            if (!thisFunction.ReturnType.IsEquivalentTo(otherFunction.ReturnType))
            {
                return false;
            }

            IEnumerator<IEdmFunctionParameter> otherFunctionParameterEnumerator = otherFunction.Parameters.GetEnumerator();
            foreach (IEdmFunctionParameter parameter in thisFunction.Parameters)
            {
                otherFunctionParameterEnumerator.MoveNext();
                if (!parameter.IsEquivalentTo(otherFunctionParameterEnumerator.Current))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the compared function parameter is semantically equivalent to this function parameter.
        /// </summary>
        /// <param name="thisParameter">Reference to the calling object.</param>
        /// <param name="otherParameter">Function parameter being compared to.</param>
        /// <returns>Equivalence of the two function parameters.</returns>
        private static bool IsEquivalentTo(this IEdmFunctionParameter thisParameter, IEdmFunctionParameter otherParameter)
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
                   thisParameter.Mode == otherParameter.Mode &&
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
                case EdmPrimitiveTypeKind.Time:
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
