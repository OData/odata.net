//---------------------------------------------------------------------
// <copyright file="JsonNoMetadataLevel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    using Microsoft.OData.Evaluation;
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
        /// Returns the oracle to use when determing the type name to write for entries and values.
        /// </summary>
        /// <returns>An oracle that can be queried to determine the type name to write.</returns>
        internal override JsonLightTypeNameOracle GetTypeNameOracle()
        {
            return new JsonNoMetadataTypeNameOracle();
        }

        /// <summary>
        /// Creates the metadata builder for the given resource. If such a builder is set, asking for payload
        /// metadata properties (like EditLink) of the resource may return a value computed by convention,
        /// depending on the metadata level and whether the user manually set an edit link or not.
        /// </summary>
        /// <param name="resource">The resource to create the metadata builder for.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource or resource set.</param>
        /// <param name="serializationInfo">The serialization info for the resource.</param>
        /// <param name="actualResourceType">The structured type of the resource.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="isResponse">true if the resource metadata builder to create should be for a response payload; false for a request.</param>
        /// <param name="keyAsSegment">true if keys should go in separate segments in auto-generated URIs, false if they should go in parentheses.</param>
        /// <param name="odataUri">The OData Uri.</param>
        /// <returns>The created metadata builder.</returns>
        internal override ODataResourceMetadataBuilder CreateResourceMetadataBuilder(
            ODataResourceBase resource,
            IODataResourceTypeContext typeContext,
            ODataResourceSerializationInfo serializationInfo,
            IEdmStructuredType actualResourceType,
            SelectedPropertiesNode selectedProperties,
            bool isResponse,
            bool keyAsSegment,
            ODataUri odataUri)
        {
            return ODataResourceMetadataBuilder.Null;
        }
    }
}
