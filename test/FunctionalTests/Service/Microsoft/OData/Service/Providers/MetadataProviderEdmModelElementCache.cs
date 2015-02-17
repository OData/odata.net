//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmModelElementCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces

    /// <summary>
    /// Class caching a set of items based on a string key.
    /// </summary>
    /// <typeparam name="T">The type of the items in the cache.</typeparam>
    internal sealed class MetadataProviderEdmModelElementCache<T> where T : class
    {
        /// <summary>
        /// The dictionary backing the cache.
        /// </summary>
        private Dictionary<string, T> cache;

        /// <summary>
        /// Enumerable to enumerate all the items in the cache.
        /// </summary>
        public IEnumerable<T> Items
        {
            get
            {
                if (this.cache == null)
                {
                    return Enumerable.Empty<T>();
                }

                return this.cache.Values;
            }
        }

        /// <summary>
        /// true if the cache is completely filled; otherwise false.
        /// </summary>
        internal bool IsCompletelyFilled
        {
            get;
            set;
        }

        /// <summary>
        /// Tries to find the item with the specified <paramref name="key"/> in the cache.
        /// </summary>
        /// <param name="key">The key used for the look up.</param>
        /// <param name="item">The item found with the specified <paramref name="key"/> or null if no such item was found.</param>
        /// <returns>true if an item with the specified <paramref name="key"/> was found in the cache; otherwise false.</returns>
        public bool TryGetCachedItem(string key, out T item)
        {
            Debug.Assert(key != null, "key != null");

            if (this.cache == null)
            {
                item = null;
                return false;
            }

            return this.cache.TryGetValue(key, out item);
        }

        /// <summary>
        /// Adds a new item to the cache.
        /// </summary>
        /// <param name="key">The key to use for storing the new item.</param>
        /// <param name="item">The item to store.</param>
        public void Add(string key, T item)
        {
            Debug.Assert(key != null, "key != null");
            Debug.Assert(item != null, "item != null");

            if (this.cache == null)
            {
                this.cache = new Dictionary<string, T>(StringComparer.Ordinal);
            }

            this.cache.Add(key, item);
        }

        /// <summary>
        /// Checks whether a given key is stored in the cache.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if an item with the specified <paramref name="key"/>exists in the cache; otherwise false.</returns>
        public bool ContainsKey(string key)
        {
            Debug.Assert(key != null, "key != null");

            if (this.cache == null)
            {
                return false;
            }

            return this.cache.ContainsKey(key);
        }
    }
}
