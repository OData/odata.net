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
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using System.Xml;
    #endregion Namespaces.

    /// <summary>
    /// OData writer for the ATOM format.
    /// </summary>
    internal sealed class ODataAtomWriter : ODataWriterCore
    {
        /// <summary>Atom xml writer.</summary>
        private XmlWriter writer;

        /// <summary>The async output stream underlying the Xml writer</summary>
        private AsyncBufferedStream outputStream;

        /// <summary>
        /// Constructor creating an OData writer using the ATOM format.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="odataWriterSettings">Configuration settings for the writer to create.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="writingResponse">True if the writer is to write a response payload; false if it's to write a request payload.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="writingFeed">True if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        internal ODataAtomWriter(
            Stream stream, 
            ODataWriterSettings odataWriterSettings, 
            Encoding encoding, 
            bool writingResponse,
            DataServiceMetadataProviderWrapper metadataProvider,
            bool writingFeed,
            bool synchronous)
            : base(
                odataWriterSettings.Version, 
                odataWriterSettings.BaseUri, 
                writingResponse, 
                metadataProvider,
                writingFeed,
                synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(odataWriterSettings != null, "odataWriterSettings != null");

            this.outputStream = new AsyncBufferedStream(stream);
            this.writer = ODataAtomWriterUtils.CreateXmlWriter(this.outputStream, odataWriterSettings, encoding);
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        protected sealed override void FlushSynchronously()
        {
            this.writer.Flush();
            this.outputStream.FlushSync();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        protected sealed override Task FlushAsynchronously()
        {
            this.writer.Flush();
            return this.outputStream.FlushAsync();
        }
#endif

        /// <summary>
        /// Flush and close the writer.
        /// </summary>
        /// <param name="discardBufferedData">If true discard any buffered data before closing the writer.</param>
        /// <remarks>This is only called during disposal of the writer.</remarks>
        protected override void FlushAndCloseWriter(bool discardBufferedData)
        {
            try
            {
                // flush the Xml writer to the underlying stream so we guarantee that there is no data buffered in the Xml writer;
                // the underlying stream verifies that no data is still buffered when disposed below.
                this.writer.Flush();

                if (!IsErrorState(this.State))
                {
                    // XmlWriter.Close() guarantees that well-formed xml is produced by injecting close elements
                    // if any is missing.  In the case of an exception, we want the stream to end at the close
                    // element </m:error>.  So we skip writer.Dispose here if we are in error state.
                    Utils.TryDispose(this.writer);
                }

                if (discardBufferedData)
                {
                    this.outputStream.Clear();
                }

                this.outputStream.Dispose();
            }
            finally
            {
                this.writer = null;
                this.outputStream = null;
            }
        }

        /// <summary>
        /// Start writing an OData payload.
        /// </summary>
        protected override void StartPayload()
        {
            this.writer.WriteStartDocument(true);
        }

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected override void EndPayload()
        {
            // This method is only called if no error has been written so it is safe to
            // call WriteEndDocument() here (since it closes all open elements which we don't want in error state)
            this.writer.WriteEndDocument();
        }

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected override void StartEntry(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");

            // <entry>
            this.writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomEntryElementName, AtomConstants.AtomNamespace);

            if (this.IsTopLevel)
            {
                ODataAtomWriterUtils.WriteBaseUriAndDefaultNamespaceAttributes(this.writer, this.BaseUri);
            }

            string etag = entry.ETag;
            if (etag != null)
            {
                // TODO, ckerer: if this is a top-level entry also put the ETag into the headers.
                ODataAtomWriterUtils.WriteETag(this.writer, etag);
            }
        }

        /// <summary>
        /// Finish writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected override void EndEntry(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");

            string typeName = entry.TypeName;
            ResourceType entryType = ValidationUtils.ValidateTypeName(this.MetadataProvider, typeName, ResourceTypeKind.EntityType, false);

            // Initialize the property value cache and cache the entry properties.
            EntryPropertiesValueCache propertyValueCache = new EntryPropertiesValueCache(entry);
            EpmResourceTypeAnnotation epmResourceTypeAnnotation = null;
            if (entryType != null)
            {
                Debug.Assert(entryType.IsReadOnly, "The resource must be read-only to be applied to an entry.");
                entryType.EnsureEpmAvailability();
                epmResourceTypeAnnotation = entryType.Epm();
            }

            // <atom:id>idValue</atom:id>
            // NOTE: do not generate a relative Uri for the ID; it is independent of xml:base
            ODataAtomWriterUtils.WriteElementWithTextContent(
                this.writer, 
                AtomConstants.AtomNamespacePrefix, 
                AtomConstants.AtomIdElementName, 
                AtomConstants.AtomNamespace, 
                entry.Id);

            // <category term="type" scheme="odatascheme"/>
            // If no type information is provided, don't include the category element for type at all
            // NOTE: the validation of the type name happened at the beginning of this method.
            if (typeName != null)
            {
                ODataAtomWriterMetadataUtils.WriteCategory(this.writer, typeName, AtomConstants.ODataSchemeNamespace, null);
            }

            Uri editLink = entry.EditLink;
            if (editLink != null)
            {
                // <link rel="edit" title="Title" href="LinkHRef"/>
                ODataAtomWriterUtils.WriteReadOrEditLink(this.writer, this.BaseUri, editLink, AtomConstants.AtomEditRelationAttributeValue, typeName);
            }

            Uri readLink = entry.ReadLink;
            if (readLink != null)
            {
                // <link rel="self" title="Title" href="LinkHRef"/>
                ODataAtomWriterUtils.WriteReadOrEditLink(this.writer, this.BaseUri, readLink, AtomConstants.AtomSelfRelationAttributeValue, typeName);
            }

            // named streams
            IEnumerable<ODataMediaResource> namedStreams = entry.NamedStreams;
            if (namedStreams != null)
            {
                foreach (ODataMediaResource namedStream in namedStreams)
                {
                    ValidationUtils.ValidateNamedStream(namedStream, this.Version);

                    ODataAtomWriterUtils.WriteNamedStream(this.writer, this.BaseUri, namedStream);
                }
            }

            // association links
            IEnumerable<ODataAssociationLink> associationLinks = entry.AssociationLinks;
            if (associationLinks != null)
            {
                foreach (ODataAssociationLink associationLink in associationLinks)
                {
                    ValidationUtils.ValidateAssociationLink(associationLink, this.Version);

                    ODataAtomWriterUtils.WriteAssociationLink(this.writer, this.BaseUri, entry, associationLink);
                }
            }

            // write entry metadata including syndication EPM
            AtomEntryMetadata epmEntryMetadata = null;
            if (epmResourceTypeAnnotation != null)
            {
                ODataVersionChecker.CheckEntityPropertyMapping(this.Version, entryType);

                epmEntryMetadata = EpmSyndicationWriter.WriteEntryEpm(
                    epmResourceTypeAnnotation.EpmTargetTree,
                    propertyValueCache,
                    entryType,
                    this.MetadataProvider,
                    this.Version);
            }

            ODataAtomWriterMetadataUtils.WriteEntryMetadata(this.writer, this.BaseUri, entry, epmEntryMetadata);

            // write the content
            this.WriteEntryContent(entry, entryType, propertyValueCache, epmResourceTypeAnnotation == null ? null : epmResourceTypeAnnotation.EpmSourceTree.Root);

            // write custom EPM
            if (epmResourceTypeAnnotation != null)
            {
                EpmCustomWriter.WriteEntryEpm(
                    this.writer,
                    epmResourceTypeAnnotation.EpmTargetTree,
                    propertyValueCache,
                    entryType,
                    this.MetadataProvider);
            }

            // </entry>
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected override void StartFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            // <atom:feed>
            this.writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomFeedElementName, AtomConstants.AtomNamespace);

            if (this.IsTopLevel)
            {
                ODataAtomWriterUtils.WriteBaseUriAndDefaultNamespaceAttributes(this.writer, this.BaseUri);

                if (feed.Count.HasValue)
                {
                    ODataAtomWriterUtils.WriteCount(this.writer, feed.Count.Value, false);
                }
            }
        }

        /// <summary>
        /// Finish writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected override void EndFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            Uri nextPageLink = feed.NextPageLink;
            if (nextPageLink != null)
            {
                // <atom:link rel="next" href="next-page-link" />
                this.writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomLinkElementName, AtomConstants.AtomNamespace);
                this.writer.WriteAttributeString(AtomConstants.AtomLinkRelationAttributeName, AtomConstants.AtomLinkRelationNextAttributeValue);
                this.writer.WriteAttributeString(AtomConstants.AtomHRefAttributeName, AtomUtils.ToUrlAttributeValue(nextPageLink, this.BaseUri));
                this.writer.WriteEndElement();
            }

            ODataAtomWriterMetadataUtils.WriteFeedMetadata(this.writer, this.BaseUri, feed);

            // </atom:feed>
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Start writing a link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected override void WriteLink(ODataLink link)
        {
            Debug.Assert(link != null, "link != null");

            this.WriteLinkStart(link);
            this.WriteLinkEnd();
        }

        /// <summary>
        /// Start writing am expanded link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected override void StartExpandedLink(ODataLink link)
        {
            Debug.Assert(link != null, "link != null");

            // write the start element of the link
            this.WriteLinkStart(link);

            // <m:inline>
            this.writer.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataInlineElementName, AtomConstants.ODataMetadataNamespace);
        }

        /// <summary>
        /// Finish writing an expanded link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected override void EndExpandedLink(ODataLink link)
        {
            Debug.Assert(link != null, "link != null");

            // </m:inline>
            this.writer.WriteEndElement();

            this.WriteLinkEnd();
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name='error'>The error to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        protected override void SerializeError(ODataError error, bool includeDebugInformation)
        {
            Debug.Assert(error != null, "error != null");
            ODataAtomWriterUtils.WriteError(this.writer, error, includeDebugInformation);
        }

        /// <summary>
        /// Write the content of the given entry.
        /// </summary>
        /// <param name="entry">The entry for which to write properties.</param>
        /// <param name="entryType">The <see cref="ResourceType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="propertiesValueCache">The cache of properties.</param>
        /// <param name="rootSourcePathSegment">The root of the EPM source tree, if there's an EPM applied.</param>
        private void WriteEntryContent(ODataEntry entry, ResourceType entryType, EntryPropertiesValueCache propertiesValueCache, EpmSourcePathSegment rootSourcePathSegment)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(propertiesValueCache != null, "propertiesValueCache != null");

            ODataMediaResource mediaResource = entry.MediaResource;
            if (mediaResource == null)
            {
                // <content type="application/xml">
                this.writer.WriteStartElement(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomContentElementName,
                    AtomConstants.AtomNamespace);

                this.writer.WriteAttributeString(
                    AtomConstants.AtomTypeAttributeName,
                    MimeConstants.MimeApplicationXml);

                // <m:properties>
                // we always write the <m:properties> element even if there are no properties
                this.writer.WriteStartElement(
                    AtomConstants.ODataMetadataNamespacePrefix,
                    AtomConstants.AtomPropertiesElementName,
                    AtomConstants.ODataMetadataNamespace);

                ODataAtomWriterUtils.WriteProperties(
                    this.writer, 
                    this.MetadataProvider,
                    entryType,
                    propertiesValueCache.EntryProperties,
                    this.Version,
                    false,
                    propertiesValueCache,
                    rootSourcePathSegment);

                // </m:properties>
                this.writer.WriteEndElement();

                // </content>
                this.writer.WriteEndElement();
            }
            else
            {
                Uri mediaEditLink = mediaResource.EditLink;
                if (mediaEditLink != null)
                {
                    // <link rel="edit-media" href="href" />
                    this.writer.WriteStartElement(
                        AtomConstants.AtomNamespacePrefix,
                        AtomConstants.AtomLinkElementName,
                        AtomConstants.AtomNamespace);

                    this.writer.WriteAttributeString(
                        AtomConstants.AtomLinkRelationAttributeName,
                        AtomConstants.AtomEditMediaRelationAttributeValue);

                    this.writer.WriteAttributeString(
                        AtomConstants.AtomHRefAttributeName,
                        AtomUtils.ToUrlAttributeValue(mediaEditLink, this.BaseUri));

                    string mediaETag = mediaResource.ETag;
                    if (mediaETag != null)
                    {
                        this.writer.WriteAttributeString(
                            AtomConstants.ODataMetadataNamespacePrefix,
                            AtomConstants.ODataETagAttributeName,
                            AtomConstants.ODataMetadataNamespace,
                            mediaETag);
                    }

                    // </link>
                    this.writer.WriteEndElement();
                }

                Debug.Assert(mediaEditLink != null || mediaResource.ETag == null, "The default stream edit link and etag should have been validated by now.");

                // <content type="type" src="src">
                this.writer.WriteStartElement(
                    AtomConstants.AtomNamespacePrefix,
                    AtomConstants.AtomContentElementName,
                    AtomConstants.AtomNamespace);

                Debug.Assert(!string.IsNullOrEmpty(mediaResource.ContentType), "The default stream content type should have been validated by now.");
                this.writer.WriteAttributeString(
                    AtomConstants.AtomTypeAttributeName,
                    mediaResource.ContentType);

                Debug.Assert(mediaResource.ReadLink != null, "The default stream read link should have been validated by now.");
                this.writer.WriteAttributeString(
                    AtomConstants.MediaLinkEntryContentSourceAttributeName,
                    AtomUtils.ToUrlAttributeValue(mediaResource.ReadLink, this.BaseUri));

                // </content>
                this.writer.WriteEndElement();

                // <m:properties>
                // we always write the <m:properties> element even if there are no properties
                this.writer.WriteStartElement(
                    AtomConstants.ODataMetadataNamespacePrefix,
                    AtomConstants.AtomPropertiesElementName,
                    AtomConstants.ODataMetadataNamespace);

                ODataAtomWriterUtils.WriteProperties(
                    this.writer, 
                    this.MetadataProvider,
                    entryType,
                    propertiesValueCache.EntryProperties, 
                    this.Version,
                    false,
                    propertiesValueCache,
                    rootSourcePathSegment);

                // </m:properties>
                this.writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes the link's start element and atom metadata.
        /// </summary>
        /// <param name="link">The link to write.</param>
        private void WriteLinkStart(ODataLink link)
        {
            ValidationUtils.ValidateLink(link);

            // Link must specify the Url
            // NOTE: we currently only require a non-null Url for ATOM payloads and non-expanded links in JSON.
            //       There is no place in JSON to write a Url if the link is expanded. We can't change that for v1 and v2; we
            //       might fix the protocol for v3.
            if (link.Url == null)
            {
                throw new ODataException(Strings.ODataWriter_LinkMustSpecifyUrl);
            }

            // <atom:link>
            this.writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomLinkElementName, AtomConstants.AtomNamespace);

            ODataAtomWriterMetadataUtils.WriteODataLinkMetadata(this.writer, this.BaseUri, link);
        }

        /// <summary>
        /// Writes custom extensions and the end element for a link
        /// </summary>
        private void WriteLinkEnd()
        {
            // </atom:link>
            this.writer.WriteEndElement();
        }
    }
}
