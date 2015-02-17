//---------------------------------------------------------------------
// <copyright file="OperationCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    /// <summary>
    /// Strongly typed cache for operations. Uses the operation name and binding parameter type as a cache-key.
    /// </summary>
    internal class OperationCache
    {
        /// <summary>
        /// The underlying cache which actually stores the wrappers based on a cache-key computed from the operation name and binding parameter type.
        /// </summary>
        private readonly Dictionary<string, OperationWrapper> underlyingCache = new Dictionary<string, OperationWrapper>(StringComparer.Ordinal);

        /// <summary>
        /// Tries to find a wrapper for an operation with the given name and binding parameter type.
        /// </summary>
        /// <param name="operationName">The operation name.</param>
        /// <param name="bindingType">The operation's binding parameter's type, or null.</param>
        /// <param name="wrapper">The wrapper, if found.</param>
        /// <returns>Whether or not a wrapper was found.</returns>
        internal bool TryGetWrapper(string operationName, ResourceType bindingType, out OperationWrapper wrapper)
        {
            var cacheKey = GetCacheKey(operationName, bindingType);
            return this.underlyingCache.TryGetValue(cacheKey, out wrapper);
        }

        /// <summary>
        /// Tries to find a wrapper for the given operation.
        /// </summary>
        /// <param name="operation">The operation to find a wrapper for.</param>
        /// <param name="wrapper">The wrapper, if found.</param>
        /// <returns>Whether or not a wrapper was found.</returns>
        internal bool TryGetWrapper(Operation operation, out OperationWrapper wrapper)
        {
            var cacheKey = GetCacheKey(operation);
            return this.underlyingCache.TryGetValue(cacheKey, out wrapper);
        }

        /// <summary>
        /// Adds the given operation wrapper to the cache.
        /// </summary>
        /// <param name="wrapper">The wrapper to add.</param>
        internal void Add(OperationWrapper wrapper)
        {
            var cacheKey = GetCacheKey(wrapper);
            this.underlyingCache.Add(cacheKey, wrapper);
        }

        /// <summary>
        /// Determines whether the given operation has already been cached.
        /// </summary>
        /// <param name="operation">The operation to look for.</param>
        /// <returns>Whether or not the operation has been cached.</returns>
        internal bool Contains(Operation operation)
        {
            return this.underlyingCache.ContainsKey(GetCacheKey(operation));
        }

        /// <summary>
        /// Determines whether the given operation has already been cached.
        /// </summary>
        /// <param name="operationWrapper">The operation wrapper to look for.</param>
        /// <returns>Whether or not the operation has been cached.</returns>
        internal bool Contains(OperationWrapper operationWrapper)
        {
            return this.underlyingCache.ContainsKey(GetCacheKey(operationWrapper));
        }

        /// <summary>
        /// Creates a cache-key from the operation name and binding parameter type.
        /// </summary>
        /// <param name="operationName">The operation name.</param>
        /// <param name="bindingType">The binding parameter type.</param>
        /// <returns>The cache-key.</returns>
        private static string GetCacheKey(string operationName, ResourceType bindingType)
        {
            var cacheKey = operationName;
            if (bindingType != null)
            {
                cacheKey += "_" + bindingType.FullName;
            }

            return cacheKey;
        }

        /// <summary>
        /// Creates a cache-key for the given operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>The cache-key.</returns>
        private static string GetCacheKey(Operation operation)
        {
            Debug.Assert(operation != null, "operation != null");
            operation.EnsureReadOnly();
            
            ResourceType bindingType = operation.OperationBindingParameter == null ? null : operation.OperationBindingParameter.ParameterType;
            return GetCacheKey(operation.Name, bindingType);
        }

        /// <summary>
        /// Creates a cache-key for the given operation wrapper.
        /// </summary>
        /// <param name="operation">The operation wrapper.</param>
        /// <returns>The cache-key.</returns>
        private static string GetCacheKey(OperationWrapper operation)
        {
            Debug.Assert(operation != null, "operation != null");
            ResourceType bindingType = operation.BindingParameter == null ? null : operation.BindingParameter.ParameterType;
            return GetCacheKey(operation.Name, bindingType);
        }
    }
}