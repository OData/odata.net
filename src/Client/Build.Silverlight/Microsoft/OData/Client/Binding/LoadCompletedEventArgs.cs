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
    #region Namespaces.
    using System;
    using System.ComponentModel;
    #endregion Namespaces.

    /// <summary>Used as the <see cref="T:System.EventArgs" /> class for the <see cref="E:Microsoft.OData.Service.Client.DataServiceCollection`1.LoadCompleted" /> event.Supported only by the WCF Data Services 5.0 client for Silverlight.</summary>
    public sealed class LoadCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>The <see cref="QueryOperationResponse"/> which represents
        /// the response for the Load operation.</summary>
        /// <remarks>This field is non-null only when the Load operation was successfull.
        /// Otherwise it's null.</remarks>
        private QueryOperationResponse queryOperationResponse;

        /// <summary>Constructor</summary>
        /// <param name="queryOperationResponse">The response for the Load operation. null when the operation didn't succeed.</param>
        /// <param name="error"><see cref="Exception"/> which represents the error if the Load operation failed. null if the operation
        /// didn't fail.</param>
        /// <remarks>This constructor doesn't allow creation of canceled event args.</remarks>
        internal LoadCompletedEventArgs(QueryOperationResponse queryOperationResponse, Exception error)
            : this(queryOperationResponse, error, false)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="queryOperationResponse">The response for the Load operation. null when the operation didn't succeed.</param>
        /// <param name="error"><see cref="Exception"/> which represents the error if the Load operation failed. null if the operation
        /// didn't fail.</param>
        /// <param name="cancelled">True, if the LoadAsync operation was cancelled, False otherwise.</param>
        /// <remarks>This constructor doesn't allow creation of canceled event args.</remarks>
        internal LoadCompletedEventArgs(
            QueryOperationResponse queryOperationResponse,
            Exception error,
            bool cancelled)
            : base(error, cancelled, null)
        {
            this.queryOperationResponse = queryOperationResponse;
        }

        /// <summary>Gets the response to an asynchronous load operation.Supported only by the WCF Data Services 5.0 client for Silverlight.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Service.Client.QueryOperationResponse" /> that represents the response to a load operation.</returns>
        /// <remarks>Accessing this property will throw exception if the Load operation failed
        /// or it was canceled.</remarks>
        public QueryOperationResponse QueryOperationResponse
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return this.queryOperationResponse;
            }
        }
    }
}
