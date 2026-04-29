//---------------------------------------------------------------------
// <copyright file="CollectionRootPathNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// rootExprCol = begin-array 
    ///             [rootExpr *( value-separator rootExpr ) ]
    ///              end-array
    /// </summary>
    public sealed class CollectionRootPathNode : CollectionNode
    {
        /// <summary>
        /// Initializes a new <see cref="CollectionRootPathNode"/>.
        /// </summary>
        /// <param name="typeReference">The collection type reference.</param>
        public CollectionRootPathNode(IEdmCollectionTypeReference typeReference)
        {
            CollectionType = typeReference;
        }

        /// <summary>
        /// Gets the number of root path nodes
        /// </summary>
        public int Count => this.Collection.Count;

        /// <summary>
        /// Gets the collection of root path nodes.
        /// </summary>
        public IList<RootPathNode> Collection { get; } = new List<RootPathNode>();

        /// <summary>
        /// Get or Set the literal text for this node's value. It could be null.
        /// </summary>
        public ReadOnlyMemory<char> LiteralText { get; set; }

        /// <summary>
        /// Gets the item type. This is the type of the items in the collection. It could be null.
        /// </summary>
        public override IEdmTypeReference ItemType => CollectionType?.ElementType();

        /// <summary>
        /// Gets the collection type of this collection node. It could be null.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType { get; }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionRootExpr;

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> to walk a tree of <see cref="QueryNode"/>s.
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