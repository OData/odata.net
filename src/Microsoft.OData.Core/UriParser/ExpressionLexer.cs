//---------------------------------------------------------------------
// <copyright file="ExpressionLexer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Parsers.UriParsers;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>Use this class to parse an expression in the OData URI format.</summary>
    /// <remarks>
    /// Literals (non-normative "handy" reference - see spec for correct expression):
    /// Null            null
    /// Boolean         true | false
    /// Int32           (digit+)
    /// Int64           (digit+)[L|l]
    /// Decimal         (digit+ ['.' digit+])[M|m]
    /// Float           (digit+ ['.' digit+][e|E [+|-] digit+)[f|F]
    /// Double          (digit+ ['.' digit+][e|E [+|-] digit+)[d|D]
    /// String          "'" .* "'"
    /// DateTime        datetime"'"dddd-dd-dd[T|' ']dd:mm[ss[.fffffff]]"'"
    /// DateTimeOffset  dddd-dd-dd[T|' ']dd:mm[ss[.fffffff]]-dd:mm
    /// Duration            time"'"dd:mm[ss[.fffffff]]"'"
    /// Binary          (binary|X)'digit*'
    /// GUID            8HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 12HEXDIG 
    /// 
    /// Note: ABNF v4.0 actually has forbidden numeric string's trailing L,M,F,D though we allow them to be optional
    /// http://docs.oasis-open.org/odata/odata/v4.0/cs01/abnf/odata-abnf-construction-rules.txt
    /// decimalValue = [SIGN] 1*DIGIT ["." 1*DIGIT]
    /// doubleValue = decimalValue [ "e" [SIGN] 1*DIGIT ] / nanInfinity ; with restricted number range
    /// singleValue = doubleValue                                       ; with restricted number range
    /// nanInfinity = 'NaN' / '-INF' / 'INF'
    ///
    /// </remarks>
    [DebuggerDisplay("ExpressionLexer ({text} @ {textPos} [{token}])")]
    internal class ExpressionLexer
    {
        #region Protected and Private fields

        /// <summary>Text being parsed.</summary>
        protected readonly string Text;

        /// <summary>Length of text being parsed.</summary>
        protected readonly int TextLen;

        /// <summary>Position on text being parsed.</summary>
        protected int textPos;

        /// <summary>Character being processed.</summary>
        protected char? ch;

        /// <summary>Token being processed.</summary>
        protected ExpressionToken token;

        /// <summary>
        /// For an identifier, EMD supports chars that match the regex  [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]
        /// IsLetterOrDigit covers Ll, Lu, Lt, Lo, Lm, Nd, this set covers the rest 
        /// </summary>
        private static readonly HashSet<UnicodeCategory> AdditionalUnicodeCategoriesForIdentifier = new HashSet<UnicodeCategory>(new UnicodeCategoryEqualityComparer())
        {
            UnicodeCategory.LetterNumber,
            UnicodeCategory.NonSpacingMark,
            UnicodeCategory.SpacingCombiningMark, 
            UnicodeCategory.ConnectorPunctuation, // covers "_"
            UnicodeCategory.Format
        };

        /// <summary> flag to indicate whether to delimit on a semicolon. </summary>
        private readonly bool useSemicolonDelimeter;

        /// <summary>Whether the lexer is being used to parse function parameters. If true, will allow/recognize parameter aliases and typed nulls.</summary>
        private readonly bool parsingFunctionParameters;

        /// <summary>Lexer ignores whitespace</summary>
        private bool ignoreWhitespace;

        #endregion Protected and Private fields

        #region Constructors

        /// <summary>Initializes a new <see cref="ExpressionLexer"/>.</summary>
        /// <param name="expression">Expression to parse.</param>
        /// <param name="moveToFirstToken">If true, this constructor will call NextToken() to move to the first token.</param>
        /// <param name="useSemicolonDelimeter">If true, the lexer will tokenize based on semicolons as well.</param>
        internal ExpressionLexer(string expression, bool moveToFirstToken, bool useSemicolonDelimeter)
            : this(expression, moveToFirstToken, useSemicolonDelimeter, false /*parsingFunctionParameters*/)
        {
        }

        /// <summary>Initializes a new <see cref="ExpressionLexer"/>.</summary>
        /// <param name="expression">Expression to parse.</param>
        /// <param name="moveToFirstToken">If true, this constructor will call NextToken() to move to the first token.</param>
        /// <param name="useSemicolonDelimeter">If true, the lexer will tokenize based on semicolons as well.</param>
        /// <param name="parsingFunctionParameters">Whether the lexer is being used to parse function parameters. If true, will allow/recognize parameter aliases and typed nulls.</param>
        internal ExpressionLexer(string expression, bool moveToFirstToken, bool useSemicolonDelimeter, bool parsingFunctionParameters)
        {
            Debug.Assert(expression != null, "expression != null");

            this.ignoreWhitespace = true;
            this.Text = expression;
            this.TextLen = this.Text.Length;
            this.useSemicolonDelimeter = useSemicolonDelimeter;
            this.parsingFunctionParameters = parsingFunctionParameters;

            this.SetTextPos(0);

            if (moveToFirstToken)
            {
                this.NextToken();
            }
        }

        #endregion Constructors

        #region Internal properties

        /// <summary>Token being processed.</summary>
        internal ExpressionToken CurrentToken
        {
            get
            {
                return this.token;
            }

            set
            {
                this.token = value;
            }
        }

        /// <summary>Text being parsed.</summary>
        internal string ExpressionText
        {
            get
            {
                return this.Text;
            }
        }

        /// <summary>Position on text being parsed.</summary>
        internal int Position
        {
            get
            {
                return this.token.Position;
            }
        }

        #endregion Internal properties

        #region Private properties
        /// <summary>
        /// Gets if the current char is whitespace.
        /// </summary>
        protected bool IsValidWhiteSpace
        {
            get
            {
                return this.ch != null && Char.IsWhiteSpace(this.ch.Value);
            }
        }

        /// <summary>
        /// Gets if the current char is digit.
        /// </summary>
        private bool IsValidDigit
        {
            get
            {
                return this.ch != null && Char.IsDigit(this.ch.Value);
            }
        }

        /// <summary>
        /// Is the current char a valid starting char for an identifier.
        /// Valid starting chars for identifier include all that are supported by EDM ([\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}]) and '_'.
        /// </summary>
        private bool IsValidStartingCharForIdentifier
        {
            get
            {
                return this.ch != null && (
                    Char.IsLetter(this.ch.Value) ||       // IsLetter covers: Ll, Lu, Lt, Lo, Lm
                    this.ch == '_' ||
                    this.ch == '$' ||
                    PlatformHelper.GetUnicodeCategory(this.ch.Value) == UnicodeCategory.LetterNumber);
            }
        }

        /// <summary>
        /// Is the current char a valid non-starting char for an identifier.
        /// Valid non-starting chars for identifier include all that are supported 
        /// by EDM  [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]. 
        /// This list includes '_', which is ConnectorPunctuation (Pc)
        /// </summary>
        private bool IsValidNonStartingCharForIdentifier
        {
            get
            {
                return this.ch != null && (
                    Char.IsLetterOrDigit(this.ch.Value) ||    // covers: Ll, Lu, Lt, Lo, Lm, Nd
                    AdditionalUnicodeCategoriesForIdentifier.Contains(PlatformHelper.GetUnicodeCategory(this.ch.Value)));  // covers the rest
            }
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Determines if the next token can be processed without error without advancing the token.
        /// </summary>
        /// <param name="resultToken">The next ExpressionToken. This value is undefined if error is defined.</param>
        /// <param name="error">Exception generated from trying to process the next token.</param>
        /// <returns>True if the next token can be processed, false otherwise.</returns>
        internal bool TryPeekNextToken(out ExpressionToken resultToken, out Exception error)
        {
            int savedTextPos = this.textPos;
            char? savedChar = this.ch;
            ExpressionToken savedToken = this.token;

            resultToken = this.NextTokenImplementation(out error);

            this.textPos = savedTextPos;
            this.ch = savedChar;
            this.token = savedToken;

            return error == null;
        }

        /// <summary>Reads the next token, skipping whitespace as necessary, advancing the Lexer.</summary>
        /// <returns>The next token.</returns>
        /// <remarks>Throws on error.</remarks>
        internal ExpressionToken NextToken()
        {
            Exception error = null;
            ExpressionToken nextToken = this.NextTokenImplementation(out error);

            if (error != null)
            {
                throw error;
            }

            return nextToken;
        }

        /// <summary>
        /// Starting from an identifier, reads a sequence of dots and 
        /// identifiers, and returns the text for it, with whitespace 
        /// stripped.
        /// </summary>
        /// <param name="acceptStar">do we allow a star in this identifier</param>
        /// <returns>The dotted identifier starting at the current identifier.</returns>
        internal string ReadDottedIdentifier(bool acceptStar)
        {
            this.ValidateToken(ExpressionTokenKind.Identifier);
            StringBuilder builder = null;
            string result = this.CurrentToken.Text;
            this.NextToken();
            while (this.CurrentToken.Kind == ExpressionTokenKind.Dot)
            {
                this.NextToken();
                if (this.CurrentToken.Kind != ExpressionTokenKind.Identifier &&
                    this.CurrentToken.Kind != ExpressionTokenKind.QuotedLiteral)
                {
                    if (this.CurrentToken.Kind == ExpressionTokenKind.Star)
                    {
                        // if we accept a star and this is the last token in the identifier, then we're ok... otherwise we throw.
                        if (!acceptStar || (this.PeekNextToken().Kind != ExpressionTokenKind.End && this.PeekNextToken().Kind != ExpressionTokenKind.Comma))
                        {
                            throw ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.Text));
                        }
                    }
                    else
                    {
                        throw ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.Text));
                    }
                }

                if (builder == null)
                {
                    builder = new StringBuilder(result, result.Length + 1 + this.CurrentToken.Text.Length);
                }

                builder.Append('.');
                builder.Append(this.CurrentToken.Text);
                this.NextToken();
            }

            if (builder != null)
            {
                result = builder.ToString();
            }

            return result;
        }

        /// <summary>Returns the next token without advancing the lexer.</summary>
        /// <returns>The next token.</returns>
        internal ExpressionToken PeekNextToken()
        {
            ExpressionToken outToken;
            Exception error;
            this.TryPeekNextToken(out outToken, out error);

            if (error != null)
            {
                throw error;
            }

            return outToken;
        }

        /// <summary>
        /// Check whether the current identifier is a function. If so, expand the token text to the function signature
        /// </summary>
        /// <returns>True if the current identifier is a function call</returns>
        internal bool ExpandIdentifierAsFunction()
        {
            // FUNCTION := (<ID> {<DOT>}) ... <ID> <OpenParen>
            // if we fail to match then we leave the token as it
            ExpressionTokenKind id = this.token.Kind;
            if (id != ExpressionTokenKind.Identifier)
            {
                return false;
            }

            int savedTextPos = this.textPos;
            char? savedChar = this.ch;
            ExpressionToken savedToken = this.token;
            bool savedIgnoreWs = this.ignoreWhitespace;
            this.ignoreWhitespace = false;

            // Expansion left anchor
            int tokenStartPos = this.token.Position;

            while (this.MoveNextWhenMatch(ExpressionTokenKind.Dot) && this.MoveNextWhenMatch(ExpressionTokenKind.Identifier))
            {
            }

            bool matched = this.CurrentToken.Kind == ExpressionTokenKind.Identifier && this.PeekNextToken().Kind == ExpressionTokenKind.OpenParen;

            if (matched)
            {
                this.token.Text = this.Text.Substring(tokenStartPos, this.textPos - tokenStartPos);
                this.token.Position = tokenStartPos;
            }
            else
            {
                this.textPos = savedTextPos;
                this.ch = savedChar;
                this.token = savedToken;
            }

            this.ignoreWhitespace = savedIgnoreWs;

            return matched;
        }

        /// <summary>Validates the current token is of the specified kind.</summary>
        /// <param name="t">Expected token kind.</param>
        internal void ValidateToken(ExpressionTokenKind t)
        {
            if (this.token.Kind != t)
            {
                throw ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.Text));
            }
        }

        /// <summary>
        /// Advances the lexer until a semicolon, an unbalanced close parens occurs, or the text ends.
        /// Any string literals (text in single quotes) will be skipped when checking for delimiters.
        /// The CurrentToken of the lexer after this method call will be whatever comes after the advanced text.
        /// </summary>
        /// <returns>All of the text that was read.</returns>
        internal string AdvanceThroughExpandOption()
        {
            int startingPosition = this.textPos;
            string textToReturn;

            while (true)
            {
                if (this.ch == '\'')
                {
                    this.AdvanceToNextOccuranceOf('\'');
                }

                if (this.ch == '(')
                {
                    this.NextChar();
                    this.AdvanceThroughBalancedParentheticalExpression();
                    continue;
                }

                if (this.ch == ';' || this.ch == ')')
                {
                    textToReturn = this.Text.Substring(startingPosition, this.textPos - startingPosition);
                    break;
                }

                if (this.ch == null)
                {
                    textToReturn = this.Text.Substring(startingPosition);
                    break;
                }

                this.NextChar();
            }

            // Move the lexer to be on the delimiter (or past, if the delimiter isn't something special)
            this.NextToken();

            return textToReturn;
        }

        /// <summary>
        /// Advances through a balanced expression that we do not want to parse, beginning with a '(' and ending with a ')'.
        /// </summary>
        /// <remarks>
        /// 1. This method will identify and advance through inner pairs of parenthesis.
        /// 2. The lexer is expected to have a CurrentToken which is the open parenthesis at the start, meaning that we are positioned
        ///    on the first character of the expression inside that.
        /// 3. When we are done we will be right after the closing ')', but we will have have set CurrentToken to anything.
        ///    For this reason, you probably want to call NextToken() after this method, since CurrentToken wil be garbage.
        /// </remarks>
        /// <returns>The parenthesis expression, including the outer parenthesis.</returns>
        internal string AdvanceThroughBalancedParentheticalExpression()
        {
            int startPosition = this.Position;
            this.AdvanceThroughBalancedExpression('(', ')');
            var expressionText = this.Text.Substring(startPosition, this.textPos - startPosition);

            //// TODO: Consider introducing a token type and setting up the current token instead of returning string.
            //// We've done weird stuff, and the state of hte lexer is weird now. All will be well once NextToken() is called, 
            //// but until then CurrentToken is stale and misleading.

            return expressionText;
        }

        #endregion Internal methods

        #region Private methods
        /// <summary>Creates an exception for a parse error.</summary>
        /// <param name="message">Message text.</param>
        /// <returns>A new Exception.</returns>
        protected static Exception ParseError(string message)
        {
            return new ODataException(message);
        }

        /// <summary>Advanced to the next character.</summary>
        protected void NextChar()
        {
            if (this.textPos < this.TextLen)
            {
                this.textPos++;
                if (this.textPos < this.TextLen)
                {
                    this.ch = this.Text[this.textPos];
                    return;
                }
            }

            this.ch = null;
        }

        /// <summary>
        /// Parses white spaces
        /// </summary>
        protected void ParseWhitespace()
        {
            while (this.IsValidWhiteSpace)
            {
                this.NextChar();
            }
        }

        /// <summary>
        /// Advance the pointer to the next occurance of the given value, swallowing all characters in between.
        /// </summary>
        /// <param name="endingValue">the ending delimiter.</param>
        protected void AdvanceToNextOccuranceOf(char endingValue)
        {
            this.NextChar();
            while (this.ch.HasValue && (this.ch != endingValue))
            {
                this.NextChar();
            }
        }

        /// <summary>Reads the next token, skipping whitespace as necessary.</summary> 
        /// <param name="error">Error that occurred while trying to process the next token.</param>
        /// <returns>The next token, which may be 'bad' if an error occurs.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This parser method is all about the switch statement and would be harder to maintain if it were broken up.")]
        protected virtual ExpressionToken NextTokenImplementation(out Exception error)
        {
            error = null;

            if (this.ignoreWhitespace)
            {
                this.ParseWhitespace();
            }

            ExpressionTokenKind t;
            int tokenPos = this.textPos;
            switch (this.ch)
            {
                case '(':
                    this.NextChar();
                    t = ExpressionTokenKind.OpenParen;
                    break;
                case ')':
                    this.NextChar();
                    t = ExpressionTokenKind.CloseParen;
                    break;
                case ',':
                    this.NextChar();
                    t = ExpressionTokenKind.Comma;
                    break;
                case '-':
                    bool hasNext = this.textPos + 1 < this.TextLen;
                    if (hasNext && Char.IsDigit(this.Text[this.textPos + 1]))
                    {
                        // don't separate '-' and its following digits : -2147483648 is valid int.MinValue, but 2147483648 is long.
                        t = this.ParseFromDigit();
                        if (ExpressionLexerUtils.IsNumeric(t))
                        {
                            break;
                        }

                        // If it looked like a numeric but wasn't (because it was a binary 0x... value for example), 
                        // we'll rewind and fall through to a simple '-' token.
                        this.SetTextPos(tokenPos);
                    }
                    else if (hasNext && this.Text[tokenPos + 1] == ExpressionConstants.InfinityLiteral[0])
                    {
                        this.NextChar();
                        this.ParseIdentifier();
                        string currentIdentifier = this.Text.Substring(tokenPos + 1, this.textPos - tokenPos - 1);

                        if (ExpressionLexerUtils.IsInfinityLiteralDouble(currentIdentifier))
                        {
                            t = ExpressionTokenKind.DoubleLiteral;
                            break;
                        }
                        else if (ExpressionLexerUtils.IsInfinityLiteralSingle(currentIdentifier))
                        {
                            t = ExpressionTokenKind.SingleLiteral;
                            break;
                        }

                        // If it looked like '-INF' but wasn't we'll rewind and fall through to a simple '-' token.
                        this.SetTextPos(tokenPos);
                    }

                    this.NextChar();
                    t = ExpressionTokenKind.Minus;
                    break;
                case '=':
                    this.NextChar();
                    t = ExpressionTokenKind.Equal;
                    break;
                case '/':
                    this.NextChar();
                    t = ExpressionTokenKind.Slash;
                    break;
                case '?':
                    this.NextChar();
                    t = ExpressionTokenKind.Question;
                    break;
                case '.':
                    this.NextChar();
                    t = ExpressionTokenKind.Dot;
                    break;
                case '\'':
                    char quote = this.ch.Value;
                    do
                    {
                        this.AdvanceToNextOccuranceOf(quote);

                        if (this.textPos == this.TextLen)
                        {
                            error = ParseError(ODataErrorStrings.ExpressionLexer_UnterminatedStringLiteral(this.textPos, this.Text));
                        }

                        this.NextChar();
                    }
                    while (this.ch.HasValue && (this.ch.Value == quote));
                    t = ExpressionTokenKind.StringLiteral;
                    break;
                case '*':
                    this.NextChar();
                    t = ExpressionTokenKind.Star;
                    break;
                case ':':
                    this.NextChar();
                    t = ExpressionTokenKind.Colon;
                    break;
                case '{':
                    this.NextChar();
                    this.AdvanceThroughBalancedExpression('{', '}');
                    t = ExpressionTokenKind.BracketedExpression;
                    break;
                case '[':
                    this.NextChar();
                    this.AdvanceThroughBalancedExpression('[', ']');
                    t = ExpressionTokenKind.BracketedExpression;
                    break;
                default:
                    if (this.IsValidWhiteSpace)
                    {
                        Debug.Assert(!this.ignoreWhitespace, "should not hit ws while ignoring it");
                        this.ParseWhitespace();
                        t = ExpressionTokenKind.Unknown;
                        break;
                    }

                    if (this.IsValidStartingCharForIdentifier)
                    {
                        this.ParseIdentifier();

                        // Guids will have '-' in them
                        // guidValue = 8HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 12HEXDIG 
                        if (this.ch == '-'
                            && this.TryParseGuid(tokenPos))
                        {
                            t = ExpressionTokenKind.GuidLiteral;
                            break;
                        }

                        t = ExpressionTokenKind.Identifier;
                        break;
                    }

                    if (this.IsValidDigit)
                    {
                        t = this.ParseFromDigit();
                        break;
                    }

                    if (this.textPos == this.TextLen)
                    {
                        t = ExpressionTokenKind.End;
                        break;
                    }

                    if (this.useSemicolonDelimeter && this.ch == ';')
                    {
                        this.NextChar();
                        t = ExpressionTokenKind.SemiColon;
                        break;
                    }

                    if (this.parsingFunctionParameters && this.ch == '@')
                    {
                        this.NextChar();

                        if (this.textPos == this.TextLen)
                        {
                            error = ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.Text));
                            t = ExpressionTokenKind.Unknown;
                            break;
                        }

                        if (!this.IsValidStartingCharForIdentifier)
                        {
                            error = ParseError(ODataErrorStrings.ExpressionLexer_InvalidCharacter(this.ch, this.textPos, this.Text));
                            t = ExpressionTokenKind.Unknown;
                            break;
                        }

                        this.ParseIdentifier();
                        t = ExpressionTokenKind.ParameterAlias;
                        break;
                    }

                    error = ParseError(ODataErrorStrings.ExpressionLexer_InvalidCharacter(this.ch, this.textPos, this.Text));
                    t = ExpressionTokenKind.Unknown;
                    break;
            }

            this.token.Kind = t;
            this.token.Text = this.Text.Substring(tokenPos, this.textPos - tokenPos);
            this.token.Position = tokenPos;

            this.HandleTypePrefixedLiterals();

            return this.token;
        }

        /// <summary>
        /// Expand the token selection if the next token matches the input token
        /// </summary>
        /// <param name="id">the list of token id to match</param>
        /// <returns>true if matched</returns>
        private bool MoveNextWhenMatch(ExpressionTokenKind id)
        {
            ExpressionToken next = this.PeekNextToken();

            if (id == next.Kind)
            {
                this.NextToken();
                return true;
            }

            return false;
        }

        /// <summary>Handles lexeres that are formed by identifiers.</summary>
        /// <remarks>This method modified the token field as necessary.</remarks>
        private void HandleTypePrefixedLiterals()
        {
            if (this.token.Kind != ExpressionTokenKind.Identifier)
            {
                return;
            }

            // Get literal of quoted values
            if (this.ch == '\'')
            {
                // Get custom literal if exists.
                IEdmTypeReference edmTypeOfCustomLiteral = CustomUriLiteralPrefixes.GetEdmTypeByCustomLiteralPrefix(this.token.Text);
                if (edmTypeOfCustomLiteral != null)
                {
                    this.token.SetCustomEdmTypeLiteral(edmTypeOfCustomLiteral);
                }
                else
                {
                    // Get built in type literal prefix for quoted values
                    this.token.Kind = this.GetBuiltInTypesLiteralPrefixWithQuotedValue(this.token.Text);
                }

                this.HandleQuotedValues();
            }
            else
            {
                // Handle keywords.
                // Get built in type literal prefix.
                ExpressionTokenKind? regularTokenKind = GetBuiltInTypesLiteralPrefix(this.token.Text);
                if (regularTokenKind.HasValue)
                {
                    this.token.Kind = regularTokenKind.Value;
                }
            }
        }

        private void HandleQuotedValues()
        {
            int startPosition = this.token.Position;

            do
            {
                do
                {
                    this.NextChar();
                }
                while (this.ch.HasValue && this.ch != '\'');

                if (this.ch == null)
                {
                    throw ParseError(ODataErrorStrings.ExpressionLexer_UnterminatedLiteral(this.textPos, this.Text));
                }

                this.NextChar();
            }
            while (this.ch.HasValue && this.ch == '\'');

            // Update token.Text to include the literal + the quoted value
            this.token.Text = this.Text.Substring(startPosition, this.textPos - startPosition);
        }

        /// <summary>
        /// Get type-prefixed literals such as double, boolean...
        /// </summary>
        /// <param name="tokenText">Token texk</param>
        /// <returns>ExpressionTokenKind by the token text</returns>
        private static ExpressionTokenKind? GetBuiltInTypesLiteralPrefix(string tokenText)
        {
            if (ExpressionLexerUtils.IsInfinityOrNaNDouble(tokenText))
            {
                return ExpressionTokenKind.DoubleLiteral;
            }
            else if (ExpressionLexerUtils.IsInfinityOrNanSingle(tokenText))
            {
                return ExpressionTokenKind.SingleLiteral;
            }
            else if (tokenText == ExpressionConstants.KeywordTrue || tokenText == ExpressionConstants.KeywordFalse)
            {
                return ExpressionTokenKind.BooleanLiteral;
            }
            else if (tokenText == ExpressionConstants.KeywordNull)
            {
                return ExpressionTokenKind.NullLiteral;
            }

            return null;
        }

        /// <summary>
        /// Get type-prefixed literals with quoted values duration, binary and spatial types.
        /// </summary>
        /// <param name="tokenText">Token text</param>
        /// <returns>ExpressionTokenKind</returns>
        /// <example>geometry'POINT (79 84)'. 'geometry' is the tokenText </example>
        private ExpressionTokenKind GetBuiltInTypesLiteralPrefixWithQuotedValue(string tokenText)
        {
            if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixDuration, StringComparison.OrdinalIgnoreCase))
            {
                return ExpressionTokenKind.DurationLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixBinary, StringComparison.OrdinalIgnoreCase))
            {
                return ExpressionTokenKind.BinaryLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixGeography, StringComparison.OrdinalIgnoreCase))
            {
                return ExpressionTokenKind.GeographyLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixGeometry, StringComparison.OrdinalIgnoreCase))
            {
                return ExpressionTokenKind.GeometryLiteral;
            }
            else if (string.Equals(tokenText, ExpressionConstants.KeywordNull, StringComparison.OrdinalIgnoreCase))
            {
                // typed null literals are not supported.
                throw ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.Text));
            }
            else
            {
                // treat as quoted literal
                return ExpressionTokenKind.QuotedLiteral;
            }
        }

        /// <summary>Parses a token that starts with a digit.</summary>
        /// <returns>The kind of token recognized.</returns>
        private ExpressionTokenKind ParseFromDigit()
        {
            Debug.Assert(this.IsValidDigit || ('-' == this.ch), "this.IsValidDigit || ('-' == this.ch)");
            ExpressionTokenKind result;
            int tokenPos = this.textPos;
            char startChar = this.ch.Value;
            this.NextChar();
            if (startChar == '0' && (this.ch == 'x' || this.ch == 'X'))
            {
                result = ExpressionTokenKind.BinaryLiteral;
                do
                {
                    this.NextChar();
                }
                while (this.ch.HasValue && UriParserHelper.IsCharHexDigit(this.ch.Value));
            }
            else
            {
                result = ExpressionTokenKind.IntegerLiteral;
                while (this.IsValidDigit)
                {
                    this.NextChar();
                }

                // DateTimeOffset, Date and Guids will have '-' in them
                if (this.ch == '-')
                {
                    if (this.TryParseDateTimeoffset(tokenPos))
                    {
                        return ExpressionTokenKind.DateTimeOffsetLiteral;
                    }
                    else if (this.TryParseGuid(tokenPos))
                    {
                        return ExpressionTokenKind.GuidLiteral;
                    }
                }

                // TimeOfDay will have ":" in them
                if (this.ch == ':')
                {
                    if (this.TryParseTimeOfDay(tokenPos))
                    {
                        return ExpressionTokenKind.TimeOfDayLiteral;
                    }
                }

                // Guids will have alpha-numeric characters along with '-', so if a letter is encountered
                // try to see if this is Guid or not. 
                if (this.ch.HasValue && Char.IsLetter(this.ch.Value))
                {
                    if (this.TryParseGuid(tokenPos))
                    {
                        return ExpressionTokenKind.GuidLiteral;
                    }
                }

                if (this.ch == '.')
                {
                    result = ExpressionTokenKind.DoubleLiteral;
                    this.NextChar();
                    this.ValidateDigit();

                    do
                    {
                        this.NextChar();
                    }
                    while (this.IsValidDigit);
                }

                if (this.ch == 'E' || this.ch == 'e')
                {
                    result = ExpressionTokenKind.DoubleLiteral;
                    this.NextChar();
                    if (this.ch == '+' || this.ch == '-')
                    {
                        this.NextChar();
                    }

                    this.ValidateDigit();
                    do
                    {
                        this.NextChar();
                    }
                    while (this.IsValidDigit);
                }

                if (this.ch == 'M' || this.ch == 'm')
                {
                    result = ExpressionTokenKind.DecimalLiteral;
                    this.NextChar();
                }
                else if (this.ch == 'd' || this.ch == 'D')
                {
                    result = ExpressionTokenKind.DoubleLiteral;
                    this.NextChar();
                }
                else if (this.ch == 'L' || this.ch == 'l')
                {
                    result = ExpressionTokenKind.Int64Literal;
                    this.NextChar();
                }
                else if (this.ch == 'f' || this.ch == 'F')
                {
                    result = ExpressionTokenKind.SingleLiteral;
                    this.NextChar();
                }
                else
                {
                    string valueStr = this.Text.Substring(tokenPos, this.textPos - tokenPos);
                    result = MakeBestGuessOnNoSuffixStr(valueStr, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Tries to parse Guid from current text
        /// If it's not Guid, then this.textPos and this.ch are reset
        /// </summary>
        /// <param name="tokenPos">Start index</param>
        /// <returns>True if the substring that starts from tokenPos is a Guid, false otherwise</returns>
        private bool TryParseGuid(int tokenPos)
        {
            int initialIndex = this.textPos;

            string guidStr = ParseLiteral(tokenPos);

            Guid tmpGuidValue;
            if (UriUtils.TryUriStringToGuid(guidStr, out tmpGuidValue))
            {
                return true;
            }
            else
            {
                this.textPos = initialIndex;
                this.ch = this.Text[initialIndex];
                return false;
            }
        }

        /// <summary>
        /// Tries to parse Guid from current text
        /// If it's not Guid, then this.textPos and this.ch are reset
        /// </summary>
        /// <param name="tokenPos">Start index</param>
        /// <returns>True if the substring that starts from tokenPos is a Guid, false otherwise</returns>
        private bool TryParseDateTimeoffset(int tokenPos)
        {
            int initialIndex = this.textPos;

            string datetimeOffsetStr = ParseLiteral(tokenPos);

            DateTimeOffset tmpdatetimeOffsetValue;
            if (UriUtils.ConvertUriStringToDateTimeOffset(datetimeOffsetStr, out tmpdatetimeOffsetValue))
            {
                return true;
            }
            else
            {
                this.textPos = initialIndex;
                this.ch = this.Text[initialIndex];
                return false;
            }
        }

        /// <summary>
        /// Tries to parse TimeOfDay from current text
        /// If it's not TimeOfDay, then this.textPos and this.ch are reset
        /// </summary>
        /// <param name="tokenPos">Start index</param>
        /// <returns>True if the substring that starts from tokenPos is a TimeOfDay, false otherwise</returns>
        private bool TryParseTimeOfDay(int tokenPos)
        {
            int initialIndex = this.textPos;

            string timeOfDayStr = ParseLiteral(tokenPos);

            TimeOfDay tmpTimeOfDayValue;
            if (UriUtils.TryUriStringToTimeOfDay(timeOfDayStr, out tmpTimeOfDayValue))
            {
                return true;
            }
            else
            {
                this.textPos = initialIndex;
                this.ch = this.Text[initialIndex];
                return false;
            }
        }

        /// <summary>
        /// Parses a literal be checking for delimiting characters '\0', ',',')' and ' '
        /// </summary>
        /// <param name="tokenPos">Index from which the substring starts</param>
        /// <returns>Substring from this.text that has parsed the literal and ends in one of above delimiting characters</returns>
        private string ParseLiteral(int tokenPos)
        {
            do
            {
                this.NextChar();
            }
            while (this.ch.HasValue && this.ch != ',' && this.ch != ')' && this.ch != ' ');

            if (this.ch == null)
            {
                this.NextChar();
            }

            string numericStr = this.Text.Substring(tokenPos, this.textPos - tokenPos);
            return numericStr;
        }

        /// <summary>
        /// Makes best guess on numeric string without trailing letter like L, F, M, D
        /// </summary>
        /// <param name="numericStr">The numeric string.</param>
        /// <param name="guessedKind">The possbile kind (IntegerLiteral or DoubleLiteral) from ParseFromDigit() method.</param>
        /// <returns>A more accurate ExpressionTokenKind</returns>
        private static ExpressionTokenKind MakeBestGuessOnNoSuffixStr(string numericStr, ExpressionTokenKind guessedKind)
        {
            // no suffix, so 
            // (1) make a best guess (note: later we support promoting each to later one: int32->int64->single->double->decimal).
            // look at value:       "2147483647" may be Int32/long, "2147483649" must be long.
            // look at precision:   "3258.67876576549" may be sinle/double/decimal, "3258.678765765489753678965390" must be decimal.
            // (2) then let MetadataUtilsCommon.CanConvertPrimitiveTypeTo() method does further promotion when knowing expected sematics type. 
            int tmpInt = 0;
            long tmpLong = 0;
            float tmpFloat = 0;
            double tmpDouble = 0;
            decimal tmpDecimal = 0;

            if (guessedKind == ExpressionTokenKind.IntegerLiteral)
            {
                if (int.TryParse(numericStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out tmpInt))
                {
                    return ExpressionTokenKind.IntegerLiteral;
                }

                if (long.TryParse(numericStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out tmpLong))
                {
                    return ExpressionTokenKind.Int64Literal;
                }
            }

            bool canBeSingle = float.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out tmpFloat);
            bool canBeDouble = double.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out tmpDouble);
            bool canBeDecimal = decimal.TryParse(numericStr, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out tmpDecimal);

            // 1. try high precision -> low precision
            if (canBeDouble && canBeDecimal)
            {
                decimal doubleToDecimalR;
                decimal doubleToDecimalN;

                // To keep the full precision of the current value, which if necessary is all 17 digits of precision supported by the Double type.
                bool doubleCanBeDecimalR = decimal.TryParse(tmpDouble.ToString("R", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out doubleToDecimalR);

                // To cover the scientific notation case, such as 1e+19 in the tmpDouble
                bool doubleCanBeDecimalN = decimal.TryParse(tmpDouble.ToString("N29", CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out doubleToDecimalN);

                if ((doubleCanBeDecimalR && doubleToDecimalR != tmpDecimal) || (!doubleCanBeDecimalR && doubleCanBeDecimalN && doubleToDecimalN != tmpDecimal))
                {
                    // losing precision as double, so choose decimal
                    return ExpressionTokenKind.DecimalLiteral;
                }
            }

            // here can't use normal casting like the above double VS decimal.
            // prevent losing precision in float -> double, e.g. (double)1.234f will be 1.2339999675750732d not 1.234d
            if (canBeSingle && canBeDouble && (double.Parse(tmpFloat.ToString("R", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) != tmpDouble))
            {
                // losing precision as single, so choose double
                return ExpressionTokenKind.DoubleLiteral;
            }

            // 2. try most compatible -> least compatible
            if (canBeSingle)
            {
                return ExpressionTokenKind.SingleLiteral;
            }

            if (canBeDouble)
            {
                return ExpressionTokenKind.DoubleLiteral;
            }

            throw new ODataException(ODataErrorStrings.ExpressionLexer_InvalidNumericString(numericStr));
        }

        /// <summary>
        /// Parses an expression of text that we do not know how to handle in this class, which is between a
        /// <paramref name="startingCharacter"></paramref> and an <paramref name="endingCharacter"/>.
        /// </summary>
        /// <param name="startingCharacter">the starting delimiter</param>
        /// <param name="endingCharacter">the ending delimiter.</param>
        private void AdvanceThroughBalancedExpression(char startingCharacter, char endingCharacter)
        {
            int currentBracketDepth = 1;

            while (currentBracketDepth > 0)
            {
                if (this.ch == '\'')
                {
                    this.AdvanceToNextOccuranceOf('\'');
                }

                if (this.ch == startingCharacter)
                {
                    currentBracketDepth++;
                }
                else if (this.ch == endingCharacter)
                {
                    currentBracketDepth--;
                }

                if (this.ch == null)
                {
                    throw new ODataException(ODataErrorStrings.ExpressionLexer_UnbalancedBracketExpression);
                }

                this.NextChar();
            }
        }



        /// <summary>Parses an identifier by advancing the current character.</summary>
        private void ParseIdentifier()
        {
            Debug.Assert(this.IsValidStartingCharForIdentifier, "Expected valid starting char for identifier");
            do
            {
                this.NextChar();
            }
            while (this.IsValidNonStartingCharForIdentifier);
        }

        /// <summary>Sets the text position.</summary>
        /// <param name="pos">New text position.</param>
        private void SetTextPos(int pos)
        {
            this.textPos = pos;
            this.ch = this.textPos < this.TextLen ? this.Text[this.textPos] : (char?)null;
        }

        /// <summary>Validates the current character is a digit.</summary>
        private void ValidateDigit()
        {
            if (!this.IsValidDigit)
            {
                throw ParseError(ODataErrorStrings.ExpressionLexer_DigitExpected(this.textPos, this.Text));
            }
        }

        #endregion Private methods

        #region Private classes
        /// <summary>This class implements IEqualityComparer for UnicodeCategory</summary>
        /// <remarks>
        /// Using this class rather than EqualityComparer&lt;T&gt;.Default 
        /// saves from JIT'ing it in each AppDomain.
        /// </remarks>
        private sealed class UnicodeCategoryEqualityComparer : IEqualityComparer<UnicodeCategory>
        {
            /// <summary>
            /// Checks whether two unicode categories are equal
            /// </summary>
            /// <param name="x">first unicode category</param>
            /// <param name="y">second unicode category</param>
            /// <returns>true if they are equal, false otherwise</returns>
            public bool Equals(UnicodeCategory x, UnicodeCategory y)
            {
                return x == y;
            }

            /// <summary>
            /// Gets a hash code for the specified unicode category
            /// </summary>
            /// <param name="obj">the input value</param>
            /// <returns>The hash code for the given input unicode category, the underlying int</returns>
            public int GetHashCode(UnicodeCategory obj)
            {
                return (int)obj;
            }
        }
        #endregion
    }
}
