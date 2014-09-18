//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
