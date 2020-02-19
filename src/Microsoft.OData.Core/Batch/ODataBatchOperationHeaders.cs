//---------------------------------------------------------------------
// <copyright file="ODataBatchOperationHeaders.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// A dictionary implementation with special key-matching semantics; it accepts case-insensitive matches
    /// but prefers a case-sensitive one (if present).
    /// </summary>
    /// <remarks>As an implementation choice we did not use a second dictionary to maintain a cache of case-insensitive
    /// keys since we don't want to pay the price of an extra dictionary for cases where the looked up keys
    /// match case sensitively (as per spec, should be the default case).</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class ODataBatchOperationHeaders : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>The backing dictionary using case-sensitive key comparison.</summary>
        private readonly Dictionary<string, string> caseSensitiveDictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ODataBatchOperationHeaders()
        {
            this.caseSensitiveDictionary = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public string this[string key]
        {
            get
            {
                string value;
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }

                throw new KeyNotFoundException(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound(key));
            }

            set
            {
                this.caseSensitiveDictionary[key] = value;
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the dictionary.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, string value)
        {
            this.caseSensitiveDictionary.Add(key, value);
        }

        /// <summary>
        /// Determines whether the dictionary contains an element with the specified key using case-sensitive comparison.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>true if the dictionary contains an element with the <paramref name="key"/>; otherwise, false.</returns>
        /// <remarks>This method will only try to match the key using case-sensitive comparison.</remarks>
        public bool ContainsKeyOrdinal(string key)
        {
            return this.caseSensitiveDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Removes the resource with the specified <paramref name="key"/> from the headers.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns>true if the item with the specified <paramref name="key"/> was removed; otherwise false.</returns>
        public bool Remove(string key)
        {
            if (this.caseSensitiveDictionary.Remove(key))
            {
                return true;
            }

            key = this.FindKeyIgnoreCase(key);
            if (key == null)
            {
                return false;
            }

            return this.caseSensitiveDictionary.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out string value)
        {
            if (this.caseSensitiveDictionary.TryGetValue(key, out value))
            {
                return true;
            }

            key = this.FindKeyIgnoreCase(key);
            if (key == null)
            {
                value = null;
                return false;
            }

            return this.caseSensitiveDictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.caseSensitiveDictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.caseSensitiveDictionary.GetEnumerator();
        }

        /// <summary>
        /// Finds <paramref name="key"/> in the case sensitive dictionary ignoring the case for comparison.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <returns>The key from the case sensitive dictionary that matched the <paramref name="key"/> or null if no match was found.</returns>
        /// <remarks>This method throws if multiple case insensitive matches for the specified <paramref name="key"/> exist.</remarks>
        private string FindKeyIgnoreCase(string key)
        {
            string match = null;
            foreach (string caseSensitiveKey in this.caseSensitiveDictionary.Keys)
            {
                if (string.Compare(caseSensitiveKey, key, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (match != null)
                    {
                        throw new ODataException(Strings.ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys(key));
                    }

                    match = caseSensitiveKey;
                }
            }

            return match;
        }
    }
}
