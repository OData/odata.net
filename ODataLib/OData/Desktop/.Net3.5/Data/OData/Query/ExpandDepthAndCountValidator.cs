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

namespace Microsoft.Data.OData.Query
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// A component for walking an expand tree and determining if the depth or number of items exceed user-specified limits.
    /// </summary>
    internal sealed class ExpandDepthAndCountValidator
    {
        /// <summary>
        /// The maximum depth of any expand tree being validated.
        /// </summary>
        private readonly int maxDepth;

        /// <summary>
        /// The maximum number of expand items allowed in any expand tree being validated, including leaf and non-leaf nodes.
        /// </summary>
        private readonly int maxCount;

        /// <summary>
        /// The current count when validating a particular tree.
        /// </summary>
        private int currentCount;

        /// <summary>
        /// Initializes a new instance of <see cref="ExpandDepthAndCountValidator"/>.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of an expand tree.</param>
        /// <param name="maxCount">The maximum number of expanded items allowed in a tree.</param>
        internal ExpandDepthAndCountValidator(int maxDepth, int maxCount)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(maxDepth >= 0, "Max depth cannot be negative.");
            Debug.Assert(maxCount >= 0, "Max count cannot be negative.");
            this.maxDepth = maxDepth;
            this.maxCount = maxCount;
        }

        /// <summary>
        /// Validates the given tree against the user-specified limits.
        /// </summary>
        /// <param name="expandTree">The expand tree to validate.</param>
        internal void Validate(SelectExpandClause expandTree)
        {
            DebugUtils.CheckNoExternalCallers();
            this.currentCount = 0;
            this.EnsureMaximumCountAndDepthAreNotExceeded(expandTree, /*currentDepth*/ 0);
        }

        /// <summary>
        /// Recursively ensures that the maximum count/depth are not exceeded by walking the tree.
        /// </summary>
        /// <param name="expandTree">The expand tree to walk and validate.</param>
        /// <param name="currentDepth">The current depth of the tree walk.</param>
        private void EnsureMaximumCountAndDepthAreNotExceeded(SelectExpandClause expandTree, int currentDepth)
        {
            Debug.Assert(expandTree != null, "expandTree != null");
            if (currentDepth > this.maxDepth)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.UriParser_ExpandDepthExceeded(currentDepth, this.maxDepth));
            }

            foreach (var expandItem in expandTree.SelectedItems.OfType<ExpandedNavigationSelectItem>())
            {
                this.currentCount++;
                if (this.currentCount > this.maxCount)
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.UriParser_ExpandCountExceeded(this.currentCount, this.maxCount));
                }

                this.EnsureMaximumCountAndDepthAreNotExceeded(expandItem.SelectAndExpand, currentDepth + 1);
            }
        }
    }
}
