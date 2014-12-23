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
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Core.Json;
    #endregion Namespaces

    /// <summary>
    /// Representation of an OData format.
    /// </summary>
    public abstract class ODataFormat
    {
        /// <summary>The ATOM format instance.</summary>
        private static ODataAtomFormat atomFormat = new ODataAtomFormat();

        /// <summary>The JSON Light format instance.</summary>
        private static ODataJsonFormat JsonFormat = new ODataJsonFormat();

        /// <summary>The RAW format instance.</summary>
        private static ODataRawValueFormat rawValueFormat = new ODataRawValueFormat();

        /// <summary>The batch format instance.</summary>
        private static ODataBatchFormat batchFormat = new ODataBatchFormat();

        /// <summary>The metadata format instance.</summary>
        private static ODataMetadataFormat metadataFormat = new ODataMetadataFormat();

        /// <summary>Specifies the ATOM format; we also use this for all Xml based formats (if ATOM can't be used).</summary>
        /// <returns>The ATOM format.</returns>
        [System.Obsolete("ATOM support is obsolete.")]
        public static ODataFormat Atom
        {
            get
            {
                return atomFormat;
            }
        }

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

#if ODATALIB_ASYNC
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
    }
}
