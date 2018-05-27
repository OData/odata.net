//---------------------------------------------------------------------
// <copyright file="CollectionComplexNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Node represents a collection complex property.
    /// </summary>
    public class CollectionComplexNode : CollectionResourceNode
    {
        /// <summary>
        /// The resource node containing the property.
        /// </summary>
        private readonly SingleResourceNode source;

        /// <summary>
        /// The EDM property which is to be accessed.
        /// </summary>
        private readonly IEdmProperty property;

        /// <summary>
        /// The complex type of a single item from the collection represented by this node.
        /// </summary>
        private readonly IEdmComplexTypeReference itemType;

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// The navigation source that our collection comes from.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Constructs a new <see cref="CollectionComplexNode"/>.
        /// </summary>
        /// <param name="source">The value containing the property.</param>
        /// <param name="property">The EDM property which is to be accessed.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or property is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input property is not a collection of structural properties</exception>
        public CollectionComplexNode(SingleResourceNode source, IEdmProperty property)
            : this(ExceptionUtils.CheckArgumentNotNull(source, "source").NavigationSource, property)
        {
            this.source = source;
        }

        /// <summary>
        /// Constructs a new <see cref="CollectionComplexNode"/>.
        /// </summary>
        /// <param name="navigationSource">The navigation source containing the property.</param>
        /// <param name="property">The EDM property which is to be accessed.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or property is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input property is not a collection of structural properties</exception>
        private CollectionComplexNode(IEdmNavigationSource navigationSource, IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            if (property.PropertyKind != EdmPropertyKind.Structural)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_PropertyAccessShouldBeNonEntityProperty(property.Name));
            }

            this.property = property;
            this.collectionTypeReference = property.Type.AsCollection();
            this.itemType = this.collectionTypeReference.ElementType().AsComplex();
            this.navigationSource = navigationSource;
        }

        /// <summary>
        /// Gets the resource node containing the property.
        /// </summary>
        public SingleResourceNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the EDM property which is to be accessed.
        /// </summary>
        public IEdmProperty Property
        {
            get { return this.property; }
        }

        /// <summary>
        /// Gets the type of a single item from the collection represented by this node.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get { return this.itemType; }
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType
        {
            get { return this.collectionTypeReference; }
        }

        /// <summary>
        /// Gets the structured type of a single item from the collection represented by this node.
        /// </summary>
        public override IEdmStructuredTypeReference ItemStructuredType
        {
            get { return this.itemType; }
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
                return InternalQueryNodeKind.CollectionComplexNode;
            }
        }

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
