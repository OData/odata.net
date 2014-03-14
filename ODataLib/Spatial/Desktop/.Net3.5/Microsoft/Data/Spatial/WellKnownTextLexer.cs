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
