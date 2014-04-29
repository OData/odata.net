//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for collections.
    /// </summary>
    internal sealed class ODataAtomCollectionDeserializer : ODataAtomPropertyAndValueDeserializer
    {
        /// <summary>Cached duplicate property names checker to use if the items are complex values.</summary>
        private readonly DuplicatePropertyNamesChecker duplicatePropertyNamesChecker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomCollectionDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            this.duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
        }

        /// <summary>
        /// Reads the start element of a collection.
        /// </summary>
        /// <param name="isCollectionElementEmpty">true, if the collection element is empty; false otherwise.</param>
        /// <returns>An <see cref="ODataCollectionStart"/> representing the collection-level information. Currently this only contains 
        /// the name of the collection.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element - The start element of the collection.
        /// Post-Condition:  Any                 - The next node after the start element node of the collection or the 
        ///                                        empty collection element node.
        /// </remarks>
        internal ODataCollectionStart ReadCollectionStart(out bool isCollectionElementEmpty)
        {
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);

            if (!this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
            {
                throw new ODataException(ODataErrorStrings.ODataAtomCollectionDeserializer_TopLevelCollectionElementWrongNamespace(this.XmlReader.NamespaceURI, this.XmlReader.ODataMetadataNamespace));
            }

            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace) &&
                   (this.XmlReader.LocalNameEquals(this.AtomTypeAttributeName) ||
                   (this.XmlReader.LocalNameEquals(this.ODataNullAttributeName))))
                {
                    // make sure that m:type or m:null attributes are not present in the root element of the collection.
                    throw new ODataException(ODataErrorStrings.ODataAtomCollectionDeserializer_TypeOrNullAttributeNotAllowed);
                }
            }

            // ignore all other attributes.
            this.XmlReader.MoveToElement();

            ODataCollectionStart collectionStart = new ODataCollectionStart();

            isCollectionElementEmpty = this.XmlReader.IsEmptyElement;

            if (!isCollectionElementEmpty)
            {
                // if the collection start element is not an empty element than read over the 
                // start element.
                this.XmlReader.Read();
            }

            return collectionStart;
        }

        /// <summary>
        /// Reads the end of a collection.
        /// </summary>
        /// <remarks>
        /// Pre-condition:  XmlNodeType.EndElement - The end element of the collection.
        ///                 XmlNodeType.Element    - The start element of the collection, if the element is empty.
        /// Post-condition: Any                    - Next node after the end element of the collection.
        /// </remarks>
        internal void ReadCollectionEnd()
        {
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(true, XmlNodeType.EndElement);

            // read over the end tag of the collection or the start tag if the collection is empty.
            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads an item in the collection. 
        /// </summary>
        /// <param name="expectedItemType">The expected type of the item to read.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <returns>The value of the collection item that was read; this can be an ODataComplexValue, a primitive value or 'null'.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element    - The start element node of the item in the collection.
        /// Post-Condition: Any                    - The next node after the end tag of the item.
        /// </remarks>
        internal object ReadCollectionItem(IEdmTypeReference expectedItemType, CollectionWithoutExpectedTypeValidator collectionValidator)
        {
            this.XmlReader.AssertNotBuffering();
            this.AssertXmlCondition(XmlNodeType.Element);

            // the caller should guarantee that we are reading elements in the OData namespace or the custom namespace specified through the reader settings.
            Debug.Assert(this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace), "The 'element' node should be in the OData Metadata Namespace or in the user specified Metadata Namespace");

            // make sure that the item is named as 'element'.
            if (!this.XmlReader.LocalNameEquals(this.ODataCollectionItemElementName))
            {
                throw new ODataException(ODataErrorStrings.ODataAtomCollectionDeserializer_WrongCollectionItemElementName(this.XmlReader.LocalName, this.XmlReader.ODataNamespace));
            }

            object item = this.ReadNonEntityValue(expectedItemType, this.duplicatePropertyNamesChecker, collectionValidator, /*validateNullValue*/ true);

            // read over the end tag of the element or the start tag if the element was empty.
            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();

            return item;
        }

        /// <summary>
        /// Reads from the Xml reader skipping all nodes until an Element or an EndElement in the OData Metadata namespace  
        /// is found or the reader.EOF is reached.
        /// </summary>
        internal void SkipToElementInODataMetadataNamespace()
        {
            this.XmlReader.AssertNotBuffering();

            do
            {
                switch (this.XmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                        {
                            return;
                        }

                        // skip anything which is not in the OData Metadata Namespace.
                        this.XmlReader.Skip();

                        break;

                    case XmlNodeType.EndElement:
                        return;

                    default:
                        this.XmlReader.Skip();
                        break;
                }
            }
            while (!this.XmlReader.EOF);
        }
    }
}
