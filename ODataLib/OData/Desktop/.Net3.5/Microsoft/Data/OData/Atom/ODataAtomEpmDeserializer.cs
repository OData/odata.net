//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using System.Linq;
    using System.Xml;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for EPM.
    /// </summary>
    internal abstract class ODataAtomEpmDeserializer : ODataAtomMetadataDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomEpmDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Reads an extension element in non-ATOM namespace in the content of the entry element.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <returns>true if a mapping for the current custom element was found and the element was read; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the element in non-ATOM namespace to read.
        /// Post-Condition: Any                 - the node after the extension element which was read.
        /// </remarks>
        internal bool TryReadExtensionElementInEntryContent(IODataAtomReaderEntryState entryState)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI != AtomConstants.AtomNamespace, "Only elements in non-ATOM namespace can be read by this method.");

            ODataEntityPropertyMappingCache cachedEpm = entryState.CachedEpm;
            if (cachedEpm == null)
            {
                return false;
            }

            EpmTargetPathSegment epmTargetPathSegment = cachedEpm.EpmTargetTree.NonSyndicationRoot;
            return this.TryReadCustomEpmElement(entryState, epmTargetPathSegment);
        }

        /// <summary>
        /// Reads an element for custom EPM.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="epmTargetPathSegment">The EPM target segment for the parent element to which the element belongs.</param>
        /// <returns>true if a mapping for the current custom element was found and the element was read; otherwise false.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the element to read.
        /// Post-Condition: Any                 - the node after the element which was read.
        /// 
        /// The method works on any element, it checks if the element should be used for EPM or not.
        /// </remarks>
        private bool TryReadCustomEpmElement(IODataAtomReaderEntryState entryState, EpmTargetPathSegment epmTargetPathSegment)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(epmTargetPathSegment != null, "epmTargetPathSegment != null");

            string localName = this.XmlReader.LocalName;
            string namespaceUri = this.XmlReader.NamespaceURI;
            EpmTargetPathSegment elementSegment = epmTargetPathSegment.SubSegments.FirstOrDefault(
                segment => !segment.IsAttribute &&
                string.CompareOrdinal(segment.SegmentName, localName) == 0 &&
                string.CompareOrdinal(segment.SegmentNamespaceUri, namespaceUri) == 0);

            if (elementSegment == null)
            {
                // Skip elements that are not part of EPM
                return false;
            }

            if (elementSegment.HasContent && entryState.EpmCustomReaderValueCache.Contains(elementSegment.EpmInfo))
            {
                // Skip elements for which we already have value.
                // Both WCF DS client and server will only read the first value from custom EPM for any given EPM info.
                // It also follows the behavior for syndication EPM, where we read only the first author if there are multiple (for example).
                // We don't want to try to parse such elements since the parsing itself may fail and we should not be failing on values
                // which we don't care about.
                return false;
            }

            while (this.XmlReader.MoveToNextAttribute())
            {
                this.ReadCustomEpmAttribute(entryState, elementSegment);
            }

            this.XmlReader.MoveToElement();

            if (elementSegment.HasContent)
            {
                string stringValue;

                // Read the value of the element.
                stringValue = this.ReadElementStringValue();

                entryState.EpmCustomReaderValueCache.Add(elementSegment.EpmInfo, stringValue);
            }
            else
            {
                if (!this.XmlReader.IsEmptyElement)
                {
                    // Move to the first child node of the element.
                    this.XmlReader.Read();

                    while (this.XmlReader.NodeType != XmlNodeType.EndElement)
                    {
                        switch (this.XmlReader.NodeType)
                        {
                            case XmlNodeType.EndElement:
                                break;

                            case XmlNodeType.Element:
                                if (!this.TryReadCustomEpmElement(entryState, elementSegment))
                                {
                                    this.XmlReader.Skip();
                                }

                                break;

                            default:
                                this.XmlReader.Skip();
                                break;
                        }
                    }
                }

                // Read the end element or the empty start element.
                this.XmlReader.Read();
            }

            return true;
        }

        /// <summary>
        /// Reads an attribute for custom EPM.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry being read.</param>
        /// <param name="epmTargetPathSegmentForElement">The EPM target segment for the element to which the attribute belongs.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Attribute - the attribute to read.
        /// Post-Condition: XmlNodeType.Attribute - the same attribute, the method doesn't move the reader.
        /// 
        /// The method works on any attribute, it checks if the attribute should be used for EPM or not.
        /// </remarks>
        private void ReadCustomEpmAttribute(IODataAtomReaderEntryState entryState, EpmTargetPathSegment epmTargetPathSegmentForElement)
        {
            Debug.Assert(entryState != null, "entryState != null");
            this.AssertXmlCondition(XmlNodeType.Attribute);
            Debug.Assert(epmTargetPathSegmentForElement != null, "epmTargetPathSegmentForElement != null");

            string localName = this.XmlReader.LocalName;
            string namespaceUri = this.XmlReader.NamespaceURI;
            EpmTargetPathSegment attributeSegment = epmTargetPathSegmentForElement.SubSegments.FirstOrDefault(
                segment => segment.IsAttribute && 
                string.CompareOrdinal(segment.AttributeName, localName) == 0 &&
                string.CompareOrdinal(segment.SegmentNamespaceUri, namespaceUri) == 0);
            if (attributeSegment != null)
            {
                // Don't add values which we already have
                // Both WCF DS client and server will only read the first value from custom EPM for any given EPM info.
                // It also follows the behavior for syndication EPM, where we read only the first author if there are multiple (for example).
                if (!entryState.EpmCustomReaderValueCache.Contains(attributeSegment.EpmInfo))
                {
                    // Note that there's no way for an attribute to specify null value.
                    entryState.EpmCustomReaderValueCache.Add(attributeSegment.EpmInfo, this.XmlReader.Value);
                }
            }
        }
    }
}
