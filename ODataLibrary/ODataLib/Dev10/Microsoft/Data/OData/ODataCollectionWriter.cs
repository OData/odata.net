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
    /// Base class for OData collection writers.
    /// </summary>
    public abstract class ODataCollectionWriter : IODataWriter
    {
        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collection">The <see cref="ODataCollectionResult"/> representing the collection.</param>
        public abstract void WriteStart(ODataCollectionResult collection);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a collection.
        /// </summary>
        /// <param name="collection">The <see cref="ODataCollectionResult"/> representing the collection.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync(ODataCollectionResult collection);
#endif

        /// <summary>
        /// Start writing a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        public abstract void WriteItem(object item);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteItemAsync(object item);
#endif

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        public abstract void WriteEnd();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously finish writing a collection.
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
        /// Create a func which creates an <see cref="ODataCollectionWriter"/> for a given message and stream.
        /// </summary>
        /// <param name="message">The message to create the writer for.</param>
        /// <param name="settings">Configuration settings for the writer to create.</param>
        /// <param name="format">The OData format used for serialization of the payload.</param>
        /// <param name="encoding">The encoding used for serialization of the payload.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <param name="writingResponse">true if the writer is created for a response message; false for a request message.</param>
        /// <returns>A task returning an OData collection writer to write the payload of the response.</returns>
        internal static Func<Stream, ODataCollectionWriter> Create(
            ODataMessage message,
            ODataMessageWriterSettings settings,
            ODataFormat format,
            Encoding encoding,
            IEdmModel model,
            bool synchronous,
            bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            return (stream) => CreateCollectionWriter(format, encoding, stream, settings, model, synchronous, writingResponse);
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
        /// Creates an <see cref="ODataCollectionWriter"/> for the specified message and its stream.
        /// </summary>
        /// <param name="format">The serialization format to create the writer for.</param>
        /// <param name="encoding">The encoding to create the writer with.</param>
        /// <param name="stream">The response stream to write to.</param>
        /// <param name="settings">Message writer settings to use.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <param name="writingResponse">true if the writer is created for a response message; false for a request message.</param>
        /// <returns>The newly created <see cref="ODataCollectionWriter"/> instance.</returns>
        /// <remarks>This is used to create the collection writer once we've obtained the stream from the response.</remarks>
        private static ODataCollectionWriter CreateCollectionWriter(
            ODataFormat format, 
            Encoding encoding,
            Stream stream, 
            ODataMessageWriterSettings settings, 
            IEdmModel model,
            bool synchronous,
            bool writingResponse)
        {
            Debug.Assert(settings.BaseUri == null || settings.BaseUri.IsAbsoluteUri, "We should have validated that BaseUri is absolute.");

            switch (format)
            {
                case ODataFormat.Json:
                    return new ODataJsonCollectionWriter(stream, settings, encoding, model, synchronous, writingResponse);
                case ODataFormat.Atom:
                    return new ODataAtomCollectionWriter(stream, settings, encoding, model, synchronous, writingResponse);
                case ODataFormat.Default:
                    Debug.Assert(false, "Should never get here as content-type negotiation should not return Default format for collection.");
                    throw new ODataException(Strings.ODataCollectionWriter_CannotCreateCollectionWriterForFormat(format.ToString()));
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionWriter_CreateCollectionWriter_UnreachableCodePath));
            }
        }
    }
}
