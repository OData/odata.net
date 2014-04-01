//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if ORCAS
    using System.Runtime.Serialization;
#endif
    #endregion Namespaces

    /// <summary>Exception type representing exception when Content-Type of a message is not supported.</summary>
#if !PORTABLELIB
    [Serializable]
#endif
    [DebuggerDisplay("{Message}")]
    public class ODataContentTypeException : ODataException
    {
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.ODataContentTypeException" /> class.</summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message
        /// that describes the error. This message takes into account the
        /// current system culture.
        /// </remarks>
        public ODataContentTypeException()
            : this(Strings.ODataException_GeneralError)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.ODataContentTypeException" /> class.</summary>
        /// <param name="message">Plain text error message for this exception.</param>
        public ODataContentTypeException(string message)
            : this(message, null)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.ODataContentTypeException" /> class.</summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        public ODataContentTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if ORCAS
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.ODataContentTypeException" /> class from the  specified SerializationInfo and StreamingContext instances.</summary>
        /// <param name="info"> A SerializationInfo containing the information required to serialize  the new ODataException. </param>
        /// <param name="context"> A StreamingContext containing the source of the serialized stream  associated with the new ODataException. </param>
        [SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        protected ODataContentTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
