//---------------------------------------------------------------------
// <copyright file="MethodReplacingQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Dictionary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Test.OData.Framework.TestProviders.Common;

    /// <summary>
    /// Query provider implementation that replaces methods before evaluating the query against an underlying provider
    /// </summary>
    internal class MethodReplacingQueryProvider : IQueryProvider
    {
        private ExpressionVisitor replacingVisitor;

        /// <summary>
        /// Initializes a new instance of the MethodReplacingQueryProvider class
        /// </summary>
        /// <param name="underlyingProvider">The underlying provider</param>
        /// <param name="replacingVisitor">The visitor for replacing method calls</param>
        public MethodReplacingQueryProvider(IQueryProvider underlyingProvider, ExpressionVisitor replacingVisitor)
        {
            ExceptionUtilities.CheckArgumentNotNull(underlyingProvider, "underlyingProvider");
            ExceptionUtilities.CheckArgumentNotNull(replacingVisitor, "replacingVisitor");

            this.UnderlyingProvider = underlyingProvider;
            this.replacingVisitor = replacingVisitor;
        }

        /// <summary>
        /// Gets the real underlying query provider
        /// </summary>
        internal IQueryProvider UnderlyingProvider { get; private set; }

        /// <summary>
        /// Creates a query for the given expression
        /// </summary>
        /// <typeparam name="TElement">The element type of the query</typeparam>
        /// <param name="expression">The given expression</param>
        /// <returns>A query with the given expression</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            return this.CreateMethodReplacingQueryable<TElement>(expression);
        }

        /// <summary>
        /// Creates a query for the given expression
        /// </summary>
        /// <param name="expression">The given expression</param>
        /// <returns>A query with the given expression</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            // Only the generic overload of CreateQuery should be used
            throw new NotImplementedException("Should not be called.");
        }

        /// <summary>
        /// Executes the given query expression
        /// </summary>
        /// <typeparam name="TResult">The return type of the expression</typeparam>
        /// <param name="expression">The expression to execute</param>
        /// <returns>The result of executing the expression</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            return this.UnderlyingProvider.Execute<TResult>(this.replacingVisitor.Visit(expression));
        }

        /// <summary>
        /// Executes the given query expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <returns>The result of executing the expression</returns>
        public object Execute(Expression expression)
        {
            throw new NotImplementedException("Not implemented");
        }

        internal IQueryable<TElement> CreateUnderlyingQuery<TElement>(Expression expression)
        {
            return this.UnderlyingProvider.CreateQuery<TElement>(this.replacingVisitor.Visit(expression));
        }

        internal IQueryable CreateUnderlyingQuery(Expression expression)
        {
            return this.UnderlyingProvider.CreateQuery(this.replacingVisitor.Visit(expression));
        }

        private IQueryable<TElement> CreateMethodReplacingQueryable<TElement>(Expression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            return new MethodReplacingQueryable<TElement>(expression, this);
        }

        /// <summary>
        /// Queryable implementation that replaces late-bound methods when enumerated
        /// </summary>
        /// <typeparam name="TElement">The element type of the queryable</typeparam>
        private class MethodReplacingQueryable<TElement> : IOrderedQueryable<TElement>
        {
            private MethodReplacingQueryProvider provider;

            /// <summary>
            /// Initializes a new instance of the MethodReplacingQueryable class
            /// </summary>
            /// <param name="expression">The query expression</param>
            /// <param name="provider">The query provider</param>
            public MethodReplacingQueryable(Expression expression, MethodReplacingQueryProvider provider)
            {
                ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
                ExceptionUtilities.CheckArgumentNotNull(provider, "provider");

                this.Expression = expression;
                this.ElementType = typeof(TElement);
                this.provider = provider;
            }

            /// <summary>
            /// Gets the element type of the queryable
            /// </summary>
            public Type ElementType { get; private set; }
            
            /// <summary>
            /// Gets the expression of the queryable
            /// </summary>
            public Expression Expression { get; private set; }

            /// <summary>
            /// Gets the query provider to use instead of the wrapped queryable's provider
            /// </summary>
            public IQueryProvider Provider
            {
                get { return this.provider; }
            }

            /// <summary>
            /// Gets an enumerator for the wrapped queryable
            /// </summary>
            /// <returns>An enumerator</returns>
            public IEnumerator<TElement> GetEnumerator()
            {
                return this.provider.CreateUnderlyingQuery<TElement>(this.Expression).GetEnumerator();
            }

            /// <summary>
            /// Gets an enumerator for the wrapped queryable
            /// </summary>
            /// <returns>An enumerator</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.provider.CreateUnderlyingQuery(this.Expression).GetEnumerator();
            }
        }
    }
}