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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using System.Xml;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// ODataCollectionWriter for the ATOM format.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:ImplementIDisposable", Justification = "IDisposable is implemented on ODataMessageWriter.")]
    internal sealed class ODataAtomCollectionWriter : ODataCollectionWriterCore
    {
        /// <summary>Atom xml writer.</summary>
        private XmlWriter writer;

        /// <summary>The async output stream underlying the Xml writer</summary>
        private AsyncBufferedStream outputStream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="messageWriterSettings">Configuration settings for the writer to create.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="synchronous">true if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <param name="writingResponse">true if we are writing a response, false if it's a request.</param>
        internal ODataAtomCollectionWriter(
            Stream stream, 
            ODataMessageWriterSettings messageWriterSettings, 
            Encoding encoding, 
            IEdmModel model,
            bool synchronous,
            bool writingResponse)
            : base(messageWriterSettings.Version.Value, writingResponse, messageWriterSettings.WriterBehavior, model, synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            this.outputStream = new AsyncBufferedStream(stream);
            this.writer = ODataAtomWriterUtils.CreateXmlWriter(this.outputStream, messageWriterSettings, encoding);
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.writer.Flush();
            this.outputStream.FlushSync();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        protected override Task FlushAsynchronously()
        {
            this.writer.Flush();
            return this.outputStream.FlushAsync();
        }
#endif

        /// <summary>
        /// Flushes and closes the writer. This method is only called during disposing the ODataCollectionWriter.
        /// </summary>
        protected override void FlushAndCloseWriter()
        {
            try
            {
                // flush the Xml writer to the underlying stream so we guarantee that there is no data buffered in the Xml writer;
                // the underlying stream verifies that no data is still buffered when disposed below.
                this.writer.Flush();

                // XmlWriter.Dispose calls XmlWriter.Close which writes missing end elements.
                // Thus we can't dispose the XmlWriter since that might end up writing more data into the stream right here
                // and thus callers would have no way to prevent us from writing synchronously into the underlying stream.
                // (note that all other cases where we write to the stream can be followed by FlushAsync which will perform
                //  async write to the stream, but Dispose is the only case where that's not true).
                // Also in case of in-stream error we intentionally want to not write the end elements to keep the payload invalid.
                // Always flush the data synchronously before dispose.
                this.outputStream.FlushSync();

                this.outputStream.Dispose();
            }
            finally
            {
                this.writer = null;
                this.outputStream = null;
            }
        }

        /// <summary>
        /// Start writing an OData payload.
        /// </summary>
        protected override void StartPayload()
        {
            this.writer.WriteStartDocument();
        }

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected override void EndPayload()
        {
            // This method is only called if no error has been written so it is safe to
            // call WriteEndDocument() here (since it closes all open elements which we don't want in error state)
            this.writer.WriteEndDocument();
        }

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collection">The <see cref="ODataCollectionResult"/> representing the collection.</param>
        protected override void StartCollection(ODataCollectionResult collection)
        {
            Debug.Assert(collection != null, "collection != null");

            string collectionName = collection.Name;
            if (collectionName == null)
            {
                // null collection names are not allowed in ATOM
                throw new ODataException(Strings.ODataAtomCollectionWriter_CollectionNameMustNotBeNull);
            }

            // TODO: include metadata validation to ensure the collection name is a service operation name

            // <collectionName>
            this.writer.WriteStartElement(collectionName, AtomConstants.ODataNamespace);

            // xmlns:="ODataNamespace"
            this.writer.WriteAttributeString(
                AtomConstants.XmlnsNamespacePrefix,
                AtomConstants.XmlNamespacesNamespace,
                AtomConstants.ODataNamespace);
        }

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        protected override void EndCollection()
        {
            // </collectionName>
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a collection item (either primitive or complex)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        protected override void WriteCollectionItem(object item)
        {
            // <d:element>
            this.writer.WriteStartElement(AtomConstants.ODataCollectionItemElementName, AtomConstants.ODataNamespace);

            if (item == null)
            {
                // NOTE can't use ODataAtomWriterUtils.WriteNullAttribute because that method assumes the
                //      default 'm' prefix for the metadata namespace.
                this.writer.WriteAttributeString(
                    AtomConstants.ODataNullAttributeName,
                    AtomConstants.ODataMetadataNamespace,
                    AtomConstants.AtomTrueLiteral);
            }
            else
            {
                ODataComplexValue complexValue = item as ODataComplexValue;
                if (complexValue != null)
                {
                    ODataAtomWriterUtils.WriteComplexValue(
                        this.writer,
                        this.Model,
                        complexValue,
                        null  /* metadataType */,
                        false /* isOpenPropertyType */,
                        true  /* isWritingCollection */,
                        this.WritingResponse,
                        this.WriterBehavior,
                        null  /* beforePropertiesAction */,
                        null  /* afterPropertiesAction */,
                        this.DuplicatePropertyNamesChecker,
                        null  /* multiValueItemTypeName */,
                        this.Version,
                        null  /* epmValueCache */,
                        null  /* epmSourcePathSegment */,
                        null  /* projectedProperties */);
                    this.DuplicatePropertyNamesChecker.Clear();
                }
                else
                {
                    Debug.Assert(!(item is ODataMultiValue), "!(item is ODataMultiValue)");
                    Debug.Assert(!(item is ODataStreamReferenceValue), "!(item is ODataStreamReferenceValue)");
                    ODataAtomWriterUtils.WritePrimitiveValue(this.writer, item, null);
                }
            }

            // </d:element>
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name='error'>The error to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        protected override void SerializeError(ODataError error, bool includeDebugInformation)
        {
            Debug.Assert(error != null, "error != null");
            ODataAtomWriterUtils.WriteError(this.writer, error, includeDebugInformation);
        }
    }
}
