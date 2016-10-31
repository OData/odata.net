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
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Evaluation;

    /// <summary>
    /// Class responsible for logic specific to the JSON Light minimal metadata level (indicated by "odata=minimalmetadata" in the media type, or lack of an "odata" parameter in a v3 and above request).
    /// </summary>
    /// <remarks>
    /// The general rule-of-thumb for minimal-metadata payloads is that they include all "odata.*" annotations that can't be computed client-side, assuming the client has the server model available
    /// as well as the ability to compute missing payload metadata based on the standard conventions.
    /// </remarks>
    internal sealed class JsonMinimalMetadataLevel : JsonLightMetadataLevel
    {
        /// <summary>
        /// Returns the oracle to use when determing the type name to write for entries and values.
        /// </summary>
        /// <param name="autoComputePayloadMetadataInJson">Not used in this implementation of the abstract method.</param>
        /// <returns>An oracle that can be queried to determine the type name to write.</returns>
        internal override JsonLightTypeNameOracle GetTypeNameOracle(bool autoComputePayloadMetadataInJson)
        {
            DebugUtils.CheckNoExternalCallers();

            return new JsonMinimalMetadataTypeNameOracle();
        }

        /// <summary>
        /// Indicates whether the "odata.metadata" URI should be written based on the current metadata level.
        /// </summary>
        /// <returns>true if the metadata URI should be written, false otherwise.</returns>
        internal override bool ShouldWriteODataMetadataUri()
        {
            DebugUtils.CheckNoExternalCallers();
            return true;
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
        /// <returns>The created metadata builder.</returns>
        internal override ODataEntityMetadataBuilder CreateEntityMetadataBuilder(
            ODataEntry entry, 
            IODataFeedAndEntryTypeContext typeContext, 
            ODataFeedAndEntrySerializationInfo serializationInfo, 
            IEdmEntityType actualEntityType, 
            SelectedPropertiesNode selectedProperties, 
            bool isResponse, 
            bool? keyAsSegment)
        {
            // For minimal metadata we don't want to change the metadata builder that's currently on the entry because the entry might come from a JSON light
            // reader and it would contain the metadata builder from the reader.  Until we give the user the ability to choose whether to write what was reported
            // by the reader versus what was on the wire, we no-op here so the writer will just write what's on the OM for now.
            DebugUtils.CheckNoExternalCallers();
            return null;
        }

        /// <summary>
        /// Injects the appropriate metadata builder based on the metadata level.
        /// </summary>
        /// <param name="entry">The entry to inject the builder.</param>
        /// <param name="builder">The metadata builder to inject.</param>
        internal override void InjectMetadataBuilder(ODataEntry entry, ODataEntityMetadataBuilder builder)
        {
            // For minimal metadata we don't want to change the metadata builder that's currently on the entry because the entry might come from a JSON light
            // reader and it would contain the metadata builder from the reader.  Until we give the user the ability to choose whether to write what was reported
            // by the reader versus what was on the wire, we no-op here so the writer will just write what's on the OM for now.
            DebugUtils.CheckNoExternalCallers();
        }
    }
}
