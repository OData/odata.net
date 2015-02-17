//---------------------------------------------------------------------
// <copyright file="IdentifierTokenizerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Unit tests for the IdentifierTokenizer class.
    /// </summary>
    [TestClass]
    public class IdentifierTokenizerTests
    {
        private readonly HashSet<string> parameters = new HashSet<string>
        {
            ExpressionConstants.It
        };

        // Constructor
        [TestMethod]
        public void ConstructorParametersCannotBeNull()
        {
            Action create = () => new IdentifierTokenizer(null, GetRealFunctionCallParser("stuff"));
            create.ShouldThrow<ArgumentNullException>(Error.ArgumentNull("parameters").ToString());

            create = () => new IdentifierTokenizer(this.parameters, null);
            create.ShouldThrow<ArgumentNullException>(Error.ArgumentNull("functionCallParser").ToString());
        }

        // ParseIdentifier Short-Span Integration Tests
        [TestMethod]
        public void FunctionIdentifierParsedCorrectly()
        {
            var tokenizer = GetIdentifierTokenizerWithRealFunctionParser("func()");
            QueryToken result = tokenizer.ParseIdentifier(null);
            result.ShouldBeFunctionCallToken("func");
        }

        [TestMethod]
        public void MemberAccessParsedCorrectly()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("stuff");
            QueryToken result = tokenizer.ParseIdentifier(null);
            result.ShouldBeEndPathToken("stuff");
        }

        // ParseMemberAccess
        [TestMethod]
        public void MemberAccessWithStarParsedCorrectly()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("*");
            RangeVariableToken fakeToken = new RangeVariableToken(ExpressionConstants.It);
            QueryToken result = tokenizer.ParseMemberAccess(fakeToken);
            result.ShouldBeStarToken().And.NextToken.Should().BeSameAs(fakeToken);
        }

        [TestMethod]
        public void MemberAccessWithNoIdentifierReturnsRangeVariableToken()
        {
            this.parameters.Add("variable");
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("variable");
            QueryToken result = tokenizer.ParseMemberAccess(null);
            result.ShouldBeRangeVariableToken("variable");
        }

        [TestMethod]
        public void MemberAccessWithIdentifierReturnsPropertyAccessToken()
        {
            var tokenizer = this.GetIdentifierTokenizerWithRealFunctionParser("stuff");
            RangeVariableToken rangeVariableToken = new RangeVariableToken(ExpressionConstants.It);
            QueryToken result = tokenizer.ParseMemberAccess(rangeVariableToken);
            result.ShouldBeEndPathToken("stuff").And.NextToken.As<RangeVariableToken>().Name.Should().Be(ExpressionConstants.It);
        }

        // ParseStarMemberAccess
        [TestMethod]
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
