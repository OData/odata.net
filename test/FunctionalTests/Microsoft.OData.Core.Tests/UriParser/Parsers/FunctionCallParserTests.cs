//---------------------------------------------------------------------
// <copyright file="FunctionCallParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.UriParser;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Parsers
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
            Assert.Throws<ArgumentNullException>("lexer", createWithNullLexer);
        }

        [Fact]
        public void ParserCannotBeNull()
        {
            Action createWithNullLexer = () => new FunctionCallParser(new ExpressionLexer("foo", true, false), null /*resolveAlias*/);
            Assert.Throws<ArgumentNullException>("parser", createWithNullLexer);
        }

        [Fact]
        public void ParseDottedIdentifier()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("geo.distance()");
            QueryToken result;
            bool success = tokenizer.TryParseIdentifierAsFunction(null, out result);
            Assert.True(success);
            Assert.Empty(result.ShouldBeFunctionCallToken("geo.distance").Arguments);
        }

        [Fact]
        public void ParseManyDottedIdentifier()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("one.two.three.four.five.six()");
            QueryToken result;
            bool success = tokenizer.TryParseIdentifierAsFunction(null, out result);
            Assert.True(success);
            Assert.Empty(result.ShouldBeFunctionCallToken("one.two.three.four.five.six").Arguments);
        }

        [Fact]
        public void ParseNonDottedIdentifier()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("func()");
            QueryToken result;
            bool success = tokenizer.TryParseIdentifierAsFunction(null, out result);
            Assert.True(success);
            Assert.Empty(result.ShouldBeFunctionCallToken("func").Arguments);
        }

        [Fact]
        public void ParsedFunctionWithAParent()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("func()");
            InnerPathToken parent = new InnerPathToken("Customer", null, null);
            QueryToken result;
            bool success = tokenizer.TryParseIdentifierAsFunction(parent, out result);
            Assert.True(success);
            Assert.Same(parent, result.ShouldBeFunctionCallToken("func").Source);
        }

        [Fact]
        public void ParsedFunctionWithAParentAndArgs()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("func(x='blah')");
            InnerPathToken parent = new InnerPathToken("Customer", null, null);
            QueryToken result;
            bool success = tokenizer.TryParseIdentifierAsFunction(parent, out result);
            Assert.True(success);
            FunctionCallToken functionCallToken = result.ShouldBeFunctionCallToken("func");
            Assert.Same(parent, functionCallToken.Source);
            Assert.Single(functionCallToken.Arguments);
        }

        [Fact]
        public void ArgsMustBeDelimitedByParens()
        {
            FunctionCallParser tokenizerWithoutClosingParen = GetFunctionCallParser("(stuff, stuff");
            FunctionCallParser tokenizerWithoutOpeningParen = GetFunctionCallParser("stuff, stuff)");
            Action createWithoutClosingParen = () => tokenizerWithoutClosingParen.ParseArgumentListOrEntityKeyList();
            Action createWithoutOpeningParen = () => tokenizerWithoutOpeningParen.ParseArgumentListOrEntityKeyList();
            createWithoutClosingParen.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(13, "(stuff, stuff"));
            createWithoutOpeningParen.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(0, "stuff, stuff)"));
        }

        [Fact]
        public void FunctionWithOneArgument()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("func(1)");
            QueryToken result;
            bool success = tokenizer.TryParseIdentifierAsFunction(null, out result);
            Assert.True(success);
            Assert.Single(result.ShouldBeFunctionCallToken("func").Arguments);
        }

        [Fact]
        public void FunctionWithTwoArguments()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("func(1, 2)");
            QueryToken result;
            bool success = tokenizer.TryParseIdentifierAsFunction(null, out result);
            Assert.True(success);
            Assert.Equal(2, result.ShouldBeFunctionCallToken("func").Arguments.Count());
        }

        [Fact]
        public void FunctionCallWithNamedArguments()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("func(stuff=1, morestuff=2)");
            QueryToken result;
            bool success = tokenizer.TryParseIdentifierAsFunction(null, out result);
            Assert.True(success);
            Assert.Equal(2, result.ShouldBeFunctionCallToken("func").Arguments.Count());
        }

        [Fact]
        public void FunctionCallWithOnlyOpeningParenthesis()
        {
            FunctionCallParser tokenizer = GetFunctionCallParser("func(");
            QueryToken result;
            Action parse = () => tokenizer.TryParseIdentifierAsFunction(null, out result);
            parse.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(5, "func("));
        }

        private static FunctionCallParser GetFunctionCallParser(string expression)
        {
            ExpressionLexer lexer = new ExpressionLexer(expression, true, false);
            UriQueryExpressionParser parser = new UriQueryExpressionParser(345, lexer);
            return new FunctionCallParser(lexer, parser /*resolveAlias*/);
        }
    }
}
