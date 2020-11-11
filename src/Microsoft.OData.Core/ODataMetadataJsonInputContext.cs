//---------------------------------------------------------------------
// <copyright file="ODataMetadataJsonInputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData
{
    /// <summary>
    /// Implementation of the OData input for JSON metadata documents.
    /// </summary>
    internal sealed class ODataMetadataJsonInputContext : ODataInputContext
    {
        /// <summary>The stream to read from.</summary>
        private Stream messageStream;

        /// <summary>Constructor.</summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        public ODataMetadataJsonInputContext(
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
            : base(ODataFormat.Metadata, messageInfo, messageReaderSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");
            messageStream = messageInfo.MessageStream;
        }

        /// <summary>
        /// Read a metadata document.
        /// This method reads the metadata document from the input and returns
        /// an <see cref="IEdmModel"/> that represents the read metadata document.
        /// </summary>
        /// <returns>An <see cref="IEdmModel"/> representing the read metadata document.</returns>
        internal override IEdmModel ReadMetadataDocument()
        {
            return ReadMetadataDocument(csdlReaderSettings: null);
        }

        /// <summary>
        /// Read a metadata document.
        /// This method reads the metadata document from the input and returns
        /// an <see cref="IEdmModel"/> that represents the read metadata document.
        /// </summary>
        /// <param name="csdlReaderSettings">The given CSDL reader settings.</param>
        /// <returns>An <see cref="IEdmModel"/> representing the read metadata document.</returns>
        internal override IEdmModel ReadMetadataDocument(CsdlReaderSettingsBase csdlReaderSettings)
        {
            // Be noted: If the input setting is not JSON CSDL setting, let's use the default setting.
            CsdlJsonReaderSettings settings = csdlReaderSettings as CsdlJsonReaderSettings;
            CsdlJsonReaderSettings setting = settings ?? CsdlJsonReaderSettings.Default;

            // We can't use stream.Read(Span<byte> buffer), this method is introduced since .NET Core 2.1. :(
            byte[] bytes = this.messageStream.ReadAllBytes();

            ReadOnlySpan<byte> jsonReadOnlySpan = new ReadOnlySpan<byte>(bytes);

            Utf8JsonReader jsonReader = new Utf8JsonReader(jsonReadOnlySpan);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            if (!CsdlReader.TryParse(ref jsonReader, setting, out model, out errors))
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
                    this.messageStream.Dispose();
                }
                finally
                {
                    this.messageStream = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
#endif