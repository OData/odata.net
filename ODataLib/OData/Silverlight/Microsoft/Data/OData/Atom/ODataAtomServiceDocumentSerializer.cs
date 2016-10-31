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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for service documents.
    /// </summary>
    internal sealed class ODataAtomServiceDocumentSerializer : ODataAtomSerializer
    {
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
            DebugUtils.CheckNoExternalCallers();

            this.atomServiceDocumentMetadataSerializer = new ODataAtomServiceDocumentMetadataSerializer(atomOutputContext);
        }

        /// <summary>
        /// Writes a service document in ATOM/XML format.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        internal void WriteServiceDocument(ODataWorkspace defaultWorkspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(defaultWorkspace != null, "defaultWorkspace != null");

            IEnumerable<ODataResourceCollectionInfo> collections = defaultWorkspace.Collections;

            this.WritePayloadStart();

            // <app:service>
            this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.AtomPublishingServiceElementName, AtomConstants.AtomPublishingNamespace);

            // xml:base=...
            if (this.MessageWriterSettings.BaseUri != null)
            {
                this.XmlWriter.WriteAttributeString(AtomConstants.XmlBaseAttributeName, AtomConstants.XmlNamespace, this.MessageWriterSettings.BaseUri.AbsoluteUri);
            }

            // xmlns=http://www.w3.org/2007/app
            this.XmlWriter.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, AtomConstants.XmlNamespacesNamespace, AtomConstants.AtomPublishingNamespace);

            // xmlns:atom="http://www.w3.org/2005/Atom"
            this.XmlWriter.WriteAttributeString(
                AtomConstants.NonEmptyAtomNamespacePrefix,
                AtomConstants.XmlNamespacesNamespace,
                AtomConstants.AtomNamespace);

            // <app:workspace>
            this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.AtomPublishingWorkspaceElementName, AtomConstants.AtomPublishingNamespace);

            this.atomServiceDocumentMetadataSerializer.WriteWorkspaceMetadata(defaultWorkspace);

            if (collections != null)
            {
                foreach (ODataResourceCollectionInfo collectionInfo in collections)
                {
                    // validate that the collection has a non-null url.
                    ValidationUtils.ValidateResourceCollectionInfo(collectionInfo);

                    // <app:collection>
                    this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.AtomPublishingCollectionElementName, AtomConstants.AtomPublishingNamespace);

                    // The name of the collection is the entity set name; The href of the <app:collection> element must be the link for the entity set.
                    // Since we model the collection as having a 'Name' (for JSON) we require a base Uri for Atom/Xml.
                    this.XmlWriter.WriteAttributeString(AtomConstants.AtomHRefAttributeName, this.UriToUrlAttributeValue(collectionInfo.Url));

                    this.atomServiceDocumentMetadataSerializer.WriteResourceCollectionMetadata(collectionInfo);

                    // </app:collection>
                    this.XmlWriter.WriteEndElement();
                }
            }

            // </app:workspace>
            this.XmlWriter.WriteEndElement();

            // </app:service>
            this.XmlWriter.WriteEndElement();

            this.WritePayloadEnd();
        }
    }
}
