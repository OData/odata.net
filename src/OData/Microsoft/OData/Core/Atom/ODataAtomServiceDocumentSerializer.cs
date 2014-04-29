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
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for service documents.
    /// </summary>
    internal sealed class ODataAtomServiceDocumentSerializer : ODataAtomSerializer
    {
        /// <summary>The context uri builder to use.</summary>
        private readonly ODataContextUriBuilder contextUriBuilder;

        /// <summary>
        /// The serializer for service document ATOM metadata.
        /// </summary>
        private readonly ODataAtomServiceDocumentMetadataSerializer atomServiceDocumentMetadataSerializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomServiceDocumentSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
            this.atomServiceDocumentMetadataSerializer = new ODataAtomServiceDocumentMetadataSerializer(atomOutputContext);

            // DEVNOTE: grab this early so that any validation errors are thrown at creation time rather than when Write___ is called.
            this.contextUriBuilder = atomOutputContext.CreateContextUriBuilder();
        }

        /// <summary>
        /// Writes a service document in ATOM/XML format.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        internal void WriteServiceDocument(ODataServiceDocument serviceDocument)
        {
            Debug.Assert(serviceDocument != null, "serviceDocument != null");

            this.WritePayloadStart();

            // <app:service>
            this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.AtomPublishingServiceElementName, AtomConstants.AtomPublishingNamespace);

            // xml:base=...
            if (this.MessageWriterSettings.PayloadBaseUri != null)
            {
                this.XmlWriter.WriteAttributeString(AtomConstants.XmlBaseAttributeName, AtomConstants.XmlNamespace, this.MessageWriterSettings.PayloadBaseUri.AbsoluteUri);
            }

            // xmlns=http://www.w3.org/2007/app
            this.XmlWriter.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, AtomConstants.XmlNamespacesNamespace, AtomConstants.AtomPublishingNamespace);

            // xmlns:atom="http://www.w3.org/2005/Atom"
            this.XmlWriter.WriteAttributeString(
                AtomConstants.NonEmptyAtomNamespacePrefix,
                AtomConstants.XmlNamespacesNamespace,
                AtomConstants.AtomNamespace);

            this.XmlWriter.WriteAttributeString(
                AtomConstants.ODataMetadataNamespacePrefix,
                AtomConstants.XmlNamespacesNamespace,
                AtomConstants.ODataMetadataNamespace);

            // metadata:context=...
            this.WriteContextUriProperty(this.contextUriBuilder.BuildContextUri(ODataPayloadKind.ServiceDocument));

            // <app:serviceDocument>
            this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.AtomPublishingWorkspaceElementName, AtomConstants.AtomPublishingNamespace);

            this.atomServiceDocumentMetadataSerializer.WriteServiceDocumentMetadata(serviceDocument);

            if (serviceDocument.EntitySets != null)
            {
                foreach (ODataEntitySetInfo collectionInfo in serviceDocument.EntitySets)
                {
                    this.WriteEntitySetInfo(collectionInfo);
                }
            }

            if (serviceDocument.Singletons != null)
            {
                foreach (ODataSingletonInfo singletonInfo in serviceDocument.Singletons)
                {
                    this.WriteSingletonInfo(singletonInfo);
                }
            }

            HashSet<string> functionImportsWritten = new HashSet<string>(StringComparer.Ordinal);

            if (serviceDocument.FunctionImports != null)
            {
                foreach (ODataFunctionImportInfo functionImportInfo in serviceDocument.FunctionImports)
                {
                    if (functionImportInfo == null)
                    {
                        throw new ODataException(Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
                    }

                    if (!functionImportsWritten.Contains(functionImportInfo.Name))
                    {
                        functionImportsWritten.Add(functionImportInfo.Name);
                        this.WriteFunctionImportInfo(functionImportInfo);
                    }
                }
            }

            // </app:serviceDocument>
            this.XmlWriter.WriteEndElement();

            // </app:service>
            this.XmlWriter.WriteEndElement();

            this.WritePayloadEnd();
        }

        /// <summary>
        /// Writes a entity set in service document.
        /// </summary>
        /// <param name="entitySetInfo">The entity set info to write.</param>
        private void WriteEntitySetInfo(ODataEntitySetInfo entitySetInfo)
        {
            // validate that the resource has a non-null url.
#pragma warning disable 618
            ValidationUtils.ValidateServiceDocumentElement(entitySetInfo, ODataFormat.Atom);
#pragma warning restore 618

            // <app:collection>
            this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.AtomPublishingCollectionElementName, AtomConstants.AtomPublishingNamespace);

            // The name of the collection is the resource name; The href of the <app:collection> element must be the link for the entity set.
            // Since we model the collection as having a 'Name' (for JSON) we require a base Uri for Atom/Xml.
            this.XmlWriter.WriteAttributeString(AtomConstants.AtomHRefAttributeName, this.UriToUrlAttributeValue(entitySetInfo.Url));

            this.atomServiceDocumentMetadataSerializer.WriteEntitySetInfoMetadata(entitySetInfo);

            // </app:collection>
            this.XmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes a singleton resource in service document.
        /// </summary>
        /// <param name="singletonInfo">The singleton resource to write.</param>
        private void WriteSingletonInfo(ODataSingletonInfo singletonInfo)
        {
            WriteNonEntitySetInfoElement(singletonInfo, AtomConstants.AtomServiceDocumentSingletonElementName);
        }

        /// <summary>
        /// Writes a function import resource in service document.
        /// </summary>
        /// <param name="functionInfo">The function import resource to write.</param>
        private void WriteFunctionImportInfo(ODataFunctionImportInfo functionInfo)
        {
            WriteNonEntitySetInfoElement(functionInfo, AtomConstants.AtomServiceDocumentFunctionImportElementName);
        }

        /// <summary>
        /// Writes a service document element in service document.
        /// </summary>
        /// <param name="serviceDocumentElement">The serviceDocument element resource to write.</param>
        /// <param name="elementName">The element name of the service document element to write.</param>
        private void WriteNonEntitySetInfoElement(ODataServiceDocumentElement serviceDocumentElement, string elementName)
        {
            // validate that the resource has a non-null url.
#pragma warning disable 618
            ValidationUtils.ValidateServiceDocumentElement(serviceDocumentElement, ODataFormat.Atom);
#pragma warning restore 618

            // <metadata:elementName>
            this.XmlWriter.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, elementName, AtomConstants.ODataMetadataNamespace);

            // The name of the elementName is the resource name; The href of the <m:elementName> element must be the link for the elementName.
            // Since we model the collection as having a 'Name' (for JSON) we require a base Uri for Atom/Xml.
            this.XmlWriter.WriteAttributeString(AtomConstants.AtomHRefAttributeName, this.UriToUrlAttributeValue(serviceDocumentElement.Url));

            // TODO: According to the V4 spec we might want to omit writing this if the Url is the same as the Name, writing it always for now.
            this.atomServiceDocumentMetadataSerializer.WriteTextConstruct(AtomConstants.NonEmptyAtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, serviceDocumentElement.Name);

            // </metadata:elementName>
            this.XmlWriter.WriteEndElement();
        }
    }
}
