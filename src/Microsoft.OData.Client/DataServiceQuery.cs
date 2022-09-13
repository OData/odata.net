//---------------------------------------------------------------------
// <copyright file="DataServiceQuery.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>non-generic placeholder for generic implementation</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010", Justification = "required for this feature")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710", Justification = "required for this feature")]
    public abstract class DataServiceQuery : DataServiceRequest, IQueryable
    {
        /// <summary>internal constructor so that only our assembly can provide an implementation</summary>
        internal DataServiceQuery()
        {
        }

        /// <summary>Represents an expression that contains the query to the data service.</summary>
        /// <returns>An <see cref="System.Linq.Expressions.Expression" /> object that represents the query.</returns>
        public abstract Expression Expression
        {
            get;
        }

        /// <summary>Represents the query provider instance.</summary>
        /// <returns>An <see cref="System.Linq.IQueryProvider" /> representing the data source provider.</returns>
        public abstract IQueryProvider Provider
        {
            get;
        }

        /// <summary>Gets the <see cref="System.Collections.IEnumerator" /> object that can be used to iterate through the collection returned by the query.</summary>
        /// <returns>An enumerator over the query results.</returns>
        /// <remarks>Expect derived class to override this with an explicit interface implementation</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033", Justification = "required for this feature")]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw Error.NotImplemented();
        }

        /// <summary>Executes the query against the data service.</summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}" /> that contains the results of the query operation.</returns>
        /// <exception cref="Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        public virtual IEnumerable Execute()
        {
            return this.ExecuteInternal();
        }

        /// <summary>Asynchronously sends a request to execute the data service query.</summary>
        /// <returns>An <see cref="System.IAsyncResult" /> object that is used to track the status of the asynchronous operation.</returns>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        public virtual IAsyncResult BeginExecute(AsyncCallback callback, object state)
        {
            return this.BeginExecuteInternal(callback, state);
        }

        /// <summary>Asynchronously sends a request to execute the data service query.</summary>
        /// <returns>A task represents An <see cref="System.Collections.Generic.IEnumerable{T}" /> that contains the results of the query operation.</returns>
        public virtual Task<IEnumerable> ExecuteAsync()
        {
            return Task<IEnumerable>.Factory.FromAsync(this.BeginExecute, this.EndExecute, null);
        }

        /// <summary>Called to complete the asynchronous operation of executing a data service query.</summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}" /> that contains the results of the query operation.</returns>
        /// <param name="asyncResult">The result from the <see cref="Microsoft.OData.Client.DataServiceQuery.BeginExecute(System.AsyncCallback,System.Object)" /> operation that contains the query results.</param>
        /// <exception cref="Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        public virtual IEnumerable EndExecute(IAsyncResult asyncResult)
        {
            return this.EndExecuteInternal(asyncResult);
        }

        /// <summary>
        /// Returns an IEnumerable from an Internet resource.
        /// </summary>
        /// <returns>An IEnumerable that contains the response from the Internet resource.</returns>
        internal abstract IEnumerable ExecuteInternal();

        /// <summary>
        /// Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        internal abstract IAsyncResult BeginExecuteInternal(AsyncCallback callback, object state);

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response. </param>
        /// <returns>An IEnumerable that contains the response from the Internet resource.</returns>
        internal abstract IEnumerable EndExecuteInternal(IAsyncResult asyncResult);
    }
}
