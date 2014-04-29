//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Operation response base class</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010", Justification = "required for this feature")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710", Justification = "required for this feature")]
    public abstract class OperationResponse
    {
        /// <summary>Http headers of the response.</summary>
        private readonly HeaderCollection headers;

        /// <summary>Http status code of the response.</summary>
        private int statusCode;

        /// <summary>exception to throw during get results</summary>
        private Exception innerException;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="headers">HTTP headers</param>
        internal OperationResponse(HeaderCollection headers)
        {
            Debug.Assert(null != headers, "null headers");
            this.headers = headers;
        }

        /// <summary>When overridden in a derived class, contains the HTTP response headers associated with a single operation.</summary>
        /// <returns><see cref="T:System.Collections.IDictionary" /> object that contains name value pairs of headers and values.</returns>
        public IDictionary<string, string> Headers
        {
            get { return this.headers.UnderlyingDictionary; }
        }

        /// <summary>When overridden in a derived class, gets or sets the HTTP response code associated with a single operation.</summary>
        /// <returns>Integer value that contains response code.</returns>
        public int StatusCode
        {
            get { return this.statusCode; }
            internal set { this.statusCode = value; }
        }

        /// <summary>Gets error thrown by the operation.</summary>
        /// <returns>An <see cref="T:System.Exception" /> object that contains the error.</returns>
        public Exception Error
        {
            get
            {
                return this.innerException;
            }

            set
            {
                this.innerException = value;
            }
        }

        /// <summary>Http headers of the response.</summary>
        internal HeaderCollection HeaderCollection
        {
            get { return this.headers; }
        }
    }
}
