//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// The ATOM OData format.
    /// </summary>
    internal sealed class ODataAtomFormat : ODataFormat
    {
        /// <summary>
        /// The text representation - the name of the format.
        /// </summary>
        /// <returns>The name of the format.</returns>
        public override string ToString()
        {
            return "Atom";
        }

        /// <summary>
        /// Detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="responseMessage">The response message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>The set of <see cref="ODataPayloadKind"/>s that are supported with the specified payload.</returns>
        internal override IEnumerable<ODataPayloadKind> DetectPayloadKind(
            IODataResponseMessage responseMessage,
            ODataPayloadKindDetectionInfo detectionInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(responseMessage, "responseMessage");
            ExceptionUtils.CheckArgumentNotNull(detectionInfo, "detectionInfo");

            Stream messageStream = ((ODataMessage)responseMessage).GetStream();
            return this.DetectPayloadKindImplementation(messageStream, /*readingResponse*/ true, /*synchronous*/ true, detectionInfo);
        }

        /// <summary>
        /// Detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="requestMessage">The request message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>The set of <see cref="ODataPayloadKind"/>s that are supported with the specified payload.</returns>
        internal override IEnumerable<ODataPayloadKind> DetectPayloadKind(
            IODataRequestMessage requestMessage,
            ODataPayloadKindDetectionInfo detectionInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(requestMessage, "requestMessage");
            ExceptionUtils.CheckArgumentNotNull(detectionInfo, "detectionInfo");

            Stream messageStream = ((ODataMessage)requestMessage).GetStream();
            return this.DetectPayloadKindImplementation(messageStream, /*readingResponse*/ false, /*synchronous*/ true, detectionInfo);
        }

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
        internal override ODataInputContext CreateInputContext(
            ODataPayloadKind readerPayloadKind,
            ODataMessage message,
            MediaType contentType,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            object payloadKindDetectionFormatState)
        {
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

            Stream messageStream = message.GetStream();
            return new ODataAtomInputContext(
                this,
                messageStream,
                encoding,
                messageReaderSettings,
                version,
                readingResponse,
                /*synchronous*/ true,
                model,
                urlResolver);
        }

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
        internal override ODataOutputContext CreateOutputContext(
            ODataMessage message, 
            MediaType mediaType, 
            Encoding encoding,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            IEdmModel model, 
            IODataUrlResolver urlResolver)
        {
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(messageWriterSettings, "messageWriterSettings");

            Stream messageStream = message.GetStream();
            return new ODataAtomOutputContext(
                this,
                messageStream,
                encoding,
                messageWriterSettings,
                writingResponse,
                /*synchronous*/ true,
                model,
                urlResolver);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="responseMessage">The response message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>A task that when completed returns the set of <see cref="ODataPayloadKind"/>s 
        /// that are supported with the specified payload.</returns>
        internal override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
            IODataResponseMessageAsync responseMessage,
            ODataPayloadKindDetectionInfo detectionInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(responseMessage, "responseMessage");
            ExceptionUtils.CheckArgumentNotNull(detectionInfo, "detectionInfo");

            // NOTE: After getting the message stream we already (asynchronously) buffered the whole stream in memory (in the AsyncBufferedStream).
            //       Until we get Task-based async stream APIs and retire the AsyncBufferedStream, we call the synchronous method on the buffered stream.
            return ((ODataMessage)responseMessage).GetStreamAsync()
                .FollowOnSuccessWith(streamTask => this.DetectPayloadKindImplementation(streamTask.Result, /*readingResponse*/ true, /*synchronous*/ false, detectionInfo));
        }

        /// <summary>
        /// Asynchronously detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="requestMessage">The request message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>A task that when completed returns the set of <see cref="ODataPayloadKind"/>s 
        /// that are supported with the specified payload.</returns>
        internal override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
            IODataRequestMessageAsync requestMessage,
            ODataPayloadKindDetectionInfo detectionInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(requestMessage, "requestMessage");
            ExceptionUtils.CheckArgumentNotNull(detectionInfo, "detectionInfo");

            // NOTE: After getting the message stream we already (asynchronously) buffered the whole stream in memory (in the AsyncBufferedStream).
            //       Until we get Task-based async stream APIs and retire the AsyncBufferedStream, we call the synchronous method on the buffered stream.
            return ((ODataMessage)requestMessage).GetStreamAsync()
                .FollowOnSuccessWith(streamTask => this.DetectPayloadKindImplementation(streamTask.Result, /*readingResponse*/ false, /*synchronous*/ false, detectionInfo));
        }

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
        internal override Task<ODataInputContext> CreateInputContextAsync(
            ODataPayloadKind readerPayloadKind,
            ODataMessage message,
            MediaType contentType,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            object payloadKindDetectionFormatState)
        {
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

            return message.GetStreamAsync()
                .FollowOnSuccessWith(
                    (streamTask) => (ODataInputContext)new ODataAtomInputContext(
                        this,
                        streamTask.Result,
                        encoding,
                        messageReaderSettings,
                        version,
                        readingResponse,
                        /*synchronous*/ false,
                        model,
                        urlResolver));
        }

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
        internal override Task<ODataOutputContext> CreateOutputContextAsync(
            ODataMessage message,
            MediaType mediaType,
            Encoding encoding,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            IEdmModel model, 
            IODataUrlResolver urlResolver)
        {
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(messageWriterSettings, "messageWriterSettings");

            return message.GetStreamAsync()
                .FollowOnSuccessWith(
                    (streamTask) => (ODataOutputContext)new ODataAtomOutputContext(
                        this,
                        streamTask.Result,
                        encoding,
                        messageWriterSettings,
                        writingResponse,
                        /*synchronous*/ false,
                        model,
                        urlResolver));
        }
#endif

        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="messageStream">The message stream to read from for payload kind detection.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero or more payload kinds depending on what payload kinds were detected.</returns>
        private IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(
            Stream messageStream,
            bool readingResponse,
            bool synchronous,
            ODataPayloadKindDetectionInfo detectionInfo)
        {
            using (ODataAtomInputContext inputContext = new ODataAtomInputContext(
                this,
                messageStream,
                detectionInfo.GetEncoding(),
                detectionInfo.MessageReaderSettings,
                ODataVersion.V4,    // NOTE: we don't rely on the version for payload kind detection; taking the latest.
                readingResponse,
                synchronous,
                detectionInfo.Model,
                /*urlResolver*/ null))
            {
                return inputContext.DetectPayloadKind(detectionInfo);
            }
        }
    }
}
