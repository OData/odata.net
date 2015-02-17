//---------------------------------------------------------------------
// <copyright file="EnumerableProxy.cs" company="Microsoft">
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
    /// Wraps the 'System.Collections.IEnumerable' type.
    /// </summary>
    public partial class EnumerableProxy : System.Collections.IEnumerable, IProxyObject, IDisposable
    {
        private static readonly IDictionary<string, Type> TypeCache = new Dictionary<string, Type>();
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        private System.Collections.IEnumerable underlyingImplementation;
        
        /// <summary>
        /// Initializes a new instance of the EnumerableProxy class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="underlyingImplementation">The underlying implementation of the proxy.</param>
        public EnumerableProxy(IWrapperScope wrapperScope, System.Collections.IEnumerable underlyingImplementation)
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
        /// Wraps the 'System.Collections.IEnumerator GetEnumerator()' on the 'System.Collections.IEnumerable' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.Collections.IEnumerator GetEnumerator()
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Collections.IEnumerable", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "System.Collections.IEnumerator GetEnumerator()");
            return (System.Collections.IEnumerator)ProxyFactory.CreateProxyIfPossible(this.Scope, WrapperUtilities.InvokeMethodAndCast<System.Collections.IEnumerator>(this, methodInfo, new object[] { }), typeof(System.Collections.IEnumerator));
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
