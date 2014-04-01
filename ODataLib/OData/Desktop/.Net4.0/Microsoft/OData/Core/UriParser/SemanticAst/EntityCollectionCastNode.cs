//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Node representing a type segment that casts an entity collection node.
    /// </summary>
    public sealed class EntityCollectionCastNode : EntityCollectionNode
    {
        /// <summary>
        /// The entity collection node that we're casting.
        /// </summary>
        private readonly EntityCollectionNode source;

        /// <summary>
        /// The target type that we're casting our entity collection node to.
        /// </summary>
        private readonly IEdmEntityTypeReference edmTypeReference;

        /// <summary>
        /// the type of the collection returned by this function
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// The navigation source that our collection comes from.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Create a CollectionCastNode with the given source node and the given target type.
        /// </summary>
        /// <param name="source">Parent <see cref="CollectionNode"/> that is being cast.</param>
        /// <param name="entityType">Type to cast to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or entityType are null.</exception>
        public EntityCollectionCastNode(EntityCollectionNode source, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.source = source;
            this.edmTypeReference = new EdmEntityTypeReference(entityType, false);
            this.navigationSource = source.NavigationSource;

            // creating a new collection type here because the type in the request is just the item type, there is no user-provided collection type.
            this.collectionTypeReference = EdmCoreModel.GetCollection(this.edmTypeReference);
        }

        /// <summary>
        /// Gets the entity collection node that we're casting.
        /// </summary>
        public EntityCollectionNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the type that we're casting all items in this collection to.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get { return this.edmTypeReference; }
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType
        {
            get { return this.collectionTypeReference; }
        }

        /// <summary>
        /// Gets the entity type that we're casting all items in this collection to.
        /// </summary>
        public override IEdmEntityTypeReference EntityItemType
        {
            get { return this.edmTypeReference; }
        }

        /// <summary>
        /// Gets the navigation source that our collection comes from.
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.EntityCollectionCast;
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
