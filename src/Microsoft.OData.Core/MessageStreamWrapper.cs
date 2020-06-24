//---------------------------------------------------------------------
// <copyright file="MessageStreamWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData
{
    /// <summary>
    /// Factory class for the various wrapper streams around the actual message stream.
    /// </summary>
    internal static class MessageStreamWrapper
    {
        /// <summary>
        /// Creates a non-disposing stream.
        /// </summary>
        /// <param name="innerStream">The inner <see cref="Stream"/> to wrap.</param>
        /// <returns>A stream wrapping the <paramref name="innerStream"/> that ignores calls to Dispose.</returns>
        internal static Stream CreateNonDisposingStream(Stream innerStream)
        {
            Debug.Assert(innerStream != null, "innerStream != null");

            return new MessageStreamWrappingStream(innerStream, /*ignoreDispose*/ true, /*maxBytesToBeRead*/ -1);
        }

        /// <summary>
        /// Creates a stream with a given maximum size.
        /// </summary>
        /// <param name="innerStream">The inner <see cref="Stream"/> to wrap.</param>
        /// <param name="maxBytesToBeRead">The maximum number of bytes to be read from the <paramref name="innerStream"/>.</param>
        /// <returns>A stream wrapping the <paramref name="innerStream"/> that
        /// enforces the maximum number of bytes to be read from the stream.</returns>
        internal static Stream CreateStreamWithMaxSize(Stream innerStream, long maxBytesToBeRead)
        {
            Debug.Assert(innerStream != null, "innerStream != null");

            return new MessageStreamWrappingStream(innerStream, /*ignoreDispose*/ false, maxBytesToBeRead);
        }

        /// <summary>
        /// Creates a non-disposing stream with a given maximum size.
        /// </summary>
        /// <param name="innerStream">The inner <see cref="Stream"/> to wrap.</param>
        /// <param name="maxBytesToBeRead">The maximum number of bytes to be read from the <paramref name="innerStream"/>.</param>
        /// <returns>A stream wrapping the <paramref name="innerStream"/> that ignores calls to Dispose and
        /// enforces the maximum number of bytes to be read from the stream.</returns>
        internal static Stream CreateNonDisposingStreamWithMaxSize(Stream innerStream, long maxBytesToBeRead)
        {
            Debug.Assert(innerStream != null, "innerStream != null");

            return new MessageStreamWrappingStream(innerStream, /*ignoreDispose*/ true, maxBytesToBeRead);
        }

        /// <summary>
        /// Checks whether the provided stream already ignores calls to Dispose.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to check.</param>
        /// <returns>true if the <paramref name="stream"/> ignores calls to Dispose; otherwise false.</returns>
        internal static bool IsNonDisposingStream(Stream stream)
        {
            Debug.Assert(stream != null, "stream != null");

            MessageStreamWrappingStream wrappingStream = stream as MessageStreamWrappingStream;
            return wrappingStream != null && wrappingStream.IgnoreDispose;
        }

        /// <summary>
        /// Stream wrapper that supports counting the total number of bytes read from the stream and ensures
        /// that they don't exceed a specified maximum (used for security purposes) and ignoring calls
        /// to Dispose if the underlying stream should not be disposed.
        /// </summary>
        private sealed class MessageStreamWrappingStream : Stream
        {
            /// <summary>The maximum number of bytes to be read from the stream before reporting an error.</summary>
            private readonly long maxBytesToBeRead;

            /// <summary>true to not dispose the inner stream when Dispose is called; otherwise false.</summary>
            private readonly bool ignoreDispose;

            /// <summary>Stream that is being wrapped.</summary>
            private Stream innerStream;

            /// <summary>The total number of bytes read from the stream so far.</summary>
            private long totalBytesRead;

            /// <summary>
            /// Constructs an instance of the byte counting stream wrapper class.
            /// </summary>
            /// <param name="innerStream">Stream that is being wrapped.</param>
            /// <param name="ignoreDispose">true if calls to Dispose should be ignored; otherwise false.</param>
            /// <param name="maxBytesToBeRead">The maximum number of bytes to be read from the stream before reporting an error.</param>
            internal MessageStreamWrappingStream(Stream innerStream, bool ignoreDispose, long maxBytesToBeRead)
            {
                Debug.Assert(innerStream != null, "innerStream != null");

                this.innerStream = innerStream;
                this.ignoreDispose = ignoreDispose;
                this.maxBytesToBeRead = maxBytesToBeRead;
            }

            /// <summary>
            /// Determines if the stream can read.
            /// </summary>
            public override bool CanRead
            {
                get { return this.innerStream.CanRead; }
            }

            /// <summary>
            /// Determines if the stream can seek.
            /// </summary>
            public override bool CanSeek
            {
                get { return this.innerStream.CanSeek; }
            }

            /// <summary>
            /// Determines if the stream can write.
            /// </summary>
            public override bool CanWrite
            {
                get { return this.innerStream.CanWrite; }
            }

            /// <summary>
            /// Returns the length of the stream.
            /// </summary>
            public override long Length
            {
                get { return this.innerStream.Length; }
            }

            /// <summary>
            /// Gets or sets the position in the stream.
            /// </summary>
            public override long Position
            {
                get { return this.innerStream.Position; }
                set { this.innerStream.Position = value; }
            }

            /// <summary>true if the wrapping stream ignores calls to Dispose; otherwise false.</summary>
            internal bool IgnoreDispose
            {
                get
                {
                    return this.ignoreDispose;
                }
            }

            /// <summary>
            /// Flush the stream to the underlying storage.
            /// </summary>
            public override void Flush()
            {
                this.innerStream.Flush();
            }

            /// <inheritdoc />
            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return this.innerStream.FlushAsync(cancellationToken);
            }

            /// <summary>
            /// Reads data from the stream.
            /// </summary>
            /// <param name="buffer">The buffer to read the data to.</param>
            /// <param name="offset">The offset in the buffer to write to.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <returns>The number of bytes actually read.</returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                int bytesRead = this.innerStream.Read(buffer, offset, count);
                this.IncreaseTotalBytesRead(bytesRead);
                return bytesRead;
            }

            /// <inheritdoc />
            public async override Task<int> ReadAsync(
                byte[] buffer,
                int offset,
                int count,
                CancellationToken cancellationToken)
            {
                int bytesRead = await innerStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
                this.IncreaseTotalBytesRead(bytesRead);
                return bytesRead;
            }

            /// <summary>
            /// Seeks the stream.
            /// </summary>
            /// <param name="offset">The offset to seek to.</param>
            /// <param name="origin">The origin of the seek operation.</param>
            /// <returns>The new position in the stream.</returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.innerStream.Seek(offset, origin);
            }

            /// <summary>
            /// Sets the length of the stream.
            /// </summary>
            /// <param name="value">The length in bytes to set.</param>
            public override void SetLength(long value)
            {
                this.innerStream.SetLength(value);
            }

            /// <summary>
            /// Writes to the stream.
            /// </summary>
            /// <param name="buffer">The buffer to get data from.</param>
            /// <param name="offset">The offset in the buffer to start from.</param>
            /// <param name="count">The number of bytes to write.</param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                this.innerStream.Write(buffer, offset, count);
            }

            /// <inheritdoc />
            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return this.innerStream.WriteAsync(buffer, offset, count, cancellationToken);
            }

            /// <summary>
            /// Dispose this wrapping stream and the underlying stream.
            /// </summary>
            /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (!this.ignoreDispose && this.innerStream != null)
                    {
                        this.innerStream.Dispose();
                        this.innerStream = null;
                    }
                }

                base.Dispose(disposing);
            }

            /// <summary>
            /// Increases the number of total bytes read from the stream.
            /// </summary>
            /// <param name="bytesRead">The number of bytes read from the stream during the last read operation.</param>
            /// <remarks>Since we don't own the underlying stream we also have to prepare for streams returning &lt; 0 bytes read.</remarks>
            private void IncreaseTotalBytesRead(int bytesRead)
            {
                // Only count the bytes if we have a maximum specified.
                if (this.maxBytesToBeRead <= 0)
                {
                    return;
                }

                this.totalBytesRead += bytesRead < 0 ? 0 : bytesRead;
                if (this.totalBytesRead > this.maxBytesToBeRead)
                {
                    throw new ODataException(Strings.MessageStreamWrappingStream_ByteLimitExceeded(this.totalBytesRead, this.maxBytesToBeRead));
                }
            }
        }
    }
}
