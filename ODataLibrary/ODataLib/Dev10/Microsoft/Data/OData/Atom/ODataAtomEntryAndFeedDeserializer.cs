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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.Data.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for entries and feeds.
    /// </summary>
    internal sealed class ODataAtomEntryAndFeedDeserializer : ODataAtomPropertyAndValueDeserializer
    {
        #region Atomized strings
        /// <summary>The empty namespace used for attributes in no namespace.</summary>
        private readonly string EmptyNamespace;

        /// <summary>Schema namespace for Atom.</summary>
        private readonly string AtomNamespace;

        /// <summary>XML element name to mark entry element in Atom.</summary>
        private readonly string AtomEntryElementName;

        /// <summary>'category' - XML element name for ATOM 'category' element for entries.</summary>
        private readonly string AtomCategoryElementName;

        /// <summary>'term' - XML attribute name for ATOM 'term' attribute for categories.</summary>
        private readonly string AtomCategoryTermAttributeName;

        /// <summary>'scheme' - XML attribute name for ATOM 'scheme' attribute for categories.</summary>
        private readonly string AtomCategorySchemeAttributeName;

        /// <summary>XML element name to mark content element in Atom.</summary>
        private readonly string AtomContentElementName;

        /// <summary>XML element name to mark link element in Atom.</summary>
        private readonly string AtomLinkElementName;

        /// <summary>Element containing property values when 'content' is used for media link entries</summary>
        private readonly string AtomPropertiesElementName;

        /// <summary>XML element name to mark feed element in Atom.</summary>
        private readonly string AtomFeedElementName;

        /// <summary>XML element name to mark id element in Atom.</summary>
        private readonly string AtomIdElementName;

        /// <summary>XML attribute name of the link relation attribute in Atom.</summary>
        private readonly string AtomLinkRelationAttributeName;

        /// <summary>XML attribute name of the href attribute of a link in Atom.</summary>
        private readonly string AtomLinkHrefAttributeName;

        /// <summary>Atom source attribute name for the content of media link entries.</summary>
        private readonly string MediaLinkEntryContentSourceAttributeName;

        /// <summary>OData attribute which indicates the etag value for the declaring entry element.</summary>
        private readonly string ODataETagAttributeName;

        /// <summary>OData element name for the 'count' element</summary>
        private readonly string ODataCountElementName;

        /// <summary>'Inline' - wrapping element for inlined entry/feed content.</summary>
        private readonly string ODataInlineElementName;

        #endregion

        /// <summary>
        /// ATOM deserializer for ATOM metadata on entries.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomEntryMetadataDeserializer entryMetadataDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomEntryAndFeedDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            DebugUtils.CheckNoExternalCallers();

            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.EmptyNamespace = nameTable.Add(string.Empty);
            this.AtomNamespace = nameTable.Add(AtomConstants.AtomNamespace);
            this.AtomEntryElementName = nameTable.Add(AtomConstants.AtomEntryElementName);
            this.AtomCategoryElementName = nameTable.Add(AtomConstants.AtomCategoryElementName);
            this.AtomCategoryTermAttributeName = nameTable.Add(AtomConstants.AtomCategoryTermAttributeName);
            this.AtomCategorySchemeAttributeName = nameTable.Add(AtomConstants.AtomCategorySchemeAttributeName);
            this.AtomContentElementName = nameTable.Add(AtomConstants.AtomContentElementName);
            this.AtomLinkElementName = nameTable.Add(AtomConstants.AtomLinkElementName);
            this.AtomPropertiesElementName = nameTable.Add(AtomConstants.AtomPropertiesElementName);
            this.AtomFeedElementName = nameTable.Add(AtomConstants.AtomFeedElementName);
            this.AtomIdElementName = nameTable.Add(AtomConstants.AtomIdElementName);
            this.AtomLinkRelationAttributeName = nameTable.Add(AtomConstants.AtomLinkRelationAttributeName);
            this.AtomLinkHrefAttributeName = nameTable.Add(AtomConstants.AtomLinkHrefAttributeName);
            this.MediaLinkEntryContentSourceAttributeName = nameTable.Add(AtomConstants.MediaLinkEntryContentSourceAttributeName);
            this.ODataETagAttributeName = nameTable.Add(AtomConstants.ODataETagAttributeName);
            this.ODataCountElementName = nameTable.Add(AtomConstants.ODataCountElementName);
            this.ODataInlineElementName = nameTable.Add(AtomConstants.ODataInlineElementName);
    }

        /// <summary>
        /// ATOM deserializer for ATOM metadata on entries.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomEntryMetadataDeserializer EntryMetadataDeserializer
        {
            get 
            {
                return this.entryMetadataDeserializer ??
                       (this.entryMetadataDeserializer = new ODataAtomEntryMetadataDeserializer(this.AtomInputContext));
            }
        }

        /// <summary>
        /// Read the start of the entry.
        /// This method verifies that the current element is atom:entry and it reads the ETag from it.
        /// </summary>
        /// <param name="entry">The entry instance to fill the properties on.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element   - The method will fail if it's not called atom:entry (but won't validate that it's an element)
        /// Post-Condition: XmlNodeType.Element   - The atom:entry element
        /// </remarks>
        internal void ReadEntryStart(ODataEntry entry)
        {
            DebugUtils.CheckNoExternalCallers();
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);

            if (!this.XmlReader.NamespaceEquals(this.AtomNamespace) || !this.XmlReader.LocalNameEquals(this.AtomEntryElementName))
            {
                throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_EntryElementWrongName(this.XmlReader.LocalName, this.XmlReader.NamespaceURI));
            }

            // Read the etag attribute from the entry element
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace) && this.XmlReader.LocalNameEquals(this.ODataETagAttributeName))
                {
                    entry.ETag = this.XmlReader.Value;
                    break;
                }
            }

            this.XmlReader.MoveToElement();

            this.AssertXmlCondition(XmlNodeType.Element);
            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the content of an entry (child nodes of the atom:entry, not the atom:content element).
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <returns>A <see cref="ODataNavigationLink"/> instance representing the navigation link detected;
        /// null if no navigation link was found and the end of the entry was reached.</returns>
        /// <remarks>
        /// Pre-Condition:  Anything but Attribute - the child node of the atom:entry element, can be pretty much anything, the method will skip over insignificant nodes and text nodes if found.
        /// Post-Condition: XmlNodeType.EndElement atom:entry - The end of the atom:entry element if no nav. link was found and the end of the entry was reached.
        ///                 XmlNodeType.Element atom:link     - The start tag of the atom:link element representing a navigation link.
        /// </remarks>
        internal ODataNavigationLink ReadEntryContent(IODataAtomReaderEntryState entryState)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            this.XmlReader.AssertNotBuffering();
            Debug.Assert(this.XmlReader.NodeType != XmlNodeType.Attribute, "The reader must be positioned on a child node of the atom:entry element.");

            ODataNavigationLink navigationLink = null;

            while (this.XmlReader.NodeType != XmlNodeType.EndElement)
            {
                if (this.XmlReader.NodeType != XmlNodeType.Element)
                {
                    Debug.Assert(this.XmlReader.NodeType != XmlNodeType.EndElement, "EndElement should have been handled already.");

                    // Skip everything but elements, including insignificant nodes, text nodes and CDATA nodes
                    this.XmlReader.Skip();
                    continue;
                }

                if (this.XmlReader.NamespaceEquals(this.AtomNamespace))
                {
                    navigationLink = this.ReadAtomElementInEntry(entryState);
                    if (navigationLink != null)
                    {
                        entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNavigationLinkStart(navigationLink);
                        break;
                    }
                }
                else if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                {
                    if (this.XmlReader.LocalNameEquals(this.AtomPropertiesElementName))
                    {
                        // m:properties outside of content -> MLE
                        EnsureMediaResource(entryState);
                        this.ReadProperties(entryState.EntityType, ReaderUtils.GetPropertiesList(entryState.Entry.Properties), entryState.DuplicatePropertyNamesChecker);

                        // Read over the end element or the empty start element.
                        this.XmlReader.Read();
                    }
                }
                else
                {
                    // non-ATOM elements
                    // Read it for EPM and skip the rest.
                    if (entryState.CachedEpm != null)
                    {
                        this.EntryMetadataDeserializer.ReadExtensionElementInEntryContent(entryState);
                    }
                    else
                    {
                        this.XmlReader.Skip();
                    }
                }
            }

            Debug.Assert(
                this.XmlReader.NodeType != XmlNodeType.EndElement || 
                    (this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomEntryElementName),
                "EndElement found but for element other than atom:entry.");
            Debug.Assert(
                this.XmlReader.NodeType != XmlNodeType.Element ||
                    (this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName),
                "Only atom:link elements can be reported as navigation links.");

            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.EndElement);
            this.XmlReader.AssertNotBuffering();
            return navigationLink;
        }

        /// <summary>
        /// Read the end of the entry.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (empty) atom:entry - The atom:entry empty element to read end of.
        ///                 XmlNodeType.EndElement atom:entry      - The end element of atom:entry to read.
        /// Post-Condition: Any                                    - The node right after the entry element.
        /// </remarks>
        internal void ReadEntryEnd()
        {
            DebugUtils.CheckNoExternalCallers();
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomEntryElementName,
                "This method should only be called on atom:entry start (empty) tag or end tag.");

            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Read the start of the feed.
        /// This method verifies that the current element is atom:feed.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element   - The method will fail if it's not called atom:feed (but won't validate that it's an element)
        /// Post-Condition: XmlNodeType.Element   - The atom:feed element
        /// </remarks>
        internal void ReadFeedStart()
        {
            DebugUtils.CheckNoExternalCallers();
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);

            if (!this.XmlReader.NamespaceEquals(this.AtomNamespace) || !this.XmlReader.LocalNameEquals(this.AtomFeedElementName))
            {
                throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_FeedElementWrongName(this.XmlReader.LocalName, this.XmlReader.NamespaceURI));
            }

            this.AssertXmlCondition(XmlNodeType.Element);
            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the content of a feed (child nodes of the atom:feed).
        /// </summary>
        /// <param name="feedState">The reader feed state for the feed being read.</param>
        /// <returns>true if an entry was found or false if no more entries were found in the feed.</returns>
        /// <remarks>
        /// Pre-Condition:  Anything but Attribute           - the child node of the atom:feed element, can be pretty much anything, the method will skip over insignificant nodes and text nodes if found.
        /// Post-Condition: XmlNodeType.EndElement atom:feed - The end of the atom:feed element if no entry was found and the end of the feed was reached.
        ///                 XmlNodeType.Element atom:entry   - The start tag of the atom:entry element representing an entry in the feed.
        /// </remarks>
        internal bool ReadFeedContent(IODataAtomReaderFeedState feedState)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(feedState != null, "feedState != null");
            this.XmlReader.AssertNotBuffering();
            Debug.Assert(this.XmlReader.NodeType != XmlNodeType.Attribute, "The reader must be positioned on a child node of the atom:feed element.");

            bool entryFound = false;
            while (this.XmlReader.NodeType != XmlNodeType.EndElement)
            {
                if (this.XmlReader.NodeType != XmlNodeType.Element)
                {
                    Debug.Assert(this.XmlReader.NodeType != XmlNodeType.EndElement, "EndElement should have been handled already.");

                    // Skip everything but elements, including insignificant nodes, text nodes and CDATA nodes
                    this.XmlReader.Skip();
                    continue;
                }

                if (this.XmlReader.NamespaceEquals(this.AtomNamespace))
                {
                    if (this.ReadAtomElementInFeed(feedState))
                    {
                        // We've found an entry - return.
                        entryFound = true;
                        break;
                    }
                }
                else if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                {
                    if (this.XmlReader.LocalNameEquals(this.ODataCountElementName))
                    {
                        // m:count
                        // TODO: Fail if we find more than one m:count element.
                        // TODO: What to do in inner feeds - do we fail? Note that Astoria server completely ignores m:count in inline feeds and thus us trying to parse it
                        // may cause failures which didn't occure before.
                        long countValue = (long)AtomValueUtils.ReadPrimitiveValue(this.XmlReader, EdmCoreModel.Instance.GetInt64(true));
                        feedState.Feed.Count = countValue;

                        // Read over the end element or the empty start element.
                        this.XmlReader.Read();
                    }
                    else
                    {
                        // Any other element in the m namespace is to be ignored.
                        this.XmlReader.Skip();
                    }
                }
                else
                {
                    // non-ATOM elements, ignore them
                    this.XmlReader.Skip();
                }
            }

            Debug.Assert(
                this.XmlReader.NodeType != XmlNodeType.EndElement ||
                    (this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomFeedElementName),
                "EndElement found but for element other than atom:feed.");
            Debug.Assert(
                this.XmlReader.NodeType != XmlNodeType.Element ||
                    (this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomEntryElementName),
                "Only atom:entry elements can be reported as entries.");

            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.EndElement);
            this.XmlReader.AssertNotBuffering();
            return entryFound;
        }

        /// <summary>
        /// Read the end of the feed.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (empty) atom:feed - The atom:feed empty element to read end of.
        ///                 XmlNodeType.EndElement atom:feed      - The end element of atom:feed to read.
        /// Post-Condition: Any                                   - The node right after the entry element.
        /// </remarks>
        internal void ReadFeedEnd()
        {
            DebugUtils.CheckNoExternalCallers();
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomFeedElementName,
                "This method should only be called on atom:feed start (empty) tag or end tag.");

            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Read the content of the navigation link before any expansion was found.
        /// </summary>
        /// <returns>
        /// An enumeration value indicating what content was found:
        /// None - no m:inline was found, the link is deferred. The reader is positioned on the atom:link end element.
        /// Empty - empty m:inline was found - usually means null entry. The reader is positioned on empty start m:inline or end element m:inline.
        /// Entry - expanded entry was found. The reader is positioned on the atom:entry element.
        /// Feed - expanded feed was found. The reader is positioned on the atom:feed element.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  Any                                  - a node in the atom:link content.
        /// Post-Condition: XmlNodeType.EndElement atom:link     - the end of the navigation link was reached.
        ///                 XmlNodeType.Element atom:feed        - an expanded feed was found.
        ///                 XmlNodeType.Element atom:entry       - an expanded entry was found.
        ///                 XmlNodeType.Element (empty) m:inline - empty inline was found.
        ///                 XmlNodeType.EndElement m:inline      - empty inline was found.
        /// </remarks>
        internal ODataAtomDeserializerExpandedNavigationLinkContent ReadNavigationLinkContentBeforeExpansion()
        {
            DebugUtils.CheckNoExternalCallers();
            this.XmlReader.AssertNotBuffering();

            if (this.ReadNavigationLinkContent())
            {
                // m:inline found.
                this.AssertXmlCondition(XmlNodeType.Element);
                Debug.Assert(
                    this.XmlReader.LocalName == AtomConstants.ODataInlineElementName &&
                    this.XmlReader.NamespaceURI == AtomConstants.ODataMetadataNamespace,
                    "The reader must be on the m:inline element.");

                if (this.XmlReader.IsEmptyElement)
                {
                    return ODataAtomDeserializerExpandedNavigationLinkContent.Empty;
                }

                // Move to the first node inside the m:inline
                this.XmlReader.Read();

                return this.ReadInlineElementContent();
            }

            // End of atom:link reached but we didn't find any interesting payload
            // report it as deferred link.
            this.AssertXmlCondition(XmlNodeType.EndElement);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomLinkElementName &&
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on the atom:link end element.");

            return ODataAtomDeserializerExpandedNavigationLinkContent.None;
        }

        /// <summary>
        /// Read the content of the navigation link after the expansion was found.
        /// The method returns when the entire atom:link was read.
        /// </summary>
        /// <param name="emptyInline">
        /// true if the reader is positioned on the empty start tag or end tag of the m:inline element.
        /// false if the reader is inside m:inline (or on the end tag of m:inline).
        /// </param>
        /// <remarks>
        /// Pre-Condition:  Any                                  - child node of the m:inline element (emptyInline == false)
        ///                 XmlNodeType.Element (empty) m:inline - empty m:inline element (emptyInline = true)
        ///                 XmlNodeType.EndElement m:inline      - end of the m:inline element.
        /// Post-Condition: XmlNodeType.EndElement atom:link     - the end element of the navigation link.
        /// </remarks>
        internal void ReadNavigationLinkContentAfterExpansion(bool emptyInline)
        {
            DebugUtils.CheckNoExternalCallers();
            this.XmlReader.AssertNotBuffering();

            if (!emptyInline)
            {
                // Read till the end of the m:inline
                ODataAtomDeserializerExpandedNavigationLinkContent expandedNavigationLinkContent = this.ReadInlineElementContent();
                if (expandedNavigationLinkContent != ODataAtomDeserializerExpandedNavigationLinkContent.Empty)
                {
                    Debug.Assert(
                        expandedNavigationLinkContent == ODataAtomDeserializerExpandedNavigationLinkContent.Entry ||
                        expandedNavigationLinkContent == ODataAtomDeserializerExpandedNavigationLinkContent.Feed,
                        "Only entry or feed could be found inside m:inline");

                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline(expandedNavigationLinkContent.ToString()));
                }
            }

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.ODataInlineElementName &&
                this.XmlReader.NamespaceURI == AtomConstants.ODataMetadataNamespace,
                "The reader must be on the m:inline empty start tag or end element.");

            // Read the end of the m:inline (or the empty start element).
            this.XmlReader.Read();

            if (this.ReadNavigationLinkContent())
            {
                // Found a second m:inline.
                throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_MultipleInlineElementsInLink);
            }

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomLinkElementName &&
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on the atom:link end element.");
        }

        /// <summary>
        /// Read the end of the navigation link.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (empty) atom:link  - the empty atom:link element of a deferred navigation link.
        ///                 XmlNodeType.EndElement atom:link       - the end element atom:link.
        /// Post-Condition: Any                                    - The node right after the link element.
        /// </remarks>
        internal void ReadNavigationLinkEnd()
        {
            DebugUtils.CheckNoExternalCallers();
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "This method should only be called on atom:link start (empty) tag or end tag.");

            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the entry content in buffering mode and looks for the type name (the category element with the right attributes).
        /// </summary>
        /// <returns>The type name if one of found or null if none was found.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element   - the atom:entry element
        /// Post-Condition: XmlNodeType.Element   - the atom:entry element on which the reader started.
        /// </remarks>
        internal string FindTypeName()
        {
            DebugUtils.CheckNoExternalCallers();
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);

            this.XmlReader.MoveToElement();
            Debug.Assert(
                this.XmlReader.NamespaceEquals(this.AtomNamespace) && this.XmlReader.LocalNameEquals(this.AtomEntryElementName),
                "This method must start on the atom:entry element.");

            this.XmlReader.StartBuffering();
            try
            {
                if (!this.XmlReader.IsEmptyElement)
                {
                    // Move to the first child node of the atom:entry element
                    this.XmlReader.Read();

                    while (true)
                    {
                        switch (this.XmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (this.XmlReader.NamespaceEquals(this.AtomNamespace) && this.XmlReader.LocalNameEquals(this.AtomCategoryElementName))
                                {
                                    string typeName = null;
                                    bool foundODataScheme = false;
                                    while (this.XmlReader.MoveToNextAttribute())
                                    {
                                        if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                                        {
                                            if (this.XmlReader.LocalNameEquals(this.AtomCategorySchemeAttributeName))
                                            {
                                                if (string.CompareOrdinal(this.XmlReader.Value, AtomConstants.ODataSchemeNamespace) != 0)
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    foundODataScheme = true;
                                                }
                                            }
                                            else if (this.XmlReader.LocalNameEquals(this.AtomCategoryTermAttributeName))
                                            {
                                                typeName = this.XmlReader.Value;
                                            }
                                        }
                                    }

                                    // Note that we use the first typename we find, this is consistent with the behavior of both WCF DS Client and Server.
                                    if (foundODataScheme && typeName != null)
                                    {
                                        return typeName;
                                    }
                                }

                                // Skip the element (note that it doesn't matter if we're on an attribute, Skip behaves like Read in this regard)
                                this.XmlReader.Skip();
                                break;

                            case XmlNodeType.EndElement:
                                // End of the entry element - we didn't find any type
                                return null;

                            default:
                                // Any other node - skip it
                                this.XmlReader.Skip();
                                break;
                        }
                    }
                }
            }
            finally
            {
                this.XmlReader.StopBuffering();
            }

            return null;
        }

        /// <summary>
        /// Returns an existing stream property value if it already exists in the list of OData properties otherwise creates a new 
        /// ODataProperty for the stream property and returns the value of that property.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="streamPropertyName">The name of the stream property to return.</param>
        /// <returns>A new or an existing stream property value.</returns>
        private static ODataStreamReferenceValue GetNewOrExistingStreamPropertyValue(IODataAtomReaderEntryState entryState, string streamPropertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(streamPropertyName != null, "streamPropertyName != null");

            List<ODataProperty> properties = ReaderUtils.GetPropertiesList(entryState.Entry.Properties);

            // Property names are case sensitive, so compare in a case sensitive way.
            ODataProperty streamProperty = properties.FirstOrDefault(p => String.CompareOrdinal(p.Name, streamPropertyName) == 0);

            ODataStreamReferenceValue streamReferenceValue;
            if (streamProperty == null)
            {
                streamReferenceValue = new ODataStreamReferenceValue();
                streamProperty = new ODataProperty
                {
                    Name = streamPropertyName,
                    Value = streamReferenceValue
                };

                ReaderValidationUtils.ValidateStreamReferenceProperty(streamProperty, entryState.EntityType);
                entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(streamProperty);
                properties.Add(streamProperty);
            }
            else
            {
                streamReferenceValue = streamProperty.Value as ODataStreamReferenceValue;
                if (streamReferenceValue == null)
                {
                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_StreamPropertyDuplicatePropertyName(streamPropertyName));
                }
            }

            return streamReferenceValue;
        }

        /// <summary>
        /// Ensure a media resource is created for the specified entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        private static void EnsureMediaResource(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");

            entryState.MediaLinkEntry = true;
            ODataEntry entry = entryState.Entry;
            if (entry.MediaResource == null)
            {
                entry.MediaResource = new ODataStreamReferenceValue();
            }
        }

        /// <summary>
        /// Reads an ATOM element inside the atom:entry from the input.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <returns>
        /// If the atom element is representing a navigation link an instance of <see cref="ODataNavigationLink"/> is returned,
        /// otherwise null.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element in ATOM namespace - The element in ATOM namespace to read.
        /// Post-Condition: Any                                   - The node after the ATOM element if it's not a navigation link.
        ///                 XmlNodeType.Element atom:link         - The start tag of atom:link if it's a navigation link.
        /// </remarks>
        private ODataNavigationLink ReadAtomElementInEntry(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on an element in the ATOM namespace for this method to work.");

            // ATOM elements
            if (this.XmlReader.LocalNameEquals(this.AtomContentElementName))
            {
                // atom:content
                this.ReadAtomContentElement(entryState);
            }
            else if (this.XmlReader.LocalNameEquals(this.AtomIdElementName))
            {
                // atom:id
                this.ReadAtomIdElementInEntry(entryState);
            }
            else if (this.XmlReader.LocalNameEquals(this.AtomCategoryElementName))
            {
                // atom:category
                // If we don't have syndication EPM for category and we're not to store ATOM metadata, we can safely skip all category elements
                // since we've already read the typename and no other category element holds anything of interest.
                // That's true even if there are multiple category elements of interest, for typename we already took the first.
                if (entryState.CachedEpm != null)
                {
                    this.EntryMetadataDeserializer.ReadAtomCategoryElementInEntryContent(entryState);
                }
                else
                {
                    this.XmlReader.Skip();
                }
            }
            else if (this.XmlReader.LocalNameEquals(this.AtomLinkElementName))
            {
                // atom:link
                return this.ReadAtomLinkElementInEntry(entryState);
            }
            else
            {
                if (entryState.CachedEpm != null)
                {
                    this.EntryMetadataDeserializer.ReadAtomElementInEntryContent(entryState);
                }
                else
                {
                    // Skip the element since we don't need it.
                    this.XmlReader.Skip();
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the atom:content element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:content  - The atom:content element to read.
        /// Post-Condition:  Any                               - The node after the atom:content element.
        /// </remarks>
        private void ReadAtomContentElement(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomContentElementName,
                "The XML reader must be on the atom:content element for this method to work.");

            // atom:content
            // Read the attributes - we're interested in type and src
            string contentType = null;
            string contentSource = null;
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
                    if (this.XmlReader.LocalNameEquals(this.AtomTypeAttributeName))
                    {
                        // type attribute
                        contentType = this.XmlReader.Value;
                    }
                    else if (this.XmlReader.LocalNameEquals(this.MediaLinkEntryContentSourceAttributeName))
                    {
                        // src attribute
                        contentSource = this.XmlReader.Value;
                    }
                }
            }

            if (contentSource != null)
            {
                // atom:content/@src means this is an MLE
                ODataEntry entry = entryState.Entry;
                EnsureMediaResource(entryState);
                entry.MediaResource.ReadLink = this.ProcessUriFromPayload(contentSource, this.XmlReader.XmlBaseUri);
                entry.MediaResource.ContentType = contentType;

                // Verify that the atom:content element is empty, since for MLEs there must be no content in-line.
                if (!this.XmlReader.TryReadEmptyElement())
                {
                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty);
                }
            }
            else
            {
                if (contentType == null || !HttpUtils.CompareMediaTypeNames(contentType, MimeConstants.MimeApplicationXml))
                {
                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_ContentWithWrongType(contentType ?? string.Empty));
                }

                // Normal content with properties - not an MLE
                entryState.MediaLinkEntry = false;

                this.XmlReader.MoveToElement();
                if (!this.XmlReader.IsEmptyElement)
                {
                    // TODO: How much to ignore here inside atom:content before m:properties (and after)?
                    // Astoria client is very strict and doesn't allow anything but whitespace before m:properties.
                    // Astoria server is very lax and if there's anything before m:properties other than whitespace it will ignore the entire content of atom:content.
                    // After m:properties the Astoria server completely ignores the content.
                    // After m:properties the Astoria client will ignore anything - text/elements/...
                    this.XmlReader.ReadStartElement();
                    this.XmlReader.SkipInsignificantNodes();

                    // If the element has no significant content, treat it as empty.
                    if (this.XmlReader.NodeType != XmlNodeType.EndElement)
                    {
                        // Test for m:properties element
                        if (this.XmlReader.NodeType != XmlNodeType.Element ||
                            !this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace) ||
                            !this.XmlReader.LocalNameEquals(this.AtomPropertiesElementName))
                        {
                            throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_ContentWithInvalidNode(this.XmlReader.NodeType.ToString()));
                        }

                        this.ReadProperties(entryState.EntityType, ReaderUtils.GetPropertiesList(entryState.Entry.Properties), entryState.DuplicatePropertyNamesChecker);

                        // Read over the end element or the empty start element.
                        this.XmlReader.Read();

                        // Verify that there's no other meaningful node after the m:properties
                        this.XmlReader.SkipInsignificantNodes();
                        if (this.XmlReader.NodeType != XmlNodeType.EndElement)
                        {
                            throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_ContentWithInvalidNode(this.XmlReader.NodeType.ToString()));
                        }
                    }
                }
            }

            // Read over the end element, or empty start element.
            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the atom:id element in the atom:entry element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:id - The atom:id element to read.
        /// Post-Condition:  Any                         - The node after the atom:id element.
        /// </remarks>
        private void ReadAtomIdElementInEntry(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomIdElementName,
                "The XML reader must be on the atom:id element for this method to work.");

            string idValue = this.XmlReader.ReadElementValue();

            entryState.Entry.Id = idValue != null && idValue.Length == 0 ? null : idValue;
        }

        /// <summary>
        /// Reads the atom:link element in atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <returns>
        /// If the link is a navigation link the method returns a new instance of <see cref="ODataNavigationLink"/> representing that link,
        /// otherwise the method returns null.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if it's not a navigation link.
        ///                  XmlNodeType.Element atom:link  - The atom:link start tag if it's a navigation link.
        /// </remarks>
        private ODataNavigationLink ReadAtomLinkElementInEntry(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            // Read all the attributes which we will need for all the links (rel and href)
            // NOTE: this method does not move the reader; thus a potential xml:base definition on the element
            //       works correctly for the values read from the attributes until we move the reader.
            string linkRelation, linkHRef;
            this.ReadAtomLinkRelationAndHRef(out linkRelation, out linkHRef);

            // Standard relations like "edit" can be either "edit" or "IANANamespace/edit". The GetNameFromAtomLinkRelationAttribute
            // method is rather expensice (as it has to unescape the value, create a URI and then compare the prefix for the IANA namespace)
            // so we first compare the simple "edit" case, and only if that fails we try the "IANANamespace/edit" for all the standard
            // relations.
            if (linkRelation != null)
            {
                if (this.TryReadAtomStandardRelationLinkInEntry(entryState, linkRelation, linkHRef))
                {
                    return null;
                }

                string ianaRelation = AtomUtils.GetNameFromAtomLinkRelationAttribute(linkRelation, AtomConstants.IanaLinkRelationsNamespace);
                if (ianaRelation != null && this.TryReadAtomStandardRelationLinkInEntry(entryState, ianaRelation, linkHRef))
                {
                    return null;
                }

                ODataNavigationLink navigationLink = this.TryReadNavigationLinkInEntry(linkRelation, linkHRef);
                if (navigationLink != null)
                {
                    return navigationLink;
                }

                if (this.TryReadStreamPropertyLinkInEntry(entryState, linkRelation, linkHRef))
                {
                    return null;
                }

                if (this.TryReadAssociationLinkInEntry(entryState, linkRelation, linkHRef))
                {
                    return null;
                }
            }

            // TODO: Handle ATOM metadata for link elements
            if (entryState.CachedEpm != null)
            {
                this.EntryMetadataDeserializer.ReadAtomLinkElementInEntryContent(entryState, linkRelation, linkHRef);
            }
            else
            {
                // Skip the element since we don't need it.
                this.XmlReader.Skip();
            }

            return null;
        }

        /// <summary>
        /// Reads the atom:link element with one of the standard relation values in the atom:entry element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="linkRelation">The rel attribute value for the link.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <returns>If the rel was one of the recognized standard relations and this method read the link
        /// the return value is true. Otherwise the method doesn't move the reader and returns false.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if the link was read by this method.
        ///                  XmlNodeType.Element atom:link  - The atom:link element to read if the link was not read by this method.
        /// </remarks>
        private bool TryReadAtomStandardRelationLinkInEntry(IODataAtomReaderEntryState entryState, string linkRelation, string linkHRef)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(linkRelation != null, "linkRelation != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomEditRelationAttributeValue) == 0)
            {
                // edit link
                // TODO: only process the first edit link we find as the edit link
                // TODO: if ATOM metadata reading is on, read the link into EditLink on AtomEntryMetadata
                if (linkHRef != null)
                {
                    entryState.Entry.EditLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                this.XmlReader.Skip();
                return true;
            }

            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomSelfRelationAttributeValue) == 0)
            {
                // self link
                // TODO: only process the first self link we find as the self link
                // TODO: if ATOM metadata reading is on, read the link into SelfLink on AtomEntryMetadata
                if (linkHRef != null)
                {
                    entryState.Entry.ReadLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                this.XmlReader.Skip();
                return true;
            }

            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomEditMediaRelationAttributeValue) == 0)
            {
                // edit-media link
                // TODO: only process the first edit-media link we find as the edit-media link
                // TODO: if ATOM metadata reading is on, read the link into AtomStreamReferenceMetadata.EditLink on the entry.MediaResource.
                EnsureMediaResource(entryState);
                ODataEntry entry = entryState.Entry;
                if (linkHRef != null)
                {
                    entry.MediaResource.EditLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                string mediaETagValue = this.XmlReader.GetAttributeValue(this.ODataETagAttributeName, this.XmlReader.ODataMetadataNamespace);
                if (mediaETagValue != null)
                {
                    entry.MediaResource.ETag = mediaETagValue;
                }

                this.XmlReader.Skip();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads a navigation link in entry element.
        /// </summary>
        /// <param name="linkRelation">The value of the rel attribute of the link to read.</param>
        /// <param name="linkHRef">The value of the href attribute of the link to read.</param>
        /// <returns>An instance of <see cref="ODataNavigationLink"/> if a navigation link was found; null otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element atom:link - the start tag of the atom:link element to read.
        /// Post-Condition: XmlNodeType.Element atom:link - the start tag of the atom:link element - the reader doesn't move
        /// </remarks>
        private ODataNavigationLink TryReadNavigationLinkInEntry(string linkRelation, string linkHRef)
        {
            Debug.Assert(linkRelation != null, "linkRelation != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            string navigationLinkName = AtomUtils.GetNameFromAtomLinkRelationAttribute(linkRelation, AtomConstants.ODataNavigationPropertiesRelatedLinkRelationPrefix);
            if (string.IsNullOrEmpty(navigationLinkName))
            {
                return null;
            }

            // Navigation link
            ODataNavigationLink navigationLink = new ODataNavigationLink { Name = navigationLinkName };

            // Get the type of the link
            string navigationLinkType = this.XmlReader.GetAttributeValue(this.AtomTypeAttributeName, this.EmptyNamespace);

            if (!string.IsNullOrEmpty(navigationLinkType))
            {
                string mediaTypeName;
                Encoding encoding;
                IList<KeyValuePair<string, string>> contentTypeParameters = HttpUtils.ReadContentType(navigationLinkType, out mediaTypeName, out encoding);
                if (!HttpUtils.CompareMediaTypeNames(mediaTypeName, MimeConstants.MimeApplicationAtomXml))
                {
                    return null;
                }

                string typeParameterValue = null;
                if (contentTypeParameters != null)
                {
                    for (int contentTypeParameterIndex = 0; contentTypeParameterIndex < contentTypeParameters.Count; contentTypeParameterIndex++)
                    {
                        KeyValuePair<string, string> contentTypeParameter = contentTypeParameters[contentTypeParameterIndex];
                        if (string.Compare(contentTypeParameter.Key, MimeConstants.MimeTypeParameterName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            typeParameterValue = contentTypeParameter.Value;
                            break;
                        }
                    }
                }

                if (typeParameterValue != null)
                {
                    if (string.Compare(typeParameterValue, MimeConstants.MimeTypeParameterValueEntry, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        navigationLink.IsCollection = false;
                    }
                    else if (string.Compare(typeParameterValue, MimeConstants.MimeTypeParameterValueFeed, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        navigationLink.IsCollection = true;
                    }
                }
            }

            if (linkHRef != null)
            {
                navigationLink.Url = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
            }

            this.XmlReader.MoveToElement();
            return navigationLink;
        }

        /// <summary>
        /// Reads a stream property link in an atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="linkRelation">The rel attribute value for the link.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <returns>true, if the named stream is read succesfully, false otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if the link was read by this method.
        ///                  XmlNodeType.Element atom:link  - The atom:link element to read if the link was not read by this method.
        /// </remarks>
        private bool TryReadStreamPropertyLinkInEntry(IODataAtomReaderEntryState entryState, string linkRelation, string linkHRef)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(linkRelation != null, "linkRelation != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            // check if this is an edit link
            string streamPropertyName = AtomUtils.GetNameFromAtomLinkRelationAttribute(linkRelation, AtomConstants.ODataStreamPropertyEditMediaRelatedLinkRelationPrefix);
            if (streamPropertyName != null)
            {
                if (streamPropertyName.Length == 0)
                {
                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithEmptyName);
                }

                this.ReadStreamPropertyEditOrReadLinkInEntry(entryState, streamPropertyName, linkHRef, true /*editLink*/);
                return true;
            }

            // check if this is a read link.
            streamPropertyName = AtomUtils.GetNameFromAtomLinkRelationAttribute(linkRelation, AtomConstants.ODataStreamPropertyMediaResourceRelatedLinkRelationPrefix);
            if (streamPropertyName != null)
            {
                if (streamPropertyName.Length == 0)
                {
                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithEmptyName);
                }

                this.ReadStreamPropertyEditOrReadLinkInEntry(entryState, streamPropertyName, linkHRef, false /*editLink*/);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads a read or edit link for a stream property in an atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="streamPropertyName">The name of the stream property.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <param name="editLink">true if link is an edit-link; false if it's a read link.</param>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if the link was read by this method.
        ///                  XmlNodeType.Element atom:link  - The atom:link element to read if the link was not read by this method.
        /// </remarks>
        private void ReadStreamPropertyEditOrReadLinkInEntry(IODataAtomReaderEntryState entryState, string streamPropertyName, string linkHRef, bool editLink)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(streamPropertyName != null, "streamPropertyName != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            ODataStreamReferenceValue streamReferenceValue = GetNewOrExistingStreamPropertyValue(entryState, streamPropertyName);
            Debug.Assert(streamReferenceValue != null, "streamReferenceValue != null");

            // set the edit-link or the read-link
            Uri href = linkHRef == null ? null : this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);

            if (editLink)
            {
                // edit-link
                if (streamReferenceValue.EditLink != null)
                {
                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleEditLinks(streamPropertyName));
                }

                streamReferenceValue.EditLink = href;
            }
            else
            {
                // read-link
                if (streamReferenceValue.ReadLink != null)
                {
                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleReadLinks(streamPropertyName));
                }

                streamReferenceValue.ReadLink = href;
            }

            // set the ContentType
            string contentType = this.XmlReader.GetAttributeValue(this.AtomTypeAttributeName, this.EmptyNamespace);

            if (contentType != null && streamReferenceValue.ContentType != null)
            {
                if (string.CompareOrdinal(contentType, streamReferenceValue.ContentType) != 0)
            {
                    throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleContentTypes(streamPropertyName));
                }
            }

            streamReferenceValue.ContentType = contentType;

            // set the ETag
            if (editLink)
            {
                string etag = this.XmlReader.GetAttributeValue(this.ODataETagAttributeName, this.XmlReader.ODataMetadataNamespace);
                streamReferenceValue.ETag = etag;
            }

            // Skip the entire Atom:link element.
            this.XmlReader.Skip();
        }

        /// <summary>
        /// Reads a an association link in atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="linkRelation">The rel attribute value for the link.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <returns>true, if the association link was read succesfully, false otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if the link was read by this method.
        ///                  XmlNodeType.Element atom:link  - The atom:link element to read if the link was not read by this method.
        /// </remarks>
        private bool TryReadAssociationLinkInEntry(IODataAtomReaderEntryState entryState, string linkRelation, string linkHRef)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(linkRelation != null, "linkRelation != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            string associationLinkName = AtomUtils.GetNameFromAtomLinkRelationAttribute(linkRelation, AtomConstants.ODataNavigationPropertiesAssociationLinkRelationPrefix);
            if (string.IsNullOrEmpty(associationLinkName))
            {
                return false;
            }

            // Get the type of the link
            string navigationLinkType = this.XmlReader.GetAttributeValue(this.AtomTypeAttributeName, this.EmptyNamespace);

            if (!string.IsNullOrEmpty(navigationLinkType) &&
                string.Compare(navigationLinkType, MimeConstants.MimeApplicationXml, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }

            ODataAssociationLink associationLink = new ODataAssociationLink { Name = associationLinkName };

            if (linkHRef != null)
            {
                associationLink.Url = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
            }

            ValidationUtils.ValidateNavigationPropertyDefined(associationLink.Name, entryState.EntityType);
            entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(associationLink);
            ReaderUtils.AddAssociationLinkToEntry(entryState.Entry, associationLink);

            this.XmlReader.Skip();
            return true;
        }


        /// <summary>
        /// Reads an ATOM element inside the atom:feed from the input.
        /// </summary>
        /// <param name="feedState">The reader feed state for the feed being read.</param>
        /// <returns>true if the atom:entry element was found and the reader was not moved;
        /// false otherwise and the reader is positioned on the next node after the ATOM element.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element in ATOM namespace - The element in ATOM namespace to read.
        /// Post-Condition: Any                                   - The node after the ATOM element which was consumed.
        ///                 XmlNodeType.Element atom:entry        - The start of the atom:entry element (the reader did not move in this case).
        /// </remarks>
        private bool ReadAtomElementInFeed(IODataAtomReaderFeedState feedState)
        {
            Debug.Assert(feedState != null, "feedState != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "The reader must be on an element in the ATOM namespace for this method to work.");

            // ATOM elements
            if (this.XmlReader.LocalNameEquals(this.AtomEntryElementName))
            {
                // atom:entry
                return true;
            }

            if (this.XmlReader.LocalNameEquals(this.AtomLinkElementName))
            {
                // atom:link
                // NOTE: this method does not move the reader; thus a potential xml:base definition on the element
                //       works correctly for the values read from the attributes until we move the reader.
                string linkRelation, linkHRef;
                this.ReadAtomLinkRelationAndHRef(out linkRelation, out linkHRef);

                if (linkRelation != null)
                {
                    if (this.ReadAtomStandardRelationLinkInFeed(feedState, linkRelation, linkHRef))
                    {
                        return false;
                    }

                    string ianaRelation = AtomUtils.GetNameFromAtomLinkRelationAttribute(linkRelation, AtomConstants.IanaLinkRelationsNamespace);
                    if (ianaRelation != null && this.ReadAtomStandardRelationLinkInFeed(feedState, ianaRelation, linkHRef))
                    {
                        return false;
                    }
                }

                // TODO: Handle ATOM metadata for link elements
                this.XmlReader.Skip();
            }
            else if (this.XmlReader.LocalNameEquals(this.AtomIdElementName))
            {
                // atom:id
                // The id should be an IRI (so text only), but we already allow much more for entry/id, so we should do the same for feed/id
                // just to be consistent.
                string idValue = this.XmlReader.ReadElementValue();

                feedState.Feed.Id = idValue;
            }
            else
            {
                // TODO: Handle ATOM metadata here
                // For now skip over such elements
                this.XmlReader.Skip();
            }

            return false;
        }

        /// <summary>
        /// Reads the atom:link element with one of the standard relation values in the atom:feed element.
        /// </summary>
        /// <param name="feedState">The reader feed state for the feed being read.</param>
        /// <param name="linkRelation">The rel attribute value for the link.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <returns>If the rel was one of the recognized standard relations and this method read the link
        /// the return value is true. Otherwise the method doesn't move the reader and returns false.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if the link was read by this method.
        ///                  XmlNodeType.Element atom:link  - The atom:link element to read if the link was not read by this method.
        /// </remarks>
        private bool ReadAtomStandardRelationLinkInFeed(IODataAtomReaderFeedState feedState, string linkRelation, string linkHRef)
        {
            Debug.Assert(feedState != null, "feedState != null");
            Debug.Assert(linkRelation != null, "linkRelation != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomLinkRelationNextAttributeValue) == 0)
            {
                // next link
                // TODO: only process the first next link we find as the edit link
                // TODO: do we want to add ATOM metadata for the next link? We have it for almost every other link.
                if (linkHRef != null)
                {
                    feedState.Feed.NextPageLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                this.XmlReader.Skip();
                return true;
            }

            // TODO read self link into special ATOM metadata
            return false;
        }

        /// <summary>
        /// Reads the atom:link element's rel and href attributes.
        /// </summary>
        /// <param name="linkRelation">The value of the rel attribute or null if no such attribute was found.</param>
        /// <param name="linkHRef">The value of the href attribute or null if no such attribute was found.</param>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  XmlNodeType.Element atom:link  - The atom:link element to read - the reader doesn't not move.
        /// </remarks>
        private void ReadAtomLinkRelationAndHRef(out string linkRelation, out string linkHRef)
        {
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            linkRelation = null;
            linkHRef = null;
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
                    if (this.XmlReader.LocalNameEquals(this.AtomLinkRelationAttributeName))
                    {
                        linkRelation = this.XmlReader.Value;
                        if (linkHRef != null)
                        {
                            break;
                        }
                    }
                    else if (this.XmlReader.LocalNameEquals(this.AtomLinkHrefAttributeName))
                    {
                        linkHRef = this.XmlReader.Value;
                        if (linkRelation != null)
                        {
                            break;
                        }
                    }
                }
            }

            this.XmlReader.MoveToElement();
        }

        /// <summary>
        /// Reads the content of navigation link.
        /// </summary>
        /// <returns>
        /// true if m:inline was found,
        /// false if the end of the atom:link was found.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  Any                              - a child node of the atom:link element.
        /// Post-Condition: XmlNodeType.Element m:inline     - the m:inline was found, the method returns true.
        ///                 XmlNodeType.EndElement atom:link - the atom:link end element, end of the navigation link, the method returns false.
        /// </remarks>
        private bool ReadNavigationLinkContent()
        {
            this.XmlReader.AssertNotBuffering();

            while (true)
            {
                switch (this.XmlReader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        // End of the atom:link
                        return false;

                    case XmlNodeType.Element:
                        if (this.XmlReader.LocalNameEquals(this.ODataInlineElementName) && this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                        {
                            return true;
                        }

                        this.XmlReader.Skip();
                        break;

                    default:
                        this.XmlReader.Skip();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads content of the m:inline element.
        /// </summary>
        /// <returns>
        /// Enumeration denoting what was found in the content.
        /// Empty - The end of the m:inline element was found - the reader is positioned on the m:inline end element.
        /// Entry - An expanded entry was found - the reader is positioned on atom:entry element.
        /// Feed - An expanded feed was found - the reader is positioned on atom:feed element.
        /// None - will never be returned.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  Any                                  - child node of the m:inline element.
        /// Post-Condition: XmlNodeType.Element atom:feed        - an expanded feed was found.
        ///                 XmlNodeType.Element atom:entry       - an expanded entry was found.
        ///                 XmlNodeType.EndElement m:inline      - empty inline was found.
        /// </remarks>
        private ODataAtomDeserializerExpandedNavigationLinkContent ReadInlineElementContent()
        {
            this.XmlReader.AssertNotBuffering();

            while (true)
            {
                switch (this.XmlReader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        // End of m:inline reached but we didn't find any interesting payload
                        Debug.Assert(
                            this.XmlReader.LocalName == AtomConstants.ODataInlineElementName &&
                            this.XmlReader.NamespaceURI == AtomConstants.ODataMetadataNamespace,
                            "The reader must be on the m:inline end element.");
                        return ODataAtomDeserializerExpandedNavigationLinkContent.Empty;

                    case XmlNodeType.Element:
                        if (this.XmlReader.NamespaceEquals(this.AtomNamespace))
                        {
                            if (this.XmlReader.LocalNameEquals(this.AtomEntryElementName))
                            {
                                return ODataAtomDeserializerExpandedNavigationLinkContent.Entry;
                            }

                            if (this.XmlReader.LocalNameEquals(this.AtomFeedElementName))
                            {
                                return ODataAtomDeserializerExpandedNavigationLinkContent.Feed;
                            }

                            throw new ODataException(Strings.ODataAtomEntryAndFeedDeserializer_UnknownElementInInline(this.XmlReader.LocalName));
                        }

                        // Skip all elements not in the ATOM namespaces
                        this.XmlReader.Skip();
                        break;

                    default:
                        // Skip all other nodes
                        this.XmlReader.Skip();
                        break;
                }
            }
        }
    }
}
