//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces
    using System;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Query node representing a collection navigation property.
    /// </summary>
    public sealed class CollectionNavigationNode : EntityCollectionNode
    {
        /// <summary>
        /// The navigation property of the single entity this node represents.
        /// </summary>
        private readonly IEdmNavigationProperty navigationProperty;

        /// <summary>
        /// The resouce type of a single entity item from the collection represented by this node.
        /// </summary>
        private readonly IEdmEntityTypeReference edmEntityTypeReference;

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// The parent node.
        /// </summary>
        private readonly SingleValueNode source;

        /// <summary>
        /// The EntitySet from which the collection of entities comes from.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// Creates a CollectionNavigationNode.
        /// </summary>
        /// <param name="navigationProperty">The navigation property that defines the collection node.</param>
        /// <param name="source">The parent of this collection navigation node.</param>
        /// <returns>The collection node.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or navigation property is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigation doesn't target a collection.</exception>
        public CollectionNavigationNode(IEdmNavigationProperty navigationProperty, SingleEntityNode source)
            : this(navigationProperty)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            this.source = source;
            this.entitySet = source.EntitySet != null ? source.EntitySet.FindNavigationTarget(navigationProperty) : null;
        }

        /// <summary>
        /// Creates a CollectionNavigationNode.
        /// </summary>
        /// <param name="navigationProperty">The navigation property that defines the collection node.</param>
        /// <param name="sourceSet">The source entity set.</param>
        /// <returns>The collection node.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigation property is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigation doesn't target a collection.</exception>
        public CollectionNavigationNode(IEdmNavigationProperty navigationProperty, IEdmEntitySet sourceSet)
            : this(navigationProperty)
        {
            this.entitySet = sourceSet != null ? sourceSet.FindNavigationTarget(navigationProperty) : null;
        }

        /// <summary>
        /// Creates a CollectionNavigationNode.
        /// </summary>
        /// <param name="navigationProperty">The navigation property that defines the collection node.</param>
        /// <returns>The collection node.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigation property is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigation doesn't target a collection.</exception>
        private CollectionNavigationNode(IEdmNavigationProperty navigationProperty)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationProperty, "navigationProperty");

            if (navigationProperty.TargetMultiplicityTemporary() != EdmMultiplicity.Many)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveManyMultiplicity);
            }

            this.navigationProperty = navigationProperty;
            this.collectionTypeReference = navigationProperty.Type.AsCollection();
            this.edmEntityTypeReference = this.collectionTypeReference.ElementType().AsEntityOrNull();
        }

        /// <summary>
        /// Gets the parent node of this Collection Navigation Node.
        /// </summary>
        public SingleValueNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the target multiplicity.
        /// </summary>
        public EdmMultiplicity TargetMultiplicity
        {
            get { return this.navigationProperty.TargetMultiplicityTemporary(); }
        }

        /// <summary>
        /// Gets the Navigation Property that defines this collection Node.
        /// </summary>
        /// <value> The navigation property that defines this collection node. </value>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets a reference to the resource type a single entity in the collection.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get { return this.edmEntityTypeReference; }
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType
        {
            get { return this.collectionTypeReference; }
        }

        /// <summary>
        /// Gets the resouce type of a single entity from the collection.
        /// </summary>
        public override IEdmEntityTypeReference EntityItemType
        {
            get { return this.edmEntityTypeReference; }
        }

        /// <summary>
        /// Gets the entity set containing this collection.
        /// </summary>
        public override IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.CollectionNavigationNode;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
