//---------------------------------------------------------------------
// <copyright file="ODataRawInputContext.cs" company="Microsoft">
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
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the OData input for RAW OData format (raw value and batch).
    /// </summary>
    internal class ODataRawInputContext : ODataInputContext
    {
        /// <summary>The encoding to use to read from the batch stream.</summary>
        protected readonly Encoding Encoding;

        /// <summary>Use a buffer size of 4k that is read from the stream at a time.</summary>
        private const int BufferSize = 4096;

        /// <summary>The <see cref="ODataPayloadKind"/> to read.</summary>
        private readonly ODataPayloadKind readerPayloadKind;

        /// <summary>The input stream to read the data from.</summary>
        private Stream stream;

        /// <summary>The text reader to read non-binary values from.</summary>
        private TextReader textReader;

        /// <summary>Constructor.</summary>
        /// <param name="format">The format for this input context.</param>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        public ODataRawInputContext(
            ODataFormat format,
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
            : base(format, messageInfo, messageReaderSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");
            Debug.Assert(messageInfo.PayloadKind != ODataPayloadKind.Unsupported, "readerPayloadKind != ODataPayloadKind.Unsupported");

            try
            {
                this.stream = messageInfo.MessageStream;
                this.Encoding = messageInfo.Encoding;
                this.readerPayloadKind = messageInfo.PayloadKind;
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the input context.
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    messageInfo.MessageStream.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// The stream of the raw input context.
        /// </summary>
        public Stream Stream
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

#if PORTABLELIB
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
        /// Read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected primitive type for the value to be read; null if no expected type is available.</param>
        /// <returns>An <see cref="object"/> representing the read value.</returns>
        internal override object ReadValue(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            return this.ReadValueImplementation(expectedPrimitiveTypeReference);
        }

#if PORTABLELIB
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
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
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

            base.Dispose(disposing);
        }

        /// <summary>
        /// Create a <see cref="ODataAsynchronousReader"/>.
        /// </summary>
        /// <returns>The newly created <see cref="ODataAsynchronousReader"/>.</returns>
        private ODataAsynchronousReader CreateAsynchronousReaderImplementation()
        {
            return new ODataAsynchronousReader(this, this.Encoding);
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
                this.textReader = this.Encoding == null ? new StreamReader(this.stream) : new StreamReader(this.stream, this.Encoding);
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
            if (expectedPrimitiveTypeReference != null && this.MessageReaderSettings.EnablePrimitiveTypeConversion)
            {
                rawValue = ODataRawValueUtils.ConvertStringToPrimitive(stringFromStream, expectedPrimitiveTypeReference);
            }
            else
            {
                rawValue = stringFromStream;
            }

            return rawValue;
        }
    }
}
