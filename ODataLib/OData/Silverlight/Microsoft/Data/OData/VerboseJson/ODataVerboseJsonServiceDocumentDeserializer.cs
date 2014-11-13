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

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// OData JSON deserializer for service documents.
    /// </summary>
    internal sealed class ODataVerboseJsonServiceDocumentDeserializer : ODataVerboseJsonDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonInputContext">The Verbose JSON input context to read from.</param>
        internal ODataVerboseJsonServiceDocumentDeserializer(ODataVerboseJsonInputContext verboseJsonInputContext)
            : base(verboseJsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal ODataWorkspace ReadServiceDocument()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            List<ODataResourceCollectionInfo> collections = null;

            // Read the response wrapper "d"
            this.ReadPayloadStart(false /*isReadingNestedPayload*/);

            // Read the start of the object container around the service document { "EntitySets": ... }
            this.JsonReader.ReadStartObject();

            // read all the properties in the service document object; we ignore all except 'EntitySets'
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();

                if (string.CompareOrdinal(JsonConstants.ODataServiceDocumentEntitySetsName, propertyName) == 0)
                {
                    if (collections != null)
                    {
                        throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_MultipleEntitySetsPropertiesForServiceDocument);
                    }

                    collections = new List<ODataResourceCollectionInfo>();

                    // read the value of the 'EntitySets' property
                    this.JsonReader.ReadStartArray();

                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                    {
                        string collectionUrl = this.JsonReader.ReadStringValue();
                        ValidationUtils.ValidateResourceCollectionInfoUrl(collectionUrl);

                        ODataResourceCollectionInfo collection = new ODataResourceCollectionInfo
                        {
                            Url = this.ProcessUriFromPayload(collectionUrl, /*requireAbsoluteUri*/ false)
                        };

                        collections.Add(collection);
                    }

                    this.JsonReader.ReadEndArray();
                }
                else
                {
                    this.JsonReader.SkipValue();
                }
            }

            if (collections == null)
            {
                throw new ODataException(Strings.ODataJsonServiceDocumentDeserializer_NoEntitySetsPropertyForServiceDocument);
            }

            // Read over the end object (nothing else can happen after all properties have been read)
            this.JsonReader.ReadEndObject();

            // Read the end of the response wrapper "d".
            this.ReadPayloadEnd(false /*isReadingNestedPayload*/);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return new ODataWorkspace
            {
                Collections = new ReadOnlyEnumerable<ODataResourceCollectionInfo>(collections)
            };
        }
    }
}
