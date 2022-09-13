//---------------------------------------------------------------------
// <copyright file="DataServiceActionQuery.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Object of an action which returns nothing.
    /// </summary>
    public class DataServiceActionQuery
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
        /// Object of an action which returns nothing.
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
        /// Executes the action and returns the operation response.
        /// </summary>
        /// <returns>Operation result.</returns>
        /// <exception cref="InvalidOperationException">Problem materializing result of query into object.</exception>
        public OperationResponse Execute()
        {
            return Context.Execute(this.RequestUri, XmlConstants.HttpMethodPost, Parameters);
        }

        /// <summary>Asynchronously sends a request to the data service to execute a specific URI.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        public IAsyncResult BeginExecute(AsyncCallback callback, object state)
        {
            return Context.BeginExecute(this.RequestUri, callback, state, XmlConstants.HttpMethodPost, Parameters);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        public Task<OperationResponse> ExecuteAsync()
        {
            return this.ExecuteAsync(CancellationToken.None);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task<OperationResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Context.ExecuteAsync(this.RequestUri, XmlConstants.HttpMethodPost, cancellationToken, Parameters);
        }

        /// <summary>Called to complete the <see cref="Microsoft.OData.Client.DataServiceActionQuery.BeginExecute(System.AsyncCallback,System.Object)" />.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="asyncResult">An <see cref="System.IAsyncResult" /> that represents the status of the asynchronous operation.</param>
        /// <remarks>This method should be used in combination with the BeginExecute overload which
        /// expects the request uri to end with an action that returns void.</remarks>
        public OperationResponse EndExecute(IAsyncResult asyncResult)
        {
            return Context.EndExecute(asyncResult);
        }
    }
}
