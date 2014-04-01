//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A list of all expanded items at the next level down.
    /// </summary>
    internal sealed class Expansion
    {
        /// <summary>
        /// The list of all expanded items at the next level down.
        /// </summary>
        private readonly IEnumerable<ExpandedNavigationSelectItem> expandItems;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="expandItems">The list of all expanded items at the next level down.</param>
        public Expansion(IEnumerable<ExpandedNavigationSelectItem> expandItems)
        {
            DebugUtils.CheckNoExternalCallers();
            this.expandItems = expandItems as ExpandedNavigationSelectItem[] ?? expandItems.ToArray();
        }
        
        /// <summary>
        /// The list of all expanded items at the next level down.
        /// </summary>
        public IEnumerable<ExpandedNavigationSelectItem> ExpandItems
        {
            get
            {
                DebugUtils.CheckNoExternalCallers(); 
                return this.expandItems;
            }
        }
    }
}
