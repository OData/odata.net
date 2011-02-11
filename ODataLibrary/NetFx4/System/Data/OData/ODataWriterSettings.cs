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

namespace System.Data.OData
{
    #region Namespaces
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Configuration settings for OData writers.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class ODataWriterSettings
#else
    public sealed class ODataWriterSettings
#endif
    {
        /// <summary>
        /// The acceptable charsets used to the determine the encoding of the message.
        /// This is a comma separated list of charsets as specified in RFC 2616, Section 14.2
        /// </summary>
        private string acceptCharSets;

        /// <summary>
        /// The acceptable media types used to determine the content type of the message.
        /// This is a comma separated list of content types as specified in RFC 2616, Section 14.1
        /// </summary>
        private string acceptMediaTypes;

        /// <summary>
        /// The format to use when writing the payload; this replaces the 'AcceptHeader' and 'AcceptCharSetHeader' 
        /// fields and uses the default values for the respective format. If ODataFormat.Default is specified
        /// the default format and the default media type will be picked depending on the writer these settings are used with.
        /// </summary>
        private ODataFormat format = ODataFormat.Default;

        /// <summary>Maximum number of query operations and changesets allowed in a single batch./// </summary>
        private int maxBatchSize;

        /// <summary>Maximum number of requests allowed in a single changeset.</summary>
        private int maxChangeSetSize;

        /// <summary>
        /// Constructor to create default settings for OData writers.
        /// </summary>
        public ODataWriterSettings()
        {
            this.Version = ODataConstants.ODataDefaultProtocolVersion;
            this.maxBatchSize = int.MaxValue;
            this.maxChangeSetSize = int.MaxValue;

            // on writing the default value for 'CheckCharacters' is set to true so that we 
            // produce valid Xml documents per default.
            this.CheckCharacters = true;
        }

        /// <summary>
        /// The OData protocol version to be used for writing payloads.
        /// </summary>
        public ODataVersion Version
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to control whether the writer should use indentation or not.
        /// </summary>
        public bool Indent
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to control whether the writer should check for valid Xml characters or not.
        /// </summary>
        public bool CheckCharacters
        {
            get;
            set;
        }

        /// <summary>
        /// Document base Uri (used as base for all relative Uris).
        /// </summary>
        public Uri BaseUri
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum number of query operations and changesets allowed in a single batch.
        /// </summary>
        public int MaxBatchSize
        {
            get
            {
                return this.maxBatchSize;
            }

            set
            {
                ExceptionUtils.CheckIntegerNotNegative(value, "value");
                this.maxBatchSize = value;
            }
        }

        /// <summary>
        /// Maximum number of requests allowed in a single changeset.
        /// </summary>
        public int MaxChangesetSize
        {
            get
            {
                return this.maxChangeSetSize;
            }

            set
            {
                ExceptionUtils.CheckIntegerNotNegative(value, "value");
                this.maxChangeSetSize = value;
            }
        }

        /// <summary>
        /// The acceptable media types used to determine the content type of the message.
        /// This is a comma separated list of content types as specified in RFC 2616, Section 14.1
        /// </summary>
        /// <remarks>A null or empty accept header means that all content types are acceptable.</remarks>
        /// <remarks>For response messages this is usually the 'Accept' header of the request message.</remarks>
        internal string AcceptableMediaTypes
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.acceptMediaTypes;
            }
        }

        /// <summary>
        /// The acceptable charsets used to the determine the encoding of the message.
        /// This is a comma separated list of charsets as specified in RFC 2616, Section 14.2
        /// </summary>
        /// <remarks>A null or empty accept charset header means that all charsets are acceptable.</remarks>
        /// <remarks>For response messages this is usually the 'Accept-Charset' header of the request message.</remarks>
        internal string AcceptableCharsets
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.acceptCharSets;
            }
        }

        /// <summary>
        /// The format to use when writing the payload; this replaces the 'AcceptHeader' and 'AcceptCharSetHeader' 
        /// properties and uses the default values for the respective format. If ODataFormat.Default is specified
        /// the default format and the default media type will be picked depending on the writer these settings are used with.
        /// </summary>
        internal ODataFormat Format
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.format;
            }
        }

        /// <summary>
        /// Sets the acceptable media types and character sets from which the content type will be computed when writing the payload.
        /// </summary>
        /// <param name="acceptableMediaTypes">
        /// The acceptable media types used to determine the content type of the message.
        /// This is a comma separated list of content types as specified in RFC 2616, Section 14.1
        /// </param>
        /// <param name="acceptableCharSets">
        /// The acceptable charsets to use to the determine the encoding of the message.
        /// This is a comma separated list of charsets as specified in RFC 2616, Section 14.2
        /// </param>
        /// <remarks>Calling this method replaces any previously set content-type settings.</remarks>
        public void SetContentType(string acceptableMediaTypes, string acceptableCharSets)
        {
            ExceptionUtils.CheckArgumentNotNull(acceptableMediaTypes, "acceptableMediaTypes");

            this.acceptMediaTypes = acceptableMediaTypes;
            this.acceptCharSets = acceptableCharSets;
            this.format = ODataFormat.Default;
        }

        /// <summary>
        /// Sets the format to be used when writing the payload. This will automatically set a compatible 
        /// content type header.
        /// </summary>
        /// <param name="payloadFormat">The format to use for writing the payload.</param>
        /// <remarks>Calling this method replaces any previously set content-type settings.</remarks>
        public void SetContentType(ODataFormat payloadFormat)
        {
            this.acceptCharSets = null;
            this.acceptMediaTypes = null;
            this.format = payloadFormat;
        }
    }
}
