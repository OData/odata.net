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
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData ATOM Metadata deserializers.
    /// </summary>
    internal abstract class ODataAtomMetadataDeserializer : ODataAtomDeserializer
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
        internal ODataAtomMetadataDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.EmptyNamespace = nameTable.Add(string.Empty);
            this.AtomNamespace = nameTable.Add(AtomConstants.AtomNamespace);
        }

        /// <summary>
        /// Flag indicating if ATOM metadata is required to be read by the user.
        /// </summary>
        protected bool ReadAtomMetadata
        {
            get
            {
                return this.AtomInputContext.MessageReaderSettings.EnableAtomMetadataReading;
            }
        }

        /// <summary>
        /// Reads a person (author/contributor) element.
        /// </summary>
        /// <returns>The person metadata object with the read values.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element (atom:contributor/atom:author) - the atom:author/atom:contributor element to read.
        /// Post-Condition: Any                                                - the node after the atom:author/atom:contributor element which was read.
        /// </remarks>
        protected AtomPersonMetadata ReadAtomPersonConstruct()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                (this.XmlReader.LocalName == AtomConstants.AtomAuthorElementName || this.XmlReader.LocalName == AtomConstants.AtomContributorElementName) &&
                this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace,
                "Only atom:author or atom:contributor elements can be read by this method.");

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
                                if (this.ReadAtomMetadata)
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
        /// Reads the element value as DateTimeOffset value.
        /// </summary>
        /// <returns>The DateTimeOffset value of the element.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the element which value to read.
        /// Post-Condition: Any                 - the node after the element.
        /// 
        /// This method is not used in WCF DS client mode.
        /// </remarks>
        protected DateTimeOffset? ReadAtomDateConstruct()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "The element must be in ATOM namespace for this method work.");
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomUpdatedElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomPublishedElementName,
                "Only atom:updated and atom:published elements are supported by this method.");

            // TODO: Note that OData V3 doesn't support m:null for Updated and Published, because syndication API didn't allow us to do so
            // Now with ODataLib we might want to reenable this
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
            return PlatformHelper.ConvertStringToDateTimeOffset(stringValue);
        }

        /// <summary>
        /// Read the ATOM text construct element.
        /// </summary>
        /// <returns>The element read represented as ATOM text construct.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the element to read.
        /// Post-Condition: Any                 - the node after the element.
        /// </remarks>
        protected AtomTextConstruct ReadAtomTextConstruct()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "The element must be in ATOM namespace for this method to work.");
            Debug.Assert(
                this.XmlReader.LocalName == AtomConstants.AtomRightsElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomSummaryElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomSubtitleElementName ||
                this.XmlReader.LocalName == AtomConstants.AtomTitleElementName,
                "Only atom:rights, atom:summary, atom:subtitle, and atom:title elements are supported by this method.");

            // TODO: Client ignores the type attribute and reads everything as plain text.
            AtomTextConstruct textConstruct = new AtomTextConstruct();

            string typeValue = null;
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace) && string.CompareOrdinal(this.XmlReader.LocalName, AtomConstants.AtomTypeAttributeName) == 0)
                {
                    // type attribute
                    typeValue = this.XmlReader.Value;
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

            if (textConstruct.Kind == AtomTextConstructKind.Xhtml)
            {
                this.XmlReader.AssertNotBuffering();
                textConstruct.Text = this.XmlReader.ReadInnerXml();
            }
            else
            {
                textConstruct.Text = this.ReadElementStringValue();
            }

            return textConstruct;
        }

        /// <summary>
        /// Reads the value of the current XML element and returns it as a string.
        /// </summary>
        /// <returns>The string value read.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the element which value to read.
        /// Post-Condition: Any                 - the node after the element.
        /// </remarks>
        protected string ReadElementStringValue()
        {
            this.AssertXmlCondition(XmlNodeType.Element);

            return this.XmlReader.ReadElementValue();
        }

        /// <summary>
        /// Reads an "atom:title" element and returns an <seealso cref="AtomTextConstruct"/>.
        /// </summary>
        /// <returns>An <seealso cref="AtomTextConstruct"/> with the title information.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - The start of the atom:title element.
        /// Post-Condition: Any                 - The next node after the atom:title element. 
        /// </remarks>
        protected AtomTextConstruct ReadTitleElement()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalName == AtomConstants.AtomTitleElementName, "Expected element named 'title'.");
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "Element 'title' should be in the atom namespace.");

            return this.ReadAtomTextConstruct();
        }
    }
}
