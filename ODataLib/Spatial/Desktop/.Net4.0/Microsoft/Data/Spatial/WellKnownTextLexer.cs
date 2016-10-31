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

namespace Microsoft.Data.Spatial
{
    using System;
    using System.IO;
    using System.Spatial;

    /// <summary>
    /// WellKnownText Lexer
    /// </summary>
    internal class WellKnownTextLexer : TextLexerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Input text</param>
        public WellKnownTextLexer(TextReader text)
            : base(text)
        {
        }

        /// <summary>
        /// Examine the current character and determine its token type
        /// </summary>
        /// <param name="nextChar">The next char that will be read.</param>
        /// <param name="activeTokenType">The currently active token type</param>
        /// <param name="tokenType">The matched token type</param>
        /// <returns>Whether the current character is a delimiter, thereby terminate the current token immediately</returns>
        protected override bool MatchTokenType(char nextChar, int? activeTokenType, out int tokenType)
        {
            switch (nextChar)
            {
                case '=':
                    tokenType = (int)WellKnownTextTokenType.Equals;
                    return true;
                case ';':
                    tokenType = (int)WellKnownTextTokenType.Semicolon;
                    return true;
                case '(':
                    tokenType = (int)WellKnownTextTokenType.LeftParen;
                    return true;
                case ')':
                    tokenType = (int)WellKnownTextTokenType.RightParen;
                    return true;
                case '.':
                    tokenType = (int)WellKnownTextTokenType.Period;
                    return true;
                case ',':
                    tokenType = (int)WellKnownTextTokenType.Comma;
                    return true;
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    tokenType = (int)WellKnownTextTokenType.WhiteSpace;
                    return false;
                case '-':
                case '+':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    tokenType = (int)WellKnownTextTokenType.Number;
                    return false;
                
                // E is special because of exponents
                case 'e':
                case 'E':
                    if (activeTokenType == (int)WellKnownTextTokenType.Number)
                    {
                        tokenType = (int)WellKnownTextTokenType.Number;
                    }
                    else
                    {
                        tokenType = (int)WellKnownTextTokenType.Text;
                    }

                    return false;
                default:
                    if ((nextChar >= 'A' && nextChar <= 'Z') || nextChar >= 'a' && nextChar <= 'z')
                    {
                        tokenType = (int)WellKnownTextTokenType.Text;
                        return false;
                    }

                    throw new FormatException(Strings.WellKnownText_UnexpectedCharacter(nextChar));
            }
        }
    }
}
