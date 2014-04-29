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
    using System.Xml;
    using Microsoft.OData.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for entity reference links.
    /// </summary>
    internal sealed class ODataAtomEntityReferenceLinkDeserializer : ODataAtomDeserializer
    {
        #region Atomized strings
        /// <summary>OData element name for the 'feed' element</summary>
        private readonly string ODataFeedElementName;

        /// <summary>OData element name for the 'count' element</summary>
        private readonly string ODataCountElementName;

        /// <summary>OData element name for the 'next' element</summary>
        private readonly string ODataNextElementName;

        /// <summary>OData element name for the 'ref' element</summary>
        private readonly string ODataRefElementName;
        #endregion Atomized strings

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomEntityReferenceLinkDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.ODataFeedElementName = nameTable.Add(AtomConstants.AtomFeedElementName);
            this.ODataCountElementName = nameTable.Add(AtomConstants.ODataCountElementName);
            this.ODataNextElementName = nameTable.Add(AtomConstants.ODataNextLinkElementName);
            this.ODataRefElementName = nameTable.Add(AtomConstants.ODataRefElementName);
        }

        /// <summary>
        /// An enumeration of the various kinds of properties on an entity reference link collection.
        /// </summary>
        [Flags]
        private enum DuplicateEntityReferenceLinksElementBitMask
        {
            /// <summary>No duplicates.</summary>
            None = 0,

            /// <summary>The 'm:count' element of the 'feed' element.</summary>
            Count = 1,

            /// <summary>The 'd:next' element of the 'feed' element.</summary>
            NextLink = 2,
        }

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        /// <remarks>
        /// Pre-Condition:  PayloadStart        - assumes that the XML reader has not been used yet.
        /// Post-Condition: XmlNodeType.None    - The reader must be at the end of the input.
        /// </remarks>
        internal ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");
            this.XmlReader.AssertNotBuffering();

            // Read the start of the payload up to the first element
            this.ReadPayloadStart();
            this.AssertXmlCondition(XmlNodeType.Element);

            if (!this.XmlReader.NamespaceEquals(this.XmlReader.NamespaceURI) || !this.XmlReader.LocalNameEquals(this.ODataFeedElementName))
            {
                throw new ODataException(
                    Strings.ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksStartElement(this.XmlReader.LocalName, this.XmlReader.NamespaceURI));
            }

            ODataEntityReferenceLinks entityReferenceLinks = this.ReadFeedElement();

            // Read the payload end
            this.ReadPayloadEnd();
            this.XmlReader.AssertNotBuffering();

            return entityReferenceLinks;
        }

        /// <summary>
        /// Reads a top-level entity reference link.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> instance representing the read entity reference link.</returns>
        /// <remarks>
        /// Pre-Condition:  PayloadStart        - assumes that the XML reader has not been used yet.
        /// Post-Condition: XmlNodeType.None    - The reader must be at the end of the input.
        /// </remarks>
        internal ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");

            // Read the start of the payload up to the first element
            this.ReadPayloadStart();
            this.AssertXmlCondition(XmlNodeType.Element);

            // We need to accept both OData and OData metadata namespace for the "ref" element due to backward compatibility.
            // Per spec the element should be in OData namespace, by WCF DS client was using the metadata namespace, so it's easier to accept that here as well.
            if ((!this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace)) ||
                !this.XmlReader.LocalNameEquals(this.ODataRefElementName))
            {
                throw new ODataException(
                    Strings.ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinkStartElement(this.XmlReader.LocalName, this.XmlReader.NamespaceURI));
            }

            ODataEntityReferenceLink entityReferenceLink = this.ReadEntityReferenceId();

            // Read the payload end
            this.ReadPayloadEnd();
            this.XmlReader.AssertNotBuffering();

            return entityReferenceLink;
        }

        /// <summary>
        /// Verifies that the specified element was not yet found in the entity reference links element.
        /// </summary>
        /// <param name="elementsFoundBitField">The bit field which stores which elements of an inner error were found so far.</param>
        /// <param name="elementFoundBitMask">The bit mask for the element to check.</param>
        /// <param name="elementNamespace">The namespace name of the element ot check (used for error reporting).</param>
        /// <param name="elementName">The name of the element to check (used for error reporting).</param>
        private static void VerifyEntityReferenceLinksElementNotFound(
            ref DuplicateEntityReferenceLinksElementBitMask elementsFoundBitField,
            DuplicateEntityReferenceLinksElementBitMask elementFoundBitMask,
            string elementNamespace,
            string elementName)
        {
            Debug.Assert(((int)elementFoundBitMask & (((int)elementFoundBitMask) - 1)) == 0, "elementFoundBitMask is not a power of 2.");
            Debug.Assert(!string.IsNullOrEmpty(elementName), "!string.IsNullOrEmpty(elementName)");

            if ((elementsFoundBitField & elementFoundBitMask) == elementFoundBitMask)
            {
                throw new ODataException(Strings.ODataAtomEntityReferenceLinkDeserializer_MultipleEntityReferenceLinksElementsWithSameName(elementNamespace, elementName));
            }

            elementsFoundBitField |= elementFoundBitMask;
        }

        /// <summary>
        /// Reads all top-level entity reference links and the (optional) inline count and next link elements.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> instance representing the read entity reference links.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - The 'feed' element.
        /// Post-Condition:  any                - The node after the 'feed' end element (or empty 'feed' element).
        /// </remarks>
        private ODataEntityReferenceLinks ReadFeedElement()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace");
            Debug.Assert(this.XmlReader.LocalName == this.ODataFeedElementName, "this.XmlReader.LocalName == AtomConstants.ODataLinksElementName");

            ODataEntityReferenceLinks links = new ODataEntityReferenceLinks();
            List<ODataEntityReferenceLink> linkList = new List<ODataEntityReferenceLink>();
            DuplicateEntityReferenceLinksElementBitMask elementsReadBitmask = DuplicateEntityReferenceLinksElementBitMask.None;

            if (!this.XmlReader.IsEmptyElement)
            {
                // Move to the first child node of the element.
                this.XmlReader.Read();

                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            // end of the <feed> element
                            continue;

                        case XmlNodeType.Element:
                            // <m:count>
                            if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace) &&
                                this.XmlReader.LocalNameEquals(this.ODataCountElementName))
                            {
                                VerifyEntityReferenceLinksElementNotFound(
                                    ref elementsReadBitmask,
                                    DuplicateEntityReferenceLinksElementBitMask.Count,
                                    this.XmlReader.ODataMetadataNamespace,
                                    AtomConstants.ODataCountElementName);

                                // Note that we allow negative values to be read.
                                long countValue = (long)AtomValueUtils.ReadPrimitiveValue(this.XmlReader, EdmCoreModel.Instance.GetInt64(false));
                                links.Count = countValue;

                                // Read over the end element of the <m:count> element
                                this.XmlReader.Read();

                                continue;
                            }

                            // <m:ref>
                            if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace)
                                && this.XmlReader.LocalNameEquals(this.ODataRefElementName))
                            {
                                ODataEntityReferenceLink link = this.ReadEntityReferenceId();
                                linkList.Add(link);

                                continue;
                            }

                            // <d:next>
                            if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace)
                                && this.XmlReader.LocalNameEquals(this.ODataNextElementName))
                            {
                                VerifyEntityReferenceLinksElementNotFound(
                                    ref elementsReadBitmask,
                                    DuplicateEntityReferenceLinksElementBitMask.NextLink,
                                    this.XmlReader.ODataNamespace,
                                    AtomConstants.ODataNextLinkElementName);

                                // NOTE: get the base URI here before we read the content as string; reading the content as string will move the 
                                //       reader to the end element and thus we lose the xml:base definition on the element.
                                Uri xmlBaseUri = this.XmlReader.XmlBaseUri;
                                string uriString = this.XmlReader.ReadElementValue();
                                links.NextPageLink = this.ProcessUriFromPayload(uriString, xmlBaseUri);

                                continue;
                            }

                            break;
                        default:
                            break;
                    }

                    this.XmlReader.Skip();
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            }

            // Read over the end element, or empty start element.
            this.XmlReader.Read();

            links.Links = new ReadOnlyEnumerable<ODataEntityReferenceLink>(linkList);
            return links;
        }

        /// <summary>
        /// Read an entity reference link.
        /// </summary>
        /// <returns>An instance of <see cref="ODataEntityReferenceLink"/> which was read.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the 'm:ref' element to read.
        /// Post-Condition: Any                 - the node after the 'm:ref' element which was read.
        /// </remarks>
        private ODataEntityReferenceLink ReadEntityReferenceId()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == this.XmlReader.ODataMetadataNamespace, "this.XmlReader.NamespaceURI == this.XmlReader.ODataMetadataNamespace");
            Debug.Assert(this.XmlReader.LocalName == this.ODataRefElementName, "this.XmlReader.LocalName == this.ODataRefElementName");

            ODataEntityReferenceLink link = new ODataEntityReferenceLink();

            string uriString = this.XmlReader.GetAttribute(AtomConstants.AtomIdElementName);
            Debug.Assert(uriString != null, "In ATOM a entity reference id attribute on ref element can never represent a null value.");
            Uri xmlBaseUri = this.XmlReader.XmlBaseUri;
            Uri uri = this.ProcessUriFromPayload(uriString, xmlBaseUri);
            link.Url = uri;

            this.XmlReader.Skip();
            ReaderValidationUtils.ValidateEntityReferenceLink(link);
            return link;
        }
    }
}
