//---------------------------------------------------------------------
// <copyright file="ODataConventionalIdMetadataBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Evaluation
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    #endregion

    /// <summary>
    /// Implementation of OData metadata builder that generates ids based on OData protocol conventions.
    /// This specifically does NOT generate navigation links/stream properties as it is used for deltas.
    /// </summary>
    internal class ODataConventionalIdMetadataBuilder : ODataConventionalResourceMetadataBuilder
    {
        /// <summary>The computed ID of this entity instance.</summary>
        /// <remarks>
        /// This is always built from the key properties, and never comes from the resource.
        /// </remarks>
        private Uri computedId;

        /// <summary>The computed key property name and value pairs of the resource.</summary>
        private ICollection<KeyValuePair<string, object>> computedKeyProperties;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceMetadataContext">The context to answer basic metadata questions about the resource.</param>
        /// <param name="metadataContext">The metadata context.</param>
        /// <param name="uriBuilder">The uri builder to use.</param>
        internal ODataConventionalIdMetadataBuilder(IODataResourceMetadataContext resourceMetadataContext, IODataMetadataContext metadataContext, ODataUriBuilder uriBuilder)
            : base(resourceMetadataContext, metadataContext, uriBuilder)
        {
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
        /// Lazy evaluated computed entity Id. This is always a computed value and never comes from the resource.
        /// </summary>
        protected Uri ComputedId
        {
            get
            {
                this.ComputeAndCacheId();
                return this.computedId;
            }
        }

        /// <summary>
        /// The computed key property name and value pairs of the resource.
        /// If a value is unsigned integer, it will be automatically converted to its underlying type.
        /// </summary>
        private ICollection<KeyValuePair<string, object>> ComputedKeyProperties
        {
            get
            {
                if (computedKeyProperties == null)
                {
                    computedKeyProperties = new List<KeyValuePair<string, object>>();

                    foreach (var originalKeyProperty in this.ResourceMetadataContext.KeyProperties)
                    {
                        object newValue = this.MetadataContext.Model.ConvertToUnderlyingTypeIfUIntValue(originalKeyProperty.Value);
                        computedKeyProperties.Add(new KeyValuePair<string, object>(originalKeyProperty.Key, newValue));
                    }
                }

                return computedKeyProperties;
            }
        }

        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        /// <returns>
        /// The ID for the entity.
        /// Or null if it is not possible to determine the ID.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override Uri GetId()
        {
            // If the Id were on the wire, use that wire value for the Id.
            // If the resource was transient resource, return null for Id
            // Otherwise compute it based on the key values.
            return this.ResourceMetadataContext.Resource.HasNonComputedId ? this.ResourceMetadataContext.Resource.NonComputedId : (this.ResourceMetadataContext.Resource.IsTransient ? null : this.ComputedId);
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
        /// Computes and sets the field for the computed Id.
        /// </summary>
        private void ComputeAndCacheId()
        {
            if (this.computedId != null)
            {
                return;
            }

            Uri uri;
            switch (this.ResourceMetadataContext.TypeContext.NavigationSourceKind)
            {
                case EdmNavigationSourceKind.Singleton:
                    uri = this.ComputeIdForSingleton();
                    break;
                case EdmNavigationSourceKind.ContainedEntitySet:
                    uri = this.ComputeIdForContainment();
                    break;
                case EdmNavigationSourceKind.UnknownEntitySet:
                    throw new ODataException(Strings.ODataMetadataBuilder_UnknownEntitySet(this.ResourceMetadataContext.TypeContext.NavigationSourceName));
                default:
                    uri = this.ComputeId();
                    break;
            }

            this.computedId = uri;
        }

        /// <summary>
        /// Compute id for neither singleton, nor containment scenario.
        /// </summary>
        /// <returns>
        /// The <see cref="Uri"/> of @odata.id.
        /// </returns>
        private Uri ComputeId()
        {
            if (this.ResourceMetadataContext.KeyProperties.Any())
            {
                Uri uri = this.UriBuilder.BuildBaseUri();
                uri = this.UriBuilder.BuildEntitySetUri(uri, this.ResourceMetadataContext.TypeContext.NavigationSourceName);
                uri = this.UriBuilder.BuildEntityInstanceUri(uri, this.ComputedKeyProperties, this.ResourceMetadataContext.ActualResourceTypeName);
                return uri;
            }

            return null;
        }

        /// <summary>
        /// Compute id for containment scenario.
        /// </summary>
        /// <returns>
        /// The <see cref="Uri"/> of @odata.id.
        /// </returns>
        private Uri ComputeIdForContainment()
        {
            Uri uri;

            if (!TryComputeIdFromParent(out uri))
            {
                // Compute ID from context URL rather than from parent.
                uri = this.UriBuilder.BuildBaseUri();
                ODataUri odataUri = this.ODataUri ?? this.MetadataContext.ODataUri;

                if (odataUri == null || odataUri.Path == null || odataUri.Path.Count == 0)
                {
                    throw new ODataException(Strings.ODataMetadataBuilder_MissingParentIdOrContextUrl);
                }

                uri = this.GetContainingEntitySetUri(uri, odataUri);
            }

            // A path segment for the containment navigation property
            uri = this.UriBuilder.BuildEntitySetUri(uri, this.ResourceMetadataContext.TypeContext.NavigationSourceName);

            if (this.ResourceMetadataContext.TypeContext.IsFromCollection)
            {
                if (this.ResourceMetadataContext.KeyProperties.Any())
                {
                    uri = this.UriBuilder.BuildEntityInstanceUri(
                        uri,
                        this.ComputedKeyProperties,
                        this.ResourceMetadataContext.ActualResourceTypeName);
                }
                else
                {
                    uri = null;
                }
            }

            return uri;
        }

        private bool TryComputeIdFromParent(out Uri uri)
        {
            try
            {
                ODataConventionalResourceMetadataBuilder parent = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
                if (parent != null && parent != this)
                {
                    // Get the parent canonical url
                    uri = parent.GetCanonicalUrl();

                    if (uri != null)
                    {
                        // And append cast (if needed).
                        // A cast segment if the navigation property is defined on a type derived from the type expected.
                        IODataResourceTypeContext typeContext = parent.ResourceMetadataContext.TypeContext;

                        if (parent.ResourceMetadataContext.ActualResourceTypeName != typeContext.ExpectedResourceTypeName)
                        {
                            // Do not append type cast if we know that the navigation property is in base type, not in derived type.
                            ODataResourceTypeContext.ODataResourceTypeContextWithModel typeContextWithModel =
                                typeContext as ODataResourceTypeContext.ODataResourceTypeContextWithModel;
                            if (typeContextWithModel == null ||
                                typeContextWithModel.ExpectedResourceType.FindProperty(
                                    this.ResourceMetadataContext.TypeContext.NavigationSourceName) == null)
                            {
                                uri = new Uri(UriUtils.EnsureTaillingSlash(uri),
                                    parent.ResourceMetadataContext.ActualResourceTypeName);
                            }
                        }

                        return true;
                    }
                }
            }
            catch (ODataException)
            {
            }

            uri = null;
            return false;
        }

        /// <summary>
        /// Compute id for singleton scenario.
        /// </summary>
        /// <returns>
        /// The <see cref="Uri"/> of @odata.id.
        /// </returns>
        private Uri ComputeIdForSingleton()
        {
            // Do not append key for singleton.
            Uri uri = this.UriBuilder.BuildBaseUri();
            uri = this.UriBuilder.BuildEntitySetUri(uri, this.ResourceMetadataContext.TypeContext.NavigationSourceName);
            return uri;
        }

        /// <summary>
        /// Gets resource path from service root, and request Uri.
        /// </summary>
        /// <param name="baseUri">The service root Uri.</param>
        /// <param name="odataUri">The request Uri.</param>
        /// <returns>The resource path.</returns>
        private Uri GetContainingEntitySetUri(Uri baseUri, ODataUri odataUri)
        {
            ODataPath path = odataUri.Path;
            List<ODataPathSegment> segments = path.ToList();
            int lastIndex = segments.Count - 1;
            ODataPathSegment lastSegment = segments[lastIndex];
            while (!(lastSegment is NavigationPropertySegment) && !(lastSegment is OperationSegment))
            {
                lastSegment = segments[--lastIndex];
            }

            while (lastSegment is TypeSegment)
            {
                ODataPathSegment previousSegment = segments[lastIndex - 1];
                IEdmStructuredType owningType = previousSegment.TargetEdmType as IEdmStructuredType;
                if (owningType != null && owningType.FindProperty(lastSegment.Identifier) != null)
                {
                    lastSegment = segments[--lastIndex];
                }
                else
                {
                    break;
                }
            }

            // trim all the un-needed segments 
            segments = segments.GetRange(0, lastIndex);

            // append each segment to base uri
            Uri uri = baseUri;
            foreach (ODataPathSegment segment in segments)
            {
                var keySegment = segment as KeySegment;
                if (keySegment == null)
                {
                    uri = this.UriBuilder.BuildEntitySetUri(uri, segment.Identifier);
                }
                else
                {
                    uri = this.UriBuilder.BuildEntityInstanceUri(
                        uri,
                        keySegment.Keys.ToList(),
                        keySegment.EdmType.FullTypeName());
                }
            }

            return uri;
        }
    }
}