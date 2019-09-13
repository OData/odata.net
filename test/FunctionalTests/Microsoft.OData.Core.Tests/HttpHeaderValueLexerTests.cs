//---------------------------------------------------------------------
// <copyright file="HttpHeaderValueLexerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class HttpHeaderValueLexerTests
    {
        [Fact]
        public void CreateWithNullValueShouldReturnHttpHeaderStart()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Start, HttpHeaderValueLexer.Create("headerName", null).Type);
        }

        [Fact]
        public void CreateWithEmptyValueShouldReturnHttpHeaderStart()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Start, HttpHeaderValueLexer.Create("headerName", "").Type);
        }

        [Fact]
        public void CreateShouldReturnHttpHeaderStart()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Start, HttpHeaderValueLexer.Create("headerName", "token").Type);
        }

        [Fact]
        public void StartCannotTransitionToQuotedString()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", "\"quoted-string\"").ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString("headerName", "\"quoted-string\"", 0));
        }

        [Fact]
        public void StartCannotTrnasistionToElementSeparator()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", ",").ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", ",", 0));            
        }

        [Fact]
        public void StartCannotTrnasistionToParameterSeparator()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", ";").ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", ";", 0));
        }

        [Fact]
        public void StartCannotTrnasistionToValueSeparator()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", "=").ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", "=", 0));
        }

        [Fact]
        public void StartCanTransitionToToken()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Token, HttpHeaderValueLexer.Create("headerName", "token").ReadNext().Type);
        }

        [Fact]
        public void StartCanTransitionToEnd()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.End, HttpHeaderValueLexer.Create("headerName", " ").ReadNext().Type);
        }

        [Fact]
        public void TokenCanTransitionToElementSeparator()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator, HttpHeaderValueLexer.Create("headerName", "token,").ReadNext().ReadNext().Type);
        }

        [Fact]
        public void TokenCanTransitionToParameterSeparator()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator, HttpHeaderValueLexer.Create("headerName", "token ;").ReadNext().ReadNext().Type);
        }

        [Fact]
        public void TokenCanTransitionToValueSeparator()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator, HttpHeaderValueLexer.Create("headerName", "token  =  ").ReadNext().ReadNext().Type);
        }

        [Fact]
        public void TokenCanTransitionToEnd()
        {
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.End, HttpHeaderValueLexer.Create("headerName", "token").ReadNext().ReadNext().Type);
        }

        [Fact]
        public void TokenCannotNotTransitionToQuotedString()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", "token\"quotes-string\"").ReadNext().ReadNext();
            test.Throws<ODataException>(Strings.HttpUtils_EscapeCharWithoutQuotes("headerName", "token\"quotes-string\"", 5, "\""));
        }

        [Fact]
        public void QuotedStringCanTransitionToEnd()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=\"quotes-string\"").ReadNext().ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString, lexer.Type);
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.End, lexer.ReadNext().Type);
        }

        [Fact]
        public void QuotedStringCanTransitionToElementSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token = \"quotes-string\" ,").ReadNext().ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString, lexer.Type);
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator, lexer.ReadNext().Type);
        }

        [Fact]
        public void QuotedStringCanTransitionToParameterSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=   \"quotes-string\";").ReadNext().ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString, lexer.Type);
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator, lexer.ReadNext().Type);
        }

        [Fact]
        public void QuotedStringCannotTransitionToValueSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=\"quotes-string\"=").ReadNext().ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString("headerName", "token=\"quotes-string\"=", 21, "="));
        }

        [Fact]
        public void QuotesStringCannotTransitionToToken()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=\"quotes-string\"token").ReadNext().ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_UnrecognizedSeparator("headerName", "token=\"quotes-string\"token", 21, "t"));            
        }

        [Fact]
        public void QuotedStringCannotTransitionToQuotedString()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=\"quotes-string\"\"quotes-string\"").ReadNext().ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_UnrecognizedSeparator("headerName", "token=\"quotes-string\"\"quotes-string\"", 21, "\""));                        
        }

        [Fact]
        public void ElementSeparatorCannotTransitionToEnd()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token,").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_EndOfFileAfterSeparator("headerName", "token,", 6, ","));
        }

        [Fact]
        public void ParameterSeparatorCannotTransitionToEnd()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token;").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_EndOfFileAfterSeparator("headerName", "token;", 6, ";"));
        }

        [Fact]
        public void ValueSeparatorCannotTransitionToEnd()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_EndOfFileAfterSeparator("headerName", "token=", 6, "="));
        }

        [Fact]
        public void ValueSeparatorCanTransitionToToken()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token = token").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator, lexer.Type);
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Token, lexer.ReadNext().Type);
        }

        [Fact]
        public void ValueSeparatorCanTransitionToQuotedString()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token= \"quoted-string\"").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator, lexer.Type);
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString, lexer.ReadNext().Type);
        }

        [Fact]
        public void ValueSeparatorCannotTransitionToSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token= =").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", "token= =", 7));
        }

        [Fact]
        public void ElementSeparatorCanTransitionToToken()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token,token").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator, lexer.Type);
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Token, lexer.ReadNext().Type);
        }

        [Fact]
        public void ElementSeparatorCannotTransitionToQuotedString()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token,\"quoted-string\"").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString("headerName", "token,\"quoted-string\"", 6));
        }

        [Fact]
        public void ElementSeparatorCannotTransitionToSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token,,").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", "token,,", 6));
        }

        [Fact]
        public void ParameterSeparatorCanTransitionToToken()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token  ;   token").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator, lexer.Type);
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Token, lexer.ReadNext().Type);
        }

        [Fact]
        public void ParameterSeparatorCannotTransitionToQuotedString()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token;\"quoted-string\"").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString("headerName", "token;\"quoted-string\"", 6));
        }

        [Fact]
        public void ParameterSeparatorCannotTransitionToSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token;;").ReadNext().ReadNext();
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator, lexer.Type);
            Action test = () => lexer.ReadNext();
            test.Throws<ODataException>(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", "token;;", 6));
        }

        [Fact]
        public void HttpHeaderValueEndToHttpHeaderValueElementsShouldReturnEmptyDictionary()
        {
            var lexer = HttpHeaderValueLexer.Create("header", null);
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Start, lexer.Type);
            Assert.Empty(lexer.ToHttpHeaderValue());
        }

        [Fact]
        public void EmptyHeaderValueToHttpHeaderValueElementsShouldReturnEmptyDictionary()
        {
            var lexer = HttpHeaderValueLexer.Create("header", " ");
            Assert.Equal(HttpHeaderValueLexer.HttpHeaderValueItemType.Start, lexer.Type);
            Assert.Empty(lexer.ToHttpHeaderValue());
        }

        [Fact]
        public void ToHttpHeaderValueElementsShouldReturnDictionaryContainingAllElements()
        {
            HttpHeaderValueElement e1 = new HttpHeaderValueElement("e1", null, new KeyValuePair<string, string>[0]);
            HttpHeaderValueElement e2 = new HttpHeaderValueElement("e2", "token", new KeyValuePair<string, string>[0]);
            HttpHeaderValueElement e3 = new HttpHeaderValueElement("e3", "\"quoted-string\"", new KeyValuePair<string, string>[0]);
            HttpHeaderValueElement e4 = new HttpHeaderValueElement("e4", null, new[] { new KeyValuePair<string, string>("p1", "v1"), new KeyValuePair<string, string>("p2", null) });
            HttpHeaderValueElement e5 = new HttpHeaderValueElement("e5", "token", new[] {new KeyValuePair<string, string>("p1", null), new KeyValuePair<string, string>("p2", "\"v2\"")});
            HttpHeaderValueElement e6 = new HttpHeaderValueElement("e6", "\"quoted-string\"", new[] {new KeyValuePair<string, string>("p1", null), new KeyValuePair<string, string>("p2", null), new KeyValuePair<string, string>("p3", "v3"), new KeyValuePair<string, string>("p4", "\"v4\"")});
            const string eStr1 = "e1";
            const string eStr2 = "e2=token";
            const string eStr3 = "e3=\"quoted-string\"";
            const string eStr4 = "e4;p1=v1;p2";
            const string eStr5 = "e5=token;p1;p2=\"v2\"";
            const string eStr6 = "e6=\"quoted-string\";p1;p2;p3=v3;p4=\"v4\"";

            string headerValue = string.Join(",", eStr1, eStr2, eStr3, eStr4, eStr5, eStr6);
            var lexer = HttpHeaderValueLexer.Create("header", headerValue);
            var elements = lexer.ToHttpHeaderValue();
            Assert.Equal(6, elements.Count);
            AssertEqual(e1, elements["e1"]);
            AssertEqual(e2, elements["e2"]);
            AssertEqual(e3, elements["e3"]);
            AssertEqual(e4, elements["e4"]);
            AssertEqual(e5, elements["e5"]);
            AssertEqual(e6, elements["e6"]);
        }

        private static void AssertEqual(HttpHeaderValueElement element1, HttpHeaderValueElement element2)
        {
            Assert.Equal(element1.Name, element2.Name);
            Assert.Equal(element1.Value, element2.Value);
            Assert.Equal(element1.Parameters.Count(), element2.Parameters.Count());
            var parameters1 = element1.Parameters.ToArray();
            var parameters2 = element2.Parameters.ToArray();
            for (int i = 0; i < parameters1.Length; i++)
            {
                Assert.Equal(parameters1[i], parameters2[i]);
            }
        }
    }
}