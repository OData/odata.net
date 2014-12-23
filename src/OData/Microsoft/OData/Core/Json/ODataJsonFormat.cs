//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.Json
{
    #region Namespaces
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// The JsonLight OData format.
    /// </summary>
    internal sealed class ODataJsonFormat : ODataFormat
    {
        /// <summary>
        /// The text representation - the name of the format.
        /// </summary>
        /// <returns>The name of the format.</returns>
        public override string ToString()
        {
            return "JsonLight";
        }

        /// <summary>
        /// Detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="settings">Configuration settings of the OData reader.</param>
        /// <returns>The set of <see cref="ODataPayloadKind"/>s that are supported with the specified payload.</returns>
        public override IEnumerable<ODataPayloadKind> DetectPayloadKind(
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings settings)
        {
            ExceptionUtils.CheckArgumentNotNull(messageInfo, "messageInfo");
            return this.DetectPayloadKindImplementation(
                    messageInfo.GetMessageStream(),
                    messageInfo.IsResponse,
                    new ODataPayloadKindDetectionInfo(
                        messageInfo.MediaType,
                        messageInfo.Encoding,
                        settings,
                        messageInfo.Model));
        }

        /// <summary>
        /// Creates an instance of the input context for this format.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <returns>The newly created input context.</returns>
        public override ODataInputContext CreateInputContext(
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
        {
            ExceptionUtils.CheckArgumentNotNull(messageInfo, "messageInfo");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

            return new ODataJsonLightInputContext(
                this,
                messageInfo.GetMessageStream(),
                messageInfo.MediaType,
                messageInfo.Encoding,
                messageReaderSettings,
                messageInfo.IsResponse,
                /*synchronous*/ true,
                messageInfo.Model,
                messageInfo.UrlResolver);
        }

        /// <summary>
        /// Creates an instance of the output context for this format.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <returns>The newly created output context.</returns>
        public override ODataOutputContext CreateOutputContext(
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
        {
            ExceptionUtils.CheckArgumentNotNull(messageInfo, "messageInfo");
            ExceptionUtils.CheckArgumentNotNull(messageWriterSettings, "messageWriterSettings");

            return new ODataJsonLightOutputContext(
                this,
                messageInfo.GetMessageStream(),
                messageInfo.MediaType,
                messageInfo.Encoding,
                messageWriterSettings,
                messageInfo.IsResponse,
                /*synchronous*/ true,
                messageInfo.Model,
                messageInfo.UrlResolver);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="settings">Configuration settings of the OData reader.</param>
        /// <returns>A task that when completed returns the set of <see cref="ODataPayloadKind"/>s 
        /// that are supported with the specified payload.</returns>
        public override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings settings)
        {
            ExceptionUtils.CheckArgumentNotNull(messageInfo, "messageInfo");
            return messageInfo.GetMessageStreamAsync()
                 .FollowOnSuccessWithTask(streamTask => this.DetectPayloadKindImplementationAsync(
                    streamTask.Result,
                     /*readingResponse*/ messageInfo.IsResponse,
                    new ODataPayloadKindDetectionInfo(
                        messageInfo.MediaType,
                        messageInfo.Encoding,
                        settings,
                        messageInfo.Model)));
        }

        /// <summary>
        /// Asynchronously creates an instance of the input context for this format.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <returns>Task which when completed returned the newly created input context.</returns>
        public override Task<ODataInputContext> CreateInputContextAsync(
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
        {
            ExceptionUtils.CheckArgumentNotNull(messageInfo, "messageInfo");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

            return messageInfo.GetMessageStreamAsync()
                .FollowOnSuccessWith(
                    (streamTask) => (ODataInputContext)new ODataJsonLightInputContext(
                        this,
                        streamTask.Result,
                        messageInfo.MediaType,
                        messageInfo.Encoding,
                        messageReaderSettings,
                        messageInfo.IsResponse,
                        /*synchronous*/ false,
                        messageInfo.Model,
                        messageInfo.UrlResolver));
        }

        /// <summary>
        /// Creates an instance of the output context for this format.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <returns>Task which represents the pending create operation.</returns>
        public override Task<ODataOutputContext> CreateOutputContextAsync(
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
        {
            ExceptionUtils.CheckArgumentNotNull(messageInfo, "messageInfo");
            ExceptionUtils.CheckArgumentNotNull(messageWriterSettings, "messageWriterSettings");

            return messageInfo.GetMessageStreamAsync()
                .FollowOnSuccessWith(
                    (streamTask) => (ODataOutputContext)new ODataJsonLightOutputContext(
                        this,
                        streamTask.Result,
                        messageInfo.MediaType,
                        messageInfo.Encoding,
                        messageWriterSettings,
                        messageInfo.IsResponse,
                        /*synchronous*/ false,
                        messageInfo.Model,
                        messageInfo.UrlResolver));
        }
#endif

        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="messageStream">The message stream to read from for payload kind detection.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        private IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(
            Stream messageStream,
            bool readingResponse,
            ODataPayloadKindDetectionInfo detectionInfo)
        {
            using (ODataJsonLightInputContext jsonLightInputContext = new ODataJsonLightInputContext(
                this,
                messageStream,
                detectionInfo.ContentType,
                detectionInfo.GetEncoding(),
                detectionInfo.MessageReaderSettings,
                readingResponse,
                /*synchronous*/ true,
                detectionInfo.Model,
                /*urlResolver*/ null))
            {
                return jsonLightInputContext.DetectPayloadKind(detectionInfo);
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="messageStream">The message stream to read from for payload kind detection.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        private Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindImplementationAsync(
            Stream messageStream,
            bool readingResponse,
            ODataPayloadKindDetectionInfo detectionInfo)
        {
            ODataJsonLightInputContext jsonLightInputContext = new ODataJsonLightInputContext(
                this,
                messageStream,
                detectionInfo.ContentType,
                detectionInfo.GetEncoding(),
                detectionInfo.MessageReaderSettings,
                readingResponse,
                /*synchronous*/ false,
                detectionInfo.Model,
                /*urlResolver*/ null);

            return jsonLightInputContext.DetectPayloadKindAsync(detectionInfo)
                .FollowAlwaysWith(t =>
                    {
                        jsonLightInputContext.Dispose();
                    });
        }
#endif
    }
}
