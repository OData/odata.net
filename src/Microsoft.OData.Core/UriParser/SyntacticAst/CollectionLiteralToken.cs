//---------------------------------------------------------------------
// <copyright file="CollectionLiteralToken.cs" company="Microsoft">
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
    /// Lexical token representing a collection literal, that is a JSON array or parenthesized list of items for 'in' operator.
    /// </summary>
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix",
        Justification = "CollectionLiteralToken is a syntax token type in the URI parser, not a general-purpose collection. The 'Token' suffix is part of the established naming pattern for all syntax tree nodes.")]
    public sealed class CollectionLiteralToken : QueryToken, ICollection<QueryToken>
    {
        /// <summary>
        /// The items in the collection.
        /// </summary>
        private IList<QueryToken> _items = new List<QueryToken>();

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind => QueryTokenKind.CollectionLiteral;

        /// <summary>
        /// Gets or sets the collection type reference associated with the collection litoken if applicable.
        /// </summary>
        public IEdmCollectionTypeReference CollectionType { get; set; }

        /// <summary>
        /// Gets/sets the original text of the collection token, which is used for error reporting and other purposes.
        /// </summary>
        public ReadOnlyMemory<char> OriginalText { get; set; }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => _items.IsReadOnly;

        /// <summary>
        /// Adds the specified query token to the collection.
        /// </summary>
        /// <param name="item">The query token to add to the collection. Cannot be null.</param>
        public void Add(QueryToken item)
        {
            ExceptionUtils.CheckArgumentNotNull(item, "item");
            _items.Add(item);
        }

        /// <summary>
        /// Determines whether the collection contains the specified query token.
        /// </summary>
        /// <param name="item">The query token to locate in the collection.</param>
        /// <returns>true if the specified query token is found in the collection; otherwise, false.</returns>
        public bool Contains(QueryToken item) => _items.Contains(item);

        /// <summary>
        /// Copies the elements of the collection to the specified array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array of QueryToken objects that is the destination of the elements copied from the
        /// collection. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in the destination array at which copying begins.</param>
        public void CopyTo(QueryToken[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurrence of the specified query token from the collection.
        /// </summary>
        /// <param name="item">The query token to remove from the collection.</param>
        /// <returns>true if the query token was successfully removed; otherwise, false. This method also returns false if the
        /// token was not found in the collection.</returns>
        public bool Remove(QueryToken item) => _items.Remove(item);

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear() => _items.Clear();

        /// <summary>
        /// Returns an enumerator that iterates through the collection of query tokens.
        /// </summary>
        /// <returns>An enumerator for the collection of <see cref="QueryToken"/> objects.</returns>
        public IEnumerator<QueryToken> GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_items).GetEnumerator();

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
    }
}