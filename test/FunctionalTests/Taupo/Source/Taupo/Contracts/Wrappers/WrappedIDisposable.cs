//---------------------------------------------------------------------
// <copyright file="WrappedIDisposable.cs" company="Microsoft">
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
    /// Wraps the 'System.IDisposable' type.
    /// </summary>
    public partial class WrappedIDisposable : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedIDisposable class.
        /// </summary>
        static WrappedIDisposable()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.IDisposable", "mscorlib");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedIDisposable class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedIDisposable(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Wraps the 'Void Dispose()' on the 'System.IDisposable' type.
        /// </summary>
        public virtual void Dispose()
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void Dispose()"), new object[] { });
        }
    }
}
