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
    #endregion Namespaces

    /// <summary>
    /// Exception type representing an in-stream error parsed when reading a payload.
    /// </summary>
    [DebuggerDisplay("{Message}")]
    public sealed class ODataErrorException : ODataException
    {
        /// <summary>The <see cref="ODataErrorExceptionSafeSerializationState"/> value containing <see cref="ODataError"/> instance representing the error
        /// read from the payload.
        /// </summary>
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

        /// <summary>Gets the <see cref="Microsoft.OData.ODataError" /> instance representing the error read from the payload.</summary>
        /// <returns>The <see cref="Microsoft.OData.ODataError" /> instance representing the error read from the payload.</returns>
        public ODataError Error
        {
            get
            {
                return this.state.ODataError;
            }
        }

        /// <summary>
        /// Implement the ISafeSerializationData interface to contain custom exception data in a partially trusted assembly.
        /// </summary>
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
