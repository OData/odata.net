//---------------------------------------------------------------------
// <copyright file="UriPathParserTests.cs" company="Microsoft">
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
    public class UriPathParserTests
    {
        private readonly Uri baseUri = new Uri("http://www.example.com/Tests/OData.svc/");
        private UriPathParser pathParser;
        private ODataUriParserSettings uriParserSettings = new ODataUriParserSettings() { PathLimit = 1000 };

        public UriPathParserTests()
        {
            this.pathParser = new UriPathParser(uriParserSettings);
        }

        // TODO: Consider if we want to do anything special for the characters discussed here: http://blogs.msdn.com/b/peter_qian/archive/2010/05/25/using-wcf-data-service-with-restricted-characrters-as-keys.aspx

        [Fact]
        public void ParsePathSplitsManySimpleSegmentsCorrectly()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two/Three/Four/Five/Six/Seven/Eight/Nine/Ten/Eleven"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathHandlesParenthesisKeyAsPartOfSameSegment()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two(1)/Three"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Two(1)", "Three" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        // TODO: Astoria does this. Not quite sure what the spec says.
        [Fact]
        public void ParsePathIgnoresExtraSlashes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One////Three"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Three" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathShouldAllowParenthesisKeyWithStringValue()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "EntitySet('stringkey')"), this.baseUri);
            string[] expectedListOrder = new[] { "EntitySet('stringkey')" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathRespectsSlashAsSegmentMarkerOverSingleQuotes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "EntitySet('string/key')"), this.baseUri);
            string[] expectedListOrder = new[] { "EntitySet('string", "key')" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathKeepsUnescapedSpace()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "S p a c e"), this.baseUri);
            string[] expectedListOrder = new[] { "S p a c e" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathRemovesUnescapedNewline()
        {
             // If an un-escaped character like a newline, tab, carriage return or whatever is in a segment, it is lost.
             // This is unlikely in practice, since real URLs will have them escaped, but we should ensure
             // that we behavior in some reasonable manner.
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "Newline\n"), this.baseUri);
            string[] expectedListOrder = new[] { "Newline" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathKeepsEscapedSpace()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Space ")), this.baseUri);
            string[] expectedListOrder = new[] { "Space " };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathKeepsEscapedTab()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Tab\t")), this.baseUri);
            string[] expectedListOrder = new[] { "Tab\t" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathKeepsEscapedNewline()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Newline\n")), this.baseUri);
            string[] expectedListOrder = new[] { "Newline\n" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathKeepsEscapedCarriageReturn()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("CarriageReturn\r")), this.baseUri);
            string[] expectedListOrder = new[] { "CarriageReturn\r" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParseMultiPathIgnoresQueryString()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two?query=value"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Two" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathIgnoresQueryStringEvenWhenItHasSlashes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two?query=value/with/slashes"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Two" };

#if NETCOREAPP1_0
            list.SequenceEqual(expectedListOrder).Should().BeTrue();
#else
            list.Should().ContainExactly(expectedListOrder);
#endif
        }

        [Fact]
        public void ParsePathRequiresBaseUriToMatch()
        {
            var absoluteUri = new Uri("http://www.example.com/EntitySet/");

            Action enumerate = () => this.pathParser.ParsePathIntoSegments(absoluteUri, this.baseUri);
            enumerate.ShouldThrow<ODataException>().WithMessage(
                Strings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(absoluteUri, baseUri));
        }

        [Fact]
        public void ParsePathThrowsIfDepthLimitIsReached()
        {
            this.pathParser = new UriPathParser(new ODataUriParserSettings() { PathLimit = 2});
            Action enumerate = () => this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two/Three"), this.baseUri);
            enumerate.ShouldThrow<ODataException>(Strings.UriQueryPathParser_TooManySegments);
        }

        [Fact]
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

        [Fact]
        public void SegmentWithParensShouldStayAsOneSegment()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "EntitySet('KeyValue')"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => s.Should().Be("EntitySet('KeyValue')"),
            });
        }

        [Fact]
        public void SegmentWithMultiPartKeyStoredThemAll()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "EntitySet(first=1,second=2)"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => s.Should().Be("EntitySet(first=1,second=2)"),
            });
        }

        [Fact]
        public void ParsePathIgnoresQueryString()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "one?foo=bar/bar$cool"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => s.Should().Be("one"),
            });
        }

        [Fact]
        public void IsCharHexDigitTest()
        {
            UriParserHelper.IsCharHexDigit(' ').Should().BeFalse();
            UriParserHelper.IsCharHexDigit('0').Should().BeTrue();
            UriParserHelper.IsCharHexDigit('1').Should().BeTrue();
            UriParserHelper.IsCharHexDigit('9').Should().BeTrue();
            UriParserHelper.IsCharHexDigit(':').Should().BeFalse();
            UriParserHelper.IsCharHexDigit('A').Should().BeTrue();
            UriParserHelper.IsCharHexDigit('B').Should().BeTrue();
            UriParserHelper.IsCharHexDigit('F').Should().BeTrue();
            UriParserHelper.IsCharHexDigit('G').Should().BeFalse();
            UriParserHelper.IsCharHexDigit('a').Should().BeTrue();
            UriParserHelper.IsCharHexDigit('b').Should().BeTrue();
            UriParserHelper.IsCharHexDigit('f').Should().BeTrue();
            UriParserHelper.IsCharHexDigit('g').Should().BeFalse();
        }

        [Fact]
        public void TryRemoveQuotesTest()
        {
            string test = "' '";
            UriParserHelper.TryRemoveQuotes(ref test).Should().BeTrue();
            test.Should().Be(" ");

            test = "invalid";
            UriParserHelper.TryRemoveQuotes(ref test).Should().BeFalse();
            test.Should().Be("invalid");

            test = "'invalid";
            UriParserHelper.TryRemoveQuotes(ref test).Should().BeFalse();
            test.Should().Be("'invalid");
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
