//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces
    using System;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        /// The navigation source from which the collection of entities comes from.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

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

            this.navigationSource = source.NavigationSource != null ? source.NavigationSource.FindNavigationTarget(navigationProperty) : null;
        }

        /// <summary>
        /// Creates a CollectionNavigationNode.
        /// </summary>
        /// <param name="navigationProperty">The navigation property that defines the collection node.</param>
        /// <param name="source">The navigation source.</param>
        /// <returns>The collection node.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigation property is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input navigation doesn't target a collection.</exception>
        public CollectionNavigationNode(IEdmNavigationProperty navigationProperty, IEdmNavigationSource source)
            : this(navigationProperty)
        {
            this.navigationSource = source != null ? source.FindNavigationTarget(navigationProperty) : null;
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
        /// Gets the navigation source containing this collection.
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
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
