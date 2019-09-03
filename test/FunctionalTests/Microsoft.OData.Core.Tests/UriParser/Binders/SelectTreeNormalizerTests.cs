//---------------------------------------------------------------------
// <copyright file="SelectTreeNormalizerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class SelectTreeNormalizerTests
    {
        [Fact]
        public void NormalizeTreeResultsInReversedPath()
        {
            // Arrange: $select=1/2/3
            NonSystemToken endPath = new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null)));
            Assert.Equal("3/2/1", endPath.ToPathString());

            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(endPath)
            });

            // Act
            SelectToken normalizedToken = SelectTreeNormalizer.NormalizeSelectTree(selectToken);

            // Assert
            Assert.NotNull(normalizedToken);
            SelectTermToken updatedSegmentToken = Assert.Single(normalizedToken.SelectTerms);
            PathSegmentToken segmentToken = updatedSegmentToken.PathToProperty;
            segmentToken.ShouldBeNonSystemToken("1")
                .NextToken.ShouldBeNonSystemToken("2")
                .NextToken.ShouldBeNonSystemToken("3");

            Assert.Equal("1/2/3", segmentToken.ToPathString());
        }

        [Fact]
        public void NormalizeTreeWorksForMultipleTerms()
        {
            // Arrange: $select=1/2/3,4/5/6
            NonSystemToken endPath1 = new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null)));
            Assert.Equal("3/2/1", endPath1.ToPathString());

            NonSystemToken endPath2 = new NonSystemToken("6", null, new NonSystemToken("5", null, new NonSystemToken("4", null, null)));
            Assert.Equal("6/5/4", endPath2.ToPathString());

            // Act
            SelectToken selectToken = new SelectToken(new SelectTermToken[]
            {
                new SelectTermToken(endPath1), new SelectTermToken(endPath2)
            });
            SelectToken normalizedToken = SelectTreeNormalizer.NormalizeSelectTree(selectToken);

            // Assert
            List<PathSegmentToken> tokens = normalizedToken.Properties.ToList();
            Assert.Equal(2, tokens.Count);

            tokens[0].ShouldBeNonSystemToken("1")
                .NextToken.ShouldBeNonSystemToken("2")
                .NextToken.ShouldBeNonSystemToken("3");
            Assert.Equal("1/2/3", tokens[0].ToPathString());

            tokens[1].ShouldBeNonSystemToken("4")
                .NextToken.ShouldBeNonSystemToken("5")
                .NextToken.ShouldBeNonSystemToken("6");
            Assert.Equal("4/5/6", tokens[1].ToPathString());
        }

        [Fact]
        public void CombineTermsThrowsForMultipleCountOptions()
        {
            // Arrange: $select=1($count=true), 1($count=false)
            List<SelectTermToken> selectTerms = new List<SelectTermToken>();
            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                null, null, null, null, true, null, null, null));

            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                null, null, null, null, false, null, null, null));

            SelectToken select = new SelectToken(selectTerms);

            // Act
            System.Action test = () => SelectTreeNormalizer.CombineSelectToken(select);

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal("Found mutliple '$count' query options at one $select.", exception.Message);
        }

        [Fact]
        public void CombineTermsWorksForMultipleSelectQueryOptions()
        {
            // Arrange: $select=1($select=2;$top=5), 1($select=3;$count=true)
            List<SelectTermToken> selectTerms = new List<SelectTermToken>();
            var token2 = new NonSystemToken("2", null, null);
            var token3 = new NonSystemToken("3", null, null);
            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                null, null, 5, null, null, null,
                new SelectToken(new List<SelectTermToken>() { new SelectTermToken(token2) }), null));

            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                null, null, null, null, true, null,
                new SelectToken(new List<SelectTermToken>() { new SelectTermToken(token3) }), null));

            SelectToken select = new SelectToken(selectTerms);

            // Act
            SelectToken combinedSelect = SelectTreeNormalizer.CombineSelectToken(select);

            // Assert
            SelectTermToken finalTermToken = Assert.Single(combinedSelect.SelectTerms);
            finalTermToken.ShouldBeSelectTermToken("1", true);

            // $top
            Assert.NotNull(finalTermToken.TopOption);
            Assert.Equal(5, finalTermToken.TopOption);

            // $count
            Assert.NotNull(finalTermToken.CountQueryOption);
            Assert.True(finalTermToken.CountQueryOption);

            Assert.Null(finalTermToken.FilterOption);
            Assert.Null(finalTermToken.OrderByOptions);
            Assert.Null(finalTermToken.SkipOption);
            Assert.Null(finalTermToken.ComputeOption);

            // $select
            Assert.NotNull(finalTermToken.SelectOption);
            Assert.Equal(2, finalTermToken.SelectOption.SelectTerms.Count());

            SelectTermToken innnerTermToken = finalTermToken.SelectOption.SelectTerms.ElementAt(0);
            innnerTermToken.ShouldBeSelectTermToken("2", true);

            innnerTermToken = finalTermToken.SelectOption.SelectTerms.ElementAt(1);
            innnerTermToken.ShouldBeSelectTermToken("3", true);

            string originalSelectPath = new SelectExpandTokenSyntacticTreeVisitor().Visit(select);
            Assert.Equal("$select=1($top=5;$select=2),1($count=true;$select=3)", originalSelectPath);

            string combinedSelectPath = new SelectExpandTokenSyntacticTreeVisitor().Visit(combinedSelect);
            Assert.Equal("$select=1($top=5;$count=true;$select=2,3)", combinedSelectPath);
        }

        [Fact]
        public void CombineTermsWorksForMultipleSelectQueryOptionsAtDifferentLevel()
        {
            // Arrange: $select=1($top=8;$select=2($top=5)),1($count=false;$select=3,2($count=true))
            List<SelectTermToken> selectTerms = new List<SelectTermToken>();
            var token2 = new NonSystemToken("2", null, null);
            var token3 = new NonSystemToken("3", null, null);
            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                null, null, 8, null, null, null,
                new SelectToken(new List<SelectTermToken>() { new SelectTermToken(token2, null, 
                null, 5, null, null, null, null, null) }), null));

            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                null, null, null, null, false, null,
                new SelectToken(new List<SelectTermToken>() { new SelectTermToken(token3), new SelectTermToken(token2, null,
                null, null, null, true, null, null, null)}), null));

            SelectToken select = new SelectToken(selectTerms);

            // Act
            SelectToken combinedSelect = SelectTreeNormalizer.CombineSelectToken(select);

            // Assert
            SelectTermToken finalTermToken = Assert.Single(combinedSelect.SelectTerms);
            finalTermToken.ShouldBeSelectTermToken("1", true);

            Assert.NotNull(finalTermToken.TopOption);
            Assert.Equal(8, finalTermToken.TopOption);

            Assert.NotNull(finalTermToken.CountQueryOption);
            Assert.False(finalTermToken.CountQueryOption);

            Assert.Null(finalTermToken.FilterOption);
            Assert.Null(finalTermToken.OrderByOptions);
            Assert.Null(finalTermToken.SkipOption);
            Assert.Null(finalTermToken.ComputeOption);

            Assert.NotNull(finalTermToken.SelectOption);
            Assert.Equal(2, finalTermToken.SelectOption.SelectTerms.Count());

            SelectTermToken innnerTermToken = finalTermToken.SelectOption.SelectTerms.ElementAt(0);
            innnerTermToken.ShouldBeSelectTermToken("2", true);

            innnerTermToken = finalTermToken.SelectOption.SelectTerms.ElementAt(1);
            innnerTermToken.ShouldBeSelectTermToken("3", true);

            string originalSelectPath = new SelectExpandTokenSyntacticTreeVisitor().Visit(select);
            Assert.Equal("$select=1($top=8;$select=2($top=5)),1($count=false;$select=3,2($count=true))", originalSelectPath);

            string combinedSelectPath = new SelectExpandTokenSyntacticTreeVisitor().Visit(combinedSelect);
            Assert.Equal("$select=1($top=8;$count=false;$select=2($top=5;$count=true),3)", combinedSelectPath);
        }
    }
}
