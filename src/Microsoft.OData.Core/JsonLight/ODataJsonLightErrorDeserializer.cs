//---------------------------------------------------------------------
// <copyright file="ODataJsonLightErrorDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for errors.
    /// </summary>
    internal sealed class ODataJsonLightErrorDeserializer : ODataJsonLightDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightErrorDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
        }

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None       - The reader must not have been used yet.
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        internal ODataError ReadTopLevelError()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            Debug.Assert(!this.JsonReader.DisableInStreamErrorDetection, "!JsonReader.DisableInStreamErrorDetection");
            this.JsonReader.AssertNotBuffering();

            // prevent the buffering JSON reader from detecting in-stream errors - we read the error ourselves
            // to throw proper exceptions
            this.JsonReader.DisableInStreamErrorDetection = true;

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            try
            {
                // Position the reader on the first node
                this.ReadPayloadStart(
                    ODataPayloadKind.Error,
                    duplicatePropertyNamesChecker,
                    /*isReadingNestedPayload*/false,
                    /*allowEmptyPayload*/false);

                ODataError result = this.ReadTopLevelErrorImplementation();

                Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: JsonNodeType.EndOfInput");
                this.JsonReader.AssertNotBuffering();

                return result;
            }
            finally
            {
                this.JsonReader.DisableInStreamErrorDetection = false;
            }
        }

#if PORTABLELIB

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataError"/> representing the read error.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None       - The reader must not have been used yet.
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        internal Task<ODataError> ReadTopLevelErrorAsync()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            Debug.Assert(!this.JsonReader.DisableInStreamErrorDetection, "!JsonReader.DisableInStreamErrorDetection");
            this.JsonReader.AssertNotBuffering();

            // prevent the buffering JSON reader from detecting in-stream errors - we read the error ourselves
            // to throw proper exceptions
            this.JsonReader.DisableInStreamErrorDetection = true;

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            // Position the reader on the first node
            return this.ReadPayloadStartAsync(
                ODataPayloadKind.Error,
                duplicatePropertyNamesChecker,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false)

                .FollowOnSuccessWith(t =>
                {
                    ODataError result = this.ReadTopLevelErrorImplementation();

                    Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: JsonNodeType.EndOfInput");
                    this.JsonReader.AssertNotBuffering();

                    return result;
                })

                .FollowAlwaysWith(t =>
                {
                    this.JsonReader.DisableInStreamErrorDetection = false;
                });
        }
#endif

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property       - The first property of the top level object.
        ///                 JsonNodeType.EndObject      - If there are no properties in the top level object.
        ///                 any                         - Will throw if anything else.
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        private ODataError ReadTopLevelErrorImplementation()
        {
            ODataError error = null;

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                if (string.CompareOrdinal(JsonLightConstants.ODataErrorPropertyName, propertyName) != 0)
                {
                    // we only allow a single 'error' property for a top-level error object
                    throw new ODataException(Strings.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty(propertyName));
                }

                if (error != null)
                {
                    throw new ODataException(Strings.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName(JsonLightConstants.ODataErrorPropertyName));
                }

                error = new ODataError();

                this.ReadODataErrorObject(error);
            }

            // Read the end of the error object
            this.JsonReader.ReadEndObject();

            // Read the end of the response.
            this.ReadPayloadEnd(false /*isReadingNestedPayload*/);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: JsonNodeType.EndOfInput");

            return error;
        }

        /// <summary>
        /// Reads all the properties in a single JSON object scope, calling <paramref name="readPropertyWithValue"/> for each non-annotation property encountered.
        /// </summary>
        /// <param name="readPropertyWithValue">
        /// An action which takes the name of the current property and processes the property value as necessary.
        /// At the start of this action, the reader is positioned at the property value node.
        /// The action should leave the reader positioned on the node after the property value.
        /// </param>
        /// <remarks>
        ///
        /// This method should only be used for scopes where we allow (and ignore) annotations in a custom namespace, i.e. scopes which directly correspond to a class in the OM.
        ///
        /// Pre-Condition:  JsonNodeType.StartObject    - The start of the JSON object being processed.
        ///                 any                         - Will throw if not StartObject.
        /// Post-Condition: any                         - The node after the EndObject node for the JSON object being processed.
        /// </remarks>
        private void ReadJsonObjectInErrorPayload(Action<string, DuplicatePropertyNamesChecker> readPropertyWithValue)
        {
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            this.JsonReader.ReadStartObject();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    this.ReadErrorPropertyAnnotationValue,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                throw new ODataException(Strings.ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload(propertyName));
                            case PropertyParsingResult.CustomInstanceAnnotation:
                                readPropertyWithValue(propertyName, duplicatePropertyNamesChecker);
                                break;
                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError(propertyName));
                            case PropertyParsingResult.PropertyWithValue:
                                readPropertyWithValue(propertyName, duplicatePropertyNamesChecker);
                                break;
                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                        }
                    });
            }

            this.JsonReader.ReadEndObject();
        }

        /// <summary>
        /// Reads a value of property annotation on an error payload.
        /// </summary>
        /// <param name="propertyAnnotationName">The name of the property annotation to read.</param>
        /// <returns>The value of the property annotation.</returns>
        /// <remarks>
        /// This method should read the property annotation value and return a representation of the value which will be later
        /// consumed by the resource reading code, or throw if ther is something unexpected.
        ///
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the property annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the error object
        ///                 JsonNodeType.Property               The next property after the property annotation
        /// </remarks>
        private object ReadErrorPropertyAnnotationValue(string propertyAnnotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyAnnotationName), "!string.IsNullOrEmpty(propertyAnnotationName)");
            Debug.Assert(
                propertyAnnotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
                "The method should only be called with OData. annotations");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            if (string.CompareOrdinal(propertyAnnotationName, ODataAnnotationNames.ODataType) == 0)
            {
                string typeName = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName(this.JsonReader.ReadStringValue()));
                if (typeName == null)
                {
                    throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName(propertyAnnotationName));
                }

                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
                return typeName;
            }

            throw new ODataException(Strings.ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload(propertyAnnotationName));
        }

        /// <summary>
        /// Reads the JSON object which is the value of the "error" property.
        /// </summary>
        /// <param name="error">The <see cref="ODataError"/> object to update with data from the payload.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject    - The start of the "error" object.
        ///                 any                         - Will throw if not StartObject.
        /// Post-Condition: any                         - The node after the "error" object's EndNode.
        /// </remarks>
        private void ReadODataErrorObject(ODataError error)
        {
            this.ReadJsonObjectInErrorPayload((propertyName, duplicationPropertyNameChecker) => this.ReadPropertyValueInODataErrorObject(error, propertyName, duplicationPropertyNameChecker));
        }

        /// <summary>
        /// Reads an inner error payload.
        /// </summary>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <returns>An <see cref="ODataInnerError"/> representing the read inner error.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject    - The start of the "innererror" object.
        ///                 any                         - will throw if not StartObject.
        /// Post-Condition: any                         - The node after the "innererror" object's EndNode.
        /// </remarks>
        private ODataInnerError ReadInnerError(int recursionDepth)
        {
            Debug.Assert(this.JsonReader.DisableInStreamErrorDetection, "JsonReader.DisableInStreamErrorDetection");
            this.JsonReader.AssertNotBuffering();

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, this.MessageReaderSettings.MessageQuotas.MaxNestingDepth);

            ODataInnerError innerError = new ODataInnerError();

            this.ReadJsonObjectInErrorPayload((propertyName, duplicatePropertyNamesChecker) => this.ReadPropertyValueInInnerError(recursionDepth, innerError, propertyName));

            this.JsonReader.AssertNotBuffering();
            return innerError;
        }

        /// <summary>
        /// Reads a property value which occurs in the "innererror" object scope.
        /// </summary>
        /// <param name="recursionDepth">The number of parent inner errors for this inner error.</param>
        /// <param name="innerError">The <see cref="ODataError"/> object to update with the data from this property value.</param>
        /// <param name="propertyName">The name of the property whose value is to be read.</param>
        /// <remarks>
        /// Pre-Condition:  any                         - The value of the property being read.
        /// Post-Condition: JsonNodeType.Property       - The property after the one being read.
        ///                 JsonNodeType.EndObject      - The end of the "innererror" object.
        ///                 any                         - Anything else after the property value is an invalid payload (but won't fail in this method).
        /// </remarks>
        private void ReadPropertyValueInInnerError(int recursionDepth, ODataInnerError innerError, string propertyName)
        {
            switch (propertyName)
            {
                case JsonConstants.ODataErrorInnerErrorMessageName:
                    innerError.Message = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorMessageName);
                    break;

                case JsonConstants.ODataErrorInnerErrorTypeNameName:
                    innerError.TypeName = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName);
                    break;

                case JsonConstants.ODataErrorInnerErrorStackTraceName:
                    innerError.StackTrace = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorStackTraceName);
                    break;

                case JsonConstants.ODataErrorInnerErrorInnerErrorName:
                    innerError.InnerError = this.ReadInnerError(recursionDepth);
                    break;

                default:
                    // skip any unsupported properties in the inner error
                    this.JsonReader.SkipValue();
                    break;
            }
        }

        /// <summary>
        /// Reads a property value which occurs in the "error" object scope.
        /// </summary>
        /// <param name="error">The <see cref="ODataError"/> object to update with the data from this property value.</param>
        /// <param name="propertyName">The name of the property whose value is to be read.</param>
        /// <param name="duplicationPropertyNameChecker">DuplicatePropertyNamesChecker to use for extracting property annotations
        /// targetting any custom instance annotations on the error.</param>
        /// <remarks>
        /// Pre-Condition:  any                         - The value of the property being read.
        /// Post-Condition: JsonNodeType.Property       - The property after the one being read.
        ///                 JsonNodeType.EndObject      - The end of the "error" object.
        ///                 any                         - Anything else after the property value is an invalid payload (but won't fail in this method).
        /// </remarks>
        private void ReadPropertyValueInODataErrorObject(ODataError error, string propertyName, DuplicatePropertyNamesChecker duplicationPropertyNameChecker)
        {
            switch (propertyName)
            {
                case JsonConstants.ODataErrorCodeName:
                    error.ErrorCode = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorCodeName);
                    break;

                case JsonConstants.ODataErrorMessageName:
                    error.Message = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorMessageName);
                    break;

                case JsonConstants.ODataErrorTargetName:
                    error.Target = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorTargetName);
                    break;

                case JsonConstants.ODataErrorDetailsName:
                    error.Details = this.ReadDetails();
                    break;

                case JsonConstants.ODataErrorInnerErrorName:
                    error.InnerError = this.ReadInnerError(0 /* recursionDepth */);
                    break;

                default:
                    // See if it's an instance annotation
                    if (ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName))
                    {
                        ODataJsonLightPropertyAndValueDeserializer valueDeserializer = new ODataJsonLightPropertyAndValueDeserializer(this.JsonLightInputContext);
                        object typeName = null;

                        var odataAnnotations = duplicationPropertyNameChecker.GetODataPropertyAnnotations(propertyName);
                        if (odataAnnotations != null)
                        {
                            odataAnnotations.TryGetValue(ODataAnnotationNames.ODataType, out typeName);
                        }

                        var value = valueDeserializer.ReadNonEntityValue(
                            typeName as string,
                            null /*expectedValueTypeReference*/,
                            null /*duplicatePropertiesNamesChecker*/,
                            null /*collectionValidator*/,
                            false /*validateNullValue*/,
                            false /*isTopLevelPropertyValue*/,
                            false /*insideComplexValue*/,
                            propertyName);

                        error.GetInstanceAnnotations().Add(new ODataInstanceAnnotation(propertyName, value.ToODataValue()));
                    }
                    else
                    {
                        // we only allow a 'code', 'message', 'target', 'details, and 'innererror' properties
                        // in the value of the 'error' property or custom instance annotations
                        throw new ODataException(Strings.ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty(propertyName));
                    }

                    break;
            }
        }

        private ICollection<ODataErrorDetail> ReadDetails()
        {
            var details = new List<ODataErrorDetail>();

            // [
            this.JsonReader.ReadStartArray();

            while (JsonReader.NodeType == JsonNodeType.StartObject)
            {
                var detail = ReadDetail();
                details.Add(detail);
            }

            // ]
            this.JsonReader.ReadEndArray();

            return details;
        }

        private ODataErrorDetail ReadDetail()
        {
            var detail = new ODataErrorDetail();

            ReadJsonObjectInErrorPayload(
                (propertyName, duplicationPropertyNameChecker) =>
                ReadPropertyValueInODataErrorDetailObject(detail, propertyName));

            return detail;
        }

        private void ReadPropertyValueInODataErrorDetailObject(ODataErrorDetail detail, string propertyName)
        {
            switch (propertyName)
            {
                case JsonConstants.ODataErrorCodeName:
                    detail.ErrorCode = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorCodeName);
                    break;

                case JsonConstants.ODataErrorMessageName:
                    detail.Message = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorMessageName);
                    break;

                case JsonConstants.ODataErrorTargetName:
                    detail.Target = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorTargetName);
                    break;

                default:
                    this.JsonReader.SkipValue();
                    break;
            }
        }
    }
}