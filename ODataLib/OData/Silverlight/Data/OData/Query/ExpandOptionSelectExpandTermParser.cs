//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Object that knows how to parse a single term within a select expression. That is, a path to a property, 
    /// a wildcard, operation name, etc.
    /// </summary>
    internal sealed class ExpandOptionSelectExpandTermParser : SelectExpandTermParser
    {
        /// <summary>
        /// Build the ExpandOption strategy.
        /// </summary>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <param name="maxDepth">max recursive depth</param>
        public ExpandOptionSelectExpandTermParser(string clauseToParse, int maxDepth) : base(clauseToParse, maxDepth)
        {   
        }

        /// <summary>
        /// Build the list of expand options
        /// Depends on whether options are allowed or not.
        /// </summary>
        /// <param name="isInnerTerm">is this an inner expand term</param>
        /// <param name="pathToken">the current level token, as a PathToken</param>
        /// <returns>An expand term token based on the path token, and all available expand options.</returns>
        internal override ExpandTermToken BuildExpandTermToken(bool isInnerTerm, PathSegmentToken pathToken)
        {
            DebugUtils.CheckNoExternalCallers();

            QueryToken filterOption = null;
            OrderByToken orderByOption = null;
            long? topOption = null;
            long? skipOption = null;
            InlineCountKind? inlineCountOption  = null;
            SelectToken selectOption = null;
            ExpandToken expandOption = null;

            if (this.Lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            {
                // could have a filter, orderby, etc. or another select and expand.
                while (this.Lexer.PeekNextToken().Kind != ExpressionTokenKind.CloseParen)
                {
                    switch (this.Lexer.NextToken().Text)
                    {
                        case ExpressionConstants.QueryOptionFilter:
                            {
                                // advance to the equal sign
                                this.Lexer.NextToken();
                                string filterText = this.ReadQueryOption();
                                UriQueryExpressionParser filterParser = new UriQueryExpressionParser(this.MaxDepth);
                                filterOption = filterParser.ParseFilter(filterText);
                                break;
                            }

                        case ExpressionConstants.QueryOptionOrderby:
                            {
                                // advance to the equal sign
                                this.Lexer.NextToken();
                                string orderByText = this.ReadQueryOption();
                                UriQueryExpressionParser orderbyParser = new UriQueryExpressionParser(this.MaxDepth);
                                orderByOption = orderbyParser.ParseOrderBy(orderByText).Single();
                                break;
                            }

                        case ExpressionConstants.QueryOptionTop:
                            {
                                // advance to the equal sign
                                this.Lexer.NextToken();
                                string topText = this.ReadQueryOption();

                                // TryParse requires a non-nullable long.
                                long top;
                                if (!long.TryParse(topText, out top))
                                {
                                    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidTopOption(topText));
                                }

                                topOption = top;

                                break;
                            }

                        case ExpressionConstants.QueryOptionSkip:
                            {
                                // advance to the equal sign
                                this.Lexer.NextToken();
                                string skipText = this.ReadQueryOption();

                                // TryParse requires a non-nullable long.
                                long skip;
                                if (!long.TryParse(skipText, out skip))
                                {
                                    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidSkipOption(skipText));
                                }

                                skipOption = skip;

                                break;
                            }

                        case ExpressionConstants.QueryOptionInlineCount:
                            {
                                // advance to the equal sign
                                this.Lexer.NextToken();
                                string inlineCountText = this.ReadQueryOption();
                                switch (inlineCountText)
                                {
                                    case ExpressionConstants.InlineCountNone:
                                        {
                                            inlineCountOption = InlineCountKind.None;
                                            break;
                                        }

                                    case ExpressionConstants.InlineCountAllPages:
                                        {
                                            inlineCountOption = InlineCountKind.AllPages;
                                            break;
                                        }

                                    default:
                                        {
                                            throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.Lexer.ExpressionText));
                                        }
                                }

                                break;
                            }

                        case ExpressionConstants.QueryOptionSelect:
                            {
                                // advance to the equal sign
                                this.Lexer.NextToken();
                                selectOption = this.ParseSelect();
                                break;
                            }

                        case ExpressionConstants.QueryOptionExpand:
                            {
                                // advance to the equal sign
                                this.Lexer.NextToken();
                                expandOption = this.ParseExpand();
                                break;
                            }

                        default:
                            {
                                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.Lexer.ExpressionText));
                            }
                    }
                }
            }
            else if (this.IsNotEndOfTerm(isInnerTerm))
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.Lexer.ExpressionText));
            }

            return new ExpandTermToken(pathToken, filterOption, orderByOption, topOption, skipOption, inlineCountOption, selectOption, expandOption);
        }

        /// <summary>
        /// determine whether we're at the end of a select or expand term
        /// </summary>
        /// <param name="isInnerTerm">flag to indicate whether this is an outer or inner select.</param>
        /// <returns>true if we are not at the end of a select term.</returns>
        internal override bool IsNotEndOfTerm(bool isInnerTerm)
        {
            DebugUtils.CheckNoExternalCallers();
            if (!isInnerTerm)
            {
                return this.Lexer.CurrentToken.Kind != ExpressionTokenKind.End &&
                       this.Lexer.CurrentToken.Kind != ExpressionTokenKind.Comma;
            }

            return this.Lexer.CurrentToken.Kind != ExpressionTokenKind.End &&
                   this.Lexer.CurrentToken.Kind != ExpressionTokenKind.Comma &&
                   this.Lexer.CurrentToken.Kind != ExpressionTokenKind.SemiColon;
        }

        /// <summary>
        /// Read a query option from the lexer.
        /// </summary>
        /// <returns>the query option as a string.</returns>
        private string ReadQueryOption()
        {
            if (this.Lexer.CurrentToken.Kind != ExpressionTokenKind.Equal)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.Lexer.ExpressionText));
            }

            // a query option looks like
            // <anytext>;

            // advance the lexer to the beginning of the query option.
            this.Lexer.NextToken();

            // get the full text from the current location onward
            string textToReturn = this.Lexer.ExpressionText.Substring(this.Lexer.Position);
            textToReturn = textToReturn.Split(';').First();

            while (this.Lexer.PeekNextToken().Kind != ExpressionTokenKind.SemiColon)
            {
                this.Lexer.NextToken();
            }

            // next token is a semicolon, so advance there
            this.Lexer.NextToken();
            return textToReturn;
        }
    }
}
