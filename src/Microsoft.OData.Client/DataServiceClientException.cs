//---------------------------------------------------------------------
// <copyright file="DataServiceClientException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>
    /// The exception that is thrown when the server returns an error.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Message}")]
    public sealed class DataServiceClientException : InvalidOperationException
    {
        private readonly int statusCode;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.DataServiceClientException" /> class with a system-supplied message that describes the error. </summary>
        public DataServiceClientException()
            : this(SRResources.DataServiceException_GeneralError)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.DataServiceClientException" /> class with a specified message that describes the error. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public DataServiceClientException(string message)
            : this(message, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.DataServiceClientException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
        public DataServiceClientException(string message, Exception innerException)
            : this(message, innerException, 500)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.DataServiceClientException" /> class. </summary>
        /// <param name="message">The string value that contains the error message.</param>
        /// <param name="statusCode">The integer value that contains status code.</param>
        public DataServiceClientException(string message, int statusCode)
            : this(message, null, statusCode)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.DataServiceClientException" /> class. </summary>
        /// <param name="message">The string value that contains the error message.</param>
        /// <param name="innerException">The System.Exception object that contains the inner exception.</param>
        /// <param name="statusCode">The integer value that contains status code.</param>
        public DataServiceClientException(string message, Exception innerException, int statusCode)
            : base(message, innerException)
        {
            this.statusCode = statusCode;
        }

        #endregion Constructors


        #region Public properties

        /// <summary>Gets the HTTP error status code returned after <see cref="Microsoft.OData.Client.DataServiceClientException" />.</summary>
        /// <returns>An integer value that represents the exception.</returns>
        public int StatusCode
        {
            get { return this.statusCode; }
        }

        #endregion Public properties
    }
}