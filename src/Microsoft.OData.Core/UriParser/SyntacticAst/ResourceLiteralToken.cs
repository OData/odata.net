//---------------------------------------------------------------------
// <copyright file="ResourceLiteralToken.cs" company="Microsoft">
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
    /// JSON Object token, it's a key value pairs (resource literal), for example: "{ \"Name\":\"abc\", \"Age\":18}"
    /// Key should be a string literal token, so just use the string value directly.
    /// Value can be any query token, including another resource literal token.
    /// </summary>
    public sealed class ResourceLiteralToken : QueryToken
    {
        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind => QueryTokenKind.ResourceLiteral;

        /// <summary>
        /// Gets the key value pairs (Properties) in the added order.
        /// </summary>
        public IList<KeyValuePair<string, QueryToken>> Properties { get; } = new List<KeyValuePair<string, QueryToken>>();

        /// <summary>
        /// Gets or sets the '@odata.type'/'@type' property value if exists, otherwise null.
        /// Be noted, this key/value pair is not stored in the Properties list, so it won't be enumerated when enumerating the resource literal token.
        /// Be noted, the string could contains the '#' prefix.
        /// </summary>
        public string TypeNameFromLiteral { get; set; }

        /// <summary>
        /// Gets or sets the original text of the resource literal token.
        /// </summary>
        public ReadOnlyMemory<char> OriginalText { get; set; }

        /// <summary>
        /// The expected edm type of this resource literal token from metadata.
        /// </summary>
        public IEdmStructuredTypeReference ExpectedType { get; set; }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
    }
}