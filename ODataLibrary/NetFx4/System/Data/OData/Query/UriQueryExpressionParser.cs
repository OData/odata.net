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

namespace System.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Parser which consumes the query expression ($filter, $orderby) and produces the lexical object model.
    /// </summary>
    internal sealed class UriQueryExpressionParser
    {
        /// <summary>
        /// Empty list of arguments.
        /// </summary>
        private static QueryToken[] EmptyArguments = new QueryToken[0];

        /// <summary>
        /// The maximum number of recursion nesting allowed.
        /// </summary>
        private int maxDepth;

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
        /// Parses a literal.
        /// </summary>
        /// <param name="lexer">The lexer to use.</param>
        /// <returns>The literal query token or null if something else was found.</returns>
        internal static LiteralQueryToken TryParseLiteral(ExpressionLexer lexer)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(lexer != null, "lexer != null");

            switch (lexer.CurrentToken.Kind)
            {
                case ExpressionTokenKind.BooleanLiteral:
                    return ParseTypedLiteral(lexer, typeof(bool), EdmConstants.EdmBooleanTypeName);
                case ExpressionTokenKind.DateTimeLiteral:
                    return ParseTypedLiteral(lexer, typeof(DateTime), EdmConstants.EdmDateTimeTypeName);
                case ExpressionTokenKind.DecimalLiteral:
                    return ParseTypedLiteral(lexer, typeof(decimal), EdmConstants.EdmDecimalTypeName);
                case ExpressionTokenKind.NullLiteral:
                    return ParseNullLiteral(lexer);
                case ExpressionTokenKind.StringLiteral:
                    return ParseTypedLiteral(lexer, typeof(string), EdmConstants.EdmStringTypeName);
                case ExpressionTokenKind.Int64Literal:
                    return ParseTypedLiteral(lexer, typeof(Int64), EdmConstants.EdmInt64TypeName);
                case ExpressionTokenKind.IntegerLiteral:
                    return ParseTypedLiteral(lexer, typeof(Int32), EdmConstants.EdmInt32TypeName);
                case ExpressionTokenKind.DoubleLiteral:
                    return ParseTypedLiteral(lexer, typeof(double), EdmConstants.EdmDoubleTypeName);
                case ExpressionTokenKind.SingleLiteral:
                    return ParseTypedLiteral(lexer, typeof(Single), EdmConstants.EdmSingleTypeName);
                case ExpressionTokenKind.GuidLiteral:
                    return ParseTypedLiteral(lexer, typeof(Guid), EdmConstants.EdmGuidTypeName);
                case ExpressionTokenKind.BinaryLiteral:
                    return ParseTypedLiteral(lexer, typeof(byte[]), EdmConstants.EdmBinaryTypeName);
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
            this.lexer = new ExpressionLexer(filter);

            QueryToken result = this.ParseExpression();
            this.lexer.ValidateToken(ExpressionTokenKind.End);

            return result;
        }

        /// <summary>
        /// Parses the $orderby expression.
        /// </summary>
        /// <param name="orderBy">The $orderby expression string to parse.</param>
        /// <returns>The enumeraion of lexical tokens representing order by tokens.</returns>
        internal IEnumerable<OrderByQueryToken> ParseOrderBy(string orderBy)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(orderBy != null, "orderBy != null");

            this.recursionDepth = 0;
            this.lexer = new ExpressionLexer(orderBy);

            List<OrderByQueryToken> orderByTokens = new List<OrderByQueryToken>();
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

                OrderByQueryToken orderByToken = new OrderByQueryToken()
                {
                    Direction = ascending ? OrderByDirection.Ascending : OrderByDirection.Descending,
                    Expression = expression
                };
                orderByTokens.Add(orderByToken);
                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Comma)
                {
                    break;
                }

                this.lexer.NextToken();
            }

            this.lexer.ValidateToken(ExpressionTokenKind.End);

            return new ReadOnlyCollection<OrderByQueryToken>(orderByTokens);
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
        /// <param name="targetType">Expected type to be parsed.</param>
        /// <param name="targetTypeName">The EDM type name of the expected type to be parsed.</param>
        /// <returns>The literal token produced by building the given literal.</returns>
        private static LiteralQueryToken ParseTypedLiteral(ExpressionLexer lexer, Type targetType, string targetTypeName)
        {
            Debug.Assert(lexer != null, "lexer != null");

            object targetValue;
            if (!UriPrimitiveTypeParser.TryUriStringToPrimitive(lexer.CurrentToken.Text, targetType, out targetValue))
            {
                string message = Strings.UriQueryExpressionParser_UnrecognizedLiteral(
                    targetTypeName,
                    lexer.CurrentToken.Text,
                    lexer.CurrentToken.Position,
                    lexer.ExpressionText);
                throw ParseError(message);
            }

            LiteralQueryToken result = new LiteralQueryToken()
            {
                Value = targetValue,
                OriginalText = lexer.CurrentToken.Text
            };
            lexer.NextToken();
            return result;
        }

        /// <summary>
        /// Parses null literals.
        /// </summary>
        /// <param name="lexer">The lexer to use.</param>
        /// <returns>The literal token produced by building the given literal.</returns>
        private static LiteralQueryToken ParseNullLiteral(ExpressionLexer lexer)
        {
            Debug.Assert(lexer != null, "lexer != null");
            Debug.Assert(lexer.CurrentToken.Kind == ExpressionTokenKind.NullLiteral, "this.lexer.CurrentToken.Kind == ExpressionTokenKind.NullLiteral");

            LiteralQueryToken result = new LiteralQueryToken()
            {
                Value = null,
                OriginalText = lexer.CurrentToken.Text
            };

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
                left = new BinaryOperatorQueryToken()
                {
                    OperatorKind = BinaryOperatorKind.Or,
                    Left = left,
                    Right = right
                };
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
                left = new BinaryOperatorQueryToken()
                {
                    OperatorKind = BinaryOperatorKind.And,
                    Left = left,
                    Right = right
                };
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
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.UriQueryExpressionParser_ParseComparison));
                }

                this.lexer.NextToken();
                QueryToken right = this.ParseAdditive();
                left = new BinaryOperatorQueryToken()
                {
                    OperatorKind = binaryOperatorKind,
                    Left = left,
                    Right = right
                };
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
                left = new BinaryOperatorQueryToken()
                {
                    OperatorKind = binaryOperatorKind,
                    Left = left,
                    Right = right
                };
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
                left = new BinaryOperatorQueryToken()
                {
                    OperatorKind = binaryOperatorKind,
                    Left = left,
                    Right = right
                };
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
                if (operatorToken.Kind == ExpressionTokenKind.Minus && (ExpressionLexer.IsNumeric(this.lexer.CurrentToken.Kind)))
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
                return new UnaryOperatorQueryToken()
                {
                    OperatorKind = unaryOperatorKind,
                    Operand = operand
                };
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
            QueryToken expr = this.ParsePrimaryStart();
            while (true)
            {
                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Slash)
                {
                    this.lexer.NextToken();
                    expr = this.ParseMemberAccess(expr);
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
                    return this.ParseIdentifier();
                case ExpressionTokenKind.OpenParen:
                    return this.ParseParenExpression();
                default:
                    QueryToken primitiveLiteralToken = TryParseLiteral(this.lexer);
                    if (primitiveLiteralToken == null)
                    {
                        throw ParseError(Strings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                    }

                    return primitiveLiteralToken;
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
                throw ParseError(Strings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();
            QueryToken result = this.ParseExpression();
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                throw ParseError(Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();
            return result;
        }

        /// <summary>
        /// Parses identifiers.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseIdentifier()
        {
            this.lexer.ValidateToken(ExpressionTokenKind.Identifier);

            // An open paren here would indicate calling a method in regular C# syntax.
            bool identifierIsFunction = this.lexer.PeekNextToken().Kind == ExpressionTokenKind.OpenParen;
            if (identifierIsFunction)
            {
                return this.ParseIdentifierAsFunction();
            }
            else
            {
                return this.ParseMemberAccess(null);
            }
        }

        /// <summary>
        /// Parses identifiers which have been recognizes as function calls.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseIdentifierAsFunction()
        {
            ExpressionToken functionToken = this.lexer.CurrentToken;
            Debug.Assert(functionToken.Kind == ExpressionTokenKind.Identifier, "Only identifier tokens can be treated as function calls.");

            this.lexer.NextToken();
            QueryToken[] arguments = this.ParseArgumentList();

            return new FunctionCallQueryToken()
            {
                Name = functionToken.Text,
                Arguments = arguments
            };
        }

        /// <summary>
        /// Parses argument lists.
        /// </summary>
        /// <returns>The lexical tokens representing the arguments.</returns>
        private QueryToken[] ParseArgumentList()
        {
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(Strings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();
            QueryToken[] arguments = this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen ? this.ParseArguments() : EmptyArguments;
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                throw ParseError(Strings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();
            return arguments;
        }

        /// <summary>
        /// Parses comma-separated arguments.
        /// </summary>
        /// <returns>The lexical tokens representing the arguments.</returns>
        private QueryToken[] ParseArguments()
        {
            List<QueryToken> argList = new List<QueryToken>();
            while (true)
            {
                argList.Add(this.ParseExpression());
                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Comma)
                {
                    break;
                }

                this.lexer.NextToken();
            }

            return argList.ToArray();
        }

        /// <summary>
        /// Parses member access.
        /// </summary>
        /// <param name="instance">Instance being accessed.</param>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseMemberAccess(QueryToken instance)
        {
            string propertyName = this.lexer.CurrentToken.GetIdentifier();
            this.lexer.NextToken();

            return new PropertyAccessQueryToken()
            {
                Name = propertyName,
                Parent = instance
            };
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
                throw new ODataException(Strings.UriQueryExpressionParser_TooDeep);
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
