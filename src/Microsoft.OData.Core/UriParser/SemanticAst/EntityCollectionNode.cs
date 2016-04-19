//---------------------------------------------------------------------
// <copyright file="EntityCollectionNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Base class for all semantically bound nodes which represent a composable collection of values.
    /// </summary>
    public abstract class EntityCollectionNode : CollectionNode
    {
        /// <summary>
        /// Get the resouce type of a single entity from the collection represented by this node.
        /// </summary>
        public abstract IEdmEntityTypeReference EntityItemType { get; }

        /// <summary>
        /// Get the navigation source that contains this collection.
        /// </summary>
        public abstract IEdmNavigationSource NavigationSource { get; }
    }
}