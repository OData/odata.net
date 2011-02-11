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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    #endregion Namespaces.

    /// <summary>
    /// Helper methods used by the OData writer to write ATOM metadata.
    /// </summary>
    internal static class ODataAtomWriterMetadataUtils
    {
        /// <summary>
        /// Writes the 'atom:category' element given category metadata.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="category">The category information to write.</param>
        internal static void WriteCategory(XmlWriter writer, AtomCategoryMetadata category)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(category != null, "Category must not be null.");

            WriteCategory(writer, category.Term, category.Scheme, category.Label);
        }

        /// <summary>
        /// Writes the 'atom:category' element with the specified attributes.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="term">The value for the 'term' attribute (required).</param>
        /// <param name="scheme">The value for the 'scheme' attribute (optional).</param>
        /// <param name="label">The value for the 'label' attribute (optional).</param>
        internal static void WriteCategory(XmlWriter writer, string term, string scheme, string label)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            writer.WriteStartElement(
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomCategoryElementName,
                AtomConstants.AtomNamespace);

            if (term == null)
            {
                throw new ODataException(Strings.ODataAtomWriterMetadataUtils_CategoryMustSpecifyTerm);
            }

            writer.WriteAttributeString(
                AtomConstants.AtomCategoryTermAttributeName,
                term);

            if (scheme != null)
            {
                writer.WriteAttributeString(AtomConstants.AtomCategorySchemeAttributeName, scheme);
            }

            if (label != null)
            {
                writer.WriteAttributeString(AtomConstants.AtomCategoryLabelAttributeName, label);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the ATOM metadata for an entry
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="entry">The entry for which to write the metadata.</param>
        /// <param name="epmEntryMetadata">The ATOM metadata for the entry which came from EPM.</param>
        internal static void WriteEntryMetadata(XmlWriter writer, Uri baseUri, ODataEntry entry, AtomEntryMetadata epmEntryMetadata)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // TODO, ckerer: implement the rule around authors (an entry has to have an author directly or in the <entry:source> unless the feed has an author).
            //               currently we make all entries have an author.
            AtomEntryMetadata customEntryMetadata = entry.GetAnnotation<AtomEntryMetadata>();
            AtomEntryMetadata entryMetadata = ODataAtomWriterMetadataEpmMergeUtils.MergeCustomAndEpmEntryMetadata(customEntryMetadata, epmEntryMetadata);
            if (entryMetadata == null)
            {
                // write all required metadata elements with default content

                // <atom:title></atom:title>
                ODataAtomWriterUtils.WriteEmptyElement(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace);

                // <atom:updated>dateTimeOffset</atom:updated>
                // NOTE: the <updated> element is required and if not specified the best we can do is to create a default
                //       one with the current date/time.
                ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomUpdatedElementName, AtomConstants.AtomNamespace, ODataAtomConvert.ToString(DateTimeOffset.UtcNow));

                WriteEmptyAuthor(writer);
            }
            else
            {
                // <atom:title>text</atom:title>
                // NOTE: writes an empty element even if no title was specified since the title is required
                ODataAtomWriterMetadataUtils.WriteTextConstruct(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, entryMetadata.Title);

                AtomTextConstruct summary = entryMetadata.Summary;
                if (summary != null)
                {
                    // <atom:summary>text</atom:summary>
                    ODataAtomWriterMetadataUtils.WriteTextConstruct(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomSummaryElementName, AtomConstants.AtomNamespace, summary);
                }

                DateTimeOffset? published = entryMetadata.Published;
                if (published.HasValue)
                {
                    // <atom:published>dateTimeOffset</atom:published>
                    ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomPublishedElementName, AtomConstants.AtomNamespace, ODataAtomConvert.ToString(published.Value));
                }

                // <atom:updated>date</atom:updated>
                // NOTE: the <updated> element is required and if not specified the best we can do is to create a default
                //       one with the current date/time.
                DateTimeOffset updated = entryMetadata.Updated.HasValue ? entryMetadata.Updated.Value : DateTimeOffset.UtcNow;
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer,
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomUpdatedElementName,
                    AtomConstants.AtomNamespace,
                    ODataAtomConvert.ToString(updated));

                bool wroteAuthor = false;
                IEnumerable<AtomPersonMetadata> authors = entryMetadata.Authors;
                if (authors != null)
                {
                    foreach (AtomPersonMetadata author in authors)
                    {
                        if (author == null)
                        {
                            throw new ODataException(Strings.ODataAtomWriterMetadataUtils_AuthorMetadataMustNotContainNull);
                        }

                        // <atom:author>author data</atom:author>
                        writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorElementName, AtomConstants.AtomNamespace);
                        WritePersonMetadata(writer, baseUri, author);
                        writer.WriteEndElement();
                        wroteAuthor = true;
                    }
                }

                if (!wroteAuthor)
                {
                    // write empty authors since they are required
                    WriteEmptyAuthor(writer);
                }

                IEnumerable<AtomPersonMetadata> contributors = entryMetadata.Contributors;
                if (contributors != null)
                {
                    foreach (AtomPersonMetadata contributor in contributors)
                    {
                        if (contributor == null)
                        {
                            throw new ODataException(Strings.ODataAtomWriterMetadataUtils_ContributorMetadataMustNotContainNull);
                        }

                        // <atom:contributor>contributor data</atom:contributor>
                        writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomContributorElementName, AtomConstants.AtomNamespace);
                        WritePersonMetadata(writer, baseUri, contributor);
                        writer.WriteEndElement();
                    }
                }

                IEnumerable<AtomLinkMetadata> links = entryMetadata.Links;
                if (links != null)
                {
                    foreach (AtomLinkMetadata link in links)
                    {
                        if (link == null)
                        {
                            throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull);
                        }

                        // <atom:link>...</atom:link>
                        WriteAtomLinkMetadata(writer, baseUri, link);
                    }
                }

                IEnumerable<AtomCategoryMetadata> categories = entryMetadata.Categories;
                if (categories != null)
                {
                    foreach (AtomCategoryMetadata category in categories)
                    {
                        if (category == null)
                        {
                            throw new ODataException(Strings.ODataAtomWriterMetadataUtils_CategoryMetadataMustNotContainNull);
                        }

                        // <atom:category term="..." scheme="..." label="..."></atom:category>
                        WriteCategory(writer, category);
                    }
                }

                if (entryMetadata.Rights != null)
                {
                    // <atom:rights>rights</atom:rights>
                    ODataAtomWriterMetadataUtils.WriteTextConstruct(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomRightsElementName, AtomConstants.AtomNamespace, entryMetadata.Rights);
                }

                Uri icon = entryMetadata.Icon;
                if (icon != null)
                {
                    // <atom:icon>Uri</atom:icon>
                    ODataAtomWriterUtils.WriteElementWithTextContent(
                        writer, 
                        AtomConstants.AtomNamespacePrefix, 
                        AtomConstants.AtomIconElementName, 
                        AtomConstants.AtomNamespace,
                        AtomUtils.ToUrlAttributeValue(icon, baseUri));
                }

                AtomFeedMetadata source = entryMetadata.Source;
                if (source != null)
                {
                    // <atom:source>
                    writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomSourceElementName, AtomConstants.AtomNamespace);

                    WriteFeedMetadata(writer, baseUri, source, null);

                    // </atom:source>
                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Write the ATOM metadata for a feed
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="feed">The feed for which to write the metadata.</param>
        internal static void WriteFeedMetadata(XmlWriter writer, Uri baseUri, ODataFeed feed)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(feed != null, "feed != null");

            AtomFeedMetadata feedMetadata = feed.GetAnnotation<AtomFeedMetadata>();

            if (feedMetadata == null)
            {
                // create the required metadata elements with default values.

                // <atom:id>idValue</atom:id>
                Debug.Assert(!string.IsNullOrEmpty(feed.Id), "The feed Id should have been validated by now.");
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer,
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomIdElementName,
                    AtomConstants.AtomNamespace,
                    feed.Id);

                // <atom:title></atom:title>
                ODataAtomWriterUtils.WriteEmptyElement(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace);

                // <atom:updated>dateTimeOffset</atom:updated>
                ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomUpdatedElementName, AtomConstants.AtomNamespace, ODataAtomConvert.ToString(DateTimeOffset.UtcNow));
            }
            else
            {
                WriteFeedMetadata(writer, baseUri, feedMetadata, feed);
            }
        }

        /// <summary>
        /// Writes the specified start/end tags and the specified person metadata as content
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="personMetadata">The person metadata to write.</param>
        internal static void WritePersonMetadata(XmlWriter writer, Uri baseUri, AtomPersonMetadata personMetadata)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(personMetadata != null, "Person metadata must not be null.");

            // <atom:name>name of person</atom:name>
            // NOTE: write an empty element if no name is specified because the element is required.
            ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomPersonNameElementName, AtomConstants.AtomNamespace, personMetadata.Name);

            string uriString = personMetadata.UriFromEpm;
            if (uriString != null)
            {
                Debug.Assert(
                    personMetadata.Uri == null,
                    "If the internal UriFromEpm was used, then the Uri property must be left null. The merge between custom and EPM is probably wrong.");
            }
            else
            {
                Uri uri = personMetadata.Uri;
                if (uri != null)
                {
                    uriString = AtomUtils.ToUrlAttributeValue(uri, baseUri);
                }
            }

            if (uriString != null)
            {
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer,
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomPersonUriElementName,
                    AtomConstants.AtomNamespace,
                    uriString);
            }

            string email = personMetadata.Email;
            if (email != null)
            {
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer, 
                    AtomConstants.AtomNamespacePrefix, 
                    AtomConstants.AtomPersonEmailElementName, 
                    AtomConstants.AtomNamespace, 
                    email);
            }
        }

        /// <summary>
        /// Write the metadata of a link in ATOM format
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="linkMetadata">The link metadata to write.</param>
        internal static void WriteAtomLinkMetadata(XmlWriter writer, Uri baseUri, AtomLinkMetadata linkMetadata)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(linkMetadata != null, "Link metadata must not be null.");

            // <atom:link ... >
            writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomLinkElementName, AtomConstants.AtomNamespace);

            string linkHref = linkMetadata.Href == null ? null : AtomUtils.ToUrlAttributeValue(linkMetadata.Href, baseUri);
            WriteAtomLinkMetadataAttributes(writer, linkMetadata.Relation, linkHref, linkMetadata.HrefLang, linkMetadata.Title, linkMetadata.MediaType, linkMetadata.Length);

            // </atom:link>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the metadata for an OData link; makes sure any duplicate of the link's values duplicated in metadata are equal.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="link">The link for which to write the metadata.</param>
        internal static void WriteODataLinkMetadata(XmlWriter writer, Uri baseUri, ODataLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(link != null, "link != null");
            Debug.Assert(!string.IsNullOrEmpty(link.Name), "The link name was not verified yet.");
            Debug.Assert(link.Url != null, "The link Url was not verified yet.");

            string linkRelation = AtomUtils.ComputeODataLinkRelation(link);
            string linkHref = AtomUtils.ToUrlAttributeValue(link.Url, baseUri);
            string linkHrefLang = null;
            string linkType = AtomUtils.ComputeODataLinkType(link);
            string linkTitle = link.Name;
            int? linkLength = null;
            
            AtomLinkMetadata linkMetadata = link.Atom();
            if (linkMetadata != null)
            {
                Uri metadataHref = linkMetadata.Href;
                if (metadataHref != null)
                {
                    string metadataHrefString = AtomUtils.ToUrlAttributeValue(metadataHref, baseUri);
                    if (metadataHrefString != linkHref)
                    {
                        throw new ODataException(Strings.ODataAtomWriter_LinkMetadataHrefMustBeEqualWithLinkUrl(metadataHrefString, linkHref));
                    }
                }

                string metadataRelation = linkMetadata.Relation;
                if (metadataRelation != null && metadataRelation != linkRelation)
                {
                    throw new ODataException(Strings.ODataAtomWriter_LinkMetadataRelationMustBeEqualWithComputedRelation(metadataRelation, linkRelation));
                }

                string metadataType = linkMetadata.MediaType;
                if (metadataType != null && metadataType != linkType)
                {
                    throw new ODataException(Strings.ODataAtomWriter_LinkMetadataMediaTypeMustBeEqualWithComputedType(metadataType, linkType));
                }

                string metadataTitle = linkMetadata.Title;
                if (metadataTitle != null && metadataTitle != linkTitle)
                {
                    throw new ODataException(Strings.ODataAtomWriter_LinkMetadataTitleMustBeEqualWithLinkName(metadataTitle, linkTitle));
                }

                linkHrefLang = linkMetadata.HrefLang;
                linkLength = linkMetadata.Length;
            }

            WriteAtomLinkMetadataAttributes(writer, linkRelation, linkHref, linkHrefLang, linkTitle, linkType, linkLength);
        }

        /// <summary>
        /// Write the metadata for an OData association link; makes sure any duplicate of the link's values duplicated in metadata are equal.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="entry">The entry for which to write the association link.</param>
        /// <param name="associationLink">The association link for which to write the metadata.</param>
        internal static void WriteODataAssociationLinkMetadata(XmlWriter writer, Uri baseUri, ODataEntry entry, ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(associationLink != null, "link != null");
            Debug.Assert(!string.IsNullOrEmpty(associationLink.Name), "The link name was not verified yet.");
            Debug.Assert(associationLink.Url != null, "The link Url was not verified yet.");

            string linkRelation = AtomUtils.ComputeODataAssociationLinkRelation(associationLink);
            string linkHref = AtomUtils.ToUrlAttributeValue(associationLink.Url, baseUri);
            string linkHrefLang = null;
            string linkType = MimeConstants.MimeApplicationXml;
            string linkTitle = associationLink.Name;
            int? linkLength = null;

            AtomLinkMetadata linkMetadata = null;
            AtomEntryMetadata entryMetadata = entry.Atom();
            if (entryMetadata != null)
            {
                // TODO: Determine the link metadata from the entry
            }

            if (linkMetadata != null)
            {
                Uri metadataHref = linkMetadata.Href;
                if (metadataHref != null)
                {
                    string metadataHrefString = AtomUtils.ToUrlAttributeValue(metadataHref, baseUri);
                    if (metadataHrefString != linkHref)
                    {
                        throw new ODataException(Strings.ODataAtomWriter_LinkMetadataHrefMustBeEqualWithLinkUrl(metadataHrefString, linkHref));
                    }
                }

                string metadataRelation = linkMetadata.Relation;
                if (metadataRelation != null && metadataRelation != linkRelation)
                {
                    throw new ODataException(Strings.ODataAtomWriter_LinkMetadataRelationMustBeEqualWithComputedRelation(metadataRelation, linkRelation));
                }

                string metadataType = linkMetadata.MediaType;
                if (metadataType != null && metadataType != linkType)
                {
                    throw new ODataException(Strings.ODataAtomWriter_LinkMetadataMediaTypeMustBeEqualWithComputedType(metadataRelation, linkType));
                }

                string metadataTitle = linkMetadata.Title;
                if (metadataTitle != null && metadataTitle != linkTitle)
                {
                    throw new ODataException(Strings.ODataAtomWriter_LinkMetadataTitleMustBeEqualWithLinkName(metadataTitle, linkTitle));
                }

                linkHrefLang = linkMetadata.HrefLang;
                linkLength = linkMetadata.Length;
            }

            WriteAtomLinkMetadataAttributes(writer, linkRelation, linkHref, linkHrefLang, linkTitle, linkType, linkLength);
        }

        /// <summary>
        /// Writes an Xml element with the specified primitive value as content.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="prefix">The prefix for the element's namespace.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace of the element.</param>
        /// <param name="textConstruct">The <see cref="AtomTextConstruct"/> value to be used as element content.</param>
        internal static void WriteTextConstruct(XmlWriter writer, string prefix, string localName, string ns, AtomTextConstruct textConstruct)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(prefix != null, "prefix != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");
            Debug.Assert(!string.IsNullOrEmpty(ns), "!string.IsNullOrEmpty(ns)");

            writer.WriteStartElement(prefix, localName, ns);

            if (textConstruct != null)
            {
                AtomTextConstructKind textKind = textConstruct.Kind;

                writer.WriteAttributeString(AtomConstants.AtomTypeAttributeName, AtomValueUtils.ToString(textConstruct.Kind));

                string textValue = textConstruct.Text ?? string.Empty;
                if (textKind == AtomTextConstructKind.Xhtml)
                {
                    writer.WriteRaw(textValue);
                }
                else
                {
                    writer.WriteString(textValue);
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the ATOM metadata for a single workspace element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="workspace">The workspace element to get the metadata for and write it.</param>
        internal static void WriteWorkspaceMetadata(XmlWriter writer, ODataWorkspace workspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(workspace != null, "workspace != null");

            AtomWorkspaceMetadata metadata = workspace.Atom();
            string title = null;
            if (metadata != null)
            {
                title = metadata.Title;
            }

            if (title == null)
            {
                title = AtomConstants.AtomWorkspaceDefaultTitle;
            }

            // <atom:title>title</atom:title>
            ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.NonEmptyAtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, title);
        }

        /// <summary>
        /// Writes the ATOM metadata for a single (resource) collection element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="collection">The collection element to get the metadata for and write it.</param>
        internal static void WriteCollectionMetadata(XmlWriter writer, ODataResourceCollectionInfo collection)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(collection != null, "collection != null");
            Debug.Assert(collection.Name != null, "collection.Name should have been validated at this point");

            // TODO: implement writing the rest of the NYI collection metadata [see AtomPub spec].
            AtomResourceCollectionMetadata metadata = collection.Atom();

            string title = null;
            if (metadata != null)
            {
                title = metadata.Title;
            }

            if (title == null)
            {
                title = collection.Name;
            }

            // <atom:title>title</atom:title>
            ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.NonEmptyAtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, title);
        }

        /// <summary>
        /// Write the given feed metadata in atom format
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="feedMetadata">The metadata to write.</param>
        /// <param name="feed">The feed for which to write the meadata or null if it is the metadata of an atom:source element.</param>
        private static void WriteFeedMetadata(XmlWriter writer, Uri baseUri, AtomFeedMetadata feedMetadata, ODataFeed feed)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(feedMetadata != null, "Feed metadata must not be null!");

            // <atom:id>text</atom:id>
            // NOTE: this is the Id of the feed. For a regular feed this is stored on the feed itself;
            // if used in the context of an <atom:source> element it is stored in metadata
            Debug.Assert(feed == null || !string.IsNullOrEmpty(feed.Id), "The feed Id should have been validated by now.");
            string id = feed == null ? feedMetadata.SourceId : feed.Id;
            ODataAtomWriterUtils.WriteElementWithTextContent(
                writer,
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomIdElementName,
                AtomConstants.AtomNamespace,
                id);

            // <atom:title>text</atom:title>
            // NOTE: write an empty element if no title is specified since the element is required
            ODataAtomWriterMetadataUtils.WriteTextConstruct(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, feedMetadata.Title);

            if (feedMetadata.Subtitle != null)
            {
                // <atom:subtitle>text</atom:subtitle>
                ODataAtomWriterMetadataUtils.WriteTextConstruct(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomSubtitleElementName, AtomConstants.AtomNamespace, feedMetadata.Subtitle);
            }

            // <atom:updated>date</atom:updated>
            // NOTE: the <updated> element is required and if not specified the best we can do is to create a default
            //       one with the current date/time.
            DateTimeOffset updated = feedMetadata.Updated.HasValue ? feedMetadata.Updated.Value : DateTimeOffset.UtcNow;
            ODataAtomWriterUtils.WriteElementWithTextContent(
                writer,
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomUpdatedElementName,
                AtomConstants.AtomNamespace,
                ODataAtomConvert.ToString(updated));

            IEnumerable<AtomPersonMetadata> authors = feedMetadata.Authors;
            if (authors != null)
            {
                foreach (AtomPersonMetadata author in authors)
                {
                    // <atom:author>author data</atom:author>
                    writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorElementName, AtomConstants.AtomNamespace);
                    WritePersonMetadata(writer, baseUri, author);
                    writer.WriteEndElement();
                }
            }

            IEnumerable<AtomLinkMetadata> links = feedMetadata.Links;
            if (links != null)
            {
                foreach (AtomLinkMetadata link in links)
                {
                    // <atom:link>...</atom:link>
                    WriteAtomLinkMetadata(writer, baseUri, link);
                }
            }

            IEnumerable<AtomCategoryMetadata> categories = feedMetadata.Categories;
            if (categories != null)
            {
                foreach (AtomCategoryMetadata category in categories)
                {
                    // <atom:category term="..." scheme="..." label="..."></atom:category>
                    WriteCategory(writer, category);
                }
            }

            Uri logo = feedMetadata.Logo;
            if (logo != null)
            {
                // <atom:logo>Uri</atom:logo>
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer, 
                    AtomConstants.AtomNamespacePrefix, 
                    AtomConstants.AtomLogoElementName, 
                    AtomConstants.AtomNamespace,
                    AtomUtils.ToUrlAttributeValue(logo, baseUri));
            }

            if (feedMetadata.Rights != null)
            {
                // <atom:rights>rights</atom:rights>
                ODataAtomWriterMetadataUtils.WriteTextConstruct(
                    writer, 
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
                    writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomContributorElementName, AtomConstants.AtomNamespace);
                    WritePersonMetadata(writer, baseUri, contributor);
                    writer.WriteEndElement();
                }
            }

            AtomGeneratorMetadata generator = feedMetadata.Generator;
            if (generator != null)
            {
                // <atom:generator uri="..." version="...">name</atom:generator>
                writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomGeneratorElementName, AtomConstants.AtomNamespace);
                if (generator.Uri != null)
                {
                    writer.WriteAttributeString(AtomConstants.AtomGeneratorUriAttributeName, AtomUtils.ToUrlAttributeValue(generator.Uri, baseUri));
                }

                if (!string.IsNullOrEmpty(generator.Version))
                {
                    writer.WriteAttributeString(AtomConstants.AtomGeneratorVersionAttributeName, generator.Version);
                }

                writer.WriteString(generator.Name);
                writer.WriteEndElement();
            }

            Uri icon = feedMetadata.Icon;
            if (icon != null)
            {
                // <atom:icon>Uri</atom:icon>
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer, 
                    AtomConstants.AtomNamespacePrefix, 
                    AtomConstants.AtomIconElementName, 
                    AtomConstants.AtomNamespace,
                    AtomUtils.ToUrlAttributeValue(icon, baseUri));
            }
        }

        /// <summary>
        /// Write the metadata attributes of a link in ATOM format
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="relation">The value for the 'rel' attribute.</param>
        /// <param name="href">The value for the 'href' attribute.</param>
        /// <param name="hrefLang">The value for the 'hreflang' attribute.</param>
        /// <param name="title">The value for the 'title' attribute.</param>
        /// <param name="mediaType">The value for the 'type' attribute.</param>
        /// <param name="length">The value for the 'length' attribute.</param>
        private static void WriteAtomLinkMetadataAttributes(XmlWriter writer, string relation, string href, string hrefLang, string title, string mediaType, int? length)
        {
            Debug.Assert(writer != null, "writer != null");

            // rel="..."
            if (relation != null)
            {
                writer.WriteAttributeString(AtomConstants.AtomLinkRelationAttributeName, relation);
            }

            // type="..."
            if (mediaType != null)
            {
                writer.WriteAttributeString(AtomConstants.AtomLinkTypeAttributeName, mediaType);
            }

            // title="..."
            if (title != null)
            {
                writer.WriteAttributeString(AtomConstants.AtomLinkTitleAttributeName, title);
            }

            // href="..."
            if (href == null)
            {
                throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkMustSpecifyHref);
            }

            writer.WriteAttributeString(AtomConstants.AtomHRefAttributeName, href);

            // hreflang="..."
            if (hrefLang != null)
            {
                writer.WriteAttributeString(AtomConstants.AtomLinkHrefLangAttributeName, hrefLang);
            }

            // length="..."
            if (length.HasValue)
            {
                writer.WriteAttributeString(AtomConstants.AtomLinkLengthAttributeName, ODataAtomConvert.ToString(length.Value));
            }
        }

        /// <summary>
        /// Write an empty author element that has the required name element
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        private static void WriteEmptyAuthor(XmlWriter writer)
        {
            // <atom:author>
            writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorElementName, AtomConstants.AtomNamespace);

            // <atom:Name></atom:Name>
            ODataAtomWriterUtils.WriteEmptyElement(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorNameElementName, AtomConstants.AtomNamespace);

            // </atom:author>
            writer.WriteEndElement();
        }
     }
}
