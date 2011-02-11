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
    #region Namespaces.
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    #endregion Namespaces.

    /// <summary>
    /// Exception type representing exceptions in the OData library.
    /// </summary>
#if !WINDOWS_PHONE && !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("{Message}")]
#if INTERNAL_DROP
    internal sealed class ODataException : InvalidOperationException
#else
    public sealed class ODataException : InvalidOperationException
#endif
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new instance of the ODataException class.
        /// </summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message 
        /// that describes the error. This message takes into account the 
        /// current system culture.
        /// </remarks>
        public ODataException()
            : this(Strings.ODataException_GeneralError)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataException class.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        public ODataException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataException class.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        public ODataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#pragma warning disable 0628
#if !WINDOWS_PHONE && !SILVERLIGHT
        // Warning CS0628:
        // A sealed class cannot introduce a protected member because no other class will be able to inherit from the 
        // sealed class and use the protected member.
        //
        // This method is used by the runtime when deserializing an exception. It follows the standard pattern,
        // which will also be necessary when this class is subclassed by ODataException<T>.

        /// <summary>
        /// Initializes a new instance of the ODataException class from the 
        /// specified SerializationInfo and StreamingContext instances.
        /// </summary>
        /// <param name="serializationInfo">
        /// A SerializationInfo containing the information required to serialize 
        /// the new ODataException.
        /// </param>
        /// <param name="streamingContext">
        /// A StreamingContext containing the source of the serialized stream 
        /// associated with the new ODataException.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        protected ODataException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
#endif
#pragma warning restore 0628

        #endregion Constructors.
    }
}
