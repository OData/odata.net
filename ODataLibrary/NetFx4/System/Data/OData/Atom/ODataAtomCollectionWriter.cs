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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Data.OData.Atom;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using System.Xml;
    #endregion Namespaces.

    /// <summary>
    /// ODataCollectionWriter for the ATOM format.
    /// </summary>
    internal class ODataAtomCollectionWriter : ODataCollectionWriterCore
    {
        /// <summary>Atom xml writer.</summary>
        private XmlWriter writer;

        /// <summary>The async output stream underlying the Xml writer</summary>
        private AsyncBufferedStream outputStream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="writerSettings">Configuration settings for the writer to create.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        internal ODataAtomCollectionWriter(
            Stream stream, 
            ODataWriterSettings writerSettings, 
            Encoding encoding, 
            DataServiceMetadataProviderWrapper metadataProvider,
            bool synchronous)
            : base(writerSettings.Version, metadataProvider, synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(writerSettings != null, "writerSettings != null");

            this.outputStream = new AsyncBufferedStream(stream);
            this.writer = ODataAtomWriterUtils.CreateXmlWriter(this.outputStream, writerSettings, encoding);
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        protected sealed override void FlushSynchronously()
        {
            this.writer.Flush();
            this.outputStream.FlushSync();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        protected sealed override Task FlushAsynchronously()
        {
            this.writer.Flush();
            return this.outputStream.FlushAsync();
        }
#endif

        /// <summary>
        /// Flushes and closes the writer. This method is only called during disposing the ODataCollectionWriter.
        /// </summary>
        /// <param name="discardBufferedData">
        /// If this parameter is true the close of the writer should not throw if some data is still buffered.
        /// If the argument is false the writer is expected to throw if data is still buffered and the writer is closed.
        /// </param>
        protected override void FlushAndCloseWriter(bool discardBufferedData)
        {
            try
            {
                // flush the Xml writer to the underlying stream so we guarantee that there is no data buffered in the Xml writer;
                // the underlying stream verifies that no data is still buffered when disposed below.
                this.writer.Flush();

                if (!IsErrorState(this.State))
                {
                    // XmlWriter.Close() guarantees that well-formed xml is produced by injecting close elements
                    // if any is missing.  In the case of an exception, we want the stream to end at the close
                    // element </m:error>.  So we skip writer.Dispose here if we are in error state.
                    Utils.TryDispose(this.writer);
                }

                if (discardBufferedData)
                {
                    this.outputStream.Clear();
                }

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
            this.writer.WriteStartDocument(true);
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
        /// <param name="collectionName">The name of the collection.</param>
        protected override void StartCollection(string collectionName)
        {
            Debug.Assert(!string.IsNullOrEmpty(collectionName), "!string.IsNullOrEmpty(collectionName)");

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
        protected override void WriteItemImplementation(object item)
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
                    ODataAtomWriterUtils.WriteComplexValue(this.writer, this.MetadataProvider, complexValue, null, false, true, this.Version, null, null);
                }
                else
                {
                    ODataMultiValue multiValue = item as ODataMultiValue;
                    if (multiValue != null)
                    {
                        throw new ODataException(Strings.ODataCollectionWriter_MultiValuesNotSupportedInCollections);
                    }

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
