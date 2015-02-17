//---------------------------------------------------------------------
// <copyright file="SyntacticPathParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests that the syntactic parser can parse a path.
    /// </summary>
    [TestClass]
    public class SyntacticPathParserTests
    {
        private UriPathParser pathParser;
        private readonly Uri baseUri = new Uri("http://www.example.com/");

        [TestInitialize]
        public void Init()
        {
            this.pathParser = new UriPathParser(1000);
        }

        [TestMethod]
        public void ParsePathReturnsSegmentQueryTokensForEachSegment()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "one/two/three"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => s.Should().Be("one"),
                s => s.Should().Be("two"),
                s => s.Should().Be("three")
            });
        }

        [TestMethod]
        public void SegmentWithParensShouldStayAsOneSegment()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "EntitySet('KeyValue')"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => s.Should().Be("EntitySet('KeyValue')"),
            });
        }

        [TestMethod]
        public void SegmentWithMultiPartKeyStoredThemAll()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "EntitySet(first=1,second=2)"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => s.Should().Be("EntitySet(first=1,second=2)"),
            });
        }

        [TestMethod]
        public void ParsePathIgnoresQueryString()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "one?foo=bar/bar$cool"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => s.Should().Be("one"),
            });
        }

        /// <summary>
        /// Enumerates the segments in a path and calls a corresponding delegate verifier on each segment.
        /// </summary>
        private void VerifyPath(IEnumerable<string> path, Action<string>[] segmentVerifiers)
        {
            path.Count().Should().Be(segmentVerifiers.Count());

            var i = 0;
            foreach (var segment in path)
            {
                segmentVerifiers[i++](segment);
            }
        }
    }
}
