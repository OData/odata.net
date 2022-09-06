//---------------------------------------------------------------------
// <copyright file="WrappedReadOnlyCollection`1.cs" company="Microsoft">
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
    /// Wraps the 'System.Collections.ObjectModel.ReadOnlyCollection`1' type.
    /// </summary>
    /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
    public partial class WrappedReadOnlyCollection<T> : WrappedObject
      where T : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedReadOnlyCollection class.
        /// </summary>
        static WrappedReadOnlyCollection()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Collections.ObjectModel.ReadOnlyCollection`1", "mscorlib");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedReadOnlyCollection class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedReadOnlyCollection(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'Count' property on 'System.Collections.ObjectModel.ReadOnlyCollection`1'
        /// </summary>
        public virtual int Count
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<int>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int32 get_Count()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Item' property on 'System.Collections.ObjectModel.ReadOnlyCollection`1'
        /// </summary>
        /// <param name="index">The value of the 'index' parameter.</param>
        public virtual T this[int index]
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<T>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "T get_Item(Int32)"), new object[] { index });
            }
        }
        
        /// <summary>
        /// Wraps the 'Boolean Contains(T)' on the 'System.Collections.ObjectModel.ReadOnlyCollection`1' type.
        /// </summary>
        /// <param name="value">The value of the 'value' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool Contains(T value)
        {
            return WrapperUtilities.InvokeMethodAndCast<bool>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean Contains(T)"), new object[] { value });
        }
        
        /// <summary>
        /// Wraps the 'Void CopyTo(T[], Int32)' on the 'System.Collections.ObjectModel.ReadOnlyCollection`1' type.
        /// </summary>
        /// <param name="array">The value of the 'array' parameter.</param>
        /// <param name="index">The value of the 'index' parameter.</param>
        public virtual void CopyTo(WrappedArray<T> array, int index)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void CopyTo(T[], Int32)"), new object[] { array, index });
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerator`1[T] GetEnumerator()' on the 'System.Collections.ObjectModel.ReadOnlyCollection`1' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerator<T> GetEnumerator()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerator<T>>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerator`1[T] GetEnumerator()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Int32 IndexOf(T)' on the 'System.Collections.ObjectModel.ReadOnlyCollection`1' type.
        /// </summary>
        /// <param name="value">The value of the 'value' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual int IndexOf(T value)
        {
            return WrapperUtilities.InvokeMethodAndCast<int>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int32 IndexOf(T)"), new object[] { value });
        }
    }
}
