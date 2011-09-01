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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the ODataWriter for the JSON format.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:ImplementIDisposable", Justification = "IDisposable is implemented on ODataMessageWriter.")]
    internal sealed class ODataJsonWriter : ODataWriterCore
    {
        /// <summary>
        /// The output stream used when writing synchronously.
        /// </summary>
        private Stream synchronousOutputStream;

        /// <summary>
        /// The output stream used when writing asynchronously.
        /// A helper buffering stream to overcome the limitation of text writer of supporting only synchronous APIs.
        /// </summary>
        private AsyncBufferedStream asynchronousOutputStream;

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
        /// <param name="messageWriterSettings">Configuration settings for the writer to create.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="writingResponse">true if the writer is to write a response payload; false if it's to write a request payload.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="writingFeed">true if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        /// <param name="synchronous">true if the writer is created for synchronous operation; false for asynchronous.</param>
        internal ODataJsonWriter(
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
            
            this.textWriter = new StreamWriter(outputStream, encoding);

            this.jsonWriter = new JsonWriter(this.textWriter, messageWriterSettings.Indent);
        }

        /// <summary>
        /// Returns the current JsonFeedScope.
        /// </summary>
        private JsonFeedScope CurrentFeedScope
        {
            get
            {
                JsonFeedScope currentJsonFeedScope = this.CurrentScope as JsonFeedScope;
                Debug.Assert(currentJsonFeedScope != null, "Asking for JsonFeedScope when the current scope is not a JsonFeedScope.");
                return currentJsonFeedScope;
            }
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.jsonWriter.Flush();

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
            this.jsonWriter.Flush();

            Debug.Assert(this.synchronousOutputStream == null && this.asynchronousOutputStream != null, "FlushAsynchronously called, but we don't have an async stream.");
            return this.asynchronousOutputStream.FlushAsync();
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
            if (entry == null)
            {
                Debug.Assert(
                    this.ParentExpandedLink != null && !this.ParentExpandedLink.IsCollection.Value,
                        "when entry == null, it has to be and expanded single entry navigation");

                // this is a null expanded single entry and it is null, so write a JSON null as value.
                this.jsonWriter.WriteValue(null);
                return;
            }

            // Write just the object start, nothing else, since we might not have complete information yet
            this.jsonWriter.StartObjectScope();

            // Get the projected properties
            ProjectedPropertiesAnnotation projectedProperties = entry.GetAnnotation<ProjectedPropertiesAnnotation>();

            // Write the metadata
            this.WriteEntryMetadata(entry, projectedProperties);

        }

        /// <summary>
        /// Finish writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected override void EndEntry(ODataEntry entry)
        {
            if (entry == null)
            {
                Debug.Assert(
                    this.ParentExpandedLink != null && !this.ParentExpandedLink.IsCollection.Value,
                        "when entry == null, it has to be and expanded single entry navigation");

                // this is a null expanded single entry and it is null, JSON null should be written as value in StartEntry()
                return;
            }

            // Get the projected properties
            ProjectedPropertiesAnnotation projectedProperties = entry.GetAnnotation<ProjectedPropertiesAnnotation>();

            // Write the properties
            ODataJsonWriterUtils.WriteProperties(
                this.jsonWriter,
                this.UrlResolver,
                this.Model,
                this.EntryEntityType,
                entry.Properties,
                true /*allowNamedStreamProperty*/,
                this.BaseUri,
                this.Version,
                this.DuplicatePropertyNamesChecker,
                projectedProperties,
                this.WritingResponse,
                this.WriterBehavior);

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

            // According to OIPI feeds are allowed in requests and responses:
            //   v1 feeds can appear in requests and responses and don't have a 'results' wrapper
            //   >= v2 feeds can appear at the top level of responses and then have a 'results' wrapper
            //   >= v2 feeds can appear as expanded link content in requests and responses and then have a 'results' wrapper 
            // NOTE: OIPI does not specify a format for top-level >= v2 feeds in requests; as a result we use the v1 format 
            //       for these (i.e., no 'results' wrapper)
            if (this.Version >= ODataVersion.V2 && (this.WritingResponse || !this.IsTopLevel))
            {
                // {
                this.jsonWriter.StartObjectScope();

                // "__count": "number"
                // Write if it's available.
                this.WriteFeedCount(feed);

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
                // "__count": "number"
                // Will be written only if it hasn't been already in the StartFeed.
                this.WriteFeedCount(feed);

                // "__next": "url"
                if (nextPageLink != null)
                {
                    this.jsonWriter.WriteName(JsonConstants.ODataNextLinkName);
                    this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(nextPageLink, this.BaseUri, this.UrlResolver));
                }

                this.jsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Start writing a deferred (non-expanded) navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected override void WriteDeferredNavigationLink(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            // Link must specify the Url for non-expanded links
            // NOTE: we currently only require a non-null Url for ATOM payloads and non-expanded links in JSON.
            //       There is no place in JSON to write a Url if the navigation link is expanded. We can't change that for v1 and v2; we
            //       might fix the protocol for v3.
            if (navigationLink.Url == null)
            {
                throw new ODataException(Strings.ODataWriter_NavigationLinkMustSpecifyUrl);
            }

            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "The navigation link Name should have been validated by now.");
            this.jsonWriter.WriteName(navigationLink.Name);

            // A deferred navigation link is represented as an object
            this.jsonWriter.StartObjectScope();

            // "__deferred": {
            this.jsonWriter.WriteName(JsonConstants.ODataDeferredName);
            this.jsonWriter.StartObjectScope();

            Debug.Assert(navigationLink.Url != null, "The navigation link Url should have been validated by now.");
            this.jsonWriter.WriteName(JsonConstants.ODataNavigationLinkUriName);
            this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(navigationLink.Url, this.BaseUri, this.UrlResolver));

            // End the __deferred object
            this.jsonWriter.EndObjectScope();

            // End the navigation link value
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Start writing an expanded link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected override void StartExpandedLink(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "The navigation link name should have been verified by now.");
            this.jsonWriter.WriteName(navigationLink.Name);

            // Everything else is done in the expanded feed or entry since we include the link related information
            // in the feed/entry object.
            
            // TODO: Design issue # 27: JSON doesn't store link Url for expanded links
            //   figure out if we are going to fix this for V3 and then we would need to write it somewhere around here.
        }

        /// <summary>
        /// Finish writing an expanded navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected override void EndExpandedLink(ODataNavigationLink navigationLink)
        {
            Debug.Assert(navigationLink != null, "navigationLink != null");

            // Nothing to do here, the navigation link is represented as a JSON object which is either the feed or entry
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
        /// <remarks>This is only called during disposal of the writer.</remarks>
        protected override void FlushAndCloseWriter()
        {
            try
            {
                // Flush the JSON writer so that we guarantee that there's no data buffered in the JSON writer;
                // the underlying stream verifies that no data is still buffered when disposed below.
                this.jsonWriter.Flush();

                // Always flush the data synchronously before close.
                if (this.synchronousOutputStream != null)
                {
                    Debug.Assert(this.asynchronousOutputStream == null, "Can't have both sync and async streams.");
                    this.synchronousOutputStream.Flush();

                    // Do not dispose the sync stream since we don't own it.
                }
                else
                {
                    Debug.Assert(this.asynchronousOutputStream != null, "Must have async stream if we don't have a sync stream.");
                    this.asynchronousOutputStream.FlushSync();
                    this.asynchronousOutputStream.Dispose();
                }

                // Do not dispose the text writer since that will always dispose the underlying stream
                // which we can't do at least in the sync case where we don't own that stream.
            }
            finally
            {
                this.jsonWriter = null;
                this.textWriter = null;
                this.synchronousOutputStream = null;
                this.asynchronousOutputStream = null;
            }
        }

        /// <summary>
        /// Create a new feed scope.
        /// </summary>
        /// <param name="feed">The feed for the new scope.</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <returns>The newly create scope.</returns>
        protected override FeedScope CreateFeedScope(ODataFeed feed, bool skipWriting)
        {
            return new JsonFeedScope(feed, skipWriting);
        }

        /// <summary>
        /// Create a new entry scope.
        /// </summary>
        /// <param name="entry">The entry for the new scope.</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <returns>The newly create scope.</returns>
        protected override EntryScope CreateEntryScope(ODataEntry entry, bool skipWriting)
        {
            return new EntryScope(entry, skipWriting, this.WritingResponse, this.WriterBehavior);
        }

        /// <summary>
        /// Writes the __metadata property and its content for an entry
        /// </summary>
        /// <param name="entry">The entry for which to write the metadata.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        private void WriteEntryMetadata(ODataEntry entry, ProjectedPropertiesAnnotation projectedProperties)
        {
            Debug.Assert(entry != null, "entry != null");

            // Write the "__metadata" for the entry
            this.jsonWriter.WriteName(JsonConstants.ODataMetadataName);
            this.jsonWriter.StartObjectScope();

            // Write the "id": "Entity Id"
            string id = entry.Id;
            if (id != null)
            {
                this.jsonWriter.WriteName(JsonConstants.ODataEntryIdName);
                this.jsonWriter.WriteValue(id);
            }

            // Write the "uri": "edit/read-link-uri"
            Uri uriValue = entry.EditLink ?? entry.ReadLink;

            if (uriValue != null)
            { 
                this.jsonWriter.WriteName(JsonConstants.ODataMetadataUriName);
                this.jsonWriter.WriteValue(ODataJsonWriterUtils.UriToAbsoluteUriString(uriValue, this.BaseUri, this.UrlResolver));
            }

            // Write the "etag": "ETag value"
            // TODO: if this is a top-level entry also put the ETag into the headers.
            string etag = entry.ETag;
            if (etag != null)
            {
                ODataJsonWriterUtils.WriteETag(this.jsonWriter, JsonConstants.ODataMetadataETagName, etag);
            }

            // Write the "type": "typename"
            string typeName = entry.TypeName;
            SerializationTypeNameAnnotation serializationTypeNameAnnotation = entry.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

            if (typeName != null)
            {
                this.jsonWriter.WriteName(JsonConstants.ODataMetadataTypeName);
                this.jsonWriter.WriteValue(typeName);
            }

            // Write MLE metadata
            ODataStreamReferenceValue mediaResource = entry.MediaResource;
            if (mediaResource != null)
            {
                WriterValidationUtils.ValidateStreamReferenceValue(mediaResource, true);
                ODataJsonWriterUtils.WriteStreamReferenceValueContent(this.jsonWriter, this.BaseUri, this.UrlResolver, mediaResource);
            }

            // Write properties metadata
            // For now only association links are supported here
            IEnumerable<ODataAssociationLink> associationLinks = entry.AssociationLinks;
            if (associationLinks != null)
            {
                bool firstAssociationLink = true;

                foreach (ODataAssociationLink associationLink in associationLinks)
                {
                    ValidationUtils.ValidateAssociationLinkNotNull(associationLink);
                    if (projectedProperties.ShouldSkipProperty(associationLink.Name))
                    {
                        continue;
                    }

                    if (firstAssociationLink)
                    {
                        // Write the "properties": {
                        this.jsonWriter.WriteName(JsonConstants.ODataMetadataPropertiesName);
                        this.jsonWriter.StartObjectScope();

                        firstAssociationLink = false;
                    }

                    this.ValidateAssociationLink(associationLink);
                    ODataJsonWriterUtils.WriteAssociationLink(this.jsonWriter, this.BaseUri, this.UrlResolver, associationLink, this.DuplicatePropertyNamesChecker);
                }

                if (!firstAssociationLink)
                {
                    // Close the "properties" object
                    this.jsonWriter.EndObjectScope();
                }
            }

            // Close the __metadata object scope
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes the __count property for a feed if it has not been written yet (and the count is specified on the feed).
        /// </summary>
        /// <param name="feed">The feed to write the count for.</param>
        private void WriteFeedCount(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");

            // If we haven't written the count yet and it's available, write it.
            long? inlineCount = feed.Count;
            if (inlineCount.HasValue && !this.CurrentFeedScope.CountWritten)
            {
                this.jsonWriter.WriteName(JsonConstants.ODataCountName);

                this.jsonWriter.WriteValue(inlineCount.Value);
                this.CurrentFeedScope.CountWritten = true;
            }
        }

        /// <summary>
        /// A scope for an feed.
        /// </summary>
        private sealed class JsonFeedScope : FeedScope
        {
            /// <summary>true if the __count was already written, false otherwise.</summary>
            private bool countWritten;

            /// <summary>
            /// Constructor to create a new feed scope.
            /// </summary>
            /// <param name="feed">The feed for the new scope.</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            internal JsonFeedScope(ODataFeed feed, bool skipWriting)
                : base(feed, skipWriting)
            {
                DebugUtils.CheckNoExternalCallers();
            }

            /// <summary>
            /// true if the __count was already written, false otherwise.
            /// </summary>
            internal bool CountWritten
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.countWritten;
                }

                set
                {
                    DebugUtils.CheckNoExternalCallers();
                    this.countWritten = value;
                }
            }
        }
    }
}
