//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Annotation to capture all of the custom instance annotations on an ODataAnnotatable.
    /// </summary>
    /// <remarks>
    /// Currently only <see cref="ODataError"/>, <see cref="ODataEntry"/>, and <see cref="ODataFeed"/> supports instance annotations.
    /// Additionally, instance annotations will only be serialized in Json.
    /// </remarks>
    [Obsolete("The InstanceAnnotationCollection class is deprecated, use the InstanceAnnotations property on objects that support instance annotations instead.")]
    public sealed class InstanceAnnotationCollection : IEnumerable<KeyValuePair<string, ODataValue>>
    {
        /// <summary>
        /// Backing dictionary of instance annotation term name/object pairs.
        /// </summary>
        private readonly Dictionary<string, ODataValue> inner;

        /// <summary>
        /// Creates a new <see cref="InstanceAnnotationCollection"/> to hold instance annotations for an <see cref="ODataAnnotatable"/>.
        /// </summary>
        public InstanceAnnotationCollection()
        {
            this.inner = new Dictionary<string, ODataValue>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="InstanceAnnotationCollection"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="InstanceAnnotationCollection"/>.
        /// </returns>
        public int Count
        {
            get { return this.inner.Count; }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public ODataValue this[string key]
        {
            get { return this.inner[key]; }
            set { this.inner[key] = value; }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the ICollection> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(string key)
        {
            return this.inner.ContainsKey(key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<string, ODataValue>> GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        /// <summary>
        /// Removes all items from the <see cref="InstanceAnnotationCollection"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="InstanceAnnotationCollection"/> is read-only. </exception>
        public void Clear()
        {
            this.inner.Clear();
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param><param name="value">The object to use as the value of the element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(string key, ODataValue value)
        {
            ODataInstanceAnnotation.ValidateName(key);
            ODataInstanceAnnotation.ValidateValue(value);
            this.inner.Add(key, value);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(string key)
        {
            return this.inner.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.ICollection`1"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool TryGetValue(string key, out ODataValue value)
        {
            return this.inner.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets an enumerator for this object.
        /// </summary>
        /// <returns>An enumerator for this object.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
