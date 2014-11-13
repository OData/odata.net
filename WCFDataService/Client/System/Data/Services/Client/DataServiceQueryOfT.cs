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

namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
#if ASTORIA_LIGHT && !PORTABLELIB
    using System.Data.Services.Http;
#endif
    using System.Reflection;
    using System.Collections;
    using System.Data.Services.Common;

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
#if WINDOWS_PHONE
        private static readonly MethodInfo expandGenericMethodInfo = (MethodInfo)typeof(DataServiceQuery<TElement>).GetMember("Expand").Single(m => ((MethodInfo)m).GetGenericArguments().Count() == 1);
#else
#if WINRT
        private static readonly MethodInfo expandGenericMethodInfo = typeof(DataServiceQuery<TElement>).GetMethodWithGenericArgs("Expand", true /*isPublic*/, false /*isStatic*/, 1 /*genericArgCount*/);
#else
        private static readonly MethodInfo expandGenericMethodInfo = (MethodInfo)typeof(DataServiceQuery<TElement>).GetMember("Expand*").Single(m => ((MethodInfo)m).GetGenericArguments().Count() == 1);
#endif
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
        private DataServiceQuery(Expression expression, DataServiceQueryProvider provider)
        {
            Debug.Assert(null != provider.Context, "null context");
            Debug.Assert(expression != null, "null expression");
            Debug.Assert(provider is DataServiceQueryProvider, "Currently only support Web Query Provider");

            this.queryExpression = expression;
            this.queryProvider = provider;
        }

        #region IQueryable implementation
        /// <summary>Returns the type of the object used in the template to create the <see cref="T:System.Data.Services.Client.DataServiceQuery`1" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Type" /> representing the type used in the template when the query is created.</returns>
        public override Type ElementType
        {
            get { return typeof(TElement); }
        }

        /// <summary>Represents an expression containing the query to the data service.</summary>
        /// <returns>A <see cref="T:System.Linq.Expressions.Expression" /> object representing the query.</returns>
        public override Expression Expression
        {
            get { return this.queryExpression; }
        }

        /// <summary>Represents the query provider instance.</summary>
        /// <returns>A <see cref="T:System.Linq.IQueryProvider" /> representing the data source provider.</returns>
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

        /// <summary>The ProjectionPlan for the request (if precompiled in a previous page).</summary>
        internal override ProjectionPlan Plan
        {
            get { return null; }
        }

        /// <summary>Context associated with this query.</summary>
        private DataServiceContext Context
        {
            get { return this.queryProvider.Context; }
        }

        /// <summary>Starts an asynchronous network operation that executes the query represented by this object instance.</summary>
        /// <returns>An <see cref="T:System.IAsyncResult" /> that represents the status of the asynchronous operation.</returns>
        /// <param name="callback">The delegate to invoke when the operation completes.</param>
        /// <param name="state">User defined object used to transfer state between the start of the operation and the callback defined by <paramref name="callback" />.</param>
        public new IAsyncResult BeginExecute(AsyncCallback callback, object state)
        {
            return base.BeginExecute(this, this.Context, callback, state, Util.ExecuteMethodName);
        }

        /// <summary>Ends an asynchronous query request to a data service.</summary>
        /// <returns>Returns an <see cref="T:System.Collections.Generic.IEnumerable`1" />  that contains the results of the query operation.</returns>
        /// <param name="asyncResult">The pending asynchronous query request.</param>
        /// <exception cref="T:System.Data.Services.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        public new IEnumerable<TElement> EndExecute(IAsyncResult asyncResult)
        {
            return DataServiceRequest.EndExecute<TElement>(this, this.Context, Util.ExecuteMethodName, asyncResult);
        }

#if !ASTORIA_LIGHT && !PORTABLELIB // Synchronous methods not available
        /// <summary>Executes the query and returns the results as a collection that implements IEnumerable.Not supported by the WCF Data Services 5.0 client for Silverlight.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> in which TElement represents the type of the query results.</returns>
        /// <exception cref="T:System.Data.Services.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        /// <exception cref="T:System.NotSupportedException">When during materialization an object is encountered in the input stream that cannot be deserialized to an instance of TElement.</exception>
        public new IEnumerable<TElement> Execute()
        {
            return this.Execute<TElement>(this.Context, this.Translate());
        }
#endif

        /// <summary>Expands a query to include entities from a related entity set in the query response.</summary>
        /// <returns>A new query that includes the requested $expand query option appended to the URI of the supplied query.</returns>
        /// <param name="path">The expand path in the format Orders/Order_Details.</param>
        public DataServiceQuery<TElement> Expand(string path)
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
        /// <returns>Returns a <see cref="T:System.Data.Services.Client.DataServiceQuery`1" /> that with the expand option included.</returns>
        /// <param name="navigationPropertyAccessor">A lambda expression that indicates the navigation property that returns the entity set to include in the expanded query.</param>
        /// <typeparam name="TTarget">Target type of the last property on the expand path.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
        public DataServiceQuery<TElement> Expand<TTarget>(Expression<Func<TElement, TTarget>> navigationPropertyAccessor)
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
        /// <returns>A new <see cref="T:System.Data.Services.Client.DataServiceQuery`1" /> object that has the inline count option set.</returns>
        public DataServiceQuery<TElement> IncludeTotalCount()
        {
            MethodInfo mi = typeof(DataServiceQuery<TElement>).GetMethod("IncludeTotalCount");

            return (DataServiceQuery<TElement>)this.Provider.CreateQuery<TElement>(
                Expression.Call(
                    Expression.Convert(this.Expression, typeof(DataServiceQuery<TElement>.DataServiceOrderedQuery)),
                    mi));
        }

        /// <summary>Creates a new <see cref="T:System.Data.Services.Client.DataServiceQuery`1" /> with the query option set in the URI generated by the returned query.</summary>
        /// <returns>A new query that includes the requested query option appended to the URI of the supplied query</returns>
        /// <param name="name">The string value that contains the name of the query string option to add.</param>
        /// <param name="value">The object that contains the value of the query string option.</param>
        public DataServiceQuery<TElement> AddQueryOption(string name, object value)
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
#if !ASTORIA_LIGHT && !PORTABLELIB // Synchronous methods not available
        public IEnumerator<TElement> GetEnumerator()
        {
            return this.Execute().GetEnumerator();
        }
#else
        public IEnumerator<TElement> GetEnumerator()
        {
            throw Error.NotSupported(Strings.DataServiceQuery_EnumerationNotSupported);
        }
#endif

        /// <summary>Represents the URI of the query to the data service.</summary>
        /// <returns>A URI as string that represents the query to the data service for this <see cref="T:System.Data.Services.Client.DataServiceQuery`1" /> instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0010", Justification = "ToString for display purpose is OK")]
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
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
#if !ASTORIA_LIGHT && !PORTABLELIB // Synchronous methods not available
            return this.GetEnumerator();
#else
            throw Error.NotSupported();
#endif
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

#if !ASTORIA_LIGHT && !PORTABLELIB
        /// Synchronous methods not available
        /// <summary>
        /// Returns an IEnumerable from an Internet resource. 
        /// </summary>
        /// <returns>An IEnumerable that contains the response from the Internet resource.</returns>
        internal override IEnumerable ExecuteInternal()
        {
            return this.Execute();
        }
#endif

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
        /// Ordered DataServiceQuery which implements IOrderedQueryable.
        /// </summary>
        internal class DataServiceOrderedQuery : DataServiceQuery<TElement>, IOrderedQueryable<TElement>, IOrderedQueryable
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
        }
    }
}
