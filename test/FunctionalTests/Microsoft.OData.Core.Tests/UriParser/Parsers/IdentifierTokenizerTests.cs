//---------------------------------------------------------------------
// <copyright file="IdentifierTokenizerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests for the IdentifierTokenizer class.
    /// </summary>
    public class IdentifierTokenizerTests
    {
        private readonly HashSet<string> parameters = new HashSet<string>
        {
            ExpressionConstants.It,
            ExpressionConstants.This
        };

        // Constructor
        [Fact]
        public void ConstructorParametersCannotBeNull()
        {
            Action create = () => new IdentifierTokenizer(null, GetRealFunctionCallParser("stuff"));
            Assert.Throws<ArgumentNullException>("parameters", create);

            create = () => new IdentifierTokenizer(this.parameters, null);
            Assert.Throws<ArgumentNullException>("functionCallParser", create);
        }

        // ParseIdentifier Short-Span Integration Tests
        [Fact]
        public void FunctionIdentifierParsedCorrectly()
        {
            var tokenizer = GetIdentifierTokenizerWithRealFunctionParser("func()");
            QueryToken result = tokenizer.ParseIdentifier(null);
            result.ShouldBeFunctionCallToken("func");
        }

        [Fact]
        public void MemberAccessParsedCorrectly()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("stuff");
            QueryToken result = tokenizer.ParseIdentifier(null);
            result.ShouldBeEndPathToken("stuff");
        }

        // ParseMemberAccess
        [Fact]
        public void MemberAccessWithStarParsedCorrectly()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("*");
            RangeVariableToken fakeToken = new RangeVariableToken(ExpressionConstants.It);
            QueryToken result = tokenizer.ParseMemberAccess(fakeToken);
            Assert.Same(fakeToken, result.ShouldBeStarToken().NextToken);
        }

        [Fact]
        public void MemberAccessWithNoIdentifierReturnsRangeVariableToken()
        {
            this.parameters.Add("variable");
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("variable");
            QueryToken result = tokenizer.ParseMemberAccess(null);
            result.ShouldBeRangeVariableToken("variable");
        }

        [Fact]
        public void MemberAccessWithNoIdentifierAndDollarThisAsLexerReturnsRangeVariableToken()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("$this");
            QueryToken result = tokenizer.ParseMemberAccess(null);
            result.ShouldBeRangeVariableToken(ExpressionConstants.This);
        }

        [Fact]
        public void MemberAccessWithIdentifierReturnsPropertyAccessToken()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("stuff");
            RangeVariableToken rangeVariableToken = new RangeVariableToken(ExpressionConstants.It);
            QueryToken result = tokenizer.ParseMemberAccess(rangeVariableToken);
            var rangeToken = Assert.IsType<RangeVariableToken>(result.ShouldBeEndPathToken("stuff").NextToken);
            Assert.Equal(ExpressionConstants.It, rangeToken.Name);
        }

        // ParseStarMemberAccess
        [Fact]
        public void CannotCallStarMemberAccessWithANonStarToken()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("stuff");
            RangeVariableToken rangeVariableToken = new RangeVariableToken(ExpressionConstants.It);
            Action createWithNonStarToken = () => tokenizer.ParseStarMemberAccess(rangeVariableToken);
            createWithNonStarToken.Throws<ODataException>(ODataErrorStrings.UriQueryExpressionParser_CannotCreateStarTokenFromNonStar("stuff"));
        }

        // Helpers
        private IdentifierTokenizer GetIdentifierTokenizerWithRealFunctionParser(string expression)
        {
            IdentifierTokenizer tokenizer = new IdentifierTokenizer(this.parameters, GetRealFunctionCallParser(expression));
            return tokenizer;
        }

        private static IFunctionCallParser GetRealFunctionCallParser(string expression)
        {
            ExpressionLexer lexer = new ExpressionLexer(expression, true, false);
            UriQueryExpressionParser parser = new UriQueryExpressionParser(345, lexer);
            return new FunctionCallParser(lexer, parser /*resolveAlias*/);
        }
    }
}
