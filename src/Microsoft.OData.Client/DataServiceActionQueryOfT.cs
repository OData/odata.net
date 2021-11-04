//---------------------------------------------------------------------
// <copyright file="DataServiceActionQueryOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Object of an action which returns a collection.
    /// </summary>
    /// <typeparam name="T">Type of object to materialize.</typeparam>
    public sealed class DataServiceActionQuery<T>
    {
        /// <summary>
        /// Context associated with this query.
        /// </summary>
        private readonly DataServiceContext Context;

        /// <summary>
        /// Parameters of this action.
        /// </summary>
        private readonly BodyOperationParameter[] Parameters;

        /// <summary>
        /// Object of an action which returns a collection.
        /// </summary>
        /// <param name="context">Context associated with this query.</param>
        /// <param name="requestUriString">The URI string for this action.</param>
        /// <param name="parameters">Parameters of this action.</param>
        public DataServiceActionQuery(DataServiceContext context, string requestUriString, params BodyOperationParameter[] parameters)
        {
            this.Context = context;
            this.RequestUri = new Uri(requestUriString);
            this.Parameters = parameters;
        }

        /// <summary>
        /// The URI for this action.
        /// </summary>
        public Uri RequestUri { get; private set; }

        /// <summary>
        /// Executes the action and returns the results as a collection.
        /// </summary>
        /// <returns>Action results.</returns>
        /// <exception cref="InvalidOperationException">Problem materializing result of query into object.</exception>
        public IEnumerable<T> Execute()
        {
            return Context.Execute<T>(this.RequestUri, XmlConstants.HttpMethodPost, false, Parameters);
        }

        /// <summary>Asynchronously sends a request to the data service to execute a specific URI.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IAsyncResult BeginExecute(AsyncCallback callback, object state)
        {
            return Context.BeginExecute<T>(this.RequestUri, callback, state, XmlConstants.HttpMethodPost, false, Parameters);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        public Task<IEnumerable<T>> ExecuteAsync()
        {
            return this.ExecuteAsync(CancellationToken.None);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task<IEnumerable<T>> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Context.ExecuteAsync<T>(this.RequestUri, XmlConstants.HttpMethodPost, false, cancellationToken, Parameters);
        }

        /// <summary>Called to complete the <see cref="Microsoft.OData.Client.DataServiceActionQuery.BeginExecute(System.AsyncCallback,System.Object)" />.</summary>
        /// <returns>The results returned by the query operation.</returns>
        /// <param name="asyncResult">
        ///   <see cref="System.IAsyncResult" /> object.</param>
        /// <exception cref="System.ArgumentNullException">When<paramref name=" asyncResult" /> is null.</exception>
        /// <exception cref="System.ArgumentException">When<paramref name=" asyncResult" /> did not originate from this <see cref="Microsoft.OData.Client.DataServiceContext" /> instance. -or- When the <see cref="Microsoft.OData.Client.DataServiceContext.EndExecute{TElement}(System.IAsyncResult)" /> method was previously called.</exception>
        /// <exception cref="System.InvalidOperationException">When an error is raised either during execution of the request or when it converts the contents of the response message into objects.</exception>
        /// <exception cref="Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IEnumerable<T> EndExecute(IAsyncResult asyncResult)
        {
            Util.CheckArgumentNull(asyncResult, "asyncResult");
            return Context.EndExecute<T>(asyncResult);
        }

        /// <summary>
        /// Executes the query and returns the results as a collection.
        /// </summary>
        /// <returns>A typed enumerator over the results in which TElement represents the type of the query results.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Execute().GetEnumerator();
        }
    }
}
