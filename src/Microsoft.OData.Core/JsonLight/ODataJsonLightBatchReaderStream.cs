//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchReaderStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core.Json;

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    #endregion Namespaces

    internal sealed class ODataJsonLightBatchReaderStream: ODataBatchReaderStream
    {

        private readonly ODataJsonLightInputContext inputContext;

        internal ODataJsonLightBatchReaderStream(
            ODataJsonLightInputContext inputContext,
            Encoding batchEncoding)
            : base(batchEncoding)
        {
            Debug.Assert(inputContext != null, "inputContext != null");
            this.inputContext = inputContext;
        }

        internal BufferingJsonReader JsonReader
        {
            get
            {
                return this.inputContext.JsonReader;
            }
        }

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
            Debug.Assert(this.batchEncoding != null, "Batch encoding should have been established on first call to SkipToBoundary.");

            //// NOTE: if we have a stream with length we don't even check for boundaries but rely solely on the content length

            int remainingNumberOfBytesToRead = count;
            while (remainingNumberOfBytesToRead > 0)
            {
                // check whether we can satisfy the full read request from the buffer
                // or whether we have to split the request and read more data into the buffer.
                if (this.batchBuffer.NumberOfBytesInBuffer >= remainingNumberOfBytesToRead)
                {
                    // we can satisfy the full read request from the buffer
                    Buffer.BlockCopy(this.batchBuffer.Bytes, this.batchBuffer.CurrentReadPosition, userBuffer, userBufferOffset, remainingNumberOfBytesToRead);
                    this.batchBuffer.SkipTo(this.batchBuffer.CurrentReadPosition + remainingNumberOfBytesToRead);
                    remainingNumberOfBytesToRead = 0;
                }
                else
                {
                    // we can only partially satisfy the read request
                    int availableBytesToRead = this.batchBuffer.NumberOfBytesInBuffer;
                    Buffer.BlockCopy(this.batchBuffer.Bytes, this.batchBuffer.CurrentReadPosition, userBuffer, userBufferOffset, availableBytesToRead);
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
                        this.underlyingStreamExhausted = this.batchBuffer.RefillFrom(this.inputContext.Stream, /*preserveFrom*/ ODataBatchReaderStreamBuffer.BufferLength);
                    }
                }
            }

            // return the number of bytes that were read
            return count - remainingNumberOfBytesToRead;
        }

    }
}
