//---------------------------------------------------------------------
// <copyright file="ODataErrorException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
#if ORCAS
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
#endif
    #endregion Namespaces

    /// <summary>
    /// Exception type representing an in-stream error parsed when reading a payload.
    /// </summary>
#if ORCAS
    [Serializable]
#endif
    [DebuggerDisplay("{Message}")]
    public sealed class ODataErrorException : ODataException
    {
        /// <summary>The <see cref="ODataErrorExceptionSafeSerializationState"/> value containing <see cref="ODataError"/> instance representing the error
        /// read from the payload.
        /// </summary>
#if ORCAS
        // Because we don't want the exception state to be serialized normally, we take care of that in the constructor.
        [NonSerialized]
#endif
        private ODataErrorExceptionSafeSerializationState state;

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataErrorException" /> class with default values.</summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message
        /// that describes the error. This message takes into account the
        /// current system culture. The Error property will be initialized with an empty <see cref="ODataError"/> instance.
        /// </remarks>
        public ODataErrorException()
            : this(Strings.ODataErrorException_GeneralError)
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataErrorException" /> class with an error message.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <remarks>
        /// The Error property will be initialized with an empty <see cref="ODataError"/> instance.
        /// </remarks>
        public ODataErrorException(string message)
            : this(message, (Exception)null)
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataErrorException" /> class with an error message and an inner exception.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        /// <remarks>
        /// The Error property will be initialized with an empty <see cref="ODataError"/> instance.
        /// </remarks>
        public ODataErrorException(string message, Exception innerException)
            : this(message, innerException, new ODataError())
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataErrorException" /> class with an <see cref="Microsoft.OData.ODataError" /> object.</summary>
        /// <param name="error">The <see cref="Microsoft.OData.ODataError" /> instance representing the error read from the payload.</param>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message
        /// that describes the error. This message takes into account the
        /// current system culture.
        /// </remarks>
        public ODataErrorException(ODataError error)
            : this(Strings.ODataErrorException_GeneralError, null, error)
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataErrorException" /> class with an error message and an <see cref="Microsoft.OData.ODataError" /> object.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="error">The <see cref="Microsoft.OData.ODataError" /> instance representing the error read from the payload.</param>
        public ODataErrorException(string message, ODataError error)
            : this(message, null, error)
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataErrorException" /> class with an error message, an inner exception, and an <see cref="Microsoft.OData.ODataError" /> object.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        /// <param name="error">The <see cref="Microsoft.OData.ODataError" /> instance representing the error read from the payload.</param>
        public ODataErrorException(string message, Exception innerException, ODataError error)
            : base(message, innerException)
        {
            this.state.ODataError = error;
        }

#if ORCAS
#pragma warning disable 0628
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

            this.state.ODataError = (ODataError)info.GetValue("Error", typeof(ODataError));
        }
#pragma warning restore 0628
#endif

        /// <summary>Gets the <see cref="Microsoft.OData.ODataError" /> instance representing the error read from the payload.</summary>
        /// <returns>The <see cref="Microsoft.OData.ODataError" /> instance representing the error read from the payload.</returns>
        public ODataError Error
        {
            get
            {
                return this.state.ODataError;
            }
        }

#if ORCAS
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
            info.AddValue("Error", this.state.ODataError);
        }
#endif

        /// <summary>
        /// Implement the ISafeSerializationData interface to contain custom exception data in a partially trusted assembly.
        /// Use this interface in post-ORCAS to replace the Exception.GetObjectData method, which is marked with the SecurityCriticalAttribute.
        /// </summary>
#if ORCAS
        [Serializable]
#endif
        private struct ODataErrorExceptionSafeSerializationState
        {
            /// <summary>
            /// Gets or sets the <see cref="ODataError"/> object.
            /// </summary>
            public ODataError ODataError
            {
                get;
                set;
            }
        }
    }
}
