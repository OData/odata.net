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
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Visitors;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a segment in a path.
    /// </summary>
    /// 
    internal sealed class NonSystemToken : PathSegmentToken
    {
        /// <summary>
        /// Any named values for this NonSystemToken
        /// </summary>
        private readonly IEnumerable<NamedValue> namedValues;

        /// <summary>
        /// The identifier for this token.
        /// </summary>
        private readonly string identifier;

        /// <summary>
        /// Build a NonSystemToken
        /// </summary>
        /// <param name="identifier">the identifier of this token</param>
        /// <param name="namedValues">a list of named values for this token</param>
        /// <param name="nextToken">the next token in the path</param>
        public NonSystemToken(string identifier, IEnumerable<NamedValue> namedValues, PathSegmentToken nextToken)
            : base(nextToken)
        {
            ExceptionUtils.CheckArgumentNotNull(identifier, "identifier");

            this.identifier = identifier;
            this.namedValues = namedValues;
        }

        /// <summary>
        /// Get the list of named values for this token.
        /// </summary>
        public IEnumerable<NamedValue> NamedValues
        {
            get { return this.namedValues; }
        }

        /// <summary>
        /// Get the identifier for this token.
        /// </summary>
        public override string Identifier
        {
            get { return this.identifier; }
        }

        /// <summary>
        /// Is this token namespace or container qualified.
        /// </summary>
        /// <returns>true if this token is namespace or container qualified.</returns>
        public override bool IsNamespaceOrContainerQualified()
        {
            return this.identifier.Contains(".");
        }

        /// <summary>
        /// Accept a <see cref="IPathSegmentTokenVisitor{T}"/> to walk a tree of <see cref="PathSegmentToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(IPathSegmentTokenVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accept a <see cref="IPathSegmentTokenVisitor"/> to walk a tree of <see cref="PathSegmentToken"/>s.
        /// </summary>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        public override void Accept(IPathSegmentTokenVisitor visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            visitor.Visit(this);
        }
    }
}
