//---------------------------------------------------------------------
// <copyright file="WrappedIEnumerable.cs" company="Microsoft">
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
    /// Wraps the 'System.Collections.IEnumerable' type.
    /// </summary>
    public partial class WrappedIEnumerable : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedIEnumerable class.
        /// </summary>
        static WrappedIEnumerable()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Collections.IEnumerable", "mscorlib");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedIEnumerable class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedIEnumerable(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Wraps the 'System.Collections.IEnumerator GetEnumerator()' on the 'System.Collections.IEnumerable' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerator GetEnumerator()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerator>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.IEnumerator GetEnumerator()"), new object[] { });
        }
    }
}
