//---------------------------------------------------------------------
// <copyright file="ODataRawOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// RAW format output context. Used by RAW values and batch.
    /// </summary>
    internal class ODataRawOutputContext : ODataOutputContext
    {
        /// <summary>The encoding to use for the output.</summary>
        protected Encoding encoding;

        /// <summary>Listener to notify when writing in-stream errors.</summary>
        protected IODataOutputInStreamErrorListener outputInStreamErrorListener;

        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

        /// <summary>The asynchronous output stream if we're writing asynchronously.</summary>
        private AsyncBufferedStream asynchronousOutputStream;

        /// <summary>The output stream to write to (both sync and async cases).</summary>
        private Stream outputStream;

        /// <summary>RawValueWriter used to write actual values to the stream.</summary>
        private RawValueWriter rawValueWriter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        internal ODataRawOutputContext(
            ODataFormat format,
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
            : base(format, messageInfo, messageWriterSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");

            try
            {
                this.messageOutputStream = messageInfo.MessageStream;
                this.encoding = messageInfo.Encoding;

                if (this.Synchronous)
                {
                    this.outputStream = this.messageOutputStream;
                }
                else
                {
                    this.asynchronousOutputStream = new AsyncBufferedStream(this.messageOutputStream);
                    this.outputStream = this.asynchronousOutputStream;
                }
            }
            catch
            {
                this.messageOutputStream.Dispose();
                throw;
            }
        }

        /// <summary>
        /// The output stream to write the payload to.
        /// </summary>
        internal Stream OutputStream
        {
            get
            {
                return this.outputStream;
            }
        }

        /// <summary>
        /// The text writer to use to write text into the payload.
        /// </summary>
        /// <remarks>
        /// InitializeRawValueWriter must be called before this is used.
        ///
        /// Also, within this class we should be using RawValueWriter for everything. Ideally we wouldn't leak the TextWriter out, but
        /// the Batch writer needs it at the moment.
        /// </remarks>
        internal TextWriter TextWriter
        {
            get
            {
                return this.rawValueWriter.TextWriter;
            }
        }

        /// <summary>
        /// Synchronously flush the writer.
        /// </summary>
        internal void Flush()
        {
            this.AssertSynchronous();

            if (this.rawValueWriter != null)
            {
                this.rawValueWriter.Flush();
            }
        }

#if PORTABLELIB
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
                    if (this.rawValueWriter != null)
                    {
                        this.rawValueWriter.Flush();
                    }

                    Debug.Assert(this.asynchronousOutputStream != null, "In async writing we must have the async buffered stream.");
                    return this.asynchronousOutputStream.FlushAsync();
                })
                .FollowOnSuccessWithTask((asyncBufferedStreamFlushTask) => this.messageOutputStream.FlushAsync());
        }
#endif

        /// <summary>
        /// Writes an <see cref="ODataError"/> into the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <remarks>
        /// This method is called if the ODataMessageWriter.WriteError is called once some other
        /// write operation has already started.
        /// The method should write the in-stream error representation for the specific format into the current payload.
        /// Before the method is called no flush is performed on the output context or any active writer.
        /// It is the responsibility of this method to flush the output before the method returns.
        /// </remarks>
        internal override void WriteInStreamError(ODataError error, bool includeDebugInformation)
        {
            if (this.outputInStreamErrorListener != null)
            {
                this.outputInStreamErrorListener.OnInStreamError();
            }

            throw new ODataException(Strings.ODataMessageWriter_CannotWriteInStreamErrorForRawValues);
        }

#if PORTABLELIB
        /// <summary>
        /// Writes an <see cref="ODataError"/> into the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <returns>Task which represents the pending write operation.</returns>
        /// <remarks>
        /// This method is called if the ODataMessageWriter.WriteError is called once some other
        /// write operation has already started.
        /// The method should write the in-stream error representation for the specific format into the current payload.
        /// Before the method is called no flush is performed on the output context or any active writer.
        /// It is the responsibility of this method to make sure that all the data up to this point are written before
        /// the in-stream error is written.
        /// It is the responsibility of this method to flush the output before the task finishes.
        /// </remarks>
        internal override Task WriteInStreamErrorAsync(ODataError error, bool includeDebugInformation)
        {
            if (this.outputInStreamErrorListener != null)
            {
                this.outputInStreamErrorListener.OnInStreamError();
            }

            throw new ODataException(Strings.ODataMessageWriter_CannotWriteInStreamErrorForRawValues);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataAsynchronousWriter" /> to write an async response.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataAsynchronousWriter CreateODataAsynchronousWriter()
        {
            this.AssertSynchronous();

            return this.CreateODataAsynchronousWriterImplementation();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataAsynchronousWriter" /> to write an async response.
        /// </summary>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataAsynchronousWriter> CreateODataAsynchronousWriterAsync()
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataAsynchronousWriterImplementation());
        }
#endif

        /// <summary>
        /// Writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteValue(object value)
        {
            this.AssertSynchronous();

            this.WriteValueImplementation(value);
            this.Flush();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>A running task representing the writing of the value.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteValueAsync(object value)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteValueImplementation(value);
                    return this.FlushAsync();
                });
        }
#endif

        /// <summary>
        /// Initialized a new text writer over the message payload stream.
        /// </summary>
        /// <remarks>This can only be called if the text writer was not yet initialized or it has been closed.
        /// It can be called several times with CloseWriter calls in between though.</remarks>
        internal void InitializeRawValueWriter()
        {
            Debug.Assert(this.rawValueWriter == null, "The rawValueWriter has already been initialized.");

            this.rawValueWriter = new RawValueWriter(this.MessageWriterSettings, this.outputStream, this.encoding);
        }

        /// <summary>
        /// Closes the text writer.
        /// </summary>
        internal void CloseWriter()
        {
            Debug.Assert(this.rawValueWriter != null, "The text writer has not been initialized yet.");

            this.rawValueWriter.Dispose();
            this.rawValueWriter = null;
        }

        /// <summary>
        /// Verifies the output context was not yet disposed, fails otherwise.
        /// </summary>
        internal void VerifyNotDisposed()
        {
            if (this.messageOutputStream == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Flushes all buffered data to the underlying stream synchronously.
        /// </summary>
        internal void FlushBuffers()
        {
            if (this.asynchronousOutputStream != null)
            {
                this.asynchronousOutputStream.FlushSync();
            }
        }

#if PORTABLELIB
        /// <summary>
        /// Flushes all buffered data to the underlying stream asynchronously.
        /// </summary>
        /// <returns>Task which represents the pending operation.</returns>
        internal Task FlushBuffersAsync()
        {
            if (this.asynchronousOutputStream != null)
            {
                return this.asynchronousOutputStream.FlushAsync();
            }
            else
            {
                return TaskUtils.CompletedTask;
            }
        }
#endif

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "rawValueWriter", Justification = "We intentionally don't dispose rawValueWriter, we instead dispose the underlying stream manually.")]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.messageOutputStream != null)
                {
                    if (this.rawValueWriter != null)
                    {
                        this.rawValueWriter.Flush();
                    }

                    // In the async case the underlying stream is the async buffered stream, so we have to flush that explicitly.
                    if (this.asynchronousOutputStream != null)
                    {
                        this.asynchronousOutputStream.FlushSync();
                        this.asynchronousOutputStream.Dispose();
                    }

                    // Dispose the message stream (note that we OWN this stream, so we always dispose it).
                    this.messageOutputStream.Dispose();
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.asynchronousOutputStream = null;
                this.outputStream = null;
                this.rawValueWriter = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <remarks>Once the method returns all the data should be written, the only other call after this will be Dispose on the output context.</remarks>
        private void WriteValueImplementation(object value)
        {
            byte[] binaryValue = value as byte[];

            if (binaryValue != null)
            {
                // write the bytes directly
                this.OutputStream.Write(binaryValue, 0, binaryValue.Length);
            }
            else
            {
                value = this.Model.ConvertToUnderlyingTypeIfUIntValue(value);

                this.InitializeRawValueWriter();
                this.rawValueWriter.Start();
                this.rawValueWriter.WriteRawValue(value);
                this.rawValueWriter.End();
            }
        }

        /// <summary>
        /// Creates an async writer.
        /// </summary>
        /// <returns>The newly created async writer.</returns>
        private ODataAsynchronousWriter CreateODataAsynchronousWriterImplementation()
        {
            // Async writer needs the default encoding to not use the preamble.
            this.encoding = this.encoding ?? MediaTypeUtils.EncodingUtf8NoPreamble;
            ODataAsynchronousWriter asyncWriter = new ODataAsynchronousWriter(this);
            this.outputInStreamErrorListener = asyncWriter;
            return asyncWriter;
        }
    }
}
