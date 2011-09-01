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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Xml;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Csdl;
    using Microsoft.Data.Edm.Validation;
    using Microsoft.Data.OData.Atom;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the OData input for metadata documents.
    /// </summary>
    internal sealed class ODataMetadataInputContext : ODataInputContext
    {
        /// <summary>The XML reader used to parse the input.</summary>
        /// <remarks>Do not use this to actually read the input, instead use the xmlReader.</remarks>
        private XmlReader baseXmlReader;

        /// <summary>The XML reader to read from.</summary>
        private BufferingXmlReader xmlReader;

        /// <summary>Constructor.</summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="encoding">The encoding to use to read the input.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "encoding", Justification = "Not yet implemented")]
        private ODataMetadataInputContext(
            Stream stream,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : base(messageReaderSettings, version, readingResponse, synchronous, model, urlResolver)
        {
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            this.baseXmlReader = ODataAtomReaderUtils.CreateXmlReader(stream, messageReaderSettings);

            // We use the buffering reader here only for in-stream error detection (not for buffering).
            this.xmlReader = new BufferingXmlReader(this.baseXmlReader, false);
        }

        /// <summary>
        /// Create a metadata input context.
        /// </summary>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <returns>The newly created input context.</returns>
        internal static ODataInputContext Create(
            ODataMessage message,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            Stream messageStream = ODataInputContext.GetMessageStream(message, messageReaderSettings.DisableMessageStreamDisposal);
            return new ODataMetadataInputContext(
                messageStream,
                encoding,
                messageReaderSettings,
                version,
                readingResponse,
                true,
                model,
                urlResolver);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base type for the entities in the feed.</param>
        /// <returns>The newly created reader given the input to read from.</returns>
        internal override ODataReader CreateFeedReader(IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_CreateFeedReader));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base type for the entities in the feed.</param>
        /// <returns>Task which when completed returns the newly created reader given the input to read from.</returns>
        internal override Task<ODataReader> CreateFeedReaderAsync(IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_CreateFeedReader));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_CreateEntryReader));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_CreateEntryReader));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_CreateCollectionReader));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_CreateCollectionReader));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_CreateBatchReader));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_CreateBatchReader));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadServiceDocument));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadServiceDocument));
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
            return this.ReadMetadataDocumentImplementation();
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadProperty));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadProperty));
        }
#endif

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        internal override ODataError ReadError()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadError));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataError"/> representing the read error.</returns>
        internal override Task<ODataError> ReadErrorAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadError));
        }
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadEntityReferenceLinks));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadEntityReferenceLinks));
        }
#endif

        /// <summary>
        /// Reads a top-level entity reference link.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadEntityReferenceLink));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level entity reference link.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadEntityReferenceLink));
        }
#endif

        /// <summary>
        /// Read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>An <see cref="object"/> representing the read value.</returns>
        internal override object ReadValue(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadValue));
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
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataInputContext_ReadValue));
        }
#endif

        /// <summary>
        /// Disposes the input context.
        /// </summary>
        protected override void DisposeImplementation()
        {
            try
            {
                if (this.baseXmlReader != null)
                {
                    ((IDisposable)this.baseXmlReader).Dispose();
                }
            }
            finally
            {
                this.baseXmlReader = null;
                this.xmlReader = null;
            }
        }

        /// <summary>
        /// This methods reads the metadata from the input and returns an <see cref="IEdmModel"/>
        /// representing the read metadata information.
        /// </summary>
        /// <returns>An <see cref="IEdmModel"/> instance representing the read metadata.</returns>
        private IEdmModel ReadMetadataDocumentImplementation()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            if (!EdmxReader.TryParse(this.xmlReader, true, out model, out errors))
            {
                Debug.Assert(errors != null, "errors != null");

                StringBuilder builder = new StringBuilder();
                foreach (EdmError error in errors)
                {
                    builder.AppendLine(error.ToString());
                }

                throw new ODataException(Strings.ODataMetadataInputContext_ErrorReadingMetadata(builder.ToString()));
            }

            Debug.Assert(model != null, "model != null");
            ValidationUtils.ValidateModel(model);

            // Load all the OData annotations, i.e., translate them from their serializable form into the more usable in-memory form.
            model.LoadODataAnnotations();

            return model;
        }
    }
}
