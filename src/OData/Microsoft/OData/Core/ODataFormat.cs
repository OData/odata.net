//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Core.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Representation of an OData format.
    /// </summary>
    public abstract class ODataFormat
    {
        /// <summary>The ATOM format instance.</summary>
        private static ODataAtomFormat atomFormat = new ODataAtomFormat();

        /// <summary>The JSON Light format instance.</summary>
        private static ODataJsonLightFormat jsonLightFormat = new ODataJsonLightFormat();

        /// <summary>The RAW format instance.</summary>
        private static ODataRawValueFormat rawValueFormat = new ODataRawValueFormat();

        /// <summary>The batch format instance.</summary>
        private static ODataBatchFormat batchFormat = new ODataBatchFormat();

        /// <summary>The metadata format instance.</summary>
        private static ODataMetadataFormat metadataFormat = new ODataMetadataFormat();

        /// <summary>Specifies the ATOM format; we also use this for all Xml based formats (if ATOM can't be used).</summary>
        /// <returns>The ATOM format.</returns>
        [System.Obsolete("ATOM support is obsolete.")]
        public static ODataFormat Atom
        {
            get
            {
                return atomFormat;
            }
        }

        /// <summary>Specifies the JSON format.</summary>
        /// <returns>The JSON format.</returns>
        public static ODataFormat Json
        {
            get
            {
                return jsonLightFormat;
            }
        }

        /// <summary>Specifies the RAW format; used for raw values.</summary>
        /// <returns>The RAW format.</returns>
        public static ODataFormat RawValue
        {
            get
            {
                return rawValueFormat;
            }
        }

        /// <summary>Gets the batch format instance.</summary>
        /// <returns>The batch format instance.</returns>
        public static ODataFormat Batch
        {
            get
            {
                return batchFormat;
            }
        }

        /// <summary>Gets the metadata format instance.</summary>
        /// <returns>The metadata format instance.</returns>
        public static ODataFormat Metadata
        {
            get
            {
                return metadataFormat;
            }
        }

        /// <summary>
        /// Detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="responseMessage">The response message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>The set of <see cref="ODataPayloadKind"/>s that are supported with the specified payload.</returns>
        internal abstract IEnumerable<ODataPayloadKind> DetectPayloadKind(IODataResponseMessage responseMessage, ODataPayloadKindDetectionInfo detectionInfo);

        /// <summary>
        /// Detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="requestMessage">The request message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>The set of <see cref="ODataPayloadKind"/>s that are supported with the specified payload.</returns>
        internal abstract IEnumerable<ODataPayloadKind> DetectPayloadKind(IODataRequestMessage requestMessage, ODataPayloadKindDetectionInfo detectionInfo);

        /// <summary>
        /// Creates an instance of the input context for this format.
        /// </summary>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="contentType">The content type of the message to read.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <param name="payloadKindDetectionFormatState">Format specific state stored during payload kind detection
        /// using the <see cref="ODataPayloadKindDetectionInfo.SetPayloadKindDetectionFormatState"/>.</param>
        /// <returns>The newly created input context.</returns>
        internal abstract ODataInputContext CreateInputContext(
            ODataPayloadKind readerPayloadKind,
            ODataMessage message,
            MediaType contentType,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            object payloadKindDetectionFormatState);

        /// <summary>
        /// Creates an instance of the output context for this format.
        /// </summary>
        /// <param name="message">The message to use.</param>
        /// <param name="mediaType">The specific media type being written.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <returns>The newly created output context.</returns>
        internal abstract ODataOutputContext CreateOutputContext(
            ODataMessage message, 
            MediaType mediaType,
            Encoding encoding, 
            ODataMessageWriterSettings messageWriterSettings, 
            bool writingResponse, 
            IEdmModel model,
            IODataUrlResolver urlResolver);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="responseMessage">The response message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>A task that when completed returns the set of <see cref="ODataPayloadKind"/>s 
        /// that are supported with the specified payload.</returns>
        internal abstract Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(IODataResponseMessageAsync responseMessage, ODataPayloadKindDetectionInfo detectionInfo);

        /// <summary>
        /// Asynchronously detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="requestMessage">The request message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>A task that when completed returns the set of <see cref="ODataPayloadKind"/>s 
        /// that are supported with the specified payload.</returns>
        internal abstract Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(IODataRequestMessageAsync requestMessage, ODataPayloadKindDetectionInfo detectionInfo);

        /// <summary>
        /// Asynchronously creates an instance of the input context for this format.
        /// </summary>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="contentType">The content type of the message to read.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <param name="payloadKindDetectionFormatState">Format specific state stored during payload kind detection
        /// using the <see cref="ODataPayloadKindDetectionInfo.SetPayloadKindDetectionFormatState"/>.</param>
        /// <returns>Task which when completed returned the newly created input context.</returns>
        internal abstract Task<ODataInputContext> CreateInputContextAsync(
            ODataPayloadKind readerPayloadKind,
            ODataMessage message,
            MediaType contentType,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            object payloadKindDetectionFormatState);

        /// <summary>
        /// Creates an instance of the output context for this format.
        /// </summary>
        /// <param name="message">The message to use.</param>
        /// <param name="mediaType">The specific media type being written.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <returns>Task which represents the pending create operation.</returns>
        internal abstract Task<ODataOutputContext> CreateOutputContextAsync(
            ODataMessage message, 
            MediaType mediaType, 
            Encoding encoding, 
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse, 
            IEdmModel model,
            IODataUrlResolver urlResolver);
#endif
    }
}
