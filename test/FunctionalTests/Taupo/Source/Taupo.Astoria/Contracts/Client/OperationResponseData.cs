//---------------------------------------------------------------------
// <copyright file="OperationResponseData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Abstract class that represents the response data of a single query or create, update, or delete operation.
    /// </summary>
    public abstract class OperationResponseData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResponseData"/> class.
        /// </summary>
        protected OperationResponseData()
        {
            this.Headers = new Dictionary<string, string>();
        }
        
        /// <summary>
        /// Gets or sets the exception identity for the operation.
        /// </summary>
        public string ExceptionId { get; set; }

        /// <summary>
        /// Gets a dictionay that contains the HTTP response headers associated
        /// with a single operation.
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }
        
        /// <summary>
        /// Gets or sets the HTTP response code associated with a single operation.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
