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
        /// <summary>The URI builder to use.</summary>
        protected readonly ODataUriBuilder UriBuilder;

        /// <summary>The context to answer basic metadata questions about the resource.</summary>
        protected readonly IODataResourceMetadataContext ResourceMetadataContext;

        /// <summary>The metadata context.</summary>
        protected readonly IODataMetadataContext MetadataContext;

        /// <summary>The list of nested info that have been processed. Here navigation property and complex will both be marked for convenience.</summary>
        protected readonly HashSet<string> ProcessedNestedResourceInfos;

        /// <summary>The enumerator for unprocessed navigation links.</summary>
        private IEnumerator<ODataJsonLightReaderNestedResourceInfo> unprocessedNavigationLinks;

        /// <summary>The read link.</summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the resource or computed.</remarks>
        private Uri computedReadLink;

        /// <summary>The edit link.</summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the resource or computed.</remarks>
        private Uri computedEditLink;

        /// <summary>
        /// The resource whose payload metadata is being queried.
        /// </summary>
        private ODataResource resource;

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
        /// Compute edit link of current resource from:
        /// 1. NonComputedEditLink
        /// 2. Parent edit link
        /// </summary>
        /// <returns>The computed edit link.</returns>
        /// <remarks>The method is used when current resource edit link is not needed for writing, but its child resource may need. For example, to compute navigation link under complex.</remarks>
        public virtual Uri GetComputedEditLink()
        {
            if (this.computedEditLink != null)
            {
                return this.computedEditLink;
            }

            // computedEditLink = NonComputedEditLink
            if (this.resource.HasNonComputedEditLink)
            {
                return this.computedEditLink = this.resource.NonComputedEditLink;
            }

            // compute edit link from parent
            var parent = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
            if (this.NameAsProperty != null
                && parent != null
                && parent.GetComputedEditLink() != null)
            {
                // If parent is collection of complex, the edit link for this resource should be null.
                if (parent.IsFromCollection && !(parent is ODataConventionalEntityMetadataBuilder))
                {
                    return this.computedEditLink = null;
                }

                // computedEditLink = parentEditLink/propertyName
                this.computedEditLink = new Uri(parent.GetComputedEditLink() + "/" + this.NameAsProperty, UriKind.RelativeOrAbsolute);

                // Append possible type cast
                if (this.ResourceMetadataContext.ActualResourceTypeName != this.ResourceMetadataContext.TypeContext.ExpectedResourceTypeName)
                {
                    this.computedEditLink = this.UriBuilder.AppendTypeSegment(computedEditLink, this.ResourceMetadataContext.ActualResourceTypeName);
                }
            }

            return this.computedEditLink;
        }

        /// <summary>
        /// Compute read link of current resource from:
        /// 1. NonComputedReadLink
        /// 2. Computed edit link
        /// 3. Parent read link
        /// </summary>
        /// <returns>The computed read link.</returns>
        /// <remarks>The method is used when current resource read link is not needed for writing, but its child resource may need. For example, to compute navigation link under complex.</remarks>
        public virtual Uri GetComputedReadLink()
        {
            if (this.computedReadLink != null)
            {
                return this.computedReadLink;
            }

            // computedReadLink = NonComputedReadLink
            if (this.resource.HasNonComputedReadLink)
            {
                return this.resource.NonComputedReadLink;
            }

            // Compute readLink from editLink
            var editLink = this.GetComputedEditLink();
            if (editLink != null)
            {
                return this.computedReadLink = editLink;
            }

            // Compute readLink from parent readLink
            var parent = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
            if (this.NameAsProperty != null
                && parent != null
                && parent.GetComputedReadLink() != null)
            {
                // If parent is collection of complex, the read link for this resource should be null.
                if (parent.IsFromCollection && !(parent is ODataConventionalEntityMetadataBuilder))
                {
                    return this.computedReadLink = null;
                }

                // computedReadLink = parentReadLink/propertyName
                this.computedReadLink = new Uri(parent.GetComputedReadLink() + "/" + this.NameAsProperty, UriKind.RelativeOrAbsolute);

                // Append possible type cast
                if (this.ResourceMetadataContext.ActualResourceTypeName !=
                    this.ResourceMetadataContext.TypeContext.ExpectedResourceTypeName)
                {
                    this.computedReadLink = this.UriBuilder.AppendTypeSegment(computedReadLink, this.ResourceMetadataContext.ActualResourceTypeName);
                }
            }

            return this.computedReadLink;
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

            var readLink = this.GetComputedReadLink();

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

            var readLink = this.GetComputedReadLink();

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
