//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    #region Namespaces

    using System.Collections.Generic;
    using Microsoft.Data.OData.Query.SemanticAst;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing an expand operation.
    /// </summary>
    internal sealed class ExpandToken : QueryToken
    {
        /// <summary>
        /// The properties according to which to expand in the results.
        /// </summary>
        private readonly IEnumerable<ExpandTermToken> expandTerms;

        /// <summary>
        /// Create a ExpandToken given the property-accesses of the expand query.
        /// </summary>
        /// <param name="expandTerms">The properties according to which to expand the results.</param>
        public ExpandToken(IEnumerable<ExpandTermToken> expandTerms)
        {
            this.expandTerms = new ReadOnlyEnumerableForUriParser<ExpandTermToken>(expandTerms ?? new ExpandTermToken[0]);
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
