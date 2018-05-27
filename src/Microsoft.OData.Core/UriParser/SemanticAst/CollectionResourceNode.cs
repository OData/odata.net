//---------------------------------------------------------------------
// <copyright file="CollectionResourceNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Base class for all semantically bound nodes which represent a composable collection of values.
    /// </summary>
    public abstract class CollectionResourceNode : CollectionNode
    {
        /// <summary>
        /// Get the type of a single resource from the collection represented by this node.
        /// </summary>
        public abstract IEdmStructuredTypeReference ItemStructuredType { get; }

        /// <summary>
        /// Get the navigation source that contains this collection.
        /// </summary>
        public abstract IEdmNavigationSource NavigationSource { get; }
    }
}