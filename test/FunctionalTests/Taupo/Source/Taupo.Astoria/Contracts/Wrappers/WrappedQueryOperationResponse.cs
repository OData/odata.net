//---------------------------------------------------------------------
// <copyright file="WrappedQueryOperationResponse.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.QueryOperationResponse' type.
    /// </summary>
    public partial class WrappedQueryOperationResponse : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedQueryOperationResponse class.
        /// </summary>
        static WrappedQueryOperationResponse()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.QueryOperationResponse", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedQueryOperationResponse class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedQueryOperationResponse(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'Query' property on 'Microsoft.OData.Client.QueryOperationResponse'
        /// </summary>
        public virtual WrappedDataServiceRequest Query
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceRequest>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceRequest get_Query()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'TotalCount' property on 'Microsoft.OData.Client.QueryOperationResponse'
        /// </summary>
        public virtual long TotalCount
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<long>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int64 get_TotalCount()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQueryContinuation GetContinuation()' on the 'Microsoft.OData.Client.QueryOperationResponse' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceQueryContinuation GetContinuation()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQueryContinuation>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQueryContinuation GetContinuation()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQueryContinuation GetContinuation(System.Collections.IEnumerable)' on the 'Microsoft.OData.Client.QueryOperationResponse' type.
        /// </summary>
        /// <param name="collection">The value of the 'collection' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceQueryContinuation GetContinuation(WrappedIEnumerable collection)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQueryContinuation>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQueryContinuation GetContinuation(System.Collections.IEnumerable)"), new object[] { collection });
        }
        
        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQueryContinuation`1[T] GetContinuation[T](System.Collections.Generic.IEnumerable`1[T])' on the 'Microsoft.OData.Client.QueryOperationResponse' type.
        /// </summary>
        /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
        /// <param name="typeT">The CLR generic type for the 'T' parameter.</param>
        /// <param name="collection">The value of the 'collection' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceQueryContinuation<T> GetContinuation<T>(Type typeT, WrappedIEnumerable<T> collection)
          where T : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQueryContinuation<T>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQueryContinuation`1[T] GetContinuation[T](System.Collections.Generic.IEnumerable`1[T])"), new object[] { collection }, new Type[] { typeT });
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.IEnumerator GetEnumerator()' on the 'Microsoft.OData.Client.QueryOperationResponse' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerator GetEnumerator()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerator>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.IEnumerator GetEnumerator()"), new object[] { });
        }
    }
}
