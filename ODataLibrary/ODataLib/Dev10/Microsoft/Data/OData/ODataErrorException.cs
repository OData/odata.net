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
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    #endregion Namespaces

    /// <summary>
    /// Exception type representing an in-stream error parsed when reading a payload.
    /// </summary>
#if !WINDOWS_PHONE && !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("{Message}")]
    public sealed class ODataErrorException : ODataException
    {
        /// <summary>The <see cref="ODataError"/> instance representing the error read from the payload.</summary>
        private readonly ODataError error;

        /// <summary>
        /// Initializes a new instance of the ODataErrorException class.
        /// </summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message 
        /// that describes the error. This message takes into account the 
        /// current system culture. The Error property will be initialized with an empty <see cref="ODataError"/> instance.
        /// </remarks>
        public ODataErrorException()
            : this(Strings.ODataErrorException_GeneralError)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataException class.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <remarks>
        /// The Error property will be initialized with an empty <see cref="ODataError"/> instance.
        /// </remarks>
        public ODataErrorException(string message)
            : this(message, (Exception)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataException class.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        /// <remarks>
        /// The Error property will be initialized with an empty <see cref="ODataError"/> instance.
        /// </remarks>
        public ODataErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.error = new ODataError();
        }

        /// <summary>
        /// Initializes a new instance of the ODataErrorException class.
        /// </summary>
        /// <param name="error">The <see cref="ODataError"/> instance representing the error read from the payload.</param>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message 
        /// that describes the error. This message takes into account the 
        /// current system culture.
        /// </remarks>
        public ODataErrorException(ODataError error)
            : base(Strings.ODataErrorException_GeneralError)
        {
            this.error = error;
        }

        /// <summary>
        /// Initializes a new instance of the ODataException class.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="error">The <see cref="ODataError"/> instance representing the error read from the payload.</param>
        public ODataErrorException(string message, ODataError error)
            : this(message, null, error)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataException class.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        /// <param name="error">The <see cref="ODataError"/> instance representing the error read from the payload.</param>
        public ODataErrorException(string message, Exception innerException, ODataError error)
            : base(message, innerException)
        {
            this.error = error;
        }

#pragma warning disable 0628
#if !WINDOWS_PHONE && !SILVERLIGHT
        // Warning CS0628:
        // A sealed class cannot introduce a protected member because no other class will be able to inherit from the 
        // sealed class and use the protected member.
        //
        // This method is used by the runtime when deserializing an exception.

        /// <summary>
        /// Initializes a new instance of the ODataErrorException class from the 
        /// specified SerializationInfo and StreamingContext instances.
        /// </summary>
        /// <param name="info">
        /// A SerializationInfo containing the information required to serialize 
        /// the new ODataException.
        /// </param>
        /// <param name="context">
        /// A StreamingContext containing the source of the serialized stream 
        /// associated with the new ODataErrorException.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        protected ODataErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ExceptionUtils.CheckArgumentNotNull(info, "serializationInfo");

            this.error = (ODataError)info.GetValue("Error", typeof(ODataError));
        }
#endif
#pragma warning restore 0628

        /// <summary>
        /// The <see cref="ODataError"/> instance describing the in-stream error represented by this exception.
        /// </summary>
        public ODataError Error
        {
            get
            {
                return this.error;
            }
        }

#if !WINDOWS_PHONE && !SILVERLIGHT
        /// <summary>
        /// Recreates the <see cref="ODataError"/> instance of the exception.
        /// </summary>
        /// <param name="info">
        /// A SerializationInfo containing the information required to serialize 
        /// the ODataErrorException.
        /// </param>
        /// <param name="context">
        /// A StreamingContext containing the source of the serialized stream 
        /// associated with the new ODataErrorException.
        /// </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ExceptionUtils.CheckArgumentNotNull(info, "serializationInfo");

            base.GetObjectData(info, context);
            info.AddValue("Error", this.error);
        }
#endif
    }
}
