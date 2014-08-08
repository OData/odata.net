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

    using System;
    using Microsoft.Data.Edm;

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
