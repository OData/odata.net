//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System.Collections.Generic;

    /// <summary>
    /// Class that represents a partial subset of items on a given type that have been selected at this level of the select expand tree.
    /// </summary>
    internal sealed class PartialSelection : Selection
    {
        /// <summary>
        /// The subset of items that has been selected at this level.
        /// </summary>
        private readonly IEnumerable<SelectItem> selectedItems;

        /// <summary>
        /// Creates a <see cref="PartialSelection"/> with the specified set of <see cref="SelectItem"/>.
        /// </summary>
        /// <param name="selectedItems">The list of items on the that has been selected.</param>
        public PartialSelection(IEnumerable<SelectItem> selectedItems)
        {
            DebugUtils.CheckNoExternalCallers();
            this.selectedItems = selectedItems ?? new SelectItem[0];
        }

        /// <summary>
        /// The subset of items that has been selected at this level.
        /// </summary>
        public IEnumerable<SelectItem> SelectedItems
        {
            get
            {
                DebugUtils.CheckNoExternalCallers(); 
                return this.selectedItems;
            }
        }
    }
}
