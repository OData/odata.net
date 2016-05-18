//---------------------------------------------------------------------
// <copyright file="TextLexerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Lexer base
    /// </summary>
    internal abstract class TextLexerBase
    {
        /// <summary>
        /// Input text
        /// </summary>
        private TextReader reader;

        /// <summary>
        /// Current lexer output
        /// </summary>
        private LexerToken currentToken;

        /// <summary>
        /// Peek lexer output, if this is not null then we have advanced already
        /// </summary>
        private LexerToken peekToken;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">The input text</param>
        protected TextLexerBase(TextReader text)
        {
            this.reader = text;
        }

        /// <summary>
        /// Current token
        /// </summary>
        public LexerToken CurrentToken
        {
            get { return this.currentToken; }
        }

        /// <summary>
        /// Peek one token ahead of the current position
        /// </summary>
        /// <param name="token">The peeked token</param>
        /// <returns>True if there is one more token after the current position, otherwise false</returns>
        public bool Peek(out LexerToken token)
        {
            // read but don't advance
            // implementation: actually advance the lexer
            // but make sure Peek/Next call returns the same token
            if (this.peekToken != null)
            {
                // we've already peeked
                token = this.peekToken;
                return true;
            }

            // store current token
            LexerToken temp = this.currentToken;

            // advance parser
            if (this.Next())
            {
                this.peekToken = this.currentToken;
                token = this.currentToken;

                // restore current state
                this.currentToken = temp;
                return true;
            }
            else
            {
                this.peekToken = null;
                token = null;

                // restore current state
                this.currentToken = temp;
                return false;
            }
        }

        /// <summary>
        /// Move to the next token
        /// </summary>
        /// <returns>True if lexer has moved, otherwise false</returns>
        public bool Next()
        {
            // DEVNOTE(pqian):
            // This function is called a very large number of times during text parsing.
            // For example, experiments show that during simple WKT parsing, this function accounts for over 70%
            // of the workload. Thus, efforts should be taken to keep this function as light weight as possible.
            // Token Accumulation Logic:
            // we'll accumulate whenever we are:
            // 1a. starting fresh, hence currentType is null.
            // 1b. we are inside a token that's building, and the new character is still part of the same token.
            // we should break out of the loop if:
            // 2a. MatchTokenType tell us to (terminate = true)
            // 2b. We are inside a token and we've encountered a new token (currentType and peek type is different)
            // It follows that most of the time we will either accumulate the token or break out of the loop
            // to return the token, except in one case, where we are starting fresh AND the first character is a terminal
            // character (delimeter). In this case we will both accumulate that char and break out of the loop.
            if (this.peekToken != null)
            {
                this.currentToken = this.peekToken;
                this.peekToken = null;
                return true;
            }

            LexerToken originalToken = this.CurrentToken;
            int? currentTokenType = null;
            int textValue;
            StringBuilder nextTokenText = null;
            bool isDelimiter = false;

            while (!isDelimiter && (textValue = this.reader.Peek()) >= 0)
            {
                char currentChar = (char)textValue;
                int newTokenType;
                isDelimiter = this.MatchTokenType(currentChar, currentTokenType, out newTokenType);

                if (!currentTokenType.HasValue)
                {
                    // fresh token
                    currentTokenType = newTokenType;
                    nextTokenText = new StringBuilder();
                    nextTokenText.Append(currentChar);
                    this.reader.Read();
                }
                else
                {
                    // existing token
                    if (currentTokenType == newTokenType)
                    {
                        // continuation of the current token
                        nextTokenText.Append(currentChar);
                        this.reader.Read();
                    }
                    else
                    {
                        // starting a new token
                        isDelimiter = true;
                    }
                }
            }

            // we got here due to end of stream, could still have unprocessed tokens
            if (currentTokenType.HasValue)
            {
                Debug.Assert(nextTokenText != null, "Token text should not be null if current Token type has value");
                this.currentToken = new LexerToken() { Text = nextTokenText.ToString(), Type = currentTokenType.Value };
            }

            return originalToken != this.currentToken;
        }

        /// <summary>
        /// Examine the current character and determine its token type
        /// </summary>
        /// <param name="nextChar">The char that will be read next</param>
        /// <param name="currentType">The currently active token type</param>
        /// <param name="type">The matched token type</param>
        /// <returns>Whether the current character is a delimiter, thereby terminate the current token immediately</returns>
        protected abstract bool MatchTokenType(char nextChar, int? currentType, out int type);
    }
}
