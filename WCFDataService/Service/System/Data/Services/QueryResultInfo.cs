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
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>
    /// Keeps all the information about the query results.
    /// </summary>
    internal class QueryResultInfo : IDisposable
    {
        /// <summary>
        /// Actuals results from the provider.
        /// </summary>
        private readonly IEnumerable results;

        /// <summary>
        /// Query that needs to be disposed.
        /// </summary>
        private readonly QueryResultInfo originalQueryResults;

        /// <summary>
        /// Enumerator instance that we got from the results.
        /// </summary>
        private IEnumerator enumerator;

        /// <summary>
        /// Keeps track whether the enumerator has been successfully advanced to the first element.
        /// </summary>
        private bool hasMoved;

        /// <summary>
        /// Creates a new instance of QueryResultInfo.
        /// </summary>
        /// <param name="queryResults">Query results that we need to enumerate and serialize.</param>
        internal QueryResultInfo(IEnumerable queryResults)
        {
            Debug.Assert(queryResults != null, "queryResults != null");
            this.results = queryResults;
        }

        /// <summary>
        /// Creates a new instance of QueryResultInfo.
        /// For performance reasons we reuse results from existing query to read a projected value. We create an enumerator
        /// containing the projected value but must not dispose the original query until later. This wrapper allows us to 
        /// pass the created enumerator and dispose the query at the right time. 
        /// </summary>
        /// <param name="queryResults">Query results that we need to enumerate and serialize.</param>
        /// <param name="originalResults">Actual query that we need to dispose.</param>
        internal QueryResultInfo(IEnumerable queryResults, QueryResultInfo originalResults) :
            this(queryResults)
        {
            this.originalQueryResults = originalResults;
        }

        /// <summary>
        /// Current instance from the result.
        /// </summary>
        public object Current
        {
            get
            {
                Debug.Assert(this.enumerator != null, "this.enumerator != null");
                return this.enumerator.Current;
            }
        }

        /// <summary>
        /// Returns true if the results has been moved forward.
        /// </summary>
        internal bool HasMoved
        {
            get
            {
                Debug.Assert(this.enumerator != null, "this.enumerator != null");
                return this.hasMoved;
            }
        }

        /// <summary>
        /// Read the next element from the results.
        /// </summary>
        /// <returns>true if there is a next element, otherwise returns false.</returns>
        public bool MoveNext()
        {
            if (this.enumerator == null)
            {
                this.enumerator = WebUtil.GetRequestEnumerator(this.results);
            }

            this.hasMoved = this.enumerator.MoveNext();
            return this.hasMoved;
        }

        /// <summary>
        /// Dispose the enumerator and the innerQueryResults, if specified.
        /// The outer enumerable must be disposed by the ResponseBodyWriter.
        /// </summary>
        public void Dispose()
        {
            if (this.enumerator != null)
            {
                WebUtil.Dispose(this.enumerator);
            }

            if (this.originalQueryResults != null)
            {
                this.originalQueryResults.Dispose();
            }
        }

        /// <summary>
        /// Checks the results enumerator for IExpandedResult.
        /// </summary>
        /// <returns>an instance of IExpandedResult.</returns>
        internal IExpandedResult AsIExpandedResult()
        {
            Debug.Assert(this.enumerator != null, "this.enumerator != null");
            return this.enumerator as IExpandedResult;
        }

        /// <summary>
        /// Get the continuation token from the paging provider.
        /// </summary>
        /// <param name="pagingProvider">Instance of DataServicePagingProviderWrapper instance.</param>
        /// <returns>list of the continuation tokens as provided by the wrapper.</returns>
        internal object[] GetContinuationTokenFromPagingProvider(DataServicePagingProviderWrapper pagingProvider)
        {
            return pagingProvider.PagingProviderInterface.GetContinuationToken(BasicExpandProvider.ExpandedEnumerator.UnwrapEnumerator(this.enumerator));
        }

        /// <summary>
        /// Forms an instance of DataServiceODataWriterFeedArgs that contains results.
        /// </summary>
        /// <param name="feed">ODataFeed instance.</param>
        /// <param name="operationContext">DataServicesOperationContext instance.</param>
        /// <returns>DataServiceODataWriterFeedArgs instance</returns>
        internal DataServiceODataWriterFeedArgs GetDataServiceODataWriterFeedArgs(ODataFeed feed, DataServiceOperationContext operationContext)
        {
            return new DataServiceODataWriterFeedArgs(feed, this.results, operationContext);
        }
    }
}
