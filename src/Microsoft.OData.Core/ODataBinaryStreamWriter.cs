//---------------------------------------------------------------------
// <copyright file="ODataBinaryStreamWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.OData.Buffers;
    using Microsoft.OData.Json;

    /// <summary>
    /// A stream for writing base64 encoded binary values.
    /// </summary>
    internal sealed class ODataBinaryStreamWriter : Stream
    {
        /// <summary>Desired minimum bytes per write operation.</summary>
        private const int MinBytesPerWriteEvent = 3;

        /// <summary>The writer to write to the underlying stream.</summary>
        private readonly TextWriter Writer;

        /// <summary>Trailing bytes from a previous write to be prepended to the next write.</summary>
        private byte[] trailingBytes = new byte[0];

        /// <summary>
        /// The wrapped buffer to help with streaming responses.
        /// </summary>
        private Ref<char[]> wrappedBuffer;

        /// <summary>
        /// Get/sets the character buffer pool for streaming.
        /// </summary>
        private ICharArrayPool bufferPool;

        /// <summary>An empty byte[].</summary>
        private static byte[] emptyByteArray = new byte[0];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="writer">A Textwriter for writing to the stream.</param>
        public ODataBinaryStreamWriter(TextWriter writer)
        {
            Debug.Assert(writer != null, "Creating a binary stream writer for a null textWriter.");

            this.Writer = writer;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="writer">A Textwriter for writing to the stream.</param>
        /// <param name="streamingBuffer">A temporary buffer to use when converting binary values.</param>
        /// <param name="bufferPool">Array pool for renting a buffer.</param>
        public ODataBinaryStreamWriter(TextWriter writer, Ref<char[]> wrappedBuffer, ICharArrayPool bufferPool)
        {
            this.Writer = writer;
            this.wrappedBuffer = wrappedBuffer;
            this.bufferPool = bufferPool;
        }

        /// <summary>
        /// Determines if the stream can read - this one can't
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the stream can seek - this one can't
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the stream can write - this one can
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the length of the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the position in the stream. Setting of the position is not supported since the stream doesn't support seeking.
        /// </summary>
        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="bytes">The bytes to write to the stream.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] bytes, int offset, int count)
        {
            Debug.Assert(this.wrappedBuffer != null, "this.wrappedBuffer != null");

            // if we have less than 3 bytes, store the bytes and continue
            if (count + trailingBytes.Length < MinBytesPerWriteEvent)
            {
                this.trailingBytes = this.trailingBytes.Concat(bytes.Skip(offset).Take(count)).ToArray();
                return;
            }

            byte[] byteArray;

            PrepareByteArray(bytes, offset, count, out byteArray);

            JsonValueUtils.WriteBinaryString(this.Writer, byteArray, this.wrappedBuffer, this.bufferPool);
        }

        /// <inheritdoc />
        public override async Task WriteAsync(byte[] bytes, int offset, int count, CancellationToken cancellationToken)
        {
            Debug.Assert(this.wrappedBuffer != null, "this.wrappedBuffer != null");

            // if we have less than 3 bytes, store the bytes and continue
            if (count + trailingBytes.Length < MinBytesPerWriteEvent)
            {
                this.trailingBytes = this.trailingBytes.Concat(bytes.Skip(offset).Take(count)).ToArray();
                return;
            }

            byte[] byteArray;

            PrepareByteArray(bytes, offset, count, out byteArray);

            await JsonValueUtils.WriteBinaryStringAsync(this.Writer, 
                byteArray, this.wrappedBuffer, this.bufferPool).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads data from the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="buffer">The buffer to read the data to.</param>
        /// <param name="offset">The offset in the buffer to write to.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Seeks the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The origin of the seek operation.</param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Flush the stream to the underlying batch stream.
        /// </summary>
        public override void Flush()
        {
            this.Writer.Flush();
        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return this.Writer.FlushAsync();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called form the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            // write any trailing bytes to stream
            if (disposing && this.trailingBytes != null && this.trailingBytes.Length > 0)
            {
                this.Writer.Write(Convert.ToBase64String(trailingBytes, 0, trailingBytes.Length));
                trailingBytes = null;
            }

            this.Writer.Flush();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Prepares bytes to be written for the particular write event
        /// </summary>
        /// <param name="bytes">The bytes to write to the stream.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes targeted in this write event.</param>
        /// <param name="byteArray">The bytes to be written in this write event.</param>
        private void PrepareByteArray(byte[] bytes, int offset, int count, out byte[] byteArray)
        {
            int trailingBytesLength = this.trailingBytes.Length;
            byte[] prefixByteString = emptyByteArray;
            int numberOfBytesToPrefix = trailingBytesLength > 0 ? MinBytesPerWriteEvent - trailingBytesLength : 0;

            // if we have bytes left over from the previous write, prepend them
            if (trailingBytesLength > 0)
            {
                // convert the trailing bytes plus the first 3-trailingByteLength bytes of the new byte[]
                prefixByteString = trailingBytes.Concat(bytes.Skip(offset).Take(numberOfBytesToPrefix)).ToArray();
            }

            // compute if there will be trailing bytes from this write
            int remainingBytes = (count - numberOfBytesToPrefix) % MinBytesPerWriteEvent;
            trailingBytes = bytes.Skip(offset + count - remainingBytes).Take(remainingBytes).ToArray();

            // TODO: Too much LINQ? Investigate a more performant way of achieving this
            byteArray = prefixByteString.Concat(bytes
                .Skip(offset + numberOfBytesToPrefix)
                .Take(count - numberOfBytesToPrefix - remainingBytes)).ToArray();
        }
    }
}
