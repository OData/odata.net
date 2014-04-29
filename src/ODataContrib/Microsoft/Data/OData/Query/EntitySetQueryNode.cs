//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Query node representing an entity set, the query root.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class EntitySetQueryNode : CollectionQueryNode
#else
    public sealed class EntitySetQueryNode : CollectionQueryNode
#endif
    {
        /// <summary>
        /// The entity set this node represents.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get;
            set;
        }

        /// <summary>
        /// The type of a single item from the collection represented by this node.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get
            {
                if (this.EntitySet == null)
                {
                    return null;
                }
                else
                {
                    return this.EntitySet.ElementType.ToTypeReference();
                }
            }
        }

        /// <summary>
        /// The kind of the query node.
        /// </summary>
        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.EntitySet;
            }
        }
    }
}
