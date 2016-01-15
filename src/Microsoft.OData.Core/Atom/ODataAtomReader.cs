//---------------------------------------------------------------------
// <copyright file="ODataAtomReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// OData reader for the ATOM format.
    /// </summary>
    internal sealed class ODataAtomReader : ODataReaderCore
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataAtomInputContext atomInputContext;

        /// <summary>The deserializer to use to read input.</summary>
        private ODataAtomEntryAndFeedDeserializer atomEntryAndFeedDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The input to read the payload from.</param>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read (in case of entry reader) or entries in the feed to be read (in case of feed reader).</param>
        /// <param name="readingFeed">true if the reader is created for reading a feed; false when it is created for reading an entry.</param>
        internal ODataAtomReader(
            ODataAtomInputContext atomInputContext, 
            IEdmNavigationSource navigationSource, 
            IEdmEntityType expectedEntityType, 
            bool readingFeed)
            : base(atomInputContext, readingFeed, false /*readingDelta*/, null /*listener*/)
        {
            Debug.Assert(atomInputContext != null, "atomInputContext != null");
            Debug.Assert(
                expectedEntityType == null || atomInputContext.Model.IsUserModel(),
                "If the expected type is specified we need model as well. We should have verified that by now.");

            this.atomInputContext = atomInputContext;
            this.atomEntryAndFeedDeserializer = new ODataAtomEntryAndFeedDeserializer(atomInputContext);

            this.EnterScope(new Scope(ODataReaderState.Start, /*item*/ null, navigationSource, expectedEntityType, /*contextUri*/null));
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
            Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");

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
        ///                  XmlNodeType.Element atom:entry         - The start tag of the first entry element to read.
        ///                  XmlNodeType.Element m:inline (empty)   - The empty start tag of an expanded navigation link m:inline element which we report as empty feed.
        ///                  XmlNodeType.EndElement m:inline        - The end tag of an expanded navigation link m:inline element which we report as empty feed.
        /// Post-Condition:  XmlNodeType.Element atom:feed (empty)  - The empty start tag of atom:feed.
        ///                  XmlNodeType.EndElement atom:feed       - The end tag of the atom:feed.
        ///                  XmlNodeType.Element atom:entry (empty) - The empty start tag of the first entry in the feed.
        ///                  XmlNodeType.EndElement atom:entry      - The end tag of the first entry in the feed (if it had no nav. links).
        ///                  XmlNodeType.Element atom:link          - the atom:link element representing the first navigation link in the first entry in the feed.
        ///                  XmlNodeType.Element m:inline (empty)   - The empty start tag of an expanded navigation link m:inline element which we report as empty feed.
        ///                  XmlNodeType.EndElement m:inline        - The end tag of an expanded navigation link m:inline element which we report as empty feed.
        /// </remarks>
        protected override bool ReadAtFeedStartImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.FeedStart, "this.State == ODataReaderState.FeedStart");

            if (this.atomEntryAndFeedDeserializer.XmlReader.NodeType == XmlNodeType.EndElement ||
                this.CurrentFeedState.FeedElementEmpty)
            {
                Debug.Assert(
                    (this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomFeedElementName &&
                     this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace) ||
                    (this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.ODataInlineElementName &&
                     this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.ODataMetadataNamespace),
                    "The reader must be on either end element or empty start element with name either atom:feed or m:inline.");

                this.ReplaceScopeToFeedEnd();
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
        ///                  XmlNodeType.Element m:inline (empty)   - The empty start tag of an expanded navigation link m:inline element which we report as empty feed.
        ///                  XmlNodeType.EndElement m:inline        - The end tag of an expanded navigation link m:inline element which we report as empty feed.
        /// Post-Condition:  Any                                    - The node right after the top-level atom:feed element
        ///                  XmlNodeType.EndElement atom:link       - The end of parent expanded link.
        /// </remarks>
        protected override bool ReadAtFeedEndImplementation()
        {
            Debug.Assert(this.State == ODataReaderState.FeedEnd, "this.State == ODataReaderState.FeedEnd");

            bool isTopLevelFeed = this.IsTopLevel;
            bool isEmptyInlineFeed = this.atomEntryAndFeedDeserializer.IsReaderOnInlineEndElement();
            Debug.Assert(!isTopLevelFeed || !isEmptyInlineFeed, "Empty inline feed can't be detected on the top-level.");

            // If we're on the m:inline of the parent navigation link, it means we reported an empty m:inline element as an empty feed
            // in that case read nothing here, just move to the navigation link end state.
            if (!isEmptyInlineFeed)
            {
                Debug.Assert(
                    this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomFeedElementName &&
                    this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                    "The reader must be on atom:feed end element or empty start element");

                // Read over the end element (or the empty start element)
                this.atomEntryAndFeedDeserializer.ReadFeedEnd();
            }

            this.PopScope(ODataReaderState.FeedEnd);

            bool result;
            if (isTopLevelFeed)
            {
                // Read the end of the payload
                Debug.Assert(this.State == ODataReaderState.Start, "this.State == ODataReaderState.Start");
                this.atomEntryAndFeedDeserializer.ReadPayloadEnd();

                // replace the 'Start' scope with the 'Completed' scope
                this.ReplaceScope(ODataReaderState.Completed);
                result = false;
            }
            else
            {
                this.atomEntryAndFeedDeserializer.ReadNavigationLinkContentAfterExpansion(isEmptyInlineFeed);

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
            else if (this.atomInputContext.UseServerApiBehavior)
            {
                // In WCF DS Server mode we don't read ahead but report the entry right after type name.
                // So we need to read the entry content now.
                ODataAtomReaderNavigationLinkDescriptor navigationLinkDescriptor = this.atomEntryAndFeedDeserializer.ReadEntryContent(this.CurrentEntryState);
                if (navigationLinkDescriptor == null)
                {
                    // End of the entry
                    this.EndEntry();
                }
                else
                {
                    // Next navigation link on the entry
                    this.StartNavigationLink(navigationLinkDescriptor);
                }
            }
            else
            {
                Debug.Assert(this.CurrentEntryState.FirstNavigationLinkDescriptor != null, "We must have remembered the first navigation link.");

                this.StartNavigationLink(this.CurrentEntryState.FirstNavigationLinkDescriptor);
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
                if (this.atomEntryAndFeedDeserializer.ReadFeedContent(this.CurrentFeedState, this.IsExpandedLinkContent))
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

                    this.ReplaceScopeToFeedEnd();
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

            ODataNavigationLink currentNavigationLink = this.CurrentNavigationLink;
            IODataAtomReaderEntryState parentEntryState = (IODataAtomReaderEntryState)this.LinkParentEntityScope;

            AtomScope entryScope = (AtomScope)this.CurrentScope;
            IEdmNavigationProperty navigationProperty = entryScope.NavigationProperty;

            if (this.atomEntryAndFeedDeserializer.XmlReader.IsEmptyElement)
            {
                this.ReadAtNonExpandedNavigationLinkStart();
            }
            else
            {
                // Move to the first node in the atom:link element.
                this.atomEntryAndFeedDeserializer.XmlReader.Read();

                ODataAtomDeserializerExpandedNavigationLinkContent expandedNavigationLinkContent = this.atomEntryAndFeedDeserializer.ReadNavigationLinkContentBeforeExpansion();
                if (expandedNavigationLinkContent != ODataAtomDeserializerExpandedNavigationLinkContent.None)
                {
                    if (navigationProperty == null && this.atomInputContext.Model.IsUserModel() &&
                        this.atomInputContext.MessageReaderSettings.ReportUndeclaredLinkProperties)
                    {
                        // Undeclared navigation link with content which we should read anyway.
                        // If we are to ignore value properties, then skip the content and read the link only as deferred link, otherwise fail.
                        if (this.atomInputContext.MessageReaderSettings.IgnoreUndeclaredValueProperties)
                        {
                            // The reader is positioned on some element inside the link (either </m:inline> or <entry> or <feed>)
                            // So we need to read until we find the end-element for the link and skip everything in between.
                            this.atomEntryAndFeedDeserializer.SkipNavigationLinkContentOnExpansion();

                            // And report it as a deferred link.
                            this.ReadAtNonExpandedNavigationLinkStart();
                            return true;
                        }
                        else
                        {
                            throw new ODataException(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType(currentNavigationLink.Name, this.LinkParentEntityScope.EntityType.FullTypeName()));
                        }
                    }
                }

                switch (expandedNavigationLinkContent)
                {
                    case ODataAtomDeserializerExpandedNavigationLinkContent.None:
                        this.ReadAtNonExpandedNavigationLinkStart();
                        break;

                    case ODataAtomDeserializerExpandedNavigationLinkContent.Empty:
                        // Empty m:inline found
                        // [Client-ODataLib-Integration] Client reads empty m:inline as empty feed, server doesn't.
                        // We decided to relax ODataLib and read empty inline element as an empty feed.
                        if (currentNavigationLink.IsCollection == true)
                        {
                            ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentNavigationLink, true, true);

                            // The expected entity type for an expanded empty feed is the same as for the navigation link around it.
                            // Report completely empty feed.
                            this.EnterScope(ODataReaderState.FeedStart, new ODataFeed(), this.CurrentEntityType);

                            // We mark the feed as having an empty element, this makes us treat it as empty in the FeedStart state.
                            // Later when we're in the FeedEnd state we will inspect the actual element name to recognize m:inline
                            // as a special case.
                            this.CurrentFeedState.FeedElementEmpty = true;
                        }
                        else
                        {
                            // Set the collection to false - it might not have been set yet.
                            currentNavigationLink.IsCollection = false;
                            ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentNavigationLink, true, false);

                            // The expected entity type for an expanded entry is the same as for the navigation link around it.
                            // Report null expanded entry.
                            this.EnterScope(ODataReaderState.EntryStart, null, this.CurrentEntityType);
                        }

                        break;

                    case ODataAtomDeserializerExpandedNavigationLinkContent.Entry:
                        // Expanded entry
                        // For expanded links, we could infer the cardinality in three different ways:
                        //  (a) From metadata
                        //  (b) From the "type" attribute in entry/content/@type
                        //  (c) From the child of "m:inline" element
                        //  Note that (c) always trumps (b). We need to make sure (a) and (c) match.
                        //  The IsCollection property will also be set based on entry/content/@type. So, double check 
                        //  to see if the metadata says the nav link is for a collection.
                        if (currentNavigationLink.IsCollection == true || (navigationProperty != null && navigationProperty.Type.IsCollection()))
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomReader_ExpandedEntryInFeedNavigationLink);
                        }

                        // Set the collection to false - it might not have been set yet.
                        currentNavigationLink.IsCollection = false;
                        ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentNavigationLink, true, false);

                        this.ReadEntryStart();
                        break;

                    case ODataAtomDeserializerExpandedNavigationLinkContent.Feed:
                        // Expanded feed
                        if (currentNavigationLink.IsCollection == false)
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomReader_ExpandedFeedInEntryNavigationLink);
                        }

                        // Set the collection to true - it might not have been set yet.
                        currentNavigationLink.IsCollection = true;
                        ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentNavigationLink, true, true);

                        this.ReadFeedStart();
                        break;

                    default:
                        throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataAtomReader_ReadAtNavigationLinkStartImplementation));
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

            ODataAtomReaderNavigationLinkDescriptor navigationLinkDescriptor = this.atomEntryAndFeedDeserializer.ReadEntryContent(this.CurrentEntryState);
            if (navigationLinkDescriptor == null)
            {
                // End of the entry
                this.EndEntry();
            }
            else
            {
                // Next navigation link on the entry
                this.StartNavigationLink(navigationLinkDescriptor);
            }

            return true;
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (empty) atom:link - the empty atom:link element of an entity reference link.
        ///                 XmlNodeType.EndElement atom:link      - the end element atom:link of the entity reference link.
        /// Post-Condition: Unchanged                             - the reader doesn't change its position.
        /// </remarks>
        protected override bool ReadAtEntityReferenceLink()
        {
            this.atomEntryAndFeedDeserializer.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(
                this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomLinkElementName &&
                this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on the atom:link element (empty) or end element.");

            this.PopScope(ODataReaderState.EntityReferenceLink);
            Debug.Assert(this.State == ODataReaderState.NavigationLinkStart, "this.State == ODataReaderState.NavigationLinkStart");

            // Change the start to end, since entity reference link has no content or anything and can only apear alone in the navigation link.
            // Note that this is a reader limitation, writer allows multiple entity reference links inside a single nav. link.
            this.ReplaceScope(ODataReaderState.NavigationLinkEnd);

            return true;
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
                this.atomEntryAndFeedDeserializer.ReadFeedContent(this.CurrentFeedState, this.IsExpandedLinkContent);
            }
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

            // Setup the new entry state
            this.EnterScope(ODataReaderState.EntryStart, entry, this.CurrentEntityType);
            AtomScope entryScope = (AtomScope)this.CurrentScope;
            entryScope.DuplicatePropertyNamesChecker = this.atomInputContext.CreateDuplicatePropertyNamesChecker();

            // Read ahead to detect the type name and use it.
            string typeNameFromPayload = this.atomEntryAndFeedDeserializer.FindTypeName();

            this.ApplyEntityTypeNameFromPayload(typeNameFromPayload);

            // Validate type with feed validator if available
            if (this.CurrentFeedValidator != null)
            {
                this.CurrentFeedValidator.ValidateEntry(this.CurrentEntityType);
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

                // In WCF DS Server mode we must not read ahead and report the type name only.
                if (this.atomInputContext.UseServerApiBehavior)
                {
                    this.CurrentEntryState.FirstNavigationLinkDescriptor = null;
                }
                else
                {
                    // Read the entry content.
                    // If we find a nav. link, store it on the scope for reporting once we report the entry itself.
                    this.CurrentEntryState.FirstNavigationLinkDescriptor = this.atomEntryAndFeedDeserializer.ReadEntryContent(this.CurrentEntryState);
                }
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
                // Apply ATOM metadata to the entry
                if (entryState.AtomEntryMetadata != null)
                {
                    entry.SetAnnotation(entryState.AtomEntryMetadata);
                }

                var entityType = entryState.EntityType;

                // If the entry in the model has a default stream and if entry/m:properties and entry/content do not exist, 
                // create an empty MediaResource.
                if (entryState.MediaLinkEntry == null && entityType != null && entityType.HasStream)
                {
                    ODataAtomEntryAndFeedDeserializer.EnsureMediaResource(entryState);
                }

                // By default validate media resource
                // In WCF DS Server mode, validate media resource in ATOM (here)
                // In WCF DS Client mode, do not validate media resource.
                // Client doesn't validate default media resource against metadata.
                //   Server doesn't validate default media resource when reading JSON against model.
                bool validateMediaResource =
                    this.atomInputContext.UseDefaultFormatBehavior ||
                    this.atomInputContext.UseServerFormatBehavior;
                ValidationUtils.ValidateEntryMetadataResource(entry, entityType, this.atomInputContext.Model, validateMediaResource);
            }

            this.EndEntry(new AtomScope(ODataReaderState.EntryEnd, this.Item, this.CurrentEntityType));
        }

        /// <summary>
        /// Starts the navigation link.
        /// Does metadata validation of the navigation link and sets up the reader to report it.
        /// </summary>
        /// <param name="navigationLinkDescriptor">The navigation link descriptor for the navigation link to start.</param>
        private void StartNavigationLink(ODataAtomReaderNavigationLinkDescriptor navigationLinkDescriptor)
        {
            Debug.Assert(navigationLinkDescriptor != null, "navigationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationLinkDescriptor.NavigationLink.Name), "Found navigation link without a name, or with empty name.");
            Debug.Assert(
                navigationLinkDescriptor.NavigationProperty == null || navigationLinkDescriptor.NavigationLink.Name == navigationLinkDescriptor.NavigationProperty.Name,
                "The navigation link doesn't match the navigation property.");
            Debug.Assert(
                navigationLinkDescriptor.NavigationProperty != null || !this.atomInputContext.Model.IsUserModel() ||
                this.atomInputContext.MessageReaderSettings.ReportUndeclaredLinkProperties,
                "We don't support open navigation links yet, so either we must not have a model or the reading of undeclared nav. props needs to be allowed.");
            this.atomEntryAndFeedDeserializer.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.atomEntryAndFeedDeserializer.XmlReader.LocalName == AtomConstants.AtomLinkElementName &&
                this.atomEntryAndFeedDeserializer.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on atom:link element.");

            // Note that it is enough to just rely on the navigation property type, we don't need to traverse the association.
            //   And we don't have to because the type of the navigation property is the same as the type of the other association end.
            //   Associatiion sets and MEST are not really in the picture at this point.
            IEdmEntityType targetEntityType = null;
            if (navigationLinkDescriptor.NavigationProperty != null)
            {
                IEdmTypeReference navigationPropertyType = navigationLinkDescriptor.NavigationProperty.Type;
                if (!navigationPropertyType.IsCollection())
                {
                    if (navigationLinkDescriptor.NavigationLink.IsCollection == true)
                    {
                        throw new ODataException(ODataErrorStrings.ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty(navigationLinkDescriptor.NavigationLink.Name));
                    }

                    // Set it to false, it's either already set, or it's null.
                    navigationLinkDescriptor.NavigationLink.IsCollection = false;

                    targetEntityType = navigationPropertyType.AsEntity().EntityDefinition();
                }
                else
                {
                    // The server needs to allow entry type for collection properties. 
                    //  ODL allows entry type for collection properties only when processing entity reference links in requests.
                    //  This validation is done when reading the navigation link.
                    if (!navigationLinkDescriptor.NavigationLink.IsCollection.HasValue)
                    {
                        // Only set collection to true if it was not set already. We need the payload to win so that we can later validate
                        // that if the link is expanded, the expansion is a feed.
                        navigationLinkDescriptor.NavigationLink.IsCollection = true;
                    }

                    targetEntityType = navigationPropertyType.AsCollection().ElementType().AsEntity().EntityDefinition();
                }
            }

            this.EnterScope(ODataReaderState.NavigationLinkStart, navigationLinkDescriptor.NavigationLink, targetEntityType);

            // Store a reference to the navigation property in the scope, so that we could validate cardinality of the link when reading it.
            ((AtomScope)this.CurrentScope).NavigationProperty = navigationLinkDescriptor.NavigationProperty;
        }

        /// <summary>
        /// Moves the reader from the start state of a non-expanded navigation link.
        /// </summary>
        private void ReadAtNonExpandedNavigationLinkStart()
        {
            ODataNavigationLink currentNavigationLink = this.CurrentNavigationLink;
            IODataAtomReaderEntryState parentEntryState = (IODataAtomReaderEntryState)this.LinkParentEntityScope;

            ReaderUtils.CheckForDuplicateNavigationLinkNameAndSetAssociationLink(parentEntryState.DuplicatePropertyNamesChecker, currentNavigationLink, false, currentNavigationLink.IsCollection);

            if (this.atomInputContext.ReadingResponse)
            {
                // For deferred responses, do not allow 'entry' link type if the model says the property is a collection.
                AtomScope entryScope = (AtomScope)this.CurrentScope;
                IEdmNavigationProperty navigationProperty = entryScope.NavigationProperty;
                if (currentNavigationLink.IsCollection == false && (navigationProperty != null && navigationProperty.Type.IsCollection()))
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomReader_DeferredEntryInFeedNavigationLink);
                }

                // The link is not expanded - report a deferred link
                this.ReplaceScope(ODataReaderState.NavigationLinkEnd);
            }
            else
            {
                // The link is not expanded - report an entity reference link
                this.EnterScope(ODataReaderState.EntityReferenceLink, new ODataEntityReferenceLink { Url = currentNavigationLink.Url }, null);
            }
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
        /// Replaces the current scope with a new FeedEnd scope and the item of the current scope.
        /// </summary>
        private void ReplaceScopeToFeedEnd()
        {
            IODataAtomReaderFeedState feedState = this.CurrentFeedState;
            ODataFeed feed = this.CurrentFeed;

            // Apply ATOM metadata to the feed.
            if (this.atomInputContext.MessageReaderSettings.EnableAtomMetadataReading)
            {
                feed.SetAnnotation(feedState.AtomFeedMetadata);
            }

            // End of the feed
            this.ReplaceScope(ODataReaderState.FeedEnd);
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
            /// Bitfield to track the current state of the ATOM scope.
            /// </summary>
            private AtomScopeStateBitMask atomScopeState;

            /// <summary>
            /// The ATOM entry metadata to fill as we read the content of the entry.
            /// </summary>
            /// <remarks>
            /// This is lazily initialized only when it's actually needed.
            /// </remarks>
            private AtomEntryMetadata atomEntryMetadata;

            /// <summary>
            /// The ATOM feed metadata to fill as we read the content of the entry.
            /// </summary>
            /// <remarks>
            /// This is lazily initialized only when it's actually needed.
            /// </remarks>
            private AtomFeedMetadata atomFeedMetadata;

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
                : base(state, item, /*navigationSource*/ null, expectedEntityType, /*contextUri*/null)
            {
            }

            /// <summary>
            /// An enumeration of the various kinds of properties on an entity reference link collection.
            /// </summary>
            [Flags]
            private enum AtomScopeStateBitMask
            {
                /// <summary>No state information.</summary>
                None = 0,

                /// <summary>Empty element.</summary>
                EmptyElement = 1,

                /// <summary>A read link has been detected for this entry.</summary>
                HasReadLink = 2,

                /// <summary>An edit link has been detected for this entry.</summary>
                HasEditLink = 4,

                /// <summary>An id link has been detected for this entry.</summary>
                HasId = 8,

                /// <summary>A content element has been detected for this entry.</summary>
                HasContent = 16,

                /// <summary>A category element which has the required type name has been detected for this entry.</summary>
                HasTypeNameCategory = 32,

                /// <summary>A m:properties element has been detected for this entry.</summary>
                HasProperties = 64,

                /// <summary>A m:count link has been detected for this feed.</summary>
                HasCount = 128,

                /// <summary>A link[@rel='next'] link has been detected for this feed.</summary>
                HasNextPageLinkInFeed = 256,

                /// <summary>A link[@rel='self'] link has been detected for this feed.</summary>
                HasReadLinkInFeed = 512,

                /// <summary>An edit-media link has been detected for this entry.</summary>
                HasEditMediaLink = 1024,

                /// <summary>A link[@rel='http://docs.oasis-open.org/odata/ns/delta'] has been detected for this entry.</summary>
                HasDeltaLink = 2048,
            }

            /// <summary>
            /// Flag which indicates that the element representing the current state is empty.
            /// </summary>
            public bool ElementEmpty
            {
                get
                {
                    return (this.atomScopeState & AtomScopeStateBitMask.EmptyElement) == AtomScopeStateBitMask.EmptyElement;
                }

                set
                {
                    if (value)
                    {
                        this.atomScopeState |= AtomScopeStateBitMask.EmptyElement;
                    }
                    else
                    {
                        this.atomScopeState &= ~AtomScopeStateBitMask.EmptyElement;
                    }
                }
            }

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
                        throw new ODataException(ODataErrorStrings.ODataAtomReader_MediaLinkEntryMismatch);
                    }

                    this.mediaLinkEntry = value;
                }
            }

            /// <summary>
            /// If the reader finds a navigation link to report, but it must first report the parent entry
            /// it will store the navigation link descriptor in this property. So this will only ever store the first navigation link of an entry.
            /// </summary>
            public ODataAtomReaderNavigationLinkDescriptor FirstNavigationLinkDescriptor { get; set; }

            /// <summary>
            /// The duplicate property names checker for the entry represented by the current state.
            /// </summary>
            public DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker { get; set; }

            /// <summary>
            /// The navigation property retrieved from the metadata when reading a navigation link.
            /// </summary>
            public IEdmNavigationProperty NavigationProperty { get; set; }

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
            /// Flag which indicates whether we have found a read link for this entry (even if it had a null URI value).
            /// </summary>
            bool IODataAtomReaderEntryState.HasReadLink
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasReadLink);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasReadLink);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found an edit link for this entry (even if it had a null URI value).
            /// </summary>
            bool IODataAtomReaderEntryState.HasEditLink
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasEditLink);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasEditLink);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found an edit-media link for this entry (even if it had a null URI value).
            /// </summary>
            bool IODataAtomReaderEntryState.HasEditMediaLink
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasEditMediaLink);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasEditMediaLink);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found an id element for this entry.
            /// </summary>
            bool IODataAtomReaderEntryState.HasId
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasId);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasId);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found a content element for this entry.
            /// </summary>
            bool IODataAtomReaderEntryState.HasContent
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasContent);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasContent);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found a category element for this entry.
            /// </summary>
            bool IODataAtomReaderEntryState.HasTypeNameCategory
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasTypeNameCategory);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasTypeNameCategory);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found a m:properties element for this entry.
            /// </summary>
            bool IODataAtomReaderEntryState.HasProperties
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasProperties);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasProperties);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found a m:count elemnent for this feed.
            /// </summary>
            bool IODataAtomReaderFeedState.HasCount
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasCount);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasCount);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found a link[@rel='next'] elemnent for this feed.
            /// </summary>
            bool IODataAtomReaderFeedState.HasNextPageLink
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasNextPageLinkInFeed);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasNextPageLinkInFeed);
                }
            }

            /// <summary>
            /// Flag which indicates whether we have found a link[@rel='self'] elemnent for this feed.
            /// </summary>
            bool IODataAtomReaderFeedState.HasReadLink
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasReadLinkInFeed);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasReadLinkInFeed);
                }
            }

            /// <summary>
            /// Flag which indicates if a link[@rel='http://docs.oasis-open.org/odata/ns/delta'] element was found.
            /// </summary>
            bool IODataAtomReaderFeedState.HasDeltaLink
            {
                get
                {
                    return this.GetAtomScopeState(AtomScopeStateBitMask.HasDeltaLink);
                }

                set
                {
                    this.SetAtomScopeState(value, AtomScopeStateBitMask.HasDeltaLink);
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
            /// The feed metadata to fill as we read the content of a feed.
            /// </summary>
            AtomFeedMetadata IODataAtomReaderFeedState.AtomFeedMetadata
            {
                get
                {
                    // Note that we lazy init the property when asked for it through the interface
                    // which is what all the deserializers use.
                    if (this.atomFeedMetadata == null)
                    {
                        this.atomFeedMetadata = AtomMetadataReaderUtils.CreateNewAtomFeedMetadata();
                    }

                    return this.atomFeedMetadata;
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

            /// <summary>
            /// Sets the bit identified by the <paramref name="bitMask"/> if <paramref name="value"/> is true, otherwise clears it.
            /// </summary>
            /// <param name="value">Indicates if the <paramref name="bitMask"/> should be set</param>
            /// <param name="bitMask">Identifies the bit to set in atomScopeState</param>
            private void SetAtomScopeState(bool value, AtomScopeStateBitMask bitMask)
            {
                if (value)
                {
                    this.atomScopeState |= bitMask;
                }
                else
                {
                    this.atomScopeState &= ~bitMask;
                }
            }

            /// <summary>
            /// Returns true if the bit identified by <paramref name="bitMask"/> is set, false otherwise.
            /// </summary>
            /// <param name="bitMask">Identifies the bit to set in atomScopeState</param>
            /// <returns>True if the bit identified by the <paramref name="bitMask"/> is set, false otherwise</returns>
            private bool GetAtomScopeState(AtomScopeStateBitMask bitMask)
            {
                return (this.atomScopeState & bitMask) == bitMask;
            }
        }
    }
}
