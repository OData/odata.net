//---------------------------------------------------------------------
// <copyright file="L2OParameterizedQuery.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections;
    using System.Linq.Expressions;

    /// <summary>
    /// The ling to object parameterized query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class L2OParameterizedQuery<T> : IOrderedQueryable<T>
    {
        /// <summary>
        /// The query provider
        /// </summary>
        readonly IQueryProvider queryProvider;

        /// <summary>
        /// The current expression
        /// </summary>
        readonly Expression expression;

        /// <summary>
        /// Create an instance of L2OParameterizedQuery
        /// </summary>
        /// <param name="expression">The original expression</param>
        /// <param name="queryProvider">The query builder</param>
        public L2OParameterizedQuery(Expression expression, IQueryProvider queryProvider)
        {
            this.expression = expression;
            this.queryProvider = queryProvider;
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return queryProvider.Execute<IEnumerable<T>>(expression).GetEnumerator();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The expression
        /// </summary>
        public Expression Expression
        {
            get { return expression; }
        }

        /// <summary>
        /// The expression element type
        /// </summary>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// The query provider
        /// </summary>
        public IQueryProvider Provider
        {
            get { return queryProvider; }
        }
    }
}
