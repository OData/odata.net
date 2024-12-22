//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriter.Stream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System;
    using System.Buffers;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed partial class ODataUtf8JsonWriter
    {
        /// <summary>
        /// Current stream for writing a binary property.
        /// </summary>
        private Stream binaryValueStream = null;

        /// <summary>
        /// Starts a scope for writing a stream value.
        /// </summary>
        /// <returns>A stream for writing the stream value.</returns>
        public Stream StartStreamValueScope()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();

            this.WriteItemWithSeparatorIfNeeded();

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);
            this.Flush();

            this.binaryValueStream =  new ODataUtf8JsonWriteStream(this);

            return this.binaryValueStream;
        }

        /// <summary>
        /// Ends a scope for writing a stream value.
        /// </summary>
        public void EndStreamValueScope()
        {
            this.binaryValueStream?.Dispose();
            this.binaryValueStream = null;
            this.Flush();

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            CheckIfSeparatorNeeded();
            CheckIfManualValueAtArrayStart();
        }

        /// <summary>
        /// Asynchronously starts a scope for writing a stream value.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains a stream for writing the stream value.</returns>
        public async Task<Stream> StartStreamValueScopeAsync()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();

            this.WriteItemWithSeparatorIfNeeded();
            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);
            await this.FlushAsync().ConfigureAwait(false);

            this.binaryValueStream = new ODataUtf8JsonWriteStream(this);

            return this.binaryValueStream;
        }

        /// <summary>
        /// Asynchronously ends a scope for writing a stream value.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task EndStreamValueScopeAsync()
        {
            if (this.binaryValueStream != null)
            {
                await this.binaryValueStream.DisposeAsync().ConfigureAwait(false);
                this.binaryValueStream = null;
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            CheckIfSeparatorNeeded();
            CheckIfManualValueAtArrayStart();
        }

        /// <summary>
        /// Represents a stream for writing UTF-8 JSON data to an ODataUtf8JsonWriter.
        /// </summary>
        internal sealed class ODataUtf8JsonWriteStream : Stream
        {
            private readonly ODataUtf8JsonWriter jsonWriter = null;
            private byte[] buffer;
            int numBytesNotWrittenFromPreviousChunk = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="ODataUtf8JsonWriter"> class with the specified ODataUtf8JsonWriter.
            /// </summary>
            /// <param name="writer">The OData UTF-8 JSON writer to write to.</param>
            internal ODataUtf8JsonWriteStream(ODataUtf8JsonWriter writer)
            {
                this.jsonWriter = writer;
            }

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            /// <summary>
            /// Gets the length in bytes of the stream. This is not supported by this stream.
            public override long Length => throw new NotSupportedException();

            /// <summary>
            /// Gets or sets the position within the stream. This is not supported by this stream.
            /// </summary>
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            /// <summary>
            /// Flushes any buffered data to the underlying stream synchronously.
            /// </summary>
            public override void Flush()
            {
                this.jsonWriter.Flush();
            }

            /// <summary>
            /// Flushes any buffered data to the underlying stream asynchronously.
            /// </summary>
            /// <param name="cancellationToken">A A cancellation token to observe while waiting for the flush operation to complete.</param>
            /// <returns>A task representing the asynchronous flush operation.</returns>
            public override async Task FlushAsync(CancellationToken cancellationToken)
            {
                await this.jsonWriter.FlushAsync().ConfigureAwait(false);
            }

            /// <summary>
            /// Disposes the object.
            /// </summary>
            /// <param name="disposing">true if called from Dispose; false if called form the finalizer.</param>
            protected override void Dispose(bool disposing)
            {
                if (this.numBytesNotWrittenFromPreviousChunk > 0)
                {
                    // If there are unprocessed bytes, encode and write them as the final block.
                    ReadOnlySpan<byte> bytesNotProcessedFromPreviousChunk = this.buffer.AsSpan().Slice(0, this.numBytesNotWrittenFromPreviousChunk);

                    this.jsonWriter.Base64EncodeAndWriteChunk(bytesNotProcessedFromPreviousChunk, isFinalBlock: true, out this.numBytesNotWrittenFromPreviousChunk);
                    Debug.Assert(numBytesNotWrittenFromPreviousChunk == 0, "numBytesNotWrittenFromPreviousChunk == 0");
                }

                if (this.buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(this.buffer);
                }

                this.Flush();
                base.Dispose(disposing);
            }

            /// <summary>
            /// Asynchronously disposes of the current object and performs cleanup operations.
            /// </summary>
            /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
            public override async ValueTask DisposeAsync()
            {
                if (this.numBytesNotWrittenFromPreviousChunk > 0)
                {
                    // If there are unprocessed bytes, encode and write them as the final block.
                    ReadOnlyMemory<byte> bytesNotProcessedFromPreviousChunk = this.buffer.AsMemory().Slice(0, this.numBytesNotWrittenFromPreviousChunk);

                    this.jsonWriter.Base64EncodeAndWriteChunk(bytesNotProcessedFromPreviousChunk.Span, isFinalBlock: true, out this.numBytesNotWrittenFromPreviousChunk);
                    Debug.Assert(numBytesNotWrittenFromPreviousChunk == 0, "numBytesNotWrittenFromPreviousChunk == 0");
                }

                if (this.buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(this.buffer);
                }

                await this.jsonWriter.FlushAsync().ConfigureAwait(false);
            }

            /// <summary>
            /// Reads a sequence of bytes from the current stream and advances the position within the stream by the numbers of bytes read. This operation is not supported by this stream.
            /// </summary>
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Sets the position within the stream. This operation is not supported by this stream.
            /// </summary>
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets the length in bytes of the stream. This is not supported by this stream.
            /// </summary>
            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Writes a portion of a byte array to the underlying stream in chunks.
            /// </summary>
            /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/>bytes from buffer to the current stream.</param>
            /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
            /// <param name="count">The number of bytes to be written to the current stream.</param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                Span<byte> value = buffer.AsSpan().Slice(offset, count);

                this.WriteByteValueInChunks(value);
            }

            /// <summary>
            /// Asynchronously writes a portion of a byte array to the underlying stream in chunks.
            /// </summary>
            /// <param name="buffer">The byte array from which data will be written.</param>
            /// <param name="offset">The zero-based byte offset in the buffer at which to begin copying bytes to the stream.</param>
            /// <param name="count">The maximum number of bytes to write.</param>
            /// <param name="token">A CancellationToken to observe while waiting for the task to complete.</param>
            /// <returns>A task representing the asynchronous write operation.</returns>
            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
            {
                ReadOnlyMemory<byte> value = buffer.AsMemory().Slice(offset, count);
                await this.WriteByteValueInChunksAsync(value).ConfigureAwait(false);
            }

            /// <summary>
            /// Writes the byte value represented by the provided read-only span in chunks using Base64 encoding.
            /// </summary>
            /// <param name="value">The read-only span containing the byte value to be written.</param>
            private void WriteByteValueInChunks(ReadOnlySpan<byte> value)
            {
                // Process the input value in chunks
                for (int i = 0; i < value.Length; i += ODataUtf8JsonWriter.chunkSize)
                {
                    int remainingBytes = Math.Min(ODataUtf8JsonWriter.chunkSize, value.Length - i);
                    bool isFinalBlock = false;

                    // Take a chunk of bytes from the input value.
                    ReadOnlySpan<byte> chunk = value.Slice(i, remainingBytes);

                    // If the buffer is not empty, then we copy the bytes from the buffer
                    // to the current chunk being processed.
                    if (this.numBytesNotWrittenFromPreviousChunk > 0)
                    {
                        // Get unprocessed bytes from the buffer.
                        ReadOnlySpan<byte> bytesNotProcessedFromPreviousChunk = this.buffer.AsSpan().Slice(0, this.numBytesNotWrittenFromPreviousChunk);
                        int totalLength = bytesNotProcessedFromPreviousChunk.Length + chunk.Length;

                        // Rent an array to hold bytes from both previous and current chunks.
                        byte[] combinedArray = ArrayPool<byte>.Shared.Rent(totalLength);

                        // Copy bytes from bytesNotProcessedFromPreviousChunk to the combined array
                        bytesNotProcessedFromPreviousChunk.CopyTo(combinedArray);

                        // Copy bytes from chunk to the combined array, starting from the end of bytesNotProcessedFromPreviousChunk
                        chunk.CopyTo(combinedArray.AsSpan().Slice(bytesNotProcessedFromPreviousChunk.Length));

                        WriteChunk(combinedArray.AsSpan().Slice(0, totalLength), totalLength, isFinalBlock);

                        ArrayPool<byte>.Shared.Return(combinedArray);
                    }
                    else
                    {
                        WriteChunk(chunk, chunk.Length, isFinalBlock);
                    }

                    // Flush the writer if the buffer threshold is reached
                    this.jsonWriter.DrainBufferIfThresholdReached();
                }
            }

            /// <summary>
            /// Writes the byte value represented by the provided read-only memory in chunks using Base64 encoding.
            /// </summary>
            /// <param name="value">The read-only memory containing the byte value to be written.</param>
            private async ValueTask WriteByteValueInChunksAsync(ReadOnlyMemory<byte> value)
            {
                // Process the input value in chunks
                for (int i = 0; i < value.Length; i += ODataUtf8JsonWriter.chunkSize)
                {
                    int remainingBytes = Math.Min(ODataUtf8JsonWriter.chunkSize, value.Length - i);
                    bool isFinalBlock = false ;

                    // Take a chunk of bytes from the input value.
                    ReadOnlyMemory<byte> chunk = value.Slice(i, remainingBytes);

                    // If the buffer is not empty, then we copy the bytes from the buffer
                    // to the current chunk being processed.
                    if (this.numBytesNotWrittenFromPreviousChunk > 0)
                    {
                        ReadOnlyMemory<byte> bytesNotProcessedFromPreviousChunk = this.buffer.AsMemory().Slice(0, this.numBytesNotWrittenFromPreviousChunk);

                        int totalLength = bytesNotProcessedFromPreviousChunk.Length + chunk.Length;

                        // Rent an array to hold bytes from both previous and current chunks.
                        byte[] combinedArray = ArrayPool<byte>.Shared.Rent(totalLength);

                        // Copy bytes from bytesNotProcessedFromPreviousChunk to the combined array
                        bytesNotProcessedFromPreviousChunk.Span.CopyTo(combinedArray);

                        // Copy bytes from chunk to the combined array, starting from the end of bytesNotProcessedFromPreviousChunk
                        chunk.Span.CopyTo(combinedArray.AsSpan(bytesNotProcessedFromPreviousChunk.Length));

                        WriteChunk(combinedArray.AsSpan().Slice(0, totalLength), totalLength, isFinalBlock);

                        ArrayPool<byte>.Shared.Return(combinedArray);
                    }
                    else
                    {
                        WriteChunk(chunk.Span, chunk.Length, isFinalBlock);
                    }

                    // Flush the writer if the buffer threshold is reached
                    await this.jsonWriter.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
                }
            }

            /// <summary>
            /// Writes a chunk of data, encoding it using Base64, and updates the buffer with any unprocessed bytes.
            /// </summary>
            /// <param name="chunk">The chunk of data to be encoded and written.</param>
            /// <param name="isFinalBlock">A boolean indicating whether this is the final block of data.</param>
            private void WriteChunk(ReadOnlySpan<byte> chunk, int chunkLength, bool isFinalBlock)
            {
                // Encode the current chunk using Base64 and write it
                this.jsonWriter.Base64EncodeAndWriteChunk(chunk.Slice(0, chunkLength), isFinalBlock, out this.numBytesNotWrittenFromPreviousChunk);

                if (this.numBytesNotWrittenFromPreviousChunk > 0)
                {
                    if (this.buffer == null)
                    {
                        this.buffer = ArrayPool<byte>.Shared.Rent(ODataUtf8JsonWriter.chunkSize);
                    }

                    // Update the buffer with unprocessed bytes from the current chunk.
                    chunk.Slice(chunkLength - this.numBytesNotWrittenFromPreviousChunk).CopyTo(this.buffer.AsSpan());
                }
            }
        }
    }
}
