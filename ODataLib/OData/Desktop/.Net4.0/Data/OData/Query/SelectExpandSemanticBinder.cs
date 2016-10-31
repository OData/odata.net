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
