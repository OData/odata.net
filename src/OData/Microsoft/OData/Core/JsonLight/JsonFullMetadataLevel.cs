//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for logic specific to the JSON Light full metadata level (indicated by "odata.metadata=full" in the media type).
    /// </summary>
    /// <remarks>
    /// The general rule-of-thumb for full-metadata payloads is that they include all "odata.*" annotations that would be included in minimal metadata mode,
    /// plus any "odata.*" annotations that could be computed client-side if we the client had a model.
    /// </remarks>
    internal sealed class JsonFullMetadataLevel : JsonLightMetadataLevel
    {
        /// <summary>
        /// The Edm model.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>
        /// The metadata document uri from the writer settings.
        /// </summary>
        private readonly Uri metadataDocumentUri;

        /// <summary>
        /// Constructs a new <see cref="JsonFullMetadataLevel"/>.
        /// </summary>
        /// <param name="metadataDocumentUri">The metadata document uri from the writer settings.</param>
        /// <param name="model">The Edm model.</param>
        internal JsonFullMetadataLevel(Uri metadataDocumentUri, IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");

            this.metadataDocumentUri = metadataDocumentUri;
            this.model = model;
        }

        /// <summary>
        /// Indicates which level of context Url should be used when writing payload.
        /// </summary>
        internal override ODataContextUrlLevel ContextUrlLevel
        {
            get { return ODataContextUrlLevel.Full; }
        }

        /// <summary>
        /// Returns the metadata document URI which has been validated to be non-null.
        /// </summary>
        private Uri NonNullMetadataDocumentUri
        {
            get
            {
                if (this.metadataDocumentUri == null)
                {
                    throw new ODataException(OData.Core.Strings.ODataOutputContext_MetadataDocumentUriMissing);
                }

                return this.metadataDocumentUri;
            }
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
                return new JsonFullMetadataTypeNameOracle();
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
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(typeContext != null, "typeContext != null");
            Debug.Assert(selectedProperties != null, "selectedProperties != null");

            IODataMetadataContext metadataContext = new ODataMetadataContext(
                isResponse,
                this.model,
                this.NonNullMetadataDocumentUri,
                odataUri);
            
            UrlConvention urlConvention = UrlConvention.ForUserSettingAndTypeContext(keyAsSegment, typeContext);
            ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(metadataContext.ServiceBaseUri, urlConvention);

            IODataEntryMetadataContext entryMetadataContext = ODataEntryMetadataContext.Create(entry, typeContext, serializationInfo, actualEntityType, metadataContext, selectedProperties);
            return new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext, uriBuilder);
        }

        /// <summary>
        /// Injects the appropriate metadata builder based on the metadata level.
        /// </summary>
        /// <param name="entry">The entry to inject the builder.</param>
        /// <param name="builder">The metadata builder to inject.</param>
        internal override void InjectMetadataBuilder(ODataEntry entry, ODataEntityMetadataBuilder builder)
        {
            base.InjectMetadataBuilder(entry, builder);

            // Inject to the Media Resource.
            var mediaResource = entry.NonComputedMediaResource;
            if (mediaResource != null)
            {
                mediaResource.SetMetadataBuilder(builder, /*propertyName*/null);
            }

            // Inject to named stream property values
            if (entry.NonComputedProperties != null)
            {
                foreach (ODataProperty property in entry.NonComputedProperties)
                {
                    var streamReferenceValue = property.ODataValue as ODataStreamReferenceValue;
                    if (streamReferenceValue != null)
                    {
                        streamReferenceValue.SetMetadataBuilder(builder, property.Name);
                    }
                }
            }

            // Inject to operations
            IEnumerable<ODataOperation> operations = ODataUtilsInternal.ConcatEnumerables((IEnumerable<ODataOperation>)entry.NonComputedActions, (IEnumerable<ODataOperation>)entry.NonComputedFunctions);
            if (operations != null)
            {
                foreach (ODataOperation operation in operations)
                {
                    operation.SetMetadataBuilder(builder, this.NonNullMetadataDocumentUri);
                }
            }
        }
    }
}
