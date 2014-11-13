//   OData .NET Libraries ver. 5.6.3
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

namespace System.Data.Services
{
    using System;

    /// <summary>Use this class to look at the request uri and doing some custom validation.</summary>
    public sealed class ProcessRequestArgs
    {
        #region Constructors

        /// <summary>Initalizes a new <see cref="ProcessRequestArgs"/> instance.</summary>
        /// <param name="operationContext">Context about the current operation being processed.</param>
        internal ProcessRequestArgs(DataServiceOperationContext operationContext)
        {
            this.OperationContext = operationContext;
        }

        #endregion Constructors

        #region Public properties

        /// <summary>Gets the URI of an HTTP request to be process.</summary>
        /// <returns>A <see cref="T:System.Uri" /> that contains the URI of the request to be processed.</returns>
        public Uri RequestUri
        {
            get { return this.OperationContext.AbsoluteRequestUri; }
            set { this.OperationContext.AbsoluteRequestUri = value; }
        }

        /// <summary>The absolute base URI of the service.</summary>
        public Uri ServiceUri
        {
            get { return this.OperationContext.AbsoluteServiceUri; }
            set { this.OperationContext.AbsoluteServiceUri = value; }
        }

        /// <summary>Gets a Boolean value that indicates whether the HTTP request to the data service is a batch operation.</summary>
        /// <returns>The Boolean value that indicates whether the HTTP request to the data service is a batch operation. </returns>
        public bool IsBatchOperation
        {
            get { return this.OperationContext.IsInnerBatchOperation; }
        }

        /// <summary>Gets the context that contains information about the current operation being processed.</summary>
        /// <returns>An <see cref="T:System.Data.Services.DataServiceOperationContext" /> object that contains information about the current operation. </returns>
        public DataServiceOperationContext OperationContext
        {
            get;
            private set;
        }

        #endregion Public properties
    }
}
