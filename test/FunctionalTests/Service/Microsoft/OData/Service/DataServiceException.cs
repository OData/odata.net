//---------------------------------------------------------------------
// <copyright file="DataServiceException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    #endregion Namespaces

    /// <summary>
    /// The exception that is thrown when an error occurs while processing
    /// a web data service request.
    /// </summary>
    /// <remarks>
    /// The DataServiceException is thrown to indicate an error during
    /// request processing, specifying the appropriate response for
    /// the request.
    /// 
    /// RFC2616 about the status code values:
    ///     1xx: Informational  - Request received, continuing process
    ///     "100"  ; Section 10.1.1: Continue
    ///     "101"  ; Section 10.1.2: Switching Protocols
    ///     
    ///     2xx: Success        - The action was successfully received, understood, and accepted
    ///     "200"  ; Section 10.2.1: OK
    ///     "201"  ; Section 10.2.2: Created
    ///     "202"  ; Section 10.2.3: Accepted
    ///     "203"  ; Section 10.2.4: Non-Authoritative Information
    ///     "204"  ; Section 10.2.5: No Content
    ///     "205"  ; Section 10.2.6: Reset Content
    ///     "206"  ; Section 10.2.7: Partial Content
    ///     
    ///     3xx: Redirection    - Further action must be taken in order to complete the request
    ///     "300"  ; Section 10.3.1: Multiple Choices
    ///     "301"  ; Section 10.3.2: Moved Permanently
    ///     "302"  ; Section 10.3.3: Found
    ///     "303"  ; Section 10.3.4: See Other
    ///     "304"  ; Section 10.3.5: Not Modified
    ///     "305"  ; Section 10.3.6: Use Proxy
    ///     "307"  ; Section 10.3.8: Temporary Redirect
    ///     
    ///     4xx: Client Error   - The request contains bad syntax or cannot be fulfilled
    ///     "400"  ; Section 10.4.1: Bad Request
    ///     "401"  ; Section 10.4.2: Unauthorized
    ///     "402"  ; Section 10.4.3: Payment Required
    ///     "403"  ; Section 10.4.4: Forbidden
    ///     "404"  ; Section 10.4.5: Not Found
    ///     "405"  ; Section 10.4.6: Method Not Allowed
    ///     "406"  ; Section 10.4.7: Not Acceptable
    ///     "407"  ; Section 10.4.8: Proxy Authentication Required
    ///     "408"  ; Section 10.4.9: Request Time-out
    ///     "409"  ; Section 10.4.10: Conflict
    ///     "410"  ; Section 10.4.11: Gone
    ///     "411"  ; Section 10.4.12: Length Required
    ///     "412"  ; Section 10.4.13: Precondition Failed
    ///     "413"  ; Section 10.4.14: Request Entity Too Large
    ///     "414"  ; Section 10.4.15: Request-URI Too Large
    ///     "415"  ; Section 10.4.16: Unsupported Media Type
    ///     "416"  ; Section 10.4.17: Requested range not satisfiable
    ///     "417"  ; Section 10.4.18: Expectation Failed
    ///     
    ///     5xx: Server Error   - The server failed to fulfill an apparently valid request
    ///     "500"  ; Section 10.5.1: Internal Server Error
    ///     "501"  ; Section 10.5.2: Not Implemented
    ///     "502"  ; Section 10.5.3: Bad Gateway
    ///     "503"  ; Section 10.5.4: Service Unavailable
    ///     "504"  ; Section 10.5.5: Gateway Time-out
    ///     "505"  ; Section 10.5.6: HTTP Version not supported
    /// </remarks>
    [Serializable]
    [DebuggerDisplay("{StatusCode}: {Message}")]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "No longer relevant after .NET 4 introduction of SerializeObjectState event and ISafeSerializationData interface.")]
    public sealed class DataServiceException : InvalidOperationException
    {
        #region Private fields

        /// <summary>
        /// Contains the state of this exception.
        /// </summary>
        [NonSerialized]
        private DataServiceExceptionSerializationState state = new DataServiceExceptionSerializationState();

        #endregion Private fields

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.DataServiceException" /> class with a system-supplied message that describes the error.</summary>
        /// <remarks>
        /// The Message property is initialized to a system-supplied message
        /// that describes the error. This message takes into account the
        /// current system culture. The StatusCode property is set to 500
        /// (Internal Server Error).
        /// </remarks>
        public DataServiceException()
            : this(500, Strings.DataServiceException_GeneralError)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.DataServiceException" /> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        /// <remarks>
        /// The StatusCode property is set to 500 (Internal Server Error).
        /// </remarks>
        public DataServiceException(string message)
            : this(500, message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.DataServiceException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. </param>
        /// <remarks>
        /// The StatusCode property is set to 500 (Internal Server Error).
        /// </remarks>
        public DataServiceException(string message, Exception innerException)
            : this(500, null, message, null, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.DataServiceException" /> class. </summary>
        /// <param name="statusCode">The HTTP status code returned by the exception.</param>
        /// <param name="message">The error message for the exception.</param>
        public DataServiceException(int statusCode, string message)
            : this(statusCode, null, message, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.DataServiceException" /> class. </summary>
        /// <param name="statusCode">The HTTP status code of the exception.</param>
        /// <param name="errorCode">The string value that contains the error code.</param>
        /// <param name="message">The string value that contains the error message.</param>
        /// <param name="messageXmlLang">The string value that indicates the language of the error message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DataServiceException(int statusCode, string errorCode, string message, string messageXmlLang, Exception innerException)
            : base(message, innerException)
        {
            this.state.ErrorCode = errorCode ?? String.Empty;
            this.state.MessageLanguage = messageXmlLang ?? CultureInfo.CurrentCulture.Name;
            this.state.StatusCode = statusCode;

            this.SerializeObjectState += (sender, e) => e.AddSerializedState(this.state);
        }

        #endregion Constructors

        #region Public properties

        /// <summary>Gets the error code.</summary>
        /// <returns>The integer value that represents the error code.</returns>
        public string ErrorCode
        {
            get { return this.state.ErrorCode; }
        }

        /// <summary>Gets the error message language.</summary>
        /// <returns>The string value that represents the message language.</returns>
        public string MessageLanguage
        {
            get { return this.state.MessageLanguage; }
        }

        /// <summary>Gets the HTTP status code returned by the exception.</summary>
        /// <returns>HTTP status code for the exception.</returns>
        public int StatusCode
        {
            get { return this.state.StatusCode; }
        }

        #endregion Public properties

        #region Internal properties

        /// <summary>'Allow' response for header.</summary>
        internal string ResponseAllowHeader
        {
            get { return this.state.ResponseAllowHeader; }
        }

        #endregion Internal properties

        #region Methods

        /// <summary>Creates a new "Bad Request" exception for recursion limit exceeded.</summary>
        /// <param name="recursionLimit">Recursion limit that was reaced.</param>
        /// <returns>A new exception to indicate that the request is rejected.</returns>
        internal static DataServiceException CreateDeepRecursion(int recursionLimit)
        {
            return DataServiceException.CreateBadRequestError(Strings.BadRequest_DeepRecursion(recursionLimit));
        }

        /// <summary>Creates a new "Bad Request" exception for recursion limit exceeded.</summary>
        /// <returns>A new exception to indicate that the request is rejected.</returns>
        internal static DataServiceException CreateDeepRecursion_General()
        {
            return DataServiceException.CreateBadRequestError(Strings.BadRequest_DeepRecursion_General);
        }

        /// <summary>Creates a new "Forbidden" exception.</summary>
        /// <returns>A new exception to indicate that the request is forbidden.</returns>
        internal static DataServiceException CreateForbidden()
        {
            // 403: Forbidden
            return new DataServiceException(403, Strings.RequestUriProcessor_Forbidden);
        }

        /// <summary>Creates a new "Resource Not Found" exception.</summary>
        /// <param name="identifier">segment identifier information for which resource was not found.</param>
        /// <returns>A new exception to indicate the requested resource cannot be found.</returns>
        internal static DataServiceException CreateResourceNotFound(string identifier)
        {
            // 404: Not Found
            return new DataServiceException(404, Strings.RequestUriProcessor_ResourceNotFound(identifier));
        }

        /// <summary>Creates a new "Resource Not Found" exception.</summary>
        /// <param name="errorMessage">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate the requested resource cannot be found.</returns>
        internal static DataServiceException ResourceNotFoundError(string errorMessage)
        {
            // 404: Not Found
            return new DataServiceException(404, errorMessage);
        }

        /// <summary>Creates a new exception to indicate a syntax error.</summary>
        /// <returns>A new exception to indicate a syntax error.</returns>
        internal static DataServiceException CreateSyntaxError()
        {
            return CreateSyntaxError(Strings.RequestUriProcessor_SyntaxError);
        }

        /// <summary>Creates a new exception to indicate a syntax error.</summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate a syntax error.</returns>
        internal static DataServiceException CreateSyntaxError(string message)
        {
            return DataServiceException.CreateBadRequestError(message);
        }

        /// <summary>
        /// Creates a new exception to indicate Precondition error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate a Precondition failed error.</returns>
        internal static DataServiceException CreatePreConditionFailedError(string message)
        {
            // 412 - Precondition failed
            return new DataServiceException(412, message);
        }

        /// <summary>
        /// Creates a new exception to indicate Precondition error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Inner Exception.</param>
        /// <returns>A new exception to indicate a Precondition failed error.</returns>
        internal static DataServiceException CreatePreConditionFailedError(string message, Exception innerException)
        {
            // 412 - Precondition failed
            return new DataServiceException(412, null, message, null, innerException);
        }

        /// <summary>
        /// Creates a new exception to indicate BadRequest error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate a bad request error.</returns>
        internal static DataServiceException CreateBadRequestError(string message)
        {
            // 400 - Bad Request
            return new DataServiceException(400, message);
        }

        /// <summary>
        /// Creates a new exception to indicate BadRequest error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Inner Exception.</param>
        /// <returns>A new exception to indicate a bad request error.</returns>
        internal static DataServiceException CreateBadRequestError(string message, Exception innerException)
        {
            // 400 - Bad Request
            return new DataServiceException(400, null, message, null, innerException);
        }

        /// <summary>Creates a new "Method Not Allowed" exception.</summary>
        /// <param name="message">Error message.</param>
        /// <param name="allow">String value for 'Allow' header in response.</param>
        /// <returns>A new exception to indicate the requested method is not allowed on the response.</returns>
        internal static DataServiceException CreateMethodNotAllowed(string message, string allow)
        {
            // 405 - Method Not Allowed
            DataServiceException result = new DataServiceException(405, message);
            result.state.ResponseAllowHeader = allow;
            return result;
        }

        /// <summary>
        /// Creates a new exception to indicate MethodNotImplemented error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate a MethodNotImplemented error.</returns>
        internal static DataServiceException CreateMethodNotImplemented(string message)
        {
            // 501 - Method Not Implemented
            return new DataServiceException(501, message);
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Contains the state of the exception, used for serialization in security transparent code.
        /// </summary>
        [Serializable]
        private struct DataServiceExceptionSerializationState : ISafeSerializationData
        {
            /// <summary>Gets or sets the language for the exception message.</summary>
            public string MessageLanguage { get; set; }

            /// <summary>Gets or sets the error code to be used in payloads.</summary>
            public string ErrorCode { get; set; }

            /// <summary>Gets or sets the HTTP response status code for this exception.</summary>
            public int StatusCode { get; set; }

            /// <summary>Gets or sets the 'Allow' response for header.</summary>
            public string ResponseAllowHeader { get; set; }

            /// <summary>
            /// Called when deserialization of the exception is complete.
            /// </summary>
            /// <param name="deserialized">The deserialized exception.</param>
            void ISafeSerializationData.CompleteDeserialization(object deserialized)
            {
                var exception = (DataServiceException)deserialized;
                exception.state = this;
            }
        }

        #endregion Nested Types
    }
}
