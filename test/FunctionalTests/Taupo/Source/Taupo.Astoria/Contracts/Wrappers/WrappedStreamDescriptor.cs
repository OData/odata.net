//---------------------------------------------------------------------
// <copyright file="WrappedStreamDescriptor.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.StreamDescriptor' type.
    /// </summary>
    public partial class WrappedStreamDescriptor : WrappedDescriptor
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedStreamDescriptor class.
        /// </summary>
        static WrappedStreamDescriptor()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.StreamDescriptor", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedStreamDescriptor class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedStreamDescriptor(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets or sets a value of the 'EntityDescriptor' property on 'Microsoft.OData.Client.StreamDescriptor'
        /// </summary>
        public virtual WrappedEntityDescriptor EntityDescriptor
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedEntityDescriptor>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.EntityDescriptor get_EntityDescriptor()"), new object[] { });
            }
            
            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_EntityDescriptor(Microsoft.OData.Client.EntityDescriptor)"), new object[] { value });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'StreamLink' property on 'Microsoft.OData.Client.StreamDescriptor'
        /// </summary>
        public virtual WrappedDataServiceStreamLink StreamLink
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceStreamLink>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceStreamLink get_StreamLink()"), new object[] { });
            }
        }
    }
}
