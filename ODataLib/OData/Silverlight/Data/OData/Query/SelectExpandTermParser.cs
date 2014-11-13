//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Object that knows how to parse a single term within a select expression. That is, a path to a property, 
    /// a wildcard, operation name, etc.
    /// </summary>
    internal abstract class SelectExpandTermParser : ISelectExpandTermParser
    {
        /// <summary>
        /// Lexer used to parse an expression.
        /// </summary>
        public ExpressionLexer Lexer;

        /// <summary>
        /// are we parsing select.
        /// </summary>
        private bool isSelect;

        /// <summary>
        /// the maximum allowable recursive depth.
        /// </summary>
        private int maxDepth;

        /// <summary>
        /// The current recursion depth.
        /// </summary>
        private int recursionDepth;

        /// <summary>
        /// Create a SelectExpandTermParser
        /// </summary>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <param name="maxDepth">the maximum recursive depth</param>
        protected SelectExpandTermParser(string clauseToParse, int maxDepth)
        {
            this.maxDepth = maxDepth;
            this.recursionDepth = 0;
            this.Lexer = clauseToParse != null ? new ExpressionLexer(clauseToParse, false, true) : null;
        }

        /// <summary>
        /// The maximum recursive depth.
        /// </summary>
        public int MaxDepth 
        {
            get { return this.maxDepth; }
        }

        /// <summary>
        /// Parses a full $select expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public SelectToken ParseSelect()
        {
            this.isSelect = true;
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
        public ExpandToken ParseExpand()
        {
            this.isSelect = false;
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
        public PathSegmentToken ParseSingleSelectTerm(bool isInnerTerm)
        {
            this.isSelect = true;         
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
        public ExpandTermToken ParseSingleExpandTerm(bool isInnerTerm)
        {
            this.isSelect = false;
            this.RecurseEnter();

            PathSegmentToken property  = this.ParseSelectExpandProperty();

            this.RecurseLeave();
            return this.BuildExpandTermToken(isInnerTerm, property);
        }

        /// <summary>
        /// Build the list of expand options
        /// Depends on whether options are allowed or not.
        /// </summary>
        /// <param name="isInnerTerm">is this an inner expand term</param>
        /// <param name="pathToken">the current level token, as a PathToken</param>
        /// <returns>An expand term token based on the path token.</returns>
        internal abstract ExpandTermToken BuildExpandTermToken(bool isInnerTerm, PathSegmentToken pathToken);

        /// <summary>
        /// determine whether we're at the end of a select or expand term
        /// </summary>
        /// <param name="isInnerTerm">flag to indicate whether this is an outer or inner select.</param>
        /// <returns>true if we are not at the end of a select term.</returns>
        internal abstract bool IsNotEndOfTerm(bool isInnerTerm);

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
                if (currentDepth > this.maxDepth)
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
                propertyName = this.Lexer.ReadDottedIdentifier(this.isSelect);
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
            Debug.Assert(this.recursionDepth <= this.maxDepth, "The recursion depth was already exceeded, we should have failed.");

            this.recursionDepth++;
            if (this.recursionDepth > this.maxDepth)
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
            Debug.Assert(this.recursionDepth > 0, "Decreasing recursion depth below zero, imbalanced recursion calls.");
            this.recursionDepth--;
        }
    }
}
