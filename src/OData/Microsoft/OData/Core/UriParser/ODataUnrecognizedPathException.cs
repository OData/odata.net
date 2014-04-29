//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
#if ORCAS
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
#endif
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// The exception that is thrown when path parsing detects an unrecognized or unresolvable token in a path (which servers should treat as a 404).
    /// </summary>
#if ORCAS
    [Serializable]
#endif
    [DebuggerDisplay("{Message}")]
    public sealed class ODataUnrecognizedPathException : ODataException
    {
        /// <summary>
        /// Initializes a new instance of the ODataUnrecognizedPathException class.
        /// </summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message 
        /// that describes the error. This message takes into account the 
        /// current system culture. 
        /// </remarks>
        public ODataUnrecognizedPathException()
            : this((string)Strings.ODataUriParserException_GeneralError, (Exception)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataUnrecognizedPathException class.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        public ODataUnrecognizedPathException(string message)
            : this(message, (Exception)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataServiceException class.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        public ODataUnrecognizedPathException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
#if ORCAS
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.ODataUnrecognizedPathException" /> class from the  specified SerializationInfo and StreamingContext instances.</summary>
        /// <param name="info"> A SerializationInfo containing the information required to serialize the new ODataUnrecognizedPathException. </param>
        /// <param name="context"> A StreamingContext containing the source of the serialized stream  associated with the new ODataUnrecognizedPathException. </param>
        [SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        private ODataUnrecognizedPathException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        
        /// <summary>
        /// Segments that have been successfully parsed when this exception was thrown.
        /// </summary>
        public IEnumerable<ODataPathSegment> ParsedSegments { get; set; }

        /// <summary>
        /// Current segment UriParser was dealing with when exception was thrown.
        /// </summary>
        public string CurrentSegment { get; set; }

        /// <summary>
        /// Unparsed segments.
        /// </summary>
        public IEnumerable<string> UnparsedSegments { get; set; }
    }
}
