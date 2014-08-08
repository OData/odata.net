//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the OData input for ATOM OData format.
    /// </summary>
    internal sealed class ODataAtomInputContext : ODataInputContext
    {
        /// <summary>The XML reader used to parse the input.</summary>
        /// <remarks>Do not use this to actually read the input, instead use the xmlReader.</remarks>
        private XmlReader baseXmlReader;

        /// <summary>The XML reader to read from.</summary>
        /// <remarks>If entry XML customization is used this is the reader for the current entry.</remarks>
        private BufferingXmlReader xmlReader;

        /// <summary>A stack used to track XML customization readers.</summary>
        /// <remarks>
        /// At the beginning the base reader is pushed to the stack.
        /// Each non-null entry has an item on this stack.
        /// If the XML customization was used for a given entry the reader returned by the customization will be pushed to the stack for it.
        /// This is only used from ODataAtomReader, other readers don't use this.
        /// </remarks>
        private Stack<BufferingXmlReader> xmlCustomizationReaders;

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
        internal ODataAtomInputContext(
            ODataFormat format,
            Stream messageStream,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : base(format, messageReaderSettings, version, readingResponse, synchronous, model, urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageStream != null, "stream != null");

            try
            {
                ExceptionUtils.CheckArgumentNotNull(format, "format");
                ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

                this.baseXmlReader = ODataAtomReaderUtils.CreateXmlReader(messageStream, encoding, messageReaderSettings);

                // For WCF DS Server behavior we need to turn off xml:base processing for V1/V2 back compat.
                this.xmlReader = new BufferingXmlReader(
                    this.baseXmlReader,
                    /*parentXmlReader*/ null,
                    messageReaderSettings.BaseUri,
                    /*disableXmlBase*/ this.UseServerFormatBehavior && this.Version < ODataVersion.V3,
                    messageReaderSettings.MessageQuotas.MaxNestingDepth,
                    messageReaderSettings.ReaderBehavior.ODataNamespace);
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
        /// Returns the <see cref="BufferingXmlReader"/> which is to be used to read the content of the message.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Not yet implemented.")]
        internal BufferingXmlReader XmlReader
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.xmlReader != null, "Trying to get XmlReader while none is available.");
                return this.xmlReader;
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base type for the entries in the feed.</param>
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
        /// <param name="expectedBaseEntityType">The expected base type for the entries in the feed.</param>
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
            return TaskUtils.GetTaskForSynchronousOperation(() => this.ReadServiceDocumentImplementation());
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
        /// <param name="property">The <see cref="IEdmProperty"/> or <see cref="IEdmFunctionImport"/> producing the property to be read.</param>
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
            return TaskUtils.GetCompletedTask(this.ReadEntityReferenceLinksImplementation());
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
        /// Detects the payload kind(s) of the payload.
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero or more payload kinds depending on what payload kinds were detected.</returns>
        internal IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataPayloadKindDetectionInfo detectionInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(detectionInfo != null, "detectionInfo != null");

            ODataAtomPayloadKindDetectionDeserializer payloadKindDetectionDeserializer = new ODataAtomPayloadKindDetectionDeserializer(this);
            return payloadKindDetectionDeserializer.DetectPayloadKind(detectionInfo);
        }

        /// <summary>
        /// Initializes the ability to use customization readers.
        /// </summary>
        /// <remarks>
        /// This needs to be called before any of the reader customization functionality is used.
        /// </remarks>
        internal void InitializeReaderCustomization()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.xmlReader != null, "There must always be an active reader.");

            this.xmlCustomizationReaders = new Stack<BufferingXmlReader>();
            this.xmlCustomizationReaders.Push(this.xmlReader);
        }

        /// <summary>
        /// Pushes a reader on the top of the customization stack.
        /// </summary>
        /// <param name="customXmlReader">The reader to push.</param>
        /// <param name="xmlBaseUri">The xml:base URI to use as the base uri for all of the payload read from that reader.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The newly created BufferingXmlReader wraps existing XmlReader which we don't own, so we must not dispose either of them.")]
        internal void PushCustomReader(XmlReader customXmlReader, Uri xmlBaseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.xmlCustomizationReaders != null, "The reader customization was not initialized.");
            Debug.Assert(
                object.ReferenceEquals(this.xmlCustomizationReaders.Peek(), this.xmlReader),
                "The this.xmlReader must always be the same reader as the top of the XML customization stack.");
            Debug.Assert(this.xmlCustomizationReaders.Count > 0, "The root must always be on the stack.");

            if (!object.ReferenceEquals(this.xmlReader, customXmlReader))
            {
                Debug.Assert(!this.UseServerFormatBehavior, "Xml reader customizations are not supported on the server.");

                BufferingXmlReader bufferingCustomXmlReader = new BufferingXmlReader(
                    customXmlReader,
                    xmlBaseUri,
                    this.MessageReaderSettings.BaseUri,
                    /*disableXmlBase*/ false,
                    this.MessageReaderSettings.MessageQuotas.MaxNestingDepth,
                    this.MessageReaderSettings.ReaderBehavior.ODataNamespace);
                this.xmlCustomizationReaders.Push(bufferingCustomXmlReader);
                this.xmlReader = bufferingCustomXmlReader;
            }
            else
            {
                // If it's the exact same reader, just push it to the stack so that we know what happened when we pop it.
                this.xmlCustomizationReaders.Push(this.xmlReader);
            }
        }

        /// <summary>
        /// Pops a reader from the top of the customization stack.
        /// </summary>
        /// <returns>The popped reader, the one which was on the top of the stack before the operation.</returns>
        internal BufferingXmlReader PopCustomReader()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.xmlCustomizationReaders != null, "The reader customization was not initialized.");
            Debug.Assert(
                object.ReferenceEquals(this.xmlCustomizationReaders.Peek(), this.xmlReader),
                "The this.xmlReader must always be the same readers as the top of the XML customization stack.");
            Debug.Assert(this.xmlCustomizationReaders.Count > 1, "To Pop we need the root and the reader to pop on the stack.");

            BufferingXmlReader bufferingCustomXmlReader = this.xmlCustomizationReaders.Pop();
            this.xmlReader = this.xmlCustomizationReaders.Peek();
            return bufferingCustomXmlReader;
        }

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
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base type for the entries in the feed.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateFeedReaderImplementation(IEdmEntitySet entitySet, IEdmEntityType expectedBaseEntityType)
        {
            return new ODataAtomReader(this, entitySet, expectedBaseEntityType, true);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateEntryReaderImplementation(IEdmEntitySet entitySet, IEdmEntityType expectedEntityType)
        {
            return new ODataAtomReader(this, entitySet, expectedEntityType, false);
        }

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Newly create <see cref="ODataCollectionReader"/>.</returns>
        private ODataCollectionReader CreateCollectionReaderImplementation(IEdmTypeReference expectedItemTypeReference)
        {
            return new ODataAtomCollectionReader(this, expectedItemTypeReference);
        }

        /// <summary>
        /// This method creates and reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        private ODataProperty ReadPropertyImplementation(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            ODataAtomPropertyAndValueDeserializer atomPropertyAndValueDeserializer = new ODataAtomPropertyAndValueDeserializer(this);
            return atomPropertyAndValueDeserializer.ReadTopLevelProperty(property, expectedPropertyTypeReference);
        }

        /// <summary>
        /// This methods creates and reads a service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> representing the service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the service document.</returns>
        private ODataWorkspace ReadServiceDocumentImplementation()
        {
            ODataAtomServiceDocumentDeserializer atomServiceDocumentDeserializer = new ODataAtomServiceDocumentDeserializer(this);
            return atomServiceDocumentDeserializer.ReadServiceDocument();
        }

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        private ODataError ReadErrorImplementation()
        {
            ODataAtomErrorDeserializer atomErrorDeserializer = new ODataAtomErrorDeserializer(this);
            return atomErrorDeserializer.ReadTopLevelError();
        }

        /// <summary>
        /// Reads top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        private ODataEntityReferenceLinks ReadEntityReferenceLinksImplementation()
        {
            ODataAtomEntityReferenceLinkDeserializer atomEntityReferenceLinkDeserializer = new ODataAtomEntityReferenceLinkDeserializer(this);
            return atomEntityReferenceLinkDeserializer.ReadEntityReferenceLinks();
        }

        /// <summary>
        /// Reads a top-level entity reference link.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        private ODataEntityReferenceLink ReadEntityReferenceLinkImplementation()
        {
            ODataAtomEntityReferenceLinkDeserializer atomEntityReferenceLinkDeserializer = new ODataAtomEntityReferenceLinkDeserializer(this);
            return atomEntityReferenceLinkDeserializer.ReadEntityReferenceLink();
        }
    }
}
