//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
#if WINDOWS_PHONE
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Simplistic implementation of HashSet for platforms where it is not available
    /// </summary>
    /// <typeparam name="T">The type of entries to store in the HashSet</typeparam>
    internal class HashSet<T> : IEnumerable<T>
    {
        /// <summary>
        /// We're using Dictionary for the real implementation.
        /// </summary>
        private Dictionary<T, bool> dictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="equalityComparer">The equality comparer to use.</param>
        internal HashSet(IEqualityComparer<T> equalityComparer)
        {
            this.dictionary = new Dictionary<T, bool>(equalityComparer);
        }
    
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="collection">The collection to initialize the hash set with.</param>
        /// <param name="equalityComparer">The equality comparer to use.</param>
        internal HashSet(IEnumerable<T> collection, IEqualityComparer<T> equalityComparer)
        {
            this.dictionary = new Dictionary<T, bool>(equalityComparer);
            foreach (T item in collection)
            {
                this.dictionary.Add(item, true);
            }
        }

        /// <summary>
        /// Gets an enumerator over the items in the hash set.
        /// </summary>
        /// <returns>An enumerator over the items in the hash set.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.dictionary.Keys.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator over the items in the hash set.
        /// </summary>
        /// <returns>An enumerator over the items in the hash set.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dictionary.Keys.GetEnumerator();
        }

        /// <summary>
        /// Determines if the hashset contains the specified item.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>true if the item was found, false otherwise.</returns>
        internal bool Contains(T item)
        {
            return this.dictionary.ContainsKey(item);
        }

        /// <summary>
        /// Adds an item to the hashset.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>false if the item already exists in the set or true otherwise.</returns>
        internal bool Add(T item)
        {
            if (this.Contains(item))
            {
                return false;
            }

            this.dictionary.Add(item, true);
            return true;
        }

        /// <summary>
        /// Removes an item from the hashset.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>false if the item does not exist in the set or true otherwise.</returns>
        internal bool Remove(T item)
        {
            return this.dictionary.Remove(item);
        }

        /// <summary>
        /// Clears the hashset.
        /// </summary>
        internal void Clear()
        {
            this.dictionary.Clear();
        }
    }
#endif
}
