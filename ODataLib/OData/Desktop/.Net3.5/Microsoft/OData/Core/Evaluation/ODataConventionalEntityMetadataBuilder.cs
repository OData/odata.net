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
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Core.UriParser;
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
        private string computedId;

        /// <summary>A computed uri that is equivalent to the ID or the edit-link without a type segment.</summary>
        private Uri computedEntityInstanceUri;

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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryMetadataContext != null, "entryMetadataContext != null");
            Debug.Assert(metadataContext != null, "metadataContext != null");
            Debug.Assert(uriBuilder != null, "uriBuilder != null");

            this.entryMetadataContext = entryMetadataContext;
            this.uriBuilder = uriBuilder;
            this.metadataContext = metadataContext;
            this.processedNavigationLinks = new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Lazy evaluated computed entity Id. This is always a computed value and never comes from the entry.
        /// </summary>
        private string ComputedId
        {
            get
            {
                this.ComputeAndCacheId();
                return this.computedId;
            }
        }

        /// <summary>
        /// Lazy evaluated computed entity instance uri. This is always a computed value and never comes from the entry.
        /// </summary>
        private Uri ComputedEntityInstanceUri
        {
            get
            {
                this.ComputeAndCacheId();
                return this.computedEntityInstanceUri;
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
            DebugUtils.CheckNoExternalCallers();
            return this.entryMetadataContext.Entry.HasNonComputedEditLink ? this.entryMetadataContext.Entry.NonComputedEditLink : (this.computedEditLink ?? (this.computedEditLink = this.ComputeEditLink()));
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
            DebugUtils.CheckNoExternalCallers();
            return this.entryMetadataContext.Entry.HasNonComputedReadLink ? this.entryMetadataContext.Entry.NonComputedReadLink : (this.computedReadLink ?? (this.computedReadLink = this.GetEditLink()));
        }

        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        /// <returns>
        /// The ID for the entity.
        /// Or null if it is not possible to determine the ID.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override string GetId()
        {
            DebugUtils.CheckNoExternalCallers();

            // If the Id, ReadLink, or EditLink were on the wire (in that order), use that wire value for the Id.
            // Otherwise compute it based on the key values. We specifically do not want to use the ReadLink/EditLink
            // values if they were already previously computed instead of just being directly set on the entry.
            if (this.entryMetadataContext.Entry.HasNonComputedId)
            {
                return this.entryMetadataContext.Entry.NonComputedId;
            }

            if (this.entryMetadataContext.Entry.HasNonComputedReadLink)
            {
                return UriUtilsCommon.UriToString(this.entryMetadataContext.Entry.NonComputedReadLink);
            }

            if (this.entryMetadataContext.Entry.NonComputedEditLink != null)
            {
                return UriUtilsCommon.UriToString(this.entryMetadataContext.Entry.NonComputedEditLink);
            }

            return this.ComputedId;
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            return ODataUtilsInternal.ConcatEnumerables(nonComputedProperties, this.GetComputedStreamProperties(nonComputedProperties));
        }

        /// <summary>
        /// Gets the list of computed and non-computed actions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed actions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataAction> GetActions()
        {
            DebugUtils.CheckNoExternalCallers();
            return ODataUtilsInternal.ConcatEnumerables(this.entryMetadataContext.Entry.NonComputedActions, this.MissingOperationGenerator.GetComputedActions());
        }

        /// <summary>
        /// Gets the list of computed and non-computed functions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed functions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal override IEnumerable<ODataFunction> GetFunctions()
        {
            DebugUtils.CheckNoExternalCallers();
            return ODataUtilsInternal.ConcatEnumerables(this.entryMetadataContext.Entry.NonComputedFunctions, this.MissingOperationGenerator.GetComputedFunctions());
        }

        /// <summary>
        /// Marks the given navigation link as processed.
        /// </summary>
        /// <param name="navigationPropertyName">The navigation link we've already processed.</param>
        internal override void MarkNavigationLinkProcessed(string navigationPropertyName)
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            Uri baseUri;
            if (string.IsNullOrEmpty(bindingParameterTypeName) || this.entryMetadataContext.Entry.NonComputedEditLink != null)
            {
                // if there is no parameter type name to append, or the edit-link is an opaque non-computed value, then use the edit-link as normal.
                baseUri = this.GetEditLink();
            }
            else
            {
                // Otherwise, use the computed entity-instance URI which has no type segment
                baseUri = this.ComputedEntityInstanceUri;
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            // TODO TASK 905785: What is the convention for operation title?
            return operationName;
        }

        /// <summary>
        /// Computes the edit link.
        /// </summary>
        /// <returns>Uri that was computed based on the computed Id and possible type segment.</returns>
        private Uri ComputeEditLink()
        {
            Uri uri = this.ComputedEntityInstanceUri;

            Debug.Assert(this.entryMetadataContext != null && this.entryMetadataContext.TypeContext != null, "this.entryMetadataContext != null && this.entryMetadataContext.TypeContext != null");
            if (this.entryMetadataContext.ActualEntityTypeName != this.entryMetadataContext.TypeContext.EntitySetElementTypeName)
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
            if (this.computedEntityInstanceUri == null)
            {
                Uri uri = this.uriBuilder.BuildBaseUri();
                uri = this.uriBuilder.BuildEntitySetUri(uri, this.entryMetadataContext.TypeContext.EntitySetName);
                uri = this.uriBuilder.BuildEntityInstanceUri(uri, this.entryMetadataContext.KeyProperties, this.entryMetadataContext.ActualEntityTypeName);

                this.computedEntityInstanceUri = uri;
                this.computedId = UriUtilsCommon.UriToString(uri);
            }
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
    }
}
