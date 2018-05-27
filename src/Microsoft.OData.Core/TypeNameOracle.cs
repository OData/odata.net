//---------------------------------------------------------------------
// <copyright file="TypeNameOracle.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
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
        /// <param name="expectStructuredType">This value indicates if a structured type is expected to be return.
        /// True for structured type, false for non-structured type, null for indetermination.</param>
        /// <param name="writerValidator">The writer validator to use for validation.</param>
        /// <returns>The type with the given name and kind if a user model was available, otherwise null.</returns>
        internal static IEdmType ResolveAndValidateTypeName(IEdmModel model, string typeName, EdmTypeKind expectedTypeKind, bool? expectStructuredType, IWriterValidator writerValidator)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(
                !expectStructuredType.HasValue
                || !expectStructuredType.Value && !expectedTypeKind.IsStructured()
                || expectStructuredType.Value && (expectedTypeKind.IsStructured() || expectedTypeKind == EdmTypeKind.None),
                "!expectStructuredType.HasValue || !expectStructuredType.Value && !expectedTypeKind.IsStructured() || expectStructuredType.Value && (expectedTypeKind.IsStructured() || expectedTypeKind == EdmTypeKind.None)");

            if (typeName == null)
            {
                // if we have metadata, the type name of a resource must not be null
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

            if (resolvedType.TypeKind != EdmTypeKind.Untyped)
            {
                writerValidator.ValidateTypeKind(resolvedType.TypeKind, expectedTypeKind, expectStructuredType, resolvedType);
            }

            return resolvedType;
        }

        /// <summary>
        /// Resolve a resource type name
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="expectedType">The type inferred from the model or null if the model is not a user model.</param>
        /// <param name="typeName">Name of the type to resolve.</param>
        /// <param name="writerValidator">The writer validator to use for validation.</param>
        /// <returns>A type for primitive value</returns>
        internal static IEdmStructuredType ResolveAndValidateTypeFromTypeName(IEdmModel model, IEdmStructuredType expectedType, string typeName, IWriterValidator writerValidator)
        {
            if (typeName == null && expectedType != null)
            {
                return expectedType;
            }

            // TODO: Clean up handling of expected types/sets during writing
            IEdmType typeFromResource = ResolveAndValidateTypeName(model, typeName, EdmTypeKind.None, /* expectStructuredType */ true, writerValidator);
            IEdmTypeReference typeReferenceFromValue = ResolveTypeFromMetadataAndValue(
                expectedType.ToTypeReference(),
                typeFromResource == null ? null : typeFromResource.ToTypeReference(),
                writerValidator);

            if (typeReferenceFromValue != null && typeReferenceFromValue.IsUntyped())
            {
                return new EdmUntypedStructuredType();
            }

            return typeReferenceFromValue == null ? null : typeReferenceFromValue.ToStructuredType();
        }

        /// <summary>
        /// Resolve a primitive value type name
        /// </summary>
        /// <param name="primitiveValue">The value to get the type name from.</param>
        /// <returns>A type for primitive value</returns>
        internal static IEdmTypeReference ResolveAndValidateTypeForPrimitiveValue(ODataPrimitiveValue primitiveValue)
        {
            return EdmLibraryExtensions.GetPrimitiveTypeReference(primitiveValue.Value.GetType());
        }

        /// <summary>
        /// Resolve a type name against the provided <paramref name="model"/>. If not payload type name is specified,
        /// derive the type from the model type (if available).
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="enumValue">The value in question to resolve the type for.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <returns>A type for the <paramref name="enumValue"/> or null if no type name is specified and no metadata is available.</returns>
        internal static IEdmTypeReference ResolveAndValidateTypeForEnumValue(IEdmModel model, ODataEnumValue enumValue, bool isOpenPropertyType)
        {
            Debug.Assert(model != null, "model != null");

            ValidateIfTypeNameMissing(enumValue.TypeName, model, isOpenPropertyType);

            // starting from enum type, we want to skip validation (but still let the above makes sure open type's enum value has .TypeName)
            return null;
        }

        /// <summary>
        /// Resolve a type name against the provided <paramref name="model"/>. If not payload type name is specified,
        /// derive the type from the model type (if available).
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeReferenceFromMetadata">The type inferred from the model or null if the model is not a user model.</param>
        /// <param name="collectionValue">The value in question to resolve the type for.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <param name="writerValidator">The writer validator to use for validation.</param>
        /// <returns>A type for the <paramref name="collectionValue"/> or null if no type name is specified and no metadata is available.</returns>
        internal static IEdmTypeReference ResolveAndValidateTypeForCollectionValue(IEdmModel model, IEdmTypeReference typeReferenceFromMetadata, ODataCollectionValue collectionValue, bool isOpenPropertyType, IWriterValidator writerValidator)
        {
            Debug.Assert(model != null, "model != null");

            var typeName = collectionValue.TypeName;

            ValidateIfTypeNameMissing(typeName, model, isOpenPropertyType);

            IEdmType typeFromValue = typeName == null ? null : ResolveAndValidateTypeName(model, typeName, EdmTypeKind.Collection, false, writerValidator);
            if (typeReferenceFromMetadata != null)
            {
                writerValidator.ValidateTypeKind(EdmTypeKind.Collection, typeReferenceFromMetadata.TypeKind(), false, typeFromValue);
            }

            IEdmTypeReference typeReferenceFromValue = ResolveTypeFromMetadataAndValue(typeReferenceFromMetadata, typeFromValue == null ? null : typeFromValue.ToTypeReference(), writerValidator);
            if (typeReferenceFromValue != null)
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
        /// Try to get type name from ODataValue annotation.
        /// </summary>
        /// <param name="value">The value to get type annotation.</param>
        /// <param name="propertyName">The type name from annotation</param>
        /// <returns>True if there is type name annotation.</returns>
        internal static bool TryGetTypeNameFromAnnotation(ODataValue value, out string propertyName)
        {
            if (value.TypeAnnotation != null)
            {
                propertyName = value.TypeAnnotation.TypeName;
                return true;
            }

            propertyName = null;
            return false;
        }

        /// <summary>
        /// Gets the type name from the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to get the type name from. This can be an ODataPrimitiveValue, an ODataCollectionValue or a Clr primitive object.</param>
        /// <returns>The type name for the given <paramref name="value"/>.</returns>
        protected static string GetTypeNameFromValue(object value)
        {
            Debug.Assert(value != null, "value != null");

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                // primitiveValueTypeReference == null means: the EDM type of the primitive value cannot be determined.
                // This could possibly be due to value being an unsigned int.
                // In this case, simply return null because:
                //   - If the property is regular property, the type is not needed since service model knows its exact type.
                //   - If the property is dynamic property, ODL does not support dynamic property containing unsigned int value
                //     since we don't know its underlying type as well as how to serialize it.
                IEdmPrimitiveTypeReference primitiveValueTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(primitiveValue.Value.GetType());
                return primitiveValueTypeReference == null ? null : primitiveValueTypeReference.FullName();
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

            return primitiveTypeReference.FullName();
        }

        /// <summary>
        /// Validate if type name is missing
        /// </summary>
        /// <param name="typeName">Type name of the property to be validated.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="isOpenPropertyType">If the property is open.</param>
        private static void ValidateIfTypeNameMissing(string typeName, IEdmModel model, bool isOpenPropertyType)
        {
            // if we have metadata, the type name of an open property value must not be null
            if (typeName == null && model.IsUserModel() && isOpenPropertyType)
            {
                throw new ODataException(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
            }
        }

        /// <summary>
        /// Validates that the (optional) <paramref name="typeReferenceFromMetadata"/> is the same as the (optional) <paramref name="typeReferenceFromValue"/>.
        /// </summary>
        /// <param name="typeReferenceFromMetadata">The (optional) type from the metadata definition (the expected type).</param>
        /// <param name="typeReferenceFromValue">The (optional) type from the value (the actual type).</param>
        /// <param name="writerValidator">The writer validator to use for validation.</param>
        /// <returns>The type as derived from the <paramref name="typeReferenceFromMetadata"/> and/or <paramref name="typeReferenceFromValue"/>.</returns>
        private static IEdmTypeReference ResolveTypeFromMetadataAndValue(IEdmTypeReference typeReferenceFromMetadata, IEdmTypeReference typeReferenceFromValue, IWriterValidator writerValidator)
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

            writerValidator.ValidateTypeReference(typeReferenceFromMetadata, typeReferenceFromValue);

            return typeReferenceFromValue;
        }
    }
}
