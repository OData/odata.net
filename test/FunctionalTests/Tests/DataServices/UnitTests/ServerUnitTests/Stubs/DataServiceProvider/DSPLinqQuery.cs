//---------------------------------------------------------------------
// <copyright file="DSPLinqQuery.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>The implementation of <see cref="IOrderedQueryable"/> which translates
    /// queries generated against the DSP ResourceType/ResourceProperty model into
    /// a LINQ to Objects query accessing properties directly on the resources.</summary>
    /// <typeparam name="T">The type of an element returned by the query.</typeparam>
    internal class DSPLinqQuery<T> : IOrderedQueryable<T>
    {
        /// <summary>The expression represented by this query (this is the DSP expression).</summary>
        private Expression queryExpression;
        /// <summary>The query provider we use.</summary>
        private DSPLinqQueryProvider queryProvider;

        /// <summary>Internal constructor.</summary>
        /// <param name="queryProvider">The query provider to use for this query.</param>
        /// <param name="queryExpression">The query expression for this query (the DSP version).</param>
        internal DSPLinqQuery(DSPLinqQueryProvider queryProvider, Expression queryExpression)
        {
            this.queryProvider = queryProvider;
            this.queryExpression = queryExpression;
        }

        #region IEnumerable<T> Members

        /// <summary>Executes the query and returns an enumerator with the results.</summary>
        /// <returns>The results of the query execution.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.queryProvider.ExecuteQuery<T>(this.queryExpression);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>Executes the query and returns an enumerator with the results.</summary>
        /// <returns>The results of the query execution.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.queryProvider.ExecuteQuery<T>(this.queryExpression);
        }

        #endregion

        #region IQueryable Members

        /// <summary>The type of the result of the query.</summary>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>The expression tree for this query (the DSP version).</summary>
        public Expression Expression
        {
            get { return this.queryExpression; }
        }

        /// <summary>The provider for this query.</summary>
        public IQueryProvider Provider
        {
            get { return this.queryProvider; }
        }

        #endregion
    }
}
