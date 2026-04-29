//---------------------------------------------------------------------
// <copyright file="CollectionLiteralToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// JSON array token, Lexical token representing a collection literal, that is a JSON array or parenthesized list of items for 'in' operator.
    /// For example: "[1,2,3]", "['a','b','c']" or "(1,2,3)" for 'in' operator.
    /// </summary>
    public sealed class CollectionLiteralToken : QueryToken
    {
        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind => QueryTokenKind.CollectionLiteral;

        /// <summary>
        /// The items in the collection.
        /// </summary>
        public IList<QueryToken> Items { get; } = new List<QueryToken>();

        /// <summary>
        /// Gets or sets the collection type reference expected from metadata for the collection literal token if applicable.
        /// </summary>
        public IEdmCollectionTypeReference ExpectedCollectionType { get; set; }

        /// <summary>
        /// Gets/sets the original text of the collection literal token, which is used for error reporting and other purposes.
        /// </summary>
        public ReadOnlyMemory<char> OriginalText { get; set; }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
    }
}