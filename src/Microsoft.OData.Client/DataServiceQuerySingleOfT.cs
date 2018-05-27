//---------------------------------------------------------------------
// <copyright file="DataServiceQuerySingleOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Query object of a single item.
    /// </summary>
    /// <typeparam name="TElement">Type of object to materialize.</typeparam>
    public class DataServiceQuerySingle<TElement>
    {
        /// <summary>
        /// Query object.
        /// </summary>
        internal DataServiceQuery<TElement> Query;

        /// <summary>
        /// The flag of whether this query is a function.
        /// </summary>
        private readonly bool isFunction;

        /// <summary>
        /// Query object of a single item.
        /// </summary>
        /// <param name="context">Context associated with this query.</param>
        /// <param name="path">A string that resolves to a URI.</param>
        public DataServiceQuerySingle(DataServiceContext context, string path)
        {
            this.Context = context;
            this.Query = context.CreateSingletonQuery<TElement>(path);
            this.IsComposable = true;
            this.isFunction = false;
        }

        /// <summary>
        /// Query object of a single item.
        /// </summary>
        /// <param name="context">Context associated with this query.</param>
        /// <param name="path">A string that resolves to a URI.</param>
        /// <param name="isComposable">Whether this query is composable.</param>
        public DataServiceQuerySingle(DataServiceContext context, string path, bool isComposable)
        {
            this.Context = context;
            this.Query = context.CreateSingletonQuery<TElement>(path);
            this.IsComposable = isComposable;
            this.isFunction = true;
        }

        /// <summary>Create a query of a single item based on another one.</summary>
        /// <param name="query">The query.</param>
        public DataServiceQuerySingle(DataServiceQuerySingle<TElement> query)
        {
            this.Context = query.Context;
            this.Query = query.Query;
            this.IsComposable = query.IsComposable;
            this.isFunction = query.isFunction;
        }

        /// <summary>
        /// Query object of a function which returns a single item.
        /// </summary>
        /// <param name="query">Query object.</param>
        /// <param name="isComposable">Whether this query is composable.</param>
        internal DataServiceQuerySingle(DataServiceQuery<TElement> query, bool isComposable)
        {
            this.Query = query;
            this.Context = query.Context;
            this.IsComposable = isComposable;
            this.isFunction = query.IsFunction;
        }

        /// <summary>
        /// Context associated with this query.
        /// </summary>
        public DataServiceContext Context { get; private set; }

        /// <summary>
        /// Whether this query is composable.
        /// </summary>
        public bool IsComposable { get; private set; }

        /// <summary>
        /// Get the URI for the query.
        /// </summary>
        public Uri RequestUri
        {
            get
            {
                if (this.Query == null)
                {
                    this.Query = Context.CreateSingletonQuery<TElement>(GetPath(null));
                }

                return Query.RequestUri;
            }
        }

        /// <summary>Creates a data service query for function which return collection of data.</summary>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <param name="functionName">The function name.</param>
        /// <param name="isComposable">Whether this query is composable.</param>
        /// <param name="parameters">The function parameters.</param>
        /// <returns>A new <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> instance that represents the function call.</returns>
        public DataServiceQuery<T> CreateFunctionQuery<T>(string functionName, bool isComposable, params UriOperationParameter[] parameters)
        {
            return this.Query.CreateFunctionQuery<T>(functionName, isComposable, parameters);
        }

        /// <summary>Creates a data service query for function which return single data.</summary>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <param name="functionName">The function name.</param>
        /// <param name="isComposable">Whether this query is composable.</param>
        /// <param name="parameters">The function parameters.</param>
        /// <returns>A new <see cref="T:Microsoft.OData.Client.DataServiceQuerySingle`1" /> instance that represents the function call.</returns>
        public DataServiceQuerySingle<T> CreateFunctionQuerySingle<T>(string functionName, bool isComposable, params UriOperationParameter[] parameters)
        {
            return new DataServiceQuerySingle<T>(this.CreateFunctionQuery<T>(functionName, isComposable, parameters), isComposable);
        }

#if !PORTABLELIB // Synchronous methods not available
        /// <summary>
        /// Executes the query and returns the result.
        /// </summary>
        /// <returns>Query result.</returns>
        /// <exception cref="InvalidOperationException">Problem materializing result of query into object.</exception>
        public TElement GetValue()
        {
            if (this.isFunction)
            {
                return this.Context.Execute<TElement>(this.RequestUri, XmlConstants.HttpMethodGet, true).SingleOrDefault();
            }

            return this.Query.Execute().SingleOrDefault();
        }
#endif

        /// <summary>Starts an asynchronous network operation that executes the query represented by this object instance.</summary>
        /// <returns>An <see cref="T:System.IAsyncResult" /> that represents the status of the asynchronous operation.</returns>
        /// <param name="callback">The delegate to invoke when the operation completes.</param>
        /// <param name="state">User defined object used to transfer state between the start of the operation and the callback defined by <paramref name="callback" />.</param>
        public IAsyncResult BeginGetValue(AsyncCallback callback, object state)
        {
            if (this.isFunction)
            {
                return this.Context.BeginExecute<TElement>(this.RequestUri, callback, state, XmlConstants.HttpMethodGet, true);
            }

            return this.Query.BeginExecute(callback, state);
        }

        /// <summary>Starts an asynchronous network operation that executes the query represented by this object instance.</summary>
        /// <returns>A task that represents the result of the query operation.</returns>
        public Task<TElement> GetValueAsync()
        {
            return Task<TElement>.Factory.FromAsync(this.BeginGetValue, this.EndGetValue, null);
        }

        /// <summary>Ends an asynchronous query request to a data service.</summary>
        /// <returns>Returns the results of the query operation.</returns>
        /// <param name="asyncResult">The pending asynchronous query request.</param>
        /// <exception cref="T:Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        public TElement EndGetValue(IAsyncResult asyncResult)
        {
            Util.CheckArgumentNull(asyncResult, "asyncResult");
            if (this.isFunction)
            {
                return this.Context.EndExecute<TElement>(asyncResult).SingleOrDefault();
            }
            else
            {
                return this.Query.EndExecute(asyncResult).SingleOrDefault();
            }
        }

        /// <summary>
        /// Get a new URI path string by adding <paramref name="nextSegment"/> to the original one.
        /// </summary>
        /// <param name="nextSegment">The next segment to add to path.</param>
        /// <returns>The new URI path string.</returns>
        public string GetPath(string nextSegment)
        {
            string resourcePath = UriUtil.UriToString(this.RequestUri).Substring(UriUtil.UriToString(this.Context.BaseUri).Length);
            return nextSegment == null ? resourcePath : resourcePath + UriHelper.FORWARDSLASH + nextSegment;
        }

        /// <summary>
        /// Get a new URI string by adding <paramref name="nextSegment"/> to the original one.
        /// </summary>
        /// <param name="nextSegment">Name of the action.</param>
        /// <returns>The new URI string.</returns>
        public string AppendRequestUri(string nextSegment)
        {
            return UriUtil.UriToString(this.RequestUri).Replace(this.RequestUri.AbsolutePath, this.RequestUri.AbsolutePath + UriHelper.FORWARDSLASH + nextSegment);
        }

        /// <summary>
        /// Projects the element of this query into a new form.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="selector">A lambda expression that indicates the property returns.</param>
        /// <returns>A <see cref="T:Microsoft.OData.Client.DataServiceQuerySingle`1" /> whose element is the result of invoking the transform function on the element of source.</returns>
        public DataServiceQuerySingle<TResult> Select<TResult>(Expression<Func<TElement, TResult>> selector)
        {
            if (this.Query == null)
            {
                this.Query = Context.CreateSingletonQuery<TElement>(GetPath(null));
            }

            return new DataServiceQuerySingle<TResult>((DataServiceQuery<TResult>)Query.Select(selector), true);
        }

        /// <summary>
        /// Expands a query to include entity from a related entity set in the query response, where the related entity is of a specific type in a type hierarchy.
        /// </summary>
        /// <typeparam name="TTarget">Target type of the last property on the expand path.</typeparam>
        /// <param name="navigationPropertyAccessor">A lambda expression that indicates the navigation property that returns the entity set to include in the expanded query.</param>
        /// <returns>Returns a <see cref="T:Microsoft.OData.Client.DataServiceQuerySingle`1" /> that with the expand option included.</returns>
        public DataServiceQuerySingle<TElement> Expand<TTarget>(Expression<Func<TElement, TTarget>> navigationPropertyAccessor)
        {
            return new DataServiceQuerySingle<TElement>(this.Query.Expand(navigationPropertyAccessor), true);
        }

        /// <summary>Expands a query to include entities from a related entity set in the query response.</summary>
        /// <returns>A new query that includes the requested $expand query option appended to the URI of the supplied query.</returns>
        /// <param name="path">The expand path in the format Orders/Order_Details.</param>
        public DataServiceQuerySingle<TElement> Expand(string path)
        {
            return new DataServiceQuerySingle<TElement>(this.Query.Expand(path), true);
        }

        /// <summary>
        /// Cast this query type into its derived type.
        /// </summary>
        /// <typeparam name="TResult">Derived type of TElement to be casted to.</typeparam>
        /// <returns>Returns a <see cref="T:Microsoft.OData.Client.DataServiceQuerySingle`1" /> of TResult type.</returns>
        public DataServiceQuerySingle<TResult> CastTo<TResult>()
        {
            return new DataServiceQuerySingle<TResult>((DataServiceQuery<TResult>)this.Query.OfType<TResult>(), true);
        }
    }
}
