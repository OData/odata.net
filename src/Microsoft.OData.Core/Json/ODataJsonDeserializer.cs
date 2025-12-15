//---------------------------------------------------------------------
// <copyright file="ODataJsonDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData Json deserializers.
    /// </summary>
    internal abstract class ODataJsonDeserializer : ODataDeserializer
    {
        /// <summary>The Json input context to use for reading.</summary>
        private readonly ODataJsonInputContext jsonInputContext;

        /// <summary>Context for resource metadata centric responsibilities.</summary>
        private IODataMetadataContext metadataContext;

        /// <summary>Result of parsing the context URI for the payload (or null if none are available).</summary>
        /// <remarks>This field is only available after the ReadPayloadStart was called.</remarks>
        private ODataJsonContextUriParseResult contextUriParseResult;

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
        /// <param name="jsonInputContext">The Json input context to read from.</param>
        protected ODataJsonDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
            Debug.Assert(jsonInputContext != null, "jsonInputContext != null");

            this.jsonInputContext = jsonInputContext;
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

            /// <summary>A property representing a nested delta resource set was found.</summary>
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

                ODataUriSlim? odataUri = null;
                if (this.ODataUri != null)
                {
                    odataUri = new ODataUriSlim(this.ODataUri);
                }

                return this.metadataContext ?? (this.metadataContext = new ODataMetadataContext(
                    this.ReadingResponse,
                    null,
                    this.JsonInputContext.EdmTypeResolver,
                    this.Model,
                    this.MetadataDocumentUri,
                    odataUri,
                    this.JsonInputContext.MetadataLevel));
            }
        }

        /// <summary>
        /// Returns the <see cref="BufferingJsonReader"/> which is to be used to read the content of the message.
        /// </summary>
        internal BufferingJsonReader JsonReader
        {
            get
            {
                return this.jsonInputContext.JsonReader;
            }
        }

        /// <summary>Result of parsing the context URI for the payload (or null if none are available).</summary>
        /// <remarks>This property is only available after the ReadPayloadStart was called.</remarks>
        internal ODataJsonContextUriParseResult ContextUriParseResult
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
        /// The Json input context to use for reading.
        /// </summary>
        internal ODataJsonInputContext JsonInputContext
        {
            get { return this.jsonInputContext; }
        }

        /// <summary>
        /// Function called to read property custom annotation value.
        /// </summary>
        /// <Remarks>
        /// Function takes:
        /// * The duplicate property names checker
        /// * The annotation name
        /// Function returns:
        /// * The read property annotation value
        /// </Remarks>
        protected Func<PropertyAndAnnotationCollector, string, object> ReadPropertyCustomAnnotationValue
        {
            get;
            set;
        }

        /// <summary>
        /// Delegate called to read property custom annotation value asynchronously.
        /// </summary>
        /// <Remarks>
        /// Function takes:
        /// * The duplicate property names checker
        /// * The annotation name
        /// Function returns:
        /// * A task that represents the asynchronous read operation. The value of the TResult parameter contains the read property annotation value
        /// </Remarks>
        protected Func<PropertyAndAnnotationCollector, string, Task<object>> ReadPropertyCustomAnnotationValueAsync
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

            int propertyAnnotationSeparatorIndex = propertyAnnotationName.IndexOf(ODataJsonConstants.ODataPropertyAnnotationSeparatorChar, StringComparison.Ordinal);
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
            bool allowEmptyPayload,
            IEdmNavigationSource navigationSource = null)
        {
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            string contextUriAnnotationValue = this.ReadPayloadStartImplementation(
                payloadKind,
                propertyAndAnnotationCollector,
                isReadingNestedPayload,
                allowEmptyPayload);

            //Concatenate the metadata uri with the contextUri, when the contextUri is a relativeUri. 
            Uri contextUri;
            if (this.BaseUri != null &&
                !string.IsNullOrEmpty(contextUriAnnotationValue) &&
                !Uri.TryCreate(contextUriAnnotationValue, UriKind.Absolute, out contextUri))
            {
                // If the Base uri string is http://odata.org/test
                // The MetadataDocumentUri will be http://odata.org/test/$metadata
                // If the contextUriAnnotation value is Customers(1)/Name, $metadata#Customers(1)/Name, or ../$metadata#Customers(1)/Name
                // The generated context uri will be http://odata.org/test/$metadata#Customers(1)/Name
                ODataUri oDataUri = new ODataUri() { ServiceRoot = this.BaseUri };

                contextUriAnnotationValue = contextUriAnnotationValue.TrimStart('/', '.');

                contextUriAnnotationValue = contextUriAnnotationValue.StartsWith("$metadata#", StringComparison.OrdinalIgnoreCase)
                    ? this.BaseUri + contextUriAnnotationValue
                    : oDataUri.MetadataDocumentUri.ToString() + ODataConstants.ContextUriFragmentIndicator + contextUriAnnotationValue;
            }

            ODataJsonContextUriParseResult parseResult = null;

            // The context URI is only recognized in non-error response top-level payloads.
            // If the payload is nested (for example when we read URL literals) we don't recognize the context URI.
            // Top-level error payloads don't need and use the context URI.
            if (!isReadingNestedPayload && payloadKind != ODataPayloadKind.Error && contextUriAnnotationValue != null)
            {
                parseResult = ODataJsonContextUriParser.Parse(
                    this.Model,
                    contextUriAnnotationValue,
                    payloadKind,
                    this.MessageReaderSettings.ClientCustomTypeResolver,
                    this.JsonInputContext.ReadingResponse || payloadKind == ODataPayloadKind.Delta,
                    this.JsonInputContext.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata,
                    this.BaseUri, 
                    navigationSource);
            }

            this.contextUriParseResult = parseResult;
#if DEBUG
            this.contextUriParseResultReady = true;
#endif
        }


        /// <summary>
        /// Asynchronously read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="payloadKind">The kind of payload we are reading; this guides the parsing of the context URI.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. a resource, a resource set or a collection within a parameters payload.</param>
        /// <param name="allowEmptyPayload">true if we allow a completely empty payload; otherwise false.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first property of the payload after having read (or skipped) the context URI property.
        ///                 Or the reader is positioned on an end-object node if there are no properties (other than the context URI which is required in responses and optional in requests).
        /// </remarks>
        internal Task ReadPayloadStartAsync(
            ODataPayloadKind payloadKind,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool isReadingNestedPayload,
            bool allowEmptyPayload,
            IEdmNavigationSource navigationSource = null)
        {
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None,
                "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            Task<string> readPayloadStartTask = this.ReadPayloadStartImplementationAsync(
                        payloadKind,
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload,
                        allowEmptyPayload);

            if (readPayloadStartTask.IsCompletedSuccessfully)
            {
                ProcessContextUriAnnotationValue(
                    this,
                    payloadKind,
                    isReadingNestedPayload,
                    readPayloadStartTask.Result,
                    navigationSource);

                return Task.CompletedTask;
            }

            return AwaitReadPayloadStartAsync(
                this,
                payloadKind,
                isReadingNestedPayload,
                readPayloadStartTask.Result,
                navigationSource,
                readPayloadStartTask);

            static void ProcessContextUriAnnotationValue(
                ODataJsonDeserializer thisParam,
                ODataPayloadKind payloadKindParam,
                bool isReadingNestedPayloadParam,
                string contextUriAnnotationValueParam,
                IEdmNavigationSource navigationSourceParam)
            {
                // The context URI is only recognized in non-error response top-level payloads.
                // If the payload is nested (for example when we read URL literals) we don't recognize the context URI.
                // Top-level error payloads don't need and use the context URI.
                if (!isReadingNestedPayloadParam && payloadKindParam != ODataPayloadKind.Error && contextUriAnnotationValueParam != null)
                {
                    thisParam.contextUriParseResult = ODataJsonContextUriParser.Parse(
                        thisParam.Model,
                        contextUriAnnotationValueParam,
                        payloadKindParam,
                        thisParam.MessageReaderSettings.ClientCustomTypeResolver,
                        thisParam.JsonInputContext.ReadingResponse || payloadKindParam == ODataPayloadKind.Delta,
                        thisParam.JsonInputContext.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata,
                        thisParam.BaseUri,
                        navigationSourceParam);
                }

#if DEBUG
                thisParam.contextUriParseResultReady = true;
#endif
            }

            static async Task AwaitReadPayloadStartAsync(
                ODataJsonDeserializer thisParam,
                ODataPayloadKind payloadKindParam,
                bool isReadingNestedPayloadParam,
                string contextUriAnnotationValueParam,
                IEdmNavigationSource navigationSourceParam,
                Task<string> pendingReadPayloadStartTask)
            {
                string contextUriAnnotationValue = await pendingReadPayloadStartTask.ConfigureAwait(false);

                ProcessContextUriAnnotationValue(
                    thisParam,
                    payloadKindParam,
                    isReadingNestedPayloadParam,
                    contextUriAnnotationValue,
                    navigationSourceParam);
            }
        }

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
            ODataJsonReaderUtils.ValidateAnnotationValue(valueRead, annotationName);
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

            ODataJsonReaderUtils.ValidateAnnotationValue(value, annotationName);

            if ((value is string) ^ this.JsonReader.IsIeee754Compatible)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter, Metadata.EdmConstants.EdmInt64TypeName));
            }

            return (long)ODataJsonReaderUtils.ConvertValue(
                    value,
                    EdmCoreModel.Instance.GetInt64(false),
                    this.MessageReaderSettings,
                    /*validateNullValue*/ true,
                    annotationName,
                    this.JsonInputContext.PayloadValueConverter);
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
            Uri resolvedUri = this.JsonInputContext.ResolveUri(metadataDocumentUri, uri);
            if (resolvedUri != null)
            {
                return resolvedUri;
            }

            if (!uri.IsAbsoluteUri)
            {
                if (metadataDocumentUri == null)
                {
                    throw new ODataException(Error.Format(SRResources.ODataJsonDeserializer_RelativeUriUsedWithouODataMetadataAnnotation, uriFromPayload, ODataAnnotationNames.ODataContext));
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
                // Read over the property name
                this.JsonReader.Read();

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
                this.JsonReader.GetValue(),
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

                throw new ODataException(SRResources.ODataJsonDeserializer_ContextLinkNotFoundAsFirstProperty);
            }

            // Must make sure the input odata.context has a '@' prefix
            string propertyName = this.JsonReader.GetPropertyName();
            if (!string.Equals(ODataJsonConstants.PrefixedODataContextPropertyName, propertyName, StringComparison.Ordinal)
                && !this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataContextPropertyName, propertyName))
            {
                if (!failOnMissingContextUriAnnotation || payloadKind == ODataPayloadKind.Unsupported)
                {
                    // Do not fail during payload kind detection
                    return null;
                }

                throw new ODataException(SRResources.ODataJsonDeserializer_ContextLinkNotFoundAsFirstProperty);
            }

            if (propertyAndAnnotationCollector != null)
            {
                propertyAndAnnotationCollector.MarkPropertyAsProcessed(propertyName);
            }

            // Read over the property name
            this.JsonReader.ReadNext();
            return this.JsonReader.ReadStringValue();
        }

        /// <summary>
        /// Asynchronously reads and validates a string value from the json reader.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the string that was read.
        /// </returns>
        internal Task<string> ReadAndValidateAnnotationStringValueAsync(string annotationName)
        {
            Task<string> readStringValueTask = this.JsonReader.ReadStringValueAsync();

            if (!readStringValueTask.IsCompletedSuccessfully)
            {
                return AwaitReadStringValueAsync(annotationName, readStringValueTask);
            }

            try
            {
                string valueRead = readStringValueTask.Result;
                ODataJsonReaderUtils.ValidateAnnotationValue(valueRead, annotationName);

                return Task.FromResult(valueRead);
            }
            catch (ODataException ex)
            {
                return Task.FromException<string>(ex);
            }

            static async Task<string> AwaitReadStringValueAsync(string annotationNameParam, Task<string> pendingReadStringValueTask)
            {
                string valueRead = await pendingReadStringValueTask.ConfigureAwait(false);

                ODataJsonReaderUtils.ValidateAnnotationValue(valueRead, annotationNameParam);

                return valueRead;
            }
        }

        /// <summary>
        /// Asynchronously reads a string value from the json reader.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the string that was read.
        /// </returns>
        internal Task<string> ReadAnnotationStringValueAsync(string annotationName)
        {
            return this.JsonReader.ReadStringValueAsync(annotationName);
        }

        /// <summary>
        /// Asynchronously reads a string value from the json reader and processes it as a Uri.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the Uri that was read.
        /// </returns>
        internal Task<Uri> ReadAnnotationStringValueAsUriAsync(string annotationName)
        {
            Task<string> readStringValueTask = this.JsonReader.ReadStringValueAsync();

            if (readStringValueTask.IsCompletedSuccessfully)
            {
                return Task.FromResult(ProcessUri(this, readStringValueTask.Result));
            }

            return AwaitReadStringValueAsync(this, readStringValueTask);

            static Uri ProcessUri(ODataJsonDeserializer thisParam, string stringValueParam)
            {
                if (stringValueParam == null)
                {
                    return null;
                }

                return thisParam.ReadingResponse ? thisParam.ProcessUriFromPayload(stringValueParam) : new Uri(stringValueParam, UriKind.RelativeOrAbsolute);
            }

            static async Task<Uri> AwaitReadStringValueAsync(ODataJsonDeserializer thisParam, Task<string> pendingReadStringValueTask)
            {
                var stringValue = await pendingReadStringValueTask.ConfigureAwait(false);

                return ProcessUri(thisParam, stringValue);
            }
        }

        /// <summary>
        /// Asynchronously reads and validates a string value from the json reader and processes it as a Uri.
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read (used for error reporting).</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the Uri that was read.
        /// </returns>
        internal Task<Uri> ReadAndValidateAnnotationStringValueAsUriAsync(string annotationName)
        {
            Task<string> readAndValidateAnnotationStringValueTask = this.ReadAndValidateAnnotationStringValueAsync(annotationName);

            if (readAndValidateAnnotationStringValueTask.IsCompletedSuccessfully)
            {
                return Task.FromResult(ProcessUri(this, readAndValidateAnnotationStringValueTask.Result));
            }

            return AwaitReadAndValidateAnnotationStringValueAsync(this, readAndValidateAnnotationStringValueTask);

            static async Task<Uri> AwaitReadAndValidateAnnotationStringValueAsync(
                ODataJsonDeserializer thisParam,
                Task<string> pendingReadAndValidateAnnotationTask)
            {
                string stringValueParam = await pendingReadAndValidateAnnotationTask
                    .ConfigureAwait(false);

                return ProcessUri(thisParam, stringValueParam);
            }

            static Uri ProcessUri(ODataJsonDeserializer thisParam, string stringValueParam)
            {
                return thisParam.ReadingResponse ? thisParam.ProcessUriFromPayload(stringValueParam) : new Uri(stringValueParam, UriKind.RelativeOrAbsolute);
            }
        }

        /// <summary>
        /// Asynchronously reads and validates a value from the json reader and processes it as a long.
        /// The input value could be string or number
        /// </summary>
        /// <param name="annotationName">The name of the annotation being read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the <see cref="long"/> value that was read.
        /// </returns>
        internal Task<long> ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync(string annotationName)
        {
            Task<object> readPrimitiveValueTask = this.JsonReader.ReadPrimitiveValueAsync();

            if (readPrimitiveValueTask.IsCompletedSuccessfully)
            {
                try
                {
                    return Task.FromResult(ProcessAnnotationValue(this, readPrimitiveValueTask.Result, annotationName));
                }
                catch (ODataException ex)
                {
                    return Task.FromException<long>(ex);
                }
            }

            return AwaitReadPrimitiveValueAsync(this, annotationName, readPrimitiveValueTask);

            static long ProcessAnnotationValue(ODataJsonDeserializer thisParam, object valueParam, string annotationNameParam)
            {
                ODataJsonReaderUtils.ValidateAnnotationValue(valueParam, annotationNameParam);

                if ((valueParam is string) ^ thisParam.JsonReader.IsIeee754Compatible)
                {
                    throw new ODataException(
                        Error.Format(SRResources.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter, Metadata.EdmConstants.EdmInt64TypeName));
                }

                return (long)ODataJsonReaderUtils.ConvertValue(
                        valueParam,
                        EdmCoreModel.Instance.GetInt64(false),
                        thisParam.MessageReaderSettings,
                        validateNullValue: true,
                        propertyName: annotationNameParam,
                        converter: thisParam.JsonInputContext.PayloadValueConverter);
            }

            static async Task<long> AwaitReadPrimitiveValueAsync(ODataJsonDeserializer thisParam, string annotationNameParam, Task<object> pendingReadPrimitiveValueTask)
            {
                object value = await pendingReadPrimitiveValueTask.ConfigureAwait(false);

                return ProcessAnnotationValue(thisParam, value, annotationNameParam);
            }
        }

        /// <summary>
        /// Asynchronously parses JSON object property starting with the current position of the JSON reader.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use, it will also store the property annotations found.</param>
        /// <param name="readPropertyAnnotationValueDelegate">Delegate to read property annotation value.</param>
        /// <param name="handlePropertyDelegate">Delegate to handle the result of parsing property.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal Task ProcessPropertyAsync(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            Func<string, Task<object>> readPropertyAnnotationValueDelegate,
            Func<PropertyParsingResult, string, Task> handlePropertyDelegate)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, $"{nameof(propertyAndAnnotationCollector)} != null");
            Debug.Assert(readPropertyAnnotationValueDelegate != null, $"{nameof(readPropertyAnnotationValueDelegate)} != null");
            Debug.Assert(handlePropertyDelegate != null, $"{nameof(handlePropertyDelegate)} != null");
            this.AssertJsonCondition(JsonNodeType.Property);

            ValueTask<(PropertyParsingResult propertyParsingResult, string propertyName)> parsePropertyTask = this.ParsePropertyAsync(
                propertyAndAnnotationCollector,
                readPropertyAnnotationValueDelegate);

            if (!parsePropertyTask.IsCompletedSuccessfully)
            {
                return AwaitParsePropertyAsync(
                    this,
                    propertyAndAnnotationCollector,
                    readPropertyAnnotationValueDelegate,
                    handlePropertyDelegate,
                    parsePropertyTask);
            }

            return ContinueAfterParsePropertyAsync(
                this,
                propertyAndAnnotationCollector,
                readPropertyAnnotationValueDelegate,
                handlePropertyDelegate,
                parsePropertyTask.Result.propertyParsingResult,
                parsePropertyTask.Result.propertyName);

            static Task ContinueAfterParsePropertyAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                Func<PropertyParsingResult, string, Task> handlePropertyDelegateParam,
                PropertyParsingResult propertyParsingResultParam,
                string propertyNameParam)
            {
                while (true)
                {
                    if (propertyParsingResultParam != PropertyParsingResult.CustomInstanceAnnotation
                        || !thisParam.ShouldSkipCustomInstanceAnnotation(propertyNameParam))
                    {
                        break;
                    }

                    ValueTask<(PropertyParsingResult propertyParsingResult, string propertyName)> processPropertyLocalTask = ProcessPropertyLocalAsync(
                        thisParam,
                        propertyAndAnnotationCollectorParam,
                        readPropertyAnnotationValueDelegateParam);

                    if (!processPropertyLocalTask.IsCompletedSuccessfully)
                    {
                        return AwaitParsePropertyLocalAsync(
                            thisParam,
                            propertyAndAnnotationCollectorParam,
                            readPropertyAnnotationValueDelegateParam,
                            handlePropertyDelegateParam,
                            processPropertyLocalTask);
                    }
                }

                return FinalizeProcessPropertyAsync(
                    thisParam,
                    propertyAndAnnotationCollectorParam,
                    handlePropertyDelegateParam,
                    propertyParsingResultParam,
                    propertyNameParam);
            }

            static ValueTask<(PropertyParsingResult propertyParsingResult, string propertyName)> ProcessPropertyLocalAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam)
            {
                // Read over the property name
                Task<bool> readTask = thisParam.JsonReader.ReadAsync();

                if (!readTask.IsCompletedSuccessfully)
                {
                    return AwaitReadAsync(
                        thisParam,
                        propertyAndAnnotationCollectorParam,
                        readPropertyAnnotationValueDelegateParam,
                        readTask);
                }

                return ContinueAfterReadAsync(thisParam, propertyAndAnnotationCollectorParam, readPropertyAnnotationValueDelegateParam);
            }

            static ValueTask<(PropertyParsingResult PropertyParsingResult, string PropertyName)> ContinueAfterReadAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam)
            {
                // Skip over the instance annotation value
                Task skipValueTask = thisParam.JsonReader.SkipValueAsync();

                if (!skipValueTask.IsCompletedSuccessfully)
                {
                    return AwaitSkipValueAsync(
                        thisParam,
                        propertyAndAnnotationCollectorParam,
                        readPropertyAnnotationValueDelegateParam,
                        skipValueTask);
                }

                return thisParam.ParsePropertyAsync(
                    propertyAndAnnotationCollectorParam,
                    readPropertyAnnotationValueDelegateParam);
            }

            static async ValueTask<(PropertyParsingResult PropertyParsingResult, string PropertyName)> AwaitReadAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                Task<bool> pendingReadTask)
            {
                await pendingReadTask.ConfigureAwait(false);

                return await ContinueAfterReadAsync(
                    thisParam,
                    propertyAndAnnotationCollectorParam,
                    readPropertyAnnotationValueDelegateParam).ConfigureAwait(false);
            }

            static async ValueTask<(PropertyParsingResult PropertyParsingResult, string PropertyName)> AwaitSkipValueAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                Task pendingSkipValueTask)
            {
                await pendingSkipValueTask.ConfigureAwait(false);

                return await thisParam.ParsePropertyAsync(
                    propertyAndAnnotationCollectorParam,
                    readPropertyAnnotationValueDelegateParam).ConfigureAwait(false);
            }

            static async Task AwaitParsePropertyLocalAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                Func<PropertyParsingResult, string, Task> handlePropertyDelegateParam,
                ValueTask<(PropertyParsingResult PropertyParsingResult, string PropertyName)> pendingProcessPropertyLocalTask)
            {
                (PropertyParsingResult propertyParsingResult, string propertyName) = await pendingProcessPropertyLocalTask.ConfigureAwait(false);

                while (true)
                {
                    if (propertyParsingResult != PropertyParsingResult.CustomInstanceAnnotation
                        || !thisParam.ShouldSkipCustomInstanceAnnotation(propertyName))
                    {
                        break;
                    }

                    (propertyParsingResult, propertyName) = await ProcessPropertyLocalAsync(
                        thisParam,
                        propertyAndAnnotationCollectorParam,
                        readPropertyAnnotationValueDelegateParam).ConfigureAwait(false);
                }

                await FinalizeProcessPropertyAsync(
                    thisParam,
                    propertyAndAnnotationCollectorParam,
                    handlePropertyDelegateParam,
                    propertyParsingResult,
                    propertyName).ConfigureAwait(false);
            }

            static async Task FinalizeProcessPropertyAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<PropertyParsingResult, string, Task> handlePropertyDelegateParam,
                PropertyParsingResult propertyParsingResultParam,
                string propertyNameParam)
            {
                await handlePropertyDelegateParam(propertyParsingResultParam, propertyNameParam)
                    .ConfigureAwait(false);
                if (propertyParsingResultParam != PropertyParsingResult.EndOfObject
                    && propertyParsingResultParam != PropertyParsingResult.CustomInstanceAnnotation)
                {
                    propertyAndAnnotationCollectorParam.MarkPropertyAsProcessed(propertyNameParam);
                }
            }

            static async Task AwaitParsePropertyAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                Func<PropertyParsingResult, string, Task> handlePropertyDelegateParam,
                ValueTask<(PropertyParsingResult propertyParsingResult, string propertyName)> pendingParsePropertyTask)
            {
                (PropertyParsingResult propertyParsingResult, string propertyName) = await pendingParsePropertyTask
                    .ConfigureAwait(false);

                await ContinueAfterParsePropertyAsync(
                    thisParam,
                    propertyAndAnnotationCollectorParam,
                    readPropertyAnnotationValueDelegateParam,
                    handlePropertyDelegateParam,
                    propertyParsingResult,
                    propertyName).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously reads the odata.context annotation.
        /// </summary>
        /// <param name="payloadKind">The payload kind for which to read the context URI.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="failOnMissingContextUriAnnotation">true if the method should fail if the context URI annotation is missing, false if that can be ignored.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the context URI annotation.
        /// </returns>
        internal Task<string> ReadContextUriAnnotationAsync(
            ODataPayloadKind payloadKind,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool failOnMissingContextUriAnnotation)
        {
            if (this.JsonReader.NodeType != JsonNodeType.Property)
            {
                if (!failOnMissingContextUriAnnotation || payloadKind == ODataPayloadKind.Unsupported)
                {
                    // Do not fail during payload kind detection
                    return Task.FromResult<string>(null);
                }

                throw new ODataException(SRResources.ODataJsonDeserializer_ContextLinkNotFoundAsFirstProperty);
            }

            // Must make sure the input odata.context has a '@' prefix
            Task<string> getPropertyNameTask = this.JsonReader.GetPropertyNameAsync();
            if (!getPropertyNameTask.IsCompletedSuccessfully)
            {
                return AwaitGetPropertyNameAsync(
                    this,
                    payloadKind,
                    propertyAndAnnotationCollector,
                    failOnMissingContextUriAnnotation,
                    getPropertyNameTask);
            }

            string propertyName = getPropertyNameTask.Result;

            return ContinueAfterGetPropertyNameAsync(this, payloadKind, propertyAndAnnotationCollector, failOnMissingContextUriAnnotation, propertyName);

            static Task<string> ContinueAfterGetPropertyNameAsync(
                ODataJsonDeserializer thisParam,
                ODataPayloadKind payloadKindParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                bool failOnMissingContextUriAnnotationParam,
                string propertyNameParam)
            {
                if (!string.Equals(ODataJsonConstants.PrefixedODataContextPropertyName, propertyNameParam, StringComparison.Ordinal)
                    && !thisParam.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataContextPropertyName, propertyNameParam))
                {
                    if (!failOnMissingContextUriAnnotationParam || payloadKindParam == ODataPayloadKind.Unsupported)
                    {
                        // Do not fail during payload kind detection
                        return Task.FromResult<string>(null);
                    }

                    throw new ODataException(SRResources.ODataJsonDeserializer_ContextLinkNotFoundAsFirstProperty);
                }

                if (propertyAndAnnotationCollectorParam != null)
                {
                    propertyAndAnnotationCollectorParam.MarkPropertyAsProcessed(propertyNameParam);
                }

                // Read over the property name
                Task<JsonNodeType> readNextTask = thisParam.JsonReader.ReadNextAsync();
                if (!readNextTask.IsCompletedSuccessfully)
                {
                    return AwaitReadNextAsync(thisParam.JsonReader, readNextTask);
                }
                
                return thisParam.JsonReader.ReadStringValueAsync();
            }

            static async Task<string> AwaitGetPropertyNameAsync(
                ODataJsonDeserializer thisParam,
                ODataPayloadKind payloadKindParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                bool failOnMissingContextUriAnnotationParam,
                Task<string> pendingGetPropertyNameTask)
            {
                string propertyName = await pendingGetPropertyNameTask.ConfigureAwait(false);

                return await ContinueAfterGetPropertyNameAsync(
                    thisParam,
                    payloadKindParam,
                    propertyAndAnnotationCollectorParam,
                    failOnMissingContextUriAnnotationParam,
                    propertyName);
            }

            static async Task<string> AwaitReadNextAsync(IJsonReader jsonReaderParam, Task<JsonNodeType> pendingReadNextTask)
            {
                await pendingReadNextTask.ConfigureAwait(false);

                return await jsonReaderParam.ReadStringValueAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Compares the JSON property name with the simplified OData annotation property name.
        /// </summary>
        /// <param name="simplifiedPropertyName">The simplified OData annotation property name.</param>
        /// <param name="propertyName">The JSON property name read from the payload.</param>
        /// <returns>If the JSON property name equals the simplified OData annotation property name.</returns>
        protected bool CompareSimplifiedODataAnnotation(string simplifiedPropertyName, string propertyName)
        {
            Debug.Assert(simplifiedPropertyName.IndexOf('@', StringComparison.Ordinal) == 0, "simplifiedPropertyName must start with '@'.");
            Debug.Assert(simplifiedPropertyName.IndexOf('.', StringComparison.Ordinal) == -1, "simplifiedPropertyName must not be namespace-qualified.");

            return this.JsonInputContext.OptionalODataPrefix &&
                   string.Equals(simplifiedPropertyName, propertyName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Completes the simplified OData annotation name with "odata.".
        /// </summary>
        /// <param name="annotationName">The annotation name to be completed.</param>
        /// <returns>The complete OData annotation name.</returns>
        protected string CompleteSimplifiedODataAnnotation(string annotationName)
        {
            if (!this.JsonInputContext.OptionalODataPrefix 
                || annotationName.IndexOf('.', StringComparison.Ordinal) >= 0)
            {
                return annotationName;
            }

            return annotationName switch
            {
                ODataAnnotationNames.ODataTypeNoPrefix => ODataAnnotationNames.ODataType,
                ODataAnnotationNames.ODataContextNoPrefix => ODataAnnotationNames.ODataContext,
                ODataAnnotationNames.ODataIdNoPrefix => ODataAnnotationNames.ODataId,
                ODataAnnotationNames.ODataCountNoPrefix => ODataAnnotationNames.ODataCount,
                ODataAnnotationNames.ODataETagNoPrefix => ODataAnnotationNames.ODataETag,
                ODataAnnotationNames.ODataDeltaNoPrefix => ODataAnnotationNames.ODataDelta,
                ODataAnnotationNames.ODataRemovedNoPrefix => ODataAnnotationNames.ODataRemoved,
                ODataAnnotationNames.ODataNextLinkNoPrefix => ODataAnnotationNames.ODataNextLink,
                ODataAnnotationNames.ODataDeltaLinkNoPrefix => ODataAnnotationNames.ODataDeltaLink,
                ODataAnnotationNames.ODataReadLinkNoPrefix => ODataAnnotationNames.ODataReadLink,
                ODataAnnotationNames.ODataEditLinkNoPrefix => ODataAnnotationNames.ODataEditLink,
                ODataAnnotationNames.ODataBindNoPrefix => ODataAnnotationNames.ODataBind,
                ODataAnnotationNames.ODataNavigationLinkUrlNoPrefix => ODataAnnotationNames.ODataNavigationLinkUrl,
                ODataAnnotationNames.ODataAssociationLinkUrlNoPrefix => ODataAnnotationNames.ODataAssociationLinkUrl,
                ODataAnnotationNames.ODataMediaETagNoPrefix => ODataAnnotationNames.ODataMediaETag,
                ODataAnnotationNames.ODataMediaReadLinkNoPrefix => ODataAnnotationNames.ODataMediaReadLink,
                ODataAnnotationNames.ODataMediaEditLinkNoPrefix => ODataAnnotationNames.ODataMediaEditLink,
                ODataAnnotationNames.ODataMediaContentTypeNoPrefix => ODataAnnotationNames.ODataMediaContentType,
                ODataAnnotationNames.ODataNullNoPrefix => ODataAnnotationNames.ODataNull,
                _ => string.Concat(ODataJsonConstants.ODataAnnotationNamespacePrefix, annotationName)
            };
        }

        /// <summary>
        /// Completes the simplified OData annotation name with "odata.".
        /// </summary>
        /// <param name="annotationName">The annotation name to be completed.</param>
        /// <returns>The complete OData annotation name.</returns>
        protected ReadOnlyMemory<char> CompleteSimplifiedODataAnnotation(ReadOnlyMemory<char> annotationName)
        {
            ReadOnlySpan<char> annotationNameSpan = annotationName.Span;

            if (!this.JsonInputContext.OptionalODataPrefix || annotationNameSpan.IndexOf('.') >= 0)
            {
                // Return the original memory slice without allocating
                return annotationName;
            }

            // Match known no-prefix names using guards
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataTypeNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataType.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataContextNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataContext.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataIdNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataId.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataCountNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataCount.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataETagNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataETag.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataDeltaNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataDelta.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataRemovedNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataRemoved.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataNextLinkNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataNextLink.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataDeltaLinkNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataDeltaLink.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataReadLinkNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataReadLink.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataEditLinkNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataEditLink.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataBindNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataBind.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataNavigationLinkUrlNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataNavigationLinkUrl.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataAssociationLinkUrlNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataAssociationLinkUrl.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataMediaETagNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataMediaETag.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataMediaReadLinkNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataMediaReadLink.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataMediaEditLinkNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataMediaEditLink.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataMediaContentTypeNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataMediaContentType.AsMemory();
            if (annotationNameSpan.SequenceEqual(ODataAnnotationNames.ODataNullNoPrefix.AsSpan()))
                return ODataAnnotationNames.ODataNull.AsMemory();

            // Fallback: allocate once for "odata." + name
            ReadOnlySpan<char> prefix = ODataJsonConstants.ODataAnnotationNamespacePrefix.AsSpan();
            char[] buffer = new char[prefix.Length + annotationNameSpan.Length];
            prefix.CopyTo(buffer);
            annotationNameSpan.CopyTo(buffer.AsSpan(prefix.Length));

            return buffer.AsMemory();
        }

        /// <summary>
        /// Returns true if <paramref name="annotationName"/> should be skipped by the reader; false otherwise.
        /// </summary>
        /// <param name="annotationName">The custom instance annotation name in question.</param>
        /// <returns>Returns true if <paramref name="annotationName"/> should be skipped by the reader; false otherwise.</returns>
        private bool ShouldSkipCustomInstanceAnnotation(string annotationName)
        {
            // By default we always reading custom instance annotation on error payloads.
            if (this is ODataJsonErrorDeserializer && this.MessageReaderSettings.ShouldIncludeAnnotation == null)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInstanceAnnotation(string annotationName)
        {
            if (!string.IsNullOrEmpty(annotationName) && annotationName[0] == ODataJsonConstants.ODataPropertyAnnotationSeparatorChar)
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
        /// <param name="annotationValue">The annotation value that was read.</param>
        /// <returns>Returns true if the annotation name and value is skipped; returns false otherwise.</returns>
        private bool SkipOverUnknownODataAnnotation(string annotationName, out object annotationValue)
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
                annotationValue = this.JsonReader.GetValue();
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
                if (isPropertyAnnotation && string.Equals(this.CompleteSimplifiedODataAnnotation(annotationNameFromReader), ODataAnnotationNames.ODataDelta, StringComparison.Ordinal))
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
                if (parsedPropertyName != null && !string.Equals(parsedPropertyName, propertyNameFromReader, StringComparison.Ordinal))
                {
                    if (ODataJsonReaderUtils.IsAnnotationProperty(parsedPropertyName))
                    {
                        throw new ODataException(Error.Format(SRResources.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue, lastPropertyAnnotationNameFound, parsedPropertyName));
                    }

                    return PropertyParsingResult.PropertyWithoutValue;
                }

                object annotationValue = null;
                if (isPropertyAnnotation)
                {
                    annotationNameFromReader = this.CompleteSimplifiedODataAnnotation(annotationNameFromReader);

                    // If this is a unknown odata annotation targeting a property, we skip over it. See remark on the method SkipOverUnknownODataAnnotation() for detailed explanation.
                    // Note that we don't skip over unknown odata annotations targeting another annotation. We don't allow annotations (except odata.type) targeting other annotations,
                    // so this.ProcessPropertyAnnotation() will test and fail for that case.
                    if (!ODataJsonReaderUtils.IsAnnotationProperty(propertyNameFromReader) && this.SkipOverUnknownODataAnnotation(annotationNameFromReader, out annotationValue))
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

                // If this is a unknown odata annotation, skip over it. See remark on the method SkipOverUnknownODataAnnotation() for detailed explanation.
                if (isInstanceAnnotation && this.SkipOverUnknownODataAnnotation(propertyNameFromReader, out annotationValue))
                {
                    // collect 'odata.<unknown>' annotation:
                    // here we know the original property name contains no '@', but '.' dot
                    Debug.Assert(annotationNameFromReader == null, "annotationNameFromReader == null");
                    propertyAndAnnotationCollector.AddODataScopeAnnotation(propertyNameFromReader, annotationValue);
                    continue;
                }

                // We are encountering the property name for the first time.
                // Don't read over property name, as that would cause the buffering reader to read ahead in the StartObject
                // state, which would break our ability to stream inline json. Instead, callers of ParseProperty will have to
                // call this.JsonReader.Read() as appropriate to read past the property name.
                parsedPropertyName = propertyNameFromReader;

                if (!isInstanceAnnotation && ODataJsonUtils.IsMetadataReferenceProperty(propertyNameFromReader))
                {
                    return PropertyParsingResult.MetadataReferenceProperty;
                }

                if (!isInstanceAnnotation && !ODataJsonReaderUtils.IsAnnotationProperty(propertyNameFromReader))
                {
                    // Normal property
                    return PropertyParsingResult.PropertyWithValue;
                }

                // collect 'xxx.yyyy' annotation:
                // here we know the original property name contains no '@', but '.' dot
                Debug.Assert(annotationNameFromReader == null, "annotationNameFromReader == null");

                // Handle 'odata.XXXXX' annotations
                if (isInstanceAnnotation && ODataJsonReaderUtils.IsODataAnnotationName(propertyNameFromReader))
                {
                    return PropertyParsingResult.ODataInstanceAnnotation;
                }

                // Handle custom annotations
                return PropertyParsingResult.CustomInstanceAnnotation;
            }

            this.AssertJsonCondition(JsonNodeType.EndObject);
            if (parsedPropertyName != null)
            {
                if (ODataJsonReaderUtils.IsAnnotationProperty(parsedPropertyName))
                {
                    throw new ODataException(Error.Format(SRResources.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue, lastPropertyAnnotationNameFound, parsedPropertyName));
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
            if (ODataJsonReaderUtils.IsAnnotationProperty(annotatedPropertyName) && !string.Equals(annotationName, ODataAnnotationNames.ODataType, StringComparison.Ordinal))
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation, annotationName, annotatedPropertyName, ODataAnnotationNames.ODataType));
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
            if (ODataJsonReaderUtils.IsODataAnnotationName(annotationName))
            {
                // OData annotations are read
                propertyAndAnnotationCollector.AddODataPropertyAnnotation(annotatedPropertyName, annotationName, readPropertyAnnotationValue(annotationName));
            }
            else
            {
                if (this.ShouldSkipCustomInstanceAnnotation(annotationName) || (this is ODataJsonErrorDeserializer && this.MessageReaderSettings.ShouldIncludeAnnotation == null))
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
                    bool failOnMissingContextUriAnnotation = this.jsonInputContext.ReadingResponse;

                    // In responses we expect the context URI to be the first thing in the payload
                    // (except for error payloads). In requests we ignore the context URI.
                    return this.ReadContextUriAnnotation(payloadKind, propertyAndAnnotationCollector, failOnMissingContextUriAnnotation);
                }

                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            }

            return null;
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
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1) true if the annotation name and value is skipped; otherwise false.
        /// 2) The annotation value that was read.
        /// </returns>
        private ValueTask<(bool IsUnknownODataAnnotationName, object AnnotationValue)> SkipOverUnknownODataAnnotationAsync(string annotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            this.AssertJsonCondition(JsonNodeType.Property);

            if (!ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName))
            {
                return ValueTask.FromResult((false, (object)null));
            }

            ValueTask<object> readODataOrCustomInstanceAnnotationValueTask = this.ReadODataOrCustomInstanceAnnotationValueAsync();
            if (readODataOrCustomInstanceAnnotationValueTask.IsCompletedSuccessfully)
            {
                return ValueTask.FromResult((true, readODataOrCustomInstanceAnnotationValueTask.Result));
            }

            return AwaitReadODataOrCustomInstanceAnnotationValueAsync(readODataOrCustomInstanceAnnotationValueTask);

            static async ValueTask<(bool, object)> AwaitReadODataOrCustomInstanceAnnotationValueAsync(
                ValueTask<object> pendingReadODataOrCustomInstanceAnnotationValueTask)
            {
                object annotationValue = await pendingReadODataOrCustomInstanceAnnotationValueTask.ConfigureAwait(false);

                return (true,  annotationValue);
            }
        }

        /// <summary>
        /// Asynchronously reads "odata." or custom instance annotation's value.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the annotation value.
        /// </returns>
        private ValueTask<object> ReadODataOrCustomInstanceAnnotationValueAsync()
        {
            Task<bool> readTask = this.JsonReader.ReadAsync();
            if (!readTask.IsCompletedSuccessfully)
            {
                return AwaitReadAsync(this, readTask);
            }

            if (this.JsonReader.NodeType != JsonNodeType.PrimitiveValue)
            {
                Task<ODataValue> readAsUntypedOrNullValueTask = this.JsonReader.ReadAsUntypedOrNullValueAsync();
                if (readAsUntypedOrNullValueTask.IsCompletedSuccessfully)
                {
                    return ValueTask.FromResult<object>(readAsUntypedOrNullValueTask.Result);
                }

                return AwaitReadAsUntypedOrNullValueAsync(readAsUntypedOrNullValueTask);
            }

            Task<object> getValueTask = this.JsonReader.GetValueAsync();
            if (!getValueTask.IsCompletedSuccessfully)
            {
                return AwaitGetValueThenSkipValueAsync(this.JsonReader, getValueTask);
            }

            object annotationValue = getValueTask.Result;
            Task skipValueTask = this.JsonReader.SkipValueAsync();
            if (skipValueTask.IsCompletedSuccessfully)
            {
                return ValueTask.FromResult(annotationValue);
            }

            return AwaitSkipValueAsync(annotationValue, skipValueTask);

            static async ValueTask<object> AwaitReadAsync(ODataJsonDeserializer thisParam, Task<bool> pendingReadTask)
            {
                await pendingReadTask.ConfigureAwait(false);

                if (thisParam.JsonReader.NodeType != JsonNodeType.PrimitiveValue)
                {
                    return await thisParam.JsonReader.ReadAsUntypedOrNullValueAsync().ConfigureAwait(false);
                }

                object annotationValue = await thisParam.JsonReader.GetValueAsync().ConfigureAwait(false);
                await thisParam.JsonReader.SkipValueAsync().ConfigureAwait(false);

                return annotationValue;
            }

            static async ValueTask<object> AwaitReadAsUntypedOrNullValueAsync(Task<ODataValue> pendingReadAsUntypedOrNullValueTask)
            {
                return await pendingReadAsUntypedOrNullValueTask.ConfigureAwait(false);
            }

            static async ValueTask<object> AwaitGetValueThenSkipValueAsync(IJsonReader jsonReaderParam, Task<object> pendingGetValueTask)
            {
                object annotationValue = await pendingGetValueTask.ConfigureAwait(false);
                await jsonReaderParam.SkipValueAsync().ConfigureAwait(false);

                return annotationValue;
            }

            static async ValueTask<object> AwaitSkipValueAsync(object annotationValueParam, Task pendingSkipValueTask)
            {
                await pendingSkipValueTask.ConfigureAwait(false);

                return annotationValueParam;
            }
        }

        /// <summary>
        /// Local classification used by <see cref="ParsePropertyAsync"/> to cheaply categorize a JSON property name.
        /// </summary>
        /// <remarks>
        /// The classification is dictated upon by the presence and position of the '@' character in the property name from the reader"/>
        /// </remarks>
        private enum ParsedPropertyKind
        {
            /// <summary>
            /// Property name contains '@' character in the middle (not at index 0) and not as the last character;
            /// treated as a property annotation: Property@an.notation.
            /// </summary>
            PropertyAnnotation,

            /// <summary>
            /// Property name starts with '@' character; treated as an instance annotation: @an.notation.
            /// </summary>
            InstanceAnnotation,

            /// <summary>
            /// Name has no valid '@' character; treated as a normal property.
            /// </summary>
            NormalProperty
        }

        /// <summary>
        /// Lightweight result for a single parse step used to drive property parsing in <see cref="ParsePropertyAsync"/>.
        /// </summary>
        /// <param name="ShouldContinueParsing"><c>true</c> if parsing should continue; otherwise, <c>false</c>.</param>
        /// <param name="ParsingResult">
        /// The result when parsing has terminated (e.g., PropertyWithValue, PropertyWithoutValue, ODataInstanceAnnotation,
        /// CustomInstanceAnnotation,MetadataReferenceProperty, NestedDeltaResourceSet, EndOfObject).
        /// Ignored when <paramref name="ShouldContinueParsing"/> is <c>true</c>.
        /// </param>
        /// <param name="ParsedPropertyName">
        /// The current property or annotation name associated with <paramref name="ParsingResult"/> or carried forward when continuing.
        /// </param>
        /// <param name="LastPropertyAnnotationNameFound">
        /// The last property annotation name found for the property being parsed.
        /// </param>
        private readonly record struct PropertyParsingLocalResult(
            bool ShouldContinueParsing,
            PropertyParsingResult ParsingResult,
            ReadOnlyMemory<char> ParsedPropertyName,
            ReadOnlyMemory<char> LastPropertyAnnotationNameFound);

        /// <summary>
        /// Asynchronously parses JSON object property starting with the current position of the JSON reader.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use, it will also store the property annotations found.</param>
        /// <param name="readPropertyAnnotationValueDelegate">Delegate called to read property annotation value.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple containing:
        /// 1). The first component contains PropertyWithValue if a property with value was found, while the second component contains the name of the property.
        ///                             The reader is positioned on the property value.
        /// 2). The first component contains PropertyWithoutValue if a property without a value was found, while the second component contains the name of the property.
        ///                             The reader is positioned on the node after property annotations (so either a property or end of object).
        /// 3). The first component contains ODataInstanceAnnotation if an odata instance annotation was found, while the second component contains the name of the annotation.
        ///                             The reader is positioned on the value of the annotation.
        /// 4). The first component contains CustomInstanceAnnotation if a custom instance annotation was found, while the second component contains name of the annotation.
        ///                             The reader is positioned on the value of the annotation.
        /// 5). The first component contains MetadataReferenceProperty if a property which is a reference into the metadata was found, while the second component contains the name of the property.
        ///                             The reader is positioned on the value of the property.
        /// 6). The first component contains NestedDeltaResourceSet if a property representing a nested delta resource set was found, while the second component contains the name of the property.
        ///                             The reader is positioned on the array value of the property.
        /// 7). The first component contains EndOfObject if end of the object scope was reached and no properties are to be reported, while the second component contains null.
        ///                             This can only happen if there's a property annotation which is ignored (for example custom one) at the end of the object.
        /// </returns>
        private ValueTask<(PropertyParsingResult ParsingResult, string PropertyName)> ParsePropertyAsync(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            Func<string, Task<object>> readPropertyAnnotationValueDelegate)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, $"{nameof(propertyAndAnnotationCollector)} != null");
            Debug.Assert(readPropertyAnnotationValueDelegate != null, $"{nameof(readPropertyAnnotationValueDelegate)} != null");

            ReadOnlyMemory<char> lastPropertyAnnotationNameFound = ReadOnlyMemory<char>.Empty;
            ReadOnlyMemory<char> parsedPropertyName = ReadOnlyMemory<char>.Empty;

            while (true)
            {
                if (this.JsonReader.NodeType != JsonNodeType.Property)
                {
                    break;
                }

                ValueTask<PropertyParsingLocalResult> parsePropertyLocalTask = ParsePropertyLocalAsync(
                    this,
                    propertyAndAnnotationCollector,
                    readPropertyAnnotationValueDelegate,
                    parsedPropertyName,
                    lastPropertyAnnotationNameFound);

                if (!parsePropertyLocalTask.IsCompletedSuccessfully)
                {
                    return ContinueAfterParsePropertyLocalAsync(
                        this,
                        propertyAndAnnotationCollector,
                        readPropertyAnnotationValueDelegate,
                        parsedPropertyName,
                        lastPropertyAnnotationNameFound,
                        parsePropertyLocalTask);
                }

                PropertyParsingLocalResult propertyParsingLocalResult = parsePropertyLocalTask.Result;

                if (propertyParsingLocalResult.ShouldContinueParsing)
                {
                    lastPropertyAnnotationNameFound = propertyParsingLocalResult.LastPropertyAnnotationNameFound;
                    parsedPropertyName = propertyParsingLocalResult.ParsedPropertyName;
                    continue;
                }

                Debug.Assert(propertyParsingLocalResult.ParsingResult != default,
                    $"{nameof(propertyParsingLocalResult.ParsingResult)} != default");
                Debug.Assert(!propertyParsingLocalResult.ParsedPropertyName.IsEmpty,
                    $"!{nameof(propertyParsingLocalResult.ParsedPropertyName)}.IsEmpty");
                return ValueTask.FromResult((propertyParsingLocalResult.ParsingResult, propertyParsingLocalResult.ParsedPropertyName.GetString()));
            }

            return FinalizeParseProperty(this, parsedPropertyName, lastPropertyAnnotationNameFound);

            static async ValueTask<(PropertyParsingResult, string)> ContinueAfterParsePropertyLocalAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam,
                ValueTask<PropertyParsingLocalResult> pendingParsePropertyLocalTask)
            {
                PropertyParsingLocalResult propertyParsingLocalResult = await pendingParsePropertyLocalTask.ConfigureAwait(false);

                while (true)
                {
                    if (propertyParsingLocalResult.ShouldContinueParsing)
                    {
                        lastPropertyAnnotationNameFoundParam = propertyParsingLocalResult.LastPropertyAnnotationNameFound;
                        parsedPropertyNameParam = propertyParsingLocalResult.ParsedPropertyName;
                        
                        if (thisParam.JsonReader.NodeType != JsonNodeType.Property)
                        {
                            break;
                        }

                        propertyParsingLocalResult = await ParsePropertyLocalAsync(
                            thisParam,
                            propertyAndAnnotationCollectorParam,
                            readPropertyAnnotationValueDelegateParam,
                            parsedPropertyNameParam,
                            lastPropertyAnnotationNameFoundParam).ConfigureAwait(false);

                        continue;
                    }

                    Debug.Assert(propertyParsingLocalResult.ParsingResult != default,
                        $"{nameof(propertyParsingLocalResult.ParsingResult)} != default");
                    Debug.Assert(!propertyParsingLocalResult.ParsedPropertyName.IsEmpty,
                        $"!{nameof(propertyParsingLocalResult.ParsedPropertyName)}.IsEmpty");
                    return (propertyParsingLocalResult.ParsingResult, propertyParsingLocalResult.ParsedPropertyName.GetString());
                }

                return await FinalizeParseProperty(thisParam, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam);
            }

            static ValueTask<PropertyParsingLocalResult> ParsePropertyLocalAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam)
            {
                Task<string> getPropertyNameTask = thisParam.JsonReader.GetPropertyNameAsync();

                if (!getPropertyNameTask.IsCompletedSuccessfully)
                {
                    return AwaitGetPropertyNameAsync(
                        thisParam,
                        propertyAndAnnotationCollectorParam,
                        readPropertyAnnotationValueDelegateParam,
                        parsedPropertyNameParam,
                        lastPropertyAnnotationNameFoundParam,
                        getPropertyNameTask);
                }

                string nameFromReader = getPropertyNameTask.Result;

                return ContinueAfterGetPropertyNameAsync(
                    thisParam,
                    propertyAndAnnotationCollectorParam,
                    readPropertyAnnotationValueDelegateParam,
                    parsedPropertyNameParam,
                    lastPropertyAnnotationNameFoundParam,
                    nameFromReader);
            }

            static async ValueTask<PropertyParsingLocalResult> AwaitGetPropertyNameAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam,
                Task<string> pendingGetPropertyNameTask)
            {
                string nameFromReader = await pendingGetPropertyNameTask.ConfigureAwait(false);

                return await ContinueAfterGetPropertyNameAsync(
                    thisParam,
                    propertyAndAnnotationCollectorParam,
                    readPropertyAnnotationValueDelegateParam,
                    parsedPropertyNameParam,
                    lastPropertyAnnotationNameFoundParam,
                    nameFromReader).ConfigureAwait(false);
            }

            static ValueTask<PropertyParsingLocalResult> ContinueAfterGetPropertyNameAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam,
                string nameFromReaderParam)
            {
                // Most payload properties are not annotations; optimize for that case
                int annotationSeparatorIndex = nameFromReaderParam.IndexOf(
                    ODataJsonConstants.ODataPropertyAnnotationSeparatorChar,
                    StringComparison.Ordinal);
                ParsedPropertyKind parsedPropertyKind = ParsePropertyKind(nameFromReaderParam, annotationSeparatorIndex);
                (ReadOnlyMemory<char> propertyNameFromReaderMem, ReadOnlyMemory<char> annotationNameFromReaderMem, ReadOnlyMemory<char> prefixedAnnotationNameMem) = ParseNameFromReader(
                    thisParam,
                    nameFromReaderParam.AsMemory(),
                    annotationSeparatorIndex,
                    parsedPropertyKind);

                // Reading a nested delta resource set
                if (parsedPropertyKind == ParsedPropertyKind.PropertyAnnotation && prefixedAnnotationNameMem.Span.SequenceEqual(ODataAnnotationNames.ODataDelta.AsSpan()))
                {
                    return HandleNestedDeltaResourceSetAsync(
                        thisParam,
                        propertyAndAnnotationCollectorParam,
                        readPropertyAnnotationValueDelegateParam,
                        parsedPropertyNameParam,
                        lastPropertyAnnotationNameFoundParam,
                        propertyNameFromReaderMem);
                }

                // If parsedPropertyName is set and is different from the property name the reader is currently on,
                // we have parsed a property annotation for a different property than the one at the current position.
                if (!parsedPropertyNameParam.IsEmpty && !parsedPropertyNameParam.Span.SequenceEqual(propertyNameFromReaderMem.Span))
                {
                    if (ODataJsonReaderUtils.IsAnnotationProperty(parsedPropertyNameParam.Span))
                    {
                        return ValueTask.FromException<PropertyParsingLocalResult>(
                            new ODataException(Error.Format(
                                SRResources.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue,
                                lastPropertyAnnotationNameFoundParam.GetString(),
                                parsedPropertyNameParam.GetString())));
                    }

                    return ValueTask.FromResult(
                        new PropertyParsingLocalResult(false, PropertyParsingResult.PropertyWithoutValue, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam));
                }

                if (parsedPropertyKind == ParsedPropertyKind.PropertyAnnotation)
                {
                    return HandlePropertyAnnotationAsync(
                        thisParam,
                        propertyAndAnnotationCollectorParam,
                        readPropertyAnnotationValueDelegateParam,
                        parsedPropertyNameParam,
                        lastPropertyAnnotationNameFoundParam,
                        propertyNameFromReaderMem,
                        annotationNameFromReaderMem,
                        prefixedAnnotationNameMem);
                }
                else if (parsedPropertyKind == ParsedPropertyKind.InstanceAnnotation)
                {
                    return HandleInstanceAnnotationAsync(
                        thisParam,
                        propertyAndAnnotationCollectorParam,
                        readPropertyAnnotationValueDelegateParam,
                        parsedPropertyNameParam,
                        lastPropertyAnnotationNameFoundParam,
                        propertyNameFromReaderMem,
                        annotationNameFromReaderMem);
                }
                else
                {
                    // We are encountering the property name for the first time.
                    // Don't read over property name, as that would cause the buffering reader to read ahead in the StartObject
                    // state, which would break our ability to stream inline json. Instead, callers of ParsePropertyAsync will have to
                    // call this.JsonReader.ReadAsync() as appropriate to read past the property name.
                    parsedPropertyNameParam = propertyNameFromReaderMem;

                    if (ODataJsonUtils.IsMetadataReferenceProperty(propertyNameFromReaderMem.Span))
                    {
                        return ValueTask.FromResult(
                            new PropertyParsingLocalResult(false, PropertyParsingResult.MetadataReferenceProperty, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam));
                    }

                    if (!ODataJsonReaderUtils.IsAnnotationProperty(propertyNameFromReaderMem.Span))
                    {
                        // Normal property
                        return ValueTask.FromResult(
                            new PropertyParsingLocalResult(false, PropertyParsingResult.PropertyWithValue, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam));
                    }
                }

                return ValueTask.FromResult(
                    HandleCustomInstanceAnnotation(parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam, annotationNameFromReaderMem));
            }

            static ParsedPropertyKind ParsePropertyKind(ReadOnlySpan<char> nameFromReaderParam, int annotationSeparatorIndexParam)
            {
                // Most payload properties are not annotations; optimize for that case
                // TODO: Verify behaviour for odata.type or custom.annotation (i.e., missing @)
                if (annotationSeparatorIndexParam == 0)
                {
                    return ParsedPropertyKind.InstanceAnnotation;
                }

                if (annotationSeparatorIndexParam > 0 && annotationSeparatorIndexParam != nameFromReaderParam.Length - 1)
                {
                    return ParsedPropertyKind.PropertyAnnotation;
                }

                // If there's no '@' anywhere in the name, it's either:
                // - a normal property (most common) -> PropertyWithValue
                // - a metadata reference property -> MetadataReferenceProperty
                return ParsedPropertyKind.NormalProperty;
            }

            static (ReadOnlyMemory<char> propertyName, ReadOnlyMemory<char> annotationName, ReadOnlyMemory<char> prefixedAnnotationName) ParseNameFromReader(
                ODataJsonDeserializer thisParam,
                ReadOnlyMemory<char> nameFromReaderParam,
                int annotationSeparatorIndexParam,
                ParsedPropertyKind parsedPropertyKindParam)
            {
                switch (parsedPropertyKindParam)
                {
                    case ParsedPropertyKind.PropertyAnnotation:
                        {
                            ReadOnlyMemory<char> propertyName = nameFromReaderParam[..annotationSeparatorIndexParam];
                            ReadOnlyMemory<char> annotationName = nameFromReaderParam[(annotationSeparatorIndexParam + 1)..];
                            ReadOnlyMemory<char> prefixedAnnotationName = thisParam.CompleteSimplifiedODataAnnotation(annotationName);

                            return (propertyName, annotationName, prefixedAnnotationName);
                        }

                    case ParsedPropertyKind.InstanceAnnotation:
                        {
                            // If this is not a property annotation, determine whether it's an instance annotation, i.e. starts with @ prefix
                            // If we find that it's an instance annotation and OptionalODataPrefix setting is set to true, complete the simplified
                            // annotation (if necessary) by prepending it with "odata."
                            ReadOnlyMemory<char> propertyNameMem = thisParam.CompleteSimplifiedODataAnnotation(nameFromReaderParam[1..]);

                            return (propertyNameMem, ReadOnlyMemory<char>.Empty, ReadOnlyMemory<char>.Empty);
                        }

                    default:
                        return (nameFromReaderParam, ReadOnlyMemory<char>.Empty, ReadOnlyMemory<char>.Empty);
                }
            }

            static async ValueTask<PropertyParsingLocalResult> HandleNestedDeltaResourceSetAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam,
                ReadOnlyMemory<char> propertyNameFromReaderParam)
            {
                // Read over the property name.
                await thisParam.JsonReader.ReadAsync()
                    .ConfigureAwait(false);
                parsedPropertyNameParam = propertyNameFromReaderParam;
                
                return new PropertyParsingLocalResult(false, PropertyParsingResult.NestedDeltaResourceSet, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam);
            }

            static async ValueTask<PropertyParsingLocalResult> HandlePropertyAnnotationAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam,
                ReadOnlyMemory<char> propertyNameFromReaderParam,
                ReadOnlyMemory<char> annotationNameFromReaderParam,
                ReadOnlyMemory<char> prefixedAnnotationNameParam)
            {
                object annotationValue = null;
                // If this is a unknown odata annotation targeting a property, we skip over it.
                // See remark on the method SkipOverUnknownODataAnnotationAsync() for detailed explanation.
                // Note that we don't skip over unknown odata annotations targeting another annotation.
                // We don't allow annotations (except odata.type) targeting other annotations,
                // so ProcessPropertyAnnotationAsync() will test and fail for that case.
                if (!ODataJsonReaderUtils.IsAnnotationProperty(propertyNameFromReaderParam.Span))
                {
                    string prefixedAnnotationName = prefixedAnnotationNameParam.GetString();
                    (bool isUnknownODataAnnotationName, object tempAnnotationValue) = await thisParam.SkipOverUnknownODataAnnotationAsync(
                        prefixedAnnotationName).ConfigureAwait(false);
                    if (isUnknownODataAnnotationName)
                    {
                        string propertyNameFromReader = propertyNameFromReaderParam.GetString();
                        annotationValue = tempAnnotationValue;
                        propertyAndAnnotationCollectorParam.AddODataPropertyAnnotation(propertyNameFromReader, prefixedAnnotationName, annotationValue);
                        return new PropertyParsingLocalResult(true, default, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam);
                    }
                }

                // We have another property annotation for the same property we parsed.
                parsedPropertyNameParam = propertyNameFromReaderParam;
                lastPropertyAnnotationNameFoundParam = prefixedAnnotationNameParam;

                await thisParam.ProcessPropertyAnnotationAsync(
                    propertyNameFromReaderParam.Span,
                    prefixedAnnotationNameParam.Span,
                    propertyAndAnnotationCollectorParam,
                    readPropertyAnnotationValueDelegateParam).ConfigureAwait(false);
                return new PropertyParsingLocalResult(true, default, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam);
            }

            static async ValueTask<PropertyParsingLocalResult> HandleInstanceAnnotationAsync(
                ODataJsonDeserializer thisParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam,
                ReadOnlyMemory<char> propertyNameFromReaderParam,
                ReadOnlyMemory<char> annotationNameFromReaderParam)
            {
                object annotationValue = null;
                string propertyNameFromReader = propertyNameFromReaderParam.GetString();
                (bool isUnknownODataAnnotationName, object tempAnnotationValue) = await thisParam.SkipOverUnknownODataAnnotationAsync(
                    propertyNameFromReader).ConfigureAwait(false);
                if (isUnknownODataAnnotationName)
                {
                    annotationValue = tempAnnotationValue;
                    // collect 'odata.<unknown>' annotation:
                    // here we know the original property name contains no '@', but '.' dot
                    Debug.Assert(annotationNameFromReaderParam.IsEmpty, $"{nameof(annotationNameFromReaderParam)}.IsEmpty");
                    propertyAndAnnotationCollectorParam.AddODataScopeAnnotation(propertyNameFromReader, annotationValue);
                    return new PropertyParsingLocalResult(true, default, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam);
                }

                // We are encountering the property name for the first time.
                // Don't read over property name, as that would cause the buffering reader to read ahead in the StartObject
                // state, which would break our ability to stream inline json. Instead, callers of ParsePropertyAsync will have to
                // call this.JsonReader.ReadAsync() as appropriate to read past the property name.
                parsedPropertyNameParam = propertyNameFromReaderParam;

                // collect 'xxx.yyyy' annotation:
                // here we know the original property name contains no '@', but '.' dot
                Debug.Assert(annotationNameFromReaderParam.IsEmpty, $"{nameof(annotationNameFromReaderParam)}.IsEmpty");

                // Handle 'odata.XXXXX' annotations
                if (ODataJsonReaderUtils.IsODataAnnotationName(propertyNameFromReaderParam.Span))
                {
                    return new PropertyParsingLocalResult(false, PropertyParsingResult.ODataInstanceAnnotation, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam);
                }

                return HandleCustomInstanceAnnotation(parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam, annotationNameFromReaderParam);
            }

            static PropertyParsingLocalResult HandleCustomInstanceAnnotation(
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam,
                ReadOnlyMemory<char> annotationNameFromReaderParam)
            {
                // collect 'xxx.yyyy' annotation:
                // here we know the original property name contains no '@', but '.' dot
                Debug.Assert(annotationNameFromReaderParam.IsEmpty, $"{nameof(annotationNameFromReaderParam)}.IsEmpty");

                // We'd only end up here for "if (isInstanceAnnotation)" and "else" cases
                // Handle custom annotations
                return new PropertyParsingLocalResult(false, PropertyParsingResult.CustomInstanceAnnotation, parsedPropertyNameParam, lastPropertyAnnotationNameFoundParam);
            }

            static ValueTask<(PropertyParsingResult, string)> FinalizeParseProperty(
                ODataJsonDeserializer thisParam,
                ReadOnlyMemory<char> parsedPropertyNameParam,
                ReadOnlyMemory<char> lastPropertyAnnotationNameFoundParam)
            {
                thisParam.AssertJsonCondition(JsonNodeType.EndObject);

                string parsedPropertyName = parsedPropertyNameParam.IsEmpty ? null : parsedPropertyNameParam.GetString();
                if (!parsedPropertyNameParam.IsEmpty)
                {
                    if (ODataJsonReaderUtils.IsAnnotationProperty(parsedPropertyNameParam.Span))
                    {
                        string lastPropertyAnnotationNameFound = lastPropertyAnnotationNameFoundParam.GetString();
                        return ValueTask.FromException<(PropertyParsingResult, string)>(new ODataException(
                            Error.Format(SRResources.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue,
                                lastPropertyAnnotationNameFound,
                                parsedPropertyName)));
                    }

                    return ValueTask.FromResult((PropertyParsingResult.PropertyWithoutValue, parsedPropertyName));
                }

                return ValueTask.FromResult((PropertyParsingResult.EndOfObject, parsedPropertyName));
            }
        }

        /// <summary>
        /// Asynchronously process the current property annotation.
        /// </summary>
        /// <param name="annotatedPropertyNameSpan">The name being annotated. Can be a property or an instance annotation.</param>
        /// <param name="annotationNameSpan">The annotation targeting the <paramref name="annotatedPropertyNameSpan"/>.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="readPropertyAnnotationValueDelegate">Delegate to read the property annotation value.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private ValueTask ProcessPropertyAnnotationAsync(
            ReadOnlySpan<char> annotatedPropertyNameSpan,
            ReadOnlySpan<char> annotationNameSpan,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            Func<string, Task<object>> readPropertyAnnotationValueDelegate)
        {
            string annotatedPropertyName = annotatedPropertyNameSpan.ToString();
            string annotationName = annotationNameSpan.ToString();

            // We don't currently support annotation targeting an instance annotation except for the @odata.type property annotation.
            if (ODataJsonReaderUtils.IsAnnotationProperty(annotatedPropertyName)
                && !string.Equals(annotationName, ODataAnnotationNames.ODataType, StringComparison.Ordinal))
            {
                return ValueTask.FromException(
                    new ODataException(Error.Format(SRResources.ODataJsonDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation,
                        annotationName,
                        annotatedPropertyName,
                        ODataAnnotationNames.ODataType)));
            }

            return ReadODataOrCustomInstanceAnnotationValueAsync(
                annotatedPropertyName,
                annotationName,
                propertyAndAnnotationCollector,
                readPropertyAnnotationValueDelegate);
        }

        /// <summary>
        /// Asynchronously reads the end of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. a resource, a resource set or a collection within a parameters payload.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  any node:                when reading response or a nested payload, will fail if find anything else then EndObject.
        ///                 JsonNodeType.EndOfInput: otherwise
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isReadingNestedPayload", Justification = "The parameter is used in debug builds.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal Task ReadPayloadEndAsync(bool isReadingNestedPayload)
        {
            Debug.Assert(
                isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.EndOfInput,
                "Pre-Condition: JsonNodeType.EndOfInput if not reading a nested payload.");
            this.JsonReader.AssertNotBuffering();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="annotatedPropertyName">The property name.</param>
        /// <param name="annotationName">The annotation name</param>
        /// <param name="propertyAndAnnotationCollector">The PropertyAndAnnotationCollector.</param>
        /// <param name="readPropertyAnnotationValueDelegate">Delegate to read the property annotation value.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private ValueTask ReadODataOrCustomInstanceAnnotationValueAsync(
            string annotatedPropertyName,
            string annotationName,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            Func<string, Task<object>> readPropertyAnnotationValueDelegate)
        {
            // Read over the property name.
            Task<bool> readTask = this.JsonReader.ReadAsync();

            if (!readTask.IsCompletedSuccessfully)
            {
                return AwaitReadAsync(this, annotatedPropertyName, annotationName, propertyAndAnnotationCollector, readPropertyAnnotationValueDelegate, readTask);
            }

            return ContinueAfterReadAsync(this, annotatedPropertyName, annotationName, propertyAndAnnotationCollector, readPropertyAnnotationValueDelegate);

            static ValueTask ContinueAfterReadAsync(
                ODataJsonDeserializer thisParam,
                string annotatedPropertyNameParam,
                string annotationNameParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam)
            {
                if (ODataJsonReaderUtils.IsODataAnnotationName(annotationNameParam))
                {
                    // OData annotations are read
                    Task<object> readPropertyAnnotationValueTask = readPropertyAnnotationValueDelegateParam(annotationNameParam);
                    if (!readPropertyAnnotationValueTask.IsCompletedSuccessfully)
                    {
                        return AwaitReadPropertyAnnotationValueDelegate(
                            annotatedPropertyNameParam,
                            annotationNameParam,
                            propertyAndAnnotationCollectorParam,
                            readPropertyAnnotationValueTask);
                    }

                    propertyAndAnnotationCollectorParam.AddODataPropertyAnnotation(annotatedPropertyNameParam, annotationNameParam, readPropertyAnnotationValueTask.Result);

                    return ValueTask.CompletedTask;
                }

                if (thisParam.ShouldSkipCustomInstanceAnnotation(annotationNameParam)
                    || (thisParam is ODataJsonErrorDeserializer && thisParam.MessageReaderSettings.ShouldIncludeAnnotation == null))
                {
                    propertyAndAnnotationCollectorParam.CheckIfPropertyOpenForAnnotations(annotatedPropertyNameParam, annotationNameParam);
                    Task skipValueTask = thisParam.JsonReader.SkipValueAsync();
                    if (!skipValueTask.IsCompletedSuccessfully)
                    {
                        return AwaitSkipValueAsync(skipValueTask);
                    }

                    return ValueTask.CompletedTask;
                }

                Debug.Assert(thisParam.ReadPropertyCustomAnnotationValueAsync != null, $"{nameof(ReadPropertyCustomAnnotationValueAsync)} != null");
                Task<object> readPropertyCustomAnnotationValueTask = thisParam.ReadPropertyCustomAnnotationValueAsync(
                    propertyAndAnnotationCollectorParam,
                    annotationNameParam);
                if (!readPropertyCustomAnnotationValueTask.IsCompletedSuccessfully)
                {
                    return AwaitReadPropertyCustomAnnotationValueAsync(annotatedPropertyNameParam, annotationNameParam, propertyAndAnnotationCollectorParam, readPropertyCustomAnnotationValueTask);
                }

                propertyAndAnnotationCollectorParam.AddCustomPropertyAnnotation(annotatedPropertyNameParam, annotationNameParam, readPropertyCustomAnnotationValueTask.Result);

                return ValueTask.CompletedTask;
            }

            static async ValueTask AwaitReadAsync(
                ODataJsonDeserializer thisParam,
                string annotatedPropertyNameParam,
                string annotationNameParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Func<string, Task<object>> readPropertyAnnotationValueDelegateParam,
                Task<bool> pendingReadTask)
            {
                await pendingReadTask.ConfigureAwait(false);

                await ContinueAfterReadAsync(
                    thisParam,
                    annotatedPropertyNameParam,
                    annotationNameParam,
                    propertyAndAnnotationCollectorParam,
                    readPropertyAnnotationValueDelegateParam);
            }

            static async ValueTask AwaitReadPropertyAnnotationValueDelegate(
                string annotatedPropertyNameParam,
                string annotationNameParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Task<object> pendingReadPropertyAnnotationValueTask)
            {
                object propertyAnnotationValue = await pendingReadPropertyAnnotationValueTask.ConfigureAwait(false);

                propertyAndAnnotationCollectorParam.AddODataPropertyAnnotation(annotatedPropertyNameParam, annotationNameParam, propertyAnnotationValue);
            }

            static async ValueTask AwaitReadPropertyCustomAnnotationValueAsync(
                string annotatedPropertyNameParam,
                string annotationNameParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                Task<object> pendingReadPropertyCustomAnnotationValueTask)
            {
                object propertyCustomAnnotationValue = await pendingReadPropertyCustomAnnotationValueTask.ConfigureAwait(false);
                
                propertyAndAnnotationCollectorParam.AddCustomPropertyAnnotation(annotatedPropertyNameParam, annotationNameParam, propertyCustomAnnotationValue);
            }

            static async ValueTask AwaitSkipValueAsync(Task pendingSkipValueTask)
            {
                await pendingSkipValueTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="payloadKind">The kind of payload we are reading; this guides the parsing of the context URI.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload,
        /// e.g. a resource, a resource set or a collection within a parameters payload.</param>
        /// <param name="allowEmptyPayload">true if we allow a completely empty payload; otherwise false.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the context URI annotation (or null if it was not found).
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      Assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first property of the payload after having read (or skipped) the context URI property.
        ///                 Or the reader is positioned on an end-object node if there are no properties (other than the context URI which is required in responses and optional in requests).
        /// </remarks>
        private Task<string> ReadPayloadStartImplementationAsync(
            ODataPayloadKind payloadKind,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool isReadingNestedPayload,
            bool allowEmptyPayload)
        {
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None,
                "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            if (isReadingNestedPayload)
            {
                return CachedTasks.StringNull;
            }

            // Position the reader on the first node inside the outermost object.
            Task<bool> readTask = this.JsonReader.ReadAsync();
            if (!readTask.IsCompletedSuccessfully)
            {
                return AwaitReadAsync(this, payloadKind, propertyAndAnnotationCollector, allowEmptyPayload, readTask);
            }

            return ContinueAfterReadAsync(this, payloadKind, propertyAndAnnotationCollector, allowEmptyPayload);

            static Task<string> ContinueAfterReadAsync(
                ODataJsonDeserializer thisParam,
                ODataPayloadKind payloadKindParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                bool allowEmptyPayloadParam)
            {
                if (allowEmptyPayloadParam && thisParam.JsonReader.NodeType == JsonNodeType.EndOfInput)
                {
                    return CachedTasks.StringNull;
                }

                // Read the StartObject node and position the reader on the first property
                // (or the end object node).
                ValueTask readStartObjectTask = thisParam.JsonReader.ReadStartObjectAsync();
                if (!readStartObjectTask.IsCompletedSuccessfully)
                {
                    return AwaitReadStartObjectAsync(
                        thisParam,
                        payloadKindParam,
                        propertyAndAnnotationCollectorParam,
                        readStartObjectTask);
                }

                return ContinueAfterReadStartObjectAsync(
                    thisParam,
                    payloadKindParam,
                    propertyAndAnnotationCollectorParam);
            }

            static Task<string> ContinueAfterReadStartObjectAsync(ODataJsonDeserializer thisParam,
                ODataPayloadKind payloadKindParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam)
            {
                if (payloadKindParam == ODataPayloadKind.Error)
                {
                    thisParam.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
                    return CachedTasks.StringNull;
                }

                // Skip over the context URI annotation in request payloads or when we've already read it
                // during payload kind detection.
                bool failOnMissingContextUriAnnotation = thisParam.jsonInputContext.ReadingResponse;

                // In responses we expect the context URI to be the first thing in the payload
                // (except for error payloads). In requests we ignore the context URI.
                return thisParam.ReadContextUriAnnotationAsync(
                    payloadKindParam,
                    propertyAndAnnotationCollectorParam,
                    failOnMissingContextUriAnnotation);

            }

            static async Task<string> AwaitReadAsync(
                ODataJsonDeserializer thisParam,
                ODataPayloadKind payloadKindParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                bool allowEmptyPayloadParam,
                Task<bool> pendingReadTask)
            {
                await pendingReadTask.ConfigureAwait(false);

                return await ContinueAfterReadAsync(thisParam, payloadKindParam, propertyAndAnnotationCollectorParam, allowEmptyPayloadParam);
            }

            static async Task<string> AwaitReadStartObjectAsync(
                ODataJsonDeserializer thisParam,
                ODataPayloadKind payloadKindParam,
                PropertyAndAnnotationCollector propertyAndAnnotationCollectorParam,
                ValueTask pendingStartObjectTask)
            {
                await pendingStartObjectTask.ConfigureAwait(false);

                return await ContinueAfterReadStartObjectAsync(
                    thisParam,
                    payloadKindParam,
                    propertyAndAnnotationCollectorParam);
            }
        }
    }
}
