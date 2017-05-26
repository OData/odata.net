//---------------------------------------------------------------------
// <copyright file="SearchParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Parser which consumes the $search expression and produces the lexical object model.
    /// </summary>
    internal sealed class SearchParser
    {
        /// <summary>
        /// The maximum number of recursion nesting allowed.
        /// </summary>
        private readonly int maxDepth;

        /// <summary>
        /// The current recursion depth.
        /// </summary>
        private int recursionDepth;

        /// <summary>
        /// The lexer being used for the parsing.
        /// </summary>
        private ExpressionLexer lexer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        internal SearchParser(int maxDepth)
        {
            Debug.Assert(maxDepth >= 0, "maxDepth >= 0");

            this.maxDepth = maxDepth;
        }

        /// <summary>
        /// Parse expression text into Token.
        /// </summary>
        /// <param name="expressionText">The expression string to Parse.</param>
        /// <returns>The lexical token representing the expression text.</returns>
        internal QueryToken ParseSearch(string expressionText)
        {
            Debug.Assert(expressionText != null, "expressionText != null");

            this.recursionDepth = 0;
            this.lexer = new SearchLexer(expressionText);
            QueryToken result = this.ParseExpression();
            this.lexer.ValidateToken(ExpressionTokenKind.End);

            return result;
        }

        /// <summary>Creates an exception for a parse error.</summary>
        /// <param name="message">Message text.</param>
        /// <returns>A new Exception.</returns>
        private static Exception ParseError(string message)
        {
            return new ODataException(message);
        }

        /// <summary>
        /// Parses the expression.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseExpression()
        {
            this.RecurseEnter();
            QueryToken token = this.ParseLogicalOr();
            this.RecurseLeave();
            return token;
        }

        /// <summary>
        /// Parses the or operator.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseLogicalOr()
        {
            this.RecurseEnter();
            QueryToken left = this.ParseLogicalAnd();
            while (this.TokenIdentifierIs(ExpressionConstants.SearchKeywordOr))
            {
                this.lexer.NextToken();
                QueryToken right = this.ParseLogicalAnd();
                left = new BinaryOperatorToken(BinaryOperatorKind.Or, left, right);
            }

            this.RecurseLeave();
            return left;
        }

        /// <summary>
        /// Parses the and operator.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseLogicalAnd()
        {
            this.RecurseEnter();
            QueryToken left = this.ParseUnary();
            while (this.TokenIdentifierIs(ExpressionConstants.SearchKeywordAnd)
                || this.TokenIdentifierIs(ExpressionConstants.SearchKeywordNot)
                || this.lexer.CurrentToken.Kind == ExpressionTokenKind.StringLiteral
                || this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            {
                // Handle A NOT B, A (B)
                // Bypass only when next token is AND
                if (this.TokenIdentifierIs(ExpressionConstants.SearchKeywordAnd))
                {
                    this.lexer.NextToken();
                }

                QueryToken right = this.ParseUnary();
                left = new BinaryOperatorToken(BinaryOperatorKind.And, left, right);
            }

            this.RecurseLeave();
            return left;
        }

        /// <summary>
        /// Parses the -, not unary operators.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseUnary()
        {
            this.RecurseEnter();
            if (this.TokenIdentifierIs(ExpressionConstants.SearchKeywordNot))
            {
                this.lexer.NextToken();
                QueryToken operand = this.ParseUnary();

                this.RecurseLeave();
                return new UnaryOperatorToken(UnaryOperatorKind.Not, operand);
            }

            this.RecurseLeave();
            return this.ParsePrimary();
        }

        /// <summary>
        /// Parses the primary expressions.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParsePrimary()
        {
            QueryToken expr = null;
            this.RecurseEnter();

            switch (this.lexer.CurrentToken.Kind)
            {
                case ExpressionTokenKind.OpenParen:
                    expr = this.ParseParenExpression();
                    break;
                case ExpressionTokenKind.StringLiteral:
                    expr = new StringLiteralToken(this.lexer.CurrentToken.Text);
                    this.lexer.NextToken();
                    break;
                default:
                    throw new ODataException(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.RecurseLeave();
            return expr;
        }

        /// <summary>
        /// Parses parenthesized expressions.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseParenExpression()
        {
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();
            QueryToken result = this.ParseExpression();
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrOperatorExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();
            return result;
        }


        /// <summary>
        /// Checks that the current token has the specified identifier.
        /// </summary>
        /// <param name="id">Identifier to check.</param>
        /// <returns>true if the current token is an identifier with the specified text.</returns>
        private bool TokenIdentifierIs(string id)
        {
            return this.lexer.CurrentToken.IdentifierIs(id, false);
        }

        /// <summary>
        /// Marks the fact that a recursive method was entered, and checks that the depth is allowed.
        /// </summary>
        private void RecurseEnter()
        {
            Debug.Assert(this.lexer != null, "Trying to recurse without a lexer, nothing to parse without a lexer.");
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
            Debug.Assert(this.lexer != null, "Trying to recurse without a lexer, nothing to parse without a lexer.");
            Debug.Assert(this.recursionDepth > 0, "Decreasing recursion depth below zero, imbalanced recursion calls.");

            this.recursionDepth--;
        }
    }
}