//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchReaderStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class used by the <see cref="ODataJsonLightBatchReader"/> to read the various pieces of a batch payload
    /// in application/json format.
    /// </summary>
    internal sealed class ODataJsonLightBatchReaderStream : ODataBatchReaderStream
    {
        /// <summary>
        /// The input context used by the JsonLight reader.
        /// </summary>
        private readonly ODataJsonLightInputContext inputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The JsonLight input context.</param>
        internal ODataJsonLightBatchReaderStream(ODataJsonLightInputContext inputContext)
        {
            Debug.Assert(inputContext != null, "inputContext != null");
            this.inputContext = inputContext;
        }

        /// <summary>
        /// The reader providing access to payload in Json format.
        /// </summary>
        internal BufferingJsonReader JsonReader
        {
            get
            {
                return this.inputContext.JsonReader;
            }
        }

        /// <summary>
        /// This method is not applicable for application/json format, and throws an exception.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        internal override int ReadWithDelimiter(byte[] userBuffer, int userBufferOffset, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads from the batch stream without checking for a boundary delimiter since we
        /// know the length of the stream.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        internal override int ReadWithLength(byte[] userBuffer, int userBufferOffset, int count)
        {
            Debug.Assert(userBuffer != null, "userBuffer != null");
            Debug.Assert(userBufferOffset >= 0, "userBufferOffset >= 0");
            Debug.Assert(count >= 0, "count >= 0");

            //// NOTE: if we have a stream with length we don't even check for boundaries but rely solely on the content length
            int remainingNumberOfBytesToRead = count;
            while (remainingNumberOfBytesToRead > 0)
            {
                // check whether we can satisfy the full read request from the buffer
                // or whether we have to split the request and read more data into the buffer.
                if (this.BatchBuffer.NumberOfBytesInBuffer >= remainingNumberOfBytesToRead)
                {
                    // we can satisfy the full read request from the buffer
                    Buffer.BlockCopy(this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, userBuffer, userBufferOffset, remainingNumberOfBytesToRead);
                    this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + remainingNumberOfBytesToRead);
                    remainingNumberOfBytesToRead = 0;
                }
                else
                {
                    // we can only partially satisfy the read request
                    int availableBytesToRead = this.BatchBuffer.NumberOfBytesInBuffer;
                    Buffer.BlockCopy(this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, userBuffer, userBufferOffset, availableBytesToRead);
                    remainingNumberOfBytesToRead -= availableBytesToRead;
                    userBufferOffset += availableBytesToRead;

                    // we exhausted the buffer; if the underlying stream is not exhausted, refill the buffer
                    if (this.underlyingStreamExhausted)
                    {
                        // We cannot fully satisfy the read request since there are not enough bytes in the stream.
                        // This means that the content length of the stream was incorrect; this should never happen
                        // since the caller should already have checked this.
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReaderStreamBuffer_ReadWithLength));
                    }
                    else
                    {
                        this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.inputContext.Stream, /*preserveFrom*/ ODataBatchReaderStreamBuffer.BufferLength);
                    }
                }
            }

            // return the number of bytes that were read
            return count - remainingNumberOfBytesToRead;
        }
    }
}