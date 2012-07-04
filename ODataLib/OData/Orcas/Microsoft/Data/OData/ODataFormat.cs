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
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Representation of an OData format.
    /// </summary>
    public abstract class ODataFormat
    {
        /// <summary>The ATOM format instance.</summary>
        private static ODataAtomFormat atomFormat = new ODataAtomFormat();

        /// <summary>The verbose JSON format instance.</summary>
        private static ODataVerboseJsonFormat verboseJsonFormat = new ODataVerboseJsonFormat();

        /// <summary>The RAW format instance.</summary>
        private static ODataRawValueFormat rawValueFormat = new ODataRawValueFormat();

        /// <summary>The batch format instance.</summary>
        private static ODataBatchFormat batchFormat = new ODataBatchFormat();

        /// <summary>The metadata format instance.</summary>
        private static ODataMetadataFormat metadataFormat = new ODataMetadataFormat();

        /// <summary>ATOM format; we also use this for all Xml based formats (if ATOM can't be used).</summary>
        public static ODataFormat Atom
        {
            get
            {
                return atomFormat;
            }
        }

        /// <summary>Verbose JSON format</summary>
        public static ODataFormat VerboseJson
        {
            get
            {
                return verboseJsonFormat;
            }
        }

        /// <summary>RAW format; used for raw values.</summary>
        public static ODataFormat RawValue
        {
            get
            {
                return rawValueFormat;
            }
        }

        /// <summary>The batch format instance.</summary>
        public static ODataFormat Batch
        {
            get
            {
                return batchFormat;
            }
        }

        /// <summary>The metadata format instance.</summary>
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
#endif
    }
}
