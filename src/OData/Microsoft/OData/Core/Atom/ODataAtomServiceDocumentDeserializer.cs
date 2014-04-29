//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// /// OData ATOM deserializer for service document.
    /// </summary>
    internal sealed class ODataAtomServiceDocumentDeserializer : ODataAtomDeserializer
    {
        #region Atomized strings

        /// <summary>The name of the top-level service document element.</summary>
        private readonly string AtomPublishingServiceElementName;

        /// <summary>The name of the 'workspace' element of a service document.</summary>
        private readonly string AtomPublishingWorkspaceElementName;

        /// <summary>href attribute name in Atom.</summary>
        private readonly string AtomHRefAttributeName;

        /// <summary>The name of the 'collection' element of a service document.</summary>
        private readonly string AtomPublishingCollectionElementName;
        
        /// <summary>The name of the 'categories' element of a service document.</summary>
        private readonly string AtomPublishingCategoriesElementName;

        /// <summary>The name of the 'accept' element of a service document.</summary>
        private readonly string AtomPublishingAcceptElementName;

        /// <summary>The Atom Publishing Protocol (APP) namespace.</summary>
        private readonly string AtomPublishingNamespace;

        /// <summary>The ATOM namespace.</summary>
        private readonly string AtomNamespace;

        /// <summary>The name of the 'title' element of a service document.</summary>
        private readonly string AtomTitleElementName;

        /// <summary>The emtpy namespace used for attributes in no namespace.</summary>
        private readonly string EmptyNamespace;

        /// <summary>Atomized string representation for the OData metadata namespace.</summary>
        private readonly string ODataMetadataNamespace;

        /// <summary>Atomized string representation of the element name used for function import </summary>
        private readonly string ODataFunctionImportElementName;

        /// <summary>Atomized string representation of the element name used for singleton name </summary>
        private readonly string ODataSingletonElementName;

        /// <summary>Atomized string representation of the name attribute. </summary>
        private readonly string ODataNameAttribute;
        #endregion

        /// <summary>
        /// ATOM deserializer for ATOM metadata on service documents.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomServiceDocumentMetadataDeserializer serviceDocumentMetadataDeserializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomServiceDocumentDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.AtomPublishingServiceElementName = nameTable.Add(AtomConstants.AtomPublishingServiceElementName);
            this.AtomPublishingWorkspaceElementName = nameTable.Add(AtomConstants.AtomPublishingWorkspaceElementName);
            this.AtomPublishingCollectionElementName = nameTable.Add(AtomConstants.AtomPublishingCollectionElementName);
            this.AtomPublishingAcceptElementName = nameTable.Add(AtomConstants.AtomPublishingAcceptElementName);
            this.AtomPublishingCategoriesElementName = nameTable.Add(AtomConstants.AtomPublishingCategoriesElementName);
            this.AtomHRefAttributeName = nameTable.Add(AtomConstants.AtomHRefAttributeName);
            this.AtomPublishingNamespace = nameTable.Add(AtomConstants.AtomPublishingNamespace);
            this.AtomNamespace = nameTable.Add(AtomConstants.AtomNamespace);
            this.AtomTitleElementName = nameTable.Add(AtomConstants.AtomTitleElementName);
            this.EmptyNamespace = nameTable.Add(string.Empty);
            this.ODataMetadataNamespace = nameTable.Add(AtomConstants.ODataMetadataNamespace);
            this.ODataFunctionImportElementName = nameTable.Add(AtomConstants.AtomServiceDocumentFunctionImportElementName);
            this.ODataSingletonElementName = nameTable.Add(AtomConstants.AtomServiceDocumentSingletonElementName);
            this.ODataNameAttribute = nameTable.Add(AtomConstants.ODataNameAttribute);
        }

        /// <summary>
        /// ATOM deserializer for ATOM metadata on service documents.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomServiceDocumentMetadataDeserializer ServiceDocumentMetadataDeserializer
        {
            get
            {
                if (this.serviceDocumentMetadataDeserializer == null)
                {
                    this.serviceDocumentMetadataDeserializer = new ODataAtomServiceDocumentMetadataDeserializer(this.AtomInputContext);
                }

                return this.serviceDocumentMetadataDeserializer;
            }
        }

        /// <summary>
        /// Reads a service document.
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element - The start element of the service document.
        /// Post-Condtion:   XmlNodeType.None    - The reader must be at the end of the input.
        /// </remarks>   
        internal ODataServiceDocument ReadServiceDocument()
        {
            Debug.Assert(this.ReadingResponse, "Service document can only appear in a response message");
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");

            this.ReadPayloadStart();
            this.AssertXmlCondition(XmlNodeType.Element);

            // read the 'service' element.
            if (!this.XmlReader.NamespaceEquals(this.AtomPublishingNamespace)
                || !this.XmlReader.LocalNameEquals(this.AtomPublishingServiceElementName))
            {
                throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_ServiceDocumentRootElementWrongNameOrNamespace(
                    this.XmlReader.LocalName, this.XmlReader.NamespaceURI));
            }

            ODataServiceDocument serviceDocument = null;

            if (!this.XmlReader.IsEmptyElement)
            {
                // read over the start tag of the 'service' element.
                this.XmlReader.Read();
                serviceDocument = this.ReadWorkspace();
            }

            if (serviceDocument == null)
            {
                throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_MissingWorkspaceElement);
            }

            // skip anything which is not in the ATOM publishing namespace.
            this.SkipToElementInAtomPublishingNamespace();

            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.EndElement);
            
            // When we reach here, we should be at the end-tag of the service element.
            if (this.XmlReader.NodeType == XmlNodeType.Element)
            {
                Debug.Assert(this.XmlReader.NamespaceEquals(this.AtomPublishingNamespace), "The current Xml element should have been in the 'app' namespace");

                // We only support a single serviceDocument inside a service document.
                if (this.XmlReader.LocalNameEquals(this.AtomPublishingWorkspaceElementName))
                {
                    throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_MultipleWorkspaceElementsFound);
                }

                // Throw error, if we encounter any other element in the ATOM publishing namespace.
                throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument(this.XmlReader.LocalName));
            }
                
            // read over the end tag of the 'service' element.
            this.XmlReader.Read();

            this.ReadPayloadEnd();

            return serviceDocument;
        }

        /// <summary>
        /// Reads a workspace of a service document.
        /// </summary>
        /// <returns>An <see cref="ODataServiceDocument"/> representing the workspace of a service document.</returns>
        /// <remarks>
        /// Pre-Condition:  Any    - the next node after the service element.
        /// Post-Condition: Any    - The next node after the workspace element. 
        /// </remarks>
        private ODataServiceDocument ReadWorkspace()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");

            bool enableAtomMetadataReading = this.AtomInputContext.MessageReaderSettings.EnableAtomMetadataReading;

            // skip anything which is not in the ATOM publishing namespace.
            this.SkipToElementInAtomPublishingNamespace();

            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.EndElement);

            // if we already found an EndElement, it means that there is no serviceDocument.
            if (this.XmlReader.NodeType == XmlNodeType.EndElement)
            {
                return null;
            }

            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.NamespaceEquals(this.AtomPublishingNamespace), "The current element should have been in the Atom publishing namespace.");
                
            if (!this.XmlReader.LocalNameEquals(this.AtomPublishingWorkspaceElementName))
            {
                throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument(this.XmlReader.LocalName));
            }

            List<ODataEntitySetInfo> collections = new List<ODataEntitySetInfo>();
            List<ODataFunctionImportInfo> functionImportInfos = new List<ODataFunctionImportInfo>();
            List<ODataSingletonInfo> singletons = new List<ODataSingletonInfo>();
            AtomWorkspaceMetadata workspaceMetadata = null;

            if (enableAtomMetadataReading)
            {
                workspaceMetadata = new AtomWorkspaceMetadata();
            }

            if (!this.XmlReader.IsEmptyElement)
            {
                // read over the 'serviceDocument' element.
                this.XmlReader.ReadStartElement();

                do
                {
                    this.XmlReader.SkipInsignificantNodes();

                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.AtomPublishingNamespace))
                            {
                                if (this.XmlReader.LocalNameEquals(this.AtomPublishingCollectionElementName))
                                {
                                    ODataEntitySetInfo collection = this.ReadEntitySet();
                                    Debug.Assert(collection != null, "collection != null");
                                    collections.Add(collection);
                                }
                                else
                                {
                                    // Throw error if we find anything other then a 'collection' element in the Atom publishing namespace.
                                    throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace(this.XmlReader.LocalName));
                                }
                            }
                            else if (this.XmlReader.NamespaceEquals(this.ODataMetadataNamespace))
                            {
                                if (this.XmlReader.LocalNameEquals(this.ODataFunctionImportElementName))
                                {
                                    ODataFunctionImportInfo functionImportInfo = this.ReadFunctionImportInfo();
                                    Debug.Assert(functionImportInfo != null, "functionImportInfo != null");
                                    functionImportInfos.Add(functionImportInfo);
                                }
                                else if (this.XmlReader.LocalNameEquals(this.ODataSingletonElementName))
                                {
                                    ODataSingletonInfo singletonInfo = this.ReadSingletonInfo();
                                    Debug.Assert(singletonInfo != null, "singletonInfo != null");
                                    singletons.Add(singletonInfo);
                                }
                                else
                                {
                                    // Throw error if we find anything other then a 'function-import' or 'singleton' element in the odata metadata namespace.
                                    throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_UnexpectedODataElementInWorkspace(this.XmlReader.LocalName));
                                }
                            }
                            else if (enableAtomMetadataReading && this.XmlReader.NamespaceEquals(this.AtomNamespace))
                            {
                                if (this.XmlReader.LocalNameEquals(this.AtomTitleElementName))
                                {
                                    this.ServiceDocumentMetadataDeserializer.ReadTitleElementInWorkspace(workspaceMetadata);
                                }
                                else
                                {
                                    this.XmlReader.Skip();
                                }
                            }
                            else
                            {
                                // skip all other elements
                                this.XmlReader.Skip();
                            }

                            break;

                        case XmlNodeType.EndElement:
                            // end of 'serviceDocument' element.
                            break;

                        default:
                            // ignore all other nodes.
                            this.XmlReader.Skip();
                            break;
                    }
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            } // if (!this.XmlReader.IsEmptyElement)

            // read over the end tag of the serviceDocument element or the start tag if the serviceDocument element is empty.
            this.XmlReader.Read();

            ODataServiceDocument serviceDocument = new ODataServiceDocument
            {
                EntitySets = new ReadOnlyEnumerable<ODataEntitySetInfo>(collections),
                FunctionImports = new ReadOnlyCollection<ODataFunctionImportInfo>(functionImportInfos),
                Singletons = new ReadOnlyCollection<ODataSingletonInfo>(singletons)
            };

            if (enableAtomMetadataReading)
            {
                serviceDocument.SetAnnotation<AtomWorkspaceMetadata>(workspaceMetadata);
            }
                
            return serviceDocument;
        }

        /// <summary>
        /// Reads a entity set element of a service document.
        /// </summary>
        /// <returns>An <see cref="ODataEntitySetInfo"/> representing the entity set in a service document.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the collection element inside the service document.
        /// Post-Condition: Any    - The next node after the entity set element. 
        /// </remarks>   
        private ODataEntitySetInfo ReadEntitySet()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalNameEquals(this.AtomPublishingCollectionElementName), "Expected element named 'collection'.");
            Debug.Assert(this.XmlReader.NamespaceEquals(this.AtomPublishingNamespace), "Element 'collection' should be in the atom publishing namespace.");

            return this.ReadServiceDocumentElement<ODataEntitySetInfo>();
        }

        /// <summary>
        /// Reads a entity set element of a service document.
        /// </summary>
        /// <returns>An <see cref="ODataFunctionImportInfo"/> representing the functionImport in a service document.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the function import element inside the service document.
        /// Post-Condition: Any    - The next node after the function import element. 
        /// </remarks>   
        private ODataFunctionImportInfo ReadFunctionImportInfo()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalNameEquals(this.ODataFunctionImportElementName), "Expected element named 'function-import'.");
            Debug.Assert(this.XmlReader.NamespaceEquals(this.ODataMetadataNamespace), "Element 'collection' should be in the odata metadata namespace.");

            return ReadServiceDocumentElement<ODataFunctionImportInfo>();
        }

        /// <summary>
        /// Reads a entity set element of a service document.
        /// </summary>
        /// <returns>An <see cref="ODataSingletonInfo"/> representing the singleton in a service document.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the singleton element inside the service document.
        /// Post-Condition: Any    - The next node after the singleton element. 
        /// </remarks>   
        private ODataSingletonInfo ReadSingletonInfo()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalNameEquals(this.ODataSingletonElementName), "Expected element named 'singleton'.");
            Debug.Assert(this.XmlReader.NamespaceEquals(this.ODataMetadataNamespace), "Element 'collection' should be in the odata metadata namespace.");

            return ReadServiceDocumentElement<ODataSingletonInfo>();
        }

        /// <summary>
        /// Reads the service document element and returns the new element instance.
        /// </summary>
        /// <typeparam name="T">Type of service element to read.</typeparam>
        /// <returns>Service Element instance.</returns>
        private T ReadServiceDocumentElement<T>() where T : ODataServiceDocumentElement, new()
        {
            T serviceDocumentElement = new T();
            string href = this.XmlReader.GetAttribute(this.AtomHRefAttributeName, this.EmptyNamespace);
            ValidationUtils.ValidateServiceDocumentElementUrl(href);

            serviceDocumentElement.Url = this.ProcessUriFromPayload(href, this.XmlReader.XmlBaseUri);
            bool enableAtomMetadataReading = this.MessageReaderSettings.EnableAtomMetadataReading;

            string name = this.XmlReader.GetAttribute(this.ODataNameAttribute, this.ODataMetadataNamespace);
            serviceDocumentElement.Name = name;

            AtomResourceCollectionMetadata collectionMetadata = null;

            if (enableAtomMetadataReading)
            {
                collectionMetadata = new AtomResourceCollectionMetadata();
            }

            if (!this.XmlReader.IsEmptyElement)
            {
                // read over the service document element.
                this.XmlReader.ReadStartElement();

                bool atomTitlesReadAlready = false;

                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.AtomPublishingNamespace))
                            {
                                if (this.XmlReader.LocalNameEquals(this.AtomPublishingCategoriesElementName))
                                {
                                    if (enableAtomMetadataReading)
                                    {
                                        this.ServiceDocumentMetadataDeserializer.ReadCategoriesElementInCollection(collectionMetadata);
                                    }
                                    else
                                    {
                                        this.XmlReader.Skip();
                                    }
                                }
                                else if (this.XmlReader.LocalNameEquals(this.AtomPublishingAcceptElementName))
                                {
                                    if (enableAtomMetadataReading)
                                    {
                                        this.ServiceDocumentMetadataDeserializer.ReadAcceptElementInCollection(collectionMetadata);
                                    }
                                    else
                                    {
                                        this.XmlReader.Skip();
                                    }
                                }
                                else
                                {
                                    // Throw error if we find anything other then a 'app:categories' or an 'app:accept' element in the ATOM publishing namespace.
                                    throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_UnexpectedElementInResourceCollection(this.XmlReader.LocalName));
                                }
                            }
                            else if (this.XmlReader.NamespaceEquals(this.AtomNamespace))
                            {
                                if (this.XmlReader.LocalNameEquals(this.AtomTitleElementName))
                                {
                                    if (atomTitlesReadAlready)
                                    {
                                        throw new ODataException(Strings.ODataAtomServiceDocumentMetadataDeserializer_MultipleTitleElementsFound(AtomConstants.AtomPublishingCollectionElementName));
                                    }

                                    this.ServiceDocumentMetadataDeserializer.ReadTitleElementInCollection(collectionMetadata, serviceDocumentElement);
                                    atomTitlesReadAlready = true;
                                }
                                else
                                {
                                    // Skip all other elements in the atom namespace
                                    this.XmlReader.Skip();
                                }
                            }
                            else
                            {
                                // For now, skip all other elements.
                                this.XmlReader.Skip();
                            }

                            break;

                        case XmlNodeType.EndElement:
                            // end of service document element.
                            break;

                        default:
                            // ignore all other nodes.
                            this.XmlReader.Skip();
                            break;
                    }
                } 
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            }

            this.AssertXmlCondition(true, XmlNodeType.EndElement);

            // read over the end tag of the service document element or the start tag if the collection element is empty.
            this.XmlReader.Read();

            if (enableAtomMetadataReading)
            {
                serviceDocumentElement.SetAnnotation(collectionMetadata);
            }

            return serviceDocumentElement;
        }

        /// <summary>
        /// Reads from the Xml reader skipping all nodes until an Element or an EndElement in the ATOM
        /// publishing namespace is found.
        /// </summary>
        private void SkipToElementInAtomPublishingNamespace()
        {
            this.XmlReader.AssertNotBuffering();

            while (true)
            {
                switch (this.XmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (this.XmlReader.NamespaceEquals(this.AtomPublishingNamespace))
                        {
                            return;
                        }
                        else
                        {
                            // skip anything which is not in the ATOM publishing namespace.
                            this.XmlReader.Skip();
                        }

                        break;

                    case XmlNodeType.EndElement:
                        return;

                    default:
                        this.XmlReader.Skip();
                        break;
                }
            }
        }
    }
}
