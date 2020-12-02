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
    /// A dictionary for storing headers to be used with ODataBatchOperations.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class ODataBatchOperationHeaders : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>The backing dictionary for headers.</summary>
        private readonly Dictionary<string, string> headersDictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ODataBatchOperationHeaders()
        {
            this.headersDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
                if (this.TryGetValue(key, out var value))
                {
                    return value;
                }

                throw new KeyNotFoundException(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound(key));
            }

            set
            {
                this.headersDictionary[key] = value;
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the dictionary.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, string value)
        {
            this.headersDictionary.Add(key, value);
        }

        /// <summary>
        /// Determines whether the dictionary contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>true if the dictionary contains an element with the <paramref name="key"/>; otherwise, false.</returns>
        /// <remarks>This method will match the key using case-insensitive comparison.</remarks>

        public bool ContainsKeyOrdinal(string key)
        {
            return this.headersDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Removes the resource with the specified <paramref name="key"/> from the headers.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns>true if the item with the specified <paramref name="key"/> was removed; otherwise false.</returns>
        public bool Remove(string key)
        {
            return this.headersDictionary.Remove(key);
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
            return this.headersDictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.headersDictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.headersDictionary.GetEnumerator();
        }
    }
}
