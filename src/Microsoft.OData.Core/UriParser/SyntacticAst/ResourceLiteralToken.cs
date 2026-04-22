//---------------------------------------------------------------------
// <copyright file="ResourceLiteralToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OData.Edm;

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// JSON Object token, it's key value pairs (resource literal)
    /// Key should be a string literal token, so just use the string value directly.
    /// Value can be any query token, including another resource literal token.
    /// </summary>
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix",
       Justification = "ResourceLiteralToken is a syntax token type in the URI parser, not a general-purpose map. The 'Token' suffix is part of the established naming pattern for all syntax tree nodes.")]
    public sealed class ResourceLiteralToken : QueryToken, ICollection<KeyValuePair<string, QueryToken>>
    {
        /// <summary>
        /// The key value pairs in the added order.
        /// </summary>
        private IList<KeyValuePair<string, QueryToken>> _properties = new List<KeyValuePair<string, QueryToken>>();

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind => QueryTokenKind.ResourceLiteral;

        /// <summary>
        /// Gets or sets the '@odata.type' property value if exists, otherwise null.
        /// </summary>
        internal string TypeNameFromLiteral { get; set; }

        /// <summary>
        /// The expected edm type of this resource literal token.
        /// </summary>
        internal IEdmStructuredTypeReference ExpectedType { get; set; }

        internal ReadOnlyMemory<char> OriginalText { get; set; }

        /// <summary>
        /// Gets the number of key value pairs in the resource literal token.
        /// </summary>
        public int Count => _properties.Count;

        /// <summary>
        /// Gets a value indicating whether the resource literal token is read-only.
        /// </summary>
        public bool IsReadOnly => _properties.IsReadOnly;

        /// <summary>
        /// Adds a key value pair to the resource literal token.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, QueryToken value)
        {
            ExceptionUtils.CheckArgumentNotNull(key, "key");
            ExceptionUtils.CheckArgumentNotNull(value, "value");
            _properties.Add(new KeyValuePair<string, QueryToken>(key, value));
        }

        /// <summary>
        /// Adds a key value pair to the map token.
        /// </summary>
        /// <param name="item">The given key value pair item.</param>
        public void Add(KeyValuePair<string, QueryToken> item) => _properties.Add(item);

        /// <summary>
        /// Removes all elements from the collection.   
        /// </summary>
        /// <remarks>After calling this method, the collection will be empty. This operation does not
        /// throw an exception if the collection is already empty.</remarks>
        public void Clear() => _properties.Clear();

        /// <summary>
        /// Determines whether the collection contains an element with the specified key and value.
        /// </summary>
        /// <param name="item">The key/value pair to locate in the collection. The key is compared using the collection's key comparer, and
        /// the value is compared using the value's equality comparer.</param>
        /// <returns>true if the collection contains an element with the specified key and value; otherwise, false.</returns>
        public bool Contains(KeyValuePair<string, QueryToken> item) => _properties.Contains(item);

        /// <summary>
        /// Copies the elements of the collection to the specified array, starting at the specified array index.
        /// </summary>
        /// <remarks>The elements are copied in the same order as they are enumerated by the collection.
        /// The destination array must have sufficient space to accommodate the copied elements.</remarks>
        /// <param name="array">The one-dimensional array of type KeyValuePair&lt;string, QueryToken&gt; that is the destination of the
        /// elements copied from the collection. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in the destination array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, QueryToken>[] array, int arrayIndex) => _properties.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific key and value pair from the collection.
        /// </summary>
        /// <param name="item">The key and value pair to remove from the collection.</param>
        /// <returns>true if the key and value pair is successfully removed; otherwise, false. This method also returns false if
        /// the pair was not found in the collection.</returns>
        public bool Remove(KeyValuePair<string, QueryToken> item) => _properties.Remove(item);

        /// <summary>
        /// Returns an enumerator that iterates through the collection of property name and query token pairs.
        /// </summary>
        /// <returns>An enumerator for the collection of key value pair objects representing
        /// property names and their associated query tokens.</returns>
        public IEnumerator<KeyValuePair<string, QueryToken>> GetEnumerator() => _properties.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_properties).GetEnumerator();

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
    }
}