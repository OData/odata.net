//---------------------------------------------------------------------
// <copyright file="ODataNotificationWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Wrapper for TextWriter to listen for dispose.
    /// </summary>
#if NETSTANDARD2_0
    internal sealed class ODataNotificationWriter : TextWriter, IAsyncDisposable
#else
    internal sealed class ODataNotificationWriter : TextWriter
#endif
    {
        private TextWriter textWriter;
        private IODataStreamListener listener;
        private bool disposed = false;
        private readonly bool synchronous;

        internal ODataNotificationWriter(TextWriter textWriter, IODataStreamListener listener, bool synchronous = true)
            : base(System.Globalization.CultureInfo.InvariantCulture)
        {
            Debug.Assert(textWriter != null, "Creating a notification writer for a null textWriter.");
            Debug.Assert(listener != null, "Creating a notification writer with a null listener.");

            this.textWriter = textWriter;
            this.listener = listener;
            this.synchronous = synchronous;
        }

        /// <inheritdoc/>
        public override Encoding Encoding
        {
            get
            {
                return this.textWriter.Encoding;
            }
        }

        /// <inheritdoc/>
        public override string NewLine
        {
            get
            {
                return this.textWriter.NewLine;
            }

            set
            {
                this.textWriter.NewLine = value;
            }
        }

        /// <inheritdoc/>
        public override IFormatProvider FormatProvider
        {
            get
            {
                return this.textWriter.FormatProvider;
            }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            this.textWriter.Flush();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.textWriter.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.textWriter.ToString();
        }

        #region Write methods

        /// <inheritdoc/>
        public override void Write(char value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(bool value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(string value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(char[] buffer)
        {
            this.textWriter.Write(buffer);
        }

        /// <inheritdoc/>
        public override void Write(char[] buffer, int index, int count)
        {
            this.textWriter.Write(buffer, index, count);
        }

        /// <inheritdoc/>
        public override void Write(string format, params object[] arg)
        {
            this.textWriter.Write(format, arg);
        }

        /// <inheritdoc/>
        public override void Write(decimal value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(object value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(double value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(float value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(int value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(long value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(uint value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(ulong value)
        {
            this.textWriter.Write(value);
        }

        #endregion

        #region WriteLine methods

        /// <inheritdoc/>
        public override void WriteLine()
        {
            this.textWriter.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(bool value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(char value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(char[] buffer)
        {
            this.textWriter.WriteLine(buffer);
        }

        /// <inheritdoc/>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            this.textWriter.WriteLine(buffer, index, count);
        }

        /// <inheritdoc/>
        public override void WriteLine(decimal value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(double value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(float value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(int value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(long value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(object value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(string format, params object[] arg)
        {
            this.textWriter.WriteLine(format, arg);
        }

        /// <inheritdoc/>
        public override void WriteLine(string value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(uint value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(ulong value)
        {
            this.textWriter.WriteLine(value);
        }

        #endregion

        #region async methods

        /// <inheritdoc/>
        public override Task FlushAsync()
        {
            return this.textWriter.FlushAsync();
        }

        /// <inheritdoc/>
        public override Task WriteAsync(char value)
        {
            return this.textWriter.WriteAsync(value);
        }

        /// <inheritdoc/>
        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return this.textWriter.WriteAsync(buffer, index, count);
        }

        /// <inheritdoc/>
        public override Task WriteAsync(string value)
        {
            return this.textWriter.WriteAsync(value);
        }

        /// <inheritdoc/>
        public override Task WriteLineAsync()
        {
            return this.textWriter.WriteLineAsync();
        }

        /// <inheritdoc/>
        public override Task WriteLineAsync(char value)
        {
            return this.textWriter.WriteLineAsync(value);
        }

        /// <inheritdoc/>
        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return this.textWriter.WriteLineAsync(buffer, index, count);
        }

        /// <inheritdoc/>
        public override Task WriteLineAsync(string value)
        {
            return this.textWriter.WriteLineAsync(value);
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
                // NOTE: Do not dispose the text writer since this instance does not own it.
                this.textWriter = null;
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
                // NOTE: Do not dispose the text writer since this instance does not own it.
                this.textWriter = null;
            }

            // Dispose unmanaged resources
            // Pass `false` to ensure functional equivalence with the synchronous dispose pattern
            this.Dispose(false);
        }
#endif
    }
}
