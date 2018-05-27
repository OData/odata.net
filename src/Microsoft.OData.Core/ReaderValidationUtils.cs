//---------------------------------------------------------------------
// <copyright file="ReaderValidationUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.JsonLight;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.UriParser;
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
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            if (messageReaderSettings.BaseUri != null && !messageReaderSettings.BaseUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute(UriUtils.UriToString(messageReaderSettings.BaseUri)));
            }
        }

        /// <summary>
        /// Validates an entity reference link.
        /// </summary>
        /// <param name="link">The entity reference link to check.</param>
        internal static void ValidateEntityReferenceLink(ODataEntityReferenceLink link)
        {
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
        /// <param name="throwOnUndeclaredLinkProperty">Whether ThrowOnUndeclaredLinkProperty validation setting is enabled.</param>
        internal static void ValidateStreamReferenceProperty(
            ODataProperty streamProperty, IEdmStructuredType structuredType,
            IEdmProperty streamEdmProperty, bool throwOnUndeclaredLinkProperty)
        {
            Debug.Assert(streamProperty != null, "streamProperty != null");

            ValidationUtils.ValidateStreamReferenceProperty(streamProperty, streamEdmProperty);

            if (structuredType != null && structuredType.IsOpen)
            {
                // If no property match was found in the metadata and an error wasn't raised,
                // it is an open property (which is not supported for streams).
                if (streamEdmProperty == null && throwOnUndeclaredLinkProperty)
                {
                    // Fails with the correct error message.
                    ValidationUtils.ValidateOpenPropertyValue(streamProperty.Name, streamProperty.Value);
                }
            }
        }

        /// <summary>
        /// Validate a null value.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type of the null value.</param>
        /// <param name="enablePrimitiveTypeConversion">Whether primitive type conversion is enabled.</param>
        /// <param name="validateNullValue">true to validate the null value; false to only check whether the type is supported.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <param name="isDynamicProperty">Indicates whether the property is dynamic or unknown.</param>
        internal static void ValidateNullValue(
            IEdmTypeReference expectedTypeReference,
            bool enablePrimitiveTypeConversion,
            bool validateNullValue,
            string propertyName,
            bool? isDynamicProperty)
        {
            // For a null value if we have an expected type
            //   - we validate that the expected type is nullable if type conversion is enabled
            //   - don't validate any primitive types if primitive type conversion is disabled
            // For a null value without an expected type
            //   - we simply return null (treat it is as primitive null value)
            if (expectedTypeReference != null)
            {
                if (enablePrimitiveTypeConversion || expectedTypeReference.TypeKind() != EdmTypeKind.Primitive)
                {
                    ValidateNullValueAllowed(expectedTypeReference, validateNullValue, propertyName, isDynamicProperty);
                }
            }
        }

        /// <summary>
        /// Validates that a property with the specified name exists on a given structured type.
        /// The structured type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="owningStructuredType">The owning type of the property with name <paramref name="propertyName"/>
        /// or null if no metadata is available.</param>
        /// <param name="throwOnUndeclaredPropertyForNonOpenType">Whether ThrowOnUndeclaredPropertyForNonOpenType validation setting is enabled.</param>
        /// <returns>The <see cref="IEdmProperty"/> instance representing the property with name <paramref name="propertyName"/>
        /// or null if no metadata is available.</returns>
        internal static IEdmProperty ValidatePropertyDefined(string propertyName,
                                                                  IEdmStructuredType owningStructuredType,
                                                                  bool throwOnUndeclaredPropertyForNonOpenType)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (owningStructuredType == null)
            {
                return null;
            }

            IEdmProperty property = owningStructuredType.FindProperty(propertyName);
            if (property == null && !owningStructuredType.IsOpen)
            {
                if (throwOnUndeclaredPropertyForNonOpenType)
                {
                    throw new ODataException(Strings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, owningStructuredType.FullTypeName()));
                }
                else
                {
                    Debug.Assert(!throwOnUndeclaredPropertyForNonOpenType, "!throwOnUndeclaredPropertyForNonOpenType");
                }
            }

            return property;
        }

        /// <summary>
        /// Creates an exception used when primitive type conversion fails.
        /// </summary>
        /// <param name="targetTypeReference">The target type reference to which the conversion failed.</param>
        /// <param name="innerException">Possible inner exception with more information about the failure.</param>
        /// <param name="stringValue">The string representation for the value.</param>
        /// <returns>The exception object to throw.</returns>
        internal static ODataException GetPrimitiveTypeConversionException(IEdmPrimitiveTypeReference targetTypeReference, Exception innerException, string stringValue)
        {
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");

            return new ODataException(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue(stringValue, targetTypeReference.FullName()), innerException);
        }

        /// <summary>
        /// Resolved the payload type name to the type.
        /// </summary>
        /// <param name="model">The model to use for the resolution.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadTypeName">The payload type name to resolve.</param>
        /// <param name="expectedTypeKind">The default payload type kind, this is used when the resolution is not possible,
        /// but the type name is not empty. (Should be either Complex or Entity).</param>
        /// <param name="clientCustomTypeResolver">The function of client custom type resolver.</param>
        /// <param name="payloadTypeKind">This is set to the detected payload type kind, or None if the type was not specified.</param>
        /// <returns>The resolved type. This may be null if either no user-specified model is specified, or the type name is not recognized by the model.</returns>
        /// <remarks>The method detects the payload kind even if the model does not recognize the type. It figures out primitive and collection types always,
        /// and uses the <paramref name="expectedTypeKind"/> for the rest.</remarks>
        internal static IEdmType ResolvePayloadTypeName(
            IEdmModel model,
            IEdmTypeReference expectedTypeReference,
            string payloadTypeName,
            EdmTypeKind expectedTypeKind,
            Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
            out EdmTypeKind payloadTypeKind)
        {
            if (payloadTypeName == null)
            {
                payloadTypeKind = EdmTypeKind.None;
                return null;
            }

            // Empty type names are allowed.
            if (payloadTypeName.Length == 0)
            {
                payloadTypeKind = expectedTypeKind;
                return null;
            }

            IEdmType payloadType = MetadataUtils.ResolveTypeNameForRead(
                model,
                expectedTypeReference == null ? null : expectedTypeReference.Definition,
                payloadTypeName,
                clientCustomTypeResolver,
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
        /// <param name="expectStructuredType">This value indicates if a structured type is expected to be return.
        /// True for structured type, false for non-structured type, null for indetermination.</param>
        /// <param name="defaultPrimitivePayloadType">The default payload type if none is specified in the payload;
        /// for ATOM this is Edm.String, for JSON it is null since there is no payload type name for primitive types in the payload.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadTypeName">The payload type name, or null if no payload type was specified.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="clientCustomTypeResolver">Custom type resolver used by the client.</param>
        /// <param name="throwIfTypeConflictsWithMetadata">Whether ThrowIfTypeConflictsWithMetadata is enabled.</param>
        /// <param name="enablePrimitiveTypeConversion">Whether primitive type conversion is enabled.</param>
        /// <param name="typeKindFromPayloadFunc">A func to compute the type kind from the payload shape if it could not be determined from the expected type or the payload type.</param>
        /// <param name="targetTypeKind">The target type kind to be used to read the payload.</param>
        /// <param name="typeAnnotation">Potentially non-null instance of an annotation to put on the value reported from the reader.</param>
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
            bool? expectStructuredType,
            IEdmType defaultPrimitivePayloadType,
            IEdmTypeReference expectedTypeReference,
            string payloadTypeName,
            IEdmModel model,
            Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
            bool throwIfTypeConflictsWithMetadata,
            bool enablePrimitiveTypeConversion,
            Func<EdmTypeKind> typeKindFromPayloadFunc,
            out EdmTypeKind targetTypeKind,
            out ODataTypeAnnotation typeAnnotation)
        {
            Debug.Assert(typeKindFromPayloadFunc != null, "typeKindFromPayloadFunc != null");
            Debug.Assert(
                expectedTypeKind == EdmTypeKind.None
                || expectedTypeKind.IsStructured() && expectStructuredType != false
                || !expectedTypeKind.IsStructured() && expectStructuredType != true,
                "expectedTypeKind == EdmTypeKind.None || expectedTypeKind.IsStructured() && expectStructuredType != false || !expectedTypeKind.IsStructured() && expectStructuredType != true");

            typeAnnotation = null;

            // What is the right behavior if both expected and actual types are specified for complex value?
            //             We decided that they have to match exactly.

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
                clientCustomTypeResolver,
                out payloadTypeKind);

            // Compute the target type kind based on the expected type, the payload type kind
            // and a function to detect the target type kind from the shape of the payload.
            bool forResource = expectStructuredType == true
                               || !expectStructuredType.HasValue && payloadTypeKind.IsStructured();

            targetTypeKind = ComputeTargetTypeKind(
                expectedTypeReference,
                forResource,
                payloadTypeName,
                payloadTypeKind,
                clientCustomTypeResolver,
                throwIfTypeConflictsWithMetadata,
                enablePrimitiveTypeConversion,
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
                    clientCustomTypeResolver,
                    enablePrimitiveTypeConversion,
                    throwIfTypeConflictsWithMetadata);
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
                    clientCustomTypeResolver,
                    throwIfTypeConflictsWithMetadata);

                if (targetTypeReference != null)
                {
                    typeAnnotation = CreateODataTypeAnnotation(payloadTypeName, payloadType, targetTypeReference);
                }
            }

            if ((expectedTypeKind != EdmTypeKind.None || (targetTypeKind != EdmTypeKind.Untyped && expectStructuredType == true)) && targetTypeReference != null)
            {
                ValidationUtils.ValidateTypeKind(targetTypeKind, expectedTypeKind, forResource, payloadTypeName);
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
        /// <param name="clientCustomTypeResolver">Custom type resolver used by client, or null if none.</param>
        /// <param name="enablePrimitiveTypeConversion">Whether primitive type conversion is enabled.</param>
        /// <param name="throwIfTypeConflictsWithMetadata">Whether ThrowIfTypeConflictsWithMetadata is enabled.</param>
        /// <returns>The target type reference to use for parsing the value. This method never returns null.</returns>
        internal static IEdmTypeReference ResolveAndValidatePrimitiveTargetType(
            IEdmTypeReference expectedTypeReference,
            EdmTypeKind payloadTypeKind,
            IEdmType payloadType,
            string payloadTypeName,
            IEdmType defaultPayloadType,
            IEdmModel model,
            Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
            bool enablePrimitiveTypeConversion,
            bool throwIfTypeConflictsWithMetadata)
        {
            Debug.Assert(
                payloadTypeKind == EdmTypeKind.Primitive || payloadTypeKind == EdmTypeKind.Complex ||
                payloadTypeKind == EdmTypeKind.Entity || payloadTypeKind == EdmTypeKind.Collection ||
                payloadTypeKind == EdmTypeKind.None || payloadTypeKind == EdmTypeKind.TypeDefinition,
                "The payload type kind must be one of None, Primitive, Complex, Entity, Collection or TypeDefinition.");
            Debug.Assert(
                expectedTypeReference == null || expectedTypeReference.TypeKind() == EdmTypeKind.Primitive,
                "This method only works for primitive expected type.");
            Debug.Assert(
                payloadType == null || payloadType.TypeKind == payloadTypeKind,
                "The payload type kind must match the payload type if that is available.");
            Debug.Assert(payloadType == null || payloadTypeName != null, "If we have a payload type, we must have its name as well.");

            bool useExpectedTypeOnlyForTypeResolution = clientCustomTypeResolver != null && payloadType != null;

            // Validate type kinds except for open properties or when in lax mode, but only if primitive type conversion is enabled.
            // If primitive type conversion is disabled, the type kind must match, no matter what validation mode is used.
            // The rules for primitive types are:
            // - In the strict mode the payload value must be convertible to the expected type. So the payload type must be a primitive type.
            // - In the lax mode the payload type is ignored, so its type kind is not verified in any way
            // - If the DisablePrimitiveTypeConversion == true, the lax/strict mode doesn't matter and we will read the payload value on its own.
            //     In this case we require the payload value to always be a primitive type (so type kinds must match), but it may not be convertible
            //     to the expected type, it will still be reported to the caller.
            if (payloadTypeKind != EdmTypeKind.None && (!enablePrimitiveTypeConversion || throwIfTypeConflictsWithMetadata))
            {
                // Make sure that the type kinds match.
                ValidationUtils.ValidateTypeKind(payloadTypeKind, EdmTypeKind.Primitive, null, payloadTypeName);
            }

            if (!model.IsUserModel())
            {
                // If there's no model, it means we should not have the expected type either, and that there's no type to use,
                // no metadata validation to perform.
                Debug.Assert(expectedTypeReference == null, "If we don't have a model, we must not have expected type either.");
                return MetadataUtils.GetNullablePayloadTypeReference(payloadType ?? defaultPayloadType);
            }

            // If the primitive type conversion is off, use the payload type always.
            // If there's no expected type or the expected type is ignored, use the payload type as well.
            if (expectedTypeReference == null || useExpectedTypeOnlyForTypeResolution || !enablePrimitiveTypeConversion)
            {
                // If there's no payload type, use the default payload type.
                // Note that in collections the items without type should inherit the type name from the collection, in that case the expectedTypeReference
                // is never null (assuming we do have a model), so we won't get here.
                return MetadataUtils.GetNullablePayloadTypeReference(payloadType ?? defaultPayloadType);
            }

            // The server ignores the payload type when expected type is specified
            if (!throwIfTypeConflictsWithMetadata)
            {
                // Always use the expected type, the payload type is ignored.
                return expectedTypeReference;
            }

            // We assume the expected type in the case where no payload type is specified
            // for a primitive value; if no expected type is available we assume Edm.String.
            if (payloadType != null)
            {
                // The payload type must be convertible to the expected type.
                // Note that we compare the type definitions, since we want to ignore nullability (the payload type doesn't specify nullability).
                if (!MetadataUtilsCommon.CanConvertPrimitiveTypeTo(
                    null /* sourceNodeOrNull */,
                    (IEdmPrimitiveType)payloadType.AsActualType(),
                    (IEdmPrimitiveType)(expectedTypeReference.Definition)))
                {
                    throw new ODataException(Strings.ValidationUtils_IncompatibleType(payloadTypeName, expectedTypeReference.FullName()));
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
        /// <param name="clientCustomTypeResolver">Custom type resolver used by client, or null if none.</param>
        /// <param name="throwIfTypeConflictsWithMetadata">Whether ThrowIfTypeConflictsWithMetadata is enabled.</param>
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
            Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
            bool throwIfTypeConflictsWithMetadata)
        {
            Debug.Assert(
                expectedTypeKind == EdmTypeKind.Enum || expectedTypeKind == EdmTypeKind.Complex || expectedTypeKind == EdmTypeKind.Entity ||
                expectedTypeKind == EdmTypeKind.Collection || expectedTypeKind == EdmTypeKind.TypeDefinition || expectedTypeKind == EdmTypeKind.Untyped,
                "The expected type kind must be one of Enum, Complex, Entity, Collection or TypeDefinition.");
            Debug.Assert(
                payloadTypeKind == EdmTypeKind.Complex || payloadTypeKind == EdmTypeKind.Entity ||
                payloadTypeKind == EdmTypeKind.Collection || payloadTypeKind == EdmTypeKind.None ||
                payloadTypeKind == EdmTypeKind.Primitive || payloadTypeKind == EdmTypeKind.Enum ||
                payloadTypeKind == EdmTypeKind.TypeDefinition || payloadTypeKind == EdmTypeKind.Untyped,
                "The payload type kind must be one of None, Primitive, Enum, Untyped, Complex, Entity, Collection or TypeDefinition.");
            Debug.Assert(
                expectedTypeReference == null || expectedTypeReference.TypeKind() == expectedTypeKind,
                "The expected type kind must match the expected type reference if that is available.");
            Debug.Assert(
                payloadType == null || payloadType.TypeKind == payloadTypeKind,
                "The payload type kind must match the payload type if that is available.");
            Debug.Assert(payloadType == null || payloadTypeName != null, "If we have a payload type, we must have its name as well.");

            bool useExpectedTypeOnlyForTypeResolution = clientCustomTypeResolver != null && payloadType != null;
            if (!useExpectedTypeOnlyForTypeResolution)
            {
                // We should validate that the payload type resolved before anything else to produce reasonable error messages
                // Otherwise we might report errors which are somewhat confusing (like "Type '' is Complex but Collection was expected.").
                if (model.IsUserModel() && (expectedTypeReference == null || throwIfTypeConflictsWithMetadata))
                {
                    // When using a type resolver (i.e., useExpectedTypeOnlyForTypeResolution == true) then we don't have to
                    // call this method because the contract with the type resolver is to always resolve the type name and thus
                    // we will always get a defined type.
                    VerifyPayloadTypeDefined(payloadTypeName, payloadType);
                }
            }

            if (payloadTypeKind != EdmTypeKind.None && (throwIfTypeConflictsWithMetadata || expectedTypeReference == null))
            {
                // Make sure that the type kinds match.
                ValidationUtils.ValidateTypeKind(payloadTypeKind, expectedTypeKind, null, payloadTypeName);
            }

            if (!model.IsUserModel())
            {
                // If there's no model, it means we should not have the expected type either, and that there's no type to use,
                // no metadata validation to perform.
                Debug.Assert(expectedTypeReference == null, "If we don't have a model, we must not have expected type either.");
                return null;
            }

            if (expectedTypeReference == null || useExpectedTypeOnlyForTypeResolution)
            {
                Debug.Assert(payloadTypeName == null || payloadType != null, "The payload type must have resolved before we get here.");
                return ResolveAndValidateTargetTypeWithNoExpectedType(
                    expectedTypeKind,
                    payloadType);
            }

            if (!throwIfTypeConflictsWithMetadata)
            {
                return ResolveAndValidateTargetTypeStrictValidationDisabled(
                    expectedTypeKind,
                    expectedTypeReference,
                    payloadType);
            }

            Debug.Assert(payloadTypeName == null || payloadType != null, "The payload type must have resolved before we get here.");
            return ResolveAndValidateTargetTypeStrictValidationEnabled(
                expectedTypeKind,
                expectedTypeReference,
                payloadType);
        }

        /// <summary>
        /// Validates that the specified encoding is supported in batch/changeset envelopes (headers, boundaries, preamble, etc.).
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to check.</param>
        internal static void ValidateEncodingSupportedInBatch(Encoding encoding)
        {
            Debug.Assert(encoding != null, "encoding != null");

#if !ORCAS
            if (string.CompareOrdinal(Encoding.UTF8.WebName, encoding.WebName) != 0)
#else
            if (!encoding.IsSingleByte && Encoding.UTF8.CodePage != encoding.CodePage)
#endif
            {
                // TODO: Batch reader does not support multi codepoint encodings
                // We decided to not support multi-byte encodings other than UTF8 for now.
                throw new ODataException(Strings.ODataBatchReaderStream_MultiByteEncodingsNotSupported(encoding.WebName));
            }
        }

        /// <summary>
        /// Validates that the specified encoding is supported in async envelopes (headers, preamble, etc.).
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to check.</param>
        internal static void ValidateEncodingSupportedInAsync(Encoding encoding)
        {
            Debug.Assert(encoding != null, "encoding != null");

#if !ORCAS
            if (string.CompareOrdinal(Encoding.UTF8.WebName, encoding.WebName) != 0)
#else
            if (!encoding.IsSingleByte && Encoding.UTF8.CodePage != encoding.CodePage)
#endif
            {
                // We decided to not support multi-byte encodings other than UTF8 for now.
                throw new ODataException(Strings.ODataAsyncReader_MultiByteEncodingsNotSupported(encoding.WebName));
            }
        }

        /// <summary>
        /// Validates that the parsed context URI from the payload is consistent with the expected
        /// entity set and entity type when reading a resource set or resource payload.
        /// </summary>
        /// <param name="contextUriParseResult">The parse result of the context URI from the payload.</param>
        /// <param name="scope">The top-level scope representing the reader state.</param>
        /// <param name="updateScope">Whether to update scope when validating.</param>
        internal static void ValidateResourceSetOrResourceContextUri(ODataJsonLightContextUriParseResult contextUriParseResult, ODataReaderCore.Scope scope, bool updateScope)
        {
            if (contextUriParseResult.EdmType is IEdmCollectionType)
            {
                ValidateResourceSetContextUri(contextUriParseResult, scope, updateScope);
                return;
            }

            Debug.Assert(contextUriParseResult != null, "contextUriParseResult != null");
            Debug.Assert(
                contextUriParseResult.Path != null && contextUriParseResult.Path.IsUndeclared() ||
                contextUriParseResult.NavigationSource != null || (contextUriParseResult.EdmType != null && contextUriParseResult.EdmType is IEdmStructuredType),
                "contextUriParseResult.Path != null && contextUriParseResult.Path.IsUndeclared() || contextUriParseResult.NavigationSource != null || (contextUriParseResult.EdmType != null && contextUriParseResult.EdmType is IEdmStructuredType)");

            // Set the navigation source name or make sure the navigation source names match.
            if (scope.NavigationSource == null)
            {
                if (updateScope)
                {
                    scope.NavigationSource = contextUriParseResult.NavigationSource;
                }
            }
            else if (contextUriParseResult.NavigationSource != null && string.CompareOrdinal(scope.NavigationSource.FullNavigationSourceName(), contextUriParseResult.NavigationSource.FullNavigationSourceName()) != 0)
            {
                throw new ODataException(Strings.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet(
                    UriUtils.UriToString(contextUriParseResult.ContextUri),
                    contextUriParseResult.NavigationSource.FullNavigationSourceName(),
                    scope.NavigationSource.FullNavigationSourceName()));
            }

            // Set the resource type or make sure the resource types are assignable.
            IEdmStructuredType payloadEntityType = (IEdmStructuredType)contextUriParseResult.EdmType;

            if (payloadEntityType == null)
            {
                // for dynmaic path, the contextUriParseResult.EdmType might be null;
                return;
            }

            if (scope.ResourceType == null)
            {
                if (updateScope)
                {
                    scope.ResourceType = payloadEntityType;
                }
            }
            else if (scope.ResourceType.IsAssignableFrom(payloadEntityType))
            {
                if (updateScope)
                {
                    scope.ResourceType = payloadEntityType;
                }
            }
            else if (!payloadEntityType.IsAssignableFrom(scope.ResourceType))
            {
                throw new ODataException(Strings.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntityType(
                    UriUtils.UriToString(contextUriParseResult.ContextUri),
                    contextUriParseResult.EdmType.FullTypeName(),
                    scope.ResourceType.FullTypeName()));
            }
        }

        /// <summary>
        /// Validates that the parsed context URI from the payload is consistent with the expected
        /// collection item type when reading collection payloads.
        /// </summary>
        /// <param name="contextUriParseResult">The parse result of the context URI from the payload.</param>
        /// <param name="expectedItemTypeReference">The expected item type of the collection items.</param>
        /// <returns>The actual item type of the collection items.</returns>
        internal static IEdmTypeReference ValidateCollectionContextUriAndGetPayloadItemTypeReference(
            ODataJsonLightContextUriParseResult contextUriParseResult,
            IEdmTypeReference expectedItemTypeReference)
        {
            if (contextUriParseResult == null || contextUriParseResult.EdmType == null)
            {
                return expectedItemTypeReference;
            }

            if (contextUriParseResult.EdmType is IEdmCollectionType)
            {
                Debug.Assert(contextUriParseResult.EdmType is IEdmCollectionType, "contextUriParseResult.EdmType is IEdmCollectionType");
                IEdmCollectionType actualCollectionType = (IEdmCollectionType)contextUriParseResult.EdmType;
                if (expectedItemTypeReference != null)
                {
                    // We allow co-variance in collection types (e.g., expecting the item type of Geography from a payload of Collection(GeographyPoint).
                    if (!expectedItemTypeReference.IsAssignableFrom(actualCollectionType.ElementType))
                    {
                        throw new ODataException(Strings.ReaderValidationUtils_ContextUriDoesNotReferTypeAssignableToExpectedType(
                            UriUtils.UriToString(contextUriParseResult.ContextUri),
                            actualCollectionType.ElementType.FullName(),
                            expectedItemTypeReference.FullName()));
                    }
                }

                return actualCollectionType.ElementType;
            }

            Debug.Assert(contextUriParseResult.EdmType is IEdmComplexType && contextUriParseResult.DetectedPayloadKinds.Any(k => k == ODataPayloadKind.Collection),
                "contextUriParseResult.EdmType is IEdmComplexType");
            if (expectedItemTypeReference != null && !expectedItemTypeReference.Definition.IsAssignableFrom(contextUriParseResult.EdmType))
            {
                throw new ODataException(Strings.ReaderValidationUtils_ContextUriDoesNotReferTypeAssignableToExpectedType(
                    UriUtils.UriToString(contextUriParseResult.ContextUri),
                    contextUriParseResult.EdmType.FullTypeName(),
                    expectedItemTypeReference.Definition.FullTypeName()));
            }

            return contextUriParseResult.EdmType.ToTypeReference(true);
        }

        /// <summary>
        /// Validates that the property in an operation (an action or a function) is valid.
        /// </summary>
        /// <param name="propertyValue">The value of the property.</param>
        /// <param name="propertyName">The name of the property (used for error reporting).</param>
        /// <param name="metadata">The metadata value for the operation (used for error reporting).</param>
        /// <param name="operationsHeader">The header for the operation, either 'actions' or 'functions'.</param>
        internal static void ValidateOperationProperty(object propertyValue, string propertyName, string metadata, string operationsHeader)
        {
            Debug.Assert(!String.IsNullOrEmpty(metadata), "!string.IsNullOrEmpty(metadata)");
            Debug.Assert(!String.IsNullOrEmpty(operationsHeader), "!string.IsNullOrEmpty(operationsHeader)");

            if (propertyValue == null)
            {
                throw new ODataException(Strings.ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull(
                    propertyName,
                    metadata,
                    operationsHeader));
            }
        }

        /// <summary>
        /// Resolves the payload type if there's no expected type.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="payloadType">The payload type, or null if the payload type was not specified, or it didn't resolve against the model.</param>
        /// <returns>The target type reference to use for parsing the value.</returns>
        private static IEdmTypeReference ResolveAndValidateTargetTypeWithNoExpectedType(
            EdmTypeKind expectedTypeKind,
            IEdmType payloadType)
        {
            // No expected type (for example an open property, but other scenarios are possible)
            // We need some type to go on. We do have a model, so we must perform metadata validation and for that we need a type.
            if (payloadType == null)
            {
                if (expectedTypeKind == EdmTypeKind.Entity)
                {
                    throw new ODataException(Strings.ReaderValidationUtils_ResourceWithoutType);
                }

                return null; // supports undeclared property
            }

            // Payload types are always nullable.
            IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference(/*nullable*/ true);

            // Use the payload type (since we don't have any other).
            return payloadTypeReference;
        }

        /// <summary>
        /// Resolves the payload type versus the expected type and validates that such combination is allowed when the strict validation is disabled.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadType">The payload type, or null if the payload type was not specified, or it didn't resolve against the model.</param>
        /// <returns>The target type reference to use for parsing the value.</returns>
        private static IEdmTypeReference ResolveAndValidateTargetTypeStrictValidationDisabled(
            EdmTypeKind expectedTypeKind,
            IEdmTypeReference expectedTypeReference,
            IEdmType payloadType)
        {
            // Lax validation logic
            switch (expectedTypeKind)
            {
                case EdmTypeKind.Complex:
                    // if the expectedTypeKind is different from the payloadType.TypeKind the types are not related
                    // in any way. In that case we will just use the expected type.
                    if (payloadType != null && expectedTypeKind == payloadType.TypeKind)
                    {
                        if (EdmLibraryExtensions.IsAssignableFrom(expectedTypeReference.AsComplex().ComplexDefinition(), (IEdmComplexType)payloadType))
                        {
                            return payloadType.ToTypeReference(/*nullable*/ true);
                        }
                    }

                    break;
                case EdmTypeKind.Entity:
                    // if the expectedTypeKind is different from the payloadType.TypeKind the types are not related
                    // in any way. In that case we will just use the expected type.
                    if (payloadType != null && expectedTypeKind == payloadType.TypeKind)
                    {
                        // If the type is assignable (equal or derived) we will use the payload type, since we want to allow derived entities
                        if (EdmLibraryExtensions.IsAssignableFrom(expectedTypeReference.AsEntity().EntityDefinition(), (IEdmEntityType)payloadType))
                        {
                            IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference(/*nullable*/ true);
                            return payloadTypeReference;
                        }
                    }

                    break;
                case EdmTypeKind.Collection:
                    // if the expectedTypeKind is different from the payloadType.TypeKind the types are not related
                    // in any way. In that case we will just use the expected type.
                    if (payloadType != null && expectedTypeKind == payloadType.TypeKind)
                    {
                        VerifyCollectionComplexItemType(expectedTypeReference, payloadType);
                    }

                    break;
                case EdmTypeKind.Untyped: // untyped: no validation (can be anything)

                    break;
                case EdmTypeKind.Enum: // enum: no validation

                    break;
                case EdmTypeKind.TypeDefinition: // type definition: no validation

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind));
            }

            // Either there's no payload type, in which case use the expected one, or the payload one and the expected one are equal.
            return expectedTypeReference;
        }

        /// <summary>
        /// Resolves the payload type versus the expected type and validates that such combination is allowed when strict validation is enabled.
        /// </summary>
        /// <param name="expectedTypeKind">The expected type kind for the value.</param>
        /// <param name="expectedTypeReference">The expected type reference, or null if no expected type is available.</param>
        /// <param name="payloadType">The payload type, or null if the payload type was not specified, or it didn't resolve against the model.</param>
        /// <returns>The target type reference to use for parsing the value.</returns>
        private static IEdmTypeReference ResolveAndValidateTargetTypeStrictValidationEnabled(
            EdmTypeKind expectedTypeKind,
            IEdmTypeReference expectedTypeReference,
            IEdmType payloadType)
        {
            // Strict validation logic
            switch (expectedTypeKind)
            {
                case EdmTypeKind.Complex:
                    if (payloadType != null)
                    {
                        // The payload type must be compatible to the expected type.
                        VerifyComplexType(expectedTypeReference, payloadType, /* failIfNotRelated */ true);

                        // Use the payload type
                        return payloadType.ToTypeReference(/*nullable*/ true);
                    }

                    break;
                case EdmTypeKind.Entity:
                    if (payloadType != null)
                    {
                        // The payload type must be assignable to the expected type.
                        IEdmTypeReference payloadTypeReference = payloadType.ToTypeReference(/*nullable*/ true);
                        ValidationUtils.ValidateEntityTypeIsAssignable((IEdmEntityTypeReference)expectedTypeReference, (IEdmEntityTypeReference)payloadTypeReference);

                        // Use the payload type
                        return payloadTypeReference;
                    }

                    break;
                case EdmTypeKind.Enum:
                    if (payloadType != null && string.CompareOrdinal(payloadType.FullTypeName(), expectedTypeReference.FullName()) != 0)
                    {
                        throw new ODataException(Strings.ValidationUtils_IncompatibleType(payloadType.FullTypeName(), expectedTypeReference.FullName()));
                    }

                    break;
                case EdmTypeKind.Collection:
                    // The type must be exactly equal - note that we intentionally ignore nullability of the items here, since the payload type
                    // can't specify that.
                    if (payloadType != null && !payloadType.IsElementTypeEquivalentTo(expectedTypeReference.Definition))
                    {
                        VerifyCollectionComplexItemType(expectedTypeReference, payloadType);

                        throw new ODataException(Strings.ValidationUtils_IncompatibleType(payloadType.FullTypeName(), expectedTypeReference.FullName()));
                    }

                    break;
                case EdmTypeKind.TypeDefinition:
                    if (payloadType != null && !expectedTypeReference.Definition.IsAssignableFrom(payloadType))
                    {
                        throw new ODataException(Strings.ValidationUtils_IncompatibleType(payloadType.FullTypeName(), expectedTypeReference.FullName()));
                    }

                    break;
                case EdmTypeKind.Untyped:
                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind));
            }

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
        /// The method verifies that the <paramref name="payloadType"/> equals to or derives from the <paramref name="expectedTypeReference"/>
        /// and always fails in other cases.
        /// </remarks>
        private static void VerifyComplexType(IEdmTypeReference expectedTypeReference, IEdmType payloadType, bool failIfNotRelated)
        {
            Debug.Assert(expectedTypeReference != null, "expectedTypeReference != null");
            Debug.Assert(payloadType != null, "payloadType != null");

            // Note that we compare the type definitions, since we want to ignore nullability (the payload type doesn't specify nullability).
            IEdmStructuredType structuredExpectedType = expectedTypeReference.AsStructured().StructuredDefinition();
            IEdmStructuredType structuredPayloadType = (IEdmStructuredType)payloadType;

            if (!EdmLibraryExtensions.IsAssignableFrom(structuredExpectedType, structuredPayloadType))
            {
                if (failIfNotRelated)
                {
                    // And now the generic one when the types are not related at all.
                    throw new ODataException(Strings.ValidationUtils_IncompatibleType(structuredPayloadType.FullTypeName(), structuredExpectedType.FullTypeName()));
                }
            }
        }

        /// <summary>
        /// Verifies that in case of collection types, the item type is valid.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference.</param>
        /// <param name="payloadType">The payload type.</param>
        /// <remarks>
        /// This method verifies that item type is compatible with expected type.
        /// </remarks>
        private static void VerifyCollectionComplexItemType(IEdmTypeReference expectedTypeReference, IEdmType payloadType)
        {
            Debug.Assert(expectedTypeReference != null, "expectedTypeReference != null");
            Debug.Assert(payloadType != null, "payloadType != null");
            Debug.Assert(expectedTypeReference.IsNonEntityCollectionType(), "This method only works on atomic collections.");
            Debug.Assert(payloadType.IsNonEntityCollectionType(), "This method only works on atomic collections.");

            IEdmCollectionTypeReference collectionTypeReference = ValidationUtils.ValidateCollectionType(expectedTypeReference);
            IEdmTypeReference expectedItemTypeReference = collectionTypeReference.GetCollectionItemType();
            if (expectedItemTypeReference != null && expectedItemTypeReference.IsODataComplexTypeKind())
            {
                IEdmCollectionTypeReference payloadCollectionTypeReference = ValidationUtils.ValidateCollectionType(payloadType.ToTypeReference());
                IEdmTypeReference payloadItemTypeReference = payloadCollectionTypeReference.GetCollectionItemType();
                if (payloadItemTypeReference != null && payloadItemTypeReference.IsODataComplexTypeKind())
                {
                    // Note that this method is called from both strict and lax code paths, so we must not fail if the types are not related.
                    VerifyComplexType(expectedItemTypeReference, payloadItemTypeReference.Definition, /* failIfNotRelated */ false);
                }
            }
        }

        /// <summary>
        /// Conditionally creates the annotation to put on the read value in order to retain the type name from the payload.
        /// </summary>
        /// <param name="payloadTypeName">The payload type name.</param>
        /// <param name="payloadType">The payload type.</param>
        /// <param name="targetTypeReference">The type reference into which we're going to parse.</param>
        /// <returns>The annotation to report to the reader for adding on the read value.</returns>
        private static ODataTypeAnnotation CreateODataTypeAnnotation(string payloadTypeName, IEdmType payloadType, IEdmTypeReference targetTypeReference)
        {
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");

            if (payloadType != null && !payloadType.IsEquivalentTo(targetTypeReference.Definition))
            {
                return new ODataTypeAnnotation(payloadTypeName, payloadType);
            }

            // Add the annotation with a null type name so that we know when the payload type is inferred from the expected type.
            // This is useful when validating a payload that inserts a derived entity (that does not specify a payload type) into the entity set.
            if (payloadType == null)
            {
                return new ODataTypeAnnotation();
            }

            return null;
        }

        /// <summary>
        /// Computes the type kind to be used to read the payload from the expected type, the payload type and
        /// possibly the payload shape.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference used to read the payload value.</param>
        /// <param name="forResource">true when resolving a type name for a resource; false for a non-resource.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="payloadTypeKind">The type kind of the payload value.</param>
        /// <param name="clientCustomTypeResolver">Custom type resolver used by the client.</param>
        /// <param name="throwIfTypeConflictsWithMetadata">Whether ThrowIfTypeConflictsWithMetadata is enabled.</param>
        /// <param name="enablePrimitiveTypeConversion">Whether primitive type conversion is disabled.</param>
        /// <param name="typeKindFromPayloadFunc">A func to determine the type kind of the value by analyzing the payload data.</param>
        /// <returns>The type kind to be used to read the payload.</returns>
        private static EdmTypeKind ComputeTargetTypeKind(
            IEdmTypeReference expectedTypeReference,
            bool forResource,
            string payloadTypeName,
            EdmTypeKind payloadTypeKind,
            Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
            bool throwIfTypeConflictsWithMetadata,
            bool enablePrimitiveTypeConversion,
            Func<EdmTypeKind> typeKindFromPayloadFunc)
        {
            Debug.Assert(typeKindFromPayloadFunc != null, "typeKindFromPayloadFunc != null");

            // If we have a type resolver, we always use the type returned by the resolver
            // and use the expected type only for the resolution.
            bool useExpectedTypeOnlyForTypeResolution = clientCustomTypeResolver != null && payloadTypeKind != EdmTypeKind.None;

            // Determine the target type kind
            // If the EnablePrimitiveTypeConversion is off, we must not infer the type kind from the expected type
            // but instead we need to read it from the payload.
            // This is necessary to correctly fail on complex/collection as well as to correctly read spatial values.
            EdmTypeKind expectedTypeKind = EdmTypeKind.None;
            if (!useExpectedTypeOnlyForTypeResolution)
            {
                expectedTypeKind = ReaderUtils.GetExpectedTypeKind(expectedTypeReference, enablePrimitiveTypeConversion);
            }

            EdmTypeKind targetTypeKind;
            if (expectedTypeKind != EdmTypeKind.None)
            {
                // If we have an expected type, use that.
                targetTypeKind = expectedTypeKind;
            }
            else
            {
                if (payloadTypeKind != EdmTypeKind.None)
                {
                    // If we have a type kind based on the type name, use it.
                    if (!forResource)
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
            }

            Debug.Assert(targetTypeKind != EdmTypeKind.None, "We should have determined the target type kind by now.");

            if (ShouldValidatePayloadTypeKind(
                clientCustomTypeResolver,
                throwIfTypeConflictsWithMetadata,
                enablePrimitiveTypeConversion,
                expectedTypeReference, payloadTypeKind))
            {
                ValidationUtils.ValidateTypeKind(targetTypeKind, expectedTypeReference.TypeKind(), null, payloadTypeName);
            }

            return targetTypeKind;
        }

        /// <summary>
        /// Determines if the expect value type and the current settings mandate us to validate type kinds of payload values.
        /// </summary>
        /// <param name="clientCustomTypeResolver">Custom type resolver used by the client.</param>
        /// <param name="throwIfTypeConflictsWithMetadata">Whether ThrowIfTypeConflictsWithMetadata is enabled.</param>
        /// <param name="enablePrimitiveTypeConversion">Whether primitive type conversion is enabled.</param>
        /// <param name="expectedValueTypeReference">The expected type reference for the value inferred from the model.</param>
        /// <param name="payloadTypeKind">The type kind of the payload value.</param>
        /// <returns>true if the payload value kind must be verified, false otherwise.</returns>
        /// <remarks>This method deals with the strict versus lax behavior, as well as with the behavior when primitive type conversion is disabled.</remarks>
        private static bool ShouldValidatePayloadTypeKind(
            Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
            bool throwIfTypeConflictsWithMetadata,
            bool enablePrimitiveTypeConversion,
            IEdmTypeReference expectedValueTypeReference,
            EdmTypeKind payloadTypeKind)
        {
            // If we have a type resolver, we always use the type returned by the resolver
            // and use the expected type only for the resolution.
            bool useExpectedTypeOnlyForTypeResolution = clientCustomTypeResolver != null && payloadTypeKind != EdmTypeKind.None;

            // Type kind validation must happen when
            // - ThrowIfTypeConflictsWithMetadata is set
            // - Target type is primitive and primitive type conversion is disabled
            // And the EnablePrimitiveTypeConversion overrides the ThrowIfTypeConflictsWithMetadata behavior.
            // If there's no expected type, then there's nothing to validate against (open property).
            return expectedValueTypeReference != null &&
                (throwIfTypeConflictsWithMetadata ||
                useExpectedTypeOnlyForTypeResolution ||
                (expectedValueTypeReference.IsODataPrimitiveTypeKind() && !enablePrimitiveTypeConversion));
        }

        /// <summary>
        /// Validates that the specified <paramref name="expectedValueTypeReference"/> allows null values.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type for the value, or null if no such type is available.</param>
        /// <param name="validateNullValue">true to validate the null value; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <param name="isDynamicProperty">Indicates whether the property is dynamic or unknown.</param>
        private static void ValidateNullValueAllowed(IEdmTypeReference expectedValueTypeReference, bool validateNullValue, string propertyName, bool? isDynamicProperty)
        {
            if (validateNullValue && expectedValueTypeReference != null)
            {
                Debug.Assert(
                    expectedValueTypeReference.IsODataPrimitiveTypeKind() ||
                    expectedValueTypeReference.IsODataTypeDefinitionTypeKind() ||
                    expectedValueTypeReference.IsODataEnumTypeKind() ||
                    expectedValueTypeReference.IsODataComplexTypeKind() ||
                    expectedValueTypeReference.IsNonEntityCollectionType(),
                    "Only primitive, type definition, Enum, complex and collection types are supported by this method.");

                if (expectedValueTypeReference.IsODataPrimitiveTypeKind())
                {
                    // COMPAT 55: WCF DS allows null values for non-nullable properties
                    // For now ODataLib will always fail on null value when it is to be reported for a non-nullable property
                    // We should add a knob since WCF DS might need the different behavior.
                    if (!expectedValueTypeReference.IsNullable)
                    {
                        ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
                    }
                }
                else if (expectedValueTypeReference.IsODataEnumTypeKind())
                {
                    if (!expectedValueTypeReference.IsNullable)
                    {
                        ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
                    }
                }
                else if (expectedValueTypeReference.IsNonEntityCollectionType())
                {
                    if (isDynamicProperty != true)
                    {
                        ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
                    }
                }
                else if (expectedValueTypeReference.IsODataComplexTypeKind())
                {
                    IEdmComplexTypeReference complexTypeReference = expectedValueTypeReference.AsComplex();
                    if (!complexTypeReference.IsNullable)
                    {
                        ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
                    }
                }
            }
        }

        /// <summary>
        /// Create and throw exception that a null value was found when the expected type is non-nullable.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type for this value.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable.</param>
        private static void ThrowNullValueForNonNullableTypeException(IEdmTypeReference expectedValueTypeReference, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ODataException(Strings.ReaderValidationUtils_NullValueForNonNullableType(expectedValueTypeReference.FullName()));
            }

            throw new ODataException(Strings.ReaderValidationUtils_NullNamedValueForNonNullableType(propertyName, expectedValueTypeReference.FullName()));
        }


        /// <summary>
        /// The validate resource set context uri.
        /// </summary>
        /// <param name="contextUriParseResult">
        /// The context uri parse result.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <param name="updateScope">
        /// The update scope.
        /// </param>
        private static void ValidateResourceSetContextUri(ODataJsonLightContextUriParseResult contextUriParseResult, ODataReaderCore.Scope scope, bool updateScope)
        {
            // TODO: add validation logic for a resource set context uri
        }
    }
}
