//---------------------------------------------------------------------
// <copyright file="WrappedIQueryable.cs" company="Microsoft">
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
    /// Wraps the 'System.Linq.IQueryable' type.
    /// </summary>
    public partial class WrappedIQueryable : WrappedIEnumerable
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedIQueryable class.
        /// </summary>
        static WrappedIQueryable()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Linq.IQueryable", "System.Core");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedIQueryable class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedIQueryable(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Gets a value of the 'ElementType' property on 'System.Linq.IQueryable'
        /// </summary>
        public virtual Type ElementType
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<Type>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Type get_ElementType()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Expression' property on 'System.Linq.IQueryable'
        /// </summary>
        public virtual WrappedObject Expression
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Linq.Expressions.Expression get_Expression()"), new object[] { });
            }
        }
        
        /// <summary>
        /// Gets a value of the 'Provider' property on 'System.Linq.IQueryable'
        /// </summary>
        public virtual WrappedIQueryProvider Provider
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedIQueryProvider>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Linq.IQueryProvider get_Provider()"), new object[] { });
            }
        }
    }
}
