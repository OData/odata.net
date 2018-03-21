//---------------------------------------------------------------------
// <copyright file="JsonLightMetadataLevel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for logic that varies based on the JSON Light metadata level.
    /// </summary>
    internal abstract class JsonLightMetadataLevel
    {
        /// <summary>
        /// Creates the appropriate metadata level based on the media type being written.
        /// </summary>
        /// <param name="mediaType">The full media type being written. This media type must have a type/subtype of "application/json".</param>
        /// <param name="metadataDocumentUri">The metadata document uri from the writer settings.</param>
        /// <param name="model">The edm model.</param>
        /// <param name="writingResponse">true if we are writing a response, false otherwise.</param>
        /// <returns>The JSON Light metadata level being written.</returns>
        internal static JsonLightMetadataLevel Create(ODataMediaType mediaType, Uri metadataDocumentUri, IEdmModel model, bool writingResponse)
        {
            Debug.Assert(mediaType != null, "mediaType != null");

            Debug.Assert(
                string.Compare(mediaType.FullTypeName, MimeConstants.MimeApplicationJson, StringComparison.OrdinalIgnoreCase) == 0,
                "media type should be application/json at this point");

            if (writingResponse && mediaType.Parameters != null)
            {
                foreach (KeyValuePair<string, string> parameter in mediaType.Parameters)
                {
                    if (!HttpUtils.IsMetadataParameter(parameter.Key))
                    {
                        // Only look at the "odata.metadata" parameter.
                        continue;
                    }

                    if (string.Compare(parameter.Value, MimeConstants.MimeMetadataParameterValueMinimal, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return new JsonMinimalMetadataLevel();
                    }

                    if (string.Compare(parameter.Value, MimeConstants.MimeMetadataParameterValueFull, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return new JsonFullMetadataLevel(metadataDocumentUri, model);
                    }

                    if (string.Compare(parameter.Value, MimeConstants.MimeMetadataParameterValueNone, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return new JsonNoMetadataLevel();
                    }

                    Debug.Assert(
                        string.Compare(parameter.Value, MimeConstants.MimeMetadataParameterValueVerbose, StringComparison.OrdinalIgnoreCase) != 0,
                        "media type should not indicate verbose json at this point.");
                }
            }

            // No "odata.metadata" media type parameter implies minimal metadata.
            return new JsonMinimalMetadataLevel();
        }

        /// <summary>
        /// Returns the oracle to use when determing the type name to write for entries and values.
        /// </summary>
        /// <returns>An oracle that can be queried to determine the type name to write.</returns>
        internal abstract JsonLightTypeNameOracle GetTypeNameOracle();

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
        /// <param name="keyAsSegment">true if keys should go in separate segments in auto-generated URIs, false if they should go in parentheses.
        /// A null value means the user hasn't specified a preference and we should look for an annotation in the entity container, if available.</param>
        /// <param name="odataUri">The OData Uri.</param>
        /// <returns>The created metadata builder.</returns>
        internal abstract ODataResourceMetadataBuilder CreateResourceMetadataBuilder(
            ODataResourceBase resource,
            IODataResourceTypeContext typeContext,
            ODataResourceSerializationInfo serializationInfo,
            IEdmStructuredType actualResourceType,
            SelectedPropertiesNode selectedProperties,
            bool isResponse,
            bool keyAsSegment,
            ODataUri odataUri);

        /// <summary>
        /// Injects the appropriate metadata builder based on the metadata level.
        /// </summary>
        /// <param name="resource">The resource to inject the builder.</param>
        /// <param name="builder">The metadata builder to inject.</param>
        internal virtual void InjectMetadataBuilder(ODataResourceBase resource, ODataResourceMetadataBuilder builder)
        {
            resource.MetadataBuilder = builder;
        }
    }
}
