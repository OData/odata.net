//---------------------------------------------------------------------
// <copyright file="EntitySetNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Node representing an entity set.
    /// </summary>
    internal sealed class EntitySetNode : CollectionResourceNode
    {
        /// <summary>
        /// The entity set this node represents.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The resource type of a single entity in the entity set.
        /// </summary>
        private readonly IEdmEntityTypeReference entityType;

        /// <summary>
        /// the type of the collection returned by this function
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// Creates an <see cref="EntitySetNode"/>
        /// </summary>
        /// <param name="entitySet">The entity set this node represents</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input entitySet is null.</exception>
        public EntitySetNode(IEdmEntitySet entitySet)
        {
            ExceptionUtils.CheckArgumentNotNull(entitySet, "entitySet");
            this.entitySet = entitySet;
            this.entityType = new EdmEntityTypeReference(this.NavigationSource.EntityType(), false);
            this.collectionTypeReference = EdmCoreModel.GetCollection(this.entityType);
        }

        /// <summary>
        /// Gets the resouce type of a single entity in the entity set.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get
            {
                return this.entityType;
            }
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType
        {
            get { return this.collectionTypeReference; }
        }

        /// <summary>
        /// Gets the resource type of a single entity in the entity set.
        /// </summary>
        public IEdmEntityTypeReference EntityItemType
        {
            get { return this.entityType; }
        }

        public override IEdmStructuredTypeReference ItemStructuredType
        {
            get { return this.entityType; }
        }

        /// <summary>
        /// Gets the entity set this node represents.
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind for this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.EntitySet;
            }
        }
    }
}