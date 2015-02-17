//---------------------------------------------------------------------
// <copyright file="TopNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !PORTABLELIB
namespace Microsoft.OData.Core.Query.SemanticAst
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Query node representing a top operator.
    /// </summary>
    internal sealed class TopNode : CollectionNode
    {   
        /// <summary>
        /// The number of entities to include in the result.
        /// </summary>
        private readonly QueryNode amount;

        /// <summary>
        /// The collection to take items from.
        /// </summary>
        private readonly CollectionNode collection;

        /// <summary>
        /// Creates a <see cref="TopNode"/>.
        /// </summary>
        /// <param name="amount">The number of entities to include in the result.</param>
        /// <param name="collection">The collection to take items from.</param>
        public TopNode(QueryNode amount, CollectionNode collection)
        {
            ExceptionUtils.CheckArgumentNotNull(amount, "amount");
            ExceptionUtils.CheckArgumentNotNull(collection, "collection");

            this.amount = amount;
            this.collection = collection;
        }

        /// <summary>
        /// The number of entities to include in the result.
        /// </summary>
        public QueryNode Amount
        {
            get { return this.amount; }
        }

        /// <summary>
        /// The collection to take items from.
        /// </summary>
        public CollectionNode Collection
        {
            get { return this.collection; }
        }

        /// <summary>
        /// The resouce type of a single item from the collection represented by this node.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get
            {
                return this.Collection.ItemType;
            }
        }

        /// <summary>
        /// The kind of the query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.Top;
            }
        }
    }
}
#endif