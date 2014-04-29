//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class responsible for logic specific to the JSON Light no metadata level (indicated by "odata.metadata=none" in the media type).
    /// </summary>
    /// <remarks>
    /// The general rule-of-thumb for no-metadata payloads is that they omit any "odata.*" annotations, 
    /// except for odata.nextlink and odata.count, since the client would get a inaccurate representation of the data available if they were left out.
    /// </remarks>
    internal sealed class JsonNoMetadataLevel : JsonLightMetadataLevel
    {
        /// <summary>
        /// Indicates which level of context Url should be used when writing payload.
        /// </summary>
        internal override ODataContextUrlLevel ContextUrlLevel
        {
            get { return ODataContextUrlLevel.None; }
        }

        /// <summary>
        /// Returns the oracle to use when determing the type name to write for entries and values.
        /// </summary>
        /// <param name="autoComputePayloadMetadataInJson">
        /// If true, the type name to write according to full metadata rules. 
        /// If false, the type name writing according to minimal metadata rules.
        /// This is for backwards compatibility.
        /// </param>
        /// <returns>An oracle that can be queried to determine the type name to write.</returns>
        internal override JsonLightTypeNameOracle GetTypeNameOracle(bool autoComputePayloadMetadataInJson)
        {
            if (autoComputePayloadMetadataInJson)
            {
                return new JsonNoMetadataTypeNameOracle();
            }

            return new JsonMinimalMetadataTypeNameOracle();
        }

        /// <summary>
        /// Creates the metadata builder for the given entry. If such a builder is set, asking for payload
        /// metadata properties (like EditLink) of the entry may return a value computed by convention, 
        /// depending on the metadata level and whether the user manually set an edit link or not.
        /// </summary>
        /// <param name="entry">The entry to create the metadata builder for.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry or feed.</param>
        /// <param name="serializationInfo">The serialization info for the entry.</param>
        /// <param name="actualEntityType">The entity type of the entry.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="isResponse">true if the entity metadata builder to create should be for a response payload; false for a request.</param>
        /// <param name="keyAsSegment">true if keys should go in seperate segments in auto-generated URIs, false if they should go in parentheses.
        /// A null value means the user hasn't specified a preference and we should look for an annotation in the entity container, if available.</param>
        /// <param name="odataUri">The OData Uri.</param>
        /// <returns>The created metadata builder.</returns>
        internal override ODataEntityMetadataBuilder CreateEntityMetadataBuilder(
            ODataEntry entry, 
            IODataFeedAndEntryTypeContext typeContext, 
            ODataFeedAndEntrySerializationInfo serializationInfo, 
            IEdmEntityType actualEntityType, 
            SelectedPropertiesNode selectedProperties, 
            bool isResponse, 
            bool? keyAsSegment,
            ODataUri odataUri)
        {
            return ODataEntityMetadataBuilder.Null;
        }
    }
}
