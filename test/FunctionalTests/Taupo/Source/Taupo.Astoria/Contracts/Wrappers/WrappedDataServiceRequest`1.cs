//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceRequest`1.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.DataServiceRequest`1' type.
    /// </summary>
    /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
    public partial class WrappedDataServiceRequest<TElement> : WrappedDataServiceRequest
      where TElement : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceRequest class.
        /// </summary>
        static WrappedDataServiceRequest()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceRequest`1", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceRequest class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceRequest(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'ElementType' property on 'Microsoft.OData.Client.DataServiceRequest`1'
        /// </summary>
        public new virtual Type ElementType
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<Type>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Type get_ElementType()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'RequestUri' property on 'Microsoft.OData.Client.DataServiceRequest`1'
        /// </summary>
        public new virtual System.Uri RequestUri
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_RequestUri()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'System.String ToString()' on the 'Microsoft.OData.Client.DataServiceRequest`1' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public new virtual string ToString()
        {
            return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String ToString()"), new object[] { });
        }
    }
}
