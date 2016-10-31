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
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Parser which consumes the query expression ($filter, $orderby) and produces the lexical object model.
    /// </summary>
    internal sealed class UriQueryExpressionParser
    {
        /// <summary>
        /// The maximum number of recursion nesting allowed.
        /// </summary>
        private readonly int maxDepth;

        /// <summary>
        /// Set of parsed parameters
        /// </summary>
        private readonly HashSet<string> parameters = new HashSet<string>(StringComparer.Ordinal)
        {
            ExpressionConstants.It
        };

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
        internal UriQueryExpressionParser(int maxDepth)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(maxDepth >= 0, "maxDepth >= 0");

            this.maxDepth = maxDepth;
        }

        /// <summary>
        /// Delegate for a function that parses an expression and translates it into a QueryToken.
        /// </summary>
        /// <returns>A QueryToken</returns>
        internal delegate QueryToken Parser();

        /// <summary>
        /// Parses a literal.
        /// </summary>
        /// <param name="lexer">The lexer to use.</param>
        /// <returns>The literal query token or null if something else was found.</returns>
        internal static LiteralToken TryParseLiteral(ExpressionLexer lexer)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(lexer != null, "lexer != null");

            switch (lexer.CurrentToken.Kind)
            {
                case ExpressionTokenKind.BooleanLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetBoolean(false), Microsoft.Data.OData.Metadata.EdmConstants.EdmBooleanTypeName);
                case ExpressionTokenKind.DateTimeLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTime, false), Microsoft.Data.OData.Metadata.EdmConstants.EdmDateTimeTypeName);
                case ExpressionTokenKind.DateTimeOffsetLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false), Microsoft.Data.OData.Metadata.EdmConstants.EdmDateTimeOffsetTypeName);
                case ExpressionTokenKind.TimeLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Time, false), Microsoft.Data.OData.Metadata.EdmConstants.EdmTimeTypeName);
                case ExpressionTokenKind.DecimalLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetDecimal(false), Microsoft.Data.OData.Metadata.EdmConstants.EdmDecimalTypeName);
                case ExpressionTokenKind.NullLiteral:
                    return ParseNullLiteral(lexer);
                case ExpressionTokenKind.StringLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetString(true), Microsoft.Data.OData.Metadata.EdmConstants.EdmStringTypeName);
                case ExpressionTokenKind.Int64Literal:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetInt64(false), Microsoft.Data.OData.Metadata.EdmConstants.EdmInt64TypeName);
                case ExpressionTokenKind.IntegerLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetInt32(false), Microsoft.Data.OData.Metadata.EdmConstants.EdmInt32TypeName);
                case ExpressionTokenKind.DoubleLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetDouble(false), Microsoft.Data.OData.Metadata.EdmConstants.EdmDoubleTypeName);
                case ExpressionTokenKind.SingleLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetSingle(false), Microsoft.Data.OData.Metadata.EdmConstants.EdmSingleTypeName);
                case ExpressionTokenKind.GuidLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetGuid(false), Microsoft.Data.OData.Metadata.EdmConstants.EdmGuidTypeName);
                case ExpressionTokenKind.BinaryLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetBinary(true), Microsoft.Data.OData.Metadata.EdmConstants.EdmBinaryTypeName);
                case ExpressionTokenKind.GeographyLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false), Microsoft.Data.OData.Metadata.EdmConstants.EdmGeographyTypeName);
                case ExpressionTokenKind.GeometryLiteral:
                    return ParseTypedLiteral(lexer, EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, false), Microsoft.Data.OData.Metadata.EdmConstants.EdmGeometryTypeName);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Parses the $filter expression.
        /// </summary>
        /// <param name="filter">The $filter expression string to parse.</param>
        /// <returns>The lexical token representing the filter.</returns>
        internal QueryToken ParseFilter(string filter)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(filter != null, "filter != null");

            this.recursionDepth = 0;
            this.lexer = CreateLexerForFilterOrOrderByExpression(filter);
            QueryToken result = this.ParseExpression();
            this.lexer.ValidateToken(ExpressionTokenKind.End);

            return result;
        }

        /// <summary>
        /// Parses the $orderby expression.
        /// </summary>
        /// <param name="orderBy">The $orderby expression string to parse.</param>
        /// <returns>The enumeraion of lexical tokens representing order by tokens.</returns>
        internal IEnumerable<OrderByToken> ParseOrderBy(string orderBy)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(orderBy != null, "orderBy != null");

            this.recursionDepth = 0;
            this.lexer = CreateLexerForFilterOrOrderByExpression(orderBy);

            List<OrderByToken> orderByTokens = new List<OrderByToken>();
            while (true)
            {
                QueryToken expression = this.ParseExpression();
                bool ascending = true;
                if (this.TokenIdentifierIs(ExpressionConstants.KeywordAscending))
                {
                    this.lexer.NextToken();
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordDescending))
                {
                    this.lexer.NextToken();
                    ascending = false;
                }

                OrderByToken orderByToken = new OrderByToken(expression, ascending ? OrderByDirection.Ascending : OrderByDirection.Descending);
                orderByTokens.Add(orderByToken);
                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Comma)
                {
                    break;
                }

                this.lexer.NextToken();
            }

            this.lexer.ValidateToken(ExpressionTokenKind.End);

            return new ReadOnlyCollection<OrderByToken>(orderByTokens);
        }

        /// <summary>
        /// Creates a new <see cref="ExpressionLexer"/> for the given filter or orderby expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The lexer for the expression, which will have already moved to the first token.</returns>
        private static ExpressionLexer CreateLexerForFilterOrOrderByExpression(string expression)
        {
            return new ExpressionLexer(expression, true /*moveToFirstToken*/, false /*useSemicolonDelimeter*/, true /*allowAliases*/);
        }

        /// <summary>Creates an exception for a parse error.</summary>
        /// <param name="message">Message text.</param>
        /// <returns>A new Exception.</returns>
        private static Exception ParseError(string message)
        {
            return new ODataException(message);
        }

        /// <summary>
        /// Parses typed literals.
        /// </summary>
        /// <param name="lexer">The lexer to use.</param>
        /// <param name="targetTypeReference">Expected type to be parsed.</param>
        /// <param name="targetTypeName">The EDM type name of the expected type to be parsed.</param>
        /// <returns>The literal token produced by building the given literal.</returns>
        private static LiteralToken ParseTypedLiteral(ExpressionLexer lexer, IEdmPrimitiveTypeReference targetTypeReference, string targetTypeName)
        {
            Debug.Assert(lexer != null, "lexer != null");

            object targetValue;
            if (!UriPrimitiveTypeParser.TryUriStringToPrimitive(lexer.CurrentToken.Text, targetTypeReference, out targetValue))
            {
                string message = ODataErrorStrings.UriQueryExpressionParser_UnrecognizedLiteral(
                    targetTypeName,
                    lexer.CurrentToken.Text,
                    lexer.CurrentToken.Position,
                    lexer.ExpressionText);
                throw ParseError(message);
            }

            LiteralToken result = new LiteralToken(targetValue, lexer.CurrentToken.Text);
            lexer.NextToken();
            return result;
        }

        /// <summary>
        /// Parses null literals.
        /// </summary>
        /// <param name="lexer">The lexer to use.</param>
        /// <returns>The literal token produced by building the given literal.</returns>
        private static LiteralToken ParseNullLiteral(ExpressionLexer lexer)
        {
            Debug.Assert(lexer != null, "lexer != null");
            Debug.Assert(lexer.CurrentToken.Kind == ExpressionTokenKind.NullLiteral, "this.lexer.CurrentToken.InternalKind == ExpressionTokenKind.NullLiteral");

            LiteralToken result = new LiteralToken(null, lexer.CurrentToken.Text);

            lexer.NextToken();
            return result;
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
            while (this.TokenIdentifierIs(ExpressionConstants.KeywordOr))
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
            QueryToken left = this.ParseComparison();
            while (this.TokenIdentifierIs(ExpressionConstants.KeywordAnd))
            {
                this.lexer.NextToken();
                QueryToken right = this.ParseComparison();
                left = new BinaryOperatorToken(BinaryOperatorKind.And, left, right);
            }

            this.RecurseLeave();
            return left;
        }

        /// <summary>
        /// Parses the eq, ne, lt, gt, le, ge operators.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseComparison()
        {
            this.RecurseEnter();
            QueryToken left = this.ParseAdditive();
            while (this.lexer.CurrentToken.IsComparisonOperator)
            {
                BinaryOperatorKind binaryOperatorKind;
                switch (this.lexer.CurrentToken.Text)
                {
                    case ExpressionConstants.KeywordEqual:
                        binaryOperatorKind = BinaryOperatorKind.Equal;
                        break;
                    case ExpressionConstants.KeywordNotEqual:
                        binaryOperatorKind = BinaryOperatorKind.NotEqual;
                        break;
                    case ExpressionConstants.KeywordGreaterThan:
                        binaryOperatorKind = BinaryOperatorKind.GreaterThan;
                        break;
                    case ExpressionConstants.KeywordGreaterThanOrEqual:
                        binaryOperatorKind = BinaryOperatorKind.GreaterThanOrEqual;
                        break;
                    case ExpressionConstants.KeywordLessThan:
                        binaryOperatorKind = BinaryOperatorKind.LessThan;
                        break;
                    case ExpressionConstants.KeywordLessThanOrEqual:
                        binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
                        break;
                    default:
                        throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.UriQueryExpressionParser_ParseComparison));
                }

                this.lexer.NextToken();
                QueryToken right = this.ParseAdditive();
                left = new BinaryOperatorToken(binaryOperatorKind, left, right);
            }

            this.RecurseLeave();
            return left;
        }

        /// <summary>
        /// Parses the add, sub operators.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseAdditive()
        {
            this.RecurseEnter();
            QueryToken left = this.ParseMultiplicative();
            while (this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordAdd) ||
                this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordSub))
            {
                BinaryOperatorKind binaryOperatorKind;
                if (this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordAdd))
                {
                    binaryOperatorKind = BinaryOperatorKind.Add;
                }
                else
                {
                    Debug.Assert(this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordSub), "Was a new binary operator added?");
                    binaryOperatorKind = BinaryOperatorKind.Subtract;
                }

                this.lexer.NextToken();
                QueryToken right = this.ParseMultiplicative();
                left = new BinaryOperatorToken(binaryOperatorKind, left, right);
            }

            this.RecurseLeave();
            return left;
        }

        /// <summary>
        /// Parses the mul, div, mod operators.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseMultiplicative()
        {
            this.RecurseEnter();
            QueryToken left = this.ParseUnary();
            while (this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordMultiply) ||
                this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordDivide) ||
                this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordModulo))
            {
                BinaryOperatorKind binaryOperatorKind;
                if (this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordMultiply))
                {
                    binaryOperatorKind = BinaryOperatorKind.Multiply;
                }
                else if (this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordDivide))
                {
                    binaryOperatorKind = BinaryOperatorKind.Divide;
                }
                else
                {
                    Debug.Assert(this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordModulo), "Was a new binary operator added?");
                    binaryOperatorKind = BinaryOperatorKind.Modulo;
                }

                this.lexer.NextToken();
                QueryToken right = this.ParseUnary();
                left = new BinaryOperatorToken(binaryOperatorKind, left, right);
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
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Minus || this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.KeywordNot))
            {
                ExpressionToken operatorToken = this.lexer.CurrentToken;
                this.lexer.NextToken();
                if (operatorToken.Kind == ExpressionTokenKind.Minus && (ExpressionLexerUtils.IsNumeric(this.lexer.CurrentToken.Kind)))
                {
                    ExpressionToken numberLiteral = this.lexer.CurrentToken;
                    numberLiteral.Text = "-" + numberLiteral.Text;
                    numberLiteral.Position = operatorToken.Position;
                    this.lexer.CurrentToken = numberLiteral;
                    this.RecurseLeave();
                    return this.ParsePrimary();
                }

                QueryToken operand = this.ParseUnary();
                UnaryOperatorKind unaryOperatorKind;
                if (operatorToken.Kind == ExpressionTokenKind.Minus)
                {
                    unaryOperatorKind = UnaryOperatorKind.Negate;
                }
                else
                {
                    Debug.Assert(operatorToken.IdentifierIs(ExpressionConstants.KeywordNot), "Was a new unary operator added?");
                    unaryOperatorKind = UnaryOperatorKind.Not;
                }

                this.RecurseLeave();
                return new UnaryOperatorToken(unaryOperatorKind, operand);
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
            this.RecurseEnter();
            QueryToken expr;
            if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash)
            {
                expr = this.ParseSegment(null);
            }
            else
            {
                expr = this.ParsePrimaryStart();
            }

            while (true)
            {
                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Slash)
                {
                    this.lexer.NextToken();
                    if (this.lexer.CurrentToken.Text == "any")
                    {
                        expr = this.ParseAny(expr);
                    }
                    else if (this.lexer.CurrentToken.Text == "all")
                    {
                        expr = this.ParseAll(expr);
                    }
                    else if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash)
                    {
                        expr = this.ParseSegment(expr);
                    }
                    else
                    {
                        IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this.ParseExpression));
                        expr = identifierTokenizer.ParseIdentifier(expr);
                    }
                }
                else
                {
                    break;
                }
            }

            this.RecurseLeave();
            return expr;
        }

        /// <summary>
        /// Handles the start of primary expressions.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParsePrimaryStart()
        {
            switch (this.lexer.CurrentToken.Kind)
            {
                case ExpressionTokenKind.Identifier:
                {
                    IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this.ParseExpression));
                    return identifierTokenizer.ParseIdentifier(null);
                }

                case ExpressionTokenKind.OpenParen:
                {
                    return this.ParseParenExpression();
                }
                    
                case ExpressionTokenKind.Star:
                {
                    IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this.ParseExpression));
                    return identifierTokenizer.ParseStarMemberAccess(null);
                }    

                default:
                {
                    QueryToken primitiveLiteralToken = TryParseLiteral(this.lexer);
                    if (primitiveLiteralToken == null)
                    {
                        throw ParseError(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                    }

                    return primitiveLiteralToken;
                }
            }
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
        /// Parses the Any portion of the query
        /// </summary>
        /// <param name="parent">The parent of the Any node.</param>
        /// <returns>The lexical token representing the Any query.</returns>
        private QueryToken ParseAny(QueryToken parent)
        {
            return this.ParseAnyAll(parent, true);
        }

        /// <summary>
        /// Parses the All portion of the query
        /// </summary>
        /// <param name="parent">The parent of the All node.</param>
        /// <returns>The lexical token representing the All query.</returns>
        private QueryToken ParseAll(QueryToken parent)
        {
            return this.ParseAnyAll(parent, false);
        }

        /// <summary>
        /// Parses the Any/All portion of the query
        /// </summary>
        /// <param name="parent">The parent of the Any/All node.</param>
        /// <param name="isAny">Denotes whether an Any or All is to be parsed.</param>
        /// <returns>The lexical token representing the Any/All query.</returns>
        private QueryToken ParseAnyAll(QueryToken parent, bool isAny)
        {
            this.lexer.NextToken();
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            // When faced with Any(), return the same thing as if you encountered Any(a : true)
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
            {
                this.lexer.NextToken();
                if (isAny)
                {
                    return new AnyToken(new LiteralToken(true, "True"), null, parent);
                }
                else
                {
                    return new AllToken(new LiteralToken(true, "True"), null, parent);
                }
            }

            string parameter = this.lexer.CurrentToken.GetIdentifier();
            if (!this.parameters.Add(parameter))
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_RangeVariableAlreadyDeclared(parameter));
            }

            // read the ':' separating the range variable from the expression.
            this.lexer.NextToken();
            this.lexer.ValidateToken(ExpressionTokenKind.Colon);

            this.lexer.NextToken();
            QueryToken expr = this.ParseExpression();
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            // forget about the range variable after parsing the expression for this lambda.
            this.parameters.Remove(parameter);

            this.lexer.NextToken();
            if (isAny)
            {
                return new AnyToken(expr, parameter, parent);
            }
            else
            {
                return new AllToken(expr, parameter, parent);
            }
        }

        /// <summary>
        /// Parses a segment.
        /// </summary>
        /// <param name="parent">The parent of the segment node.</param>
        /// <returns>The lexical token representing the segment.</returns>
        private QueryToken ParseSegment(QueryToken parent)
        {
            string propertyName = this.lexer.CurrentToken.GetIdentifier();
            this.lexer.NextToken();
            if (this.parameters.Contains(propertyName) && parent == null)
            {
                return new RangeVariableToken(propertyName);
            }

            return new InnerPathToken(propertyName, parent, null);
        }

        /// <summary>
        /// Checks that the current token has the specified identifier.
        /// </summary>
        /// <param name="id">Identifier to check.</param>
        /// <returns>true if the current token is an identifier with the specified text.</returns>
        private bool TokenIdentifierIs(string id)
        {
            return this.lexer.CurrentToken.IdentifierIs(id);
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
