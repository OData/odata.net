//---------------------------------------------------------------------
// <copyright file="UriQueryExpressionParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.UriParser.Aggregation;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Parser which consumes the query expression ($filter, $orderby) and produces the lexical object model.
    /// </summary>
    public sealed class UriQueryExpressionParser
    {
        /// <summary>
        /// The maximum number of recursion nesting allowed.
        /// </summary>
        private readonly int maxDepth;

        /// <summary>
        /// List of supported $apply keywords
        /// </summary>
        private static readonly string supportedKeywords = string.Join("|", new string[] { ExpressionConstants.KeywordAggregate, ExpressionConstants.KeywordFilter, ExpressionConstants.KeywordGroupBy, ExpressionConstants.KeywordCompute, ExpressionConstants.KeywordExpand });

        /// <summary>
        /// Set of parsed parameters
        /// </summary>
        private readonly HashSet<string> parameters = new HashSet<string>(StringComparer.Ordinal)
        {
            ExpressionConstants.It,
            ExpressionConstants.This
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
        /// Whether to allow no-dollar query options.
        /// </summary>
        private bool enableNoDollarQueryOptions = false;

        /// <summary>
        /// Tracks the depth of aggregate expression recursion.
        /// </summary>
        private int parseAggregateExpressionDepth = 0;

        /// <summary>
        /// Tracks expression parents of aggregate expression recursion.
        /// </summary>
        private Stack<QueryToken> aggregateExpressionParents = new Stack<QueryToken>();

        /// <summary>
        /// Creates a UriQueryExpressionParser.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        public UriQueryExpressionParser(int maxDepth)
            : this(maxDepth, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        internal UriQueryExpressionParser(int maxDepth, bool enableCaseInsensitiveBuiltinIdentifier = false)
            : this(maxDepth, enableCaseInsensitiveBuiltinIdentifier, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        /// <param name="enableNoDollarQueryOptions">Whether to allow no-dollar query options.</param>
        internal UriQueryExpressionParser(int maxDepth, bool enableCaseInsensitiveBuiltinIdentifier = false, bool enableNoDollarQueryOptions = false)
        {
            Debug.Assert(maxDepth >= 0, "maxDepth >= 0");

            this.maxDepth = maxDepth;
            this.enableCaseInsensitiveBuiltinIdentifier = enableCaseInsensitiveBuiltinIdentifier;
            this.enableNoDollarQueryOptions = enableNoDollarQueryOptions;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        /// <param name="lexer">The ExpressionLexer containing text to be parsed.</param>
        internal UriQueryExpressionParser(int maxDepth, ExpressionLexer lexer) : this(maxDepth)
        {
            Debug.Assert(lexer != null, "lexer != null");
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
        /// Reference to the enableCaseInsensitiveBuiltinIdentifier.
        /// </summary>
        internal bool EnableCaseInsensitiveBuiltinIdentifier
        {
            get { return this.enableCaseInsensitiveBuiltinIdentifier; }
        }

        /// <summary>
        /// Reference to the enableNoDollarQueryOptions.
        /// </summary>
        internal bool EnableNoDollarQueryOptions
        {
            get { return this.enableNoDollarQueryOptions; }
        }

        /// <summary>
        /// Gets if this parser is currently within an aggregate expression parsing stack.
        /// </summary>
        private bool IsInAggregateExpression
        {
            get
            {
                return this.parseAggregateExpressionDepth > 0;
            }
        }

        /// <summary>
        /// Parses the $filter expression.
        /// </summary>
        /// <param name="filter">The $filter expression string to parse.</param>
        /// <returns>The lexical token representing the filter.</returns>
        public QueryToken ParseFilter(string filter)
        {
            return this.ParseExpressionText(filter);
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

                case ExpressionTokenKind.BracedExpression:
                case ExpressionTokenKind.BracketedExpression:
                case ExpressionTokenKind.ParenthesesExpression:
                    {
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
                    return Microsoft.OData.Metadata.EdmConstants.EdmBooleanTypeName;
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return Microsoft.OData.Metadata.EdmConstants.EdmTimeOfDayTypeName;
                case EdmPrimitiveTypeKind.Date:
                    return Microsoft.OData.Metadata.EdmConstants.EdmDateTypeName;
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return Microsoft.OData.Metadata.EdmConstants.EdmDateTimeOffsetTypeName;
                case EdmPrimitiveTypeKind.Duration:
                    return Microsoft.OData.Metadata.EdmConstants.EdmDurationTypeName;
                case EdmPrimitiveTypeKind.Decimal:
                    return Microsoft.OData.Metadata.EdmConstants.EdmDecimalTypeName;
                case EdmPrimitiveTypeKind.String:
                    return Microsoft.OData.Metadata.EdmConstants.EdmStringTypeName;
                case EdmPrimitiveTypeKind.Int64:
                    return Microsoft.OData.Metadata.EdmConstants.EdmInt64TypeName;
                case EdmPrimitiveTypeKind.Int32:
                    return Microsoft.OData.Metadata.EdmConstants.EdmInt32TypeName;
                case EdmPrimitiveTypeKind.Double:
                    return Microsoft.OData.Metadata.EdmConstants.EdmDoubleTypeName;
                case EdmPrimitiveTypeKind.Single:
                    return Microsoft.OData.Metadata.EdmConstants.EdmSingleTypeName;
                case EdmPrimitiveTypeKind.Guid:
                    return Microsoft.OData.Metadata.EdmConstants.EdmGuidTypeName;
                case EdmPrimitiveTypeKind.Binary:
                    return Microsoft.OData.Metadata.EdmConstants.EdmBinaryTypeName;
                case EdmPrimitiveTypeKind.Geography:
                    return Microsoft.OData.Metadata.EdmConstants.EdmGeographyTypeName;
                case EdmPrimitiveTypeKind.Geometry:
                    return Microsoft.OData.Metadata.EdmConstants.EdmGeometryTypeName;
                default:
                    return edmTypeReference.Definition.FullTypeName();
            }
        }

        // parses $apply compute expression (.e.g. compute(UnitPrice mul SalesPrice as computePrice)
        internal ComputeToken ParseCompute()
        {
            Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordCompute), "token identifier is compute");

            lexer.NextToken();

            // '('
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            lexer.NextToken();

            List<ComputeExpressionToken> transformationTokens = new List<ComputeExpressionToken>();

            while (true)
            {
                ComputeExpressionToken computed = this.ParseComputeExpression();
                transformationTokens.Add(computed);
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

            return new ComputeToken(transformationTokens);
        }

        // parses $compute query option.
        internal ComputeToken ParseCompute(string compute)
        {
            Debug.Assert(compute != null, "compute != null");

            List<ComputeExpressionToken> transformationTokens = new List<ComputeExpressionToken>();

            if (string.IsNullOrEmpty(compute))
            {
                return new ComputeToken(transformationTokens);
            }

            this.recursionDepth = 0;
            this.lexer = CreateLexerForFilterOrOrderByOrApplyExpression(compute);

            while (true)
            {
                ComputeExpressionToken computed = this.ParseComputeExpression();
                transformationTokens.Add(computed);
                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Comma)
                {
                    break;
                }

                this.lexer.NextToken();
            }

            this.lexer.ValidateToken(ExpressionTokenKind.End);

            return new ComputeToken(transformationTokens);
        }

        internal ExpandToken ParseExpand()
        {
            Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordExpand), "token identifier is expand");
            lexer.NextToken();

            // '('
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            List<ExpandTermToken> termTokens = new List<ExpandTermToken>();

            // First token must be Path
            var termParser = new SelectExpandTermParser(this.lexer, this.maxDepth - 1, false);
            PathSegmentToken pathToken = termParser.ParseTerm(allowRef: true);

            QueryToken filterToken = null;
            ExpandToken nestedExpand = null;

            // Followed (optionally) by filter and expand
            // Syntax for expand inside $apply is different (and much simpler)  from $expand clause => had to use different parsing approach
            while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
            {
                this.lexer.NextToken();
                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Identifier)
                {
                    switch (this.lexer.CurrentToken.GetIdentifier())
                    {
                        case ExpressionConstants.KeywordFilter:
                            filterToken = this.ParseApplyFilter();
                            break;
                        case ExpressionConstants.KeywordExpand:
                            ExpandToken tempNestedExpand = ParseExpand();
                            nestedExpand = nestedExpand == null
                                ? tempNestedExpand
                                : new ExpandToken(nestedExpand.ExpandTerms.Concat(tempNestedExpand.ExpandTerms));
                            break;
                        default:
                            throw ParseError(ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(supportedKeywords, this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                    }
                }
            }

            // Leaf level expands require filter
            if (filterToken == null && nestedExpand == null)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_InnerMostExpandRequireFilter(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            ExpandTermToken expandTermToken = new ExpandTermToken(pathToken, filterToken, null, null, null, null, null, null, null, nestedExpand);
            termTokens.Add(expandTermToken);

            // ")"
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();


            return new ExpandToken(termTokens);
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
                    case ExpressionConstants.KeywordCompute:
                        transformationTokens.Add(ParseCompute());
                        break;
                    case ExpressionConstants.KeywordExpand:
                        transformationTokens.Add(ParseExpand());
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

        // parses $apply aggregate transformation (.e.g. aggregate(UnitPrice with sum as TotalUnitPrice))
        internal AggregateToken ParseAggregate()
        {
            Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordAggregate), "token identifier is aggregate");
            lexer.NextToken();

            return new AggregateToken(ParseAggregateExpressions());
        }

        internal List<AggregateTokenBase> ParseAggregateExpressions()
        {
            // '('
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.NextToken();

            // series of statements separates by commas
            List<AggregateTokenBase> statements = new List<AggregateTokenBase>();
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

            return statements;
        }

        internal AggregateTokenBase ParseAggregateExpression()
        {
            try
            {
                this.parseAggregateExpressionDepth++;

                // expression
                QueryToken expression = ParseLogicalOr();

                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
                {
                    // When there's a parenthesis after the expression we have a entity set aggregation.
                    // The syntax is the same as the aggregate expression itself, so we recurse on ParseAggregateExpressions.
                    this.aggregateExpressionParents.Push(expression);
                    List<AggregateTokenBase> statements = ParseAggregateExpressions();
                    this.aggregateExpressionParents.Pop();

                    return new EntitySetAggregateToken(expression, statements);
                }

                AggregationMethodDefinition verb;

                // "with" verb
                EndPathToken endPathExpression = expression as EndPathToken;
                if (endPathExpression != null && endPathExpression.Identifier == ExpressionConstants.QueryOptionCount)
                {
                    // e.g. aggregate($count as Count)
                    verb = AggregationMethodDefinition.VirtualPropertyCount;
                }
                else
                {
                    // e.g. aggregate(UnitPrice with sum as Total)
                    verb = this.ParseAggregateWith();
                }

                // "as" alias
                StringLiteralToken alias = this.ParseAggregateAs();

                return new AggregateExpressionToken(expression, verb, alias.Text);
            }
            finally
            {
                this.parseAggregateExpressionDepth--;
            }
        }

        // parses $apply groupby transformation (.e.g. groupby(ProductID, CategoryId, aggregate(UnitPrice with sum as TotalUnitPrice))
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

        // parses $apply filter transformation (.e.g. filter(ProductName eq 'Aniseed Syrup'))
        internal QueryToken ParseApplyFilter()
        {
            Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordFilter), "token identifier is filter");
            lexer.NextToken();

            // '(' expression ')'
            return this.ParseParenExpression();
        }

        /// <summary>
        /// Parse compute expression text into a token.
        /// </summary>
        /// <returns>The lexical token representing the compute expression text.</returns>
        internal ComputeExpressionToken ParseComputeExpression()
        {
            // expression
            QueryToken expression = this.ParseExpression();

            // "as" alias
            StringLiteralToken alias = this.ParseAggregateAs();

            return new ComputeExpressionToken(expression, alias.Text);
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
        /// <returns>The enumeration of lexical tokens representing order by tokens.</returns>
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
            return new ExpressionLexer(expression, true /*moveToFirstToken*/, true /*useSemicolonDelimiter*/, true /*parsingFunctionParameters*/);
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
        /// Parses the eq, ne, lt, gt, le, and ge operators.
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
                    return this.ParseInHas();
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
            return this.ParseInHas();
        }

        /// <summary>
        /// Parses the has and in operators.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseInHas()
        {
            this.RecurseEnter();
            QueryToken left = this.ParsePrimary();
            while (true)
            {
                if (this.TokenIdentifierIs(ExpressionConstants.KeywordIn))
                {
                    this.lexer.NextToken();
                    QueryToken right = this.ParsePrimary();
                    left = new InToken(left, right);
                }
                else if (this.TokenIdentifierIs(ExpressionConstants.KeywordHas))
                {
                    this.lexer.NextToken();
                    QueryToken right = this.ParsePrimary();
                    left = new BinaryOperatorToken(BinaryOperatorKind.Has, left, right);
                } 
                else
                {
                    break;
                }
            }

            this.RecurseLeave();
            return left;
        }

        /// <summary>
        /// Parses the primary expressions.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParsePrimary()
        {
            this.RecurseEnter();
            QueryToken expr = this.aggregateExpressionParents.Count > 0 ? this.aggregateExpressionParents.Peek() : null;
            if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash)
            {
                expr = this.ParseSegment(expr);
            }
            else
            {
                expr = this.ParsePrimaryStart();
            }

            while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Slash)
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
                else if (this.TokenIdentifierIs(ExpressionConstants.QueryOptionCount))
                {
                    expr = this.ParseCountSegment(expr);
                }
                else if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash)
                {
                    expr = this.ParseSegment(expr);
                }
                else
                {
                    IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression));
                    expr = identifierTokenizer.ParseIdentifier(expr);
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
                        IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression));
                        QueryToken parent = this.aggregateExpressionParents.Count > 0 ? this.aggregateExpressionParents.Peek() : null;
                        return identifierTokenizer.ParseIdentifier(parent);
                    }

                case ExpressionTokenKind.OpenParen:
                    {
                        return this.ParseParenExpression();
                    }

                case ExpressionTokenKind.Star:
                    {
                        IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression));
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
        /// Parses a $count segment.
        /// </summary>
        /// <param name="parent">The parent of the segment node.</param>
        /// <returns>The lexical token representing the $count segment.</returns>
        private QueryToken ParseCountSegment(QueryToken parent)
        {
            this.lexer.NextToken();

            CountSegmentParser countSegmentParser = new CountSegmentParser(this.lexer, this);
            return countSegmentParser.CreateCountSegmentToken(parent);
        }

        private AggregationMethodDefinition ParseAggregateWith()
        {
            if (!TokenIdentifierIs(ExpressionConstants.KeywordWith))
            {
                throw ParseError(ODataErrorStrings.UriQueryExpressionParser_WithExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            lexer.NextToken();

            AggregationMethodDefinition verb;
            int identifierStartPosition = lexer.CurrentToken.Position;
            string methodLabel = lexer.ReadDottedIdentifier(false /* acceptStar */);

            switch (methodLabel)
            {
                case ExpressionConstants.KeywordAverage:
                    verb = AggregationMethodDefinition.Average;
                    break;
                case ExpressionConstants.KeywordCountDistinct:
                    verb = AggregationMethodDefinition.CountDistinct;
                    break;
                case ExpressionConstants.KeywordMax:
                    verb = AggregationMethodDefinition.Max;
                    break;
                case ExpressionConstants.KeywordMin:
                    verb = AggregationMethodDefinition.Min;
                    break;
                case ExpressionConstants.KeywordSum:
                    verb = AggregationMethodDefinition.Sum;
                    break;
                default:
                    if (!methodLabel.Contains(OData.ExpressionConstants.SymbolDot))
                    {
                        throw ParseError(
                            ODataErrorStrings.UriQueryExpressionParser_UnrecognizedWithMethod(
                                methodLabel,
                                identifierStartPosition,
                                this.lexer.ExpressionText));
                    }

                    verb = AggregationMethodDefinition.Custom(methodLabel);
                    break;
            }

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
