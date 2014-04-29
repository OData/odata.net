//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Add semantic meaning to a Select or Expand token.
    /// </summary>
    //// TODO: Rename this to SelectExpandSemanticBinder when we're only using V4
    internal sealed class SelectExpandSemanticBinder
    {
        /// <summary>
        /// Add semantic meaning to a Select or Expand Token
        /// </summary>
        /// <param name="elementType">the top level entity type.</param>
        /// <param name="navigationSource">the top level navigation source</param>
        /// <param name="expandToken">the syntactically parsed expand token</param>
        /// <param name="selectToken">the syntactically parsed select token</param>
        /// <param name="configuration">The configuration to use for parsing.</param>
        /// <returns>A select expand clause bound to metadata.</returns>
        public SelectExpandClause Bind(
            IEdmStructuredType elementType, 
            IEdmNavigationSource navigationSource,
            ExpandToken expandToken, 
            SelectToken selectToken, 
            ODataUriParserConfiguration configuration)
        {
            ExpandToken unifiedSelectExpandToken = SelectExpandSyntacticUnifier.Combine(expandToken, selectToken);

            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            ExpandToken normalizedSelectExpandToken = expandTreeNormalizer.NormalizeExpandTree(unifiedSelectExpandToken);

            SelectExpandBinder selectExpandBinder = new SelectExpandBinder(configuration, elementType, navigationSource);
            SelectExpandClause clause = selectExpandBinder.Bind(normalizedSelectExpandToken);

            SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(clause);

            new ExpandDepthAndCountValidator(configuration.Settings.MaximumExpansionDepth, configuration.Settings.MaximumExpansionCount).Validate(clause);

            return clause;
        }
    }
}
