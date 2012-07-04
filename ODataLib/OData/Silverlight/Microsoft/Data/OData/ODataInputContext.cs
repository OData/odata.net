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
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for all input contexts, defines the interface 
    /// to be implemented by the specific formats.
    /// </summary>
    internal abstract class ODataInputContext : IDisposable
    {
        /// <summary>The format for this input context.</summary>
        private readonly ODataFormat format;

        /// <summary>The message reader settings to be used for reading.</summary>
        private readonly ODataMessageReaderSettings messageReaderSettings;

        /// <summary>The protocol version to use when reading the payload.</summary>
        private readonly ODataVersion version;

        /// <summary>Set to true if this context is reading a response payload.</summary>
        private readonly bool readingResponse;

        /// <summary>true if the input should be read synchronously; false if it should be read asynchronously.</summary>
        private readonly bool synchronous;

        /// <summary>The model to use.</summary>
        private readonly IEdmModel model;

        /// <summary>The optional URL resolver to perform custom URL resolution for URLs read from the payload.</summary>
        private readonly IODataUrlResolver urlResolver;

        /// <summary>Set to true if the input was disposed.</summary>
        private bool disposed;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this input context.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        protected ODataInputContext(
            ODataFormat format,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            ExceptionUtils.CheckArgumentNotNull(format, "format");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

            this.format = format;
            this.messageReaderSettings = messageReaderSettings;
            this.version = version;
            this.readingResponse = readingResponse;
            this.synchronous = synchronous;
            this.model = model;
            this.urlResolver = urlResolver;
        }

        /// <summary>
        /// The message reader settings to be used for reading.
        /// </summary>
        internal ODataMessageReaderSettings MessageReaderSettings
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.messageReaderSettings;
            }
        }

        /// <summary>
        /// The version of the OData protocol to use.
        /// </summary>
        internal ODataVersion Version
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.version;
            }
        }

        /// <summary>
        /// Set to true if a response is being read.
        /// </summary>
        internal bool ReadingResponse
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.readingResponse;
            }
        }

        /// <summary>
        /// true if the input should be read synchronously; false if it should be read asynchronously.
        /// </summary>
        internal bool Synchronous
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.synchronous;
            }
        }

        /// <summary>
        /// The model to use or null if no metadata is available.
        /// </summary>
        internal IEdmModel Model
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.model;
            }
        }

        /// <summary>
        /// The optional URL resolver to perform custom URL resolution for URLs read from the payload.
        /// </summary>
        internal IODataUrlResolver UrlResolver
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.urlResolver;
            }
        }

        /// <summary>
        /// true if the WCF DS client compatibility format behavior should be used; otherwise false.
        /// </summary>
        protected internal bool UseClientFormatBehavior
        {
            get
            {
                return this.messageReaderSettings.ReaderBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient;
            }
        }

        /// <summary>
        /// true if the WCF DS server compatibility format behavior should be used; otherwise false.
        /// </summary>
        protected internal bool UseServerFormatBehavior
        {
            get
            {
                return this.messageReaderSettings.ReaderBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesServer;
            }
        }

        /// <summary>
        /// true if the default format behavior should be used; otherwise false.
        /// </summary>
        protected internal bool UseDefaultFormatBehavior
        {
            get
            {
                return this.messageReaderSettings.ReaderBehavior.FormatBehaviorKind == ODataBehaviorKind.Default;
            }
        }

        /// <summary>
        /// true if the WCF DS client compatibility API behavior should be used; otherwise false.
        /// </summary>
        protected internal bool UseClientApiBehavior
        {
            get
            {
                return this.messageReaderSettings.ReaderBehavior.ApiBehaviorKind == ODataBehaviorKind.WcfDataServicesClient;
            }
        }

        /// <summary>
        /// true if the WCF DS server compatibility API behavior should be used; otherwise false.
        /// </summary>
        protected internal bool UseServerApiBehavior
        {
            get
            {
                return this.messageReaderSettings.ReaderBehavior.ApiBehaviorKind == ODataBehaviorKind.WcfDataServicesServer;
            }
        }

        /// <summary>
        /// true if the default API behavior should be used; otherwise false.
        /// </summary>
        protected internal bool UseDefaultApiBehavior
        {
            get
            {
                return this.messageReaderSettings.ReaderBehavior.ApiBehaviorKind == ODataBehaviorKind.Default;
            }
        }

        /// <summary>
        /// IDisposable.Dispose() implementation to cleanup unmanaged resources of the context.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates an instance of the input context for the specified format.
        /// </summary>
        /// <param name="format">The format to create the context for.</param>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <returns>The newly created input context.</returns>
        internal static ODataInputContext CreateInputContext(
            ODataFormat format,
            ODataPayloadKind readerPayloadKind,
            ODataMessage message,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();

            if (format == ODataFormat.Atom)
            {
                return ODataAtomInputContext.Create(
                    format,
                    message,
                    encoding,
                    messageReaderSettings,
                    version,
                    readingResponse,
                    model,
                    urlResolver);
            }

            if (format == ODataFormat.VerboseJson)
            {
                return ODataJsonInputContext.Create(
                    format,
                    message,
                    encoding,
                    messageReaderSettings,
                    version,
                    readingResponse,
                    model,
                    urlResolver);
            }

            if (format == ODataFormat.Metadata)
            {
                return ODataMetadataInputContext.Create(
                    format,
                    message,
                    encoding,
                    messageReaderSettings,
                    version,
                    readingResponse,
                    model,
                    urlResolver);
            }
            
            if (format == ODataFormat.Batch || format == ODataFormat.RawValue)
            {
                return ODataRawInputContext.Create(
                    format,
                    message,
                    encoding,
                    messageReaderSettings,
                    version,
                    readingResponse,
                    model,
                    urlResolver,
                    readerPayloadKind);
            }

            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataInputContext_CreateInputContext_UnrecognizedFormat));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an instance of the input context for the specified format.
        /// </summary>
        /// <param name="format">The format to create the context for.</param>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <returns>Task which when completed returned the newly created input context.</returns>
        internal static Task<ODataInputContext> CreateInputContextAsync(
            ODataFormat format,
            ODataPayloadKind readerPayloadKind,
            ODataMessage message,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(format != ODataFormat.Metadata, "Async reading of metadata documents is not supported.");
            Debug.Assert(readerPayloadKind != ODataPayloadKind.MetadataDocument, "Async reading of metadata documents is not supported.");

            if (format == ODataFormat.Atom)
            {
                return ODataAtomInputContext.CreateAsync(
                    format,
                    message,
                    encoding,
                    messageReaderSettings,
                    version,
                    readingResponse,
                    model,
                    urlResolver);
            }

            if (format == ODataFormat.VerboseJson)
            {
                return ODataJsonInputContext.CreateAsync(
                    format,
                    message,
                    encoding,
                    messageReaderSettings,
                    version,
                    readingResponse,
                    model,
                    urlResolver);
            }

            if (format == ODataFormat.Batch || format == ODataFormat.RawValue)
            {
                return ODataRawInputContext.CreateAsync(
                    format,
                    message,
                    encoding,
                    messageReaderSettings,
                    version,
                    readingResponse,
                    model,
                    urlResolver,
                    readerPayloadKind);
            }

            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataInputContext_CreateInputContext_UnrecognizedFormat));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal virtual ODataReader CreateFeedReader(IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Feed);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal virtual Task<ODataReader> CreateFeedReaderAsync(IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Feed);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal virtual ODataReader CreateEntryReader(IEdmEntityType expectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Entry);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal virtual Task<ODataReader> CreateEntryReaderAsync(IEdmEntityType expectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Entry);
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>The newly created <see cref="ODataCollectionReader"/>.</returns>
        internal virtual ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataCollectionReader"/>.</returns>
        internal virtual Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <returns>The newly created <see cref="ODataBatchReader"/>.</returns>
        /// <remarks>
        /// Since we don't want to support batch format extensibility (at least not yet) this method should remain internal.
        /// </remarks>
        internal virtual ODataBatchReader CreateBatchReader(string batchBoundary)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataBatchReader"/>.</returns>
        /// <remarks>
        /// Since we don't want to support batch format extensibility (at least not yet) this method should remain internal.
        /// </remarks>
        internal virtual Task<ODataBatchReader> CreateBatchReaderAsync(string batchBoundary)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        internal virtual ODataParameterReader CreateParameterReader(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataParameterReader"/>.</returns>
        internal virtual Task<ODataParameterReader> CreateParameterReaderAsync(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);
        }
#endif

        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal virtual ODataWorkspace ReadServiceDocument()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal virtual Task<ODataWorkspace> ReadServiceDocumentAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);
        }
#endif

        /// <summary>
        /// Read a metadata document. 
        /// This method reads the metadata document from the input and returns 
        /// an <see cref="IEdmModel"/> that represents the read metadata document.
        /// </summary>
        /// <returns>An <see cref="IEdmModel"/> representing the read metadata document.</returns>
        internal virtual IEdmModel ReadMetadataDocument()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.MetadataDocument);
        }

        /// <summary>
        /// Read the property from the input and 
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal virtual ODataProperty ReadProperty(IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read the property from the input and 
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>Task which when completed returns an <see cref="ODataProperty"/> representing the read property.</returns>
        internal virtual Task<ODataProperty> ReadPropertyAsync(IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);
        }
#endif

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        internal virtual ODataError ReadError()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataError"/> representing the read error.</returns>
        internal virtual Task<ODataError> ReadErrorAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal virtual ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal virtual Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);
        }
#endif

        /// <summary>
        /// Read a top-level entity reference link.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal virtual ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level entity reference link.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal virtual Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);
        }
#endif

        /// <summary>
        /// Read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>An <see cref="object"/> representing the read value.</returns>
        internal virtual object ReadValue(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>Task which when completed returns an <see cref="object"/> representing the read value.</returns>
        internal virtual Task<object> ReadValueAsync(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);
        }
#endif

        /// <summary>
        /// Check if the object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the object has already been disposed.</exception>
        internal void VerifyNotDisposed()
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Asserts that the input context was created for synchronous operation.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal void AssertSynchronous()
        {
            DebugUtils.CheckNoExternalCallers();

#if DEBUG
            Debug.Assert(this.synchronous, "The method should only be called on a synchronous input context.");
#endif
        }

        /// <summary>
        /// Asserts that the input context was created for asynchronous operation.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal void AssertAsynchronous()
        {
            DebugUtils.CheckNoExternalCallers();

#if DEBUG
            Debug.Assert(!this.synchronous, "The method should only be called on an asynchronous input context.");
#endif
        }

        /// <summary>
        /// Creates a new instance of a duplicate property names checker.
        /// </summary>
        /// <returns>The newly created instance of duplicate property names checker.</returns>
        internal DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker()
        {
            DebugUtils.CheckNoExternalCallers();

            return new DuplicatePropertyNamesChecker(this.MessageReaderSettings.ReaderBehavior.AllowDuplicatePropertyNames, this.ReadingResponse);
        }

        /// <summary>
        /// Disposes the input context.
        /// </summary>
        protected abstract void DisposeImplementation();

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        private void Dispose(bool disposing)
        {
            this.disposed = true;
            if (disposing)
            {
                this.DisposeImplementation();
            }
        }

        /// <summary>
        /// Creates an exception which reports that the specified payload kind if not support by this format.
        /// </summary>
        /// <param name="payloadKind">The payload kind which is not supported.</param>
        /// <returns>An exception to throw.</returns>
        private ODataException CreatePayloadKindNotSupportedException(ODataPayloadKind payloadKind)
        {
            return new ODataException(Strings.ODataInputContext_UnsupportedPayloadKindForFormat(this.format.ToString(), payloadKind.ToString()));
        }
    }
}
