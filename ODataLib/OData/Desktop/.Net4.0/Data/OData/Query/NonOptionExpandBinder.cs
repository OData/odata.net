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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;

    /// <summary>
    /// NonOption variant of an ExpandBinder, where the default selection at each level is based on the top level select
    /// clause. If that top level select is not populated, then we select all from this level, instead of selecting nothing.
    /// </summary>
    internal sealed class NonOptionExpandBinder : ExpandBinder
    {
        /// <summary>
        /// Build the NonOption variant of an ExpandBinder
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="entityType">The entity type of the top level expand item.</param>
        /// <param name="entitySet">The entity set of the top level expand item.</param>
        public NonOptionExpandBinder(ODataUriParserConfiguration configuration, IEdmEntityType entityType, IEdmEntitySet entitySet)
            : base(configuration, entityType, entitySet)
        {
        }

        /// <summary>
        /// Generate a SubExpand based on the current nav property and the curren token
        /// </summary>
        /// <param name="currentNavProp">the current navigation property</param>
        /// <param name="tokenIn">the current token</param>
        /// <returns>a new SelectExpand clause bound to the current token and nav prop</returns>
        protected override SelectExpandClause GenerateSubExpand(IEdmNavigationProperty currentNavProp, ExpandTermToken tokenIn)
        {
            Debug.Assert(tokenIn.SelectOption == null, "Should not have select on individual expand items for this binder, because the V3 syntax does not support selects within the $expand clause.");
            ExpandBinder nextLevelBinder = new NonOptionExpandBinder(this.Configuration, currentNavProp.ToEntityType(), this.EntitySet != null ? this.EntitySet.FindNavigationTarget(currentNavProp) : null);
            return nextLevelBinder.Bind(tokenIn.ExpandOption);
        }

        /// <summary>
        /// Decorate an expand tree using a select token.
        /// </summary>
        /// <param name="subExpand">the already built sub expand</param>
        /// <param name="currentNavProp">the current navigation property</param>
        /// <param name="select">the select token to use</param>
        /// <returns>A new SelectExpand clause decorated with the select token.</returns>
        protected override SelectExpandClause DecorateExpandWithSelect(SelectExpandClause subExpand, IEdmNavigationProperty currentNavProp, SelectToken select)
        {
            Debug.Assert(select == null, "Should not have select on individual expand items for this binder, because the V3 syntax does not support selects within the $expand clause.");
            return subExpand;
        }
    }
}
