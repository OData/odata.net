//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.Data.Edm;
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
        /// The EntitySet containing the collection this key referrs to.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

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
            this.entitySet = source.EntitySet;
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
        /// Gets the EntitySet that contains the collection this key referrs to.
        /// </summary>
        public override IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the kind for this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return InternalQueryNodeKind.KeyLookup;
            }
        }
    }
}
