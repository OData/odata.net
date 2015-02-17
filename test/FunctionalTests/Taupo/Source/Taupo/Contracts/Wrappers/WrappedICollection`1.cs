//---------------------------------------------------------------------
// <copyright file="WrappedICollection`1.cs" company="Microsoft">
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
    /// Wraps the 'System.Collections.Generic.ICollection`1' type.
    /// </summary>
    /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
    public partial class WrappedICollection<T> : WrappedIEnumerable<T>
      where T : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedICollection class.
        /// </summary>
        static WrappedICollection()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Collections.Generic.ICollection`1", "mscorlib");
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
        /// Gets a value of the 'Count' property on 'System.Collections.Generic.ICollection`1'
        /// </summary>
        public virtual int Count
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<int>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int32 get_Count()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether IsReadOnly on 'System.Collections.Generic.ICollection`1' is set to true
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean get_IsReadOnly()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'Void Add(T)' on the 'System.Collections.Generic.ICollection`1' type.
        /// </summary>
        /// <param name="item">The value of the 'item' parameter.</param>
        public virtual void Add(T item)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Add(T)"), new object[] { item });
        }
        
        /// <summary>
        /// Wraps the 'Void Clear()' on the 'System.Collections.Generic.ICollection`1' type.
        /// </summary>
        public virtual void Clear()
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Clear()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Boolean Contains(T)' on the 'System.Collections.Generic.ICollection`1' type.
        /// </summary>
        /// <param name="item">The value of the 'item' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool Contains(T item)
        {
            return WrapperUtilities.InvokeMethodAndCast<bool>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean Contains(T)"), new object[] { item });
        }
        
        /// <summary>
        /// Wraps the 'Void CopyTo(T[], Int32)' on the 'System.Collections.Generic.ICollection`1' type.
        /// </summary>
        /// <param name="array">The value of the 'array' parameter.</param>
        /// <param name="arrayIndex">The value of the 'arrayIndex' parameter.</param>
        public virtual void CopyTo(WrappedArray<T> array, int arrayIndex)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void CopyTo(T[], Int32)"), new object[] { array, arrayIndex });
        }
        
        /// <summary>
        /// Wraps the 'Boolean Remove(T)' on the 'System.Collections.Generic.ICollection`1' type.
        /// </summary>
        /// <param name="item">The value of the 'item' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool Remove(T item)
        {
            return WrapperUtilities.InvokeMethodAndCast<bool>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean Remove(T)"), new object[] { item });
        }
    }
}
