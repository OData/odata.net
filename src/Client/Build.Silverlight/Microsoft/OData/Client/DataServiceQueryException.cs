//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Exception that indicates an error occurred while querying the data service. </summary>
#if !ASTORIA_LIGHT && !PORTABLELIB
    [Serializable]
#endif
    [System.Diagnostics.DebuggerDisplay("{Message}")]
    public sealed class DataServiceQueryException : InvalidOperationException
    {
        #region Private fields

        /// <summary>Actual response object.</summary>
#if !ASTORIA_LIGHT  && !PORTABLELIB
        [NonSerialized]
#endif
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

#if !ASTORIA_LIGHT && !PORTABLELIB
#pragma warning disable 0628
        /// <summary>
        /// Initializes a new instance of the DataServiceQueryException class from the 
        /// specified SerializationInfo and StreamingContext instances.
        /// </summary>
        /// <param name="info">
        /// A SerializationInfo containing the information required to serialize 
        /// the new DataServiceQueryException.</param>
        /// <param name="context">
        /// A StreamingContext containing the source of the serialized stream 
        /// associated with the new DataServiceQueryException.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1047", Justification = "Follows serialization info pattern.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032", Justification = "Follows serialization info pattern.")]
        protected DataServiceQueryException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
#pragma warning restore 0628
#endif

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
