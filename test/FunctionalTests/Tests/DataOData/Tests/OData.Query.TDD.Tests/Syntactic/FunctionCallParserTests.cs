//---------------------------------------------------------------------
// <copyright file="FunctionCallParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Unit tests for the FunctionCallParser implementation of IFunctionCallParser.
    /// </summary>
    [TestClass]
    public class FunctionCallParserTests
    {
        [TestMethod]
        public void LexerCannotBeNull()
        {
            UriQueryExpressionParser parser = new UriQueryExpressionParser(345, new ExpressionLexer("stuff", true, false));
            Action createWithNullLexer = () => new FunctionCallParser(null, parser  /*resolveAlias*/);
            createWithNullLexer.ShouldThrow<Exception>().WithMessage("lexer", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void ParserCannotBeNull()
        {
            Action createWithNullLexer = () => new FunctionCallParser(new ExpressionLexer("foo", true, false), null /*resolveAlias*/);
            createWithNullLexer.ShouldThrow<Exception>().WithMessage("parser", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void ParseDottedIdentifier()
        {
            var tokenizer = GetFunctionCallParser("geo.distance()");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("geo.distance").And.Arguments.Should().BeEmpty();
        }

        [TestMethod]
        public void ParseManyDottedIdentifier()
        {
            var tokenizer = GetFunctionCallParser("one.two.three.four.five.six()");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("one.two.three.four.five.six").And.Arguments.Should().BeEmpty();
        }

        [TestMethod]
        public void ParseNonDottedIdentifier()
        {
            var tokenizer = GetFunctionCallParser("func()");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("func").And.Arguments.Should().BeEmpty();
        }

        [TestMethod]
        public void ParsedFunctionWithAParent()
        {
            var tokenizer = GetFunctionCallParser("func()");
            var parent = new InnerPathToken("Customer", null, null);
            QueryToken result = tokenizer.ParseIdentifierAsFunction(parent);
            result.ShouldBeFunctionCallToken("func").And.Source.Should().BeSameAs(parent);
        }

        [TestMethod]
        public void ParsedFunctionWithAParentAndArgs()
        {
            var tokenizer = GetFunctionCallParser("func(x='blah')");
            var parent = new InnerPathToken("Customer", null, null);
            QueryToken result = tokenizer.ParseIdentifierAsFunction(parent);
            var functionCallToken = result.ShouldBeFunctionCallToken("func").And;
            functionCallToken.Source.Should().BeSameAs(parent);
            functionCallToken.Arguments.Should().HaveCount(1);
        }

        [TestMethod]
        public void ArgsMustBeDelimitedByParens()
        {
            var tokenizerWithoutClosingParen = GetFunctionCallParser("(stuff, stuff");
            var tokenizerWithoutOpeningParen = GetFunctionCallParser("stuff, stuff)");
            Action createWithoutClosingParen = () => tokenizerWithoutClosingParen.ParseArgumentListOrEntityKeyList();
            Action createWithoutOpeningParen = () => tokenizerWithoutOpeningParen.ParseArgumentListOrEntityKeyList();
            createWithoutClosingParen.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(13, "(stuff, stuff"));
            createWithoutOpeningParen.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(0, "stuff, stuff)"));
        }

        [TestMethod]
        public void FunctionWithOneArgument()
        {
            var tokenizer = GetFunctionCallParser("func(1)");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("func")
                  .And.Arguments.Should().HaveCount(1);
        }

        [TestMethod]
        public void FunctionWithTwoArguments()
        {
            var tokenizer = GetFunctionCallParser("func(1, 2)");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("func")
                  .And.Arguments.Should().HaveCount(2);
        }

        [TestMethod]
        public void FunctionCallWithNamedArguments()
        {
            var tokenizer = GetFunctionCallParser("func(stuff=1, morestuff=2)");
            QueryToken result = tokenizer.ParseIdentifierAsFunction(null);
            result.ShouldBeFunctionCallToken("func")
                  .And.Arguments.Should().HaveCount(2);
        }

        [TestMethod]
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
