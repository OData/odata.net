//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    /// <summary>
    /// Lexical token representing a type segment.
    /// </summary>
    internal sealed class DottedIdentifierToken : PathToken
    {
        /// <summary>
        /// The Identifier of the type segment.
        /// </summary>
        private readonly string identifier;

        /// <summary>
        /// The parent segment.
        /// </summary>
        private QueryToken nextToken;

        /// <summary>
        /// Create a TypeSegmentQueryToken given the Identifier and the parent (if any)
        /// </summary>
        /// <param name="identifier">The Identifier of the type segment, including the namespace.</param>
        /// <param name="nextToken">The parent segment.</param>
        public DottedIdentifierToken(string identifier, QueryToken nextToken)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(identifier, "Identifier");

            this.identifier = identifier;
            this.nextToken = nextToken;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.DottedIdentifier; }
        }

        /// <summary>
        /// The full name of the type.
        /// </summary>
        public override string Identifier
        {
            get { return this.identifier; }
        }

        /// <summary>
        /// The parent.
        /// </summary>
        public override QueryToken NextToken
        {
            get { return this.nextToken; }
            set { this.nextToken = value; }
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
