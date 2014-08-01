//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Query
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// Builder to build a given uri
    /// </summary>
    public sealed class ODataQueryUriBuilder
    {
        /// <summary>
        /// build a uri path to string 
        /// </summary>
        /// <param name="odataUri">parse odataUri to a string representation</param>
        /// <param name="urlConventions">whether is KeyAsSegment</param>
        /// <returns>uri of the semantic tree</returns>
        public static Uri CreateUri(ODataUri odataUri, ODataUrlConventions urlConventions)
        {
            NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
            SelectExpandClauseToStringBuilder selectExpandClauseToStringBuilder = new SelectExpandClauseToStringBuilder();

            String queryOptions = String.Empty;
            bool writeQueryPrefix = true;
            if (odataUri.Filter != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$filter", ExpressionConstants.SymbolEqual, nodeToStringBuilder.TranslateFilterClause(odataUri.Filter));
            }

            if (odataUri.SelectAndExpand != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, selectExpandClauseToStringBuilder.TranslateSelectExpandClause(odataUri.SelectAndExpand));
            }

            if (odataUri.OrderBy != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$orderby", ExpressionConstants.SymbolEqual, nodeToStringBuilder.TranslateOrderByClause(odataUri.OrderBy));
            }

            if (odataUri.Top != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$top", ExpressionConstants.SymbolEqual, odataUri.Top.ToString());
            }

            if (odataUri.Skip != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$skip", ExpressionConstants.SymbolEqual, odataUri.Skip.ToString());
            }

            if (odataUri.QueryCount != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$count", ExpressionConstants.SymbolEqual);
                if (odataUri.QueryCount == true)
                {
                    queryOptions += ExpressionConstants.KeywordTrue;
                }
                else
                {
                    queryOptions += ExpressionConstants.KeywordFalse;
                }
            }

            if (odataUri.Search != null)
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
                queryOptions = string.Concat(queryOptions, "$search", ExpressionConstants.SymbolEqual, nodeToStringBuilder.TranslateSearchClause(odataUri.Search));
            }

            if (odataUri.ParameterAliasNodes.Count > 0)
            {
                string aliasNode = nodeToStringBuilder.TranslateParameterAliasNodes(odataUri.ParameterAliasNodes);
                queryOptions = String.IsNullOrEmpty(aliasNode) ? queryOptions : String.Concat(WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions), aliasNode);
                writeQueryPrefix = false;
            }

            if (writeQueryPrefix || String.IsNullOrEmpty(queryOptions))
            {
                queryOptions = WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
                writeQueryPrefix = false;
            }

            string res = String.Concat(odataUri.Path.ToResourcePathString(urlConventions.UrlConvention.GenerateKeyAsSegment), queryOptions);
            return new Uri(odataUri.ServiceRoot, new Uri(res, UriKind.Relative));
        }

        /// <summary>
        /// Write ? or &amp; depending on whether it is the start of the whole query or query part.
        /// </summary>
        /// <param name="writeQueryPrefix">True if start of whole query, false if not.  
        /// This is set to false after this method is called.</param>
        /// <param name="queryOptions">add a queryPrefix to the queryOptions</param>
        /// <returns>return  the queryOptions added a queryPrefix</returns>
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
