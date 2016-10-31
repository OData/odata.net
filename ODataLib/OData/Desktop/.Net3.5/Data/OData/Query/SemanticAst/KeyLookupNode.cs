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
