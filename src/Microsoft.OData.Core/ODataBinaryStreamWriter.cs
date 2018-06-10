//---------------------------------------------------------------------
// <copyright file="ODataBinaryStreamWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif

    // todo (mikep): move this to it's own file...
    internal sealed class ODataNotificationStream : Stream
    {
        private readonly Stream stream;
        private IODataBatchOperationListener listener;

        internal ODataNotificationStream(Stream underlyingStream, IODataBatchOperationListener listener)
        {
            this.stream = underlyingStream;
            this.listener = listener;
        }

        public override bool CanRead
        {
            get
            {
                return this.stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this.stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.stream.Position;
            }

            set
            {
                this.stream.Position = value;
            }
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
        }

        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.listener != null)
                {
                    // Tell the listener that the stream is being disposed.
                    // todo (mikep): okay that the writer is going to do work here, including (a?)synchronously writing to the underlying stream?
                    this.listener.BatchOperationContentStreamDisposed();
                    this.listener = null;
                }
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// A stream for writing stream values.
    /// </summary>
    internal abstract class ODataStreamWriter : Stream
    {
        /// <summary>The writer to write to the underlying stream.</summary>
        protected readonly TextWriter Writer;

        public ODataStreamWriter(TextWriter writer)
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
    }

    /// <summary>
    /// A stream for writing base64 URL encoded binary values.
    /// </summary>
    internal sealed class ODataBinaryStreamWriter : ODataStreamWriter
    {
        /// <summary>
        /// Whether the stream should be urlEncoded upon writing.
        /// Binary properties are not urlEncoded, stream properties are.
        /// </summary>
        private readonly bool urlEncode;

        /// <summary>Trailing bytes from a previous write to be prepended to the next write.</summary>
        private Byte[] trailingBytes = new Byte[0];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="writer">A Textwriter for writing to the stream.</param>
        /// <param name="urlEncode">True if content should be urlEncoded.</param>
        public ODataBinaryStreamWriter(TextWriter writer, bool urlEncode) : base(writer)
        {
            Debug.Assert(writer != null, "writer cannot be null");
            this.urlEncode = urlEncode;
        }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
                this.Writer.Write(Base64Encode(buffer, offset, count));
        }

#if PORTABLELIB
        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.Writer.WriteAsync(Base64Encode(buffer, offset, count));
        }
#endif

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called form the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            // write any trailing bytes to stream
            if (disposing && this.trailingBytes != null && this.trailingBytes.Length > 0)
            {
                this.Writer.Write(UrlEncode(Convert.ToBase64String(trailingBytes, 0, trailingBytes.Length)));
                trailingBytes = null;
            }

            this.Writer.Flush();
            base.Dispose(disposing);
        }

        private string Base64Encode(byte[] bytes, int offset, int length)
        {
            string prefixByteString = String.Empty;
            int trailingByteLength = trailingBytes.Length;
            int numberOfBytesToPrefix = trailingByteLength > 0 ? 3 - trailingByteLength : 0;

            // if we have less than 3 bytes, store the bytes and continue
            if (length + trailingByteLength < 3)
            {
                trailingBytes = trailingBytes.Concat(bytes.Skip(offset).Take(length)).ToArray();
                return String.Empty;
            }

            // if we have bytes left over from the previous write, prepend them
            if (trailingByteLength > 0)
            {
                // convert the trailing bytes plus the first 3-trailingByteLength bytes of the new byte[]
                prefixByteString = Convert.ToBase64String(trailingBytes.Concat(bytes.Skip(offset).Take(numberOfBytesToPrefix)).ToArray(), 0, 3);
            }

            // compute if there will be trailing bytes from this write
            int remainingBytes = (length - numberOfBytesToPrefix) % 3;
            trailingBytes = bytes.Skip(offset + length - remainingBytes).Take(remainingBytes).ToArray();

            string base64String = prefixByteString + Convert.ToBase64String(bytes, offset + numberOfBytesToPrefix, length - numberOfBytesToPrefix - remainingBytes);
            return this.urlEncode ? UrlEncode(base64String) : base64String;
        }

        private static string UrlEncode(string unencodedString)
        {
            // todo (mikep): official method for UrlEncoding the string?
            return unencodedString.Replace('/', '_').Replace('+', '-');
        }
    }

//    /// <summary>
//    /// A stream for writing text values.
//    /// </summary>
//    internal sealed class ODataTextStreamWriter : ODataStreamWriter
//    {
//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="writer">A Textwriter for writing to the stream.</param>
//        public ODataTextStreamWriter(TextWriter writer) : base(writer)
//        {
//            Debug.Assert(writer != null, "writer cannot be null");
//        }

//        /// <summary>
//        /// Writes to the stream.
//        /// </summary>
//        /// <param name="buffer">The buffer to get data from.</param>
//        /// <param name="offset">The offset in the buffer to start from.</param>
//        /// <param name="count">The number of bytes to write.</param>
//        public override void Write(byte[] buffer, int offset, int count)
//        {
//            this.Writer.Write(this.Writer.Encoding.GetChars(buffer, offset, count));
//        }

//#if PORTABLELIB
//        /// <inheritdoc />
//        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
//        {
//            // todo (mikep) what about cancellationToken?
//            return this.Writer.WriteAsync(this.Writer.Encoding.GetChars(buffer, offset, count));
//        }
//#endif
//    }
}
