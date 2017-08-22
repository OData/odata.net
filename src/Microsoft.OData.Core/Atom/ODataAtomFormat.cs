//---------------------------------------------------------------------
// <copyright file="ODataAtomFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
                /*readingResponse*/ messageInfo.IsResponse,
                /*synchronous*/ true,
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

            return new ODataAtomInputContext(
                this,
                messageInfo.GetMessageStream(),
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

            return new ODataAtomOutputContext(
                this,
                messageInfo.GetMessageStream(),
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
                 .FollowOnSuccessWith(
                    streamTask => this.DetectPayloadKindImplementation(
                                    streamTask.Result,
                                    /*readingResponse*/ messageInfo.IsResponse,
                                    /*synchronous*/ false,
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
            ExceptionUtils.CheckArgumentNotNull(messageInfo, "message");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

            return messageInfo.GetMessageStreamAsync()
                .FollowOnSuccessWith(
                    (streamTask) => (ODataInputContext)new ODataAtomInputContext(
                        this,
                        streamTask.Result,
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
                    (streamTask) => (ODataOutputContext)new ODataAtomOutputContext(
                        this,
                        streamTask.Result,
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
                readingResponse,
                synchronous,
                detectionInfo.Model,
                /*UrlResolver*/ null))
            {
                return inputContext.DetectPayloadKind(detectionInfo);
            }
        }
    }
}
