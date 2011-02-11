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
    #region Namespaces.
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
    /// ODataCollectionWriter for the JSON format.
    /// </summary>
    internal class ODataJsonCollectionWriter : ODataCollectionWriterCore
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
        /// <param name="writerSettings">Configuration settings for the writer to create.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        internal ODataJsonCollectionWriter(
            Stream stream, 
            ODataWriterSettings writerSettings, 
            Encoding encoding, 
            DataServiceMetadataProviderWrapper metadataProvider,
            bool synchronous)
            : base(writerSettings.Version, metadataProvider, synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(writerSettings != null, "writerSettings != null");

            this.outputStream = new AsyncBufferedStream(stream);
            this.textWriter = new StreamWriter(this.outputStream, encoding);

            this.jsonWriter = new JsonWriter(this.textWriter, writerSettings.Indent);
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
        /// Flushes and closes the writer. This method is only called during disposing the ODataCollectionWriter.
        /// </summary>
        /// <param name="discardBufferedData">
        /// If this parameter is true the close of the writer should not throw if some data is still buffered.
        /// If the argument is false the writer is expected to throw if data is still buffered and the writer is closed.
        /// </param>
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
        /// Start writing an OData payload.
        /// </summary>
        protected override void StartPayload()
        {
            // If we're writing a response payload the entire JSON should be wrapped in { "d":  } to guard against XSS attacks
            // it makes the payload a valid JSON but invalid JScript statement.
            this.jsonWriter.StartObjectScope();
            this.jsonWriter.WriteDataWrapper();
        }

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected override void EndPayload()
        {
            // If we were writing a response payload the entire JSON is wrapped in an object scope, which we need to close here.
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        protected override void StartCollection(string collectionName)
        {
            // at the top level, we need to write the "results" wrapper for V2 and higher and for responses only
            if (this.Version >= ODataVersion.V2)
            {
                // { "results":
                this.jsonWriter.StartObjectScope();
                this.jsonWriter.WriteDataArrayName();
            }

            // Write the start of the array for the collection items
            // "["
            this.jsonWriter.StartArrayScope();
        }

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        protected override void EndCollection()
        {
            // Write the end of the array for the collection items
            // "]"
            this.jsonWriter.EndArrayScope();

            // at the top level, we need to close the "results" wrapper for V2 and higher and for responses only
            if (this.Version >= ODataVersion.V2)
            {
                // "}"
                this.jsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Writes a collection item (either primitive or complex)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        protected override void WriteItemImplementation(object item)
        {
            if (item == null)
            {
                this.jsonWriter.WriteValue(null);
            }
            else
            {
                ODataComplexValue complexValue = item as ODataComplexValue;
                if (complexValue != null)
                {
                    ODataJsonWriterUtils.WriteComplexValue(this.jsonWriter, this.MetadataProvider, complexValue, null, false, this.Version);
                }
                else
                {
                    ODataMultiValue multiValue = item as ODataMultiValue;
                    if (multiValue != null)
                    {
                        throw new ODataException(Strings.ODataCollectionWriter_MultiValuesNotSupportedInCollections);
                    }

                    ODataJsonWriterUtils.WritePrimitiveValue(this.jsonWriter, item, null);
                }
            }
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
    }
}
