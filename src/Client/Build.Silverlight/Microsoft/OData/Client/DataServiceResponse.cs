//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Data service response to ExecuteBatch &amp; SaveChanges
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010", Justification = "required for this feature")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710", Justification = "required for this feature")]
    public sealed class DataServiceResponse : IEnumerable<OperationResponse>
    {
        /// <summary>Http headers of the response.</summary>
        private readonly HeaderCollection headers;

        /// <summary>Http status code of the response.</summary>
        private readonly int statusCode;

        /// <summary>responses</summary>
        private readonly IEnumerable<OperationResponse> response;

        /// <summary>true if this is a batch response, otherwise false.</summary>
        private readonly bool batchResponse;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="headers">HTTP headers</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="response">list of responses</param>
        /// <param name="batchResponse">true if this represents a batch response, otherwise false.</param>
        internal DataServiceResponse(HeaderCollection headers, int statusCode, IEnumerable<OperationResponse> response, bool batchResponse)
        {
            Debug.Assert(response != null, "response!=null");
            this.headers = headers ?? new HeaderCollection();
            this.statusCode = statusCode;
            this.batchResponse = batchResponse;
            this.response = response;
        }

        /// <summary>The headers from an HTTP response associated with a batch request.</summary>
        /// <returns>An <see cref="T:System.Collections.IDictionary" /> object containing the name-value pairs of an HTTP response.</returns>
        public IDictionary<string, string> BatchHeaders
        {
            get { return this.headers.UnderlyingDictionary; }
        }

        /// <summary>The status code from an HTTP response associated with a batch request.</summary>
        /// <returns>An integer based on status codes defined in Hypertext Transfer Protocol.</returns>
        public int BatchStatusCode
        {
            get { return this.statusCode; }
        }

        /// <summary>Gets a Boolean value that indicates whether the response contains multiple results.</summary>
        /// <returns>A Boolean value that indicates whether the response contains multiple results.</returns>
        public bool IsBatchResponse
        {
            get { return this.batchResponse; }
        }

        /// <summary>Gets an enumerator that enables retrieval of responses to operations being tracked by <see cref="T:Microsoft.OData.Client.OperationResponse" /> objects within the <see cref="T:Microsoft.OData.Client.DataServiceResponse" />.</summary>
        /// <returns>An enumerator over the response received from the service.</returns>
        public IEnumerator<OperationResponse> GetEnumerator()
        {
            return this.response.GetEnumerator();
        }

        /// <summary>Gets an enumerator that enables retrieval of responses to operations being tracked by <see cref="T:Microsoft.OData.Client.OperationResponse" /> objects within the <see cref="T:Microsoft.OData.Client.DataServiceResponse" />.</summary>
        /// <returns>An enumerator over the response received from the service.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
