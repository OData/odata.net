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
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Object that knows how to parse a select or expand expression. That is, a path to a property, 
    /// a wildcard, operation name, etc.
    /// TODO 1466134 We don't need this version of the parser once V4 is working and always used.
    /// </summary>
    internal sealed class NonOptionSelectExpandParser : BaseSelectExpandParser
    {
        /// <summary>
        /// Build the NonOption strategy.
        /// </summary>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <param name="maxRecursiveDepth">max recursive depth</param>
        public NonOptionSelectExpandParser(string clauseToParse, int maxRecursiveDepth) : base(clauseToParse, maxRecursiveDepth)
        {
        }

        /// <summary>
        /// Parses a full $select expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public override SelectToken ParseSelect()
        {
            this.IsSelect = true;
            if (this.Lexer == null)
            {
                return new SelectToken(new List<PathSegmentToken>());
            }

            List<PathSegmentToken> selectTerms = new List<PathSegmentToken>();
            bool isInnerTerm = this.Lexer.CurrentToken.Kind == ExpressionTokenKind.Equal;
            while (this.Lexer.PeekNextToken().Kind != ExpressionTokenKind.End &&
                    this.Lexer.PeekNextToken().Kind != ExpressionTokenKind.CloseParen)
            {
                selectTerms.Add(this.ParseSingleSelectTerm(isInnerTerm));
            }

            return new SelectToken(selectTerms);
        }

        /// <summary>
        /// Parses a full $expand expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public override ExpandToken ParseExpand()
        {
            this.IsSelect = false;
            if (this.Lexer == null)
            {
                return new ExpandToken(new List<ExpandTermToken>());
            }

            List<ExpandTermToken> expandTerms = new List<ExpandTermToken>();
            bool isInnerTerm = this.Lexer.CurrentToken.Kind == ExpressionTokenKind.Equal;
            while (this.Lexer.PeekNextToken().Kind != ExpressionTokenKind.End &&
                    this.Lexer.PeekNextToken().Kind != ExpressionTokenKind.CloseParen)
            {
                expandTerms.Add(this.ParseSingleExpandTerm(isInnerTerm));
            }

            return new ExpandToken(expandTerms);
        }

        /// <summary>
        /// Parses a single term in a comma seperated list of things to select.
        /// </summary>
        /// <param name="isInnerTerm">is this an inner or outer select term</param>
        /// <returns>A token representing thing to select.</returns>
        internal PathSegmentToken ParseSingleSelectTerm(bool isInnerTerm)
        {
            DebugUtils.CheckNoExternalCallers();
            this.IsSelect = true;
            PathSegmentToken path = this.ParseSelectExpandProperty();

            if (this.IsNotEndOfTerm(isInnerTerm))
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.Lexer.ExpressionText));
            }

            return path;
        }

        /// <summary>
        /// Parses a single term in a comma seperated list of things to expand.
        /// </summary>
        /// <param name="isInnerTerm">is this an inner or outer term.</param>
        /// <returns>A token representing thing to expand.</returns>
        internal ExpandTermToken ParseSingleExpandTerm(bool isInnerTerm)
        {
            DebugUtils.CheckNoExternalCallers();
            this.IsSelect = false;
            this.RecurseEnter();

            PathSegmentToken property = this.ParseSelectExpandProperty();

            this.RecurseLeave();
            return this.BuildExpandTermToken(isInnerTerm, property);
        }

        /// <summary>
        /// Parses a select or expand term into a query token
        /// </summary>
        /// <returns>parsed query token</returns>
        private PathSegmentToken ParseSelectExpandProperty()
        {
            PathSegmentToken token = null;
            int currentDepth = 0;
            do
            {
                currentDepth++;
                if (currentDepth > this.MaxRecursiveDepth)
                {
                    throw new ODataException(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
                }

                this.Lexer.NextToken();

                // allow a single trailing slash for backwards compatibility with the WCF DS Server parser.
                if (currentDepth > 1 && this.Lexer.CurrentToken.Kind == ExpressionTokenKind.End)
                {
                    break;
                }

                token = this.ParseNext(token);
            }
            while (this.Lexer.CurrentToken.Kind == ExpressionTokenKind.Slash);

            return token;
        }

        /// <summary>
        /// Uses the ExpressionLexer to visit the next ExpressionToken, and delegates parsing of segments, type segments, identifiers, 
        /// and the star token to other methods.
        /// </summary>
        /// <param name="previousToken">Previously parsed QueryToken, or null if this is the first token.</param>
        /// <returns>A parsed QueryToken representing the next part of the expression.</returns>
        private PathSegmentToken ParseNext(PathSegmentToken previousToken)
        {
            if (this.Lexer.CurrentToken.Text.StartsWith("$", StringComparison.CurrentCulture))
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_SystemTokenInSelectExpand(this.Lexer.CurrentToken.Text, this.Lexer.ExpressionText));
            }
            else
            {
                return this.ParseSegment(previousToken);
            }
        }

        /// <summary>
        /// Parses a segment; a expression that is followed by a slash.
        /// </summary>
        /// <param name="parent">The parent of the segment node.</param>
        /// <returns>The lexical token representing the segment.</returns>
        private PathSegmentToken ParseSegment(PathSegmentToken parent)
        {
            string propertyName;
            if (this.Lexer.PeekNextToken().Kind == ExpressionTokenKind.Dot)
            {
                propertyName = this.Lexer.ReadDottedIdentifier(this.IsSelect);
            }
            else if (this.Lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
            {
                if (this.Lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash)
                {
                    throw new ODataException(ODataErrorStrings.ExpressionToken_IdentifierExpected(this.Lexer.Position));
                }

                propertyName = this.Lexer.CurrentToken.Text;
                this.Lexer.NextToken();
            }
            else
            {
                propertyName = this.Lexer.CurrentToken.GetIdentifier();
                this.Lexer.NextToken();
            }

            return new NonSystemToken(propertyName, null, parent);
        }

        /// <summary>
        /// Marks the fact that a recursive method was entered, and checks that the depth is allowed.
        /// </summary>
        private void RecurseEnter()
        {
            Debug.Assert(this.Lexer != null, "Trying to recurse without a lexer, nothing to parse without a lexer.");
            Debug.Assert(this.RecursionDepth <= this.MaxRecursiveDepth, "The recursion depth was already exceeded, we should have failed.");

            this.RecursionDepth++;
            if (this.RecursionDepth > this.MaxRecursiveDepth)
            {
                throw new ODataException(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
            }
        }

        /// <summary>
        /// Marks the fact that a recursive method is leaving.
        /// </summary>
        private void RecurseLeave()
        {
            Debug.Assert(this.Lexer != null, "Trying to recurse without a lexer, nothing to parse without a lexer.");
            Debug.Assert(this.RecursionDepth > 0, "Decreasing recursion depth below zero, imbalanced recursion calls.");
            this.RecursionDepth--;
        }

        /// <summary>
        /// Build the list of expand options
        /// Depends on whether options are allowed or not.
        /// </summary>
        /// <param name="isInnerTerm">is this an inner expand term</param>
        /// <param name="pathToken">the current level token, as a PathToken</param>
        /// <returns>An expand term token based on the path token.</returns>
        private ExpandTermToken BuildExpandTermToken(bool isInnerTerm, PathSegmentToken pathToken)
        {
            DebugUtils.CheckNoExternalCallers();
            if (this.IsNotEndOfTerm(false))
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.Lexer.ExpressionText));
            }

            return new ExpandTermToken(pathToken);
        }

        /// <summary>
        /// determine whether we're at the end of a select or expand term
        /// </summary>
        /// <param name="isInnerTerm">flag to indicate whether this is an outer or inner select.</param>
        /// <returns>true if we are not at the end of a select term.</returns>
        private bool IsNotEndOfTerm(bool isInnerTerm)
        {
            DebugUtils.CheckNoExternalCallers();
            return this.Lexer.CurrentToken.Kind != ExpressionTokenKind.End &&
                   this.Lexer.CurrentToken.Kind != ExpressionTokenKind.Comma;
        }
    }
}
