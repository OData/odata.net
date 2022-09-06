//---------------------------------------------------------------------
// <copyright file="ReadWriteOnlyStreamProxy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Stream proxy that only implements simple read/write APIs
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Class wraps a stream, so it should end with 'proxy'")]
    [DebuggerDisplay("ReadWriteOnlyStreamProxy({this.Underlying.ToString()})")]
    public class ReadWriteOnlyStreamProxy : Stream
    {
        internal const string NotSupportedMessage = "Only simple APIs like read, write, flush, and close are supported";

        private bool writeable;

        /// <summary>
        /// Initializes a new instance of the ReadWriteOnlyStreamProxy class
        /// </summary>
        /// <param name="underlying">The underlying stream to contain</param>
        /// <param name="writable">A value indicating whether the stream can be written to</param>
        public ReadWriteOnlyStreamProxy(Stream underlying, bool writable)
        {
            ExceptionUtilities.CheckArgumentNotNull(underlying, "underlying");
            this.Underlying = underlying;
            this.writeable = writable;
        }

        /// <summary>
        /// Gets a value indicating whether the stream can be read
        /// </summary>
        public override bool CanRead
        {
            get
            {
                ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
                return this.Underlying.CanRead;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stream can be written to
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
                return this.writeable && this.Underlying.CanWrite;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stream can seek. Throws exception if called.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override bool CanSeek
        {
            get { throw new TaupoNotSupportedException(NotSupportedMessage); }
        }

        /// <summary>
        /// Gets the length of the stream. Throws exception if called.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override long Length
        {
            get { throw new TaupoNotSupportedException(NotSupportedMessage); }
        }

        /// <summary>
        /// Gets or sets the position of the stream. Throws exception if called.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override long Position
        {
            get
            {
                throw new TaupoNotSupportedException(NotSupportedMessage);
            }

            set
            {
                throw new TaupoNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stream can timeout. Throws exception if called.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override bool CanTimeout
        {
            get
            {
                throw new TaupoNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Gets or sets the read timeout. Throws exception if called.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override int ReadTimeout
        {
            get
            {
                throw new TaupoNotSupportedException(NotSupportedMessage);
            }

            set
            {
                throw new TaupoNotSupportedException(NotSupportedMessage);
            }
        }

        /// <summary>
        /// Gets or sets the write timeout. Throws exception if called.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override int WriteTimeout
        {
            get
            {
                throw new TaupoNotSupportedException(NotSupportedMessage);
            }

            set
            {
                throw new TaupoNotSupportedException(NotSupportedMessage);
            }
        }

        internal Stream Underlying { get; private set; }

        /// <summary>
        /// Flushes the underlying stream
        /// </summary>
        public override void Flush()
        {
            ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
            this.Underlying.Flush();
        }

        /// <summary>
        /// Reads from the underlying stream.
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="offset">The offset to write into the buffer</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The number of bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
            return this.Underlying.Read(buffer, offset, count);
        }

        /// <summary>
        /// Seeks to the given offset. Throws exception if called.
        /// </summary>
        /// <param name="offset">The seek offset</param>
        /// <param name="origin">The seek origin</param>
        /// <returns>The new position of the stream?</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new TaupoNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Sets the length of the steam. Throws exception if called.
        /// </summary>
        /// <param name="value">The new length</param>
        public override void SetLength(long value)
        {
            throw new TaupoNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to write from</param>
        /// <param name="offset">The offset into the buffer</param>
        /// <param name="count">The number of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ExceptionUtilities.Assert(this.writeable, "Writing to this stream is not allowed");
            ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
            this.Underlying.Write(buffer, offset, count);
        }

        /// <summary>
        /// Begins an async read from the underlying stream.
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="offset">The offset into the buffer</param>
        /// <param name="count">The number of bytes to read</param>
        /// <param name="callback">The async callback</param>
        /// <param name="state">The async state</param>
        /// <returns>The async result</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
            return this.Underlying.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Begins an async write to the underlying stream.
        /// </summary>
        /// <param name="buffer">The buffer to write from</param>
        /// <param name="offset">The offset into the buffer</param>
        /// <param name="count">The number of bytes to write</param>
        /// <param name="callback">The async callback</param>
        /// <param name="state">The async state</param>
        /// <returns>The async result</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ExceptionUtilities.Assert(this.writeable, "Writing to this stream is not allowed");
            ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
            return this.Underlying.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Closes the stream
        /// </summary>
        public override void Close()
        {
            // actual dispose logic is here to deal with Stream not implementing Dispose in a normal way
            if (this.Underlying != null)
            {
                this.Underlying.Dispose();
                this.Underlying = null;
            }
        }

        /// <summary>
        /// Reads a single byte. Throws exception if called.
        /// </summary>
        /// <returns>The byte read</returns>
        public override int ReadByte()
        {
            throw new TaupoNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Ends an async read from the underlying stream. 
        /// </summary>
        /// <param name="asyncResult">The async result from a BeginRead call</param>
        /// <returns>The number of bytes read</returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
            return this.Underlying.EndRead(asyncResult);
        }

        /// <summary>
        /// Writes a single byte. Throws exception if called.
        /// </summary>
        /// <param name="value">The byte to write</param>
        public override void WriteByte(byte value)
        {
            throw new TaupoNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Ends an async write to the underlying stream.
        /// </summary>
        /// <param name="asyncResult">The result from a call to BeginWrite</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            ExceptionUtilities.Assert(this.writeable, "Writing to this stream is not allowed");
            ExceptionUtilities.CheckObjectNotNull(this.Underlying, "Underlying stream was null");
            this.Underlying.EndWrite(asyncResult);
        }

        /// <summary>
        /// Converts the stream to a string. Throws exception if called.
        /// </summary>
        /// <returns>String representation of the stream</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override string ToString()
        {
            throw new TaupoNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Returns whether or not the current object is equal to the one given. Throws exception if called.
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>Whether the object is equal to the current object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override bool Equals(object obj)
        {
            throw new TaupoNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Gets the hash code for this object. Throws exception if called.
        /// </summary>
        /// <returns>The hash code</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should not be used by the product")]
        public override int GetHashCode()
        {
            throw new TaupoNotSupportedException(NotSupportedMessage);
        }

        /// <summary>
        /// Disposes the stream
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Should never be called")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2215:Dispose methods should call base class dispose", Justification = "Should never be called")]
        protected override void Dispose(bool disposing)
        {
            // should not be called, because it is only ever called from Close in the base Stream implementation
            throw new TaupoNotSupportedException(NotSupportedMessage);
        }
    }
}