//---------------------------------------------------------------------
// <copyright file="ODataNotificationStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Wrapper to listen for dispose on a <see cref="Stream"/>.
    /// </summary>
#if NETSTANDARD2_0
    internal sealed class ODataNotificationStream : Stream, IAsyncDisposable
#else
    internal sealed class ODataNotificationStream : Stream
#endif
    {
        private Stream stream;
        private IODataStreamListener listener;
        private bool disposed = false;
        private bool synchronous;

        internal ODataNotificationStream(Stream underlyingStream, IODataStreamListener listener, bool synchronous = true)
        {
            Debug.Assert(underlyingStream != null, "Creating a notification stream for a null stream.");
            Debug.Assert(listener != null, "Creating a notification stream with a null listener.");

            this.stream = underlyingStream;
            this.listener = listener;
            this.synchronous = synchronous;
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

#endregion

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                // Tell the listener that the stream is being disposed.
                if (synchronous)
                {
                    this.listener?.StreamDisposed();
                }
                else
                {
                    this.listener?.StreamDisposedAsync().Wait();
                }

                this.listener = null;
                // NOTE: Do not dispose the stream since this instance does not own it.
                this.stream = null;
            }

            this.disposed = true;
            base.Dispose(disposing);
        }

#if NETSTANDARD2_0
        public async ValueTask DisposeAsync()
        {
            if (!this.disposed && this.listener != null)
            {
                await this.listener.StreamDisposedAsync()
                    .ConfigureAwait(false);

                this.listener = null;
                // NOTE: Do not dispose the stream since this instance does not own it.
                this.stream = null;
            }

            // Dispose unmanaged resources
            // Pass `false` to ensure functional equivalence with the synchronous dispose pattern
            this.Dispose(false);
        }
#endif
    }
}
