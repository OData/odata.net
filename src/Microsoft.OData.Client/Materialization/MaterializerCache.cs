//---------------------------------------------------------------------
// <copyright file="MaterializerCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.Client.Materialization
{
    /// <summary>
    /// A cache used to store temporary materialization metadata
    /// for deserialized <see cref="ODataItem"/>s during materialization of response payloads
    /// into client CLR objects. Keys are identified using reference equality.
    /// </summary>
    /// <remarks>
    /// Instances of the cache are not thread-safe. Each instance is expected to be used within the
    /// scope of a single request.
    /// The cache is designed to store one entry type per key. For example, if the key is
    /// an <see cref="ODataResource"/>, its value should be a <see cref="MaterializerEntry"/>. It also
    /// expects the value for any given key will not change.
    /// For this reason, you should call `cache.SetAnnotation(odataResource, materializerEntry)` to add a cache entry
    /// and `cache.GetAnnotations<MaterializerEntry>(odataResource)` to retrieve the entry.
    /// If the requirements change such that we need to allow updating the value of a key to a different type,
    /// then the cache should be redesigned.
    /// </remarks>
    internal sealed class MaterializerCache
    {
        private readonly Dictionary<ODataAnnotatable, object> cache = new Dictionary<ODataAnnotatable, object>(ReferenceEqualityComparer<ODataAnnotatable>.Instance);

        /// <summary>
        /// Adds an entry to the cache with the <paramref name="annotatable"/>
        /// set as key.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="annotatable">The key of the entry.</param>
        /// <param name="value">The value to associate with the key.</param>
        public void SetAnnotation<T>(ODataAnnotatable annotatable, T value) where T : class
        {
            Debug.Assert(annotatable != null, "annotatable != null");
            this.cache.Add(annotatable, value);
        }

        /// <summary>
        /// Retrieves the value associated with the specified <paramref name="annotatable"/>.
        /// Returns null if the cache does not contain an entry with the specified key.
        /// </summary>
        /// <typeparam name="T">The expected type of the value.</typeparam>
        /// <param name="annotatable">The key of the entry.</param>
        /// <returns>The value associated with the specified <paramref name="annotatable"/> if the entry exists; otherwise null.</returns>
        public T GetAnnotation<T>(ODataAnnotatable annotatable) where T: class
        {
            if (this.cache.TryGetValue(annotatable, out object value))
            {
                T valueAsT = value as T;

                if (valueAsT != null)
                {
                    return valueAsT;
                }
            }

            return default(T);
        }
    }
}
