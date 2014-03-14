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
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if !WINDOWS_PHONE && !SILVERLIGHT  && !PORTABLELIB
    using System.Runtime.Serialization;
#endif
    #endregion Namespaces

    /// <summary>
    /// Exception type representing exceptions in the OData library.
    /// </summary>
#if !WINDOWS_PHONE && !SILVERLIGHT  && !PORTABLELIB
    [Serializable]
#endif
    [DebuggerDisplay("{Message}")]
    public class ODataException : InvalidOperationException
    {
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.ODataException" /> class with default values.</summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message
        /// that describes the error. This message takes into account the
        /// current system culture.
        /// </remarks>
        public ODataException()
            : this(Strings.ODataException_GeneralError)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.ODataException" /> class with an error message.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        public ODataException(string message)
            : this(message, null)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.ODataException" /> class with an error message and an inner exception.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        public ODataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !WINDOWS_PHONE && !SILVERLIGHT && !PORTABLELIB
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.ODataException" /> class from the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" /> instances.</summary>
        /// <param name="info"> A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> containing the information required to serialize  the new <see cref="T:Microsoft.Data.OData.ODataException" />. </param>
        /// <param name="context"> A <see cref="T:System.Runtime.Serialization.StreamingContext" /> containing the source of the serialized stream  associated with the new <see cref="T:Microsoft.Data.OData.ODataException" />. </param>
        [SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        protected ODataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
