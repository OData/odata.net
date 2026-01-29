//---------------------------------------------------------------------
// <copyright file="ParameterAliasCollectionNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Represents a parameter alias that refers to a collection value.
    /// </summary>
    public class ParameterAliasCollectionNode : CollectionNode
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="alias">The parameter alias.</param>
        /// <param name="collectionType">The alias' type which is inferred from the type of alias value's CollectionNode.</param>
        public ParameterAliasCollectionNode(string alias, IEdmCollectionTypeReference collectionType)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionType, "collectionType");

            Alias = alias;
            CollectionType = collectionType;
        }

        /// <summary>
        /// The parameter alias.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Gets the type of a single item from the collection represented by this node.
        /// </summary>
        public override IEdmTypeReference ItemType => CollectionType.ElementType();

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType { get; }

        /// <summary>
        /// Is InternalQueryNodeKind.ParameterAliasCollection.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.ParameterAliasCollection;

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
