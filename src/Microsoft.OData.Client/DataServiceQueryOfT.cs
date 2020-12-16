//---------------------------------------------------------------------
// <copyright file="DataServiceQueryOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// query object
    /// </summary>
    /// <typeparam name="TElement">type of object to materialize</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "required for this feature")]
    public class DataServiceQuery<TElement> : DataServiceQuery, IQueryable<TElement>
    {
        #region Private fields

        /// <summary>Method info for the v1 Expand method.</summary>
        private static readonly MethodInfo expandMethodInfo = typeof(DataServiceQuery<TElement>).GetMethod("Expand", new Type[] { typeof(string) });

        /// <summary>Method info for the generic version of the Expand method</summary>
#if PORTABLELIB
        private static readonly MethodInfo expandGenericMethodInfo = typeof(DataServiceQuery<TElement>).GetMethodWithGenericArgs("Expand", true /*isPublic*/, false /*isStatic*/, 1 /*genericArgCount*/);
#else
        private static readonly MethodInfo expandGenericMethodInfo = (MethodInfo)typeof(DataServiceQuery<TElement>).GetMember("Expand*").Single(m => ((MethodInfo)m).GetGenericArguments().Length == 1);
#endif

        /// <summary>Linq Expression</summary>
        private readonly Expression queryExpression;

        /// <summary>Linq Query Provider</summary>
        private readonly DataServiceQueryProvider queryProvider;

        /// <summary>Uri, Projection, Version for translated query</summary>
        private QueryComponents queryComponents;

        #endregion Private fields

        /// <summary>
        /// query object
        /// </summary>
        /// <param name="expression">expression for query</param>
        /// <param name="provider">query provider for query</param>
        public DataServiceQuery(Expression expression, DataServiceQueryProvider provider)
            : this(expression, provider, true)
        {
            this.IsFunction = false;
        }

        /// <summary>
        /// query object of a function which returns a collection of items
        /// </summary>
        /// <param name="expression">expression for query</param>
        /// <param name="provider">query provider for query</param>
        /// <param name="isComposable">whether this query is composable</param>
        public DataServiceQuery(Expression expression, DataServiceQueryProvider provider, bool isComposable)
        {
            Debug.Assert(provider.Context != null, "null context");
            Debug.Assert(expression != null, "null expression");
            Debug.Assert(provider is DataServiceQueryProvider, "Currently only support Web Query Provider");

            this.queryExpression = expression;
            this.queryProvider = provider;
            this.IsComposable = isComposable;
            this.IsFunction = true;
        }

        #region IQueryable implementation
        /// <summary>Returns the type of the object used in the template to create the <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> instance.</summary>
        /// <returns>Returns <see cref="System.Type" /> representing the type used in the template when the query is created.</returns>
        public override Type ElementType
        {
            get { return typeof(TElement); }
        }

        /// <summary>Represents an expression containing the query to the data service.</summary>
        /// <returns>A <see cref="System.Linq.Expressions.Expression" /> object representing the query.</returns>
        public override Expression Expression
        {
            get { return this.queryExpression; }
        }

        /// <summary>Represents the query provider instance.</summary>
        /// <returns>A <see cref="System.Linq.IQueryProvider" /> representing the data source provider.</returns>
        public override IQueryProvider Provider
        {
            get { return this.queryProvider; }
        }

        #endregion

        /// <summary>Get the URI for the query.</summary>
        /// <returns>The URI of the request.</returns>
        public override Uri RequestUri
        {
            get
            {
                return this.Translate().Uri;
            }

            internal set
            {
                this.Translate().Uri = value;
            }
        }

        /// <summary>Context associated with this query.</summary>
        public virtual DataServiceContext Context
        {
            get { return this.queryProvider.Context; }
        }

        /// <summary>
        /// Whether this query is composable
        /// </summary>
        public virtual bool IsComposable { get; private set; }

        /// <summary>
        /// The flag of whether this query is a function.
        /// </summary>
        internal bool IsFunction { get; private set; }

        /// <summary>The ProjectionPlan for the request (if precompiled in a previous page).</summary>
        internal override ProjectionPlan Plan
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a new URI string with keys.
        /// </summary>
        /// <param name="keyString">The string representing keys.</param>
        /// <returns>The new URI string with keys.</returns>
        public virtual string GetKeyPath(string keyString)
        {
            string resourcePath = UriUtil.UriToString(this.RequestUri).Substring(UriUtil.UriToString(this.Context.BaseUri).Length);
            if (this.Context.UrlKeyDelimiter == DataServiceUrlKeyDelimiter.Slash)
            {
                return resourcePath + UriHelper.FORWARDSLASH + keyString;
            }
            else
            {
                return resourcePath + UriHelper.LEFTPAREN + keyString + UriHelper.RIGHTPAREN;
            }
        }

        /// <summary>Creates a data service query for function which return collection of data.</summary>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <param name="functionName">The function name.</param>
        /// <param name="isComposable">Whether this query is composable.</param>
        /// <param name="parameters">The function parameters.</param>
        /// <returns>A new <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> instance that represents the function call.</returns>
        public virtual DataServiceQuery<T> CreateFunctionQuery<T>(string functionName, bool isComposable, params UriOperationParameter[] parameters)
        {
            Dictionary<string, string> operationParameters = this.Context.SerializeOperationParameters(parameters);
            ResourceSetExpression rse = new ResourceSetExpression(typeof(IOrderedQueryable<T>), this.Expression, null, typeof(T), null, CountOption.None, null, null, null, null, functionName, operationParameters, false);
            return new DataServiceQuery<T>.DataServiceOrderedQuery(rse, new DataServiceQueryProvider(this.Context), isComposable);
        }

        /// <summary>Creates a data service query for function which return single data.</summary>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <param name="functionName">The function name.</param>
        /// <param name="isComposable">Whether this query is composable.</param>
        /// <param name="parameters">The function parameters.</param>
        /// <returns>A new <see cref="Microsoft.OData.Client.DataServiceQuerySingle{T}" /> instance that represents the function call.</returns>
        public virtual DataServiceQuerySingle<T> CreateFunctionQuerySingle<T>(string functionName, bool isComposable, params UriOperationParameter[] parameters)
        {
            return new DataServiceQuerySingle<T>(CreateFunctionQuery<T>(functionName, isComposable, parameters), isComposable);
        }

        /// <summary>
        /// Get a new URI string by adding <paramref name="nextSegment"/> to the original one.
        /// </summary>
        /// <param name="nextSegment">Name of the action.</param>
        /// <returns>The new URI string.</returns>
        public virtual string AppendRequestUri(string nextSegment)
        {
            Uri requestUri = this.RequestUri;
            return UriUtil.UriToString(requestUri).Replace(requestUri.AbsolutePath, requestUri.AbsolutePath + UriHelper.FORWARDSLASH + nextSegment);
        }

        /// <summary>
        /// Get a new URI path string by adding <paramref name="nextSegment"/> to the original one.
        /// </summary>
        /// <param name="nextSegment">The next segment to add to path.</param>
        /// <returns>The new URI path string.</returns>
        public virtual string GetPath(string nextSegment)
        {
            string resourcePath = UriUtil.UriToString(this.RequestUri).Substring(UriUtil.UriToString(this.Context.BaseUri).Length);
            return resourcePath + UriHelper.FORWARDSLASH + nextSegment;
        }

        /// <summary>Starts an asynchronous network operation that executes the query represented by this object instance.</summary>
        /// <returns>An <see cref="System.IAsyncResult" /> that represents the status of the asynchronous operation.</returns>
        /// <param name="callback">The delegate to invoke when the operation completes.</param>
        /// <param name="state">User defined object used to transfer state between the start of the operation and the callback defined by <paramref name="callback" />.</param>
        public virtual new IAsyncResult BeginExecute(AsyncCallback callback, object state)
        {
            if (this.IsFunction)
            {
                return this.Context.BeginExecute<TElement>(this.RequestUri, callback, state, XmlConstants.HttpMethodGet, false);
            }
            else
            {
                return base.BeginExecute(this, this.Context, callback, state, Util.ExecuteMethodName);
            }
        }

        /// <summary>Starts an asynchronous network operation that executes the query represented by this object instance.</summary>
        /// <returns>A task that represents an <see cref="System.Collections.Generic.IEnumerable{T}" />  that contains the results of the query operation.</returns>
        public virtual new Task<IEnumerable<TElement>> ExecuteAsync()
        {
            return ExecuteAsync(CancellationToken.None);
        }

        /// <summary>Starts an asynchronous network operation that executes the query represented by this object instance.</summary>
        /// <returns>A task that represents an <see cref="System.Collections.Generic.IEnumerable{T}" />  that contains the results of the query operation.</returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public virtual new Task<IEnumerable<TElement>> ExecuteAsync(CancellationToken cancellationToken)
        {
            return this.Context.FromAsync(this.BeginExecute, this.EndExecute, cancellationToken);
        }

        /// <summary>Ends an asynchronous query request to a data service.</summary>
        /// <returns>Returns an <see cref="System.Collections.Generic.IEnumerable{T}" />  that contains the results of the query operation.</returns>
        /// <param name="asyncResult">The pending asynchronous query request.</param>
        /// <exception cref="Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        public virtual new IEnumerable<TElement> EndExecute(IAsyncResult asyncResult)
        {
            if (this.IsFunction)
            {
                return this.Context.EndExecute<TElement>(asyncResult);
            }
            else
            {
                return DataServiceRequest.EndExecute<TElement>(this, this.Context, Util.ExecuteMethodName, asyncResult);
            }
        }

        /// <summary>
        /// Asynchronously sends a request to get all items by auto iterating all pages
        /// </summary>
        /// <returns>A task that represents an <see cref="System.Collections.Generic.IEnumerable{T}" /> that contains the results of the query operation.</returns>
        public virtual Task<IEnumerable<TElement>> GetAllPagesAsync()
        {
            return GetAllPagesAsync(CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously sends a request to get all items by auto iterating all pages
        /// </summary>
        /// <returns>A task that represents an <see cref="System.Collections.Generic.IEnumerable{T}" /> that contains the results of the query operation.</returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public virtual Task<IEnumerable<TElement>> GetAllPagesAsync(CancellationToken cancellationToken)
        {
            var currentTask = this.Context.FromAsync(this.BeginExecute, this.EndExecute, cancellationToken);
            var nextTask = currentTask.ContinueWith(t => this.ContinuePage(t.Result, cancellationToken), cancellationToken);
            return nextTask;
        }

        /// <summary>Executes the query and returns the results as a collection that implements IEnumerable.</summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}" /> in which TElement represents the type of the query results.</returns>
        /// <exception cref="Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        /// <exception cref="System.NotSupportedException">When during materialization an object is encountered in the input stream that cannot be deserialized to an instance of TElement.</exception>
        public virtual new IEnumerable<TElement> Execute()
        {
            if (this.IsFunction)
            {
                return this.Context.Execute<TElement>(this.RequestUri, XmlConstants.HttpMethodGet, false);
            }
            else
            {
                return this.Execute<TElement>(this.Context, this.Translate());
            }
        }

        /// <summary>
        /// Get all items by auto iterating all pages, will send the request of first page as default, regardless if it's iterated.
        /// </summary>
        /// <returns>The items retrieved</returns>
        public virtual IEnumerable<TElement> GetAllPages()
        {
            QueryOperationResponse<TElement> response = this.Execute<TElement>(this.Context, this.Translate());
            return this.GetRestPages(response);
        }

        /// <summary>Expands a query to include entities from a related entity set in the query response.</summary>
        /// <returns>A new query that includes the requested $expand query option appended to the URI of the supplied query.</returns>
        /// <param name="path">The expand path in the format Orders/Order_Details.</param>
        public virtual DataServiceQuery<TElement> Expand(string path)
        {
            Util.CheckArgumentNullAndEmpty(path, "path");
            Debug.Assert(DataServiceQuery<TElement>.expandMethodInfo != null, "DataServiceQuery<TElement>.expandMethodInfo != null");

            return (DataServiceQuery<TElement>)this.Provider.CreateQuery<TElement>(
                Expression.Call(
                    Expression.Convert(this.Expression, typeof(DataServiceQuery<TElement>.DataServiceOrderedQuery)),
                    DataServiceQuery<TElement>.expandMethodInfo,
                    new Expression[] { Expression.Constant(path) }));
        }

        /// <summary>Expands a query to include entities from a related entity set in the query response, where the related entity is of a specific type in a type hierarchy. </summary>
        /// <returns>Returns a <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> that with the expand option included.</returns>
        /// <param name="navigationPropertyAccessor">A lambda expression that indicates the navigation property that returns the entity set to include in the expanded query.</param>
        /// <typeparam name="TTarget">Target type of the last property on the expand path.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
        public virtual DataServiceQuery<TElement> Expand<TTarget>(Expression<Func<TElement, TTarget>> navigationPropertyAccessor)
        {
            Util.CheckArgumentNull(navigationPropertyAccessor, "navigationPropertyAccessor");
            Debug.Assert(DataServiceQuery<TElement>.expandGenericMethodInfo != null, "DataServiceQuery<TElement>.expandGenericMethodInfo != null");

            MethodInfo mi = DataServiceQuery<TElement>.expandGenericMethodInfo.MakeGenericMethod(typeof(TTarget));
            return (DataServiceQuery<TElement>)this.Provider.CreateQuery<TElement>(
                Expression.Call(
                    Expression.Convert(this.Expression, typeof(DataServiceQuery<TElement>.DataServiceOrderedQuery)),
                    mi,
                    new Expression[] { navigationPropertyAccessor }));
        }

        /// <summary>Requests that the count of all entities in the entity set be returned inline with the query results.</summary>
        /// <returns>A new <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> object that has the inline count option set.</returns>
        [Obsolete("Please use IncludeCount()")]
        public virtual DataServiceQuery<TElement> IncludeTotalCount()
        {
            return this.IncludeCount(true);
        }

        /// <summary>Requests that the count of all entities in the entity set be returned inline with the query results.</summary>
        /// <returns>A new <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> object that has the inline count option set.</returns>
        /// <param name="countQuery">Whether to include total count.</param>
        [Obsolete("Please use IncludeCount(bool countQuery)")]
        public virtual DataServiceQuery<TElement> IncludeTotalCount(bool countQuery)
        {
            return this.IncludeCount(countQuery);
        }

        /// <summary>Requests that the count of all entities in the entity set be returned inline with the query results.</summary>
        /// <returns>A new <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> object that has the inline count option set.</returns>
        public virtual DataServiceQuery<TElement> IncludeCount()
        {
            return this.IncludeCount(true);
        }

        /// <summary>Requests that the count of all entities in the entity set be returned inline with the query results.</summary>
        /// <returns>A new <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> object that has the inline count option set.</returns>
        /// <param name="countQuery">Whether to include count.</param>
        public virtual DataServiceQuery<TElement> IncludeCount(bool countQuery)
        {
            MethodInfo mi = typeof(DataServiceQuery<TElement>).GetMethods()
                .First(m => m.Name == nameof(IncludeCount) && m.GetParameters().Length == 1);

            return (DataServiceQuery<TElement>)this.Provider.CreateQuery<TElement>(
                Expression.Call(
                    Expression.Convert(this.Expression, typeof(DataServiceQuery<TElement>.DataServiceOrderedQuery)),
                    mi,
                    new Expression[] { Expression.Constant(countQuery, typeof(bool)) }));
        }

        /// <summary>Creates a new <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> with the query option set in the URI generated by the returned query.</summary>
        /// <returns>A new query that includes the requested query option appended to the URI of the supplied query</returns>
        /// <param name="name">The string value that contains the name of the query string option to add.</param>
        /// <param name="value">The object that contains the value of the query string option.</param>
        public virtual DataServiceQuery<TElement> AddQueryOption(string name, object value)
        {
            Util.CheckArgumentNull(name, "name");
            Util.CheckArgumentNull(value, "value");
            MethodInfo mi = typeof(DataServiceQuery<TElement>).GetMethod("AddQueryOption");
            return (DataServiceQuery<TElement>)this.Provider.CreateQuery<TElement>(
                Expression.Call(
                    Expression.Convert(this.Expression, typeof(DataServiceQuery<TElement>.DataServiceOrderedQuery)),
                    mi,
                    new Expression[] { Expression.Constant(name), Expression.Constant(value, typeof(object)) }));
        }

        /// <summary>Executes the query and returns the results as a collection.</summary>
        /// <returns>A typed enumerator over the results in which TElement represents the type of the query results.</returns>
        public virtual IEnumerator<TElement> GetEnumerator()
        {
            return this.Execute().GetEnumerator();
        }

        /// <summary>Represents the URI of the query to the data service.</summary>
        /// <returns>A URI as string that represents the query to the data service for this <see cref="Microsoft.OData.Client.DataServiceQuery{TElement}" /> instance.</returns>
        public override string ToString()
        {
            try
            {
                return this.QueryComponents(this.Context.Model).Uri.ToString();
            }
            catch (NotSupportedException e)
            {
                return Strings.ALinq_TranslationError(e.Message);
            }
        }

        /// <summary>Executes the query and returns the results as a collection.</summary>
        /// <returns>An enumerator over the query results.</returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// gets the UriTranslateResult for a the query
        /// </summary>
        /// <param name="model">The client model.</param>
        /// <returns>an instance of QueryComponents.</returns>
        internal override QueryComponents QueryComponents(ClientEdmModel model)
        {
            return this.Translate();
        }

        /// Synchronous methods not available
        /// <summary>
        /// Returns an IEnumerable from an Internet resource.
        /// </summary>
        /// <returns>An IEnumerable that contains the response from the Internet resource.</returns>
        internal override IEnumerable ExecuteInternal()
        {
            return this.Execute();
        }

        /// <summary>
        /// Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        internal override IAsyncResult BeginExecuteInternal(AsyncCallback callback, object state)
        {
            return this.BeginExecute(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response. </param>
        /// <returns>An IEnumerable that contains the response from the Internet resource.</returns>
        internal override IEnumerable EndExecuteInternal(IAsyncResult asyncResult)
        {
            return this.EndExecute(asyncResult);
        }

        /// <summary>
        /// gets the query components for the query after translating
        /// </summary>
        /// <returns>QueryComponents for query</returns>
        private QueryComponents Translate()
        {
            if (this.queryComponents == null)
            {
                this.queryComponents = this.queryProvider.Translate(this.queryExpression);
            }

            return this.queryComponents;
        }

        /// <summary>
        /// Continues to asynchronously send a request to get items of the next page
        /// </summary>
        /// <param name="response">The response of the previous page</param>
        /// <returns>The items retrieved</returns>
        private IEnumerable<TElement> ContinuePage(IEnumerable<TElement> response, CancellationToken cancellationToken)
        {
            foreach (var element in response)
            {
                yield return element;
            }

            var continuation = (response as QueryOperationResponse).GetContinuation() as DataServiceQueryContinuation<TElement>;
            if (continuation != null)
            {
                var asyncResult = this.Context.BeginExecute(continuation, null, null);
                cancellationToken.Register(() => this.Context.CancelRequest(asyncResult));
                var currentTask = Task<IEnumerable<TElement>>.Factory.FromAsync(asyncResult, this.Context.EndExecute<TElement>);
                var nextTask = currentTask.ContinueWith(t => ContinuePage(t.Result, cancellationToken), cancellationToken);
                nextTask.Wait(cancellationToken);
                foreach (var element in nextTask.Result)
                {
                    yield return element;
                }
            }
        }

        /// Synchronous methods not available
        /// <summary>
        /// Returns an IEnumerable from an Internet resource.
        /// </summary>
        /// <param name="response">The response of the previous page</param>
        /// <returns>An IEnumerable that contains the response from the Internet resource.</returns>
        private IEnumerable<TElement> GetRestPages(IEnumerable<TElement> response)
        {
            foreach (var element in response)
            {
                yield return element;
            }

            var continuation = (response as QueryOperationResponse<TElement>).GetContinuation();
            while (continuation != null)
            {
                response = this.Context.Execute(continuation);
                foreach (var element in response)
                {
                    yield return element;
                }

                continuation = (response as QueryOperationResponse<TElement>).GetContinuation();
            }
        }

        /// <summary>
        /// Ordered DataServiceQuery which implements IOrderedQueryable.
        /// </summary>
        public class DataServiceOrderedQuery : DataServiceQuery<TElement>, IOrderedQueryable<TElement>, IOrderedQueryable
        {
            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="expression">expression for query</param>
            /// <param name="provider">query provider for query</param>
            internal DataServiceOrderedQuery(Expression expression, DataServiceQueryProvider provider)
                : base(expression, provider)
            {
            }

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="expression">expression for query</param>
            /// <param name="provider">query provider for query</param>
            /// <param name="isComposable">whether this query is composable</param>
            internal DataServiceOrderedQuery(Expression expression, DataServiceQueryProvider provider, bool isComposable)
                : base(expression, provider, isComposable)
            {
            }
        }
    }
}
