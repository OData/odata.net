//---------------------------------------------------------------------
// <copyright file="ODataFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections.Generic;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using System.Text;
    using Microsoft.OData.Json;
    using Microsoft.OData.MultipartMixed;
    #endregion Namespaces

    /// <summary>
    /// Representation of an OData format.
    /// </summary>
    public abstract class ODataFormat
    {
        /// <summary>The JSON Light format instance.</summary>
        private static ODataJsonFormat JsonFormat = new ODataJsonFormat();

        /// <summary>The RAW format instance.</summary>
        private static ODataRawValueFormat rawValueFormat = new ODataRawValueFormat();

        /// <summary>The batch format instance.</summary>
        private static ODataMultipartMixedBatchFormat batchFormat = new ODataMultipartMixedBatchFormat();

        /// <summary>The metadata format instance.</summary>
        private static ODataMetadataFormat metadataFormat = new ODataMetadataFormat();

        /// <summary>Specifies the JSON format.</summary>
        /// <returns>The JSON format.</returns>
        public static ODataFormat Json
        {
            get
            {
                return JsonFormat;
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
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="settings">Configuration settings of the OData reader.</param>
        /// <returns>The set of <see cref="ODataPayloadKind"/>s that are supported with the specified payload.</returns>
        /// <remarks>
        /// The stream returned by GetMessageStream of <paramref name="messageInfo"/> could be used for reading for
        /// payload kind detection. Reading this stream won't affect later reading operations of payload processing.
        /// </remarks>
        public abstract IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings);

        /// <summary>
        /// Creates an instance of the input context for this format.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <returns>The newly created input context.</returns>
        public abstract ODataInputContext CreateInputContext(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings);

        /// <summary>
        /// Creates an instance of the output context for this format.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <returns>The newly created output context.</returns>
        public abstract ODataOutputContext CreateOutputContext(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings);

#if PORTABLELIB
        /// <summary>
        /// Asynchronously detects the payload kinds supported by this format for the specified message payload.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="settings">Configuration settings of the OData reader.</param>
        /// <returns>A task that when completed returns the set of <see cref="ODataPayloadKind"/>s
        /// that are supported with the specified payload.</returns>
        /// <remarks>
        /// The stream returned by GetMessageStream of <paramref name="messageInfo"/> could be used for reading for
        /// payload kind detection. Reading this stream won't affect later reading operations of payload processing.
        /// </remarks>
        public abstract Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings);

        /// <summary>
        /// Asynchronously creates an instance of the input context for this format.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <returns>Task which when completed returned the newly created input context.</returns>
        public abstract Task<ODataInputContext> CreateInputContextAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings);

        /// <summary>
        /// Creates an instance of the output context for this format.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <returns>Task which represents the pending create operation.</returns>
        public abstract Task<ODataOutputContext> CreateOutputContextAsync(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings);
#endif

        /// <summary>
        /// Returns the appropriate content-type for this format.
        /// </summary>
        /// <param name="mediaType">The specified media type.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <param name="writingResponse">True if the message writer is being used to write a response.</param>
        /// <param name="mediaTypeParameters"> The resultant parameters list of the media type. Parameters list could be updated
        /// when getting content type and should be returned if that is the case.
        /// </param>
        /// <returns>The content-type value for the format.</returns>
        internal virtual string GetContentType(ODataMediaType mediaType, Encoding encoding,
            bool writingResponse, out IEnumerable<KeyValuePair<string, string>> mediaTypeParameters)
        {
            mediaTypeParameters = mediaType.Parameters;
            return HttpUtils.BuildContentType(mediaType, encoding);
        }
    }
}
