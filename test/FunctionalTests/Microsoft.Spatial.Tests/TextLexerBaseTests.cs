//---------------------------------------------------------------------
// <copyright file="TextLexerBaseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Data.Spatial;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class TextLexerBaseTests
    {
        internal enum TestTokenType : int
        {
            Space = 0,
            LowerCaseText = 1,
            UpperCaseText = 2,
            Comma,
        }

        internal class TestTextLexer : TextLexerBase
        {
            public TestTextLexer(string text)
                : base(new System.IO.StringReader(text))
            {

            }

            protected override bool MatchTokenType(char nextChar, int? activeTokenType, out int tokenType)
            {
                if (nextChar == ' ')
                {
                    tokenType = (int)TestTokenType.Space;
                    return false;
                }
                else if (nextChar >= 'a' && nextChar <= 'z')
                {
                    tokenType = (int)TestTokenType.LowerCaseText;
                    return false;
                }
                else if (nextChar >= 'A' && nextChar <= 'Z')
                {
                    tokenType = (int)TestTokenType.UpperCaseText;
                    return false;
                }
                else if (nextChar == ',')
                {
                    tokenType = (int)TestTokenType.Comma;
                    return true;
                }
                else
                {
                    tokenType = -1;
                    return false;
                }
            }
        }

        [Fact]
        public void TestLexerNext_Simple()
        {
            this.TestLexerNext("AB cd",
                new LexerToken() { Text = "AB", Type = (int)TestTokenType.UpperCaseText },
                new LexerToken() { Text = " ", Type = (int)TestTokenType.Space },
                new LexerToken() { Text = "cd", Type = (int)TestTokenType.LowerCaseText });
        }

        [Fact]
        public void TestLexerNext_Delimiter()
        {
            this.TestLexerNext(",,",
                new LexerToken() { Text = ",", Type = (int)TestTokenType.Comma },
                new LexerToken() { Text = ",", Type = (int)TestTokenType.Comma });

            this.TestLexerNext("AB,,cd  ,ee",
                new LexerToken() { Text = "AB", Type = (int)TestTokenType.UpperCaseText },
                new LexerToken() { Text = ",", Type = (int)TestTokenType.Comma },
                new LexerToken() { Text = ",", Type = (int)TestTokenType.Comma },
                new LexerToken() { Text = "cd", Type = (int)TestTokenType.LowerCaseText },
                new LexerToken() { Text = "  ", Type = (int)TestTokenType.Space },
                new LexerToken() { Text = ",", Type = (int)TestTokenType.Comma },
                new LexerToken() { Text = "ee", Type = (int)TestTokenType.LowerCaseText });

            this.TestLexerNext(",cdAB",
                               new LexerToken() {Text = ",", Type = (int) TestTokenType.Comma},
                               new LexerToken() { Text = "cd", Type = (int)TestTokenType.LowerCaseText },
                               new LexerToken() {Text = "AB", Type = (int) TestTokenType.UpperCaseText});
        }

        [Fact]
        public void TestLexerNext_Empty()
        {
            this.TestLexerNext("");
        }

        [Fact]
        public void TestLexerNext_Single()
        {
            this.TestLexerNext(" ", new LexerToken() { Text = " ", Type = (int)TestTokenType.Space });
        }

        private void TestLexerNext(String text, params LexerToken[] expected)
        {
            TextLexerBase lex = new TestTextLexer(text);

            LexerToken peekToken;

            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.True(lex.Peek(out peekToken));
                this.TestTokenEqual(expected[i], peekToken);

                Assert.True(lex.Peek(out peekToken));
                this.TestTokenEqual(expected[i], peekToken);

                Assert.True(lex.Next());
                this.TestTokenEqual(expected[i], lex.CurrentToken);
            }

            Assert.False(lex.Peek(out peekToken));
            Assert.False(lex.Next());
        }

        private void TestTokenEqual(LexerToken token, LexerToken other)
        {
            Assert.Equal(token.Text, other.Text);
            Assert.Equal(token.Type, other.Type);
        }
    }
}
