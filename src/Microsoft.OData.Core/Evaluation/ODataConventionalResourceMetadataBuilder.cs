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
    using System.Text;
    using Microsoft.OData.JsonLight;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    #endregion

    /// <summary>
    /// Implementation of OData resource metadata builder based on OData protocol conventions.
    /// </summary>
    internal sealed class ODataConventionalResourceMetadataBuilder : ODataResourceMetadataBuilder
    {
        /// <summary>The URI builder to use.</summary>
        private readonly ODataUriBuilder uriBuilder;

        /// <summary>The context to answer basic metadata questions about the resource.</summary>
        private readonly IODataResourceMetadataContext resourceMetadataContext;

        /// <summary>The metadata context.</summary>
        private readonly IODataMetadataContext metadataContext;

        /// <summary>The list of navigation links that have been processed.</summary>
        private readonly HashSet<string> processedNestedResourceInfos;

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

        /// <summary>The computed ID of this entity instance.</summary>
        /// <remarks>
        /// This is always built from the key properties, and never comes from the resource.
        /// </remarks>
        private Uri computedId;

        /// <summary>The computed MediaResource for MLEs.</summary>
        private ODataStreamReferenceValue computedMediaResource;

        /// <summary>The list of computed stream properties.</summary>
        private List<ODataProperty> computedStreamProperties;

        /// <summary>The enumerator for unprocessed navigation links.</summary>
        private IEnumerator<ODataJsonLightReaderNestedResourceInfo> unprocessedNestedResourceInfos;

        /// <summary>The missing operation generator for the current resource.</summary>
        private ODataMissingOperationGenerator missingOperationGenerator;

        /// <summary>The computed key property name and value pairs of the resource.</summary>
        private ICollection<KeyValuePair<string, object>> computedKeyProperties;

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

            this.resourceMetadataContext = resourceMetadataContext;
            this.uriBuilder = uriBuilder;
            this.metadataContext = metadataContext;
            this.processedNestedResourceInfos = new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// OData uri that parsed based on the context url
        /// </summary>
        internal ODataUri ODataUri { get; set; }

        /// <summary>
        /// Lazy evaluated computed entity Id. This is always a computed value and never comes from the resource.
        /// </summary>
        private Uri ComputedId
        {
            get
            {
                this.ComputeAndCacheId();
                return this.computedId;
            }
        }

        /// <summary>
        /// The missig operation generator for the current resource.
        /// </summary>
        private ODataMissingOperationGenerator MissingOperationGenerator
        {
            get { return this.missingOperationGenerator ?? (this.missingOperationGenerator = new ODataMissingOperationGenerator(this.resourceMetadataContext, this.metadataContext)); }
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

                    foreach (var originalKeyProperty in this.resourceMetadataContext.KeyProperties)
                    {
                        object newValue = this.metadataContext.Model.ConvertToUnderlyingTypeIfUIntValue(originalKeyProperty.Value);
                        computedKeyProperties.Add(new KeyValuePair<string, object>(originalKeyProperty.Key, newValue));
                    }
                }

                return computedKeyProperties;
            }
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
            if (this.resourceMetadataContext.Resource.HasNonComputedEditLink)
            {
                return this.resourceMetadataContext.Resource.NonComputedEditLink;
            }
            else
            {
                // For readonly entity if ReadLink is there and EditLink is null, we should not calculate the EditLink in both serializer and deserializer.
                if (this.resourceMetadataContext.Resource.IsTransient || this.resourceMetadataContext.Resource.HasNonComputedReadLink)
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
            if (this.resourceMetadataContext.Resource.HasNonComputedReadLink)
            {
                return this.resourceMetadataContext.Resource.NonComputedReadLink;
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
            return this.resourceMetadataContext.Resource.HasNonComputedId ? this.resourceMetadataContext.Resource.NonComputedId : (this.resourceMetadataContext.Resource.IsTransient ? null : this.ComputedId);
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
            if (this.resourceMetadataContext.Resource.HasNonComputedETag)
            {
                return this.resourceMetadataContext.Resource.NonComputedETag;
            }

            if (!this.etagComputed)
            {
                StringBuilder resultBuilder = new StringBuilder();
                foreach (KeyValuePair<string, object> etagProperty in this.resourceMetadataContext.ETagProperties)
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
            if (this.resourceMetadataContext.Resource.NonComputedMediaResource != null)
            {
                return this.resourceMetadataContext.Resource.NonComputedMediaResource;
            }

            if (this.computedMediaResource == null && this.resourceMetadataContext.TypeContext.IsMediaLinkEntry)
            {
                this.computedMediaResource = new ODataStreamReferenceValue();
                this.computedMediaResource.SetMetadataBuilder(this, /*propertyName*/ null);
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
            return ODataUtilsInternal.ConcatEnumerables(nonComputedProperties, this.GetComputedStreamProperties(nonComputedProperties));
        }

        /// <summary>
        /// Gets the list of computed and non-computed actions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed actions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataAction> GetActions()
        {
            return ODataUtilsInternal.ConcatEnumerables(this.resourceMetadataContext.Resource.NonComputedActions, this.MissingOperationGenerator.GetComputedActions());
        }

        /// <summary>
        /// Gets the list of computed and non-computed functions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed functions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataFunction> GetFunctions()
        {
            return ODataUtilsInternal.ConcatEnumerables(this.resourceMetadataContext.Resource.NonComputedFunctions, this.MissingOperationGenerator.GetComputedFunctions());
        }

        /// <summary>
        /// Marks the given nested resource info as processed.
        /// </summary>
        /// <param name="navigationPropertyName">The nested resource info we've already processed.</param>
        internal override void MarkNestedResourceInfoProcessed(string navigationPropertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(navigationPropertyName), "!string.IsNullOrEmpty(navigationPropertyName)");
            Debug.Assert(this.processedNestedResourceInfos != null, "this.processedNestedResourceInfos != null");
            this.processedNestedResourceInfos.Add(navigationPropertyName);
        }

        /// <summary>
        /// Returns the next unprocessed navigation link or null if there's no more navigation links to process.
        /// </summary>
        /// <returns>Returns the next unprocessed navigation link or null if there's no more navigation links to process.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override ODataJsonLightReaderNestedResourceInfo GetNextUnprocessedNavigationLink()
        {
            if (this.unprocessedNestedResourceInfos == null)
            {
                Debug.Assert(this.resourceMetadataContext != null, "this.resourceMetadataContext != null");
                this.unprocessedNestedResourceInfos = this.resourceMetadataContext.SelectedNavigationProperties
                    .Where(p => !this.processedNestedResourceInfos.Contains(p.Name))
                    .Select(ODataJsonLightReaderNestedResourceInfo.CreateProjectedNestedResourceInfo)
                    .GetEnumerator();
            }

            if (this.unprocessedNestedResourceInfos.MoveNext())
            {
                return this.unprocessedNestedResourceInfos.Current;
            }

            return null;
        }

        /// <summary>
        /// Gets the edit link of a stream value.
        /// </summary>
        /// <param name="streamPropertyName">The name of the stream property the edit link is computed for; 
        /// or null for the default media resource.</param>
        /// <returns>
        /// The absolute URI of the edit link for the specified stream property or the default media resource.
        /// Or null if it is not possible to determine the stream edit link.
        /// </returns>
        internal override Uri GetStreamEditLink(string streamPropertyName)
        {
            ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, "streamPropertyName");

            return this.uriBuilder.BuildStreamEditLinkUri(this.GetEditLink(), streamPropertyName);
        }

        /// <summary>
        /// Gets the read link of a stream value.
        /// </summary>
        /// <param name="streamPropertyName">The name of the stream property the read link is computed for; 
        /// or null for the default media resource.</param>
        /// <returns>
        /// The absolute URI of the read link for the specified stream property or the default media resource.
        /// Or null if it is not possible to determine the stream read link.
        /// </returns>
        internal override Uri GetStreamReadLink(string streamPropertyName)
        {
            ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, "streamPropertyName");

            return this.uriBuilder.BuildStreamReadLinkUri(this.GetReadLink(), streamPropertyName);
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

            return hasNestedResourceInfoUrl ? navigationLinkUrl : this.uriBuilder.BuildNavigationLinkUri(this.GetReadLink(), navigationPropertyName);
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

            return hasAssociationLinkUrl ? associationLinkUrl : this.uriBuilder.BuildAssociationLinkUri(this.GetReadLink(), navigationPropertyName);
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
            if (string.IsNullOrEmpty(bindingParameterTypeName) || this.resourceMetadataContext.Resource.NonComputedEditLink != null)
            {
                // if there is no parameter type name to append, or the edit-link is an opaque non-computed value, then use the edit-link as normal.
                baseUri = this.GetEditLink();
            }
            else
            {
                // Otherwise, use the computed URI which has no type segment
                baseUri = this.GetId();
            }

            return this.uriBuilder.BuildOperationTargetUri(baseUri, operationName, bindingParameterTypeName, parameterNames);
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
            id = this.resourceMetadataContext.Resource.IsTransient ? null : this.GetId();
            return true;
        }

        /// <summary>
        /// Computes the edit link.
        /// </summary>
        /// <returns>Uri that was computed based on the computed Id and possible type segment.</returns>
        private Uri ComputeEditLink()
        {
            Uri uri = this.resourceMetadataContext.Resource.HasNonComputedId ? this.resourceMetadataContext.Resource.NonComputedId : this.ComputedId;

            Debug.Assert(this.resourceMetadataContext != null && this.resourceMetadataContext.TypeContext != null, "this.resourceMetadataContext != null && this.resourceMetadataContext.TypeContext != null");
            if (this.resourceMetadataContext.ActualResourceTypeName != this.resourceMetadataContext.TypeContext.NavigationSourceEntityTypeName)
            {
                uri = this.uriBuilder.AppendTypeSegment(uri, this.resourceMetadataContext.ActualResourceTypeName);
            }

            return uri;
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
            switch (this.resourceMetadataContext.TypeContext.NavigationSourceKind)
            {
                case EdmNavigationSourceKind.Singleton:
                    uri = this.ComputeIdForSingleton();
                    break;
                case EdmNavigationSourceKind.ContainedEntitySet:
                    uri = this.ComputeIdForContainment();
                    break;
                case EdmNavigationSourceKind.UnknownEntitySet:
                    throw new ODataException(Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
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
            Uri uri = this.uriBuilder.BuildBaseUri();
            uri = this.uriBuilder.BuildEntitySetUri(uri, this.resourceMetadataContext.TypeContext.NavigationSourceName);
            uri = this.uriBuilder.BuildEntityInstanceUri(uri, this.ComputedKeyProperties, this.resourceMetadataContext.ActualResourceTypeName);
            return uri;
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

            // check if has parent
            ODataConventionalResourceMetadataBuilder parent = this.ParentMetadataBuilder as ODataConventionalResourceMetadataBuilder;
            if (parent != null)
            {
                // $expand scenario.  Get the parent id
                uri = parent.GetId();

                // And append cast (if needed).
                // A cast segment if the navigation property is defined on a type derived from the entity
                // type declared for the entity set
                IODataResourceTypeContext typeContext = parent.resourceMetadataContext.TypeContext;
                if (typeContext.NavigationSourceEntityTypeName != typeContext.ExpectedEntityTypeName)
                {
                    // Do not append type cast if we know that the navigation property is in base type, not in derived type.
                    ODataResourceTypeContext.ODataResourceTypeContextWithModel typeContextWithModel = typeContext as ODataResourceTypeContext.ODataResourceTypeContextWithModel;
                    if (typeContextWithModel == null || typeContextWithModel.NavigationSourceEntityType.FindProperty(this.resourceMetadataContext.TypeContext.NavigationSourceName) == null)
                    {
                        uri = new Uri(UriUtils.EnsureTaillingSlash(uri), parent.resourceMetadataContext.ActualResourceTypeName);    
                    }
                }
            }
            else
            {
                // direct access scenario.
                uri = this.uriBuilder.BuildBaseUri();
                ODataUri odataUri = this.ODataUri ?? this.metadataContext.ODataUri;
                uri = this.GetContainingEntitySetUri(uri, odataUri);
            }

            // A path segment for the containment navigation property
            uri = this.uriBuilder.BuildEntitySetUri(uri, this.resourceMetadataContext.TypeContext.NavigationSourceName);

            if (this.resourceMetadataContext.TypeContext.IsFromCollection)
            {
                uri = this.uriBuilder.BuildEntityInstanceUri(
                    uri,
                    this.ComputedKeyProperties,
                    this.resourceMetadataContext.ActualResourceTypeName);
            }

            return uri;
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
            Uri uri = this.uriBuilder.BuildBaseUri();
            uri = this.uriBuilder.BuildEntitySetUri(uri, this.resourceMetadataContext.TypeContext.NavigationSourceName);
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
                IDictionary<string, IEdmStructuralProperty> projectedStreamProperties = this.resourceMetadataContext.SelectedStreamProperties;
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
                        this.computedStreamProperties.Add(new ODataProperty { Name = missingStreamPropertyName, Value = streamPropertyValue });
                    }
                }
            }

            return this.computedStreamProperties;
        }

        /// <summary>
        /// Gets resource path from service root, and request Uri.
        /// </summary>
        /// <param name="baseUri">The service root Uri.</param>
        /// <param name="odataUri">The request Uri.</param>
        /// <returns>The resource path.</returns>
        private Uri GetContainingEntitySetUri(Uri baseUri, ODataUri odataUri)
        {
            if (odataUri == null || odataUri.Path == null)
            {
                throw new ODataException(Strings.ODataMetadataBuilder_MissingODataUri);
            }

            ODataPath path = odataUri.Path;
            List<ODataPathSegment> segments = path.ToList();
            ODataPathSegment lastSegment = segments.Last();
            while (!(lastSegment is NavigationPropertySegment))
            {
                segments.Remove(lastSegment);
                lastSegment = segments.Last();
            }

            // also removed the last navigation property segment
            segments.Remove(lastSegment);

            // Remove the unnecessary type cast segment.
            ODataPathSegment nextToLastSegment = segments.Last();
            while (nextToLastSegment is TypeSegment)
            {
                ODataPathSegment previousSegment = segments[segments.Count - 2];
                IEdmEntityType ownerType = previousSegment.TargetEdmType as IEdmEntityType;
                if (ownerType != null && ownerType.FindProperty(lastSegment.Identifier) != null)
                {
                    segments.Remove(nextToLastSegment);
                    nextToLastSegment = segments.Last();
                }
                else
                {
                    break;
                }
            }

            // append each segment to base uri
            Uri uri = baseUri;
            foreach (ODataPathSegment segment in segments)
            {
                var keySegment = segment as KeySegment;
                if (keySegment == null)
                {
                    uri = this.uriBuilder.BuildEntitySetUri(uri, segment.Identifier);
                }
                else
                {
                    uri = this.uriBuilder.BuildEntityInstanceUri(
                        uri,
                        keySegment.Keys.ToList(),
                        keySegment.EdmType.FullTypeName());
                }
            }

            return uri;
        }
    }
}
