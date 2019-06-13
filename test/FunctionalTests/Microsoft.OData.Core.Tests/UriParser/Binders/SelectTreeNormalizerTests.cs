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

            SelectToken selectToken = new SelectToken(new NonSystemToken[]{endPath});

            // Act
            SelectToken normalizedToken = SelectTreeNormalizer.NormalizeSelectTree(selectToken);

            // Assert
            Assert.NotNull(normalizedToken);
            PathSegmentToken updatedSegmentToken = Assert.Single(normalizedToken.Properties);
            updatedSegmentToken.ShouldBeNonSystemToken("1")
                .NextToken.ShouldBeNonSystemToken("2")
                .NextToken.ShouldBeNonSystemToken("3");

            Assert.Equal("1/2/3", updatedSegmentToken.ToPathString());
        }

        [Fact]
        public void NormalizeTreeWorksForMultipleTerms()
        {
            // Arrange: $select=1/2/3,4/5/6
            NonSystemToken endPath = new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null)));
            Assert.Equal("3/2/1", endPath.ToPathString());

            NonSystemToken endPath1 = new NonSystemToken("6", null, new NonSystemToken("5", null, new NonSystemToken("4", null, null)));
            Assert.Equal("6/5/4", endPath1.ToPathString());

            // Act
            SelectToken selectToken = new SelectToken(new NonSystemToken[] { endPath, endPath1 });
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
    }
}
