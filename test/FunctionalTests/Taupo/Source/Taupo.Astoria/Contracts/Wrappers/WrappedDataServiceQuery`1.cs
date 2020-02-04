//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceQuery`1.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.DataServiceQuery`1' type.
    /// </summary>
    /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
    public partial class WrappedDataServiceQuery<TElement> : WrappedDataServiceQuery
      where TElement : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceQuery class.
        /// </summary>
        static WrappedDataServiceQuery()
        {
#if SILVERLIGHT && !WIN8 && !WINDOWS_PHONE
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceQuery`1", "Microsoft.OData.Client.SL");
#else
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceQuery`1", "Microsoft.OData.Client");
#endif
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceQuery class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceQuery(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'ElementType' property on 'Microsoft.OData.Client.DataServiceQuery`1'
        /// </summary>
        public new virtual Type ElementType
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<Type>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Type get_ElementType()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Expression' property on 'Microsoft.OData.Client.DataServiceQuery`1'
        /// </summary>
        public new virtual WrappedObject Expression
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Linq.Expressions.Expression get_Expression()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Provider' property on 'Microsoft.OData.Client.DataServiceQuery`1'
        /// </summary>
        public new virtual WrappedIQueryProvider Provider
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedIQueryProvider>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Linq.IQueryProvider get_Provider()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'RequestUri' property on 'Microsoft.OData.Client.DataServiceQuery`1'
        /// </summary>
        public new virtual System.Uri RequestUri
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_RequestUri()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQuery`1[TElement] AddQueryOption(System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <param name="name">The value of the 'name' parameter.</param>
        /// <param name="value">The value of the 'value' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceQuery<TElement> AddQueryOption(string name, WrappedObject value)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQuery<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQuery`1[TElement] AddQueryOption(System.String, System.Object)"), new object[] { name, value });
        }
        
        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginExecute(System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public new virtual System.IAsyncResult BeginExecute(System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginExecute(System.AsyncCallback, System.Object)"), new object[] { callback, state });
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerable`1[TElement] EndExecute(System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public new virtual WrappedIEnumerable<TElement> EndExecute(System.IAsyncResult asyncResult)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerable<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerable`1[TElement] EndExecute(System.IAsyncResult)"), new object[] { asyncResult });
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerable`1[TElement] Execute()' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public new virtual WrappedIEnumerable<TElement> Execute()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerable<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerable`1[TElement] Execute()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQuery`1[TElement] Expand(System.String)' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <param name="path">The value of the 'path' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceQuery<TElement> Expand(string path)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQuery<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQuery`1[TElement] Expand(System.String)"), new object[] { path });
        }
        
        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQuery`1[TElement] Expand[TTarget](System.Linq.Expressions.Expression`1[System.Func`2[TElement,TTarget]])' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <typeparam name="TTarget">The wrapper type for the 'TTarget' generic parameter.</typeparam>
        /// <param name="typeTTarget">The CLR generic type for the 'TTarget' parameter.</param>
        /// <param name="navigationPropertyAccessor">The value of the 'navigationPropertyAccessor' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceQuery<TElement> Expand<TTarget>(Type typeTTarget, System.Linq.Expressions.Expression<System.Func<TElement, TTarget>> navigationPropertyAccessor)
          where TTarget : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQuery<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQuery`1[TElement] Expand[TTarget](System.Linq.Expressions.Expression`1[System.Func`2[TElement,TTarget]])"), new object[] { navigationPropertyAccessor }, new Type[] { typeTTarget });
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerator`1[TElement] GetEnumerator()' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerator<TElement> GetEnumerator()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerator<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerator`1[TElement] GetEnumerator()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQuery`1[TElement] IncludeCount()' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceQuery<TElement> IncludeCount()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQuery<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQuery`1[TElement] IncludeCount()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'System.String ToString()' on the 'Microsoft.OData.Client.DataServiceQuery`1' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public new virtual string ToString()
        {
            return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String ToString()"), new object[] { });
        }
    }
}
