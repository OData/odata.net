//---------------------------------------------------------------------
// <copyright file="StreamingTestStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Test.Taupo.OData.Common;

    /// <summary>
    /// Class for testing streaming scenarios.
    /// </summary>
    class StreamingTestStream : TestStream
    {
        private bool ignoreDispose;
        private long readPosition;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StreamingTestStream()
            : base(new MemoryStream(), /*ignoreDispose*/ true)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        public StreamingTestStream(Stream stream)
            : base(stream, /*ignoreDispose*/ true)
        {
            Debug.Assert(stream != null, "stream != null");
        }
     
        /// <summary>
        /// Flushes the inner stream.
        /// </summary>
        public override void Flush()
        {
            if (this.innerStream != null)
            {
                this.innerStream.Flush();
            }
        }

        /// <summary>
        /// Returns the length of the inner stream.
        /// </summary>
        public override long Length
        {
            get { return this.innerStream.Length; }
        }

        /// <summary>
        /// Returns if the inner stream can read.
        /// </summary>
        public override bool CanRead
        {
            get { return this.innerStream.CanRead; }
        }

        /// <summary>
        /// Returns if the inner stream can write.
        /// </summary>
        public override bool CanWrite 
        { 
            get { return this.innerStream.CanWrite; } 
        }

        /// <summary>
        /// Returns if the inner stream can seek.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return base.CanSeek;
            }
        }

        /// <summary>
        /// Returns if the inner stream can timeout.
        /// </summary>
        public override bool CanTimeout
        {
            get
            {
                return base.CanTimeout;
            }
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        public override void Close()
        {
            base.Close();
        }

        /// <summary>
        /// Gets or sets the read timeout.
        /// </summary>
        public override int ReadTimeout
        {
            get
            {
                return base.ReadTimeout;
            }
            set
            {
                base.ReadTimeout = value;
            }
        }

        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns>Returns -1 if at end of stream otherwise returns 1.</returns>
        public override int ReadByte()
        {
            this.innerStream.Position = this.readPosition;
            int bytesRead = base.ReadByte();
            if (bytesRead > 0)
            {
                this.readPosition += bytesRead;
            }

            return bytesRead;
        }

        /// <summary>
        /// Writes the given byte to the stream.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        public override void WriteByte(byte value)
        {
            this.innerStream.Position = this.innerStream.Length;
            base.WriteByte(value);
        }

        /// <summary>
        /// Gets or sets the position of the inner stream.
        /// </summary>
        public override long Position
        {
            get { return this.innerStream.Position; }
            set { this.innerStream.Position = value; }
        }

        /// <summary>
        /// Read from the stream
        /// </summary>
        /// <param name="buffer">The buffer to read to.</param>
        /// <param name="offset">How far into the stream we should offset.</param>
        /// <param name="count">How many bytes we should read.</param>
        /// <returns>The number of bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.FailNextCall)
            {
                this.FailNextCall = false;
                throw new SystemException("Simulated stream failure.");
            }

            if (this.FailSynchronousCalls)
            {
                throw new SystemException("Synchronous calls are not allowed on the stream.");
            }

            this.innerStream.Position = this.readPosition;
            int bytesRead = this.innerStream.Read(buffer, offset, count);
            this.readPosition += bytesRead;
            return bytesRead;
        }

        /// <summary>
        /// Asynchronous version of read.
        /// </summary>
        /// <param name="buffer">The buffer to read to.</param>
        /// <param name="offset">How far into the stream we should offset.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <param name="callback">The async callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>An IAsyncResult for the operation.</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (this.FailNextCall)
            {
                this.FailNextCall = false;
                throw new SystemException("Simulated stream failure.");
            }

            if (this.FailAsynchronousCalls)
            {
                throw new SystemException("Asynchronous calls are not allowed on the stream.");
            }

            this.innerStream.Position = this.readPosition;
            return this.innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Asynchronous end read.
        /// </summary>
        /// <param name="asyncResult">The async result for the asynchronous operation.</param>
        /// <returns>The number of bytes read.</returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            int bytesRead = this.innerStream.EndRead(asyncResult);
            this.readPosition += bytesRead;
            return bytesRead;
        }

        /// <summary>
        /// Seek to the position specified.
        /// </summary>
        /// <param name="offset">How far after the SeekOrigin to move.</param>
        /// <param name="origin">The origin point.</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.InnerStream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the inner stream.
        /// </summary>
        /// <param name="value">The new length</param>
        public override void SetLength(long value)
        {
            this.InnerStream.SetLength(value);
        }

        /// <summary>
        /// Write the given bytes to the stream
        /// </summary>
        /// <param name="buffer">The bytes to write to the stream.</param>
        /// <param name="offset">The write offset.</param>
        /// <param name="count">The number of bytes.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // This is important since the same stream is used by the reader and writer; the reader will
            // be disposed first and thus dispose the stream; the writer will then try to flush (though
            // there will be nothing to write) so we have to return here (or ignore dispose completely).
            if (count == 0)
            {
                return;
            }

            if (this.FailNextCall)
            {
                this.FailNextCall = false;
                throw new SystemException("Simulated stream failure.");
            }

            if (this.FailSynchronousCalls)
            {
                throw new SystemException("Synchronous calls are not allowed on the stream.");
            }

            this.innerStream.Position = this.innerStream.Length;
            this.InnerStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Asynchronous begin write.
        /// </summary>
        /// <param name="buffer">The bytes to write.</param>
        /// <param name="offset">The offset to move before writing.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="callback">The AsyncCallback.</param>
        /// <param name="state">Current state.</param>
        /// <returns>An IASyncResult for the write operation.</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (this.FailNextCall)
            {
                this.FailNextCall = false;
                throw new SystemException("Simulated stream failure.");
            }

            if (this.FailAsynchronousCalls)
            {
                throw new SystemException("Asynchronous calls are not allowed on the stream.");
            }

            this.innerStream.Position = this.innerStream.Length;
            return base.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// The end of the async write operation
        /// </summary>
        /// <param name="asyncResult">The IAsyncResult returned when calling begin.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            base.EndWrite(asyncResult);
        }

        /// <summary>
        /// Ignore calls to dispose so that the inner stream stays available.
        /// </summary>
        /// <param name="disposing">True if called via IDisposable; false when called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // NOTE: this stream is used by the reader as well as the writer; as such we can never satisfy the
            //       check in the message reader test wrapper that dispose must be called exactly once (or never
            //       if the IgnoreDispose flag is set). So we will always set it to 0 or 1 based on the IgnoreDispose setting.
            if (disposing)
            {
                this.DisposeCount = this.ignoreDispose ? 0 : 1;
            }
        }
    }
}
