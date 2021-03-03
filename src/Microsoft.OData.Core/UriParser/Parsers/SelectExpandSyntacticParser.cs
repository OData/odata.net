//---------------------------------------------------------------------
// <copyright file="SelectExpandSyntacticParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Parse the raw select and expand clause syntax.
    /// </summary>
    internal static class SelectExpandSyntacticParser
    {
        /// <summary>
        /// Parse the raw select and expand strings into Abstract Syntax Trees
        /// </summary>
        /// <param name="selectClause">the raw select string</param>
        /// <param name="expandClause">the raw expand string</param>
        /// <param name="parentStructuredType">the parent entity type for expand option</param>
        /// <param name="configuration">the OData URI parser configuration</param>
        /// <param name="expandTree">the resulting expand AST</param>
        /// <param name="selectTree">the resulting select AST</param>
        public static void Parse(
            string selectClause,
            string expandClause,
            IEdmStructuredType parentStructuredType,
            ODataUriParserConfiguration configuration,
            out ExpandToken expandTree,
            out SelectToken selectTree)
        {
            SelectExpandParser selectParser = new SelectExpandParser(
                selectClause,
                configuration.Settings.SelectExpandLimit,
                configuration.EnableCaseInsensitiveUriFunctionIdentifier,
                configuration.EnableNoDollarQueryOptions)
            {
                MaxPathDepth = configuration.Settings.PathLimit
            };
            selectTree = selectParser.ParseSelect();

            SelectExpandParser expandParser = new SelectExpandParser(
                configuration.Resolver,
                expandClause,
                parentStructuredType,
                configuration.Settings.SelectExpandLimit,
                configuration.EnableCaseInsensitiveUriFunctionIdentifier,
                configuration.EnableNoDollarQueryOptions)
            {
                MaxPathDepth = configuration.Settings.PathLimit,
                MaxFilterDepth = configuration.Settings.FilterLimit,
                MaxOrderByDepth = configuration.Settings.OrderByLimit,
                MaxSearchDepth = configuration.Settings.SearchLimit
            };
            expandTree = expandParser.ParseExpand();
        }
    }
}