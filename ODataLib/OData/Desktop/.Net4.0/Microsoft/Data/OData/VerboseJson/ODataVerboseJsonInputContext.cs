//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the OData input for Verbose JSON OData format.
    /// </summary>
    internal sealed class ODataVerboseJsonInputContext : ODataInputContext
    {
        /// <summary>The text reader created for the input stream.</summary>
        /// <remarks>
        /// The ODataJsonInputContext instance owns the textReader instance and thus disposes it. 
        /// We further set this field to null when the input is disposed and use it for checks whether the instance has already been disposed.
        /// </remarks>
        private TextReader textReader;

        /// <summary>The JSON reader to read from.</summary>
        private BufferingJsonReader jsonReader;

        /// <summary>Constructor.</summary>
        /// <param name="format">The format for this input context.</param>
        /// <param name="reader">The reader to read data from.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        internal ODataVerboseJsonInputContext(
            ODataFormat format,
            TextReader reader,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : base(format, messageReaderSettings, version, readingResponse, synchronous, model, urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(reader != null, "reader != null");

            try
            {
                ExceptionUtils.CheckArgumentNotNull(format, "format");
                ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

                this.textReader = reader;
                if (this.UseServerFormatBehavior)
                {
                    this.jsonReader = new PropertyDeduplicatingJsonReader(this.textReader, messageReaderSettings.MessageQuotas.MaxNestingDepth);
                }
                else
                {
                    this.jsonReader = new BufferingJsonReader(this.textReader, JsonConstants.ODataErrorName, messageReaderSettings.MessageQuotas.MaxNestingDepth, ODataFormat.VerboseJson);
                }
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the input context.
                if (ExceptionUtils.IsCatchableExceptionType(e) && reader != null)
                {
                    reader.Dispose();
                }

                throw;
            }
        }

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
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The CreateTestReaderForMessageStreamConstructor returns TextReader which needs to be discposed, but the other constructor which we pass it to takes full ownership of it.")]
        internal ODataVerboseJsonInputContext(
            ODataFormat format,
            Stream messageStream,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : this(
                format,
                CreateTextReaderForMessageStreamConstructor(messageStream, encoding),
                messageReaderSettings,
                version,
                readingResponse,
                synchronous,
                model,
                urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Returns the <see cref="BufferingJsonReader"/> which is to be used to read the content of the message.
        /// </summary>
        internal BufferingJsonReader JsonReader
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.jsonReader != null, "Trying to get JsonReader while none is available.");
                return this.jsonReader;
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal override ODataReader CreateFeedReader(IEdmEntitySet entitySet, IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateFeedReaderImplementation(entitySet, expectedBaseEntityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal override Task<ODataReader> CreateFeedReaderAsync(IEdmEntitySet entitySet, IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateFeedReaderImplementation(entitySet, expectedBaseEntityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal override ODataReader CreateEntryReader(IEdmEntitySet entitySet, IEdmEntityType expectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateEntryReaderImplementation(entitySet, expectedEntityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal override Task<ODataReader> CreateEntryReaderAsync(IEdmEntitySet entitySet, IEdmEntityType expectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateEntryReaderImplementation(entitySet, expectedEntityType));
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
            this.AssertSynchronous();

            return this.CreateCollectionReaderImplementation(expectedItemTypeReference);
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
            this.AssertAsynchronous();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateCollectionReaderImplementation(expectedItemTypeReference));
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        internal override ODataParameterReader CreateParameterReader(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();
            VerifyCanCreateParameterReader(functionImport);

            return this.CreateParameterReaderImplementation(functionImport);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataParameterReader"/>.</returns>
        internal override Task<ODataParameterReader> CreateParameterReaderAsync(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();
            VerifyCanCreateParameterReader(functionImport);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateParameterReaderImplementation(functionImport));
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
            this.AssertSynchronous();

            return this.ReadServiceDocumentImplementation();
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
            this.AssertAsynchronous();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation<ODataWorkspace>(this.ReadServiceDocumentImplementation);
        }
#endif

        /// <summary>
        /// This method creates an reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal override ODataProperty ReadProperty(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.ReadPropertyImplementation(property, expectedPropertyTypeReference);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read the property from the input and 
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>Task which when completed returns an <see cref="ODataProperty"/> representing the read property.</returns>
        internal override Task<ODataProperty> ReadPropertyAsync(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.ReadPropertyImplementation(property, expectedPropertyTypeReference));
        }
#endif

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        internal override ODataError ReadError()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.ReadErrorImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataError"/> representing the read error.</returns>
        internal override Task<ODataError> ReadErrorAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.ReadErrorImplementation());
        }
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override ODataEntityReferenceLinks ReadEntityReferenceLinks(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.ReadEntityReferenceLinksImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.ReadEntityReferenceLinksImplementation());
        }
#endif

        /// <summary>
        /// Reads a top-level entity reference link.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference link.</param>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override ODataEntityReferenceLink ReadEntityReferenceLink(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.ReadEntityReferenceLinkImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level entity reference link.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference link.</param>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetCompletedTask(this.ReadEntityReferenceLinkImplementation());
        }
#endif

        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        internal IEnumerable<ODataPayloadKind> DetectPayloadKind()
        {
            DebugUtils.CheckNoExternalCallers();

            ODataVerboseJsonPayloadKindDetectionDeserializer payloadKindDetectionDeserializer = new ODataVerboseJsonPayloadKindDetectionDeserializer(this);
            return payloadKindDetectionDeserializer.DetectPayloadKind();
        }

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
            }
            finally
            {
                this.textReader = null;
                this.jsonReader = null;
            }
        }

        /// <summary>
        /// Helper method to create a TextReader over the message stream. This is needed by the constructor to dispose the message stream if the creation fails
        /// since this is called from the constructor in place where exception handling is not possible.
        /// </summary>
        /// <param name="messageStream">The stream to read data from.</param>
        /// <param name="encoding">The encoding to use to read the input.</param>
        /// <returns>The newly created text reader.</returns>
        private static TextReader CreateTextReaderForMessageStreamConstructor(Stream messageStream, Encoding encoding)
        {
            Debug.Assert(messageStream != null, "stream != null");

            try
            {
                return new StreamReader(messageStream, encoding);
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
        /// Verifies that CreateParameterReader can be called.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        private static void VerifyCanCreateParameterReader(IEdmFunctionImport functionImport)
        {
            if (functionImport == null)
            {
                throw new ArgumentNullException("functionImport", ODataErrorStrings.ODataJsonInputContext_FunctionImportCannotBeNullForCreateParameterReader("functionImport"));
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateFeedReaderImplementation(IEdmEntitySet entitySet, IEdmEntityType expectedBaseEntityType)
        {
            return new ODataVerboseJsonReader(this, entitySet, expectedBaseEntityType, true, null /*listener*/);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateEntryReaderImplementation(IEdmEntitySet entitySet, IEdmEntityType expectedEntityType)
        {
            return new ODataVerboseJsonReader(this, entitySet, expectedEntityType, false, null /*listener*/);
        }

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Newly create <see cref="ODataCollectionReader"/>.</returns>
        private ODataCollectionReader CreateCollectionReaderImplementation(IEdmTypeReference expectedItemTypeReference)
        {
            return new ODataVerboseJsonCollectionReader(this, expectedItemTypeReference, null /*listener*/);
        }

        /// <summary>
        /// Create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        private ODataParameterReader CreateParameterReaderImplementation(IEdmFunctionImport functionImport)
        {
            return new ODataVerboseJsonParameterReader(this, functionImport);
        }

        /// <summary>
        /// This method creates and reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="Property">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        private ODataProperty ReadPropertyImplementation(IEdmStructuralProperty Property, IEdmTypeReference expectedPropertyTypeReference)
        {
            ODataVerboseJsonPropertyAndValueDeserializer jsonPropertyAndValueDeserializer = new ODataVerboseJsonPropertyAndValueDeserializer(this);
            return jsonPropertyAndValueDeserializer.ReadTopLevelProperty(Property, expectedPropertyTypeReference);
        }

        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        private ODataWorkspace ReadServiceDocumentImplementation()
        {
            ODataVerboseJsonServiceDocumentDeserializer jsonServiceDocumentDeserializer = new ODataVerboseJsonServiceDocumentDeserializer(this);
            return jsonServiceDocumentDeserializer.ReadServiceDocument();
        }

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        private ODataError ReadErrorImplementation()
        {
            ODataVerboseJsonErrorDeserializer jsonErrorDeserializer = new ODataVerboseJsonErrorDeserializer(this);
            return jsonErrorDeserializer.ReadTopLevelError();
        }

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        private ODataEntityReferenceLinks ReadEntityReferenceLinksImplementation()
        {
            ODataVerboseJsonEntityReferenceLinkDeserializer jsonEntityReferenceLinkDeserializer = new ODataVerboseJsonEntityReferenceLinkDeserializer(this);
            return jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinks();
        }
    
        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        private ODataEntityReferenceLink ReadEntityReferenceLinkImplementation()
        {
            ODataVerboseJsonEntityReferenceLinkDeserializer jsonEntityReferenceLinkDeserializer = new ODataVerboseJsonEntityReferenceLinkDeserializer(this);
            return jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLink();
        }
    }
}
