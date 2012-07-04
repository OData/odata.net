//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
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
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Metadata;
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
        /// <param name="responseMessage">The response message with the payload stream.</param>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>The set of <see cref="ODataPayloadKind"/>s that are supported with the specified payload.</returns>
        internal override IEnumerable<ODataPayloadKind> DetectPayloadKind(
            IODataResponseMessage responseMessage,
            ODataPayloadKindDetectionInfo detectionInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(responseMessage, "responseMessage");
            ExceptionUtils.CheckArgumentNotNull(detectionInfo, "detectionInfo");

            Stream messageStream = ((ODataMessage)responseMessage).GetStream();
            return DetectPayloadKindImplementation(messageStream, detectionInfo);
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(requestMessage, "requestMessage");
            ExceptionUtils.CheckArgumentNotNull(detectionInfo, "detectionInfo");

            // Metadata is not supported in requests!
            return Enumerable.Empty<ODataPayloadKind>();
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(responseMessage, "responseMessage");
            ExceptionUtils.CheckArgumentNotNull(detectionInfo, "detectionInfo");

            // NOTE: After getting the message stream we already (asynchronously) buffered the whole stream in memory (in the AsyncBufferedStream).
            //       Until we get Task-based async stream APIs and retire the AsyncBufferedStream, we call the synchronous method on the buffered stream.
            return ((ODataMessage)responseMessage).GetStreamAsync()
                .FollowOnSuccessWith(streamTask => DetectPayloadKindImplementation(streamTask.Result, detectionInfo));
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(requestMessage, "requestMessage");
            ExceptionUtils.CheckArgumentNotNull(detectionInfo, "detectionInfo");

            // Metadata is not supported in requests!
            return TaskUtils.GetCompletedTask(Enumerable.Empty<ODataPayloadKind>());
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
                    if (reader.TryReadToNextElement() && string.CompareOrdinal(EdmConstants.EdmxName, reader.LocalName) == 0)
                    {
                        switch (reader.NamespaceURI)
                        {
                            case EdmConstants.EdmxVersion1Namespace:
                            case EdmConstants.EdmxVersion2Namespace:
                            case EdmConstants.EdmxVersion3Namespace:
                                return new ODataPayloadKind[] { ODataPayloadKind.MetadataDocument };
                        }
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
