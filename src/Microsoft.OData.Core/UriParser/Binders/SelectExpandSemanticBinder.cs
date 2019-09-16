//---------------------------------------------------------------------
// <copyright file="SelectExpandSemanticBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Add semantic meaning to a Select or Expand token.
    /// </summary>
    internal sealed class SelectExpandSemanticBinder
    {
        /// <summary>
        /// Add semantic meaning to a Select or Expand Token
        /// </summary>
        /// <param name="odataPathInfo">The path info from Uri path.</param>
        /// <param name="expandToken">the syntactically parsed expand token</param>
        /// <param name="selectToken">the syntactically parsed select token</param>
        /// <param name="configuration">The configuration to use for parsing.</param>
        /// <param name="state">The state of binding.</param>
        /// <returns>A select expand clause bound to metadata.</returns>
        public static SelectExpandClause Bind(
            ODataPathInfo odataPathInfo,
            ExpandToken expandToken,
            SelectToken selectToken,
            ODataUriParserConfiguration configuration,
            BindingState state)
        {
            ExpandToken normalizedExpand = ExpandTreeNormalizer.NormalizeExpandTree(expandToken);
            SelectToken normalizedSelect = SelectTreeNormalizer.NormalizeSelectTree(selectToken);

            SelectExpandBinder selectExpandBinder = new SelectExpandBinder(configuration, odataPathInfo, state);

            SelectExpandClause clause = selectExpandBinder.Bind(normalizedExpand, normalizedSelect);

            SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(clause);

            new ExpandDepthAndCountValidator(configuration.Settings.MaximumExpansionDepth, configuration.Settings.MaximumExpansionCount).Validate(clause);

            return clause;
        }
    }
}