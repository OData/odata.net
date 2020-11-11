//---------------------------------------------------------------------
// <copyright file="ODataMetadataJsonOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData
{
    /// <summary>
    /// JSON Metadata format output context.
    /// </summary>
    internal sealed class ODataMetadataJsonOutputContext : ODataOutputContext
    {
        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

        /// <summary>The jsonWriter to write to.</summary>
        private Utf8JsonWriter jsonWriter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        internal ODataMetadataJsonOutputContext(
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
            : base(ODataFormat.Metadata, messageInfo, messageWriterSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");
            Debug.Assert(!messageInfo.IsAsync, "Metadata output context is only supported in synchronous operations.");

            try
            {
                this.messageOutputStream = messageInfo.MessageStream;
                this.jsonWriter = CreateJsonWriter(this.messageOutputStream, messageWriterSettings);
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

            this.jsonWriter.Flush();
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

            // What error should we write here?

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

            CsdlJsonWriterSettings writerSettings = new CsdlJsonWriterSettings
            {
                IsIeee754Compatible = MessageWriterSettings.IsIeee754Compatible
            };

            if (!CsdlWriter.TryWriteCsdl(this.Model, this.jsonWriter, writerSettings, out errors))
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
        /// Creates a JSON writer over the specified stream, with the provided settings and encoding.
        /// </summary>
        /// <param name="stream">The stream to create the XmlWriter over.</param>
        /// <param name="messageWriterSettings">The OData message writer settings used to control the settings of the Xml writer.</param>
        /// <param name="encoding">The encoding used for writing.</param>
        /// <returns>An <see cref="Utf8JsonWriter"/> instance configured with the provided settings and encoding.</returns>
        internal static Utf8JsonWriter CreateJsonWriter(Stream stream, ODataMessageWriterSettings messageWriterSettings)
        {
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            JsonWriterOptions options = new JsonWriterOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Indented = true, // TODO, do we need a setting in ODataMessageWriterSettings?
                SkipValidation = false // skip the validation
            };

            Utf8JsonWriter jsonWriter = new Utf8JsonWriter(stream, options);
            return jsonWriter;
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.jsonWriter != null)
                {
                    this.jsonWriter.Flush();
                    this.jsonWriter.Dispose();
                    this.messageOutputStream.Dispose();
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.jsonWriter = null;
            }

            base.Dispose(disposing);
        }
    }
}
#endif
