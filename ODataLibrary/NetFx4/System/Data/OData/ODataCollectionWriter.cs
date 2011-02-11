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
    /// Base class for OData collection writers.
    /// </summary>
#if INTERNAL_DROP
    internal abstract class ODataCollectionWriter : IDisposable
#else
    public abstract class ODataCollectionWriter : IDisposable
#endif
    {
        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        public abstract void WriteStart(string collectionName);

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        public abstract void WriteItem(object item);

        /// <summary>
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        public abstract void WriteError(ODataError errorInstance, bool includeDebugInformation);

        /// <summary>
        /// Finish writing a collection.
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
        /// Create a func which created an <see cref="ODataCollectionWriter"/> for a given request message and stream.
        /// </summary>
        /// <param name="message">The message to create the writer for.</param>
        /// <param name="settings">Configuration settings for the writer to create.</param>
        /// <param name="format">The OData format used for serialization of the payload.</param>
        /// <param name="encoding">The encoding used for serialization of the payload.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>A task returning an OData collection writer to write the payload of the request/response.</returns>
        internal static Func<Stream, ODataCollectionWriter> Create(
            ODataMessage message,
            ODataWriterSettings settings,
            ODataFormat format,
            Encoding encoding,
            DataServiceMetadataProviderWrapper metadataProvider,
            bool synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            return (stream) => CreateCollectionWriter(format, encoding, stream, settings, metadataProvider, synchronous);
        }

        /// <summary>
        /// Implement disposal of unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called via IDispose; false if called via a finalizer.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter"/> for the specified message and its stream.
        /// </summary>
        /// <param name="format">The serialization format to create the writer for.</param>
        /// <param name="encoding">The encoding to create the writer with.</param>
        /// <param name="stream">The response stream to write to.</param>
        /// <param name="settings">Writer settings to use.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>The newly created <see cref="ODataCollectionWriter"/> instance.</returns>
        /// <remarks>This is used to create the collection writer once we've obtained the stream from the response.</remarks>
        private static ODataCollectionWriter CreateCollectionWriter(
            ODataFormat format, 
            Encoding encoding,
            Stream stream, 
            ODataWriterSettings settings, 
            DataServiceMetadataProviderWrapper metadataProvider,
            bool synchronous)
        {
            if (settings.BaseUri != null && !settings.BaseUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.ODataWriter_BaseUriMustBeNullOrAbsolute(UriUtils.UriToString(settings.BaseUri)));
            }

            switch (format)
            {
                case ODataFormat.Json:
                    return new ODataJsonCollectionWriter(stream, settings, encoding, metadataProvider, synchronous);
                case ODataFormat.Atom:
                    return new ODataAtomCollectionWriter(stream, settings, encoding, metadataProvider, synchronous);
                case ODataFormat.Default:
                    Debug.Assert(false, "Should never get here as content-type negotiation should not return Default format for collection.");
                    throw new ODataException(Strings.ODataCollectionWriter_CannotCreateCollectionWriterForFormat(format.ToString()));
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionWriter_CreateCollectionWriter_UnreachableCodePath));
            }
        }
    }
}
