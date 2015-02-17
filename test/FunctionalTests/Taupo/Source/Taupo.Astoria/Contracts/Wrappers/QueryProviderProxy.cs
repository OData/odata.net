//---------------------------------------------------------------------
// <copyright file="QueryProviderProxy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this code has been generated using WrapperGenerator
// Do not modify the file, but instead modify and re-run the generator.

// disable warnings about 'new' keyword being required/not needed and for obsolete classes
#pragma warning disable 108, 109, 618

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    
    /// <summary>
    /// Wraps the 'System.Linq.IQueryProvider' type.
    /// </summary>
    public partial class QueryProviderProxy : System.Linq.IQueryProvider, IProxyObject, IDisposable
    {
        private static readonly IDictionary<string, Type> TypeCache = new Dictionary<string, Type>();
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        private System.Linq.IQueryProvider underlyingImplementation;
        
        /// <summary>
        /// Initializes a new instance of the QueryProviderProxy class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="underlyingImplementation">The underlying implementation of the proxy.</param>
        public QueryProviderProxy(IWrapperScope wrapperScope, System.Linq.IQueryProvider underlyingImplementation)
        {
            ExceptionUtilities.CheckArgumentNotNull(wrapperScope, "wrapperScope");
            ExceptionUtilities.CheckArgumentNotNull(underlyingImplementation, "underlyingImplementation");
            
            this.Scope = wrapperScope;
            this.underlyingImplementation = underlyingImplementation;
        }
        
        /// <summary>
        /// Gets the wrapper scope.
        /// </summary>
        public IWrapperScope Scope { get; private set; }
        
        /// <summary>
        /// Gets the product instance wrapped by this wrapper.
        /// </summary>
        public object Product
        {
            get { return this.underlyingImplementation; }
        }
        
        /// <summary>
        /// Wraps the 'System.Linq.IQueryable CreateQuery(System.Linq.Expressions.Expression)' on the 'System.Linq.IQueryProvider' type.
        /// </summary>
        /// <param name="expression">The value of the 'expression' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.Linq.IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Linq.IQueryProvider", "System.Core", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "System.Linq.IQueryable CreateQuery(System.Linq.Expressions.Expression)");
            return (System.Linq.IQueryable)ProxyFactory.CreateProxyIfPossible(this.Scope, WrapperUtilities.InvokeMethodAndCast<System.Linq.IQueryable>(this, methodInfo, new object[] { expression }), typeof(System.Linq.IQueryable));
        }
        
        /// <summary>
        /// Wraps the 'System.Linq.IQueryable`1[TElement] CreateQuery[TElement](System.Linq.Expressions.Expression)' on the 'System.Linq.IQueryProvider' type.
        /// </summary>
        /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
        /// <param name="expression">The value of the 'expression' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.Linq.IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Linq.IQueryProvider", "System.Core", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "System.Linq.IQueryable`1[TElement] CreateQuery[TElement](System.Linq.Expressions.Expression)");
            return (System.Linq.IQueryable<TElement>)ProxyFactory.CreateProxyIfPossible(this.Scope, WrapperUtilities.InvokeMethodAndCast<System.Linq.IQueryable<TElement>>(this, methodInfo, new object[] { expression }, new Type[] { typeof(TElement) }), typeof(System.Linq.IQueryable<TElement>));
        }
        
        /// <summary>
        /// Wraps the 'System.Object Execute(System.Linq.Expressions.Expression)' on the 'System.Linq.IQueryProvider' type.
        /// </summary>
        /// <param name="expression">The value of the 'expression' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual object Execute(System.Linq.Expressions.Expression expression)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Linq.IQueryProvider", "System.Core", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "System.Object Execute(System.Linq.Expressions.Expression)");
            return WrapperUtilities.InvokeMethodAndCast<object>(this, methodInfo, new object[] { expression });
        }
        
        /// <summary>
        /// Wraps the 'TResult Execute[TResult](System.Linq.Expressions.Expression)' on the 'System.Linq.IQueryProvider' type.
        /// </summary>
        /// <typeparam name="TResult">The wrapper type for the 'TResult' generic parameter.</typeparam>
        /// <param name="expression">The value of the 'expression' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Linq.IQueryProvider", "System.Core", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "TResult Execute[TResult](System.Linq.Expressions.Expression)");
            return WrapperUtilities.InvokeMethodAndCast<TResult>(this, methodInfo, new object[] { expression }, new Type[] { typeof(TResult) });
        }
        
        /// <summary>
        /// Disposes the wrapped instance if it implements IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Disposes the wrapped instance if it implements IDisposable.
        /// </summary>
        /// <param name="disposing">Whether or not to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            CallOrderUtilities.TryWrapArbitraryMethodCall(
                () => this.Dispose(),
                () =>
                    {
                        var d = this.underlyingImplementation as IDisposable;
                        if (d != null)
                        {
                            d.Dispose();
                            this.underlyingImplementation = null;
                        }
                    });
        }
    }
}
