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
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for ATOM metadata in a feed
    /// </summary>
    internal sealed class ODataAtomFeedMetadataSerializer : ODataAtomMetadataSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomFeedMetadataSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
        }

        /// <summary>
        /// Write the given feed metadata in atom format
        /// </summary>
        /// <param name="feedMetadata">The metadata to write.</param>
        /// <param name="feed">The feed for which to write the meadata or null if it is the metadata of an atom:source element.</param>
        /// <param name="updatedTime">Value for the atom:updated element.</param>
        /// <param name="authorWritten">Set to true if the author element was written, false otherwise.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Does not make sense to break up writing the various parts of feed metadata. Very linear code; complexity not high.")]
        internal void WriteFeedMetadata(AtomFeedMetadata feedMetadata, ODataFeed feed, string updatedTime, out bool authorWritten)
        {
            Debug.Assert(feedMetadata != null, "Feed metadata must not be null!");

            // <atom:id>text</atom:id>
            // NOTE: this is the Id of the feed. For a regular feed this is stored on the feed itself;
            // if used in the context of an <atom:source> element it is stored in metadata
            Uri id = feed == null ? feedMetadata.SourceId : feed.Id;
            this.WriteElementWithTextContent(
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomIdElementName,
                AtomConstants.AtomNamespace,
                id == null ? null : UriUtils.UriToString(id));

            // <atom:title>text</atom:title>
            // NOTE: write an empty element if no title is specified since the element is required
            this.WriteTextConstruct(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, feedMetadata.Title);

            if (feedMetadata.Subtitle != null)
            {
                // <atom:subtitle>text</atom:subtitle>
                this.WriteTextConstruct(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomSubtitleElementName, AtomConstants.AtomNamespace, feedMetadata.Subtitle);
            }

            // <atom:updated>date</atom:updated>
            // NOTE: the <updated> element is required and if not specified we use a single 'default/current' date/time for the whole payload.
            string updated = feedMetadata.Updated.HasValue ? ODataAtomConvert.ToAtomString(feedMetadata.Updated.Value) : updatedTime;
            this.WriteElementWithTextContent(
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomUpdatedElementName,
                AtomConstants.AtomNamespace,
                updated);

            AtomLinkMetadata selfLinkMetadata = feedMetadata.SelfLink;
            if (selfLinkMetadata != null)
            {
                AtomLinkMetadata mergedSelfLinkMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(
                    selfLinkMetadata,
                    AtomConstants.AtomSelfRelationAttributeValue,
                    null /* href */,
                    null /* title */,
                    null /* media type */);
                this.WriteAtomLink(mergedSelfLinkMetadata, null /*etag*/);
            }

            IEnumerable<AtomLinkMetadata> links = feedMetadata.Links;
            if (links != null)
            {
                foreach (AtomLinkMetadata link in links)
                {
                    // DeltaLink is written from ODataFeed, so it shouldn't be written again from AtomFeedMetadata.
                    if (link.Relation != AtomConstants.AtomDeltaRelationAttributeValue)
                    {
                        // <atom:link>...</atom:link>
                        this.WriteAtomLink(link, null /* etag */);
                    }
                }
            }

            IEnumerable<AtomCategoryMetadata> categories = feedMetadata.Categories;
            if (categories != null)
            {
                foreach (AtomCategoryMetadata category in categories)
                {
                    // <atom:category term="..." scheme="..." label="..."></atom:category>
                    this.WriteCategory(category);
                }
            }

            Uri logo = feedMetadata.Logo;
            if (logo != null)
            {
                // <atom:logo>Uri</atom:logo>
                this.WriteElementWithTextContent(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomLogoElementName,
                    AtomConstants.AtomNamespace,
                    this.UriToUrlAttributeValue(logo));
            }

            if (feedMetadata.Rights != null)
            {
                // <atom:rights>rights</atom:rights>
                this.WriteTextConstruct(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomRightsElementName,
                    AtomConstants.AtomNamespace,
                    feedMetadata.Rights);
            }

            IEnumerable<AtomPersonMetadata> contributors = feedMetadata.Contributors;
            if (contributors != null)
            {
                foreach (AtomPersonMetadata contributor in contributors)
                {
                    // <atom:contributor>contributor data</atom:contributor>
                    this.XmlWriter.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomContributorElementName, AtomConstants.AtomNamespace);
                    this.WritePersonMetadata(contributor);
                    this.XmlWriter.WriteEndElement();
                }
            }

            AtomGeneratorMetadata generator = feedMetadata.Generator;
            if (generator != null)
            {
                // <atom:generator uri="..." version="...">name</atom:generator>
                this.XmlWriter.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomGeneratorElementName, AtomConstants.AtomNamespace);
                if (generator.Uri != null)
                {
                    this.XmlWriter.WriteAttributeString(AtomConstants.AtomGeneratorUriAttributeName, this.UriToUrlAttributeValue(generator.Uri));
                }

                if (!string.IsNullOrEmpty(generator.Version))
                {
                    this.XmlWriter.WriteAttributeString(AtomConstants.AtomGeneratorVersionAttributeName, generator.Version);
                }

                ODataAtomWriterUtils.WriteString(this.XmlWriter, generator.Name);
                this.XmlWriter.WriteEndElement();
            }

            Uri icon = feedMetadata.Icon;
            if (icon != null)
            {
                // <atom:icon>Uri</atom:icon>
                this.WriteElementWithTextContent(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomIconElementName,
                    AtomConstants.AtomNamespace,
                    this.UriToUrlAttributeValue(icon));
            }

            IEnumerable<AtomPersonMetadata> authors = feedMetadata.Authors;
            authorWritten = false;
            if (authors != null)
            {
                foreach (AtomPersonMetadata author in authors)
                {
                    // <atom:author>author data</atom:author>
                    authorWritten = true;
                    this.XmlWriter.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorElementName, AtomConstants.AtomNamespace);
                    this.WritePersonMetadata(author);
                    this.XmlWriter.WriteEndElement();
                }
            }
        }
    }
}
