//---------------------------------------------------------------------
// <copyright file="WrappedICollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this code has been generated using WrapperGenerator
// Do not modify the file, but instead modify and re-run the generator.

// disable warnings about 'new' keyword being required/not needed
#pragma warning disable 108, 109

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Contracts;
    
    /// <summary>
    /// Wraps the 'System.Collections.ICollection' type.
    /// </summary>
    public partial class WrappedICollection : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedICollection class.
        /// </summary>
        static WrappedICollection()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Collections.ICollection", "mscorlib");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedICollection class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedICollection(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'Count' property on 'System.Collections.ICollection'
        /// </summary>
        public virtual int Count
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<int>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int32 get_Count()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether IsSynchronized on 'System.Collections.ICollection' is set to true
        /// </summary>
        public virtual bool IsSynchronized
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean get_IsSynchronized()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'SyncRoot' property on 'System.Collections.ICollection'
        /// </summary>
        public virtual WrappedObject SyncRoot
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Object get_SyncRoot()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'Void CopyTo(System.Array, Int32)' on the 'System.Collections.ICollection' type.
        /// </summary>
        /// <param name="array">The value of the 'array' parameter.</param>
        /// <param name="index">The value of the 'index' parameter.</param>
        public virtual void CopyTo(WrappedObject array, int index)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void CopyTo(System.Array, Int32)"), new object[] { array, index });
        }
    }
}
