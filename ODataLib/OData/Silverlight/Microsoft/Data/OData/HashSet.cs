//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
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
