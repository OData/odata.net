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

namespace System.Data.Services.Providers
{
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Wrapper class for IDataServiceExecutionProvider interface.
    /// </summary>
    internal class DataServiceExecutionProviderWrapper
    {
        #region Private Fields

        /// <summary>
        /// Data service instance
        /// </summary>
        private readonly IDataService dataService;

        /// <summary>
        /// Execution provider instance
        /// </summary>
        private IDataServiceExecutionProvider executionProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the wrapper class for IDataServiceExecutionProvider
        /// </summary>
        /// <param name="dataService">The IDataService associated with this execution provider.</param>
        public DataServiceExecutionProviderWrapper(IDataService dataService)
        {
            Debug.Assert(dataService != null, "dataService != null");
            this.dataService = dataService;
        }

        #endregion

        /// <summary>
        /// Gets the default DataServiceExecutionProvider.
        /// </summary>
        private IDataServiceExecutionProvider ExecutionProvider
        {
            get
            {
                if (this.executionProvider == null)
                {
                    this.executionProvider = new DataServiceExecutionProvider();
                }

                return this.executionProvider;
            }
        }

        #region Internal Methods

        /// <summary>
        /// Get the single result from the given segment info
        /// </summary>
        /// <param name="segmentInfo">segmentInfo which contains the request query</param>
        /// <returns>query result as returned by the IQueryable query</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing the Message")]
        internal static QueryResultInfo GetSingleResultFromRequest(SegmentInfo segmentInfo)
        {
            Debug.Assert(segmentInfo != null && segmentInfo.RequestEnumerable != null, "segmentInfo != null && segmentInfo.RequestEnumerable != null");
            var queryResults = new QueryResultInfo(segmentInfo.RequestEnumerable);
            try
            {
                WebUtil.CheckResourceExists(queryResults.MoveNext(), segmentInfo.Identifier);
                WebUtil.CheckNullDirectReference(queryResults.Current, segmentInfo);
                return queryResults;
            }
            catch
            {
                // Dispose the Enumerator in case of error
                WebUtil.Dispose(queryResults);
                throw;
            }
        }

        /// <summary>
        /// Invokes the given request expression and return the resulting IEnumerable.
        /// </summary>
        /// <param name="segmentInfo">Request segment</param>
        /// <returns>Result enumeration</returns>
        internal IEnumerable GetResultEnumerableFromRequest(SegmentInfo segmentInfo)
        {
            object result = this.Execute(segmentInfo.RequestExpression);

            // This is a hook for exposing the query out to debuggers and tests.
            IQueryable query = result as IQueryable;
            if (query != null)
            {
                this.dataService.InternalOnRequestQueryConstructed(query);
            }

            if ((segmentInfo.SingleResult && query == null) || result is IDataServiceInvokable)
            {
                // If the result is a single object, we wrap it in an object array.
                result = new object[] { result };
            }

            return (IEnumerable)result;
        }

        /// <summary>
        /// Passes the expression along to the execution provider, which invokes it. 
        /// </summary>
        /// <seealso cref="IDataServiceExecutionProvider"/>
        /// <param name="requestExpression"> An expression that includes calls to
        /// one or more MethodInfo or one or more calls to 
        /// IDataServiceUpdateProvider2.InvokeAction(..) or 
        /// IDataServiceQueryProvider2.InvokeFunction(..)</param>
        /// <returns>The object the invoked expression returns.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "operationContext", Justification = "Will be part of public API.")]
        internal object Execute(Expression requestExpression)
        {
            Debug.Assert(requestExpression != null, "requestExpression != null");
            this.dataService.ProcessingPipeline.AssertDebugStateAtExecuteExpression(this.dataService);

            return this.ExecutionProvider.Execute(requestExpression, this.dataService.OperationContext);
        }

        #endregion
    }
}
