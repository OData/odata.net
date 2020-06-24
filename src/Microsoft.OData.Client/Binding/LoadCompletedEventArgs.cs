//---------------------------------------------------------------------
// <copyright file="LoadCompletedEventArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces.
    using System;
    using System.ComponentModel;
    #endregion Namespaces.

    /// <summary>Used as the <see cref="System.EventArgs" /> class for the <see cref="Microsoft.OData.Client.DataServiceCollection{T}.LoadCompleted" /> event.Supported only by the WCF Data Services 5.0 client for Silverlight.</summary>
    public sealed class LoadCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>The <see cref="QueryOperationResponse"/> which represents
        /// the response for the Load operation.</summary>
        /// <remarks>This field is non-null only when the Load operation was successful.
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
        /// <returns>A <see cref="Microsoft.OData.Client.QueryOperationResponse" /> that represents the response to a load operation.</returns>
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