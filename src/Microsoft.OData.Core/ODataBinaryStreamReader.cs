//---------------------------------------------------------------------
// <copyright file="ODataBinaryStreamReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Threading.Tasks;

    #endregion Namespaces

    /// <summary>
    /// A stream for reading base64 (possibly URL encoded) binary values.
    /// </summary>
    internal sealed class ODataBinaryStreamReader : Stream
    {
        /// <summary>
        /// A function to read a specified number of characters into
        /// a character array. Returns the actual number of characters read.
        /// </summary>
        private readonly Func<char[], int, int, int> reader;

        /// <summary>Size of character buffer.</summary>
        /// <remarks>
        /// In Base64 encoding, four characters represent three bytes, so character array size
        /// must be divisible by four to prevent truncating bytes
        /// </remarks>
        private readonly int charLength = 1024;

        /// <summary>Character buffer for reading base64 URL encoded string.</summary>
        private char[] chars;

        /// <summary>Current offset into buffer.</summary>
        private int bytesOffset = 0;

        /// <summary>Buffer for reading the stream content.</summary>
        private byte[] bytes = new byte[0];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">A function from which to read character values.</param>
        internal ODataBinaryStreamReader(Func<char[], int, int, int> reader)
        {
            Debug.Assert(reader != null, "reader cannot be null");

            this.reader = reader;
            this.chars = new char[this.charLength];
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
        /// Returns the length of the stream. Not supported by this stream.
        /// </summary>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the position in the stream. Not supported by this stream.
        /// </summary>
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesCopied = 0;
            int bytesRemaining = this.bytes.Length - this.bytesOffset;

            while (bytesCopied < count)
            {
                if (bytesRemaining == 0)
                {
                    int charsRead = this.reader(this.chars, offset, this.charLength);
                    if (charsRead < 1)
                    {
                        break;
                    }

                   // chars = chars.Select(c => c == '_' ? '/' : c == '-' ? '+' : c).ToArray();
                    this.bytes = Convert.FromBase64CharArray(this.chars, 0, charsRead);

                    bytesRemaining = this.bytes.Length;
                    this.bytesOffset = 0;

                    // If the remaining characters were padding characters then no bytes will be returned
                    if (bytesRemaining < 1)
                    {
                        break;
                    }
                }

                buffer[bytesCopied] = this.bytes[this.bytesOffset];
                bytesCopied++;
                this.bytesOffset++;
                bytesRemaining--;
            }

            return bytesCopied;
        }

        /// <summary>
        /// Flush the stream; not supported for a read stream.
        /// </summary>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
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
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
