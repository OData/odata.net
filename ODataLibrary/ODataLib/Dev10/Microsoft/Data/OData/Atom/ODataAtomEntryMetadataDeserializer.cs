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
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for ATOM metadata on entries.
    /// </summary>
    internal sealed class ODataAtomEntryMetadataDeserializer : ODataAtomEpmDeserializer
    {
        #region Atomized strings
        /// <summary>The empty namespace used for attributes in no namespace.</summary>
        private readonly string EmptyNamespace;

        /// <summary>Schema namespace for Atom.</summary>
        private readonly string AtomNamespace;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomEntryMetadataDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            DebugUtils.CheckNoExternalCallers();

            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.EmptyNamespace = nameTable.Add(string.Empty);
            this.AtomNamespace = nameTable.Add(AtomConstants.AtomNamespace);
        }

        /// <summary>
        /// Flag indicating if ATOM metadata is required to be read by the user.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Not yet fully implemented")]
        private bool ReadAtomMetadata
        {
            get
            {
                return false;
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "Only atom:* elements can be read by this method.");

            ODataEntityPropertyMappingCache cachedEpm = entryState.CachedEpm;
            EpmTargetPathSegment epmTargetPathSegment = null;
            if (cachedEpm != null)
            {
                epmTargetPathSegment = cachedEpm.EpmTargetTree.SyndicationRoot;
            }

            EpmTargetPathSegment subSegment;
            if (this.ShouldReadElement(epmTargetPathSegment, this.XmlReader.LocalName, out subSegment))
            {
                switch (this.XmlReader.LocalName)
                {
                    case AtomConstants.AtomAuthorElementName:
                        this.ReadAuthorElement(entryState, subSegment);
                        return;
                    case AtomConstants.AtomContributorElementName:
                        this.ReadContributorElement(entryState, subSegment);
                        return;
                    case AtomConstants.AtomUpdatedElementName:
                        if (this.ShouldReadSingletonElement(entryState.AtomEntryMetadata.Updated.HasValue))
                        {
                            entryState.AtomEntryMetadata.Updated = this.ReadElementDateTimeOffsetValue();
                            return;
                        }

                        break;
                    case AtomConstants.AtomPublishedElementName:
                        if (this.ShouldReadSingletonElement(entryState.AtomEntryMetadata.Published.HasValue))
                        {
                            entryState.AtomEntryMetadata.Published = this.ReadElementDateTimeOffsetValue();
                            return;
                        }

                        break;
                    case AtomConstants.AtomRightsElementName:
                        if (this.ShouldReadSingletonElement(entryState.AtomEntryMetadata.Rights != null))
                        {
                            entryState.AtomEntryMetadata.Rights = this.ReadAtomTextConstruct();
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
                        // TODO: Implement EPM and ATOM metadata reading.
                        break;
                }
            }

            // Skip everything we didn't read.
            this.XmlReader.Skip();
        }

        /// <summary>
        /// Reads the atom:link element in the entry content.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="relation">The value of the rel attribute for the link element.</param>
        /// <param name="hrefStringValue">The value of the href attribute for the link element.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:link) - the atom:link element to read.
        /// Post-Condition: Any                             - the node after the atom:link which was read.
        /// </remarks>
        internal void ReadAtomLinkElementInEntryContent(IODataAtomReaderEntryState entryState, string relation, string hrefStringValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "Only atom:link element can be read by this method.");

            ODataEntityPropertyMappingCache cachedEpm = entryState.CachedEpm;
            EpmTargetPathSegment subSegment = GetLinkTargetSegment(cachedEpm != null ? cachedEpm.EpmTargetTree.SyndicationRoot : null, relation);
            if (this.ReadAtomMetadata || subSegment != null)
            {
                AtomLinkMetadata linkMetadata = new AtomLinkMetadata();
                linkMetadata.Relation = relation;
                if (this.ReadAtomMetadata)
                {
                    linkMetadata.Href = hrefStringValue == null ? null : this.ProcessUriFromPayload(hrefStringValue, this.XmlReader.XmlBaseUri);
                }

                // Note that href must always be mapped, so no need to check.
                if (subSegment != null)
                {
                    linkMetadata.HrefFromEpm = hrefStringValue;
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
                                // We must NOT try to parse the value into a number if we don't need it (either ATOM metadata or EPM)
                                if (this.ReadAtomMetadata || (subSegment != null &&
                                    subSegment.SubSegments.Any(segment => string.CompareOrdinal(segment.AttributeName, AtomConstants.AtomLinkLengthAttributeName) == 0)))
                                {
                                    string lengthStringValue = this.XmlReader.Value;
                                    int length;
                                    if (int.TryParse(lengthStringValue, NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out length))
                                    {
                                        linkMetadata.Length = length;
                                    }
                                    else
                                    {
                                        throw new ODataException(Strings.EpmSyndicationWriter_InvalidLinkLengthValue(lengthStringValue));
                                    }
                                }

                                break;
                            default:
                                // Ignore all other attributes.
                                break;
                        }
                    }
                }

                // TODO: We might want to store the subSegment on the linkMetadata so that later when we apply EPM the search would be faster.
                AtomMetadataReaderUtils.AddLinkToEntryMetadata(entryState.AtomEntryMetadata, linkMetadata);
            }

            // Skip the element in any case (we only ever consume attributes on it anyway).
            this.XmlReader.Skip();
        }

        /// <summary>
        /// Reads the atom:category element in the entry content.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:category) - the atom:category element to read.
        /// Post-Condition: Any                                 - the node after the atom:link which was read.
        /// </remarks>
        internal void ReadAtomCategoryElementInEntryContent(IODataAtomReaderEntryState entryState)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomCategoryElementName,
                "Only atom:category element can be read by this method.");

            ODataEntityPropertyMappingCache cachedEpm = entryState.CachedEpm;
            EpmTargetPathSegment epmTargetPathSegment = null;
            if (cachedEpm != null)
            {
                epmTargetPathSegment = cachedEpm.EpmTargetTree.SyndicationRoot;
            }

            // Rough estimate if we will need the category for EPM - we can't tell for sure since we don't know the scheme value yet.
            bool hasCategoryEpm = epmTargetPathSegment != null && epmTargetPathSegment.SubSegments.Any(segment =>
                string.CompareOrdinal(segment.SegmentName, AtomConstants.AtomCategoryElementName) == 0);

            // Read the attributes and create the category metadata regardless if we will need it or not.
            // We can do this since there's no validation done on any of the values and thus this operation will never fail.
            // If we then decide we don't need it, we can safely throw it away.
            if (this.ReadAtomMetadata || hasCategoryEpm)
            {
                AtomCategoryMetadata categoryMetadata = new AtomCategoryMetadata();

                // Read the attributes
                while (this.XmlReader.MoveToNextAttribute())
                {
                    if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                    {
                        switch (this.XmlReader.LocalName)
                        {
                            case AtomConstants.AtomCategorySchemeAttributeName:
                                categoryMetadata.Scheme = this.XmlReader.Value;
                                break;
                            case AtomConstants.AtomCategoryTermAttributeName:
                                categoryMetadata.Term = this.XmlReader.Value;
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

                // No point in trying to figure out if we will need the category for EPM or not here.
                // Our EPM syndication reader must handle unneeded categories anyway (if ATOM metadata reading is on)
                // So instead of burning the cycles to compute if we need it, just store it anyway.
                AtomMetadataReaderUtils.AddCategoryToEntryMetadata(entryState.AtomEntryMetadata, categoryMetadata);
            }

            // Skip the element in any case (we only ever consume attributes on it anyway).
            this.XmlReader.Skip();
        }

        /// <summary>
        /// Gets the EPM target path segment for a link with a given relation.
        /// </summary>
        /// <param name="epmSyndicationRootSegment">The syndication EPM tree root segment (or null if no EPM is available).</param>
        /// <param name="relation">The relation value for the link in question.</param>
        /// <returns>The EPM target path segment for the link, or null if none is available and the link doesn't participate in EPM.</returns>
        private static EpmTargetPathSegment GetLinkTargetSegment(EpmTargetPathSegment epmSyndicationRootSegment, string relation)
        {
            EpmTargetPathSegment subSegment = null;
            if (epmSyndicationRootSegment != null)
            {
                // If the relation is not an allowed criteria value then it can't be mapped through EPM
                if (EntityPropertyMappingInfo.IsValidLinkRelCriteriaValue(relation))
                {
                    IEnumerable<EpmTargetPathSegment> linkTargetPathSegments = epmSyndicationRootSegment.SubSegments.Where(segment =>
                        string.CompareOrdinal(segment.SegmentName, AtomConstants.AtomLinkElementName) == 0);

                    // First try to find an exact match for the criteria value
                    subSegment = linkTargetPathSegments.FirstOrDefault(segment => segment.CriteriaValue != null &&
                        string.Compare(segment.CriteriaValue, relation, StringComparison.OrdinalIgnoreCase) == 0);

                    if (subSegment == null)
                    {
                        // If we didn't find an exact match see if we have an uncoditional link mapping
                        subSegment = linkTargetPathSegments.FirstOrDefault(segment => segment.CriteriaValue == null);
                    }
                }
            }

            return subSegment;
        }

        /// <summary>
        /// Reads an author element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="epmTargetPathSegment">The EPM target path segment for the element to read, or null if no EPM for that element is defined.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:author) - the atom:author element to read.
        /// Post-Condition: Any                               - the node after the atom:author element which was read.
        /// </remarks>
        private void ReadAuthorElement(IODataAtomReaderEntryState entryState, EpmTargetPathSegment epmTargetPathSegment)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomAuthorElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:author elements can be read by this method.");

            if (this.ShouldReadCollectionElement(entryState.AtomEntryMetadata.Authors.Any(), epmTargetPathSegment))
            {
                AtomMetadataReaderUtils.AddAuthorToEntryMetadata(
                    entryState.AtomEntryMetadata,
                    this.ReadPersonElement(epmTargetPathSegment));
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
        /// <param name="epmTargetPathSegment">The EPM target path segment for the element to read, or null if no EPM for that element is defined.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:contributor) - the atom:contributor element to read.
        /// Post-Condition: Any                                    - the node after the atom:contributor element which was read.
        /// </remarks>
        private void ReadContributorElement(IODataAtomReaderEntryState entryState, EpmTargetPathSegment epmTargetPathSegment)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomContributorElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:contributor elements can be read by this method.");

            if (this.ShouldReadCollectionElement(entryState.AtomEntryMetadata.Contributors.Any(), epmTargetPathSegment))
            {
                AtomMetadataReaderUtils.AddContributorToEntryMetadata(
                    entryState.AtomEntryMetadata,
                    this.ReadPersonElement(epmTargetPathSegment));
            }
            else
            {
                this.XmlReader.Skip();
            }
        }

        /// <summary>
        /// Reads a person (author/contributor) element.
        /// </summary>
        /// <param name="epmTargetPathSegment">The EPM target path segment for the element to read, or null if no EPM for that element is defined.</param>
        /// <returns>The person metadata object with the read values.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:contributor/atom:author) - the atom:author/atom:contributor element to read.
        /// Post-Condition: Any                                                - the node after the atom:author/atom:contributor element which was read.
        /// </remarks>
        private AtomPersonMetadata ReadPersonElement(EpmTargetPathSegment epmTargetPathSegment)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                (this.XmlReader.LocalName == AtomConstants.AtomAuthorElementName || this.XmlReader.LocalName == AtomConstants.AtomContributorElementName) && 
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:author or atom:contributor elements can be read by this method.");

            EpmTargetPathSegment subSegment;
            AtomPersonMetadata personMetadata = new AtomPersonMetadata();

            if (!this.XmlReader.IsEmptyElement)
            {
                // Move to the first child
                this.XmlReader.Read();

                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            // End of the atom:author/atom:contributor element
                            continue;
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.AtomNamespace))
                            {
                                if (this.ShouldReadElement(epmTargetPathSegment, this.XmlReader.LocalName, out subSegment))
                                {
                                    switch (this.XmlReader.LocalName)
                                    {
                                        case AtomConstants.AtomPersonNameElementName:
                                            personMetadata.Name = this.ReadElementStringValue();
                                            continue;
                                        case AtomConstants.AtomPersonUriElementName:
                                            // NOTE: get the base URI here before we read the content as string; reading the content as string will move the 
                                            //       reader to the end element and thus we lose the xml:base definition on the element.
                                            Uri xmlBaseUri = this.XmlReader.XmlBaseUri;
                                            string textValue = this.ReadElementStringValue();
                                            if (subSegment != null)
                                            {
                                                personMetadata.UriFromEpm = textValue;
                                            }

                                            if (this.ReadAtomMetadata)
                                            {
                                                personMetadata.Uri = this.ProcessUriFromPayload(textValue, xmlBaseUri);
                                            }

                                            continue;
                                        case AtomConstants.AtomPersonEmailElementName:
                                            personMetadata.Email = this.ReadElementStringValue();
                                            continue;
                                        default:
                                            break;
                                    }
                                }
                            }

                            break;
                        default:
                            break;
                    }

                    // Skip everything we haven't read yet.
                    this.XmlReader.Skip();
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            }

            // Read over the end element or the empty start element.
            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(
                (this.XmlReader.LocalName == AtomConstants.AtomAuthorElementName || this.XmlReader.LocalName == AtomConstants.AtomContributorElementName) &&
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:author or atom:contributor elements can be read by this method.");
            this.XmlReader.Read();

            return personMetadata;
        }

        /// <summary>
        /// Read the ATOM text construct element.
        /// </summary>
        /// <returns>The element read represented as ATOM text construct.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the element to read.
        /// Post-Condition: Any                 - the node after the element.
        /// </remarks>
        private AtomTextConstruct ReadAtomTextConstruct()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "The element must be in ATOM namespace for this method work.");
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomRightsElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomSummaryElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomTitleElementName,
                "Only atom:rights, atom:summary and atom:title elements are supported by this method.");

            AtomTextConstruct textConstruct = new AtomTextConstruct();

            string typeValue = null;
            bool hasMetadataNullAttributeWithTrueValue = false;
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace) && string.CompareOrdinal(this.XmlReader.LocalName, AtomConstants.AtomTypeAttributeName) == 0)
                {
                    // type attribute
                    typeValue = this.XmlReader.Value;
                }
                else
                {
                    this.TryReadMetadataNullAttribute(out hasMetadataNullAttributeWithTrueValue);
                }
            }

            this.XmlReader.MoveToElement();
            if (typeValue == null)
            {
                textConstruct.Kind = AtomTextConstructKind.Text;
            }
            else
            {
                switch (typeValue)
                {
                    case AtomConstants.AtomTextConstructTextKind:
                        textConstruct.Kind = AtomTextConstructKind.Text;
                        break;
                    case AtomConstants.AtomTextConstructHtmlKind:
                        textConstruct.Kind = AtomTextConstructKind.Html;
                        break;
                    case AtomConstants.AtomTextConstructXHtmlKind:
                        textConstruct.Kind = AtomTextConstructKind.Xhtml;
                        break;
                    default:
                        throw new ODataException(Strings.ODataAtomEntryMetadataDeserializer_InvalidTextConstructKind(typeValue, this.XmlReader.LocalName));
                }
            }

            textConstruct.HasMetadataNullAttribute = hasMetadataNullAttributeWithTrueValue;

            // If we are reading just EPM (not ATOM metadata) and there's m:null on the element
            // do NOT read the content of the element at all (it might fail), just skip over it.
            if (!this.ReadAtomMetadata && hasMetadataNullAttributeWithTrueValue)
            {
                // The m:null attribute has a precedence over the content of the element, thus if we find m:null='true' we ignore the content of the element.
                this.XmlReader.Skip();
            }
            else
            {
                if (textConstruct.Kind == AtomTextConstructKind.Xhtml)
                {
                    this.XmlReader.AssertNotBuffering();
                    textConstruct.Text = this.XmlReader.ReadInnerXml();
                }
                else
                {
                    textConstruct.Text = this.ReadElementStringValue();
                }
            }

            return textConstruct;
        }

        /// <summary>
        /// Reads the element value as DateTimeOffset value.
        /// </summary>
        /// <returns>The DateTimeOffset value of the element.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the element which value to read.
        /// Post-Condition: Any                 - the node after the element.
        /// </remarks>
        private DateTimeOffset? ReadElementDateTimeOffsetValue()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "The element must be in ATOM namespace for this method work.");
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomUpdatedElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomPublishedElementName,
                "Only atom:updated and atom:published elements are supported by this method.");

            string stringValue = this.ReadElementStringValue();

            // The following algorithm is a copy of the algorithm in Syndication API used for parsing the DateTimeOffset
            // The original algorithm failed if the string was shorter than 20 characters and if both of the
            // DateTimeOffset.TryParseExact calls failed. This version in all those cases tried the XmlConvert.ToDateTimeOffset
            // so that we get compatibility with the XSD spec as well (it's relaxing change).
            // We need to use this algorithm instead of simple XmlConvert.ToDateTimeOffset since WCF DS server used this
            // and it seems more allowing in certain cases.
            stringValue = stringValue.Trim();
            if (stringValue.Length >= 20)
            {
                if (stringValue[19] == '.')
                {
                    int startIndex = 20;
                    while ((stringValue.Length > startIndex) && char.IsDigit(stringValue[startIndex]))
                    {
                        startIndex++;
                    }

                    stringValue = stringValue.Substring(0, 19) + stringValue.Substring(startIndex);
                }

                DateTimeOffset result;
                if (DateTimeOffset.TryParseExact(
                    stringValue,
                    "yyyy-MM-ddTHH:mm:sszzz",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.None,
                    out result))
                {
                    return result;
                }

                if (DateTimeOffset.TryParseExact(
                    stringValue,
                    "yyyy-MM-ddTHH:mm:ssZ",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out result))
                {
                    return result;
                }
            }
            
            // As a fallback use XmlConvert.ToDateTimeOffset (Note that Syndication API would actually fails if it got here).
            // This is what ATOM RFC specifies, the value should conform to xsd:dateTime, which is exactly what the below method parses.
            return XmlConvert.ToDateTimeOffset(stringValue);
        }

        /// <summary>
        /// Reads the value of the current XML element, as per syndication EPM rules and returns it as a string.
        /// </summary>
        /// <returns>The string value read.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the element which value to read.
        /// Post-Condition: Any                 - the node after the element.
        /// </remarks>
        private string ReadElementStringValue()
        {
            this.AssertXmlCondition(XmlNodeType.Element);

            return this.XmlReader.ReadElementValue();
        }

        /// <summary>
        /// Determines if we need to read a child element (either for EPM or for ATOM metadata).
        /// </summary>
        /// <param name="parentSegment">The parent EPM target path segment.</param>
        /// <param name="segmentName">The name of the element/segment to read.</param>
        /// <param name="subSegment">The EPM target path subsegment which describes the element, or null if there's none.</param>
        /// <returns>true if the subelement should be read, false otherwise.</returns>
        private bool ShouldReadElement(EpmTargetPathSegment parentSegment, string segmentName, out EpmTargetPathSegment subSegment)
        {
            if (parentSegment != null)
            {
                subSegment = parentSegment.SubSegments.FirstOrDefault(segment => string.CompareOrdinal(segment.SegmentName, segmentName) == 0);
                Debug.Assert(
                    subSegment == null || subSegment.SegmentNamespaceUri == AtomConstants.AtomNamespace,
                    "Only elements from ATOM namespace should appear in the syndication mappings.");
            }
            else
            {
                subSegment = null;
            }

            return (subSegment != null) || this.ReadAtomMetadata;
        }

        /// <summary>
        /// Determines if a person element should be read or skipped.
        /// </summary>
        /// <param name="someAlreadyExist">true if some elements from the collection in question already exist; false if this is the first one.</param>
        /// <param name="epmTargetPathSegment">The EPM target segment for the collection element.</param>
        /// <returns>true if the collection element should be read; false if it should be skipped.</returns>
        private bool ShouldReadCollectionElement(bool someAlreadyExist, EpmTargetPathSegment epmTargetPathSegment)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace &&
                (this.XmlReader.LocalName == AtomConstants.AtomAuthorElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomContributorElementName),
                "This method should only be called if the reader is on an element which can appear in ATOM entry multiple times.");

            // Only read multiple author/contributor elements if either ATOM metadata reading is on, or the author/contributor is mapped to a multivalue.
            // If we're reading it only because of single property EPM, then we should only read the first one and completely skip
            // the others, to avoid failures in places where WCF DS didn't fail before (and we really don't care about those values anyway).
            // Note that author/contributor can only be mapped to directly by a multivalue property, so it is part of multivalue EPM if and only if
            // the author/contributor itself is mapped to the multivalue (as its children can't be mapped to the multivalue property itself, just to its items).
            return this.ReadAtomMetadata ||
                epmTargetPathSegment.IsMultiValueProperty ||
                !someAlreadyExist;
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
                this.XmlReader.LocalName == AtomConstants.AtomUpdatedElementName),
                "This method should only be called if the reader is on an element which can appear in ATOM entry just once.");

            if (alreadyExists)
            {
                if (this.ReadAtomMetadata)
                {
                    // TODO: Throw error, we should not allow multiple elements in case ATOM spec doesn't allow it, when we're reading ATOM metadata.
                }

                // Otherwise we're reading this only for EPM, in which case any additional elements like this should be skipped.
                return false;
            }

            return true;
        }
    }
}
