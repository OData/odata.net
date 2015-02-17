//---------------------------------------------------------------------
// <copyright file="EFParameterizedQuery.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Collections;
    using System.Linq.Expressions;

    /// <summary>
    /// EF Parameterized Query of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class EFParameterizedQuery<T> : IOrderedQueryable<T>, IObjectQueryWrapper
    {
        /// <summary>
        /// The original EF ObjectQuery
        /// </summary>
        readonly ObjectQuery objectQuery;

        /// <summary>
        /// The query provider
        /// </summary>
        readonly IQueryProvider queryProvider;

        /// <summary>
        /// Create an instance of type EFParameterizedQuery
        /// </summary>
        /// <param name="objectQuery">The original EF ObjectQuery</param>
        /// <param name="queryProvider">The query provider</param>
        public EFParameterizedQuery(ObjectQuery<T> objectQuery, IQueryProvider queryProvider)
        {
            this.queryProvider = queryProvider;
            this.objectQuery = objectQuery;
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return queryProvider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Get the current expression
        /// </summary>
        public Expression Expression
        {
            get { return ((IQueryable)objectQuery).Expression; }
        }

        /// <summary>
        /// Get the element type
        /// </summary>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Get the query provider
        /// </summary>
        public IQueryProvider Provider
        {
            get { return queryProvider; }
        }

        /// <summary>
        /// The original EF ObjectQuery
        /// </summary>
        public ObjectQuery ObjectQuery
        {
            get { return objectQuery; }
        }
    }
}
