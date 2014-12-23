//   OData .NET Libraries ver. 6.9
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
