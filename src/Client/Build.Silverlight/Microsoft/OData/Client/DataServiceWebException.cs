//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
#if !ASTORIA_LIGHT
    using System.Runtime.Serialization;
#endif
    using Microsoft.OData.Core;

    /// <summary>
    /// Class to describe errors thrown by transport layer.
    /// </summary>
#if !ASTORIA_LIGHT && !PORTABLELIB
    [Serializable]
#endif
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "No longer relevant after .NET 4 introduction of SerializeObjectState event and ISafeSerializationData interface.")]
    public class DataServiceTransportException : InvalidOperationException
    {
        /// <summary>
        /// Contains the state for this exception.
        /// </summary>
#if !ASTORIA_LIGHT  && !PORTABLELIB
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

#if !ASTORIA_LIGHT && !PORTABLELIB
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
#if !ASTORIA_LIGHT  && !PORTABLELIB
        [Serializable]
#endif
        private struct DataServiceWebExceptionSerializationState
#if !ASTORIA_LIGHT  && !PORTABLELIB
            : ISafeSerializationData
#endif
        {
            /// <summary>
            /// Gets or sets the response message for this exception.
            /// </summary>
            public IODataResponseMessage ResponseMessage { get; set; }

#if !ASTORIA_LIGHT && !PORTABLELIB
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
