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
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// OData JSON deserializer for service documents.
    /// </summary>
    internal sealed class ODataJsonServiceDocumentDeserializer : ODataJsonDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The JSON input context to read from.</param>
        internal ODataJsonServiceDocumentDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
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
            this.ReadDataWrapperStart();

            // Read the start of the object container around the service document { "EntitySets": ... }
            this.JsonReader.ReadStartObject();

            // read all the properties in the service document object; we ignore all except 'EntityS>Patets'
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
            this.ReadDataWrapperEnd();

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return new ODataWorkspace
            {
                Collections = new ReadOnlyEnumerable<ODataResourceCollectionInfo>(collections)
            };
        }
    }
}
