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
    using System.Collections.Generic;
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
            DebugUtils.CheckNoExternalCallers();
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
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element - The start element of the service document.
        /// Post-Condtion:   XmlNodeType.None    - The reader must be at the end of the input.
        /// </remarks>   
        internal ODataWorkspace ReadServiceDocument()
        {
            DebugUtils.CheckNoExternalCallers();
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

            ODataWorkspace workspace = null;

            if (!this.XmlReader.IsEmptyElement)
            {
                // read over the start tag of the 'service' element.
                this.XmlReader.Read();
                workspace = this.ReadWorkspace();
            }

            if (workspace == null)
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

                // We only support a single workspace inside a service document.
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

            return workspace;
        }

        /// <summary>
        /// Reads a workspace of a service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the workspace of a service document.</returns>
        /// <remarks>
        /// Pre-Condition:  Any    - the next node after the service element.
        /// Post-Condition: Any    - The next node after the workspace element. 
        /// </remarks>
        private ODataWorkspace ReadWorkspace()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");

            bool enableAtomMetadataReading = this.AtomInputContext.MessageReaderSettings.EnableAtomMetadataReading;

            // skip anything which is not in the ATOM publishing namespace.
            this.SkipToElementInAtomPublishingNamespace();

            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.EndElement);

            // if we already found an EndElement, it means that there is no workspace.
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

            List<ODataResourceCollectionInfo> collections = new List<ODataResourceCollectionInfo>();
            AtomWorkspaceMetadata workspaceMetadata = null;

            if (enableAtomMetadataReading)
            {
                workspaceMetadata = new AtomWorkspaceMetadata();
            }

            if (!this.XmlReader.IsEmptyElement)
            {
                // read over the 'workspace' element.
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
                                    ODataResourceCollectionInfo collection = this.ReadCollectionElement();
                                    Debug.Assert(collection != null, "collection != null");
                                    collections.Add(collection);
                                }
                                else
                                {
                                    // Throw error if we find anything other then a 'collection' element in the Atom publishing namespace.
                                    throw new ODataException(Strings.ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace(this.XmlReader.LocalName));
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
                            // end of 'workspace' element.
                            break;

                        default:
                            // ignore all other nodes.
                            this.XmlReader.Skip();
                            break;
                    }
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            } // if (!this.XmlReader.IsEmptyElement)

            // read over the end tag of the workspace element or the start tag if the workspace element is empty.
            this.XmlReader.Read();

            ODataWorkspace workspace = new ODataWorkspace
            {
                Collections = new ReadOnlyEnumerable<ODataResourceCollectionInfo>(collections)
            };

            if (enableAtomMetadataReading)
            {
                workspace.SetAnnotation<AtomWorkspaceMetadata>(workspaceMetadata);
            }
                
            return workspace;
        }

        /// <summary>
        /// Reads a resource collection element of a workspace of the service document.
        /// </summary>
        /// <returns>An <see cref="ODataResourceCollectionInfo"/> representing the resource collection in a workspace of a service document.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the collection element inside the workspace.
        /// Post-Condition: Any    - The next node after the collection element. 
        /// </remarks>   
        private ODataResourceCollectionInfo ReadCollectionElement()
        {
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.XmlReader.LocalNameEquals(this.AtomPublishingCollectionElementName), "Expected element named 'collection'.");
            Debug.Assert(this.XmlReader.NamespaceEquals(this.AtomPublishingNamespace), "Element 'collection' should be in the atom publishing namespace.");

            ODataResourceCollectionInfo collectionInfo = new ODataResourceCollectionInfo();

            string href = this.XmlReader.GetAttribute(this.AtomHRefAttributeName, this.EmptyNamespace);
            ValidationUtils.ValidateResourceCollectionInfoUrl(href);

            collectionInfo.Url = ProcessUriFromPayload(href, this.XmlReader.XmlBaseUri);
            bool enableAtomMetadataReading = this.MessageReaderSettings.EnableAtomMetadataReading;

            AtomResourceCollectionMetadata collectionMetadata = null;

            if (enableAtomMetadataReading)
            {
                collectionMetadata = new AtomResourceCollectionMetadata();
            }

            if (!this.XmlReader.IsEmptyElement)
            {
                // read over the 'collection' element.
                this.XmlReader.ReadStartElement();

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
                                    this.ServiceDocumentMetadataDeserializer.ReadTitleElementInCollection(collectionMetadata, collectionInfo);
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
                            // end of 'collection' element.
                            break;

                        default:
                            // ignore all other nodes.
                            this.XmlReader.Skip();
                            break;
                    }
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            } // if (!this.XmlReader.IsEmptyElement)

            this.AssertXmlCondition(true, XmlNodeType.EndElement);

            // read over the end tag of the collection element or the start tag if the collection element is empty.
            this.XmlReader.Read();

            if (enableAtomMetadataReading)
            {
                collectionInfo.SetAnnotation(collectionMetadata);
            }

            return collectionInfo;
        }

        /// <summary>
        /// Reads from the Xml reader skipping all nodes until an Element or an EndElement in the ATOM
        /// publishing namespace is found.
        /// </summary>
        private void SkipToElementInAtomPublishingNamespace()
        {
            DebugUtils.CheckNoExternalCallers();
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
