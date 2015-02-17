//---------------------------------------------------------------------
// <copyright file="KeyLookupNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Node representing a key lookup on a collection.
    /// </summary>
    internal sealed class KeyLookupNode : SingleEntityNode
    {
        /// <summary>
        /// The collection that this key is referring to.
        /// </summary>
        private readonly EntityCollectionNode source;

        /// <summary>
        /// The navigation source containing the collection this key referrs to.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// The resouce type of the single value the key referrs to.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// List of the properties and their values that we use to look up our return value.
        /// </summary>
        private readonly IEnumerable<KeyPropertyValue> keyPropertyValues;

        /// <summary>
        /// Constructs a KeyLookupNode.
        /// </summary>
        /// <param name="source">The collection that this key is referring to.</param>
        /// <param name="keyPropertyValues">List of the properties and their values that we use to look up our return value.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input source is null.</exception>
        public KeyLookupNode(EntityCollectionNode source, IEnumerable<KeyPropertyValue> keyPropertyValues)
        {
            ExceptionUtils.CheckArgumentNotNull(source, "source");
            this.source = source;
            this.navigationSource = source.NavigationSource;
            this.entityTypeReference = source.EntityItemType;
            this.keyPropertyValues = keyPropertyValues;
        }

        /// <summary>
        /// Gets the collection that this key is referring to.
        /// </summary>
        public EntityCollectionNode Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the list of the properties and their values that we use to look up our return value.
        /// </summary>
        public IEnumerable<KeyPropertyValue> KeyPropertyValues
        {
            get { return this.keyPropertyValues; }
        }

        /// <summary>
        /// Gets the resouce type of the single value that the key referrs to.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                return this.entityTypeReference;
            }
        }

        /// <summary>
        /// Gets the resouce type of the single value that the key referrs to.
        /// </summary>
        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the navigation source that contains the collection this key referrs to.
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the kind for this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.KeyLookup;
            }
        }
    }
}