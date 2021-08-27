//---------------------------------------------------------------------
// <copyright file="WrappedEntityDescriptor.cs" company="Microsoft">
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
    /// Wraps the 'Microsoft.OData.Client.EntityDescriptor' type.
    /// </summary>
    public partial class WrappedEntityDescriptor : WrappedDescriptor
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedEntityDescriptor class.
        /// </summary>
        static WrappedEntityDescriptor()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.EntityDescriptor", "Microsoft.OData.Client");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedEntityDescriptor class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedEntityDescriptor(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'EditLink' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual System.Uri EditLink
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_EditLink()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'EditStreamUri' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual System.Uri EditStreamUri
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_EditStreamUri()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Entity' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual WrappedObject Entity
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Object get_Entity()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'ETag' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual string ETag
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_ETag()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Identity' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual string Identity
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_Identity()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'LinkInfos' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual WrappedReadOnlyCollection<WrappedLinkInfo> LinkInfos
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedReadOnlyCollection<WrappedLinkInfo>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.OData.Client.LinkInfo] get_LinkInfos()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'OperationDescriptors' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual WrappedReadOnlyCollection<WrappedOperationDescriptor> OperationDescriptors
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedReadOnlyCollection<WrappedOperationDescriptor>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.OData.Client.OperationDescriptor] get_OperationDescriptors()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'ParentForInsert' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual WrappedEntityDescriptor ParentForInsert
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedEntityDescriptor>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.EntityDescriptor get_ParentForInsert()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'ParentPropertyForInsert' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual string ParentPropertyForInsert
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_ParentPropertyForInsert()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'ReadStreamUri' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual System.Uri ReadStreamUri
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_ReadStreamUri()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'SelfLink' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual System.Uri SelfLink
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_SelfLink()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'ServerTypeName' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual string ServerTypeName
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_ServerTypeName()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'StreamDescriptors' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual WrappedReadOnlyCollection<WrappedStreamDescriptor> StreamDescriptors
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedReadOnlyCollection<WrappedStreamDescriptor>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.OData.Client.StreamDescriptor] get_StreamDescriptors()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'StreamETag' property on 'Microsoft.OData.Client.EntityDescriptor'
        /// </summary>
        public virtual string StreamETag
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_StreamETag()"), new object[] { });
            }
        }
    }
}
