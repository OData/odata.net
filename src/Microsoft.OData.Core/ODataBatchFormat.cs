//---------------------------------------------------------------------
// <copyright file="ODataBatchFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
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
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// The $batch OData format.
    /// </summary>
    internal sealed class ODataBatchFormat : ODataFormat
    {
        /// <summary>
        /// The text representation - the name of the format.
        /// </summary>
        /// <returns>The name of the format.</returns>
        public override string ToString()
        {
            return "Batch";
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
            return DetectPayloadKindImplementation(messageInfo.MediaType);
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

            return new ODataRawInputContext(
                this,
                messageInfo.GetMessageStream(),
                messageInfo.Encoding,
                messageReaderSettings,
                messageInfo.IsResponse,
                /*synchronous*/ true,
                messageInfo.Model,
                messageInfo.UrlResolver,
                messageInfo.PayloadKind);
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

            return new ODataRawOutputContext(
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
            return TaskUtils.GetTaskForSynchronousOperation(() => DetectPayloadKindImplementation(messageInfo.MediaType));
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
                    (streamTask) => (ODataInputContext)new ODataRawInputContext(
                        this,
                        streamTask.Result,
                        messageInfo.Encoding,
                        messageReaderSettings,
                        messageInfo.IsResponse,
                        /*synchronous*/ false,
                        messageInfo.Model,
                        messageInfo.UrlResolver,
                        messageInfo.PayloadKind));
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
                    (streamTask) => (ODataOutputContext)new ODataRawOutputContext(
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
        /// <param name="contentType">The content type of the message.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        private static IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(ODataMediaType contentType)
        {
            IEnumerable<ODataPayloadKind> batchKinds = new ODataPayloadKind[] { ODataPayloadKind.Batch };

            // NOTE: for batch payloads we only use the content type header of the message to detect the payload kind.
            // Batch payload is valid when either of the followings:
            // a). The content type is multipart/mixed and a boundary parameter exists;
            // b). The content type is application/json.
            if (IsMultipartMixed(contentType) || IsApplicationJson(contentType))
            {
                return batchKinds;
            }

            return Enumerable.Empty<ODataPayloadKind>();
        }

        private static bool IsMultipartMixed(ODataMediaType contentType)
        {
            return HttpUtils.CompareMediaTypeNames(MimeConstants.MimeMultipartType, contentType.Type) &&
                    HttpUtils.CompareMediaTypeNames(MimeConstants.MimeMixedSubType, contentType.SubType) &&
                    contentType.Parameters != null &&
                    contentType.Parameters.Any(
                        kvp => HttpUtils.CompareMediaTypeParameterNames(ODataConstants.HttpMultipartBoundary, kvp.Key));
        }

        private static bool IsApplicationJson(ODataMediaType contentType)
        {
            return HttpUtils.CompareMediaTypeNames(contentType.Type, MimeConstants.MimeApplicationType) &&
                   HttpUtils.CompareMediaTypeNames(contentType.SubType, MimeConstants.MimeJsonSubType);
        }
    }
}
