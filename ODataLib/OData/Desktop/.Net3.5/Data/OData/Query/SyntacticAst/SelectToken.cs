//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    #region Namespaces

    using System.Collections.Generic;
    using Microsoft.Data.OData.Query.SemanticAst;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a select operation.
    /// </summary>
    internal sealed class SelectToken : QueryToken
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
