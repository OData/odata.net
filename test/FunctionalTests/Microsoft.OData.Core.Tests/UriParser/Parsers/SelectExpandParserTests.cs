//---------------------------------------------------------------------
// <copyright file="SelectExpandParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Short-span integration tests for the SelectExpandParser, which knows how to use a lexer to 
    /// syntactically parse a V4 $expand or $select query option with nested expand options. 
    /// Unit testing the underlying pieces of parsing here that is be better, but tests at this level are
    /// still quite nice.
    /// TODO: Need more tests for $levels, $ref, and mixing all of the stuff together.
    /// </summary>
    public class SelectExpandParserTests
    {
        #region ParseSelect
        [Fact]
        public void SelectParsesEachTermOnce()
        {
            SelectExpandParser parser = new SelectExpandParser("foo,bar,thing,prop,yoda", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseSelect();
            results.Properties.Count().Should().Be(5);
        }

        [Fact]
        public void NullSelectBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser(null, ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseSelect();
            results.Properties.Should().NotBeNull();
            results.Properties.Should().BeEmpty();
        }

        [Fact]
        public void EmptySelectStringBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser("", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseSelect();
            results.Properties.Should().NotBeNull();
            results.Properties.Should().BeEmpty();
        }

        [Fact]
        public void JustSpaceSelectStringBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser("         ", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseSelect();
            results.Properties.Should().NotBeNull();
            results.Properties.Should().BeEmpty();
        }

        [Fact]
        public void EmptySelectTermShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("one,,two", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseSelect();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionToken_IdentifierExpected("4"));
        }

        [Fact]
        public void MaxRecursionDepthIsEnforcedInSelect()
        {
            SelectExpandParser parser = new SelectExpandParser("stuff/stuff/stuff/stuff", 3);
            Action parse = () => parser.ParseSelect();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void SemicolonOnlyAllowedInParensInSelect()
        {
            SelectExpandParser parser = new SelectExpandParser("one;two", 3);
            Action parse = () => parser.ParseSelect();
            parse.ShouldThrow<ODataException>("one;two is not valid in a $select or $expand expression.");
        }

        [Fact]
        public void WhitespaceInMiddleOfSegmentShouldThrowInSelect()
        {
            SelectExpandParser parser = new SelectExpandParser("what happens here/foo", 3);
            Action parse = () => parser.ParseSelect();
            parse.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_TermIsNotValid("what happens here/foo"));
        }

        #endregion

        #region ParseExpand
        [Fact]
        public void ExpandParsesEachTermOnce()
        {
            SelectExpandParser parser = new SelectExpandParser("foo,bar,thing,prop,yoda", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Count().Should().Be(5);
        }

        [Fact]
        public void NullExpandBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser(null, ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Should().NotBeNull();
            results.ExpandTerms.Should().BeEmpty();
        }

        [Fact]
        public void EmptyExpandStringBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser("", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Should().NotBeNull();
            results.ExpandTerms.Should().BeEmpty();
        }

        [Fact]
        public void JustSpaceExpandStringBecomesEmptyList()
        {
            SelectExpandParser parser = new SelectExpandParser("         ", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Should().NotBeNull();
            results.ExpandTerms.Should().BeEmpty();
        }

        [Fact]
        public void EmptyExpandTermShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("one,,two", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionToken_IdentifierExpected("4"));
        }

        [Fact]
        public void NestedOptionsOnMultipleExpansionsIsOk()
        {
            SelectExpandParser parser = new SelectExpandParser("one($filter=true), two($filter=false)", ODataUriParserSettings.DefaultSelectExpandLimit);
            var results = parser.ParseExpand();
            results.ExpandTerms.Should().HaveCount(2);
            results.ExpandTerms.First().FilterOption.Should().NotBeNull();
            results.ExpandTerms.Last().FilterOption.Should().NotBeNull();
        }

        [Fact]
        public void NestedOptionsWithoutClosingParenthesisShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("one($filter=true", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseExpand();
            // TODO: Make this error message make sense for parenthetical expressions. Either generalize it or refactor code.
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_UnbalancedBracketExpression);
        }

        [Fact]
        public void NestedOptionsWithExtraCloseParenShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("one($filter=true)), two", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriSelectParser_TermIsNotValid("one($filter=true)), two"));
        }

        [Fact]
        public void OpenCloseParensAfterNavPropShouldThrow()
        {
            SelectExpandParser parser = new SelectExpandParser("NavProp()", ODataUriParserSettings.DefaultSelectExpandLimit);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_MissingExpandOption("NavProp"));
        }

        [Fact]
        public void MaxRecursionDepthIsEnforcedInExpand()
        {
            SelectExpandParser parser = new SelectExpandParser("stuff/stuff/stuff/stuff", 3);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void SemicolonOnlyAllowedInParensInExpand()
        {
            SelectExpandParser parser = new SelectExpandParser("one;two", 3);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_InvalidCharacter(";", 3, "one;two"));
        }

        [Fact]
        public void WhitespaceInMiddleOfSegmentShouldThrowInExpand()
        {
            SelectExpandParser parser = new SelectExpandParser("what happens here/foo", 3);
            Action parse = () => parser.ParseExpand();
            parse.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_TermIsNotValid("what happens here/foo"));
        }
        #endregion

        #region Star Expand Testing
        [Fact]
        public void ParseStarInExpand()
        {
            var results = this.StarExpandTesting("*", "Persons");
            results.ExpandTerms.Should().HaveCount(1);
        }

        [Fact]
        public void ParseStarWithRefInExpand()
        {
            var results = this.StarExpandTesting("*/$ref", "Persons");
            results.ExpandTerms.Should().HaveCount(1);
        }

        [Fact]
        public void ParseStarWithEntitySetInExpand()
        {
            var results = this.StarExpandTesting("*/$ref,CityHall", "Cities");
            results.ExpandTerms.Should().HaveCount(3);
            results.ExpandTerms.First().PathToNavProp.Identifier.ShouldBeEquivalentTo("CityHall");
            results.ExpandTerms.Last().PathToNavProp.Identifier.ShouldBeEquivalentTo("$ref");
        }

        [Fact]
        public void ParseStarWithMultipleEntitySetInExpand()
        {
            var results = this.StarExpandTesting("CityHall($levels=2),*/$ref,PoliceStation($select=Id, Address)", "Cities");
            results.ExpandTerms.Should().HaveCount(3);
            results.ExpandTerms.First().PathToNavProp.Identifier.ShouldBeEquivalentTo("CityHall");
            results.ExpandTerms.Last().PathToNavProp.Identifier.ShouldBeEquivalentTo("$ref");
        }

        [Fact]
        public void NestedStarExpand()
        {
            var results = this.StarExpandTesting("Friend($expand=*)", "Persons");
            results.ExpandTerms.Should().HaveCount(1);
        }

        [Fact]
        public void ParseStarWithOptions()
        {
            var results = this.StarExpandTesting("*($levels=2)", "Persons");
            results.ExpandTerms.Should().HaveCount(1);
        }

        [Fact]
        public void ParseStarInvalidWithOptions()
        {
            Action action = () => this.StarExpandTesting("*($select=*)", "Persons");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriExpandParser_TermIsNotValidForStar("($select=*)"));
        }

        [Fact]
        public void ParseStarRefWithInvalidOptions()
        {
            Action action = () => this.StarExpandTesting("*/$ref($levels=2)", "Persons");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriExpandParser_TermIsNotValidForStarRef("($levels=2)"));
        }

        [Fact]
        public void ParseMultipleStarInExpand()
        {
            Action action = () => this.StarExpandTesting("*, Friend, */$ref", "Persons");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriExpandParser_TermWithMultipleStarNotAllowed("*, Friend, */$ref"));
        }

        private ExpandToken StarExpandTesting(string expand, String entitySetType)
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(EdmCoreModel.Instance)
            {
                Settings = { PathLimit = 10, FilterLimit = 10, OrderByLimit = 10, SearchLimit = 10, SelectExpandLimit = 10 }
            };
            var parentEntityType = configuration.Resolver.ResolveNavigationSource(model, entitySetType).EntityType();
            SelectExpandParser expandParser = new SelectExpandParser(configuration.Resolver, expand, parentEntityType, configuration.Settings.SelectExpandLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier)
            {
                MaxPathDepth = configuration.Settings.PathLimit,
                MaxFilterDepth = configuration.Settings.FilterLimit,
                MaxOrderByDepth = configuration.Settings.OrderByLimit,
                MaxSearchDepth = configuration.Settings.SearchLimit
            };
            return expandParser.ParseExpand();
        }

        [Fact]
        public void ParentEntityTypeIsNullForExpandStar()
        {
            var expand = "*";

            ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(EdmCoreModel.Instance)
            {
                Settings = { PathLimit = 10, FilterLimit = 10, OrderByLimit = 10, SearchLimit = 10, SelectExpandLimit = 10 }
            };

            SelectExpandParser expandParser = new SelectExpandParser(configuration.Resolver, expand, null, configuration.Settings.SelectExpandLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier)
            {
                MaxPathDepth = configuration.Settings.PathLimit,
                MaxFilterDepth = configuration.Settings.FilterLimit,
                MaxOrderByDepth = configuration.Settings.OrderByLimit,
                MaxSearchDepth = configuration.Settings.SearchLimit
            };

            Action action = () => expandParser.ParseExpand();
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriExpandParser_ParentEntityIsNull(""));
        }
        #endregion

        #region Limit Test
        [Fact]
        public void PathLimitTest()
        {
            this.LimitationTest("d1/d2", null);
            Action action = () => this.LimitationTest("d1/d2/d3", null);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);

            this.LimitationTest(null, "d1/d2");
            action = () => this.LimitationTest(null,"d1/d2/d3");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void FilterLimitTest()
        {
            this.LimitationTest(null, "d1($filter=a or b and c)");
            Action action = () => this.LimitationTest(null, "d1($filter=(a or b) and c)");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void OrderByLimitTest()
        {
            this.LimitationTest(null, "d1($orderby=a or b and c)");
            Action action = () => this.LimitationTest(null, "d1($orderby=(a or b) and c)");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void SearchLimitTest()
        {
            this.LimitationTest(null, "d1($search=a OR b AND c)");
            Action action = () => this.LimitationTest(null, "d1($search=(a OR b) AND c)");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
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
            SelectExpandSyntacticParser.Parse(select, expand, null , configuration, out expandTree, out selectTree);
        }
        #endregion
    }
}
