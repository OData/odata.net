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
    /// Base class for all semantic metadata bound nodes which represent a composable collection of values.
    /// </summary>
#if INTERNAL_DROP
    internal abstract class CollectionQueryNode : QueryNode
#else
    public abstract class CollectionQueryNode : QueryNode
#endif
    {
        /// <summary>
        /// The resouce type of a single item from the collection represented by this node.
        /// </summary>
        public abstract ResourceType ItemType
        {
            get;
        }
    }
}
