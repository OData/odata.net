//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Parse the raw select and expand clause syntax.
    /// </summary>
    internal static class SelectExpandSyntacticParser
    {
        /// <summary>
        /// Parse the raw select and expand strings into Abstract Syntax Trees
        /// </summary>
        /// <param name="selectClause">The raw select string</param>
        /// <param name="expandClause">the raw expand string</param>
        /// <param name="configuration">Configuration parameters</param>
        /// <param name="expandTree">the resulting expand AST</param>
        /// <param name="selectTree">the resulting select AST</param>
        public static void Parse(
            string selectClause, 
            string expandClause, 
            ODataUriParserConfiguration configuration, 
            out ExpandToken expandTree, 
            out SelectToken selectTree)
        {
            SelectExpandParser selectParser = new SelectExpandParser(selectClause, configuration.Settings.SelectExpandLimit)
            {
                MaxPathDepth = configuration.Settings.PathLimit
            };
            selectTree = selectParser.ParseSelect();

            SelectExpandParser expandParser = new SelectExpandParser(expandClause, configuration.Settings.SelectExpandLimit)
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
