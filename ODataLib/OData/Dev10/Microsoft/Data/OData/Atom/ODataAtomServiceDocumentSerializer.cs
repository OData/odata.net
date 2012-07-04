//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
