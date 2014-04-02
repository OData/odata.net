//---------------------------------------------------------------------
// <copyright file="WebException.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Exception for web problems.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
{
    using System;

    /// <summary>
    /// The exception that is thrown when an error occurs while accessing the 
    /// network through a pluggable protocol.
    /// </summary>
    internal sealed class WebException : InvalidOperationException
    {
        /// <summary>
        /// The actual response which caused this exception (if available)
        /// </summary>
        private HttpWebResponse response;

        /// <summary>
        /// Initializes a new instance of the WebException class.
        /// </summary>
        public WebException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebException class with the specified error message.
        /// </summary>
        /// <param name="message">The text of the error message. </param>
        public WebException(string message) : this(message, (Exception)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebException class with the specified error message and nested exception.
        /// </summary>
        /// <param name="message">The text of the error message. </param>
        /// <param name="innerException">A nested exception.</param>
        public WebException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebException class with the specified error message and nested exception.
        /// </summary>
        /// <param name="message">The text of the error message. </param>
        /// <param name="innerException">A nested exception.</param>
        /// <param name="response">The actual response which caused this exception (if available)</param>
        public WebException(string message, Exception innerException, HttpWebResponse response)
            : base(message, innerException)
        {
            this.response = response;
        }

        /// <summary>Gets the response that the remote host returned.</summary>
        public Microsoft.OData.Service.Http.HttpWebResponse Response
        {
            get
            {
                return this.response;
            }
        }

        /// <summary>
        /// Creates a new WebException with a generic error message.
        /// </summary>
        /// <param name="location">Location of internal error, typically "Type.Method".</param>
        /// <returns>A new WebException instance.</returns>
        internal static WebException CreateInternal(string location)
        {
            return new WebException(Microsoft.OData.Client.Strings.HttpWeb_Internal(location));
        }
    }
}
