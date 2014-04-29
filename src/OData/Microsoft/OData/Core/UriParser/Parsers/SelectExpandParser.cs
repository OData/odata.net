//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Object that knows how to parse a select or expand expression. That is, a path to a property, 
    /// a wildcard, operation name, etc, including nested expand options.
    /// </summary>
    internal sealed class SelectExpandParser
    {
        /// <summary>
        /// Object to handle the parsing of any nested expand options that we discover.
        /// </summary>
        private readonly ExpandOptionParser expandOptionParser;

        /// <summary>
        /// The maximum allowable recursive depth.
        /// </summary>
        private readonly int maxRecursiveDepth;

        /// <summary>
        /// Lexer used to parse an the $select or $expand string.
        /// </summary>
        private ExpressionLexer lexer;

        /// <summary>
        /// True if we are we parsing $select.
        /// </summary>
        private bool isSelect;


        /// <summary>
        /// Build the ExpandOption strategy.
        /// TODO: Really should not take the clauseToParse here. Instead it should be provided with a call to ParseSelect() or ParseExpand().
        /// </summary>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <param name="maxRecursiveDepth">max recursive depth</param>
        public SelectExpandParser(string clauseToParse, int maxRecursiveDepth)
        {
            this.maxRecursiveDepth = maxRecursiveDepth;
            expandOptionParser = new ExpandOptionParser(this.maxRecursiveDepth);

            // Sets up our lexer. We don't turn useSemicolonDelimiter on since the parsing code for expand options, 
            // which is the only thing that needs it, is in a different class that uses it's own lexer.
            this.lexer = clauseToParse != null ? new ExpressionLexer(clauseToParse, false /*moveToFirstToken*/, false /*useSemicolonDelimiter*/) : null;
        }

        /// <summary>
        /// The maximum recursive depth.
        /// </summary>
        public int MaxRecursiveDepth 
        {
            get { return this.maxRecursiveDepth; }
        }

        /// <summary>
        /// Parses a full $select expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public SelectToken ParseSelect()
        {
            this.isSelect = true;
            return this.ParseCommaSeperatedList(termTokens => new SelectToken(termTokens), this.ParseSingleSelectTerm);
        }

        /// <summary>
        /// Parses a full $expand expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public ExpandToken ParseExpand()
        {
            this.isSelect = false;
            return this.ParseCommaSeperatedList(termTokens => new ExpandToken(termTokens), this.ParseSingleExpandTerm);
        }

        /// <summary>
        /// Parses a single term in a comma seperated list of things to select.
        /// </summary>
        /// <returns>A token representing thing to select.</returns>
        private PathSegmentToken ParseSingleSelectTerm()
        {
            this.isSelect = true;

            // TODO: Why are we using the max recursion depth as the max path length? Weird.
            var termParser = new SelectExpandTermParser(this.lexer, this.MaxRecursiveDepth, this.isSelect);
            return termParser.ParseTerm();
        }

        /// <summary>
        /// Parses a single term in a comma seperated list of things to expand.
        /// </summary>
        /// <returns>A token representing thing to expand.</returns>
        private ExpandTermToken ParseSingleExpandTerm()
        {
            this.isSelect = false;

            // TODO: Why are we using the max recursion depth as the max path length? Weird.
            var termParser = new SelectExpandTermParser(this.lexer, this.maxRecursiveDepth, this.isSelect);
            PathSegmentToken pathToken = termParser.ParseTerm();

            string optionsText = null;
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            {
                optionsText = this.lexer.AdvanceThroughBalancedParentheticalExpression();

                // Move lexer to what is after the parenthesis expression. Now CurrentToken will be the next thing.
                this.lexer.NextToken();
            }

            return this.expandOptionParser.BuildExpandTermToken(pathToken, optionsText);
        }

        /// <summary>
        /// Parsed a comma seperated list of $select or $expand terms.
        /// </summary>
        /// <typeparam name="TFinalToken">The type of the final token to produce.</typeparam>
        /// <typeparam name="TTermToken">The type of the term tokens that are fed into the final token.</typeparam>
        /// <param name="ctor">A method to contruct the final token from the term tokens.</param>
        /// <param name="termParsingFunc">A method to parse each individual term.</param>
        /// <returns>A token representing the entire $expand or $select clause syntactically.</returns>
        private TFinalToken ParseCommaSeperatedList<TFinalToken, TTermToken>(Func<IEnumerable<TTermToken>, TFinalToken> ctor, Func<TTermToken> termParsingFunc)
        {
            List<TTermToken> termTokens = new List<TTermToken>();

            // This happens if we were passed a null string
            if (this.lexer == null)
            {
                return ctor(termTokens);
            }

            // Move to the first token
            this.lexer.NextToken();

            // This happens if it was just whitespace. e.g. fake.svc/Customers?$expand=     &$filter=IsCool&$orderby=ID
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.End)
            {
                return ctor(termTokens);
            }

            // Process first term
            termTokens.Add(termParsingFunc());

            // If it was a list of terms, then commas will be seperating them
            while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
            {
                // Move over the ',' to the next term
                this.lexer.NextToken();
                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
                {
                    termTokens.Add(termParsingFunc());
                }
                else
                {
                    break;
                }
            }

            // If there isn't a comma, then we must be done. Otherwise there is a syntax error
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            return ctor(termTokens);
        }
    }
}
