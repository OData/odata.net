//---------------------------------------------------------------------
// <copyright file="EnumeratorProxy.cs" company="Microsoft">
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
    /// Wraps the 'System.Collections.IEnumerator' type.
    /// </summary>
    public partial class EnumeratorProxy : System.Collections.IEnumerator, IProxyObject, IDisposable
    {
        private static readonly IDictionary<string, Type> TypeCache = new Dictionary<string, Type>();
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        private System.Collections.IEnumerator underlyingImplementation;
        
        /// <summary>
        /// Initializes a new instance of the EnumeratorProxy class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="underlyingImplementation">The underlying implementation of the proxy.</param>
        public EnumeratorProxy(IWrapperScope wrapperScope, System.Collections.IEnumerator underlyingImplementation)
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
        /// Gets a value of the 'Current' property on 'System.Collections.IEnumerator'
        /// </summary>
        public virtual object Current
        {
            get
            {
                var type = WrapperUtilities.GetTypeFromCache("System.Collections.IEnumerator", "mscorlib", TypeCache);
                var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "System.Object get_Current()");
                return WrapperUtilities.InvokeMethodAndCast<object>(this, methodInfo, new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'Boolean MoveNext()' on the 'System.Collections.IEnumerator' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool MoveNext()
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Collections.IEnumerator", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Boolean MoveNext()");
            return WrapperUtilities.InvokeMethodAndCast<bool>(this, methodInfo, new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Void Reset()' on the 'System.Collections.IEnumerator' type.
        /// </summary>
        public virtual void Reset()
        {
            var type = WrapperUtilities.GetTypeFromCache("System.Collections.IEnumerator", "mscorlib", TypeCache);
            var methodInfo = WrapperUtilities.GetMethodInfo(type, MethodInfoCache, "Void Reset()");
            WrapperUtilities.InvokeMethodWithoutResult(this, methodInfo, new object[] { });
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
