//   OData .NET Libraries ver. 6.9
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

    /// <summary>Represents the error thrown if the data service returns a response code less than 200 or greater than 299, or the top-level element in the response is &lt;error&gt;. This class cannot be inherited.</summary>
#if !ASTORIA_LIGHT && !PORTABLELIB
    [Serializable]
#endif
    [System.Diagnostics.DebuggerDisplay("{Message}")]
    public sealed class DataServiceRequestException : InvalidOperationException
    {
        /// <summary>Actual response object.</summary>
#if !ASTORIA_LIGHT && !PORTABLELIB
        [NonSerialized]
#endif
        private readonly DataServiceResponse response;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestException" /> class with a system-supplied message that describes the error. </summary>
        public DataServiceRequestException()
            : base(Strings.DataServiceException_GeneralError)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestException" /> class with a specified message that describes the error. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.The error message text.</param>
        public DataServiceRequestException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception. </summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
        public DataServiceRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestException" /> class. </summary>
        /// <param name="message">Error message text.</param>
        /// <param name="innerException">Exception object that contains the inner exception.</param>
        /// <param name="response"><see cref="T:Microsoft.OData.Client.DataServiceResponse" /> object.</param>
        public DataServiceRequestException(string message, Exception innerException, DataServiceResponse response)
            : base(message, innerException)
        {
            this.response = response;
        }

#if !ASTORIA_LIGHT && !PORTABLELIB
#pragma warning disable 0628
        /// <summary>
        /// Initializes a new instance of the DataServiceQueryException class from the 
        /// specified SerializationInfo and StreamingContext instances.
        /// </summary>
        /// <param name="info">
        /// A SerializationInfo containing the information required to serialize 
        /// the new DataServiceException.</param>
        /// <param name="context">
        /// A StreamingContext containing the source of the serialized stream 
        /// associated with the new DataServiceException.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        protected DataServiceRequestException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
#pragma warning restore 0628
#endif

        #endregion Constructors

        #region Public properties

        /// <summary>Gets the response as a <see cref="T:Microsoft.OData.Client.DataServiceResponse" /> object. </summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.DataServiceResponse" /> object.</returns>
        public DataServiceResponse Response
        {
            get { return this.response; }
        }

        #endregion Public properties
    }
}
