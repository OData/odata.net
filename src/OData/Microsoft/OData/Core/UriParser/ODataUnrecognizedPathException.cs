//   OData .NET Libraries ver. 6.8.1
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
