//---------------------------------------------------------------------
// <copyright file="SingleValuePropertyAccessNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Node representing an access to a property value.
    /// </summary>
    public sealed class SingleValuePropertyAccessNode : SingleValueNode
    {
        /// <summary>
        /// The value containing this property.
        /// </summary>
        private readonly SingleValueNode source;

        /// <summary>
        /// The EDM property which is to be accessed.
        /// </summary>
        /// <remarks>Only non-entity, non-collection properties are supported by this node.</remarks>
        private readonly IEdmProperty property;

        /// <summary>
        /// Constructs a <see cref="SingleValuePropertyAccessNode"/>.
        /// </summary>
        /// <param name="source">The value containing this property.</param>
        /// <param name="property">The EDM property which is to be accessed.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input source or property is null.</exception>
        /// <exception cref="ArgumentException">Throws if input property is not structural, or is a collection.</exception>
        public SingleValuePropertyAccessNode(SingleValueNode source, IEdmProperty property)
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
        }

        /// <summary>
        /// Gets the value containing this property.
        /// </summary>
        public SingleValueNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the EDM property which is to be accessed.
        /// </summary>
        /// <remarks>Only non-entity, non-collection properties are supported by this node.</remarks>
        public IEdmProperty Property
        {
            get { return this.property; }
        }

        /// <summary>
        /// Gets the type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.Property.Type; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.SingleValuePropertyAccess;
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