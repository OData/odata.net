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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer to write ATOM metadata.
    /// </summary>
    internal static class ODataAtomWriterMetadataUtils
    {
        /// <summary>
        /// Creates a new <see cref="AtomLinkMetadata"/> instance by merging the given
        /// <paramref name="metadata"/> (if any) with the specified <paramref name="href"/>,
        /// <paramref name="relation"/> and (optional) <paramref name="title"/>.
        /// </summary>
        /// <param name="metadata">The metadata to merge with the <paramref name="href"/>, <paramref name="relation"/> and (optional) <paramref name="title"/>.</param>
        /// <param name="relation">The relation to use in the merged metadata.</param>
        /// <param name="href">The href to use in the merged metadata.</param>
        /// <param name="title">The (optional) title to use in the merged metadata.</param>
        /// <param name="mediaType">The (optional) media type to use in the merged metadata.</param>
        /// <returns>A new <see cref="AtomLinkMetadata"/> instance created by merging all the arguments.</returns>
        /// <remarks>
        /// If the <paramref name="metadata"/> already holds values for <paramref name="href"/>,
        /// <paramref name="relation"/>, <paramref name="title"/>, or <paramref name="mediaType"/> this method validates that they
        /// are the same as the ones specified in the method arguments.
        /// </remarks>
        internal static AtomLinkMetadata MergeLinkMetadata(
            AtomLinkMetadata metadata, 
            string relation,
            Uri href,
            string title,
            string mediaType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(relation != null, "relation != null");

            AtomLinkMetadata mergedMetadata = new AtomLinkMetadata(metadata);

            // set the relation
            string metadataRelation = mergedMetadata.Relation;
            if (metadataRelation != null)
            {
                // validate that the relations are the same
                if (string.CompareOrdinal(relation, metadataRelation) != 0)
                {
                    throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkRelationsMustMatch(relation, metadataRelation));
                }
            }
            else
            {
                mergedMetadata.Relation = relation;
            }

            // set the href if it was specified
            if (href != null)
            {
                Uri metadataHref = mergedMetadata.Href;
                if (metadataHref != null)
                {
                    // validate that the hrefs are the same
                    if (!href.Equals(metadataHref))
                    {
                        throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkHrefsMustMatch(href, metadataHref));
                    }
                }
                else
                {
                    mergedMetadata.Href = href;
                }
            }

            // set the title if it was specified
            if (title != null)
            {
                string metadataTitle = mergedMetadata.Title;
                if (metadataTitle != null)
                {
                    // validate that the relations are the same
                    if (string.CompareOrdinal(title, metadataTitle) != 0)
                    {
                        throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkTitlesMustMatch(title, metadataTitle));
                    }
                }
                else
                {
                    mergedMetadata.Title = title;
                }
            }

            // set the content type if it was specified
            if (mediaType != null)
            {
                string metadataMediaType = mergedMetadata.MediaType;
                if (metadataMediaType != null)
                {
                    // validate that the relations are the same
                    if (string.CompareOrdinal(mediaType, metadataMediaType) != 0)
                    {
                        throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkMediaTypesMustMatch(mediaType, metadataMediaType));
                    }
                }
                else
                {
                    mergedMetadata.MediaType = mediaType;
                }
            }

            return mergedMetadata;
        }

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

            WriteCategory(writer, AtomConstants.AtomNamespacePrefix, category.Term, category.Scheme, category.Label);
        }

        /// <summary>
        /// Writes the 'atom:category' element with the specified attributes.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="atomPrefix">The prefix to use for the 'category' element.</param>
        /// <param name="term">The value for the 'term' attribute (required).</param>
        /// <param name="scheme">The value for the 'scheme' attribute (optional).</param>
        /// <param name="label">The value for the 'label' attribute (optional).</param>
        internal static void WriteCategory(XmlWriter writer, string atomPrefix, string term, string scheme, string label)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            writer.WriteStartElement(
                atomPrefix,
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
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="entryMetadata">The entry metadata to write.</param>
        /// <param name="epmEntryMetadata">The ATOM metadata for the entry which came from EPM.</param>
        /// <param name="updatedTime">Value for the atom:updated element.</param>
        internal static void WriteEntryMetadata(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, AtomEntryMetadata entryMetadata, AtomEntryMetadata epmEntryMetadata, string updatedTime)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(!string.IsNullOrEmpty(updatedTime), "!string.IsNullOrEmpty(updatedTime)");
#if DEBUG
            DateTimeOffset tempDateTimeOffset;
            Debug.Assert(DateTimeOffset.TryParse(updatedTime, out tempDateTimeOffset), "DateTimeOffset.TryParse(updatedTime, out tempDateTimeOffset)");
#endif

            // TODO, ckerer: implement the rule around authors (an entry has to have an author directly or in the <entry:source> unless the feed has an author).
            //               currently we make all entries have an author.
            AtomEntryMetadata mergedEntryMetadata = ODataAtomWriterMetadataEpmMergeUtils.MergeCustomAndEpmEntryMetadata(entryMetadata, epmEntryMetadata);
            if (mergedEntryMetadata == null)
            {
                // write all required metadata elements with default content

                // <atom:title></atom:title>
                ODataAtomWriterUtils.WriteEmptyElement(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace);

                // <atom:updated>dateTimeOffset</atom:updated>
                // NOTE: the <updated> element is required and if not specified we use a single 'default/current' date/time for the whole payload.
                ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomUpdatedElementName, AtomConstants.AtomNamespace, updatedTime);

                WriteEmptyAuthor(writer);
            }
            else
            {
                // <atom:title>text</atom:title>
                // NOTE: writes an empty element even if no title was specified since the title is required
                ODataAtomWriterMetadataUtils.WriteTextConstruct(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, mergedEntryMetadata.Title);

                AtomTextConstruct summary = mergedEntryMetadata.Summary;
                if (summary != null)
                {
                    // <atom:summary>text</atom:summary>
                    ODataAtomWriterMetadataUtils.WriteTextConstruct(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomSummaryElementName, AtomConstants.AtomNamespace, summary);
                }

                DateTimeOffset? published = mergedEntryMetadata.Published;
                if (published.HasValue)
                {
                    // <atom:published>dateTimeOffset</atom:published>
                    ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomPublishedElementName, AtomConstants.AtomNamespace, ODataAtomConvert.ToString(published.Value));
                }

                // <atom:updated>date</atom:updated>
                // NOTE: the <updated> element is required and if not specified we use a single 'default/current' date/time for the whole payload.
                string updated = mergedEntryMetadata.Updated.HasValue ? ODataAtomConvert.ToString(mergedEntryMetadata.Updated.Value) : updatedTime;
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer,
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomUpdatedElementName,
                    AtomConstants.AtomNamespace,
                    updated);

                bool wroteAuthor = false;
                IEnumerable<AtomPersonMetadata> authors = mergedEntryMetadata.Authors;
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
                        WritePersonMetadata(writer, baseUri, urlResolver, author);
                        writer.WriteEndElement();
                        wroteAuthor = true;
                    }
                }

                if (!wroteAuthor)
                {
                    // write empty authors since they are required
                    WriteEmptyAuthor(writer);
                }

                IEnumerable<AtomPersonMetadata> contributors = mergedEntryMetadata.Contributors;
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
                        WritePersonMetadata(writer, baseUri, urlResolver, contributor);
                        writer.WriteEndElement();
                    }
                }

                IEnumerable<AtomLinkMetadata> links = mergedEntryMetadata.Links;
                if (links != null)
                {
                    foreach (AtomLinkMetadata link in links)
                    {
                        if (link == null)
                        {
                            throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkMetadataMustNotContainNull);
                        }

                        // <atom:link>...</atom:link>
                        WriteAtomLink(writer, baseUri, urlResolver, link, null /*edit-link-etag*/);
                    }
                }

                IEnumerable<AtomCategoryMetadata> categories = mergedEntryMetadata.Categories;
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

                if (mergedEntryMetadata.Rights != null)
                {
                    // <atom:rights>rights</atom:rights>
                    ODataAtomWriterMetadataUtils.WriteTextConstruct(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomRightsElementName, AtomConstants.AtomNamespace, mergedEntryMetadata.Rights);
                }

                Uri icon = mergedEntryMetadata.Icon;
                if (icon != null)
                {
                    // <atom:icon>Uri</atom:icon>
                    ODataAtomWriterUtils.WriteElementWithTextContent(
                        writer, 
                        AtomConstants.AtomNamespacePrefix, 
                        AtomConstants.AtomIconElementName, 
                        AtomConstants.AtomNamespace,
                        icon.ToUrlAttributeValue(baseUri, urlResolver));
                }

                AtomFeedMetadata source = mergedEntryMetadata.Source;
                if (source != null)
                {
                    // <atom:source>
                    writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomSourceElementName, AtomConstants.AtomNamespace);

                    bool authorWritten;
                    WriteFeedMetadata(writer, baseUri, urlResolver, source, null /* feed */, updatedTime, out authorWritten);

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
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="feed">The feed for which to write the metadata.</param>
        /// <param name="updatedTime">Value for the atom:updated element.</param>
        /// <param name="authorWritten">set to true if the author element was written, false otherwise.</param>
        internal static void WriteFeedMetadata(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, ODataFeed feed, string updatedTime, out bool authorWritten)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(!string.IsNullOrEmpty(updatedTime), "!string.IsNullOrEmpty(updatedTime)");
#if DEBUG
            DateTimeOffset tempDateTimeOffset;
            Debug.Assert(DateTimeOffset.TryParse(updatedTime, out tempDateTimeOffset), "DateTimeOffset.TryParse(updatedTime, out tempDateTimeOffset)");
#endif

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
                ODataAtomWriterUtils.WriteElementWithTextContent(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomUpdatedElementName, AtomConstants.AtomNamespace, updatedTime);

                authorWritten = false;
            }
            else
            {
                WriteFeedMetadata(writer, baseUri, urlResolver, feedMetadata, feed, updatedTime, out authorWritten);
            }
        }

        /// <summary>
        /// Writes the specified start/end tags and the specified person metadata as content
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="personMetadata">The person metadata to write.</param>
        internal static void WritePersonMetadata(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, AtomPersonMetadata personMetadata)
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
                    uriString = uri.ToUrlAttributeValue(baseUri, urlResolver);
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
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="linkMetadata">The link metadata to write.</param>
        /// <param name="etag">The (optional) ETag for a link.</param>
        internal static void WriteAtomLink(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, AtomLinkMetadata linkMetadata, string etag)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(linkMetadata != null, "Link metadata must not be null.");

            // <atom:link ...
            writer.WriteStartElement(
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomLinkElementName,
                AtomConstants.AtomNamespace);

            // write the attributes of the link
            WriteAtomLinkAttributes(writer, baseUri, urlResolver, linkMetadata, etag);

            // </atom:link>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the metadata of a link in ATOM format
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="linkMetadata">The link metadata to write.</param>
        /// <param name="etag">The (optional) ETag for a link.</param>
        internal static void WriteAtomLinkAttributes(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, AtomLinkMetadata linkMetadata, string etag)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(linkMetadata != null, "Link metadata must not be null.");

            string linkHref;
            if (linkMetadata.HrefFromEpm != null)
            {
                // there is a mapping to atom:link/@href so use the string representation of Href
                linkHref = linkMetadata.HrefFromEpm;
            }
            else
            {
                linkHref = linkMetadata.Href == null ? null : linkMetadata.Href.ToUrlAttributeValue(baseUri, urlResolver);
            }

            WriteAtomLinkMetadataAttributes(writer, linkMetadata.Relation, linkHref, linkMetadata.HrefLang, linkMetadata.Title, linkMetadata.MediaType, linkMetadata.Length);

            if (etag != null)
            {
                ODataAtomWriterUtils.WriteETag(writer, etag);
            }
        }

        /// <summary>
        /// Write the metadata for an ODataNavigationLink link; makes sure any duplicate of the link's values duplicated in metadata are equal.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="navigationLink">The navigation link for which to write the metadata.</param>
        internal static void WriteODataNavigationLinkAttributes(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, ODataNavigationLink navigationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "The navigation link name was not verified yet.");
            Debug.Assert(navigationLink.Url != null, "The navigation link Url was not verified yet.");
            Debug.Assert(navigationLink.IsCollection.HasValue, "navigationLink.IsCollection.HasValue");

            string linkRelation = AtomUtils.ComputeODataNavigationLinkRelation(navigationLink);
            string linkType = AtomUtils.ComputeODataNavigationLinkType(navigationLink);
            string linkTitle = navigationLink.Name;
            
            AtomLinkMetadata linkMetadata = navigationLink.Atom();
            AtomLinkMetadata mergedMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(linkMetadata, linkRelation, navigationLink.Url, linkTitle, linkType);
            ODataAtomWriterMetadataUtils.WriteAtomLinkAttributes(writer, baseUri, urlResolver, mergedMetadata, null /* etag */);
        }

        /// <summary>
        /// Write the metadata for an OData association link; makes sure any duplicate of the link's values duplicated in metadata are equal.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="associationLink">The association link for which to write the metadata.</param>
        internal static void WriteODataAssociationLink(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(associationLink != null, "associationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(associationLink.Name), "The association link name was not verified yet.");
            Debug.Assert(associationLink.Url != null, "The association link Url was not verified yet.");

            AtomLinkMetadata linkMetadata = associationLink.Atom();
            string linkRelation = AtomUtils.ComputeODataAssociationLinkRelation(associationLink);
            AtomLinkMetadata mergedLinkMetadata = ODataAtomWriterMetadataUtils.MergeLinkMetadata(linkMetadata, linkRelation, associationLink.Url, associationLink.Name, MimeConstants.MimeApplicationXml);
            ODataAtomWriterMetadataUtils.WriteAtomLink(writer, baseUri, urlResolver, mergedLinkMetadata, null /* etag*/);
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
                
                string textValue = textConstruct.Text;
                if (textValue == null)
                {
                    if (textConstruct.HasMetadataNullAttribute)
                    {
                        ODataAtomWriterUtils.WriteNullAttribute(writer);
                    }

                    textValue = String.Empty;
                }

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
        /// <param name="writerBehavior">The writer behavior to use.</param>
        /// <param name="workspace">The workspace element to get the metadata for and write it.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.OData.Atom.AtomTextConstruct.set_Text(System.String)", Justification = "The value 'Default' is a spec defined constant which is not to be localized.")]
        internal static void WriteWorkspaceMetadata(XmlWriter writer, ODataWriterBehavior writerBehavior, ODataWorkspace workspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(workspace != null, "workspace != null");

            AtomWorkspaceMetadata metadata = workspace.Atom();
            AtomTextConstruct title = null;
            if (metadata != null)
            {
                title = metadata.Title;
            }

            if (title == null)
            {
                title = new AtomTextConstruct { Text = AtomConstants.AtomWorkspaceDefaultTitle };
            }

            if (writerBehavior.BehaviorKind == ODataBehaviorKind.WcfDataServicesServer && title.Kind == AtomTextConstructKind.Text)
            {
                // For WCF DS server we must not write the type attribute, just a simple <atom:title>Default<atom:title>
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer,
                    AtomConstants.NonEmptyAtomNamespacePrefix,
                    AtomConstants.AtomTitleElementName,
                    AtomConstants.AtomNamespace,
                    title.Text);
            }
            else
            {
                // <atom:title>title</atom:title>
                WriteTextConstruct(writer, AtomConstants.NonEmptyAtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, title);
            }
        }

        /// <summary>
        /// Writes the ATOM metadata for a single (resource) collection element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="writerBehavior">The writer behavior to use.</param>
        /// <param name="collection">The collection element to get the metadata for and write it.</param>
        internal static void WriteCollectionMetadata(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, ODataWriterBehavior writerBehavior, ODataResourceCollectionInfo collection)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(collection != null, "collection != null");
            Debug.Assert(collection.Url != null, "collection.Url should have been validated at this point");

            AtomResourceCollectionMetadata metadata = collection.Atom();

            AtomTextConstruct title = null;
            if (metadata != null)
            {
                title = metadata.Title;
            }

            // The ATOMPUB specification requires a title.
            // <atom:title>title</atom:title>
            // Note that this will write an empty atom:title element even if the title is null.
            if (writerBehavior.BehaviorKind == ODataBehaviorKind.WcfDataServicesServer && title.Kind == AtomTextConstructKind.Text)
            {
                // For WCF DS server we must not write the type attribute, just a simple <atom:title>title<atom:title>
                ODataAtomWriterUtils.WriteElementWithTextContent(
                    writer,
                    AtomConstants.NonEmptyAtomNamespacePrefix,
                    AtomConstants.AtomTitleElementName,
                    AtomConstants.AtomNamespace,
                    title.Text);
            }
            else
            {
                WriteTextConstruct(writer, AtomConstants.NonEmptyAtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, title);
            }

            if (metadata != null)
            {
                string accept = metadata.Accept;
                if (accept != null)
                {
                    // <app:accept>accept</app:accept>
                    ODataAtomWriterUtils.WriteElementWithTextContent(
                        writer,
                        string.Empty,
                        AtomConstants.AtomPublishingAcceptElementName,
                        AtomConstants.AtomPublishingNamespace,
                        accept);
                }

                AtomCategoriesMetadata categories = metadata.Categories;
                if (categories != null)
                {
                    // <app:categories>
                    writer.WriteStartElement(string.Empty, AtomConstants.AtomPublishingCategoriesElementName, AtomConstants.AtomPublishingNamespace);

                    Uri href = categories.Href;
                    bool? fixedValue = categories.Fixed;
                    string scheme = categories.Scheme;
                    IEnumerable<AtomCategoryMetadata> categoriesCollection = categories.Categories;
                    if (href != null)
                    {
                        // Out of line categories document
                        if (fixedValue.HasValue || scheme != null ||
                            (categoriesCollection != null && categoriesCollection.Any()))
                        {
                            throw new ODataException(Strings.ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues);
                        }

                        writer.WriteAttributeString(AtomConstants.AtomHRefAttributeName, href.ToUrlAttributeValue(baseUri, urlResolver));
                    }
                    else
                    {
                        // Inline categories document

                        // fixed='yes|no'
                        if (fixedValue.HasValue)
                        {
                            writer.WriteAttributeString(
                                AtomConstants.AtomPublishingFixedAttributeName,
                                fixedValue.Value ? AtomConstants.AtomPublishingFixedYesValue : AtomConstants.AtomPublishingFixedNoValue);
                        }

                        // scheme='scheme'
                        if (scheme != null)
                        {
                            writer.WriteAttributeString(AtomConstants.AtomCategorySchemeAttributeName, scheme);
                        }

                        if (categoriesCollection != null)
                        {
                            foreach (AtomCategoryMetadata category in categoriesCollection)
                            {
                                // <atom:category/>
                                WriteCategory(writer, AtomConstants.NonEmptyAtomNamespacePrefix, category.Term, category.Scheme, category.Label);
                            }
                        }
                    }                    

                    // </app:categories>
                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Write an empty author element that has the required name element
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        internal static void WriteEmptyAuthor(XmlWriter writer)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // <atom:author>
            writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorElementName, AtomConstants.AtomNamespace);

            // <atom:Name></atom:Name>
            ODataAtomWriterUtils.WriteEmptyElement(writer, AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorNameElementName, AtomConstants.AtomNamespace);

            // </atom:author>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the given feed metadata in atom format
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="baseUri">The base Uri of the document or null if none was specified.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="feedMetadata">The metadata to write.</param>
        /// <param name="feed">The feed for which to write the meadata or null if it is the metadata of an atom:source element.</param>
        /// <param name="updatedTime">Value for the atom:updated element.</param>
        /// <param name="authorWritten">Set to true if the author element was written, false otherwise.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Does not make sense to break up writing the various parts of feed metadata. Very linear code; complexity not high.")]
        private static void WriteFeedMetadata(XmlWriter writer, Uri baseUri, IODataUrlResolver urlResolver, AtomFeedMetadata feedMetadata, ODataFeed feed, string updatedTime, out bool authorWritten)
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
            // NOTE: the <updated> element is required and if not specified we use a single 'default/current' date/time for the whole payload.
            string updated = feedMetadata.Updated.HasValue ? ODataAtomConvert.ToString(feedMetadata.Updated.Value) : updatedTime;
            ODataAtomWriterUtils.WriteElementWithTextContent(
                writer,
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
                WriteAtomLink(writer, baseUri, urlResolver, mergedSelfLinkMetadata, null /*etag*/);
            }

            IEnumerable<AtomLinkMetadata> links = feedMetadata.Links;
            if (links != null)
            {
                foreach (AtomLinkMetadata link in links)
                {
                    // <atom:link>...</atom:link>
                    WriteAtomLink(writer, baseUri, urlResolver, link, null /* etag */);
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
                    logo.ToUrlAttributeValue(baseUri, urlResolver));
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
                    WritePersonMetadata(writer, baseUri, urlResolver, contributor);
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
                    writer.WriteAttributeString(AtomConstants.AtomGeneratorUriAttributeName, generator.Uri.ToUrlAttributeValue(baseUri, urlResolver));
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
                    icon.ToUrlAttributeValue(baseUri, urlResolver));
            }

            IEnumerable<AtomPersonMetadata> authors = feedMetadata.Authors;
            authorWritten = false;
            if (authors != null)
            {
                foreach (AtomPersonMetadata author in authors)
                {
                    // <atom:author>author data</atom:author>
                    authorWritten = true;
                    writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomAuthorElementName, AtomConstants.AtomNamespace);
                    WritePersonMetadata(writer, baseUri, urlResolver, author);
                    writer.WriteEndElement();
                }
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
    }
}
