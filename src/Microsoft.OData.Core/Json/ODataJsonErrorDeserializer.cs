//---------------------------------------------------------------------
// <copyright file="ODataJsonErrorDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// OData Json deserializer for errors.
    /// </summary>
    internal sealed class ODataJsonErrorDeserializer : ODataJsonDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The Json input context to read from.</param>
        internal ODataJsonErrorDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
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
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            try
            {
                // Position the reader on the first node
                this.ReadPayloadStart(
                    ODataPayloadKind.Error,
                    propertyAndAnnotationCollector,
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


        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>A task which returns an <see cref="ODataError"/> representing the read error.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None       - The reader must not have been used yet.
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        internal async Task<ODataError> ReadTopLevelErrorAsync()
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            Debug.Assert(!this.JsonReader.DisableInStreamErrorDetection, "!JsonReader.DisableInStreamErrorDetection");
            this.JsonReader.AssertNotBuffering();

            // Prevent the buffering JSON reader from detecting in-stream errors - we read the error ourselves
            // to throw proper exceptions
            this.JsonReader.DisableInStreamErrorDetection = true;

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            // Position the reader on the first node
            await this.ReadPayloadStartAsync(
                ODataPayloadKind.Error,
                propertyAndAnnotationCollector,
                isReadingNestedPayload: false,
                allowEmptyPayload: false).ConfigureAwait(false);

            ODataError error = await this.ReadTopLevelErrorImplementationAsync()
                .ConfigureAwait(false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            this.JsonReader.DisableInStreamErrorDetection = false;

            return error;
        }

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
                if (!string.Equals(ODataJsonConstants.ODataErrorPropertyName, propertyName, StringComparison.Ordinal))
                {
                    // we only allow a single 'error' property for a top-level error object
                    throw new ODataException(Strings.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty(propertyName));
                }

                if (error != null)
                {
                    throw new ODataException(Strings.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName(ODataJsonConstants.ODataErrorPropertyName));
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
        private void ReadJsonObjectInErrorPayload(Action<string, PropertyAndAnnotationCollector> readPropertyWithValue)
        {
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            this.JsonReader.ReadStartObject();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ProcessProperty(
                    propertyAndAnnotationCollector,
                    this.ReadErrorPropertyAnnotationValue,
                    (propertyParsingResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            this.JsonReader.Read();
                        }

                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                throw new ODataException(Strings.ODataJsonErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload(propertyName));
                            case PropertyParsingResult.CustomInstanceAnnotation:
                                readPropertyWithValue(propertyName, propertyAndAnnotationCollector);
                                break;
                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonErrorDeserializer_PropertyAnnotationWithoutPropertyForError(propertyName));
                            case PropertyParsingResult.PropertyWithValue:
                                readPropertyWithValue(propertyName, propertyAndAnnotationCollector);
                                break;
                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
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
        /// consumed by the resource reading code, or throw if there is something unexpected.
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
                propertyAnnotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
                "The method should only be called with OData. annotations");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            if (string.Equals(propertyAnnotationName, ODataAnnotationNames.ODataType, StringComparison.Ordinal))
            {
                string typeName = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName(this.JsonReader.ReadStringValue()));
                if (typeName == null)
                {
                    throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_InvalidTypeName(propertyAnnotationName));
                }

                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
                return typeName;
            }

            throw new ODataException(Strings.ODataJsonErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload(propertyAnnotationName));
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

            this.ReadJsonObjectInErrorPayload((propertyName, propertyAndAnnotationCollector) => this.ReadPropertyValueInInnerError(recursionDepth, innerError, propertyName));

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
                    string innerErrorMessage = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorMessageName);

                    innerError.Properties.Add(
                        JsonConstants.ODataErrorInnerErrorMessageName,
                        new ODataPrimitiveValue(innerErrorMessage));
                    break;

                case JsonConstants.ODataErrorInnerErrorTypeNameName:
                    string innerErrorTypeName = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName);

                    innerError.Properties.Add(
                        JsonConstants.ODataErrorInnerErrorTypeNameName,
                        new ODataPrimitiveValue(innerErrorTypeName));
                    break;

                case JsonConstants.ODataErrorInnerErrorStackTraceName:
                    string innerErrorStackTrace = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorStackTraceName);

                    innerError.Properties.Add(
                        JsonConstants.ODataErrorInnerErrorStackTraceName,
                        new ODataPrimitiveValue(innerErrorStackTrace));
                    break;

                case JsonConstants.ODataErrorInnerErrorInnerErrorName:
                case JsonConstants.ODataErrorInnerErrorName:
                    innerError.InnerError = this.ReadInnerError(recursionDepth);
                    break;

                default:
                    if (!innerError.Properties.ContainsKey(propertyName))
                    {
                        innerError.Properties.Add(propertyName, this.JsonReader.ReadODataValue());
                    }
                    else
                    {
                        innerError.Properties[propertyName] = this.JsonReader.ReadODataValue();
                    }

                    break;
            }
        }

        /// <summary>
        /// Reads a property value which occurs in the "error" object scope.
        /// </summary>
        /// <param name="error">The <see cref="ODataError"/> object to update with the data from this property value.</param>
        /// <param name="propertyName">The name of the property whose value is to be read.</param>
        /// <param name="duplicationPropertyNameChecker">PropertyAndAnnotationCollector to use for extracting property annotations
        /// targeting any custom instance annotations on the error.</param>
        /// <remarks>
        /// Pre-Condition:  any                         - The value of the property being read.
        /// Post-Condition: JsonNodeType.Property       - The property after the one being read.
        ///                 JsonNodeType.EndObject      - The end of the "error" object.
        ///                 any                         - Anything else after the property value is an invalid payload (but won't fail in this method).
        /// </remarks>
        private void ReadPropertyValueInODataErrorObject(ODataError error, string propertyName, PropertyAndAnnotationCollector duplicationPropertyNameChecker)
        {
            switch (propertyName)
            {
                case JsonConstants.ODataErrorCodeName:
                    error.Code = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorCodeName);
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
                    if (ODataJsonReaderUtils.IsAnnotationProperty(propertyName))
                    {
                        ODataJsonPropertyAndValueDeserializer valueDeserializer = new ODataJsonPropertyAndValueDeserializer(this.JsonInputContext);
                        object typeName = null;

                        duplicationPropertyNameChecker.GetODataPropertyAnnotations(propertyName)
                        .TryGetValue(ODataAnnotationNames.ODataType, out typeName);

                        var value = valueDeserializer.ReadNonEntityValue(
                            typeName as string,
                            null /*expectedValueTypeReference*/,
                            null /*duplicatePropertiesNamesChecker*/,
                            null /*collectionValidator*/,
                            false /*validateNullValue*/,
                            false /*isTopLevelPropertyValue*/,
                            false /*insideResourceValue*/,
                            propertyName);

                        error.GetInstanceAnnotations().Add(new ODataInstanceAnnotation(propertyName, value.ToODataValue()));
                    }
                    else
                    {
                        // we only allow a 'code', 'message', 'target', 'details, and 'innererror' properties
                        // in the value of the 'error' property or custom instance annotations
                        throw new ODataException(Strings.ODataJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty(propertyName));
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
                    detail.Code = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorCodeName);
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

        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an <see cref="ODataError"/> representing the read error.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property       - The first property of the top level object.
        ///                 JsonNodeType.EndObject      - If there are no properties in the top level object.
        ///                 any                         - Will throw if anything else.
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        private async Task<ODataError> ReadTopLevelErrorImplementationAsync()
        {
            ODataError error = null;

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = await this.JsonReader.ReadPropertyNameAsync()
                    .ConfigureAwait(false);

                if (!string.Equals(ODataJsonConstants.ODataErrorPropertyName, propertyName, StringComparison.Ordinal))
                {
                    // We only allow a single 'error' property for a top-level error object
                    throw new ODataException(Strings.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty(propertyName));
                }

                if (error != null)
                {
                    throw new ODataException(Strings.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName(ODataJsonConstants.ODataErrorPropertyName));
                }

                error = new ODataError();

                await this.ReadODataErrorObjectAsync(error)
                    .ConfigureAwait(false);
            }

            // Read the end of the error object
            await this.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);

            // Read the end of the response.
            await this.ReadPayloadEndAsync(isReadingNestedPayload: false)
                .ConfigureAwait(false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: JsonNodeType.EndOfInput");

            return error;
        }

        /// <summary>
        /// Asynchronously reads all the properties in a single JSON object scope,
        /// calling <paramref name="readPropertyWithValueDelegate"/> for each non-annotation property encountered.
        /// </summary>
        /// <param name="readPropertyWithValueDelegate">
        /// A delegate which takes the name of the current property and processes the property value as necessary.
        /// At the start of this delegate, the reader is positioned at the property value node.
        /// The action should leave the reader positioned on the node after the property value.
        /// </param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// This method should only be used for scopes where we allow (and ignore) annotations in a custom namespace,
        /// i.e. scopes which directly correspond to a class in the OM.
        ///
        /// Pre-Condition:  JsonNodeType.StartObject    - The start of the JSON object being processed.
        ///                 any                         - Will throw if not StartObject.
        /// Post-Condition: any                         - The node after the EndObject node for the JSON object being processed.
        /// </remarks>
        private async Task ReadJsonObjectInErrorPayloadAsync(Func<string, PropertyAndAnnotationCollector, Task> readPropertyWithValueDelegate)
        {
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            await this.JsonReader.ReadStartObjectAsync()
                .ConfigureAwait(false);

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                await this.ProcessPropertyAsync(
                    propertyAndAnnotationCollector,
                    this.ReadErrorPropertyAnnotationValueAsync,
                    async (propertyParsingResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            await this.JsonReader.ReadAsync()
                                .ConfigureAwait(false);
                        }

                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                throw new ODataException(Strings.ODataJsonErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload(propertyName));
                            case PropertyParsingResult.CustomInstanceAnnotation:
                                await readPropertyWithValueDelegate(propertyName, propertyAndAnnotationCollector)
                                    .ConfigureAwait(false);
                                break;
                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Strings.ODataJsonErrorDeserializer_PropertyAnnotationWithoutPropertyForError(propertyName));
                            case PropertyParsingResult.PropertyWithValue:
                                await readPropertyWithValueDelegate(propertyName, propertyAndAnnotationCollector)
                                    .ConfigureAwait(false);
                                break;
                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                        }
                    }).ConfigureAwait(false);
            }

            await this.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously reads a value of property annotation on an error payload.
        /// </summary>
        /// <param name="propertyAnnotationName">The name of the property annotation to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the property annotation.
        /// </returns>
        /// <remarks>
        /// This method should read the property annotation value and return a representation of the value which will be later
        /// consumed by the resource reading code, or throw if there is something unexpected.
        ///
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the property annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the error object
        ///                 JsonNodeType.Property               The next property after the property annotation
        /// </remarks>
        private async Task<object> ReadErrorPropertyAnnotationValueAsync(string propertyAnnotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyAnnotationName), "!string.IsNullOrEmpty(propertyAnnotationName)");
            Debug.Assert(
                propertyAnnotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
                "The method should only be called with OData. annotations");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            if (string.Equals(propertyAnnotationName, ODataAnnotationNames.ODataType, StringComparison.Ordinal))
            {
                string typeName = ReaderUtils.AddEdmPrefixOfTypeName(
                    ReaderUtils.RemovePrefixOfTypeName(
                        await this.JsonReader.ReadStringValueAsync().ConfigureAwait(false)));

                if (typeName == null)
                {
                    throw new ODataException(Strings.ODataJsonPropertyAndValueDeserializer_InvalidTypeName(propertyAnnotationName));
                }

                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);


                return typeName;
            }

            throw new ODataException(Strings.ODataJsonErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload(propertyAnnotationName));
        }

        /// <summary>
        /// Asynchronously reads the JSON object which is the value of the "error" property.
        /// </summary>
        /// <param name="error">The <see cref="ODataError"/> object to update with data from the payload.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject    - The start of the "error" object.
        ///                 any                         - Will throw if not StartObject.
        /// Post-Condition: any                         - The node after the "error" object's EndNode.
        /// </remarks>
        private Task ReadODataErrorObjectAsync(ODataError error)
        {
            return this.ReadJsonObjectInErrorPayloadAsync(
                (propertyName, duplicationPropertyNameChecker) => this.ReadPropertyValueInODataErrorObjectAsync(error, propertyName, duplicationPropertyNameChecker));
        }

        /// <summary>
        /// Asynchronously reads an inner error object.
        /// </summary>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an <see cref="ODataInnerError"/> representing the read inner error.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject    - The start of the "innererror" object.
        ///                 any                         - will throw if not StartObject.
        /// Post-Condition: any                         - The node after the "innererror" object's EndNode.
        /// </remarks>
        private async Task<ODataInnerError> ReadInnerErrorAsync(int recursionDepth)
        {
            Debug.Assert(this.JsonReader.DisableInStreamErrorDetection, "JsonReader.DisableInStreamErrorDetection");
            this.JsonReader.AssertNotBuffering();

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, this.MessageReaderSettings.MessageQuotas.MaxNestingDepth);

            ODataInnerError innerError = new ODataInnerError();

            await this.ReadJsonObjectInErrorPayloadAsync(
                (propertyName, propertyAndAnnotationCollector) => this.ReadPropertyValueInInnerErrorAsync(recursionDepth, innerError, propertyName)).ConfigureAwait(false);

            this.JsonReader.AssertNotBuffering();

            return innerError;
        }

        /// <summary>
        /// Asynchronously reads a property value which occurs in the "innererror" object scope.
        /// </summary>
        /// <param name="recursionDepth">The number of parent inner errors for this inner error.</param>
        /// <param name="innerError">The <see cref="ODataError"/> object to update with the data from this property value.</param>
        /// <param name="propertyName">The name of the property whose value is to be read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  any                         - The value of the property being read.
        /// Post-Condition: JsonNodeType.Property       - The property after the one being read.
        ///                 JsonNodeType.EndObject      - The end of the "innererror" object.
        ///                 any                         - Anything else after the property value is an invalid payload (but won't fail in this method).
        /// </remarks>
        private async Task ReadPropertyValueInInnerErrorAsync(int recursionDepth, ODataInnerError innerError, string propertyName)
        {
            switch (propertyName)
            {
                case JsonConstants.ODataErrorInnerErrorMessageName:
                    string innerErrorMessage = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorInnerErrorMessageName)
                        .ConfigureAwait(false);

                    innerError.Properties.Add(
                        JsonConstants.ODataErrorInnerErrorMessageName,
                        new ODataPrimitiveValue(innerErrorMessage));
                    break;

                case JsonConstants.ODataErrorInnerErrorTypeNameName:
                    string innerErrorTypeName = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorInnerErrorTypeNameName)
                        .ConfigureAwait(false);

                    innerError.Properties.Add(
                        JsonConstants.ODataErrorInnerErrorTypeNameName,
                        new ODataPrimitiveValue(innerErrorTypeName));
                    break;

                case JsonConstants.ODataErrorInnerErrorStackTraceName:
                    string innerErrorStackTrace = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorInnerErrorStackTraceName)
                        .ConfigureAwait(false);

                    innerError.Properties.Add(
                        JsonConstants.ODataErrorInnerErrorStackTraceName,
                        new ODataPrimitiveValue(innerErrorStackTrace));
                    break;

                case JsonConstants.ODataErrorInnerErrorInnerErrorName:
                case JsonConstants.ODataErrorInnerErrorName:
                    innerError.InnerError = await this.ReadInnerErrorAsync(recursionDepth)
                        .ConfigureAwait(false);
                    break;

                default:
                    if (!innerError.Properties.ContainsKey(propertyName))
                    {
                        innerError.Properties.Add(propertyName, await this.JsonReader.ReadODataValueAsync().ConfigureAwait(false));
                    }
                    else
                    {
                        innerError.Properties[propertyName] = await this.JsonReader.ReadODataValueAsync()
                            .ConfigureAwait(false);
                    }

                    break;
            }
        }

        /// <summary>
        /// Asynchronously reads a property value which occurs in the "error" object scope.
        /// </summary>
        /// <param name="error">The <see cref="ODataError"/> object to update with the data from this property value.</param>
        /// <param name="propertyName">The name of the property whose value is to be read.</param>
        /// <param name="duplicationPropertyNameChecker"><see cref="PropertyAndAnnotationCollector"/> to use for extracting property annotations
        /// targeting any custom instance annotations on the error.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  any                         - The value of the property being read.
        /// Post-Condition: JsonNodeType.Property       - The property after the one being read.
        ///                 JsonNodeType.EndObject      - The end of the "error" object.
        ///                 any                         - Anything else after the property value is an invalid payload (but won't fail in this method).
        /// </remarks>
        private async Task ReadPropertyValueInODataErrorObjectAsync(ODataError error, string propertyName, PropertyAndAnnotationCollector duplicationPropertyNameChecker)
        {
            switch (propertyName)
            {
                case JsonConstants.ODataErrorCodeName:
                    error.Code = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorCodeName)
                        .ConfigureAwait(false);
                    break;

                case JsonConstants.ODataErrorMessageName:
                    error.Message = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorMessageName)
                        .ConfigureAwait(false);
                    break;

                case JsonConstants.ODataErrorTargetName:
                    error.Target = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorTargetName)
                        .ConfigureAwait(false);
                    break;

                case JsonConstants.ODataErrorDetailsName:
                    error.Details = await this.ReadErrorDetailsAsync()
                        .ConfigureAwait(false);
                    break;

                case JsonConstants.ODataErrorInnerErrorName:
                    error.InnerError = await this.ReadInnerErrorAsync(recursionDepth: 0)
                        .ConfigureAwait(false);
                    break;

                default:
                    // See if it's an instance annotation
                    if (ODataJsonReaderUtils.IsAnnotationProperty(propertyName))
                    {
                        ODataJsonPropertyAndValueDeserializer propertyAndValueDeserializer = new ODataJsonPropertyAndValueDeserializer(this.JsonInputContext);
                        object typeName;

                        duplicationPropertyNameChecker.GetODataPropertyAnnotations(propertyName).TryGetValue(ODataAnnotationNames.ODataType, out typeName);

                        var value = await propertyAndValueDeserializer.ReadNonEntityValueAsync(
                            payloadTypeName: typeName as string,
                            expectedValueTypeReference: null,
                            propertyAndAnnotationCollector: null,
                            collectionValidator: null,
                            validateNullValue: false,
                            isTopLevelPropertyValue: false,
                            insideResourceValue: false,
                            propertyName: propertyName).ConfigureAwait(false);

                        error.GetInstanceAnnotations().Add(new ODataInstanceAnnotation(propertyName, value.ToODataValue()));
                    }
                    else
                    {
                        // We only allow a 'code', 'message', 'target', 'details, and 'innererror' properties
                        // in the value of the 'error' property or custom instance annotations
                        throw new ODataException(Strings.ODataJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty(propertyName));
                    }

                    break;
            }
        }

        /// <summary>
        /// Asynchronously reads an error details array.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task<ICollection<ODataErrorDetail>> ReadErrorDetailsAsync()
        {
            var details = new List<ODataErrorDetail>();

            // [
            await this.JsonReader.ReadStartArrayAsync()
                .ConfigureAwait(false);

            while (JsonReader.NodeType == JsonNodeType.StartObject)
            {
                var detail = new ODataErrorDetail();

                await ReadJsonObjectInErrorPayloadAsync(
                    (propertyName, duplicationPropertyNameChecker) => ReadPropertyValueInODataErrorDetailObjectAsync(detail, propertyName)).ConfigureAwait(false);

                details.Add(detail);
            }

            // ]
            await this.JsonReader.ReadEndArrayAsync()
                .ConfigureAwait(false);

            return details;
        }

        /// <summary>
        /// Asynchronously reads a property value which occurs in the error detail object scope.
        /// </summary>
        /// <param name="errorDetail">The <see cref="ODataErrorDetail"/> object to update with the data from this property value.</param>
        /// <param name="propertyName">The name of the property whose value is to be read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task ReadPropertyValueInODataErrorDetailObjectAsync(ODataErrorDetail errorDetail, string propertyName)
        {
            switch (propertyName)
            {
                case JsonConstants.ODataErrorCodeName:
                    errorDetail.Code = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorCodeName)
                        .ConfigureAwait(false);
                    break;

                case JsonConstants.ODataErrorMessageName:
                    errorDetail.Message = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorMessageName)
                        .ConfigureAwait(false);
                    break;

                case JsonConstants.ODataErrorTargetName:
                    errorDetail.Target = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataErrorTargetName)
                        .ConfigureAwait(false);
                    break;

                default:
                    await this.JsonReader.SkipValueAsync()
                        .ConfigureAwait(false);
                    break;
            }
        }
    }
}
