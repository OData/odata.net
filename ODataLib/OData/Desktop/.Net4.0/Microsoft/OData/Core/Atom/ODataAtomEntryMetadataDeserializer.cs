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
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for ATOM metadata on entries.
    /// </summary>
    internal sealed class ODataAtomEntryMetadataDeserializer : ODataAtomMetadataDeserializer
    {
        #region Atomized strings
        /// <summary>The empty namespace used for attributes in no namespace.</summary>
        private readonly string EmptyNamespace;

        /// <summary>Schema namespace for Atom.</summary>
        private readonly string AtomNamespace;
        #endregion

        /// <summary>
        /// Feed ATOM metadata deserializer for deserializing the atom:source element in an entry.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomFeedMetadataDeserializer sourceMetadataDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomEntryMetadataDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.EmptyNamespace = nameTable.Add(string.Empty);
            this.AtomNamespace = nameTable.Add(AtomConstants.AtomNamespace);
        }

        /// <summary>
        /// Feed ATOM metadata deserializer for deserializing the atom:source element in an entry.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomFeedMetadataDeserializer SourceMetadataDeserializer
        {
            get
            {
                return this.sourceMetadataDeserializer ??
                       (this.sourceMetadataDeserializer = new ODataAtomFeedMetadataDeserializer(this.AtomInputContext, true));
            }
        }

        /// <summary>
        /// Reads an element in ATOM namespace in the content of the entry element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:*) - the ATOM element to read.
        /// Post-Condition: Any                          - the node after the ATOM element which was read.
        /// </remarks>
        internal void ReadAtomElementInEntryContent(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "Only atom:* elements can be read by this method.");

            if (this.ReadAtomMetadata)
            {
                switch (this.XmlReader.LocalName)
                {
                    case AtomConstants.AtomAuthorElementName:
                        this.ReadAuthorElement(entryState);
                        return;
                    case AtomConstants.AtomContributorElementName:
                        this.ReadContributorElement(entryState);
                        return;
                    case AtomConstants.AtomUpdatedElementName:
                    {
                        AtomEntryMetadata entryMetadata = entryState.AtomEntryMetadata;
                        if (this.ShouldReadSingletonElement(entryMetadata.Updated.HasValue))
                        {
                            entryMetadata.Updated = this.ReadAtomDateConstruct();
                            return;
                        }
                    }

                        break;
                    case AtomConstants.AtomPublishedElementName:
                    {
                        AtomEntryMetadata entryMetadata = entryState.AtomEntryMetadata;
                        if (this.ShouldReadSingletonElement(entryMetadata.Published.HasValue))
                        {
                            entryMetadata.Published = this.ReadAtomDateConstruct();
                            return;
                        }
                    }

                        break;
                    case AtomConstants.AtomRightsElementName:
                        if (this.ShouldReadSingletonElement(entryState.AtomEntryMetadata.Rights != null))
                        {
                            entryState.AtomEntryMetadata.Rights = this.ReadAtomTextConstruct();
                            return;
                        }

                        break;
                    case AtomConstants.AtomSourceElementName:
                        if (this.ShouldReadSingletonElement(entryState.AtomEntryMetadata.Source != null))
                        {
                            entryState.AtomEntryMetadata.Source = this.ReadAtomSourceInEntryContent();
                            return;
                        }

                        break;
                    case AtomConstants.AtomSummaryElementName:
                        if (this.ShouldReadSingletonElement(entryState.AtomEntryMetadata.Summary != null))
                        {
                            entryState.AtomEntryMetadata.Summary = this.ReadAtomTextConstruct();
                            return;
                        }

                        break;
                    case AtomConstants.AtomTitleElementName:
                        if (this.ShouldReadSingletonElement(entryState.AtomEntryMetadata.Title != null))
                        {
                            entryState.AtomEntryMetadata.Title = this.ReadAtomTextConstruct();
                            return;
                        }

                        break;
                    default:
                        break;
                }
            }

            // Skip everything we didn't read.
            this.XmlReader.Skip();
        }

        /// <summary>
        /// Reads the atom:link element in the entry content.
        /// </summary>
        /// <param name="relation">The value of the rel attribute for the link element.</param>
        /// <param name="hrefStringValue">The value of the href attribute for the link element.</param>
        /// <returns>An <see cref="AtomLinkMetadata"/> instance storing the information about this link, or null if link info doesn't need to be stored.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:link) - the atom:link element to read.
        /// Post-Condition: XmlNodeType.Element (atom:link) - the atom:link element which was read.
        /// </remarks>
        internal AtomLinkMetadata ReadAtomLinkElementInEntryContent(string relation, string hrefStringValue)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "Only atom:link element can be read by this method.");

            AtomLinkMetadata linkMetadata = null;

            if (this.ReadAtomMetadata)
            {
                linkMetadata = new AtomLinkMetadata();
                linkMetadata.Relation = relation;
                if (this.ReadAtomMetadata)
                {
                    linkMetadata.Href = hrefStringValue == null ? null : this.ProcessUriFromPayload(hrefStringValue, this.XmlReader.XmlBaseUri);
                }

                // Read the attributes
                while (this.XmlReader.MoveToNextAttribute())
                {
                    if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                    {
                        // Note that it's OK to store values which we don't validate in any way even if we might not need them.
                        // The EPM reader will ignore them if they're not needed and the fact that we don't validate them means that there are no observable differences
                        // if we store them. It keeps the code simpler (less ifs).
                        switch (this.XmlReader.LocalName)
                        {
                            case AtomConstants.AtomLinkTypeAttributeName:
                                linkMetadata.MediaType = this.XmlReader.Value;
                                break;
                            case AtomConstants.AtomLinkHrefLangAttributeName:
                                linkMetadata.HrefLang = this.XmlReader.Value;
                                break;
                            case AtomConstants.AtomLinkTitleAttributeName:
                                linkMetadata.Title = this.XmlReader.Value;
                                break;
                            case AtomConstants.AtomLinkLengthAttributeName:
                                // We must NOT try to parse the value into a number if we don't need it for ATOM metadata.
                                if (this.ReadAtomMetadata)
                                {
                                    string lengthStringValue = this.XmlReader.Value;
                                    int length;
                                    if (int.TryParse(lengthStringValue, NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out length))
                                    {
                                        linkMetadata.Length = length;
                                    }
                                    else
                                    {
                                        throw new ODataException(Strings.ODataAtomEntryMetadataDeserializer_InvalidLinkLengthValue(lengthStringValue));
                                    }
                                }

                                break;
                            default:
                                // Ignore all other attributes.
                                break;
                        }
                    }
                }
            }

            this.XmlReader.MoveToElement();

            return linkMetadata;
        }

        /// <summary>
        /// Reads the atom:category element in the entry content.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:category) - the atom:category element to read.
        /// Post-Condition: Any                                 - the node after the atom:category which was read.
        /// </remarks>
        internal void ReadAtomCategoryElementInEntryContent(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomCategoryElementName,
                "Only atom:category element can be read by this method.");

            // Read the attributes and create the category metadata regardless if we will need it or not.
            // We can do this since there's no validation done on any of the values and thus this operation will never fail.
            // If we then decide we don't need it, we can safely throw it away.
            if (this.ReadAtomMetadata)
            {
                AtomCategoryMetadata categoryMetadata = this.ReadAtomCategoryElement();

                // No point in trying to figure out if we will need the category for EPM or not here.
                // Our EPM syndication reader must handle unneeded categories anyway (if ATOM metadata reading is on)
                // So instead of burning the cycles to compute if we need it, just store it anyway.
                entryState.AtomEntryMetadata.AddCategory(categoryMetadata);
            }
            else
            {
                // Skip the element in any case (we only ever consume attributes on it anyway).
                this.XmlReader.Skip();
            }
        }

        /// <summary>
        /// Reads the atom:category element.
        /// </summary>
        /// <returns>The ATOM category metadata read.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:category) - the atom:category element to read.
        /// Post-Condition: Any                                 - the node after the atom:category which was read.
        /// </remarks>
        internal AtomCategoryMetadata ReadAtomCategoryElement()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomCategoryElementName,
                "Only atom:category element can be read by this method.");

            AtomCategoryMetadata categoryMetadata = new AtomCategoryMetadata();

            // Read the attributes
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
                    switch (this.XmlReader.LocalName)
                    {
                        case AtomConstants.AtomCategorySchemeAttributeName:
                            categoryMetadata.Scheme = categoryMetadata.Scheme ?? this.XmlReader.Value;
                            break;
                        case AtomConstants.AtomCategoryTermAttributeName:
                            categoryMetadata.Term = categoryMetadata.Term ?? this.XmlReader.Value;
                            break;
                        case AtomConstants.AtomCategoryLabelAttributeName:
                            categoryMetadata.Label = this.XmlReader.Value;
                            break;
                        default:
                            // Ignore all other attributes.
                            break;
                    }
                }
            }

            // Skip the element in any case (we only ever consume attributes on it anyway).
            this.XmlReader.Skip();

            return categoryMetadata;
        }

        /// <summary>
        /// Reads the atom:source element in the entry content.
        /// </summary>
        /// <returns>The information in the source element as <see cref="AtomFeedMetadata"/>.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:source) - the atom:source element to read.
        /// Post-Condition: Any                               - the node after the atom:source which was read.
        /// </remarks>
        internal AtomFeedMetadata ReadAtomSourceInEntryContent()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomSourceElementName,
                "Only atom:source element can be read by this method.");

            AtomFeedMetadata atomFeedMetadata = AtomMetadataReaderUtils.CreateNewAtomFeedMetadata();

            if (this.XmlReader.IsEmptyElement)
            {
                // Advance reader past this element.
                this.XmlReader.Read();
                return atomFeedMetadata;
            }

            // Read the start tag of the source element.
            this.XmlReader.Read();

            while (this.XmlReader.NodeType != XmlNodeType.EndElement)
            {
                if (this.XmlReader.NodeType != XmlNodeType.Element)
                {
                    Debug.Assert(this.XmlReader.NodeType != XmlNodeType.EndElement, "EndElement should have been handled already.");

                    // Skip everything but elements, including insignificant nodes, text nodes and CDATA nodes.
                    this.XmlReader.Skip();
                    continue;
                }

                if (this.XmlReader.NamespaceEquals(this.AtomNamespace))
                {
                    // Use a feed metadata deserializer to process this element and modify atomFeedMetadata appropriately.
                    this.SourceMetadataDeserializer.ReadAtomElementAsFeedMetadata(atomFeedMetadata);
                }
                else
                {
                    // Skip all elements not in the ATOM namespace.
                    this.XmlReader.Skip();
                }
            }

            // Advance the reader past the end tag of the source element.
            this.XmlReader.Read();

            return atomFeedMetadata;
        }

        /// <summary>
        /// Reads an author element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:author) - the atom:author element to read.
        /// Post-Condition: Any                               - the node after the atom:author element which was read.
        /// </remarks>
        private void ReadAuthorElement(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomAuthorElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:author elements can be read by this method.");

            if (this.ShouldReadCollectionElement(entryState.AtomEntryMetadata.Authors.Any()))
            {
                entryState.AtomEntryMetadata.AddAuthor(this.ReadAtomPersonConstruct());
            }
            else
            {
                // Skip the element as we don't care about it
                this.XmlReader.Skip();
            }
        }

        /// <summary>
        /// Reads a contributor element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:contributor) - the atom:contributor element to read.
        /// Post-Condition: Any                                    - the node after the atom:contributor element which was read.
        /// </remarks>
        private void ReadContributorElement(IODataAtomReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomContributorElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:contributor elements can be read by this method.");

            if (this.ShouldReadCollectionElement(entryState.AtomEntryMetadata.Contributors.Any()))
            {
                entryState.AtomEntryMetadata.AddContributor(this.ReadAtomPersonConstruct());
            }
            else
            {
                this.XmlReader.Skip();
            }
        }

        /// <summary>
        /// Determines if a person element should be read or skipped.
        /// </summary>
        /// <param name="someAlreadyExist">true if some elements from the collection in question already exist; false if this is the first one.</param>
        /// <returns>true if the collection element should be read; false if it should be skipped.</returns>
        private bool ShouldReadCollectionElement(bool someAlreadyExist)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace &&
                (this.XmlReader.LocalName == AtomConstants.AtomAuthorElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomContributorElementName),
                "This method should only be called if the reader is on an element which can appear in ATOM entry multiple times.");

            // Only read multiple author/contributor elements if ATOM metadata reading is on.
            return this.ReadAtomMetadata || !someAlreadyExist;
        }

        /// <summary>
        /// Determines if we should read an element which is allowed to appear only once in ATOM.
        /// </summary>
        /// <param name="alreadyExists">true if we already found such element before; false if this is the first occurence.</param>
        /// <returns>true if the element should be processed; false if the element should be skipped.</returns>
        /// <remarks>The method may throw if multiple occurences of such element occure and they should be treated as an error.</remarks>
        private bool ShouldReadSingletonElement(bool alreadyExists)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace &&
                (this.XmlReader.LocalName == AtomConstants.AtomRightsElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomSummaryElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomTitleElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomPublishedElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomUpdatedElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomSourceElementName),
                "This method should only be called if the reader is on an element which can appear in ATOM entry just once.");

            if (alreadyExists)
            {
                if (this.ReadAtomMetadata || this.AtomInputContext.UseDefaultFormatBehavior)
                {
                    // We should not allow multiple elements per the ATOM spec, when we're reading ATOM metadata.
                    // The default ODataLib behavior is also to disallow duplicates. EPM behavior is also the same.
                    throw new ODataException(Strings.ODataAtomMetadataDeserializer_MultipleSingletonMetadataElements(this.XmlReader.LocalName, AtomConstants.AtomEntryElementName));
                }

                // Otherwise we're reading this only for EPM in WCF DS Client of Server mode,
                // in which case any additional elements like this should be skipped.
                return false;
            }

            return true;
        }
    }
}
