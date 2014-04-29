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
