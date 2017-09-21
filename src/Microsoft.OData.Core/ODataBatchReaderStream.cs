//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Class used by the <see cref="ODataBatchReader"/> to read the various pieces of a batch payload.
    /// </summary>
    /// <remarks>
    /// This is the base class for format-specific derived classes to process batch requests in different formats,
    /// such as multipart/mixed format and application/json format.
    /// </remarks>
    internal abstract class ODataBatchReaderStream
    {
        /// <summary>The buffer used by the batch reader stream to scan for boundary strings.</summary>
        protected readonly ODataBatchReaderStreamBuffer BatchBuffer;

        /// <summary>The encoding to use to read from the batch stream.</summary>
        protected Encoding batchEncoding;

        /// <summary>The encoding for a given changeset.</summary>
        protected Encoding changesetEncoding;

        /// <summary>
        /// true if the underlying stream was exhausted during a read operation; we won't try to read from the
        /// underlying stream again once it was exhausted.
        /// </summary>
        protected bool underlyingStreamExhausted;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchEncoding">The encoding to use to read from the batch stream.</param>
        internal ODataBatchReaderStream(
            Encoding batchEncoding)
        {
            this.batchEncoding = batchEncoding;

            this.BatchBuffer = new ODataBatchReaderStreamBuffer();
        }

        /// <summary>
        /// The current encoding to use when reading from the stream.
        /// </summary>
        /// <remarks>This is the changeset encoding when reading a changeset or the batch encoding otherwise.</remarks>
        protected Encoding CurrentEncoding
        {
            get
            {
                return this.changesetEncoding ?? this.batchEncoding;
            }
        }

        /// <summary>
        /// Reads from the batch stream while ensuring that we stop reading at each boundary.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        internal abstract int ReadWithDelimiter(byte[] userBuffer, int userBufferOffset, int count);


        /// <summary>
        /// Reads from the batch stream without checking for a boundary delimiter since we
        /// know the length of the stream.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        internal abstract int ReadWithLength(byte[] userBuffer, int userBufferOffset, int count);

        /// <summary>
        /// Dispose internal resource.
        /// </summary>
        /// <remarks>
        /// Default implementation at base level is no-op.
        /// Derived types provide specific implementation as needed.
        /// Note that, instead of defining IDisposable on the root level and causing ripples to handle IDisposable
        /// in other related types, we use this out-of-band method and its overrides for disposing.
        /// </remarks>
        protected internal virtual void DisposeResources()
        {
        }

        /// <summary>
        /// Ensure that a batch encoding exists; if not, detect it from the first couple of bytes of the stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        protected void EnsureBatchEncoding(Stream stream)
        {
            // If no batch encoding is specified we detect it from the first bytes in the buffer.
            if (this.batchEncoding == null)
            {
                // NOTE: The batch encoding will only ever be null on the first call to this method which
                //       happens before the batch reader skips to the first boundary.
                this.batchEncoding = this.DetectEncoding(stream);
            }

            // Verify that we only allow single byte encodings and UTF-8 for now.
            ReaderValidationUtils.ValidateEncodingSupportedInBatch(this.batchEncoding);
        }

        /// <summary>Detect the encoding based data from the stream.</summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The encoding discovered from the bytes in the buffer or the fallback encoding.</returns>
        /// <remarks>
        /// We don't have to skip a potential preamble of the encoding since the batch reader
        /// will skip over everything (incl. the potential preamble) until it finds the first
        /// boundary.
        /// </remarks>
        protected Encoding DetectEncoding(Stream stream)
        {
            // We need at most 4 bytes in the buffer to determine the encoding; if we have less than that,
            // refill the buffer.
            while (!this.underlyingStreamExhausted && this.BatchBuffer.NumberOfBytesInBuffer < 4)
            {
                this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(stream, this.BatchBuffer.CurrentReadPosition);
            }

            // Now we should have a full buffer unless the underlying stream did not have enough bytes.
            int numberOfBytesInBuffer = this.BatchBuffer.NumberOfBytesInBuffer;
            if (numberOfBytesInBuffer < 2)
            {
                Debug.Assert(this.underlyingStreamExhausted, "Underlying stream must be exhausted if we have less than 2 bytes in the buffer after refilling.");

                // If we cannot read any of the known preambles we fall back to the default encoding, which is US-ASCII.
#if !ORCAS
                // ASCII not available; use UTF8 without preamble
                return MediaTypeUtils.FallbackEncoding;
#else
                return Encoding.ASCII;
#endif
            }
            else if (this.BatchBuffer[this.BatchBuffer.CurrentReadPosition] == 0xFE && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 1] == 0xFF)
            {
                // Big Endian Unicode
                return new UnicodeEncoding(/*bigEndian*/ true, /*byteOrderMark*/ true);
            }
            else if (this.BatchBuffer[this.BatchBuffer.CurrentReadPosition] == 0xFF && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 1] == 0xFE)
            {
                // Little Endian Unicode, or possibly little endian UTF32
                if (numberOfBytesInBuffer >= 4 &&
                    this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 2] == 0 &&
                    this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 3] == 0)
                {
#if !ORCAS
                    // Little Endian UTF32 not available
                    throw Error.NotSupported();
#else
                    return new UTF32Encoding(/*bigEndian*/ false, /*byteOrderMark*/ true);
#endif
                }
                else
                {
                    return new UnicodeEncoding(/*bigEndian*/ false, /*byteOrderMark*/ true);
                }
            }
            else if (numberOfBytesInBuffer >= 3 &&
                     this.BatchBuffer[this.BatchBuffer.CurrentReadPosition] == 0xEF &&
                     this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 1] == 0xBB &&
                     this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 2] == 0xBF)
            {
                // UTF-8
                return Encoding.UTF8;
            }
            else if (numberOfBytesInBuffer >= 4 &&
                     this.BatchBuffer[this.BatchBuffer.CurrentReadPosition] == 0 &&
                     this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 1] == 0 &&
                     this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 2] == 0xFE &&
                     this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 3] == 0xFF)
            {
                // Big Endian UTF32
#if !ORCAS
                // Big Endian UTF32 not available
                throw Error.NotSupported();
#else
                return new UTF32Encoding(/*bigEndian*/ true, /*byteOrderMark*/ true);
#endif
            }
            else
            {
#if !ORCAS
                // ASCII not available; use UTF8 without preamble
                return MediaTypeUtils.FallbackEncoding;
#else
                return Encoding.ASCII;
#endif
            }
        }
    }
}
