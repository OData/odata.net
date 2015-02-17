//---------------------------------------------------------------------
// <copyright file="SelectExpandParserTests.cs" company="Microsoft">
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
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Short-span integration tests for the SelectExpandParser, which knows how to use a lexer to 
    /// syntactically parse a V4 $expand or $select query option with nested expand options. 
    /// Unit testing the underlying pieces of parsing here that is be better, but tests at this level are
    /// still quite nice.
    /// TODO: Need more tests for $levels, $ref, and mixing all of the stuff together.
    /// </summary>
    [TestClass]
    public class SelectExpandParserTests
    {
        #region ParseSelect
        [TestMethod]
        public void SelectParsesEachTermOnce()
        {
            SelectExpandParser parser = new SelectExpandParser("foo,bar,thing,prop,yoda", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseSelect();
            results.Properties.Count().Should().Be(5);
        }

        [TestMethod]
        public void NullSelectBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser(null, ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseSelect();
            results.Properties.Should().NotBeNull();
            results.Properties.Should().BeEmpty();
        }

        [TestMethod]
        public void EmptySelectStringBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser("", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseSelect();
            results.Properties.Should().NotBeNull();
            results.Properties.Should().BeEmpty();
        }

        [TestMethod]
        public void JustSpaceSelectStringBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser("         ", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseSelect();
            results.Properties.Should().NotBeNull();
            results.Properties.Should().BeEmpty();
        }

        [TestMethod]
        public void EmptySelectTermShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("one,,two", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseSelect();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionToken_IdentifierExpected("4"));
        }

        [TestMethod]
        public void MaxRecursionDepthIsEnforcedInSelect()
        {
            SelectExpandParser parser = new SelectExpandParser("stuff/stuff/stuff/stuff", 3);
            Action parse = () => parser.ParseSelect();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void SemicolonOnlyAllowedInParensInSelect()
        {
            SelectExpandParser parser = new SelectExpandParser("one;two", 3);
            Action parse = () => parser.ParseSelect();
            parse.ShouldThrow<ODataException>("one;two is not valid in a $select or $expand expression.");
        }

        [TestMethod]
        public void WhitespaceInMiddleOfSegmentShouldThrowInSelect()
        {
            SelectExpandParser parser = new SelectExpandParser("what happens here/foo", 3);
            Action parse = () => parser.ParseSelect();
            parse.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_TermIsNotValid("what happens here/foo"));
        }

        #endregion

        #region ParseExpand
        [TestMethod]
        public void ExpandParsesEachTermOnce()
        {
            SelectExpandParser parser = new SelectExpandParser("foo,bar,thing,prop,yoda", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Count().Should().Be(5);
        }

        [TestMethod]
        public void NullExpandBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser(null, ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Should().NotBeNull();
            results.ExpandTerms.Should().BeEmpty();
        }

        [TestMethod]
        public void EmptyExpandStringBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser("", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Should().NotBeNull();
            results.ExpandTerms.Should().BeEmpty();
        }

        [TestMethod]
        public void JustSpaceExpandStringBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser("         ", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Should().NotBeNull();
            results.ExpandTerms.Should().BeEmpty();
        }

        [TestMethod]
        public void EmptyExpandTermShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("one,,two", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionToken_IdentifierExpected("4"));
        }

        [TestMethod]
        public void NestedOptionsOnMultipleExpansionsIsOk()
        {
            SelectExpandParser parser = new SelectExpandParser("one($filter=true), two($filter=false)", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Should().HaveCount(2);
            results.ExpandTerms.First().FilterOption.Should().NotBeNull();
            results.ExpandTerms.Last().FilterOption.Should().NotBeNull();
        }

        [TestMethod]
        public void NestedOptionsWithoutClosingParenthesisShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("one($filter=true", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseExpand();
            // TODO: Make this error message make sense for parenthetical expressions. Either generalize it or refactor code.
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_UnbalancedBracketExpression);
        }

        [TestMethod]
        public void NestedOptionsWithExtraCloseParenShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("one($filter=true)), two", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriSelectParser_TermIsNotValid("one($filter=true)), two"));
        }

        [TestMethod]
        public void OpenCloseParensAfterNavPropShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("NavProp()", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_MissingExpandOption("NavProp"));
        }

        [TestMethod]
        public void MaxRecursionDepthIsEnforcedInExpand()
        {
            SelectExpandParser parser = new SelectExpandParser("stuff/stuff/stuff/stuff", 3);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void SemicolonOnlyAllowedInParensInExpand()
        {
            SelectExpandParser parser = new SelectExpandParser("one;two", 3);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_InvalidCharacter(";", 3, "one;two"));
        }

        [TestMethod]
        public void WhitespaceInMiddleOfSegmentShouldThrowInExpand()
        {
            SelectExpandParser parser = new SelectExpandParser("what happens here/foo", 3);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_TermIsNotValid("what happens here/foo"));
        }
        #endregion

        #region Limit Test
        [TestMethod]
        public void PathLimitTest()
        {
            this.LimitationTest("d1/d2", null);
            Action action = () => this.LimitationTest("d1/d2/d3", null);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);

            this.LimitationTest(null, "d1/d2");
            action = () => this.LimitationTest(null,"d1/d2/d3");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void FilterLimitTest()
        {
            this.LimitationTest(null, "d1($filter=a or b and c)");
            Action action = () => this.LimitationTest(null, "d1($filter=(a or b) and c)");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void OrderByLimitTest()
        {
            this.LimitationTest(null, "d1($orderby=a or b and c)");
            Action action = () => this.LimitationTest(null, "d1($orderby=(a or b) and c)");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void SearchLimitTest()
        {
            this.LimitationTest(null, "d1($search=a OR b AND c)");
            Action action = () => this.LimitationTest(null, "d1($search=(a OR b) AND c)");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void ExpandLimitTest()
        {
            this.LimitationTest(null, "d1($expand=d2($expand=d3($expand=d4($expand=d5))))");
            Action action = () => this.LimitationTest(null, "d1($expand=d2($expand=d3($expand=d4($expand=d5($expand=d6)))))");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        private void LimitationTest(string select, string expand)
        {
            SelectToken selectTree;
            ExpandToken expandTree;
            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(EdmCoreModel.Instance)
            {
                Settings = { PathLimit = 2, FilterLimit = 7, OrderByLimit = 7, SearchLimit = 7, SelectExpandLimit = 5 }
            };

            SelectExpandSyntacticParser.Parse(select, expand, configuration, out expandTree, out selectTree);
        }
        #endregion
    }
}
