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
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Evaluation;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for logic specific to the JSON Light full metadata level (indicated by "odata=fullmetadata" in the media type).
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");

            this.metadataDocumentUri = metadataDocumentUri;
            this.model = model;
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
                    throw new ODataException(OData.Strings.ODataJsonLightOutputContext_MetadataDocumentUriMissing);
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
            DebugUtils.CheckNoExternalCallers();

            if (autoComputePayloadMetadataInJson)
            {
                return new JsonFullMetadataTypeNameOracle();
            }

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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(typeContext != null, "typeContext != null");
            Debug.Assert(selectedProperties != null, "selectedProperties != null");

            IODataMetadataContext metadataContext = new ODataMetadataContext(isResponse, this.model, this.NonNullMetadataDocumentUri);
            
            UrlConvention urlConvention = UrlConvention.ForUserSettingAndTypeContext(keyAsSegment, typeContext);
            ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(metadataContext.ServiceBaseUri, urlConvention);
            
            IODataEntryMetadataContext entryMetadataContext = ODataEntryMetadataContext.Create(entry, typeContext, serializationInfo, actualEntityType, metadataContext, selectedProperties);
            return new ODataConventionalEntityMetadataBuilderWriter(entryMetadataContext, metadataContext, uriBuilder);
        }

        /// <summary>
        /// Injects the appropriate metadata builder based on the metadata level.
        /// </summary>
        /// <param name="entry">The entry to inject the builder.</param>
        /// <param name="builder">The metadata builder to inject.</param>
        internal override void InjectMetadataBuilder(ODataEntry entry, ODataEntityMetadataBuilder builder)
        {
            DebugUtils.CheckNoExternalCallers();
            entry.MetadataBuilder = builder;

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
