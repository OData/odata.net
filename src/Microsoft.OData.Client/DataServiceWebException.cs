//---------------------------------------------------------------------
// <copyright file="DataServiceWebException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using Microsoft.OData;

    /// <summary>
    /// Class to describe errors thrown by transport layer.
    /// </summary>
#if !PORTABLELIB
    [Serializable]
#endif
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "No longer relevant after .NET 4 introduction of SerializeObjectState event and ISafeSerializationData interface.")]
    public class DataServiceTransportException : InvalidOperationException
    {
        /// <summary>
        /// Contains the state for this exception.
        /// </summary>
#if !PORTABLELIB
        [NonSerialized]
#endif
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

#if !PORTABLELIB
            this.SerializeObjectState += (sender, e) => e.AddSerializedState(this.state);
#endif
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
#if !PORTABLELIB
        [Serializable]
#endif
        private struct DataServiceWebExceptionSerializationState
#if !PORTABLELIB
            : ISafeSerializationData
#endif
        {
            /// <summary>
            /// Gets or sets the response message for this exception.
            /// </summary>
            public IODataResponseMessage ResponseMessage { get; set; }

#if !PORTABLELIB
            /// <summary>
            /// Called when deserialization of the exception is complete.
            /// </summary>
            /// <param name="deserialized">The deserialized exception.</param>
            void ISafeSerializationData.CompleteDeserialization(object deserialized)
            {
                var exception = (DataServiceTransportException)deserialized;
                exception.state = this;
            }
#endif
        }
    }
}
