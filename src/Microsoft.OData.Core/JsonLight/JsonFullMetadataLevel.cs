//---------------------------------------------------------------------
// <copyright file="JsonFullMetadataLevel.cs" company="Microsoft">
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
        /// Returns the metadata document URI which has been validated to be non-null.
        /// </summary>
        private Uri NonNullMetadataDocumentUri
        {
            get
            {
                if (this.metadataDocumentUri == null)
                {
                    throw new ODataException(Strings.ODataOutputContext_MetadataDocumentUriMissing);
                }

                return this.metadataDocumentUri;
            }
        }

        /// <summary>
        /// Returns the oracle to use when determing the type name to write for entries and values.
        /// </summary>
        /// <returns>An oracle that can be queried to determine the type name to write.</returns>
        internal override JsonLightTypeNameOracle GetTypeNameOracle()
        {
            return new JsonFullMetadataTypeNameOracle();
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
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(typeContext != null, "typeContext != null");
            Debug.Assert(selectedProperties != null, "selectedProperties != null");

            IODataMetadataContext metadataContext = new ODataMetadataContext(
                isResponse,
                this.model,
                this.NonNullMetadataDocumentUri,
                odataUri);

            ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(metadataContext.ServiceBaseUri,
                keyAsSegment ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses);

            IODataResourceMetadataContext resourceMetadataContext = ODataResourceMetadataContext.Create(resource, typeContext, serializationInfo, actualResourceType, metadataContext, selectedProperties);

            // Create ODataConventionalEntityMetadataBuilder if actualResourceType is entity type or typeContext.NavigationSourceKind is not none (complex type would be none) for no model scenario.
            if (actualResourceType != null && actualResourceType.TypeKind == EdmTypeKind.Entity ||
                actualResourceType == null && typeContext.NavigationSourceKind != EdmNavigationSourceKind.None)
            {
                return new ODataConventionalEntityMetadataBuilder(resourceMetadataContext, metadataContext, uriBuilder);
            }
            else
            {
                return new ODataConventionalResourceMetadataBuilder(resourceMetadataContext, metadataContext, uriBuilder);
            }
        }

        /// <summary>
        /// Injects the appropriate metadata builder based on the metadata level.
        /// </summary>
        /// <param name="resource">The resource to inject the builder.</param>
        /// <param name="builder">The metadata builder to inject.</param>
        internal override void InjectMetadataBuilder(ODataResourceBase resource, ODataResourceMetadataBuilder builder)
        {
            base.InjectMetadataBuilder(resource, builder);

            // Inject to the Media Resource.
            var mediaResource = resource.NonComputedMediaResource;
            if (mediaResource != null)
            {
                mediaResource.SetMetadataBuilder(builder, /*propertyName*/null);
            }

            // Inject to named stream property values
            if (resource.NonComputedProperties != null)
            {
                foreach (ODataProperty property in resource.NonComputedProperties)
                {
                    var streamReferenceValue = property.ODataValue as ODataStreamReferenceValue;
                    if (streamReferenceValue != null)
                    {
                        streamReferenceValue.SetMetadataBuilder(builder, property.Name);
                    }
                }
            }

            // Inject to operations
            IEnumerable<ODataOperation> operations = ODataUtilsInternal.ConcatEnumerables((IEnumerable<ODataOperation>)resource.NonComputedActions, (IEnumerable<ODataOperation>)resource.NonComputedFunctions);
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
