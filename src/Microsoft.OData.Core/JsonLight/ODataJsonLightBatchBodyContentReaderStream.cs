//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchBodyContentReaderStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Microsoft.OData.Core.Json;

    #endregion Namespaces

    internal sealed class ODataJsonLightBatchBodyContentReaderStream : ODataBatchReaderStream, IDisposable
    {
        // Input memory stream providing body content data.
        private readonly Stream bodyContentStream = null;

        // Writer for writing Json data to memory stream.
        private readonly StreamWriter streamWriter = null;

        // Status to ensure that the underlying memory stream is ready.
        private bool isDataPopulatedToStream;

        // Type of the data in request body
        private BatchRequestBodyContentType contentType;

        private enum BatchRequestBodyContentType
        {
            // The body of the request contains Json data.
            Json,

            // The body of the request contains binary data, e.g. jpeg image raw data.
            Binary,
        }


        /// <summary>
        /// Constructor using default encoding (UTF-8 without the BOM preamble)
        /// </summary>
        /// <param name="batchEncoding">The encoding to use to read & write from the batch stream.</param>
        internal ODataJsonLightBatchBodyContentReaderStream()
            : this(MediaTypeUtils.FallbackEncoding)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchEncoding">The encoding to use to read & write from the batch stream.</param>
        internal ODataJsonLightBatchBodyContentReaderStream(
            Encoding batchEncoding)
            : base(batchEncoding)
        {
            this.bodyContentStream = new MemoryStream();

            this.streamWriter = new StreamWriter(this.bodyContentStream, batchEncoding);

            this.isDataPopulatedToStream = false;
        }

        /// <summary>
        /// The length of data in the stream.
        /// </summary>
        internal int StreamContentLength
        {
            get
            {
                long len = this.bodyContentStream.Length;
                if (len > ODataConstants.DefaultMaxReadMessageSize)
                {
                    throw new ODataException(String.Format(
                        CultureInfo.InvariantCulture,
                        "Single request body size {0} exceeds max size of {1}",
                        len,
                        ODataConstants.DefaultMaxReadMessageSize));
                }
                return (int)len;
            }
        }

        /// <summary>
        /// Reads from the batch stream while ensuring that we stop reading at delimiter.
        /// Not implemented.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <throws>NotImplementedException</returns>
        internal override int ReadWithDelimiter(byte[] userBuffer, int userBufferOffset, int count)
        {
            throw new NotImplementedException("Read with delimiter is not applicable for body content stream.");
        }

        private void EnsureBatchRequestBodyContentType(JsonReader jsonReader)
        {
            if (jsonReader.NodeType == JsonNodeType.StartObject)
            {
                this.contentType = BatchRequestBodyContentType.Json;
            }
            else if (jsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                this.contentType = BatchRequestBodyContentType.Binary;
            }
            else
            {
                throw new ODataException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Unsupported body content of NodeType: [{0}]",
                    jsonReader.NodeType));
            }
        }

        internal void PopulateBodyContent(JsonReader jsonReader)
        {
            Debug.Assert(!this.isDataPopulatedToStream, "!this.isDataPopulatedToStream");
            EnsureBatchRequestBodyContentType(jsonReader);

            switch (this.contentType)
            {
                case BatchRequestBodyContentType.Json:
                    {
                        WriteJsonContent(jsonReader);

                    }
                    break;
                case BatchRequestBodyContentType.Binary:
                    {
                        // body content is a string value of binary data.
                        string contentStr = jsonReader.ReadStringValue();

                        byte[] contentBytes = Encoding.UTF8.GetBytes(contentStr);

                        WriteBase64EncodedContent(contentBytes);
                    }
                    break;
                default:
                    throw new NotSupportedException("unknow / undefined type, new type that needs to be supported?");
            }

            // Set the flag so that data is only popuplated once.
            this.isDataPopulatedToStream = true;

            // Set the stream position to the beginning of the stream for reading.
            this.bodyContentStream.Position = 0;
        }

        /// <summary>
        /// Convert the binary data into base64-encoded bytes.
        /// </summary>
        /// <param name="bytes">Bindary data to be encoded.</param>
        /// <returns>Base64-encoded char array</returns>
        internal static char[] GetBase64Encode(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ODataException("Binary data cannot be null");
            }
            long encodedLength = (long)((4.0d / 3.0d) * bytes.Length);

            // Round up to the next the base64 encoded length to multiple of 4.
            if (encodedLength % 4 != 0)
            {
                encodedLength += 4 - encodedLength % 4;
            }

            char[] encodedChars = new char[encodedLength];
            Convert.ToBase64CharArray(bytes,
                         0,
                         bytes.Length,
                         encodedChars,
                         0);

            return encodedChars;
        }

        /// <summary>
        /// Read off the data of the starting Json object from the Json reader,
        /// and populate the data into the memory stream.
        /// </summary>
        /// <param name="reader"> The json reader pointing at the json structure whose data needs to
        /// be populated into an memory stream.
        /// </param>
        private void WriteJsonContent(JsonReader reader)
        {
            Debug.Assert(!this.isDataPopulatedToStream, "!this.isDataPopulatedToStream");

            // Reader is on the value node after the "body" property name node.
            IJsonWriter jsonWriter = new JsonWriter(
                streamWriter,
                false /*indent*/,
                ODataFormat.Json,
                true /*isIeee754Compatible*/);

            WriteCurrentJsonObject(reader, jsonWriter);
        }

        private void WriteBase64EncodedContent(byte[] bytes)
        {
            streamWriter.Write(bytes);
            streamWriter.Flush();
        }

        /// <summary>
        /// Write the current Json object.
        /// </summary>
        /// <param name="reader">The Json reader providing the data.</param>
        /// <param name="jsonWriter">The Json writer writes data into memory stream.</param>
        private static void WriteCurrentJsonObject(JsonReader reader, IJsonWriter jsonWriter)
        {
            Stack<JsonNodeType> nodeTypes = new Stack<JsonNodeType>();

            do
            {
                switch (reader.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        {
                            jsonWriter.WritePrimitiveValue(reader.Value);
                        }
                        break;
                    case JsonNodeType.Property:
                        {
                            jsonWriter.WriteName(reader.Value.ToString());
                        }
                        break;
                    case JsonNodeType.StartObject:
                        {
                            nodeTypes.Push(reader.NodeType);
                            jsonWriter.StartObjectScope();
                        }
                        break;
                    case JsonNodeType.StartArray:
                        {
                            nodeTypes.Push(reader.NodeType);
                            jsonWriter.StartArrayScope();
                        }
                        break;
                    case JsonNodeType.EndObject:
                        {
                            Debug.Assert(nodeTypes.Peek() == JsonNodeType.StartObject);
                            nodeTypes.Pop();
                            jsonWriter.EndObjectScope();
                        }
                        break;
                    case JsonNodeType.EndArray:
                        {
                            Debug.Assert(nodeTypes.Peek() == JsonNodeType.StartArray);
                            nodeTypes.Pop();
                            jsonWriter.EndArrayScope();
                        }
                        break;
                    default:
                        {
                            throw new ODataException(String.Format(
                                CultureInfo.InvariantCulture,
                                "Unexpected reader.NodeType: {0}.",
                                reader.NodeType));
                        }
                }

                reader.ReadNext(); // This can be EndOfInput, where nodeTypes should be empty.

            }
            while (nodeTypes.Count != 0);

            jsonWriter.Flush();
        }

        /// <summary>
        /// Reads from the batch stream since we know the length of the stream.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        internal override int ReadWithLength(byte[] userBuffer, int userBufferOffset, int count)
        {
            Debug.Assert(this.isDataPopulatedToStream, "this.isDataPopulatedToStream");
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
                    Buffer.BlockCopy(
                        this.batchBuffer.Bytes,
                        this.batchBuffer.CurrentReadPosition,
                        userBuffer,
                        userBufferOffset,
                        remainingNumberOfBytesToRead);

                    this.batchBuffer.SkipTo(this.batchBuffer.CurrentReadPosition + remainingNumberOfBytesToRead);
                    remainingNumberOfBytesToRead = 0;
                }
                else
                {
                    // we can only partially satisfy the read request
                    int availableBytesToRead = this.batchBuffer.NumberOfBytesInBuffer;
                    Buffer.BlockCopy(
                        this.batchBuffer.Bytes,
                        this.batchBuffer.CurrentReadPosition,
                        userBuffer,
                        userBufferOffset,
                        availableBytesToRead);

                    remainingNumberOfBytesToRead -= availableBytesToRead;
                    userBufferOffset += availableBytesToRead;

                    // we exhausted the buffer; if the underlying stream is not exhausted, refill the buffer
                    if (this.underlyingStreamExhausted)
                    {
                        // We cannot fully satisfy the read request since there are not enough bytes in the stream.
                        // This means that the content length of the stream was incorrect; this should never happen
                        // since the caller should already have checked this.
                        throw new ODataException(Strings.General_InternalError(
                            InternalErrorCodes.ODataBatchReaderStreamBuffer_ReadWithLength));
                    }
                    else
                    {
                        this.underlyingStreamExhausted = this.batchBuffer.RefillFrom(
                                this.bodyContentStream,
                                /*preserveFrom*/ ODataBatchReaderStreamBuffer.BufferLength);
                    }
                }
            }

            // return the number of bytes that were read
            return count - remainingNumberOfBytesToRead;
        }

        /// <summary>
        /// Dispose internal resource of stream writer and associated memory stream.
        /// This is the internal api overriding default no-op implementation of <see cref="ODataBatchReaderStream"/>
        /// to ensure proper disposing of resources.
        /// </summary>
        protected internal override void DisposeResources()
        {
            // dispose the IDisposable resources used by this class.
            this.Dispose();
            base.DisposeResources();
        }

        /// <summary>
        /// Dispose ther IDisposable resources in this class.
        /// </summary>
        public void Dispose()
        {
            if (this.streamWriter != null)
            {
                // Note that the stream writer will dispose the underlying stream as well.
                this.streamWriter.Dispose();
            }
        }
    }
}

