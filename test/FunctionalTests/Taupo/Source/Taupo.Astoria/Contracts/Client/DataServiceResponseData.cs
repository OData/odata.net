//---------------------------------------------------------------------
// <copyright file="DataServiceResponseData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents the response data of the response to operations sent to the data service.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Following naming convention of the product (i.e. DataServiceResponse class) which also does not have 'Collection' suffix.")]
    public sealed class DataServiceResponseData : IEnumerable<OperationResponseData>
    {
        private List<OperationResponseData> operationResponses;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceResponseData"/> class.
        /// </summary>
        public DataServiceResponseData()
        {
            this.BatchHeaders = new Dictionary<string, string>();
            this.operationResponses = new List<OperationResponseData>();
        }

        /// <summary>
        /// Gets the dictionary representing headers from an HTTP response associated with a batch request.
        /// </summary>
        public IDictionary<string, string> BatchHeaders { get; private set; }
 
        /// <summary>
        /// Gets or sets the status code from an HTTP response associated with a batch request.
        /// </summary>
        public int BatchStatusCode { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the response contains multiple results.
        /// </summary>
        public bool IsBatchResponse { get; set; }

        /// <summary>
        /// Gets the number of response datas
        /// </summary>
        internal int Count
        {
            get
            {
                return this.operationResponses.Count;
            }
        }

        /// <summary>
        /// Gets the operation response data at the given index
        /// </summary>
        /// <param name="index">The given index</param>
        /// <returns>The operation response data</returns>
        internal OperationResponseData this[int index]
        {
            get
            {
                return this.operationResponses[index];
            }
        }

        /// <summary>
        /// Adds the specified operation response data.
        /// </summary>
        /// <param name="operationResponseData">The operation response data.</param>
        public void Add(OperationResponseData operationResponseData)
        {
            ExceptionUtilities.CheckArgumentNotNull(operationResponseData, "operationResponseData");

            this.operationResponses.Add(operationResponseData);
        }

        /// <summary>
        /// Gets an enumerator over the operation responses data within this data service response
        /// </summary>
        /// <returns>An enumerator over the operation responses data within this data service response.</returns>
        public IEnumerator<OperationResponseData> GetEnumerator()
        {
            return this.operationResponses.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator over the operation responses data within this data service response
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/>An enumerator over the operation responses data within this data service response.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
