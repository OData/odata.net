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

namespace System.Data.OData.Json
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Implementation of the ODataWriter for the JSON format.
    /// </summary>
    internal sealed class ODataJsonWriter : ODataWriterCore
    {
        /// <summary>
        /// A helper buffering stream to overcome the limitation of text writer of supporting only synchronous APIs.
        /// </summary>
        private AsyncBufferedStream outputStream;

        /// <summary>
        /// The text writer used over the stream to write characters to it.
        /// </summary>
        private StreamWriter textWriter;

        /// <summary>
        /// The underlying JSON writer (low level implementation of JSON)
        /// </summary>
        private JsonWriter jsonWriter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="odataWriterSettings">Configuration settings for the writer to create.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="writingResponse">True if the writer is to write a response payload; false if it's to write a request payload.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="writingFeed">True if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        internal ODataJsonWriter(
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
            this.textWriter = new StreamWriter(this.outputStream, encoding);

            this.jsonWriter = new JsonWriter(this.textWriter, odataWriterSettings.Indent);
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        protected sealed override void FlushSynchronously()
        {
            this.jsonWriter.Flush();
            this.outputStream.FlushSync();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        protected sealed override Task FlushAsynchronously()
        {
            this.jsonWriter.Flush();
            return this.outputStream.FlushAsync();
        }
#endif

        /// <summary>
        /// Starts writing a payload (called exactly once before anything else)
        /// </summary>
        protected override void StartPayload()
        {
            if (this.WritingResponse)
            {
                // If we're writing a response payload the entire JSON should be wrapped in { "d":  } to guard against XSS attacks
                // it makes the payload a valid JSON but invalid JScript statement.
                this.jsonWriter.StartObjectScope();
                this.jsonWriter.WriteDataWrapper();
            }
        }

        /// <summary>
        /// Ends writing a payload (called exactly once after everything else in case of success)
        /// </summary>
        protected override void EndPayload()
        {
            if (this.WritingResponse)
            {
                // If we were writing a response payload the entire JSON is wrapped in an object scope, which we need to close here.
                this.jsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected override void StartEntry(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");

            // Write just the object start, nothing else, since we might not have complete information yet
            this.jsonWriter.StartObjectScope();

            ODataLink parentExpandedLink = this.ParentExpandedLink;
            if (parentExpandedLink != null)
            {
                // TODO: Write out the relationship link for the expanded link here
                // TODO: Version check for relationship link
            }
        }

        /// <summary>
        /// Finish writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected override void EndEntry(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");

            // Write the metadata
            ResourceType entryType = this.WriteEntryMetadata(entry);

            // named streams
            // TODO: implement support for named streams
            if (entry.NamedStreams != null)
            {
            }

            // Write the properties
            ODataJsonWriterUtils.WriteProperties(this.jsonWriter, this.MetadataProvider, entryType, entry.Properties, this.Version);

            // Close the object scope
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected override void StartFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            // We need to write the "results" wrapper for V2 and higher and only for responses or for expanded links
            // Feeds are allowed in expanded links even in requests, but they are not allowed on top-level in requests.
            if (this.Version >= ODataVersion.V2 && (this.WritingResponse || !this.IsTopLevel))
            {
                // {
                this.jsonWriter.StartObjectScope();

                long? inlineCount = feed.Count;
                if (inlineCount.HasValue)
                {
                    this.jsonWriter.WriteName(JsonConstants.ODataCountName);

                    this.jsonWriter.WriteValue(inlineCount.Value);
                }

                // If the feed is an expanded collection of entities this is the place to write the relationship links
                ODataLink parentExpandedLink = this.ParentExpandedLink;
                if (parentExpandedLink != null)
                {
                    // TODO: Write relationship link of the this.ParentExpandedLink here
                    // TODO: Version check for relationship link
                }

                // "results":
                this.jsonWriter.WriteDataArrayName();
            }

            // Start array which will hold the entries in the feed.
            this.jsonWriter.StartArrayScope();
        }

        /// <summary>
        /// Finish writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected override void EndFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            // End the array which holds the entries in the feed.
            this.jsonWriter.EndArrayScope();

            // We need to close the "results" wrapper for V2 and higher and only for responses
            Uri nextPageLink = feed.NextPageLink;
            if (this.Version >= ODataVersion.V2 && (this.WritingResponse || !this.IsTopLevel))
            {
                // "__next": "url"
                if (nextPageLink != null)
                {
                    this.jsonWriter.WriteName(JsonConstants.ODataNextLinkName);
                    this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(nextPageLink, this.BaseUri));
                }

                this.jsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Start writing a regular (non-expanded) link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected override void WriteLink(ODataLink link)
        {
            Debug.Assert(link != null, "link != null");

            ValidationUtils.ValidateLink(link);

            // Link must specify the Url for non-expanded links
            // NOTE: we currently only require a non-null Url for ATOM payloads and non-expanded links in JSON.
            //       There is no place in JSON to write a Url if the link is expanded. We can't change that for v1 and v2; we
            //       might fix the protocol for v3.
            if (link.Url == null)
            {
                throw new ODataException(Strings.ODataWriter_LinkMustSpecifyUrl);
            }

            Debug.Assert(!string.IsNullOrEmpty(link.Name), "The link Name should have been validated by now.");
            this.jsonWriter.WriteName(link.Name);

            // A deferred link is represented as an object
            this.jsonWriter.StartObjectScope();

            // "__deferred": {
            this.jsonWriter.WriteName(JsonConstants.ODataDeferredName);
            this.jsonWriter.StartObjectScope();

            Debug.Assert(link.Url != null, "The link Url should have been validated by now.");
            this.jsonWriter.WriteName(JsonConstants.ODataLinkUriName);
            this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(link.Url, this.BaseUri));

            // End the __deferred object
            this.jsonWriter.EndObjectScope();

            // End the link value
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Start writing an expanded link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected override void StartExpandedLink(ODataLink link)
        {
            Debug.Assert(link != null, "link != null");

            ValidationUtils.ValidateLink(link);

            Debug.Assert(!string.IsNullOrEmpty(link.Name), "The link name should have been verified by now.");
            this.jsonWriter.WriteName(link.Name);

            // Everything else is done in the expanded feed or entry since we include the link related information
            // in the feed/entry object.
            
            // TODO: Design issue # 27: JSON doesn't store link Url for expanded links
            //   figure out if we are going to fix this for V3 and then we would need to write it somewhere around here.
        }

        /// <summary>
        /// Finish writing an expanded link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected override void EndExpandedLink(ODataLink link)
        {
            Debug.Assert(link != null, "link != null");

            // Nothing to do here, the link is represented as a JSON object which is either the feed or entry
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name='error'>The error to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        protected override void SerializeError(ODataError error, bool includeDebugInformation)
        {
            Debug.Assert(error != null, "error != null");
            ODataJsonWriterUtils.WriteError(this.jsonWriter, error, includeDebugInformation);
        }

        /// <summary>
        /// Flush and close the writer.
        /// </summary>
        /// <param name="discardBufferedData">If true discard any buffered data before closing the writer.</param>
        /// <remarks>This is only called during disposal of the writer.</remarks>
        protected override void FlushAndCloseWriter(bool discardBufferedData)
        {
            try
            {
                // Flush the JSON writer so that we guarantee that there's no data buffered in the JSON writer;
                // the underlying stream verifies that no data is still buffered when disposed below.
                this.jsonWriter.Flush();

                if (discardBufferedData)
                {
                    this.outputStream.Clear();
                }

                // The text writer will also dispose the this.outputStream since it owns that stream
                // which in turn will dispose the real output stream underneath it.
                this.textWriter.Dispose();
            }
            finally
            {
                this.jsonWriter = null;
                this.textWriter = null;
                this.outputStream = null;
            }
        }

        /// <summary>
        /// Writes the __metadata property and its content for an entry
        /// </summary>
        /// <param name="entry">The entry for which to write the metadata.</param>
        /// <returns>The resource type of the entry or null if no metadata is available.</returns>
        private ResourceType WriteEntryMetadata(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");

            // Write the "__metadata" for the entry
            this.jsonWriter.WriteName(JsonConstants.ODataMetadataName);
            this.jsonWriter.StartObjectScope();

            // Write the "uri": "edit/read-link-uri"
            Uri uriValue = entry.EditLink;
            if (uriValue == null)
            {
                uriValue = entry.ReadLink;
            }

            if (uriValue != null)
            { 
                this.jsonWriter.WriteName(JsonConstants.ODataMetadataUriName);
                this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(uriValue, this.BaseUri));
            }

            // Write the "etag": "ETag value"
            // TODO: if this is a top-level entry also put the ETag into the headers.
            string etag = entry.ETag;
            if (etag != null)
            {
                this.jsonWriter.WriteName(JsonConstants.ODataMetadataETagName);
                this.jsonWriter.WriteValue(etag);
            }

            // Write the "type": "typename"
            string typeName = entry.TypeName;
            ResourceType entryType = ValidationUtils.ValidateTypeName(this.MetadataProvider, typeName, ResourceTypeKind.EntityType, false);
            if (typeName != null)
            {
                this.jsonWriter.WriteName(JsonConstants.ODataMetadataTypeName);
                this.jsonWriter.WriteValue(typeName);
            }

            // Write MLE metadata
            ODataMediaResource mediaResource = entry.MediaResource;
            if (mediaResource != null)
            {
                // Write the "edit_media": "url"
                Uri mediaEditLink = mediaResource.EditLink;
                if (mediaEditLink != null)
                {
                    this.jsonWriter.WriteName(JsonConstants.ODataMetadataEditMediaName);
                    this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(mediaEditLink, this.BaseUri));
                }

                // Write the "media_src": "url"
                Debug.Assert(mediaResource.ReadLink != null, "The default stream read link should have been validated by now.");
                this.jsonWriter.WriteName(JsonConstants.ODataMetadataMediaUriName);
                this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(mediaResource.ReadLink, this.BaseUri));

                // Write the "content_type": "type"
                Debug.Assert(!string.IsNullOrEmpty(mediaResource.ContentType), "The default stream content type should have been validated by now.");
                this.jsonWriter.WriteName(JsonConstants.ODataMetadataContentTypeName);
                this.jsonWriter.WriteValue(mediaResource.ContentType);

                // Write the "media_etag": "etag"
                string mediaETag = mediaResource.ETag;
                if (mediaETag != null)
                {
                    Debug.Assert(mediaEditLink != null, "The default stream edit link and etag should have been validated by now.");
                    this.jsonWriter.WriteName(JsonConstants.ODataMetadataMediaETagName);
                    this.jsonWriter.WriteValue(mediaETag);
                }
            }

            // Write properties metadata
            // For now only association links are supported here
            IEnumerable<ODataAssociationLink> associationLinks = entry.AssociationLinks;
            if (associationLinks != null)
            {
                bool firstAssociationLink = true;

                foreach (ODataAssociationLink associationLink in associationLinks)
                {
                    ValidationUtils.ValidateAssociationLink(associationLink, this.Version);

                    if (firstAssociationLink)
                    {
                        // Write the "properties": {
                        this.jsonWriter.WriteName(JsonConstants.ODataMetadataPropertiesName);
                        this.jsonWriter.StartObjectScope();

                        firstAssociationLink = false;
                    }

                    // Write the "LinkName": {
                    this.jsonWriter.WriteName(associationLink.Name);
                    this.jsonWriter.StartObjectScope();

                    // Write the "__associationuri": "url"
                    this.jsonWriter.WriteName(JsonConstants.ODataMetadataPropertiesAssociationUriName);
                    this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(associationLink.Url, this.BaseUri));

                    // Close the "LinkName" object
                    this.jsonWriter.EndObjectScope();
                }

                if (!firstAssociationLink)
                {
                    // Close the "properties" object
                    this.jsonWriter.EndObjectScope();
                }
            }

            // Close the __metadata object scope
            this.jsonWriter.EndObjectScope();

            return entryType;
        }
    }
}
