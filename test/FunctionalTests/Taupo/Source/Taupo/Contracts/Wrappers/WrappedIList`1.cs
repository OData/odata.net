//---------------------------------------------------------------------
// <copyright file="WrappedIList`1.cs" company="Microsoft">
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
    /// Wraps the 'System.Collections.Generic.IList`1' type.
    /// </summary>
    /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
    public partial class WrappedIList<T> : WrappedICollection<T>
      where T : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedIList class.
        /// </summary>
        static WrappedIList()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Collections.Generic.IList`1", "mscorlib");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedIList class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedIList(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets or sets a value of the 'Item' property on 'System.Collections.Generic.IList`1'
        /// </summary>
        /// <param name="index">The value of the 'index' parameter.</param>
        public virtual T this[int index]
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<T>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "T get_Item(Int32)"), new object[] { index });
            }
            
            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_Item(Int32, T)"), new object[] { index, value });
            }
        }
        
        /// <summary>
        /// Wraps the 'Int32 IndexOf(T)' on the 'System.Collections.Generic.IList`1' type.
        /// </summary>
        /// <param name="item">The value of the 'item' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual int IndexOf(T item)
        {
            return WrapperUtilities.InvokeMethodAndCast<int>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int32 IndexOf(T)"), new object[] { item });
        }
        
        /// <summary>
        /// Wraps the 'Void Insert(Int32, T)' on the 'System.Collections.Generic.IList`1' type.
        /// </summary>
        /// <param name="index">The value of the 'index' parameter.</param>
        /// <param name="item">The value of the 'item' parameter.</param>
        public virtual void Insert(int index, T item)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Insert(Int32, T)"), new object[] { index, item });
        }
        
        /// <summary>
        /// Wraps the 'Void RemoveAt(Int32)' on the 'System.Collections.Generic.IList`1' type.
        /// </summary>
        /// <param name="index">The value of the 'index' parameter.</param>
        public virtual void RemoveAt(int index)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void RemoveAt(Int32)"), new object[] { index });
        }
    }
}
