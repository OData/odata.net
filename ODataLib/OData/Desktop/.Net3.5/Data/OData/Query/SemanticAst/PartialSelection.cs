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
