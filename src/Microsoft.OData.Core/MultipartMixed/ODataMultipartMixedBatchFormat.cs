//---------------------------------------------------------------------
// <copyright file="ODataMultipartMixedBatchFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.MultipartMixed
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif

    #endregion Namespaces

    /// <summary>
    /// The $batch OData format.
    /// </summary>
    internal sealed class ODataMultipartMixedBatchFormat : ODataFormat
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


            return new ODataMultipartMixedBatchInputContext(this, messageInfo, messageReaderSettings);
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

            return new ODataMultipartMixedBatchOutputContext(this, messageInfo, messageWriterSettings);
        }

#if PORTABLELIB
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

            return Task.FromResult<ODataInputContext>(
                new ODataMultipartMixedBatchInputContext(this, messageInfo, messageReaderSettings));
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

            return Task.FromResult<ODataOutputContext>(
                new ODataMultipartMixedBatchOutputContext(this, messageInfo, messageWriterSettings));
        }
#endif

        /// <summary>
        /// Returns the content type for the MultipartMime Batch format
        /// </summary>
        /// <param name="mediaType">The specified media type.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <param name="writingResponse">True if the message writer is being used to write a response.</param>
        /// <param name="mediaTypeParameters"> The resultant parameters list of the media type.
        /// For multipart/mixed batch type, boundary parameter will be created as required and be added to parameters list.
        /// </param>
        /// <returns>The content-type value for the format.</returns>
        internal override string GetContentType(ODataMediaType mediaType, Encoding encoding,
            bool writingResponse, out IEnumerable<KeyValuePair<string, string>> mediaTypeParameters)
        {
            ExceptionUtils.CheckArgumentNotNull(mediaType, "mediaType");

            IEnumerable<KeyValuePair<string, string>> origParameters = mediaType.Parameters != null
                ? mediaType.Parameters
                : new List<KeyValuePair<string, string>>();

            IEnumerable<KeyValuePair<string, string>> boundaryParameters = origParameters.Where(
                p =>
                    string.Compare(p.Key, ODataConstants.HttpMultipartBoundary, StringComparison.OrdinalIgnoreCase) == 0);

            string batchBoundary;
            if (boundaryParameters.Count() > 1)
            {
                throw new ODataContentTypeException(
                    Strings.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified(mediaType.ToText()));
            }
            else if (boundaryParameters.Count() == 1)
            {
                batchBoundary = boundaryParameters.First().Value;
                mediaTypeParameters = mediaType.Parameters;
            }
            else
            {
                // No boundary parameters found.
                // Create and add the boundary parameter required by the multipart/mixed batch format.
                batchBoundary = ODataMultipartMixedBatchWriterUtils.CreateBatchBoundary(writingResponse);
                List<KeyValuePair<string, string>> newList = new List<KeyValuePair<string, string>>(origParameters);
                newList.Add(new KeyValuePair<string, string>(ODataConstants.HttpMultipartBoundary, batchBoundary));
                mediaTypeParameters = newList;
            }

            // Set the content type header here since all headers have to be set before getting the stream
            // Note that the mediaType may have additional parameters, which we ignore here (intentional as per MIME spec).
            // Note that we always generate a new boundary string here, even if the accept header contained one.
            // We need the boundary to be as unique as possible to avoid possible collision with content of the batch operation payload.
            // Our boundary string are generated to fulfill this requirement, client specified ones might not which might lead to wrong responses
            // and at least in theory security issues.
            return ODataMultipartMixedBatchWriterUtils.CreateMultipartMixedContentType(batchBoundary);
        }

        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="contentType">The content type of the message.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        private static IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(ODataMediaType contentType)
        {
            // NOTE: for batch payloads we only use the content type header of the message to detect the payload kind.
            //       We assume a valid batch payload if the content type is multipart/mixed and a boundary parameter exists
            // Require 'multipart/mixed' content type with a boundary parameter to be considered batch.
            if (HttpUtils.CompareMediaTypeNames(MimeConstants.MimeMultipartType, contentType.Type) &&
                HttpUtils.CompareMediaTypeNames(MimeConstants.MimeMixedSubType, contentType.SubType) &&
                contentType.Parameters != null &&
                contentType.Parameters.Any(kvp => HttpUtils.CompareMediaTypeParameterNames(ODataConstants.HttpMultipartBoundary, kvp.Key)))
            {
                return new ODataPayloadKind[] { ODataPayloadKind.Batch };
            }

            return Enumerable.Empty<ODataPayloadKind>();
        }
    }
}
