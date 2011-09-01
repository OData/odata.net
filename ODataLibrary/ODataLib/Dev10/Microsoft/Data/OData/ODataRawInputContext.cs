//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
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
    using Microsoft.Data.Edm;
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

        /// <summary>The encoding to use to read the stream.</summary>
        private readonly Encoding encoding;

        /// <summary>The input stream to read the data from.</summary>
        private Stream stream;

        /// <summary>The text reader to read non-binary values from.</summary>
        private TextReader textReader;

        /// <summary>Constructor.</summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="encoding">The encoding to use to read the input.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        private ODataRawInputContext(
            Stream stream,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            ODataPayloadKind readerPayloadKind)
            : base(messageReaderSettings, version, readingResponse, synchronous, model, urlResolver)
        {
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(readerPayloadKind != ODataPayloadKind.Unsupported, "readerPayloadKind != ODataPayloadKind.Unsupported");

            this.stream = stream;
            this.encoding = encoding;
            this.readerPayloadKind = readerPayloadKind;
        }

        /// <summary>
        /// Create RAW input context.
        /// </summary>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        /// <returns>The newly created input context.</returns>
        internal static ODataInputContext Create(
            ODataMessage message,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            ODataPayloadKind readerPayloadKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");
            Debug.Assert(readerPayloadKind != ODataPayloadKind.Unsupported, "readerPayloadKind != ODataPayloadKind.Unsupported");

            Stream messageStream = ODataInputContext.GetMessageStream(message, messageReaderSettings.DisableMessageStreamDisposal);
            return new ODataRawInputContext(
                messageStream,
                encoding,
                messageReaderSettings,
                version,
                readingResponse,
                true,
                model,
                urlResolver,
                readerPayloadKind);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create RAW input context.
        /// </summary>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        /// <returns>Task which when completed returns the newly create input context.</returns>
        internal static Task<ODataInputContext> CreateAsync(
            ODataMessage message,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            ODataPayloadKind readerPayloadKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            // TODO: Note that this will buffer the entire input. We need this for batch and for raw values since they
            // both use TextReader - verify that this is really what we want to do.
            return ODataInputContext.GetMessageStreamAsync(message, messageReaderSettings.DisableMessageStreamDisposal).ContinueWith(
                (streamTask) => (ODataInputContext)new ODataRawInputContext(
                    streamTask.Result,
                    encoding,
                    messageReaderSettings,
                    version,
                    readingResponse,
                    false,
                    model,
                    urlResolver,
                    readerPayloadKind),
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entities in the feed.</param>
        /// <returns>The newly created reader given the input to read from.</returns>
        internal override ODataReader CreateFeedReader(IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_CreateFeedReader));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entities in the feed.</param>
        /// <returns>Task which when completed returns the newly created reader given the input to read from.</returns>
        internal override Task<ODataReader> CreateFeedReaderAsync(IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_CreateFeedReader));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created reader given a stream to read from.</returns>
        internal override ODataReader CreateEntryReader(IEdmEntityType expectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_CreateEntryReader));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>Task which when completed returns the newly created reader given a stream to read from.</returns>
        internal override Task<ODataReader> CreateEntryReaderAsync(IEdmEntityType expectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_CreateEntryReader));
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Newly create <see cref="ODataCollectionReader"/>.</returns>
        internal override ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_CreateCollectionReader));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Task which when completed returns the newly create <see cref="ODataCollectionReader"/>.</returns>
        internal override Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_CreateCollectionReader));
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <returns>The newly created <see cref="ODataCollectionReader"/>.</returns>
        internal override ODataBatchReader CreateBatchReader(string batchBoundary)
        {
            DebugUtils.CheckNoExternalCallers();

            return new ODataBatchReader(this, batchBoundary);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataCollectionReader"/>.</returns>
        internal override Task<ODataBatchReader> CreateBatchReaderAsync(string batchBoundary)
        {
            DebugUtils.CheckNoExternalCallers();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateBatchReader(batchBoundary));
        }
#endif
        
        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal override ODataWorkspace ReadServiceDocument()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadServiceDocument));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal override Task<ODataWorkspace> ReadServiceDocumentAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadServiceDocument));
        }
#endif

        /// <summary>
        /// Read a metadata document. 
        /// This method reads the metadata document from the input and returns 
        /// an <see cref="IEdmModel"/> that represents the read metadata document.
        /// </summary>
        /// <returns>An <see cref="IEdmModel"/> representing the read metadata document.</returns>
        internal override IEdmModel ReadMetadataDocument()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadMetadataDocument));
        }

        /// <summary>
        /// This method creates an reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal override ODataProperty ReadProperty(IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadProperty));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read the property from the input and 
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>Task which when completed returns an <see cref="ODataProperty"/> representing the read property.</returns>
        internal override Task<ODataProperty> ReadPropertyAsync(IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadProperty));
        }
#endif

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        internal override ODataError ReadError()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadError));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataError"/> representing the read error.</returns>
        internal override Task<ODataError> ReadErrorAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadError));
        }
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadEntityReferenceLinks));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadEntityReferenceLinks));
        }
#endif

        /// <summary>
        /// Reads a top-level entity reference link.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadEntityReferenceLink));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level entity reference link.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataRawInputContext_ReadEntityReferenceLink));
        }
#endif

        /// <summary>
        /// Read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected primitive type for the value to be read; null if no expected type is available.</param>
        /// <returns>An <see cref="object"/> representing the read value.</returns>
        internal override object ReadValue(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();

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

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>Task which when completed returns an <see cref="object"/> representing the read value.</returns>
        internal override Task<object> ReadValueAsync(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.ReadValue(expectedPrimitiveTypeReference));
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
        /// Read the binary value from the stream.
        /// </summary>
        /// <returns>A byte array containing all the data read.</returns>
        private byte[] ReadBinaryValue()
        {
            //// This method is copied from Deserializer.ReadByteStream

            byte[] data;

            long numberOfBytesRead = 0;
            int result = 0;
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
