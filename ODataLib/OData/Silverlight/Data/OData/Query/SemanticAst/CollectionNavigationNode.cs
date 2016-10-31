//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
