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

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Core.Metadata;
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
                ? DetectPayloadKindImplementation(
                    messageInfo.GetMessageStream(),
                    new ODataPayloadKindDetectionInfo(
                        messageInfo.MediaType,
                        messageInfo.Encoding,
                        settings,
                        messageInfo.Model))
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

            return new ODataMetadataInputContext(
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

            return new ODataMetadataOutputContext(
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
            return messageInfo.IsResponse
                ? messageInfo.GetMessageStreamAsync()
                    .FollowOnSuccessWith(streamTask => DetectPayloadKindImplementation(
                            streamTask.Result,
                            new ODataPayloadKindDetectionInfo(
                                messageInfo.MediaType,
                                messageInfo.Encoding,
                                settings,
                                messageInfo.Model)))
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

            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataMetadataFormat_CreateOutputContextAsync));
        }
#endif

        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="messageStream">The message stream to read from for payload kind detection.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero or one payload kinds depending on whether the metadata payload kind was detected or not.</returns>
        private static IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(Stream messageStream, ODataPayloadKindDetectionInfo detectionInfo)
        {
            try
            {
                using (XmlReader reader = ODataAtomReaderUtils.CreateXmlReader(messageStream, detectionInfo.GetEncoding(), detectionInfo.MessageReaderSettings))
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
