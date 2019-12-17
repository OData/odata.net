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
    /// Class to describe errors thrown by transport layer.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "No longer relevant after .NET 4 introduction of SerializeObjectState event and ISafeSerializationData interface.")]
    public class DataServiceTransportException : InvalidOperationException
    {
        /// <summary>
        /// Contains the state for this exception.
        /// </summary>
        private DataServiceWebExceptionSerializationState state;

        /// <summary>
        /// Constructs a new instance of DataServiceTransportException.
        /// </summary>
        /// <param name="response">ResponseMessage from the exception so that the error payload can be read.</param>
        /// <param name="innerException">Actual exception that this exception is wrapping.</param>
        public DataServiceTransportException(IODataResponseMessage response, Exception innerException)
            : base(innerException.Message, innerException)
        {
            Util.CheckArgumentNull(innerException, "innerException");

            this.state.ResponseMessage = response;
        }

        /// <summary>
        /// Gets the response message for this exception.
        /// </summary>
        public IODataResponseMessage Response
        {
            get { return this.state.ResponseMessage; }
        }

        /// <summary>
        /// Contains the state of the exception, used for serialization in security transparent code.
        /// </summary>
        private struct DataServiceWebExceptionSerializationState
        {
            /// <summary>
            /// Gets or sets the response message for this exception.
            /// </summary>
            public IODataResponseMessage ResponseMessage { get; set; }
        }
    }
}
