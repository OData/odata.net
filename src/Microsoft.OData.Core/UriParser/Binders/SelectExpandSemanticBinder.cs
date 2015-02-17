//---------------------------------------------------------------------
// <copyright file="SelectExpandSemanticBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Add semantic meaning to a Select or Expand token.
    /// </summary>
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