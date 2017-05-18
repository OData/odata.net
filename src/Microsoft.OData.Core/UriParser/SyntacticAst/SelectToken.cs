//---------------------------------------------------------------------
// <copyright file="SelectToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    #region Namespaces

    using System.Collections.Generic;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a select operation.
    /// </summary>
    public sealed class SelectToken : QueryToken
    {
        /// <summary>
        /// The properties according to which to select the results.
        /// </summary>
        private readonly IEnumerable<PathSegmentToken> properties;

        /// <summary>
        /// Create a SelectToken given the property-accesses of the select query.
        /// </summary>
        /// <param name="properties">The properties according to which to select the results.</param>
        public SelectToken(IEnumerable<PathSegmentToken> properties)
        {
            this.properties = properties != null ? new ReadOnlyEnumerableForUriParser<PathSegmentToken>(properties)
                                                 : new ReadOnlyEnumerableForUriParser<PathSegmentToken>(new List<PathSegmentToken>());
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Select; }
        }

        /// <summary>
        /// The properties according to which to select the results.
        /// </summary>
        public IEnumerable<PathSegmentToken> Properties
        {
            get { return this.properties; }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}