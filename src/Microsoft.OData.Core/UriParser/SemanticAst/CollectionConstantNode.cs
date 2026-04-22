//---------------------------------------------------------------------
// <copyright file="CollectionConstantNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
    using Microsoft.VisualBasic;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    #endregion Namespaces

    /// <summary>
    /// Node representing a collection of constant value, can either be primitive, enum, complex, entity, or collection of them.
    /// </summary>
    public sealed class CollectionConstantNode : CollectionNode
    {
        /// <summary>
        /// Collection of ConstantNodes, ResourceConstantNode (such as, {...} ), CollectionConstantNode (such as, [....]).
        /// </summary>
        private readonly IList<QueryNode> collection = new List<QueryNode>();

        /// <summary>
        /// Creates a new instance of the <see cref="CollectionConstantNode"/> class.
        /// </summary>
        /// <param name="collectionType">The expected collection type of this node. It could be null.</param>
        /// <param name="literalText">The literal text for this node's value, formatted according to the OData URI literal formatting rules. It could be empty.</param>
        public CollectionConstantNode(IEdmCollectionTypeReference collectionType, ReadOnlyMemory<char> literalText)
        {
            LiteralText = literalText;
            CollectionType = collectionType;
            ItemType = collectionType?.ElementType();
        }

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

            LiteralText = literalText.AsMemory();
            EdmCollectionType edmCollectionType = collectionType.Definition as EdmCollectionType;
            ItemType = edmCollectionType.ElementType;
            CollectionType = collectionType;

            foreach (object item in objectCollection)
            {
                this.collection.Add(new ConstantNode(item, item != null ? item.ToString() : "null", ItemType));
            }
        }

        /// <summary>
        /// Create a CollectionConstantNode
        /// </summary>
        /// <param name="objectCollection">A collection of objects.</param>
        /// <param name="literalText">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
        /// <param name="collectionType">The reference to the collection type.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input literalText is null.</exception>
        internal CollectionConstantNode(IEnumerable<object> objectCollection, ReadOnlyMemory<char> literalText, IEdmCollectionTypeReference collectionType)
        {
            ExceptionUtils.CheckArgumentNotNull(objectCollection, "objectCollection");
            ExceptionUtils.CheckArgumentNotNull(collectionType, "collectionType");

            LiteralText = literalText;
            EdmCollectionType edmCollectionType = collectionType.Definition as EdmCollectionType;
            ItemType = edmCollectionType.ElementType;
            CollectionType = collectionType;

            foreach (object item in objectCollection)
            {
                this.collection.Add(new ConstantNode(item, item != null ? item.ToString() : "null", ItemType));
            }
        }

        internal CollectionConstantNode(IEnumerable<QueryNode> nodes, string literalText, IEdmCollectionTypeReference collectionType)
        {
            ExceptionUtils.CheckArgumentNotNull(nodes, nameof(nodes));
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalText, nameof(literalText));
            ExceptionUtils.CheckArgumentNotNull(collectionType, "collectionType");

            this.LiteralText = literalText.AsMemory();
            EdmCollectionType edmCollectionType = collectionType.Definition as EdmCollectionType;
            ItemType = edmCollectionType.ElementType;
            CollectionType = collectionType;

            foreach (var node in nodes)
            {
                if (node == null)
                {
                    this.collection.Add(new ConstantNode(null, "null", ItemType));
                }
                else if (node is not ConstantNode || node is not ResourceConstantNode || node is not CollectionConstantNode)
                {
                    throw new ODataException("SRResources.MetadataBinder_CollectionItemsMustBeConstant");
                }
                else
                {
                    this.collection.Add(node);
                }
            }
        }

        internal CollectionConstantNode(IList<ConstantNode> constantNodeCollection, string literalText, IEdmCollectionTypeReference collectionType)
        {
            ExceptionUtils.CheckArgumentNotNull(constantNodeCollection, "objectCollection");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalText, "literalText");
            ExceptionUtils.CheckArgumentNotNull(collectionType, "collectionType");

            this.collection = constantNodeCollection.Select(a => a as QueryNode).ToList();
            this.LiteralText = literalText.AsMemory();
            EdmCollectionType edmCollectionType = collectionType.Definition as EdmCollectionType;
            ItemType = edmCollectionType.ElementType;
            CollectionType = collectionType;
        }

        private static QueryNode GetSourceIfConvertNode(QueryNode node)
        {
            if (node != null && node.Kind == QueryNodeKind.Convert)
            {
                return GetSourceIfConvertNode(((ConvertNode)node).Source);
            }

            return node;
        }

        /// <summary>
        /// Gets the collection of ConstantNodes.
        /// </summary>
        public IList<ConstantNode> Collection => new ReadOnlyCollection<ConstantNode>(this.collection.OfType<ConstantNode>().ToList());

        public ReadOnlyCollection<QueryNode> Items => this.collection.AsReadOnly();

        public int Count => this.collection.Count;

        /// <summary>
        /// Adds a single value constant item.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void Add(ConstantNode item) => AddItem(item);

        /// <summary>
        /// Adds a resource constant item.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void Add(ResourceConstantNode item) => AddItem(item);

        /// <summary>
        /// Adds a collection constant item.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void Add(CollectionConstantNode item) => AddItem(item);

        internal void AddItem(QueryNode item)
        {
            ExceptionUtils.CheckArgumentNotNull(item, nameof(item));

            if (item.Kind == QueryNodeKind.CollectionConstant ||
                item.Kind == QueryNodeKind.ResourceConstant)
            {
                this.collection.Add(item);
                return;
            }

            QueryNode source = item;
            while (source != null && source.Kind == QueryNodeKind.Convert)
            {
                source = ((ConvertNode)source).Source;
            }

            // Constant or Convert(Constant)
            if (source != null && source.Kind == QueryNodeKind.Constant)
            {
                this.collection.Add(item);
                return;
            }

            throw new ODataException("TODO: ");
        }

        /// <summary>
        /// Get or Set the literal text for this node's value, formatted according to the OData URI literal formatting rules. May be null if the text was not provided at construction time.
        /// </summary>
        public ReadOnlyMemory<char> LiteralText { get; }

        /// <summary>
        /// Gets the resource type of a single item from the collection represented by this node. This could be null.
        /// </summary>
        public override IEdmTypeReference ItemType { get; }

        /// <summary>
        /// The type of the collection type represented/expected by this node. This could be null.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType { get; }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionConstant;

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