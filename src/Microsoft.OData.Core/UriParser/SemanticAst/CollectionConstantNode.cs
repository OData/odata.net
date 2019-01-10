//---------------------------------------------------------------------
// <copyright file="CollectionConstantNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Node representing a constant value, can either be primitive, complex, entity, or collection value.
    /// </summary>
    public sealed class CollectionConstantNode : CollectionNode
    {
        /// <summary>
        /// Collection of ConstantNodes.
        /// </summary>
        private readonly IList<ConstantNode> collection = new List<ConstantNode>();

        /// <summary>
        /// Cache for the TypeReference after it has been calculated for the current state of the node.
        /// </summary>
        private readonly IEdmTypeReference itemType;

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// Create a CollectionConstantNode
        /// </summary>
        /// <param name="objectCollection">A collection of objects.</param>
        /// <param name="literalText">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
        /// <param name="collectionType">The reference to the collection type.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input literalText is null.</exception>
        public CollectionConstantNode(IEnumerable<object> objectCollection, string literalText, IEdmCollectionTypeReference collectionType)
        {
            ExceptionUtils.CheckArgumentNotNull(objectCollection, "objectCollection");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalText, "literalText");
            ExceptionUtils.CheckArgumentNotNull(collectionType, "collectionType");

            this.LiteralText = literalText;
            EdmCollectionType edmCollectionType = collectionType.Definition as EdmCollectionType;
            this.itemType = edmCollectionType.ElementType;
            this.collectionTypeReference = collectionType;

            foreach (object item in objectCollection)
            {
                this.collection.Add(new ConstantNode(item, item != null ? item.ToString() : "null", this.itemType));
            }
        }

        /// <summary>
        /// Gets the collection of ConstantNodes.
        /// </summary>
        public IList<ConstantNode> Collection
        {
            get
            {
                return new ReadOnlyCollection<ConstantNode>(this.collection);
            }
        }

        /// <summary>
        /// Get or Set the literal text for this node's value, formatted according to the OData URI literal formatting rules. May be null if the text was not provided at construction time.
        /// </summary>
        public string LiteralText { get; private set; }

        /// <summary>
        /// Gets the resource type of a single item from the collection represented by this node.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get
            {
                return this.itemType;
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
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.CollectionConstant;
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