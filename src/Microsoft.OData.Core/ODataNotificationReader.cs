//---------------------------------------------------------------------
// <copyright file="ODataNotificationReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Wrapper for TextReader to listen for dispose.
    /// </summary>
    internal sealed class ODataNotificationReader : TextReader, IAsyncDisposable
    {
        private readonly TextReader textReader;
        private readonly IODataStreamListener listener;
        private readonly bool synchronous;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataNotificationReader"/> class.
        /// </summary>
        /// <param name="textReader">The wrapped text reader.</param>
        /// <param name="listener">Listener to notify when the text reader is being disposed.</param>
        /// <param name="synchronous">true if execution context is synchronous; otherwise false.</param>
        /// <remarks>
        /// When an instance of this class is disposed, it in turn disposes the wrapped text reader.
        /// </remarks>
        internal ODataNotificationReader(TextReader textReader, IODataStreamListener listener, bool synchronous = true)
        {
            Debug.Assert(textReader != null, "Creating a notification reader for a null textReader.");
            Debug.Assert(listener != null, "Creating a notification reader with a null listener.");

            this.textReader = textReader;
            this.listener = listener;
            this.synchronous = synchronous;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.textReader.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.textReader.ToString();
        }

        /// <inheritdoc/>
        public override int Peek()
        {
            return this.textReader.Peek();
        }

        #region Read methods

        /// <inheritdoc/>
        public override int Read()
        {
            return this.textReader.Read();
        }

        /// <inheritdoc/>
        public override int Read(char[] buffer, int index, int count)
        {
            return this.textReader.Read(buffer, index, count);
        }

        /// <inheritdoc/>
        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return this.textReader.ReadBlock(buffer, index, count);
        }

        /// <inheritdoc/>
        public override string ReadLine()
        {
            return this.textReader.ReadLine();
        }

        /// <inheritdoc/>
        public override string ReadToEnd()
        {
            return this.textReader.ReadToEnd();
        }

        #endregion

        #region asyncMethods

        /// <inheritdoc/>
        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            return this.textReader.ReadAsync(buffer, index, count);
        }

        /// <inheritdoc/>
        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        {
            return this.ReadBlockAsync(buffer, index, count);
        }

        /// <inheritdoc/>
        public override Task<string> ReadLineAsync()
        {
            return this.textReader.ReadLineAsync();
        }

        /// <inheritdoc/>
        public override Task<string> ReadToEndAsync()
        {
            return this.textReader.ReadToEndAsync();
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
                    this.listener.StreamDisposed();
                }
                else
                {
                    this.listener.StreamDisposedAsync().Wait();
                }
            }

            this.disposed = true;
            this.textReader?.Dispose();
            base.Dispose(disposing);
        }

        public async ValueTask DisposeAsync()
        {
            if (!this.disposed)
            {
                await this.listener.StreamDisposedAsync()
                    .ConfigureAwait(false);
            }

            // Dispose unmanaged resources
            // Pass `false` to ensure functional equivalence with the synchronous dispose pattern
            this.Dispose(false);
        }
    }
}