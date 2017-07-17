//---------------------------------------------------------------------
// <copyright file="ODataMetadataOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData
{
    /// <summary>
    /// RAW format output context.
    /// </summary>
    internal sealed class ODataMetadataOutputContext : ODataOutputContext
    {
        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

        /// <summary>The XmlWriter to write to.</summary>
        private XmlWriter xmlWriter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        internal ODataMetadataOutputContext(
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
            : base(ODataFormat.Metadata, messageInfo, messageWriterSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");
            Debug.Assert(!messageInfo.IsAsync, "Metadata output context is only supported in synchronous operations.");

            try
            {
                this.messageOutputStream = messageInfo.MessageStream;
                this.xmlWriter = ODataMetadataWriterUtils.CreateXmlWriter(this.messageOutputStream, messageWriterSettings, messageInfo.Encoding);
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the output context.
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    this.messageOutputStream.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// Synchronously flush the writer.
        /// </summary>
        internal void Flush()
        {
            this.AssertSynchronous();

            // XmlWriter.Flush will call the underlying Stream.Flush.
            // In the synchronous case the underlying stream is the message stream itself, which will then Flush as well.
            this.xmlWriter.Flush();
        }

        /// <summary>
        /// Writes an <see cref="ODataError"/> into the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <remarks>
        /// This method is called if the ODataMessageWriter.WriteError is called once some other
        /// write operation has already started.
        /// The method should write the in-stream error representation for the specific format into the current payload.
        /// Before the method is called no flush is performed on the output context or any active writer.
        /// It is the responsibility of this method to flush the output before the method returns.
        /// </remarks>
        internal override void WriteInStreamError(ODataError error, bool includeDebugInformation)
        {
            this.AssertSynchronous();

            ODataMetadataWriterUtils.WriteError(this.xmlWriter, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth);
            this.Flush();
        }

        /// <summary>
        /// Writes the metadata document as the message body.
        /// </summary>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteMetadataDocument()
        {
            this.AssertSynchronous();

            IEnumerable<EdmError> errors;
            if (!CsdlWriter.TryWriteCsdl(this.Model, this.xmlWriter, CsdlTarget.OData, out errors))
            {
                Debug.Assert(errors != null, "errors != null");

                StringBuilder builder = new StringBuilder();
                foreach (EdmError error in errors)
                {
                    builder.AppendLine(error.ToString());
                }

                throw new ODataException(Strings.ODataMetadataOutputContext_ErrorWritingMetadata(builder.ToString()));
            }

            this.Flush();
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.xmlWriter != null)
                {
                    // XmlWriter.Flush will call the underlying Stream.Flush.
                    this.xmlWriter.Flush();

                    // XmlWriter.Dispose calls XmlWriter.Close which writes missing end elements.
                    // Thus we can't dispose the XmlWriter since that might end up writing more data into the stream right here
                    // and thus callers would have no way to prevent us from writing synchronously into the underlying stream.
                    // Also in case of in-stream error we intentionally want to not write the end elements to keep the payload invalid.
                    // In the async case the underlying stream is the async buffered stream, so we have to flush that explicitely.

                    // Dipose the message stream (note that we OWN this stream, so we always dispose it).
                    this.messageOutputStream.Dispose();
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.xmlWriter = null;
            }

            base.Dispose(disposing);
        }
    }
}
