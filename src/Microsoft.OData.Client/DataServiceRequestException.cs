//---------------------------------------------------------------------
// <copyright file="DataServiceRequestException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;

    /// <summary>Represents the error thrown if the data service returns a response code less than 200 or greater than 299, or the top-level element in the response is &lt;error&gt;. This class cannot be inherited.</summary>
    [DebuggerDisplay("{Message}")]
    public sealed class DataServiceRequestException : InvalidOperationException
    {
        /// <summary>Actual response object.</summary>
        [NonSerialized]
        private readonly DataServiceResponse response;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="DataServiceRequestException" /> class with a system-supplied message that describes the error. </summary>
        public DataServiceRequestException()
            : base(SRResources.DataServiceException_GeneralError)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DataServiceRequestException" /> class with a specified message that describes the error. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.The error message text.</param>
        public DataServiceRequestException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DataServiceRequestException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
        public DataServiceRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DataServiceRequestException" /> class. </summary>
        /// <param name="message">Error message text.</param>
        /// <param name="innerException">Exception object that contains the inner exception.</param>
        /// <param name="response"><see cref="DataServiceResponse" /> object.</param>
        public DataServiceRequestException(string message, Exception innerException, DataServiceResponse response)
            : base(message, innerException)
        {
            this.response = response;
        }

        #endregion Constructors

        #region Public properties

        /// <summary>Gets the response as a <see cref="DataServiceResponse" /> object. </summary>
        /// <returns>A <see cref="DataServiceResponse" /> object.</returns>
        public DataServiceResponse Response
        {
            get { return this.response; }
        }

        #endregion Public properties
    }
}
