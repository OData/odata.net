//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Evaluation
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    #endregion

    /// <summary>
    /// Implementation of OData entity metadata builder based on OData protocol conventions.
    /// </summary>
    internal sealed class ODataConventionalEntityMetadataBuilder : ODataEntityMetadataBuilder
    {
        /// <summary>The URI builder to use.</summary>
        private readonly ODataUriBuilder uriBuilder;

        /// <summary>The context to answer basic metadata questions about the entry.</summary>
        private readonly IODataEntryMetadataContext entryMetadataContext;

        /// <summary>The metadata context.</summary>
        private readonly IODataMetadataContext metadataContext;

        /// <summary>The list of navigation links that have been processed.</summary>
        private readonly HashSet<string> processedNavigationLinks;

        /// <summary>The edit link.</summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the entry or computed.</remarks>
        private Uri computedEditLink;

        /// <summary>The read link.</summary>
        /// <remarks>This is lazily evaluated. It may be retrieved from the entry or computed.</remarks>
        private Uri computedReadLink;

        /// <summary>The computed ETag.</summary>
        private string computedETag;

        /// <summary>true if the etag value has been computed, false otherwise.</summary>
        private bool etagComputed;

        /// <summary>The computed ID of this entity instance.</summary>
        /// <remarks>
        /// This is always built from the key properties, and never comes from the entry.
        /// </remarks>
        private Uri computedId;

        /// <summary>The computed MediaResource for MLEs.</summary>
        private ODataStreamReferenceValue computedMediaResource;

        /// <summary>The list of computed stream properties.</summary>
        private List<ODataProperty> computedStreamProperties;

        /// <summary>The enumerator for unprocessed navigation links.</summary>
        private IEnumerator<ODataJsonLightReaderNavigationLinkInfo> unprocessedNavigationLinks;

        /// <summary>The missing operation generator for the current entry.</summary>
        private ODataMissingOperationGenerator missingOperationGenerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entryMetadataContext">The context to answer basic metadata questions about the entry.</param>
        /// <param name="metadataContext">The metadata context.</param>
        /// <param name="uriBuilder">The uri builder to use.</param>
        internal ODataConventionalEntityMetadataBuilder(IODataEntryMetadataContext entryMetadataContext, IODataMetadataContext metadataContext, ODataUriBuilder uriBuilder)
        {
            Debug.Assert(entryMetadataContext != null, "entryMetadataContext != null");
            Debug.Assert(metadataContext != null, "metadataContext != null");
            Debug.Assert(uriBuilder != null, "uriBuilder != null");

            this.entryMetadataContext = entryMetadataContext;
            this.uriBuilder = uriBuilder;
            this.metadataContext = metadataContext;
            this.processedNavigationLinks = new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// OData uri that parsed based on the context url
        /// </summary>
        internal ODataUri ODataUri { get; set; }

        /// <summary>
        /// Lazy evaluated computed entity Id. This is always a computed value and never comes from the entry.
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
        /// The missig operation generator for the current entry.
        /// </summary>
        private ODataMissingOperationGenerator MissingOperationGenerator
        {
            get { return this.missingOperationGenerator ?? (this.missingOperationGenerator = new ODataMissingOperationGenerator(this.entryMetadataContext, this.metadataContext)); }
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
            if (this.entryMetadataContext.Entry.HasNonComputedEditLink)
            {
                return this.entryMetadataContext.Entry.NonComputedEditLink;
            }
            else
            {
                // For readonly entity if ReadLink is there and EditLink is null, we should not calculate the EditLink in both serializer and deserializer.
                if (this.entryMetadataContext.Entry.IsTransient || this.entryMetadataContext.Entry.HasNonComputedReadLink)
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
            if (this.entryMetadataContext.Entry.HasNonComputedReadLink)
            {
                return this.entryMetadataContext.Entry.NonComputedReadLink;
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
            // If the entry was transient entry, return null for Id
            // Otherwise compute it based on the key values.
            return this.entryMetadataContext.Entry.HasNonComputedId ? this.entryMetadataContext.Entry.NonComputedId : (this.entryMetadataContext.Entry.IsTransient ? null : this.ComputedId);
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
            if (this.entryMetadataContext.Entry.HasNonComputedETag)
            {
                return this.entryMetadataContext.Entry.NonComputedETag;
            }

            if (!this.etagComputed)
            {
                StringBuilder resultBuilder = new StringBuilder();
                foreach (KeyValuePair<string, object> etagProperty in this.entryMetadataContext.ETagProperties)
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
            if (this.entryMetadataContext.Entry.NonComputedMediaResource != null)
            {
                return this.entryMetadataContext.Entry.NonComputedMediaResource;
            }

            if (this.computedMediaResource == null && this.entryMetadataContext.TypeContext.IsMediaLinkEntry)
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
            return ODataUtilsInternal.ConcatEnumerables(this.entryMetadataContext.Entry.NonComputedActions, this.MissingOperationGenerator.GetComputedActions());
        }

        /// <summary>
        /// Gets the list of computed and non-computed functions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed functions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataFunction> GetFunctions()
        {
            return ODataUtilsInternal.ConcatEnumerables(this.entryMetadataContext.Entry.NonComputedFunctions, this.MissingOperationGenerator.GetComputedFunctions());
        }

        /// <summary>
        /// Marks the given navigation link as processed.
        /// </summary>
        /// <param name="navigationPropertyName">The navigation link we've already processed.</param>
        internal override void MarkNavigationLinkProcessed(string navigationPropertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(navigationPropertyName), "!string.IsNullOrEmpty(navigationPropertyName)");
            Debug.Assert(this.processedNavigationLinks != null, "this.processedNavigationLinks != null");
            this.processedNavigationLinks.Add(navigationPropertyName);
        }

        /// <summary>
        /// Returns the next unprocessed navigation link or null if there's no more navigation links to process.
        /// </summary>
        /// <returns>Returns the next unprocessed navigation link or null if there's no more navigation links to process.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override ODataJsonLightReaderNavigationLinkInfo GetNextUnprocessedNavigationLink()
        {
            if (this.unprocessedNavigationLinks == null)
            {
                Debug.Assert(this.entryMetadataContext != null, "this.entryMetadataContext != null");
                this.unprocessedNavigationLinks = this.entryMetadataContext.SelectedNavigationProperties
                    .Where(p => !this.processedNavigationLinks.Contains(p.Name))
                    .Select(ODataJsonLightReaderNavigationLinkInfo.CreateProjectedNavigationLinkInfo)
                    .GetEnumerator();
            }

            if (this.unprocessedNavigationLinks.MoveNext())
            {
                return this.unprocessedNavigationLinks.Current;
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
        /// <param name="hasNavigationLinkUrl">true if the value of the <paramref name="navigationLinkUrl"/> was seen on the wire or provided explicitly by the user or previously returned by
        /// the metadata builder, false otherwise. This flag allows the metadata builder to determine whether a null navigation link url is an uninitialized value or a value that was set explicitly.</param>
        /// <returns>
        /// The navigation link URI for the navigation property.
        /// null if its not possible to determine the navigation link for the specified navigation property.
        /// </returns>
        internal override Uri GetNavigationLinkUri(string navigationPropertyName, Uri navigationLinkUrl, bool hasNavigationLinkUrl)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");

            return hasNavigationLinkUrl ? navigationLinkUrl : this.uriBuilder.BuildNavigationLinkUri(this.GetEditLink(), navigationPropertyName);
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

            return hasAssociationLinkUrl ? associationLinkUrl : this.uriBuilder.BuildAssociationLinkUri(this.GetEditLink(), navigationPropertyName);
        }

        /// <summary>
        /// Get the operation target URI for the specified <paramref name="operationName"/>.
        /// </summary>
        /// <param name="operationName">The fully qualified name of the operation for which to get the target URI.</param>
        /// <param name="bindingParameterTypeName">The binding parameter type name to include in the target, or null/empty if there is none.</param>
        /// <returns>
        /// The target URI for the operation.
        /// null if it is not possible to determine the target URI for the specified operation.
        /// </returns>
        internal override Uri GetOperationTargetUri(string operationName, string bindingParameterTypeName)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            Uri baseUri;
            if (string.IsNullOrEmpty(bindingParameterTypeName) || this.entryMetadataContext.Entry.NonComputedEditLink != null)
            {
                // if there is no parameter type name to append, or the edit-link is an opaque non-computed value, then use the edit-link as normal.
                baseUri = this.GetEditLink();
            }
            else
            {
                // Otherwise, use the computed URI which has no type segment
                baseUri = this.ComputedId;
            }

            return this.uriBuilder.BuildOperationTargetUri(baseUri, operationName, bindingParameterTypeName);
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
            id = this.entryMetadataContext.Entry.IsTransient ? null : this.GetId();
            return true;
        }

        /// <summary>
        /// Computes the edit link.
        /// </summary>
        /// <returns>Uri that was computed based on the computed Id and possible type segment.</returns>
        private Uri ComputeEditLink()
        {
            Uri uri = this.entryMetadataContext.Entry.HasNonComputedId ? this.entryMetadataContext.Entry.NonComputedId : this.ComputedId;

            Debug.Assert(this.entryMetadataContext != null && this.entryMetadataContext.TypeContext != null, "this.entryMetadataContext != null && this.entryMetadataContext.TypeContext != null");
            if (this.entryMetadataContext.ActualEntityTypeName != this.entryMetadataContext.TypeContext.NavigationSourceEntityTypeName)
            {
                uri = this.uriBuilder.AppendTypeSegment(uri, this.entryMetadataContext.ActualEntityTypeName);
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
            switch (this.entryMetadataContext.TypeContext.NavigationSourceKind)
            {
                case EdmNavigationSourceKind.Singleton:
                    uri = this.ComputeIdForSingleton();
                    break;
                case EdmNavigationSourceKind.ContainedEntitySet:
                    uri = this.ComputeIdForContainment();
                    break;
                case EdmNavigationSourceKind.UnknownEntitySet:
                    throw new ODataException(OData.Core.Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
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
            uri = this.uriBuilder.BuildEntitySetUri(uri, this.entryMetadataContext.TypeContext.NavigationSourceName);
            uri = this.uriBuilder.BuildEntityInstanceUri(uri, this.entryMetadataContext.KeyProperties, this.entryMetadataContext.ActualEntityTypeName);
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
            ODataConventionalEntityMetadataBuilder parent = this.ParentMetadataBuilder as ODataConventionalEntityMetadataBuilder;
            if (parent != null)
            {
                // $expand scenario.  Get the parent id
                uri = parent.GetId();

                // And append cast (if needed).
                // A cast segment if the navigation property is defined on a type derived from the entity
                // type declared for the entity set
                IODataFeedAndEntryTypeContext typeContext = parent.entryMetadataContext.TypeContext;
                if (typeContext.NavigationSourceEntityTypeName != typeContext.ExpectedEntityTypeName)
                {
                    // Do not append type cast if we know that the navigation property is in base type, not in derived type.
                    ODataFeedAndEntryTypeContext.ODataFeedAndEntryTypeContextWithModel typeContextWithModel = typeContext as ODataFeedAndEntryTypeContext.ODataFeedAndEntryTypeContextWithModel;
                    if (typeContextWithModel == null || typeContextWithModel.NavigationSourceEntityType.FindProperty(this.entryMetadataContext.TypeContext.NavigationSourceName) == null)
                    {
                        uri = new Uri(Core.UriUtils.EnsureTaillingSlash(uri), parent.entryMetadataContext.ActualEntityTypeName);    
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
            uri = this.uriBuilder.BuildEntitySetUri(uri, this.entryMetadataContext.TypeContext.NavigationSourceName);

            if (this.entryMetadataContext.TypeContext.IsFromCollection)
            {
                uri = this.uriBuilder.BuildEntityInstanceUri(
                    uri,
                    this.entryMetadataContext.KeyProperties,
                    this.entryMetadataContext.ActualEntityTypeName);
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
            uri = this.uriBuilder.BuildEntitySetUri(uri, this.entryMetadataContext.TypeContext.NavigationSourceName);
            return uri;
        }

        /// <summary>
        /// Computes all projected or missing stream properties.
        /// </summary>
        /// <param name="nonComputedProperties">Non-computed properties from the entity.</param>
        /// <returns>The the computed stream properties for the entry.</returns>
        private IEnumerable<ODataProperty> GetComputedStreamProperties(IEnumerable<ODataProperty> nonComputedProperties)
        {
            if (this.computedStreamProperties == null)
            {
                // Remove all the projected properties that were already read from the payload
                IDictionary<string, IEdmStructuralProperty> projectedStreamProperties = this.entryMetadataContext.SelectedStreamProperties;
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
                throw new ODataException(OData.Core.Strings.ODataMetadataBuilder_MissingODataUri);
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
