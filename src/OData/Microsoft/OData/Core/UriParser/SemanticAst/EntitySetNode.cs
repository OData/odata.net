//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Node representing an entity set.
    /// TODO: This should be deleted but it is used in many, many tests.
    /// </summary>
    internal sealed class EntitySetNode : EntityCollectionNode
    {
        /// <summary>
        /// The entity set this node represents.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The resouce type of a single entity in the entity set.
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
        /// Gets the resouce type of a single entity in the entity set.
        /// </summary>
        public override IEdmEntityTypeReference EntityItemType
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
