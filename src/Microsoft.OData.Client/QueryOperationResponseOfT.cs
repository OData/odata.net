//---------------------------------------------------------------------
// <copyright file="QueryOperationResponseOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// Response to a batched query or Execute call.
    /// </summary>
    /// <typeparam name="T">The type to construct for the request results</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710", Justification = "required for this feature")]
    public sealed class QueryOperationResponse<T> : QueryOperationResponse, IEnumerable<T>
    {
        #region Constructors

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="headers">HTTP headers</param>
        /// <param name="query">original query</param>
        /// <param name="results">retrieved objects</param>
        internal QueryOperationResponse(HeaderCollection headers, DataServiceRequest query, MaterializeAtom results)
            : base(headers, query, results)
        {
        }

        #endregion Constructors

        #region Public properties

        /// <summary>The server result set count value from a query, if the query has requested the value.</summary>
        /// <returns>The return value can be either zero or a positive value equal to the number of entities in the set on the server.</returns>
        [Obsolete("Please use Count")]
        public override long TotalCount
        {
            get
            {
                return Count;
            }
        }

        /// <summary>The server result set count value from a query, if the query has requested the value.</summary>
        /// <returns>The return value can be either zero or a positive value equal to the number of entities in the set on the server.</returns>
        public override long Count
        {
            get
            {
                if (this.Results != null && this.Results.IsCountable)
                {
                    return this.Results.CountValue();
                }
                else
                {
                    throw new InvalidOperationException(Strings.MaterializeFromAtom_CountNotPresent);
                }
            }
        }

        #endregion Public properties

        #region Public methods

        /// <summary>Gets a <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that contains the URI that is used to retrieve the next results page.</summary>
        /// <returns>An object that contains the URI that is used to return the next results page.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "required for this feature")]
        public new DataServiceQueryContinuation<T> GetContinuation()
        {
            return (DataServiceQueryContinuation<T>)base.GetContinuation();
        }

        /// <summary>Executes the <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> and gets <see cref="T:Microsoft.OData.Client.QueryOperationResponse`1" /> items.</summary>
        /// <returns>An enumerator to a collection of <see cref="T:Microsoft.OData.Client.QueryOperationResponse`1" /> items.</returns>
        /// <remarks>In the case of Collection(primitive) or Collection(complex), the entire collection is
        /// materialized when this is called.</remarks>
        public new IEnumerator<T> GetEnumerator()
        {
            return this.GetEnumeratorHelper<IEnumerator<T>>(() => this.Results.Cast<T>().GetEnumerator());
        }

        #endregion Public methods.
    }
}
