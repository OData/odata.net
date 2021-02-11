//---------------------------------------------------------------------
// <copyright file="ODataConventionalEntityMetadataBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Evaluation
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies.V1;
    #endregion

    /// <summary>
    /// Implementation of OData entity metadata builder based on OData protocol conventions.
    /// </summary>
    internal sealed class ODataConventionalEntityMetadataBuilder : ODataConventionalIdMetadataBuilder
    {
        /// <summary>The edit link.</summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the resource or computed.</remarks>
        private Uri computedEditLink;

        /// <summary>The read link.</summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the resource or computed.</remarks>
        private Uri computedReadLink;

        /// <summary>The computed ETag.</summary>
        private string computedETag;

        /// <summary>true if the etag value has been computed, false otherwise.</summary>
        private bool etagComputed;

        /// <summary>The computed MediaResource for MLEs.</summary>
        private ODataStreamReferenceValue computedMediaResource;

        /// <summary>The list of computed stream properties.</summary>
        private List<ODataProperty> computedStreamProperties;

        /// <summary>
        /// Mark if we are at state of ResourceEnd, if it is true, GetProperties would concat computed stream properties.
        /// </summary>
        private bool isResourceEnd;

        /// <summary>The missing operation generator for the current resource.</summary>
        private ODataMissingOperationGenerator missingOperationGenerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceMetadataContext">The context to answer basic metadata questions about the resource.</param>
        /// <param name="metadataContext">The metadata context.</param>
        /// <param name="uriBuilder">The uri builder to use.</param>
        internal ODataConventionalEntityMetadataBuilder(IODataResourceMetadataContext resourceMetadataContext, IODataMetadataContext metadataContext, ODataUriBuilder uriBuilder)
            : base(resourceMetadataContext, metadataContext, uriBuilder)
        {
            this.isResourceEnd = true;  // Keep default behavior
        }

        /// <summary>
        /// The missing operation generator for the current resource.
        /// </summary>
        private ODataMissingOperationGenerator MissingOperationGenerator
        {
            get { return this.missingOperationGenerator ?? (this.missingOperationGenerator = new ODataMissingOperationGenerator(this.ResourceMetadataContext, this.MetadataContext)); }
        }

        /// <summary>
        /// Gets canonical url of current resource.
        /// </summary>
        /// <returns>The canonical url of current resource.</returns>
        public override Uri GetCanonicalUrl()
        {
            return this.GetId();
        }

        /// <summary>
        /// Gets the edit url of current entity.
        /// </summary>
        /// <returns>The edit url of current entity.</returns>
        public override Uri GetEditUrl()
        {
            return this.GetEditLink();
        }

        /// <summary>
        /// Gets the read url of current entity.
        /// </summary>
        /// <returns>The read url of current entity.</returns>
        public override Uri GetReadUrl()
        {
            return this.GetReadLink();
        }

        /// <summary>
        /// Gets the edit link of the entity.
        /// </summary>
        /// <returns>
        /// The absolute URI of the edit link for the entity.
        /// Or null if it is not possible to determine the edit link.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override Uri GetEditLink()
        {
            if (this.ResourceMetadataContext.Resource.HasNonComputedEditLink)
            {
                return this.ResourceMetadataContext.Resource.NonComputedEditLink;
            }
            else
            {
                // For readonly entity if ReadLink is there and EditLink is null, we should not calculate the EditLink in both serializer and deserializer.
                if (this.ResourceMetadataContext.Resource.IsTransient || this.ResourceMetadataContext.Resource.HasNonComputedReadLink)
                {
                    return null;
                }
                else
                {
                    if (this.computedEditLink != null)
                    {
                        return this.computedEditLink;
                    }
                    else
                    {
                        return this.computedEditLink = this.ComputeEditLink();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the read link of the entity.
        /// </summary>
        /// <returns>
        /// The absolute URI of the read link for the entity.
        /// Or null if it is not possible to determine the read link.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override Uri GetReadLink()
        {
            if (this.ResourceMetadataContext.Resource.HasNonComputedReadLink)
            {
                return this.ResourceMetadataContext.Resource.NonComputedReadLink;
            }
            else
            {
                if (this.computedReadLink != null)
                {
                    return this.computedReadLink;
                }
                else
                {
                    return this.computedReadLink = this.GetEditLink();
                }
            }
        }

        /// <summary>
        /// Gets the ETag of the entity.
        /// </summary>
        /// <returns>
        /// The ETag for the entity.
        /// Or null if it is not possible to determine the ETag.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override string GetETag()
        {
            if (this.ResourceMetadataContext.Resource.HasNonComputedETag)
            {
                return this.ResourceMetadataContext.Resource.NonComputedETag;
            }

            if (!this.etagComputed)
            {
                StringBuilder resultBuilder = new StringBuilder();
                foreach (KeyValuePair<string, object> etagProperty in this.ResourceMetadataContext.ETagProperties)
                {
                    if (resultBuilder.Length > 0)
                    {
                        resultBuilder.Append(',');
                    }
                    else
                    {
                        resultBuilder.Append("W/\"");
                    }

                    string keyValueText;
                    if (etagProperty.Value == null)
                    {
                        keyValueText = ExpressionConstants.KeywordNull;
                    }
                    else
                    {
                        keyValueText = LiteralFormatter.ForConstants.Format(etagProperty.Value);
                    }

                    resultBuilder.Append(keyValueText);
                }

                if (resultBuilder.Length > 0)
                {
                    resultBuilder.Append('"');
                    this.computedETag = resultBuilder.ToString();
                }

                this.etagComputed = true;
            }

            return this.computedETag;
        }

        /// <summary>
        /// Gets the default media resource of the entity.
        /// </summary>
        /// <returns>
        /// The the default media resource of the entity.
        /// Or null if the entity is not an MLE.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override ODataStreamReferenceValue GetMediaResource()
        {
            if (this.ResourceMetadataContext.Resource.NonComputedMediaResource != null)
            {
                return this.ResourceMetadataContext.Resource.NonComputedMediaResource;
            }

            if (this.computedMediaResource == null && this.ResourceMetadataContext.TypeContext.IsMediaLinkEntry)
            {
                this.computedMediaResource = new ODataStreamReferenceValue();
                this.computedMediaResource.SetMetadataBuilder(this, /*propertyName*/ null);

                // from OData spec: Media entity types MAY specify a list of acceptable media types using an annotation with term Core.AcceptableMediaTypes
                IEdmEntityType entityType = this.ResourceMetadataContext.ActualResourceType as IEdmEntityType;
                if (entityType != null)
                {
                    var mediaTypes = this.MetadataContext.Model.GetVocabularyStringCollection(entityType, CoreVocabularyModel.AcceptableMediaTypesTerm);
                    if (mediaTypes.Count() == 1)
                    {
                        // Be noted: AcceptableMediaTypes might have more than one media type,
                        // Convention (default) behavior only works if AcceptableMediaTypes is a collection of one.
                        this.computedMediaResource.ContentType = mediaTypes.ElementAt(0);
                    }
                }
            }

            return this.computedMediaResource;
        }

        /// <summary>
        /// Gets the entity properties.
        /// </summary>
        /// <param name="nonComputedProperties">Non-computed properties from the entity.</param>
        /// <returns>The the computed and non-computed entity properties.</returns>
        internal override IEnumerable<ODataProperty> GetProperties(IEnumerable<ODataProperty> nonComputedProperties)
        {
            if (!isResourceEnd)
            {
                return nonComputedProperties;
            }
            else
            {
                return ODataUtilsInternal.ConcatEnumerables(nonComputedProperties, this.GetComputedStreamProperties(nonComputedProperties));
            }
        }

        /// <summary>
        /// Mark the resource is just started to process.
        /// </summary>
        internal override void StartResource()
        {
            this.isResourceEnd = false;
        }

        /// <summary>
        /// Mark the resource has finished the processing. So GetProperties() need append ComputedStreamProperties for entity.
        /// </summary>
        internal override void EndResource()
        {
            this.isResourceEnd = true;
        }

        /// <summary>
        /// Gets the list of computed and non-computed actions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed actions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataAction> GetActions()
        {
            return ODataUtilsInternal.ConcatEnumerables(this.ResourceMetadataContext.Resource.NonComputedActions, this.MissingOperationGenerator.GetComputedActions());
        }

        /// <summary>
        /// Gets the list of computed and non-computed functions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed functions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataFunction> GetFunctions()
        {
            return ODataUtilsInternal.ConcatEnumerables(this.ResourceMetadataContext.Resource.NonComputedFunctions, this.MissingOperationGenerator.GetComputedFunctions());
        }

        /// <summary>
        /// Gets the navigation link URI for the specified navigation property.
        /// </summary>
        /// <param name="navigationPropertyName">The name of the navigation property to get the navigation link URI for.</param>
        /// <param name="navigationLinkUrl">The value of the link URI as seen on the wire or provided explicitly by the user or previously returned by the metadata builder, which may be null.</param>
        /// <param name="hasNestedResourceInfoUrl">true if the value of the <paramref name="navigationLinkUrl"/> was seen on the wire or provided explicitly by the user or previously returned by
        /// the metadata builder, false otherwise. This flag allows the metadata builder to determine whether a null navigation link url is an uninitialized value or a value that was set explicitly.</param>
        /// <returns>
        /// The navigation link URI for the navigation property.
        /// null if its not possible to determine the navigation link for the specified navigation property.
        /// </returns>
        internal override Uri GetNavigationLinkUri(string navigationPropertyName, Uri navigationLinkUrl, bool hasNestedResourceInfoUrl)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");

            return hasNestedResourceInfoUrl ? navigationLinkUrl : this.UriBuilder.BuildNavigationLinkUri(this.GetReadLink(), navigationPropertyName);
        }

        /// <summary>
        /// Gets the association link URI for the specified navigation property.
        /// </summary>
        /// <param name="navigationPropertyName">The name of the navigation property to get the association link URI for.</param>
        /// <param name="associationLinkUrl">The value of the link URI as seen on the wire or provided explicitly by the user or previously returned by the metadata builder, which may be null.</param>
        /// <param name="hasAssociationLinkUrl">true if the value of the <paramref name="associationLinkUrl"/> was seen on the wire or provided explicitly by the user or previously returned by
        /// the metadata builder, false otherwise. This flag allows the metadata builder to determine whether a null association link url is an uninitialized value or a value that was set explicitly.</param>
        /// <returns>
        /// The association link URI for the navigation property.
        /// null if its not possible to determine the association link for the specified navigation property.
        /// </returns>
        internal override Uri GetAssociationLinkUri(string navigationPropertyName, Uri associationLinkUrl, bool hasAssociationLinkUrl)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");

            return hasAssociationLinkUrl ? associationLinkUrl : this.UriBuilder.BuildAssociationLinkUri(this.GetReadLink(), navigationPropertyName);
        }

        /// <summary>
        /// Get the operation target URI for the specified <paramref name="operationName"/>.
        /// </summary>
        /// <param name="operationName">The fully qualified name of the operation for which to get the target URI.</param>
        /// <param name="bindingParameterTypeName">The binding parameter type name.</param>
        /// <param name="parameterNames">The parameter names to include in the target, or null/empty if there is none.</param>
        /// <returns>
        /// The target URI for the operation.
        /// null if it is not possible to determine the target URI for the specified operation.
        /// </returns>
        internal override Uri GetOperationTargetUri(string operationName, string bindingParameterTypeName, string parameterNames)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            Uri baseUri;
            if (string.IsNullOrEmpty(bindingParameterTypeName) || this.ResourceMetadataContext.Resource.NonComputedEditLink != null)
            {
                // if there is no parameter type name to append, or the edit-link is an opaque non-computed value, then use the edit-link as normal.
                baseUri = this.GetEditLink();
            }
            else
            {
                // Otherwise, use the computed URI which has no type segment
                baseUri = this.GetId();
            }

            return this.UriBuilder.BuildOperationTargetUri(baseUri, operationName, bindingParameterTypeName, parameterNames);
        }

        /// <summary>
        /// Get the operation title for the specified <paramref name="operationName"/>.
        /// </summary>
        /// <param name="operationName">The fully qualified name of the operation for which to get the target URI.</param>
        /// <returns>
        /// The title for the operation.
        /// null if it is not possible to determine the title for the specified operation.
        /// </returns>
        internal override string GetOperationTitle(string operationName)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            // TODO: What is the convention for operation title?
            return operationName;
        }

        /// <summary>
        /// Get the id that need to be written into wire
        /// </summary>
        /// <param name="id">The id return to the caller</param>
        /// <returns>
        /// If writer should write odata.id property into wire
        /// </returns>
        internal override bool TryGetIdForSerialization(out Uri id)
        {
            id = this.ResourceMetadataContext.Resource.IsTransient ? null : this.GetId();
            return true;
        }

        /// <summary>
        /// Computes the edit link.
        /// </summary>
        /// <returns>Uri that was computed based on the computed Id and possible type segment.</returns>
        private Uri ComputeEditLink()
        {
            Uri uri = this.ResourceMetadataContext.Resource.HasNonComputedId ? this.ResourceMetadataContext.Resource.NonComputedId : this.ComputedId;

            Debug.Assert(this.ResourceMetadataContext != null && this.ResourceMetadataContext.TypeContext != null, "this.resourceMetadataContext != null && this.resourceMetadataContext.TypeContext != null");
            if (this.ResourceMetadataContext.ActualResourceTypeName != this.ResourceMetadataContext.TypeContext.NavigationSourceEntityTypeName)
            {
                uri = this.UriBuilder.AppendTypeSegment(uri, this.ResourceMetadataContext.ActualResourceTypeName);
            }

            return uri;
        }

        /// <summary>
        /// Computes all projected or missing stream properties.
        /// </summary>
        /// <param name="nonComputedProperties">Non-computed properties from the entity.</param>
        /// <returns>The the computed stream properties for the resource.</returns>
        private IEnumerable<ODataProperty> GetComputedStreamProperties(IEnumerable<ODataProperty> nonComputedProperties)
        {
            if (this.computedStreamProperties == null)
            {
                // Remove all the projected properties that were already read from the payload
                IDictionary<string, IEdmStructuralProperty> projectedStreamProperties = this.ResourceMetadataContext.SelectedStreamProperties;
                if (nonComputedProperties != null)
                {
                    foreach (ODataProperty payloadProperty in nonComputedProperties)
                    {
                        projectedStreamProperties.Remove(payloadProperty.Name);
                    }
                }

                this.computedStreamProperties = new List<ODataProperty>();
                if (projectedStreamProperties.Count > 0)
                {
                    // Create all the missing stream properties and set the metadata builder
                    foreach (string missingStreamPropertyName in projectedStreamProperties.Keys)
                    {
                        ODataStreamReferenceValue streamPropertyValue = new ODataStreamReferenceValue();
                        streamPropertyValue.SetMetadataBuilder(this, missingStreamPropertyName);

                        // by default, let's retrieve the content type from vocabulary annotation
                        var edmProperty = projectedStreamProperties[missingStreamPropertyName];
                        var mediaTypes = this.MetadataContext.Model.GetVocabularyStringCollection(edmProperty, CoreVocabularyModel.AcceptableMediaTypesTerm);
                        if (mediaTypes.Count() == 1)
                        {
                            // Be noted: AcceptableMediaTypes might have more than one media type,
                            // Convention (default) behavior only works if AcceptableMediaTypes is a collection of one.
                            streamPropertyValue.ContentType = mediaTypes.ElementAt(0);
                        }

                        this.computedStreamProperties.Add(new ODataProperty { Name = missingStreamPropertyName, Value = streamPropertyValue });
                    }
                }
            }

            return this.computedStreamProperties;
        }
    }
}