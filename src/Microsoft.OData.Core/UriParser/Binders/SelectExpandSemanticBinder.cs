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
        /// <returns>A select expand clause bound to metadata.</returns>
        public static SelectExpandClause Bind(
            ODataPathInfo odataPathInfo,
            ExpandToken expandToken,
            SelectToken selectToken,
            ODataUriParserConfiguration configuration)
        {
            ExpandToken unifiedSelectExpandToken = SelectExpandSyntacticUnifier.Combine(expandToken, selectToken);

            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            ExpandToken normalizedSelectExpandToken = expandTreeNormalizer.NormalizeExpandTree(unifiedSelectExpandToken);

            SelectExpandBinder selectExpandBinder = new SelectExpandBinder(configuration, odataPathInfo);
            SelectExpandClause clause = selectExpandBinder.Bind(normalizedSelectExpandToken);

            SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(clause);

            new ExpandDepthAndCountValidator(configuration.Settings.MaximumExpansionDepth, configuration.Settings.MaximumExpansionCount).Validate(clause);

            return clause;
        }
    }
}