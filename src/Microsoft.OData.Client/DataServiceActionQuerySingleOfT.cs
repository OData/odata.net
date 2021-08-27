//---------------------------------------------------------------------
// <copyright file="DataServiceActionQuerySingleOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Object of an action which returns a single item.
    /// </summary>
    /// <typeparam name="T">Type of object to materialize.</typeparam>
    public sealed class DataServiceActionQuerySingle<T>
    {
        /// <summary>
        /// Context associated with this query.
        /// </summary>
        private readonly DataServiceContext context;

        /// <summary>
        /// Parameters of this action.
        /// </summary>
        private readonly BodyOperationParameter[] parameters;

        /// <summary>
        /// Object of an action which returns a single item.
        /// </summary>
        /// <param name="context">Context associated with this query.</param>
        /// <param name="requestUriString">The URI string for this action.</param>
        /// <param name="parameters">Parameters of this action.</param>
        public DataServiceActionQuerySingle(DataServiceContext context, string requestUriString, params BodyOperationParameter[] parameters)
        {
            this.context = context;
            this.RequestUri = new Uri(requestUriString);
            this.parameters = parameters;
        }

        /// <summary>
        /// The URI for this action.
        /// </summary>
        public Uri RequestUri { get; private set; }

        /// <summary>
        /// Executes the action and returns the result.
        /// </summary>
        /// <returns>Action result.</returns>
        /// <exception cref="InvalidOperationException">Problem materializing result of query into object.</exception>
        public T GetValue()
        {
            return context.Execute<T>(this.RequestUri, XmlConstants.HttpMethodPost, true, parameters).Single();
        }

        /// <summary>Asynchronously sends a request to the data service to execute a specific URI.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IAsyncResult BeginGetValue(AsyncCallback callback, object state)
        {
            return context.BeginExecute<T>(this.RequestUri, callback, state, XmlConstants.HttpMethodPost, true, parameters);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        public Task<T> GetValueAsync()
        {
            return GetValueAsync(CancellationToken.None);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task<T> GetValueAsync(CancellationToken cancellationToken)
        {
            return this.context.FromAsync(this.BeginGetValue, this.EndGetValue, cancellationToken);
        }

        /// <summary>Called to complete the <see cref="Microsoft.OData.Client.DataServiceActionQuery{T}.BeginExecute(AsyncCallback,Object)" />.</summary>
        /// <returns>The results returned by the query operation.</returns>
        /// <param name="asyncResult">
        ///   <see cref="System.IAsyncResult" /> object.</param>
        /// <exception cref="System.ArgumentNullException">When<paramref name=" asyncResult" /> is null.</exception>
        /// <exception cref="System.ArgumentException">When<paramref name=" asyncResult" /> did not originate from this <see cref="Microsoft.OData.Client.DataServiceContext" /> instance. -or- When the <see cref="Microsoft.OData.Client.DataServiceContext.EndExecute{TElement}(System.IAsyncResult)" /> method was previously called.</exception>
        /// <exception cref="System.InvalidOperationException">When an error is raised either during execution of the request or when it converts the contents of the response message into objects.</exception>
        /// <exception cref="Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public T EndGetValue(IAsyncResult asyncResult)
        {
            Util.CheckArgumentNull(asyncResult, "asyncResult");
            return context.EndExecute<T>(asyncResult).Single();
        }
    }
}
