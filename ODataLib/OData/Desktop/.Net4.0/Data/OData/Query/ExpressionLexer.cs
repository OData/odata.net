//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using Microsoft.Data.OData;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>Use this class to parse an expression in the OData URI format.</summary>
    /// <remarks>
    /// Literals (non-normative "handy" reference - see spec for correct expression):
    /// Null            null
    /// Boolean         true | false
    /// Int32           (digit+)
    /// Int64           (digit+)(L|l)
    /// Decimal         (digit+ ['.' digit+])(M|m)
    /// Float           (digit+ ['.' digit+][e|E [+|-] digit+)(f|F)
    /// Double          (digit+ ['.' digit+][e|E [+|-] digit+)
    /// String          "'" .* "'"
    /// DateTime        datetime"'"dddd-dd-dd[T|' ']dd:mm[ss[.fffffff]]"'"
    /// DateTimeOffset  datetimeoffset"'"dddd-dd-dd[T|' ']dd:mm[ss[.fffffff]]-dd:mm"'"
    /// Time            time"'"dd:mm[ss[.fffffff]]"'"
    /// Binary          (binary|X)'digit*'
    /// GUID            guid'digit*'
    /// </remarks>
    [DebuggerDisplay("ExpressionLexer ({text} @ {textPos} [{token}]")]
    internal sealed class ExpressionLexer
    {
        #region Private fields

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

        /// <summary>Text being parsed.</summary>
        private readonly string text;

        /// <summary>Length of text being parsed.</summary>
        private readonly int textLen;

        /// <summary> flag to indicate whether to delimit on a semicolon. </summary>
        private readonly bool useSemicolonDelimeter;

        /// <summary>Whether the lexer is being used to parse function parameters. If true, will allow/recognize parameter aliases and typed nulls.</summary>
        private readonly bool parsingFunctionParameters;

        /// <summary>Position on text being parsed.</summary>
        private int textPos;

        /// <summary>Character being processed.</summary>
        private char? ch;

        /// <summary>Token being processed.</summary>
        private ExpressionToken token;

        /// <summary>Lexer ignores whitespace</summary>
        private bool ignoreWhitespace;

        #endregion Private fields

        #region Constructors

        /// <summary>Initializes a new <see cref="ExpressionLexer"/>.</summary>
        /// <param name="expression">Expression to parse.</param>
        /// <param name="moveToFirstToken">If true, this constructor will call NextToken() to move to the first token.</param>
        /// <param name="useSemicolonDelimeter">If true, the lexer will tokenize based on semicolons as well.</param>
        internal ExpressionLexer(string expression, bool moveToFirstToken, bool useSemicolonDelimeter)
            : this(expression, moveToFirstToken, useSemicolonDelimeter, false /*parsingFunctionParameters*/)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>Initializes a new <see cref="ExpressionLexer"/>.</summary>
        /// <param name="expression">Expression to parse.</param>
        /// <param name="moveToFirstToken">If true, this constructor will call NextToken() to move to the first token.</param>
        /// <param name="useSemicolonDelimeter">If true, the lexer will tokenize based on semicolons as well.</param>
        /// <param name="parsingFunctionParameters">Whether the lexer is being used to parse function parameters. If true, will allow/recognize parameter aliases and typed nulls.</param>
        internal ExpressionLexer(string expression, bool moveToFirstToken, bool useSemicolonDelimeter, bool parsingFunctionParameters)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(expression != null, "expression != null");

            this.ignoreWhitespace = true;
            this.text = expression;
            this.textLen = this.text.Length;
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
                DebugUtils.CheckNoExternalCallers();
                return this.token;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.token = value;
            }
        }

        /// <summary>Text being parsed.</summary>
        internal string ExpressionText
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.text;
            }
        }

        /// <summary>Position on text being parsed.</summary>
        internal int Position
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.token.Position;
            }
        }

        #endregion Internal properties

        #region Private properties
        /// <summary>
        /// Gets if the current char is whitespace.
        /// </summary>
        private bool IsValidWhiteSpace
        {
            get
            {
                return this.ch == null
                    ?
                    false
                    :
                    Char.IsWhiteSpace(this.ch.Value);
            }
        }

        /// <summary>
        /// Gets if the current char is digit.
        /// </summary>
        private bool IsValidDigit
        {
            get
            {
                return this.ch == null
                    ?
                    false
                    :
                    Char.IsDigit(this.ch.Value);
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
                return this.ch == null
                    ?
                    false
                    :
                    Char.IsLetter(this.ch.Value) ||       // IsLetter covers: Ll, Lu, Lt, Lo, Lm
                    this.ch == '_' ||
                    this.ch == '$' ||
                    PlatformHelper.GetUnicodeCategory(this.ch.Value) == UnicodeCategory.LetterNumber;
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
                return this.ch == null
                    ?
                    false
                    :
                    (Char.IsLetterOrDigit(this.ch.Value) ||    // covers: Ll, Lu, Lt, Lo, Lm, Nd
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
            DebugUtils.CheckNoExternalCallers();

            int savedTextPos = this.textPos;
            char? savedChar = (this.ch == null) ? (char?)null : this.ch.Value;
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
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();

            this.ValidateToken(ExpressionTokenKind.Identifier);
            StringBuilder builder = null;
            string result = this.CurrentToken.Text;
            this.NextToken();
            while (this.CurrentToken.Kind == ExpressionTokenKind.Dot)
            {
                this.NextToken();
                if (this.CurrentToken.Kind != ExpressionTokenKind.Identifier)
                {
                    if (this.CurrentToken.Kind == ExpressionTokenKind.Star)
                    {
                        // if we accept a star and this is the last token in the identifier, then we're ok... otherwise we throw.
                        if (!acceptStar || (this.PeekNextToken().Kind != ExpressionTokenKind.End && this.PeekNextToken().Kind != ExpressionTokenKind.Comma))
                        {
                            throw ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.text));
                        }
                    }
                    else
                    {
                        throw ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.text));
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
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();

            // FUNCTION := (<ID> {<DOT>}) ... <ID> <OpenParen>
            // if we fail to match then we leave the token as it
            ExpressionTokenKind id = this.token.Kind;
            if (id != ExpressionTokenKind.Identifier)
            {
                return false;
            }

            int savedTextPos = this.textPos;
            char? savedChar = this.ch == null ? (char?)null : this.ch.Value;
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
                this.token.Text = this.text.Substring(tokenStartPos, this.textPos - tokenStartPos);
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
            DebugUtils.CheckNoExternalCallers();

            if (this.token.Kind != t)
            {
                throw ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.text));
            }
        }
        #endregion Internal methods

        #region Private methods
        /// <summary>Creates an exception for a parse error.</summary>
        /// <param name="message">Message text.</param>
        /// <returns>A new Exception.</returns>
        private static Exception ParseError(string message)
        {
            DebugUtils.CheckNoExternalCallers();
            return new ODataException(message);
        }

        /// <summary>Reads the next token, skipping whitespace as necessary.</summary> 
        /// <param name="error">Error that occurred while trying to process the next token.</param>
        /// <returns>The next token, which may be 'bad' if an error occurs.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This parser method is all about the switch statement and would be harder to maintain if it were broken up.")]
        private ExpressionToken NextTokenImplementation(out Exception error)
        {
            DebugUtils.CheckNoExternalCallers();
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
                    bool hasNext = this.textPos + 1 < this.textLen;
                    if (hasNext && Char.IsDigit(this.text[this.textPos + 1]))
                    {
                        this.NextChar();
                        t = this.ParseFromDigit();
                        if (ExpressionLexerUtils.IsNumeric(t))
                        {
                            break;
                        }

                        // If it looked like a numeric but wasn't (because it was a binary 0x... value for example), 
                        // we'll rewind and fall through to a simple '-' token.
                        this.SetTextPos(tokenPos);
                    }
                    else if (hasNext && this.text[tokenPos + 1] == ExpressionConstants.InfinityLiteral[0])
                    {
                        this.NextChar();
                        this.ParseIdentifier();
                        string currentIdentifier = this.text.Substring(tokenPos + 1, this.textPos - tokenPos - 1);

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

                        if (this.textPos == this.textLen)
                        {
                            error = ParseError(ODataErrorStrings.ExpressionLexer_UnterminatedStringLiteral(this.textPos, this.text));
                        }

                        this.NextChar();
                    }
                    while (this.ch.HasValue && (this.ch == quote));
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
                    this.ParseBracketedExpression('{', '}');
                    t = ExpressionTokenKind.BracketedExpression;
                    break;
                case '[':
                    this.ParseBracketedExpression('[', ']');
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
                        t = ExpressionTokenKind.Identifier;
                        break;
                    }

                    if (this.IsValidDigit)
                    {
                        t = this.ParseFromDigit();
                        break;
                    }

                    if (this.textPos == this.textLen)
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

                        if (this.textPos == this.textLen)
                        {
                            error = ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.text));
                            t = ExpressionTokenKind.Unknown;
                            break;
                        }

                        if (!this.IsValidStartingCharForIdentifier)
                        {
                            error = ParseError(ODataErrorStrings.ExpressionLexer_InvalidCharacter(this.ch, this.textPos, this.text));
                            t = ExpressionTokenKind.Unknown;
                            break;
                        }

                        this.ParseIdentifier();
                        t = ExpressionTokenKind.ParameterAlias;
                        break;
                    }

                    error = ParseError(ODataErrorStrings.ExpressionLexer_InvalidCharacter(this.ch, this.textPos, this.text));
                    t = ExpressionTokenKind.Unknown;
                    break;
            }

            this.token.Kind = t;
            this.token.Text = this.text.Substring(tokenPos, this.textPos - tokenPos);
            this.token.Position = tokenPos;

            // Handle type-prefixed literals such as binary, datetime or guid.
            this.HandleTypePrefixedLiterals();

            // Handle keywords.
            if (this.token.Kind == ExpressionTokenKind.Identifier)
            {
                if (ExpressionLexerUtils.IsInfinityOrNaNDouble(this.token.Text))
                {
                    this.token.Kind = ExpressionTokenKind.DoubleLiteral;
                }
                else if (ExpressionLexerUtils.IsInfinityOrNanSingle(this.token.Text))
                {
                    this.token.Kind = ExpressionTokenKind.SingleLiteral;
                }
                else if (this.token.Text == ExpressionConstants.KeywordTrue || this.token.Text == ExpressionConstants.KeywordFalse)
                {
                    this.token.Kind = ExpressionTokenKind.BooleanLiteral;
                }
                else if (this.token.Text == ExpressionConstants.KeywordNull)
                {
                    this.token.Kind = ExpressionTokenKind.NullLiteral;
                }
            }

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

        /// <summary>Handles lexemes that are formed by an identifier followed by a quoted string.</summary>
        /// <remarks>This method modified the token field as necessary.</remarks>
        private void HandleTypePrefixedLiterals()
        {
            ExpressionTokenKind id = this.token.Kind;
            if (id != ExpressionTokenKind.Identifier)
            {
                return;
            }

            bool quoteFollows = this.ch == '\'';
            if (!quoteFollows)
            {
                return;
            }

            string tokenText = this.token.Text;
            if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixDateTime, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.DateTimeLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixDateTimeOffset, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.DateTimeOffsetLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixTime, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.TimeLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixGuid, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.GuidLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixBinary, StringComparison.OrdinalIgnoreCase) ||
                String.Equals(tokenText, ExpressionConstants.LiteralPrefixShortBinary, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.BinaryLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixGeography, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.GeographyLiteral;
            }
            else if (String.Equals(tokenText, ExpressionConstants.LiteralPrefixGeometry, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.GeometryLiteral;
            }
            else if (this.parsingFunctionParameters && string.Equals(tokenText, ExpressionConstants.KeywordNull, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.NullLiteral;
            }
            else
            {
                return;
            }

            int tokenPos = this.token.Position;
            do
            {
                this.NextChar();
            }
            while (this.ch.HasValue && this.ch != '\'');

            if (this.ch == null)
            {
                throw ParseError(ODataErrorStrings.ExpressionLexer_UnterminatedLiteral(this.textPos, this.text));
            }

            this.NextChar();
            this.token.Kind = id;
            this.token.Text = this.text.Substring(tokenPos, this.textPos - tokenPos);
        }

        /// <summary>Advanced to the next character.</summary>
        private void NextChar()
        {
            if (this.textPos < this.textLen)
            {
                this.textPos++;
                if (this.textPos < this.textLen)
                {
                    this.ch = this.text[this.textPos];
                    return;
                }
            }

            this.ch = null;
        }

        /// <summary>Parses a token that starts with a digit.</summary>
        /// <returns>The kind of token recognized.</returns>
        private ExpressionTokenKind ParseFromDigit()
        {
            Debug.Assert(this.IsValidDigit, "this.IsValidDigit");
            ExpressionTokenKind result;
            char startChar = this.ch.Value;
            this.NextChar();
            if (startChar == '0' && this.ch == 'x' || this.ch == 'X')
            {
                result = ExpressionTokenKind.BinaryLiteral;
                do
                {
                    this.NextChar();
                }
                while (this.ch.HasValue && UriPrimitiveTypeParser.IsCharHexDigit(this.ch.Value));
            }
            else
            {
                result = ExpressionTokenKind.IntegerLiteral;
                while (this.IsValidDigit)
                {
                    this.NextChar();
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
            }

            return result;
        }

        /// <summary>
        /// Parses white spaces
        /// </summary>
        private void ParseWhitespace()
        {
            while (this.IsValidWhiteSpace)
            {
                this.NextChar();
            }
        }

        /// <summary>
        /// Parses a complex value
        /// </summary>
        /// <param name="startingCharacter">the starting delimiter</param>
        /// <param name="endingCharacter">the ending delimiter.</param>
        private void ParseBracketedExpression(char startingCharacter, char endingCharacter)
        {
            int currentBracketDepth = 1;
            this.NextChar();

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

        /// <summary>
        /// Advance the pointer to the next occurance of the given value, swallowing all characters in between.
        /// </summary>
        /// <param name="endingValue">the ending delimiter.</param>
        private void AdvanceToNextOccuranceOf(char endingValue)
        {
            this.NextChar();
            while (this.ch.HasValue && this.ch != endingValue)
            {
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
            this.ch = this.textPos < this.textLen ? this.text[this.textPos] : (char?)null;
        }

        /// <summary>Validates the current character is a digit.</summary>
        private void ValidateDigit()
        {
            if (!this.IsValidDigit)
            {
                throw ParseError(ODataErrorStrings.ExpressionLexer_DigitExpected(this.textPos, this.text));
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
