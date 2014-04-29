//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for entries and feeds.
    /// </summary>
    internal sealed class ODataAtomEntryAndFeedDeserializer : ODataAtomPropertyAndValueDeserializer
    {
        #region Atomized strings
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

        /// <summary>Element name for m:action.</summary>
        private readonly string ODataActionElementName;

        /// <summary>Element name for m:function.</summary>
        private readonly string ODataFunctionElementName;

        /// <summary>Attribute name for m:action|m:function/@metadata.</summary>
        private readonly string ODataOperationMetadataAttribute;

        /// <summary>Attribute name for m:action|m:function/@title.</summary>
        private readonly string ODataOperationTitleAttribute;

        /// <summary>Attribute name for m:action|m:function/@target.</summary>
        private readonly string ODataOperationTargetAttribute;
        #endregion

        /// <summary>
        /// The reader used to parse annotation elements.
        /// </summary>
        private readonly ODataAtomAnnotationReader atomAnnotationReader;

        /// <summary>
        /// ATOM deserializer for ATOM metadata on entries.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomEntryMetadataDeserializer entryMetadataDeserializer;

        /// <summary>
        /// ATOM deserializer for ATOM metadata on feeds.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomFeedMetadataDeserializer feedMetadataDeserializer;

        /// <summary>
        /// null id regular expression
        /// This is used to check if the id is a transient id in atom format
        /// </summary>
        private static readonly Regex transientIdRegex;

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static ODataAtomEntryAndFeedDeserializer()
        {
            transientIdRegex = new Regex(AtomConstants.AtomTransientIdRegularExpression);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomEntryAndFeedDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            XmlNameTable nameTable = this.XmlReader.NameTable;
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
            this.ODataActionElementName = nameTable.Add(AtomConstants.ODataActionElementName);
            this.ODataFunctionElementName = nameTable.Add(AtomConstants.ODataFunctionElementName);
            this.ODataOperationMetadataAttribute = nameTable.Add(AtomConstants.ODataOperationMetadataAttribute);
            this.ODataOperationTitleAttribute = nameTable.Add(AtomConstants.ODataOperationTitleAttribute);
            this.ODataOperationTargetAttribute = nameTable.Add(AtomConstants.ODataOperationTargetAttribute);

            this.atomAnnotationReader = new ODataAtomAnnotationReader(this.AtomInputContext, this);
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
        /// ATOM deserializer for ATOM metadata on feeds.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomFeedMetadataDeserializer FeedMetadataDeserializer
        {
            get
            {
                return this.feedMetadataDeserializer ??
                       (this.feedMetadataDeserializer = new ODataAtomFeedMetadataDeserializer(this.AtomInputContext, false));
            }
        }

        /// <summary>
        /// Flag indicating if ATOM metadata is required to be read by the user.
        /// </summary>
        private bool ReadAtomMetadata
        {
            get
            {
                return this.AtomInputContext.MessageReaderSettings.EnableAtomMetadataReading;
            }
        }

        /// <summary>
        /// Ensure a media resource is created for the specified entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        internal static void EnsureMediaResource(IODataAtomReaderEntryState entryState)
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
        /// Verified that the reader is positioned on the atom:entry start element node.
        /// </summary>
        internal void VerifyEntryStart()
        {
            this.XmlReader.AssertNotBuffering();

            if (this.XmlReader.NodeType != XmlNodeType.Element)
            {
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_ElementExpected(this.XmlReader.NodeType));
            }

            if (!this.XmlReader.NamespaceEquals(this.AtomNamespace) || !this.XmlReader.LocalNameEquals(this.AtomEntryElementName))
            {
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_EntryElementWrongName(this.XmlReader.LocalName, this.XmlReader.NamespaceURI));
            }
        }

        /// <summary>
        /// Read the start of the entry.
        /// This method verifies that the current element is atom:entry and it reads the ETag from it.
        /// </summary>
        /// <param name="entry">The entry instance to fill the properties on.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element   - The method will fail if it's not element called atom:entry
        /// Post-Condition: XmlNodeType.Element   - The atom:entry element
        /// </remarks>
        internal void ReadEntryStart(ODataEntry entry)
        {
            this.XmlReader.AssertNotBuffering();

            this.VerifyEntryStart();

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
        /// <returns>A descriptor representing the navigation link detected;
        /// null if no navigation link was found and the end of the entry was reached.</returns>
        /// <remarks>
        /// Pre-Condition:  Anything but Attribute - the child node of the atom:entry element, can be pretty much anything, the method will skip over insignificant nodes and text nodes if found.
        /// Post-Condition: XmlNodeType.EndElement atom:entry - The end of the atom:entry element if no nav. link was found and the end of the entry was reached.
        ///                 XmlNodeType.Element atom:link     - The start tag of the atom:link element representing a navigation link.
        /// </remarks>
        internal ODataAtomReaderNavigationLinkDescriptor ReadEntryContent(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.XmlReader.AssertNotBuffering();
            Debug.Assert(this.XmlReader.NodeType != XmlNodeType.Attribute, "The reader must be positioned on a child node of the atom:entry element.");

            ODataAtomReaderNavigationLinkDescriptor navigationLinkDescriptor = null;

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
                    navigationLinkDescriptor = this.ReadAtomElementInEntry(entryState);
                    if (navigationLinkDescriptor != null)
                    {
                        entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNamesOnNavigationLinkStart(navigationLinkDescriptor.NavigationLink);
                        break;
                    }
                }
                else if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                {
                    AtomInstanceAnnotation annotation;

                    if (this.XmlReader.LocalNameEquals(this.AtomPropertiesElementName))
                    {
                        this.ValidateDuplicateElement(entryState.HasProperties && this.AtomInputContext.UseDefaultFormatBehavior);

                        // m:properties outside of content -> MLE
                        EnsureMediaResource(entryState);
                        this.ReadProperties(entryState.EntityType, entryState.Entry.Properties.ToReadOnlyEnumerable("Properties"), entryState.DuplicatePropertyNamesChecker);

                        // Read over the end element or the empty start element.
                        this.XmlReader.Read();

                        entryState.HasProperties = true;
                    }
                    else if (this.ReadingResponse && this.TryReadOperation(entryState))
                    {
                    }
                    else if (this.atomAnnotationReader.TryReadAnnotation(out annotation))
                    {
                        // An annotation occurring as a direct child of an entry element can only target the entry it was found in.
                        // To target a property, the annotation must be found in the <m:properties> element.
                        // If we find an annotation breaking this rule, fail.
                        if (annotation.IsTargetingCurrentElement)
                        {
                            entryState.Entry.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.TermName, annotation.Value));
                        }
                        else
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_AnnotationWithNonDotTarget(annotation.Target, annotation.TermName));
                        }
                    }
                    else
                    {
                        // Ignore all other elements in the metadata namespace which we don't recognize (extensibility point)
                        this.XmlReader.Skip();
                    }
                }
                else
                {
                    this.XmlReader.Skip();
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
            return navigationLinkDescriptor;
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
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);

            if (!this.XmlReader.NamespaceEquals(this.AtomNamespace) || !this.XmlReader.LocalNameEquals(this.AtomFeedElementName))
            {
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_FeedElementWrongName(this.XmlReader.LocalName, this.XmlReader.NamespaceURI));
            }

            this.AssertXmlCondition(XmlNodeType.Element);
            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the content of a feed (child nodes of the atom:feed).
        /// </summary>
        /// <param name="feedState">The reader feed state for the feed being read.</param>
        /// <param name="isExpandedLinkContent">true if the feed is inside an expanded link.</param>
        /// <returns>true if an entry was found or false if no more entries were found in the feed.</returns>
        /// <remarks>
        /// Pre-Condition:  Anything but Attribute           - the child node of the atom:feed element, can be pretty much anything, the method will skip over insignificant nodes and text nodes if found.
        /// Post-Condition: XmlNodeType.EndElement atom:feed - The end of the atom:feed element if no entry was found and the end of the feed was reached.
        ///                 XmlNodeType.Element atom:entry   - The start tag of the atom:entry element representing an entry in the feed.
        /// </remarks>
        internal bool ReadFeedContent(IODataAtomReaderFeedState feedState, bool isExpandedLinkContent)
        {
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
                    if (this.ReadAtomElementInFeed(feedState, isExpandedLinkContent))
                    {
                        // We've found an entry - return.
                        entryFound = true;
                        break;
                    }
                }
                else if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                {
                    AtomInstanceAnnotation annotation;

                    if (this.ReadingResponse && !isExpandedLinkContent && this.XmlReader.LocalNameEquals(this.ODataCountElementName))
                    {
                        // m:count
                        this.ValidateDuplicateElement(feedState.HasCount);

                        // Note that we allow negative values to be read.
                        long countValue = (long)AtomValueUtils.ReadPrimitiveValue(this.XmlReader, EdmCoreModel.Instance.GetInt64(true));
                        feedState.Feed.Count = countValue;

                        // Read over the end element or the empty start element.
                        this.XmlReader.Read();

                        feedState.HasCount = true;
                    }
                    else if (this.atomAnnotationReader.TryReadAnnotation(out annotation))
                    {
                        // Note: There is some uncertainty over whether annotations on an expanded feed should appear on the ODataNavigationLink, the ODataFeed, or both.
                        // In the short term, we are failing when we encounter this scenario so that we can enable it later without causing a breaking change.
                        if (isExpandedLinkContent)
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_EncounteredAnnotationInNestedFeed);
                        }

                        // An annotation occurring as a direct child of a feed element can only target the feed it was found in.
                        // If we find an annotation breaking this rule, fail.
                        if (annotation.IsTargetingCurrentElement)
                        {
                            feedState.Feed.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.TermName, annotation.Value));
                        } 
                        else
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_AnnotationWithNonDotTarget(annotation.Target, annotation.TermName));
                        }
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
        /// Determines if the reader is positioned on the m:inline end element or empty m:inline start element.
        /// </summary>
        /// <returns>true if the reader is on m:inline end element or m:inline empty start element; false otherwise.</returns>
        internal bool IsReaderOnInlineEndElement()
        {
            this.XmlReader.AssertNotBuffering();

            return this.XmlReader.LocalNameEquals(this.ODataInlineElementName) && this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace) &&
                ((this.XmlReader.NodeType == XmlNodeType.Element && this.XmlReader.IsEmptyElement) ||
                 this.XmlReader.NodeType == XmlNodeType.EndElement);
        }

        /// <summary>
        /// Skips everything until an end-element for atom:link is found.
        /// </summary>
        /// <remarks>
        /// This method should only be used to skip the rest of the link content after the ReadNavigationLinkContentBeforeExpansion was called.
        /// Pre-Condition:  XmlNodeType.Element atom:entry       - start of the expanded entry
        ///                 XmlNodeType.Element atom:feed        - start of the expanded feed
        ///                 XmlNodeType.Element (empty) m:inline - empty m:inline element (emptyInline = true)
        ///                 XmlNodeType.EndElement m:inline      - end of the m:inline element.
        /// Post-Condition: XmlNodeType.EndElement atom:link     - the end element of the navigation link.
        /// </remarks>
        internal void SkipNavigationLinkContentOnExpansion()
        {
            Debug.Assert(
                !(this.XmlReader.NodeType == XmlNodeType.EndElement && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace),
                "This method must not be called when the ReadNavigationLinkContentBeforeExpansion return ODataAtomDeserializerExpandedNavigationLinkContent.Noen.");
            this.XmlReader.AssertNotBuffering();

            while (true)
            {
                // Note that Skip works like a Read on nodes which don't have any children (anything but element pretty much);
                this.XmlReader.Skip();

                if (this.XmlReader.NodeType == XmlNodeType.EndElement && this.XmlReader.LocalNameEquals(this.AtomLinkElementName) && this.XmlReader.NamespaceEquals(this.AtomNamespace))
                {
                    return;
                }
            }
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

                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline(expandedNavigationLinkContent.ToString()));
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
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleInlineElementsInLink);
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
                                        // If multiple 'scheme' or 'term' attributes are present the one with non-empty namespace wins.
                                        bool isEmptyNamespace = this.XmlReader.NamespaceEquals(this.EmptyNamespace);
                                        if (isEmptyNamespace)
                                        {
                                            if (this.XmlReader.LocalNameEquals(this.AtomCategorySchemeAttributeName))
                                            {
                                                if (string.CompareOrdinal(this.XmlReader.Value, Atom.AtomConstants.ODataSchemeNamespace) == 0)
                                                {
                                                    foundODataScheme = true;
                                                }
                                            }
                                            else if (this.XmlReader.LocalNameEquals(this.AtomCategoryTermAttributeName))
                                            {
                                                if (typeName == null)
                                                {
                                                    typeName = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName(this.XmlReader.Value));
                                                }
                                            }
                                        }
                                    }

                                    if (foundODataScheme)
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
        private ODataStreamReferenceValue GetNewOrExistingStreamPropertyValue(IODataAtomReaderEntryState entryState, string streamPropertyName)
        {
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(streamPropertyName != null, "streamPropertyName != null");

            ReadOnlyEnumerable<ODataProperty> properties = entryState.Entry.Properties.ToReadOnlyEnumerable("Properties");

            // Property names are case sensitive, so compare in a case sensitive way.
            ODataProperty streamProperty = properties.FirstOrDefault(p => String.CompareOrdinal(p.Name, streamPropertyName) == 0);

            ODataStreamReferenceValue streamReferenceValue;
            if (streamProperty == null)
            {
                // The ValidateLinkPropertyDefined will fail if a stream property is not defined and the reader settings don't allow
                // reporting undeclared link properties. So if the method returns null, it means report the undeclared property anyway.
                IEdmProperty streamEdmProperty = ReaderValidationUtils.ValidateLinkPropertyDefined(streamPropertyName, entryState.EntityType, this.MessageReaderSettings);

                streamReferenceValue = new ODataStreamReferenceValue();
                streamProperty = new ODataProperty
                {
                    Name = streamPropertyName,
                    Value = streamReferenceValue
                };

                ReaderValidationUtils.ValidateStreamReferenceProperty(streamProperty, entryState.EntityType, streamEdmProperty, this.MessageReaderSettings);
                entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(streamProperty);
                properties.AddToSourceList(streamProperty);
            }
            else
            {
                streamReferenceValue = streamProperty.Value as ODataStreamReferenceValue;
                if (streamReferenceValue == null)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_StreamPropertyDuplicatePropertyName(streamPropertyName));
                }
            }

            return streamReferenceValue;
        }

        /// <summary>
        /// If the <paramref name="duplicateElementFound"/> is true, then the default behavior should throw.
        /// </summary>
        /// <remarks>This method assumes the reader is positioned on the duplicated element.</remarks>
        /// <param name="duplicateElementFound">Used to determine if duplicate check should throw an exception.</param>
        private void ValidateDuplicateElement(bool duplicateElementFound)
        {
            if (duplicateElementFound)
            {
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_DuplicateElements(this.XmlReader.NamespaceURI, this.XmlReader.LocalName));
            }
        }

        /// <summary>
        /// Reads an ATOM element inside the atom:entry from the input.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <returns>
        /// If the atom element is representing a navigation link a descriptor for that link is returned,
        /// otherwise null.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element in ATOM namespace - The element in ATOM namespace to read.
        /// Post-Condition: Any                                   - The node after the ATOM element if it's not a navigation link.
        ///                 XmlNodeType.Element atom:link         - The start tag of atom:link if it's a navigation link.
        /// </remarks>
        private ODataAtomReaderNavigationLinkDescriptor ReadAtomElementInEntry(IODataAtomReaderEntryState entryState)
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
                string attributeValue = this.XmlReader.GetAttribute(this.AtomCategorySchemeAttributeName, this.EmptyNamespace);

                if (attributeValue != null && string.CompareOrdinal(attributeValue, Atom.AtomConstants.ODataSchemeNamespace) == 0)
                {
                    this.ValidateDuplicateElement(entryState.HasTypeNameCategory && this.AtomInputContext.UseDefaultFormatBehavior);

                    if (this.ReadAtomMetadata)
                    {
                        entryState.AtomEntryMetadata.CategoryWithTypeName = this.EntryMetadataDeserializer.ReadAtomCategoryElement();
                    }
                    else
                    {
                        this.XmlReader.Skip();
                    }

                    entryState.HasTypeNameCategory = true;
                }
                else
                {
                    // atom:category
                    // If we're not to store ATOM metadata, we can safely skip all category elements
                    // since we've already read the typename and no other category element holds anything of interest.
                    // That's true even if there are multiple category elements of interest, for typename we already took the first.
                    if (this.ReadAtomMetadata)
                    {
                        this.EntryMetadataDeserializer.ReadAtomCategoryElementInEntryContent(entryState);
                    }
                    else
                    {
                        this.XmlReader.Skip();
                    }
                }
            }
            else if (this.XmlReader.LocalNameEquals(this.AtomLinkElementName))
            {
                // atom:link
                return this.ReadAtomLinkElementInEntry(entryState);
            }
            else
            {
                if (this.ReadAtomMetadata)
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

            this.ValidateDuplicateElement(entryState.HasContent && this.AtomInputContext.UseDefaultFormatBehavior);

            // atom:content
            // Read the attributes - we're interested in type and src
            string contentType;
            string contentSource;
            this.ReadAtomContentAttributes(out contentType, out contentSource);

            if (contentSource != null)
            {
                // atom:content/@src means this is an MLE
                ODataEntry entry = entryState.Entry;
                EnsureMediaResource(entryState);

                if (!this.AtomInputContext.UseServerFormatBehavior)
                {
                    entry.MediaResource.ReadLink = this.ProcessUriFromPayload(contentSource, this.XmlReader.XmlBaseUri);
                }

                entry.MediaResource.ContentType = contentType;

                // Verify that the atom:content element is empty, since for MLEs there must be no content in-line.
                if (!this.XmlReader.TryReadEmptyElement())
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty);
                }
            }
            else
            {
                string mediaType = contentType;
                if (!string.IsNullOrEmpty(contentType))
                {
                    mediaType = this.VerifyAtomContentMediaType(contentType);
                }

                // Normal content with properties - not an MLE
                entryState.MediaLinkEntry = false;

                this.XmlReader.MoveToElement();
                if (!this.XmlReader.IsEmptyElement && this.XmlReader.NodeType != XmlNodeType.EndElement)
                {
                    if (string.IsNullOrEmpty(mediaType))
                    {
                        // Show "plain/text" media type behavior. Intentionally discard the value read.
                        this.XmlReader.ReadElementContentValue();
                    }
                    else
                    {
                        // The behavior to read atom:content is to ignore all non-element nodes and all elements
                        // that are not in the OData metadata namespace. If we find elements in the OData metadata
                        // namespace that we don't expect, we fail.
                        this.XmlReader.ReadStartElement();

                        while (this.XmlReader.NodeType != XmlNodeType.EndElement)
                        {
                            switch (this.XmlReader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    // Test for an element in the OData metadata namespace
                                    if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                                    {
                                        // We fail on any elements in the OData metadata namespace except for the 'properties' element.
                                        if (!this.XmlReader.LocalNameEquals(this.AtomPropertiesElementName))
                                        {
                                            throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_ContentWithInvalidNode(this.XmlReader.LocalName));
                                        }

                                        this.ValidateDuplicateElement(entryState.HasProperties);
                                        this.ReadProperties(entryState.EntityType, entryState.Entry.Properties.ToReadOnlyEnumerable("Properties"), entryState.DuplicatePropertyNamesChecker);

                                        entryState.HasProperties = true;
                                    }
                                    else
                                    {
                                        // Ignore all elements in the non-OData metadata namespace
                                        this.XmlReader.SkipElementContent();
                                    }

                                    // Read over the m:properties end element (or empty start element)
                                    this.XmlReader.Read();

                                    break;
                                case XmlNodeType.EndElement:
                                    break;
                                default:
                                    // Skip over all non-element nodes
                                    this.XmlReader.Skip();
                                    break;
                            }
                        }
                    }
                }
            }

            // Read over the end element, or empty start element.
            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();

            entryState.HasContent = true;
        }

        /// <summary>
        /// Reads the attributes of the &lt;atom:content&gt; element.
        /// </summary>
        /// <param name="contentType">The content type attribute value (or null if not found).</param>
        /// <param name="contentSource">The content source attribute value (or null if not found).</param>
        private void ReadAtomContentAttributes(out string contentType, out string contentSource)
        {
            contentType = null;
            contentSource = null;

            while (this.XmlReader.MoveToNextAttribute())
            {
                // If multiple 'type' or 'src' attributes are present the one with non-empty namespace wins.
                bool isEmptyNamespace = this.XmlReader.NamespaceEquals(this.EmptyNamespace);
                if (isEmptyNamespace)
                {
                    if (this.XmlReader.LocalNameEquals(this.AtomTypeAttributeName))
                    {
                        // type attribute
                        if (contentType == null)
                        {
                            contentType = this.XmlReader.Value;
                        }
                    }
                    else if (this.XmlReader.LocalNameEquals(this.MediaLinkEntryContentSourceAttributeName))
                    {
                        // src or atom:src attribute
                        if (contentSource == null)
                        {
                            contentSource = this.XmlReader.Value;
                        }
                    }
                }
            }
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

            this.ValidateDuplicateElement(entryState.HasId && this.AtomInputContext.UseDefaultFormatBehavior);

            string idValue = this.XmlReader.ReadElementValue();

            if (idValue != null && idValue.Length == 0)
            {
                entryState.Entry.Id = null;
            }
            else
            {
                if (idValue != null && IsTransientId(idValue))
                {
                    entryState.Entry.IsTransient = true;
                }
                else
                {
                    entryState.Entry.Id = UriUtils.CreateUriAsEntryOrFeedId(idValue, UriKind.Absolute);
                }
            }

            entryState.HasId = true;
        }

        /// <summary>
        /// Reads the atom:link element in atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <returns>
        /// If the link is a navigation link the method returns a descriptor representing that link,
        /// otherwise the method returns null.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if it's not a navigation link.
        ///                  XmlNodeType.Element atom:link  - The atom:link start tag if it's a navigation link.
        /// </remarks>
        private ODataAtomReaderNavigationLinkDescriptor ReadAtomLinkElementInEntry(IODataAtomReaderEntryState entryState)
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
            // method is rather expensive (as it has to unescape the value, create a URI and then compare the prefix for the IANA namespace)
            // so we first compare the simple "edit" case, and only if that fails we try the "IANANamespace/edit" for all the standard
            // relations.
            if (linkRelation != null)
            {
                bool isStreamPropertyLink = false;
                if (!this.AtomInputContext.UseServerFormatBehavior)
                {
                    if (this.TryReadAtomStandardRelationLinkInEntry(entryState, linkRelation, linkHRef))
                    {
                        return null;
                    }
                }

                // All the other checks require the rel attribute to be a valid URI value, so if the unescape operation returns null
                // we don't need to look at that link anymore.
                string unescapedLinkRelation = AtomUtils.UnescapeAtomLinkRelationAttribute(linkRelation);
                if (unescapedLinkRelation != null)
                {
                    if (!this.AtomInputContext.UseServerFormatBehavior)
                    {
                        string ianaRelation = AtomUtils.GetNameFromAtomLinkRelationAttribute(unescapedLinkRelation, AtomConstants.IanaLinkRelationsNamespace);
                        if (ianaRelation != null && this.TryReadAtomStandardRelationLinkInEntry(entryState, ianaRelation, linkHRef))
                        {
                            return null;
                        }
                    }

                    ODataAtomReaderNavigationLinkDescriptor navigationLinkDescriptor = this.TryReadNavigationLinkInEntry(entryState, unescapedLinkRelation, linkHRef);
                    if (navigationLinkDescriptor != null)
                    {
                        return navigationLinkDescriptor;
                    }

                    if (this.TryReadStreamPropertyLinkInEntry(entryState, unescapedLinkRelation, linkHRef, out isStreamPropertyLink))
                    {
                        return null;
                    }

                    if (!isStreamPropertyLink && this.TryReadAssociationLinkInEntry(entryState, unescapedLinkRelation, linkHRef))
                    {
                        return null;
                    }
                }
            }

            if (this.ReadAtomMetadata)
            {
                AtomLinkMetadata linkMetadata =
                    this.EntryMetadataDeserializer.ReadAtomLinkElementInEntryContent(linkRelation, linkHRef);
                if (linkMetadata != null)
                {
                    entryState.AtomEntryMetadata.AddLink(linkMetadata);
                }
            }
            
            // Skip the element since we don't need it.
            this.XmlReader.Skip();
            
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
                // Edit link
                //
                // First check whether we have already seen an edit link; if we have we throw in default format mode and
                // ignore any further edit links in WCF DS client or server mode.
                if (entryState.HasEditLink && !this.AtomInputContext.UseServerApiBehavior)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry(AtomConstants.AtomEditRelationAttributeValue));
                }

                if (linkHRef != null)
                {
                    entryState.Entry.EditLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                if (this.ReadAtomMetadata)
                {
                    entryState.AtomEntryMetadata.EditLink =
                        this.EntryMetadataDeserializer.ReadAtomLinkElementInEntryContent(linkRelation, linkHRef);
                }

                entryState.HasEditLink = true;

                this.XmlReader.Skip();
                return true;
            }

            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomSelfRelationAttributeValue) == 0)
            {
                // Self link
                //
                // First check whether we have already seen a self link; if we have we throw in default format mode and
                // ignore any further self links in WCF DS client or server mode.
                if (entryState.HasReadLink && !this.AtomInputContext.UseServerApiBehavior)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry(AtomConstants.AtomSelfRelationAttributeValue));
                }

                if (linkHRef != null)
                {
                    entryState.Entry.ReadLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                if (this.ReadAtomMetadata)
                {
                    entryState.AtomEntryMetadata.SelfLink =
                        this.EntryMetadataDeserializer.ReadAtomLinkElementInEntryContent(linkRelation, linkHRef);
                }

                entryState.HasReadLink = true;

                this.XmlReader.Skip();
                return true;
            }

            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomEditMediaRelationAttributeValue) == 0)
            {
                // edit-media link
                if (entryState.HasEditMediaLink && !this.AtomInputContext.UseServerApiBehavior)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry(AtomConstants.AtomEditMediaRelationAttributeValue));
                }

                
                // Note that we always mark the entry as MLE if we find the edit-media rel. This is so that we have a place to store the ATOM metadata for the link
                // (as we need the MediaResource property to be filled). We do so even without ATOM metadata to maintain consistent behavior.
                EnsureMediaResource(entryState);
                ODataEntry entry = entryState.Entry;

                if (linkHRef != null)
                {
                    entry.MediaResource.EditLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                string mediaETagValue = this.XmlReader.GetAttribute(this.ODataETagAttributeName, this.XmlReader.ODataMetadataNamespace);
                if (mediaETagValue != null)
                {
                    entry.MediaResource.ETag = mediaETagValue;
                }

                if (this.ReadAtomMetadata)
                {
                    AtomLinkMetadata linkMetadata = this.EntryMetadataDeserializer.ReadAtomLinkElementInEntryContent(linkRelation, linkHRef);

                    entry.MediaResource.SetAnnotation(
                        new AtomStreamReferenceMetadata
                        {
                            EditLink = linkMetadata
                        });
                }

                entryState.HasEditMediaLink = true;

                this.XmlReader.Skip();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads a navigation link in entry element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="linkRelation">The value of the rel attribute of the link to read, unescaped parsed URI.</param>
        /// <param name="linkHRef">The value of the href attribute of the link to read.</param>
        /// <returns>A descriptor of a navigation link if a navigation link was found; null otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element atom:link - the start tag of the atom:link element to read.
        /// Post-Condition: XmlNodeType.Element atom:link - the start tag of the atom:link element - the reader doesn't move
        /// </remarks>
        private ODataAtomReaderNavigationLinkDescriptor TryReadNavigationLinkInEntry(
            IODataAtomReaderEntryState entryState,
            string linkRelation,
            string linkHRef)
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

            // Lookup the property in metadata
            // Note that we already verified that the navigation link name is not empty.
            IEdmNavigationProperty navigationProperty = ReaderValidationUtils.ValidateNavigationPropertyDefined(navigationLinkName, entryState.EntityType, this.MessageReaderSettings);

            // Navigation link
            ODataNavigationLink navigationLink = new ODataNavigationLink { Name = navigationLinkName };

            // Get the type of the link
            string navigationLinkType = this.XmlReader.GetAttribute(this.AtomTypeAttributeName, this.EmptyNamespace);

            if (!string.IsNullOrEmpty(navigationLinkType))
            {
                // Fast path for most common link types
                bool hasEntryType, hasFeedType;
                bool isExactMatch = AtomUtils.IsExactNavigationLinkTypeMatch(navigationLinkType, out hasEntryType, out hasFeedType);
                if (!isExactMatch)
                {
                    // If the fast path did not work, we have to fully parse the media type.
                    string mediaTypeName, mediaTypeCharset;
                    IList<KeyValuePair<string, string>> contentTypeParameters = HttpUtils.ReadMimeType(navigationLinkType, out mediaTypeName, out mediaTypeCharset);
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
                            if (HttpUtils.CompareMediaTypeParameterNames(MimeConstants.MimeTypeParameterName, contentTypeParameter.Key))
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
                            hasEntryType = true;
                        }
                        else if (string.Compare(typeParameterValue, MimeConstants.MimeTypeParameterValueFeed, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            hasFeedType = true;
                        }
                    }
                }

                if (hasEntryType)
                {
                    navigationLink.IsCollection = false;
                }
                else if (hasFeedType)
                {
                    navigationLink.IsCollection = true;
                }
            }

            if (linkHRef != null)
            {
                navigationLink.Url = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
            }

            this.XmlReader.MoveToElement();

            // Read and store ATOM link metadata (captures extra info like lang, title) if ATOM metadata reading is turned on.
            AtomLinkMetadata atomLinkMetadata = this.EntryMetadataDeserializer.ReadAtomLinkElementInEntryContent(linkRelation, linkHRef);
            if (atomLinkMetadata != null)
            {
                navigationLink.SetAnnotation(atomLinkMetadata);
            }

            return new ODataAtomReaderNavigationLinkDescriptor(navigationLink, navigationProperty);
        }

        /// <summary>
        /// Reads a stream property edit or read link in an atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="linkRelation">The rel attribute value for the link, unescaped parsed URI.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <param name="isStreamPropertyLink">true if the link is a stream property read or edit link; otherwise false.</param>
        /// <returns>true, if the named stream was read successfully, false otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if the link was read by this method.
        ///                  XmlNodeType.Element atom:link  - The atom:link element to read if the link was not read by this method.
        /// </remarks>
        private bool TryReadStreamPropertyLinkInEntry(IODataAtomReaderEntryState entryState, string linkRelation, string linkHRef, out bool isStreamPropertyLink)
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
                isStreamPropertyLink = true;
                return this.ReadStreamPropertyLinkInEntry(entryState, streamPropertyName, linkRelation, linkHRef, /*editLink*/ true);
            }

            // check if this is a read link.
            streamPropertyName = AtomUtils.GetNameFromAtomLinkRelationAttribute(linkRelation, AtomConstants.ODataStreamPropertyMediaResourceRelatedLinkRelationPrefix);
            if (streamPropertyName != null)
            {
                isStreamPropertyLink = true;
                return this.ReadStreamPropertyLinkInEntry(entryState, streamPropertyName, linkRelation, linkHRef, /*editLink*/ false);
            }

            isStreamPropertyLink = false;
            return false;
        }

        /// <summary>
        /// Reads a stream property link in an atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="streamPropertyName">The name of the stream property that is being read.</param>
        /// <param name="linkRelation">The rel attribute value for the link.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <param name="editLink">true if we are reading an edit link; otherwise false.</param>
        /// <returns>true if the stream property link was read; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if the link was read by this method.
        ///                  XmlNodeType.Element atom:link  - The atom:link element to read if the link was not read by this method.
        /// </remarks>
        private bool ReadStreamPropertyLinkInEntry(IODataAtomReaderEntryState entryState, string streamPropertyName, string linkRelation, string linkHRef, bool editLink)
        {
            if (!this.ReadingResponse)
            {
                return false;
            }

            if (streamPropertyName.Length == 0)
            {
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithEmptyName);
            }

            ODataStreamReferenceValue streamReferenceValue = this.GetNewOrExistingStreamPropertyValue(entryState, streamPropertyName);
            Debug.Assert(streamReferenceValue != null, "streamReferenceValue != null");

            AtomStreamReferenceMetadata atomStreamMetadata = null;
            if (this.ReadAtomMetadata)
            {
                // First, check if there is an existing metadata annotation on the stream reference value.
                atomStreamMetadata = streamReferenceValue.GetAnnotation<AtomStreamReferenceMetadata>();

                // If not, create a new metadata annotation.
                if (atomStreamMetadata == null)
                {
                    atomStreamMetadata = new AtomStreamReferenceMetadata();
                    streamReferenceValue.SetAnnotation(atomStreamMetadata);
                }
            }

            // set the edit-link or the read-link
            Uri href = linkHRef == null ? null : this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);

            if (editLink)
            {
                // edit-link
                if (streamReferenceValue.EditLink != null)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleEditLinks(streamPropertyName));
                }

                streamReferenceValue.EditLink = href;

                if (this.ReadAtomMetadata)
                {
                    atomStreamMetadata.EditLink = this.EntryMetadataDeserializer.ReadAtomLinkElementInEntryContent(linkRelation, linkHRef);
                }
            }
            else
            {
                // read-link
                if (streamReferenceValue.ReadLink != null)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleReadLinks(streamPropertyName));
                }

                streamReferenceValue.ReadLink = href;

                if (this.ReadAtomMetadata)
                {
                    atomStreamMetadata.SelfLink = this.EntryMetadataDeserializer.ReadAtomLinkElementInEntryContent(linkRelation, linkHRef);
                }
            }

            // set the ContentType
            string contentType = this.XmlReader.GetAttribute(this.AtomTypeAttributeName, this.EmptyNamespace);

            if (contentType != null && streamReferenceValue.ContentType != null)
            {
                if (!HttpUtils.CompareMediaTypeNames(contentType, streamReferenceValue.ContentType))
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleContentTypes(streamPropertyName));
                }
            }

            streamReferenceValue.ContentType = contentType;

            // set the ETag
            if (editLink)
            {
                string etag = this.XmlReader.GetAttribute(this.ODataETagAttributeName, this.XmlReader.ODataMetadataNamespace);
                streamReferenceValue.ETag = etag;
            }

            this.XmlReader.Skip();
            return true;
        }

        /// <summary>
        /// Reads a an association link in atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="linkRelation">The rel attribute value for the link, unescaped parsed URI.</param>
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
            if (string.IsNullOrEmpty(associationLinkName) || !this.ReadingResponse)
            {
                return false;
            }

            ReaderValidationUtils.ValidateNavigationPropertyDefined(associationLinkName, entryState.EntityType, this.MessageReaderSettings);

            // Get the type of the link
            string asssociationLinkType = this.XmlReader.GetAttribute(this.AtomTypeAttributeName, this.EmptyNamespace);

            if (asssociationLinkType != null &&
                !HttpUtils.CompareMediaTypeNames(asssociationLinkType, MimeConstants.MimeApplicationXml))
            {
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_InvalidTypeAttributeOnAssociationLink(associationLinkName));
            }

            Uri associationLinkUrl = null;

            // Allow null (we won't set the Url property) and empty (relative URL) values.
            if (linkHRef != null)
            {
                associationLinkUrl = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
            }

            ReaderUtils.CheckForDuplicateAssociationLinkAndUpdateNavigationLink(entryState.DuplicatePropertyNamesChecker, associationLinkName, associationLinkUrl);

            // TODO: Association Link - Add back support for customizing association link element in Atom

            this.XmlReader.Skip();
            return true;
        }

        /// <summary>
        /// Reads a an m:action or m:function in atom:entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <returns>true, if the m:action or m:function was read succesfully, false otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element m:action|m:function - The m:action or m:function element to read.
        /// Post-Condition:  Any                                     - The node after the m:action or m:function element if it was read by this method.
        ///                  XmlNodeType.Element m:action|m:function - The m:action or m:function element to read if it was not read by this method.
        /// </remarks>
        private bool TryReadOperation(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.ODataMetadataNamespace,
                "The XML reader must be on a metadata (m:*) element for this method to work.");

            bool isAction = false;
            if (this.XmlReader.LocalNameEquals(this.ODataActionElementName))
            {
                // m:action
                isAction = true;
            }
            else if (!this.XmlReader.LocalNameEquals(this.ODataFunctionElementName))
            {
                // not an m:function either
                return false;
            }

            ODataOperation operation;
            if (isAction)
            {
                operation = new ODataAction();
                entryState.Entry.AddAction((ODataAction)operation);
            }
            else
            {
                operation = new ODataFunction();
                entryState.Entry.AddFunction((ODataFunction)operation);
            }

            string operationName = this.XmlReader.LocalName; // for error reporting
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
                    string attributeValue = this.XmlReader.Value;
                    if (this.XmlReader.LocalNameEquals(this.ODataOperationMetadataAttribute))
                    {
                        // For metadata, if the URI is relative we don't attempt to make it absolute using the service
                        // base URI, because the ODataOperation metadata URI is relative to $metadata.
                        operation.Metadata = this.ProcessUriFromPayload(attributeValue, this.XmlReader.XmlBaseUri, /*makeAbsolute*/ false);
                    }
                    else if (this.XmlReader.LocalNameEquals(this.ODataOperationTargetAttribute))
                    {
                        operation.Target = this.ProcessUriFromPayload(attributeValue, this.XmlReader.XmlBaseUri);
                    }
                    else if (this.XmlReader.LocalNameEquals(this.ODataOperationTitleAttribute))
                    {
                        operation.Title = this.XmlReader.Value;
                    }

                    // skip unknown attributes
                }
            }

            if (operation.Metadata == null)
            {
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_OperationMissingMetadataAttribute(operationName));
            }

            if (operation.Target == null)
            {
                throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_OperationMissingTargetAttribute(operationName));
            }

            // skip the content of m:action/m:function
            this.XmlReader.Skip();
            return true;
        }

        /// <summary>
        /// Reads an ATOM element inside the atom:feed from the input.
        /// </summary>
        /// <param name="feedState">The reader feed state for the feed being read.</param>
        /// <param name="isExpandedLinkContent">true if the feed is inside an expanded link.</param>
        /// <returns>true if the atom:entry element was found and the reader was not moved;
        /// false otherwise and the reader is positioned on the next node after the ATOM element.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element in ATOM namespace - The element in ATOM namespace to read.
        /// Post-Condition: Any                                   - The node after the ATOM element which was consumed.
        ///                 XmlNodeType.Element atom:entry        - The start of the atom:entry element (the reader did not move in this case).
        /// </remarks>
        private bool ReadAtomElementInFeed(IODataAtomReaderFeedState feedState, bool isExpandedLinkContent)
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
                    if (this.ReadAtomStandardRelationLinkInFeed(feedState, linkRelation, linkHRef, isExpandedLinkContent))
                    {
                        return false;
                    }

                    string unescapedLinkRelation = AtomUtils.UnescapeAtomLinkRelationAttribute(linkRelation);
                    if (unescapedLinkRelation != null)
                    {
                        string ianaRelation = AtomUtils.GetNameFromAtomLinkRelationAttribute(linkRelation, AtomConstants.IanaLinkRelationsNamespace);
                        if (ianaRelation != null && this.ReadAtomStandardRelationLinkInFeed(feedState, ianaRelation, linkHRef, isExpandedLinkContent))
                        {
                            return false;
                        }
                    }
                }

                if (this.ReadAtomMetadata)
                {
                    AtomLinkMetadata linkMetadata = this.FeedMetadataDeserializer.ReadAtomLinkElementInFeed(linkRelation, linkHRef);
                    feedState.AtomFeedMetadata.AddLink(linkMetadata);
                }
                else
                {
                    this.XmlReader.Skip();
                }
            }
            else if (this.XmlReader.LocalNameEquals(this.AtomIdElementName))
            {
                // atom:id
                // The id should be an IRI (so text only), but we already allow much more for entry/id, so we should do the same for feed/id
                // just to be consistent.
                string idValue = this.XmlReader.ReadElementValue();

                feedState.Feed.Id = UriUtils.CreateUriAsEntryOrFeedId(idValue, UriKind.Absolute);
            }
            else
            {
                if (this.ReadAtomMetadata)
                {
                    this.FeedMetadataDeserializer.ReadAtomElementAsFeedMetadata(feedState.AtomFeedMetadata);
                }
                else
                {
                    this.XmlReader.Skip();
                }
            }

            return false;
        }

        /// <summary>
        /// Reads the atom:link element with one of the standard relation values in the atom:feed element.
        /// </summary>
        /// <param name="feedState">The reader feed state for the feed being read.</param>
        /// <param name="linkRelation">The rel attribute value for the link.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <param name="isExpandedLinkContent">true if the feed is inside an expanded link.</param>
        /// <returns>If the rel was one of the recognized standard relations and this method read the link
        /// the return value is true. Otherwise the method doesn't move the reader and returns false.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element atom:link  - The atom:link element to read.
        /// Post-Condition:  Any                            - The node after the atom:link element if the link was read by this method.
        ///                  XmlNodeType.Element atom:link  - The atom:link element to read if the link was not read by this method.
        /// </remarks>
        private bool ReadAtomStandardRelationLinkInFeed(IODataAtomReaderFeedState feedState, string linkRelation, string linkHRef, bool isExpandedLinkContent)
        {
            Debug.Assert(feedState != null, "feedState != null");
            Debug.Assert(linkRelation != null, "linkRelation != null");
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "The XML reader must be on the atom:link element for this method to work.");

            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomNextRelationAttributeValue) == 0)
            {
                if (!this.ReadingResponse)
                {
                    // Return false which means the rel for the link was not recognized and it will be read as any other link.
                    return false;
                }

                if (feedState.HasNextPageLink)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed(AtomConstants.AtomNextRelationAttributeValue));
                }

                // next link
                if (linkHRef != null)
                {
                    feedState.Feed.NextPageLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                feedState.HasNextPageLink = true;

                this.ReadLinkMetadataIfRequired(linkRelation, linkHRef, (linkMetadata) => { feedState.AtomFeedMetadata.NextPageLink = linkMetadata; });

                return true;
            }
            
            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomSelfRelationAttributeValue) == 0)
            {
                if (feedState.HasReadLink && this.AtomInputContext.UseDefaultFormatBehavior)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed(AtomConstants.AtomSelfRelationAttributeValue));
                }

                this.ReadLinkMetadataIfRequired(linkRelation, linkHRef, (linkMetadata) => { feedState.AtomFeedMetadata.SelfLink = linkMetadata; });

                feedState.HasReadLink = true;

                return true;
            }

            if (string.CompareOrdinal(linkRelation, AtomConstants.AtomDeltaRelationAttributeValue) == 0)
            {
                if (!this.ReadingResponse)
                {
                    // Return false which means the rel for the link was not recognized and it will be read as any other link.
                    return false;
                }

                if (feedState.HasDeltaLink)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed(AtomConstants.AtomDeltaRelationAttributeValue));
                }

                if (isExpandedLinkContent)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_EncounteredDeltaLinkInNestedFeed);
                }

                if (linkHRef != null)
                {
                    feedState.Feed.DeltaLink = this.ProcessUriFromPayload(linkHRef, this.XmlReader.XmlBaseUri);
                }

                this.ReadLinkMetadataIfRequired(linkRelation, linkHRef, (linkMetadata) => feedState.AtomFeedMetadata.AddLink(linkMetadata));

                feedState.HasDeltaLink = true;
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Reads the Atom metadata for the link if metadata is required to be read, skip it otherwise.
        /// </summary>
        /// <param name="linkRelation">The rel attribute value for the link.</param>
        /// <param name="linkHRef">The href attribute value for the link (or null if the href attribute was not present).</param>
        /// <param name="setFeedLink">Action for adding AtomLinkMetadata to the AtomFeedMetadata</param>
        private void ReadLinkMetadataIfRequired(string linkRelation, string linkHRef, Action<AtomLinkMetadata> setFeedLink)
        {
            if (this.ReadAtomMetadata)
            {
                // Capture the ATOM metadata of the link.
                AtomLinkMetadata linkMetadata = this.FeedMetadataDeserializer.ReadAtomLinkElementInFeed(linkRelation, linkHRef);
                setFeedLink(linkMetadata);
            }
            else
            {
                this.XmlReader.Skip();
            }
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

                        // Skip all other elements
                        this.XmlReader.Skip();
                        break;

                    default:
                        // Skip all other nodes
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

                            throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_UnknownElementInInline(this.XmlReader.LocalName));
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

        /// <summary>
        /// Verifies that the specified content type of an atom:content element matches the expected value.
        /// </summary>
        /// <param name="contentType">The content type value read from the payload.</param>
        /// <returns>The verified media type name (without parameters or charset) of the content type.</returns>
        private string VerifyAtomContentMediaType(string contentType)
        {
            // Fast path: compare strings before parsing the MIME type (which is costly)
            if (!HttpUtils.CompareMediaTypeNames(MimeConstants.MimeApplicationXml, contentType) &&
                !HttpUtils.CompareMediaTypeNames(MimeConstants.MimeApplicationAtomXml, contentType))
            {
                // If the content is neither null nor empty, then the only allowed media type are application/xml and application/atom+xml.
                // WCF DS client and server will throw if the media type is not parsable.
                // WCF DS server throws if the media type is neither application/xml nor application/atom+xml.
                string mediaType, mediaTypeCharset;
                HttpUtils.ReadMimeType(contentType, out mediaType, out mediaTypeCharset);
                if (!(HttpUtils.CompareMediaTypeNames(mediaType, MimeConstants.MimeApplicationXml) || HttpUtils.CompareMediaTypeNames(mediaType, MimeConstants.MimeApplicationAtomXml)))
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_ContentWithWrongType(mediaType));
                }
            }

            return contentType;
        }

        /// <summary>
        /// Verifies if the input id is a transient id
        /// </summary>
        /// <param name="id">entry id</param>
        /// <returns>if the entry id is a transient id</returns>
        private static bool IsTransientId(string id)
        {
            Match isTransient = transientIdRegex.Match(id);
            return isTransient.Success;
        }
    }
}
