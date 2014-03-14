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
    /// OData ATOM serializer for entity reference links.
    /// </summary>
    internal sealed class ODataAtomEntityReferenceLinkSerializer : ODataAtomSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomEntityReferenceLinkSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Writes a single Uri in response to a $links query.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write out.</param>
        internal void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            this.WritePayloadStart();
            this.WriteEntityReferenceLink(entityReferenceLink, true);
            this.WritePayloadEnd();
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $links query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="entityReferenceLinks">The entity reference links to write.</param>
        internal void WriteEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");

            this.WritePayloadStart();

            // <links> ...
            this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.ODataLinksElementName, this.MessageWriterSettings.WriterBehavior.ODataNamespace);

            // xmlns=
            this.XmlWriter.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, this.MessageWriterSettings.WriterBehavior.ODataNamespace);

            if (entityReferenceLinks.Count.HasValue)
            {
                // <m:count>
                this.WriteCount(entityReferenceLinks.Count.Value, true);
            }

            IEnumerable<ODataEntityReferenceLink> entityReferenceLinkEnumerable = entityReferenceLinks.Links;
            if (entityReferenceLinkEnumerable != null)
            {
                foreach (ODataEntityReferenceLink entityReferenceLink in entityReferenceLinkEnumerable)
                {
                    WriterValidationUtils.ValidateEntityReferenceLinkNotNull(entityReferenceLink);
                    this.WriteEntityReferenceLink(entityReferenceLink, false);
                }
            }

            if (entityReferenceLinks.NextPageLink != null)
            {
                // <d:next>
                string nextLink = this.UriToUrlAttributeValue(entityReferenceLinks.NextPageLink);
                this.XmlWriter.WriteElementString(string.Empty, AtomConstants.ODataNextLinkElementName, this.MessageWriterSettings.WriterBehavior.ODataNamespace, nextLink);
            }

            // </links>
            this.XmlWriter.WriteEndElement();

            this.WritePayloadEnd();
        }

        /// <summary>
        /// Writes a single Uri in response to a $links query.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write out.</param>
        /// <param name="isTopLevel">
        /// A flag indicating whether the link is written as top-level element or not; 
        /// this controls whether to include namespace declarations etc.
        /// </param>
        private void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink, bool isTopLevel)
        {
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);

            // For backward compatibility with WCF DS, the client needs to write the top-level uri element in the metadata namespace.
            string uriElementNamespaceUri = (this.UseClientFormatBehavior && isTopLevel)
                ? AtomConstants.ODataMetadataNamespace
                : this.MessageWriterSettings.WriterBehavior.ODataNamespace;

            // <uri ...
            this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.ODataUriElementName, uriElementNamespaceUri);

            if (isTopLevel)
            {
                // xmlns=
                this.XmlWriter.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, uriElementNamespaceUri);
            }

            this.XmlWriter.WriteString(this.UriToUrlAttributeValue(entityReferenceLink.Url));

            // </uri>
            this.XmlWriter.WriteEndElement();
        }
    }
}
