//---------------------------------------------------------------------
// <copyright file="WrappedLinkDescriptor.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.LinkDescriptor' type.
    /// </summary>
    public partial class WrappedLinkDescriptor : WrappedDescriptor
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedLinkDescriptor class.
        /// </summary>
        static WrappedLinkDescriptor()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.LinkDescriptor", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedLinkDescriptor class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedLinkDescriptor(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'Source' property on 'Microsoft.OData.Client.LinkDescriptor'
        /// </summary>
        public virtual WrappedObject Source
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Object get_Source()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'SourceProperty' property on 'Microsoft.OData.Client.LinkDescriptor'
        /// </summary>
        public virtual string SourceProperty
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_SourceProperty()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Target' property on 'Microsoft.OData.Client.LinkDescriptor'
        /// </summary>
        public virtual WrappedObject Target
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Object get_Target()"), new object[] { });
            }
        }
    }
}
