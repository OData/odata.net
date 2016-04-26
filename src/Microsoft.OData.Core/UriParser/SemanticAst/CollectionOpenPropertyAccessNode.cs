//---------------------------------------------------------------------
// <copyright file="CollectionOpenPropertyAccessNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Node representing an access to a open collection property value.
    /// </summary>
    public sealed class CollectionOpenPropertyAccessNode : CollectionNode
    {
        /// <summary>
        /// The value containing the property.
        /// </summary>
        private readonly SingleValueNode source;

        /// <summary>
        /// The name of the open collection property to be bound outside the EDM model.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Constructs a new <see cref="CollectionOpenPropertyAccessNode"/>.
        /// </summary>
        /// <param name="source">The value containing the property.</param>
        /// <param name="openPropertyName">The name of the open collection property to be bound outside the EDM model.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or openPropertyName is null.</exception>
        public CollectionOpenPropertyAccessNode(SingleValueNode source, string openPropertyName)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentNotNull(openPropertyName, "openPropertyName");

            this.source = source;
            this.name = openPropertyName;
        }

        /// <summary>
        /// Gets the value containing the property.
        /// </summary>
        public SingleValueNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the name of the open property to be bound outside the EDM model.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the resouce type of a single item from the collection represented by this node.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get { return null; }
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        /// /// <remarks>
        /// The value of this property will always be null for open collection properties.
        /// </remarks>
        public override IEdmCollectionTypeReference CollectionType
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.CollectionOpenPropertyAccess;
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