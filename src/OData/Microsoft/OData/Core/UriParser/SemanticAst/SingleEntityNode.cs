//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Base class for all semantic metadata bound nodes which represent a single composable entity value.
    /// </summary>
    public abstract class SingleEntityNode : SingleValueNode
    {
        /// <summary>
        /// Gets the navigation source containing this single entity.
        /// </summary>
        public abstract IEdmNavigationSource NavigationSource { get; }
    
        /// <summary>
        /// Gets the type of this single entity.
        /// </summary>
        public abstract IEdmEntityTypeReference EntityTypeReference { get; }
    }
}
