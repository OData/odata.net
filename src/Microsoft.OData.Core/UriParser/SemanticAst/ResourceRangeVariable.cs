//---------------------------------------------------------------------
// <copyright file="ResourceRangeVariable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// A RangeVariable inside an any or all expression that refers to an entity or a complex.
    /// </summary>
    public sealed class ResourceRangeVariable : RangeVariable
    {
        /// <summary>
        ///  The name of the associated any/all parameter (null if none)
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The resource collection that this rangeVariable node iterates over
        /// </summary>
        private readonly CollectionResourceNode entityCollectionNode;

        /// <summary>
        /// The navigation source of the collection this node iterates over.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// The entity type of each item in the collection that this range variable iterates over.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// Creates a <see cref="ResourceRangeVariable"/>.
        /// </summary>
        /// <param name="name"> The name of the associated any/all parameter (null if none)</param>
        /// <param name="entityType">The entity type of each item in the collection that this range variable iterates over.</param>
        /// <param name="entityCollectionNode">The Entity collection that this rangeVariable node iterates over</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name or entityType is null.</exception>
        public ResourceRangeVariable(string name, IEdmEntityTypeReference entityType, CollectionResourceNode entityCollectionNode)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.name = name;
            this.entityTypeReference = entityType;
            this.entityCollectionNode = entityCollectionNode;
            this.navigationSource = entityCollectionNode != null ? entityCollectionNode.NavigationSource : null;
        }

        /// <summary>
        /// Creates a <see cref="ResourceRangeVariable"/>.
        /// </summary>
        /// <param name="name"> The name of the associated any/all parameter (null if none)</param>
        /// <param name="entityType">The entity type of each item in the collection that this range variable iterates over.</param>
        /// <param name="navigationSource">The navigation source of the collection this node iterates over.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name or entityType is null.</exception>
        public ResourceRangeVariable(string name, IEdmEntityTypeReference entityType, IEdmNavigationSource navigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.name = name;
            this.entityTypeReference = entityType;
            this.entityCollectionNode = null;
            this.navigationSource = navigationSource;
        }

        /// <summary>
        /// Gets the name of the associated any/all parameter (null if none)
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the Entity collection that this rangeVariable node iterates over
        /// </summary>
        public CollectionResourceNode EntityCollectionNode
        {
            get { return this.entityCollectionNode; }
        }

        /// <summary>
        /// Gets the navigation source of the collection this node iterates over.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the entity type of each item in the collection that this range variable iterates over.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the entity type of each item in the collection that this range variable iterates over.
        /// </summary>
        public IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        public override int Kind
        {
            get { return RangeVariableKind.Resource; }
        }
    }
}