//---------------------------------------------------------------------
// <copyright file="UriPathParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Microsoft.OData.Core;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class UriPathParserTests
    {
        private readonly Uri baseUri = new Uri("http://www.example.com/Tests/OData.svc/");
        private UriPathParser pathParser;

        [TestInitialize]
        public void Init()
        {
            this.pathParser = new UriPathParser(1000);
        }

        // TODO: Consider if we want to do anything special for the characters discussed here: http://blogs.msdn.com/b/peter_qian/archive/2010/05/25/using-wcf-data-service-with-restricted-characrters-as-keys.aspx

        [TestMethod]
        public void ParsePathSplitsManySimpleSegmentsCorrectly()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two/Three/Four/Five/Six/Seven/Eight/Nine/Ten/Eleven"), this.baseUri);
            list.Should().ContainExactly(new[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven" });
        }

        [TestMethod]
        public void ParsePathHandlesParenthesisKeyAsPartOfSameSegment()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two(1)/Three"), this.baseUri);
            list.Should().ContainExactly(new[] { "One", "Two(1)", "Three" });
        }

        // TODO: Astoria does this. Not quite sure what the spec says.
        [TestMethod]
        public void ParsePathIgnoresExtraSlashes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One////Three"), this.baseUri);
            list.Should().ContainExactly(new[] { "One", "Three" });
        }

        [TestMethod]
        public void ParsePathShouldAllowParenthesisKeyWithStringValue()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "EntitySet('stringkey')"), this.baseUri);
            list.Should().ContainExactly(new[] { "EntitySet('stringkey')" });
        }

        [TestMethod]
        public void ParsePathRespectsSlashAsSegmentMarkerOverSingleQuotes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "EntitySet('string/key')"), this.baseUri);
            list.Should().ContainExactly(new[] { "EntitySet('string", "key')" });
        }

        [TestMethod]
        public void ParsePathKeepsUnescapedSpace()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "S p a c e"), this.baseUri);
            list.Should().ContainExactly(new[] { "S p a c e" });
        }

        /*
         * TODO: If an unescaped chracter like a newline, tab, carriage return or whatever is in a segment, it appears
         * to be lost. I think this is impossible in practice, since real URLs will have them escaped, but we should ensure
         * that we behavior in some reasonable manner. */
        [Ignore]
        [TestMethod]
        public void ParsePathKeepsUnescapedNewline()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "Newline\n"), this.baseUri);
            list.Should().ContainExactly(new[] { "Newline\n" });
        }

        [TestMethod]
        public void ParsePathKeepsEscapedSpace()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Space ")), this.baseUri);
            list.Should().ContainExactly(new[] { "Space " });
        }

        [TestMethod]
        public void ParsePathKeepsEscapedTab()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Tab\t")), this.baseUri);
            list.Should().ContainExactly(new[] { "Tab\t" });
        }

        [TestMethod]
        public void ParsePathKeepsEscapedNewline()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Newline\n")), this.baseUri);
            list.Should().ContainExactly(new[] { "Newline\n" });
        }

        [TestMethod]
        public void ParsePathKeepsEscapedCarriageReturn()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("CarriageReturn\r")), this.baseUri);
            list.Should().ContainExactly(new[] { "CarriageReturn\r" });
        }

        [TestMethod]
        public void ParsePathIgnoresQueryString()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two?query=value"), this.baseUri);
            list.Should().ContainExactly(new[] { "One", "Two" });
        }

        [TestMethod]
        public void ParsePathIgnoresQueryStringEvenWhenItHasSlashes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two?query=value/with/slashes"), this.baseUri);
            list.Should().ContainExactly(new[] { "One", "Two" });
        }

        [TestMethod]
        public void ParsePathRequiresBaseUriToMatch()
        {
            var absoluteUri = new Uri("http://www.example.com/EntitySet/");

            Action enumerate = () => this.pathParser.ParsePathIntoSegments(absoluteUri, this.baseUri);
            enumerate.ShouldThrow<ODataException>().WithMessage(
                Strings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(absoluteUri, baseUri));
        }

        [TestMethod]
        public void ParsePathThrowsIfDepthLimitIsReached()
        {
            this.pathParser = new UriPathParser(2);
            Action enumerate = () => this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two/Three"), this.baseUri);
            enumerate.ShouldThrow<ODataException>(Strings.UriQueryPathParser_TooManySegments);
        }
    }
}
