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
        /// Collection of properties.
        /// </summary>
        private IList<RootPathNode> _collection = new List<RootPathNode>();

        public CollectionRootPathNode(IEdmCollectionTypeReference typeReference, ReadOnlyMemory<char> literal)
        {
            CollectionType = typeReference;
            LiteralText = literal;
        }

        public void Add(RootPathNode rootPathNode)
        {
            ExceptionUtils.CheckArgumentNotNull(rootPathNode, nameof(rootPathNode));
            this._collection.Add(rootPathNode);
        }

        public int Count => this._collection.Count;

        public IList<RootPathNode> Items => _collection;

        /// <summary>
        /// Get or Set the literal text for this node's value. It could be null.
        /// </summary>
        public ReadOnlyMemory<char> LiteralText { get; }

        public override IEdmTypeReference ItemType => CollectionType?.ElementType();

        public override IEdmCollectionTypeReference CollectionType { get; }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionRootExpr;
    }
}