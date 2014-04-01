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
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for ATOM metadata on feeds.
    /// </summary>
    internal sealed class ODataAtomFeedMetadataDeserializer : ODataAtomMetadataDeserializer
    {
        #region Atomized strings
        /// <summary>The empty namespace used for attributes in no namespace.</summary>
        private readonly string EmptyNamespace;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        /// <param name="inSourceElement">Whether this deserializer is reading feed metadata for a source element (true) or a feed element (false).</param>
        internal ODataAtomFeedMetadataDeserializer(ODataAtomInputContext atomInputContext, bool inSourceElement)
            : base(atomInputContext)
        {
            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.EmptyNamespace = nameTable.Add(string.Empty);

            this.InSourceElement = inSourceElement;
        }

        /// <summary>
        /// Whether this deserializer is reading feed metadata for a source element (true) or a feed element (false).
        /// </summary>
        private bool InSourceElement { get; set; }

        /// <summary>
        /// Reads an element in the ATOM namespace in feed or source content.
        /// </summary>
        /// <param name="atomFeedMetadata">The atom feed metadata object to store metadata details in.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:*) - the ATOM element to read.
        /// Post-Condition: Any                          - the node after the ATOM element which was read.
        /// 
        /// If the the property InSourceElement is true (i.e., we're reading within source content), then the value 
        /// of the atom:id element will be stored in the feed metadata as SourceId, otherwise it will be ignored.
        /// </remarks>
        internal void ReadAtomElementAsFeedMetadata(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "Only atom:* elements can be read by this method.");

            switch (this.XmlReader.LocalName)
            {
                case AtomConstants.AtomAuthorElementName:
                    this.ReadAuthorElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomCategoryElementName:
                    this.ReadCategoryElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomContributorElementName:
                    this.ReadContributorElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomGeneratorElementName:
                    this.ReadGeneratorElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomIconElementName:
                    this.ReadIconElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomIdElementName:
                    // Only store the ID as ATOM metadata if we're within an atom:source element.
                    if (this.InSourceElement)
                    {
                        this.ReadIdElementAsSourceId(atomFeedMetadata);
                    }
                    else
                    {
                        this.XmlReader.Skip();
                    }

                    break;
                case AtomConstants.AtomLinkElementName:
                    this.ReadLinkElementIntoLinksCollection(atomFeedMetadata);
                    break;
                case AtomConstants.AtomLogoElementName:
                    this.ReadLogoElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomRightsElementName:
                    this.ReadRightsElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomSubtitleElementName:
                    this.ReadSubtitleElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomTitleElementName:
                    this.ReadTitleElement(atomFeedMetadata);
                    break;
                case AtomConstants.AtomUpdatedElementName:
                    this.ReadUpdatedElement(atomFeedMetadata);
                    break;
                default:
                    // Not something we recognize, so just ignore it.
                    this.XmlReader.Skip();
                    break;
            }
        }

        /// <summary>
        /// Reads the atom:link element and returns a new ATOM link metadata object.
        /// </summary>
        /// <param name="relation">The value of the rel attribute for the link element.</param>
        /// <param name="hrefStringValue">The value of the href attribute for the link element.</param>
        /// <returns>An <see cref="AtomLinkMetadata"/> instance storing the information about this link.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:link) - the atom:link element to read.
        /// Post-Condition: Any                             - the node after the ATOM element which was read.
        /// </remarks>
        internal AtomLinkMetadata ReadAtomLinkElementInFeed(string relation, string hrefStringValue)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace && this.XmlReader.LocalName == AtomConstants.AtomLinkElementName,
                "Only atom:link element can be read by this method.");

            AtomLinkMetadata linkMetadata = new AtomLinkMetadata
            {
                Relation = relation,
                Href = hrefStringValue == null ? null : this.ProcessUriFromPayload(hrefStringValue, this.XmlReader.XmlBaseUri)
            };

            // Read the attributes
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
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

                            break;
                        case AtomConstants.AtomLinkRelationAttributeName:
                            // Only store this rel value if linkMetadata.Relation was not set yet.
                            // Note: The value supplied via the parameter "relation" takes priority 
                            //       over what we read in the XML at this point.  This is because
                            //       of situations such as having an IANA namespace prefix on the rel
                            //       value (in which case we don't store the literal rel value as it 
                            //       appears in the xml, but just the part after the IANA prefix).
                            if (linkMetadata.Relation == null)
                            {
                                linkMetadata.Relation = this.XmlReader.Value;
                            }

                            break;
                        case AtomConstants.AtomLinkHrefAttributeName:
                            // Only store the href value if linkMetadata.Href was not set yet.
                            if (linkMetadata.Href == null)
                            {
                                linkMetadata.Href = this.ProcessUriFromPayload(this.XmlReader.Value, this.XmlReader.XmlBaseUri);
                            }

                            break;
                        default:
                            // Ignore all other attributes.
                            break;
                    }
                }
            }
            
            this.XmlReader.Skip();

            return linkMetadata;
        }

        /// <summary>
        /// Reads an atom:author element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:author) - the atom:author element to read.
        /// Post-Condition: Any                               - the node after the atom:author element which was read.
        /// </remarks>
        private void ReadAuthorElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomAuthorElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:author elements can be read by this method.");

            atomFeedMetadata.AddAuthor(this.ReadAtomPersonConstruct());
        }

        /// <summary>
        /// Reads an atom:category element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:category) - the atom:category element to read.
        /// Post-Condition: Any                                 - the node after the atom:category which was read.
        /// </remarks>
        private void ReadCategoryElement(AtomFeedMetadata atomFeedMetadata)
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

            atomFeedMetadata.AddCategory(categoryMetadata);

            // Skip the rest of the category element.
            this.XmlReader.Skip();
        }

        /// <summary>
        /// Reads an atom:contributor element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:contributor) - the atom:contributor element to read.
        /// Post-Condition: Any                                    - the node after the atom:contributor element which was read.
        /// </remarks>
        private void ReadContributorElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomContributorElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:contributor elements can be read by this method.");

            atomFeedMetadata.AddContributor(this.ReadAtomPersonConstruct());
        }

        /// <summary>
        /// Reads an atom:generator element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:generator) - the atom:generator element to read.
        /// Post-Condition: Any                                  - the node after the atom:generator element which was read.
        /// </remarks>
        private void ReadGeneratorElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomGeneratorElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:generator elements can be read by this method.");

            this.VerifyNotPreviouslyDefined(atomFeedMetadata.Generator);
            AtomGeneratorMetadata generatorMetadata = new AtomGeneratorMetadata();

            // Read the attributes.
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
                    switch (this.XmlReader.LocalName)
                    {
                        case AtomConstants.AtomGeneratorUriAttributeName:
                            generatorMetadata.Uri = this.ProcessUriFromPayload(this.XmlReader.Value, this.XmlReader.XmlBaseUri);
                            break;
                        case AtomConstants.AtomGeneratorVersionAttributeName:
                            generatorMetadata.Version = this.XmlReader.Value;
                            break;
                        default:
                            // Ignore all other attributes.
                            break;
                    }
                }
            }

            // Read the value of the element as a string and store it as the name of the generator.
            this.XmlReader.MoveToElement();
            if (this.XmlReader.IsEmptyElement)
            {
                // Move to the next node after this one.
                this.XmlReader.Skip();
            }
            else
            {
                generatorMetadata.Name = this.XmlReader.ReadElementValue();
            }

            atomFeedMetadata.Generator = generatorMetadata;
        }

        /// <summary>
        /// Reads an atom:icon element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:icon) - the atom:icon element to read.
        /// Post-Condition: Any                             - the node after the atom:icon element which was read.
        /// </remarks>
        private void ReadIconElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomIconElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:icon elements can be read by this method.");

            this.VerifyNotPreviouslyDefined(atomFeedMetadata.Icon);
            atomFeedMetadata.Icon = this.ReadUriValuedElement();
        }

        /// <summary>
        /// Reads an atom:id element in a source element.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:icon) - the atom:icon element to read.
        /// Post-Condition: Any                             - the node after the atom:icon element which was read.
        /// </remarks>
        private void ReadIdElementAsSourceId(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomIdElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:id elements can be read by this method.");

            this.VerifyNotPreviouslyDefined(atomFeedMetadata.SourceId);
            string elementValue = this.XmlReader.ReadElementValue();

            atomFeedMetadata.SourceId = UriUtils.CreateUriAsEntryOrFeedId(elementValue, UriKind.Absolute);
        }

        /// <summary>
        /// Reads an atom:link element into the Links collection of feed metadata (i.e., links that are not special to the OData protocol).
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:link) - the atom:link element to read.
        /// Post-Condition: Any                             - the node after the atom:link element which was read.
        /// </remarks>
        private void ReadLinkElementIntoLinksCollection(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomLinkElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:link elements can be read by this method.");

            // By sending nulls to ReadAtomLinkElementInFeed(), the method will read and store values for href and rel from the wire (inside of using parameter overrides).
            AtomLinkMetadata linkMetadata = this.ReadAtomLinkElementInFeed(null, null);
            atomFeedMetadata.AddLink(linkMetadata);
        }

        /// <summary>
        /// Reads an atom:logo element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:logo) - the atom:logo element to read.
        /// Post-Condition: Any                             - the node after the atom:logo element which was read.
        /// </remarks>
        private void ReadLogoElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomLogoElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:logo elements can be read by this method.");

            this.VerifyNotPreviouslyDefined(atomFeedMetadata.Logo);
            atomFeedMetadata.Logo = this.ReadUriValuedElement();
        }

        /// <summary>
        /// Reads an atom:rights element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:rights) - the atom:rights element to read.
        /// Post-Condition: Any                               - the node after the atom:rights element which was read.
        /// </remarks>
        private void ReadRightsElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomRightsElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:rights elements can be read by this method.");

            this.VerifyNotPreviouslyDefined(atomFeedMetadata.Rights);
            atomFeedMetadata.Rights = this.ReadAtomTextConstruct();
        }

        /// <summary>
        /// Reads an atom:subtitle element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:subtitle) - the atom:subtitle element to read.
        /// Post-Condition: Any                                 - the node after the atom:subtitle element which was read.
        /// </remarks>
        private void ReadSubtitleElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomSubtitleElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:subtitle elements can be read by this method.");

            this.VerifyNotPreviouslyDefined(atomFeedMetadata.Subtitle);
            atomFeedMetadata.Subtitle = this.ReadAtomTextConstruct();
        }

        /// <summary>
        /// Reads an atom:title element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:title) - the atom:title element to read.
        /// Post-Condition: Any                              - the node after the atom:title element which was read.
        /// </remarks>
        private void ReadTitleElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomTitleElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:title elements can be read by this method.");

            this.VerifyNotPreviouslyDefined(atomFeedMetadata.Title);
            atomFeedMetadata.Title = this.ReadAtomTextConstruct();
        }

        /// <summary>
        /// Reads an atom:updated element in a feed.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:updated) - the atom:updated element to read.
        /// Post-Condition: Any                                - the node after the atom:updated element which was read.
        /// </remarks>
        private void ReadUpdatedElement(AtomFeedMetadata atomFeedMetadata)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomUpdatedElementName && this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:updated elements can be read by this method.");

            this.VerifyNotPreviouslyDefined(atomFeedMetadata.Updated);
            atomFeedMetadata.Updated = this.ReadAtomDateConstruct();
        }

        /// <summary>
        /// Reads an atom:* element whose value is a URI.
        /// </summary>
        /// <returns>The <see cref="Uri"/> which was read.</returns>
        private Uri ReadUriValuedElement()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "Only atom:* element can be read by this method.");

            string value = this.XmlReader.ReadElementValue();
            return this.ProcessUriFromPayload(value, this.XmlReader.XmlBaseUri);
        }

        /// <summary>
        /// Fails with the appropriate exception message if the given value is not null.
        /// </summary>
        /// <param name="metadataValue">The metadata value to ensure is null.</param>
        private void VerifyNotPreviouslyDefined(object metadataValue)
        {
            if (metadataValue != null)
            {
                // We should not allow multiple elements per the ATOM spec, when we're reading ATOM metadata.
                string parentElementName = this.InSourceElement ? AtomConstants.AtomSourceElementName : AtomConstants.AtomFeedElementName;
                throw new ODataException(Strings.ODataAtomMetadataDeserializer_MultipleSingletonMetadataElements(this.XmlReader.LocalName, parentElementName));
            }
        }
    }
}
