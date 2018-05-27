//---------------------------------------------------------------------
// <copyright file="ODataUriExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using Microsoft.OData.UriParser;

    /// <summary>
    /// Extension methods to ODataUri
    /// </summary>
    public static class ODataUriExtensions
    {
        /// <summary>
        /// Build ODataUri into a Uri, the result Uri's query options are URL encoded.
        /// </summary>
        /// <param name="odataUri">ODataUri which will be build to relative url</param>
        /// <param name="urlKeyDelimiter">Value from ODataUrlKeyDelimiter</param>
        /// <returns>Uri of the semantic tree</returns>
        public static Uri BuildUri(this ODataUri odataUri, ODataUrlKeyDelimiter urlKeyDelimiter)
        {
            NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
            SelectExpandClauseToStringBuilder selectExpandClauseToStringBuilder = new SelectExpandClauseToStringBuilder();

            String queryOptions = String.Empty;
            bool writeQueryPrefix = true;
            if (odataUri.Filter != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$filter", ExpressionConstants.SymbolEqual, Uri.EscapeDataString(nodeToStringBuilder.TranslateFilterClause(odataUri.Filter)));
            }

            if (odataUri.SelectAndExpand != null)
            {
                string result = selectExpandClauseToStringBuilder.TranslateSelectExpandClause(odataUri.SelectAndExpand, true);
                if (!string.IsNullOrEmpty(result))
                {
                    queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                    writeQueryPrefix = false;
                    queryOptions = string.Concat(queryOptions, result);
                }
            }

            if (odataUri.Apply != null)
            {
                var applyClauseToStringBuilder = new ApplyClauseToStringBuilder();
                string result = applyClauseToStringBuilder.TranslateApplyClause(odataUri.Apply);
                if (!string.IsNullOrEmpty(result))
                {
                    queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                    writeQueryPrefix = false;
                    queryOptions = string.Concat(queryOptions, result);
                }
            }

            if (odataUri.OrderBy != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$orderby", ExpressionConstants.SymbolEqual, Uri.EscapeDataString(nodeToStringBuilder.TranslateOrderByClause(odataUri.OrderBy)));
            }

            if (odataUri.Top != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$top", ExpressionConstants.SymbolEqual, Uri.EscapeDataString(odataUri.Top.ToString()));
            }

            if (odataUri.Skip != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$skip", ExpressionConstants.SymbolEqual, Uri.EscapeDataString(odataUri.Skip.ToString()));
            }

            if (odataUri.QueryCount != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$count", ExpressionConstants.SymbolEqual, Uri.EscapeDataString(odataUri.QueryCount == true ? ExpressionConstants.KeywordTrue : ExpressionConstants.KeywordFalse));
            }

            if (odataUri.Search != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$search", ExpressionConstants.SymbolEqual, Uri.EscapeDataString(nodeToStringBuilder.TranslateSearchClause(odataUri.Search)));
            }

            if (odataUri.SkipToken != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$skiptoken", ExpressionConstants.SymbolEqual, Uri.EscapeDataString(odataUri.SkipToken));
            }

            if (odataUri.DeltaToken != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$deltatoken", ExpressionConstants.SymbolEqual, Uri.EscapeDataString(odataUri.DeltaToken));
            }

            if (odataUri.ParameterAliasNodes != null && odataUri.ParameterAliasNodes.Count > 0)
            {
                string aliasNode = nodeToStringBuilder.TranslateParameterAliasNodes(odataUri.ParameterAliasNodes);
                queryOptions = String.IsNullOrEmpty(aliasNode) ? queryOptions : String.Concat(WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions), aliasNode);
                writeQueryPrefix = false;
            }

            string res = String.Concat(odataUri.Path.ToResourcePathString(urlKeyDelimiter), queryOptions);
            return odataUri.ServiceRoot == null ? new Uri(res, UriKind.Relative) : new Uri(odataUri.ServiceRoot, new Uri(res, UriKind.Relative));
        }

        /// <summary>
        /// Write ? or &amp; depending on whether it is the start of the whole query or query part.
        /// </summary>
        /// <param name="writeQueryPrefix">True if start of whole query, false if not. </param>
        /// <param name="queryOptions">add a queryPrefix to the queryOptions.</param>
        /// <returns>return  the queryOptions with a queryPrefix</returns>
        private static String WriteQueryPrefixOrSeparator(bool writeQueryPrefix, String queryOptions)
        {
            if (writeQueryPrefix)
            {
                return String.Concat(queryOptions, ExpressionConstants.SymbolQueryStart);
            }
            else
            {
                return String.Concat(queryOptions, ExpressionConstants.SymbolQueryConcatenate);
            }
        }
    }
}
