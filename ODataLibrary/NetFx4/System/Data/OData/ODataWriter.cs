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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Data.OData.Atom;
    using System.Data.OData.Json;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces.

    /// <summary>
    /// Base class for OData writers.
    /// </summary>
#if INTERNAL_DROP
    internal abstract class ODataWriter : IDisposable
#else
    public abstract class ODataWriter : IDisposable
#endif
    {
        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">Feed/collection to write.</param>
        public abstract void WriteStart(ODataFeed feed);

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">Entry/item to write.</param>
        public abstract void WriteStart(ODataEntry entry);

        /// <summary>
        /// Start writing a link.
        /// </summary>
        /// <param name="link">Link to write.</param>
        public abstract void WriteStart(ODataLink link);

        /// <summary>
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        public abstract void WriteError(ODataError errorInstance, bool includeDebugInformation);

        /// <summary>
        /// Finish writing a feed/entry/link.
        /// </summary>
        public abstract void WriteEnd();

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
        /// IDisposable.Dispose() implementation to cleanup unmanaged resources of the writer.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Create a func which creates an ODataWriter for a given request message and stream.
        /// </summary>
        /// <param name="message">The message to create the writer for.</param>
        /// <param name="settings">Configuration settings for the writer to create.</param>
        /// <param name="format">The OData format used for serialization of the payload.</param>
        /// <param name="encoding">The encoding used for serialization of the payload.</param>
        /// <param name="writingResponse">True if the message writing a response message; otherwise false.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="writingFeed">True when creating a writer to write a feed; false when creating a writer to write an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>A func which creates the writer given a stream to write to.</returns>
        internal static Func<Stream, ODataWriter> Create(
            ODataMessage message,
            ODataWriterSettings settings,
            ODataFormat format,
            Encoding encoding,
            bool writingResponse,
            DataServiceMetadataProviderWrapper metadataProvider,
            bool writingFeed,
            bool synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            return (stream) => CreateWriter(format, encoding, stream, settings, writingResponse, metadataProvider, writingFeed, synchronous);
        }

        /// <summary>
        /// Implement disposal of unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called via IDispose; false if called via a finalizer.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Creates an <see cref="ODataWriter"/> for the specified message and its stream.
        /// </summary>
        /// <param name="format">The serialization format to create the writer for.</param>
        /// <param name="encoding">The encoding to create the writer with.</param>
        /// <param name="stream">The response stream to write to.</param>
        /// <param name="settings">Writer settings to use.</param>
        /// <param name="writingResponse">True if we are writing a response message; false for request messages.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="writingFeed">True when creating a writer to write a feed; false when creating a writer to write an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>The newly created <see cref="ODataWriter"/> instance.</returns>
        /// <remarks>This is used to create the writer once we've obtained the stream from the response.</remarks>
        private static ODataWriter CreateWriter(
            ODataFormat format, 
            Encoding encoding,
            Stream stream, 
            ODataWriterSettings settings, 
            bool writingResponse,
            DataServiceMetadataProviderWrapper metadataProvider,
            bool writingFeed,
            bool synchronous)
        {
            if (settings.BaseUri != null && !settings.BaseUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.ODataWriter_BaseUriMustBeNullOrAbsolute(UriUtils.UriToString(settings.BaseUri)));
            }

            switch (format)
            {
                case ODataFormat.Json:
                    return new ODataJsonWriter(stream, settings, encoding, writingResponse, metadataProvider, writingFeed, synchronous);
                case ODataFormat.Atom:
                    return new ODataAtomWriter(stream, settings, encoding, writingResponse, metadataProvider, writingFeed, synchronous);
                case ODataFormat.Default:
                    Debug.Assert(false, "Should never get here as content-type negotiation should not return Default format for entry or feed.");
                    throw new ODataException(Strings.ODataWriter_CannotCreateWriterForFormat(format.ToString()));
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriter_CreateWriter_UnreachableCodePath));
            }
        }
    }
}
