//---------------------------------------------------------------------
// <copyright file="WrappedIEnumerator.cs" company="Microsoft">
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
    /// Wraps the 'System.Collections.IEnumerator' type.
    /// </summary>
    public partial class WrappedIEnumerator : WrappedIDisposable
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedIEnumerator class.
        /// </summary>
        static WrappedIEnumerator()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Collections.IEnumerator", "mscorlib");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedIEnumerator class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedIEnumerator(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'Current' property on 'System.Collections.IEnumerator'
        /// </summary>
        public virtual WrappedObject Current
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Object get_Current()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Wraps the 'Boolean MoveNext()' on the 'System.Collections.IEnumerator' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool MoveNext()
        {
            return WrapperUtilities.InvokeMethodAndCast<bool>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean MoveNext()"), new object[] { });
        }
        
        /// <summary>
        /// Wraps the 'Void Reset()' on the 'System.Collections.IEnumerator' type.
        /// </summary>
        public virtual void Reset()
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Reset()"), new object[] { });
        }
    }
}
