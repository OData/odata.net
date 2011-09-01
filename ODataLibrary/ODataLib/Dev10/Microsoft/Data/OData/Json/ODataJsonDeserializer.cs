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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData JSON deserializers.
    /// </summary>
    internal abstract class ODataJsonDeserializer : ODataDeserializer
    {
        /// <summary>The JSON input context to use for reading.</summary>
        private readonly ODataJsonInputContext jsonInputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The JSON input context to read from.</param>
        protected ODataJsonDeserializer(ODataJsonInputContext jsonInputContext)
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
        /// Read the start of the top-level data wrapper in JSON responses.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet.
        /// Post-Condition: The reader is positioned on the first node of the payload (this can be the first node or the value of the 'd' property node)
        /// </remarks>
        internal void ReadDataWrapperStart()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: JSON reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // position the reader on the first node
            this.JsonReader.Read();

            if (this.ReadingResponse)
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
        /// <remarks>
        /// Pre-Condition:  any node:                when reading response, will fail if find anything else then EndObject.
        ///                 JsonNodeType.EndOfInput: otherwise
        /// Post-Condition: JsonNodeType.EndOfInput
        /// </remarks>
        internal void ReadDataWrapperEnd()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                this.ReadingResponse || this.JsonReader.NodeType == JsonNodeType.EndOfInput,
                "Pre-Condition: JsonNodeType.EndOfInput if not reading a response");
            this.JsonReader.AssertNotBuffering();

            if (this.ReadingResponse)
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
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
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
            Uri resolvedUri = this.ResolveUri(this.MessageReaderSettings.BaseUri, uri);
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
    }
}
