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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for collections.
    /// </summary>
    internal sealed class ODataJsonLightCollectionSerializer : ODataJsonLightValueSerializer
    {
        /// <summary>true when writing a top-level collection that requires the 'value' wrapper object; otherwise false.</summary>
        private readonly bool writingTopLevelCollection;

        /// <summary>The metadata uri builder to use.</summary>
        private readonly ODataJsonLightMetadataUriBuilder metadataUriBuilder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="writingTopLevelCollection">true when writing a top-level collection that requires the 'value' wrapper object; otherwise false.</param>
        internal ODataJsonLightCollectionSerializer(ODataJsonLightOutputContext jsonLightOutputContext, bool writingTopLevelCollection)
            : base(jsonLightOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            this.writingTopLevelCollection = writingTopLevelCollection;

            // DEVNOTE: grab this early so that any validation errors are thrown at creation time rather than when Write___ is called.
            this.metadataUriBuilder = jsonLightOutputContext.CreateMetadataUriBuilder();
        }

        /// <summary>
        /// Writes the start of a collection.
        /// </summary>
        /// <param name="collectionStart">The collection start to write.</param>
        /// <param name="itemTypeReference">The item type of the collection or null if no metadata is available.</param>
        internal void WriteCollectionStart(ODataCollectionStart collectionStart, IEdmTypeReference itemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(collectionStart != null, "collectionStart != null");

            if (this.writingTopLevelCollection)
            {
                // "{"
                this.JsonWriter.StartObjectScope();

                // "odata.metadata":...
                Uri metadataUri;
                if (this.metadataUriBuilder.TryBuildCollectionMetadataUri(collectionStart.SerializationInfo, itemTypeReference, out metadataUri))
                {
                    this.WriteMetadataUriProperty(metadataUri);
                }

                // "value":
                this.JsonWriter.WriteValuePropertyName();
            }

            // Write the start of the array for the collection items
            // "["
            this.JsonWriter.StartArrayScope();
        }

        /// <summary>
        /// Writes the end of a collection.
        /// </summary>
        internal void WriteCollectionEnd()
        {
            DebugUtils.CheckNoExternalCallers();

            // Write the end of the array for the collection items
            // "]"
            this.JsonWriter.EndArrayScope();

            if (this.writingTopLevelCollection)
            {
                // "}"
                this.JsonWriter.EndObjectScope();
            }
        }
    }
}
