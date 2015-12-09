//---------------------------------------------------------------------
// <copyright file="PathReverserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    public class PathReverserTests
    {
        [Fact]
        public void ReversePathWorksWithNonSystemToken()
        {
            // $expand=1/2
            PathReverser pathReverser = new PathReverser();
            PathSegmentToken nonReversedPath = new NonSystemToken("2", null, new NonSystemToken("1", null, null));
            PathSegmentToken reversedPath = nonReversedPath.Accept(pathReverser);
            reversedPath.ShouldBeNonSystemToken("1").And.NextToken.ShouldBeNonSystemToken("2");
        }

        [Fact]
        public void ReversePathWorksWithStarToken()
        {
            // $expand=1/*
            PathReverser pathReverser = new PathReverser();
            PathSegmentToken nonReversedPath = new NonSystemToken("*", null, new NonSystemToken("1", null, null));
            PathSegmentToken reversedPath = nonReversedPath.Accept(pathReverser);
            reversedPath.ShouldBeNonSystemToken("1").And.NextToken.ShouldBeNonSystemToken("*");
        }

        [Fact]
        public void ReversePathWorksWithATypeToken()
        {
            // $expand=Fully.Qualified.Namespace/1
            PathReverser pathReverser = new PathReverser();
            PathSegmentToken nonReversedPath = new NonSystemToken("1", null, new NonSystemToken("Fully.Qualified.Namespace", null, null));
            PathSegmentToken reversedPath = nonReversedPath.Accept(pathReverser);
            reversedPath.ShouldBeNonSystemToken("Fully.Qualified.Namespace").And.NextToken.ShouldBeNonSystemToken("1");
        }

        [Fact]
        public void ReversePathWorksWithSingleSegment()
        {
            // $expand=1
            PathReverser pathReverser = new PathReverser();
            PathSegmentToken nonReversedPath = new NonSystemToken("1", null, null);
            PathSegmentToken reversedPath = nonReversedPath.Accept(pathReverser);
            reversedPath.ShouldBeNonSystemToken("1").And.NextToken.Should().BeNull();
        }

        [Fact]
        public void ReversePathWorksWithDeepPath()
        {
            // $expand=1/2/3/4
            PathReverser pathReverser = new PathReverser();
            NonSystemToken endPath = new NonSystemToken("4", null, new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null))));
            PathSegmentToken reversedPath = endPath.Accept(pathReverser);
            reversedPath.ShouldBeNonSystemToken("1")
                .And.NextToken.ShouldBeNonSystemToken("2")
                .And.NextToken.ShouldBeNonSystemToken("3")
                .And.NextToken.ShouldBeNonSystemToken("4");
        }
    }
}
