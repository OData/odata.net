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
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData writer for the ATOM format.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:ImplementIDisposable", Justification = "IDisposable is implemented on ODataMessageWriter.")]
    internal sealed class ODataAtomWriter : ODataWriterCore
    {
        /// <summary>Value for the atom:updated element.</summary>
        /// <remarks>
        /// The writer will use the same default value for the atom:updated element in a given payload. While there is no requirement for this,
        /// it saves us from re-querying the system time and converting it to string every time we write an item.
        /// </remarks>
        private readonly string updatedTime = ODataAtomConvert.ToString(DateTimeOffset.UtcNow);

        /// <summary>A stack used to track XML customization writers.</summary>
        /// <remarks>
        /// At the begining the root writer is pushed to the stack.
        /// Each non-null entry has an item on this stack.
        /// If the XML customization was used for a given entry the writer returned by the customization will be pushed to the stack for it.
        /// </remarks>
        private readonly Stack<XmlWriter> xmlCustomizationWriters;

        /// <summary>Atom xml writer. This is the current writer used for writing the payload.</summary>
        /// <remarks>
        /// In case XML customization of entries is used, this might contain the writer returned by the XML customization callbacks.
        /// Such writers are not owned by this class.
        /// </remarks>
        private XmlWriter writer;

        /// <summary>The XML writer created for writing the root document. This is the only writer this class owns and will dispose of.</summary>
        private XmlWriter rootWriter;

        /// <summary>The output stream used when writing synchronously.</summary>
        private Stream synchronousOutputStream;

        /// <summary>The async output stream underlying the Xml writer</summary>
        private AsyncBufferedStream asynchronousOutputStream;
        
        /// <summary>
        /// Constructor creating an OData writer using the ATOM format.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="messageWriterSettings">Configuration settings for the writer to create.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="writingResponse">True if the writer is to write a response payload; false if it's to write a request payload.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="writingFeed">True if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        internal ODataAtomWriter(
            Stream stream, 
            ODataMessageWriterSettings messageWriterSettings, 
            IODataUrlResolver urlResolver,
            Encoding encoding, 
            bool writingResponse,
            IEdmModel model,
            bool writingFeed,
            bool synchronous)
            : base(
                urlResolver,
                messageWriterSettings.Version.Value,
                messageWriterSettings.BaseUri,
                messageWriterSettings.WriterBehavior,
                writingResponse, 
                model,
                writingFeed,
                synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            Stream outputStream;
            if (synchronous)
            {
                this.synchronousOutputStream = stream;
                outputStream = stream;
            }
            else
            {
                this.asynchronousOutputStream = new AsyncBufferedStream(stream);
                outputStream = this.asynchronousOutputStream;
            }

            this.rootWriter = ODataAtomWriterUtils.CreateXmlWriter(outputStream, messageWriterSettings, encoding);
            this.writer = this.rootWriter;

            if (this.WriterBehavior.StartEntryXmlCustomizationCallback != null)
            {
                Debug.Assert(this.WriterBehavior.EndEntryXmlCustomizationCallback != null, "We should have verified that both start end end XML customization callbacks are specified.");
                this.xmlCustomizationWriters = new Stack<XmlWriter>();
                this.xmlCustomizationWriters.Push(this.rootWriter);
            }
        }

        /// <summary>
        /// Enumeration of ATOM element flags, used to keep track of which elements were already written.
        /// </summary>
        private enum AtomElement
        {
            /// <summary>The atom:id element.</summary>
            Id = 0x1,

            /// <summary>The atom:link with rel='self'.</summary>
            ReadLink = 0x2,

            /// <summary>The atom:link with rel='edit'.</summary>
            EditLink = 0x4,
        }

        /// <summary>
        /// Returns the current AtomEntryScope.
        /// </summary>
        private AtomEntryScope CurrentEntryScope
        {
            get
            {
                AtomEntryScope currentAtomEntryScope = this.CurrentScope as AtomEntryScope;
                Debug.Assert(currentAtomEntryScope != null, "Asking for AtomEntryScope when the current scope is not an AtomEntryScope.");
                return currentAtomEntryScope;
            }
        }

        /// <summary>
        /// Returns the current AtomFeedScope.
        /// </summary>
        private AtomFeedScope CurrentFeedScope
        {
            get
            {
                AtomFeedScope currentAtomFeedScope = this.CurrentScope as AtomFeedScope;
                Debug.Assert(currentAtomFeedScope != null, "Asking for AtomFeedScope when the current scope is not an AtomFeedScope.");
                return currentAtomFeedScope;
            }
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.writer.Flush();

            Debug.Assert(this.synchronousOutputStream != null && this.asynchronousOutputStream == null, "FlushSynchronously called, but we don't have a sync stream.");
            this.synchronousOutputStream.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        protected sealed override Task FlushAsynchronously()
        {
            this.writer.Flush();

            Debug.Assert(this.synchronousOutputStream == null && this.asynchronousOutputStream != null, "FlushAsynchronously called, but we don't have an async stream.");
            return this.asynchronousOutputStream.FlushAsync();
        }
#endif

        /// <summary>
        /// Flush and close the writer.
        /// </summary>
        /// <remarks>This is only called during disposal of the writer.</remarks>
        protected override void FlushAndCloseWriter()
        {
            try
            {
                // Note that the this.writer might be one of the XML customization writers, but since we don't own those
                // we won't be flushing or disposing them. We just own the root writer which we created.

                // Flush the Xml writer to the underlying stream so we guarantee that there is no data buffered in the Xml writer;
                // the underlying stream verifies that no data is still buffered when disposed below.
                this.rootWriter.Flush();

                // XmlWriter.Dispose calls XmlWriter.Close which writes missing end elements.
                // Thus we can't dispose the XmlWriter since that might end up writing more data into the stream right here
                // and thus callers would have no way to prevent us from writing synchronously into the underlying stream.
                // (note that all other cases where we write to the stream can be followed by FlushAsync which will perform
                //  async write to the stream, but Dispose is the only case where that's not true).
                // Also in case of in-stream error we intentionally want to not write the end elements to keep the payload invalid.
                // Always flush the data synchronously before dispose.
                if (this.synchronousOutputStream != null)
                {
                    Debug.Assert(this.asynchronousOutputStream == null, "Can't have both sync and async streams.");
                    this.synchronousOutputStream.Flush();

                    // Do not dispose the sync stream since we don't own it.
                }
                else
                {
                    Debug.Assert(this.asynchronousOutputStream != null, "Must have async stream if we don't have a sync one.");
                    this.asynchronousOutputStream.FlushSync();
                    this.asynchronousOutputStream.Dispose();
                }
            }
            finally
            {
                this.rootWriter = null;
                this.writer = null;
                this.synchronousOutputStream = null;
                this.asynchronousOutputStream = null;
            }
        }

        /// <summary>
        /// Start writing an OData payload.
        /// </summary>
        protected override void StartPayload()
        {
            this.writer.WriteStartDocument();
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
            if (entry == null)
            {
                Debug.Assert(
                    this.ParentExpandedLink != null && !this.ParentExpandedLink.IsCollection.Value,
                        "when entry == null, it has to be an expanded single entry navigation");

                // this is a null expanded single entry and it is null, an empty <m:inline /> will be written.
                return;
            }

            this.StartEntryXmlCustomization(entry);

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

            AtomEntryScope currentEntryScope = this.CurrentEntryScope;
            AtomEntryMetadata entryMetadata = entry.Atom();

            // Write the id if it's available here.
            // If it's not available here we will try to write it at the end of the entry again.
            string entryId = entry.Id;
            if (entryId != null)
            {
                this.WriteEntryId(entryId);
                currentEntryScope.SetWrittenElement(AtomElement.Id);
            }

            // <category term="type" scheme="odatascheme"/>
            // If no type information is provided, don't include the category element for type at all
            // NOTE: the validation of the type name is done by the core writer.
            string typeName = entry.TypeName;
            SerializationTypeNameAnnotation serializationTypeNameAnnotation = entry.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

            if (typeName != null)
            {
                ODataAtomWriterMetadataUtils.WriteCategory(this.writer, AtomConstants.AtomNamespacePrefix, typeName, AtomConstants.ODataSchemeNamespace, null);
            }

            // Write the edit link if it's available here.
            // If it's not available here we will try to write it at the end of the entry again.
            Uri editLink = entry.EditLink;
            if (editLink != null)
            {
                this.WriteEntryEditLink(editLink, entryMetadata);
                currentEntryScope.SetWrittenElement(AtomElement.EditLink);
            }

            // Write the self link if it's available here.
            // If it's not available here we will try to write it at the end of the entry again.
            Uri readLink = entry.ReadLink;
            if (readLink != null)
            {
                this.WriteEntryReadLink(readLink, entryMetadata);
                currentEntryScope.SetWrittenElement(AtomElement.ReadLink);
            }
        }

        /// <summary>
        /// Finish writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The coupling is intentional here.")]
        protected override void EndEntry(ODataEntry entry)
        {
            if (entry == null)
            {
                Debug.Assert(
                    this.ParentExpandedLink != null && !this.ParentExpandedLink.IsCollection.Value,
                        "when entry == null, it has to be and expanded single entry navigation");

                // this is a null expanded single entry and it is null, an empty <m:inline /> will be written.
                return;
            }

            IEdmEntityType entryType = this.EntryEntityType;

            // Initialize the property value cache and cache the entry properties.
            EntryPropertiesValueCache propertyValueCache = new EntryPropertiesValueCache(entry);
            ODataEntityPropertyMappingCache epmCache = entryType.EnsureEpmCache();

            // Get the projected properties annotation
            ProjectedPropertiesAnnotation projectedProperties = entry.GetAnnotation<ProjectedPropertiesAnnotation>();

            AtomEntryScope currentEntryScope = this.CurrentEntryScope;
            AtomEntryMetadata entryMetadata = entry.Atom();

            if (!currentEntryScope.IsElementWritten(AtomElement.Id))
            {
                // NOTE: We write even null id, in that case we generate an empty atom:id element.
                this.WriteEntryId(entry.Id);
            }

            Uri editLink = entry.EditLink;
            if (editLink != null && !currentEntryScope.IsElementWritten(AtomElement.EditLink))
            {
                this.WriteEntryEditLink(editLink, entryMetadata);
            }

            Uri readLink = entry.ReadLink;
            if (readLink != null && !currentEntryScope.IsElementWritten(AtomElement.ReadLink))
            {
                this.WriteEntryReadLink(readLink, entryMetadata);
            }

            // write entry metadata including syndication EPM
            AtomEntryMetadata epmEntryMetadata = null;
            if (epmCache != null)
            {
                ODataVersionChecker.CheckEntityPropertyMapping(this.Version, entryType);

                epmEntryMetadata = EpmSyndicationWriter.WriteEntryEpm(
                    epmCache.EpmTargetTree,
                    propertyValueCache,
                    entryType.ToTypeReference().AsEntity(),
                    this.Model,
                    this.Version,
                    this.WriterBehavior);
            }

            ODataAtomWriterMetadataUtils.WriteEntryMetadata(this.writer, this.BaseUri, this.UrlResolver, entryMetadata, epmEntryMetadata, this.updatedTime);

            // named streams
            IEnumerable<ODataProperty> namedStreams = propertyValueCache.EntryNamedStreams;
            if (namedStreams != null)
            {
                foreach (ODataProperty namedStreamProperty in namedStreams)
                {
                    ODataAtomWriterUtils.WriteNamedStreamProperty(
                        this.writer, 
                        this.BaseUri, 
                        this.UrlResolver,
                        namedStreamProperty, 
                        entryType, 
                        this.Version, 
                        this.DuplicatePropertyNamesChecker, 
                        projectedProperties);
                }
            }

            // association links
            IEnumerable<ODataAssociationLink> associationLinks = entry.AssociationLinks;
            if (associationLinks != null)
            {
                foreach (ODataAssociationLink associationLink in associationLinks)
                {
                    this.WriteAssociationLink(associationLink, projectedProperties);
                }
            }

            // write custom EPM
            if (epmCache != null)
            {
                EpmCustomWriter.WriteEntryEpm(
                    this.writer,
                    epmCache.EpmTargetTree,
                    propertyValueCache,
                    entryType.ToTypeReference().AsEntity(),
                    this.Model,
                    this.Version,
                    this.WriterBehavior);
            }

            // write the content
            this.WriteEntryContent(
                entry, 
                entryType, 
                propertyValueCache, 
                epmCache == null ? null : epmCache.EpmSourceTree.Root,
                projectedProperties);

            // </entry>
            this.writer.WriteEndElement();

            this.EndEntryXmlCustomization(entry);
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

            bool authorWritten;
            ODataAtomWriterMetadataUtils.WriteFeedMetadata(this.writer, this.BaseUri, this.UrlResolver, feed, this.updatedTime, out authorWritten);
            this.CurrentFeedScope.AuthorWritten = authorWritten;
        }

        /// <summary>
        /// Finish writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected override void EndFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            AtomFeedScope currentFeedScope = this.CurrentFeedScope;
            if (!currentFeedScope.AuthorWritten && currentFeedScope.EntryCount == 0)
            {
                // Write an empty author if there were no entries, since the feed must have an author if the entries don't have one as per ATOM spec
                ODataAtomWriterMetadataUtils.WriteEmptyAuthor(this.writer);
            }

            Uri nextPageLink = feed.NextPageLink;
            if (nextPageLink != null)
            {
                // <atom:link rel="next" href="next-page-link" />
                this.writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomLinkElementName, AtomConstants.AtomNamespace);
                this.writer.WriteAttributeString(AtomConstants.AtomLinkRelationAttributeName, AtomConstants.AtomLinkRelationNextAttributeValue);
                this.writer.WriteAttributeString(AtomConstants.AtomHRefAttributeName, nextPageLink.ToUrlAttributeValue(this.BaseUri, this.UrlResolver));
                this.writer.WriteEndElement();
            }

            // </atom:feed>
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected override void WriteDeferredNavigationLink(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            this.WriteNavigationLinkStart(navigationLink);
            this.WriteNavigationLinkEnd();
        }

        /// <summary>
        /// Start writing am expanded link.
        /// </summary>
        /// <param name="navigationLink">The expanded navigation link to write.</param>
        protected override void StartExpandedLink(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            // write the start element of the link
            this.WriteNavigationLinkStart(navigationLink);

            // <m:inline>
            this.writer.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataInlineElementName, AtomConstants.ODataMetadataNamespace);
        }

        /// <summary>
        /// Finish writing an expanded link.
        /// </summary>
        /// <param name="navigationLink">The expanded navigation link to write.</param>
        protected override void EndExpandedLink(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            // </m:inline>
            this.writer.WriteEndElement();

            this.WriteNavigationLinkEnd();
        }

        /// <summary>
        /// Create a new feed scope.
        /// </summary>
        /// <param name="feed">The feed for the new scope.</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <returns>The newly create scope.</returns>
        protected override FeedScope CreateFeedScope(ODataFeed feed, bool skipWriting)
        {
            return new AtomFeedScope(feed, skipWriting);
        }

        /// <summary>
        /// Create a new entry scope.
        /// </summary>
        /// <param name="entry">The entry for the new scope.</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <returns>The newly create scope.</returns>
        protected override EntryScope CreateEntryScope(ODataEntry entry, bool skipWriting)
        {
            return new AtomEntryScope(entry, skipWriting, this.WritingResponse, this.WriterBehavior);
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
        /// Writes the entry atom:id element.
        /// </summary>
        /// <param name="entryId">The value of the ODataEntry.Id property to write.</param>
        private void WriteEntryId(string entryId)
        {
            Debug.Assert(entryId == null || entryId.Length > 0, "We should have validated that the Id is not empty by now.");

            // <atom:id>idValue</atom:id>
            // NOTE: do not generate a relative Uri for the ID; it is independent of xml:base
            ODataAtomWriterUtils.WriteElementWithTextContent(
                this.writer,
                AtomConstants.AtomNamespacePrefix,
                AtomConstants.AtomIdElementName,
                AtomConstants.AtomNamespace,
                entryId);
        }

        /// <summary>
        /// Writes the read link element for an entry.
        /// </summary>
        /// <param name="readLink">The read link URL.</param>
        /// <param name="entryMetadata">The ATOM entry metatadata for the current entry.</param>
        private void WriteEntryReadLink(Uri readLink, AtomEntryMetadata entryMetadata)
        {
            Debug.Assert(readLink != null, "readLink != null");

            // we allow additional link metadata to specify the title, type, hreflang or length of the link
            AtomLinkMetadata readLinkMetadata = entryMetadata == null ? null : entryMetadata.SelfLink;

            // <link rel="self" href="LinkHRef" ... />
            ODataAtomWriterUtils.WriteReadOrEditLink(this.writer, this.BaseUri, this.UrlResolver, readLink, readLinkMetadata, AtomConstants.AtomSelfRelationAttributeValue);
        }

        /// <summary>
        /// Writes the edit link element for an entry.
        /// </summary>
        /// <param name="editLink">The edit link URL.</param>
        /// <param name="entryMetadata">The ATOM entry metatadata for the current entry.</param>
        private void WriteEntryEditLink(Uri editLink, AtomEntryMetadata entryMetadata)
        {
            Debug.Assert(editLink != null, "editLink != null");

            // we allow additional link metadata to specify the title, type, hreflang or length of the link
            AtomLinkMetadata editLinkMetadata = entryMetadata == null ? null : entryMetadata.EditLink;

            // <link rel="edit" href="LinkHRef" .../>
            ODataAtomWriterUtils.WriteReadOrEditLink(this.writer, this.BaseUri, this.UrlResolver, editLink, editLinkMetadata, AtomConstants.AtomEditRelationAttributeValue);
        }

        /// <summary>
        /// Write the content of the given entry.
        /// </summary>
        /// <param name="entry">The entry for which to write properties.</param>
        /// <param name="entryType">The <see cref="IEdmEntityType"/> of the entry (or null if not metadata is available).</param>
        /// <param name="propertiesValueCache">The cache of properties.</param>
        /// <param name="rootSourcePathSegment">The root of the EPM source tree, if there's an EPM applied.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        private void WriteEntryContent(
            ODataEntry entry,
            IEdmEntityType entryType, 
            EntryPropertiesValueCache propertiesValueCache, 
            EpmSourcePathSegment rootSourcePathSegment,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(propertiesValueCache != null, "propertiesValueCache != null");

            ODataStreamReferenceValue mediaResource = entry.MediaResource;
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

                ODataAtomWriterUtils.WriteProperties(
                    this.writer, 
                    this.Model,
                    entryType,
                    propertiesValueCache.EntryProperties,
                    this.Version,
                    false /* isWritingCollection */,
                    this.WritingResponse,
                    this.WriterBehavior,
                    ODataAtomWriterUtils.WriteEntryPropertiesStart,
                    ODataAtomWriterUtils.WriteEntryPropertiesEnd,
                    this.DuplicatePropertyNamesChecker,
                    propertiesValueCache,
                    rootSourcePathSegment,
                    projectedProperties);

                // </content>
                this.writer.WriteEndElement();
            }
            else
            {
                WriterValidationUtils.ValidateStreamReferenceValue(mediaResource, true);

                Uri mediaEditLink = mediaResource.EditLink;
                Debug.Assert(mediaEditLink != null || mediaResource.ETag == null, "The default stream edit link and etag should have been validated by now.");
                if (mediaEditLink != null)
                {
                    AtomStreamReferenceMetadata streamReferenceMetadata = mediaResource.GetAnnotation<AtomStreamReferenceMetadata>();
                    AtomLinkMetadata mediaEditMetadata = streamReferenceMetadata == null ? null : streamReferenceMetadata.EditLink;
                    AtomLinkMetadata mergedLinkMetadata = 
                        ODataAtomWriterMetadataUtils.MergeLinkMetadata(
                            mediaEditMetadata, 
                            AtomConstants.AtomEditMediaRelationAttributeValue,
                            mediaEditLink,
                            null /* title */,
                            null /* mediaType */);

                    ODataAtomWriterMetadataUtils.WriteAtomLink(this.writer, this.BaseUri, this.UrlResolver, mergedLinkMetadata, mediaResource.ETag);
                }

                if (mediaResource.ReadLink != null)
                {
                    // <content type="type" src="src">
                    this.writer.WriteStartElement(
                        AtomConstants.AtomNamespacePrefix,
                        AtomConstants.AtomContentElementName,
                        AtomConstants.AtomNamespace);

                    Debug.Assert(!string.IsNullOrEmpty(mediaResource.ContentType), "The default stream content type should have been validated by now. If we have a read link we must have a non-empty content type as well.");
                    this.writer.WriteAttributeString(
                        AtomConstants.AtomTypeAttributeName,
                        mediaResource.ContentType);

                    this.writer.WriteAttributeString(
                        AtomConstants.MediaLinkEntryContentSourceAttributeName,
                        mediaResource.ReadLink.ToUrlAttributeValue(this.BaseUri, this.UrlResolver));

                    // </content>
                    this.writer.WriteEndElement();
                }

                ODataAtomWriterUtils.WriteProperties(
                    this.writer, 
                    this.Model,
                    entryType,
                    propertiesValueCache.EntryProperties, 
                    this.Version,
                    false /* isWritingCollection */,
                    this.WritingResponse,
                    this.WriterBehavior,
                    ODataAtomWriterUtils.WriteEntryPropertiesStart,
                    ODataAtomWriterUtils.WriteEntryPropertiesEnd,
                    this.DuplicatePropertyNamesChecker,
                    propertiesValueCache,
                    rootSourcePathSegment,
                    projectedProperties);
            }
        }

        /// <summary>
        /// Writes the navigation link's start element and atom metadata.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        private void WriteNavigationLinkStart(ODataNavigationLink navigationLink)
        {
            // IsCollection is required for ATOM
            if (!navigationLink.IsCollection.HasValue)
            {
                throw new ODataException(Strings.ODataAtomWriter_AtomLinkMustSpecifyIsCollection);
            }

            // Navigation link must specify the Url
            // NOTE: we currently only require a non-null Url for ATOM payloads and non-expanded navigation links in JSON.
            //       There is no place in JSON to write a Url if the navigation link is expanded. We can't change that for v1 and v2; we
            //       might fix the protocol for v3.
            if (navigationLink.Url == null)
            {
                throw new ODataException(Strings.ODataWriter_NavigationLinkMustSpecifyUrl);
            }

            // <atom:link>
            this.writer.WriteStartElement(AtomConstants.AtomNamespacePrefix, AtomConstants.AtomLinkElementName, AtomConstants.AtomNamespace);

            ODataAtomWriterMetadataUtils.WriteODataNavigationLinkAttributes(this.writer, this.BaseUri, this.UrlResolver, navigationLink);
        }

        /// <summary>
        /// Writes custom extensions and the end element for a navigation link
        /// </summary>
        private void WriteNavigationLinkEnd()
        {
            // </atom:link>
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Determines if XML customization should be applied to the entry and applies it.
        /// </summary>
        /// <param name="entry">The entry to apply the customization to.</param>
        /// <remarks>This method must be called before anything is written for the entry in question.</remarks>
        private void StartEntryXmlCustomization(ODataEntry entry)
        {
            // If we are to customize the XML, replace the writers here:
            if (this.xmlCustomizationWriters != null)
            {
                Debug.Assert(this.xmlCustomizationWriters.Count > 0, "The root xml writer should always be on the stack.");
                Debug.Assert(
                    object.ReferenceEquals(this.xmlCustomizationWriters.Peek(), this.writer),
                    "The this.writer must always be the same writer as the top of the XML customization stack.");

                XmlWriter entryWriter = this.WriterBehavior.StartEntryXmlCustomizationCallback(entry, this.writer);
                if (entryWriter != null)
                {
                    if (object.ReferenceEquals(this.writer, entryWriter))
                    {
                        throw new ODataException(Strings.ODataAtomWriter_StartEntryXmlCustomizationCallbackReturnedSameInstance);
                    }

                    this.writer = entryWriter;
                }

                this.xmlCustomizationWriters.Push(this.writer);
            }
        }

        /// <summary>
        /// Ends XML customization for the entry (if one was applied).
        /// </summary>
        /// <param name="entry">The entry to end the customization for.</param>
        /// <remarks>This method must be called after all the XML for a given entry is written.</remarks>
        private void EndEntryXmlCustomization(ODataEntry entry)
        {
            if (this.xmlCustomizationWriters != null)
            {
                Debug.Assert(
                    this.xmlCustomizationWriters.Count > 1,
                    "The root xml writer should always be on the stack. And when we're ending the entry at least one more must be there as well.");
                Debug.Assert(
                    object.ReferenceEquals(this.xmlCustomizationWriters.Peek(), this.writer),
                    "The this.writer must always be the same writer as the top of the XML customization stack.");

                XmlWriter entryWriter = this.xmlCustomizationWriters.Pop();
                XmlWriter parentWriter = this.xmlCustomizationWriters.Peek();
                if (!object.ReferenceEquals(parentWriter, entryWriter))
                {
                    this.WriterBehavior.EndEntryXmlCustomizationCallback(entry, entryWriter, parentWriter);
                    this.writer = parentWriter;
                }
            }
        }

        /// <summary>
        /// Writes an association link.
        /// </summary>
        /// <param name="associationLink">The association link to write.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        private void WriteAssociationLink(
            ODataAssociationLink associationLink,
            ProjectedPropertiesAnnotation projectedProperties)
        {
            ValidationUtils.ValidateAssociationLinkNotNull(associationLink);
            if (projectedProperties.ShouldSkipProperty(associationLink.Name))
            {
                return;
            }

            this.ValidateAssociationLink(associationLink);
            this.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(associationLink);
            ODataAtomWriterMetadataUtils.WriteODataAssociationLink(this.writer, this.BaseUri, this.UrlResolver, associationLink);
        }

        /// <summary>
        /// A scope for an feed.
        /// </summary>
        private sealed class AtomFeedScope : FeedScope
        {
            /// <summary>true if the author element was already written, false otherwise.</summary>
            private bool authorWritten;

            /// <summary>
            /// Constructor to create a new feed scope.
            /// </summary>
            /// <param name="feed">The feed for the new scope.</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            internal AtomFeedScope(ODataFeed feed, bool skipWriting)
                : base(feed, skipWriting)
            {
                DebugUtils.CheckNoExternalCallers();
            }

            /// <summary>
            /// true if the author element was already written, false otherwise.
            /// </summary>
            internal bool AuthorWritten
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.authorWritten;
                }

                set
                {
                    DebugUtils.CheckNoExternalCallers();
                    this.authorWritten = value;
                }
            }
        }

        /// <summary>
        /// A scope for an entry in ATOM writer.
        /// </summary>
        private sealed class AtomEntryScope : EntryScope
        {
            /// <summary>Bit field of the ATOM elements written so far.</summary>
            private int alreadyWrittenElements;

            /// <summary>
            /// Constructor to create a new entry scope.
            /// </summary>
            /// <param name="entry">The entry for the new scope.</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="writingResponse">true if we are writing a response, false if it's a request.</param>
            /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
            internal AtomEntryScope(ODataEntry entry, bool skipWriting, bool writingResponse, ODataWriterBehavior writerBehavior)
                : base(entry, skipWriting, writingResponse, writerBehavior)
            {
                DebugUtils.CheckNoExternalCallers();
            }

            /// <summary>
            /// Marks the <paramref name="atomElement"/> as written in this entry scope.
            /// </summary>
            /// <param name="atomElement">The ATOM element which was written.</param>
            internal void SetWrittenElement(AtomElement atomElement)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(!this.IsElementWritten(atomElement), "Can't write the same element twice.");
                this.alreadyWrittenElements |= (int)atomElement;
            }

            /// <summary>
            /// Determines if the <paramref name="atomElement"/> was already written for this entry scope.
            /// </summary>
            /// <param name="atomElement">The ATOM element to test for.</param>
            /// <returns>true if the <paramref name="atomElement"/> was already written for this entry scope; false otherwise.</returns>
            internal bool IsElementWritten(AtomElement atomElement)
            {
                DebugUtils.CheckNoExternalCallers();
                return (this.alreadyWrittenElements & (int)atomElement) == (int)atomElement;
            }
        }
    }
}
