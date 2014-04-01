//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// NonOption variant of an ExpandBinder, where the default selection at each level is based on the top level select
    /// clause. If that top level select is not populated, then we select all from this level, instead of selecting nothing.
    /// </summary>
    //// TODO 1466134 We don't need this layer once V4 is working and always used.
    internal sealed class NonOptionExpandBinder : ExpandBinder
    {
        /// <summary>
        /// Build the NonOption variant of an ExpandBinder
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="edmType">The type of the top level expand item.</param>
        /// <param name="entitySet">The entity set of the top level expand item.</param>
        public NonOptionExpandBinder(ODataUriParserConfiguration configuration, IEdmStructuredType edmType, IEdmEntitySet entitySet)
            : base(configuration, edmType, entitySet)
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
