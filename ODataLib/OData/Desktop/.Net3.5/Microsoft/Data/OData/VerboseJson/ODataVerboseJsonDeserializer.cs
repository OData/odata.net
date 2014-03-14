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

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData Verbose JSON deserializers.
    /// </summary>
    internal abstract class ODataVerboseJsonDeserializer : ODataDeserializer
    {
        /// <summary>The JSON input context to use for reading.</summary>
        private readonly ODataVerboseJsonInputContext jsonInputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The JSON input context to read from.</param>
        protected ODataVerboseJsonDeserializer(ODataVerboseJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
            Debug.Assert(jsonInputContext != null, "jsonInputContext != null");

            this.jsonInputContext = jsonInputContext;
        }

        /// <summary>
        /// Returns the <see cref="BufferingJsonReader"/> which is to be used to read the content of the message.
        /// </summary>
        internal BufferingJsonReader JsonReader
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.jsonInputContext.JsonReader;
            }
        }

        /// <summary>
        /// The Verbose Json input context to use for reading.
        /// </summary>
        protected ODataVerboseJsonInputContext VerboseJsonInputContext
        {
            get { return this.jsonInputContext; }
        }

        /// <summary>
        /// Read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. an entry, a feed or a collection within a parameters payload.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first node of the payload (this can be the first node or the value of the 'd' property node)
        /// </remarks>
        internal void ReadPayloadStart(bool isReadingNestedPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            this.ReadPayloadStart(isReadingNestedPayload, /*expectResponseWrapper*/ true);
        }

        /// <summary>
        /// Read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. an entry, a feed or a collection within a parameters payload.</param>
        /// <param name="expectResponseWrapper">true if the response "d" wrapper should be in the payload, false otherwise.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: The reader is positioned on the first node of the payload (this can be the first node or the value of the 'd' property node)
        /// </remarks>
        internal void ReadPayloadStart(bool isReadingNestedPayload, bool expectResponseWrapper)
        {
            DebugUtils.CheckNoExternalCallers();
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: JSON reader must not have been used yet when not reading a nested payload.");

            if (!isReadingNestedPayload)
            {
                // position the reader on the first node
                this.JsonReader.Read();
            }

            if (this.ReadingResponse && expectResponseWrapper)
            {
                // when reading a response the payload should we wrapped in { "d" : ... }
                this.JsonReader.ReadStartObject();

                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string propertyName = this.JsonReader.ReadPropertyName();

                    if (string.CompareOrdinal(JsonConstants.ODataDataWrapperPropertyName, propertyName) == 0)
                    {
                        break;
                    }

                    // Skip all unrecognized properties
                    this.JsonReader.SkipValue();
                }

                if (this.JsonReader.NodeType == JsonNodeType.EndObject)
                {
                    throw new ODataException(Strings.ODataJsonDeserializer_DataWrapperPropertyNotFound);
                }
            }
        }

        /// <summary>
        /// Reads the end of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. an entry, a feed or a collection within a parameters payload.</param>
        /// <remarks>
        /// Pre-Condition:  any node:                when reading response or a nested payload, will fail if find anything else then EndObject.
        ///                 JsonNodeType.EndOfInput: otherwise
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        internal void ReadPayloadEnd(bool isReadingNestedPayload)
        {
            DebugUtils.CheckNoExternalCallers();
            this.ReadPayloadEnd(isReadingNestedPayload, /*expectResponseWrapper*/ true);
        }

        /// <summary>
        /// Reads the end of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <param name="isReadingNestedPayload">true if we are deserializing a nested payload, e.g. an entry, a feed or a collection within a parameters payload.</param>
        /// <param name="expectResponseWrapper">true if the response "d" wrapper should be in the payload, false otherwise.</param>
        /// <remarks>
        /// Pre-Condition:  any node:                when reading response or a nested payload, will fail if find anything else then EndObject.
        ///                 JsonNodeType.EndOfInput: otherwise
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isReadingNestedPayload", Justification = "The parameter is used in debug builds.")]
        internal void ReadPayloadEnd(bool isReadingNestedPayload, bool expectResponseWrapper)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                isReadingNestedPayload || this.ReadingResponse || this.JsonReader.NodeType == JsonNodeType.EndOfInput,
                "Pre-Condition: JsonNodeType.EndOfInput if not reading a response and not reading a nested payload.");

            this.JsonReader.AssertNotBuffering();
            if (this.ReadingResponse && expectResponseWrapper)
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string propertyName = this.JsonReader.ReadPropertyName();

                    if (string.CompareOrdinal(JsonConstants.ODataDataWrapperPropertyName, propertyName) == 0)
                    {
                        throw new ODataException(Strings.ODataJsonDeserializer_DataWrapperMultipleProperties);
                    }

                    // Skip all unrecognized properties
                    this.JsonReader.SkipValue();
                }

                // read the next input token (none should follow after the data wrapper end)
                this.JsonReader.ReadEndObject();
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(isReadingNestedPayload || this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
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

            return ProcessUriFromPayload(uriFromPayload, /*requireAbsoluteUri*/ true);
        }

        /// <summary>
        /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
        /// </summary>
        /// <param name="uriFromPayload">The URI string from the payload to process.</param>
        /// <param name="requireAbsoluteUri">true if the payload URI needs to be translated into an absolute URI; otherwise false.</param>
        /// <returns>An absolute URI to report.</returns>
        internal Uri ProcessUriFromPayload(string uriFromPayload, bool requireAbsoluteUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uriFromPayload != null, "uriFromPayload != null");
            Uri uri = new Uri(uriFromPayload, UriKind.RelativeOrAbsolute);

            // Try to resolve the URI using a custom URL resolver first.
            Uri resolvedUri = this.VerboseJsonInputContext.ResolveUri(this.MessageReaderSettings.BaseUri, uri);
            if (resolvedUri != null)
            {
                return resolvedUri;
            }

            if (!uri.IsAbsoluteUri)
            {
                if (this.MessageReaderSettings.BaseUri != null)
                {
                    // Try to use the base URI from the settings.
                    Debug.Assert(this.MessageReaderSettings.BaseUri.IsAbsoluteUri, "The BaseUri on settings should have been verified to be absolute by now.");
                    uri = UriUtils.UriToAbsoluteUri(this.MessageReaderSettings.BaseUri, uri);
                }
                else if (requireAbsoluteUri)
                {
                    // Otherwise fail
                    throw new ODataException(Strings.ODataJsonDeserializer_RelativeUriUsedWithoutBaseUriSpecified(uriFromPayload));
                }
            }

            Debug.Assert(uri.IsAbsoluteUri || !requireAbsoluteUri, "By now we should have absolute URI if we require one.");
            return uri;
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
    }
}
