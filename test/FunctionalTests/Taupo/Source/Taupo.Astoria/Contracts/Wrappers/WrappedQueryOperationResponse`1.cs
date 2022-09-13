//---------------------------------------------------------------------
// <copyright file="WrappedQueryOperationResponse`1.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.QueryOperationResponse`1' type.
    /// </summary>
    /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
    public partial class WrappedQueryOperationResponse<T> : WrappedQueryOperationResponse
      where T : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedQueryOperationResponse class.
        /// </summary>
        static WrappedQueryOperationResponse()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.QueryOperationResponse`1", "Microsoft.OData.Client");
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
        /// Gets a value of the 'TotalCount' property on 'Microsoft.OData.Client.QueryOperationResponse`1'
        /// </summary>
        public new virtual long TotalCount
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<long>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int64 get_TotalCount()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQueryContinuation`1[T] GetContinuation()' on the 'Microsoft.OData.Client.QueryOperationResponse`1' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public new virtual WrappedDataServiceQueryContinuation<T> GetContinuation()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQueryContinuation<T>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQueryContinuation`1[T] GetContinuation()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerator`1[T] GetEnumerator()' on the 'Microsoft.OData.Client.QueryOperationResponse`1' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public new virtual WrappedIEnumerator<T> GetEnumerator()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerator<T>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerator`1[T] GetEnumerator()"), new object[] { });
        }
    }
}
