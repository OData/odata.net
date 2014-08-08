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
    using System.Collections;
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>
    /// Class that keeps track of the ODataFeed, collection instance and other information
    /// that we need to provide to the service author when they choose to provide their own
    /// instance of ODataWriter.
    /// </summary>
    public sealed class DataServiceODataWriterFeedArgs
    {
        /// <summary>
        /// Creates a new instance of DataServiceODataWriterFeedArgs
        /// </summary>
        /// <param name="feed">ODataFeed instance.</param>
        /// <param name="results">IEnumerable instance that is getting serialized.</param>
        /// <param name="operationContext">DataServiceOperationContext instance.</param>
        public DataServiceODataWriterFeedArgs(ODataFeed feed, IEnumerable results, DataServiceOperationContext operationContext)
        {
            WebUtil.CheckArgumentNull(feed, "feed");
            Debug.Assert(results != null, "results != null");
            Debug.Assert(operationContext != null, "operationContext != null");
            this.Feed = feed;
            this.Results = results;
            this.OperationContext = operationContext;
        }

        /// <summary>
        /// Gets the instance of ODataFeed that is going to get serialized.
        /// </summary>
        public ODataFeed Feed
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
