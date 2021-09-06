//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceQueryContinuation.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.DataServiceQueryContinuation' type.
    /// </summary>
    public partial class WrappedDataServiceQueryContinuation : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceQueryContinuation class.
        /// </summary>
        static WrappedDataServiceQueryContinuation()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceQueryContinuation", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceQueryContinuation class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceQueryContinuation(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'NextLinkUri' property on 'Microsoft.OData.Client.DataServiceQueryContinuation'
        /// </summary>
        public virtual System.Uri NextLinkUri
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_NextLinkUri()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'System.String ToString()' on the 'Microsoft.OData.Client.DataServiceQueryContinuation' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public new virtual string ToString()
        {
            return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String ToString()"), new object[] { });
        }
    }
}
