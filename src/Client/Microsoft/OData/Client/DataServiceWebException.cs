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
