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

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
#if !ASTORIA_LIGHT
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// The exception that is thrown when the server returns an error.
    /// </summary>
#if !ASTORIA_LIGHT && !PORTABLELIB
    [Serializable]
#endif
    [System.Diagnostics.DebuggerDisplay("{Message}")]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "No longer relevant after .NET 4 introduction of SerializeObjectState event and ISafeSerializationData interface.")]
    public sealed class DataServiceClientException : InvalidOperationException
    {
        /// <summary>
        /// Contains the state for this exception.
        /// </summary>
#if !ASTORIA_LIGHT && !PORTABLELIB
        [NonSerialized]
#endif
        private DataServiceClientExceptionSerializationState state;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceClientException" /> class with a system-supplied message that describes the error. </summary>
        public DataServiceClientException()
            : this(Strings.DataServiceException_GeneralError)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceClientException" /> class with a specified message that describes the error. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public DataServiceClientException(string message)
            : this(message, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceClientException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
        public DataServiceClientException(string message, Exception innerException)
            : this(message, innerException, 500)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceClientException" /> class. </summary>
        /// <param name="message">The string value that contains the error message.</param>
        /// <param name="statusCode">The integer value that contains status code.</param>
        public DataServiceClientException(string message, int statusCode)
            : this(message, null, statusCode)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceClientException" /> class. </summary>
        /// <param name="message">The string value that contains the error message.</param>
        /// <param name="innerException">The System.Exception object that contains the inner exception.</param>
        /// <param name="statusCode">The integer value that contains status code.</param>
        public DataServiceClientException(string message, Exception innerException, int statusCode)
            : base(message, innerException)
        {
            this.state.StatusCode = statusCode;

#if !ASTORIA_LIGHT && !PORTABLELIB
            this.SerializeObjectState += (sender, e) => e.AddSerializedState(this.state);
#endif
        }

        #endregion Constructors

        #region Public properties

        /// <summary>Gets the HTTP error status code returned after <see cref="T:Microsoft.OData.Client.DataServiceClientException" />.</summary>
        /// <returns>An integer value that represents the exception.</returns>
        public int StatusCode
        {
            get { return this.state.StatusCode; }
        }

        #endregion Public properties

        /// <summary>
        /// Contains the state of the exception, used for serialization in security transparent code.
        /// </summary>
#if !ASTORIA_LIGHT && !PORTABLELIB
        [Serializable]
#endif
        private struct DataServiceClientExceptionSerializationState
#if !ASTORIA_LIGHT && !PORTABLELIB
            : ISafeSerializationData
#endif
        {
            /// <summary>
            /// Gets or sets the status code as returned by the server.
            /// </summary>
            public int StatusCode { get; set; }

#if !ASTORIA_LIGHT && !PORTABLELIB
            /// <summary>
            /// Called when deserialization of the exception is complete.
            /// </summary>
            /// <param name="deserialized">The deserialized exception.</param>
            void ISafeSerializationData.CompleteDeserialization(object deserialized)
            {
                var exception = (DataServiceClientException)deserialized;
                exception.state = this;
            }
#endif
        }
    }
}
