//---------------------------------------------------------------------
// <copyright file="ODataNotificationStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.IO;
    using System.Threading;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// Wrapper to listen for dispose on a stream
    /// </summary>
    internal sealed class ODataNotificationStream : Stream
    {
        private readonly Stream stream;
        private IODataStreamListener listener;

        internal ODataNotificationStream(Stream underlyingStream, IODataStreamListener listener)
        {
            this.stream = underlyingStream;
            this.listener = listener;
        }

        /// <inheritdoc/>
        public override bool CanRead
        {
            get
            {
                return this.stream.CanRead;
            }
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get
            {
                return this.stream.CanSeek;
            }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get
            {
                return this.stream.CanWrite;
            }
        }

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                return this.stream.Length;
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override bool CanTimeout
        {
            get
            {
                return this.stream.CanTimeout;
            }
        }

        /// <inheritdoc/>
        public override int ReadTimeout
        {
            get
            {
                return this.stream.ReadTimeout;
            }

            set
            {
                this.stream.ReadTimeout = value;
            }
        }

        /// <inheritdoc/>
        public override int WriteTimeout
        {
            get
            {
                return this.stream.WriteTimeout;
            }

            set
            {
                this.stream.WriteTimeout = value;
            }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            this.stream.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.stream.Seek(offset, origin);
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            return this.stream.ReadByte();
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            this.stream.WriteByte(value);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.stream.ToString();
        }

        #region async methods

#if PORTABLELIB

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return this.stream.FlushAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return this.stream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.stream.WriteAsync(buffer, offset, count, cancellationToken);
        }
#endif
        #endregion

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
                    this.listener.StreamDisposed();
                    this.listener = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
