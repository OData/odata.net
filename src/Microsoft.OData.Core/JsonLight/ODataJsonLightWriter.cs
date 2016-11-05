//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the ODataWriter for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightWriter : ODataWriterCore
    {
        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataJsonLightOutputContext jsonLightOutputContext;

        /// <summary>
        /// The JsonLight entry and feed serializer to use.
        /// </summary>
        private readonly ODataJsonLightEntryAndFeedSerializer jsonLightEntryAndFeedSerializer;

        /// <summary>
        /// True if the writer was created for writing a parameter; false otherwise.
        /// </summary>
        private readonly bool writingParameter;

        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// OData annotation writer.
        /// </summary>
        private readonly JsonLightODataAnnotationWriter odataAnnotationWriter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="writingFeed">true if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        /// <param name="writingParameter">true if the writer is created for writing a parameter; false otherwise.</param>
        /// <param name="writingDelta">True if the writer is created for writing delta response; false otherwise.</param>
        /// <param name="listener">If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</param>
        internal ODataJsonLightWriter(
            ODataJsonLightOutputContext jsonLightOutputContext,
            IEdmNavigationSource navigationSource,
            IEdmEntityType entityType,
            bool writingFeed,
            bool writingParameter = false,
            bool writingDelta = false,
            IODataReaderWriterListener listener = null)
            : base(jsonLightOutputContext, navigationSource, entityType, writingFeed, writingDelta, listener)
        {
            Debug.Assert(jsonLightOutputContext != null, "jsonLightOutputContext != null");

            this.jsonLightOutputContext = jsonLightOutputContext;
            this.jsonLightEntryAndFeedSerializer = new ODataJsonLightEntryAndFeedSerializer(this.jsonLightOutputContext);

            this.writingParameter = writingParameter;
            this.jsonWriter = this.jsonLightOutputContext.JsonWriter;
            this.odataAnnotationWriter = new JsonLightODataAnnotationWriter(this.jsonWriter, jsonLightOutputContext.MessageWriterSettings.ODataSimplified);
        }

        /// <summary>
        /// Returns the current JsonLightEntryScope.
        /// </summary>
        private JsonLightEntryScope CurrentEntryScope
        {
            get
            {
                JsonLightEntryScope currentJsonLightEntryScope = this.CurrentScope as JsonLightEntryScope;
                Debug.Assert(currentJsonLightEntryScope != null, "Asking for JsonLightEntryScope when the current scope is not an JsonLightEntryScope.");
                return currentJsonLightEntryScope;
            }
        }

        /// <summary>
        /// Returns the current JsonLightFeedScope.
        /// </summary>
        private JsonLightFeedScope CurrentFeedScope
        {
            get
            {
                JsonLightFeedScope currentJsonLightFeedScope = this.CurrentScope as JsonLightFeedScope;
                Debug.Assert(currentJsonLightFeedScope != null, "Asking for JsonFeedScope when the current scope is not a JsonFeedScope.");
                return currentJsonLightFeedScope;
            }
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        protected override void VerifyNotDisposed()
        {
            this.jsonLightOutputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.jsonLightOutputContext.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected override Task FlushAsynchronously()
        {
            return this.jsonLightOutputContext.FlushAsync();
        }
#endif

        /// <summary>
        /// Starts writing a payload (called exactly once before anything else)
        /// </summary>
        protected override void StartPayload()
        {
            this.jsonLightEntryAndFeedSerializer.WritePayloadStart();
        }

        /// <summary>
        /// Ends writing a payload (called exactly once after everything else in case of success)
        /// </summary>
        protected override void EndPayload()
        {
            this.jsonLightEntryAndFeedSerializer.WritePayloadEnd();
        }

        /// <summary>
        /// Place where derived writers can perform custom steps before the entry is writen, at the begining of WriteStartEntryImplementation.
        /// </summary>
        /// <param name="entry">Entry to write.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry or feed.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        protected override void PrepareEntryForWriteStart(ODataEntry entry, ODataFeedAndEntryTypeContext typeContext, SelectedPropertiesNode selectedProperties)
        {
            if (this.jsonLightOutputContext.MessageWriterSettings.AutoComputePayloadMetadataInJson)
            {
                EntryScope entryScope = (EntryScope)this.CurrentScope;
                Debug.Assert(entryScope != null, "entryScope != null");

                ODataEntityMetadataBuilder builder = this.jsonLightOutputContext.MetadataLevel.CreateEntityMetadataBuilder(
                    entry,
                    typeContext,
                    entryScope.SerializationInfo,
                    entryScope.EntityType,
                    selectedProperties,
                    this.jsonLightOutputContext.WritingResponse,
                    this.jsonLightOutputContext.MessageWriterSettings.UseKeyAsSegment,
                    this.jsonLightOutputContext.MessageWriterSettings.ODataUri);

                if (builder is ODataConventionalEntityMetadataBuilder)
                {
                    builder.ParentMetadataBuilder = this.FindParentEntryMetadataBuilder();
                }

                this.jsonLightOutputContext.MetadataLevel.InjectMetadataBuilder(entry, builder);
            }
        }

        /// <summary>
        /// Validates the media resource on the entry.
        /// </summary>
        /// <param name="entry">The entry to validate.</param>
        /// <param name="entityType">The entity type of the entry.</param>
        protected override void ValidateEntryMediaResource(ODataEntry entry, IEdmEntityType entityType)
        {
            if (!this.jsonLightOutputContext.MessageWriterSettings.EnableFullValidation || (this.jsonLightOutputContext.MessageWriterSettings.AutoComputePayloadMetadataInJson && this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel))
            {
                // entry.MediaResource is always null for NoMetadata mode. Skip the media resource validation.
            }
            else
            {
                base.ValidateEntryMediaResource(entry, entityType);
            }
        }

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected override void StartEntry(ODataEntry entry)
        {
            ODataNavigationLink parentNavLink = this.ParentNavigationLink;
            if (parentNavLink != null)
            {
                // Write the property name of an expanded navigation property to start the value. 
                this.jsonWriter.WriteName(parentNavLink.Name);
            }

            if (entry == null)
            {
                Debug.Assert(
                    parentNavLink != null && !parentNavLink.IsCollection.Value,
                        "when entry == null, it has to be and expanded single entry navigation");

                // this is a null expanded single entry and it is null, so write a JSON null as value.
                this.jsonWriter.WriteValue((string)null);
                return;
            }

            // Write just the object start, nothing else, since we might not have complete information yet
            this.jsonWriter.StartObjectScope();

            JsonLightEntryScope entryScope = this.CurrentEntryScope;

            if (this.IsTopLevel)
            {
                // Write odata.context
                this.jsonLightEntryAndFeedSerializer.WriteEntryContextUri(entryScope.GetOrCreateTypeContext(this.jsonLightOutputContext.Model, this.jsonLightOutputContext.WritingResponse));
            }

            // Write the metadata
            this.jsonLightEntryAndFeedSerializer.WriteEntryStartMetadataProperties(entryScope);
            this.jsonLightEntryAndFeedSerializer.WriteEntryMetadataProperties(entryScope);

            // Write custom instance annotations
            this.jsonLightEntryAndFeedSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations(entry.InstanceAnnotations, entryScope.InstanceAnnotationWriteTracker);

            // Write the properties
            ProjectedPropertiesAnnotation projectedProperties = GetProjectedPropertiesAnnotation(entryScope);

            this.jsonLightEntryAndFeedSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();
            this.jsonLightEntryAndFeedSerializer.WriteProperties(
                this.EntryEntityType,
                entry.Properties,
                false /* isComplexValue */,
                this.DuplicatePropertyNamesChecker,
                projectedProperties);
            this.jsonLightEntryAndFeedSerializer.JsonLightValueSerializer.AssertRecursionDepthIsZero();

            // COMPAT 48: Position of navigation properties/links in JSON differs.
        }

        /// <summary>
        /// Finish writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected override void EndEntry(ODataEntry entry)
        {
            if (entry == null)
            {
                Debug.Assert(
                    this.ParentNavigationLink != null && this.ParentNavigationLink.IsCollection.HasValue && !this.ParentNavigationLink.IsCollection.Value,
                        "when entry == null, it has to be and expanded single entry navigation");

                // this is a null expanded single entry and it is null, JSON null should be written as value in StartEntry()
                return;
            }

            // Get the projected properties
            JsonLightEntryScope entryScope = this.CurrentEntryScope;

            this.jsonLightEntryAndFeedSerializer.WriteEntryMetadataProperties(entryScope);

            // Write custom instance annotations
            this.jsonLightEntryAndFeedSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations(entry.InstanceAnnotations, entryScope.InstanceAnnotationWriteTracker);

            this.jsonLightEntryAndFeedSerializer.WriteEntryEndMetadataProperties(entryScope, entryScope.DuplicatePropertyNamesChecker);

            // Close the object scope
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected override void StartFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            if (this.ParentNavigationLink == null && this.writingParameter)
            {
                // Start array which will hold the entries in the feed.
                this.jsonWriter.StartArrayScope();
            }
            else if (this.ParentNavigationLink == null)
            {
                // Top-level feed.
                // "{"
                this.jsonWriter.StartObjectScope();

                // @odata.context
                this.jsonLightEntryAndFeedSerializer.WriteFeedContextUri(this.CurrentFeedScope.GetOrCreateTypeContext(this.jsonLightOutputContext.Model, this.jsonLightOutputContext.WritingResponse));

                if (this.jsonLightOutputContext.WritingResponse)
                {
                    // write "odata.actions" metadata
                    IEnumerable<ODataAction> actions = feed.Actions;
                    if (actions != null && actions.Any())
                    {
                        this.jsonLightEntryAndFeedSerializer.WriteOperations(actions.Cast<ODataOperation>(), /*isAction*/ true);
                    }

                    // write "odata.functions" metadata
                    IEnumerable<ODataFunction> functions = feed.Functions;
                    if (functions != null && functions.Any())
                    {
                        this.jsonLightEntryAndFeedSerializer.WriteOperations(functions.Cast<ODataOperation>(), /*isAction*/ false);
                    }

                    // Write the inline count if it's available.
                    this.WriteFeedCount(feed, /*propertyName*/null);

                    // Write the next link if it's available.
                    this.WriteFeedNextLink(feed, /*propertyName*/null);

                    // Write the delta link if it's available.
                    this.WriteFeedDeltaLink(feed);
                }

                // Write custom instance annotations
                this.jsonLightEntryAndFeedSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations(feed.InstanceAnnotations, this.CurrentFeedScope.InstanceAnnotationWriteTracker);

                // "value":
                this.jsonWriter.WriteValuePropertyName();

                // Start array which will hold the entries in the feed.
                this.jsonWriter.StartArrayScope();
            }
            else
            {
                // Expanded feed.
                Debug.Assert(
                    this.ParentNavigationLink != null && this.ParentNavigationLink.IsCollection.HasValue && this.ParentNavigationLink.IsCollection.Value,
                    "We should have verified that feeds can only be written into IsCollection = true links in requests.");
                string propertyName = this.ParentNavigationLink.Name;

                this.ValidateNoDeltaLinkForExpandedFeed(feed);
                this.ValidateNoCustomInstanceAnnotationsForExpandedFeed(feed);

                if (this.jsonLightOutputContext.WritingResponse)
                {
                    // Write the inline count if it's available.
                    this.WriteFeedCount(feed, propertyName);

                    // Write the next link if it's available.
                    this.WriteFeedNextLink(feed, propertyName);

                    // And then write the property name to start the value. 
                    this.jsonWriter.WriteName(propertyName);

                    // Start array which will hold the entries in the feed.
                    this.jsonWriter.StartArrayScope();
                }
                else
                {
                    JsonLightNavigationLinkScope navigationLinkScope = (JsonLightNavigationLinkScope)this.ParentNavigationLinkScope;
                    if (!navigationLinkScope.FeedWritten)
                    {
                        // Close the entity reference link array (if written)
                        if (navigationLinkScope.EntityReferenceLinkWritten)
                        {
                            this.jsonWriter.EndArrayScope();
                        }

                        // And then write the property name to start the value. 
                        this.jsonWriter.WriteName(propertyName);

                        // Start array which will hold the entries in the feed.
                        this.jsonWriter.StartArrayScope();

                        navigationLinkScope.FeedWritten = true;
                    }
                }
            }
        }

        /// <summary>
        /// Finish writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected override void EndFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            if (this.ParentNavigationLink == null && this.writingParameter)
            {
                // End the array which holds the entries in the feed.
                this.jsonWriter.EndArrayScope();
            }
            else if (this.ParentNavigationLink == null)
            {
                // End the array which holds the entries in the feed.
                this.jsonWriter.EndArrayScope();

                // Write custom instance annotations
                this.jsonLightEntryAndFeedSerializer.InstanceAnnotationWriter.WriteInstanceAnnotations(feed.InstanceAnnotations, this.CurrentFeedScope.InstanceAnnotationWriteTracker);

                if (this.jsonLightOutputContext.WritingResponse)
                {
                    // Write the next link if it's available.
                    this.WriteFeedNextLink(feed, /*propertyName*/null);

                    // Write the delta link if it's available.
                    this.WriteFeedDeltaLink(feed);
                }

                // Close the object wrapper.
                this.jsonWriter.EndObjectScope();
            }
            else
            {
                Debug.Assert(
                    this.ParentNavigationLink != null && this.ParentNavigationLink.IsCollection.HasValue && this.ParentNavigationLink.IsCollection.Value,
                    "We should have verified that feeds can only be written into IsCollection = true links in requests.");
                string propertyName = this.ParentNavigationLink.Name;

                this.ValidateNoDeltaLinkForExpandedFeed(feed);
                this.ValidateNoCustomInstanceAnnotationsForExpandedFeed(feed);

                if (this.jsonLightOutputContext.WritingResponse)
                {
                    // End the array which holds the entries in the feed.
                    // NOTE: in requests we will only write the EndArray of a feed 
                    //       when we hit the navigation link end since a navigation link
                    //       can contain multiple feeds that get collapesed into a single array value.
                    this.jsonWriter.EndArrayScope();

                    // Write the next link if it's available.
                    this.WriteFeedNextLink(feed, propertyName);
                }
            }
        }

        /// <summary>
        /// Start writing a deferred (non-expanded) navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected override void WriteDeferredNavigationLink(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(this.jsonLightOutputContext.WritingResponse, "Deferred links are only supported in response, we should have verified this already.");

            // A deferred navigation link is just the link metadata, no value.
            this.jsonLightEntryAndFeedSerializer.WriteNavigationLinkMetadata(navigationLink, this.DuplicatePropertyNamesChecker);
        }

        /// <summary>
        /// Start writing a navigation link with content.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected override void StartNavigationLinkWithContent(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "The navigation link name should have been verified by now.");

            if (this.jsonLightOutputContext.WritingResponse)
            {
                // Write @odata.context annotation for navigation property
                var containedEntitySet = this.CurrentScope.NavigationSource as IEdmContainedEntitySet;
                if (containedEntitySet != null)
                {
                    ODataContextUrlInfo info = ODataContextUrlInfo.Create(
                                                this.CurrentScope.NavigationSource,
                                                this.CurrentScope.EntityType.FullName(),
                                                containedEntitySet.NavigationProperty.Type.TypeKind() != EdmTypeKind.Collection,
                                                this.CurrentScope.ODataUri);
                    this.jsonLightEntryAndFeedSerializer.WriteNavigationLinkContextUrl(navigationLink, info);
                }

                // Write the navigation link metadata first. The rest is written by the content entry or feed.
                this.jsonLightEntryAndFeedSerializer.WriteNavigationLinkMetadata(navigationLink, this.DuplicatePropertyNamesChecker);
            }
            else
            {
                this.WriterValidator.ValidateNavigationLinkHasCardinality(navigationLink);
            }
        }

        /// <summary>
        /// Finish writing a navigation link with content.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected override void EndNavigationLinkWithContent(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            if (!this.jsonLightOutputContext.WritingResponse)
            {
                JsonLightNavigationLinkScope navigationLinkScope = (JsonLightNavigationLinkScope)this.CurrentScope;

                // If we wrote entity reference links for a collection navigation property but no 
                // feed afterwards, we have to now close the array of links.
                if (navigationLinkScope.EntityReferenceLinkWritten && !navigationLinkScope.FeedWritten && navigationLink.IsCollection.Value)
                {
                    this.jsonWriter.EndArrayScope();
                }

                // In requests, the navigation link may have multiple entries in multiple feeds in it; if we 
                // wrote at least one feed, close the resulting array here.
                if (navigationLinkScope.FeedWritten)
                {
                    Debug.Assert(navigationLink.IsCollection.Value, "navigationLink.IsCollection.Value");
                    this.jsonWriter.EndArrayScope();
                }
            }
        }

        /// <summary>
        /// Write an entity reference link.
        /// </summary>
        /// <param name="parentNavigationLink">The parent navigation link which is being written around the entity reference link.</param>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        protected override void WriteEntityReferenceInNavigationLinkContent(ODataNavigationLink parentNavigationLink, ODataEntityReferenceLink entityReferenceLink)
        {
            Debug.Assert(parentNavigationLink != null, "parentNavigationLink != null");
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");
            Debug.Assert(!this.jsonLightOutputContext.WritingResponse, "Entity reference links are only supported in request, we should have verified this already.");

            // In JSON Light, we can only write entity reference links at the beginning of a navigation link in requests;
            // once we wrote a feed, entity reference links are not allowed anymore (we require all the entity reference
            // link to come first because of the grouping in the JSON Light wire format).
            JsonLightNavigationLinkScope navigationLinkScope = (JsonLightNavigationLinkScope)this.CurrentScope;
            if (navigationLinkScope.FeedWritten)
            {
                throw new ODataException(OData.Core.Strings.ODataJsonLightWriter_EntityReferenceLinkAfterFeedInRequest);
            }

            if (!navigationLinkScope.EntityReferenceLinkWritten)
            {
                // Write the property annotation for the entity reference link(s)
                this.odataAnnotationWriter.WritePropertyAnnotationName(parentNavigationLink.Name, ODataAnnotationNames.ODataBind);
                Debug.Assert(parentNavigationLink.IsCollection.HasValue, "parentNavigationLink.IsCollection.HasValue");
                if (parentNavigationLink.IsCollection.Value)
                {
                    this.jsonWriter.StartArrayScope();
                }

                navigationLinkScope.EntityReferenceLinkWritten = true;
            }

            Debug.Assert(entityReferenceLink.Url != null, "The entity reference link Url should have been validated by now.");
            this.jsonWriter.WriteValue(this.jsonLightEntryAndFeedSerializer.UriToString(entityReferenceLink.Url));
        }

        /// <summary>
        /// Create a new feed scope.
        /// </summary>
        /// <param name="feed">The feed for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly create scope.</returns>
        protected override FeedScope CreateFeedScope(ODataFeed feed, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightFeedScope(feed, navigationSource, entityType, skipWriting, selectedProperties, odataUri);
        }

        /// <summary>
        /// Create a new entry scope.
        /// </summary>
        /// <param name="entry">The entry for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly create scope.</returns>
        protected override EntryScope CreateEntryScope(ODataEntry entry, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightEntryScope(
                entry,
                this.GetEntrySerializationInfo(entry),
                navigationSource,
                entityType,
                skipWriting,
                this.jsonLightOutputContext.WritingResponse,
                this.jsonLightOutputContext.MessageWriterSettings.WriterBehavior,
                selectedProperties,
                odataUri,
                this.jsonLightOutputContext.MessageWriterSettings.EnableFullValidation);
        }

        /// <summary>
        /// Creates a new JSON Light navigation link scope.
        /// </summary>
        /// <param name="writerState">The writer state for the new scope.</param>
        /// <param name="navLink">The navigation link for the new scope.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="odataUri">The ODataUri info of this scope.</param>
        /// <returns>The newly created JSON Light  navigation link scope.</returns>
        protected override NavigationLinkScope CreateNavigationLinkScope(WriterState writerState, ODataNavigationLink navLink, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
        {
            return new JsonLightNavigationLinkScope(writerState, navLink, navigationSource, entityType, skipWriting, selectedProperties, odataUri);
        }

        /// <summary>
        /// Find instance of the metadata builder which belong to the parent odata entry
        /// </summary>
        /// <returns>
        /// The metadata builder of the parent odata entry
        /// Or null if there is no parent odata entry
        /// </returns>
        private ODataEntityMetadataBuilder FindParentEntryMetadataBuilder()
        {
            EntryScope parentEntryScope = this.GetParentEntryScope();

            if (parentEntryScope != null)
            {
                ODataEntry entry = parentEntryScope.Item as ODataEntry;
                if (entry != null)
                {
                    return entry.MetadataBuilder;
                }
            }

            return null;
        }

        /// <summary>
        /// Writes the odata.count annotation for a feed if it has not been written yet (and the count is specified on the feed).
        /// </summary>
        /// <param name="feed">The feed to write the count for.</param>
        /// <param name="propertyName">The name of the expanded nav property or null for a top-level feed.</param>
        private void WriteFeedCount(ODataFeed feed, string propertyName)
        {
            Debug.Assert(feed != null, "feed != null");

            // If we haven't written the count yet and it's available, write it.
            long? count = feed.Count;
            if (count.HasValue)
            {
                if (propertyName == null)
                {
                    this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataCount);
                }
                else
                {
                    this.odataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataCount);
                }

                this.jsonWriter.WriteValue(count.Value);
            }
        }

        /// <summary>
        /// Writes the odata.nextLink annotation for a feed if it has not been written yet (and the next link is specified on the feed).
        /// </summary>
        /// <param name="feed">The feed to write the next link for.</param>
        /// <param name="propertyName">The name of the expanded nav property or null for a top-level feed.</param>
        private void WriteFeedNextLink(ODataFeed feed, string propertyName)
        {
            Debug.Assert(feed != null, "feed != null");

            // If we haven't written the next link yet and it's available, write it.
            Uri nextPageLink = feed.NextPageLink;
            if (nextPageLink != null && !this.CurrentFeedScope.NextPageLinkWritten)
            {
                if (propertyName == null)
                {
                    this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataNextLink);
                }
                else
                {
                    this.odataAnnotationWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataNextLink);
                }

                this.jsonWriter.WriteValue(this.jsonLightEntryAndFeedSerializer.UriToString(nextPageLink));
                this.CurrentFeedScope.NextPageLinkWritten = true;
            }
        }

        /// <summary>
        /// Writes the odata.deltaLink annotation for a feed if it has not been written yet (and the delta link is specified on the feed).
        /// </summary>
        /// <param name="feed">The feed to write the delta link for.</param>
        private void WriteFeedDeltaLink(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            // If we haven't written the delta link yet and it's available, write it.
            Uri deltaLink = feed.DeltaLink;
            if (deltaLink != null && !this.CurrentFeedScope.DeltaLinkWritten)
            {
                this.odataAnnotationWriter.WriteInstanceAnnotationName(ODataAnnotationNames.ODataDeltaLink);
                this.jsonWriter.WriteValue(this.jsonLightEntryAndFeedSerializer.UriToString(deltaLink));
                this.CurrentFeedScope.DeltaLinkWritten = true;
            }
        }

        /// <summary>
        /// Validates that the ODataFeed.InstanceAnnotations collection is empty for the given expanded feed.
        /// </summary>
        /// <param name="feed">The expanded feed in question.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "An instance field is used in a debug assert.")]
        private void ValidateNoCustomInstanceAnnotationsForExpandedFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(
                this.ParentNavigationLink != null && this.ParentNavigationLink.IsCollection.HasValue && this.ParentNavigationLink.IsCollection.Value == true,
                "This should only be called when writing an expanded feed.");

            if (feed.InstanceAnnotations.Count > 0)
            {
                throw new ODataException(OData.Core.Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedFeed);
            }
        }

        /// <summary>
        /// A scope for a JSON lite feed.
        /// </summary>
        private sealed class JsonLightFeedScope : FeedScope
        {
            /// <summary>true if the odata.nextLink was already written, false otherwise.</summary>
            private bool nextLinkWritten;

            /// <summary>true if the odata.deltaLink was already written, false otherwise.</summary>
            private bool deltaLinkWritten;

            /// <summary>
            /// Constructor to create a new feed scope.
            /// </summary>
            /// <param name="feed">The feed for the new scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            internal JsonLightFeedScope(ODataFeed feed, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(feed, navigationSource, entityType, skipWriting, selectedProperties, odataUri)
            {
            }

            /// <summary>
            /// true if the odata.nextLink annotation was already written, false otherwise.
            /// </summary>
            internal bool NextPageLinkWritten
            {
                get
                {
                    return this.nextLinkWritten;
                }

                set
                {
                    this.nextLinkWritten = value;
                }
            }

            /// <summary>
            /// true if the odata.deltaLink annotation was already written, false otherwise.
            /// </summary>
            internal bool DeltaLinkWritten
            {
                get
                {
                    return this.deltaLinkWritten;
                }

                set
                {
                    this.deltaLinkWritten = value;
                }
            }
        }

        /// <summary>
        /// A scope for an entry in JSON Light writer.
        /// </summary>
        private sealed class JsonLightEntryScope : EntryScope, IODataJsonLightWriterEntryState
        {
            /// <summary>Bit field of the JSON Light metadata properties written so far.</summary>
            private int alreadyWrittenMetadataProperties;

            /// <summary>
            /// Constructor to create a new entry scope.
            /// </summary>
            /// <param name="entry">The entry for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current entry.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="writingResponse">true if we are writing a response, false if it's a request.</param>
            /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            /// <param name="enableValidation">Enable validation or not.</param>
            internal JsonLightEntryScope(ODataEntry entry, ODataFeedAndEntrySerializationInfo serializationInfo, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool skipWriting, bool writingResponse, ODataWriterBehavior writerBehavior, SelectedPropertiesNode selectedProperties, ODataUri odataUri, bool enableValidation)
                : base(entry, serializationInfo, navigationSource, entityType, skipWriting, writingResponse, writerBehavior, selectedProperties, odataUri, enableValidation)
            {
            }

            /// <summary>
            /// Enumeration of JSON Light metadata property flags, used to keep track of which properties were already written.
            /// </summary>
            [Flags]
            private enum JsonLightEntryMetadataProperty
            {
                /// <summary>The odata.editLink property.</summary>
                EditLink = 0x1,

                /// <summary>The odata.readLink property.</summary>
                ReadLink = 0x2,

                /// <summary>The odata.mediaEditLink property.</summary>
                MediaEditLink = 0x4,

                /// <summary>The odata.mediaReadLink property.</summary>
                MediaReadLink = 0x8,

                /// <summary>The odata.mediaContentType property.</summary>
                MediaContentType = 0x10,

                /// <summary>The odata.mediaEtag property.</summary>
                MediaETag = 0x20,
            }

            /// <summary>
            /// The entry being written.
            /// </summary>
            public ODataEntry Entry
            {
                get { return (ODataEntry)this.Item; }
            }

            /// <summary>
            /// Flag which indicates that the odata.editLink metadata property has been written.
            /// </summary>
            public bool EditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.EditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.EditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.readLink metadata property has been written.
            /// </summary>
            public bool ReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.ReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.ReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEditLink metadata property has been written.
            /// </summary>
            public bool MediaEditLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaEditLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaEditLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaReadLink metadata property has been written.
            /// </summary>
            public bool MediaReadLinkWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaReadLink);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaContentType metadata property has been written.
            /// </summary>
            public bool MediaContentTypeWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaContentType);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaContentType);
                }
            }

            /// <summary>
            /// Flag which indicates that the odata.mediaEtag metadata property has been written.
            /// </summary>
            public bool MediaETagWritten
            {
                get
                {
                    return this.IsMetadataPropertyWritten(JsonLightEntryMetadataProperty.MediaETag);
                }

                set
                {
                    Debug.Assert(value == true, "The flag that a metadata property has been written should only ever be set from false to true.");
                    this.SetWrittenMetadataProperty(JsonLightEntryMetadataProperty.MediaETag);
                }
            }

            /// <summary>
            /// Marks the <paramref name="jsonLightMetadataProperty"/> as written in this entry scope.
            /// </summary>
            /// <param name="jsonLightMetadataProperty">The metadta property which was written.</param>
            private void SetWrittenMetadataProperty(JsonLightEntryMetadataProperty jsonLightMetadataProperty)
            {
                Debug.Assert(!this.IsMetadataPropertyWritten(jsonLightMetadataProperty), "Can't write the same metadata property twice.");
                this.alreadyWrittenMetadataProperties |= (int)jsonLightMetadataProperty;
            }

            /// <summary>
            /// Determines if the <paramref name="jsonLightMetadataProperty"/> was already written for this entry scope.
            /// </summary>
            /// <param name="jsonLightMetadataProperty">The metadata property to test for.</param>
            /// <returns>true if the <paramref name="jsonLightMetadataProperty"/> was already written for this entry scope; false otherwise.</returns>
            private bool IsMetadataPropertyWritten(JsonLightEntryMetadataProperty jsonLightMetadataProperty)
            {
                return (this.alreadyWrittenMetadataProperties & (int)jsonLightMetadataProperty) == (int)jsonLightMetadataProperty;
            }
        }

        /// <summary>
        /// A scope for a JSON Light navigation link.
        /// </summary>
        private sealed class JsonLightNavigationLinkScope : NavigationLinkScope
        {
            /// <summary>true if we have already written an entity reference link for this navigation link in requests; otherwise false.</summary>
            private bool entityReferenceLinkWritten;

            /// <summary>true if we have written at least one feed for this navigation link in requests; otherwise false.</summary>
            private bool feedWritten;

            /// <summary>
            /// Constructor to create a new JSON Light navigation link scope.
            /// </summary>
            /// <param name="writerState">The writer state for the new scope.</param>
            /// <param name="navLink">The navigation link for the new scope.</param>
            /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            /// <param name="odataUri">The ODataUri info of this scope.</param>
            internal JsonLightNavigationLinkScope(WriterState writerState, ODataNavigationLink navLink, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(writerState, navLink, navigationSource, entityType, skipWriting, selectedProperties, odataUri)
            {
            }

            /// <summary>
            /// true if we have already written an entity reference link for this navigation link in requests; otherwise false.
            /// </summary>
            internal bool EntityReferenceLinkWritten
            {
                get
                {
                    return this.entityReferenceLinkWritten;
                }

                set
                {
                    this.entityReferenceLinkWritten = value;
                }
            }

            /// <summary>
            /// true if we have written at least one feed for this navigation link in requests; otherwise false.
            /// </summary>
            internal bool FeedWritten
            {
                get
                {
                    return this.feedWritten;
                }

                set
                {
                    this.feedWritten = value;
                }
            }

            /// <summary>
            /// Clones this JSON Light navigation link scope and sets a new writer state.
            /// </summary>
            /// <param name="newWriterState">The writer state to set.</param>
            /// <returns>The cloned navigation link scope with the specified writer state.</returns>
            internal override NavigationLinkScope Clone(WriterState newWriterState)
            {
                return new JsonLightNavigationLinkScope(newWriterState, (ODataNavigationLink)this.Item, this.NavigationSource, this.EntityType, this.SkipWriting, this.SelectedProperties, this.ODataUri)
                {
                    EntityReferenceLinkWritten = this.entityReferenceLinkWritten,
                    FeedWritten = this.feedWritten,
                };
            }
        }
    }
}
