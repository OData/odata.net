//---------------------------------------------------------------------
// <copyright file="OperationResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
            Debug.Assert(headers != null, "null headers");
            this.headers = headers;
        }

        /// <summary>When overridden in a derived class, contains the HTTP response headers associated with a single operation.</summary>
        /// <returns><see cref="System.Collections.IDictionary" /> object that contains name value pairs of headers and values.</returns>
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
        /// <returns>An <see cref="System.Exception" /> object that contains the error.</returns>
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
