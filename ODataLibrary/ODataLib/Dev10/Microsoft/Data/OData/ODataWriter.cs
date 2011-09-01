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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData writers.
    /// </summary>
    public abstract class ODataWriter : IODataWriter
    {
        /// <summary>
        /// The optional URL resolver to perform custom URL resolution for URLs written to the payload.
        /// </summary>
        private readonly IODataUrlResolver urlResolver;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        internal ODataWriter(IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            this.urlResolver = urlResolver;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ODataWriter()
        {
        }

        /// <summary>
        /// The optional URL resolver to perform custom URL resolution for URLs written to the payload.
        /// </summary>
        internal IODataUrlResolver UrlResolver
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.urlResolver;
            }
        }

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">Feed/collection to write.</param>
        public abstract void WriteStart(ODataFeed feed);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a feed.
        /// </summary>
        /// <param name="feed">Feed/collection to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataFeed feed);
#endif

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">Entry/item to write.</param>
        public abstract void WriteStart(ODataEntry entry);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing an entry.
        /// </summary>
        /// <param name="entry">Entry/item to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataEntry entry);
#endif

        /// <summary>
        /// Start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">Navigation link to write.</param>
        public abstract void WriteStart(ODataNavigationLink navigationLink);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">Navigation link to writer.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataNavigationLink navigationLink);
#endif

        /// <summary>
        /// Finish writing a feed/entry/navigation link.
        /// </summary>
        public abstract void WriteEnd();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously finish writing a feed/entry/navigation link.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();
#endif

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public abstract void Flush();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public abstract Task FlushAsync();
#endif

        /// <summary>
        /// Synchronously flushes the write buffer to the underlying stream.
        /// </summary>
        void IODataWriter.FlushWriter()
        {
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        Task IODataWriter.FlushWriterAsync()
        {
            return this.FlushAsync();
        }
#endif

        /// <summary>
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        void IODataWriter.WriteError(ODataError errorInstance, bool includeDebugInformation)
        {
            this.WriteErrorImplementation(errorInstance, includeDebugInformation);
        }

        /// <summary>
        /// This method will be called by ODataMessageWriter.Dispose() to dispose the object implementing this interface.
        /// </summary>
        void IODataWriter.DisposeWriter()
        {
            this.DisposeWriterImplementation();
        }

        /// <summary>
        /// Create a func which creates an ODataWriter for a given request message and stream.
        /// </summary>
        /// <param name="message">The message to create the writer for.</param>
        /// <param name="settings">Configuration settings for the writer to create.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="format">The OData format used for serialization of the payload.</param>
        /// <param name="encoding">The encoding used for serialization of the payload.</param>
        /// <param name="writingResponse">True if writing a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="writingFeed">True when creating a writer to write a feed; false when creating a writer to write an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>A func which creates the writer given a stream to write to.</returns>
        internal static Func<Stream, ODataWriter> Create(
            ODataMessage message,
            ODataMessageWriterSettings settings,
            IODataUrlResolver urlResolver,
            ODataFormat format,
            Encoding encoding,
            bool writingResponse,
            IEdmModel model,
            bool writingFeed,
            bool synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            return (stream) => CreateWriter(format, encoding, stream, settings, urlResolver, writingResponse, model, writingFeed, synchronous);
        }

        /// <summary>
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        protected abstract void WriteErrorImplementation(ODataError errorInstance, bool includeDebugInformation);

        /// <summary>
        /// Implement disposal of unmanaged resources.
        /// </summary>
        protected abstract void DisposeWriterImplementation();

        /// <summary>
        /// Creates an <see cref="ODataWriter"/> for the specified message and its stream.
        /// </summary>
        /// <param name="format">The serialization format to create the writer for.</param>
        /// <param name="encoding">The encoding to create the writer with.</param>
        /// <param name="stream">The response stream to write to.</param>
        /// <param name="settings">Message writer settings to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="writingResponse">True if we are writing a response message; false for request messages.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="writingFeed">True when creating a writer to write a feed; false when creating a writer to write an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>The newly created <see cref="ODataWriter"/> instance.</returns>
        /// <remarks>This is used to create the writer once we've obtained the stream from the response.</remarks>
        private static ODataWriter CreateWriter(
            ODataFormat format, 
            Encoding encoding,
            Stream stream, 
            ODataMessageWriterSettings settings, 
            IODataUrlResolver urlResolver,
            bool writingResponse,
            IEdmModel model,
            bool writingFeed,
            bool synchronous)
        {
            Debug.Assert(settings.BaseUri == null || settings.BaseUri.IsAbsoluteUri, "We should have validated that BaseUri is absolute.");

            switch (format)
            {
                case ODataFormat.Json:
                    return new ODataJsonWriter(stream, settings, urlResolver, encoding, writingResponse, model, writingFeed, synchronous);
                case ODataFormat.Atom:
                    return new ODataAtomWriter(stream, settings, urlResolver, encoding, writingResponse, model, writingFeed, synchronous);
                case ODataFormat.Default:
                    Debug.Assert(false, "Should never get here as content-type negotiation should not return Default format for entry or feed.");
                    throw new ODataException(Strings.ODataWriter_CannotCreateWriterForFormat(format.ToString()));
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriter_CreateWriter_UnreachableCodePath));
            }
        }
    }
}
