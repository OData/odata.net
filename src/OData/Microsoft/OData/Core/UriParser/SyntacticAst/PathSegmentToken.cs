//   OData .NET Libraries ver. 6.9
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

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Visitors;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a segment in a path.
    /// </summary>
    /// 
    internal abstract class PathSegmentToken : ODataAnnotatable
    {
        /// <summary>
        /// the next token in the path
        /// </summary>
        private PathSegmentToken nextToken;

        /// <summary>
        /// build this segment token using the next token
        /// </summary>
        /// <param name="nextToken">the next token in the path</param>
        protected PathSegmentToken(PathSegmentToken nextToken)
        {
            this.nextToken = nextToken;
        }

        /// <summary>
        /// Get the NextToken in the path
        /// </summary>
        public PathSegmentToken NextToken
        {
            get { return this.nextToken; }
        }

        /// <summary>
        /// The name of the property to access.
        /// </summary>
        public abstract string Identifier { get; }

        /// <summary>
        /// Is this a structural property
        /// </summary>
        public bool IsStructuralProperty { get; set; }

        /// <summary>
        /// Is this token namespace or container qualified.
        /// </summary>
        /// <returns>true if this token is namespace or container qualified.</returns>
        public abstract bool IsNamespaceOrContainerQualified();

        /// <summary>
        /// Accept a <see cref="IPathSegmentTokenVisitor{T}"/> to walk a tree of <see cref="PathSegmentToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public abstract T Accept<T>(IPathSegmentTokenVisitor<T> visitor);

        /// <summary>
        /// Accept a <see cref="IPathSegmentTokenVisitor"/> to walk a tree of <see cref="PathSegmentToken"/>s.
        /// </summary>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        public abstract void Accept(IPathSegmentTokenVisitor visitor);

        /// <summary>
        /// internal setter for the next token.
        /// </summary>
        /// <param name="nextTokenIn">the next token to set.</param>
        internal void SetNextToken(PathSegmentToken nextTokenIn)
        {
            this.nextToken = nextTokenIn;
        }
    }
}
