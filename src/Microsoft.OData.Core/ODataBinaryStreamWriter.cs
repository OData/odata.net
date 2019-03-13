//---------------------------------------------------------------------
// <copyright file="ODataBinaryStreamWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// A stream for writing base64 encoded binary values.
    /// </summary>
    internal sealed class ODataBinaryStreamWriter : Stream
    {
        /// <summary>The writer to write to the underlying stream.</summary>
        private readonly TextWriter Writer;

        /// <summary>Trailing bytes from a previous write to be prepended to the next write.</summary>
        private Byte[] trailingBytes = new Byte[0];

        // mikep todo: get this from user
        private char[] buffer = new char[2048];

        /// <summary> An empty byte[].</summary>
        private static byte[] emptyByteArray = new byte[0];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="writer">A Textwriter for writing to the stream.</param>
        public ODataBinaryStreamWriter(TextWriter writer)
        {
            this.Writer = writer;
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
            byte[] prefixByteString = emptyByteArray;
            int trailingByteLength = trailingBytes.Length;
            int numberOfBytesToPrefix = trailingByteLength > 0 ? 3 - trailingByteLength : 0;

            // if we have less than 3 bytes, store the bytes and continue
            if (count + trailingByteLength < 3)
            {
                trailingBytes = trailingBytes.Concat(bytes.Skip(offset).Take(count)).ToArray();
                return;
            }

            // if we have bytes left over from the previous write, prepend them
            if (trailingByteLength > 0)
            {
                // convert the trailing bytes plus the first 3-trailingByteLength bytes of the new byte[]
                prefixByteString = trailingBytes.Concat(bytes.Skip(offset).Take(numberOfBytesToPrefix)).ToArray();
            }

            // compute if there will be trailing bytes from this write
            int remainingBytes = (count - numberOfBytesToPrefix) % 3;
            trailingBytes = bytes.Skip(offset + count - remainingBytes).Take(remainingBytes).ToArray();

            WriteBinaryString(this.Writer, prefixByteString.Concat(bytes.Skip(offset + numberOfBytesToPrefix).Take(count - numberOfBytesToPrefix - remainingBytes)).ToArray(), buffer);
        }

        #if PORTABLELIB
        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() =>
                this.Write(buffer, offset, count));
        }
#endif

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
            Writer.Flush();
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
        /// Write a byte array.
        /// </summary>
        /// <param name="writer">TextWriter to write the byte[]</param>
        /// <param name="value">Byte array to be written.</param>
        /// <param name="buffer">Temporary buffer to hold the converted chars</param>
        private static void WriteBinaryString(TextWriter writer, byte[] value, char[] buffer)
        {
            int bufferLength = buffer.Length;

            // Try to hold base64 string as much as possible in one converting.
            int bufferByteSize = bufferLength * 3 / 4;

            for (int offsetIn = 0; offsetIn < value.Length; offsetIn += bufferByteSize)
            {
                int length = bufferByteSize;
                if (offsetIn + length > value.Length)
                {
                    length = value.Length - offsetIn;
                }

                int output = Convert.ToBase64CharArray(value, offsetIn, length, buffer, 0);
                writer.Write(buffer, 0, output);
            }
        }
    }
}
