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

    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing an all-properties access.
    /// </summary>
    internal sealed class StarToken : PathToken
    {
        /// <summary>
        /// The NextToken token to access the property on.
        /// If this is null, then the property access has no NextToken. That usually means to access the property
        /// on the implicit parameter for the expression, the result on which the expression is being applied.
        /// </summary>
        private QueryToken nextToken;

        /// <summary>
        /// Create a new StarToken given the NextToken (if any).
        /// </summary>
        /// <param name="nextToken">The NextToken token to access the property on. Pass no if this property has no NextToken.</param>
        public StarToken(QueryToken nextToken)
        {
            this.nextToken = nextToken;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Star; }
        }

        /// <summary>
        /// The NextToken token to access the property on.
        /// If this is null, then the property access has no NextToken. That usually means to access the property
        /// on the implicit parameter for the expression, the result on which the expression is being applied.
        /// </summary>
        public override QueryToken NextToken
        {
            get { return this.nextToken; }
            set { this.nextToken = value; }
        }

        /// <summary>
        /// the name of this token(inherited from PathToken), which in this case is always "*"
        /// </summary>
        public override string Identifier
        {
            get { return "*"; }
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
