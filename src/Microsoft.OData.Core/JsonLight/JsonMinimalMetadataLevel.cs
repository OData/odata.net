//---------------------------------------------------------------------
// <copyright file="JsonMinimalMetadataLevel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Evaluation;

    /// <summary>
    /// Class responsible for logic specific to the JSON Light minimal metadata level (indicated by "odata.metadata=minimal" in the media type, or lack of an "odata" parameter in a v3 and above request).
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
        /// <returns>An oracle that can be queried to determine the type name to write.</returns>
        internal override JsonLightTypeNameOracle GetTypeNameOracle()
        {
            return new JsonMinimalMetadataTypeNameOracle();
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
            // For minimal metadata we don't want to change the metadata builder that's currently on the resource because the resource might come from a JSON light
            // reader and it would contain the metadata builder from the reader.  Until we give the user the ability to choose whether to write what was reported
            // by the reader versus what was on the wire, we no-op here so the writer will just write what's on the OM for now.
            return null;
        }

        /// <summary>
        /// Injects the appropriate metadata builder based on the metadata level.
        /// </summary>
        /// <param name="resource">The resource to inject the builder.</param>
        /// <param name="builder">The metadata builder to inject.</param>
        internal override void InjectMetadataBuilder(ODataResourceBase resource, ODataResourceMetadataBuilder builder)
        {
            // For minimal metadata we don't want to change the metadata builder that's currently on the resource because the resource might come from a JSON light
            // reader and it would contain the metadata builder from the reader.  Until we give the user the ability to choose whether to write what was reported
            // by the reader versus what was on the wire, we no-op here so the writer will just write what's on the OM for now.
        }
    }
}
