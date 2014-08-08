//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
