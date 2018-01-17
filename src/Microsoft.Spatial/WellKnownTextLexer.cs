//---------------------------------------------------------------------
// <copyright file="WellKnownTextLexer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.IO;

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
