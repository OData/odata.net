//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.OData.Evaluation;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData JsonLight deserializers.
    /// </summary>
    internal abstract class ODataJsonLightDeserializer : ODataDeserializer
    {
        /// <summary>The JsonLight input context to use for reading.</summary>
        private readonly ODataJsonLightInputContext jsonLightInputContext;

        /// <summary>Context for resource metadata centric responsibilities.</summary>
        private IODataMetadataContext metadataContext;

        /// <summary>Result of parsing the context URI for the payload (or null if none are available).</summary>
        /// <remarks>This field is only available after the ReadPayloadStart was called.</remarks>
        private ODataJsonLightContextUriParseResult contextUriParseResult;

        /// <summary>The OData Uri.</summary>
        private ODataUri odataUri;

        /// <summary>True if the odata uri is calculated, false otherwise.</summary>
        private bool isODataUriRead;

#if DEBUG
        /// <summary>
        /// Set to true when ReadPayloadStart has been called and we would have read odata.context if it's on the payload.
        /// This is a debug check to make sure we don't access contextUriParseResult until after ReadPayloadStart is called.
        /// </summary>
        private bool contextUriParseResultReady;
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
            MetadataReferenceProperty,

            /// <summary>A property representing a nested delta resoruce set was found.</summary>
            NestedDeltaResourceSet,
        }

        /// <summary>
        /// Gets the OData uri.
        /// </summary>
        internal ODataUri ODataUri
        {
            get
            {
                if (this.isODataUriRead)
                {
                    return this.odataUri;
                }

                if (this.ContextUriParseResult == null ||
                    this.ContextUriParseResult.NavigationSource == null ||
                    this.ContextUriParseResult.NavigationSource is IEdmContainedEntitySet == false ||
                    this.contextUriParseResult.Path == null)
                {
                    this.isODataUriRead = true;
                    return this.odataUri = null;
                }

                this.odataUri = new ODataUri { Path = this.ContextUriParseResult.Path };

                this.isODataUriRead = true;
                return this.odataUri;
            }
        }

        /// <summary>
        /// Context for resource metadata centric responsibilities.
        /// </summary>
        internal IODataMetadataContext MetadataContext
        {
            get
            {
                // Under the client knob, the model we're given is really a facade, which doesn't flow open-type information into the combined types.
                // However, we need to answer this question for materializing operations which were left out of the payload.
                // To get around this, the client sets a callback in the ReaderBehavior to answer the question.
                return this.metadataContext ?? (this.metadataContext = new ODataMetadataContext(
                    this.ReadingResponse,
                    null,
                    this.JsonLightInputContext.EdmTypeResolver,
                    this.Model,
                    this.MetadataDocumentUri,
                    this.ODataUri,
                    this.JsonLightInputContext.MetadataLevel));
            }
        }

        /// <summary>
        /// Returns the <see cref="BufferingJsonReader"/> which is to be used to read the content of the message.
        /// </summary>
        internal BufferingJsonReader JsonReader
        {
            get
            {
                return this.jsonLightInputContext.JsonReader;
            }
        }

        /// <summary>Result of parsing the context URI for the payload (or null if none are available).</summary>
        /// <remarks>This property is only available after the ReadPayloadStart was called.</remarks>
        internal ODataJsonLightContextUriParseResult ContextUriParseResult
        {
            get
            {
#if DEBUG
                Debug.Assert(this.contextUriParseResultReady, "The contextUriParseResult property should not be accessed before ReadPayloadStart() is called.");
#endif
                return this.contextUriParseResult;
            }
        }

        /// <summary>
        /// The Json lite input context to use for reading.
        /// </summary>
        internal ODataJsonLightInputContext JsonLightInputContext
        {
            get { return this.jsonLightInputContext; }
        }

        /// <summary>
        /// Function called to read property custom annotation value.
        /// </summary>
        protected Func<PropertyAndAnnotationCollector, string, object> ReadPropertyCustomAnnotationValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the metadata document Uri from the contextUriParseResult.
        /// </summary>
        private Uri MetadataDocumentUri
        {
            get
            {
                Uri metadataDocumentUri = this.ContextUriParseResult != null && this.ContextUriParseResult.MetadataDocumentUri != null ? this.ContextUriParseResult.MetadataDocumentUri : null;
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
        /// <param name="payloadKind">The kind of payload we are reading; this guides the parsing of the context URI.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. a resource, a resource set or a collection within a parameters payload.</param>
        /// <param name="allowEmptyPayload">true if we allow a completely empty payload; otherwise false.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first property of the payload after having read (or skipped) the context URI property.
        ///                 Or the reader is positioned on an end-object node if there are no properties (other than the context URI which is required in responses and optional in requests).
        /// </remarks>
        internal void ReadPayloadStart(
            ODataPayloadKind payloadKind,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool isReadingNestedPayload,
            bool allowEmptyPayload)
        {
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            string contextUriAnnotationValue = this.ReadPayloadStartImplementation(
                payloadKind,
                propertyAndAnnotationCollector,
                isReadingNestedPayload,
                allowEmptyPayload);

            ODataJsonLightContextUriParseResult parseResult = null;

            // The context URI is only recognized in non-error response top-level payloads.
            // If the payload is nested (for example when we read URL literals) we don't recognize the context URI.
            // Top-level error payloads don't need and use the context URI.
            if (!isReadingNestedPayload && payloadKind != ODataPayloadKind.Error && contextUriAnnotationValue != null)
            {
                parseResult = ODataJsonLightContextUriParser.Parse(
                    this.Model,
                    contextUriAnnotationValue,
                    payloadKind,
                    this.MessageReaderSettings.ClientCustomTypeResolver,
                    this.JsonLightInputContext.ReadingResponse,
                    this.JsonLightInputContext.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);
            }

            this.contextUriParseResult = parseResult;
#if DEBUG
            this.contextUriParseResultReady = true;
#endif
        }

#if PORTABLELIB

        /// <summary>
        /// Read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="payloadKind">The kind of payload we are reading; this guides the parsing of the context URI.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. a resource, a resource set or a collection within a parameters payload.</param>
        /// <param name="allowEmptyPayload">true if we allow a completely empty payload; otherwise false.</param>
        /// <returns>The parsed context URI.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first property of the payload after having read (or skipped) the context URI property.
        ///                 Or the reader is positioned on an end-object node if there are no properties (other than the context URI which is required in responses and optional in requests).
        /// </remarks>
        internal Task ReadPayloadStartAsync(
            ODataPayloadKind payloadKind,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool isReadingNestedPayload,
            bool allowEmptyPayload)
        {
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            return TaskUtils.GetTaskForSynchronousOperation(() =>
                {
                    string contextUriAnnotationValue = this.ReadPayloadStartImplementation(
                        payloadKind,
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload,
                        allowEmptyPayload);

                    // The context URI is only recognized in non-error response top-level payloads.
                    // If the payload is nested (for example when we read URL literals) we don't recognize the context URI.
                    // Top-level error payloads don't need and use the context URI.
                    if (!isReadingNestedPayload && payloadKind != ODataPayloadKind.Error && contextUriAnnotationValue != null)
                    {
                        this.contextUriParseResult = ODataJsonLightContextUriParser.Parse(
                            this.Model,
                            contextUriAnnotationValue,
                            payloadKind,
                            this.MessageReaderSettings.ClientCustomTypeResolver,
                            this.JsonLightInputContext.ReadingResponse);
                    }

#if DEBUG
                    this.contextUriParseResultReady = true;
#endif
                });
        }
#endif

        /// <summary>
        /// Reads the end of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. a resource, a resource set or a collection within a parameters payload.</param>
        /// <remarks>
        /// Pre-Condition:  any node:                when reading response or a nested payload, will fail if find anything else then EndObject.
        ///                 JsonNodeType.EndOfInput: otherwise
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isReadingNestedPayload", Justification = "The parameter is used in debug builds.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal void ReadPayloadEnd(bool isReadingNestedPayload)
        {
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
            string valueRead = this.JsonReader.ReadStringValue(annotationName);
            ODataJsonLightReaderUtils.ValidateAnnotationValue(valueRead, annotationName);
            return valueRead;
        }

        /// <summary>
        /// Reads a string value from the json reader.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>The string that was read.</returns>
        internal string ReadAnnotationStringValue(string annotationName)
        {
            return this.JsonReader.ReadStringValue(annotationName);
        }

        /// <summary>
        /// Reads a string value from the json reader and processes it as a Uri.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>The Uri that was read.</returns>
        internal Uri ReadAnnotationStringValueAsUri(string annotationName)
        {
            string stringValue = this.JsonReader.ReadStringValue(annotationName);

            if (stringValue == null)
            {
                return null;
            }

            return this.ReadingResponse ? this.ProcessUriFromPayload(stringValue) : new Uri(stringValue, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Reads and validates a string value from the json reader and processes it as a Uri.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>The Uri that was read.</returns>
        internal Uri ReadAndValidateAnnotationStringValueAsUri(string annotationName)
        {
            string stringValue = this.ReadAndValidateAnnotationStringValue(annotationName);
            return this.ReadingResponse ? this.ProcessUriFromPayload(stringValue) : new Uri(stringValue, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Reads and validates a value from the json reader and processes it as a long.
        /// The input value could be string or number
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read.</param>
        /// <returns>The long that is read.</returns>
        internal long ReadAndValidateAnnotationAsLongForIeee754Compatible(string annotationName)
        {
            object value = this.JsonReader.ReadPrimitiveValue();

            ODataJsonLightReaderUtils.ValidateAnnotationValue(value, annotationName);

            if ((value is string) ^ this.JsonReader.IsIeee754Compatible)
            {
                throw new ODataException(Strings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter(Metadata.EdmConstants.EdmInt64TypeName));
            }

            return (long)ODataJsonLightReaderUtils.ConvertValue(
                    value,
                    EdmCoreModel.Instance.GetInt64(false),
                    this.MessageReaderSettings,
                    /*validateNullValue*/ true,
                    annotationName,
                    this.JsonLightInputContext.PayloadValueConverter);
        }

        /// <summary>
        /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
        /// </summary>
        /// <param name="uriFromPayload">The URI string from the payload to process.</param>
        /// <returns>An absolute URI to report.</returns>
        internal Uri ProcessUriFromPayload(string uriFromPayload)
        {
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
                    throw new ODataException(Strings.ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation(uriFromPayload, ODataAnnotationNames.ODataContext));
                }

                uri = UriUtils.UriToAbsoluteUri(metadataDocumentUri, uri);
            }

            Debug.Assert(uri.IsAbsoluteUri, "By now we should have an absolute URI.");
            return uri;
        }

        /// <summary>
        /// Parses JSON object property starting with the current position of the JSON reader.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use, it will also store the property annotations found.</param>
        /// <param name="readPropertyAnnotationValue">Function called to read property annotation value.</param>
        /// <param name="handleProperty">Function callback to handle the result of parsing property.</param>
        internal void ProcessProperty(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            Func<string, object> readPropertyAnnotationValue,
            Action<PropertyParsingResult, string> handleProperty)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(readPropertyAnnotationValue != null, "readPropertyAnnotationValue != null");
            Debug.Assert(handleProperty != null, "handleProperty != null");
            this.AssertJsonCondition(JsonNodeType.Property);

            string propertyName;
            PropertyParsingResult propertyParsingResult = this.ParseProperty(
                propertyAndAnnotationCollector, readPropertyAnnotationValue, out propertyName);

            while (propertyParsingResult == PropertyParsingResult.CustomInstanceAnnotation && this.ShouldSkipCustomInstanceAnnotation(propertyName))
            {
                // Skip over the instance annotation value and don't report it to the OM.
                this.JsonReader.SkipValue();
                propertyParsingResult = this.ParseProperty(
                    propertyAndAnnotationCollector, readPropertyAnnotationValue, out propertyName);
            }

            handleProperty(propertyParsingResult, propertyName);
            if (propertyParsingResult != PropertyParsingResult.EndOfObject
                && propertyParsingResult != PropertyParsingResult.CustomInstanceAnnotation)
            {
                propertyAndAnnotationCollector.MarkPropertyAsProcessed(propertyName);
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
        /// Reads the odata.context annotation.
        /// </summary>
        /// <param name="payloadKind">The payload kind for which to read the context URI.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="failOnMissingContextUriAnnotation">true if the method should fail if the context URI annotation is missing, false if that can be ignored.</param>
        /// <returns>The value of the context URI annotation.</returns>
        internal string ReadContextUriAnnotation(
            ODataPayloadKind payloadKind,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool failOnMissingContextUriAnnotation)
        {
            if (this.JsonReader.NodeType != JsonNodeType.Property)
            {
                if (!failOnMissingContextUriAnnotation || payloadKind == ODataPayloadKind.Unsupported)
                {
                    // Do not fail during payload kind detection
                    return null;
                }

                throw new ODataException(Strings.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
            }

            // Must make sure the input odata.context has a '@' prefix
            string propertyName = this.JsonReader.GetPropertyName();
            if (string.CompareOrdinal(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataContext, propertyName) != 0
                && !this.CompareSimplifiedODataAnnotation(JsonLightConstants.SimplifiedODataContextPropertyName, propertyName))
            {
                if (!failOnMissingContextUriAnnotation || payloadKind == ODataPayloadKind.Unsupported)
                {
                    // Do not fail during payload kind detection
                    return null;
                }

                throw new ODataException(Strings.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
            }

            // Read over the property name
            this.JsonReader.ReadNext();
            return this.JsonReader.ReadStringValue();
        }

        /// <summary>
        /// Compares the JSON property name with the simplified OData annotation property name.
        /// </summary>
        /// <param name="simplifiedPropertyName">The simplified OData annotation property name.</param>
        /// <param name="propertyName">The JSON property name read from the payload.</param>
        /// <returns>If the JSON property name equals the simplified OData annotation property name.</returns>
        protected bool CompareSimplifiedODataAnnotation(string simplifiedPropertyName, string propertyName)
        {
            Debug.Assert(simplifiedPropertyName.IndexOf('@') == 0, "simplifiedPropertyName must start with '@'.");
            Debug.Assert(simplifiedPropertyName.IndexOf('.') == -1, "simplifiedPropertyName must not be namespace-qualified.");

            return this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingODataAnnotationWithoutPrefix &&
                   string.CompareOrdinal(simplifiedPropertyName, propertyName) == 0;
        }

        /// <summary>
        /// Completes the simplified OData annotation name with "odata.".
        /// </summary>
        /// <param name="annotationName">The annotation name to be completed.</param>
        /// <returns>The complete OData annotation name.</returns>
        protected string CompleteSimplifiedODataAnnotation(string annotationName)
        {
            if (this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingODataAnnotationWithoutPrefix &&
                annotationName.IndexOf('.') == -1)
            {
                annotationName = JsonLightConstants.ODataAnnotationNamespacePrefix + annotationName;
            }

            return annotationName;
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
        /// Test the instance annotation is start with @ prefix
        /// </summary>
        /// <param name="annotationName">the origin annotation name from reader</param>
        /// <returns>true is the instance annotation, false is not</returns>
        private static bool IsInstanceAnnotation(string annotationName)
        {
            if (!String.IsNullOrEmpty(annotationName) && annotationName[0] == JsonLightConstants.ODataPropertyAnnotationSeparatorChar)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// If <paramref name="annotationName"/> is under the odata namespace but is not known to ODataLib, move the JSON reader forward to skip the
        /// annotation name and value then return true; return false otherwise.
        /// </summary>
        /// <remarks>
        /// The unknown odata annotation is skipped so that when this version of the reader reads a resource set produced by a future version of ODataLib
        /// that contains an odata annotation that is not recognized on this version, we would simply ignore the annotation rather than failing.
        /// Note that when we add new odata annotations that cannot be skipped, we would bump the protocol version.
        /// </remarks>
        /// <param name="annotationName">The annotation name in question.</param>
        /// <param name="annotationValue">Outputs the .</param>
        /// <returns>Returns true if the annotation name and value is skipped; returns false otherwise.</returns>
        private bool SkippedOverUnknownODataAnnotation(string annotationName, out object annotationValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            this.AssertJsonCondition(JsonNodeType.Property);

            if (ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName))
            {
                annotationValue = ReadODataOrCustomInstanceAnnotationValue(annotationName);
                return true;
            }

            annotationValue = null;
            return false;
        }

        /// <summary>
        /// Reads "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="annotationName">The annotation name.</param>
        /// <returns>The annotation value.</returns>
        private object ReadODataOrCustomInstanceAnnotationValue(string annotationName)
        {
            // Read over the name.
            this.JsonReader.Read();
            object annotationValue;
            if (this.JsonReader.NodeType != JsonNodeType.PrimitiveValue)
            {
                annotationValue = this.JsonReader.ReadAsUntypedOrNullValue();
            }
            else
            {
                annotationValue = this.JsonReader.Value;
                this.JsonReader.SkipValue();
            }

            return annotationValue;
        }

        /// <summary>
        /// Parses JSON object property starting with the current position of the JSON reader.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use, it will also store the property annotations found.</param>
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
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            Func<string, object> readPropertyAnnotationValue,
            out string parsedPropertyName)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(readPropertyAnnotationValue != null, "readPropertyAnnotationValue != null");
            string lastPropertyAnnotationNameFound = null;
            parsedPropertyName = null;

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string nameFromReader = this.JsonReader.GetPropertyName();

                string propertyNameFromReader, annotationNameFromReader;
                bool isPropertyAnnotation = TryParsePropertyAnnotation(nameFromReader, out propertyNameFromReader, out annotationNameFromReader);

                // reading a nested delta resource set
                if (isPropertyAnnotation && String.CompareOrdinal(this.CompleteSimplifiedODataAnnotation(annotationNameFromReader), ODataAnnotationNames.ODataDelta) == 0)
                {
                    // Read over the property name.
                    this.JsonReader.Read();
                    parsedPropertyName = propertyNameFromReader;
                    return PropertyParsingResult.NestedDeltaResourceSet;
                }

                bool isInstanceAnnotation = false;
                if (!isPropertyAnnotation)
                {
                    isInstanceAnnotation = IsInstanceAnnotation(nameFromReader);
                    propertyNameFromReader = isInstanceAnnotation ? this.CompleteSimplifiedODataAnnotation(nameFromReader.Substring(1)) : nameFromReader;
                }

                // If parsedPropertyName is set and is different from the property name the reader is currently on,
                // we have parsed a property annotation for a different property than the one at the current position.
                if (parsedPropertyName != null && string.CompareOrdinal(parsedPropertyName, propertyNameFromReader) != 0)
                {
                    if (ODataJsonLightReaderUtils.IsAnnotationProperty(parsedPropertyName))
                    {
                        throw new ODataException(Strings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue(lastPropertyAnnotationNameFound, parsedPropertyName));
                    }

                    return PropertyParsingResult.PropertyWithoutValue;
                }

                object annotationValue = null;
                if (isPropertyAnnotation)
                {
                    annotationNameFromReader = this.CompleteSimplifiedODataAnnotation(annotationNameFromReader);

                    // If this is a unknown odata annotation targeting a property, we skip over it. See remark on the method SkippedOverUnknownODataAnnotation() for detailed explaination.
                    // Note that we don't skip over unknown odata annotations targeting another annotation. We don't allow annotations (except odata.type) targeting other annotations,
                    // so this.ProcessPropertyAnnotation() will test and fail for that case.
                    if (!ODataJsonLightReaderUtils.IsAnnotationProperty(propertyNameFromReader) && this.SkippedOverUnknownODataAnnotation(annotationNameFromReader, out annotationValue))
                    {
                        propertyAndAnnotationCollector.AddODataPropertyAnnotation(propertyNameFromReader, annotationNameFromReader, annotationValue);
                        continue;
                    }

                    // We have another property annotation for the same property we parsed.
                    parsedPropertyName = propertyNameFromReader;
                    lastPropertyAnnotationNameFound = annotationNameFromReader;

                    this.ProcessPropertyAnnotation(propertyNameFromReader, annotationNameFromReader, propertyAndAnnotationCollector, readPropertyAnnotationValue);
                    continue;
                }

                // If this is a unknown odata annotation, skip over it. See remark on the method SkippedOverUnknownODataAnnotation() for detailed explaination.
                if (isInstanceAnnotation && this.SkippedOverUnknownODataAnnotation(propertyNameFromReader, out annotationValue))
                {
                    // collect 'odata.<unknown>' annotation:
                    // here we know the original property name contains no '@', but '.' dot
                    Debug.Assert(annotationNameFromReader == null, "annotationNameFromReader == null");
                    propertyAndAnnotationCollector.AddODataScopeAnnotation(propertyNameFromReader, annotationValue);
                    continue;
                }

                // We are encountering the property name for the first time.
                // Read over the property name.
                this.JsonReader.Read();
                parsedPropertyName = propertyNameFromReader;

                if (!isInstanceAnnotation && ODataJsonLightUtils.IsMetadataReferenceProperty(propertyNameFromReader))
                {
                    return PropertyParsingResult.MetadataReferenceProperty;
                }

                if (!isInstanceAnnotation && !ODataJsonLightReaderUtils.IsAnnotationProperty(propertyNameFromReader))
                {
                    // Normal property
                    return PropertyParsingResult.PropertyWithValue;
                }

                // collect 'xxx.yyyy' annotation:
                // here we know the original property name contains no '@', but '.' dot
                Debug.Assert(annotationNameFromReader == null, "annotationNameFromReader == null");

                // Handle 'odata.XXXXX' annotations
                if (isInstanceAnnotation && ODataJsonLightReaderUtils.IsODataAnnotationName(propertyNameFromReader))
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
                    throw new ODataException(Strings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue(lastPropertyAnnotationNameFound, parsedPropertyName));
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
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="readPropertyAnnotationValue">Callback to read the property annotation value.</param>
        private void ProcessPropertyAnnotation(string annotatedPropertyName, string annotationName, PropertyAndAnnotationCollector propertyAndAnnotationCollector, Func<string, object> readPropertyAnnotationValue)
        {
            // We don't currently support annotation targeting an instance annotation except for the @odata.type property annotation.
            if (ODataJsonLightReaderUtils.IsAnnotationProperty(annotatedPropertyName) && string.CompareOrdinal(annotationName, ODataAnnotationNames.ODataType) != 0)
            {
                throw new ODataException(Strings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation(annotationName, annotatedPropertyName, ODataAnnotationNames.ODataType));
            }

            ReadODataOrCustomInstanceAnnotationValue(annotatedPropertyName, annotationName, propertyAndAnnotationCollector, readPropertyAnnotationValue);
        }

        /// <summary>
        /// Reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="annotatedPropertyName">The property name.</param>
        /// <param name="annotationName">The annotation name</param>
        /// <param name="propertyAndAnnotationCollector">The PropertyAndAnnotationCollector.</param>
        /// <param name="readPropertyAnnotationValue">Callback to read the property annotation value.</param>
        private void ReadODataOrCustomInstanceAnnotationValue(string annotatedPropertyName, string annotationName, PropertyAndAnnotationCollector propertyAndAnnotationCollector, Func<string, object> readPropertyAnnotationValue)
        {
            // Read over the property name.
            this.JsonReader.Read();
            if (ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName))
            {
                // OData annotations are read
                propertyAndAnnotationCollector.AddODataPropertyAnnotation(annotatedPropertyName, annotationName, readPropertyAnnotationValue(annotationName));
            }
            else
            {
                if (this.ShouldSkipCustomInstanceAnnotation(annotationName) || (this is ODataJsonLightErrorDeserializer && this.MessageReaderSettings.ShouldIncludeAnnotation == null))
                {
                    propertyAndAnnotationCollector.CheckIfPropertyOpenForAnnotations(annotatedPropertyName, annotationName);
                    this.JsonReader.SkipValue();
                }
                else
                {
                    Debug.Assert(ReadPropertyCustomAnnotationValue != null, "readPropertyCustomAnnotationValue != null");
                    propertyAndAnnotationCollector.AddCustomPropertyAnnotation(annotatedPropertyName, annotationName, ReadPropertyCustomAnnotationValue(propertyAndAnnotationCollector, annotationName));
                }
            }
        }

        /// <summary>
        /// Read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="payloadKind">The kind of payload we are reading; this guides the parsing of the context URI.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. a resource, a resource set or a collection within a parameters payload.</param>
        /// <param name="allowEmptyPayload">true if we allow a completely empty payload; otherwise false.</param>
        /// <returns>The value of the context URI annotation (or null if it was not found).</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first property of the payload after having read (or skipped) the context URI property.
        ///                 Or the reader is positioned on an end-object node if there are no properties (other than the context URI which is required in responses and optional in requests).
        /// </remarks>
        private string ReadPayloadStartImplementation(
            ODataPayloadKind payloadKind,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool isReadingNestedPayload,
            bool allowEmptyPayload)
        {
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
                    // Skip over the context URI annotation in request payloads or when we've already read it
                    // during payload kind detection.
                    bool failOnMissingContextUriAnnotation = this.jsonLightInputContext.ReadingResponse;

                    // In responses we expect the context URI to be the first thing in the payload
                    // (except for error payloads). In requests we ignore the context URI.
                    return this.ReadContextUriAnnotation(payloadKind, propertyAndAnnotationCollector, failOnMissingContextUriAnnotation);
                }

                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            }

            return null;
        }
    }
}
