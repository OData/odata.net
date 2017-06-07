//---------------------------------------------------------------------
// <copyright file="PathSegmentToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// Lexical token representing a segment in a path.
    /// </summary>
    ///
    public abstract class PathSegmentToken
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