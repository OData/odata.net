//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the OData input for RAW OData format (raw value and batch).
    /// </summary>
    internal sealed class ODataRawInputContext : ODataInputContext
    {
        /// <summary>Use a buffer size of 4k that is read from the stream at a time.</summary>
        private const int BufferSize = 4096;

        /// <summary>The <see cref="ODataPayloadKind"/> to read.</summary>
        private readonly ODataPayloadKind readerPayloadKind;

        /// <summary>The encoding to use to read from the batch stream.</summary>
        private readonly Encoding encoding;

        /// <summary>The input stream to read the data from.</summary>
        private Stream stream;

        /// <summary>The text reader to read non-binary values from.</summary>
        private TextReader textReader;

        /// <summary>Constructor.</summary>
        /// <param name="format">The format for this input context.</param>
        /// <param name="messageStream">The stream to read data from.</param>
        /// <param name="encoding">The encoding to use to read the input.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        internal ODataRawInputContext(
            ODataFormat format,
            Stream messageStream,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            ODataPayloadKind readerPayloadKind)
            : base(format, messageReaderSettings, version, readingResponse, synchronous, model, urlResolver)
        {
            Debug.Assert(messageStream != null, "stream != null");
            Debug.Assert(readerPayloadKind != ODataPayloadKind.Unsupported, "readerPayloadKind != ODataPayloadKind.Unsupported");

            ExceptionUtils.CheckArgumentNotNull(format, "format");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

            try
            { 
                this.stream = messageStream;
                this.encoding = encoding;
                this.readerPayloadKind = readerPayloadKind;
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the input context.
                if (ExceptionUtils.IsCatchableExceptionType(e) && messageStream != null)
                {
                    messageStream.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// The stream of the raw input context.
        /// </summary>
        internal Stream Stream
        {
            get
            {
                return this.stream;
            }
        }

        /// <summary>
        /// Create an <see cref="ODataAsynchronousReader"/>.
        /// </summary>
        /// <returns>The newly created <see cref="ODataAsynchronousReader"/>.</returns>
        internal override ODataAsynchronousReader CreateAsynchronousReader()
        {
            return this.CreateAsynchronousReaderImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create an <see cref="ODataAsynchronousReader"/>.
        /// </summary>
        /// <returns>Task which when completed returns the newly created <see cref="ODataAsynchronousReader"/>.</returns>
        internal override Task<ODataAsynchronousReader> CreateAsynchronousReaderAsync()
        {
            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateAsynchronousReaderImplementation());
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <returns>The newly created <see cref="ODataCollectionReader"/>.</returns>
        internal override ODataBatchReader CreateBatchReader(string batchBoundary)
        {
            return this.CreateBatchReaderImplementation(batchBoundary, /*synchronous*/ true);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataCollectionReader"/>.</returns>
        internal override Task<ODataBatchReader> CreateBatchReaderAsync(string batchBoundary)
        {
            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateBatchReaderImplementation(batchBoundary, /*synchronous*/ false));
        }
#endif
        
        /// <summary>
        /// Read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected primitive type for the value to be read; null if no expected type is available.</param>
        /// <returns>An <see cref="object"/> representing the read value.</returns>
        internal override object ReadValue(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            return this.ReadValueImplementation(expectedPrimitiveTypeReference);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>Task which when completed returns an <see cref="object"/> representing the read value.</returns>
        internal override Task<object> ReadValueAsync(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.ReadValueImplementation(expectedPrimitiveTypeReference));
        }
#endif

        /// <summary>
        /// Disposes the input context.
        /// </summary>
        protected override void DisposeImplementation()
        {
            try
            {
                if (this.textReader != null)
                {
                    this.textReader.Dispose();
                }
                else if (this.stream != null)
                {
                    this.stream.Dispose();
                }
            }
            finally
            {
                this.textReader = null;
                this.stream = null;
            }
        }

        /// <summary>
        /// Create a <see cref="ODataAsynchronousReader"/>.
        /// </summary>
        /// <returns>The newly created <see cref="ODataAsynchronousReader"/>.</returns>
        private ODataAsynchronousReader CreateAsynchronousReaderImplementation()
        {
            return new ODataAsynchronousReader(this, this.encoding);
        }

        /// <summary>
        /// Create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <param name="synchronous">If the reader should be created for synchronous or asynchronous API.</param>
        /// <returns>The newly created <see cref="ODataCollectionReader"/>.</returns>
        private ODataBatchReader CreateBatchReaderImplementation(string batchBoundary, bool synchronous)
        {
            return new ODataBatchReader(this, batchBoundary, this.encoding, synchronous);
        }

        /// <summary>
        /// Read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected primitive type for the value to be read; null if no expected type is available.</param>
        /// <returns>An <see cref="object"/> representing the read value.</returns>
        private object ReadValueImplementation(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            // if an expected primitive type is specified it trumps the content type/reader payload kind
            bool readBinary;
            if (expectedPrimitiveTypeReference == null)
            {
                readBinary = this.readerPayloadKind == ODataPayloadKind.BinaryValue;
            }
            else
            {
                if (expectedPrimitiveTypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.Binary)
                {
                    readBinary = true;
                }
                else
                {
                    readBinary = false;
                }
            }

            if (readBinary)
            {
                return this.ReadBinaryValue();
            }
            else
            {
                Debug.Assert(this.textReader == null, "this.textReader == null");
                this.textReader = this.encoding == null ? new StreamReader(this.stream) : new StreamReader(this.stream, this.encoding);
                return this.ReadRawValue(expectedPrimitiveTypeReference);
            }
        }

        /// <summary>
        /// Read the binary value from the stream.
        /// </summary>
        /// <returns>A byte array containing all the data read.</returns>
        private byte[] ReadBinaryValue()
        {
            //// This method is copied from Deserializer.ReadByteStream

            byte[] data;

            long numberOfBytesRead = 0;
            int result;
            List<byte[]> byteData = new List<byte[]>();

            do
            {
                data = new byte[BufferSize];
                result = this.stream.Read(data, 0, data.Length);
                numberOfBytesRead += result;
                byteData.Add(data);
            }
            while (result == data.Length);

            // Find out the total number of bytes read and copy data from byteData to data
            data = new byte[numberOfBytesRead];
            for (int i = 0; i < byteData.Count - 1; i++)
            {
                Buffer.BlockCopy(byteData[i], 0, data, i * BufferSize, BufferSize);
            }

            // For the last buffer, copy the remaining number of bytes, not always the buffer size
            Buffer.BlockCopy(byteData[byteData.Count - 1], 0, data, (byteData.Count - 1) * BufferSize, result);
            return data;
        }

        /// <summary>
        /// Reads the content of a text reader as string and, if <paramref name="expectedPrimitiveTypeReference"/> is specified and primitive type conversion
        /// is enabled, converts the string to the expected type.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type of the value being read or null if no type conversion should be performed.</param>
        /// <returns>The raw value that was read from the text reader either as string or converted to the provided <paramref name="expectedPrimitiveTypeReference"/>.</returns>
        private object ReadRawValue(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            string stringFromStream = this.textReader.ReadToEnd();

            object rawValue;
            if (expectedPrimitiveTypeReference != null && !this.MessageReaderSettings.DisablePrimitiveTypeConversion)
            {
                rawValue = AtomValueUtils.ConvertStringToPrimitive(stringFromStream, expectedPrimitiveTypeReference);
            }
            else
            {
                rawValue = stringFromStream;
            }

            return rawValue;
        }
    }
}
