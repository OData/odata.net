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

    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Node representing a type segment that casts an entity collection node.
    /// </summary>
    public sealed class EntityCollectionCastNode : EntityCollectionNode
    {
        /// <summary>
        /// The entity collection node that we're casting.
        /// </summary>
        private readonly EntityCollectionNode source;

        /// <summary>
        /// The target type that we're casting our entity collection node to.
        /// </summary>
        private readonly IEdmEntityTypeReference edmTypeReference;

        /// <summary>
        /// the type of the collection returned by this function
        /// </summary>
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        /// <summary>
        /// The EntitySet that our collection comes from.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// Create a CollectionCastNode with the given source node and the given target type.
        /// </summary>
        /// <param name="source">Parent <see cref="CollectionNode"/> that is being cast.</param>
        /// <param name="entityType">Type to cast to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source or entityType are null.</exception>
        public EntityCollectionCastNode(EntityCollectionNode source, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.source = source;
            this.edmTypeReference = new EdmEntityTypeReference(entityType, false);
            this.entitySet = source.EntitySet;

            // creating a new collection type here because the type in the request is just the item type, there is no user-provided collection type.
            this.collectionTypeReference = EdmCoreModel.GetCollection(this.edmTypeReference);
        }

        /// <summary>
        /// Gets the entity collection node that we're casting.
        /// </summary>
        public EntityCollectionNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the type that we're casting all items in this collection to.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get { return this.edmTypeReference; }
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public override IEdmCollectionTypeReference CollectionType
        {
            get { return this.collectionTypeReference; }
        }

        /// <summary>
        /// Gets the entity type that we're casting all items in this collection to.
        /// </summary>
        public override IEdmEntityTypeReference EntityItemType
        {
            get { return this.edmTypeReference; }
        }

        /// <summary>
        /// Gets the EntitySet that our collection comes from.
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
                return InternalQueryNodeKind.EntityCollectionCast;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walk a tree of <see cref="QueryNode"/>s.
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
