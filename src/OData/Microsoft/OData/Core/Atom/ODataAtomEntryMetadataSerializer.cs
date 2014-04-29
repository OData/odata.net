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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for ATOM metadata in an entry
    /// </summary>
    internal sealed class ODataAtomEntryMetadataSerializer : ODataAtomMetadataSerializer
    {
        /// <summary>
        /// Feed ATOM metadata serializer for serializing the atom:source element in an entry.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomFeedMetadataSerializer sourceMetadataSerializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomEntryMetadataSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
        }

        /// <summary>
        /// Feed ATOM metadata serializer for serializing the atom:source element in an entry.
        /// This is created on-demand only when needed, but then it's cached.
        /// </summary>
        private ODataAtomFeedMetadataSerializer SourceMetadataSerializer
        {
            get
            {
                return this.sourceMetadataSerializer ??
                       (this.sourceMetadataSerializer = new ODataAtomFeedMetadataSerializer(this.AtomOutputContext));
            }
        }

        /// <summary>
        /// Write the ATOM metadata for an entry
        /// </summary>
        /// <param name="entryMetadata">The entry metadata to write.</param>
        /// <param name="updatedTime">Value for the atom:updated element.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "No good way to refactor; logic should be kept together.")]
        internal void WriteEntryMetadata(AtomEntryMetadata entryMetadata, string updatedTime)
        {
            Debug.Assert(!string.IsNullOrEmpty(updatedTime), "!string.IsNullOrEmpty(updatedTime)");
#if DEBUG
            DateTimeOffset tempDateTimeOffset;
            Debug.Assert(DateTimeOffset.TryParse(updatedTime, out tempDateTimeOffset), "DateTimeOffset.TryParse(updatedTime, out tempDateTimeOffset)");
#endif

            // TODO: implement the rule around authors (an entry has to have an author directly or in the <entry:source> unless the feed has an author).
            //               currently we make all entries have an author.
            if (entryMetadata == null)
            {
                // write all required metadata elements with default content

                // <atom:title></atom:title>
                this.WriteEmptyElement(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomTitleElementName,
                    AtomConstants.AtomNamespace);

                // <atom:updated>dateTimeOffset</atom:updated>
                // NOTE: the <updated> element is required and if not specified we use a single 'default/current' date/time for the whole payload.
                this.WriteElementWithTextContent(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomUpdatedElementName,
                    AtomConstants.AtomNamespace,
                    updatedTime);

                this.WriteEmptyAuthor();
            }
            else
            {
                // <atom:title>text</atom:title>
                // NOTE: writes an empty element even if no title was specified since the title is required
                this.WriteTextConstruct(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, entryMetadata.Title);

                AtomTextConstruct summary = entryMetadata.Summary;
                if (summary != null)
                {
                    // <atom:summary>text</atom:summary>
                    this.WriteTextConstruct(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomSummaryElementName, AtomConstants.AtomNamespace, summary);
                }

                string published = entryMetadata.Published.HasValue
                        ? ODataAtomConvert.ToAtomString(entryMetadata.Published.Value)
                        : null;
                if (published != null)
                {
                    // <atom:published>dateTimeOffset</atom:published>
                    this.WriteElementWithTextContent(
                        AtomConstants.AtomNamespacePrefix,
                        AtomConstants.AtomPublishedElementName,
                        AtomConstants.AtomNamespace,
                        published);
                }

                // <atom:updated>date</atom:updated>
                string updated = entryMetadata.Updated.HasValue
                        ? ODataAtomConvert.ToAtomString(entryMetadata.Updated.Value)
                        : null;

                // NOTE: the <updated> element is required and if not specified we use a single 'default/current' date/time for the whole payload.
                updated = updated ?? updatedTime;
                this.WriteElementWithTextContent(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomUpdatedElementName,
                    AtomConstants.AtomNamespace,
                    updated);

                bool wroteAuthor = false;
                IEnumerable<AtomPersonMetadata> authors = entryMetadata.Authors;
                if (authors != null)
                {
                    foreach (AtomPersonMetadata author in authors)
                    {
                        if (author == null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomWriterMetadataUtils_AuthorMetadataMustNotContainNull);
                        }

                        // <atom:author>author data</atom:author>
                        this.XmlWriter.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorElementName, AtomConstants.AtomNamespace);
                        this.WritePersonMetadata(author);
                        this.XmlWriter.WriteEndElement();
                        wroteAuthor = true;
                    }
                }

                if (!wroteAuthor)
                {
                    // write empty authors since they are required
                    this.WriteEmptyAuthor();
                }

                IEnumerable<AtomPersonMetadata> contributors = entryMetadata.Contributors;
                if (contributors != null)
                {
                    foreach (AtomPersonMetadata contributor in contributors)
                    {
                        if (contributor == null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomWriterMetadataUtils_ContributorMetadataMustNotContainNull);
                        }

                        // <atom:contributor>contributor data</atom:contributor>
                        this.XmlWriter.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomContributorElementName, AtomConstants.AtomNamespace);
                        this.WritePersonMetadata(contributor);
                        this.XmlWriter.WriteEndElement();
                    }
                }

                IEnumerable<AtomLinkMetadata> links = entryMetadata.Links;
                if (links != null)
                {
                    foreach (AtomLinkMetadata link in links)
                    {
                        if (link == null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull);
                        }

                        // <atom:link>...</atom:link>
                        this.WriteAtomLink(link, null /*edit-link-etag*/);
                    }
                }

                IEnumerable<AtomCategoryMetadata> categories = entryMetadata.Categories;
                if (categories != null)
                {
                    foreach (AtomCategoryMetadata category in categories)
                    {
                        if (category == null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomWriterMetadataUtils_CategoryMetadataMustNotContainNull);
                        }

                        // <atom:category term="..." scheme="..." label="..."></atom:category>
                        this.WriteCategory(category);
                    }
                }

                if (entryMetadata.Rights != null)
                {
                    // <atom:rights>rights</atom:rights>
                    this.WriteTextConstruct(
                        AtomConstants.AtomNamespacePrefix,
                        AtomConstants.AtomRightsElementName,
                        AtomConstants.AtomNamespace,
                        entryMetadata.Rights);
                }

                AtomFeedMetadata source = entryMetadata.Source;
                if (source != null)
                {
                    // <atom:source>
                    this.XmlWriter.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomSourceElementName, AtomConstants.AtomNamespace);

                    bool authorWritten;
                    this.SourceMetadataSerializer.WriteFeedMetadata(source, null /* feed */, updatedTime, out authorWritten);

                    // </atom:source>
                    this.XmlWriter.WriteEndElement();
                }
            }
        }
    }
}
