//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchBodyContentReaderStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using Microsoft.OData.Json;

    #endregion Namespaces

    /// <summary>
    /// Wrapper stream backed by memory stream containing body content of request or response in Json batch.
    /// </summary>
    internal sealed class ODataJsonLightBatchBodyContentReaderStream : ODataBatchReaderStream, IDisposable
    {
        /// <summary>
        /// Input memory stream providing body content data.
        /// </summary>
        private readonly Stream bodyContentStream = null;

        /// <summary>
        /// Writer for writing data to memory stream.
        /// </summary>
        private readonly StreamWriter streamWriter = null;

        /// <summary>
        /// Status to ensure that the underlying memory stream is ready for reading.
        /// </summary>
        private bool isDataPopulatedToStream;

        /// <summary>
        /// Type of the data in body.
        /// </summary>
        private BatchPayloadBodyContentType contentType;

        /// <summary>
        /// Constructor using default encoding (UTF-8 without the BOM preamble)
        /// </summary>
        internal ODataJsonLightBatchBodyContentReaderStream()
            : this(MediaTypeUtils.FallbackEncoding)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchEncoding">The encoding to use to read and write from the batch stream.</param>
        private ODataJsonLightBatchBodyContentReaderStream(Encoding batchEncoding)
            : base(batchEncoding)
        {
            this.bodyContentStream = new MemoryStream();

            this.streamWriter = new StreamWriter(this.bodyContentStream, batchEncoding);

            this.isDataPopulatedToStream = false;
        }

        /// <summary>
        /// Enum type for data type of body content.
        /// </summary>
        private enum BatchPayloadBodyContentType
        {
            // The body content contains Json data.
            Json,

            // The body content contains binary data, e.g. JPEG image raw data.
            Binary
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
                        "Single batch item body content size {0} exceeds max size of {1}",
                        len,
                        ODataConstants.DefaultMaxReadMessageSize));
                }

                return (int)len;
            }
        }

        /// <summary>
        /// Dispose the IDisposable resources in this class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reads from the batch stream while ensuring that we stop reading at delimiter.
        /// Not implemented.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        /// <throws>NotImplementedException</throws>
        internal override int ReadWithDelimiter(byte[] userBuffer, int userBufferOffset, int count)
        {
            throw new NotImplementedException("Read with delimiter is not applicable for body content stream.");
        }

        /// <summary>
        /// Populate the current property value the Json reader is referencing.
        /// The property value should be of either Json type or binary type.
        /// </summary>
        /// <param name="jsonReader">The Json reader providing access to the data.</param>
        internal void PopulateBodyContent(IJsonReader jsonReader)
        {
            Debug.Assert(!this.isDataPopulatedToStream, "!this.isDataPopulatedToStream");
            DetectBatchPayloadBodyContentType(jsonReader);

            switch (this.contentType)
            {
                case BatchPayloadBodyContentType.Json:
                    {
                        WriteJsonContent(jsonReader);
                    }

                    break;

                case BatchPayloadBodyContentType.Binary:
                    {
                        // body content is a string value of binary data.
                        string contentStr = jsonReader.ReadStringValue();

                        byte[] contentBytes = Encoding.UTF8.GetBytes(contentStr);

                        WriteBinaryContent(contentBytes);
                    }

                    break;

                default:
                    throw new NotSupportedException("unknown / undefined type, new type that needs to be supported?");
            }

            // Set the flag so that data is only populated once.
            this.isDataPopulatedToStream = true;

            // Rewind the stream position to the beginning of the stream so that it is ready for consumption.
            this.bodyContentStream.Position = 0;
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
            Debug.Assert(userBuffer != null, "userBuffer != null");
            Debug.Assert(userBufferOffset >= 0, "userBufferOffset >= 0");
            Debug.Assert(count >= 0, "count >= 0");
            Debug.Assert(this.batchEncoding != null, "Batch encoding should have been established on first call to SkipToBoundary.");

            // Ensure that the memory stream contains data to be consumed.
            if (!this.isDataPopulatedToStream)
            {
                throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_ReadImplementation));
            }

            // NOTE: if we have a stream with length we don't even check for boundaries but rely solely on the content length
            int remainingNumberOfBytesToRead = count;
            while (remainingNumberOfBytesToRead > 0)
            {
                // check whether we can satisfy the full read  from the buffer
                // or whether we have to split the request and read more data into the buffer.
                if (this.BatchBuffer.NumberOfBytesInBuffer >= remainingNumberOfBytesToRead)
                {
                    // we can satisfy the full read from the buffer
                    Buffer.BlockCopy(
                        this.BatchBuffer.Bytes,
                        this.BatchBuffer.CurrentReadPosition,
                        userBuffer,
                        userBufferOffset,
                        remainingNumberOfBytesToRead);

                    this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + remainingNumberOfBytesToRead);
                    remainingNumberOfBytesToRead = 0;
                }
                else
                {
                    // we can only partially satisfy the read
                    int availableBytesToRead = this.BatchBuffer.NumberOfBytesInBuffer;
                    Buffer.BlockCopy(
                        this.BatchBuffer.Bytes,
                        this.BatchBuffer.CurrentReadPosition,
                        userBuffer,
                        userBufferOffset,
                        availableBytesToRead);

                    remainingNumberOfBytesToRead -= availableBytesToRead;
                    userBufferOffset += availableBytesToRead;

                    // we exhausted the buffer; if the underlying stream is not exhausted, refill the buffer
                    if (this.underlyingStreamExhausted)
                    {
                        // We cannot fully satisfy the read since there are not enough bytes in the stream.
                        // This means that the content length of the stream was incorrect; this should never happen
                        // since the caller should already have checked this.
                        throw new ODataException(Strings.General_InternalError(
                            InternalErrorCodes.ODataBatchReaderStreamBuffer_ReadWithLength));
                    }
                    else
                    {
                        this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(
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

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.streamWriter != null)
                {
                    // Note that the stream writer will dispose the underlying stream as well.
                    this.streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Detect batch item (request or response) body content's data type.
        /// The content of the "body" property can be either Json type or binary type.
        /// </summary>
        /// <param name="jsonReader">The json reader that provides access to the json object.</param>
        private void DetectBatchPayloadBodyContentType(IJsonReader jsonReader)
        {
            if (jsonReader.NodeType == JsonNodeType.StartObject)
            {
                this.contentType = BatchPayloadBodyContentType.Json;
            }
            else if (jsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                this.contentType = BatchPayloadBodyContentType.Binary;
            }
            else
            {
                throw new ODataException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Unsupported body content of NodeType: [{0}]",
                    jsonReader.NodeType));
            }
        }

        /// <summary>
        /// Read off the data of the starting Json object from the Json reader,
        /// and populate the data into the memory stream.
        /// </summary>
        /// <param name="reader"> The json reader pointing at the json structure whose data needs to
        /// be populated into an memory stream.
        /// </param>
        private void WriteJsonContent(IJsonReader reader)
        {
            Debug.Assert(!this.isDataPopulatedToStream, "!this.isDataPopulatedToStream");

            // Reader is on the value node after the "body" property name node.
            IJsonWriter jsonWriter = new JsonWriter(
                streamWriter,
                true /*isIeee754Compatible*/);

            WriteCurrentJsonObject(reader, jsonWriter);
        }

        /// <summary>
        /// Writes the binary bytes to the underlying memory stream.
        /// </summary>
        /// <param name="bytes">The bytes to be written.</param>
        private void WriteBinaryContent(byte[] bytes)
        {
            streamWriter.Write(bytes);
            streamWriter.Flush();
        }

        /// <summary>
        /// Write the current Json object.
        /// </summary>
        /// <param name="reader">The Json reader providing the data.</param>
        /// <param name="jsonWriter">The Json writer writes data into memory stream.</param>
        private static void WriteCurrentJsonObject(IJsonReader reader, IJsonWriter jsonWriter)
        {
            Stack<JsonNodeType> nodeTypes = new Stack<JsonNodeType>();

            do
            {
                switch (reader.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        {
                            if (reader.Value != null)
                            {
                                jsonWriter.WritePrimitiveValue(reader.Value);
                            }
                            else
                            {
                                jsonWriter.WriteValue((string)null);
                            }
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
    }
}