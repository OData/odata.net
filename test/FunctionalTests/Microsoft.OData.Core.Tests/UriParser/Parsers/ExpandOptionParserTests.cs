//---------------------------------------------------------------------
// <copyright file="ExpandOptionParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests for the ExpandOptionParser class.
    /// </summary>
    public class ExpandOptionParserTests
    {
        [Fact]
        public void ExpandTermTokenPathIsSet()
        {
            PathSegmentToken pathToken = new NonSystemToken("SomeNavProp", null, null);

            ExpandOptionParser optionParser = new ExpandOptionParser(5);
            var termToken = optionParser.BuildExpandTermToken(pathToken, "");

            termToken.ElementAt(0).PathToNavigationProp.Should().Be(pathToken);
        }

        [Fact]
        public void NestedFilterOptionIsAllowed()
        {
            var result = this.ParseExpandOptions("($filter=true)");
            result.FilterOption.ShouldBeLiteralQueryToken(true);
        }

        [Fact]
        public void NestedFilterOptionWithTrailingSemicolonIsAllowed()
        {
            var result = this.ParseExpandOptions("($filter=true;)");
            result.FilterOption.ShouldBeLiteralQueryToken(true);
        }

        [Fact]
        public void NestedFilterWithExpressionIsAllowed()
        {
            var result = this.ParseExpandOptions("($filter=1 eq 2)");
            BinaryOperatorToken binaryOperatorToken = result.FilterOption.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal).And;
            binaryOperatorToken.Left.ShouldBeLiteralQueryToken(1);
            binaryOperatorToken.Right.ShouldBeLiteralQueryToken(2);
        }

        [Fact]
        public void ExpandOptionOrderbySyntaxWorks()
        {
            var result = this.ParseExpandOptions("($orderby=two)");
            result.OrderByOptions.Single().Expression.ShouldBeEndPathToken("two");
        }

        [Fact]
        public void NestedTopIsAllowed()
        {
            var result = this.ParseExpandOptions("($top=4)");
            result.TopOption.Should().Be(4);
        }

        [Fact]
        public void NestedSkipIsAllowed()
        {
            var result = this.ParseExpandOptions("($skip=3)");
            result.SkipOption.Should().Be(3);
        }

        [Fact]
        public void NestedCountIsAllowed()
        {
            var result = this.ParseExpandOptions("($count=true)");
            result.CountQueryOption.Should().BeTrue();
        }

        [Fact]
        public void LevelsOptionIsAllowed()
        {
            var result = this.ParseExpandOptions("($levels=3)");
            result.LevelsOption.Should().Be(3);
        }

        [Fact]
        public void LevelsOptionWithMaxValueIsAllowed()
        {
            var result = this.ParseExpandOptions("($levels=max)");
            result.LevelsOption.Should().Be(long.MinValue);
        }

        [Fact]
        public void SearchOptionIsAllowed()
        {
            var result = this.ParseExpandOptions("($search=Searchme)");
            result.SearchOption.ShouldBeStringLiteralToken("Searchme");
        }

        [Fact]
        public void NestedSelectIsAllowed()
        {
            var result = this.ParseExpandOptions("($select=four,five)");
            IEnumerable<PathSegmentToken> selectTerms = result.SelectOption.Properties;
            selectTerms.Should().HaveCount(2);
            selectTerms.ElementAt(0).ShouldBeNonSystemToken("four");
            selectTerms.ElementAt(1).ShouldBeNonSystemToken("five");
        }

        [Fact]
        public void NestedExpandsAreAllowed()
        {
            var result = this.ParseExpandOptions("($expand=two)");
            ExpandTermToken two = result.ExpandOption.ExpandTerms.Single();
            two.PathToNavigationProp.ShouldBeNonSystemToken("two");
        }

        [Fact]
        public void DeepNestedExpandsAreAllowed()
        {
            var result = this.ParseExpandOptions("($expand=two($expand=three))");
            ExpandTermToken two = result.ExpandOption.ExpandTerms.Single();
            two.PathToNavigationProp.ShouldBeNonSystemToken("two");
            ExpandTermToken three = two.ExpandOption.ExpandTerms.Single();
            three.PathToNavigationProp.ShouldBeNonSystemToken("three");
        }

        [Fact]
        public void DeepNestedExpandsWithSemisAreAllowed()
        {
            var result = this.ParseExpandOptions("($expand=two($expand=three;);)");
            ExpandTermToken two = result.ExpandOption.ExpandTerms.Single();
            two.PathToNavigationProp.ShouldBeNonSystemToken("two");
            ExpandTermToken three = two.ExpandOption.ExpandTerms.Single();
            three.PathToNavigationProp.ShouldBeNonSystemToken("three");
        }

        [Fact]
        public void CanHaveAnOptionMoreThanOnce()
        {
            // This test was once written to support only one instance of filter
            // but on 6.x and 7.x, it is supported.
            this.ParseExpandOptions("($filter=true; $filter=false)");
        }

        [Fact]
        public void AllOptionsAreNullIfNotPresent()
        {
            var result = this.ParseExpandOptions("");
            result.TopOption.Should().Be(null);
            result.SkipOption.Should().Be(null);
            result.FilterOption.Should().Be(null);
            result.OrderByOptions.Should().BeNull();
            result.SelectOption.Should().Be(null);
            result.ExpandOption.Should().Be(null);
            result.CountQueryOption.Should().Be(null);
            result.LevelsOption.Should().Be(null);
            result.SearchOption.Should().Be(null);
        }

        [Fact]
        public void InvalidTextShouldThrowInTop()
        {
            Action parseWithInvalidText = () => this.ParseExpandOptions("($top=foo;)");
            parseWithInvalidText.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidTopOption("foo"));
        }

        [Fact]
        public void NegativeValueShouldThrowInTop()
        {
            Action parseWithInvalidText = () => this.ParseExpandOptions("($top=-3;)");
            parseWithInvalidText.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidTopOption("-3"));
        }

        [Fact]
        public void InvalidTextShouldThrowInSkip()
        {
            Action parseWithInvalidText = () => this.ParseExpandOptions("($skip=foo;)");
            parseWithInvalidText.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidSkipOption("foo"));
        }

        [Fact]
        public void NegativeValueShouldThrowInSkip()
        {
            Action parseWithInvalidText = () => this.ParseExpandOptions("($skip=-4;)");
            parseWithInvalidText.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidSkipOption("-4"));
        }

        [Fact]
        public void InvalidTextShouldThrowInLevels()
        {
            Action parseWithInvalidText = () => this.ParseExpandOptions("($levels=foo;)");
            parseWithInvalidText.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidLevelsOption("foo"));
        }

        [Fact]
        public void NegativeValueShouldThrowInLevels()
        {
            Action parseWithInvalidText = () => this.ParseExpandOptions("($levels=-5)");
            parseWithInvalidText.ShouldThrow<ODataException>().WithMessage(Strings.UriSelectParser_InvalidLevelsOption("-5"));
        }

        // TODO: Probably missing more simple test cases
        // TODO: Write interesting, complex parsing cases

        private ExpandTermToken ParseExpandOptions(string optionsText, int maxDepth = 100)
        {
            PathSegmentToken pathToken = new NonSystemToken("NavProp", null, null);

            ExpandOptionParser optionParser = new ExpandOptionParser(maxDepth)
            {
                MaxFilterDepth = 9,
                MaxSearchDepth = 9,
                MaxOrderByDepth = 9
            };
            return optionParser.BuildExpandTermToken(pathToken, optionsText).ElementAt(0);
        }
    }
}
