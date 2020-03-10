//---------------------------------------------------------------------
// <copyright file="QueryOperationResponse.cs" company="Microsoft">
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
    using System.Reflection;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// Response to a batched query.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010", Justification = "required for this feature")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710", Justification = "required for this feature")]
    public class QueryOperationResponse : OperationResponse, System.Collections.IEnumerable
    {
        #region Private fields

        /// <summary>Original query</summary>
        private readonly DataServiceRequest query;

        /// <summary>Enumerable of objects in query</summary>
        private readonly MaterializeAtom results;

        #endregion Private fields

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="headers">HTTP headers</param>
        /// <param name="query">original query</param>
        /// <param name="results">retrieved objects</param>
        internal QueryOperationResponse(HeaderCollection headers, DataServiceRequest query, MaterializeAtom results)
            : base(headers)
        {
            this.query = query;
            this.results = results;
        }

        /// <summary>Gets the <see cref="T:Microsoft.OData.Client.DataServiceQuery" /> that generates the <see cref="T:Microsoft.OData.Client.QueryOperationResponse" /> items. </summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.DataServiceQuery" /> object.</returns>
        public DataServiceRequest Query
        {
            get { return this.query; }
        }

        /// <summary>Gets the server result set count value from a query, if the query has requested the value.</summary>
        /// <returns>The return value can be either a zero or positive value equal to the number of entities in the set on the server.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the count tag is not found in the response stream.</exception>
        [Obsolete("Please use Count")]
        public virtual long TotalCount
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Gets the server result set count value from a query, if the query has requested the value.</summary>
        /// <returns>The return value can be either a zero or positive value equal to the number of entities in the set on the server.</returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when the count tag is not found in the response stream.</exception>
        public virtual long Count
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>get a non-null enumerable of the result</summary>
        internal MaterializeAtom Results
        {
            get
            {
                if (null != this.Error)
                {
                    throw Microsoft.OData.Client.Error.InvalidOperation(Strings.Context_BatchExecuteError, this.Error);
                }

                return this.results;
            }
        }

        /// <summary>Executes the <see cref="T:Microsoft.OData.Client.DataServiceQuery" /> and returns <see cref="T:Microsoft.OData.Client.QueryOperationResponse" /> items. </summary>
        /// <returns>The enumerator to a collection of <see cref="T:Microsoft.OData.Client.QueryOperationResponse" /> items.</returns>
        /// <remarks>In the case of Collection(primitive) or Collection(complex), the entire collection is
        /// materialized when this is called.</remarks>
        public IEnumerator GetEnumerator()
        {
            return this.GetEnumeratorHelper<IEnumerator>(() => this.Results.GetEnumerator());
        }

        /// <summary>Gets a <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation" /> object containing the URI that is used to retrieve the next results page.</summary>
        /// <returns>An object containing the URI that is used to return the next results page.</returns>
        public DataServiceQueryContinuation GetContinuation()
        {
            return this.results.GetContinuation(null);
        }

        /// <summary>Gets a <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation" /> object containing the URI that is used to retrieve the next page of related entities in the specified collection.</summary>
        /// <returns>A continuation object that points to the next page for the collection.</returns>
        /// <param name="collection">The collection of related objects being loaded.</param>
        public DataServiceQueryContinuation GetContinuation(IEnumerable collection)
        {
            return this.results.GetContinuation(collection);
        }

        /// <summary>Gets a <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that contains the URI that is used to retrieve the next page of related entities in the specified collection.</summary>
        /// <returns>A continuation object that points to the next page for the collection.</returns>
        /// <param name="collection">The collection of related objects being loaded.</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        public DataServiceQueryContinuation<T> GetContinuation<T>(IEnumerable<T> collection)
        {
            return (DataServiceQueryContinuation<T>)this.results.GetContinuation(collection);
        }

        /// <summary>
        /// Creates a generic instance of the QueryOperationResponse and return it
        /// </summary>
        /// <param name="elementType">generic type for the QueryOperationResponse.</param>
        /// <param name="headers">constructor parameter1</param>
        /// <param name="query">constructor parameter2</param>
        /// <param name="results">constructor parameter3</param>
        /// <returns>returns a new strongly typed instance of QueryOperationResponse.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        internal static QueryOperationResponse GetInstance(Type elementType, HeaderCollection headers, DataServiceRequest query, MaterializeAtom results)
        {
            Type genericType = typeof(QueryOperationResponse<>).MakeGenericType(elementType);
#if !PORTABLELIB
            return (QueryOperationResponse)Activator.CreateInstance(
                genericType,
                BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] { headers, query, results },
                System.Globalization.CultureInfo.InvariantCulture);
#else
            ConstructorInfo info = genericType.GetInstanceConstructors(false /*isPublic*/).Single();
            return (QueryOperationResponse)Util.ConstructorInvoke(info, new object[] { headers, query, results });
#endif
        }

        /// <summary>Gets the enumeration helper for the <see cref="T:Microsoft.OData.Client.QueryOperationResponse" />.</summary>
        /// <param name="getEnumerator">The enumerator.</param>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <returns>An enumerator to enumerator through the results.</returns>
        protected T GetEnumeratorHelper<T>(Func<T> getEnumerator) where T : IEnumerator
        {
            if (getEnumerator == null)
            {
                throw new ArgumentNullException("getEnumerator");
            }

            if (this.Results.Context != null)
            {
                bool? singleResult = this.Query.QueryComponents(this.Results.Context.Model).SingleResult;

                if (singleResult.HasValue && !singleResult.Value)
                {
                    // Case: collection of complex or primitive.
                    // We materialize the entire collection now, and give them the inner enumerator instead
                    IEnumerator enumerator = this.Results.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        object innerObject = enumerator.Current;
                        ICollection materializedCollection = innerObject as ICollection;

                        if (materializedCollection == null)
                        {
                            throw new DataServiceClientException(Strings.AtomMaterializer_CollectionExpectedCollection(innerObject.GetType().ToString()));
                        }

                        Debug.Assert(!enumerator.MoveNext(), "MaterializationEvents of top level collection expected one result of ICollection<>, but found at least 2 results");
                        return (T)materializedCollection.GetEnumerator();
                    }
                    else
                    {
                        Debug.Assert(false, "MaterializationEvents of top level collection expected one result of ICollection<>, but found at least no results.");
                    }
                }
            }

            return getEnumerator();
        }
    }
}
