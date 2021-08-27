//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceResponse.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.DataServiceResponse' type.
    /// </summary>
    public partial class WrappedDataServiceResponse : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceResponse class.
        /// </summary>
        static WrappedDataServiceResponse()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceResponse", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceResponse class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceResponse(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'BatchHeaders' property on 'Microsoft.OData.Client.DataServiceResponse'
        /// </summary>
        public virtual WrappedObject BatchHeaders
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IDictionary`2[System.String,System.String] get_BatchHeaders()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'BatchStatusCode' property on 'Microsoft.OData.Client.DataServiceResponse'
        /// </summary>
        public virtual int BatchStatusCode
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<int>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int32 get_BatchStatusCode()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether IsBatchResponse on 'Microsoft.OData.Client.DataServiceResponse' is set to true
        /// </summary>
        public virtual bool IsBatchResponse
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean get_IsBatchResponse()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerator`1[Microsoft.OData.Client.OperationResponse] GetEnumerator()' on the 'Microsoft.OData.Client.DataServiceResponse' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerator<WrappedObject> GetEnumerator()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerator<WrappedObject>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerator`1[Microsoft.OData.Client.OperationResponse] GetEnumerator()"), new object[] { });
        }
    }
}
