//---------------------------------------------------------------------
// <copyright file="SearchTermNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Node representing a search term.
    /// </summary>
    public sealed class SearchTermNode : SingleValueNode
    {
        /// <summary>
        /// Bool reference, as search term is used for matching
        /// </summary>
        private static readonly IEdmTypeReference BoolTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(typeof(bool));

        /// <summary>
        /// The search term value.
        /// </summary>
        private readonly string text;

        /// <summary>
        /// Create a SearchTermNode
        /// </summary>
        /// <param name="text">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input literalText is null.</exception>
        public SearchTermNode(string text)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(text, "literalText");
            this.text = text;
        }

        /// <summary>
        /// Gets the search term value.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        /// <summary>
        /// Gets the resouce type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                return BoolTypeReference;
            }
        }

        /// <summary>
        /// Gets the kind of the query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.SearchTerm;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walk a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}