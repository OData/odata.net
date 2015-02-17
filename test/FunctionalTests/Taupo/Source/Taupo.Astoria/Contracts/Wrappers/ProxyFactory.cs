//---------------------------------------------------------------------
// <copyright file="ProxyFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this code has been generated using WrapperGenerator
// Do not modify the file, but instead modify and re-run the generator.

// disable warnings about 'new' keyword being required/not needed and for obsolete classes
#pragma warning disable 108, 109, 618

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    
    /// <summary>
    /// Factory class for creating proxy wrappers
    /// </summary>
    public static class ProxyFactory
    {
        private static Dictionary<Type, Type> proxyTypes = new Dictionary<Type, Type>()
        {
            { typeof(System.IO.Stream), typeof(StreamProxy) },
            { typeof(System.Linq.IQueryProvider), typeof(QueryProviderProxy) },
            { typeof(System.Linq.IOrderedQueryable<>), typeof(OrderedQueryableProxy<>) },
            { typeof(System.Linq.IOrderedQueryable), typeof(OrderedQueryableProxy) },
            { typeof(System.Linq.IQueryable<>), typeof(QueryableProxy<>) },
            { typeof(System.Linq.IQueryable), typeof(QueryableProxy) },
            { typeof(System.Collections.Generic.IEnumerable<>), typeof(EnumerableProxy<>) },
            { typeof(System.Collections.IEnumerable), typeof(EnumerableProxy) },
            { typeof(System.Collections.Generic.IEnumerator<>), typeof(EnumeratorProxy<>) },
            { typeof(System.Collections.IEnumerator), typeof(EnumeratorProxy) },
        };
        
        /// <summary>
        /// Returns a proxy that wraps the given instance
        /// </summary>
        /// <param name="scope">the wrapper scope to use</param>
        /// <param name="instance">the instance to wrap with an proxy</param>
        /// <param name="hintType">optional type hint in case the instance implements multiple interfaces</param>
        /// <returns>A proxy or the original instance if no proxy could be built</returns>
        public static object CreateProxyIfPossible(IWrapperScope scope, object instance, Type hintType)
        {
            ExceptionUtilities.CheckArgumentNotNull(scope, "scope");
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            
            var instanceType = instance.GetType();
            if (hintType == null)
            {
                hintType = instanceType;
            }
            else
            {
                ExceptionUtilities.Assert(hintType.IsAssignableFrom(instanceType), "Invalid hint type for given instance");
            }
            
            var genericArguments = instanceType.GetGenericArguments();
            foreach (var typePair in proxyTypes)
            {
                if (typePair.Key.IsGenericTypeDefinition())
                {
                    if (typePair.Key.GetGenericArguments().Length != genericArguments.Length)
                    {
                        continue;
                    }
                    
                    var genericType = typePair.Key.MakeGenericType(genericArguments);
                    if (hintType.IsAssignableFrom(genericType) && genericType.IsAssignableFrom(instanceType))
                    {
                        return Activator.CreateInstance(typePair.Value.MakeGenericType(genericArguments), scope, instance);
                    }
                }
                else if (hintType.IsAssignableFrom(typePair.Key) && typePair.Key.IsAssignableFrom(instanceType))
                {
                    return Activator.CreateInstance(typePair.Value, scope, instance);
                }
            }
            
            return instance;
        }
    }
}
