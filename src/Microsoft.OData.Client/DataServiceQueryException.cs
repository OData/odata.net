//---------------------------------------------------------------------
// <copyright file="DataServiceQueryException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Exception that indicates an error occurred while querying the data service. </summary>
    [System.Diagnostics.DebuggerDisplay("{Message}")]
    public sealed class DataServiceQueryException : InvalidOperationException
    {
        #region Private fields

        /// <summary>Actual response object.</summary>
        private readonly QueryOperationResponse response;

        #endregion Private fields

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceQueryException" /> class with a system-supplied message that describes the error. </summary>
        public DataServiceQueryException()
            : base(Strings.DataServiceException_GeneralError)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceQueryException" /> class with a specified message that describes the error. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.The string value that the contains error message.</param>
        public DataServiceQueryException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceQueryException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. The string value that contains the error message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. The inner exception object.</param>
        public DataServiceQueryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceQueryException" /> class. </summary>
        /// <param name="message">The string value that contains the error message.</param>
        /// <param name="innerException">The inner exception object.</param>
        /// <param name="response">The <see cref="T:Microsoft.OData.Client.QueryOperationResponse" /> object.</param>
        public DataServiceQueryException(string message, Exception innerException, QueryOperationResponse response)
            : base(message, innerException)
        {
            this.response = response;
        }

        #endregion Constructors

        #region Public properties

        /// <summary>Gets the <see cref="T:Microsoft.OData.Client.QueryOperationResponse" /> that indicates the exception results.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.QueryOperationResponse" /> object that indicates the exception results.</returns>
        public QueryOperationResponse Response
        {
            get { return this.response; }
        }

        #endregion Public properties
    }
}
