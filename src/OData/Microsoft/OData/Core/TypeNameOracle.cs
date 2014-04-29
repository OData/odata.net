//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class to validate and resolve the type name to be serialized.
    /// </summary>
    internal class TypeNameOracle
    {
        /// <summary>
        /// Validates a type name to ensure that it's not an empty string and resolves it against the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeName">The type name to validate.</param>
        /// <param name="expectedTypeKind">The expected type kind for the given type name.</param>
        /// <returns>The type with the given name and kind if a user model was available, otherwise null.</returns>
        internal static IEdmType ResolveAndValidateTypeName(IEdmModel model, string typeName, EdmTypeKind expectedTypeKind)
        {
            Debug.Assert(model != null, "model != null");

            if (typeName == null)
            {
                // if we have metadata, the type name of an entry must not be null
                if (model.IsUserModel())
                {
                    throw new ODataException(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
                }

                return null;
            }

            if (typeName.Length == 0)
            {
                throw new ODataException(Strings.ValidationUtils_TypeNameMustNotBeEmpty);
            }

            if (!model.IsUserModel())
            {
                return null;
            }

            // If we do have metadata, lookup the type and translate it to a type.
            IEdmType resolvedType = MetadataUtils.ResolveTypeNameForWrite(model, typeName);
            if (resolvedType == null)
            {
                throw new ODataException(Strings.ValidationUtils_UnrecognizedTypeName(typeName));
            }

            ValidationUtils.ValidateTypeKind(resolvedType.TypeKind, expectedTypeKind, resolvedType.ODataFullName());
            return resolvedType;
        }

        /// <summary>
        /// Resolves and validates the Edm type for the given <paramref name="value"/>.
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeReferenceFromMetadata">The type inferred from the model or null if the model is not a user model.</param>
        /// <param name="value">The value in question to resolve the type for.</param>
        /// <param name="isOpenProperty">true if the type name belongs to an open property, false otherwise.</param>
        /// <returns>A type for the <paramref name="value"/> or null if no metadata is available.</returns>
        internal static IEdmTypeReference ResolveAndValidateTypeNameForValue(IEdmModel model, IEdmTypeReference typeReferenceFromMetadata, ODataValue value, bool isOpenProperty)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(value != null, "value != null");

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                Debug.Assert(primitiveValue.Value != null, "primitiveValue.Value != null");
                return EdmLibraryExtensions.GetPrimitiveTypeReference(primitiveValue.Value.GetType());
            }

            ODataComplexValue complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                return ResolveAndValidateTypeFromNameAndMetadata(model, typeReferenceFromMetadata, complexValue.TypeName, EdmTypeKind.Complex, isOpenProperty);
            }

            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                return ResolveAndValidateTypeFromNameAndMetadata(model, typeReferenceFromMetadata, enumValue.TypeName, EdmTypeKind.Enum, isOpenProperty);
            }

            ODataCollectionValue collectionValue = (ODataCollectionValue)value;
            return ResolveAndValidateTypeFromNameAndMetadata(model, typeReferenceFromMetadata, collectionValue.TypeName, EdmTypeKind.Collection, isOpenProperty);
        }

        /// <summary>
        /// Gets the type name from the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to get the type name from. This can be an ODataPrimitiveValue, an ODataComplexValue, an ODataCollectionValue or a Clr primitive object.</param>
        /// <returns>The type name for the given <paramref name="value"/>.</returns>
        protected static string GetTypeNameFromValue(object value)
        {
            Debug.Assert(value != null, "value != null");

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                return EdmLibraryExtensions.GetPrimitiveTypeReference(primitiveValue.Value.GetType()).ODataFullName();
            }

            ODataComplexValue complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                return complexValue.TypeName;
            }

            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                return enumValue.TypeName;
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                return EdmLibraryExtensions.GetCollectionTypeFullName(collectionValue.TypeName);
            }

            IEdmPrimitiveTypeReference primitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
            if (primitiveTypeReference == null)
            {
                throw new ODataException(Strings.ValidationUtils_UnsupportedPrimitiveType(value.GetType().FullName));
            }

            return primitiveTypeReference.ODataFullName();
        }

        /// <summary>
        /// Resolve a type name against the provided <paramref name="model"/>. If not payload type name is specified,
        /// derive the type from the model type (if available).
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeReferenceFromMetadata">The type inferred from the model or null if the model is not a user model.</param>
        /// <param name="typeName">The type name to be resolved.</param>
        /// <param name="typeKindFromValue">The expected type kind of the resolved type.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <returns>A type for the <paramref name="typeName"/> or null if no type name is specified and no metadata is available.</returns>
        private static IEdmTypeReference ResolveAndValidateTypeFromNameAndMetadata(IEdmModel model, IEdmTypeReference typeReferenceFromMetadata, string typeName, EdmTypeKind typeKindFromValue, bool isOpenPropertyType)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(
                typeKindFromValue == EdmTypeKind.Complex || typeKindFromValue == EdmTypeKind.Collection || typeKindFromValue == EdmTypeKind.Enum,
                "Only complex or collection or enum types are supported by this method. This method assumes that the types in question don't support inheritance.");

            // if we have metadata, the type name of an open property value must not be null
            if (typeName == null && model.IsUserModel() && isOpenPropertyType)
            {
                throw new ODataException(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
            }

            // starting from enum type, we want to skip validation (but still let the above makes sure open type's enum value has .TypeName)
            if (typeKindFromValue == EdmTypeKind.Enum)
            {
                return null;
            }

            IEdmType typeFromValue = typeName == null ? null : ResolveAndValidateTypeName(model, typeName, typeKindFromValue);
            if (typeReferenceFromMetadata != null)
            {
                ValidationUtils.ValidateTypeKind(typeKindFromValue, typeReferenceFromMetadata.TypeKind(), typeFromValue == null ? null : typeFromValue.ODataFullName());
            }

            IEdmTypeReference typeReferenceFromValue = ValidateMetadataType(typeReferenceFromMetadata, typeFromValue == null ? null : typeFromValue.ToTypeReference());
            if (typeKindFromValue == EdmTypeKind.Collection && typeReferenceFromValue != null)
            {
                // update nullability from metadata
                if (typeReferenceFromMetadata != null)
                {
                    typeReferenceFromValue = typeReferenceFromMetadata;
                }

                // validate that the collection type represents a valid Collection type (e.g., is unordered).
                typeReferenceFromValue = ValidationUtils.ValidateCollectionType(typeReferenceFromValue);
            }

            return typeReferenceFromValue;
        }

        /// <summary>
        /// Validates that the (optional) <paramref name="typeReferenceFromMetadata"/> is the same as the (optional) <paramref name="typeReferenceFromValue"/>.
        /// </summary>
        /// <param name="typeReferenceFromMetadata">The (optional) type from the metadata definition (the expected type).</param>
        /// <param name="typeReferenceFromValue">The (optional) type from the value (the actual type).</param>
        /// <returns>The type as derived from the <paramref name="typeReferenceFromMetadata"/> and/or <paramref name="typeReferenceFromValue"/>.</returns>
        private static IEdmTypeReference ValidateMetadataType(IEdmTypeReference typeReferenceFromMetadata, IEdmTypeReference typeReferenceFromValue)
        {
            if (typeReferenceFromMetadata == null)
            {
                // if we have no metadata information there is nothing to validate
                return typeReferenceFromValue;
            }

            if (typeReferenceFromValue == null)
            {
                // derive the property type from the metadata
                return typeReferenceFromMetadata;
            }

            Debug.Assert(typeReferenceFromValue.TypeKind() == typeReferenceFromMetadata.TypeKind(), "typeReferenceFromValue.TypeKind() == typeReferenceFromMetadata.TypeKind()");

            // Make sure the types are the same
            if (typeReferenceFromValue.IsODataPrimitiveTypeKind())
            {
                // Primitive types must match exactly except for nullability
                ValidationUtils.ValidateMetadataPrimitiveType(typeReferenceFromMetadata, typeReferenceFromValue);
            }
            else if (typeReferenceFromMetadata.IsEntity())
            {
                ValidationUtils.ValidateEntityTypeIsAssignable((IEdmEntityTypeReference)typeReferenceFromMetadata, (IEdmEntityTypeReference)typeReferenceFromValue);
            }
            else if (typeReferenceFromMetadata.IsComplex())
            {
                ValidationUtils.ValidateComplexTypeIsAssignable(typeReferenceFromMetadata.Definition as IEdmComplexType, typeReferenceFromValue.Definition as IEdmComplexType);
            }
            else
            {
                // Collection types must match exactly
                if (typeReferenceFromMetadata.ODataFullName() != typeReferenceFromValue.ODataFullName())
                {
                    throw new ODataException(Strings.ValidationUtils_IncompatibleType(typeReferenceFromValue.ODataFullName(), typeReferenceFromMetadata.ODataFullName()));
                }
            }

            return typeReferenceFromValue;
        }
    }
}
