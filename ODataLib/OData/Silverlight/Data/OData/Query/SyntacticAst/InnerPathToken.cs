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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    #region Namespaces

    using System.Collections.Generic;
    using Microsoft.Data.OData.Query.SemanticAst;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a single nonroot segment in the query path.
    /// </summary>
    internal sealed class InnerPathToken : PathToken
    {
        /// <summary>
        /// The Identifier of the segment.
        /// </summary>
        private readonly string identifier;

        /// <summary>
        /// The named values in the key lookup for this segment.
        /// If the segment has no key lookup, then this property is null.
        /// If the segment has empty key lookup (), then this property is an empty collection.
        /// </summary>
        private readonly IEnumerable<NamedValue> namedValues;

        /// <summary>
        /// The NextToken segment.
        /// </summary>
        private QueryToken nextToken;

        /// <summary>
        /// Create a new StartPathToken given the Identifier and NextToken and namedValues if any
        /// </summary>
        /// <param name="identifier">The Identifier of the segment, the identifier.</param>
        /// <param name="nextToken">The NextToken segment, or null if this is the root segment.</param>
        /// <param name="namedValues">The named values in the key lookup for this segment.</param>
        public InnerPathToken(string identifier, QueryToken nextToken, IEnumerable<NamedValue> namedValues)
        {
            // We allow an "empty" Identifier segment so we can create one for the case of a service-document URL (which has no path)
            ExceptionUtils.CheckArgumentNotNull(identifier, "Identifier");

            this.identifier = identifier;
            this.nextToken = nextToken;
            this.namedValues = namedValues == null ? null : new ReadOnlyEnumerableForUriParser<NamedValue>(namedValues);
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.InnerPath; }
        }

        /// <summary>
        /// The Identifier of the segment, the identifier.
        /// </summary>
        public override string Identifier
        {
            get { return this.identifier; }
        }

        /// <summary>
        /// The NextToken segment, or null if this is the root segment.
        /// </summary>
        public override QueryToken NextToken
        {
            get { return this.nextToken; }
            set { this.nextToken = value; }
        }

        /// <summary>
        /// The named values in the key lookup for this segment.
        /// If the segment has no key lookup, then this property is null.
        /// If the segment has empty key lookup (), then this property is an empty collection.
        /// </summary>
        public IEnumerable<NamedValue> NamedValues
        {
            get { return this.namedValues; }
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
