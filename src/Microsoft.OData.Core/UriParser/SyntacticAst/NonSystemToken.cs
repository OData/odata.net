//---------------------------------------------------------------------
// <copyright file="NonSystemToken.cs" company="Microsoft">
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

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a segment in a path.
    /// </summary>
    ///
    public sealed class NonSystemToken : PathSegmentToken
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