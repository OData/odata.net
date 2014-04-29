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
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for ATOM metadata in a service document
    /// </summary>
    internal sealed class ODataAtomServiceDocumentMetadataDeserializer : ODataAtomMetadataDeserializer
    {
        #region Atomized strings
        /// <summary>Schema namespace for Atom.</summary>
        private readonly string AtomNamespace;

        /// <summary>The name of the 'category' element in a service document.</summary>
        private readonly string AtomCategoryElementName;

        /// <summary>The name of the 'href' attribute in an 'app:categories' element.</summary>
        private readonly string AtomHRefAttributeName;

        /// <summary>The name of the 'fixed' attribute in an 'app:categories' element.</summary>
        private readonly string AtomPublishingFixedAttributeName;

        /// <summary>The name of the 'scheme' attribute in an 'app:categories' or 'atom:category' element.</summary>
        private readonly string AtomCategorySchemeAttributeName;

        /// <summary>The name of the 'term' attribute in an 'atom:category' element.</summary>
        private readonly string AtomCategoryTermAttributeName;

        /// <summary>The name of the 'label' attribute in an 'atom:category' element.</summary>
        private readonly string AtomCategoryLabelAttributeName;
      
        /// <summary>The empty namespace</summary>
        private readonly string EmptyNamespace;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomServiceDocumentMetadataDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.AtomNamespace = nameTable.Add(AtomConstants.AtomNamespace);
            this.AtomCategoryElementName = nameTable.Add(AtomConstants.AtomCategoryElementName);
            this.AtomHRefAttributeName = nameTable.Add(AtomConstants.AtomHRefAttributeName);
            this.AtomPublishingFixedAttributeName = nameTable.Add(AtomConstants.AtomPublishingFixedAttributeName);
            this.AtomCategorySchemeAttributeName = nameTable.Add(AtomConstants.AtomCategorySchemeAttributeName);
            this.AtomCategoryTermAttributeName = nameTable.Add(AtomConstants.AtomCategoryTermAttributeName);
            this.AtomCategoryLabelAttributeName = nameTable.Add(AtomConstants.AtomCategoryLabelAttributeName);
            this.EmptyNamespace = nameTable.Add(string.Empty);
        }

        /// <summary>
        /// Reads an atom:title element and adds the new information to <paramref name="workspaceMetadata"/>.
        /// </summary>
        /// <param name="workspaceMetadata">The non-null workspace metadata object to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - The start of the atom:title element.
        /// Post-Condition: Any                 - The next node after the atom:title element. 
        /// </remarks>
        internal void ReadTitleElementInWorkspace(AtomWorkspaceMetadata workspaceMetadata)
        {
            Debug.Assert(workspaceMetadata != null, "workspaceMetadata != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalName == AtomConstants.AtomTitleElementName, "Expected element named 'title'.");
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "Element 'title' should be in the atom namespace.");

            if (workspaceMetadata.Title != null)
            {
                throw new ODataException(Strings.ODataAtomServiceDocumentMetadataDeserializer_MultipleTitleElementsFound(AtomConstants.AtomPublishingWorkspaceElementName));
            }

            workspaceMetadata.Title = this.ReadTitleElement();
        }

        /// <summary>
        /// Reads an atom:title element and adds the new information to <paramref name="odataServiceDocumentElement"/> and (if ATOM metadata reading is on) <paramref name="collectionMetadata"/>.
        /// </summary>
        /// <param name="collectionMetadata">The collection metadata object to augment, or null if metadata reading is not on.</param>
        /// <param name="odataServiceDocumentElement">The non-null service document element info object being populated.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - The start of the title element.
        /// Post-Condition: Any                 - The next node after the title element. 
        /// </remarks>
        internal void ReadTitleElementInCollection(AtomResourceCollectionMetadata collectionMetadata, ODataServiceDocumentElement odataServiceDocumentElement)
        {
            Debug.Assert(!this.ReadAtomMetadata || collectionMetadata != null, "collectionMetadata parameter should be non-null when ATOM metadata reading is enabled.");
            Debug.Assert(odataServiceDocumentElement != null, "collectionInfo != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalName == AtomConstants.AtomTitleElementName, "Expected element named 'title'.");
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "Element 'title' should be in the atom namespace.");

            AtomTextConstruct titleTextConstruct = this.ReadTitleElement();

            if (odataServiceDocumentElement.Name == null)
            {
                odataServiceDocumentElement.Name = titleTextConstruct.Text;
            }

            if (this.ReadAtomMetadata)
            {
                collectionMetadata.Title = titleTextConstruct;
            }
        }

        /// <summary>
        /// Reads an app:categories element as well as each atom:category element contained within it, and adds the new information to <paramref name="collectionMetadata"/>.
        /// </summary>
        /// <param name="collectionMetadata">The non-null collection metadata object to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - The start of the app:categories element.
        /// Post-Condition: Any                 - The next node after the app:categories element. 
        /// </remarks>
        internal void ReadCategoriesElementInCollection(AtomResourceCollectionMetadata collectionMetadata)
        {
            Debug.Assert(collectionMetadata != null, "collectionMetadata != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalName == AtomConstants.AtomPublishingCategoriesElementName, "Expected element named 'categories'.");
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomPublishingNamespace, "Element 'categories' should be in the atom publishing namespace.");

            AtomCategoriesMetadata categoriesMetadata = new AtomCategoriesMetadata();
            List<AtomCategoryMetadata> categoryList = new List<AtomCategoryMetadata>();

            while (this.XmlReader.MoveToNextAttribute())
            {
                string attributeValue = this.XmlReader.Value;

                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
                    if (this.XmlReader.LocalNameEquals(this.AtomHRefAttributeName))
                    {
                        categoriesMetadata.Href = this.ProcessUriFromPayload(attributeValue, this.XmlReader.XmlBaseUri);
                    }
                    else if (this.XmlReader.LocalNameEquals(this.AtomPublishingFixedAttributeName))
                    {
                        if (String.CompareOrdinal(attributeValue, AtomConstants.AtomPublishingFixedYesValue) == 0)
                        {
                            categoriesMetadata.Fixed = true;
                        }
                        else if (String.CompareOrdinal(attributeValue, AtomConstants.AtomPublishingFixedNoValue) == 0)
                        {
                            categoriesMetadata.Fixed = false;
                        }
                        else
                        {
                            throw new ODataException(Strings.ODataAtomServiceDocumentMetadataDeserializer_InvalidFixedAttributeValue(attributeValue));
                        }
                    }
                    else if (this.XmlReader.LocalNameEquals(this.AtomCategorySchemeAttributeName))
                    {
                        categoriesMetadata.Scheme = attributeValue;
                    }
                }
            }

            this.XmlReader.MoveToElement();

            if (!this.XmlReader.IsEmptyElement)
            {
                // read over the categories element
                this.XmlReader.ReadStartElement();

                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.AtomNamespace) && this.XmlReader.LocalNameEquals(this.AtomCategoryElementName))
                            {
                                categoryList.Add(this.ReadCategoryElementInCollection());
                            }

                            break;
                        case XmlNodeType.EndElement:
                            // end of 'categories' element.
                            break;
                        default:
                            // ignore all other nodes.
                            this.XmlReader.Skip();
                            break;
                    }
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            } // if (!this.XmlReader.IsEmptyElement)

            // read over the end tag of the categories element or the start tag if the categories element is empty.
            this.XmlReader.Read();

            categoriesMetadata.Categories = new ReadOnlyEnumerable<AtomCategoryMetadata>(categoryList);
            collectionMetadata.Categories = categoriesMetadata;
        }

        /// <summary>
        /// Reads an "app:accept" element and adds the new information to <paramref name="collectionMetadata"/>.
        /// </summary>
        /// <param name="collectionMetadata">The non-null collection metadata object to augment.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - The start of the app:accept element.
        /// Post-Condition: Any                 - The next node after the app:accept element. 
        /// </remarks>
        internal void ReadAcceptElementInCollection(AtomResourceCollectionMetadata collectionMetadata)
        {
            Debug.Assert(collectionMetadata != null, "collectionMetadata != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalName == AtomConstants.AtomPublishingAcceptElementName, "Expected element named 'accept'.");
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomPublishingNamespace, "Element 'accept' should be in the atom publishing namespace.");

            if (collectionMetadata.Accept != null)
            {
                throw new ODataException(Strings.ODataAtomServiceDocumentMetadataDeserializer_MultipleAcceptElementsFoundInCollection);
            }

            collectionMetadata.Accept = this.XmlReader.ReadElementValue();
        }

        /// <summary>
        /// Reads an "atom:category" element and returns the data as an <seealso cref="AtomCategoryMetadata"/> object.
        /// </summary>
        /// <returns>An <seealso cref="AtomCategoryMetadata"/> object with its properties filled in according to what was found in the XML.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - The start of the atom:category element.
        /// Post-Condition: Any                 - The next node after the atom:category element.
        /// </remarks>
        private AtomCategoryMetadata ReadCategoryElementInCollection()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalName == AtomConstants.AtomCategoryElementName, "Expected element named 'category'.");
            Debug.Assert(this.XmlReader.NamespaceURI == AtomConstants.AtomNamespace, "Element 'category' should be in the atom namespace.");

            AtomCategoryMetadata categoryMetadata = new AtomCategoryMetadata();

            while (this.XmlReader.MoveToNextAttribute())
            {
                string attributeValue = this.XmlReader.Value;

                if (this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
                    if (this.XmlReader.LocalNameEquals(this.AtomCategoryTermAttributeName))
                    {
                        categoryMetadata.Term = attributeValue;
                    }
                    else if (this.XmlReader.LocalNameEquals(this.AtomCategorySchemeAttributeName))
                    {
                        categoryMetadata.Scheme = attributeValue;
                    }
                    else if (this.XmlReader.LocalNameEquals(this.AtomCategoryLabelAttributeName))
                    {
                        categoryMetadata.Label = attributeValue;
                    }
                }
            }

            return categoryMetadata;
        }
    }
}
