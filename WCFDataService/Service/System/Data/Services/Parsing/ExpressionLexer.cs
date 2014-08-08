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

namespace System.Data.Services.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    /// <summary>Use this class to parse an expression in the Astoria URI format.</summary>
    /// <remarks>
    /// Literals (non-normative "handy" reference - see spec for correct expression):
    /// Null        null
    /// Boolean     true | false
    /// Int32       (digit+)
    /// Int64       (digit+)(L|l)
    /// Decimal     (digit+ ['.' digit+])(M|m)
    /// Float       (digit+ ['.' digit+][e|E [+|-] digit+)(f|F)
    /// Double      (digit+ ['.' digit+][e|E [+|-] digit+)
    /// String      "'" .* "'"
    /// DateTime    datetime"'"dddd-dd-dd[T|' ']dd:mm[ss[.fffffff]]"'"
    /// Binary      (binary|X)'digit*'
    /// GUID        guid'digit*'
    /// </remarks>
    [DebuggerDisplay("ExpressionLexer ({text} @ {textPos} [{token}]")]
    internal class ExpressionLexer
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
        internal ExpressionLexer(string expression)
        {
            Debug.Assert(expression != null, "expression != null");

            this.ignoreWhitespace = true;
            this.text = expression;
            this.textLen = this.text.Length;
            this.SetTextPos(0);
            this.NextToken();
        }

        #endregion Constructors

        #region Internal properties

        /// <summary>Token being processed.</summary>
        internal ExpressionToken CurrentToken
        {
            get { return this.token; }
            set { this.token = value; }
        }

        /// <summary>Text being parsed.</summary>
        internal string ExpressionText
        {
            get { return this.text; }
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
                    Char.GetUnicodeCategory(this.ch.Value) == UnicodeCategory.LetterNumber;
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
                    Char.IsLetterOrDigit(this.ch.Value) ||    // covers: Ll, Lu, Lt, Lo, Lm, Nd
                    AdditionalUnicodeCategoriesForIdentifier.Contains(char.GetUnicodeCategory(this.ch.Value));  // covers the rest
            }
        }
        #endregion

        #region Internal methods.
        /// <summary>Reads the next token, skipping whitespace as necessary, advancing the Lexer.</summary>
        /// <remarks>Throws on error.</remarks>
        internal void NextToken()
        {
            this.NextTokenImplementation();
        }

        /// <summary>
        /// Starting from an identifier, reads a sequence of dots and 
        /// identifiers, and returns the text for it, with whitespace 
        /// stripped.
        /// </summary>
        /// <returns>The dotted identifier starting at the current identifie.</returns>
        internal string ReadDottedIdentifier()
        {
            return this.ReadDottedIdentifier(false /*allowEndWithDotStar*/);
        }

        /// <summary>
        /// Starting from an identifier, reads a sequence of dots and identifiers, 
        /// and can end in a star, and returns the text for it, with whitespace 
        /// stripped.
        /// Ex: identifier1.identifier2, identifier1.*, etc.
        /// </summary>
        /// <param name="allowEndWithDotStar">If true, the dotted identifier may end in .*; if false, the dotted identifier must not end in *.</param>
        /// <returns>The dotted identifier starting at the current identifier.</returns>
        internal string ReadDottedIdentifier(bool allowEndWithDotStar)
        {
            this.ValidateToken(ExpressionTokenKind.Identifier);
            StringBuilder builder = null;
            string result = this.CurrentToken.Text;
            this.NextToken();
            bool seenStar = false;
            while (this.CurrentToken.Kind == ExpressionTokenKind.Dot)
            {
                if (seenStar)
                {
                    throw ParseError(Strings.RequestQueryParser_SyntaxError);
                }

                this.NextToken();
                if (allowEndWithDotStar && this.CurrentToken.Kind == ExpressionTokenKind.Star)
                {
                    seenStar = true;
                }
                else
                {
                    this.ValidateToken(ExpressionTokenKind.Identifier);
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
            int savedTextPos = this.textPos;
            char? savedChar = (this.ch == null) ? (char?)null : this.ch.Value;
            ExpressionToken savedToken = this.token;

            this.NextToken();
            ExpressionToken result = this.token;

            this.textPos = savedTextPos;
            this.ch = savedChar;
            this.token = savedToken;

            return result;
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
            char? savedChar = (this.ch == null) ? (char?)null : this.ch.Value;
            ExpressionToken savedToken = this.token;
            bool savedIgnoreWs = this.ignoreWhitespace;
            this.ignoreWhitespace = false;

            // Expansion left anchor
            int tokenStartPos = this.token.Position;

            while (this.ExpandWhenMatch(ExpressionTokenKind.Dot) && this.ExpandWhenMatch(ExpressionTokenKind.Identifier))
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
            if (this.token.Kind != t)
            {
                throw ParseError(Strings.RequestQueryParser_SyntaxError);
            }
        }
        #endregion Internal methods

        #region Private methods
        /// <summary>Creates an exception for a parse error.</summary>
        /// <param name="message">Message text.</param>
        /// <returns>A new Exception.</returns>
        private static Exception ParseError(string message)
        {
            return DataServiceException.CreateSyntaxError(message);
        }

        /// <summary>Reads the next token, skipping whitespace as necessary.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This parser method is all about the switch statement and would be harder to maintain if it were broken up.")]
        private void NextTokenImplementation()
        {
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

                        if (ExpressionLexerUtils.IsInfinityLiteralSingle(currentIdentifier))
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
                        this.NextChar();
                        while (this.textPos < this.textLen && this.ch != quote)
                        {
                            this.NextChar();
                        }

                        if (this.textPos == this.textLen)
                        {
                            throw ParseError(Strings.RequestQueryParser_UnterminatedStringLiteral(this.text));
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
                case ';':
                    this.NextChar();
                    t = ExpressionTokenKind.Semicolon;
                    break;
                case '$':
                    this.NextChar();
                    if (this.ch == 'i')
                    {
                        this.ParseIdentifier();

                        // check if this is the special $it parameter
                        if (this.textPos - tokenPos == 3 && this.text[tokenPos + 2] == 't')
                        {
                            t = ExpressionTokenKind.Identifier;
                            break;
                        }
                    }

                    throw ParseError(Strings.RequestQueryParser_InvalidCharacter('$'));
                default:
                    if (this.IsValidWhiteSpace)
                    {
                        Debug.Assert(!this.ignoreWhitespace, "should not hit ws while ignoring it");
                        this.ParseWhitespace();
                        t = ExpressionTokenKind.WhiteSpace;
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

                    throw ParseError(Strings.RequestQueryParser_InvalidCharacter(this.ch));
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
        }

        /// <summary>
        /// Expand the token selection if the next token matches the input token
        /// </summary>
        /// <param name="id">the list of token id to match</param>
        /// <returns>true if matched</returns>
        private bool ExpandWhenMatch(ExpressionTokenKind id)
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
            if (String.Equals(tokenText, XmlConstants.LiteralPrefixDateTime, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.DateTimeLiteral;
            }
            else if (String.Equals(tokenText, XmlConstants.LiteralPrefixGuid, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.GuidLiteral;
            }
            else if (String.Equals(tokenText, XmlConstants.LiteralPrefixBinary, StringComparison.OrdinalIgnoreCase) || tokenText == "X" || tokenText == "x")
            {
                id = ExpressionTokenKind.BinaryLiteral;
            }
            else if (String.Equals(tokenText, XmlConstants.LiteralPrefixGeography, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.GeographylLiteral;
            }
            else if (String.Equals(tokenText, XmlConstants.LiteralPrefixGeometry, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.GeometryLiteral;
            }
            else if (String.Equals(tokenText, XmlConstants.LiteralPrefixTime, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.TimeLiteral;
            }
            else if (String.Equals(tokenText, XmlConstants.LiteralPrefixDateTimeOffset, StringComparison.OrdinalIgnoreCase))
            {
                id = ExpressionTokenKind.DateTimeOffsetLiteral;
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
                throw ParseError(Strings.RequestQueryParser_UnterminatedLiteral(this.text));
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
                while (this.ch.HasValue && WebConvert.IsCharHexDigit(this.ch.Value));
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
                else
                    if (this.ch == 'd' || this.ch == 'D')
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
                throw ParseError(Strings.RequestQueryParser_DigitExpected);
            }
        }

        #endregion Private methods.
    }
}
