//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceRequest.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.DataServiceRequest' type.
    /// </summary>
    public partial class WrappedDataServiceRequest : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceRequest class.
        /// </summary>
        static WrappedDataServiceRequest()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceRequest", "Microsoft.OData.Client");
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
        /// Gets a value of the 'ElementType' property on 'Microsoft.OData.Client.DataServiceRequest'
        /// </summary>
        public virtual Type ElementType
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<Type>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Type get_ElementType()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'RequestUri' property on 'Microsoft.OData.Client.DataServiceRequest'
        /// </summary>
        public virtual System.Uri RequestUri
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_RequestUri()"), new object[] { });
            }
        }
    }
}
