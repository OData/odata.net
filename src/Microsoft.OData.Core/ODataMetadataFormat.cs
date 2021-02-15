//---------------------------------------------------------------------
// <copyright file="ODataMetadataFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Threading.Tasks;
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// The metadata OData format.
    /// </summary>
    internal sealed class ODataMetadataFormat : ODataFormat
    {
        /// <summary>
        /// The text representation - the name of the format.
        /// </summary>
        /// <returns>The name of the format.</returns>
        public override string ToString()
        {
            return "Metadata";
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

            // Metadata is not supported in requests!
            return messageInfo.IsResponse
                ? DetectPayloadKindImplementation(messageInfo, settings)
                : Enumerable.Empty<ODataPayloadKind>();
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

            bool isJson = IsJsonMetadata(messageInfo.MediaType);

#if NETSTANDARD2_0
            if (isJson)
            {
                return new ODataMetadataJsonInputContext(messageInfo, messageReaderSettings);
            }

            return new ODataMetadataInputContext(messageInfo, messageReaderSettings);
#else
            if (isJson)
            {
                throw new ODataException(Strings.ODataMetadataOutputContext_NotSupportJsonMetadata);
            }

            return new ODataMetadataInputContext(messageInfo, messageReaderSettings);
#endif
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

            bool isJson = IsJsonMetadata(messageInfo.MediaType);

#if NETSTANDARD2_0
            if (isJson)
            {
                return new ODataMetadataJsonOutputContext(messageInfo, messageWriterSettings);
            }

            return new ODataMetadataOutputContext(messageInfo, messageWriterSettings);
#else
            if (isJson)
            {
                throw new ODataException(Strings.ODataMetadataOutputContext_NotSupportJsonMetadata);
            }

            return new ODataMetadataOutputContext(messageInfo, messageWriterSettings);
#endif
        }

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
            return messageInfo.IsResponse
                ? Task.FromResult(DetectPayloadKindImplementation(messageInfo, settings))
                : TaskUtils.GetCompletedTask(Enumerable.Empty<ODataPayloadKind>());
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

            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataFormat_CreateInputContextAsync));
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

            bool isJson = IsJsonMetadata(messageInfo.MediaType);

#if NETSTANDARD2_0
            if (isJson)
            {
                return Task.FromResult<ODataOutputContext>(new ODataMetadataJsonOutputContext(messageInfo, messageWriterSettings));
            }

            return Task.FromResult<ODataOutputContext>(new ODataMetadataOutputContext(messageInfo, messageWriterSettings));
#else
            if (isJson)
            {
                throw new ODataException(Strings.ODataMetadataOutputContext_NotSupportJsonMetadata);
            }

            return Task.FromResult<ODataOutputContext>(new ODataMetadataOutputContext(messageInfo, messageWriterSettings));
#endif
        }

        private static bool IsJsonMetadata(ODataMediaType contentType)
        {
            // by default, it's XML metadata
            if (contentType == null)
            {
                return false;
            }

            if (HttpUtils.CompareMediaTypeNames(MimeConstants.MimeApplicationType, contentType.Type) &&
                HttpUtils.CompareMediaTypeNames(MimeConstants.MimeJsonSubType, contentType.SubType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="settings">Configuration settings of the OData reader.</param>
        /// <returns>An enumerable of zero or one payload kinds depending on whether the metadata payload kind was detected or not.</returns>
        private static IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings settings)
        {
            var detectionInfo = new ODataPayloadKindDetectionInfo(messageInfo, settings);
            try
            {
                using (var reader = ODataMetadataReaderUtils.CreateXmlReader(
                    messageInfo.MessageStream, detectionInfo.GetEncoding(), detectionInfo.MessageReaderSettings))
                {
                    if (reader.TryReadToNextElement()
                        && string.CompareOrdinal(EdmConstants.EdmxName, reader.LocalName) == 0
                        && reader.NamespaceURI == EdmConstants.EdmxOasisNamespace)
                    {
                        return new ODataPayloadKind[] { ODataPayloadKind.MetadataDocument };
                    }
                }
            }
            catch (XmlException)
            {
                // If we are not able to read the payload as XML it is not a metadata document.
                // Return no detected payload kind below.
            }

            return Enumerable.Empty<ODataPayloadKind>();
        }
    }
}
