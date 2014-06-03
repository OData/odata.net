//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Parser that knows how to parse expand options that could come after the path part of an expand term.
    /// Delegates to other parsing code as needed. I.E., when a nested $filter comes along, this will
    /// fire up the filter parsing code to figure that out. That code won't even know that it came from a nested location.
    /// </summary>
    internal sealed class ExpandOptionParser
    {
        /// <summary>
        /// Max recursion depth. As we recurse, each new instance of this class will have this lowered by 1.
        /// </summary>
        private readonly int maxRecursionDepth;

        /// <summary>
        /// Lexer to parse over the optionsText for a single $expand term. This is NOT the same lexer used by <see cref="SelectExpandParser"/>
        /// to parse over the entirety of $select or $expand. 
        /// </summary>
        private ExpressionLexer lexer;

        /// <summary>
        /// Creates an instance of this class to parse options.
        /// </summary>
        /// <param name="maxRecursionDepth">Max recursion depth left.</param>
        internal ExpandOptionParser(int maxRecursionDepth)
        {
            this.maxRecursionDepth = maxRecursionDepth;
        }

        /// <summary>
        /// The maximum depth for $filter nested in $expand.
        /// </summary>
        internal int MaxFilterDepth { get; set; }

        /// <summary>
        /// The maximum depth for $orderby nested in $expand.
        /// </summary>
        internal int MaxOrderByDepth { get; set; }

        /// <summary>
        /// The maximum depth for $search nested in $expand.
        /// </summary>
        internal int MaxSearchDepth { get; set; }

        /// <summary>
        /// Building off of a PathSegmentToken, continue parsing any expand options (nested $filter, $expand, etc)
        /// to build up an ExpandTermToken which fully represents the tree that makes up this expand term.
        /// </summary>
        /// <param name="pathToken">The PathSegmentToken representing the parsed expand path whose options we are now parsing.</param>
        /// <param name="optionsText">A string of the text between the parenthesis after an expand option.</param>
        /// <returns>An expand term token based on the path token, and all available expand options.</returns>
        internal ExpandTermToken BuildExpandTermToken(PathSegmentToken pathToken, string optionsText)
        {
            // Setup a new lexer for parsing the optionsText
            this.lexer = new ExpressionLexer(optionsText ?? "", true /*moveToFirstToken*/, true /*useSemicolonDelimiter*/);

            QueryToken filterOption = null;
            IEnumerable<OrderByToken> orderByOptions = null;
            long? topOption = null;
            long? skipOption = null;
            bool? countOption = null;
            long? levelsOption = null;
            QueryToken searchOption = null;
            SelectToken selectOption = null;
            ExpandToken expandOption = null;

            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            {
                // advance past the '('
                this.lexer.NextToken();

                // Check for (), which is not allowed.
                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_MissingExpandOption(pathToken.Identifier));
                }

                // Look for all the supported query options
                while (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
                {
                    switch (this.lexer.CurrentToken.Text)
                    {
                        case ExpressionConstants.QueryOptionFilter:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string filterText = this.ReadQueryOption();

                                UriQueryExpressionParser filterParser = new UriQueryExpressionParser(this.MaxFilterDepth);
                                filterOption = filterParser.ParseFilter(filterText);
                                break;
                            }

                        case ExpressionConstants.QueryOptionOrderby:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string orderByText = this.ReadQueryOption();

                                UriQueryExpressionParser orderbyParser = new UriQueryExpressionParser(this.MaxOrderByDepth);
                                orderByOptions = orderbyParser.ParseOrderBy(orderByText);
                                break;
                            }

                        case ExpressionConstants.QueryOptionTop:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string topText = this.ReadQueryOption();

                                // TryParse requires a non-nullable non-negative long.
                                long top;
                                if (!long.TryParse(topText, out top) || top < 0)
                                {
                                    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidTopOption(topText));
                                }

                                topOption = top;
                                break;
                            }

                        case ExpressionConstants.QueryOptionSkip:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string skipText = this.ReadQueryOption();

                                // TryParse requires a non-nullable non-negative long.
                                long skip;
                                if (!long.TryParse(skipText, out skip) || skip < 0)
                                {
                                    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidSkipOption(skipText));
                                }

                                skipOption = skip;
                                break;
                            }

                        case ExpressionConstants.QueryOptionCount:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string countText = this.ReadQueryOption();
                                switch (countText)
                                {
                                    case ExpressionConstants.KeywordTrue:
                                        {
                                            countOption = true;
                                            break;
                                        }

                                    case ExpressionConstants.KeywordFalse:
                                        {
                                            countOption = false;
                                            break;
                                        }

                                    default:
                                        {
                                            throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidCountOption(countText));
                                        }
                                }

                                break;
                            }

                        case ExpressionConstants.QueryOptionLevels:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string levelsText = this.ReadQueryOption();
                                long level;

                                if (string.CompareOrdinal(ExpressionConstants.KeywordMax, levelsText) == 0)
                                {
                                    levelsOption = long.MinValue;
                                }
                                else if (!long.TryParse(levelsText, out level) || level < 0)
                                {
                                    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidLevelsOption(levelsText));
                                }
                                else
                                {
                                    levelsOption = level;
                                }

                                break;
                            }

                        case ExpressionConstants.QueryOptionSearch:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string searchText = this.ReadQueryOption();

                                SearchParser searchParser = new SearchParser(this.MaxSearchDepth);
                                searchOption = searchParser.ParseSearch(searchText);

                                break;
                            }

                        case ExpressionConstants.QueryOptionSelect:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string selectText = this.ReadQueryOption();

                                SelectExpandParser innerSelectParser = new SelectExpandParser(selectText, this.maxRecursionDepth);
                                selectOption = innerSelectParser.ParseSelect();
                                break;
                            }

                        case ExpressionConstants.QueryOptionExpand:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string expandText = this.ReadQueryOption();

                                SelectExpandParser innerExpandParser = new SelectExpandParser(expandText, this.maxRecursionDepth - 1);
                                expandOption = innerExpandParser.ParseExpand();
                                break;
                            }

                        default:
                            {
                                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
                            }
                    }
                }

                // Move past the ')'
                this.lexer.NextToken();
            }

            // Either there was no '(' at all or we just read past the ')' so we should be at the end
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            return new ExpandTermToken(pathToken, filterOption, orderByOptions, topOption, skipOption, countOption, levelsOption, searchOption, selectOption, expandOption);
        }

        /// <summary>
        /// Read a query option from the lexer.
        /// </summary>
        /// <returns>The query option as a string.</returns>
        private string ReadQueryOption()
        {
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Equal)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            // get the full text from the current location onward
            // there could be literals like 'A string literal; tricky!' in there, so we need to be careful.
            // Also there could be more nested (...) expressions that we ignore until we recurse enough times to get there.
            string expressionText = this.lexer.AdvanceThroughExpandOption();

            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.SemiColon)
            {
                // Move over the ';' seperator
                this.lexer.NextToken();
                return expressionText;
            }

            // If there wasn't a semicolon, it MUST be the last option. We must be at ')' in this case
            this.lexer.ValidateToken(ExpressionTokenKind.CloseParen);
            return expressionText;
        }
    }
}
