//---------------------------------------------------------------------
// <copyright file="WrappedIQueryProvider.cs" company="Microsoft">
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
    /// Wraps the 'System.Linq.IQueryProvider' type.
    /// </summary>
    public partial class WrappedIQueryProvider : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        
        /// <summary>
        /// Initializes static members of the WrappedIQueryProvider class.
        /// </summary>
        static WrappedIQueryProvider()
        {
            WrappedObjectType = WrapperUtilities.GetTypeFromAssembly("System.Linq.IQueryProvider", "System.Core");
        }
        
        /// <summary>
        /// Initializes a new instance of the WrappedIQueryProvider class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedIQueryProvider(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
        
        /// <summary>
        /// Wraps the 'System.Linq.IQueryable CreateQuery(System.Linq.Expressions.Expression)' on the 'System.Linq.IQueryProvider' type.
        /// </summary>
        /// <param name="expression">The value of the 'expression' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIQueryable CreateQuery(WrappedObject expression)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIQueryable>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Linq.IQueryable CreateQuery(System.Linq.Expressions.Expression)"), new object[] { expression });
        }
        
        /// <summary>
        /// Wraps the 'System.Linq.IQueryable`1[TElement] CreateQuery[TElement](System.Linq.Expressions.Expression)' on the 'System.Linq.IQueryProvider' type.
        /// </summary>
        /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
        /// <param name="typeTElement">The CLR generic type for the 'TElement' parameter.</param>
        /// <param name="expression">The value of the 'expression' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIQueryable<TElement> CreateQuery<TElement>(Type typeTElement, WrappedObject expression)
          where TElement : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIQueryable<TElement>>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Linq.IQueryable`1[TElement] CreateQuery[TElement](System.Linq.Expressions.Expression)"), new object[] { expression }, new Type[] { typeTElement });
        }
        
        /// <summary>
        /// Wraps the 'System.Object Execute(System.Linq.Expressions.Expression)' on the 'System.Linq.IQueryProvider' type.
        /// </summary>
        /// <param name="expression">The value of the 'expression' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedObject Execute(WrappedObject expression)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Object Execute(System.Linq.Expressions.Expression)"), new object[] { expression });
        }
        
        /// <summary>
        /// Wraps the 'TResult Execute[TResult](System.Linq.Expressions.Expression)' on the 'System.Linq.IQueryProvider' type.
        /// </summary>
        /// <typeparam name="TResult">The wrapper type for the 'TResult' generic parameter.</typeparam>
        /// <param name="typeTResult">The CLR generic type for the 'TResult' parameter.</param>
        /// <param name="expression">The value of the 'expression' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual TResult Execute<TResult>(Type typeTResult, WrappedObject expression)
          where TResult : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<TResult>(this, WrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "TResult Execute[TResult](System.Linq.Expressions.Expression)"), new object[] { expression }, new Type[] { typeTResult });
        }
    }
}
