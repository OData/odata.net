//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData reader for the Verbose JSON format.
    /// </summary>
    internal sealed class ODataVerboseJsonReader : ODataReaderCore
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataVerboseJsonInputContext verboseJsonInputContext;

        /// <summary>The entry and feed deserializer to read input with.</summary>
        private readonly ODataVerboseJsonEntryAndFeedDeserializer verboseJsonEntryAndFeedDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonInputContext">The input to read the payload from.</param>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read (in case of entry reader) or entries in the feed to be read (in case of feed reader).</param>
        /// <param name="readingFeed">true if the reader is created for reading a feed; false when it is created for reading an entry.</param>
        /// <param name="listener">If not null, the Json reader will notify the implementer of the interface of relevant state changes in the Json reader.</param>
        internal ODataVerboseJsonReader(
            ODataVerboseJsonInputContext verboseJsonInputContext, 
            IEdmEntitySet entitySet, 
            IEdmEntityType expectedEntityType, 
            bool readingFeed, 
            IODataReaderWriterListener listener)
            : base(verboseJsonInputContext, readingFeed, listener)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(verboseJsonInputContext != null, "verboseJsonInputContext != null");
            Debug.Assert(
                expectedEntityType == null || verboseJsonInputContext.Model.IsUserModel(),
                "If the expected type is specified we need model as well. We should have verified that by now.");

            this.verboseJsonInputContext = verboseJsonInputContext;
            this.verboseJsonEntryAndFeedDeserializer = new ODataVerboseJsonEntryAndFeedDeserializer(verboseJsonInputContext);

            if (!this.verboseJsonInputContext.Model.IsUserModel())
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReader_ParsingWithoutMetadata);
            }

            this.EnterScope(new Scope(ODataReaderState.Start, /*item*/ null, entitySet, expectedEntityType));
        }

        /// <summary>
        /// Returns the current entry state.
        /// </summary>
        private IODataVerboseJsonReaderEntryState CurrentEntryState
        {
            get
            {
                Debug.Assert(this.State == ODataReaderState.EntryStart, "This property can only be accessed in the EntryStart scope.");
                return (IODataVerboseJsonReaderEntryState)this.CurrentScope;
            }
        }

        /// <summary>
        /// Returns current scope cast to JsonScope
        /// </summary>
        private JsonScope CurrentJsonScope
        {
            get
            {
                return ((JsonScope)this.CurrentScope);
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
            Debug.Assert(this.IsReadingNestedPayload || this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None when not reading a nested payload.");

            // read the data wrapper depending on whether we are reading a request or response
            this.verboseJsonEntryAndFeedDeserializer.ReadPayloadStart(this.IsReadingNestedPayload);

            if (this.ReadingFeed)
            {
                // The expected type for the top-level feed is the same as for the entire reader (the start state).
                this.ReadFeedStart(/*isExpandedLinkContent*/ false);
                return true;
            }

            // The expected type for the top-level entry is the same as for the entire reader (the start state).
            this.ReadEntryStart();
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
        protected override bool ReadAtFeedStartImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.FeedStart, "this.State == ODataReaderState.FeedStart");
            this.verboseJsonEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            // figure out whether the feed contains entries or not
            switch (this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType)
            {
                // we are at the beginning of an entry
                // The expected type for an entry in the feed is the same as for the feed itself.
                case JsonNodeType.StartObject:
                    // First entry in the feed
                    this.ReadEntryStart();
                    break;
                case JsonNodeType.EndArray:
                    // End of the feed
                    this.verboseJsonEntryAndFeedDeserializer.ReadFeedEnd(this.CurrentFeed, this.CurrentJsonScope.FeedHasResultsWrapper, this.IsExpandedLinkContent);
                    this.ReplaceScope(ODataReaderState.FeedEnd);
                    break;
                default:
                    throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadEntriesOfFeed(this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType));
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.EndArray        if the feed is not wrapped in the 'results' wrapper
        ///                JsonNodeType.EndObject       if the feed is wrapped in the 'results' wrapper
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level feed when not reading a nested payload
        ///                 JsonNodeType.Property       more properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndObject      no further properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndArray       end of expanded link in request, in this case the feed doesn't actually own the array object and it won't read it.
        ///                 Any                         in case of expanded feed in request, this might be the next item in the expanded array, which is not an entry
        /// </remarks>
        protected override bool ReadAtFeedEndImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.FeedEnd, "this.State == ODataReaderState.FeedEnd");
            Debug.Assert(
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndArray ||
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                !this.IsTopLevel && !this.verboseJsonInputContext.ReadingResponse,
                "Pre-Condition: expected JsonNodeType.EndObject or JsonNodeType.EndArray");

            bool isTopLevelFeed = this.IsTopLevel;

            this.PopScope(ODataReaderState.FeedEnd);

            bool result;
            if (isTopLevelFeed)
            {
                // Read the end-object node of the wrapped feed or the end-array node of non-wrapped feed
                // and position the reader on the next input node
                // This can hit the end of the input.
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.Read();

                // read the end-of-payload suffix
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
                this.verboseJsonEntryAndFeedDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
                Debug.Assert(this.IsReadingNestedPayload || this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput, "Expected JSON reader to have reached the end of input when not reading a nested payload.");

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else
            {
                if (this.verboseJsonInputContext.ReadingResponse)
                {
                    // Read the end-object node of the wrapped feed or the end-array node of non-wrapped feed
                    // and position the reader on the next input node
                    // This can hit the end of the input.
                    this.verboseJsonEntryAndFeedDeserializer.JsonReader.Read();

                    // finish reading the expanded link
                    this.ReadExpandedNavigationLinkEnd(true);
                }
                else
                {
                    this.ReadExpandedCollectionNavigationLinkContentInRequest();
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            The first node of the navigation link property value to read next (feed wrapped in 'results' wrapper)
        ///                 JsonNodeType.StartArray             The first node of the navigation link property value to read next (feed not wrapped in 'results' wrapper)
        ///                 JsonNodeType.PrimitiveValue (null)  The null expanded entry value (representing the end of that entry)
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// Post-Condition: JsonNodeType.StartObject            The first node of the navigation link property value to read next (feed wrapped in 'results' wrapper)
        ///                 JsonNodeType.StartArray             The first node of the navigation link property value to read next (feed not wrapped in 'results' wrapper)
        ///                 JsonNodeType.PrimitiveValue (null)  The null expanded entry value (representing the end of that entry)
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        protected override bool ReadAtEntryStartImplementation()
        {
            if (this.CurrentEntry == null)
            {
                Debug.Assert(this.IsExpandedLinkContent, "null entry can only be reported in an expanded link.");
                this.verboseJsonEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.PrimitiveValue);
                Debug.Assert(this.verboseJsonEntryAndFeedDeserializer.JsonReader.Value == null, "The null entry should be represented as null value.");

                // Expanded null entry is represented as null primitive value
                // There's nothing to read, so move to the end entry state
                this.EndEntry();
            }
            else if (this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject)
            {
                // End of entry
                // All the properties have already been read before we acually entered the EntryStart state (since we read as far as we can in any given state).
                this.EndEntry();
            }
            else if (this.verboseJsonInputContext.UseServerApiBehavior)
            {
                IEdmNavigationProperty navigationProperty;
                ODataNavigationLink navigationLink = this.verboseJsonEntryAndFeedDeserializer.ReadEntryContent(
                    this.CurrentEntryState,
                    out navigationProperty);
                if (navigationLink != null)
                {
                    this.StartNavigationLink(navigationLink, navigationProperty);
                }
                else
                {
                    this.EndEntry();
                }
            }
            else
            {
                Debug.Assert(this.CurrentEntryState.FirstNavigationLink != null, "We must have remembered the first navigation link already.");

                this.StartNavigationLink(this.CurrentEntryState.FirstNavigationLink, this.CurrentEntryState.FirstNavigationProperty);
            }

            Debug.Assert(
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.verboseJsonEntryAndFeedDeserializer.JsonReader.Value == null ||
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.PrimitiveValue (null) or JsonNodeType.EndObject");

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
        protected override bool ReadAtEntryEndImplementation()
        {
            Debug.Assert(
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.verboseJsonEntryAndFeedDeserializer.JsonReader.Value == null,
                "Pre-Condition: JsonNodeType.EndObject or JsonNodeType.PrimitiveValue (null)");

            bool isTopLevel = this.IsTopLevel;
            bool isExpandedLinkContent = this.IsExpandedLinkContent;

            this.PopScope(ODataReaderState.EntryEnd);

            // Read over the end object node (or null value) and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.verboseJsonEntryAndFeedDeserializer.JsonReader.Read();
            JsonNodeType nodeType = this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType;

            // analyze the next Json token to determine whether it is start object (next entry), end array (feed end) or eof (top-level entry end)
            bool result = true;
            if (isTopLevel)
            {
                // NOTE: we rely on the underlying JSON reader to fail if there is more than one value at the root level.
                Debug.Assert(
                    (this.IsReadingNestedPayload || !this.verboseJsonEntryAndFeedDeserializer.ReadingResponse && nodeType == JsonNodeType.EndOfInput) ||  // top-level entry end in a request
                    this.verboseJsonEntryAndFeedDeserializer.ReadingResponse && nodeType == JsonNodeType.EndObject,      // top-level entry end in a response
                    "Invalid JSON reader state for reading the end of a top-level entry.");

                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");

                // read the end-of-payload suffix
                this.verboseJsonEntryAndFeedDeserializer.ReadPayloadEnd(this.IsReadingNestedPayload);
                Debug.Assert(this.IsReadingNestedPayload || this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput, "Expected JSON reader to have reached the end of input when not reading a nested payload.");

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
                Debug.Assert(this.State == ODataReaderState.FeedStart, "Expected reader to be in state feed start before reading the next entry.");

                if (this.CurrentJsonScope.FeedInExpandedNavigationLinkInRequest)
                {
                    this.ReadExpandedCollectionNavigationLinkContentInRequest();
                }
                else
                {
                    switch (nodeType)
                    {
                        case JsonNodeType.StartObject:
                            // another entry in a feed
                            Debug.Assert(this.State == ODataReaderState.FeedStart, "Expected reader to be in state feed start before reading the next entry.");
                            this.ReadEntryStart();
                            break;
                        case JsonNodeType.EndArray:
                            // we are at the end of a feed
                            Debug.Assert(this.State == ODataReaderState.FeedStart, "Expected reader to be in state feed start after reading the last entry in the feed.");
                            this.verboseJsonEntryAndFeedDeserializer.ReadFeedEnd(this.CurrentFeed, this.CurrentJsonScope.FeedHasResultsWrapper, this.IsExpandedLinkContent);
                            this.ReplaceScope(ODataReaderState.FeedEnd);
                            break;
                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadEntriesOfFeed(this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            The first node of the navigation link property value to read next 
        ///                                                     (deferred link or entry inside expanded link or wrapped feed inside expanded link)
        ///                 JsonNodeType.StartArray             feed not wrapped with 'results' wrapper inside of expanded link
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        /// Post-Condition: JsonNodeType.StartArray:            expanded link with a feed that is not wrapped with 'results' wrapper
        ///                 JsonNodeType.StartObject            expanded link with a feed that is warpped with 'results' wrapper
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        ///                 JsonNodeType.Property               deferred link with more properties in owning entry
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning entry
        /// </remarks>
        protected override bool ReadAtNavigationLinkStartImplementation()
        {
            Debug.Assert(
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject || this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.verboseJsonEntryAndFeedDeserializer.JsonReader.Value == null,
                "Pre-Condition: expected JsonNodeType.StartObject, JsonNodeType.StartArray or JsonNodeType.Primitive (null)");

            ODataNavigationLink currentLink = this.CurrentNavigationLink;
            Debug.Assert(
                currentLink.IsCollection.HasValue || this.verboseJsonInputContext.MessageReaderSettings.ReportUndeclaredLinkProperties,
                "Expect to know whether this is a singleton or collection link based on metadata.");

            IODataVerboseJsonReaderEntryState parentEntryState = (IODataVerboseJsonReaderEntryState)this.LinkParentEntityScope;

            if (this.verboseJsonInputContext.ReadingResponse && this.verboseJsonEntryAndFeedDeserializer.IsDeferredLink(/*navigationLinkFound*/ true))
            {
                ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentLink, false, currentLink.IsCollection);

                // read the deferred link
                this.verboseJsonEntryAndFeedDeserializer.ReadDeferredNavigationLink(currentLink);
                this.verboseJsonEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

                this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
            }
            else if (!currentLink.IsCollection.Value)
            {
                // We should get here only for declared navigation properties. We don't keep the navigation property around, but
                // having an entity type inside link (which is where we're right now) means we found the navigation property for the link (and thus it's declared).
                Debug.Assert(this.CurrentEntityType != null, "We must have a declared navigation property to read expanded links.");

                // In request, the expanded entry may in fact be an entity reference link, so look for that first.
                if (!this.verboseJsonInputContext.ReadingResponse && this.verboseJsonEntryAndFeedDeserializer.IsEntityReferenceLink())
                {
                    ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentLink, false, false);

                    // read the entity reference link
                    ODataEntityReferenceLink entityReferenceLink = this.verboseJsonEntryAndFeedDeserializer.ReadEntityReferenceLink();
                    this.verboseJsonEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.EndObject, JsonNodeType.Property);

                    this.EnterScope(ODataReaderState.EntityReferenceLink, entityReferenceLink, null);
                }
                else
                {
                    ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentLink, true, false);
                    if (this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                    {
                        Debug.Assert(this.verboseJsonEntryAndFeedDeserializer.JsonReader.Value == null, "If a primitive value is representing an expanded entity its value must be null.");

                        // Expanded null entry
                        // The expected type for an expanded entry is the same as for the navigation link around it.
                        this.EnterScope(ODataReaderState.EntryStart, null, this.CurrentEntityType);
                    }
                    else
                    {
                        // Expanded entry
                        // The expected type for an expanded entry is the same as for the navigation link around it.
                        this.ReadEntryStart();
                    }
                }
            }
            else
            {
                // Expanded feed
                // The expected type for an expanded feed is the same as for the navigation link around it.
                ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentLink, true, true);

                if (this.verboseJsonInputContext.ReadingResponse)
                {
                    this.ReadFeedStart(/*isExpandedLinkContent*/ true);
                }
                else
                {
                    // In requests, expanded collection navigation link has a special behavior since it's an array with possibly entity reference links
                    // and entries in it. So we can't read it as a regular feed.
                    if (this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType != JsonNodeType.StartObject && this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType != JsonNodeType.StartArray)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadFeedStart(this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType));
                    }

                    var feedHasResultsWrapper = this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject;

                    // the ODataFeed created here is not used. We just want to skip the 'results' wrapper here. 
                    this.verboseJsonEntryAndFeedDeserializer.ReadFeedStart(new ODataFeed(), feedHasResultsWrapper, /*isExpandedLinkContent*/ true);

                    // we set the flag on the current scope - which is a navigation link scope
                    this.CurrentJsonScope.FeedHasResultsWrapper = feedHasResultsWrapper;
                    this.ReadExpandedCollectionNavigationLinkContentInRequest();
                }
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject:         expanded link property is last property in owning entry
        ///                 JsonNodeType.Property:          there are more properties after the expanded link property in the owning entry
        /// Post-Condition: JsonNodeType.StartObject        The first node of the navigation link property value to read next
        ///                 JsonNodeType.StartArray         The first node of the navigation link property value with a non-wrapped feed to read next
        ///                 JsonNodeType.EndObject          If no (more) properties exist in the entry's content
        ///                 JsonNoteType.Primitive (null)   If an expanded link with null entity instance was found.
        /// </remarks>
        protected override bool ReadAtNavigationLinkEndImplementation()
        {
            this.verboseJsonEntryAndFeedDeserializer.AssertJsonCondition(
                JsonNodeType.EndObject,
                JsonNodeType.Property);

            this.PopScope(ODataReaderState.NavigationLinkEnd);
            Debug.Assert(this.State == ODataReaderState.EntryStart, "this.State == ODataReaderState.EntryStart");

            IEdmNavigationProperty navigationProperty;
            ODataNavigationLink navigationLink = this.verboseJsonEntryAndFeedDeserializer.ReadEntryContent(this.CurrentEntryState, out navigationProperty);
            if (navigationLink == null)
            {
                // End of the entry
                this.EndEntry();
            }
            else
            {
                // Next navigation link on the entry
                this.StartNavigationLink(navigationLink, navigationProperty);
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
        protected override bool ReadAtEntityReferenceLink()
        {
            this.PopScope(ODataReaderState.EntityReferenceLink);
            Debug.Assert(this.State == ODataReaderState.NavigationLinkStart, "this.State == ODataReaderState.NavigationLinkStart");

            ODataNavigationLink navigationLink = this.CurrentNavigationLink;
            if (navigationLink.IsCollection == true)
            {
                // Look at the next item in the array
                this.ReadExpandedCollectionNavigationLinkContentInRequest();
            }
            else
            {
                // Change the start to end, since a singleton entity reference link is the only child of the parent nav. link.
                this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
            }

            return true;
        }

        /// <summary>
        /// Reads the start of a feed and sets up the reader state correctly.
        /// </summary>
        /// <param name="isExpandedLinkContent">true if the feed is inside an expanded link.</param>
        /// <remarks>
        /// Pre-Condition:  The first node of the feed; this method will throw if the node is not
        ///                 JsonNodeType.StartArray:    a feed without 'results' wrapper
        ///                 JsonNodeType.StartObject:   a feed with 'results' wrapper
        /// Post-Condition: The reader is positioned on the first item in the feed, or on the end array of the feed.
        /// </remarks>
        private void ReadFeedStart(bool isExpandedLinkContent)
        {
            Debug.Assert(
                this.verboseJsonInputContext.ReadingResponse || this.State != ODataReaderState.NavigationLinkStart,
                "Expanded navigation links in requests should not call this method.");

            ODataFeed feed = new ODataFeed();

            if (this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType != JsonNodeType.StartObject && this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadFeedStart(this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType));
            }

            bool feedHasResultsWrapper = this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject;

            // read the start of the feed until we determine the next item not belonging to the feed
            this.verboseJsonEntryAndFeedDeserializer.ReadFeedStart(feed, feedHasResultsWrapper, isExpandedLinkContent);

            this.EnterScope(ODataReaderState.FeedStart, feed, this.CurrentEntityType);

            // set the flag so that we know whether to also read '}' when reading the end of the feed
            this.CurrentJsonScope.FeedHasResultsWrapper = feedHasResultsWrapper;

            this.verboseJsonEntryAndFeedDeserializer.AssertJsonCondition(JsonNodeType.EndArray, JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
        }

        /// <summary>
        /// Reads the next node in the content of an expanded navigation link which represents a collection and is in a request payload.
        /// </summary>
        /// <remarks>
        /// This method deals with all the special cases in request payload expanded navigation link for collections.
        /// It should be called when the array start of the content of such a link was already read.
        /// It should be called in these cases:
        /// - Start of the navigation link (to report the first content item of it)
        /// - Entity reference link was reported (to report the next item of the navigation link content)
        /// - Feed end was reported, to report the next non-entry item in the navigation link content
        /// - Entry end was reported, to determine if the next entry should be reported, or if the feed should be closed.
        /// </remarks>
        private void ReadExpandedCollectionNavigationLinkContentInRequest()
        {
            Debug.Assert(!this.verboseJsonInputContext.ReadingResponse, "This method should only be called for requests.");

            // We are positioned on the next item in the feed array, so it can be either an entity reference link, or expanded entry
            if (this.verboseJsonEntryAndFeedDeserializer.IsEntityReferenceLink())
            {
                if (this.State == ODataReaderState.FeedStart)
                {
                    Debug.Assert(
                        this.CurrentJsonScope.FeedInExpandedNavigationLinkInRequest,
                        "Feed inside an expanded navigation link in request is expected here.");

                    // If it's an entity reference link and we are currently inside a feed (which is the expanded feed), we need to close that feed first
                    this.ReplaceScope(ODataReaderState.FeedEnd);
                }
                else
                {
                    // If we're not in feed, we must be at the navigation link level
                    Debug.Assert(this.State == ODataReaderState.NavigationLinkStart, "Entity reference link can only occur inside a feed or inside a navigation link.");
                    this.CurrentJsonScope.ExpandedNavigationLinkInRequestHasContent = true;

                    // In this case we read the entity reference link and report it
                    // read the entity reference link
                    ODataEntityReferenceLink entityReferenceLink = this.verboseJsonEntryAndFeedDeserializer.ReadEntityReferenceLink();
                    this.EnterScope(ODataReaderState.EntityReferenceLink, entityReferenceLink, null);
                }
            }
            else if (this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndArray || this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject)
            {
                // End of the expanded navigation link array
                if (this.State == ODataReaderState.FeedStart)
                {
                    // If we are inside one of the expanded feeds, end that feed first
                    Debug.Assert(
                        this.CurrentJsonScope.FeedInExpandedNavigationLinkInRequest,
                        "Feed inside an expanded navigation link in request is expected here.");

                    Debug.Assert(
                        this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndArray,
                        "End of array expected");

                    this.verboseJsonEntryAndFeedDeserializer.ReadFeedEnd(this.CurrentFeed, this.CurrentJsonScope.FeedHasResultsWrapper, true);
                    this.ReplaceScope(ODataReaderState.FeedEnd);
                }
                else
                {
                    Debug.Assert(this.State == ODataReaderState.NavigationLinkStart, "We should be in a navigation link state.");
                    if (!this.CurrentJsonScope.ExpandedNavigationLinkInRequestHasContent)
                    {
                        // If we haven't found and reported any content for the link yet, we need to report an empty feed.
                        // This is to avoid reporting the link without any content which would be treated as a deferred link
                        // which is invalid in requests.
                        this.CurrentJsonScope.ExpandedNavigationLinkInRequestHasContent = true;
                        this.EnterScope(ODataReaderState.FeedStart, new ODataFeed(), this.CurrentEntityType);
                        this.CurrentJsonScope.FeedInExpandedNavigationLinkInRequest = true;
                    }
                    else
                    {
                        // If the expanded feed ended with entity reference link the information whether the feed was wrapped with 
                        // 'results' wrapper will be stored in the NavigationLinkStart scope which is current scope. Here we use it 
                        // to know whether to skip only ']' before we skip the character closing the extended feed.
                        if (this.CurrentJsonScope.FeedHasResultsWrapper)
                        {
                            // note that ODataFeed instance passed here is a dummy feed that is not being used anywhere else. 
                            var feed = new ODataFeed();
                            this.verboseJsonEntryAndFeedDeserializer.ReadFeedEnd(feed, true, true);
                            Debug.Assert(
                                feed.Count == null && feed.NextPageLink == null && feed.Id == null,
                                "No feed properties should be set when closing a feed");
                        }

                        // Here we are reading either the closing ']' of an unwrapped expanded feed or the closing '}' of an 
                        // wrapped expanded feed or the closing '}' of a navigation property contents.
                        this.verboseJsonEntryAndFeedDeserializer.JsonReader.Read();
                        
                        // End the navigation link
                        this.ReadExpandedNavigationLinkEnd(true);
                    }
                }
            }
            else
            {
                // The thing we're looking at is not an entity reference link
                if (this.State == ODataReaderState.FeedStart)
                {
                    Debug.Assert(
                        this.CurrentJsonScope.FeedInExpandedNavigationLinkInRequest,
                        "Feed inside an expanded navigation link in request is expected here.");

                    if (this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType != JsonNodeType.StartObject)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReader_CannotReadEntriesOfFeed(this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType));
                    }

                    // If we're already in a feed, read the item as an entry directly
                    this.ReadEntryStart();
                }
                else
                {
                    Debug.Assert(this.State == ODataReaderState.NavigationLinkStart, "Entity reference link can only occur inside a feed or inside a navigation link.");
                    this.CurrentJsonScope.ExpandedNavigationLinkInRequestHasContent = true;

                    // If we're not yet in a feed, start a new feed, note that this feed has no payload counterpart, it's just necessary for our API
                    this.EnterScope(ODataReaderState.FeedStart, new ODataFeed(), this.CurrentEntityType);
                    this.CurrentJsonScope.FeedInExpandedNavigationLinkInRequest = true;
                }
            }
        }

        /// <summary>
        /// Reads the start of an entry and sets up the reader state correctly
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject            Will fail if it's anything else
        /// Post-Condition: JsonNodeType.StartObject            The first node of the navigation link property value to read next (deferred link or entry or >=v2 feed wrapper)
        ///                 JsonNodeType.StartArray             The first node of the navigation link property value with a non-wrapped feed to read next
        ///                 JsonNodeType.PrimitiveValue (null)  The null value of the navigation link property value to read next (expanded null entry)
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        private void ReadEntryStart()
        {
            // Read the start of the entry
            this.verboseJsonEntryAndFeedDeserializer.ReadEntryStart();

            // Setup the new entry state
            this.StartEntry();

            // Read the metadata and resolve the type.
            this.ReadEntryMetadata();

            if (this.verboseJsonInputContext.UseServerApiBehavior)
            {
                this.CurrentEntryState.FirstNavigationLink = null;
                this.CurrentEntryState.FirstNavigationProperty = null;
            }
            else
            {
                IEdmNavigationProperty navigationProperty;
                this.CurrentEntryState.FirstNavigationLink = this.verboseJsonEntryAndFeedDeserializer.ReadEntryContent(
                    this.CurrentEntryState,
                    out navigationProperty);
                this.CurrentEntryState.FirstNavigationProperty = navigationProperty;
            }

            this.verboseJsonEntryAndFeedDeserializer.AssertJsonCondition(
                JsonNodeType.Property,
                JsonNodeType.StartObject,
                JsonNodeType.StartArray,
                JsonNodeType.EndObject,
                JsonNodeType.PrimitiveValue);
        }

        /// <summary>
        /// Reads the __metadata property for an entry and resolves its type.
        /// </summary>
        private void ReadEntryMetadata()
        {
            // Read ahead buffering and consume the __metadata property and all its content.
            // We need to populate all the metadata properties at the very start so that we can chain the reader and writer
            // - writer requires all the metadata properties at the entry start since it has to write the __metadata first.
            // Start buffering so we don't move the reader
            this.verboseJsonEntryAndFeedDeserializer.JsonReader.StartBuffering();

            // read through all the properties and find the __metadata one (if it exists)
            bool metadataPropertyFound = false;
            while (this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.verboseJsonEntryAndFeedDeserializer.JsonReader.ReadPropertyName();

                if (string.CompareOrdinal(propertyName, JsonConstants.ODataMetadataName) == 0)
                {
                    // Read the metadata property
                    metadataPropertyFound = true;
                    break;
                }
                else
                {
                    // skip over the value of this property
                    this.verboseJsonEntryAndFeedDeserializer.JsonReader.SkipValue();
                }
            }

            string typeNameFromPayload = null;
            object metadataPropertyBookmark = null;
            if (metadataPropertyFound)
            {
                metadataPropertyBookmark = this.verboseJsonEntryAndFeedDeserializer.JsonReader.BookmarkCurrentPosition();
                typeNameFromPayload = this.verboseJsonEntryAndFeedDeserializer.ReadTypeNameFromMetadataPropertyValue();
            }

            // Resolve the type name
            this.ApplyEntityTypeNameFromPayload(typeNameFromPayload);

            // Validate type with feed validator if available
            if (this.CurrentFeedValidator != null)
            {
                this.CurrentFeedValidator.ValidateEntry(this.CurrentEntityType);
            }

            if (metadataPropertyFound)
            {
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.MoveToBookmark(metadataPropertyBookmark);
                this.verboseJsonEntryAndFeedDeserializer.ReadEntryMetadataPropertyValue(this.CurrentEntryState);
            }

            // Stop the buffering and reset the reader to its original position
            this.verboseJsonEntryAndFeedDeserializer.JsonReader.StopBuffering();

            this.verboseJsonEntryAndFeedDeserializer.AssertJsonCondition(
                JsonNodeType.Property,
                JsonNodeType.EndObject);

            this.verboseJsonEntryAndFeedDeserializer.ValidateEntryMetadata(this.CurrentEntryState);
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

            // replace the 'NavigationLinkStart' scope with the 'NavigationLinkEnd' scope
            this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
        }

        /// <summary>
        /// Starts the entry, initializing the scopes and such. This method starts a non-null entry only.
        /// </summary>
        private void StartEntry()
        {
            this.EnterScope(ODataReaderState.EntryStart, ReaderUtils.CreateNewEntry(), this.CurrentEntityType);
            this.CurrentJsonScope.DuplicatePropertyNamesChecker = this.verboseJsonInputContext.CreateDuplicatePropertyNamesChecker();
        }

        /// <summary>
        /// Starts the navigation link.
        /// Does metadata validation of the navigation link and sets up the reader to report it.
        /// </summary>
        /// <param name="navigationLink">The navigation link to start.</param>
        /// <param name="navigationProperty">The navigation property for the navigation link to start.</param>
        private void StartNavigationLink(ODataNavigationLink navigationLink, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.verboseJsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.verboseJsonEntryAndFeedDeserializer.JsonReader.Value == null,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Primitive (null)");
            Debug.Assert(
                navigationProperty != null || this.verboseJsonInputContext.MessageReaderSettings.ReportUndeclaredLinkProperties,
                "A navigation property must be found for each link we find unless we're allowed to report undeclared links.");

            // we are at the beginning of a link
            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "Navigation links must have a name.");

            IEdmEntityType targetEntityType = null;
            if (navigationProperty != null)
            {
                IEdmTypeReference navigationPropertyType = navigationProperty.Type;
                targetEntityType = navigationPropertyType.IsCollection()
                    ? navigationPropertyType.AsCollection().ElementType().AsEntity().EntityDefinition()
                    : navigationPropertyType.AsEntity().EntityDefinition();
            }

            this.EnterScope(ODataReaderState.NavigationLinkStart, navigationLink, targetEntityType);
        }

        /// <summary>
        /// Creates a new <see cref="JsonScope"/> for the specified <paramref name="state"/> and
        /// with the provided <paramref name="item"/> and pushes it on the stack of scopes.
        /// </summary>
        /// <param name="state">The <see cref="ODataReaderState"/> to use for the new scope.</param>
        /// <param name="item">The item to attach with the state in the new scope.</param>
        /// <param name="expectedEntityType">The expected type for the new scope.</param>
        private void EnterScope(ODataReaderState state, ODataItem item, IEdmEntityType expectedEntityType)
        {
            this.EnterScope(new JsonScope(state, item, expectedEntityType));
        }

        /// <summary>
        /// Replaces the current scope with a new <see cref="JsonScope"/> with the specified <paramref name="state"/> and
        /// the item of the current scope.
        /// </summary>
        /// <param name="state">The <see cref="ODataReaderState"/> to use for the new scope.</param>
        private void ReplaceScope(ODataReaderState state)
        {
            this.ReplaceScope(new JsonScope(state, this.Item, this.CurrentEntityType));
        }

        /// <summary>
        /// Called to transition into the EntryEnd state.
        /// </summary>
        private void EndEntry()
        {
            this.EndEntry(new JsonScope(ODataReaderState.EntryEnd, this.Item, this.CurrentEntityType));
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonScope : Scope, IODataVerboseJsonReaderEntryState
        {
            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="expectedEntityType">The expected type for the scope.</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meanings for given state:
            /// Start -               it's the expected base type of the top-level entry or entries in the top-level feed.
            /// FeedStart -           it's the expected base type of the entries in the feed.
            ///                       note that it might be a more derived type than the base type of the entity set for the feed.
            /// EntryStart -          it's the expected base type of the entry. If the entry has no type name specified
            ///                       this type will be assumed. Otherwise the specified type name must be
            ///                       the expected type or a more derived type.
            /// NavigationLinkStart - it's the expected base type the entries in the expanded link (either the single entry
            ///                       or entries in the expanded feed).
            /// In all cases the specified type must be an entity type.</remarks>
            internal JsonScope(ODataReaderState state, ODataItem item, IEdmEntityType expectedEntityType)
                : base(state, item, /*entitySet*/ null, expectedEntityType)
            {
            }

            /// <summary>
            /// Flag which indicates that during parsing of the entry represented by this scope,
            /// the __metadata property was already found.
            /// </summary>
            public bool MetadataPropertyFound { get; set; }

            /// <summary>
            /// If the reader finds a navigation link to report, but it must first report the parent entry
            /// it will store the navigation link in this property. So this will only ever store the first navigation link of an entry.
            /// </summary>
            public ODataNavigationLink FirstNavigationLink { get; set; }

            /// <summary>
            /// If the reader finds a navigation link to report, but it must first report the parent entry
            /// it will store the navigation property in this property. So this will only ever store the first navigation proeprty of an entry.
            /// </summary>
            public IEdmNavigationProperty FirstNavigationProperty { get; set; }

            /// <summary>
            /// The duplicate property names checker for the entry represented by the current state.
            /// </summary>
            public DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker { get; set; }

            /// <summary>
            /// Flag which is only used on a StartFeed scope.
            /// true - if the feed is the special feed reported as content of an expanded navigation link in request.
            /// false - if the feed is any other (regular) feed.
            /// </summary>
            public bool FeedInExpandedNavigationLinkInRequest { get; set; }

            /// <summary>
            /// Flag which is used to remember whether the feed was wrapped in with 'results' wrapper and which indicates
            /// whether to expect (and read) '}' character at the end of the feed. Used on StartFeed scope for top level
            /// feeds and on NavigationLinkStart scope for nested expanded feed.
            /// true - if the feed was wrapped in results wrapper
            /// false - if the feed was not wrapped in results wrapper
            /// </summary>
            public bool FeedHasResultsWrapper { get; set; }

            /// <summary>
            /// Flag which is only used on a StartNavigationLink scope in requests.
            /// true - we already found some content for the navigation link in question and it was (or is going to be) reported to the caller.
            /// false - we haven't found any content for the navigation link yet.
            /// </summary>
            public bool ExpandedNavigationLinkInRequestHasContent { get; set; }

            /// <summary>
            /// The entry being read.
            /// </summary>
            ODataEntry IODataVerboseJsonReaderEntryState.Entry
            {
                get
                {
                    Debug.Assert(this.State == ODataReaderState.EntryStart, "The IODataJsonReaderEntryState is only supported on EntryStart scope.");
                    return (ODataEntry)this.Item;
                }
            }

            /// <summary>
            /// The entity type for the entry (if available).
            /// </summary>
            IEdmEntityType IODataVerboseJsonReaderEntryState.EntityType
            {
                get
                {
                    Debug.Assert(this.State == ODataReaderState.EntryStart, "The IODataJsonReaderEntryState is only supported on EntryStart scope.");
                    return this.EntityType;
                }
            }
        }
    }
}
