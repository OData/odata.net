//---------------------------------------------------------------------
// <copyright file="ODataConventionalResourceMetadataBuilder.cs" company="Microsoft">
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
    using Microsoft.OData.JsonLight;
    #endregion

    /// <summary>
    /// Implementation of OData resource metadata builder based on OData protocol conventions.
    /// This metadata builder will be created for complex or for the resource which is not explicitly known as entity.
    /// </summary>
    internal class ODataConventionalResourceMetadataBuilder : ODataResourceMetadataBuilder
    {
        /// <summary>The context to answer basic metadata questions about the resource.</summary>
        public readonly IODataResourceMetadataContext ResourceMetadataContext;

        /// <summary>The URI builder to use.</summary>
        protected readonly ODataUriBuilder UriBuilder;

        /// <summary>The metadata context.</summary>
        protected readonly IODataMetadataContext MetadataContext;

        /// <summary>The list of nested info that have been processed. Here navigation property and complex will both be marked for convenience.</summary>
        protected readonly HashSet<string> ProcessedNestedResourceInfos;

        /// <summary>The enumerator for unprocessed navigation links.</summary>
        private IEnumerator<ODataJsonLightReaderNestedResourceInfo> unprocessedNavigationLinks;

        /// <summary>The read url.</summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the resource or computed.</remarks>
        private Uri readUrl;

        /// <summary>The edit url.</summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the resource or computed.</remarks>
        private Uri editUrl;

        /// <summary>The canonical url. </summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the resource or computed.</remarks>
        private Uri canonicalUrl;

        /// <summary>
        /// The resource whose payload metadata is being queried.
        /// </summary>
        private ODataResourceBase resource;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceMetadataContext">The context to answer basic metadata questions about the resource.</param>
        /// <param name="metadataContext">The metadata context.</param>
        /// <param name="uriBuilder">The uri builder to use.</param>
        internal ODataConventionalResourceMetadataBuilder(IODataResourceMetadataContext resourceMetadataContext, IODataMetadataContext metadataContext, ODataUriBuilder uriBuilder)
        {
            Debug.Assert(resourceMetadataContext != null, "resourceMetadataContext != null");
            Debug.Assert(metadataContext != null, "metadataContext != null");
            Debug.Assert(uriBuilder != null, "uriBuilder != null");

            this.ResourceMetadataContext = resourceMetadataContext;
            this.UriBuilder = uriBuilder;
            this.MetadataContext = metadataContext;
            this.ProcessedNestedResourceInfos = new HashSet<string>(StringComparer.Ordinal);
            this.resource = resourceMetadataContext.Resource;
        }

        /// <summary>
        /// OData uri that parsed based on the context url
        /// </summary>
        internal ODataUri ODataUri { get; set; }

        /// <summary>
        /// Gets edit url of current resource.
        /// Computes edit url of current resource from:
        /// 1. NonComputedEditLink
        /// 2. CanonicalUrl
        /// 3. Parent edit url
        /// </summary>
        /// <returns>The edit url of current resource.</returns>
        /// <remarks>The method is used to compute edit Url for current resource, including complex.</remarks>
        public virtual Uri GetEditUrl()
        {
            if (this.editUrl != null)
            {
                return this.editUrl;
            }

            // editUrl = NonComputedEditLink
            if (this.resource.HasNonComputedEditLink)
            {
                return this.editUrl = this.resource.NonComputedEditLink;
            }

            // Compute editUrl from canonicalUrl
            var canonicalUrl = this.GetCanonicalUrl();
            if (canonicalUrl != null)
            {
                this.editUrl = canonicalUrl;
            }
            else
            {
                // compute edit url from parent
                var parent = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
                if (this.NameAsProperty != null
                    && parent != null
                    && parent.GetEditUrl() != null)
                {
                    // If parent is collection of complex, the edit url for this resource should be null.
                    if (parent.IsFromCollection && !(parent is ODataConventionalEntityMetadataBuilder))
                    {
                        return this.editUrl = null;
                    }

                    // editUrl = parentEditUrl/propertyName
                    this.editUrl = new Uri(parent.GetEditUrl() + "/" + this.NameAsProperty, UriKind.RelativeOrAbsolute);
                }
            }

            // Append possible type cast
            if (this.editUrl != null && this.ResourceMetadataContext.ActualResourceTypeName !=
                this.ResourceMetadataContext.TypeContext.ExpectedResourceTypeName)
            {
                this.editUrl = this.UriBuilder.AppendTypeSegment(editUrl,
                    this.ResourceMetadataContext.ActualResourceTypeName);
            }

            return this.editUrl;
        }

        /// <summary>
        /// Gets read url of current resource.
        /// Computes read url of current resource from:
        /// 1. NonComputedReadLink
        /// 2. Computed edit url
        /// 3. Parent read url
        /// </summary>
        /// <returns>The read url of current resource.</returns>
        /// <remarks>The method is used to compute edit url for resource, including complex.</remarks>
        public virtual Uri GetReadUrl()
        {
            if (this.readUrl != null)
            {
                return this.readUrl;
            }

            // readUrl = NonComputedReadLink
            if (this.resource.HasNonComputedReadLink)
            {
                return this.readUrl = this.resource.NonComputedReadLink;
            }

            // Compute readUrl from editUrl
            var editLink = this.GetEditUrl();
            if (editLink != null)
            {
                return this.readUrl = editLink;
            }

            // Compute readUrl from parent readUrl
            var parent = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
            if (this.NameAsProperty != null
                && parent != null
                && parent.GetReadUrl() != null)
            {
                // If parent is collection of complex, the read url for this resource should be null.
                if (parent.IsFromCollection && !(parent is ODataConventionalEntityMetadataBuilder))
                {
                    return this.readUrl = null;
                }

                // readUrl = parentReadUrl/propertyName
                this.readUrl = new Uri(parent.GetReadUrl() + "/" + this.NameAsProperty, UriKind.RelativeOrAbsolute);

                // Append possible type cast
                if (this.ResourceMetadataContext.ActualResourceTypeName !=
                    this.ResourceMetadataContext.TypeContext.ExpectedResourceTypeName)
                {
                    this.readUrl = this.UriBuilder.AppendTypeSegment(readUrl,
                        this.ResourceMetadataContext.ActualResourceTypeName);
                }
            }

            return this.readUrl;
        }

        /// <summary>
        /// Gets canonical url of current resource.
        /// </summary>
        /// <returns>The canonical url of current resource.</returns>
        public virtual Uri GetCanonicalUrl()
        {
            if (this.canonicalUrl != null)
            {
                return this.canonicalUrl;
            }

            // canonicalUrl = HasNonComputedId
            if (this.resource.HasNonComputedId)
            {
                return this.canonicalUrl = this.resource.NonComputedId;
            }

            // Compute canonicalUrl from parent canonicalUrl
            var parent = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
            if (this.NameAsProperty != null
                && parent != null
                && parent.GetCanonicalUrl() != null)
            {
                // If parent is collection of complex, the canonical url for this resource should be null.
                if (parent.IsFromCollection && !(parent is ODataConventionalEntityMetadataBuilder))
                {
                    return this.canonicalUrl = null;
                }

                // canonicalUrl = parentCanonicalUrl[/typeCast]/propertyName
                // Different from edit url and read url, canonical url only needs type cast when the property is defined on derived type.
                this.canonicalUrl = parent.GetCanonicalUrl();

                IODataResourceTypeContext typeContext = parent.ResourceMetadataContext.TypeContext;

                if (parent.ResourceMetadataContext.ActualResourceTypeName != typeContext.ExpectedResourceTypeName)
                {
                    // Do not append type cast if we know that the navigation property is in base type, not in derived type.
                    ODataResourceTypeContext.ODataResourceTypeContextWithModel typeContextWithModel =
                        typeContext as ODataResourceTypeContext.ODataResourceTypeContextWithModel;
                    if (typeContextWithModel == null ||
                        typeContextWithModel.ExpectedResourceType.FindProperty(
                            this.NameAsProperty) == null)
                    {
                        this.canonicalUrl = this.UriBuilder.AppendTypeSegment(canonicalUrl,
                            parent.ResourceMetadataContext.ActualResourceTypeName);
                    }
                }

                this.canonicalUrl = new Uri(this.canonicalUrl + "/" + this.NameAsProperty, UriKind.RelativeOrAbsolute);
            }
            else
            {
                if (this.ODataUri != null && this.ODataUri.Path.Count != 0)
                {
                    this.canonicalUrl = this.ODataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
                }
            }

            return this.canonicalUrl;
        }

        /// <summary>
        /// Mark the resource is just started to process.
        /// </summary>
        internal virtual void StartResource()
        {
        }

        /// <summary>
        /// Mark the resource has finished the processing.
        /// </summary>
        internal virtual void EndResource()
        {
        }

        /// <summary>
        /// Gets the edit link of the resource.
        /// </summary>
        /// <returns>The edit link of the resource.</returns>
        internal override Uri GetEditLink()
        {
            return this.resource.NonComputedEditLink;
        }

        /// <summary>
        /// Gets the read link of the resource.
        /// </summary>
        /// <returns>The read link of the resource.</returns>
        internal override Uri GetReadLink()
        {
            return this.resource.NonComputedReadLink;
        }

        /// <summary>
        /// Gets the id of the resource.
        /// Only applies entity, so return null here, and will be overridden by entity.
        /// </summary>
        /// <returns>null</returns>
        internal override Uri GetId()
        {
            return this.resource.IsTransient ? null : this.resource.NonComputedId;
        }

        /// <summary>
        /// Tries to get id which is needed to written into wire.
        /// Only applies to entity.
        /// </summary>
        /// <param name="id">The id returns to caller, null here and will be overridden by entity.</param>
        /// <returns>false</returns>
        internal override bool TryGetIdForSerialization(out Uri id)
        {
            if (this.resource.IsTransient)
            {
                id = null;
                return true;
            }
            else
            {
                id = this.GetId();
                return id != null;
            }
        }

        /// <summary>
        /// Gets the etag for current resource.
        /// </summary>
        /// <returns>The etag for current resource.</returns>
        internal override string GetETag()
        {
            return this.resource.NonComputedETag;
        }

        /// <summary>
        /// Gets the entity properties.
        /// </summary>
        /// <param name="nonComputedProperties">Non-computed properties from the entity.</param>
        /// <returns>The the computed and non-computed entity properties.</returns>
        internal override IEnumerable<ODataProperty> GetProperties(IEnumerable<ODataProperty> nonComputedProperties)
        {
            return nonComputedProperties;
        }

        /// <summary>
        /// Gets the list of computed and non-computed actions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed actions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataAction> GetActions()
        {
            return this.resource.NonComputedActions;
        }

        /// <summary>
        /// Gets the list of computed and non-computed functions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed functions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataFunction> GetFunctions()
        {
            return this.resource.NonComputedFunctions;
        }

        /// <summary>
        /// Marks the given nested resource info as processed.
        /// </summary>
        /// <param name="navigationPropertyName">The nested resource info we've already processed.</param>
        internal override void MarkNestedResourceInfoProcessed(string navigationPropertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(navigationPropertyName), "!string.IsNullOrEmpty(navigationPropertyName)");
            Debug.Assert(this.ProcessedNestedResourceInfos != null, "this.processedNestedResourceInfos != null");
            this.ProcessedNestedResourceInfos.Add(navigationPropertyName);
        }

        /// <summary>
        /// Returns the next unprocessed navigation link or null if there's no more navigation links to process.
        /// </summary>
        /// <returns>Returns the next unprocessed navigation link or null if there's no more navigation links to process.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override ODataJsonLightReaderNestedResourceInfo GetNextUnprocessedNavigationLink()
        {
            if (this.unprocessedNavigationLinks == null)
            {
                Debug.Assert(this.ResourceMetadataContext != null, "this.resourceMetadataContext != null");
                this.unprocessedNavigationLinks = this.ResourceMetadataContext.SelectedNavigationProperties
                    .Where(p => !this.ProcessedNestedResourceInfos.Contains(p.Name))
                    .Select(ODataJsonLightReaderNestedResourceInfo.CreateProjectedNestedResourceInfo)
                    .GetEnumerator();
            }

            if (this.unprocessedNavigationLinks.MoveNext())
            {
                return this.unprocessedNavigationLinks.Current;
            }

            return null;
        }

        //// Stream content type and ETag can't be computed from conventions.

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

            var readLink = this.GetReadUrl();

            if (readLink == null || this.IsFromCollection)
            {
                return null;
            }

            return hasNestedResourceInfoUrl ? navigationLinkUrl : this.UriBuilder.BuildNavigationLinkUri(readLink, navigationPropertyName);
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

            var readLink = this.GetReadUrl();

            if (readLink == null || this.IsFromCollection)
            {
                return null;
            }

            return hasAssociationLinkUrl ? associationLinkUrl : this.UriBuilder.BuildAssociationLinkUri(readLink, navigationPropertyName);
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
    }
}
