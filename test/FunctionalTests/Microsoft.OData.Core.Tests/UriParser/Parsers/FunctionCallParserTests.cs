//---------------------------------------------------------------------
// <copyright file="FunctionCallParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests for the FunctionCallParser implementation of IFunctionCallParser.
    /// </summary>
    public class FunctionCallParserTests
    {
        [Fact]
        public void LexerCannotBeNull()
        {
            UriQueryExpressionParser parser = new UriQueryExpressionParser(345, new ExpressionLexer("stuff", true, false));
            Action createWithNullLexer = () => new FunctionCallParser(null, parser  /*resolveAlias*/);
            createWithNullLexer.ShouldThrow<Exception>().WithMessage("lexer", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void ParserCannotBeNull()
        {
            Action createWithNullLexer = () => new FunctionCallParser(new ExpressionLexer("foo", true, false), null /*resolveAlias*/);
            createWithNullLexer.ShouldThrow<Exception>().WithMessage("parser", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void ParseDottedIdentifier()
        {
            var tokenizer = GetFunctionCallParser("geo.distance()");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("geo.distance").And.Arguments.Should().BeEmpty();
        }

        [Fact]
        public void ParseManyDottedIdentifier()
        {
            var tokenizer = GetFunctionCallParser("one.two.three.four.five.six()");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("one.two.three.four.five.six").And.Arguments.Should().BeEmpty();
        }

        [Fact]
        public void ParseNonDottedIdentifier()
        {
            var tokenizer = GetFunctionCallParser("func()");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("func").And.Arguments.Should().BeEmpty();
        }

        [Fact]
        public void ParsedFunctionWithAParent()
        {
            var tokenizer = GetFunctionCallParser("func()");
            var parent = new InnerPathToken("Customer", null, null);
            QueryToken result = tokenizer.ParseIdentifierAsFunction(parent);
            result.ShouldBeFunctionCallToken("func").And.Source.Should().BeSameAs(parent);
        }

        [Fact]
        public void ParsedFunctionWithAParentAndArgs()
        {
            var tokenizer = GetFunctionCallParser("func(x='blah')");
            var parent = new InnerPathToken("Customer", null, null);
            QueryToken result = tokenizer.ParseIdentifierAsFunction(parent);
            var functionCallToken = result.ShouldBeFunctionCallToken("func").And;
            functionCallToken.Source.Should().BeSameAs(parent);
            functionCallToken.Arguments.Should().HaveCount(1);
        }

        [Fact]
        public void ArgsMustBeDelimitedByParens()
        {
            var tokenizerWithoutClosingParen = GetFunctionCallParser("(stuff, stuff");
            var tokenizerWithoutOpeningParen = GetFunctionCallParser("stuff, stuff)");
            Action createWithoutClosingParen = () => tokenizerWithoutClosingParen.ParseArgumentListOrEntityKeyList();
            Action createWithoutOpeningParen = () => tokenizerWithoutOpeningParen.ParseArgumentListOrEntityKeyList();
            createWithoutClosingParen.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(13, "(stuff, stuff"));
            createWithoutOpeningParen.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(0, "stuff, stuff)"));
        }

        [Fact]
        public void FunctionWithOneArgument()
        {
            var tokenizer = GetFunctionCallParser("func(1)");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("func")
                  .And.Arguments.Should().HaveCount(1);
        }

        [Fact]
        public void FunctionWithTwoArguments()
        {
            var tokenizer = GetFunctionCallParser("func(1, 2)");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("func")
                  .And.Arguments.Should().HaveCount(2);
        }

        [Fact]
        public void FunctionCallWithNamedArguments()
        {
            var tokenizer = GetFunctionCallParser("func(stuff=1, morestuff=2)");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("func")
                  .And.Arguments.Should().HaveCount(2);
        }

        [Fact]
        public void FunctionCallWithOnlyOpeningParenthesis()
        {
            var tokenizer = GetFunctionCallParser("func(");
            Action parse = () => tokenizer.ParseIdentifierAsFunction(null);
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(5, "func("));
        }

        private static FunctionCallParser GetFunctionCallParser(string expression)
        {
            var lexer = new ExpressionLexer(expression, true, false);
            UriQueryExpressionParser parser = new UriQueryExpressionParser(345, lexer);
            return new FunctionCallParser(lexer, parser /*resolveAlias*/);
        }
    }
}
