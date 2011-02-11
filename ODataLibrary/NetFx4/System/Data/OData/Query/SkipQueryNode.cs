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

namespace System.Data.OData.Query
{
    #region Namespaces.
    using System.Data.Services.Providers;
    #endregion Namespaces.

    /// <summary>
    /// Query node representing a skip operator.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class SkipQueryNode : CollectionQueryNode
#else
    public sealed class SkipQueryNode : CollectionQueryNode
#endif
    {
        /// <summary>
        /// The number of entities to skip in the result.
        /// </summary>
        public QueryNode Amount
        {
            get;
            set;
        }

        /// <summary>
        /// The collection to skip items from.
        /// </summary>
        public CollectionQueryNode Collection
        {
            get;
            set;
        }

        /// <summary>
        /// The resouce type of a single item from the collection represented by this node.
        /// </summary>
        public override ResourceType ItemType
        {
            get
            {
                if (this.Collection == null)
                {
                    return null;
                }
                else
                {
                    return this.Collection.ItemType;
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
                return QueryNodeKind.Skip;
            }
        }
    }
}
