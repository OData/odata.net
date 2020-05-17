//---------------------------------------------------------------------
// <copyright file="SelectExpandOptionParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests for the SelectExpandOptionParser class.
    /// </summary>
    public class SelectExpandOptionParserTests
    {
        #region Select

        [Fact]
        public void SelectTermTokenPathIsSet()
        {
            // Arrange
            PathSegmentToken pathToken = new NonSystemToken("SomeProp", null, null);
            SelectExpandOptionParser optionParser = new SelectExpandOptionParser(5);

            // Act
            SelectTermToken selectTermToken = optionParser.BuildSelectTermToken(pathToken, "");

            // Assert
            Assert.NotNull(selectTermToken);
            Assert.NotNull(selectTermToken.PathToProperty);
            Assert.Equal("SomeProp", selectTermToken.PathToProperty.Identifier);
        }

        [Fact]
        public void AllNestedOptionsAreNullIfNotPresentInSelect()
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions("");

            // Assert
            Assert.NotNull(selectTerm);
            Assert.Null(selectTerm.FilterOption);
            Assert.Null(selectTerm.OrderByOptions);
            Assert.Null(selectTerm.TopOption);
            Assert.Null(selectTerm.SkipOption);
            Assert.Null(selectTerm.CountQueryOption);
            Assert.Null(selectTerm.SearchOption);
            Assert.Null(selectTerm.SelectOption);
        }

        [Theory]
        [InlineData("($filter=true)")]
        [InlineData("($filter=true;)")]
        public void NestedFilterOptionInSelectIsAllowed(string optionsText)
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions(optionsText);

            // Assert
            Assert.NotNull(selectTerm);
            Assert.NotNull(selectTerm.FilterOption);
            selectTerm.FilterOption.ShouldBeLiteralQueryToken(true);
        }

        [Theory]
        [InlineData("($orderby=two)")]
        [InlineData("($orderby=two;)")]
        public void NestedOrderbyInSelectIsAllowed(string optionsText)
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions(optionsText);

            // Assert
            Assert.NotNull(selectTerm);
            Assert.NotNull(selectTerm.OrderByOptions);
            OrderByToken orderBy = Assert.Single(selectTerm.OrderByOptions);
            orderBy.Expression.ShouldBeEndPathToken("two");
        }

        [Theory]
        [InlineData("($top=24)")]
        [InlineData("($top=24;)")]
        public void NestedTopInSelectIsAllowed(string optionsText)
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions(optionsText);

            // Assert
            Assert.NotNull(selectTerm);
            Assert.NotNull(selectTerm.TopOption);
            Assert.Equal(24, selectTerm.TopOption);
        }

        [Theory]
        [InlineData("($skip=42)")]
        [InlineData("($skip=42;)")]
        public void NestedSkipInSelectIsAllowed(string optionsText)
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions(optionsText);

            // Assert
            Assert.NotNull(selectTerm);
            Assert.NotNull(selectTerm.SkipOption);
            Assert.Equal(42, selectTerm.SkipOption);
        }

        [Theory]
        [InlineData("($count=true)")]
        [InlineData("($count=true;)")]
        public void NestedCountInSelectIsAllowed(string optionsText)
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions(optionsText);

            // Assert
            Assert.NotNull(selectTerm);
            Assert.NotNull(selectTerm.CountQueryOption);
            Assert.True(selectTerm.CountQueryOption);
        }

        [Theory]
        [InlineData("($search=Searchme)")]
        [InlineData("($search=Searchme;)")]
        public void NestedSearchInSelectIsAllowed(string optionsText)
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions(optionsText);

            // Assert
            Assert.NotNull(selectTerm.SearchOption);
            selectTerm.SearchOption.ShouldBeStringLiteralToken("Searchme");
        }

        [Theory]
        [InlineData("($select=four,five)")]
        [InlineData("($select=four,five;)")]
        public void NestedSelectInSelectIsAllowed(string optionsText)
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions(optionsText);

            // Assert
            Assert.NotNull(selectTerm);
            Assert.NotNull(selectTerm.SelectOption);

            SelectTermToken[] innerSelectTerms = selectTerm.SelectOption.SelectTerms.ToArray();
            Assert.Equal(2, innerSelectTerms.Length);

            innerSelectTerms[0].PathToProperty.ShouldBeNonSystemToken("four");
            innerSelectTerms[1].PathToProperty.ShouldBeNonSystemToken("five");
        }

        [Fact]
        public void DeepNestedSelectInSelectIsAllowed()
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions("($select=four($select=subfour),five($top=3))");

            // Assert
            Assert.NotNull(selectTerm);
            Assert.NotNull(selectTerm.SelectOption);

            SelectTermToken[] innerSelectTerms = selectTerm.SelectOption.SelectTerms.ToArray();
            Assert.Equal(2, innerSelectTerms.Length);

            // #1
            innerSelectTerms[0].PathToProperty.ShouldBeNonSystemToken("four");
            Assert.NotNull(innerSelectTerms[0].SelectOption);
            SelectTermToken subFour = Assert.Single(innerSelectTerms[0].SelectOption.SelectTerms);
            subFour.PathToProperty.ShouldBeNonSystemToken("subfour");

            // #2
            innerSelectTerms[1].PathToProperty.ShouldBeNonSystemToken("five");
            Assert.NotNull(innerSelectTerms[1].TopOption);
            Assert.Equal(3, innerSelectTerms[1].TopOption);
        }

        [Fact]
        public void AllNestedQueryOptionsInSelectAreAllowed()
        {
            // Arrange & Act
            SelectTermToken selectTerm = this.ParseSelectOptions("($select=one($select=subone);$filter=true;$count=true;$top=1;$skip=2)");

            // Assert
            Assert.NotNull(selectTerm);

            // $select
            Assert.NotNull(selectTerm.SelectOption);
            SelectTermToken innerSelectTerms = Assert.Single(selectTerm.SelectOption.SelectTerms);
            innerSelectTerms.PathToProperty.ShouldBeNonSystemToken("one");

            SelectTermToken innerInnerSelectTerms = Assert.Single(innerSelectTerms.SelectOption.SelectTerms);
            innerInnerSelectTerms.PathToProperty.ShouldBeNonSystemToken("subone");

            // $filter
            Assert.NotNull(selectTerm.FilterOption);
            selectTerm.FilterOption.ShouldBeLiteralQueryToken(true);

            // $count
            Assert.NotNull(selectTerm.CountQueryOption);
            Assert.True(selectTerm.CountQueryOption);

            // $top
            Assert.NotNull(selectTerm.TopOption);
            Assert.Equal(1, selectTerm.TopOption);

            // $skip
            Assert.NotNull(selectTerm.SkipOption);
            Assert.Equal(2, selectTerm.SkipOption);
        }

        [Fact]
        public void InvalidTopTextInSelectShouldThrowInTop()
        {
            // Assert & Act
            Action test = () => this.ParseSelectOptions("($top=foo;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidTopOption("foo"), exception.Message);
        }

        [Fact]
        public void NegativeValueTopInSelectShouldThrowInTop()
        {
            // Assert & Act
            Action test = () => this.ParseExpandOptions("($top=-3;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidTopOption("-3"), exception.Message);
        }

        [Fact]
        public void InvalidSkipTextInSelectShouldThrowInSkip()
        {
            // Assert & Act
            Action test = () => this.ParseSelectOptions("($skip=foo;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidSkipOption("foo"), exception.Message);
        }

        [Fact]
        public void NegativeValueSkipInSelectShouldThrowInSkip()
        {
            // Assert & Act
            Action test = () => this.ParseSelectOptions("($skip=-4;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidSkipOption("-4"), exception.Message);
        }

        [Fact]
        public void WithoutTextInRoundBracketInSelectShouldThrow()
        {
            // Assert & Act
            Action test = () => this.ParseSelectOptions("()");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriParser_MissingSelectOption("Property"), exception.Message);
        }

        [Fact]
        public void MissingRoundBracketInSelectShouldThrowInSelect()
        {
            // Assert & Act
            Action test = () => this.ParseSelectOptions("($select=prop");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ExpressionLexer_SyntaxError(13, "($select=prop"), exception.Message);
        }

        private SelectTermToken ParseSelectOptions(string optionsText, int maxDepth = 100)
        {
            PathSegmentToken pathToken = new NonSystemToken("Property", null, null);

            SelectExpandOptionParser optionParser = new SelectExpandOptionParser(maxDepth)
            {
                MaxFilterDepth = 9,
                MaxSearchDepth = 9,
                MaxOrderByDepth = 9
            };

            return optionParser.BuildSelectTermToken(pathToken, optionsText);
        }

        #endregion

        #region Expand

        [Fact]
        public void ExpandTermTokenPathIsSet()
        {
            // Arrange
            PathSegmentToken pathToken = new NonSystemToken("SomeNavProp", null, null);
            SelectExpandOptionParser optionParser = new SelectExpandOptionParser(5);

            // Act
            IList<ExpandTermToken> expandTerms = optionParser.BuildExpandTermToken(pathToken, "");

            // Assert
            Assert.NotNull(expandTerms);
            ExpandTermToken expandTerm = Assert.Single(expandTerms);
            expandTerm.PathToNavigationProp.ShouldBeNonSystemToken("SomeNavProp");
        }

        [Fact]
        public void AllNestedOptionsAreNullIfNotPresentInExpand()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions("");

            // Assert
            Assert.NotNull(expandTerm);
            Assert.Null(expandTerm.FilterOption);
            Assert.Null(expandTerm.TopOption);
            Assert.Null(expandTerm.SkipOption);
            Assert.Null(expandTerm.OrderByOptions);
            Assert.Null(expandTerm.SelectOption);
            Assert.Null(expandTerm.ExpandOption);
            Assert.Null(expandTerm.LevelsOption);
            Assert.Null(expandTerm.SearchOption);
            Assert.Null(expandTerm.ComputeOption);
            Assert.Null(expandTerm.ApplyOptions);
        }

        [Theory]
        [InlineData("($filter=true)")]
        [InlineData("($filter=true;)")]
        public void NestedFilterOptionInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.FilterOption);
            expandTerm.FilterOption.ShouldBeLiteralQueryToken(true);
        }

        [Fact]
        public void CanHaveAnOptionMoreThanOnceInExpand()
        {
            // Arrange & Act
            // This test was once written to support only one instance of filter
            // but on 6.x and 7.x, it is supported.
            ExpandTermToken expandTerm = this.ParseExpandOptions("($filter=true; $filter=false)");

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.FilterOption);
            expandTerm.FilterOption.ShouldBeLiteralQueryToken(false); // not true, the last won.
        }

        [Theory]
        [InlineData("($filter=1 eq 2)")]
        [InlineData("($filter=1 eq 2;)")]
        public void NestedFilterWithExpressionIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.FilterOption);
            BinaryOperatorToken binaryOperatorToken = expandTerm.FilterOption.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
            binaryOperatorToken.Left.ShouldBeLiteralQueryToken(1);
            binaryOperatorToken.Right.ShouldBeLiteralQueryToken(2);
        }

        [Theory]
        [InlineData("($orderby=two)")]
        [InlineData("($orderby=two;)")]
        public void ExpandOptionOrderbySyntaxWorks(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.OrderByOptions);
            expandTerm.OrderByOptions.Single().Expression.ShouldBeEndPathToken("two");
        }

        [Theory]
        [InlineData("($top=4)")]
        [InlineData("($top=4;)")]
        public void NestedTopInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.TopOption);
            Assert.Equal(4, expandTerm.TopOption);

        }

        [Theory]
        [InlineData("($skip=3)")]
        [InlineData("($skip=3;)")]
        public void NestedSkipInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.SkipOption);
            Assert.Equal(3, expandTerm.SkipOption);
        }

        [Theory]
        [InlineData("($count=true)")]
        [InlineData("($count=true;)")]
        public void NestedCountInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.CountQueryOption);
            Assert.True(expandTerm.CountQueryOption);
        }

        [Theory]
        [InlineData("($levels=3)")]
        [InlineData("($levels=3;)")]
        public void LevelsOptionInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.LevelsOption);
            Assert.Equal(3, expandTerm.LevelsOption);
        }

        [Theory]
        [InlineData("($levels=max)")]
        [InlineData("($levels=max;)")]
        public void LevelsOptionWithMaxValueInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.LevelsOption);
            Assert.Equal(long.MinValue, expandTerm.LevelsOption);
        }

        [Theory]
        [InlineData("($search=Searchme)")]
        [InlineData("($search=Searchme;)")]
        public void SearchOptionInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.SearchOption);
            expandTerm.SearchOption.ShouldBeStringLiteralToken("Searchme");
        }

        [Theory]
        [InlineData("($select=four,five)")]
        [InlineData("($select=four,five;)")]
        public void NestedSelectInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.SelectOption);

            SelectTermToken[] innerSelectTerms = expandTerm.SelectOption.SelectTerms.ToArray();
            Assert.Equal(2, innerSelectTerms.Length);
            innerSelectTerms[0].PathToProperty.ShouldBeNonSystemToken("four");
            innerSelectTerms[1].PathToProperty.ShouldBeNonSystemToken("five");
        }

        [Theory]
        [InlineData("($compute=Price mul Qty as TotalPrice)")]
        [InlineData("($compute=Price mul Qty as TotalPrice;)")]
        public void NestedComputeInExpandIsAllowed(string optionsText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionsText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.ComputeOption);
            Assert.NotNull(expandTerm.ComputeOption.Expressions);
            ComputeExpressionToken computeExpressionToken = Assert.Single(expandTerm.ComputeOption.Expressions);
            Assert.Equal("TotalPrice", computeExpressionToken.Alias);

            BinaryOperatorToken binaryOperatorToken = computeExpressionToken.Expression.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Multiply);
            binaryOperatorToken.Left.ShouldBeEndPathToken("Price");
            binaryOperatorToken.Right.ShouldBeEndPathToken("Qty");
        }

        [Fact]
        public void NestedComputeAndSelectInExpandIsAllowed()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions("($compute=Price mul Qty as TotalPrice;$select=Name,Qty,TotalPrice)");

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.ComputeOption);
            Assert.NotNull(expandTerm.ComputeOption.Expressions);

            ComputeExpressionToken computeExpressionToken = Assert.Single(expandTerm.ComputeOption.Expressions);
            Assert.Equal("TotalPrice", computeExpressionToken.Alias);

            BinaryOperatorToken binaryOperatorToken = computeExpressionToken.Expression.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Multiply);
            binaryOperatorToken.Left.ShouldBeEndPathToken("Price");
            binaryOperatorToken.Right.ShouldBeEndPathToken("Qty");

            SelectTermToken[] selectTerms = expandTerm.SelectOption.SelectTerms.ToArray();
            Assert.Equal(3, selectTerms.Length);
            selectTerms[0].PathToProperty.ShouldBeNonSystemToken("Name");
            selectTerms[1].PathToProperty.ShouldBeNonSystemToken("Qty");
            selectTerms[2].PathToProperty.ShouldBeNonSystemToken("TotalPrice");
        }

        // for example: $expand=Orders($apply=aggregate(Amount with sum as Total))
        [Fact]
        public void NestedAggregateTransformationInExpandIsAllowed()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions("($apply=aggregate(Amount with sum as Total))");

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.ApplyOptions);

            QueryToken token = Assert.Single(expandTerm.ApplyOptions);
            AggregateToken aggregate = Assert.IsType<AggregateToken>(token);
            AggregateExpressionToken aggregateExpressionToken = Assert.IsType<AggregateExpressionToken>(Assert.Single(aggregate.AggregateExpressions));
            Assert.Equal("Total", aggregateExpressionToken.Alias);
            Assert.Equal(AggregationMethod.Sum, aggregateExpressionToken.Method);
            aggregateExpressionToken.Expression.ShouldBeEndPathToken("Amount");
        }

        // for example: $expand=Orders($apply=compute(Amount mul Product/TaxRate as Tax))
        [Fact]
        public void NestedComputeTransformationInExpandIsAllowed()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions("($apply=compute(Amount mul Product/TaxRate as Tax))");

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.ApplyOptions);

            QueryToken token = Assert.Single(expandTerm.ApplyOptions);
            ComputeToken compute = Assert.IsType<ComputeToken>(token);

            ComputeExpressionToken computeExpressionToken = Assert.Single(compute.Expressions);
            Assert.Equal("Tax", computeExpressionToken.Alias);

            BinaryOperatorToken binaryOperatorToken = computeExpressionToken.Expression.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Multiply);
            binaryOperatorToken.Left.ShouldBeEndPathToken("Amount");
            EndPathToken right = binaryOperatorToken.Right.ShouldBeEndPathToken("TaxRate");
            Assert.NotNull(right.NextToken);
            right.NextToken.ShouldBeInnerPathToken("Product");
        }

        // for example: $expand=Orders($apply=groupby((Customer/Country,Product/Name),aggregate(Amount with sum as Total)))
        [Fact]
        public void NestedGroupbyTransformationsInExpandIsAllowed()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions("($apply=groupby((Customer/CountryRegion,Product/Name),aggregate(Amount with sum as Total)))");

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.ApplyOptions);
            QueryToken token = Assert.Single(expandTerm.ApplyOptions);

            GroupByToken groupBy = Assert.IsType< GroupByToken>(token);
            Assert.Equal(2, groupBy.Properties.Count());

            QueryToken queryToken = groupBy.Properties.ElementAt(0);
            EndPathToken pathToken = queryToken.ShouldBeEndPathToken("CountryRegion");
            pathToken.NextToken.ShouldBeInnerPathToken("Customer");

            queryToken = groupBy.Properties.ElementAt(1);
            pathToken = queryToken.ShouldBeEndPathToken("Name");
            pathToken.NextToken.ShouldBeInnerPathToken("Product");

            Assert.NotNull(groupBy.Child);
            AggregateToken aggregate = Assert.IsType<AggregateToken>(groupBy.Child);
            AggregateExpressionToken aggregateExpressionToken = Assert.IsType<AggregateExpressionToken>(Assert.Single(aggregate.AggregateExpressions));

            Assert.Equal("Total", aggregateExpressionToken.Alias);
            Assert.Equal(AggregationMethod.Sum, aggregateExpressionToken.Method);
            aggregateExpressionToken.Expression.ShouldBeEndPathToken("Amount");
        }

        [Fact]
        public void NestedExpandsInExpandAreAllowed()
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions("($expand=two)");

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.ExpandOption);

            ExpandTermToken two = Assert.Single(expandTerm.ExpandOption.ExpandTerms);
            two.PathToNavigationProp.ShouldBeNonSystemToken("two");
        }

        [Theory]
        [InlineData("($expand=two($expand=three))")]
        [InlineData("($expand=two($expand=three;);)")]
        public void DeepNestedExpandsAreAllowed(string optionText)
        {
            // Arrange & Act
            ExpandTermToken expandTerm = this.ParseExpandOptions(optionText);

            // Assert
            Assert.NotNull(expandTerm);
            Assert.NotNull(expandTerm.ExpandOption);

            ExpandTermToken two = Assert.Single(expandTerm.ExpandOption.ExpandTerms);
            two.PathToNavigationProp.ShouldBeNonSystemToken("two");

            ExpandTermToken three = Assert.Single(two.ExpandOption.ExpandTerms);
            three.PathToNavigationProp.ShouldBeNonSystemToken("three");
        }

        [Fact]
        public void InvalidTopTextInExpandShouldThrowInTop()
        {
            // Arrange & Act
            Action test = () => this.ParseExpandOptions("($top=foo;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidTopOption("foo"), exception.Message);
        }

        [Fact]
        public void NegativeTopValueInExpandShouldThrowInTop()
        {
            // Arrange & Act
            Action test = () => this.ParseExpandOptions("($top=-3;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidTopOption("-3"), exception.Message);
        }

        [Fact]
        public void InvalidSkipTextInExpandShouldThrowInSkip()
        {
            // Arrange & Act
            Action test = () => this.ParseExpandOptions("($skip=foo;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidSkipOption("foo"), exception.Message);
        }

        [Fact]
        public void NegativeSkipValueInExpandShouldThrowInSkip()
        {
            // Arrange & Act
            Action test = () => this.ParseExpandOptions("($skip=-4;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidSkipOption("-4"), exception.Message);
        }

        [Fact]
        public void InvalidLevelTextInExpandShouldThrowInLevels()
        {
            // Arrange & Act
            Action test = () => this.ParseExpandOptions("($levels=foo;)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidLevelsOption("foo"), exception.Message);
        }

        [Fact]
        public void NegativeLevelValueInExpandShouldThrowInLevels()
        {
            // Arrange & Act
            Action test = () => this.ParseExpandOptions("($levels=-5)");

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.UriSelectParser_InvalidLevelsOption("-5"), exception.Message);
        }

        // TODO: Probably missing more simple test cases
        // TODO: Write interesting, complex parsing cases

        private ExpandTermToken ParseExpandOptions(string optionsText, int maxDepth = 100)
        {
            PathSegmentToken pathToken = new NonSystemToken("NavProp", null, null);

            SelectExpandOptionParser optionParser = new SelectExpandOptionParser(maxDepth)
            {
                MaxFilterDepth = 9,
                MaxSearchDepth = 9,
                MaxOrderByDepth = 9
            };

            return optionParser.BuildExpandTermToken(pathToken, optionsText).ElementAt(0);
        }

        #endregion
    }
}
