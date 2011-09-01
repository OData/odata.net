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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// ODataCollectionWriter for the JSON format.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:ImplementIDisposable", Justification = "IDisposable is implemented on ODataMessageWriter.")]
    internal sealed class ODataJsonCollectionWriter : ODataCollectionWriterCore
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
        /// <param name="messageWriterSettings">Configuration settings for the writer to create.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="synchronous">true if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <param name="writingResponse">true if the writer is created for a response message; false for a request message.</param>
        internal ODataJsonCollectionWriter(
            Stream stream, 
            ODataMessageWriterSettings messageWriterSettings, 
            Encoding encoding, 
            IEdmModel model,
            bool synchronous,
            bool writingResponse)
            : base(messageWriterSettings.Version.Value, writingResponse, messageWriterSettings.WriterBehavior, model, synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            this.outputStream = new AsyncBufferedStream(stream);
            this.textWriter = new StreamWriter(this.outputStream, encoding);

            this.jsonWriter = new JsonWriter(this.textWriter, messageWriterSettings.Indent);
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.jsonWriter.Flush();
            this.outputStream.FlushSync();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        protected override Task FlushAsynchronously()
        {
            this.jsonWriter.Flush();
            return this.outputStream.FlushAsync();
        }
#endif

        /// <summary>
        /// Flushes and closes the writer. This method is only called during disposing the ODataCollectionWriter.
        /// </summary>
        protected override void FlushAndCloseWriter()
        {
            try
            {
                // Flush the JSON writer so that we guarantee that there's no data buffered in the JSON writer;
                // the underlying stream verifies that no data is still buffered when disposed below.
                this.jsonWriter.Flush();

                // Always flush the data synchronously before close.
                this.outputStream.FlushSync();

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
            if (this.WritingResponse)
            {
                this.jsonWriter.StartObjectScope();
                this.jsonWriter.WriteDataWrapper();
            }
        }

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected override void EndPayload()
        {
            // If we were writing a response payload the entire JSON is wrapped in an object scope, which we need to close here.
            if (this.WritingResponse)
            {
                this.jsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collection">The <see cref="ODataCollectionResult"/> representing the collection.</param>
        protected override void StartCollection(ODataCollectionResult collection)
        {
            // at the top level, we need to write the "results" wrapper for V2 and higher and for responses only
            if (this.WritingResponse && this.Version >= ODataVersion.V2)
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
            if (this.WritingResponse && this.Version >= ODataVersion.V2)
            {
                // "}"
                this.jsonWriter.EndObjectScope();
            }
        }

        /// <summary>
        /// Writes a collection item (either primitive or complex)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        protected override void WriteCollectionItem(object item)
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
                    ODataJsonWriterUtils.WriteComplexValue(
                        this.jsonWriter, 
                        /*urlResolver*/ null,   // NOTE: no URL resolution needed for collection writing
                        this.Model, 
                        complexValue, 
                        null, 
                        false, 
                        null, 
                        this.Version,
                        this.DuplicatePropertyNamesChecker,
                        this.WritingResponse,
                        this.WriterBehavior);
                    this.DuplicatePropertyNamesChecker.Clear();
                }
                else
                {
                    Debug.Assert(!(item is ODataMultiValue), "!(item is ODataMultiValue)");
                    Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
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
