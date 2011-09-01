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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Reader class used to read all OData payloads (entries, feeds, metadata documents, service documents, etc.).
    /// </summary>
    public sealed class ODataMessageReader : IDisposable
    {
        /// <summary>An array of all the supported payload kinds.</summary>
        /// <remarks>This array is ordered and we attempt to match content types to payload kinds in this order when GetFormat is called.</remarks>
        private static readonly ODataPayloadKind[] allSupportedPayloadKinds = new ODataPayloadKind[]
        {
            // NOTE: when GetFormat is called, we will try to match content types against the supported media types for a payload kind in the order of this array.
            ODataPayloadKind.Feed,
            ODataPayloadKind.Entry,
            ODataPayloadKind.Property,
            ODataPayloadKind.MetadataDocument,
            ODataPayloadKind.ServiceDocument,
            ODataPayloadKind.Value,
            ODataPayloadKind.BinaryValue,
            ODataPayloadKind.Collection,
            ODataPayloadKind.EntityReferenceLinks,
            ODataPayloadKind.EntityReferenceLink,
            ODataPayloadKind.Batch,
            ODataPayloadKind.Error,
        };

        /// <summary>The message for which the message reader was created.</summary>
        private readonly ODataMessage message;

        /// <summary>A flag indicating whether we are reading a request or a response message.</summary>
        private readonly bool readingResponse;

        /// <summary>The message reader settings to use when reading the message payload.</summary>
        private readonly ODataMessageReaderSettings settings;

        /// <summary>The model. Non-null if we do have metadata available.</summary>
        private readonly IEdmModel model;

        /// <summary>The <see cref="ODataVersion"/> to be used for reading the payload.</summary>
        private readonly ODataVersion version;

        /// <summary>The optional URL resolver to perform custom URL resolution for URLs read from the payload.</summary>
        private readonly IODataUrlResolver urlResolver;

        /// <summary>Flag to ensure that only a single read method is called on the message reader.</summary>
        private bool readMethodCalled;

        /// <summary>true if Dispose() has been called on this message reader, false otherwise.</summary>
        private bool isDisposed;

        /// <summary>The input context used to read the message content.</summary>
        private ODataInputContext inputContext;

        /// <summary>The payload kind of the payload to be read with this reader.</summary>
        /// <remarks>This field is either set by accessing the GetFormat method or implicitly when one of the read (or reader creation) methods is called.</remarks>
        private ODataPayloadKind readerPayloadKind = ODataPayloadKind.Unsupported;

        /// <summary>The <see cref="ODataFormat"/> of the payload to be read with this reader.</summary>
        /// <remarks>This field is either set by accessing the GetFormat method or implicitly when one of the read (or reader creation) methods is called.</remarks>
        private ODataFormat? format;

        /// <summary>The <see cref="Encoding"/> of the payload to be read with this reader.</summary>
        /// <remarks>This field is either set by accessing the GetFormat method or implicitly when one of the read (or reader creation) methods is called.</remarks>
        private Encoding encoding;

        /// <summary>The batch boundary string if the payload to be read is a batch request or response.</summary>
        /// <remarks>This is either set by accessing the GetFormat method or implicitly when the CreateBatchReader method is called.</remarks>
        private string batchBoundary;

        /// <summary>
        /// Creates a new ODataMessageReader for the given request message.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the reader.</param>
        public ODataMessageReader(IODataRequestMessage requestMessage)
            : this(requestMessage, new ODataMessageReaderSettings())
        {
        }

        /// <summary>
        /// Creates a new ODataMessageReader for the given request message and message reader settings.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the reader.</param>
        /// <param name="settings">The message reader settings to use for reading the message payload.</param>
        public ODataMessageReader(IODataRequestMessage requestMessage, ODataMessageReaderSettings settings)
            : this(requestMessage, settings, null)
        {
        }

        /// <summary>
        /// Creates a new ODataMessageReader for the given request message and message reader settings.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the reader.</param>
        /// <param name="settings">The message reader settings to use for reading the message payload.</param>
        /// <param name="model">The model to use.</param>
        public ODataMessageReader(IODataRequestMessage requestMessage, ODataMessageReaderSettings settings, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(requestMessage, "requestMessage");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            this.readingResponse = false;
            this.message = new ODataRequestMessage(requestMessage);
            this.settings = settings;
            this.urlResolver = requestMessage as IODataUrlResolver;
            this.version = ODataUtilsInternal.GetDataServiceVersion(this.message);
            if (model == null)
            {
                this.model = EdmCoreModel.Instance;
            }
            else
            {
                this.model = model;
                ValidationUtils.ValidateModel(model);
            }

            ODataVersionChecker.CheckVersionSupported(this.version);
            
            // TODO: We should clone the settings before validation to prevent users from modifying the settings while we use them.
            ReaderValidationUtils.ValidateMessageReaderSettings(this.settings);
        }

        /// <summary>
        /// Creates a new ODataMessageReader for the given response message.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the reader.</param>
        public ODataMessageReader(IODataResponseMessage responseMessage)
            : this(responseMessage, new ODataMessageReaderSettings())
        {
        }

        /// <summary>
        /// Creates a new ODataMessageReader for the given response message and message reader settings.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the reader.</param>
        /// <param name="settings">The message reader settings to use for reading the message payload.</param>
        public ODataMessageReader(IODataResponseMessage responseMessage, ODataMessageReaderSettings settings)
            : this(responseMessage, settings, null)
        {
        }

        /// <summary>
        /// Creates a new ODataMessageReader for the given response message and message reader settings.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the reader.</param>
        /// <param name="settings">The message reader settings to use for reading the message payload.</param>
        /// <param name="model">The model to use.</param>
        public ODataMessageReader(IODataResponseMessage responseMessage, ODataMessageReaderSettings settings, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(responseMessage, "responseMessage");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            this.readingResponse = true;
            this.message = new ODataResponseMessage(responseMessage);
            this.settings = settings;
            this.urlResolver = responseMessage as IODataUrlResolver;
            this.version = ODataUtilsInternal.GetDataServiceVersion(this.message);
            if (model == null)
            {
                this.model = EdmCoreModel.Instance;
            }
            else
            {
                this.model = model;
                ValidationUtils.ValidateModel(model);
            }

            ODataVersionChecker.CheckVersionSupported(this.version);

            // TODO: We should clone the settings before validation to prevent users from modifying the settings while we use them.
            ReaderValidationUtils.ValidateMessageReaderSettings(this.settings);
        }

        /// <summary>
        /// Determines the format of the payload being read and returns it.
        /// </summary>
        /// <returns>The format of the payload being read by this reader.</returns>
        /// <remarks>
        /// The format of the payload is determined by looking at the content type header of the message. 
        /// If the format cannot be derived from the content type header the method will fail.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "We decided to make this a method due to the fact that 'interesting' exceptions might be thrown from the method.")]
        public ODataFormat GetFormat()
        {
            if (!this.format.HasValue)
            {
                this.ProcessContentType(allSupportedPayloadKinds);
            }

            return this.format.Value;
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataFeedReader()
        {
            return this.CreateODataFeedReader(null);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base type for the entities in the feed.</param>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataFeedReader(IEdmEntityType expectedBaseEntityType)
        {
            this.VerifyCanCreateODataFeedReader(expectedBaseEntityType);
            return this.ReadFromInput(
                (context) => context.CreateFeedReader(expectedBaseEntityType),
                ODataPayloadKind.Feed);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <returns>A running task for the created reader.</returns>
        public Task<ODataReader> CreateODataFeedReaderAsync()
        {
            return this.CreateODataFeedReaderAsync(null);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base type for the entities in the feed.</param>
        /// <returns>A running task for the created reader.</returns>
        public Task<ODataReader> CreateODataFeedReaderAsync(IEdmEntityType expectedBaseEntityType)
        {
            this.VerifyCanCreateODataFeedReader(expectedBaseEntityType);
            return this.ReadFromInputAsync(
                (context) => context.CreateFeedReaderAsync(expectedBaseEntityType),
                ODataPayloadKind.Feed);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataEntryReader()
        {
            return this.CreateODataEntryReader(null);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataEntryReader(IEdmEntityType expectedEntityType)
        {
            this.VerifyCanCreateODataEntryReader(expectedEntityType);
            return this.ReadFromInput(
                (context) => context.CreateEntryReader(expectedEntityType),
                ODataPayloadKind.Entry);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <returns>A running task for the created reader.</returns>
        public Task<ODataReader> CreateODataEntryReaderAsync()
        {
            return this.CreateODataEntryReaderAsync(null);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>A running task for the created reader.</returns>
        public Task<ODataReader> CreateODataEntryReaderAsync(IEdmEntityType expectedEntityType)
        {
            this.VerifyCanCreateODataEntryReader(expectedEntityType);
            return this.ReadFromInputAsync(
                (context) => context.CreateEntryReaderAsync(expectedEntityType),
                ODataPayloadKind.Entry);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader" /> to read a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>The created collection reader.</returns>
        public ODataCollectionReader CreateODataCollectionReader()
        {
            return this.CreateODataCollectionReader(null);
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader" /> to read a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>The created collection reader.</returns>
        public ODataCollectionReader CreateODataCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            this.VerifyCanCreateODataCollectionReader(expectedItemTypeReference);
            return this.ReadFromInput(
                (context) => context.CreateCollectionReader(expectedItemTypeReference),
                ODataPayloadKind.Collection);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionReader" /> to read a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>A running task for the created collection reader.</returns>
        public Task<ODataCollectionReader> CreateODataCollectionReaderAsync()
        {
            return this.CreateODataCollectionReaderAsync(null);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionReader" /> to read a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>A running task for the created collection reader.</returns>
        public Task<ODataCollectionReader> CreateODataCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference)
        {
            this.VerifyCanCreateODataCollectionReader(expectedItemTypeReference);
            return this.ReadFromInputAsync(
                (context) => context.CreateCollectionReaderAsync(expectedItemTypeReference),
                ODataPayloadKind.Collection);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataBatchReader" /> to read a batch of requests or responses.
        /// </summary>
        /// <returns>The created batch reader.</returns>
        public ODataBatchReader CreateODataBatchReader()
        {
            this.VerifyCanCreateODataBatchReader();
            return this.ReadFromInput(
                (context) => context.CreateBatchReader(this.batchBoundary),
                ODataPayloadKind.Batch);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchReader" /> to read a batch of requests or responses.
        /// </summary>
        /// <returns>A running task for the created batch reader.</returns>
        public Task<ODataBatchReader> CreateODataBatchReaderAsync()
        {
            this.VerifyCanCreateODataBatchReader();
            return this.ReadFromInputAsync(
                (context) => context.CreateBatchReaderAsync(this.batchBoundary),
                ODataPayloadKind.Batch);
        }
#endif

        /// <summary>
        /// Reads a service document payload.
        /// </summary>
        /// <returns>The service document read.</returns>
        public ODataWorkspace ReadServiceDocument()
        {
            this.VerifyCanReadServiceDocument();
            return this.ReadFromInput(
                (context) => context.ReadServiceDocument(),
                ODataPayloadKind.ServiceDocument);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reads a service document payload.
        /// </summary>
        /// <returns>A task representing the asynchronous operation of reading the service document.</returns>
        public Task<ODataWorkspace> ReadServiceDocumentAsync()
        {
            this.VerifyCanReadServiceDocument();
            return this.ReadFromInputAsync(
                (context) => context.ReadServiceDocumentAsync(),
                ODataPayloadKind.ServiceDocument);
        }
#endif

        /// <summary>
        /// Reads an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <returns>The property read from the payload.</returns>
        public ODataProperty ReadProperty()
        {
            return this.ReadProperty(null);
        }

        /// <summary>
        /// Reads an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>The property read from the payload.</returns>
        public ODataProperty ReadProperty(IEdmTypeReference expectedPropertyTypeReference)
        {
            this.VerifyCanReadProperty(expectedPropertyTypeReference);
            return this.ReadFromInput(
                (context) => context.ReadProperty(expectedPropertyTypeReference),
                ODataPayloadKind.Property);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reads an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <returns>A task representing the asynchronous operation of reading the property.</returns>
        public Task<ODataProperty> ReadPropertyAsync()
        {
            return this.ReadPropertyAsync(null);
        }

        /// <summary>
        /// Asynchronously reads an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>A task representing the asynchronous operation of reading the property.</returns>
        public Task<ODataProperty> ReadPropertyAsync(IEdmTypeReference expectedPropertyTypeReference)
        {
            this.VerifyCanReadProperty(expectedPropertyTypeReference);
            return this.ReadFromInputAsync(
                (context) => context.ReadPropertyAsync(expectedPropertyTypeReference),
                ODataPayloadKind.Property);
        }
#endif

        /// <summary>
        /// Reads an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <returns>The <see cref="ODataError"/> read from the message payload.</returns>
        public ODataError ReadError()
        {
            this.VerifyCanReadError();
            return this.ReadFromInput(
                (context) => context.ReadError(),
                ODataPayloadKind.Error);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reades an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <returns>A task representing the asynchronous operation of reading the error.</returns>
        public Task<ODataError> ReadErrorAsync()
        {
            this.VerifyCanReadError();
            return this.ReadFromInputAsync(
                (context) => context.ReadErrorAsync(),
                ODataPayloadKind.Error);
        }
#endif

        /// <summary>
        /// Reads the result of a $links query (entity reference links) as the message payload.
        /// </summary>
        /// <returns>The entity reference links read as message payload.</returns>
        public ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            this.VerifyCanReadEntityReferenceLinks();
            return this.ReadFromInput(
                (context) => context.ReadEntityReferenceLinks(),
                ODataPayloadKind.EntityReferenceLinks);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reads the result of a $links query as the message payload.
        /// </summary>
        /// <returns>A task representing the asynchronous reading of the entity reference links.</returns>
        public Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            this.VerifyCanReadEntityReferenceLinks();
            return this.ReadFromInputAsync(
                (context) => context.ReadEntityReferenceLinksAsync(),
                ODataPayloadKind.EntityReferenceLinks);
        }
#endif

        /// <summary>
        /// Reads a singleton result of a $links query (entity reference link) as the message payload.
        /// </summary>
        /// <returns>The entity reference link read from the message payload.</returns>
        public ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            this.VerifyCanReadEntityReferenceLink();
            return this.ReadFromInput(
                (context) => context.ReadEntityReferenceLink(),
                ODataPayloadKind.EntityReferenceLink);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reads a singleton result of a $links query (entity reference link) as the message payload.
        /// </summary>
        /// <returns>A running task representing the reading of the entity reference link.</returns>
        public Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
        {
            this.VerifyCanReadEntityReferenceLink();
            return this.ReadFromInputAsync(
                (context) => context.ReadEntityReferenceLinkAsync(),
                ODataPayloadKind.EntityReferenceLink);
        }
#endif

        /// <summary>
        /// Reads a single value as the message body.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>The read value.</returns>
        public object ReadValue(IEdmTypeReference expectedTypeReference)
        {
            this.VerifyCanReadValue(expectedTypeReference);

            return this.ReadFromInput(
                (context) => context.ReadValue((IEdmPrimitiveTypeReference)expectedTypeReference),
                new ODataPayloadKind[] { ODataPayloadKind.Value, ODataPayloadKind.BinaryValue });
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reads a single value as the message body.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>A running task representing the reading of the value.</returns>
        public Task<object> ReadValueAsync(IEdmTypeReference expectedTypeReference)
        {
            this.VerifyCanReadValue(expectedTypeReference);
            return this.ReadFromInputAsync(
                (context) => context.ReadValueAsync((IEdmPrimitiveTypeReference)expectedTypeReference),
                new ODataPayloadKind[] { ODataPayloadKind.Value, ODataPayloadKind.BinaryValue });
        }
#endif

        /// <summary>
        /// Reads the message body as metadata document.
        /// </summary>
        /// <returns>The <see cref="IEdmModel"/> instance read from the message payload.</returns>
        public IEdmModel ReadMetadataDocument()
        {
            this.VerifyCanReadMetadataDocument();
            return this.ReadFromInput(
                (context) => context.ReadMetadataDocument(),
                ODataPayloadKind.MetadataDocument);
        }

        /// <summary>
        /// IDisposable.Dispose() implementation to cleanup unmanaged resources of the reader.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If the content type header of the message has not been processed to determine the format of the payload, the encoding and the payload kind,
        /// this method will do so. Otherwise verifies that the kind determined from the content type is compatible with one that is being passed in <paramref name="payloadKinds"/>.
        /// </summary>
        /// <param name="payloadKinds">All possible kinds of payload to be read with this message reader; must not include ODataPayloadKind.Unsupported.</param>
        private void ProcessContentTypeOrVerifyPayloadKind(params ODataPayloadKind[] payloadKinds)
        {
            Debug.Assert(!payloadKinds.Contains(ODataPayloadKind.Unsupported), "!payloadKinds.Contains(ODataPayloadKind.Unsupported)");

            // verify that no payload kind has been set or that the payload kind set previously and the 
            // payload that is attempted to being read are the same
            this.VerifyPayloadKind(payloadKinds);

            if (!this.format.HasValue)
            {
                // no format, encoding and payload kind have been set; set them now
                this.ProcessContentType(payloadKinds);
            }
            else
            {
                Debug.Assert(this.readerPayloadKind != ODataPayloadKind.Unsupported, "this.readerPayloadKind != ODataPayloadKind.Unsupported");
            }
        }

        /// <summary>
        /// Sets the content type related fields (format, encoding, payloadKind, etc.) based on the content type of the message.
        /// Fails if no content type is specified for the message. Assumes that the content type fields have not been filled yet.
        /// </summary>
        /// <param name="payloadKinds">All possible payload kinds to be read from the message.</param>
        private void ProcessContentType(ODataPayloadKind[] payloadKinds)
        {
            Debug.Assert(!this.format.HasValue, "Expected no content type fields to be set when this method is called.");
            Debug.Assert(!payloadKinds.Contains(ODataPayloadKind.Unsupported), "!payloadKinds.Contains(ODataPayloadKind.Unsupported)");

            string contentTypeHeader = this.message.GetHeader(ODataHttpHeaders.ContentType);
            contentTypeHeader = contentTypeHeader == null ? null : contentTypeHeader.Trim();
            if (string.IsNullOrEmpty(contentTypeHeader))
            {
                throw new ODataException(Strings.ODataMessageReader_NoneOrEmptyContentTypeHeader);
            }

            MediaType mediaType;
            this.format = MediaTypeUtils.GetFormatFromContentType(contentTypeHeader, payloadKinds, out mediaType, out this.encoding, out this.readerPayloadKind, out this.batchBoundary);
        }

        /// <summary>
        /// Verifies that no content type fields have been set or that the information set previously and the payload kind
        /// being read are compatible.
        /// </summary>
        /// <param name="payloadKindsToRead">All possible payload kinds that can be attempted to be read.</param>
        private void VerifyPayloadKind(ODataPayloadKind[] payloadKindsToRead)
        {
            Debug.Assert(!payloadKindsToRead.Contains(ODataPayloadKind.Unsupported), "!payloadKindsToRead.Contains(ODataPayloadKind.Unsupported)");

            if (this.format.HasValue && this.readerPayloadKind != ODataPayloadKind.Unsupported && !payloadKindsToRead.Contains(this.readerPayloadKind))
            {
                // if a payload kind has been determined via GetFormat and a different payload kind is attempted to be read, we fail.
                throw new ODataException(Strings.ODataMessageReader_IncompatiblePayloadKinds(this.readerPayloadKind, payloadKindsToRead));
            }
        }

        /// <summary>
        /// Verify arguments for creation of an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entities in the feed.</param>
        private void VerifyCanCreateODataFeedReader(IEdmEntityType expectedBaseEntityType)
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (expectedBaseEntityType != null)
            {
                if (!this.model.IsUserModel())
                {
                    throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata("expectedBaseEntityType"), "expectedBaseEntityType");
                }
            }
        }

        /// <summary>
        /// Verify arguments for creation of an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        private void VerifyCanCreateODataEntryReader(IEdmEntityType expectedEntityType)
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (expectedEntityType != null)
            {
                if (!this.model.IsUserModel())
                {
                    throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata("expectedEntityType"), "expectedEntityType");
                }
            }
        }

        /// <summary>
        /// Verify arguments for creation of an <see cref="ODataCollectionReader" /> to read a collection of primitive or complex values 
        /// (as result of a service operation invocation).
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type for the items in the collection.</param>
        private void VerifyCanCreateODataCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (expectedItemTypeReference != null)
            {
                if (!this.model.IsUserModel())
                {
                    throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata("expectedItemTypeReference"), "expectedItemTypeReference");
                }

                if (!expectedItemTypeReference.IsODataPrimitiveTypeKind() && expectedItemTypeReference.TypeKind() != EdmTypeKind.Complex)
                {
                    throw new ArgumentException(
                        Strings.ODataMessageReader_ExpectedCollectionTypeWrongKind(expectedItemTypeReference.TypeKind().ToString()),
                        "expectedItemTypeReference");
                }
            }
        }

        /// <summary>
        /// Verify arguments for creation of a batch as the message body.
        /// </summary>
        private void VerifyCanCreateODataBatchReader()
        {
            this.VerifyReaderNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verify arguments for creation of an <see cref="ODataParameterReader" /> to read the parameters for <paramref name="functionImport"/>.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        private void VerifyCanCreateODataParameterReader(IEdmFunctionImport functionImport)
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (functionImport != null && !this.model.IsUserModel())
            {
                throw new ArgumentException(Strings.ODataMessageReader_FunctionImportSpecifiedWithoutMetadata("functionImport"), "functionImport");
            }
        }

        /// <summary>
        /// Verify arguments for reading of a service document payload.
        /// </summary>
        private void VerifyCanReadServiceDocument()
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (!this.readingResponse)
            {
                throw new ODataException(Strings.ODataMessageReader_ServiceDocumentInRequest);
            }
        }

        /// <summary>
        /// Verify arguments for reading of a metadata document payload.
        /// </summary>
        private void VerifyCanReadMetadataDocument()
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (!this.readingResponse)
            {
                throw new ODataException(Strings.ODataMessageReader_MetadataDocumentInRequest);
            }
        }

        /// <summary>
        /// Verify arguments for reading of an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        private void VerifyCanReadProperty(IEdmTypeReference expectedPropertyTypeReference)
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (expectedPropertyTypeReference != null)
            {
                if (!this.model.IsUserModel())
                {
                    throw new ArgumentException(Strings.ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata("expectedPropertyTypeReference"), "expectedPropertyTypeReference");
                }

                if (expectedPropertyTypeReference.IsODataEntityTypeKind())
                {
                    throw new ArgumentException(Strings.ODataMessageReader_ExpectedPropertyTypeEntityKind, "expectedPropertyTypeReference");
                }
                else if (expectedPropertyTypeReference.IsStream())
                {
                    throw new ArgumentException(Strings.ODataMessageReader_ExpectedPropertyTypeStream, "expectedPropertyTypeReference");
                }
            }
        }

        /// <summary>
        /// Verify arguments for reading of an <see cref="ODataError"/> as the message payload.
        /// </summary>
        private void VerifyCanReadError()
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (!this.readingResponse)
            {
                // top-level errors can only be read for response messages
                throw new ODataException(Strings.ODataMessageReader_ErrorPayloadInRequest);
            }
        }

        /// <summary>
        /// Verify arguments for reading of the result of a $links query (entity reference links) as the message payload.
        /// </summary>
        private void VerifyCanReadEntityReferenceLinks()
        {
            // NOTE: we decided to not stream links for now but only make reading them async.
            this.VerifyReaderNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verify arguments for reading of a singleton result of a $links query (entity reference link) as the message payload.
        /// </summary>
        private void VerifyCanReadEntityReferenceLink()
        {
            this.VerifyReaderNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verify arguments for reading of a single value as the message body.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        private void VerifyCanReadValue(IEdmTypeReference expectedTypeReference)
        {
            this.VerifyReaderNotDisposedAndNotUsed();

            if (expectedTypeReference != null && !expectedTypeReference.IsODataPrimitiveTypeKind())
            {
                throw new ArgumentException(
                    Strings.ODataMessageReader_ExpectedValueTypeWrongKind(expectedTypeReference.TypeKind().ToString()),
                    "expectedTypeReference");
            }
        }

        /// <summary>
        /// Verifies that the ODataMessageReader has not been used before; an ODataMessageReader can only be used to
        /// read a single message payload but cannot be reused later.
        /// </summary>
        private void VerifyReaderNotDisposedAndNotUsed()
        {
            this.VerifyNotDisposed();
            if (this.readMethodCalled)
            {
                throw new ODataException(Strings.ODataMessageReader_ReaderAlreadyUsed);
            }

            this.readMethodCalled = true;
        }

        /// <summary>
        /// Check if the object has been disposed. Throws an ObjectDisposedException if the object has already been disposed.
        /// </summary>
        private void VerifyNotDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        private void Dispose(bool disposing)
        {
            this.VerifyNotDisposed();
            this.isDisposed = true;
            if (disposing)
            {
                if (this.inputContext != null)
                {
                    this.inputContext.Dispose();
                    this.inputContext = null;
                }
            }
        }

        /// <summary>
        /// Method which creates an input context around the input message and calls a func to read the input.
        /// </summary>
        /// <typeparam name="T">The type returned by the read method.</typeparam>
        /// <param name="readFunc">The read function which will be called over the created input context.</param>
        /// <param name="payloadKinds">All possible kinds of payload to read.</param>
        /// <returns>The read value from the input.</returns>
        private T ReadFromInput<T>(Func<ODataInputContext, T> readFunc, params ODataPayloadKind[] payloadKinds) where T : class
        {
            this.ProcessContentTypeOrVerifyPayloadKind(payloadKinds);
            Debug.Assert(this.format.HasValue, "By now we should have figured out which format to use.");

            this.inputContext = ODataInputContext.CreateInputContext(
                this.format.Value,
                this.readerPayloadKind,
                this.message,
                this.encoding,
                this.settings,
                this.version,
                this.readingResponse,
                this.model,
                this.urlResolver);

            return readFunc(this.inputContext);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Method which asynchronously creates an input context around the input message and calls a func to read the input.
        /// </summary>
        /// <typeparam name="T">The type returned by the read method.</typeparam>
        /// <param name="readFunc">The read function which will be called over the created input context.</param>
        /// <param name="payloadKinds">All possible kinds of payload to read.</param>
        /// <returns>A task which when completed return the read value from the input.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by the caller.")]
        private Task<T> ReadFromInputAsync<T>(Func<ODataInputContext, Task<T>> readFunc, params ODataPayloadKind[] payloadKinds) where T : class
        {
            this.ProcessContentTypeOrVerifyPayloadKind(payloadKinds);
            Debug.Assert(this.format.HasValue, "By now we should have figured out which format to use.");

            return ODataInputContext.CreateInputContextAsync(
                this.format.Value,
                this.readerPayloadKind,
                this.message,
                this.encoding,
                this.settings,
                this.version,
                this.readingResponse,
                this.model,
                this.urlResolver)
                
                .ContinueWith(
                (createInputContextTask) =>
                {
                    this.inputContext = createInputContextTask.Result;
                    return readFunc(this.inputContext);
                },
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent)

                // TODO: review how exceptions are handled by Unwrap
                .Unwrap();
        }
#endif
    }
}
