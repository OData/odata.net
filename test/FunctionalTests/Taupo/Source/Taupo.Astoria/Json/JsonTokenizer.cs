//---------------------------------------------------------------------
// <copyright file="JsonTokenizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Json
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Tokenizes Json strings to objects and exposes the original text as well
    /// </summary>
    public class JsonTokenizer
    {
        /// <summary>
        /// The entire JSON text to parse.
        /// </summary>
        private string json;

        /// <summary>
        /// Index of the last character already parsed (so starts at -1);
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// Index of a character before the start of the token (so starts at -1)
        /// </summary>
        private int tokenStart;

        /// <summary>
        /// Initializes a new instance of the JsonTokenizer class.
        /// </summary>
        /// <param name="reader">The Json reader.</param>
        public JsonTokenizer(TextReader reader)
        {
            ExceptionUtilities.CheckArgumentNotNull(reader, "reader");
            this.json = reader.ReadToEnd();
            ExceptionUtilities.Assert(this.json.Length > 0, "Cannot process empty string");
            this.currentIndex = -1;
            this.tokenStart = -1;
            this.Value = null;
            this.GetNextToken();
        }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public JsonTokenType TokenType { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The token value.</value>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the original text representation of the current token.
        /// This includes any whitespaces before the token. Whitespaces after the token are reported as part of the next token.
        /// </summary>
        public string TokenText
        {
            get
            {
                ExceptionUtilities.Assert(this.currentIndex >= this.tokenStart, "The internal state of the tokenizer is invalid, looks like a token with negative number of characters.");
                return this.json.Substring(this.tokenStart + 1, this.currentIndex - this.tokenStart);
            }
        }

        /// <summary>
        /// Gets the current character the tokenizer is positioned on
        /// </summary>
        private char CurrentChar
        {
            get
            {
                return this.json[this.currentIndex];
            }
        }

        /// <summary>
        /// Gets the next character - lookahead
        /// </summary>
        private char NextChar
        {
            get
            {
                return this.json[this.currentIndex + 1];
            }
        }

        /// <summary>
        /// Creates the marker for current position.
        /// </summary>
        /// <returns>The marker</returns>
        public object CreateMarkerForCurrentPosition()
        {
            return this.currentIndex;
        }

        /// <summary>
        /// Gets the text since marker.
        /// </summary>
        /// <param name="marker">The marker to start from.</param>
        /// <returns>The text</returns>
        public string GetTextSinceMarker(object marker)
        {
            var index = (int)marker;
            return this.json.Substring(index, (this.currentIndex - index));
        }

        /// <summary>
        /// Determines whether the reader has more tokens.
        /// </summary>
        /// <returns>
        /// <c>true</c> if [has more tokens]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMoreTokens()
        {
            this.EatWhiteSpaces();
            return !this.IsNextCharEOF();
        }

        /// <summary>
        /// Gets the next token.
        /// </summary>
        public void GetNextToken()
        {
            this.tokenStart = this.currentIndex;

            this.EatWhiteSpaces();

            if (this.IsNextCharEOF())
            {
                return;
            }

            if (this.NextChar == '"')
            {
                this.ParseString();
                return;
            }

            if (this.NextChar == '[')
            {
                this.ParseStartArray();
                return;
            }

            if (this.NextChar == ']')
            {
                this.ParseEndArray();
                return;
            }

            if (this.NextChar == ':')
            {
                this.ParseColon();
                return;
            }

            if (this.NextChar == ',')
            {
                this.ParseComma();
                return;
            }

            if (this.NextChar == '{')
            {
                this.ParseStartObject();
                return;
            }

            if (this.NextChar == '}')
            {
                this.ParseEndObject();
                return;
            }

            if (this.NextChar == 'n')
            {
                this.ParseNull();
                return;
            }

            if (this.NextChar == 't')
            {
                this.ParseTrue();
                return;
            }

            if (this.NextChar == 'f')
            {
                this.ParseFalse();
                return;
            }

            if (this.NextChar == 'N')
            {
                this.ParseNumberNaN();
                return;
            }

            if (this.NextChar == 'I')
            {
                this.ParseNumberPositiveInfinity();
                return;
            }

            if (this.NextChar == '-')
            {
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                }

                if (this.NextChar == 'I')
                {
                    this.ParseNumberNegativeInfinity();
                }
                else if (this.NextChar == '.' || (this.NextChar >= '0' && this.NextChar <= '9'))
                {
                    this.ParseNegativeNumber();
                }
                else
                {
                    throw new TaupoInvalidOperationException("Invalid token. Expected : decimal point or number. Received: " + this.NextChar);
                }

                return;
            }

            if (this.NextChar == '.' || char.IsDigit(this.NextChar))
            {
                this.ParseNumber();
                return;
            }

            throw new TaupoInvalidOperationException("Invalid start of token found: " + this.NextChar);
        }

        private void EatWhiteSpaces()
        {
            // skip whitespace
            while (!this.IsNextCharEOF() && (char.IsWhiteSpace(this.NextChar) || this.NextChar == '\t' || this.NextChar == '\n' || this.NextChar == '\r'))
            {
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                }
            }
        }

        private bool IsNextCharEOF()
        {
            return this.currentIndex + 1 >= this.json.Length;
        }

        private bool IsCurrentCharEOF()
        {
            return this.currentIndex >= this.json.Length;
        }

        private void ParseFalse()
        {
            this.AssertToken("false");
            this.TokenType = JsonTokenType.False;
            this.Value = false;
        }

        private void ParseTrue()
        {
            this.AssertToken("true");
            this.TokenType = JsonTokenType.True;
            this.Value = true;
        }

        private void ParseNull()
        {
            this.AssertToken("null");
            this.TokenType = JsonTokenType.Null;
            this.Value = null;
        }

        private void ParseEndObject()
        {
            if (!this.IsCurrentCharEOF())
            {
                this.currentIndex++;
                ExceptionUtilities.Assert(this.CurrentChar == '}', "Expected to consume }");
            }

            this.TokenType = JsonTokenType.RightCurly;
            this.Value = null;
        }

        private void ParseStartObject()
        {
            if (!this.IsCurrentCharEOF())
            {
                this.currentIndex++;
                ExceptionUtilities.Assert(this.CurrentChar == '{', "Expected to consume {");
            }

            this.TokenType = JsonTokenType.LeftCurly;
            this.Value = null;
        }

        private void ParseComma()
        {
            if (!this.IsCurrentCharEOF())
            {
                this.currentIndex++;
                ExceptionUtilities.Assert(this.CurrentChar == ',', "Expected to consume ,");
            }

            this.TokenType = JsonTokenType.Comma;
            this.Value = null;
        }

        private void ParseColon()
        {
            if (!this.IsCurrentCharEOF())
            {
                this.currentIndex++;
                ExceptionUtilities.Assert(this.CurrentChar == ':', "Expected to consume :");
            }

            this.TokenType = JsonTokenType.Colon;
            this.Value = null;
        }

        private void ParseEndArray()
        {
            if (!this.IsCurrentCharEOF())
            {
                this.currentIndex++;
                ExceptionUtilities.Assert(this.CurrentChar == ']', "Expected to consume ]");
            }

            this.TokenType = JsonTokenType.RightSquareBracket;
            this.Value = null;
        }

        private void ParseStartArray()
        {
            if (!this.IsCurrentCharEOF())
            {
                this.currentIndex++;
                ExceptionUtilities.Assert(this.CurrentChar == '[', "Expected to consume [");
            }

            this.TokenType = JsonTokenType.LeftSquareBracket;
            this.Value = null;
        }

        private void ParseString()
        {
            string text = this.ReadString();
            this.TokenType = JsonTokenType.String;
            this.Value = text;
        }

        private string ReadString()
        {
            string text = string.Empty;

            // consume the start of sting i.e the double quote
            if (!this.IsCurrentCharEOF())
            {
                this.currentIndex++;
                ExceptionUtilities.Assert(this.CurrentChar == '"', "Expected to consume \"");
            }

            // the nextChar is at the first character in the string
            while (!this.IsNextCharEOF() && this.NextChar != '"' && !this.IsCurrentCharEOF())
            {
                if (this.NextChar == '\\')
                {
                    if (!this.IsCurrentCharEOF())
                    {
                        this.currentIndex++;
                        ExceptionUtilities.Assert(this.CurrentChar == '\\', "Expected to consume \\");
                    }

                    text = this.ReadEscapeSequence(text);
                }
                else if (this.NextChar == '\'')
                {
                    text += (char)this.NextChar;
                    if (!this.IsCurrentCharEOF())
                    {
                        this.currentIndex++;
                        ExceptionUtilities.Assert(this.CurrentChar == '\'', "Expected to consume '");
                    }
                }
                else
                {
                    // any other character
                    text += (char)this.NextChar;

                    if (!this.IsCurrentCharEOF())
                    {
                        this.currentIndex++;
                    }
                }
            }

            if (!this.IsCurrentCharEOF())
            {
                if (this.IsNextCharEOF())
                {
                    throw new TaupoInvalidOperationException("ASSERTION FAILED: Invalid termination of string");
                }

                ExceptionUtilities.Assert(this.NextChar == '"', "Invalid termination of string");
                this.currentIndex++;
            }

            return text;
        }

        private string ReadEscapeSequence(string text)
        {
            if (this.IsNextCharEOF())
            {
                throw new TaupoInvalidOperationException("Unexpected End of File");
            }
            else if (this.NextChar == 'b')
            {
                text += (char)'\b';
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                    ExceptionUtilities.Assert(this.CurrentChar == 'b', "Expected to consume b");
                }
            }
            else if (this.NextChar == 't')
            {
                text += (char)'\t';
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                    ExceptionUtilities.Assert(this.CurrentChar == 't', "Expected to consume t");
                }
            }
            else if (this.NextChar == 'n')
            {
                text += (char)'\n';
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                    ExceptionUtilities.Assert(this.CurrentChar == 'n', "Expected to consume n");
                }
            }
            else if (this.NextChar == 'f')
            {
                text += (char)'\f';
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                    ExceptionUtilities.Assert(this.CurrentChar == 'f', "Expected to consume f");
                }
            }
            else if (this.NextChar == 'v')
            {
                text += (char)'\v';
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                    ExceptionUtilities.Assert(this.CurrentChar == 'v', "Expected to consume v");
                }
            }
            else if (this.NextChar == 'r')
            {
                text += (char)'\r';
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                    ExceptionUtilities.Assert(this.CurrentChar == 'r', "Expected to consume r");
                }
            }
            else if (this.NextChar == '\\')
            {
                text += (char)'\\';
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                    ExceptionUtilities.Assert(this.CurrentChar == '\\', "Expected to consume \\");
                }
            }
            else if (this.NextChar == '"' || this.NextChar == '\'' || this.NextChar == '/')
            {
                text += (char)this.NextChar; // no need to escape these in json.
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                }
            }
            else if (this.NextChar == 'u')
            {
                char[] hexValues = this.ParseHexValue();
                char hexChar = Convert.ToChar(int.Parse(new string(hexValues), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                text += (char)hexChar;
            }
            else
            {
                throw new TaupoInvalidOperationException("Bad Json escape sequence: \\" + this.CurrentChar);
            }

            return text;
        }

        private char[] ParseHexValue()
        {
            if (!this.IsCurrentCharEOF())
            {
                this.currentIndex++;
                ExceptionUtilities.Assert(this.CurrentChar == 'u', "Expected to consume u");
            }

            char[] hexValues = new char[4];
            for (int i = 0; i < hexValues.Length; i++)
            {
                if (!this.IsNextCharEOF() && !this.IsSeparatorToken(this.NextChar) && this.NextChar != '"')
                {
                    hexValues[i] = this.NextChar;
                    if (!this.IsCurrentCharEOF())
                    {
                        this.currentIndex++;
                    }
                }
                else
                {
                    throw new TaupoInvalidOperationException("Unexpected end while parsing unicode character.");
                }
            }

            return hexValues;
        }

        private bool IsSeparatorToken(char c)
        {
            if (c == '}' || c == ']' || c == ',')
            {
                return true;
            }
            else if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ParseNegativeNumber()
        {
            // the '-' has already been consumed. So now we will parse the number and negate it.
            this.ParseNumber();
            if (this.TokenType == JsonTokenType.Integer)
            {
                this.Value = -(long)this.Value;
            }
            else if (this.TokenType == JsonTokenType.Float)
            {
                this.Value = -(double)this.Value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Silverlight does not have the overload ToUpperInvariant()")]
        private void ParseNumber()
        {
            string number = null;
            bool nonBase10 = this.NextChar == '0';
            bool isNextCharNumberEnd = this.IsSeparatorToken(this.NextChar); // will always be false

            // parse until seperator character or end
            while (!isNextCharNumberEnd && !this.IsNextCharEOF())
            {
                // consume digit
                if (!this.IsCurrentCharEOF())
                {
                    this.currentIndex++;
                }

                number += this.CurrentChar;
                if (!this.IsNextCharEOF() && this.IsSeparatorToken(this.NextChar))
                {
                    isNextCharNumberEnd = true;
                }
            }

            object numberValue;

            if (number.Contains(".") || number.ToLowerInvariant().Contains("e"))
            {
                numberValue = Convert.ToDouble(number, CultureInfo.InvariantCulture);
                this.TokenType = JsonTokenType.Float;
            }
            else if (nonBase10)
            {
                // handles numbers like (octal/base 8) 0372 to which translates to 250 in decimal
                // handle numbers like (hexadecimal/base 16) 0XFA which translates to 250 in decimal
                numberValue = number.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                  ? Convert.ToInt64(number, 16)
                  : Convert.ToInt64(number, 8);
                this.TokenType = JsonTokenType.Integer;
            }
            else
            {
                numberValue = Convert.ToInt64(number, CultureInfo.InvariantCulture);
                this.TokenType = JsonTokenType.Integer;
            }

            this.Value = numberValue;
        }

        private void ParseNumberNegativeInfinity()
        {
            this.AssertToken("Infinity");
            this.TokenType = JsonTokenType.Integer;
            this.Value = double.NegativeInfinity;
        }

        private void AssertToken(string value)
        {
            this.EatWhiteSpaces();
            ExceptionUtilities.Assert((this.currentIndex + value.Length) < this.json.Length, "Match for " + value + " not found");
            ExceptionUtilities.Assert(this.json.Substring(this.currentIndex + 1, value.Length).Equals(value, StringComparison.Ordinal), "Match for " + value + " not found");
            this.currentIndex += value.Length;
            ExceptionUtilities.Assert((!this.IsNextCharEOF() && this.IsSeparatorToken(this.NextChar)) || this.IsNextCharEOF(), "Match for " + value + " not found");
        }

        private void ParseNumberPositiveInfinity()
        {
            this.AssertToken("Infinity");
            this.TokenType = JsonTokenType.Integer;
            this.Value = double.PositiveInfinity;
        }

        private void ParseNumberNaN()
        {
            this.AssertToken("NaN");
            this.TokenType = JsonTokenType.Integer;
            this.Value = double.NaN;
        }
    }
}
