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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Evaluation;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData JsonLight deserializers.
    /// </summary>
    internal abstract class ODataJsonLightDeserializer : ODataDeserializer
    {
        /// <summary>The JsonLight input context to use for reading.</summary>
        private readonly ODataJsonLightInputContext jsonLightInputContext;

        /// <summary>Context for entry etadata centric responsibilities.</summary>
        private IODataMetadataContext metadataContext;

        /// <summary>Result of parsing the metadata URI for the payload (or null if none are available).</summary>
        /// <remarks>This field is only available after the ReadPayloadStart was called.</remarks>
        private ODataJsonLightMetadataUriParseResult metadataUriParseResult;

#if DEBUG
        /// <summary>
        /// Set to true when ReadPayloadStart has been called and we would have read odata.metadata if it's on the payload.
        /// This is a debug check to make sure we don't access MetadataUriParseResult until after ReadPayloadStart is called.
        /// </summary>
        private bool metadataUriParseResultReady;
#endif

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        protected ODataJsonLightDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
            Debug.Assert(jsonLightInputContext != null, "jsonLightInputContext != null");

            this.jsonLightInputContext = jsonLightInputContext;
        }

        /// <summary>Possible results of parsing JSON object property.</summary>
        internal enum PropertyParsingResult
        {
            /// <summary>An end of object was reached without any property to be reported.</summary>
            EndOfObject,

            /// <summary>A property with value was found.</summary>
            PropertyWithValue,

            /// <summary>A property without value was found.</summary>
            PropertyWithoutValue,

            /// <summary>A 'odata' instance annotation was found.</summary>
            ODataInstanceAnnotation,

            /// <summary>A custom instance annotation was found.</summary>
            CustomInstanceAnnotation,

            /// <summary>A metadata reference property was found.</summary>
            MetadataReferenceProperty
        }

        /// <summary>
        /// Context for entry metadata centric responsibilities.
        /// </summary>
        internal IODataMetadataContext MetadataContext
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                // Under the client knob, the model we're given is really a facade, which doesn't flow open-type information into the combined types.
                // However, we need to answer this question for materializing operations which were left out of the payload.
                // To get around this, the client sets a callback in the ReaderBehavior to answer the question.
                Func<IEdmEntityType, bool> operationsBoundToEntityTypeMustBeContainerQualified = this.MessageReaderSettings.ReaderBehavior.OperationsBoundToEntityTypeMustBeContainerQualified;
                return this.metadataContext ?? (this.metadataContext = new ODataMetadataContext(this.ReadingResponse, operationsBoundToEntityTypeMustBeContainerQualified, this.JsonLightInputContext.EdmTypeResolver, this.Model, this.MetadataDocumentUri));
            }
        }

        /// <summary>
        /// Returns the <see cref="BufferingJsonReader"/> which is to be used to read the content of the message.
        /// </summary>
        internal BufferingJsonReader JsonReader
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.jsonLightInputContext.JsonReader;
            }
        }

        /// <summary>Result of parsing the metadata URI for the payload (or null if none are available).</summary>
        /// <remarks>This property is only available after the ReadPayloadStart was called.</remarks>
        internal ODataJsonLightMetadataUriParseResult MetadataUriParseResult
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
#if DEBUG
                Debug.Assert(this.metadataUriParseResultReady, "The MetadataUriParseResult property should not be accessed before ReadPayloadStart() is called.");
#endif
                return this.metadataUriParseResult;
            }
        }

        /// <summary>
        /// The Json lite input context to use for reading.
        /// </summary>
        protected ODataJsonLightInputContext JsonLightInputContext
        {
            get { return this.jsonLightInputContext; }
        }

        /// <summary>
        /// Gets the metadata document Uri from the MetadataUriParseResult.
        /// </summary>
        private Uri MetadataDocumentUri
        {
            get
            {
                Uri metadataDocumentUri = this.MetadataUriParseResult != null && this.MetadataUriParseResult.MetadataDocumentUri != null ? this.MetadataUriParseResult.MetadataDocumentUri : null;
                Debug.Assert(metadataDocumentUri == null || metadataDocumentUri.IsAbsoluteUri, "metadataDocumentUri == null || metadataDocumentUri.IsAbsoluteUri");
                return metadataDocumentUri;
            }
        }

        /// <summary>
        /// Parses the name of a property and returns the property name and annotation name if the property is a property annotation.
        /// </summary>
        /// <param name="propertyAnnotationName">The property name to parse.</param>
        /// <param name="propertyName">The name of the annotated property, or null if the property is not a property annotation.</param>
        /// <param name="annotationName">The annotation name, or null if the property is not a property annotation.</param>
        /// <returns>true if the <paramref name="propertyAnnotationName"/> is a property annotation, false otherwise.</returns>
        internal static bool TryParsePropertyAnnotation(string propertyAnnotationName, out string propertyName, out string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyAnnotationName), "!string.IsNullOrEmpty(propertyAnnotationName)");

            int propertyAnnotationSeparatorIndex = propertyAnnotationName.IndexOf(JsonLightConstants.ODataPropertyAnnotationSeparatorChar);
            if (propertyAnnotationSeparatorIndex <= 0 || propertyAnnotationSeparatorIndex == propertyAnnotationName.Length - 1)
            {
                propertyName = null;
                annotationName = null;
                return false;
            }

            propertyName = propertyAnnotationName.Substring(0, propertyAnnotationSeparatorIndex);
            annotationName = propertyAnnotationName.Substring(propertyAnnotationSeparatorIndex + 1);
            return true;
        }

        /// <summary>
        /// Read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="payloadKind">The kind of payload we are reading; this guides the parsing of the metadata URI.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker.</param>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. an entry, a feed or a collection within a parameters payload.</param>
        /// <param name="allowEmptyPayload">true if we allow a comletely empty payload; otherwise false.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first property of the payload after having read (or skipped) the metadata URI property.
        ///                 Or the reader is positioned on an end-object node if there are no properties (other than the metadata URI which is required in responses and optional in requests).
        /// </remarks>
        internal void ReadPayloadStart(
            ODataPayloadKind payloadKind,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            bool isReadingNestedPayload,
            bool allowEmptyPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            string metadataUriAnnotationValue = this.ReadPayloadStartImplementation(
                payloadKind,
                duplicatePropertyNamesChecker,
                isReadingNestedPayload,
                allowEmptyPayload);

            ODataJsonLightMetadataUriParseResult parseResult = null;

            // The metadata URI is only recognized in non-error response top-level payloads.
            // If the payload is nested (for example when we read URL literals) we don't recognize the metadata URI.
            // Top-level error payloads don't need and use the metadata URI.
            if (!isReadingNestedPayload && payloadKind != ODataPayloadKind.Error)
            {
                parseResult = this.jsonLightInputContext.PayloadKindDetectionState == null
                    ? null
                    : this.jsonLightInputContext.PayloadKindDetectionState.MetadataUriParseResult;
                if (parseResult == null && metadataUriAnnotationValue != null)
                {
                    parseResult = ODataJsonLightMetadataUriParser.Parse(
                        this.Model,
                        metadataUriAnnotationValue,
                        payloadKind,
                        this.Version,
                        this.MessageReaderSettings.ReaderBehavior);
                }
            }

            this.metadataUriParseResult = parseResult;
#if DEBUG
            this.metadataUriParseResultReady = true;
#endif
        }

#if ODATALIB_ASYNC

        /// <summary>
        /// Read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="payloadKind">The kind of payload we are reading; this guides the parsing of the metadata URI.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker.</param>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. an entry, a feed or a collection within a parameters payload.</param>
        /// <param name="allowEmptyPayload">true if we allow a comletely empty payload; otherwise false.</param>
        /// <returns>The parsed metadata URI.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first property of the payload after having read (or skipped) the metadata URI property.
        ///                 Or the reader is positioned on an end-object node if there are no properties (other than the metadata URI which is required in responses and optional in requests).
        /// </remarks>
        internal Task ReadPayloadStartAsync(
            ODataPayloadKind payloadKind,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            bool isReadingNestedPayload,
            bool allowEmptyPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            return TaskUtils.GetTaskForSynchronousOperation(() =>
                {
                    string metadataUriAnnotationValue = this.ReadPayloadStartImplementation(
                        payloadKind,
                        duplicatePropertyNamesChecker,
                        isReadingNestedPayload,
                        allowEmptyPayload);

                    // The metadata URI is only recognized in non-error response top-level payloads.
                    // If the payload is nested (for example when we read URL literals) we don't recognize the metadata URI.
                    // Top-level error payloads don't need and use the metadata URI.
                    if (!isReadingNestedPayload && payloadKind != ODataPayloadKind.Error)
                    {
                        this.metadataUriParseResult = this.jsonLightInputContext.PayloadKindDetectionState == null
                            ? null
                            : this.jsonLightInputContext.PayloadKindDetectionState.MetadataUriParseResult;
                        if (this.metadataUriParseResult == null && metadataUriAnnotationValue != null)
                        {
                            this.metadataUriParseResult = ODataJsonLightMetadataUriParser.Parse(
                                this.Model,
                                metadataUriAnnotationValue,
                                payloadKind,
                                this.Version,
                                this.MessageReaderSettings.ReaderBehavior);
                        }
                    }

#if DEBUG
                    this.metadataUriParseResultReady = true;
#endif
                });
        }
#endif

        /// <summary>
        /// Reads the end of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. an entry, a feed or a collection within a parameters payload.</param>
        /// <remarks>
        /// Pre-Condition:  any node:                when reading response or a nested payload, will fail if find anything else then EndObject.
        ///                 JsonNodeType.EndOfInput: otherwise
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isReadingNestedPayload", Justification = "The parameter is used in debug builds.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal void ReadPayloadEnd(bool isReadingNestedPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.EndOfInput,
                "Pre-Condition: JsonNodeType.EndOfInput if not reading a nested payload.");
            this.JsonReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads and validates a string value from the json reader.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>The string that was read.</returns>
        internal string ReadAndValidateAnnotationStringValue(string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            string valueRead = this.JsonReader.ReadStringValue(annotationName);
            ODataJsonLightReaderUtils.ValidateAnnotationStringValue(valueRead, annotationName);
            return valueRead;
        }

        /// <summary>
        /// Reads and validates a string value from the json reader and processes it as a Uri.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>The Uri that was read.</returns>
        internal Uri ReadAndValidateAnnotationStringValueAsUri(string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            string stringValue = this.ReadAndValidateAnnotationStringValue(annotationName);
            return this.ProcessUriFromPayload(stringValue);
        }

        /// <summary>
        /// Reads and validates a string value from the json reader and processes it as a long.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>The long that was read.</returns>
        internal long ReadAndValidateAnnotationStringValueAsLong(string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            string stringValue = this.ReadAndValidateAnnotationStringValue(annotationName);
            return (long)ODataJsonLightReaderUtils.ConvertValue(
                stringValue,
                EdmCoreModel.Instance.GetInt64(false),
                this.MessageReaderSettings,
                this.Version,
                /*validateNullValue*/ true,
                annotationName);
        }

        /// <summary>
        /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
        /// </summary>
        /// <param name="uriFromPayload">The URI string from the payload to process.</param>
        /// <returns>An absolute URI to report.</returns>
        internal Uri ProcessUriFromPayload(string uriFromPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uriFromPayload != null, "uriFromPayload != null");

            Uri uri = new Uri(uriFromPayload, UriKind.RelativeOrAbsolute);

            // Try to resolve the URI using a custom URL resolver first.
            Uri metadataDocumentUri = this.MetadataDocumentUri;
            Uri resolvedUri = this.JsonLightInputContext.ResolveUri(metadataDocumentUri, uri);
            if (resolvedUri != null)
            {
                return resolvedUri;
            }

            if (!uri.IsAbsoluteUri)
            {
                if (metadataDocumentUri == null)
                {
                    throw new ODataException(OData.Strings.ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation(uriFromPayload, ODataAnnotationNames.ODataMetadata));
                }

                uri = UriUtils.UriToAbsoluteUri(metadataDocumentUri, uri);
            }

            Debug.Assert(uri.IsAbsoluteUri, "By now we should have an absolute URI.");
            return uri;
        }

        /// <summary>
        /// Parses JSON object property starting with the current position of the JSON reader.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use, it will also store the property annotations found.</param>
        /// <param name="readPropertyAnnotationValue">Function called to read property annotation value.</param>
        /// <param name="handleProperty">Function callback to handle to resule of parse property.</param>
        internal void ProcessProperty(
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            Func<string, object> readPropertyAnnotationValue,
            Action<PropertyParsingResult, string> handleProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            Debug.Assert(readPropertyAnnotationValue != null, "readPropertyAnnotationValue != null");
            Debug.Assert(handleProperty != null, "handleProperty != null");
            this.AssertJsonCondition(JsonNodeType.Property);

            string propertyName;
            PropertyParsingResult propertyParsingResult = this.ParseProperty(duplicatePropertyNamesChecker, readPropertyAnnotationValue, out propertyName);

            while (propertyParsingResult == PropertyParsingResult.CustomInstanceAnnotation && this.ShouldSkipCustomInstanceAnnotation(propertyName))
            {
                // Make sure there's no duplicated instance annotation name even though we are skipping over it.
                duplicatePropertyNamesChecker.MarkPropertyAsProcessed(propertyName);

                // Skip over the instance annotation value and don't report it to the OM.
                this.JsonReader.SkipValue();

                propertyParsingResult = this.ParseProperty(duplicatePropertyNamesChecker, readPropertyAnnotationValue, out propertyName);
            }

            handleProperty(propertyParsingResult, propertyName);
            if (propertyParsingResult != PropertyParsingResult.EndOfObject)
            {
                duplicatePropertyNamesChecker.MarkPropertyAsProcessed(propertyName);
            }
        }

        /// <summary>
        /// Asserts that the JSON reader is positioned on one of the specified node types.
        /// </summary>
        /// <param name="allowedNodeTypes">The node types which should appear at this point.</param>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs access to this in Debug only.")]
        internal void AssertJsonCondition(params JsonNodeType[] allowedNodeTypes)
        {
            DebugUtils.CheckNoExternalCallers();

#if DEBUG
            if (allowedNodeTypes.Contains(this.JsonReader.NodeType))
            {
                return;
            }

            string message = string.Format(
                CultureInfo.InvariantCulture,
                "JSON condition failed: the JsonReader is on node {0} (Value: {1}) but it was expected be on {2}.",
                this.JsonReader.NodeType.ToString(),
                this.JsonReader.Value,
                string.Join(",", allowedNodeTypes.Select(n => n.ToString()).ToArray()));
            Debug.Assert(false, message);
#endif
        }

        /// <summary>
        /// Returns true if <paramref name="annotationName"/> should be skipped by the reader; false otherwise.
        /// </summary>
        /// <param name="annotationName">The custom instance annotation name in question.</param>
        /// <returns>Returns true if <paramref name="annotationName"/> should be skipped by the reader; false otherwise.</returns>
        private bool ShouldSkipCustomInstanceAnnotation(string annotationName)
        {
            // By default we always reading custom instance annotation on error payloads.
            if (this is ODataJsonLightErrorDeserializer && this.MessageReaderSettings.ShouldIncludeAnnotation == null)
            {
                return false;
            }

            // If readerSettings.AnnotationFilter is not set, readerSettings.ShouldSkipCustomInstanceAnnotation() will always return true, which is
            // the default we want for non-error payloads.
            // If the readerSettings.AnnotationFilter is set, readerSettings.ShouldSkipCustomInstanceAnnotation() will evaluate the annotationName based
            // of the filter. This should override the default behavior for both error and non-error payloads.
            return this.MessageReaderSettings.ShouldSkipAnnotation(annotationName);
        }

        /// <summary>
        /// If <paramref name="annotationName"/> is under the odata namespace but is not known to ODataLib, move the JSON reader forward to skip the
        /// annotation name and value then return true; return false otherwise.
        /// </summary>
        /// <remarks>
        /// The unknown odata annotation is skipped so that when this version of the reader reads a feed produced by a future version of ODataLib
        /// that contains an odata annotation that is not recognized on this version, we would simply ignore the annotation rather than failing.
        /// Note that when we add new odata annotations that cannot be skipped, we would bump the protocol version.
        /// </remarks>
        /// <param name="annotationName">The annotation name in question.</param>
        /// <param name="skippedRawJson">Outputs the skipped raw json string.</param>
        /// <returns>Returns true if the annotation name and value is skipped; returns false otherwise.</returns>
        private bool SkippedOverUnknownODataAnnotation(string annotationName, out string skippedRawJson)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            this.AssertJsonCondition(JsonNodeType.Property);

            if (ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName))
            {
                // Read over the name and value.
                this.JsonReader.Read();
                StringBuilder builder = new StringBuilder();
                this.JsonReader.SkipValue(builder);
                skippedRawJson = builder.ToString();
                return true;
            }

            skippedRawJson = null;
            return false;
        }

        /// <summary>
        /// Parses JSON object property starting with the current position of the JSON reader.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use, it will also store the property annotations found.</param>
        /// <param name="readPropertyAnnotationValue">Function called to read property annotation value.</param>
        /// <param name="parsedPropertyName">The name of the property or instance annotation found.</param>
        /// <returns>
        /// PropertyWithValue - a property with value was found. The <paramref name="parsedPropertyName"/> contains the name of the property.
        ///                     The reader is positioned on the property value.
        /// PropertyWithoutValue - a property without a value was found. The <paramref name="parsedPropertyName"/> contains the name of the property.
        ///                        The reader is positioned on the node after property annotations (so either a property or end of object).
        /// ODataInstanceAnnotation - an odata instance annotation was found. The <paramref name="parsedPropertyName"/> contains the name of the annotation.
        ///                      The reader is positioned on the value of the annotation.
        /// CustomInstanceAnnotation - a custom instance annotation was found. The <paramref name="parsedPropertyName"/> contains the name of the annotation.
        ///                      The reader is positioned on the value of the annotation.
        /// MetadataReferenceProperty - a property which is a reference into the metadata was found.
        ///                             The reader is positioned on the value of the property.
        /// EndOfObject - end of the object scope was reached and no properties are to be reported. The <paramref name="parsedPropertyName"/> is null.
        ///               This can only happen if there's a property annotation which is ignored (for example custom one) at the end of the object.
        /// </returns>
        private PropertyParsingResult ParseProperty(
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            Func<string, object> readPropertyAnnotationValue,
            out string parsedPropertyName)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            Debug.Assert(readPropertyAnnotationValue != null, "readPropertyAnnotationValue != null");
            string lastPropertyAnnotationNameFound = null;
            parsedPropertyName = null;

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string nameFromReader = this.JsonReader.GetPropertyName();

                string propertyNameFromReader, annotationNameFromReader;
                bool isPropertyAnnotation = TryParsePropertyAnnotation(nameFromReader, out propertyNameFromReader, out annotationNameFromReader);
                propertyNameFromReader = propertyNameFromReader ?? nameFromReader;

                // If parsedPropertyName is set and is different from the property name the reader is currently on,
                // we have parsed a property annotation for a different property than the one at the current position.
                if (parsedPropertyName != null && string.CompareOrdinal(parsedPropertyName, propertyNameFromReader) != 0)
                {
                    if (ODataJsonLightReaderUtils.IsAnnotationProperty(parsedPropertyName))
                    {
                        throw new ODataException(OData.Strings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue(lastPropertyAnnotationNameFound, parsedPropertyName));
                    }

                    return PropertyParsingResult.PropertyWithoutValue;
                }

                duplicatePropertyNamesChecker.AnnotationCollector.ShouldCollectAnnotation =
                    (this.MessageReaderSettings.UndeclaredPropertyBehaviorKinds
                            == ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty);
                string skippedRawJson = null;
                if (isPropertyAnnotation)
                {
                    duplicatePropertyNamesChecker.AnnotationCollector.TryPeekAndCollectAnnotationRawJson(
                        this.JsonReader, propertyNameFromReader, annotationNameFromReader);

                    // If this is a unknown odata annotation targeting a property, we skip over it. See remark on the method SkippedOverUnknownODataAnnotation() for detailed explaination.
                    // Note that we don't skip over unknown odata annotations targeting another annotation. We don't allow annotations (except odata.type) targeting other annotations,
                    // so this.ProcessPropertyAnnotation() will test and fail for that case.
                    if (!ODataJsonLightReaderUtils.IsAnnotationProperty(propertyNameFromReader) && this.SkippedOverUnknownODataAnnotation(annotationNameFromReader, out skippedRawJson))
                    {
                        continue;
                    }

                    // We have another property annotation for the same property we parsed.
                    parsedPropertyName = propertyNameFromReader;
                    lastPropertyAnnotationNameFound = annotationNameFromReader;

                    this.ProcessPropertyAnnotation(propertyNameFromReader, annotationNameFromReader, duplicatePropertyNamesChecker, readPropertyAnnotationValue);
                    continue;
                }

                // If this is a unknown odata annotation, skip over it. See remark on the method SkippedOverUnknownODataAnnotation() for detailed explaination.
                if (this.SkippedOverUnknownODataAnnotation(propertyNameFromReader, out skippedRawJson))
                {
                    // collect 'odata.<unknown>' annotation:
                    // here we know the original property name contains no '@', but '.' dot
                    Debug.Assert(annotationNameFromReader == null, "annotationNameFromReader == null");
                    duplicatePropertyNamesChecker.AnnotationCollector.TryAddPropertyAnnotationRawJson(
                        "", propertyNameFromReader, skippedRawJson);
                    continue;
                }

                // We are encountering the property name for the first time.
                // Read over the property name.
                this.JsonReader.Read();
                parsedPropertyName = propertyNameFromReader;

                if (ODataJsonLightUtils.IsMetadataReferenceProperty(propertyNameFromReader))
                {
                    return PropertyParsingResult.MetadataReferenceProperty;
                }

                if (!ODataJsonLightReaderUtils.IsAnnotationProperty(propertyNameFromReader))
                {
                    // Normal property
                    return PropertyParsingResult.PropertyWithValue;
                }

                // collect 'xxx.yyyy' annotation:
                // here we know the original property name contains no '@', but '.' dot
                Debug.Assert(annotationNameFromReader == null, "annotationNameFromReader == null");
                duplicatePropertyNamesChecker.AnnotationCollector.TryPeekAndCollectAnnotationRawJson(
                    this.JsonReader, "", propertyNameFromReader); // propertyNameFromReader is the annotation name

                // Handle 'odata.XXXXX' annotations
                if (ODataJsonLightReaderUtils.IsODataAnnotationName(propertyNameFromReader))
                {
                    return PropertyParsingResult.ODataInstanceAnnotation;
                }

                // Handle custom annotations
                return PropertyParsingResult.CustomInstanceAnnotation;
            }

            this.AssertJsonCondition(JsonNodeType.EndObject);
            if (parsedPropertyName != null)
            {
                if (ODataJsonLightReaderUtils.IsAnnotationProperty(parsedPropertyName))
                {
                    throw new ODataException(OData.Strings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue(lastPropertyAnnotationNameFound, parsedPropertyName));
                }

                return PropertyParsingResult.PropertyWithoutValue;
            }

            return PropertyParsingResult.EndOfObject;
        }

        /// <summary>
        /// Process the current property annotation.
        /// </summary>
        /// <param name="annotatedPropertyName">The name being annotated. Can be a property or an instance annotation.</param>
        /// <param name="annotationName">The annotation targeting the <paramref name="annotatedPropertyName"/>.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker.</param>
        /// <param name="readPropertyAnnotationValue">Callback to read the property annotation value.</param>
        private void ProcessPropertyAnnotation(string annotatedPropertyName, string annotationName, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, Func<string, object> readPropertyAnnotationValue)
        {
            // We don't currently support annotation targeting an instance annotation except for the @odata.type property annotation.
            if (ODataJsonLightReaderUtils.IsAnnotationProperty(annotatedPropertyName) && string.CompareOrdinal(annotationName, ODataAnnotationNames.ODataType) != 0)
            {
                throw new ODataException(OData.Strings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation(annotationName, annotatedPropertyName, ODataAnnotationNames.ODataType));
            }

            // Read over the property name.
            this.JsonReader.Read();
            if (ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName))
            {
                // OData annotations are read.
                duplicatePropertyNamesChecker.AddODataPropertyAnnotation(annotatedPropertyName, annotationName, readPropertyAnnotationValue(annotationName));
            }
            else
            {
                // All other property annotations are ignored.
                duplicatePropertyNamesChecker.AddCustomPropertyAnnotation(annotatedPropertyName, annotationName);
                this.JsonReader.SkipValue();
            }
        }

        /// <summary>
        /// Read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="payloadKind">The kind of payload we are reading; this guides the parsing of the metadata URI.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker.</param>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. an entry, a feed or a collection within a parameters payload.</param>
        /// <param name="allowEmptyPayload">true if we allow a comletely empty payload; otherwise false.</param>
        /// <returns>The value of the metadata URI annotation (or null if it was not found).</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first property of the payload after having read (or skipped) the metadata URI property.
        ///                 Or the reader is positioned on an end-object node if there are no properties (other than the metadata URI which is required in responses and optional in requests).
        /// </remarks>
        private string ReadPayloadStartImplementation(
            ODataPayloadKind payloadKind,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            bool isReadingNestedPayload,
            bool allowEmptyPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            if (!isReadingNestedPayload)
            {
                // Position the reader on the first node inside the outermost object.
                this.JsonReader.Read();

                if (allowEmptyPayload && this.JsonReader.NodeType == JsonNodeType.EndOfInput)
                {
                    return null;
                }

                // Read the start object node and position the reader on the first property
                // (or the end object node).
                this.JsonReader.ReadStartObject();

                if (payloadKind != ODataPayloadKind.Error)
                {
                    // Skip over the metadata URI annotation in request payloads or when we've already read it
                    // during payload kind detection.
                    bool failOnMissingMetadataUriAnnotation =
                        this.jsonLightInputContext.ReadingResponse &&
                        (this.jsonLightInputContext.PayloadKindDetectionState == null ||
                         this.jsonLightInputContext.PayloadKindDetectionState.MetadataUriParseResult == null);

                    // In responses we expect the metadata URI to be the first thing in the payload
                    // (except for error payloads). In requests we ignore the metadata URI.
                    return this.ReadMetadataUriAnnotation(payloadKind, duplicatePropertyNamesChecker, failOnMissingMetadataUriAnnotation);
                }

                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            }

            return null;
        }

        /// <summary>
        /// Reads the odata.metadata annotation.
        /// </summary>
        /// <param name="payloadKind">The payload kind for which to read the metadata URI.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker.</param>
        /// <param name="failOnMissingMetadataUriAnnotation">true if the method should fail if the metadata URI annotation is missing, false if that can be ignored.</param>
        /// <returns>The value of the metadata URI annotation.</returns>
        private string ReadMetadataUriAnnotation(
            ODataPayloadKind payloadKind,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            bool failOnMissingMetadataUriAnnotation)
        {
            if (this.JsonReader.NodeType != JsonNodeType.Property)
            {
                if (!failOnMissingMetadataUriAnnotation || payloadKind == ODataPayloadKind.Unsupported)
                {
                    // Do not fail during payload kind detection
                    return null;
                }

                throw new ODataException(OData.Strings.ODataJsonLightDeserializer_MetadataLinkNotFoundAsFirstProperty);
            }

            string propertyName = this.JsonReader.GetPropertyName();
            if (string.CompareOrdinal(ODataAnnotationNames.ODataMetadata, propertyName) != 0)
            {
                if (!failOnMissingMetadataUriAnnotation || payloadKind == ODataPayloadKind.Unsupported)
                {
                    // Do not fail during payload kind detection
                    return null;
                }

                throw new ODataException(OData.Strings.ODataJsonLightDeserializer_MetadataLinkNotFoundAsFirstProperty);
            }

            if (duplicatePropertyNamesChecker != null)
            {
                duplicatePropertyNamesChecker.MarkPropertyAsProcessed(propertyName);
            }

            // Read over the property name
            this.JsonReader.ReadNext();
            return this.JsonReader.ReadStringValue();
        }
    }
}
