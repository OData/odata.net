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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData reader for the ATOM format.
    /// </summary>
    internal sealed class ODataAtomReader : ODataReaderCore
    {
        /// <summary>The input to read the payload from.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Not yet implemented.")]
        private readonly ODataAtomInputContext atomInputContext;

        /// <summary>The deserializer to use to read input.</summary>
        private readonly ODataAtomEntryAndFeedDeserializer atomEntryAndFeedDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The input to read the payload from.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read (in case of entry reader) or entries in the feed to be read (in case of feed reader).</param>
        /// <param name="readingFeed">true if the reader is created for reading a feed; false when it is created for reading an entry.</param>
        internal ODataAtomReader(ODataAtomInputContext atomInputContext, IEdmEntityType expectedEntityType, bool readingFeed)
            : base(atomInputContext, expectedEntityType, readingFeed)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(atomInputContext != null, "atomInputContext != null");

            this.atomInputContext = atomInputContext;
            this.atomEntryAndFeedDeserializer = new ODataAtomEntryAndFeedDeserializer(atomInputContext);
        }

        /// <summary>
        /// Returns the current entry state.
        /// </summary>
        private IODataAtomReaderEntryState CurrentEntryState
        {
            get
            {
                Debug.Assert(this.State == ODataReaderState.EntryStart, "This property can only be accessed in the EntryStart scope.");
                return (IODataAtomReaderEntryState)this.CurrentScope;
            }
        }

        /// <summary>
        /// Returns the current feed state.
        /// </summary>
        private IODataAtomReaderFeedState CurrentFeedState
        {
            get
            {
                Debug.Assert(this.State == ODataReaderState.FeedStart, "This property can only be accessed in the FeedStart scope.");
                return (IODataAtomReaderFeedState)this.CurrentScope;
            }
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  PayloadStart - assumes that the XML reader has not been used yet.
        /// Post-Condition: XmlNodeType.Element (empty) atom:entry   - The entry element when reading top-level entry and the entry element is empty.
        ///                 XmlNodeType.EndElement atom:entry        - The end element of the top-level entry (if there were no nav. links. in it).
        ///                 XmlNodeType.Element atom:link            - the atom:link element representing the first navigation link in the top-level entry.
        ///                 XmlNodeType.Element (empty) atom:feed    - The feed element when reading top-level feed and the feed element is empty.
        ///                 XmlNodeType.EndElement atom:feed         - The end element of the top-level feed (if there were no entries in it).
        ///                 XmlNodeType.Element atom:entry           - if the feed contains an entry (the start tag of the first entry).
        /// </remarks>
        protected override bool ReadAtStartImplementation()
        {
            this.atomEntryAndFeedDeserializer.ReadPayloadStart();

            if (this.ReadingFeed)
            {
                this.ReadFeedStart();
                return true;
            }

            this.ReadEntryStart();
            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:feed (empty)  - The empty start tag of atom:feed.
        ///                  XmlNodeType.EndElement atom:feed       - The end tag of the atom:feed.
        ///                  XmlNodeTYpe.Element atom:entry         - The start tag of the first entry element to read.
        /// Post-Condition:  XmlNodeType.Element atom:feed (empty)  - The empty start tag of atom:feed.
        ///                  XmlNodeType.EndElement atom:feed       - The end tag of the atom:feed.
        ///                  XmlNodeType.Element atom:entry (empty) - The empty start tag of the first entry in the feed.
        ///                  XmlNodeType.EndElement atom:entry      - The end tag of the first entry in the feed (if it had no nav. links).
        ///                  XmlNodeType.Element atom:link          - the atom:link element representing the first navigation link in the first entry in the feed.
        /// </remarks>
        protected override bool ReadAtFeedStartImplementation()
        {
            if (this.atomEntryAndFeedDeserializer.XmlReader.NodeType == XmlNodeType.EndElement || this.CurrentFeedState.FeedElementEmpty)
            {
                // End of the feed
                this.ReplaceScope(ODataReaderState.FeedEnd);
            }
            else
            {
                // First entry in the feed
                this.atomEntryAndFeedDeserializer.AssertXmlCondition(XmlNodeType.Element);
                Debug.Assert(
                    this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomEntryElementName &&
                    this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                    "The reader must be on atom:entry element.");

                this.ReadEntryStart();
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element (empty) atom:feed  - The empty start tag of atom:feed
        ///                  XmlNodeType.EndElement atom:feed       - The end element of the atom:feed
        /// Post-Condition:  Any                                    - The node right after the top-level atom:feed element
        ///                  XmlNodeType.EndElement atom:link       - The end of parent expanded link.
        /// </remarks>
        protected override bool ReadAtFeedEndImplementation()
        {
            bool isTopLevelFeed = this.IsTopLevel;
            this.PopScope(ODataReaderState.FeedEnd);

            // Read over the end element (or the empty start element)
            this.atomEntryAndFeedDeserializer.ReadFeedEnd();

            bool result;
            if (isTopLevelFeed)
            {
                // read the end-of-payload suffix
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");

                // Read the end of the payload
                this.atomEntryAndFeedDeserializer.ReadPayloadEnd();

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else
            {
                this.atomEntryAndFeedDeserializer.ReadNavigationLinkContentAfterExpansion(false);

                // replace the 'NavigationLinkStart' scope with the 'NavigationLinkEnd' scope
                Debug.Assert(this.CurrentScope.State == ODataReaderState.NavigationLinkStart, "Should be in NavigationLinkStart state.");
                this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:entry (empty) - The empty start tag of atom:entry.
        ///                  XmlNodeType.EndElement atom:entry      - The end tag of the atom:entry.
        ///                  XmlNodeType.Element atom:link          - The start tag of the atom:link which represents the first navigation link.
        ///                  XmlNodeType.Element (empty) m:inline   - the empty m:inline element of an expanded null entry.
        ///                  XmlNodeType.EndElement m:inline        - the end element m:inline of an expanded null entry.
        /// Post-Condition:  XmlNodeType.Element atom:entry (empty) - The empty start tag of atom:entry.
        ///                  XmlNodeType.EndElement atom:entry      - The end tag of the atom:entry.
        ///                  XmlNodeType.Element atom:link          - The start tag of the atom:link which represents the first navigation link.
        ///                  XmlNodeType.Element (empty) m:inline   - the empty m:inline element of an expanded null entry.
        ///                  XmlNodeType.EndElement m:inline        - the end element m:inline of an expanded null entry.
        /// </remarks>
        protected override bool ReadAtEntryStartImplementation()
        {
            if (this.CurrentEntry == null)
            {
                Debug.Assert(this.IsExpandedLinkContent, "null entry can only be reported in an expanded link.");
                this.atomEntryAndFeedDeserializer.AssertXmlCondition(true, XmlNodeType.EndElement);
                Debug.Assert(
                    this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.ODataInlineElementName &&
                    this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.ODataMetadataNamespace,
                    "The reader must be positioned on the m:inline empty start tag, or end element");

                // Expanded null entry is represented as empty m:inline element.
                // There's nothing to read, so move to the end entry state
                this.EndEntry();
            }
            else if (this.atomEntryAndFeedDeserializer.XmlReader.NodeType == XmlNodeType.EndElement || this.CurrentEntryState.EntryElementEmpty)
            {
                // End of the entry
                this.EndEntry();
            }
            else
            {
                Debug.Assert(this.CurrentEntryState.FirstNavigationLink != null, "We must have remembered the first navigation link.");

                this.StartNavigationLink(this.CurrentEntryState.FirstNavigationLink);
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element (empty) atom:entry - The empty start tag of atom:entry.
        ///                  XmlNodeType.EndElement atom:entry      - The end element of the atom:entry.
        ///                  XmlNodeType.Element (empty) m:inline   - the empty m:inline element of an expanded null entry.
        ///                  XmlNodeType.EndElement m:inline        - the end element m:inline of an expanded null entry.
        /// Post-Condition:  Any                                    - The node right after the top-level atom:entry element.
        ///                  XmlNodeType.EndElement atom:feed       - The end element of the parent feed for the entry.
        ///                  XmlNodeType.Element (empty) atom:entry - The empty start tag of the next entry in the parent feed.
        ///                  XmlNodeType.EndElement atom:entry      - The end element of the next entry in the parent feed (if it had no nav. links).
        ///                  XmlNodeType.Element atom:link          - The start tag of the atom:link which represents the first navigation link in the next entry in the parent feed.
        ///                  XmlNodeType.EndElement atom:link       - The end of parent expanded link.
        /// </remarks>
        protected override bool ReadAtEntryEndImplementation()
        {
            bool isTopLevel = this.IsTopLevel;
            bool isExpandedLinkContent = this.IsExpandedLinkContent;
            bool isNullEntry = this.CurrentEntry == null;
            this.PopScope(ODataReaderState.EntryEnd);

            Debug.Assert(!isNullEntry || isExpandedLinkContent, "Null entry can only occure inside an expanded link.");

            if (!isNullEntry)
            {
                // Read over the end element (or the empty start element)
                this.atomEntryAndFeedDeserializer.ReadEntryEnd();
            }

            bool result = true;
            if (isTopLevel)
            {
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");

                // Read the end of the payload
                this.atomEntryAndFeedDeserializer.ReadPayloadEnd();

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else if (isExpandedLinkContent)
            {
#if DEBUG
                if (isNullEntry)
                {
                    this.atomEntryAndFeedDeserializer.AssertXmlCondition(true, XmlNodeType.EndElement);
                    Debug.Assert(
                        this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.ODataInlineElementName &&
                        this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.ODataMetadataNamespace,
                        "The reader must be positied on the m:inline empty start tag, or end element");
                }
#endif

                this.atomEntryAndFeedDeserializer.ReadNavigationLinkContentAfterExpansion(isNullEntry);

                // replace the 'NavigationLinkStart' scope with the 'NavigationLinkEnd' scope
                Debug.Assert(this.CurrentScope.State == ODataReaderState.NavigationLinkStart, "Should be in NavigationLinkStart state.");
                this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
            }
            else
            {
                // End of entry in a feed
                Debug.Assert(this.State == ODataReaderState.FeedStart, "Expected reader to be in state feed start before reading the next entry.");

                // Continue reading the content of the parent feed
                if (this.atomEntryAndFeedDeserializer.ReadFeedContent(this.CurrentFeedState))
                {
                    this.atomEntryAndFeedDeserializer.AssertXmlCondition(XmlNodeType.Element);
                    Debug.Assert(
                        this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomEntryElementName &&
                        this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                        "The reader must be on the start element of atom:entry.");

                    // Found another entry in the feed
                    this.ReadEntryStart();
                }
                else
                {
                    this.atomEntryAndFeedDeserializer.AssertXmlCondition(XmlNodeType.EndElement);
                    Debug.Assert(
                        this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomFeedElementName &&
                        this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                        "The reader must be on the end element of atom:feed.");

                    // End of the parent feed
                    this.ReplaceScope(ODataReaderState.FeedEnd);
                }
            }

            return result;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element atom:link          - the atom:link element representing the navigation link.
        /// Post-Condition: XmlNodeType.Element (empty) atom:link  - the empty atom:link element of a deferred navigation link.
        ///                 XmlNodeType.EndElement atom:link       - the end element atom:link of a deferred navigation link.
        ///                 XmlNodeType.Element atom:entry         - the atom:entry element of the expanded entry.
        ///                 XmlNodeType.Element atom:feed          - the atom:feed element of the expanded feed.
        ///                 XmlNodeType.Element (empty) m:inline   - the empty m:inline element of an expanded null entry.
        ///                 XmlNodeType.EndElement m:inline        - the end element m:inline of an expanded null entry.
        /// </remarks>
        protected override bool ReadAtNavigationLinkStartImplementation()
        {
            this.atomEntryAndFeedDeserializer.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomLinkElementName &&
                this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on the atom:link element.");

            ODataNavigationLink currentNavigationLink = this.CurrentLink;
            IODataAtomReaderEntryState parentEntryState = (IODataAtomReaderEntryState)this.LinkParentEntityScope;

            if (this.atomEntryAndFeedDeserializer.XmlReader.IsEmptyElement)
            {
                // The link is not expanded - report a deferred link
                parentEntryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentNavigationLink, false, currentNavigationLink.IsCollection);
                this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
            }
            else
            {
                // Move to the first node in the atom:link element.
                this.atomEntryAndFeedDeserializer.XmlReader.Read();

                switch (this.atomEntryAndFeedDeserializer.ReadNavigationLinkContentBeforeExpansion())
                {
                    case ODataAtomDeserializerExpandedNavigationLinkContent.None:
                        // The link is not expanded - deferred link.
                        parentEntryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentNavigationLink, false, currentNavigationLink.IsCollection);
                        this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
                        break;

                    case ODataAtomDeserializerExpandedNavigationLinkContent.Empty:
                        // Empty m:inline found
                        if (currentNavigationLink.IsCollection == true)
                        {
                            throw new ODataException(Strings.ODataAtomReader_EmptyExpansionForCollection);
                        }

                        // Set the collection to false - it might not have been set yet.
                        currentNavigationLink.IsCollection = false;
                        parentEntryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentNavigationLink, true, false);

                        // The expected entity type for an expanded entry is the same as for the navigation link around it.
                        // Report null expanded entry.
                        this.EnterScope(ODataReaderState.EntryStart, null, this.CurrentEntityType);
                        break;

                    case ODataAtomDeserializerExpandedNavigationLinkContent.Entry:
                        // Expanded entry
                        if (currentNavigationLink.IsCollection == true)
                        {
                            throw new ODataException(Strings.ODataAtomReader_ExpandedEntryInFeedNavigationLink);
                        }

                        // Set the collection to false - it might not have been set yet.
                        currentNavigationLink.IsCollection = false;
                        parentEntryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentNavigationLink, true, false);

                        this.ReadEntryStart();
                        break;

                    case ODataAtomDeserializerExpandedNavigationLinkContent.Feed:
                        // Expanded feed
                        if (currentNavigationLink.IsCollection == false)
                        {
                            throw new ODataException(Strings.ODataAtomReader_ExpandedFeedInEntryNavigationLink);
                        }

                        // Set the collection to true - it might not have been set yet.
                        currentNavigationLink.IsCollection = true;
                        parentEntryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentNavigationLink, true, true);

                        this.ReadFeedStart();
                        break;

                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataAtomReader_ReadAtNavigationLinkStartImplementation));
                }
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (empty) atom:link  - the empty atom:link element of a deferred navigation link.
        ///                 XmlNodeType.EndElement atom:link       - the end element atom:link.
        /// Post-Condition: XmlNodeType.EndElement atom:entry      - the end element atom:entry of the parent entry if there are no more navigation links.
        ///                 XmlNodeType.Element atom:link          - the atom:link element of the next navigation link of the parent entry.
        /// </remarks>
        protected override bool ReadAtNavigationLinkEndImplementation()
        {
            this.atomEntryAndFeedDeserializer.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(
                this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomLinkElementName &&
                this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on the atom:link element (empty) or end element.");

            this.atomEntryAndFeedDeserializer.ReadNavigationLinkEnd();

            this.PopScope(ODataReaderState.NavigationLinkEnd);
            Debug.Assert(this.State == ODataReaderState.EntryStart, "this.State == ODataReaderState.EntryStart");

            ODataNavigationLink navigationLink = this.atomEntryAndFeedDeserializer.ReadEntryContent(this.CurrentEntryState);
            if (navigationLink == null)
            {
                // End of the entry
                this.EndEntry();
            }
            else
            {
                // Next navigation link on the entry
                this.StartNavigationLink(navigationLink);
            }

            return true;
        }

        /// <summary>
        /// Reads the start of an entry and sets up the reader state correctly.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element                    - The method will fail if it's not atom:entry.
        /// Post-Condition: XmlNodeType.Element (empty) atom:entry - The entry element when reading entry and the entry element is empty.
        ///                 XmlNodeType.EndElement atom:entry      - The end element of the entry (if there were no nav. links. in it)
        ///                 XmlNodeType.Element atom:link          - The start tag of the atom:link which represents the first navigation link in the entry.
        /// </remarks>
        private void ReadEntryStart()
        {
            this.atomEntryAndFeedDeserializer.AssertXmlCondition(XmlNodeType.Element);

            ODataEntry entry = ReaderUtils.CreateNewEntry();

            // Read the start of the entry (etag and type name)
            this.atomEntryAndFeedDeserializer.ReadEntryStart(entry);
            this.EnterScope(ODataReaderState.EntryStart, entry, this.CurrentEntityType);
            AtomScope entryScope = (AtomScope)this.CurrentScope;
            entryScope.DuplicatePropertyNamesChecker = this.atomInputContext.CreateDuplicatePropertyNamesChecker();

            // Read ahead to detect the type name and use it.
            string typeNameFromPayload = this.atomEntryAndFeedDeserializer.FindTypeName();
            if (typeNameFromPayload != null)
            {
                ODataAtomReaderUtils.ValidateTypeName(typeNameFromPayload);
            }

            this.ApplyEntityTypeNameFromPayload(typeNameFromPayload);

            ODataEntityPropertyMappingCache cachedEpm = this.CurrentEntryState.EntityType.EnsureEpmCache();
            if (cachedEpm != null)
            {
                entryScope.CachedEpm = cachedEpm;
            }

            Debug.Assert(
                this.atomEntryAndFeedDeserializer.XmlReader.NodeType == XmlNodeType.Element &&
                this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomEntryElementName &&
                this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The XML reader must be on the atom:entry element by now.");
            if (this.atomEntryAndFeedDeserializer.XmlReader.IsEmptyElement)
            {
                // If the entry element is empty, remember that so that we can easily decide what to do next time the reader is called.
                this.CurrentEntryState.EntryElementEmpty = true;
            }
            else
            {
                // Move to the first child node of the entry
                this.atomEntryAndFeedDeserializer.XmlReader.Read();

                // Read the entry content.
                // If we find a nav. link, store it on the scope for reporting once we report the entry itself.
                this.CurrentEntryState.FirstNavigationLink = this.atomEntryAndFeedDeserializer.ReadEntryContent(this.CurrentEntryState);
            }
        }

        /// <summary>
        /// End the entry.
        /// </summary>
        private void EndEntry()
        {
            Debug.Assert(this.State == ODataReaderState.EntryStart, "We can only end entry if we're at the entry start state.");

            IODataAtomReaderEntryState entryState = this.CurrentEntryState;
            ODataEntry entry = entryState.Entry;
            if (entry != null)
            {
                // Apply EPM properties
                if (entryState.CachedEpm != null)
                {
                    AtomScope entryScope = (AtomScope)this.CurrentScope;

                    // Always read the syndication EPM, even if we have no ATOM metadata available.
                    // This is needed since mapped multivalue properties need to be set to empty multivalues
                    // if there were no items for them in the payload.
                    EpmSyndicationReader.ReadEntryEpm(
                        entryState, 
                        this.atomInputContext.Version, 
                        this.atomInputContext.MessageReaderSettings);

                    if (entryScope.HasEpmCustomReaderValueCache)
                    {
                        EpmCustomReader.ReadEntryEpm(
                            entryState,
                            this.atomInputContext.Version,
                            this.atomInputContext.MessageReaderSettings);
                    }
                }

                // By default validate media resource
                // In WCF DS Server mode, validate media resource in ATOM (here)
                // In WCF DS Client mode, do not validate media resource.
                ODataBehaviorKind behaviorKind = this.atomInputContext.MessageReaderSettings.ReaderBehavior.BehaviorKind;
                bool validateMediaResource =
                    behaviorKind == ODataBehaviorKind.Default ||
                    behaviorKind == ODataBehaviorKind.WcfDataServicesServer;
                ValidationUtils.ValidateEntryMetadata(entry, this.CurrentEntityType, validateMediaResource);
            }

            this.ReplaceScope(ODataReaderState.EntryEnd);
        }

        /// <summary>
        /// Reads the start of a feed and sets up the reader state correctly.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element                    - The method will fail if it's not atom:feed.
        /// Post-Condition: XmlNodeType.Element (empty) atom:feed  - The feed element when reading top-level feed and the feed element is empty.
        ///                 XmlNodeType.EndElement atom:feed       - The end element of the top-level feed (if there were no entries in it).
        ///                 XmlNodeType.Element atom:entry         - if the feed contains an entry (the start tag of the first entry)
        /// </remarks>
        private void ReadFeedStart()
        {
            this.atomEntryAndFeedDeserializer.AssertXmlCondition(XmlNodeType.Element);

            ODataFeed feed = new ODataFeed();

            // Read the start of the feed
            this.atomEntryAndFeedDeserializer.ReadFeedStart();
            this.EnterScope(ODataReaderState.FeedStart, feed, this.CurrentEntityType);

            Debug.Assert(
                this.atomEntryAndFeedDeserializer.XmlReader.NodeType == XmlNodeType.Element &&
                this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomFeedElementName &&
                this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The XML reader must be on the atom:feed element by now.");
            if (this.atomEntryAndFeedDeserializer.XmlReader.IsEmptyElement)
            {
                this.CurrentFeedState.FeedElementEmpty = true;
            }
            else
            {
                // Move to the first child of the feed
                this.atomEntryAndFeedDeserializer.XmlReader.Read();

                // And read the feed content until we find either entry or end of the feed
                this.atomEntryAndFeedDeserializer.ReadFeedContent(this.CurrentFeedState);
            }
        }

        /// <summary>
        /// Starts the navigation link.
        /// Does metadata validation of the navigation link and sets up the reader to report it.
        /// </summary>
        /// <param name="navigationLink">The navigation link to start.</param>
        private void StartNavigationLink(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "Found navigation link without a name, or with empty name.");
            this.atomEntryAndFeedDeserializer.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomLinkElementName &&
                this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on atom:link element.");

            // Lookup the property in metadata
            IEdmProperty edmProperty = ValidationUtils.ValidateNavigationPropertyDefined(navigationLink.Name, this.CurrentEntryState.EntityType);

            IEdmEntityType navPropertyType = null;
            if (edmProperty != null)
            {
                Debug.Assert(edmProperty.PropertyKind == EdmPropertyKind.Navigation, "We should have already validated that the nav. property is of kind Navigation.");
                IEdmNavigationProperty navProperty = (IEdmNavigationProperty)edmProperty;
                navPropertyType = navProperty.To.EntityType;
                switch (navProperty.To.Multiplicity)
                {
                    case EdmAssociationMultiplicity.ZeroOrOne:  // fall through
                    case EdmAssociationMultiplicity.One:
                        if (navigationLink.IsCollection == true)
                        {
                            throw new ODataException(Strings.ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty(navigationLink.Name));
                        }

                        // Set it to false, it's either already set, or it's null.
                        navigationLink.IsCollection = false;

                        break;
                    case EdmAssociationMultiplicity.Many:
                    if (!navigationLink.IsCollection.HasValue)
                    {
                        // Only set collection to true if it was not set already. We need the payload to win so that we can later validate
                        // that if the link is expanded, the expansion is a feed.
                        navigationLink.IsCollection = true;
                    }

                    break;
                    case EdmAssociationMultiplicity.Unknown:    // fall through
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataAtomReader_StartNavigationLink));
                }
            }

            this.EnterScope(ODataReaderState.NavigationLinkStart, navigationLink, navPropertyType);
        }

        /// <summary>
        /// Creates a new <see cref="AtomScope"/> for the specified <paramref name="state"/> and
        /// with the provided <paramref name="item"/> and pushes it on the stack of scopes.
        /// </summary>
        /// <param name="state">The <see cref="ODataReaderState"/> to use for the new scope.</param>
        /// <param name="item">The item to attach with the state in the new scope.</param>
        /// <param name="expectedEntityType">The expected type for the new scope.</param>
        private void EnterScope(ODataReaderState state, ODataItem item, IEdmEntityType expectedEntityType)
        {
            this.EnterScope(new AtomScope(state, item, expectedEntityType));
        }

        /// <summary>
        /// Replaces the current scope with a new <see cref="AtomScope"/> with the specified <paramref name="state"/> and
        /// the item of the current scope.
        /// </summary>
        /// <param name="state">The <see cref="ODataReaderState"/> to use for the new scope.</param>
        private void ReplaceScope(ODataReaderState state)
        {
            this.ReplaceScope(new AtomScope(state, this.Item, this.CurrentEntityType));
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        private sealed class AtomScope : Scope, IODataAtomReaderEntryState, IODataAtomReaderFeedState
        {
            /// <summary>
            /// Flag indicating if we have already made a decision about the current Entry (represented by this scope)
            /// and its being MLE or not.
            /// If this property have null value, we don't know for sure yet (both are possible), it it has non-null value
            /// then we already know for sure and if we find something different we should fail.
            /// </summary>
            private bool? mediaLinkEntry;

            /// <summary>
            /// The ATOM entry metadata to fill as we read the content of the entry.
            /// </summary>
            /// <remarks>
            /// This is lazily initialized only when it's actually needed.
            /// </remarks>
            private AtomEntryMetadata atomEntryMetadata;

            /// <summary>
            /// The cache for values read from custom EPM.
            /// </summary>
            /// <remarks>
            /// This is lazily initialized only when it's actually needed.
            /// </remarks>
            private EpmCustomReaderValueCache epmCustomReaderValueCache;

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
            internal AtomScope(ODataReaderState state, ODataItem item, IEdmEntityType expectedEntityType)
                : base(state, item, expectedEntityType)
            {
            }

            /// <summary>
            /// Flag which indicates that the element representing the current state is empty.
            /// </summary>
            public bool ElementEmpty { get; set; }

            /// <summary>
            /// Flag indicating if we have already made a decision about the current Entry (represented by this scope)
            /// and its being MLE or not.
            /// If this property has a null value, we don't know for sure yet (both are possible), if it has non-null value
            /// then we already know for sure and if we find something different we should fail.
            /// </summary>
            public bool? MediaLinkEntry
            {
                get
                {
                    return this.mediaLinkEntry;
                }

                set
                {
                    if (this.mediaLinkEntry.HasValue && this.mediaLinkEntry.Value != value)
                    {
                        throw new ODataException(Strings.ODataAtomReader_MediaLinkEntryMismatch);
                    }

                    this.mediaLinkEntry = value;
                }
            }

            /// <summary>
            /// If the reader finds a navigation link to report, but it must first report the parent entry
            /// it will store the navigation link in this property. So this will only ever store the first navigation link of an entry.
            /// </summary>
            public ODataNavigationLink FirstNavigationLink { get; set; }

            /// <summary>
            /// The duplicate property names checker for the entry represented by the current state.
            /// </summary>
            public DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker { get; set; }

            /// <summary>
            /// The EPM information for the entry, or null if there's no EPM for this entry.
            /// </summary>
            public ODataEntityPropertyMappingCache CachedEpm { get; set; }

            /// <summary>
            /// trye if the EpmCustomReaderValueCache has been initialized; false otherwise.
            /// </summary>
            public bool HasEpmCustomReaderValueCache
            {
                get
                {
                    return this.epmCustomReaderValueCache != null;
                }
            }

            /// <summary>
            /// The entry being read.
            /// </summary>
            ODataEntry IODataAtomReaderEntryState.Entry
            {
                get 
                { 
                    Debug.Assert(this.State == ODataReaderState.EntryStart, "The IODataAtomReaderEntryState is only supported on EntryStart scope.");
                    return (ODataEntry)this.Item;
                }
            }

            /// <summary>
            /// The entity type for the entry (if available)
            /// </summary>
            IEdmEntityType IODataAtomReaderEntryState.EntityType
            {
                get
                {
                    Debug.Assert(this.State == ODataReaderState.EntryStart, "The IODataAtomReaderEntryState is only supported on EntryStart scope.");
                    return this.EntityType;
                }
            }

            /// <summary>
            /// Flag which indicates that the ATOM entry element representing the entry is empty.
            /// </summary>
            bool IODataAtomReaderEntryState.EntryElementEmpty
            {
                get
                {
                    Debug.Assert(this.State == ODataReaderState.EntryStart, "The IODataAtomReaderEntryState is only supported on EntryStart scope.");
                    return this.ElementEmpty;
                }

                set
                {
                    Debug.Assert(this.State == ODataReaderState.EntryStart, "The IODataAtomReaderEntryState is only supported on EntryStart scope.");
                    this.ElementEmpty = value;
                }
            }

            /// <summary>
            /// The ATOM entry metadata to fill as we read the content of the entry.
            /// </summary>
            AtomEntryMetadata IODataAtomReaderEntryState.AtomEntryMetadata
            {
                get
                {
                    // Note that we lazy init the property when askes for it through the interface
                    // which is what all the deserializers use.
                    if (this.atomEntryMetadata == null)
                    {
                        this.atomEntryMetadata = AtomMetadataReaderUtils.CreateNewAtomEntryMetadata();
                    }

                    return this.atomEntryMetadata;
                }
            }

            /// <summary>
            /// The cache for values read from custom EPM.
            /// </summary>
            /// <remarks>
            /// This should only be accessed if there's CachedEpm available for this entry.
            /// </remarks>
            EpmCustomReaderValueCache IODataAtomReaderEntryState.EpmCustomReaderValueCache
            {
                get
                {
                    // Note that we lazy init the property when askes for it through the interface
                    // which is what all the deserializers use.
                    return this.epmCustomReaderValueCache ??
                           (this.epmCustomReaderValueCache = new EpmCustomReaderValueCache());
                }
            }

            /// <summary>
            /// The feed being read.
            /// </summary>
            ODataFeed IODataAtomReaderFeedState.Feed
            {
                get
                {
                    Debug.Assert(this.State == ODataReaderState.FeedStart, "The IODataAtomReaderFeedState is only supported on FeedStart scope.");
                    return (ODataFeed)this.Item;
                }
            }

            /// <summary>
            /// Flag which indicates that the ATOM feed element representing the feed is empty.
            /// </summary>
            bool IODataAtomReaderFeedState.FeedElementEmpty
            {
                get
                {
                    Debug.Assert(this.State == ODataReaderState.FeedStart, "The IODataAtomReaderFeedState is only supported on FeedStart scope.");
                    return this.ElementEmpty;
                }

                set
                {
                    Debug.Assert(this.State == ODataReaderState.FeedStart, "The IODataAtomReaderFeedState is only supported on FeedStart scope.");
                    this.ElementEmpty = value;
                }
            }
        }
    }
}
