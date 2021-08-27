//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceStreamLink.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.DataServiceStreamLink' type.
    /// </summary>
    public partial class WrappedDataServiceStreamLink : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedDataServiceStreamLink class.
        /// </summary>
        static WrappedDataServiceStreamLink()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceStreamLink", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceStreamLink class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceStreamLink(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'ContentType' property on 'Microsoft.OData.Client.DataServiceStreamLink'
        /// </summary>
        public virtual string ContentType
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_ContentType()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'EditLink' property on 'Microsoft.OData.Client.DataServiceStreamLink'
        /// </summary>
        public virtual System.Uri EditLink
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_EditLink()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'ETag' property on 'Microsoft.OData.Client.DataServiceStreamLink'
        /// </summary>
        public virtual string ETag
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_ETag()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Name' property on 'Microsoft.OData.Client.DataServiceStreamLink'
        /// </summary>
        public virtual string Name
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_Name()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'SelfLink' property on 'Microsoft.OData.Client.DataServiceStreamLink'
        /// </summary>
        public virtual System.Uri SelfLink
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_SelfLink()"), new object[] { });
            }
        }
    }
}
