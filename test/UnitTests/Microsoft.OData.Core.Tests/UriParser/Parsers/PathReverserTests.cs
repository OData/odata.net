//---------------------------------------------------------------------
// <copyright file="PathReverserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class PathReverserTests
    {
        [Fact]
        public void NullHeadReturnsNull()
        {
            // Arrange & Act
            PathSegmentToken head = null;
            PathSegmentToken reversedPath = head.Reverse();

            // Assert
            Assert.Null(reversedPath);
        }

        [Fact]
        public void ReversePathWorksWithNonSystemToken()
        {
            // Arrange: $expand=1/2
            PathSegmentToken nonReversedPath = new NonSystemToken("2", null, new NonSystemToken("1", null, null));
            Assert.Equal("2/1", nonReversedPath.ToPathString());

            // Act
            PathSegmentToken reversedPath = nonReversedPath.Reverse();

            // Assert
            reversedPath.ShouldBeNonSystemToken("1").NextToken.ShouldBeNonSystemToken("2");
            Assert.Equal("1/2", reversedPath.ToPathString());
        }

        [Fact]
        public void ReversePathWorksWithStarToken()
        {
            // Arrange: $expand=1/*
            PathSegmentToken nonReversedPath = new NonSystemToken("*", null, new NonSystemToken("1", null, null));
            Assert.Equal("*/1", nonReversedPath.ToPathString());

            // Act
            PathSegmentToken reversedPath = nonReversedPath.Reverse();

            // Assert
            reversedPath.ShouldBeNonSystemToken("1").NextToken.ShouldBeNonSystemToken("*");
            Assert.Equal("1/*", reversedPath.ToPathString());
        }

        [Fact]
        public void ReversePathWorksWithATypeToken()
        {
            // Arrange: $expand=Fully.Qualified.Namespace/1
            PathSegmentToken nonReversedPath = new NonSystemToken("1", null, new NonSystemToken("Fully.Qualified.Namespace", null, null));
            Assert.Equal("1/Fully.Qualified.Namespace", nonReversedPath.ToPathString());

            // Act
            PathSegmentToken reversedPath = nonReversedPath.Reverse();

            // Assert
            reversedPath.ShouldBeNonSystemToken("Fully.Qualified.Namespace").NextToken.ShouldBeNonSystemToken("1");
            Assert.Equal("Fully.Qualified.Namespace/1", reversedPath.ToPathString());
        }

        [Fact]
        public void ReversePathWorksWithSingleSegment()
        {
            // Arrange: $expand=1
            PathSegmentToken nonReversedPath = new NonSystemToken("1", null, null);

            // Act
            PathSegmentToken reversedPath = nonReversedPath.Reverse();

            // Assert
            NonSystemToken nonSystemToken = reversedPath.ShouldBeNonSystemToken("1");
            Assert.Null(nonSystemToken.NextToken);
        }

        [Fact]
        public void ReversePathWorksWithDeepPath()
        {
            // Arrange: $expand=1/2/3/4
            NonSystemToken endPath = new NonSystemToken("4", null, new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null))));
            Assert.Equal("4/3/2/1", endPath.ToPathString());

            // Act
            PathSegmentToken reversedPath = endPath.Reverse();

            // Assert
            reversedPath.ShouldBeNonSystemToken("1")
                .NextToken.ShouldBeNonSystemToken("2")
                .NextToken.ShouldBeNonSystemToken("3")
                .NextToken.ShouldBeNonSystemToken("4");
            Assert.Equal("1/2/3/4", reversedPath.ToPathString());
        }
    }
}
