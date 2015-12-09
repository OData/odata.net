//---------------------------------------------------------------------
// <copyright file="IdentifierTokenizerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests for the IdentifierTokenizer class.
    /// </summary>
    public class IdentifierTokenizerTests
    {
        private readonly HashSet<string> parameters = new HashSet<string>
        {
            ExpressionConstants.It
        };

        // Constructor
        [Fact]
        public void ConstructorParametersCannotBeNull()
        {
            Action create = () => new IdentifierTokenizer(null, GetRealFunctionCallParser("stuff"));
            create.ShouldThrow<ArgumentNullException>(Error.ArgumentNull("parameters").ToString());

            create = () => new IdentifierTokenizer(this.parameters, null);
            create.ShouldThrow<ArgumentNullException>(Error.ArgumentNull("functionCallParser").ToString());
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
            result.ShouldBeStarToken().And.NextToken.Should().BeSameAs(fakeToken);
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
        public void MemberAccessWithIdentifierReturnsPropertyAccessToken()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("stuff");
            RangeVariableToken rangeVariableToken = new RangeVariableToken(ExpressionConstants.It);
            QueryToken result = tokenizer.ParseMemberAccess(rangeVariableToken);
            result.ShouldBeEndPathToken("stuff").And.NextToken.As<RangeVariableToken>().Name.Should().Be(ExpressionConstants.It);
        }

        // ParseStarMemberAccess
        [Fact]
        public void CannotCallStarMemberAccessWithANonStarToken()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("stuff");
            RangeVariableToken rangeVariableToken = new RangeVariableToken(ExpressionConstants.It);
            Action createWithNonStarToken = () => tokenizer.ParseStarMemberAccess(rangeVariableToken);
            createWithNonStarToken.ShouldThrow<ODataException>(ODataErrorStrings.UriQueryExpressionParser_CannotCreateStarTokenFromNonStar("stuff"));
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
