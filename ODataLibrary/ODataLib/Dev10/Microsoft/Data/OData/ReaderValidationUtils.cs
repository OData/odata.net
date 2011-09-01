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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for validating OData content when reading.
    /// </summary>
    internal static class ReaderValidationUtils
    {
        /// <summary>
        /// Validates that message reader settings are correct.
        /// </summary>
        /// <param name="messageReaderSettings">The message reader settings to validate.</param>
        internal static void ValidateMessageReaderSettings(ODataMessageReaderSettings messageReaderSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            if (messageReaderSettings.BaseUri != null && !messageReaderSettings.BaseUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute(UriUtilsCommon.UriToString(messageReaderSettings.BaseUri)));
            }
        }

        /// <summary>
        /// Validates an entity reference link.
        /// </summary>
        /// <param name="link">The entity reference link to check.</param>
        internal static void ValidateEntityReferenceLink(ODataEntityReferenceLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(link != null, "link != null");

            if (link.Url == null)
            {
                throw new ODataException(Strings.ReaderValidationUtils_EntityReferenceLinkMissingUri);
            }
        }

        /// <summary>
        /// Validates a stream reference property to ensure it's properly defined in metadata if metadata exists.
        /// </summary>
        /// <param name="streamProperty">The stream property to check.</param>
        /// <param name="structuredType">The owning type of the stream property or null if no metadata is available.</param>
        internal static void ValidateStreamReferenceProperty(ODataProperty streamProperty, IEdmStructuredType structuredType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(streamProperty != null, "streamProperty != null");

            IEdmProperty edmProperty = null;

            if (structuredType != null)
            {
                edmProperty = ValidationUtils.ValidatePropertyDefined(streamProperty.Name, structuredType);
                
                // If no property match was found in the metadata and an error wasn't raised, 
                // it is an open property (which is not supported for streams).
                if (edmProperty == null)
                {
                    // Fails with the correct error message.
                    ValidationUtils.ValidateOpenPropertyValue(streamProperty.Name, streamProperty.Value);
                }
            }

            ValidationUtils.ValidateStreamReferenceProperty(streamProperty, edmProperty);
        }

        /// <summary>
        /// Validates that the specified <paramref name="expectedValueTypeReference"/> allows null values.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type for the value, or null if no such type is available.</param>
        internal static void ValidateNullValue(IEdmTypeReference expectedValueTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            if (expectedValueTypeReference != null)
            {
                Debug.Assert(
                    expectedValueTypeReference.IsODataPrimitiveTypeKind() || 
                    expectedValueTypeReference.IsODataComplexTypeKind() ||
                    expectedValueTypeReference.IsODataMultiValueTypeKind(),
                    "Only primitive, complex and multivalue types are supported by this method.");

                if (expectedValueTypeReference.IsODataPrimitiveTypeKind())
                {
                    IEdmPrimitiveTypeReference primitiveTypeReference = expectedValueTypeReference.AsPrimitiveOrNull();
                    if (!TypeUtils.TypeAllowsNull(primitiveTypeReference.GetInstanceType()))
                    {
                        throw new ODataException(Strings.ReaderValidationUtils_NullValueForNonNullableType(expectedValueTypeReference.ODataFullName()));
                    }
                }
                else if (expectedValueTypeReference.IsODataMultiValueTypeKind())
                {
                    throw new ODataException(Strings.ReaderValidationUtils_NullValueForNonNullableType(expectedValueTypeReference.ODataFullName()));
                }
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataEntry"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="entry">The entry to validate.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "entry", Justification = "Used only in debug asserts.")]
        internal static void ValidateEntry(ODataEntry entry)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");

            // Type name is verified in the format readers since it's shared with other non-entity types
            // and verifying it here would mean doing it twice.

            // Readers will report any combination of the other properties (for example MR and such)
            // So nothing else to check.
        }

        /// <summary>
        /// Creates an exception used when primitive type conversion fails.
        /// </summary>
        /// <param name="targetTypeReference">The target type reference to which the conversion failed.</param>
        /// <param name="innerException">Possible inner exception with more information about the failure.</param>
        /// <returns>The exception object to throw.</returns>
        internal static ODataException GetPrimitiveTypeConversionException(IEdmPrimitiveTypeReference targetTypeReference, Exception innerException)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");

            return new ODataException(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue(targetTypeReference.ODataFullName()), innerException);
        }

        /// <summary>
        /// Resolved the payload type name to the type.
        /// </summary>
        /// <param name="model">The model to use for the resolution.</param>
        /// <param name="payloadTypeName">The payload type name to resolve.</param>
        /// <param name="defaultPayloadTypeKind">The default payload type kind, this is used when the resolution is not possible,
        /// but the type name is not empty. (Should be either Complex or Entity).</param>
        /// <param name="payloadTypeKind">This is set to the detected payload type kind, or None if the type was not specified.</param>
        /// <returns>The resolved type. This may be null if either no user-specified model is specified, or the type name is not recognized by the model.</returns>
        /// <remarks>The method detects the payload kind even if the model does not recognize the type. It figures out primitive and multivalue types always,
        /// and uses the <paramref name="defaultPayloadTypeKind"/> for the rest.</remarks>
        internal static IEdmType ResolvePayloadTypeName(
            IEdmModel model,
            string payloadTypeName,
            EdmTypeKind defaultPayloadTypeKind,
            out EdmTypeKind payloadTypeKind)
        {
            DebugUtils.CheckNoExternalCallers();

            if (payloadTypeName == null)
            {
                payloadTypeKind = EdmTypeKind.None;
                return null;
            }

            IEdmType payloadType = MetadataUtils.ResolveTypeName(model, payloadTypeName, out payloadTypeKind);
            if (payloadTypeKind == EdmTypeKind.None)
            {
                payloadTypeKind = defaultPayloadTypeKind;
            }

            return payloadType;
        }

        /// <summary>
        /// Resolves and validates the payload type against the expected type and returns the target type.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadTypeName">The payload type name, or null if no payload type was specified.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="messageReaderSettings">The message reader settings to use.</param>
        /// <param name="serializationTypeNameAnnotation">Potentially non-null instance of an annotation to put on the value reported from the reader.</param>
        /// <returns>
        /// The target type reference to use for parsing the value.
        /// If there is no user specified model, this will return null.
        /// If there is a user specified model, this method never returns null.
        /// </returns>
        /// <remarks>
        /// This method cannot be used for primitive type resolution. Primitive type resolution is format dependent and format specific methods should be used instead.
        /// </remarks>
        internal static IEdmTypeReference ResolveAndValidateTargetType(
            EdmTypeKind expectedTypeKind,
            IEdmTypeReference expectedTypeReference,
            string payloadTypeName,
            IEdmModel model,
            ODataMessageReaderSettings messageReaderSettings,
            out SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(expectedTypeKind != EdmTypeKind.Primitive, "Primitive types are not supported by this method.");
            Debug.Assert(expectedTypeReference == null || !expectedTypeReference.IsODataPrimitiveTypeKind(), "Primitive types are not supported by this method.");

            EdmTypeKind payloadTypeKind;
            IEdmType payloadType = ResolvePayloadTypeName(model, payloadTypeName, expectedTypeKind, out payloadTypeKind);

            return ResolveAndValidateTargetType(
                expectedTypeKind,
                expectedTypeReference,
                payloadTypeKind,
                payloadType,
                payloadTypeName,
                model,
                messageReaderSettings,
                out serializationTypeNameAnnotation);
        }

        /// <summary>
        /// Resolves the payload type versus the expected type and validates that such combination is allowed.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadTypeKind">The payload type kind, this may be the one from the type itself, or one detected without resolving the type.</param>
        /// <param name="payloadType">The payload type, or null if the payload type was not specified, or it didn't resolve against the model.</param>
        /// <param name="payloadTypeName">The payload type name, or null if no payload type was specified.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="messageReaderSettings">The message reader settings to use.</param>
        /// <param name="serializationTypeNameAnnotation">Potentially non-null instance of an annotation to put on the value reported from the reader.</param>
        /// <returns>
        /// The target type reference to use for parsing the value.
        /// If there is no user specified model, this will return null.
        /// If there is a user specified model, this method never returns null.
        /// </returns>
        /// <remarks>
        /// This method cannot be used for primitive type resolution. Primitive type resolution is format dependent and format specific methods should be used instead.
        /// </remarks>
        internal static IEdmTypeReference ResolveAndValidateTargetType(
            EdmTypeKind expectedTypeKind,
            IEdmTypeReference expectedTypeReference,
            EdmTypeKind payloadTypeKind,
            IEdmType payloadType,
            string payloadTypeName,
            IEdmModel model,
            ODataMessageReaderSettings messageReaderSettings,
            out SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");
            Debug.Assert(
                expectedTypeKind == EdmTypeKind.Complex || expectedTypeKind == EdmTypeKind.Entity ||
                expectedTypeKind == EdmTypeKind.Collection,
                "The expected type kind must be one of Complex, Entity or MultiValue.");
            Debug.Assert(
                payloadTypeKind == EdmTypeKind.Complex || payloadTypeKind == EdmTypeKind.Entity ||
                payloadTypeKind == EdmTypeKind.Collection || payloadTypeKind == EdmTypeKind.None ||
                payloadTypeKind == EdmTypeKind.Primitive,
                "The payload type kind must be one of None, Primitive, Complex, Entity or MultiValue.");
            Debug.Assert(
                expectedTypeReference == null || expectedTypeReference.TypeKind() == expectedTypeKind,
                "The expected type kind must match the expected type reference if that is available.");
            Debug.Assert(
                payloadType == null || payloadType.TypeKind == payloadTypeKind,
                "The payload type kind must match the payload type if that is available.");
            Debug.Assert(payloadType == null || payloadTypeName != null, "If we have a payload type, we must have its name as well.");

            if (payloadTypeKind != EdmTypeKind.None)
            {
                // Make sure that the type kinds match.
                ValidationUtils.ValidateTypeKind(payloadTypeKind, expectedTypeKind, payloadTypeName);
            }

            serializationTypeNameAnnotation = null;

            if (!model.IsUserModel())
            {
                // If there's no model, it means we should not have the expected type either, and that there's no type to use,
                // no metadata validation to perform.
                Debug.Assert(expectedTypeReference == null, "If we don't have a model, we must not have expected type either.");
                return null;
            }

            if (expectedTypeReference == null)
            {
                return ResolveAndValidateTargetTypeWithNoExpectedType(
                    expectedTypeKind,
                    payloadType,
                    payloadTypeName,
                    out serializationTypeNameAnnotation);
            }

            if (messageReaderSettings.DisableStrictMetadataValidation)
            {
                return ResolveAndValidateTargetTypeStrictValidationDisabled(
                    expectedTypeKind,
                    expectedTypeReference,
                    payloadType,
                    payloadTypeName,
                    out serializationTypeNameAnnotation);
            }
            else
            {
                return ResolveAndValidateTargetTypeStrictValidationEnabled(
                    expectedTypeKind,
                    expectedTypeReference,
                    payloadType,
                    payloadTypeName,
                    out serializationTypeNameAnnotation);
            }
        }

        /// <summary>
        /// Resolves the payload type if there's no expected type.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="payloadType">The payload type, or null if the payload type was not specified, or it didn't resolve against the model.</param>
        /// <param name="payloadTypeName">The payload type name, or null if no payload type was specified.</param>
        /// <param name="serializationTypeNameAnnotation">Potentially non-null instance of an annotation to put on the value reported from the reader.</param>
        /// <returns>The target type reference to use for parsing the value.</returns>
        private static IEdmTypeReference ResolveAndValidateTargetTypeWithNoExpectedType(
            EdmTypeKind expectedTypeKind,
            IEdmType payloadType,
            string payloadTypeName,
            out SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            serializationTypeNameAnnotation = null;

            // No expected type (for example an open property, but other scenarios are possible)
            // We need some type to go on. We do have a model, so we must perform metadata validation and for that we need a type.
            VerifyPayloadTypeDefined(payloadTypeName, payloadType);
            if (payloadType == null)
            {
                if (expectedTypeKind == EdmTypeKind.Entity)
                {
                    throw new ODataException(Strings.ReaderValidationUtils_EntryWithoutType);
                }

                throw new ODataException(Strings.ReaderValidationUtils_ValueWithoutType);
            }

            IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference();
            serializationTypeNameAnnotation = CreateSerializationTypeNameAnnotation(payloadTypeName, payloadTypeReference);

            // Use the payload type (since we don't have any other).
            return payloadTypeReference;
        }

        /// <summary>
        /// Resolves the payload type versus the expected type and validates that such combination is allowed when the strict validation is disabled.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadType">The payload type, or null if the payload type was not specified, or it didn't resolve against the model.</param>
        /// <param name="payloadTypeName">The payload type name, or null if no payload type was specified.</param>
        /// <param name="serializationTypeNameAnnotation">Potentially non-null instance of an annotation to put on the value reported from the reader.</param>
        /// <returns>The target type reference to use for parsing the value.</returns>
        private static IEdmTypeReference ResolveAndValidateTargetTypeStrictValidationDisabled(
            EdmTypeKind expectedTypeKind,
            IEdmTypeReference expectedTypeReference,
            IEdmType payloadType,
            string payloadTypeName,
            out SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            // Lax validation logic
            switch (expectedTypeKind)
            {
                case EdmTypeKind.Complex:
                    if (payloadType != null)
                    {
                        // Just verify that it's not a derived complex type, in all other cases simply use the expected type.
                        VerifyComplexType(expectedTypeReference, payloadType, /* failIfNotRelated */ false);
                    }

                    break;
                case EdmTypeKind.Entity:
                    if (payloadType != null)
                    {
                        // If the type is assignable (equal or derived) we will use the payload type, since we want to allow derived entities
                        if (EdmLibraryExtensions.IsAssignableFrom(expectedTypeReference.AsEntity().EntityDefinition(), (IEdmEntityType)payloadType))
                        {
                            IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference();
                            serializationTypeNameAnnotation = CreateSerializationTypeNameAnnotation(payloadTypeName, payloadTypeReference);
                            return payloadTypeReference;
                        }
                    }

                    break;
                case EdmTypeKind.Collection:
                    if (payloadType != null)
                    {
                        VerifyMultiValueComplexItemType(expectedTypeReference, payloadType);
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind));
            }

            serializationTypeNameAnnotation = CreateSerializationTypeNameAnnotation(payloadTypeName, expectedTypeReference);
            
            // Either there's no payload type, in which case use the expected one, or the payload one and the expected one are equal.
            return expectedTypeReference;
        }

        /// <summary>
        /// Resolves the payload type versus the expected type and validates that such combination is allowed when strict validation is enabled.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadType">The payload type, or null if the payload type was not specified, or it didn't resolve against the model.</param>
        /// <param name="payloadTypeName">The payload type name, or null if no payload type was specified.</param>
        /// <param name="serializationTypeNameAnnotation">Potentially non-null instance of an annotation to put on the value reported from the reader.</param>
        /// <returns>The target type reference to use for parsing the value.</returns>
        private static IEdmTypeReference ResolveAndValidateTargetTypeStrictValidationEnabled(
            EdmTypeKind expectedTypeKind,
            IEdmTypeReference expectedTypeReference,
            IEdmType payloadType,
            string payloadTypeName,
            out SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            VerifyPayloadTypeDefined(payloadTypeName, payloadType);

            // Strict validation logic
            switch (expectedTypeKind)
            {
                case EdmTypeKind.Complex:
                    if (payloadType != null)
                    {
                        VerifyComplexType(expectedTypeReference, payloadType, /* failIfNotRelated */ true);
                    }

                    break;
                case EdmTypeKind.Entity:
                    if (payloadType != null)
                    {
                        // The payload type must be assignable to the expected type.
                        IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference();
                        ValidationUtils.ValidateEntityTypeIsAssignable((IEdmEntityTypeReference)expectedTypeReference, (IEdmEntityTypeReference)payloadTypeReference);
                        serializationTypeNameAnnotation = CreateSerializationTypeNameAnnotation(payloadTypeName, payloadTypeReference);
                        
                        // Use the payload type
                        return payloadTypeReference;
                    }

                    break;
                case EdmTypeKind.Collection:
                    // The type must be exactly equal - note that we intentionally ignore nullability of the items here, since the payload type
                    // can't specify that.
                    if (payloadType != null && string.CompareOrdinal(payloadType.ODataFullName(), expectedTypeReference.ODataFullName()) != 0)
                    {
                        VerifyMultiValueComplexItemType(expectedTypeReference, payloadType);

                        throw new ODataException(Strings.ValidationUtils_IncompatibleType(payloadType.ODataFullName(), expectedTypeReference.ODataFullName()));
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind));
            }

            serializationTypeNameAnnotation = CreateSerializationTypeNameAnnotation(payloadTypeName, expectedTypeReference);
          
            // Either there's no payload type, in which case use the expected one, or the payload one and the expected one are equal.
            return expectedTypeReference;
        }

        /// <summary>
        /// Verifies that payload type is defined if the payload type name is present.
        /// </summary>
        /// <param name="payloadTypeName">The type name from the payload.</param>
        /// <param name="payloadType">The resolved type from the model.</param>
        private static void VerifyPayloadTypeDefined(string payloadTypeName, IEdmType payloadType)
        {
            if (payloadTypeName != null && payloadType == null)
            {
                throw new ODataException(Strings.ValidationUtils_UnrecognizedTypeName(payloadTypeName));
            }
        }

        /// <summary>
        /// Verifies that complex type is valid against the expected type.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference.</param>
        /// <param name="payloadType">The payload type.</param>
        /// <param name="failIfNotRelated">true if the method should fail if the <paramref name="payloadType"/> doesn't match the <paramref name="expectedTypeReference"/>;
        /// false if the method should just return in that case.</param>
        /// <remarks>
        /// The method verifies that the <paramref name="payloadType"/> is not a derived complex type of the <paramref name="expectedTypeReference"/>
        /// and always fails in that case.
        /// </remarks>
        private static void VerifyComplexType(IEdmTypeReference expectedTypeReference, IEdmType payloadType, bool failIfNotRelated)
        {
            Debug.Assert(expectedTypeReference != null, "expectedTypeReference != null");
            Debug.Assert(payloadType != null, "payloadType != null");

            // Note that we compare the type definitions, since we want to ignore nullability (the payload type doesn't specify nullability).
            IEdmStructuredType structuredExpectedType = expectedTypeReference.AsStructured().StructuredDefinition();
            IEdmStructuredType structuredPayloadType = (IEdmStructuredType)payloadType;
            if (!structuredExpectedType.IsEquivalentTo(structuredPayloadType))
            {
                // We want a specific error message in case of inheritance.
                if (EdmLibraryExtensions.IsAssignableFrom(structuredExpectedType, structuredPayloadType))
                {
                    // We don't allow type inheritance on complex types.
                    throw new ODataException(Strings.ReaderValidationUtils_DerivedComplexTypesAreNotAllowed(structuredExpectedType.ODataFullName(), structuredPayloadType.ODataFullName()));
                }

                if (failIfNotRelated)
                {
                    // And now the generic one when the types are not related at all.
                    throw new ODataException(Strings.ValidationUtils_IncompatibleType(structuredPayloadType.ODataFullName(), structuredExpectedType.ODataFullName()));
                }
            }
        }

        /// <summary>
        /// Verifies that in case of multivalue types, the item type is valid.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference.</param>
        /// <param name="payloadType">The payload type.</param>
        /// <remarks>
        /// This method verifies that item type is not a derived complex type, we want to explicitly disallow that case for possible future enablement.
        /// </remarks>
        private static void VerifyMultiValueComplexItemType(IEdmTypeReference expectedTypeReference, IEdmType payloadType)
        {
            Debug.Assert(expectedTypeReference != null, "expectedTypeReference != null");
            Debug.Assert(payloadType != null, "payloadType != null");
            Debug.Assert(expectedTypeReference.IsODataMultiValueTypeKind(), "This method only works on multivalues.");
            Debug.Assert(payloadType.IsODataMultiValueTypeKind(), "This method only works on multivalues.");

            IEdmTypeReference expectedItemTypeReference = expectedTypeReference.GetMultiValueItemType();
            if (expectedItemTypeReference != null && expectedItemTypeReference.IsODataComplexTypeKind())
            {
                IEdmTypeReference payloadItemTypeReference = payloadType.ToTypeReference().GetMultiValueItemType();
                if (payloadItemTypeReference != null && payloadItemTypeReference.IsODataComplexTypeKind())
                {
                    // Note that this method is called from both strict and lax code paths, so we must not fail if the types are not related.
                    // The strict caller will fail if the types are not equal after this method returns. We use this method there to only get
                    // a more specific error message if the derived complex types are used.
                    VerifyComplexType(expectedItemTypeReference, payloadItemTypeReference.Definition, /* failIfNotRelated */ false);
                }
            }
        }

        /// <summary>
        /// Conditionally creates the annotation to put on the read value in order to retain the type name from the payload.
        /// </summary>
        /// <param name="payloadTypeName">The payload type name.</param>
        /// <param name="targetTypeReference">The type reference into which we're going to parse.</param>
        /// <returns>The annotation to report to the reader for adding on the read value.</returns>
        private static SerializationTypeNameAnnotation CreateSerializationTypeNameAnnotation(string payloadTypeName, IEdmTypeReference targetTypeReference)
        {
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");

            if (payloadTypeName != null && string.CompareOrdinal(payloadTypeName, targetTypeReference.ODataFullName()) != 0)
            {
                return new SerializationTypeNameAnnotation { TypeName = payloadTypeName };
            }

            return null;
        }
    }
}
