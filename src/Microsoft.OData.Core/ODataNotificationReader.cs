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
#if NETSTANDARD2_0
    internal sealed class ODataNotificationReader : TextReader, IAsyncDisposable
#else
    internal sealed class ODataNotificationReader : TextReader
#endif
    {
        private TextReader textReader;
        private IODataStreamListener listener;
        private bool disposed = false;
        private readonly bool synchronous;

        internal ODataNotificationReader(TextReader textReader, IODataStreamListener listener, bool synchronous = true)
        {
            Debug.Assert(textReader != null, "Creating a notification reader for a null textReader.");
            Debug.Assert(listener != null, "Creating a notification reader with a null textReader.");

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
                    this.listener?.StreamDisposed();
                }
                else
                {
                    this.listener?.StreamDisposedAsync().Wait();
                }

                this.listener = null;
                // NOTE: Do not dispose the text reader since this instance does not own it.
                this.textReader = null;
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
                // NOTE: Do not dispose the text reader since this instance does not own it.
                this.textReader = null;
            }

            // Dispose unmanaged resources
            // Pass `false` to ensure functional equivalence with the synchronous dispose pattern
            this.Dispose(false);
        }
#endif
    }
}