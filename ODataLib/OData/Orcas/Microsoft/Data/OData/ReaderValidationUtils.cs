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
    using System.Text;
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
        /// <param name="readingResponse">true if the settings were specified when reading a response, false when reading a request.</param>
        internal static void ValidateMessageReaderSettings(ODataMessageReaderSettings messageReaderSettings, bool readingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            if (messageReaderSettings.BaseUri != null && !messageReaderSettings.BaseUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute(UriUtilsCommon.UriToString(messageReaderSettings.BaseUri)));
            }

            if (!readingResponse && messageReaderSettings.UndeclaredPropertyBehaviorKinds != ODataUndeclaredPropertyBehaviorKinds.None)
            {
                throw new ODataException(Strings.ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedOnRequest);
            }

            if (!string.IsNullOrEmpty(messageReaderSettings.ReaderBehavior.ODataTypeScheme) && !string.Equals(messageReaderSettings.ReaderBehavior.ODataTypeScheme, Atom.AtomConstants.ODataSchemeNamespace))
            {
                ODataVersionChecker.CheckCustomTypeScheme(messageReaderSettings.MaxProtocolVersion);
            }

            if (!string.IsNullOrEmpty(messageReaderSettings.ReaderBehavior.ODataNamespace) && !string.Equals(messageReaderSettings.ReaderBehavior.ODataNamespace, Atom.AtomConstants.ODataNamespace))
            {
                ODataVersionChecker.CheckCustomDataNamespace(messageReaderSettings.MaxProtocolVersion);
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
        /// Validates a stream reference property.
        /// </summary>
        /// <param name="streamProperty">The stream property to check.</param>
        /// <param name="structuredType">The owning type of the stream property or null if no metadata is available.</param>
        /// <param name="streamEdmProperty">The stream property defined by the model.</param>
        internal static void ValidateStreamReferenceProperty(ODataProperty streamProperty, IEdmStructuredType structuredType, IEdmProperty streamEdmProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(streamProperty != null, "streamProperty != null");

            ValidationUtils.ValidateStreamReferenceProperty(streamProperty, streamEdmProperty);

            if (structuredType != null && structuredType.IsOpen)
            {
                // If no property match was found in the metadata and an error wasn't raised, 
                // it is an open property (which is not supported for streams).
                if (streamEdmProperty == null)
                {
                    // Fails with the correct error message.
                    ValidationUtils.ValidateOpenPropertyValue(streamProperty.Name, streamProperty.Value);
                }
            }
        }

        /// <summary>
        /// Validate a null value.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> used to read the payload.</param>
        /// <param name="expectedTypeReference">The expected type of the null value.</param>
        /// <param name="messageReaderSettings">The message reader settings.</param>
        /// <param name="validateNullValue">true to validate the the null value; false to only check whether the type is supported.</param>
        /// <param name="version">The version used to read the payload.</param>
        internal static void ValidateNullValue(
            IEdmModel model, 
            IEdmTypeReference expectedTypeReference, 
            ODataMessageReaderSettings messageReaderSettings,
            bool validateNullValue, 
            ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            // For a null value if we have an expected type
            //   - we validate that the expected type is nullable if type conversion is enabled
            //   - don't validate any primitive types if primitive type conversion is disabled
            // For a null value without an expected type
            //   - we simply return null (treat it is as primitive null value)
            if (expectedTypeReference != null)
            {
                ReaderValidationUtils.ValidateTypeSupported(expectedTypeReference, version);

                if (!messageReaderSettings.DisablePrimitiveTypeConversion || expectedTypeReference.TypeKind() != EdmTypeKind.Primitive)
                {
                    ReaderValidationUtils.ValidateNullValueAllowed(expectedTypeReference, validateNullValue, model);
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
        /// Finds a defined property from the model if one is available.
        /// The structured type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to find.</param>
        /// <param name="owningStructuredType">The owning type of the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</param>
        /// <param name="messageReaderSettings">The message reader settings being used.</param>
        /// <returns>The <see cref="IEdmProperty"/> instance representing the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</returns>
        internal static IEdmProperty FindDefinedProperty(string propertyName, IEdmStructuredType owningStructuredType, ODataMessageReaderSettings messageReaderSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            if (owningStructuredType == null)
            {
                return null;
            }

            IEdmProperty property = owningStructuredType.FindProperty(propertyName);

            if (property == null && owningStructuredType.IsOpen)
            {
                if (messageReaderSettings.UndeclaredPropertyBehaviorKinds != ODataUndeclaredPropertyBehaviorKinds.None)
                {
                    throw new ODataException(Strings.ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedForOpenType(propertyName, owningStructuredType.ODataFullName()));
                }
            }

            return property;
        }

        /// <summary>
        /// Validates that a property with the specified name exists on a given structured type.
        /// The structured type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="owningStructuredType">The owning type of the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</param>
        /// <param name="messageReaderSettings">The message reader settings being used.</param>
        /// <param name="ignoreProperty">true if the property should be completely ignored and not parsed/reported, in this case the return value is null.
        /// false if the property should be parsed and reported as usual.</param>
        /// <returns>The <see cref="IEdmProperty"/> instance representing the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</returns>
        internal static IEdmProperty ValidateValuePropertyDefined(string propertyName, IEdmStructuredType owningStructuredType, ODataMessageReaderSettings messageReaderSettings, out bool ignoreProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            ignoreProperty = false;

            if (owningStructuredType == null)
            {
                return null;
            }

            IEdmProperty property = FindDefinedProperty(propertyName, owningStructuredType, messageReaderSettings);
            if (property == null && !owningStructuredType.IsOpen)
            {
                if (messageReaderSettings.UndeclaredPropertyBehaviorKinds.HasFlag(ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty))
                {
                    ignoreProperty = true;
                }
                else
                {
                    throw new ODataException(Strings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, owningStructuredType.ODataFullName()));
                }
            }

            return property;
        }

        /// <summary>
        /// Validates that a property with the specified name exists on a given structured type.
        /// The structured type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="owningStructuredType">The owning type of the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</param>
        /// <param name="messageReaderSettings">The message reader settings being used.</param>
        /// <returns>The <see cref="IEdmProperty"/> instance representing the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</returns>
        internal static IEdmProperty ValidateLinkPropertyDefined(string propertyName, IEdmStructuredType owningStructuredType, ODataMessageReaderSettings messageReaderSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            if (owningStructuredType == null)
            {
                return null;
            }

            IEdmProperty property = FindDefinedProperty(propertyName, owningStructuredType, messageReaderSettings);
            if (property == null && !owningStructuredType.IsOpen)
            {
                if (!messageReaderSettings.UndeclaredPropertyBehaviorKinds.HasFlag(ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty))
                {
                    throw new ODataException(Strings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, owningStructuredType.ODataFullName()));
                }
            }

            return property;
        }

        /// <summary>
        /// Validates that a navigation property with the specified name exists on a given entity type.
        /// The entity type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="owningEntityType">The owning entity type or null if no metadata is available.</param>
        /// <param name="messageReaderSettings">The message reader settings being used.</param>
        /// <returns>The <see cref="IEdmNavigationProperty"/> instance representing the navigation property with name <paramref name="propertyName"/>
        /// or null if no metadata is available.</returns>
        internal static IEdmNavigationProperty ValidateNavigationPropertyDefined(
            string propertyName,
            IEdmEntityType owningEntityType,
            ODataMessageReaderSettings messageReaderSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            if (owningEntityType == null)
            {
                return null;
            }

            IEdmProperty property = ValidateLinkPropertyDefined(propertyName, owningEntityType, messageReaderSettings);
            if (property == null)
            {
                if (owningEntityType.IsOpen)
                {
                    // We don't support open navigation properties
                    throw new ODataException(Strings.ValidationUtils_OpenNavigationProperty(propertyName, owningEntityType.ODataFullName()));
                }
            }
            else if (property.PropertyKind != EdmPropertyKind.Navigation)
            {
                // The property must be a navigation property
                throw new ODataException(Strings.ValidationUtils_NavigationPropertyExpected(propertyName, owningEntityType.ODataFullName(), property.PropertyKind.ToString()));
            }

            return (IEdmNavigationProperty)property;
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
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadTypeName">The payload type name to resolve.</param>
        /// <param name="expectedTypeKind">The default payload type kind, this is used when the resolution is not possible,
        /// but the type name is not empty. (Should be either Complex or Entity).</param>
        /// <param name="readerBehavior">Reader behavior to use for compatibility.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <param name="payloadTypeKind">This is set to the detected payload type kind, or None if the type was not specified.</param>
        /// <returns>The resolved type. This may be null if either no user-specified model is specified, or the type name is not recognized by the model.</returns>
        /// <remarks>The method detects the payload kind even if the model does not recognize the type. It figures out primitive and collection types always,
        /// and uses the <paramref name="expectedTypeKind"/> for the rest.</remarks>
        internal static IEdmType ResolvePayloadTypeName(
            IEdmModel model,
            IEdmTypeReference expectedTypeReference,
            string payloadTypeName,
            EdmTypeKind expectedTypeKind,
            ODataReaderBehavior readerBehavior,
            ODataVersion version,
            out EdmTypeKind payloadTypeKind)
        {
            DebugUtils.CheckNoExternalCallers();

            if (payloadTypeName == null)
            {
                payloadTypeKind = EdmTypeKind.None;
                return null;
            }

            if (payloadTypeName.Length == 0)
            {
                payloadTypeKind = expectedTypeKind;
                return null;
            }

            IEdmType payloadType = MetadataUtils.ResolveTypeNameForRead(
                model,
                expectedTypeReference == null ? null : expectedTypeReference.Definition,
                payloadTypeName,
                readerBehavior,
                version,
                out payloadTypeKind);
            if (payloadTypeKind == EdmTypeKind.None)
            {
                payloadTypeKind = expectedTypeKind;
            }

            return payloadType;
        }

        /// <summary>
        /// Resolves and validates the payload type against the expected type and returns the target type.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="defaultPrimitivePayloadType">The default payload type if none is specified in the payload;
        /// for ATOM this is Edm.String, for JSON it is null since there is no payload type name for primitive types in the payload.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadTypeName">The payload type name, or null if no payload type was specified.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="messageReaderSettings">The message reader settings to use.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <param name="typeKindFromPayloadFunc">A func to compute the type kind from the payload shape if it could not be determined from the expected type or the payload type.</param>
        /// <param name="targetTypeKind">The target type kind to be used to read the payload.</param>
        /// <param name="serializationTypeNameAnnotation">Potentially non-null instance of an annotation to put on the value reported from the reader.</param>
        /// <returns>
        /// The target type reference to use for parsing the value.
        /// If there is no user specified model, this will return null.
        /// If there is a user specified model, this method never returns null.
        /// </returns>
        /// <remarks>
        /// This method cannot be used for primitive type resolution. Primitive type resolution is format dependent and format specific methods should be used instead.
        /// </remarks>
        internal static IEdmTypeReference ResolvePayloadTypeNameAndComputeTargetType(
            EdmTypeKind expectedTypeKind,
            IEdmType defaultPrimitivePayloadType,
            IEdmTypeReference expectedTypeReference,
            string payloadTypeName,
            IEdmModel model,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            Func<EdmTypeKind> typeKindFromPayloadFunc,
            out EdmTypeKind targetTypeKind,
            out SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(typeKindFromPayloadFunc != null, "typeKindFromPayloadFunc != null");

            serializationTypeNameAnnotation = null;

            // Resolve the type name and get the payload type kind; that is the type kind of the payload
            // type if available or the expected type kind if no payload type could be resolved. Since
            // we always detect primitive and collection types the expected type for unrecognized payload
            // types is EdmTypeKind.Complex.
            EdmTypeKind payloadTypeKind;
            IEdmType payloadType = ResolvePayloadTypeName(
                model,
                expectedTypeReference,
                payloadTypeName,
                EdmTypeKind.Complex,
                messageReaderSettings.ReaderBehavior,
                version,
                out payloadTypeKind);

            // Compute the target type kind based on the expected type, the payload type kind
            // and a function to detect the target type kind from the shape of the payload.
            targetTypeKind = ComputeTargetTypeKind(
                expectedTypeReference, 
                /*forEntityValue*/ expectedTypeKind == EdmTypeKind.Entity,
                payloadTypeName, 
                payloadTypeKind, 
                messageReaderSettings, 
                typeKindFromPayloadFunc);

            // Resolve potential conflicts between payload and expected types and apply all the various behavior changing flags from settings
            IEdmTypeReference targetTypeReference;
            if (targetTypeKind == EdmTypeKind.Primitive)
            {
                targetTypeReference = ReaderValidationUtils.ResolveAndValidatePrimitiveTargetType(
                    expectedTypeReference,
                    payloadTypeKind,
                    payloadType,
                    payloadTypeName,
                    defaultPrimitivePayloadType,
                    model,
                    messageReaderSettings,
                    version);
            }
            else
            {
                targetTypeReference = ReaderValidationUtils.ResolveAndValidateNonPrimitiveTargetType(
                    targetTypeKind,
                    expectedTypeReference,
                    payloadTypeKind,
                    payloadType,
                    payloadTypeName,
                    model,
                    messageReaderSettings,
                    version,
                    out serializationTypeNameAnnotation);
            }

            if (expectedTypeKind != EdmTypeKind.None && targetTypeReference != null)
            {
                ValidationUtils.ValidateTypeKind(targetTypeKind, expectedTypeKind, payloadTypeName);
            }

            return targetTypeReference;
        }

        /// <summary>
        /// Resolves the primitive payload type versus the expected type and validates that such combination is allowed.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference, if any.</param>
        /// <param name="payloadTypeKind">The kind of the payload type, or None if the detection was not possible.</param>
        /// <param name="payloadType">The resolved payload type, or null if no payload type was specified.</param>
        /// <param name="payloadTypeName">The name of the payload type, or null if no payload type was specified.</param>
        /// <param name="defaultPayloadType">The default payload type if none is specified in the payload;
        /// for ATOM this is Edm.String, for JSON it is null since there is no payload type name for primitive types in the payload.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="messageReaderSettings">The message reader settings to use.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <returns>The target type reference to use for parsing the value. This method never returns null.</returns>
        internal static IEdmTypeReference ResolveAndValidatePrimitiveTargetType(
            IEdmTypeReference expectedTypeReference,
            EdmTypeKind payloadTypeKind,
            IEdmType payloadType,
            string payloadTypeName,
            IEdmType defaultPayloadType,
            IEdmModel model,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");
            Debug.Assert(
                payloadTypeKind == EdmTypeKind.Primitive || payloadTypeKind == EdmTypeKind.Complex ||
                payloadTypeKind == EdmTypeKind.Entity || payloadTypeKind == EdmTypeKind.Collection ||
                payloadTypeKind == EdmTypeKind.None,
                "The payload type kind must be one of None, Primitive, Complex, Entity or Collection.");
            Debug.Assert(
                expectedTypeReference == null || expectedTypeReference.TypeKind() == EdmTypeKind.Primitive,
                "This method only works for primitive expected type.");
            Debug.Assert(
                payloadType == null || payloadType.TypeKind == payloadTypeKind,
                "The payload type kind must match the payload type if that is available.");
            Debug.Assert(payloadType == null || payloadTypeName != null, "If we have a payload type, we must have its name as well.");

            bool useExpectedTypeOnlyForTypeResolution = messageReaderSettings.ReaderBehavior.TypeResolver != null && payloadType != null;
            if (expectedTypeReference != null && !useExpectedTypeOnlyForTypeResolution)
            {
                ValidateTypeSupported(expectedTypeReference, version);
            }

            // Validate type kinds except for open properties or when in lax mode, but only if primitive type conversion is enabled.
            if (payloadTypeKind != EdmTypeKind.None && (messageReaderSettings.DisablePrimitiveTypeConversion || !messageReaderSettings.DisableStrictMetadataValidation))
            {
                // Make sure that the type kinds match.
                ValidationUtils.ValidateTypeKind(payloadTypeKind, EdmTypeKind.Primitive, payloadTypeName);
            }

            if (!model.IsUserModel())
            {
                // If there's no model, it means we should not have the expected type either, and that there's no type to use,
                // no metadata validation to perform.
                Debug.Assert(expectedTypeReference == null, "If we don't have a model, we must not have expected type either.");
                return GetNullablePayloadTypeReference(payloadType ?? defaultPayloadType);
            }

            // If the primitive type conversion is off, use the payload type always.
            // If there's no expected type or the expected type is ignored, use the payload type as well.
            if (expectedTypeReference == null || useExpectedTypeOnlyForTypeResolution || messageReaderSettings.DisablePrimitiveTypeConversion)
            {
                // If there's no payload type, use the default payload type.
                // Note that in collections the items without type should inherit the type name from the collection, in that case the expectedTypeReference
                // is never null (assuming we do have a model), so we won't get here.
                return GetNullablePayloadTypeReference(payloadType ?? defaultPayloadType);
            }

            if (messageReaderSettings.DisableStrictMetadataValidation)
            {
                // Lax validation logic
                // Always use the expected type, the payload type is ignored.
                return expectedTypeReference;
            }

            // Strict validation logic
            if (payloadType != null)
            {
                // The payload type must be convertible to the expected type.
                // Note that we compare the type definitions, since we want to ignore nullability (the payload type doesn't specify nullability).
                if (!MetadataUtilsCommon.CanConvertPrimitiveTypeTo(
                    (IEdmPrimitiveType)payloadType,
                    (IEdmPrimitiveType)(expectedTypeReference.Definition)))
                {
                    throw new ODataException(Strings.ValidationUtils_IncompatibleType(payloadTypeName, expectedTypeReference.ODataFullName()));
                }
            }

            // Read using the expected type. 
            // If there was a payload type we verified that it's convertible and thus we can safely read 
            // the content of the value as the expected type.
            return expectedTypeReference;
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
        /// <param name="version">The version of the payload being read.</param>
        /// <param name="serializationTypeNameAnnotation">Potentially non-null instance of an annotation to put on the value reported from the reader.</param>
        /// <returns>
        /// The target type reference to use for parsing the value.
        /// If there is no user specified model, this will return null.
        /// If there is a user specified model, this method never returns null.
        /// </returns>
        /// <remarks>
        /// This method cannot be used for primitive type resolution. Primitive type resolution is format dependent and format specific methods should be used instead.
        /// </remarks>
        internal static IEdmTypeReference ResolveAndValidateNonPrimitiveTargetType(
            EdmTypeKind expectedTypeKind,
            IEdmTypeReference expectedTypeReference,
            EdmTypeKind payloadTypeKind,
            IEdmType payloadType,
            string payloadTypeName,
            IEdmModel model,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            out SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");
            Debug.Assert(
                expectedTypeKind == EdmTypeKind.Complex || expectedTypeKind == EdmTypeKind.Entity ||
                expectedTypeKind == EdmTypeKind.Collection,
                "The expected type kind must be one of Complex, Entity or Collection.");
            Debug.Assert(
                payloadTypeKind == EdmTypeKind.Complex || payloadTypeKind == EdmTypeKind.Entity ||
                payloadTypeKind == EdmTypeKind.Collection || payloadTypeKind == EdmTypeKind.None ||
                payloadTypeKind == EdmTypeKind.Primitive,
                "The payload type kind must be one of None, Primitive, Complex, Entity or Collection.");
            Debug.Assert(
                expectedTypeReference == null || expectedTypeReference.TypeKind() == expectedTypeKind,
                "The expected type kind must match the expected type reference if that is available.");
            Debug.Assert(
                payloadType == null || payloadType.TypeKind == payloadTypeKind,
                "The payload type kind must match the payload type if that is available.");
            Debug.Assert(payloadType == null || payloadTypeName != null, "If we have a payload type, we must have its name as well.");

            bool useExpectedTypeOnlyForTypeResolution = messageReaderSettings.ReaderBehavior.TypeResolver != null && payloadType != null;
            if (!useExpectedTypeOnlyForTypeResolution)
            {
                ValidateTypeSupported(expectedTypeReference, version);

                // We should validate that the payload type resolved before anything else to produce reasonable error messages
                // Otherwise we might report errors which are somewhat confusing (like "Type '' is Complex but Collection was expected.").
                if (model.IsUserModel() && (expectedTypeReference == null || !messageReaderSettings.DisableStrictMetadataValidation))
                {
                    // When using a type resolver (i.e., useExpectedTypeOnlyForTypeResolution == true) then we don't have to
                    // call this method because the contract with the type resolver is to always resolve the type name and thus
                    // we will always get a defined type.
                    VerifyPayloadTypeDefined(payloadTypeName, payloadType);
                }
            }
            else
            {
                // Payload types are always nullable.
                ValidateTypeSupported(payloadType == null ? null : payloadType.ToTypeReference(/*nullable*/ true), version);
            }

            if (payloadTypeKind != EdmTypeKind.None && (!messageReaderSettings.DisableStrictMetadataValidation || expectedTypeReference == null))
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

            if (expectedTypeReference == null || useExpectedTypeOnlyForTypeResolution)
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

            return ResolveAndValidateTargetTypeStrictValidationEnabled(
                expectedTypeKind,
                expectedTypeReference,
                payloadType,
                payloadTypeName,
                out serializationTypeNameAnnotation);
        }

        /// <summary>
        /// Validates that the specified encoding is supported in batch/changeset envelopes (headers, boundaries, preamble, etc.).
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to check.</param>
        internal static void ValidateEncodingSupportedInBatch(Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(encoding != null, "encoding != null");

#if SILVERLIGHT || WINDOWS_PHONE
            if (string.CompareOrdinal(Encoding.UTF8.WebName, encoding.WebName) != 0)
#else
            if (!encoding.IsSingleByte && Encoding.UTF8.CodePage != encoding.CodePage)
#endif
            {
                throw new ODataException(Strings.ODataBatchReaderStream_MultiByteEncodingsNotSupported(encoding.WebName));
            }
        }

        /// <summary>
        /// Validates whether the specified type reference is supported in the current version.
        /// </summary>
        /// <param name="typeReference">The type reference to check.</param>
        /// <param name="version">The version currently used.</param>
        internal static void ValidateTypeSupported(IEdmTypeReference typeReference, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference != null)
            {
                if (typeReference.IsNonEntityODataCollectionTypeKind())
                {
                    ODataVersionChecker.CheckCollectionValue(version);
                }
                else if (typeReference.IsSpatial())
                {
                    ODataVersionChecker.CheckSpatialValue(version);
                }
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
            Debug.Assert(payloadTypeName == null || payloadType != null, "The payload type must have resolved before we get here.");
            serializationTypeNameAnnotation = null;

            // No expected type (for example an open property, but other scenarios are possible)
            // We need some type to go on. We do have a model, so we must perform metadata validation and for that we need a type.
            if (payloadType == null)
            {
                if (expectedTypeKind == EdmTypeKind.Entity)
                {
                    throw new ODataException(Strings.ReaderValidationUtils_EntryWithoutType);
                }

                throw new ODataException(Strings.ReaderValidationUtils_ValueWithoutType);
            }

            // Payload types are always nullable.
            IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference(/*nullable*/ true);
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
                    // if the expectedTypeKind is different from the payloadType.TypeKind the types are not related 
                    // in any way. In that case we will just use the expected type because we are in lax mode. 
                    if (payloadType != null && expectedTypeKind == payloadType.TypeKind)
                    {
                        // Just verify that it's not a derived complex type, in all other cases simply use the expected type.
                        VerifyComplexType(expectedTypeReference, payloadType, /* failIfNotRelated */ false);
                    }

                    break;
                case EdmTypeKind.Entity:
                    // if the expectedTypeKind is different from the payloadType.TypeKind the types are not related 
                    // in any way. In that case we will just use the expected type because we are in lax mode. 
                    if (payloadType != null && expectedTypeKind == payloadType.TypeKind)
                    {
                        // If the type is assignable (equal or derived) we will use the payload type, since we want to allow derived entities
                        if (EdmLibraryExtensions.IsAssignableFrom(expectedTypeReference.AsEntity().EntityDefinition(), (IEdmEntityType)payloadType))
                        {
                            IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference(/*nullable*/ true);
                            serializationTypeNameAnnotation = CreateSerializationTypeNameAnnotation(payloadTypeName, payloadTypeReference);
                            return payloadTypeReference;
                        }
                    }

                    break;
                case EdmTypeKind.Collection:
                    // if the expectedTypeKind is different from the payloadType.TypeKind the types are not related 
                    // in any way. In that case we will just use the expected type because we are in lax mode. 
                    if (payloadType != null && expectedTypeKind == payloadType.TypeKind)
                    {
                        VerifyCollectionComplexItemType(expectedTypeReference, payloadType);
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
            Debug.Assert(payloadTypeName == null || payloadType != null, "The payload type must have resolved before we get here.");

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
                        IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference(/*nullable*/ true);
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
                        VerifyCollectionComplexItemType(expectedTypeReference, payloadType);

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
        /// Gets the nullable type reference for a payload type; if the payload type is null, uses Edm.String.
        /// </summary>
        /// <param name="payloadType">The payload type to get the type reference for.</param>
        /// <returns>The nullable <see cref="IEdmTypeReference"/> for the <paramref name="payloadType"/>.</returns>
        private static IEdmTypeReference GetNullablePayloadTypeReference(IEdmType payloadType)
        {
            return payloadType == null ? null : payloadType.ToTypeReference(true);
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
        /// Verifies that in case of collection types, the item type is valid.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference.</param>
        /// <param name="payloadType">The payload type.</param>
        /// <remarks>
        /// This method verifies that item type is not a derived complex type, we want to explicitly disallow that case for possible future enablement.
        /// </remarks>
        private static void VerifyCollectionComplexItemType(IEdmTypeReference expectedTypeReference, IEdmType payloadType)
        {
            Debug.Assert(expectedTypeReference != null, "expectedTypeReference != null");
            Debug.Assert(payloadType != null, "payloadType != null");
            Debug.Assert(expectedTypeReference.IsNonEntityODataCollectionTypeKind(), "This method only works on atomic collections.");
            Debug.Assert(payloadType.IsNonEntityODataCollectionTypeKind(), "This method only works on atomic collections.");

            IEdmCollectionTypeReference collectionTypeReference = ValidationUtils.ValidateCollectionType(expectedTypeReference);
            IEdmTypeReference expectedItemTypeReference = collectionTypeReference.GetCollectionItemType();
            if (expectedItemTypeReference != null && expectedItemTypeReference.IsODataComplexTypeKind())
            {
                IEdmCollectionTypeReference payloadCollectionTypeReference = ValidationUtils.ValidateCollectionType(payloadType.ToTypeReference());
                IEdmTypeReference payloadItemTypeReference = payloadCollectionTypeReference.GetCollectionItemType();
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

            // Add the annotation with a null type name so that we know when the payload type is inferred from the expected type.
            // This is useful when validating a payload that inserts a derived entity (that does not specify a payload type) into the entity set. 
            if (payloadTypeName == null)
            {
                return new SerializationTypeNameAnnotation { TypeName = null };
            }

            return null;
        }

        /// <summary>
        /// Computes the type kind to be used to read the payload from the expected type, the payload type and
        /// possibly the payload shape.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference used to read the payload value.</param>
        /// <param name="forEntityValue">true when resolving a type name for an entity value; false for a non-entity value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="payloadTypeKind">The type kind of the payload value.</param>
        /// <param name="messageReaderSettings">The message reader settings.</param>
        /// <param name="typeKindFromPayloadFunc">A func to determine the type kind of the value by analyzing the payload data.</param>
        /// <returns>The type kind to be used to read the payload.</returns>
        private static EdmTypeKind ComputeTargetTypeKind(
            IEdmTypeReference expectedTypeReference,
            bool forEntityValue,
            string payloadTypeName,
            EdmTypeKind payloadTypeKind,
            ODataMessageReaderSettings messageReaderSettings,
            Func<EdmTypeKind> typeKindFromPayloadFunc)
        {
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");
            Debug.Assert(typeKindFromPayloadFunc != null, "typeKindFromPayloadFunc != null");

            // If we have a type resolver, we always use the type returned by the resolver
            // and use the expected type only for the resolution.
            bool useExpectedTypeOnlyForTypeResolution = messageReaderSettings.ReaderBehavior.TypeResolver != null && payloadTypeKind != EdmTypeKind.None;

            // Determine the target type kind
            // If the DisablePrimitiveTypeConversion is on, we must not infer the type kind from the expected type
            // but instead we need to read it from the payload.
            // This is necessary to correctly fail on complex/collection as well as to correctly read spatial values.
            EdmTypeKind targetTypeKind;
            if (expectedTypeReference != null &&
                !useExpectedTypeOnlyForTypeResolution &&
                (!expectedTypeReference.IsODataPrimitiveTypeKind() || !messageReaderSettings.DisablePrimitiveTypeConversion))
            {
                // If we have an expected type, use that.
                targetTypeKind = expectedTypeReference.TypeKind();
            }
            else if (payloadTypeKind != EdmTypeKind.None)
            {
                Debug.Assert(
                    messageReaderSettings.ReaderBehavior.TypeResolver == null || 
                    messageReaderSettings.ReaderBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient,
                    "A custom type resolver is only supported in the client format behavior.");

                // If we have a type kind based on the type name, use it.
                if (!forEntityValue)
                {
                    ValidationUtils.ValidateValueTypeKind(payloadTypeKind, payloadTypeName);
                }

                targetTypeKind = payloadTypeKind;
            }
            else
            {
                // If payloadTypeKind == EdmTypeKind.None, we could not determine the payload type kind
                // because there was no type name in the payload; detect the type kind from the shape of the payload.
                targetTypeKind = typeKindFromPayloadFunc();
            }

            Debug.Assert(targetTypeKind != EdmTypeKind.None, "We should have determined the target type kind by now.");

            if (ShouldValidatePayloadTypeKind(messageReaderSettings, expectedTypeReference, payloadTypeKind))
            {
                ValidationUtils.ValidateTypeKind(targetTypeKind, expectedTypeReference.TypeKind(), payloadTypeName);
            }

            return targetTypeKind;
        }

        /// <summary>
        /// Determines if the expect value type and the current settings mandate us to validate type kinds of payload values.
        /// </summary>
        /// <param name="messageReaderSettings">The message reader settings.</param>
        /// <param name="expectedValueTypeReference">The expected type reference for the value infered from the model.</param>
        /// <param name="payloadTypeKind">The type kind of the payload value.</param>
        /// <returns>true if the payload value kind must be verified, false otherwise.</returns>
        /// <remarks>This method deals with the strict versus lax behavior, as well as with the behavior when primitive type conversion is disabled.</remarks>
        private static bool ShouldValidatePayloadTypeKind(ODataMessageReaderSettings messageReaderSettings,  IEdmTypeReference expectedValueTypeReference, EdmTypeKind payloadTypeKind)
        {
            DebugUtils.CheckNoExternalCallers();

            // If we have a type resolver, we always use the type returned by the resolver
            // and use the expected type only for the resolution.
            bool useExpectedTypeOnlyForTypeResolution = messageReaderSettings.ReaderBehavior.TypeResolver != null && payloadTypeKind != EdmTypeKind.None;

            // Type kind validation must happen when
            // - In strict mode
            // - Target type is primitive and primitive type conversion is disabled
            // In lax mode we don't want to validate type kinds, but the DisablePrimitiveTypeConversion overrides the lax mode behavior.
            // If there's no expected type, then there's nothing to validate against (open property).
            return expectedValueTypeReference != null &&
                (!messageReaderSettings.DisableStrictMetadataValidation ||
                useExpectedTypeOnlyForTypeResolution ||
                (expectedValueTypeReference.IsODataPrimitiveTypeKind() && messageReaderSettings.DisablePrimitiveTypeConversion));
        }

        /// <summary>
        /// Validates that the specified <paramref name="expectedValueTypeReference"/> allows null values.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type for the value, or null if no such type is available.</param>
        /// <param name="validateNullValue">true to validate the null value; otherwise false.</param>
        /// <param name="model">The model to use to get the data service version.</param>
        private static void ValidateNullValueAllowed(IEdmTypeReference expectedValueTypeReference, bool validateNullValue, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "For null validation, model is required.");

            if (validateNullValue && expectedValueTypeReference != null)
            {
                Debug.Assert(
                    expectedValueTypeReference.IsODataPrimitiveTypeKind() ||
                    expectedValueTypeReference.IsODataComplexTypeKind() ||
                    expectedValueTypeReference.IsNonEntityODataCollectionTypeKind(),
                    "Only primitive, complex and collection types are supported by this method.");

                if (expectedValueTypeReference.IsODataPrimitiveTypeKind())
                {
                    if (!expectedValueTypeReference.IsNullable)
                    {
                        throw new ODataException(Strings.ReaderValidationUtils_NullValueForNonNullableType(expectedValueTypeReference.ODataFullName()));
                    }
                }
                else if (expectedValueTypeReference.IsNonEntityODataCollectionTypeKind())
                {
                    throw new ODataException(Strings.ReaderValidationUtils_NullValueForNonNullableType(expectedValueTypeReference.ODataFullName()));
                }
                else if (expectedValueTypeReference.IsODataComplexTypeKind())
                {
                    if (ValidationUtils.ShouldValidateComplexPropertyNullValue(model))
                    {
                        IEdmComplexTypeReference complexTypeReference = expectedValueTypeReference.AsComplex();
                        if (!complexTypeReference.IsNullable)
                        {
                            throw new ODataException(Strings.ReaderValidationUtils_NullValueForNonNullableType(expectedValueTypeReference.ODataFullName()));
                        }
                    }
                }
            }
        }
    }
}
