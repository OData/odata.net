//---------------------------------------------------------------------
// <copyright file="HttpHeaderValueLexerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpHeaderValueLexerTests
    {
        [TestMethod]
        public void CreateWithNullValueShouldReturnHttpHeaderStart()
        {
            HttpHeaderValueLexer.Create("headerName", null).Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Start);
        }

        [TestMethod]
        public void CreateWithEmptyValueShouldReturnHttpHeaderStart()
        {
            HttpHeaderValueLexer.Create("headerName", "").Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Start);
        }

        [TestMethod]
        public void CreateShouldReturnHttpHeaderStart()
        {
            HttpHeaderValueLexer.Create("headerName", "token").Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Start);
        }

        [TestMethod]
        public void StartCannotTransitionToQuotedString()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", "\"quoted-string\"").ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString("headerName", "\"quoted-string\"", 0));
        }

        [TestMethod]
        public void StartCannotTrnasistionToElementSeparator()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", ",").ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", ",", 0));            
        }

        [TestMethod]
        public void StartCannotTrnasistionToParameterSeparator()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", ";").ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", ";", 0));
        }

        [TestMethod]
        public void StartCannotTrnasistionToValueSeparator()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", "=").ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", "=", 0));
        }

        [TestMethod]
        public void StartCanTransitionToToken()
        {
            HttpHeaderValueLexer.Create("headerName", "token").ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Token);
        }

        [TestMethod]
        public void StartCanTransitionToEnd()
        {
            HttpHeaderValueLexer.Create("headerName", " ").ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.End);            
        }

        [TestMethod]
        public void TokenCanTransitionToElementSeparator()
        {
            HttpHeaderValueLexer.Create("headerName", "token,").ReadNext().ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator);
        }

        [TestMethod]
        public void TokenCanTransitionToParameterSeparator()
        {
            HttpHeaderValueLexer.Create("headerName", "token ;").ReadNext().ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator);            
        }

        [TestMethod]
        public void TokenCanTransitionToValueSeparator()
        {
            HttpHeaderValueLexer.Create("headerName", "token  =  ").ReadNext().ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator);
        }

        [TestMethod]
        public void TokenCanTransitionToEnd()
        {
            HttpHeaderValueLexer.Create("headerName", "token").ReadNext().ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.End);
        }

        [TestMethod]
        public void TokenCannotNotTransitionToQuotedString()
        {
            Action test = () => HttpHeaderValueLexer.Create("headerName", "token\"quotes-string\"").ReadNext().ReadNext();
            test.ShouldThrow<ODataException>(Strings.HttpHeaderValueLexer_UnrecognizedSeparator("headerName", "token\"quotes-string\"", 5, "\""));
        }

        [TestMethod]
        public void QuotedStringCanTransitionToEnd()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=\"quotes-string\"").ReadNext().ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString);
            lexer.ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.End);
        }

        [TestMethod]
        public void QuotedStringCanTransitionToElementSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token = \"quotes-string\" ,").ReadNext().ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString);
            lexer.ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator);            
        }

        [TestMethod]
        public void QuotedStringCanTransitionToParameterSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=   \"quotes-string\";").ReadNext().ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString);
            lexer.ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator);            
        }

        [TestMethod]
        public void QuotedStringCannotTransitionToValueSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=\"quotes-string\"=").ReadNext().ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString("headerName", "token=\"quotes-string\"=", 21, "="));
        }

        [TestMethod]
        public void QuotesStringCannotTransitionToToken()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=\"quotes-string\"token").ReadNext().ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_UnrecognizedSeparator("headerName", "token=\"quotes-string\"token", 21, "t"));            
        }

        [TestMethod]
        public void QuotedStringCannotTransitionToQuotedString()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=\"quotes-string\"\"quotes-string\"").ReadNext().ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_UnrecognizedSeparator("headerName", "token=\"quotes-string\"\"quotes-string\"", 21, "\""));                        
        }

        [TestMethod]
        public void ElementSeparatorCannotTransitionToEnd()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token,").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_EndOfFileAfterSeparator("headerName", "token,", 6, ","));
        }

        [TestMethod]
        public void ParameterSeparatorCannotTransitionToEnd()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token;").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_EndOfFileAfterSeparator("headerName", "token;", 6, ";"));
        }

        [TestMethod]
        public void ValueSeparatorCannotTransitionToEnd()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token=").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_EndOfFileAfterSeparator("headerName", "token=", 6, "="));
        }

        [TestMethod]
        public void ValueSeparatorCanTransitionToToken()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token = token").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator);
            lexer.ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Token);
        }

        [TestMethod]
        public void ValueSeparatorCanTransitionToQuotedString()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token= \"quoted-string\"").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator);
            lexer.ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.QuotedString);
        }

        [TestMethod]
        public void ValueSeparatorCannotTransitionToSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token= =").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ValueSeparator);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", "token= =", 7));
        }

        [TestMethod]
        public void ElementSeparatorCanTransitionToToken()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token,token").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator);
            lexer.ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Token);
        }

        [TestMethod]
        public void ElementSeparatorCannotTransitionToQuotedString()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token,\"quoted-string\"").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString("headerName", "token,\"quoted-string\"", 6));
        }

        [TestMethod]
        public void ElementSeparatorCannotTransitionToSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token,,").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ElementSeparator);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", "token,,", 6));            
        }

        [TestMethod]
        public void ParameterSeparatorCanTransitionToToken()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token  ;   token").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator);
            lexer.ReadNext().Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Token);            
        }

        [TestMethod]
        public void ParameterSeparatorCannotTransitionToQuotedString()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token;\"quoted-string\"").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString("headerName", "token;\"quoted-string\"", 6));            
        }

        [TestMethod]
        public void ParameterSeparatorCannotTransitionToSeparator()
        {
            var lexer = HttpHeaderValueLexer.Create("headerName", "token;;").ReadNext().ReadNext();
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.ParameterSeparator);
            Action test = () => lexer.ReadNext();
            test.ShouldThrow<ODataException>().WithMessage(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString("headerName", "token;;", 6));
        }

        [TestMethod]
        public void HttpHeaderValueEndToHttpHeaderValueElementsShouldReturnEmptyDictionary()
        {
            var lexer = HttpHeaderValueLexer.Create("header", null);
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Start);
            lexer.ToHttpHeaderValue().Count.Should().Be(0);
        }

        [TestMethod]
        public void EmptyHeaderValueToHttpHeaderValueElementsShouldReturnEmptyDictionary()
        {
            var lexer = HttpHeaderValueLexer.Create("header", " ");
            lexer.Type.Should().Be(HttpHeaderValueLexer.HttpHeaderValueItemType.Start);
            lexer.ToHttpHeaderValue().Count.Should().Be(0);
        }

        [TestMethod]
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
            elements.Count.Should().Be(6);
            AssertAreEqual(e1, elements["e1"]);
            AssertAreEqual(e2, elements["e2"]);
            AssertAreEqual(e3, elements["e3"]);
            AssertAreEqual(e4, elements["e4"]);
            AssertAreEqual(e5, elements["e5"]);
            AssertAreEqual(e6, elements["e6"]);
        }

        private static void AssertAreEqual(HttpHeaderValueElement element1, HttpHeaderValueElement element2)
        {
            Assert.AreEqual(element1.Name, element2.Name);
            Assert.AreEqual(element1.Value, element2.Value);
            Assert.AreEqual(element1.Parameters.Count(), element2.Parameters.Count());
            var parameters1 = element1.Parameters.ToArray();
            var parameters2 = element2.Parameters.ToArray();
            for (int i = 0; i < parameters1.Length; i++)
            {
                Assert.AreEqual(parameters1[i], parameters2[i]);
            }
        }
    }
}