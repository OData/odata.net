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
        /// Creates a new instance of the <see cref="CollectionConstantNode"/> class.
        /// </summary>
        /// <param name="collectionType">The expected collection type of this node. It could be null.</param>
        public CollectionConstantNode(IEdmCollectionTypeReference collectionType)
        {
            CollectionType = collectionType;
            ItemType = collectionType?.ElementType();
        }

        /// <summary>
        /// Gets the collection of ConstantNodes, ResourceConstantNode (such as, {...} ), CollectionConstantNode (such as, [....]).
        /// Keep it since the ASP.NET Core OData or other OData libraries may need it, even though it's not used in the ODataLib.
        /// This property will be removed in the future, and the collection of constant nodes will be stored in the "Value" property.
        /// </summary>
        [Obsolete("This property will be removed in the future, and the collection of constant nodes will be stored in the 'Items' property.")]
        public IList<ConstantNode> Collection => Items.OfType<ConstantNode>().ToList();

        /// <summary>
        /// The collection of query nodes.
        /// </summary>
        public IList<QueryNode> Items { get; } = new List<QueryNode>();

        /// <summary>
        /// Get or Set the literal text for this node's value, formatted according to the OData URI literal formatting rules. May be null if the text was not provided at construction time.
        /// </summary>
        public ReadOnlyMemory<char> LiteralText { get; set; }

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