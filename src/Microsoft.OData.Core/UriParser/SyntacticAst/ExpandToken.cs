//---------------------------------------------------------------------
// <copyright file="ExpandToken.cs" company="Microsoft">
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
    using System.Linq;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing an expand operation.
    /// </summary>
    public sealed class ExpandToken : QueryToken
    {
        /// <summary>
        /// The properties according to which to expand in the results.
        /// </summary>
        private readonly IEnumerable<ExpandTermToken> expandTerms;

        /// <summary>
        /// Creates a new instance of <see cref="ExpandToken"/> given the property-accesses of the expand query.
        /// </summary>
        /// <param name="expandTerms">The properties according to which to expand the results.</param>
        public ExpandToken(params ExpandTermToken[] expandTerms)
            : this((IEnumerable<ExpandTermToken>)expandTerms)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="ExpandToken"/> given the property-accesses of the expand query.
        /// </summary>
        /// <param name="expandTerms">The properties according to which to expand the results.</param>
        public ExpandToken(IEnumerable<ExpandTermToken> expandTerms)
        {
            this.expandTerms = new ReadOnlyEnumerableForUriParser<ExpandTermToken>(expandTerms ?? Enumerable.Empty<ExpandTermToken>());
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Expand; }
        }

        /// <summary>
        /// The properties according to which to expand in the results.
        /// </summary>
        public IEnumerable<ExpandTermToken> ExpandTerms
        {
            get { return this.expandTerms; }
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