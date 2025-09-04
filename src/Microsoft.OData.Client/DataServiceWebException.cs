//---------------------------------------------------------------------
// <copyright file="DataServiceWebException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData;

    /// <summary>
    /// Exception thrown when the underlying transport layer encounters an error while sending a request
    /// or receiving a response.
    /// </summary>
    /// <remarks>
    /// This exception wraps the original transport error (<see cref="Exception.InnerException"/>).
    /// When available, the raw response is exposed via <see cref="Response"/> to allow callers to inspect
    /// status code, headers, or read an error payload.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "Intentionally specialized transport-layer exception. It requires an inner exception and optionally an IODataResponseMessage to preserve response details (status code, headers, and error payload) for diagnostics.")]
    public class DataServiceTransportException : InvalidOperationException
    {
        private readonly IODataResponseMessage responseMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceTransportException"/> class with the response
        /// message and the underlying transport exception.
        /// </summary>
        /// <param name="response">
        /// The <see cref="IODataResponseMessage"/> associated with the failure, if available.
        /// This enables callers to inspect status code, headers, or read an error payload.
        /// May be <c>null</c> if no response was produced.
        /// </param>
        /// <param name="innerException">The exception that caused this transport error.</param>
        public DataServiceTransportException(IODataResponseMessage response, Exception innerException)
            : base(innerException.Message, innerException)
        {
            Util.CheckArgumentNull(innerException, "innerException");

            this.responseMessage = response;
        }

        /// <summary>
        /// Gets the response message associated with this exception.
        /// </summary>
        public IODataResponseMessage Response
        {
            get { return this.responseMessage; }
        }
    }
}