//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData reader for the JsonLight format.
    /// </summary>
    internal sealed class ODataJsonLightReader : ODataReaderCoreAsync
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataJsonLightInputContext jsonLightInputContext;

        /// <summary>The entry and feed deserializer to read input with.</summary>
        private readonly ODataJsonLightEntryAndFeedDeserializer jsonLightEntryAndFeedDeserializer;

        /// <summary>The scope associated with the top level of this payload.</summary>
        private readonly JsonLightTopLevelScope topLevelScope;

        /// <summary>true if the reader is created for reading parameter; false otherwise.</summary>
        private readonly bool readingParameter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The input to read the payload from.</param>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read (in case of entry reader) or entries in the feed to be read (in case of feed reader).</param>
        /// <param name="readingFeed">true if the reader is created for reading a feed; false when it is created for reading an entry.</param>
        /// <param name="readingParameter">true if the reader is created for reading a parameter; false otherwise.</param>
        /// <param name="readingDelta">true if the reader is created for reading expanded navigation property in delta response; false otherwise.</param>
        /// <param name="listener">If not null, the Json reader will notify the implementer of the interface of relevant state changes in the Json reader.</param>
        internal ODataJsonLightReader(
            ODataJsonLightInputContext jsonLightInputContext,
            IEdmNavigationSource navigationSource,
            IEdmEntityType expectedEntityType,
            bool readingFeed,
            bool readingParameter = false,
            bool readingDelta = false,
            IODataReaderWriterListener listener = null)
            : base(jsonLightInputContext, readingFeed, readingDelta, listener)
        {
            Debug.Assert(jsonLightInputContext != null, "jsonLightInputContext != null");
            Debug.Assert(
                expectedEntityType == null || jsonLightInputContext.Model.IsUserModel(),
                "If the expected type is specified we need model as well. We should have verified that by now.");

            this.jsonLightInputContext = jsonLightInputContext;
            this.jsonLightEntryAndFeedDeserializer = new ODataJsonLightEntryAndFeedDeserializer(jsonLightInputContext);
            this.readingParameter = readingParameter;
            this.topLevelScope = new JsonLightTopLevelScope(navigationSource, expectedEntityType);
            this.EnterScope(this.topLevelScope);
        }

        /// <summary>
        /// Returns the current entry state.
        /// </summary>
        private IODataJsonLightReaderEntryState CurrentEntryState
        {
            get
            {
                Debug.Assert(
                    this.State == ODataReaderState.EntryStart || this.State == ODataReaderState.EntryEnd,
                    "This property can only be accessed in the EntryStart or EntryEnd scope.");
                return (IODataJsonLightReaderEntryState)this.CurrentScope;
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonLightFeedScope
        /// </summary>
        private JsonLightFeedScope CurrentJsonLightFeedScope
        {
            get
            {
                return ((JsonLightFeedScope)this.CurrentScope);
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonLightNavigationLinkScope
        /// </summary>
        private JsonLightNavigationLinkScope CurrentJsonLightNavigationLinkScope
        {
            get
            {
                return ((JsonLightNavigationLinkScope)this.CurrentScope);
            }
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: when reading a feed:    the reader is positioned on the first item in the feed or the end array node of an empty feed
        ///                 when reading an entry:  the first node of the first navigation link value, null for a null expanded link or an end object 
        ///                                         node if there are no navigation links.
        /// </remarks>
        protected override bool ReadAtStartImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
            Debug.Assert(this.IsReadingNestedPayload || this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker =
                this.jsonLightInputContext.CreateDuplicatePropertyNamesChecker();

            // Position the reader on the first node depending on whether we are reading a nested payload or not.
            ODataPayloadKind payloadKind = this.ReadingFeed ? ODataPayloadKind.Feed : ODataPayloadKind.Entry;
            this.jsonLightEntryAndFeedDeserializer.ReadPayloadStart(
                payloadKind,
                duplicatePropertyNamesChecker,
                this.IsReadingNestedPayload,
                /*allowEmptyPayload*/false);

            return this.ReadAtStartImplementationSynchronously(duplicatePropertyNamesChecker);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: when reading a feed:    the reader is positioned on the first item in the feed or the end array node of an empty feed
        ///                 when reading an entry:  the first node of the first navigation link value, null for a null expanded link or an end object 
        ///                                         node if there are no navigation links.
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtStartImplementationAsync()
        {
            Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
            Debug.Assert(this.IsReadingNestedPayload || this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker =
                this.jsonLightInputContext.CreateDuplicatePropertyNamesChecker();

            // Position the reader on the first node depending on whether we are reading a nested payload or not.
            ODataPayloadKind payloadKind = this.ReadingFeed ? ODataPayloadKind.Feed : ODataPayloadKind.Entry;
            return this.jsonLightEntryAndFeedDeserializer.ReadPayloadStartAsync(
                payloadKind,
                duplicatePropertyNamesChecker,
                this.IsReadingNestedPayload,
                /*allowEmptyPayload*/false)

                .FollowOnSuccessWith(t =>
                    this.ReadAtStartImplementationSynchronously(duplicatePropertyNamesChecker));
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first entry in the feed
        ///                 JsonNodeType.EndArray     - The end of the feed
        /// Post-Condition: The reader is positioned over the StartObject node of the first entry in the feed or 
        ///                 on the node following the feed end in case of an empty feed
        /// </remarks>
        protected override bool ReadAtFeedStartImplementation()
        {
            return this.ReadAtFeedStartImplementationSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'FeedStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first entry in the feed
        ///                 JsonNodeType.EndArray     - The end of the feed
        /// Post-Condition: The reader is positioned over the StartObject node of the first entry in the feed or 
        ///                 on the node following the feed end in case of an empty feed
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtFeedStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtFeedStartImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.Property        if the feed has further instance or property annotations after the feed property
        ///                JsonNodeType.EndObject       if the feed has no further instance or property annotations after the feed property
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level feed when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndObject      no further properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the feed doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded feed in request, this might be the next item in the expanded array, which is not an entry
        /// </remarks>
        protected override bool ReadAtFeedEndImplementation()
        {
            return this.ReadAtFeedEndImplementationSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'FeedEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.Property        if the feed has further instance or property annotations after the feed property
        ///                JsonNodeType.EndObject       if the feed has no further instance or property annotations after the feed property
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level feed when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndObject      no further properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the feed doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded feed in request, this might be the next item in the expanded array, which is not an entry
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtFeedEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtFeedEndImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded entry of the navigation link to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded feed of the navigation link to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry of the navigation link to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded entry of the navigation link to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded feed of the navigation link to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry of the navigation link to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        protected override bool ReadAtEntryStartImplementation()
        {
            return this.ReadAtEntryStartImplementationSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded entry of the navigation link to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded feed of the navigation link to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry of the navigation link to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded entry of the navigation link to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded feed of the navigation link to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry of the navigation link to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtEntryStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtEntryStartImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the entry
        ///                 JsonNodeType.PrimitiveValue (null)  end of null expanded entry
        /// Post-Condition: The reader is positioned on the first node after the entry's end-object node
        /// </remarks>
        protected override bool ReadAtEntryEndImplementation()
        {
            return this.ReadAtEntryEndImplementationSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the entry
        ///                 JsonNodeType.PrimitiveValue (null)  end of null expanded entry
        /// Post-Condition: The reader is positioned on the first node after the entry's end-object node
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtEntryEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtEntryEndImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            start of an expanded entry
        ///                 JsonNodeType.StartArray             start of an expanded feed
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        ///                 JsonNodeType.Property               deferred link with more properties in owning entry
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning entry
        /// Post-Condition: JsonNodeType.StartArray:            start of expanded entry
        ///                 JsonNodeType.StartObject            start of expanded feed
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        ///                 JsonNodeType.Property               deferred link with more properties in owning entry
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning entry
        /// </remarks>
        protected override bool ReadAtNavigationLinkStartImplementation()
        {
            return this.ReadAtNavigationLinkStartImplementationSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            start of an expanded entry
        ///                 JsonNodeType.StartArray             start of an expanded feed
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        ///                 JsonNodeType.Property               deferred link with more properties in owning entry
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning entry
        /// Post-Condition: JsonNodeType.StartArray:            start of expanded entry
        ///                 JsonNodeType.StartObject            start of expanded feed
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        ///                 JsonNodeType.Property               deferred link with more properties in owning entry
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning entry
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtNavigationLinkStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtNavigationLinkStartImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject:         navigation link is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the navigation link in the owning entry
        /// Post-Condition: JsonNodeType.StartObject        start of the expanded entry navigation link to read next
        ///                 JsonNodeType.StartArray         start of the expanded feed navigation link to read next
        ///                 JsonNoteType.Primitive (null)   expanded null entry navigation link to read next
        ///                 JsonNoteType.Property           property after deferred link or entity reference link
        ///                 JsonNodeType.EndObject          end of the parent entry
        /// </remarks>
        protected override bool ReadAtNavigationLinkEndImplementation()
        {
            return this.ReadAtNavigationLinkEndImplementationSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject:         navigation link is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the navigation link in the owning entry
        /// Post-Condition: JsonNodeType.StartObject        start of the expanded entry navigation link to read next
        ///                 JsonNodeType.StartArray         start of the expanded feed navigation link to read next
        ///                 JsonNoteType.Primitive (null)   expanded null entry navigation link to read next
        ///                 JsonNoteType.Property           property after deferred link or entity reference link
        ///                 JsonNodeType.EndObject          end of the parent entry
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtNavigationLinkEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtNavigationLinkEndImplementationSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// This method doesn't move the reader
        /// Pre-Condition:  JsonNodeType.EndObject:         expanded link property is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning entry
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// Post-Condition: JsonNodeType.EndObject:         expanded link property is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning entry
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// </remarks>
        protected override bool ReadAtEntityReferenceLink()
        {
            return this.ReadAtEntityReferenceLinkSynchronously();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// This method doesn't move the reader
        /// Pre-Condition:  JsonNodeType.EndObject:         expanded link property is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning entry
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// Post-Condition: JsonNodeType.EndObject:         expanded link property is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning entry
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// </remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAtEntityReferenceLinkAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtEntityReferenceLinkSynchronously);
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the top-level scope.</param>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet when not reading a nested payload.
        /// Post-Condition: when reading a feed:    the reader is positioned on the first item in the feed or the end array node of an empty feed
        ///                 when reading an entry:  the first node of the first navigation link value, null for a null expanded link or an end object 
        ///                                         node if there are no navigation links.
        /// </remarks>
        private bool ReadAtStartImplementationSynchronously(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");

            // For nested payload (e.g., expanded feed or entry in delta $entity payload),
            // we usually don't have a context URL for the feed or entry:
            // {
            //   "@odata.context":"...", <--- this context URL is for delta entity only
            //   "value": [
            //     {
            //       ...
            //       "NavigationProperty": <--- usually we don't have a context URL for this
            //       [ <--- nested payload start
            //         {...}
            //       ] <--- nested payload end
            //     }
            //    ]
            // }
            //
            // The consequence is that the entry we read out from a nested payload doesn't
            // have an entity metadata builder thus you cannot compute read link, edit link,
            // etc. from the entry object.
            if (this.jsonLightInputContext.ReadingResponse && !this.IsReadingNestedPayload)
            {
                Debug.Assert(this.jsonLightEntryAndFeedDeserializer.ContextUriParseResult != null, "We should have failed by now if we don't have parse results for context URI.");

                // Validate the context URI parsed from the payload against the entity set and entity type passed in through the API.
                ReaderValidationUtils.ValidateFeedOrEntryContextUri(this.jsonLightEntryAndFeedDeserializer.ContextUriParseResult, this.CurrentScope, true);
            }

            // Get the $select query option from the metadata link, if we have one.
            string selectQueryOption = this.jsonLightEntryAndFeedDeserializer.ContextUriParseResult == null
                ? null
                : this.jsonLightEntryAndFeedDeserializer.ContextUriParseResult.SelectQueryOption;

            SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.Create(selectQueryOption);

            if (this.ReadingFeed)
            {
                ODataFeed feed = new ODataFeed();

                // Store the duplicate property names checker to use it later when reading the feed end 
                // (since we allow feed-related annotations to appear after the feed's data).
                this.topLevelScope.DuplicatePropertyNamesChecker = duplicatePropertyNamesChecker;

                bool isReordering = this.jsonLightInputContext.JsonReader is ReorderingJsonReader;
                if (!this.IsReadingNestedPayload)
                {
                    // Skip top-level feed annotations for nested feeds.
                    this.jsonLightEntryAndFeedDeserializer.ReadTopLevelFeedAnnotations(
                        feed, duplicatePropertyNamesChecker, /*forFeedStart*/true, /*readAllFeedProperties*/isReordering);
                }

                this.ReadFeedStart(feed, selectedProperties);
                return true;
            }

            this.ReadEntryStart(duplicatePropertyNamesChecker, selectedProperties);
            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  Any start node            - The first entry in the feed
        ///                 JsonNodeType.EndArray     - The end of the feed
        /// Post-Condition: The reader is positioned over the StartObject node of the first entry in the feed or 
        ///                 on the node following the feed end in case of an empty feed
        /// </remarks>
        private bool ReadAtFeedStartImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataReaderState.FeedStart, "this.State == ODataReaderState.FeedStart");
            this.jsonLightEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            // figure out whether the feed contains entries or not
            switch (this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType)
            {
                // we are at the beginning of an entry
                // The expected type for an entry in the feed is the same as for the feed itself.
                case JsonNodeType.StartObject:
                    // First entry in the feed
                    this.ReadEntryStart(/*duplicatePropertyNamesChecker*/ null, this.CurrentJsonLightFeedScope.SelectedProperties);
                    break;
                case JsonNodeType.EndArray:
                    // End of the feed
                    this.ReadFeedEnd();
                    break;
                default:
                    throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadEntriesOfFeed(this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType));
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.Property        if the feed has further instance or property annotations after the feed property
        ///                JsonNodeType.EndObject       if the feed has no further instance or property annotations after the feed property
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level feed when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndObject      no further properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the feed doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded feed in request, this might be the next item in the expanded array, which is not an entry
        /// </remarks>
        private bool ReadAtFeedEndImplementationSynchronously()
        {
            Debug.Assert(this.State == ODataReaderState.FeedEnd, "this.State == ODataReaderState.FeedEnd");
            Debug.Assert(
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                !this.IsTopLevel && !this.jsonLightInputContext.ReadingResponse,
                "Pre-Condition: expected JsonNodeType.EndObject or JsonNodeType.Property");

            bool isTopLevelFeed = this.IsTopLevel;

            this.PopScope(ODataReaderState.FeedEnd);

            // When we finish a top-level feed in a nested payload (inside parameter or delta payload),
            // we can directly turn the reader into Completed state because we don't have any JSON token
            // (e.g., EndObject in a normal feed payload) left in the stream.
            //
            // Nested feed payload:
            // [
            //   {...},
            //   ...
            // ]
            // EOF <--- current reader position
            //
            // Normal feed payload:
            // {
            //   "@odata.context":"...",
            //   ...,
            //   "value": [
            //     {...},
            //     ...
            //   ],
            //   "@odata.nextLink":"..."
            // } <--- current reader position
            // EOF
            if (this.IsReadingNestedPayload && isTopLevelFeed)
            {
                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                return false;
            }

            if (isTopLevelFeed)
            {
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");

                // Read the end-object node of the feed object and position the reader on the next input node
                // This can hit the end of the input.
                this.jsonLightEntryAndFeedDeserializer.JsonReader.Read();

                // read the end-of-payload
                this.jsonLightEntryAndFeedDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                return false;
            }
            else
            {
                // finish reading the expanded link
                this.ReadExpandedNavigationLinkEnd(true);
                return true;
            }
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Start of the expanded entry of the navigation link to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded feed of the navigation link to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry of the navigation link to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// Post-Condition: JsonNodeType.StartObject            Start of the expanded entry of the navigation link to read next.
        ///                 JsonNodeType.StartArray             Start of the expanded feed of the navigation link to read next.
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry of the navigation link to read next.
        ///                 JsonNodeType.Property               The next property after a deferred link or entity reference link
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        private bool ReadAtEntryStartImplementationSynchronously()
        {
            if (this.CurrentEntry == null)
            {
                Debug.Assert(this.IsExpandedLinkContent, "null entry can only be reported in an expanded link.");
                this.jsonLightEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.PrimitiveValue);
                Debug.Assert(this.jsonLightEntryAndFeedDeserializer.JsonReader.Value == null, "The null entry should be represented as null value.");

                // Expanded null entry is represented as null primitive value
                // There's nothing to read, so move to the end entry state
                this.EndEntry();
            }
            else if (this.jsonLightInputContext.UseServerApiBehavior)
            {
                // In WCF DS Server mode we don't read ahead but report the entry right after type name.
                // So we need to read the entry content now.
                ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = this.jsonLightEntryAndFeedDeserializer.ReadEntryContent(this.CurrentEntryState);
                if (navigationLinkInfo != null)
                {
                    this.StartNavigationLink(navigationLinkInfo);
                }
                else
                {
                    this.EndEntry();
                }
            }
            else if (this.CurrentEntryState.FirstNavigationLinkInfo != null)
            {
                this.StartNavigationLink(this.CurrentEntryState.FirstNavigationLinkInfo);
            }
            else
            {
                // End of entry
                // All the properties have already been read before we acually entered the EntryStart state (since we read as far as we can in any given state).
                this.jsonLightEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.EndObject);
                this.EndEntry();
            }

            Debug.Assert(
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonLightEntryAndFeedDeserializer.JsonReader.Value == null ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.PrimitiveValue (null) or JsonNodeType.Property or JsonNodeType.EndObject");

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              end of object of the entry
        ///                 JsonNodeType.PrimitiveValue (null)  end of null expanded entry
        /// Post-Condition: The reader is positioned on the first node after the entry's end-object node
        /// </remarks>
        private bool ReadAtEntryEndImplementationSynchronously()
        {
            Debug.Assert(
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonLightEntryAndFeedDeserializer.JsonReader.Value == null,
                "Pre-Condition: JsonNodeType.EndObject or JsonNodeType.PrimitiveValue (null)");

            // We have to cache these values here, since the PopScope below will destroy them.
            bool isTopLevel = this.IsTopLevel;
            bool isExpandedLinkContent = this.IsExpandedLinkContent;

            this.PopScope(ODataReaderState.EntryEnd);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.jsonLightEntryAndFeedDeserializer.JsonReader.Read();
            JsonNodeType nodeType = this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType;

            // Analyze the next Json token to determine whether it is start object (next entry), end array (feed end) or eof (top-level entry end)
            bool result = true;
            if (isTopLevel)
            {
                // NOTE: we rely on the underlying JSON reader to fail if there is more than one value at the root level.
                Debug.Assert(
                    this.IsReadingNestedPayload || this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput,
                    "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // read the end-of-payload
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
                this.jsonLightEntryAndFeedDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
                Debug.Assert(
                    this.IsReadingNestedPayload || this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput,
                    "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else if (isExpandedLinkContent)
            {
                Debug.Assert(
                    nodeType == JsonNodeType.EndObject ||               // expanded link entry as last property of the owning entry
                    nodeType == JsonNodeType.Property,                  // expanded link entry with more properties on the entry
                    "Invalid JSON reader state for reading end of entry in expanded link.");

                // finish reading the expanded link
                this.ReadExpandedNavigationLinkEnd(false);
            }
            else
            {
                // End of entry in a feed
                switch (nodeType)
                {
                    case JsonNodeType.StartObject:
                        // another entry in a feed
                        Debug.Assert(this.State == ODataReaderState.FeedStart, "Expected reader to be in state feed start before reading the next entry.");
                        this.ReadEntryStart(/*duplicatePropertyNamesChecker*/ null, this.CurrentJsonLightFeedScope.SelectedProperties);
                        break;
                    case JsonNodeType.EndArray:
                        // we are at the end of a feed
                        Debug.Assert(this.State == ODataReaderState.FeedStart, "Expected reader to be in state feed start after reading the last entry in the feed.");
                        this.ReadFeedEnd();
                        break;
                    default:
                        throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadEntriesOfFeed(this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType));
                }
            }

            return result;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            start of an expanded entry
        ///                 JsonNodeType.StartArray             start of an expanded feed
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        ///                 JsonNodeType.Property               deferred link with more properties in owning entry
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning entry or
        ///                                                     reporting projected navigation links missing in the payload
        /// Post-Condition: JsonNodeType.StartArray:            start of expanded entry
        ///                 JsonNodeType.StartObject            start of expanded feed
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        ///                 JsonNodeType.Property               deferred link with more properties in owning entry
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning entry or
        ///                                                     reporting projected navigation links missing in the payload
        /// </remarks>
        private bool ReadAtNavigationLinkStartImplementationSynchronously()
        {
            Debug.Assert(
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonLightEntryAndFeedDeserializer.JsonReader.Value == null,
                "Pre-Condition: expected JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray or JsonNodeType.Primitive (null)");

            ODataNavigationLink currentLink = this.CurrentNavigationLink;
            Debug.Assert(
                currentLink.IsCollection.HasValue || this.jsonLightInputContext.MessageReaderSettings.ReportUndeclaredLinkProperties,
                "Expect to know whether this is a singleton or collection link based on metadata.");

            IODataJsonLightReaderEntryState parentEntryState = (IODataJsonLightReaderEntryState)this.LinkParentEntityScope;

            if (this.jsonLightInputContext.ReadingResponse)
            {
                // If we are reporting a navigation link that was projected but not included in the payload,
                // simply change state to NavigationLinkEnd.
                if (parentEntryState.ProcessingMissingProjectedNavigationLinks)
                {
                    this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
                }
                else if (!this.jsonLightEntryAndFeedDeserializer.JsonReader.IsOnValueNode())
                {
                    // Deferred link (navigation link which doesn't have a value and is in the response)
                    ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentLink, false, currentLink.IsCollection);
                    this.jsonLightEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

                    // Record that we read the link on the parent entry's scope.
                    parentEntryState.NavigationPropertiesRead.Add(currentLink.Name);

                    this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
                }
                else if (!currentLink.IsCollection.Value)
                {
                    // We should get here only for declared navigation properties.
                    Debug.Assert(this.CurrentEntityType != null, "We must have a declared navigation property to read expanded links.");

                    // Expanded entry
                    ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentLink, true, false);
                    this.ReadExpandedEntryStart(currentLink);
                }
                else
                {
                    // Expanded feed
                    ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentLink, true, true);

                    // We store the precreated expanded feed in the navigation link info since it carries the annotations for it.
                    ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = this.CurrentJsonLightNavigationLinkScope.NavigationLinkInfo;
                    Debug.Assert(navigationLinkInfo != null, "navigationLinkInfo != null");
                    Debug.Assert(navigationLinkInfo.ExpandedFeed != null, "We must have a precreated expanded feed already.");
                    JsonLightEntryScope parentScope = (JsonLightEntryScope)this.LinkParentEntityScope;
                    SelectedPropertiesNode parentSelectedProperties = parentScope.SelectedProperties;
                    Debug.Assert(parentSelectedProperties != null, "parentProjectedProperties != null");
                    this.ReadFeedStart(navigationLinkInfo.ExpandedFeed, parentSelectedProperties.GetSelectedPropertiesForNavigationProperty(parentScope.EntityType, currentLink.Name));
                }
            }
            else
            {
                // Navigation link in request - report entity reference links and then possible expanded value.
                ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = this.CurrentJsonLightNavigationLinkScope.NavigationLinkInfo;
                ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(
                    parentEntryState.DuplicatePropertyNamesChecker,
                    currentLink,
                    navigationLinkInfo.IsExpanded,
                    currentLink.IsCollection);
                this.ReadNextNavigationLinkContentItemInRequest();
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject:         navigation link is last property in owning entry or
        ///                                                 reporting projected navigation links missing in the payload
        ///                 JsonNodeType.Property:          there are more properties after the navigation link in the owning entry
        /// Post-Condition: JsonNodeType.StartObject        start of the expanded entry navigation link to read next
        ///                 JsonNodeType.StartArray         start of the expanded feed navigation link to read next
        ///                 JsonNoteType.Primitive (null)   expanded null entry navigation link to read next
        ///                 JsonNoteType.Property           property after deferred link or entity reference link
        ///                 JsonNodeType.EndObject          end of the parent entry
        /// </remarks>
        private bool ReadAtNavigationLinkEndImplementationSynchronously()
        {
            this.jsonLightEntryAndFeedDeserializer.AssertJsonCondition(
                JsonNodeType.EndObject,
                JsonNodeType.Property);

            this.PopScope(ODataReaderState.NavigationLinkEnd);
            Debug.Assert(this.State == ODataReaderState.EntryStart, "this.State == ODataReaderState.EntryStart");

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = null;
            IODataJsonLightReaderEntryState entryState = this.CurrentEntryState;

            if (this.jsonLightInputContext.ReadingResponse && entryState.ProcessingMissingProjectedNavigationLinks)
            {
                // We are reporting navigation links that were projected but missing from the payload
                navigationLinkInfo = entryState.Entry.MetadataBuilder.GetNextUnprocessedNavigationLink();
            }
            else
            {
                navigationLinkInfo = this.jsonLightEntryAndFeedDeserializer.ReadEntryContent(entryState);
            }

            if (navigationLinkInfo == null)
            {
                // End of the entry
                this.EndEntry();
            }
            else
            {
                // Next navigation link on the entry
                this.StartNavigationLink(navigationLinkInfo);
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// This method doesn't move the reader
        /// Pre-Condition:  JsonNodeType.EndObject:         expanded link property is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning entry
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// Post-Condition: JsonNodeType.EndObject:         expanded link property is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning entry
        ///                 Any:                            expanded collection link - the node after the entity reference link.
        /// </remarks>
        private bool ReadAtEntityReferenceLinkSynchronously()
        {
            this.PopScope(ODataReaderState.EntityReferenceLink);
            Debug.Assert(this.State == ODataReaderState.NavigationLinkStart, "this.State == ODataReaderState.NavigationLinkStart");

            this.ReadNextNavigationLinkContentItemInRequest();
            return true;
        }

        /// <summary>
        /// Reads the start of the JSON array for the content of the feed and sets up the reader state correctly.
        /// </summary>
        /// <param name="feed">The feed to read the contents for.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  The first node of the feed property value; this method will throw if the node is not
        ///                 JsonNodeType.StartArray
        /// Post-Condition: The reader is positioned on the first item in the feed, or on the end array of the feed.
        /// </remarks>
        private void ReadFeedStart(ODataFeed feed, SelectedPropertiesNode selectedProperties)
        {
            Debug.Assert(feed != null, "feed != null");

            this.jsonLightEntryAndFeedDeserializer.ReadFeedContentStart();
            this.EnterScope(new JsonLightFeedScope(feed, this.CurrentNavigationSource, this.CurrentEntityType, selectedProperties, this.CurrentScope.ODataUri));

            this.jsonLightEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.StartObject);
        }

        /// <summary>
        /// Reads the end of the current feed.
        /// </summary>
        private void ReadFeedEnd()
        {
            Debug.Assert(this.State == ODataReaderState.FeedStart, "this.State == ODataReaderState.FeedStart");

            this.jsonLightEntryAndFeedDeserializer.ReadFeedContentEnd();

            ODataJsonLightReaderNavigationLinkInfo expandedNavigationLinkInfo = null;
            JsonLightNavigationLinkScope parentNavigationLinkScope = (JsonLightNavigationLinkScope)this.ExpandedLinkContentParentScope;
            if (parentNavigationLinkScope != null)
            {
                expandedNavigationLinkInfo = parentNavigationLinkScope.NavigationLinkInfo;
            }

            if (!this.IsReadingNestedPayload)
            {
                // Temp ban reading the instance annotation after the feed in parameter payload. (!this.IsReadingNestedPayload => !this.readingParameter)
                // Nested feed payload won't have a NextLink annotation after the feed itself since the payload is NOT pageable.
                this.jsonLightEntryAndFeedDeserializer.ReadNextLinkAnnotationAtFeedEnd(this.CurrentFeed,
                    expandedNavigationLinkInfo, this.topLevelScope.DuplicatePropertyNamesChecker);
            }

            this.ReplaceScope(ODataReaderState.FeedEnd);
        }

        /// <summary>
        /// Reads the start of an expanded entry (null or non-null).
        /// </summary>
        /// <param name="navigationLink">The navigation link that is being expanded.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            The start of the entry object
        ///                 JsonNodeType.PrimitiveValue (null)  The null entry value
        /// Post-Condition: JsonNodeType.StartObject            Start of expanded entry of the navigation link to read next
        ///                 JsonNodeType.StartArray             Start of expanded feed of the navigation link to read next
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry of the navigation link to read next, or the null value of the current null entry
        ///                 JsonNodeType.Property               Property after deferred link or expanded entity reference
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        private void ReadExpandedEntryStart(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            if (this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                Debug.Assert(this.jsonLightEntryAndFeedDeserializer.JsonReader.Value == null, "If a primitive value is representing an expanded entity its value must be null.");

                // Expanded null entry
                // The expected type and expected navigation source for an expanded entry are the same as for the navigation link around it.
                this.EnterScope(new JsonLightEntryScope(ODataReaderState.EntryStart, /*entry*/ null, this.CurrentNavigationSource, this.CurrentEntityType, /*duplicatePropertyNamesChecker*/null, /*projectedProperties*/null, this.CurrentScope.ODataUri));
            }
            else
            {
                // Expanded entry
                // The expected type for an expanded entry is the same as for the navigation link around it.
                JsonLightEntryScope parentScope = (JsonLightEntryScope)this.LinkParentEntityScope;
                SelectedPropertiesNode parentSelectedProperties = parentScope.SelectedProperties;
                Debug.Assert(parentSelectedProperties != null, "parentProjectedProperties != null");
                this.ReadEntryStart(/*duplicatePropertyNamesChecker*/ null, parentSelectedProperties.GetSelectedPropertiesForNavigationProperty(parentScope.EntityType, navigationLink.Name));
            }
        }

        /// <summary>
        /// Reads the start of an entry and sets up the reader state correctly
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the entry; 
        /// or null if a new one should be created.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            If the entry is in a feed - the start of the entry object
        ///                 JsonNodeType.Property               If the entry is a top-level entry and has at least one property
        ///                 JsonNodeType.EndObject              If the entry is a top-level entry and has no properties
        /// Post-Condition: JsonNodeType.StartObject            Start of expanded entry of the navigation link to read next
        ///                 JsonNodeType.StartArray             Start of expanded feed of the navigation link to read next
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null entry of the navigation link to read next
        ///                 JsonNodeType.Property               Property after deferred link or expanded entity reference
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        private void ReadEntryStart(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, SelectedPropertiesNode selectedProperties)
        {
            this.jsonLightEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.StartObject, JsonNodeType.Property, JsonNodeType.EndObject);

            // If the reader is on StartObject then read over it. This happens for entries in feed.
            // For top-level entries the reader will be positioned on the first entry property (after odata.context if it was present).
            if (this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject)
            {
                this.jsonLightEntryAndFeedDeserializer.JsonReader.Read();
            }

            if (this.ReadingFeed || this.IsExpandedLinkContent)
            {
                string contextUriStr = this.jsonLightEntryAndFeedDeserializer.ReadContextUriAnnotation(ODataPayloadKind.Entry, duplicatePropertyNamesChecker, false);
                if (contextUriStr != null)
                {
                    contextUriStr = UriUtils.UriToString(this.jsonLightEntryAndFeedDeserializer.ProcessUriFromPayload(contextUriStr));
                    var parseResult = ODataJsonLightContextUriParser.Parse(
                            this.jsonLightEntryAndFeedDeserializer.Model,
                            contextUriStr,
                            ODataPayloadKind.Entry,
                            this.jsonLightEntryAndFeedDeserializer.MessageReaderSettings.ReaderBehavior,
                            this.jsonLightInputContext.ReadingResponse);
                    if (this.jsonLightInputContext.ReadingResponse && parseResult != null)
                    {
                        ReaderValidationUtils.ValidateFeedOrEntryContextUri(parseResult, this.CurrentScope, false);
                    }
                }
            }

            // Setup the new entry state
            this.StartEntry(duplicatePropertyNamesChecker, selectedProperties);

            // Read the odata.type annotation.
            this.jsonLightEntryAndFeedDeserializer.ReadEntryTypeName(this.CurrentEntryState);

            // Resolve the type name
            Debug.Assert(
                this.CurrentNavigationSource != null || this.readingParameter,
                "We must always have an expected navigation source for each entry (since we can't deduce that from the type name).");
            this.ApplyEntityTypeNameFromPayload(this.CurrentEntry.TypeName);

            // Validate type with feed validator if available
            if (this.CurrentFeedValidator != null)
            {
                this.CurrentFeedValidator.ValidateEntry(this.CurrentEntityType);
            }

            if (this.CurrentEntityType != null)
            {
                // NOTE: once we do this for all formats we can do this in ApplyEntityTypeNameFromPayload.
                this.CurrentEntry.SetAnnotation(new ODataTypeAnnotation(this.CurrentNavigationSource, this.CurrentEntityType));
            }

            // In WCF DS Server mode we must not read ahead and report the type name only.
            if (this.jsonLightInputContext.UseServerApiBehavior)
            {
                this.CurrentEntryState.FirstNavigationLinkInfo = null;
            }
            else
            {
                this.CurrentEntryState.FirstNavigationLinkInfo = this.jsonLightEntryAndFeedDeserializer.ReadEntryContent(this.CurrentEntryState);
            }

            this.jsonLightEntryAndFeedDeserializer.AssertJsonCondition(
                JsonNodeType.Property,
                JsonNodeType.StartObject,
                JsonNodeType.StartArray,
                JsonNodeType.EndObject,
                JsonNodeType.PrimitiveValue);
        }

        /// <summary>
        /// Verifies that the current item is an <see cref="ODataNavigationLink"/> instance,
        /// sets the cardinality of the link (IsCollection property) and moves the reader
        /// into state 'NavigationLinkEnd'.
        /// </summary>
        /// <param name="isCollection">A flag indicating whether the link represents a collection or not.</param>
        private void ReadExpandedNavigationLinkEnd(bool isCollection)
        {
            Debug.Assert(this.State == ODataReaderState.NavigationLinkStart, "this.State == ODataReaderState.NavigationLinkStart");
            this.CurrentNavigationLink.IsCollection = isCollection;

            // Record that we read the link on the parent entry's scope.
            IODataJsonLightReaderEntryState parentEntryState = (IODataJsonLightReaderEntryState)this.LinkParentEntityScope;
            parentEntryState.NavigationPropertiesRead.Add(this.CurrentNavigationLink.Name);

            // replace the 'NavigationLinkStart' scope with the 'NavigationLinkEnd' scope
            this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
        }

        /// <summary>
        /// Reads the next item in a navigation link content in a request payload.
        /// </summary>
        private void ReadNextNavigationLinkContentItemInRequest()
        {
            Debug.Assert(this.CurrentScope.State == ODataReaderState.NavigationLinkStart, "Must be on 'NavigationLinkStart' scope.");

            ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo = this.CurrentJsonLightNavigationLinkScope.NavigationLinkInfo;
            if (navigationLinkInfo.HasEntityReferenceLink)
            {
                this.EnterScope(new Scope(ODataReaderState.EntityReferenceLink, navigationLinkInfo.ReportEntityReferenceLink(), null, null, this.CurrentScope.ODataUri));
            }
            else if (navigationLinkInfo.IsExpanded)
            {
                if (navigationLinkInfo.NavigationLink.IsCollection == true)
                {
                    // because this is a request, there is no $select query option.
                    SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.EntireSubtree;
                    this.ReadFeedStart(new ODataFeed(), selectedProperties);
                }
                else
                {
                    this.ReadExpandedEntryStart(navigationLinkInfo.NavigationLink);
                }
            }
            else
            {
                // replace the 'NavigationLinkStart' scope with the 'NavigationLinkEnd' scope
                this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
            }
        }

        /// <summary>
        /// Starts the entry, initializing the scopes and such. This method starts a non-null entry only.
        /// </summary>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use for the entry; 
        /// or null if a new one should be created.</param>
        /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
        private void StartEntry(DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, SelectedPropertiesNode selectedProperties)
        {
            this.EnterScope(new JsonLightEntryScope(
                ODataReaderState.EntryStart,
                ReaderUtils.CreateNewEntry(),
                this.CurrentNavigationSource,
                this.CurrentEntityType,
                duplicatePropertyNamesChecker ?? this.jsonLightInputContext.CreateDuplicatePropertyNamesChecker(),
                selectedProperties,
                this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Starts the navigation link.
        /// Does metadata validation of the navigation link and sets up the reader to report it.
        /// </summary>
        /// <param name="navigationLinkInfo">The navigation link info for the navigation link to start.</param>
        private void StartNavigationLink(ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo)
        {
            Debug.Assert(navigationLinkInfo != null, "navigationLinkInfo != null");
            ODataNavigationLink navigationLink = navigationLinkInfo.NavigationLink;
            IEdmNavigationProperty navigationProperty = navigationLinkInfo.NavigationProperty;

            Debug.Assert(
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonLightEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonLightEntryAndFeedDeserializer.JsonReader.Value == null,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Primitive (null), or JsonNodeType.Property, JsonNodeType.EndObject");
            Debug.Assert(
                navigationProperty != null || this.jsonLightInputContext.MessageReaderSettings.ReportUndeclaredLinkProperties,
                "A navigation property must be found for each link we find unless we're allowed to report undeclared links.");
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "Navigation links must have a name.");
            Debug.Assert(
                navigationProperty == null || navigationLink.Name == navigationProperty.Name,
                "The navigation property must match the navigation link.");

            // we are at the beginning of a link
            IEdmEntityType targetEntityType = null;
            if (navigationProperty != null)
            {
                IEdmTypeReference navigationPropertyType = navigationProperty.Type;
                targetEntityType = navigationPropertyType.IsCollection()
                    ? navigationPropertyType.AsCollection().ElementType().AsEntity().EntityDefinition()
                    : navigationPropertyType.AsEntity().EntityDefinition();
            }

            // Since we don't have the entity metadata builder for the entry read out from a nested payload
            // as stated in ReadAtFeedEndImplementationSynchronously(), we cannot access it here which otherwise
            // would lead to an exception.
            if (this.jsonLightInputContext.ReadingResponse && !this.IsReadingNestedPayload)
            {
                // Hookup the metadata builder to the navigation link.
                // Note that we set the metadata builder even when navigationProperty is null, which is the case when the link is undeclared.
                // For undeclared links, we will apply conventional metadata evaluation just as declared links.
                ODataEntityMetadataBuilder entityMetadataBuilder = this.jsonLightEntryAndFeedDeserializer.MetadataContext.GetEntityMetadataBuilderForReader(this.CurrentEntryState, this.jsonLightInputContext.MessageReaderSettings.UseKeyAsSegment);
                navigationLink.MetadataBuilder = entityMetadataBuilder;
            }

            Debug.Assert(this.CurrentNavigationSource != null || this.readingParameter, "Json requires an navigation source when not reading parameter.");

            IEdmNavigationSource navigationSource = this.CurrentNavigationSource == null || navigationProperty == null ? null : this.CurrentNavigationSource.FindNavigationTarget(navigationProperty);
            ODataUri odataUri = null;
            if (navigationLinkInfo.NavigationLink.ContextUrl != null)
            {
                ODataPath odataPath = ODataJsonLightContextUriParser.Parse(
                        this.jsonLightEntryAndFeedDeserializer.Model,
                        UriUtils.UriToString(navigationLinkInfo.NavigationLink.ContextUrl),
                        navigationLinkInfo.NavigationLink.IsCollection.GetValueOrDefault() ? ODataPayloadKind.Feed : ODataPayloadKind.Entry,
                        this.jsonLightEntryAndFeedDeserializer.MessageReaderSettings.ReaderBehavior,
                        this.jsonLightEntryAndFeedDeserializer.JsonLightInputContext.ReadingResponse).Path;
                odataUri = new ODataUri()
                {
                    Path = odataPath
                };
            }

            this.EnterScope(new JsonLightNavigationLinkScope(navigationLinkInfo, navigationSource, targetEntityType, odataUri));
        }

        /// <summary>
        /// Replaces the current scope with a new scope with the specified <paramref name="state"/> and
        /// the item of the current scope.
        /// </summary>
        /// <param name="state">The <see cref="ODataReaderState"/> to use for the new scope.</param>
        private void ReplaceScope(ODataReaderState state)
        {
            this.ReplaceScope(new Scope(state, this.Item, this.CurrentNavigationSource, this.CurrentEntityType, this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// Called to transition into the EntryEnd state.
        /// </summary>
        private void EndEntry()
        {
            IODataJsonLightReaderEntryState entryState = this.CurrentEntryState;

            // NOTE: the current entry will be null for an expanded null entry; no template
            //       expansion for null entries.
            //       there is no entity metadata builder for an entry from a nested payload
            //       as stated in ReadAtFeedEndImplementationSynchronously().
            if (this.CurrentEntry != null && !this.IsReadingNestedPayload)
            {
                ODataEntityMetadataBuilder builder = this.jsonLightEntryAndFeedDeserializer.MetadataContext.GetEntityMetadataBuilderForReader(this.CurrentEntryState, this.jsonLightInputContext.MessageReaderSettings.UseKeyAsSegment);
                if (builder != this.CurrentEntry.MetadataBuilder)
                {
                    // Builder should not be used outside the odataentry, lazy builder logic does not work here
                    // We should refactor this
                    foreach (string navigationPropertyName in this.CurrentEntryState.NavigationPropertiesRead)
                    {
                        builder.MarkNavigationLinkProcessed(navigationPropertyName);
                    }

                    ODataConventionalEntityMetadataBuilder conventionalEntityMetadataBuilder = builder as ODataConventionalEntityMetadataBuilder;

                    // If it's ODataConventionalEntityMetadataBuilder, then it means we need to build nested relation ship for it in containment case
                    if (conventionalEntityMetadataBuilder != null)
                    {
                        conventionalEntityMetadataBuilder.ODataUri = this.CurrentScope.ODataUri;
                    }

                    // Set the metadata builder for the entry itself
                    this.CurrentEntry.MetadataBuilder = builder;
                }
            }

            this.jsonLightEntryAndFeedDeserializer.ValidateEntryMetadata(entryState);

            // In responses, ensure that all projected properties get created.
            // Also ignore cases where the entry is 'null' which happens for expanded null entries.
            if (this.jsonLightInputContext.ReadingResponse && this.CurrentEntry != null)
            {
                // If we have a projected navigation link that was missing from the payload, report it now.
                ODataJsonLightReaderNavigationLinkInfo unprocessedNavigationLink = this.CurrentEntry.MetadataBuilder.GetNextUnprocessedNavigationLink();
                if (unprocessedNavigationLink != null)
                {
                    this.CurrentEntryState.ProcessingMissingProjectedNavigationLinks = true;
                    this.StartNavigationLink(unprocessedNavigationLink);
                    return;
                }
            }

            this.EndEntry(
                new JsonLightEntryScope(
                    ODataReaderState.EntryEnd,
                    (ODataEntry)this.Item,
                    this.CurrentNavigationSource,
                    this.CurrentEntityType,
                    this.CurrentEntryState.DuplicatePropertyNamesChecker,
                    this.CurrentEntryState.SelectedProperties,
                    this.CurrentScope.ODataUri));
        }

        /// <summary>
        /// A reader top-level scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightTopLevelScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type of the top-level entry or entries in the top-level feed.
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonLightTopLevelScope(IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)
                : base(ODataReaderState.Start, /*item*/ null, navigationSource, expectedEntityType, null)
            {
            }

            /// <summary>
            /// The duplicate property names checker for the top level scope represented by the current state.
            /// </summary>
            public DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker { get; set; }
        }

        /// <summary>
        /// A reader entry scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightEntryScope : Scope, IODataJsonLightReaderEntryState
        {
            /// <summary>The set of names of the navigation properties we have read so far while reading the entry.</summary>
            private List<string> navigationPropertiesRead;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="readerState">The reader state of the new scope that is being created.</param>
            /// <param name="entry">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker for this entry scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type of the entry. If the entry has no type name specified
            ///   this type will be assumed. Otherwise the specified type name must be
            ///   the expected type or a more derived type.
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonLightEntryScope(
                ODataReaderState readerState,
                ODataEntry entry,
                IEdmNavigationSource navigationSource,
                IEdmEntityType expectedEntityType,
                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
                SelectedPropertiesNode selectedProperties,
                ODataUri odataUri)
                : base(readerState, entry, navigationSource, expectedEntityType, odataUri)
            {
                Debug.Assert(
                    readerState == ODataReaderState.EntryStart || readerState == ODataReaderState.EntryEnd,
                    "readerState == ODataReaderState.EntryStart || readerState == ODataReaderState.EntryEnd");

                this.DuplicatePropertyNamesChecker = duplicatePropertyNamesChecker;
                this.SelectedProperties = selectedProperties;
            }

            /// <summary>
            /// The metadata builder instance for the entry.
            /// </summary>
            public ODataEntityMetadataBuilder MetadataBuilder { get; set; }

            /// <summary>
            /// Flag which indicates that during parsing of the entry represented by this state,
            /// any property which is not an instance annotation was found. This includes property annotations
            /// for property which is not present in the payload.
            /// </summary>
            /// <remarks>
            /// This is used to detect incorrect ordering of the payload (for example odata.id must not come after the first property).
            /// </remarks>
            public bool AnyPropertyFound { get; set; }

            /// <summary>
            /// If the reader finds a navigation link to report, but it must first report the parent entry
            /// it will store the navigation link info in this property. So this will only ever store the first navigation link of an entry.
            /// </summary>
            public ODataJsonLightReaderNavigationLinkInfo FirstNavigationLinkInfo { get; set; }

            /// <summary>
            /// The duplicate property names checker for the entry represented by the current state.
            /// </summary>
            public DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker { get; private set; }

            /// <summary>
            /// The selected properties that should be expanded during template evaluation.
            /// </summary>
            public SelectedPropertiesNode SelectedProperties { get; private set; }

            /// <summary>
            /// The set of names of the navigation properties we have read so far while reading the entry.
            /// true if we have started processing missing projected navigation links, false otherwise.
            /// </summary>
            public List<string> NavigationPropertiesRead
            {
                get { return this.navigationPropertiesRead ?? (this.navigationPropertiesRead = new List<string>()); }
            }

            /// <summary>
            /// true if we have started processing missing projected navigation links, false otherwise.
            /// </summary>
            public bool ProcessingMissingProjectedNavigationLinks { get; set; }

            /// <summary>
            /// The entry being read.
            /// </summary>
            ODataEntry IODataJsonLightReaderEntryState.Entry
            {
                get
                {
                    Debug.Assert(
                        this.State == ODataReaderState.EntryStart || this.State == ODataReaderState.EntryEnd,
                        "The IODataJsonReaderEntryState is only supported on EntryStart or EntryEnd scope.");
                    return (ODataEntry)this.Item;
                }
            }

            /// <summary>
            /// The entity type for the entry (if available).
            /// </summary>
            IEdmEntityType IODataJsonLightReaderEntryState.EntityType
            {
                get
                {
                    Debug.Assert(
                        this.State == ODataReaderState.EntryStart || this.State == ODataReaderState.EntryEnd,
                        "The IODataJsonReaderEntryState is only supported on EntryStart or EntryEnd scope.");
                    return this.EntityType;
                }
            }
        }

        /// <summary>
        /// A reader feed scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightFeedScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="feed">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <param name="selectedProperties">The selected properties node capturing what properties should be expanded during template evaluation.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type of the entries in the feed.
            ///   note that it might be a more derived type than the base type of the entity set for the feed.
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonLightFeedScope(ODataFeed feed, IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
                : base(ODataReaderState.FeedStart, feed, navigationSource, expectedEntityType, odataUri)
            {
                this.SelectedProperties = selectedProperties;
            }

            /// <summary>
            /// The selected properties that should be expanded during template evaluation.
            /// </summary>
            public SelectedPropertiesNode SelectedProperties { get; private set; }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonLightNavigationLinkScope : Scope
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="navigationLinkInfo">The navigation link info attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param> 
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meaning
            ///   it's the expected base type the entries in the expanded link (either the single entry
            ///   or entries in the expanded feed).
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonLightNavigationLinkScope(ODataJsonLightReaderNavigationLinkInfo navigationLinkInfo, IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType, ODataUri odataUri)
                : base(ODataReaderState.NavigationLinkStart, navigationLinkInfo.NavigationLink, navigationSource, expectedEntityType, odataUri)
            {
                this.NavigationLinkInfo = navigationLinkInfo;
            }

            /// <summary>
            /// The navigation link info for the navigation link to report.
            /// This is only used on a StartNavigationLink scope in responses.
            /// </summary>
            public ODataJsonLightReaderNavigationLinkInfo NavigationLinkInfo { get; private set; }
        }
    }
}
