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
