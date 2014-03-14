//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System.Collections.Generic;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.Edm;

    /// <summary>
    /// ExpandOption variant of an ExpandBinder, where the default selection item for a given level is based on the select at that level
    /// instead of the top level select clause. If nothing is selected for a given expand in the ExpandOption syntax, then we by default
    /// select all from that item, instead of selecting nothing (and therefore pruning the expand off of the tree).
    /// </summary>
    internal sealed class ExpandOptionExpandBinder : ExpandBinder
    {
        /// <summary>
        /// Build the ExpandOption variant of an ExpandBinder
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="entityType">The entity type of the top level expand item.</param>
        /// <param name="entitySet">The entity set of the top level expand item.</param>
        public ExpandOptionExpandBinder(ODataUriParserConfiguration configuration, IEdmEntityType entityType, IEdmEntitySet entitySet)
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
            ExpandBinder nextLevelBinder = new ExpandOptionExpandBinder(this.Configuration, currentNavProp.ToEntityType(), this.EntitySet != null ? this.EntitySet.FindNavigationTarget(currentNavProp) : null);
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
            SelectBinder selectBinder = new SelectBinder(this.Model, currentNavProp.ToEntityType(), this.Settings.SelectExpandLimit, subExpand);
            return selectBinder.Bind(select);
        }
    }
}
