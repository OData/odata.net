//---------------------------------------------------------------------
// <copyright file="SingleComplexNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Node representing a complex.
    /// </summary>
    public class SingleComplexNode : SingleResourceNode
    {
        /// <summary>
        /// The value containing this property.
        /// </summary>
        private readonly SingleResourceNode source;

        /// <summary>
        /// The EDM property which is to be accessed.
        /// </summary>
        private readonly IEdmProperty property;

        /// <summary>
        /// The target type that the property represents.
        /// </summary>
        private readonly IEdmComplexTypeReference complexTypeReference;

        /// <summary>
        /// The navigation source containing the source entity.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Constructs a <see cref="SingleComplexNode"/>.
        /// </summary>
        /// <param name="source">The value containing this property.</param>
        /// <param name="property">The EDM property which is to be accessed.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input source or property is null.</exception>
        /// <exception cref="ArgumentException">Throws if input property is not structural, or is a collection.</exception>
        public SingleComplexNode(SingleResourceNode source, IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            if (property.PropertyKind != EdmPropertyKind.Structural)
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_PropertyAccessShouldBeNonEntityProperty(property.Name));
            }

            if (property.Type.IsCollection())
            {
                throw new ArgumentException(ODataErrorStrings.Nodes_PropertyAccessTypeShouldNotBeCollection(property.Name));
            }

            this.source = source;
            this.property = property;
            this.navigationSource = source.NavigationSource;
            this.complexTypeReference = property.Type as IEdmComplexTypeReference;
        }

        /// <summary>
        /// Gets the value containing this property.
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
        /// The target type that the property represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.complexTypeReference; }
        }

        /// <summary>
        /// Gets the navigation source containing the complex.
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// The target type that the property represents.
        /// </summary>
        public override IEdmStructuredTypeReference StructuredTypeReference
        {
            get { return this.complexTypeReference; }
        }

        /// <summary>
        /// Gets the kind of this query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.SingleComplexNode;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
