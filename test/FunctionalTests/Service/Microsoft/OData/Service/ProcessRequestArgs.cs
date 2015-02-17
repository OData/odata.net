//---------------------------------------------------------------------
// <copyright file="ProcessRequestArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
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
        /// <returns>An <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> object that contains information about the current operation. </returns>
        public DataServiceOperationContext OperationContext
        {
            get;
            private set;
        }

        #endregion Public properties
    }
}
