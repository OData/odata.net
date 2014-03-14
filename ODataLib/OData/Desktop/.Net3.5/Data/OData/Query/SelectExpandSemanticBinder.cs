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

namespace Microsoft.Data.OData.Query
{
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;

    /// <summary>
    /// Add semantic meaning to a Select or Expand token.
    /// </summary>
    internal static class SelectExpandSemanticBinder
    {
        /// <summary>
        /// Add semantic meaning to a Select or Expand Token
        /// </summary>
        /// <param name="elementType">the top level entity type.</param>
        /// <param name="entitySet">the top level entity set</param>
        /// <param name="expandToken">the syntactically parsed expand token</param>
        /// <param name="selectToken">the syntactically parsed select token</param>
        /// <param name="configuration">The configuration to use for parsing.</param>
        /// <returns>A select expand clause bound to metadata.</returns>
        public static SelectExpandClause Parse(
            IEdmEntityType elementType, 
            IEdmEntitySet entitySet,
            ExpandToken expandToken, 
            SelectToken selectToken, 
            ODataUriParserConfiguration configuration)
        {
            expandToken = ExpandTreeNormalizer.NormalizeExpandTree(expandToken);
            selectToken = SelectTreeNormalizer.NormalizeSelectTree(selectToken);

            ExpandBinder expandBinder = ExpandBinderFactory.Create(elementType, entitySet, configuration);
            SelectExpandClause clause = expandBinder.Bind(expandToken);

            SelectBinder selectedPropertyBinder = new SelectBinder(configuration.Model, elementType, configuration.Settings.SelectExpandLimit, clause);
            clause = selectedPropertyBinder.Bind(selectToken);

            var prunedTree = SelectExpandTreeFinisher.PruneSelectExpandTree(clause);
            prunedTree.ComputeFinalSelectedItems();

            new ExpandDepthAndCountValidator(configuration.Settings.MaximumExpansionDepth, configuration.Settings.MaximumExpansionCount).Validate(prunedTree);

            return prunedTree;
        }
    }
}
