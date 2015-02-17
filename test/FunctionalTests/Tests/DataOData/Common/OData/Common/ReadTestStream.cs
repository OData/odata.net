//---------------------------------------------------------------------
// <copyright file="ReadTestStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;

    #endregion Namespaces

    /// <summary>
    /// Implements a Stream which returns different sizes from its Read method - synchronous only, supports reading only.
    /// </summary>
    public class ReadTestStream : Stream
    {
        /// <summary>
        /// The inner stream this stream is wrapping
        /// </summary>
        public Stream InnerStream { get; set; }

        /// <summary>
        /// Enumerator which gets invoked before every Read call to possibly change the read size.
        /// The current value is set as the read value value.
        /// </summary>
        public IEnumerator<int> ReadSizesEnumerator { get; set; }

        /// <summary>
        /// Set to true if Dispose was called on the class.
        /// </summary>
        public bool Disposed { get; set; }

        /// <summary>
        /// Constructor - creates a new inner stream
        /// </summary>
        public ReadTestStream()
            : this(new MemoryStream())
        {
        }

        /// <summary>
        /// Constructor - takes inner stream as parameter.
        /// </summary>
        /// <param name="innerStream">The stream to wrap.</param>
        public ReadTestStream(Stream innerStream)
        {
            this.InnerStream = innerStream;
        }

        /// <summary>
        /// Determines if the stream can read - this one can
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Determines if the stream can seek - this one can't
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if the stream can write - this one can't
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Flush the stream to the underlying storage.
        /// </summary>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the length of the stream, which implementation doesn't support this.
        /// </summary>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the position in the stream, this stream doesn't support seeking, so possition is also unsupported.
        /// </summary>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
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
            int readCount = Int32.MaxValue;
            if (this.ReadSizesEnumerator != null)
            {
                readCount = this.ReadSizesEnumerator.MoveNext() ? this.ReadSizesEnumerator.Current : Int32.MaxValue;
            }

            if (readCount > count)
            {
                readCount = count;
            }
            
            return this.InnerStream.Read(buffer, offset, readCount);
        }

        /// <summary>
        /// Seeks the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The origin of the seek operation.</param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes to the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">If called from Dispose or finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            this.Disposed = true;
        }
    }
}
