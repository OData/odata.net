//---------------------------------------------------------------------
// <copyright file="SelectTreeNormalizerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
        public void NormalizeSelectTreeThrowsForMultipleTermsWithSameSelectPath()
        {
            // Arrange: $select=1($count=true), 1($count=false)
            List<SelectTermToken> selectTerms = new List<SelectTermToken>();
            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                null, null, null, null, true, null, null, null));

            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                null, null, null, null, false, null, null, null));

            SelectToken select = new SelectToken(selectTerms);

            // Act
            Action test = () => SelectTreeNormalizer.NormalizeSelectTree(select);

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal("Found mutliple select terms with same select path '1' at one $select, please combine them together.", exception.Message);
        }

        [Fact]
        public void NormalizeSelectTreeThrowsForMultipleSelectTermsInDeepLevel()
        {
            // Arrange: $select=1($select=2($top=5),2($count=true))
            List<SelectTermToken> selectTerms = new List<SelectTermToken>();
            selectTerms.Add(new SelectTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null),
                new SelectToken(new List<SelectTermToken>()
                {
                    new SelectTermToken(new NonSystemToken("2", null, null), null, null, 5, null, null, null, null, null),
                    new SelectTermToken(new NonSystemToken("2", null, null), null, null, null, null, true, null, null, null)
                })));

            SelectToken select = new SelectToken(selectTerms);

            // Act
            Action test = () => SelectTreeNormalizer.NormalizeSelectTree(select);

            // Assert
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal("Found mutliple select terms with same select path '2' at one $select, please combine them together.", exception.Message);
        }
    }
}
