//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces    

    using System;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;

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
