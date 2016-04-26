//---------------------------------------------------------------------
// <copyright file="DataServiceODataWriterFeedArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Collections;
    using System.Diagnostics;
    using Microsoft.OData;

    /// <summary>
    /// Class that keeps track of the ODataResourceSet, collection instance and other information
    /// that we need to provide to the service author when they choose to provide their own
    /// instance of ODataWriter.
    /// </summary>
    public sealed class DataServiceODataWriterFeedArgs
    {
        /// <summary>
        /// Creates a new instance of DataServiceODataWriterFeedArgs
        /// </summary>
        /// <param name="feed">ODataResourceSet instance.</param>
        /// <param name="results">IEnumerable instance that is getting serialized.</param>
        /// <param name="operationContext">DataServiceOperationContext instance.</param>
        public DataServiceODataWriterFeedArgs(ODataResourceSet resourceCollection, IEnumerable results, DataServiceOperationContext operationContext)
        {
            WebUtil.CheckArgumentNull(resourceCollection, "feed");
            Debug.Assert(results != null, "results != null");
            Debug.Assert(operationContext != null, "operationContext != null");
            this.Feed = resourceCollection;
            this.Results = results;
            this.OperationContext = operationContext;
        }

        /// <summary>
        /// Gets the instance of ODataResourceSet that is going to get serialized.
        /// </summary>
        public ODataResourceSet Feed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the IEnumerable instance which represent the collection that is getting serialized.
        /// </summary>
        public IEnumerable Results
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the instance of DataServiceOperationContext.
        /// </summary>
        public DataServiceOperationContext OperationContext
        {
            get;
            private set;
        }
    }
}
