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
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for entity reference links.
    /// </summary>
    internal sealed class ODataAtomEntityReferenceLinkSerializer : ODataAtomSerializer
    {
        /// <summary>The context uri builder to use.</summary>
        private readonly ODataContextUriBuilder contextUriBuilder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomEntityReferenceLinkSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
            // DEVNOTE: grab this early so that any validation errors are thrown at creation time rather than when Write___ is called.
            this.contextUriBuilder = atomOutputContext.CreateContextUriBuilder();
        }

        /// <summary>
        /// Writes a single Uri in response to a $ref query.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write out.</param>
        internal void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            this.WritePayloadStart();
            this.WriteEntityReferenceLink(entityReferenceLink, true);
            this.WritePayloadEnd();
        }

        /// <summary>
        /// Writes a set of links (Uris) in response to a $ref query; includes optional count and next-page-link information.
        /// </summary>
        /// <param name="entityReferenceLinks">The set of entity reference links to write out.</param>
        internal void WriteEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks)
        {
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");

            this.WritePayloadStart();

            // <feed> ...
            this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.AtomFeedElementName, AtomConstants.AtomNamespace);

            // xmlns:metadata=
            this.XmlWriter.WriteAttributeString(AtomConstants.XmlnsNamespacePrefix, AtomConstants.ODataMetadataNamespacePrefix, null, AtomConstants.ODataMetadataNamespace);

            // metadata:context
            this.WriteContextUriProperty(this.contextUriBuilder.BuildContextUri(ODataPayloadKind.EntityReferenceLinks));

            if (entityReferenceLinks.Count.HasValue)
            {
                // <m:count>
                this.WriteCount(entityReferenceLinks.Count.Value);
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
                this.XmlWriter.WriteElementString(string.Empty, AtomConstants.ODataNextLinkElementName, AtomConstants.ODataNamespace, nextLink);
            }

            // </feed>
            this.XmlWriter.WriteEndElement();

            this.WritePayloadEnd();
        }

        /// <summary>
        /// Writes a single Uri in response to a $ref query.
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

            if (isTopLevel)
            {
                // <metadata:ref ...
                this.XmlWriter.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataRefElementName, AtomConstants.ODataMetadataNamespace);

                // metadata:context
                this.WriteContextUriProperty(this.contextUriBuilder.BuildContextUri(ODataPayloadKind.EntityReferenceLink));
            }
            else
            {
                // <metadata:ref ...
                this.XmlWriter.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataRefElementName, null);
            }

            this.XmlWriter.WriteAttributeString(AtomConstants.AtomIdElementName, null, this.UriToUrlAttributeValue(entityReferenceLink.Url));

            // </uri>
            this.XmlWriter.WriteEndElement();
        }
    }
}
