//---------------------------------------------------------------------
// <copyright file="CollectionNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Base class for all semantic metadata bound nodes which represent a composable collection of values.
    /// </summary>
    public abstract class CollectionNode : QueryNode
    {
        /// <summary>
        /// The resouce type of a single item from the collection represented by this node.
        /// </summary>
        public abstract IEdmTypeReference ItemType
        {
            get;
        }

        /// <summary>
        /// The type of the collection represented by this node.
        /// </summary>
        public abstract IEdmCollectionTypeReference CollectionType
        {
            get;
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        public override QueryNodeKind Kind
        {
            get { return (QueryNodeKind)this.InternalKind; }
        }
    }
}