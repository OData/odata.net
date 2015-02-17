//---------------------------------------------------------------------
// <copyright file="WrappedIEnumerator`1.cs" company="Microsoft">
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
    /// Wraps the 'System.Collections.Generic.IEnumerator`1' type.
    /// </summary>
    /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
    public partial class WrappedIEnumerator<T> : WrappedIEnumerator
      where T : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedIEnumerator class.
        /// </summary>
        static WrappedIEnumerator()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Collections.Generic.IEnumerator`1", "mscorlib");
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
        /// Gets a value of the 'Current' property on 'System.Collections.Generic.IEnumerator`1'
        /// </summary>
        public new virtual T Current
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<T>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "T get_Current()"), new object[] { });
            }
        }
    }
}
