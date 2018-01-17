//---------------------------------------------------------------------
// <copyright file="ODataMetadataInputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData
{
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
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        public ODataMetadataInputContext(
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
            : base(ODataFormat.Metadata, messageInfo, messageReaderSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");

            try
            {
                // Which encoding do we use when reading XML payloads
                this.baseXmlReader = ODataMetadataReaderUtils.CreateXmlReader(messageInfo.MessageStream, messageInfo.Encoding, messageReaderSettings);

                // We use the buffering reader here only for in-stream error detection (not for buffering).
                this.xmlReader = new BufferingXmlReader(
                    this.baseXmlReader,
                    /*parentXmlReader*/ null,
                    messageReaderSettings.BaseUri,
                    /*disableXmlBase*/ false,
                    messageReaderSettings.MessageQuotas.MaxNestingDepth);
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
        /// Read a metadata document.
        /// This method reads the metadata document from the input and returns
        /// an <see cref="IEdmModel"/> that represents the read metadata document.
        /// </summary>
        /// <param name="getReferencedModelReaderFunc">The function to load referenced model xml. If null, will stop loading the referenced models. Normally it should throw no exception.</param>
        /// <returns>An <see cref="IEdmModel"/> representing the read metadata document.</returns>
        internal override IEdmModel ReadMetadataDocument(Func<Uri, XmlReader> getReferencedModelReaderFunc)
        {
            return this.ReadMetadataDocumentImplementation(getReferencedModelReaderFunc);
        }

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

            base.Dispose(disposing);
        }

        /// <summary>
        /// This methods reads the metadata from the input and returns an <see cref="IEdmModel"/>
        /// representing the read metadata information.
        /// </summary>
        /// <param name="getReferencedModelReaderFunc">The function to load referenced model xml. If null, will stop loading the referenced models. Normally it should throw no exception.</param>
        /// <returns>An <see cref="IEdmModel"/> instance representing the read metadata.</returns>
        private IEdmModel ReadMetadataDocumentImplementation(Func<Uri, XmlReader> getReferencedModelReaderFunc)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            if (!CsdlReader.TryParse(this.xmlReader, getReferencedModelReaderFunc, out model, out errors))
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

            return model;
        }
    }
}
