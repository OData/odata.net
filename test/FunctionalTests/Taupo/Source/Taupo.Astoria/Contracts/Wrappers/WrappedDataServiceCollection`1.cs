//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceCollection`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this code has been generated using WrapperGenerator
// Do not modify the file, but instead modify and re-run the generator.

// disable warnings about 'new' keyword being required/not needed
#pragma warning disable 108, 109

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    
    /// <summary>
    /// Wraps the 'Microsoft.OData.Client.DataServiceCollection`1' type.
    /// </summary>
    /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
    public partial class WrappedDataServiceCollection<T> : WrappedObject
      where T : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceCollection class.
        /// </summary>
        static WrappedDataServiceCollection()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceCollection`1", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceCollection class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceCollection(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets or sets a value of the 'Continuation' property on 'Microsoft.OData.Client.DataServiceCollection`1'
        /// </summary>
        public virtual WrappedDataServiceQueryContinuation<T> Continuation
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQueryContinuation<T>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQueryContinuation`1[T] get_Continuation()"), new object[] { });
            }
            
            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_Continuation(Microsoft.OData.Client.DataServiceQueryContinuation`1[T])"), new object[] { value });
            }
        }
        
        /// <summary>
        /// Wraps the 'Void Clear(Boolean)' on the 'Microsoft.OData.Client.DataServiceCollection`1' type.
        /// </summary>
        /// <param name="stopTracking">The value of the 'stopTracking' parameter.</param>
        public virtual void Clear(bool stopTracking)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Clear(Boolean)"), new object[] { stopTracking });
        }
        
        /// <summary>
        /// Wraps the 'Void Detach()' on the 'Microsoft.OData.Client.DataServiceCollection`1' type.
        /// </summary>
        public virtual void Detach()
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Detach()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Void Load(System.Collections.Generic.IEnumerable`1[T])' on the 'Microsoft.OData.Client.DataServiceCollection`1' type.
        /// </summary>
        /// <param name="items">The value of the 'items' parameter.</param>
        public virtual void Load(WrappedIEnumerable<T> items)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Load(System.Collections.Generic.IEnumerable`1[T])"), new object[] { items });
        }
        
        /// <summary>
        /// Wraps the 'Void Load(T)' on the 'Microsoft.OData.Client.DataServiceCollection`1' type.
        /// </summary>
        /// <param name="item">The value of the 'item' parameter.</param>
        public virtual void Load(T item)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Load(T)"), new object[] { item });
        }
    }
}
