//---------------------------------------------------------------------
// <copyright file="UriQueryExpressionParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Parsers.Common;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.Aggregation;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        /// List of supported $apply keywords
        /// </summary>
        private static readonly string supportedKeywords = string.Join("|", new string[] { ExpressionConstants.KeywordAggregate, ExpressionConstants.KeywordFilter, ExpressionConstants.KeywordGroupBy });

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
        /// Whether to allow case insensitive for builtin identifier.
        /// </summary>
        private bool enableCaseInsensitiveBuiltinIdentifier = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        internal UriQueryExpressionParser(int maxDepth, bool enableCaseInsensitiveBuiltinIdentifier = false)
        {
            Debug.Assert(maxDepth >= 0, "maxDepth >= 0");

            this.maxDepth = maxDepth;
            this.enableCaseInsensitiveBuiltinIdentifier = enableCaseInsensitiveBuiltinIdentifier;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        /// <param name="lexer">The ExpressionLexer containing text to be parsed.</param>
        internal UriQueryExpressionParser(int maxDepth, ExpressionLexer lexer)
        {
            Debug.Assert(maxDepth >= 0, "maxDepth >= 0");
            Debug.Assert(lexer != null, "lexer != null");
            this.maxDepth = maxDepth;
            this.lexer = lexer;
        }

        /// <summary>
        /// Delegate for a function that parses an expression and translates it into a QueryToken.
        /// </summary>
        /// <returns>A QueryToken</returns>
        internal delegate QueryToken Parser();

        /// <summary>
        /// Reference to the lexer.
        /// </summary>
        internal ExpressionLexer Lexer
        {
            get { return this.lexer; }
        }

        /// <summary>
        /// Parses a literal.
        /// </summary>
        /// <param name="lexer">The lexer to use.</param>
        /// <returns>The literal query token or null if something else was found.</returns>
        internal static LiteralToken TryParseLiteral(ExpressionLexer lexer)
        {
            Debug.Assert(lexer != null, "lexer != null");

            switch (lexer.CurrentToken.Kind)
            {
                case ExpressionTokenKind.BooleanLiteral:
                case ExpressionTokenKind.DateLiteral:
                case ExpressionTokenKind.DecimalLiteral:
                case ExpressionTokenKind.StringLiteral:
                case ExpressionTokenKind.Int64Literal:
                case ExpressionTokenKind.IntegerLiteral:
                case ExpressionTokenKind.DoubleLiteral:
                case ExpressionTokenKind.SingleLiteral:
                case ExpressionTokenKind.GuidLiteral:
                case ExpressionTokenKind.BinaryLiteral:
                case ExpressionTokenKind.GeographyLiteral:
                case ExpressionTokenKind.GeometryLiteral:
                case ExpressionTokenKind.QuotedLiteral:
                case ExpressionTokenKind.DurationLiteral:
                case ExpressionTokenKind.TimeOfDayLiteral:
                case ExpressionTokenKind.DateTimeOffsetLiteral:
                case ExpressionTokenKind.CustomTypeLiteral:
                    IEdmTypeReference literalEdmTypeReference = lexer.CurrentToken.GetLiteralEdmTypeReference();

                    // Why not using EdmTypeReference.FullName? (literalEdmTypeReference.FullName)
                    string edmConstantName = GetEdmConstantNames(literalEdmTypeReference);
                    return ParseTypedLiteral(lexer, literalEdmTypeReference, edmConstantName);

                case ExpressionTokenKind.BracketedExpression:
                    {
                        // TODO: need a BracketLiteralToken for real complex type vaule like [\"Barky\",\"Junior\"]  or {...}
                        LiteralToken result = new LiteralToken(lexer.CurrentToken.Text, lexer.CurrentToken.Text);
                        lexer.NextToken();
                        return result;
                    }

                case ExpressionTokenKind.NullLiteral:
                    return ParseNullLiteral(lexer);

                default:
                    return null;
            }
        }

        internal static string GetEdmConstantNames(IEdmTypeReference edmTypeReference)
        {
            Debug.Assert(edmTypeReference != null, "Cannot be null");

            switch (edmTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Boolean:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmBooleanTypeName;
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmTimeOfDayTypeName;
                case EdmPrimitiveTypeKind.Date:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmDateTypeName;
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmDateTimeOffsetTypeName;
                case EdmPrimitiveTypeKind.Duration:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmDurationTypeName;
                case EdmPrimitiveTypeKind.Decimal:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmDecimalTypeName;
                case EdmPrimitiveTypeKind.String:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmStringTypeName;
                case EdmPrimitiveTypeKind.Int64:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmInt64TypeName;
                case EdmPrimitiveTypeKind.Int32:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmInt32TypeName;
                case EdmPrimitiveTypeKind.Double:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmDoubleTypeName;
                case EdmPrimitiveTypeKind.Single:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmSingleTypeName;
                case EdmPrimitiveTypeKind.Guid:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmGuidTypeName;
                case EdmPrimitiveTypeKind.Binary:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmBinaryTypeName;
                case EdmPrimitiveTypeKind.Geography:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmGeographyTypeName;
                case EdmPrimitiveTypeKind.Geometry:
                    return Microsoft.OData.Core.Metadata.EdmConstants.EdmGeometryTypeName;
                default:
                    return edmTypeReference.Definition.FullTypeName();
            }
        }

        /// <summary>
        /// Parses the $filter expression.
        /// </summary>
        /// <param name="filter">The $filter expression string to parse.</param>
        /// <returns>The lexical token representing the filter.</returns>
        internal QueryToken ParseFilter(string filter)
        {
            return this.ParseExpressionText(filter);
        }

        internal IEnumerable<QueryToken> ParseApply(string apply)
        {
            Debug.Assert(apply != null, "apply != null");

            List<QueryToken> transformationTokens = new List<QueryToken>();

            if (string.IsNullOrEmpty(apply))
            {
                return transformationTokens;
            }

            this.recursionDepth = 0;
            this.lexer = CreateLexerForFilterOrOrderByOrApplyExpression(apply);

            while (true)
            {
                switch (this.lexer.CurrentToken.GetIdentifier())
                {
                    case ExpressionConstants.KeywordAggregate:
                        transformationTokens.Add(ParseAggregate());
                        break;
                    case ExpressionConstants.KeywordFilter:
                        transformationTokens.Add(ParseApplyFilter());
                        break;
                    case ExpressionConstants.KeywordGroupBy:
                        transformationTokens.Add(ParseGroupBy());
                        break;
                    default:
                        throw ParseError(ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(supportedKeywords, this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                }

                // '/' indicates there are more transformations
                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Slash)
                {
                    break;
                }

                this.lexer.NextToken();
            }

            this.lexer.ValidateToken(ExpressionTokenKind.End);

            return new ReadOnlyCollection<QueryToken>(transformationTokens);
        }

        // parses $apply aggregate tranformation (.e.g. aggregate(UnitPrice with sum as TotalUnitPrice)
        internal AggregateToken ParseAggregate()
        {
            Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordAggregate), "token identifier is aggregate");
            lexer.NextToken();

            // '('
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            // series of statements separates by commas
            var statements = new List<AggregateExpressionToken>();
            while (true)
            {
                statements.Add(this.ParseAggregateExpression());

                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Comma)
                {
                    break;
                }

                this.lexer.NextToken();
            }

            // ")"
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            return new AggregateToken(statements);
        }

        internal AggregateExpressionToken ParseAggregateExpression()
        {
            // expression
            var expression = this.ParseExpression();

            // "with" verb
            var verb = this.ParseAggregateWith();

            // "as" alias
            var alias = this.ParseAggregateAs();

            return new AggregateExpressionToken(expression, verb, alias.Text);
        }

        // parses $apply groupby tranformation (.e.g. groupby(ProductID, CategoryId, aggregate(UnitPrice with sum as TotalUnitPrice))
        internal GroupByToken ParseGroupBy()
        {
            Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordGroupBy), "token identifier is groupby");
            lexer.NextToken();

            // '('
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            // '('
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            // properties
            var properties = new List<EndPathToken>();
            while (true)
            {
                var expression = this.ParsePrimary() as EndPathToken;

                if (expression == null)
                {
                    throw ParseError(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                }

                properties.Add(expression);

                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Comma)
                {
                    break;
                }

                this.lexer.NextToken();
            }

            // ")"
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrOperatorExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            // optional child transformation
            ApplyTransformationToken transformationToken = null;

            // "," (comma)
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
            {
                this.lexer.NextToken();

                if (TokenIdentifierIs(ExpressionConstants.KeywordAggregate))
                {
                    transformationToken = this.ParseAggregate();
                }
                else
                {
                    throw ParseError(ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(ExpressionConstants.KeywordAggregate, this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                }
            }

            // ")"
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            return new GroupByToken(properties, transformationToken);
        }

        // parses $apply filter tranformation (.e.g. filter(ProductName eq 'Aniseed Syrup'))
        internal QueryToken ParseApplyFilter()
        {
            Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordFilter), "token identifier is filter");
            lexer.NextToken();

            // '(' expression ')'
            return this.ParseParenExpression();
        }

        /// <summary>
        /// Parse expression text into Token.
        /// </summary>
        /// <param name="expressionText">The expression string to Parse.</param>
        /// <returns>The lexical token representing the expression text.</returns>
        internal QueryToken ParseExpressionText(string expressionText)
        {
            Debug.Assert(expressionText != null, "expressionText != null");

            this.recursionDepth = 0;
            this.lexer = CreateLexerForFilterOrOrderByOrApplyExpression(expressionText);
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
            Debug.Assert(orderBy != null, "orderBy != null");

            this.recursionDepth = 0;
            this.lexer = CreateLexerForFilterOrOrderByOrApplyExpression(orderBy);

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
        /// Parses the expression.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        internal QueryToken ParseExpression()
        {
            this.RecurseEnter();
            QueryToken token = this.ParseLogicalOr();
            this.RecurseLeave();
            return token;
        }

        /// <summary>
        /// Creates a new <see cref="ExpressionLexer"/> for the given filter, orderby or apply expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The lexer for the expression, which will have already moved to the first token.</returns>
        private static ExpressionLexer CreateLexerForFilterOrOrderByOrApplyExpression(string expression)
        {
            return new ExpressionLexer(expression, true /*moveToFirstToken*/, false /*useSemicolonDelimeter*/, true /*parsingFunctionParameters*/);
        }

        /// <summary>Creates an exception for a parse error.</summary>
        /// <param name="message">Message text.</param>
        /// <returns>A new Exception.</returns>
        private static Exception ParseError(string message)
        {
            return new ODataException(message);
        }


        /// <summary>Creates an exception for a parse error.</summary>
        /// <param name="message">Message text.</param>
        /// <param name="parsingException">Type Parsing exception</param>
        /// <returns>A new Exception.</returns>
        private static Exception ParseError(string message, UriLiteralParsingException parsingException)
        {
            return new ODataException(message, parsingException);
        }

        /// <summary>
        /// Parses parameter alias into token.
        /// </summary>
        /// <param name="lexer">The lexer to use.</param>
        /// <returns>The parameter alias token.</returns>
        private static FunctionParameterAliasToken ParseParameterAlias(ExpressionLexer lexer)
        {
            Debug.Assert(lexer != null, "lexer != null");
            FunctionParameterAliasToken ret = new FunctionParameterAliasToken(lexer.CurrentToken.Text);
            lexer.NextToken();
            return ret;
        }

        /// <summary>
        /// Parses typed literals.
        /// </summary>
        /// <param name="lexer">The lexer to use.</param>
        /// <param name="targetTypeReference">Expected type to be parsed.</param>
        /// <param name="targetTypeName">The EDM type name of the expected type to be parsed.</param>
        /// <returns>The literal token produced by building the given literal.</returns>
        private static LiteralToken ParseTypedLiteral(ExpressionLexer lexer, IEdmTypeReference targetTypeReference, string targetTypeName)
        {
            Debug.Assert(lexer != null, "lexer != null");

            UriLiteralParsingException typeParsingException;
            object targetValue = DefaultUriLiteralParser.Instance.ParseUriStringToType(lexer.CurrentToken.Text, targetTypeReference, out typeParsingException);

            if (targetValue == null)
            {
                string message;

                if (typeParsingException == null)
                {
                    message = ODataErrorStrings.UriQueryExpressionParser_UnrecognizedLiteral(
                        targetTypeName,
                        lexer.CurrentToken.Text,
                        lexer.CurrentToken.Position,
                        lexer.ExpressionText);

                    throw ParseError(message);
                }
                else
                {
                    message = ODataErrorStrings.UriQueryExpressionParser_UnrecognizedLiteralWithReason(
                        targetTypeName,
                        lexer.CurrentToken.Text,
                        lexer.CurrentToken.Position,
                        lexer.ExpressionText,
                        typeParsingException.Message);

                    throw ParseError(message, typeParsingException);
                }
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
            while (true)
            {
                BinaryOperatorKind binaryOperatorKind;
                if (this.TokenIdentifierIs(ExpressionConstants.KeywordEqual))
                {
                    binaryOperatorKind = BinaryOperatorKind.Equal;
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordNotEqual))
                {
                    binaryOperatorKind = BinaryOperatorKind.NotEqual;
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordGreaterThan))
                {
                    binaryOperatorKind = BinaryOperatorKind.GreaterThan;
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordGreaterThanOrEqual))
                {
                    binaryOperatorKind = BinaryOperatorKind.GreaterThanOrEqual;
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordLessThan))
                {
                    binaryOperatorKind = BinaryOperatorKind.LessThan;
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordLessThanOrEqual))
                {
                    binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordHas))
                {
                    binaryOperatorKind = BinaryOperatorKind.Has;
                }
                else
                {
                    break;
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
            while (this.TokenIdentifierIs(ExpressionConstants.KeywordAdd) ||
                this.TokenIdentifierIs(ExpressionConstants.KeywordSub))
            {
                BinaryOperatorKind binaryOperatorKind;
                if (this.TokenIdentifierIs(ExpressionConstants.KeywordAdd))
                {
                    binaryOperatorKind = BinaryOperatorKind.Add;
                }
                else
                {
                    Debug.Assert(this.TokenIdentifierIs(ExpressionConstants.KeywordSub), "Was a new binary operator added?");
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
            while (this.TokenIdentifierIs(ExpressionConstants.KeywordMultiply) ||
                this.TokenIdentifierIs(ExpressionConstants.KeywordDivide) ||
                this.TokenIdentifierIs(ExpressionConstants.KeywordModulo))
            {
                BinaryOperatorKind binaryOperatorKind;
                if (this.TokenIdentifierIs(ExpressionConstants.KeywordMultiply))
                {
                    binaryOperatorKind = BinaryOperatorKind.Multiply;
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordDivide))
                {
                    binaryOperatorKind = BinaryOperatorKind.Divide;
                }
                else
                {
                    Debug.Assert(this.TokenIdentifierIs(ExpressionConstants.KeywordModulo), "Was a new binary operator added?");
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
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Minus || this.TokenIdentifierIs(ExpressionConstants.KeywordNot))
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
                    Debug.Assert(operatorToken.IdentifierIs(ExpressionConstants.KeywordNot, enableCaseInsensitiveBuiltinIdentifier), "Was a new unary operator added?");
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
                    if (this.TokenIdentifierIs(ExpressionConstants.KeywordAny))
                    {
                        expr = this.ParseAny(expr);
                    }
                    else if (this.TokenIdentifierIs(ExpressionConstants.KeywordAll))
                    {
                        expr = this.ParseAll(expr);
                    }
                    else if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash)
                    {
                        expr = this.ParseSegment(expr);
                    }
                    else
                    {
                        IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this));
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
                case ExpressionTokenKind.ParameterAlias:
                    {
                        return ParseParameterAlias(this.lexer);
                    }

                case ExpressionTokenKind.Identifier:
                    {
                        IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this));
                        return identifierTokenizer.ParseIdentifier(null);
                    }

                case ExpressionTokenKind.OpenParen:
                    {
                        return this.ParseParenExpression();
                    }

                case ExpressionTokenKind.Star:
                    {
                        IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this));
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

        private AggregationMethod ParseAggregateWith()
        {
            if (!TokenIdentifierIs(ExpressionConstants.KeywordWith))
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_WithExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            lexer.NextToken();

            AggregationMethod verb;

            switch (lexer.CurrentToken.GetIdentifier())
            {
                case ExpressionConstants.KeywordAverage:
                    verb = AggregationMethod.Average;
                    break;
                case ExpressionConstants.KeywordCountDistinct:
                    verb = AggregationMethod.CountDistinct;
                    break;
                case ExpressionConstants.KeywordMax:
                    verb = AggregationMethod.Max;
                    break;
                case ExpressionConstants.KeywordMin:
                    verb = AggregationMethod.Min;
                    break;
                case ExpressionConstants.KeywordSum:
                    verb = AggregationMethod.Sum;
                    break;
                default:
                    throw ParseError(ODataErrorStrings.UriQueryExpressionParser_UnrecognizedWithVerb(lexer.CurrentToken.GetIdentifier(), this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            lexer.NextToken();

            return verb;
        }

        private StringLiteralToken ParseAggregateAs()
        {
            if (!TokenIdentifierIs(ExpressionConstants.KeywordAs))
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_AsExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            lexer.NextToken();

            var alias = new StringLiteralToken(lexer.CurrentToken.Text);

            lexer.NextToken();

            return alias;
        }


        /// <summary>
        /// Checks that the current token has the specified identifier.
        /// </summary>
        /// <param name="id">Identifier to check.</param>
        /// <returns>true if the current token is an identifier with the specified text.</returns>
        private bool TokenIdentifierIs(string id)
        {
            return this.lexer.CurrentToken.IdentifierIs(id, enableCaseInsensitiveBuiltinIdentifier);
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