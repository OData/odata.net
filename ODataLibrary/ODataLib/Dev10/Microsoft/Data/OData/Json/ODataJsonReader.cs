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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData reader for the JSON format.
    /// </summary>
    internal sealed class ODataJsonReader : ODataReaderCore
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataJsonInputContext jsonInputContext;

        /// <summary>The entry and feed deserializer to read input with.</summary>
        private readonly ODataJsonEntryAndFeedDeserializer jsonEntryAndFeedDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The input to read the payload from.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read (in case of entry reader) or entries in the feed to be read (in case of feed reader).</param>
        /// <param name="readingFeed">true if the reader is created for reading a feed; false when it is created for reading an entry.</param>
        internal ODataJsonReader(ODataJsonInputContext jsonInputContext, IEdmEntityType expectedEntityType, bool readingFeed)
            : base(jsonInputContext, expectedEntityType, readingFeed)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonInputContext != null, "jsonInputContext != null");

            this.jsonInputContext = jsonInputContext;
            this.jsonEntryAndFeedDeserializer = new ODataJsonEntryAndFeedDeserializer(jsonInputContext);

            if (!this.jsonInputContext.Model.IsUserModel())
            {
                throw new ODataException(Strings.ODataJsonReader_ParsingWithoutMetadata);
            }
        }

        /// <summary>
        /// Returns the current entry state.
        /// </summary>
        private IODataJsonReaderEntryState CurrentEntryState
        {
            get
            {
                Debug.Assert(this.State == ODataReaderState.EntryStart, "This property can only be accessed in the EntryStart scope.");
                return (IODataJsonReaderEntryState)this.CurrentScope;
            }
        }

        /// <summary>
        /// Set to true if feeds are expected to have the 'results' wrapper.
        /// Feeds are only expected to have a results wrapper if
        /// (a) the protocol version is >= 2 AND
        /// (b) we are reading a response OR an expanded link payload (in requests and responses)
        /// NOTE: OIPI does not specify a format for >= v2 feeds in requests; we thus use the v1 format and consequently do not expect a result wrapper.
        /// </summary>
        private bool IsResultsWrapperExpected
        {
            get
            {
                return this.jsonInputContext.Version >= ODataVersion.V2 && (this.jsonInputContext.ReadingResponse || !this.IsTopLevel);
            }
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.None:      assumes that the JSON reader has not been used yet.
        /// Post-Condition: The reader is positioned on the first node of the payload (this can be the first node or the value of the 'd' property node)
        /// </remarks>
        protected override bool ReadAtStartImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
            Debug.Assert(this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None");

            // read the data wrapper depending on whether we are reading a request or response
            this.jsonEntryAndFeedDeserializer.ReadDataWrapperStart();

            if (this.ReadingFeed)
            {
                // The expected type for the top-level feed is the same as for the entire reader (the start state).
                this.StartFeed();
            }
            else
            {
                // The expected type for the top-level entry is the same as for the entire reader (the start state).
                this.StartEntry();
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  The first node of the feed; this method will throw if the node is not
        ///                 JsonNodeType.StartArray:    for a v1 feed
        ///                 JsonNodeType.StartObject:   for a >=v2 feed
        /// Post-Condition: The reader is positioned over the StartObject node of the first entry in the feed or 
        ///                 on the node following the feed end in case of an empty feed
        /// </remarks>
        protected override bool ReadAtFeedStartImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.FeedStart, "this.State == ODataReaderState.FeedStart");

            if (this.IsResultsWrapperExpected && this.jsonEntryAndFeedDeserializer.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(Strings.ODataJsonEntryAndFeedDeserializer_CannotReadWrappedFeedStart(this.jsonEntryAndFeedDeserializer.JsonReader.NodeType));
            }
            else if (!this.IsResultsWrapperExpected && this.jsonEntryAndFeedDeserializer.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(Strings.ODataJsonReader_CannotReadFeedStart(this.jsonEntryAndFeedDeserializer.JsonReader.NodeType));
            }

            // read the start of the feed until we determine the next item not belonging to the feed
            ODataFeed feed = this.CurrentFeed;
            this.jsonEntryAndFeedDeserializer.ReadFeedStart(feed, this.IsResultsWrapperExpected);

            this.jsonEntryAndFeedDeserializer.JsonReader.ReadStartArray();

            // figure out whether the feed contains entries or not
            switch (this.jsonEntryAndFeedDeserializer.JsonReader.NodeType)
            {
                // we are at the beginning of an entry
                // The expected type for an entry in the feed is the same as for the feed itself.
                case JsonNodeType.StartObject:
                    this.StartEntry();
                    break;
                case JsonNodeType.EndArray:
                    this.jsonEntryAndFeedDeserializer.ReadFeedEnd(feed, this.IsResultsWrapperExpected);
                    this.ReplaceScope(ODataReaderState.FeedEnd);
                    break;
                default:
                    throw new ODataException(Strings.ODataJsonReader_CannotReadEntriesOfFeed(this.jsonEntryAndFeedDeserializer.JsonReader.NodeType));
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
        /// Post-Condition: JsonNodeType.EndOfInput     for a top-level feed
        /// Pre-Condition:  JsonNodeType.Property       more properties exist on the owning entry after the expanded link containing the feed
        ///                 JsonNodeType.EndObject      no further properties exist on the owning entry after the expanded link containing the feed
        /// </remarks>
        protected override bool ReadAtFeedEndImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.FeedEnd, "this.State == ODataReaderState.FeedEnd");
            Debug.Assert(
                !this.IsResultsWrapperExpected && this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndArray ||
                this.IsResultsWrapperExpected && this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: expected JsonNodeType.EndObject or JsonNodeType.EndArray");

            bool isTopLevelFeed = this.IsTopLevel;

            // Read the end-object node of the wrapped feed or the end-array node of the v1 feed
            // and position the reader on the next input node
            // This can hit the end of the input.
            this.jsonEntryAndFeedDeserializer.JsonReader.Read();

            this.PopScope(ODataReaderState.FeedEnd);

            bool result;
            if (isTopLevelFeed)
            {
                // read the end-of-payload suffix
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
                this.jsonEntryAndFeedDeserializer.ReadDataWrapperEnd();
                Debug.Assert(this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput, "Expected JSON reader to have reached the end of input.");

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else
            {
                // finish reading the expanded link
                this.ReadExpandedNavigationLinkEnd(true);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  First node of the entry; this method will throw if the node is not a JsonNodeType.StartObject or JsonNodeType.PrimitiveValue (null)
        /// Post-Condition: JsonNodeType.StartObject            The first node of the navigation link property value to read next
        ///                 JsonNodeType.StartArray             The first node of the navigation link property value with a v1 feed to read next
        ///                 JsonNodeType.PrimitiveValue (null)  The null expanded entry value (representing the end of that entry)
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        protected override bool ReadAtEntryStartImplementation()
        {
            if (this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue &&
                this.jsonEntryAndFeedDeserializer.JsonReader.Value == null &&
                this.IsExpandedLinkContent)
            {
                Debug.Assert(this.Item == null, "Null entry should have been reported, if null expanded entry value was found.");

                // Expanded null entry is represented as null primitive value
                // There's nothing to read, so move to the end entry state
                this.ReplaceScope(ODataReaderState.EntryEnd);
            }
            else
            {
                if (this.jsonEntryAndFeedDeserializer.JsonReader.NodeType != JsonNodeType.StartObject)
                {
                    throw new ODataException(Strings.ODataJsonReader_CannotReadEntryStart(this.jsonEntryAndFeedDeserializer.JsonReader.NodeType));
                }

                // read over the StartObject node and position the reader on the first node of the entry's content
                this.jsonEntryAndFeedDeserializer.JsonReader.ReadNext();

                // read ahead to detect the type name in __metadata
                string typeNameFromPayload = this.jsonEntryAndFeedDeserializer.FindTypeNameInPayload();
                this.ApplyEntityTypeNameFromPayload(typeNameFromPayload);

                Debug.Assert(this.CurrentEntityType != null, "We must have some entity type after the type resolution.");

                this.ReadEntryContents();
            }

            Debug.Assert(
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                !this.IsResultsWrapperExpected && this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonEntryAndFeedDeserializer.JsonReader.Value == null ||
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
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
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonEntryAndFeedDeserializer.JsonReader.Value == null,
                "Pre-Condition: JsonNodeType.EndObject or JsonNodeType.PrimitiveValue (null)");

            bool isTopLevel = this.IsTopLevel;
            bool isExpandedLinkContent = this.IsExpandedLinkContent;

            this.PopScope(ODataReaderState.EntryEnd);

            // Read over the end object node and position the reader on the next node in the input.
            // This can hit the end of the input.
            this.jsonEntryAndFeedDeserializer.JsonReader.Read();
            JsonNodeType nodeType = this.jsonEntryAndFeedDeserializer.JsonReader.NodeType;

            // analyze the next Json token to determine whether it is start object (next entry), end array (feed end) or eof (top-level entry end)
            bool result = true;
            if (isTopLevel)
            {
                // NOTE: we rely on the underlying JSON reader to fail if there is more than one value at the root level.
                Debug.Assert(
                    !this.jsonEntryAndFeedDeserializer.ReadingResponse && nodeType == JsonNodeType.EndOfInput ||  // top-level entry end in a request
                    this.jsonEntryAndFeedDeserializer.ReadingResponse && nodeType == JsonNodeType.EndObject,      // top-level entry end in a response
                    "Invalid JSON reader state for reading the end of a top-level entry.");

                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");

                // read the end-of-payload suffix
                this.jsonEntryAndFeedDeserializer.ReadDataWrapperEnd();
                Debug.Assert(this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndOfInput, "Expected JSON reader to have reached the end of input.");

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
                switch (nodeType)
                {
                    case JsonNodeType.StartObject:
                        // another entry in a feed
                        Debug.Assert(this.State == ODataReaderState.FeedStart, "Expected reader to be in state feed start before reading the next entry.");
                        this.StartEntry();
                        break;
                    case JsonNodeType.EndArray:
                        // we are at the end of a feed
                        Debug.Assert(this.State == ODataReaderState.FeedStart, "Expected reader to be in state feed start after reading the last entry in the feed.");
                        this.jsonEntryAndFeedDeserializer.ReadFeedEnd(this.CurrentFeed, this.IsResultsWrapperExpected);
                        this.ReplaceScope(ODataReaderState.FeedEnd);
                        break;
                    default:
                        Debug.Assert(false, "We should never find a node that is not covered by the above conditions.");
                        break;
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
        ///                 JsonNodeType.StartArray             v1 feed inside of expanded link
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        /// Post-Condition: JsonNodeType.StartArray:            expanded link with v1 feed
        ///                 JsonNodeType.StartObject            expanded link with >=v2 feed or entry
        ///                 JsonNodeType.PrimitiveValue (null)  expanded null entry
        ///                 JsonNodeType.Property               deferred link with more properties in owning entry
        ///                 JsonNodeType.EndObject              deferred link as last property of the owning entry
        /// </remarks>
        protected override bool ReadAtNavigationLinkStartImplementation()
        {
            Debug.Assert(
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject || this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonEntryAndFeedDeserializer.JsonReader.Value == null,
                "Pre-Condition: expected JsonNodeType.StartObject, JsonNodeType.StartArray or JsonNodeType.Primitive (null)");

            ODataNavigationLink currentLink = this.CurrentLink;
            Debug.Assert(currentLink.IsCollection.HasValue, "Expect to know whether this is a singleton or collection link based on metadata.");

            IODataJsonReaderEntryState parentEntryState = (IODataJsonReaderEntryState)this.LinkParentEntityScope;

            if (!this.jsonEntryAndFeedDeserializer.IsExpandedLink())
            {
                // read the rest of the deferred link
                parentEntryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentLink, false, currentLink.IsCollection);
                this.jsonEntryAndFeedDeserializer.ReadDeferredNavigationLinkEnd(currentLink);
                Debug.Assert(
                    this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject || this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.Property,
                    "Post-Condition: JsonNodeType.EndObject or JsonNodeType.Property");

                this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
            }
            else if (!currentLink.IsCollection.Value)
            {
                parentEntryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentLink, true, false);
                if (this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
                {
                    Debug.Assert(this.jsonEntryAndFeedDeserializer.JsonReader.Value == null, "If a primitive value is representing an expanded entity its value must be null.");

                    // The expected type for an expanded entry is the same as for the navigation link around it.
                    this.EnterScope(ODataReaderState.EntryStart, null, this.CurrentEntityType);
                }
                else
                {
                    // The expected type for an expanded entry is the same as for the navigation link around it.
                    this.StartEntry();
                }
            }
            else
            {
                // Expanded link with feed content
                // The expected type for an expanded feed is the same as for the navigation link around it.
                parentEntryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentLink, true, true);
                this.StartFeed();
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
        ///                 JsonNodeType.StartArray         The first node of the navigation link property value with a v1 feed to read next
        ///                 JsonNodeType.EndObject          If no (more) properties exist in the entry's content
        ///                 JsonNoteType.Primitive (null)   If an expanded link with null entity instance was found.
        /// </remarks>
        protected override bool ReadAtNavigationLinkEndImplementation()
        {
            Debug.Assert(
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.Property,
                "Pre-Condition: JsonNodeType.EndObject or JsonNodeType.Property");

            this.PopScope(ODataReaderState.NavigationLinkEnd);
            Debug.Assert(this.State == ODataReaderState.EntryStart, "this.State == ODataReaderState.EntryStart");

            JsonNodeType nodeType = this.jsonEntryAndFeedDeserializer.JsonReader.NodeType;
            switch (nodeType)
            {
                case JsonNodeType.EndObject:
                    this.ReplaceScope(ODataReaderState.EntryEnd);
                    break;
                case JsonNodeType.Property:
                    this.ReadEntryContents();
                    Debug.Assert(
                        this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                        !this.IsResultsWrapperExpected && this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                        this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject ||
                        (this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonEntryAndFeedDeserializer.JsonReader.Value == null),
                        "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.EndObject or JsonNoteType.Primitive (null)");
                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataJsonReader_ReadImplementation_NavigationLinkEnd));
            }

            return true;
        }

        /// <summary>
        /// Read the contents of an entry; this can be properties and/or navigation links.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The property to read
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// Post-Condition: JsonNodeType.StartObject            The first node of the navigation link property value to read next (deferred link or entry or >=v2 feed wrapper)
        ///                 JsonNodeType.StartArray             The first node of the navigation link property value with a v1 feed to read next
        ///                 JsonNodeType.PrimitiveValue (null)  The null value of the navigation link property value to read next (expanded null entry)
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the entry's content
        /// </remarks>
        private void ReadEntryContents()
        {
            Debug.Assert(
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.Property ||
                this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: JsonNodeType.Property or JsonNodeType.EndObject");

            // for now we require the entity type; we will have to relax this once we support reading JSON without metadata
            Debug.Assert(this.CurrentEntityType != null, "this.CurrentEntityType != null");
            this.jsonEntryAndFeedDeserializer.JsonReader.AssertNotBuffering();

            // read the start of the entry until we find a navigation link or the end of the entry
            IEdmNavigationProperty navigationProperty;
            ODataNavigationLink navigationLink = this.jsonEntryAndFeedDeserializer.ReadEntryContent(
                this.CurrentEntryState,
                out navigationProperty);

            if (navigationLink == null)
            {
                Debug.Assert(this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.EndObject, "Post-Condition: expected JsonNodeType.EndObject");
                Debug.Assert(navigationProperty == null, "navigationProperty == null");
                
                // at this point we read all of the entry using ReadEntryContents method above
                this.ReplaceScope(ODataReaderState.EntryEnd);
            }
            else
            {
                Debug.Assert(
                    this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartObject ||
                    !this.IsResultsWrapperExpected && this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.StartArray ||
                    this.jsonEntryAndFeedDeserializer.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.jsonEntryAndFeedDeserializer.JsonReader.Value == null,
                    "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Primitive (null)");
                Debug.Assert(navigationProperty != null, "A navigation property must be found for each link we find.");

                // we are at the beginning of a link
                Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "Navigation links must have a name.");

                this.EnterScope(ODataReaderState.NavigationLinkStart, navigationLink, navigationProperty.To.EntityType);
            }

            this.jsonEntryAndFeedDeserializer.JsonReader.AssertNotBuffering();
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
            this.CurrentLink.IsCollection = isCollection;

            // replace the 'NavigationLinkStart' scope with the 'NavigationLinkEnd' scope
            this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
        }

        /// <summary>
        /// Starts the entry, initializing the scopes and such. This method starts a non-null entry only.
        /// </summary>
        private void StartEntry()
        {
            this.EnterScope(ODataReaderState.EntryStart, ReaderUtils.CreateNewEntry(), this.CurrentEntityType);
            ((JsonScope)this.CurrentScope).DuplicatePropertyNamesChecker = this.jsonInputContext.CreateDuplicatePropertyNamesChecker();
        }

        /// <summary>
        /// Starts the feed, initializing the scopes and such.
        /// </summary>
        private void StartFeed()
        {
            this.EnterScope(ODataReaderState.FeedStart, new ODataFeed(), this.CurrentEntityType);
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
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class JsonScope : Scope, IODataJsonReaderEntryState
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
                : base(state, item, expectedEntityType)
            {
            }

            /// <summary>
            /// Flag which indicates that during parsing of the entry represented by this scope,
            /// the __metadata property was already found.
            /// </summary>
            public bool MetadataPropertyFound { get; set; }

            /// <summary>
            /// The duplicate property names checker for the entry represented by the current state.
            /// </summary>
            public DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker { get; set; }

            /// <summary>
            /// The entry being read.
            /// </summary>
            ODataEntry IODataJsonReaderEntryState.Entry
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
            IEdmEntityType IODataJsonReaderEntryState.EntityType
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
