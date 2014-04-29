//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Base class for all JSON output contexts.
    /// </summary>
    internal abstract class ODataJsonOutputContextBase : ODataOutputContext
    {
        /// <summary>An in-stream error listener to notify when in-stream error is to be written. Or null if we don't need to notify anybody.</summary>
        protected IODataOutputInStreamErrorListener outputInStreamErrorListener;

        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

        /// <summary>The asynchronous output stream if we're writing asynchronously.</summary>
        private AsyncBufferedStream asynchronousOutputStream;

        /// <summary>The text writer created for the output stream.</summary>
        private TextWriter textWriter;

        /// <summary>The JSON writer to write to.</summary>
        /// <remarks>This field is also used to determine if the output context has been disposed already.</remarks>
        private IJsonWriter jsonWriter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="textWriter">The text writer to write to.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="model">The model to use.</param>
        protected internal ODataJsonOutputContextBase(
            ODataFormat format,
            TextWriter textWriter,
            ODataMessageWriterSettings messageWriterSettings,
            IEdmModel model)
            : base(format, messageWriterSettings, false /*writingResponse*/, true /*synchronous*/, model, null /*urlResolver*/)
        {
            Debug.Assert(textWriter != null, "textWriter != null");

            this.textWriter = textWriter;
            this.jsonWriter = new JsonWriter(this.textWriter, messageWriterSettings.Indent, format, true /*isIeee754Compatible*/);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="messageStream">The message stream to write the payload to.</param>
        /// <param name="encoding">The encoding to use for the payload.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="synchronous">true if the output should be written synchronously; false if it should be written asynchronously.</param>
        /// <param name="isIeee754Compatible">true if it is IEEE754Compatible</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        protected internal ODataJsonOutputContextBase(
            ODataFormat format,
            Stream messageStream,
            Encoding encoding,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            bool synchronous,
            bool isIeee754Compatible,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : base(format, messageWriterSettings, writingResponse, synchronous, model, urlResolver)
        {
            Debug.Assert(messageStream != null, "messageStream != null");

            try
            {
                this.messageOutputStream = messageStream;

                Stream outputStream;
                if (synchronous)
                {
                    outputStream = messageStream;
                }
                else
                {
                    this.asynchronousOutputStream = new AsyncBufferedStream(messageStream);
                    outputStream = this.asynchronousOutputStream;
                }

                this.textWriter = new StreamWriter(outputStream, encoding);
                
                this.jsonWriter = new JsonWriter(this.textWriter, messageWriterSettings.Indent, format, isIeee754Compatible);
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the input context.
                if (ExceptionUtils.IsCatchableExceptionType(e) && messageStream != null)
                {
                    messageStream.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// Returns the <see cref="JsonWriter"/> which is to be used to write the content of the message.
        /// </summary>
        internal IJsonWriter JsonWriter
        {
            get
            {
                Debug.Assert(this.jsonWriter != null, "Trying to get JsonWriter while none is available.");
                return this.jsonWriter;
            }
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        internal void VerifyNotDisposed()
        {
            if (this.messageOutputStream == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Synchronously flush the writer.
        /// </summary>
        internal void Flush()
        {
            this.AssertSynchronous();

            // JsonWriter.Flush will call the underlying TextWriter.Flush.
            // The TextWriter.Flush (which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
            // In the synchronous case the underlying stream is the message stream itself, which will then Flush as well.
            this.jsonWriter.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flush the writer.
        /// </summary>
        /// <returns>Task which represents the pending flush operation.</returns>
        /// <remarks>The method should not throw directly if the flush operation itself fails, it should instead return a faulted task.</remarks>
        internal Task FlushAsync()
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    // JsonWriter.Flush will call the underlying TextWriter.Flush.
                    // The TextWriter.Flush (Which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
                    // In the async case the underlying stream is the async buffered stream, which ignores Flush call.
                    this.jsonWriter.Flush();

                    Debug.Assert(this.asynchronousOutputStream != null, "In async writing we must have the async buffered stream.");
                    return this.asynchronousOutputStream.FlushAsync();
                })
                .FollowOnSuccessWithTask((asyncBufferedStreamFlushTask) => this.messageOutputStream.FlushAsync());
        }
#endif

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "textWriter", Justification = "We don't dispose the jsonWriter or textWriter, instead we dispose the underlying stream directly.")]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            try
            {
                if (this.messageOutputStream != null)
                {
                    // JsonWriter.Flush will call the underlying TextWriter.Flush.
                    // The TextWriter.Flush (Which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
                    this.jsonWriter.Flush();

                    // In the async case the underlying stream is the async buffered stream, so we have to flush that explicitely.
                    if (this.asynchronousOutputStream != null)
                    {
                        this.asynchronousOutputStream.FlushSync();
                        this.asynchronousOutputStream.Dispose();
                    }

                    // Dipose the message stream (note that we OWN this stream, so we always dispose it).
                    this.messageOutputStream.Dispose();
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.asynchronousOutputStream = null;
                this.textWriter = null;
                this.jsonWriter = null;
            }
        }
    }
}
