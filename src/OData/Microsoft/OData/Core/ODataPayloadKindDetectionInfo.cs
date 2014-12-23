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
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Represents the set of information available for payload kind detection.
    /// </summary>
    /// <remarks>This class is used to represent the input to run payload kind detection using
    /// <see cref="ODataMessageReader.DetectPayloadKind"/>. See the documentation of that method for more 
    /// information.</remarks>
    internal sealed class ODataPayloadKindDetectionInfo
    {
        /// <summary>The parsed content type as <see cref="ODataMediaType"/>.</summary>
        private readonly ODataMediaType contentType;

        /// <summary>The encoding specified in the charset parameter of contentType or the default encoding from MediaType.</summary>
        private readonly Encoding encoding;

        /// <summary>The <see cref="ODataMessageReaderSettings"/> being used for reading the message.</summary>
        private readonly ODataMessageReaderSettings messageReaderSettings;

        /// <summary>The <see cref="IEdmModel"/> for the payload.</summary>
        private readonly IEdmModel model;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contentType">The parsed content type as <see cref="ODataMediaType"/>.</param>
        /// <param name="encoding">The encoding from the content type or the default encoding from <see cref="ODataMediaType" />.</param>
        /// <param name="messageReaderSettings">The <see cref="ODataMessageReaderSettings"/> being used for reading the message.</param>
        /// <param name="model">The <see cref="IEdmModel"/> for the payload.</param>
        internal ODataPayloadKindDetectionInfo(
            ODataMediaType contentType,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings, 
            IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(contentType, "contentType");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "readerSettings");

            this.contentType = contentType;
            this.encoding = encoding;
            this.messageReaderSettings = messageReaderSettings;
            this.model = model;
        }

        /// <summary>
        /// The <see cref="ODataMessageReaderSettings"/> being used for reading the message.
        /// </summary>
        public ODataMessageReaderSettings MessageReaderSettings
        {
            get { return this.messageReaderSettings; }
        }

        /// <summary>
        /// The <see cref="IEdmModel"/> for the payload.
        /// </summary>
        public IEdmModel Model
        {
            get { return this.model; }
        }

        /// <summary>
        /// The <see cref="ODataMessageReaderSettings"/> being used for reading the message.
        /// </summary>
        internal ODataMediaType ContentType
        {
            get
            {
                return this.contentType;
            }
        }

        /// <summary>
        /// The encoding derived from the content type or the default encoding.
        /// </summary>
        /// <returns>The encoding derived from the content type or the default encoding.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "There is computation needed to get the encoding from the content type; thus a method.")]
        public Encoding GetEncoding()
        {
            return this.encoding ?? this.contentType.SelectEncoding();
        }
    }
}
