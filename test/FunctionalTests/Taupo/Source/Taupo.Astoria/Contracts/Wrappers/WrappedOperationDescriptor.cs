//---------------------------------------------------------------------
// <copyright file="WrappedOperationDescriptor.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.OperationDescriptor' type.
    /// </summary>
    public partial class WrappedOperationDescriptor : WrappedDescriptor
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedOperationDescriptor class.
        /// </summary>
        static WrappedOperationDescriptor()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.OperationDescriptor", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedOperationDescriptor class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedOperationDescriptor(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'Metadata' property on 'Microsoft.OData.Client.OperationDescriptor'
        /// </summary>
        public virtual System.Uri Metadata
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_Metadata()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Target' property on 'Microsoft.OData.Client.OperationDescriptor'
        /// </summary>
        public virtual System.Uri Target
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_Target()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Title' property on 'Microsoft.OData.Client.OperationDescriptor'
        /// </summary>
        public virtual string Title
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_Title()"), new object[] { });
            }
        }
    }
}
