//---------------------------------------------------------------------
// <copyright file="UriPathParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Core;
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

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathHandlesParenthesisKeyAsPartOfSameSegment()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two(1)/Three"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Two(1)", "Three" };

            list.ContainExactly(expectedListOrder);
        }

        // TODO: Astoria does this. Not quite sure what the spec says.
        [Fact]
        public void ParsePathIgnoresExtraSlashes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One////Three"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Three" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathShouldAllowParenthesisKeyWithStringValue()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "EntitySet('stringkey')"), this.baseUri);
            string[] expectedListOrder = new[] { "EntitySet('stringkey')" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathDoesNotSplitOnSlashInsideSingleQuotes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "EntitySet('string/key')"), this.baseUri);
            string[] expectedListOrder = new[] { "EntitySet('string/key')" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathKeepsUnescapedSpace()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "S p a c e"), this.baseUri);
            string[] expectedListOrder = new[] { "S p a c e" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathRemovesUnescapedNewline()
        {
             // If an un-escaped character like a newline, tab, carriage return or whatever is in a segment, it is lost.
             // This is unlikely in practice, since real URLs will have them escaped, but we should ensure
             // that we behavior in some reasonable manner.
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "Newline\n"), this.baseUri);
            string[] expectedListOrder = new[] { "Newline" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathKeepsEscapedSpace()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Space ")), this.baseUri);
            string[] expectedListOrder = new[] { "Space " };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathKeepsEscapedTab()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Tab\t")), this.baseUri);
            string[] expectedListOrder = new[] { "Tab\t" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathKeepsEscapedNewline()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("Newline\n")), this.baseUri);
            string[] expectedListOrder = new[] { "Newline\n" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathKeepsEscapedCarriageReturn()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + Uri.EscapeDataString("CarriageReturn\r")), this.baseUri);
            string[] expectedListOrder = new[] { "CarriageReturn\r" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParseMultiPathIgnoresQueryString()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two?query=value"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Two" };

            list.ContainExactly(expectedListOrder);
        }

        [Fact]
        public void ParsePathIgnoresQueryStringEvenWhenItHasSlashes()
        {
            var list = this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two?query=value/with/slashes"), this.baseUri);
            string[] expectedListOrder = new[] { "One", "Two" };

            list.ContainExactly(expectedListOrder);
        }

        [Theory]
        [InlineData("root", new[] { "root" })]
        [InlineData("root:/", new[] { "root:"})]
        [InlineData("root:/:", new[] { "root:", ":" })]
        [InlineData("root:/abc", new[] { "root:", "abc" })]
        [InlineData("root:/abc:", new[] { "root:", "abc:" })]
        [InlineData("root:/::/", new[] { "root:", "::"})]
        [InlineData("root:/:/:/", new[] { "root:", ":", ":" })]
        [InlineData("root:/:///////////:/", new[] { "root:", ":", ":" })]
        [InlineData("root:/::/:", new[] { "root:", "::", ":" })]
        [InlineData("root:/abc:/property", new[] { "root:", "abc:", "property" })]
        [InlineData("root:/abc:/property:/", new[] { "root:", "abc:", "property:"})]
        [InlineData("root:/abc:/property:/:", new[] { "root:", "abc:", "property:", ":" })]
        [InlineData("root:/abc:/property:/::/", new[] { "root:", "abc:", "property:", "::" })]
        [InlineData("root:/:/property", new[] { "root:", ":", "property" })]
        [InlineData("root:/photos/2018/February", new[] { "root:", "photos", "2018", "February" })]
        [InlineData("root:/photos%2f2018%2fFebruary", new[] { "root:", "photos/2018/February" })]
        [InlineData("root:/photos%2f2018%2fFebruary/Others", new[] { "root:", "photos/2018/February", "Others" })]
        [InlineData("root:/photos%2f2018%2f/////February/Others", new[] { "root:", "photos/2018/", "February", "Others" })]
        [InlineData("root:/photos/2018%2fFebruary:/permissions", new[] { "root:", "photos", "2018/February:", "permissions" })]
        [InlineData("root:/photos:2018:/permissions:/abc", new[] { "root:", "photos:2018:", "permissions:", "abc" })]
        [InlineData("root:/photos::::::2018:/permissions:/abc", new[] { "root:", "photos::::::2018:", "permissions:", "abc" })]
        [InlineData("root:/photos/::::::2018:/permissions:/abc", new[] { "root:", "photos","::::::2018:", "permissions:", "abc" })]
        [InlineData("EntitySet('key'):/xyz:/perm", new[] { "EntitySet('key'):", "xyz:", "perm" })]
        [InlineData("EntitySet('key'):/xyz::/perm", new[] { "EntitySet('key'):", "xyz::", "perm" })]
        [InlineData("EntitySet('key'):/xyz/abc:", new[] { "EntitySet('key'):", "xyz", "abc:" })]
        [InlineData("EntitySet('key%2fvalue'):/xyz", new[] { "EntitySet('key/value'):", "xyz" })]
        [InlineData("EntitySet/key%2fvalue:/xyz", new[] { "EntitySet", "key/value:", "xyz" })]
        [InlineData("EntitySet/key:%2fvalue:/xyz", new[] { "EntitySet", "key:/value:", "xyz" })]
        [InlineData("EntitySet('key'):/xyz:/:/perm", new[] { "EntitySet('key'):", "xyz:", ":", "perm" })]
        [InlineData(":/xyz::/perm", new[] { ":", "xyz::", "perm" })]
        [InlineData(":/xyz:/:/perm", new[] { ":", "xyz:", ":", "perm" })]
        [InlineData(":/xyz://///:/perm", new[] { ":", "xyz:", ":", "perm" })]
        [InlineData("root::/abc", new[] { "root::", "abc" })]
        [InlineData("EntitySet('key')::/abc", new[] { "EntitySet('key')::", "abc" })]
        [InlineData("EntitySet/key:/abc", new[] { "EntitySet","key:", "abc" })]
        public void ParsePathShouldAllowEscapeFunctionPattern(string pattern, string[] expected)
        {
            // Arrange
            var fullUrl = new Uri(this.baseUri.AbsoluteUri + pattern);

            // Act
            var actual = this.pathParser.ParsePathIntoSegments(fullUrl, this.baseUri);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParsePathRequiresBaseUriToMatch()
        {
            var absoluteUri = new Uri("http://www.example.com/EntitySet/");

            Action enumerate = () => this.pathParser.ParsePathIntoSegments(absoluteUri, this.baseUri);
            enumerate.Throws<ODataException>(Error.Format(SRResources.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri, absoluteUri, baseUri));
        }

        [Fact]
        public void ParsePathThrowsIfDepthLimitIsReached()
        {
            this.pathParser = new UriPathParser(new ODataUriParserSettings() { PathLimit = 2});
            Action enumerate = () => this.pathParser.ParsePathIntoSegments(new Uri(this.baseUri.AbsoluteUri + "One/Two/Three"), this.baseUri);
            enumerate.Throws<ODataException>(SRResources.UriQueryPathParser_TooManySegments);
        }

        [Fact]
        public void ParsePathReturnsSegmentQueryTokensForEachSegment()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "one/two/three"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => Assert.Equal("one", s),
                s => Assert.Equal("two", s),
                s => Assert.Equal("three", s)
            });
        }

        [Fact]
        public void SegmentWithParensShouldStayAsOneSegment()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "EntitySet('KeyValue')"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => Assert.Equal("EntitySet('KeyValue')", s),
            });
        }

        [Fact]
        public void SegmentWithMultiPartKeyStoredThemAll()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "EntitySet(first=1,second=2)"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => Assert.Equal("EntitySet(first=1,second=2)", s),
            });
        }

        [Fact]
        public void ParsePathIgnoresQueryString()
        {
            var lastToken = pathParser.ParsePathIntoSegments(new Uri(baseUri, "one?foo=bar/bar$cool"), baseUri);

            VerifyPath(lastToken, new Action<string>[]
            {
                s => Assert.Equal("one", s),
            });
        }

        [Fact]
        public void IsCharHexDigitTest()
        {
            Assert.False(UriParserHelper.IsCharHexDigit(' '));
            Assert.True(UriParserHelper.IsCharHexDigit('0'));
            Assert.True(UriParserHelper.IsCharHexDigit('1'));
            Assert.True(UriParserHelper.IsCharHexDigit('9'));
            Assert.False(UriParserHelper.IsCharHexDigit(':'));
            Assert.True(UriParserHelper.IsCharHexDigit('A'));
            Assert.True(UriParserHelper.IsCharHexDigit('B'));
            Assert.True(UriParserHelper.IsCharHexDigit('F'));
            Assert.False(UriParserHelper.IsCharHexDigit('G'));
            Assert.True(UriParserHelper.IsCharHexDigit('a'));
            Assert.True(UriParserHelper.IsCharHexDigit('b'));
            Assert.True(UriParserHelper.IsCharHexDigit('f'));
            Assert.False(UriParserHelper.IsCharHexDigit('g'));
        }

        [Fact]
        public void TryRemoveQuotesTest()
        {
            string test = "' '";
            Assert.True(UriParserHelper.TryRemoveQuotes(ref test));
            Assert.Equal(" ", test);

            test = "invalid";
            Assert.False(UriParserHelper.TryRemoveQuotes(ref test));
            Assert.Equal("invalid", test);

            test = "'invalid";
            Assert.False(UriParserHelper.TryRemoveQuotes(ref test));
            Assert.Equal("'invalid", test);
        }

        [Theory]
        [InlineData("http://localhost/api/People('Foo/Bar')", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People(%27Foo/Bar%27)", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People%28'Foo/Bar'%29", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People%28%27Foo/Bar%27%29", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People(%27Foo/Bar%27%29", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People%28%27Foo/Bar%27)", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People(%27Foo/Bar')", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People('Foo/Bar%27)", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People('Foo%2FBar')", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People(%27Foo%2FBar%27)", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People%28%27Foo%2FBar%27%29", "People('Foo/Bar')")]
        [InlineData("http://localhost/api/People('Foo%2fBar')", "People('Foo/Bar')")]        // lowercase 'f'
        [InlineData("http://localhost/api/People(%27Foo%2fBar%27)", "People('Foo/Bar')")]    // lowercase 'f'
        [InlineData("http://localhost/api/People%28%27O'%27Neil/Jack%27%29", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People%28%27O%27'Neil/Jack%27%29", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People(%27O%27'Neil/Jack%27%29", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People%28%27O%27'Neil/Jack%27)", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People('O%27'Neil/Jack%27)", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People(%27O%27'Neil/Jack')", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People(%27O''Neil/Jack')", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People%28'O%27'Neil/Jack%27%29", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People%28'O%27'Neil/Jack'%29", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People%28'O''Neil/Jack'%29", "People('O''Neil/Jack')")]
        [InlineData("http://localhost/api/People%28%27Jack/O'%27Neil%27%29", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People%28%27Jack/O%27'Neil%27%29", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People(%27Jack/O%27'Neil%27%29", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People%28%27Jack/O%27'Neil%27)", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People('Jack/O%27'Neil%27)", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People(%27Jack/O%27'Neil')", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People(%27Jack/O''Neil')", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People%28'Jack/O%27'Neil%27%29", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People%28'Jack/O%27'Neil'%29", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People%28'Jack/O''Neil'%29", "People('Jack/O''Neil')")]
        [InlineData("http://localhost/api/People%28%20%27O'%27Neil/Jack%27%20%29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28%20%27O%27'Neil/Jack%27%20%29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People(%20%27O%27'Neil/Jack%27%20%29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28%20%27O%27'Neil/Jack%27%20)", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People(%20'O%27'Neil/Jack%27%20)", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People(%20%27O%27'Neil/Jack'%20)", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People(%20%27O''Neil/Jack'%20)", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28%20'O%27'Neil/Jack%27%20%29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28%20'O%27'Neil/Jack'%20%29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28%20'O''Neil/Jack'%20%29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28 %27O'%27Neil/Jack%27 %29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28 %27O%27'Neil/Jack%27 %29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People( %27O%27'Neil/Jack%27 %29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28 %27O%27'Neil/Jack%27 )", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People( 'O%27'Neil/Jack%27 )", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People( %27O%27'Neil/Jack' )", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People( %27O''Neil/Jack' )", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28 'O%27'Neil/Jack%27 %29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28 'O%27'Neil/Jack' %29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People%28 'O''Neil/Jack' %29", "People( 'O''Neil/Jack' )")]
        [InlineData("http://localhost/api/People(%27O'%27Neil%20(Jack)%27)", "People('O''Neil (Jack)')")]
        [InlineData("http://localhost/api/People%28%27O'%27Neil%20(Jack)%27%29", "People('O''Neil (Jack)')")]
        [InlineData("http://localhost/api/People(%27O'%27Neil%20%28Jack%29%27)", "People('O''Neil (Jack)')")]
        [InlineData("http://localhost/api/People%28%27O'%27Neil%20%28Jack%29%27%29", "People('O''Neil (Jack)')")]
        [InlineData("http://localhost/api/People(%27O'%27Neil%20(Jack%29%27)", "People('O''Neil (Jack)')")]
        [InlineData("http://localhost/api/People%28%27O'%27Neil%20%28Jack)%27%29", "People('O''Neil (Jack)')")]
        [InlineData("http://localhost/api/People('O%20Neil/Jack')", "People('O Neil/Jack')")] // space inside literal (encoded)
        [InlineData("http://localhost/api/People(%27O%20Neil/Jack%27)", "People('O Neil/Jack')")]
        [InlineData("http://localhost/api/People(%20%20%27Foo/Bar%27%20%20)", "People(  'Foo/Bar'  )")] // multiple spaces around literal
        [InlineData("http://localhost/api/People('Foo%2CBar')", "People('Foo,Bar')")] // comma
        [InlineData("http://localhost/api/People(%27caf%C3%A9/Bar%27)", "People('café/Bar')")] // UTF-8 é
        [InlineData("http://localhost/api/People('')", "People('')")] // empty literal
        [InlineData("http://localhost/api/People(%27%27)", "People('')")] // empty literal encoded
        [InlineData("http://localhost/api/People('''/Bar')", "People('''/Bar')")] // literal contains an escaped quote then slash
        [InlineData("http://localhost/api/People(%27%27%27/Bar%27)", "People('''/Bar')")] // encoded then raw
        [InlineData("http://localhost/api/People(%27%27%27%2FBar%27)", "People('''/Bar')")] // encoded slash
        [InlineData("http://localhost/api/People('Foo/Bar')/", "People('Foo/Bar')")] // trailing slash
        [InlineData("http://localhost/api//People('Foo/Bar')", "People('Foo/Bar')")] // trailing slash
        [InlineData("http://localhost/api/People(logonName='Foo/Bar')", "People(logonName='Foo/Bar')")]// key=value containing slash
        [InlineData("http://localhost/api/People(logonName='Foo/Bar',city='Juarez/Border')", "People(logonName='Foo/Bar',city='Juarez/Border')")] // key=value containing slash, multiple keys
        [InlineData("http://localhost/api/People('Foo+Bar')", "People('Foo+Bar')")] // + in quote stays literal
        [InlineData("http://localhost/api/People('Foo%0ABar')", "People('Foo\nBar')")] // Encoded newline in quote becomes newline
        [InlineData("http://localhost/api/People('Foo%0DBar')", "People('Foo\rBar')")] // Encoded carriage return in quote becomes carriage return
        [InlineData("http://localhost/api/People('Foo%0D%0ABar')", "People('Foo\r\nBar')")] // Encoded CRLF in quote becomes CRLF
        [InlineData("http://localhost/api/People('Foo''Bar''Baz')", "People('Foo''Bar''Baz')")] // Repeated escaped quotes
        [InlineData("http://localhost/api/People(%27Foo%27%27Bar%27%27Baz%27)", "People('Foo''Bar''Baz')")] // Repeated encoded quotes
        [InlineData("http://localhost/api/People('Foo😊')", "People('Foo😊')")] // Surrogate pair (emoji)
        [InlineData("http://localhost/api/People('é')", "People('é')")] // Combination of base character + accent
        [InlineData("http://localhost/api/People('مرحبا')", "People('مرحبا')")] // CJK literals
        [InlineData("http://localhost/api/People('日本')", "People('日本')")]
        [InlineData("http://localhost/api/People('Foo%252FBar')", "People('Foo%2FBar')")] // Double encoded
        [InlineData("http://localhost/api/People%28%27%61%62%63%64%65%66%67%68%69%6A%6B%6C%6D%2F%6E%6F%70%71%72%73%74%75%66%77%78%79%7A%27%29", "People('abcdefghijklm/nopqrstufwxyz')")]
        [InlineData("http://localhost/api/People%28%27%61%62%63%64%65%66%67%68%69%6a%6b%6c%6d%2f%6e%6f%70%71%72%73%74%75%66%77%78%79%7a%27%29", "People('abcdefghijklm/nopqrstufwxyz')")]
        [InlineData("http://localhost/api/People%28%27%61%62%63%64%65%66%67%68%69%6A%6B%6C%6D%2f%6e%6f%70%71%72%73%74%75%66%77%78%79%7a%27%29", "People('abcdefghijklm/nopqrstufwxyz')")]
        [InlineData("http://localhost/api/People('abcdefghijklm/%6e%6f%70%71%72%73%74%75%66%77%78%79%7a%27%29", "People('abcdefghijklm/nopqrstufwxyz')")]
        [InlineData("http://localhost/api/People%28%27%41%42%43%44%45%46%47%48%49%4A%4B%4C%4D%2F%4E%4F%50%51%52%53%54%55%56%57%58%59%5A%27%29", "People('ABCDEFGHIJKLM/NOPQRSTUVWXYZ')")]
        [InlineData("http://localhost/api/People%28%27%41%42%43%44%45%46%47%48%49%4a%4b%4c%4d%2f%4e%4f%50%51%52%53%54%55%56%57%58%59%5a%27%29", "People('ABCDEFGHIJKLM/NOPQRSTUVWXYZ')")]
        [InlineData("http://localhost/api/People%28%27%41%42%43%44%45%46%47%48%49%4a%4b%4c%4d%2F%4E%4F%50%51%52%53%54%55%56%57%58%59%5A%27%29", "People('ABCDEFGHIJKLM/NOPQRSTUVWXYZ')")]
        [InlineData("http://localhost/api/People%28%27%41%42%43%44%45%46%47%48%49%4a%4b%4c%4d%2FNOPQRSTUVWXYZ')", "People('ABCDEFGHIJKLM/NOPQRSTUVWXYZ')")]
        public void ParsePathParenthesizedSegmentNormalization(string uriString, string expectedSegment)
        {
            // Arrange
            var baseUri = new Uri("http://localhost/api/");
            var uri = new Uri(uriString);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);

            // Assert
            var segment = Assert.Single(segments);
            Assert.Equal(expectedSegment, segment);
        }

        [Fact]
        public void ParsePathIntoSegments()
        {
            var segments = this.pathParser.ParsePathIntoSegments(new Uri("./People('Foo/Bar')", UriKind.RelativeOrAbsolute), new Uri("./", UriKind.RelativeOrAbsolute));
        }

        [Theory]
        // Typical cases
        [InlineData("http://localhost/api/", "http://localhost/api/People('O''Neil')", "People('O''Neil')")]
        [InlineData("http://localhost/api/", "http://localhost/api/People(%27O%27%27Neil%27)", "People('O''Neil')")]
        [InlineData("http://localhost/api/", "http://localhost/api/People%28%27O%27%27Neil%27%29", "People('O''Neil')")]
        [InlineData("http://localhost/api/", "http://localhost/api/Categories('Smartphone%2FTablet')", "Categories('Smartphone/Tablet')")]
        // Per the OData V4 spec, forward slashes that are not path separators must be percent-encoded.
        // In practice, some clients (e.g., Excel or other external integrations) emit unencoded slashes inside quoted literals so we try to accommodate unencoded slash in unquoted literals.
        [InlineData("http://localhost/api/", "http://localhost/api/Categories('Smartphone/Tablet')", "Categories('Smartphone/Tablet')")]
        [InlineData("http://localhost/api/", "api/People('O''Neil')", "People('O''Neil')")]
        [InlineData("http://localhost/api/", "api/People(%27O%27%27Neil%27)", "People('O''Neil')")]
        [InlineData("http://localhost/api/", "api/People%28%27O%27%27Neil%27%29", "People('O''Neil')")]
        [InlineData("http://localhost/api/", "api/Categories('Smartphone%2FTablet')", "Categories('Smartphone/Tablet')")]
        [InlineData("http://localhost/api/", "api/Categories('Smartphone/Tablet')", "Categories('Smartphone/Tablet')")]
        [InlineData("api/", "http://localhost/api/People('O''Neil')", "People('O''Neil')")]
        [InlineData("api/", "http://localhost/api/People(%27O%27%27Neil%27)", "People('O''Neil')")]
        [InlineData("api/", "http://localhost/api/People%28%27O%27%27Neil%27%29", "People('O''Neil')")]
        [InlineData("api/", "http://localhost/api/Categories('Smartphone%2FTablet')", "Categories('Smartphone/Tablet')")]
        [InlineData("api/", "http://localhost/api/Categories('Smartphone/Tablet')", "Categories('Smartphone/Tablet')")]
        [InlineData("api/", "api/People('O''Neil')", "People('O''Neil')")]
        [InlineData("api/", "api/People(%27O%27%27Neil%27)", "People('O''Neil')")]
        [InlineData("api/", "api/People%28%27O%27%27Neil%27%29", "People('O''Neil')")]
        [InlineData("api/", "api/Categories('Smartphone%2FTablet')", "Categories('Smartphone/Tablet')")]
        [InlineData("api/", "api/Categories('Smartphone/Tablet')", "Categories('Smartphone/Tablet')")]
        // Case-insensitive hex for %2f (ensure lower-case hex works)
        [InlineData("http://localhost/api/", "http://localhost/api/Categories('Smartphone%2fTablet')", "Categories('Smartphone/Tablet')")]
        // Mixed encoded/unencoded quotes inside the same literal
        [InlineData("http://localhost/api/", "http://localhost/api/People('O%27%27Neil')", "People('O''Neil')")]
        [InlineData("http://localhost/api/", "http://localhost/api/People(%27O''Neil%27)", "People('O''Neil')")]
        // Encoded spaces and plus inside quoted literal
        [InlineData("http://localhost/api/", "http://localhost/api/Tags('C%23%20and%20C%2b%2b')", "Tags('C# and C++')")]
        // Non-ASCII percent-encoded (UTF‑8)
        [InlineData("http://localhost/api/", "http://localhost/api/Names('caf%C3%A9')", "Names('café')")]
        // Encoded parentheses in the *name* part (not only the key)
        [InlineData("http://localhost/api/", "http://localhost/api/Cate%67ories%28%27X%27%29", "Categories('X')")] // %67 = 'g'
        // Encoded percent sign itself
        [InlineData("http://localhost/api/", "http://localhost/api/Docs('%25Complete')", "Docs('%Complete')")]
        // Base has different case (case-insensitive base-of)
        [InlineData("http://localhost/API/", "http://localhost/api/People('O''Neil')", "People('O''Neil')")]
        // Base without trailing slash vs with it in full
        [InlineData("http://localhost/api", "http://localhost/api/Products(1)", "Products(1)")]
        // Encoded slash at start/end inside quotes
        [InlineData("http://localhost/api/", "http://localhost/api/Names('%2Falpha')", "Names('/alpha')")]
        [InlineData("http://localhost/api/", "http://localhost/api/Names('omega%2F')", "Names('omega/')")]
        [InlineData("http://localhost/api/", "http://localhost/api/$metadata", "$metadata")]
        [InlineData("http://localhost/api/", "http://localhost/api/%24metadata", "$metadata")]
        // Customer reported cases
        [InlineData("api/", "api/entity('/subscriptions/00000000-0000-0000-0000-000000000000')", "entity('/subscriptions/00000000-0000-0000-0000-000000000000')")]
        [InlineData("https://myservice/odata/", "https://myservice/odata/MyEntity('key/with/slashes')", "MyEntity('key/with/slashes')")]
        [InlineData("https://sample.com/", "https://sample.com/resources('http%3A%2F%2Fsample.sample.net%2Fsample%2Fservices%2FFoo')", "resources('http://sample.sample.net/sample/services/Foo')")]
        [InlineData("http://localhost:5913/efcore", "http://localhost:5913/efcore/Movies('a%30b')", "Movies('a0b')")]
        [InlineData("http://localhost:5000/odata", "http://localhost:5000/odata/Customers(%27a%30b%27)", "Customers('a0b')")]
        [InlineData("odata/", "/odata/issue1964('\"%2F\"')", "issue1964('\"/\"')")]
        [InlineData("odata/", "/odata/entity('abc/123')", "entity('abc/123')")]
        [InlineData("/", "/new_alesers(new_name='alex%2F3')", "new_alesers(new_name='alex/3')")]
        [InlineData("/", "/alssc_anglesectors(alssc_name='Water Auth/Company')", "alssc_anglesectors(alssc_name='Water Auth/Company')")]
        [InlineData("/", "/alssc_anglesectors(alssc_name='Water Auth%2FCompany')", "alssc_anglesectors(alssc_name='Water Auth/Company')")]
        public void ParsePathWithEncodedSequencesAndSlash(string baseUriString, string uriString, string expectedSegment)
        {
            // Arrange
            var baseUri = baseUriString is null ? null : new Uri(baseUriString, UriKind.RelativeOrAbsolute);
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            // Act
            var parseSegments = this.pathParser.ParsePathIntoSegments(uri, baseUri);

            // Assert
            var segment = Assert.Single(parseSegments);
            Assert.Equal(expectedSegment, segment);
        }

        [Theory]
        [InlineData("http://localhost/api/", "http://localhost/api/Products(1)/Supplier('O''Neil')", new[] { "Products(1)", "Supplier('O''Neil')" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Products(1)/Supplier(%27O%27%27Neil%27)", new[] { "Products(1)", "Supplier('O''Neil')" })]
        [InlineData("http://localhost/api/", "http://localhost/api/People('Foo/Bar')/Friends(3)", new[] { "People('Foo/Bar')", "Friends(3)" })] // unencoded slash inside quotes
        [InlineData("http://localhost/api/", "http://localhost/api/People('Foo%2FBar')/Friends(3)", new[] { "People('Foo/Bar')", "Friends(3)" })] // encoded slash inside quotes
        [InlineData("http://localhost/api/", "http://localhost/api/Team%28%27A%28B%29%27%29/Members", new[] { "Team('A(B)')", "Members" })]
        [InlineData("http://localhost/api/", "http://localhost/api/People(%27Foo/Bar%27)/Friends", new[] { "People('Foo/Bar')", "Friends" })]
        [InlineData("http://localhost/api/", "http://localhost/api/People%28%27Foo/Bar%27%29/Friends", new[] { "People('Foo/Bar')", "Friends" })]
        [InlineData("http://localhost/api/", "http://localhost/api/People('O%27'Neil/Jack')/Next", new[] { "People('O''Neil/Jack')", "Next" })]
        [InlineData("http://localhost/api/", "http://localhost/api/People('Foo/Bar)/Friends", new[] { "People('Foo", "Bar)", "Friends" })]
        [InlineData("http://localhost/api/", "http://localhost/api/People('Foo/Bar)", new[] { "People('Foo", "Bar)" })]
        [InlineData("http://localhost/api/", "http://localhost/api/People(%22Foo/Bar%22)", new[] { "People(\"Foo", "Bar\")" })]
        [InlineData("http://localhost/api/", "http://localhost/api/People(Foo=1/Bar=2)", new[] { "People(Foo=1", "Bar=2)" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Foo'/Bar", new[] { "Foo'", "Bar" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Foo%27/Bar", new[] { "Foo'", "Bar" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Customers('abc%2F/''efg')/ abc /'%2F", new[] { "Customers('abc//''efg')", " abc ", "'/" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Customers(%27abc%2F/'%27efg')/ abc' /'%2F", new[] { "Customers('abc//''efg')", " abc' ", "'/" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Customers(%27abc%2F/'%27efg')/ 'abc /'%2F", new[] { "Customers('abc//''efg')", " 'abc ", "'/" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Customers(%27abc%2F/'%27efg')/ 'abc/ijk /'%2F", new[] { "Customers('abc//''efg')", " 'abc", "ijk ", "'/" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Customers(%27abc%2F/'%27efg')/ 'abc/ijk /'pqr/xyz'", new[] { "Customers('abc//''efg')", " 'abc", "ijk ", "'pqr", "xyz'" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Products(1)/$metadata", new[] { "Products(1)", "$metadata" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Products(1)/%24metadata", new[] { "Products(1)", "$metadata" })]
        // Customer reported cases
        [InlineData("security/", "security/zones('b288f9e672c04efeb31ec39276ec4928')/environments('/subscriptions/bf92c6ed78d24690919bfb93e84682bf')", new[] { "zones('b288f9e672c04efeb31ec39276ec4928')", "environments('/subscriptions/bf92c6ed78d24690919bfb93e84682bf')" })]
        [InlineData("odata/", "/odata/entity/search('abc/123')", new[] { "entity", "search('abc/123')" })]
        [InlineData("/", "/ApplicationSegments('1234')/corsConfigurations('/Test/app')", new[] { "ApplicationSegments('1234')", "corsConfigurations('/Test/app')" })]
        [InlineData("/", "/ApplicationSegments('1234')/corsConfigurations('%2FTest/app')", new[] { "ApplicationSegments('1234')", "corsConfigurations('/Test/app')" })]
        public void ParsePathMultipleSegments(string baseUriString, string uriString, string[] expectedSegments)
        {
            // Arrange
            var baseUri = new Uri(baseUriString, UriKind.RelativeOrAbsolute);
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);

            // Assert
            Assert.Equal(expectedSegments, segments);
        }

        [Theory]
        [InlineData("http://localhost/api/", "http://localhost/api/")]
        [InlineData("http://localhost/api", "http://localhost/api")]
        public void ParsePathNoSegmentsReturnsEmpty(string baseUriString, string uriString)
        {
            // Arrange
            var baseUri = new Uri(baseUriString, UriKind.RelativeOrAbsolute);
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);

            // Assert
            Assert.Empty(segments);
        }

        [Theory]
        [InlineData("http://localhost/api/", "http://localhost/api//Products//(1)//", new[] { "Products", "(1)" })]
        [InlineData("http://localhost/api/", "http://localhost/api///", new string[0])]
        public void ParsePathDuplicateOrTrailingSlashesOmitsEmptySegments(string baseUriString, string uriString, string[] expected)
        {
            // Arrange
            var baseUri = new Uri(baseUriString, UriKind.RelativeOrAbsolute);
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);

            // Assert
            Assert.Equal(expected, segments);
        }

        [Theory]
        [InlineData("http://localhost/api/", "http://localhost/api/Products(1)?$select=Name", new[] { "Products(1)" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Products(1)#frag", new[] { "Products(1)" })]
        [InlineData("http://localhost/api/", "http://localhost/api/Products(1)?x=y#z", new[] { "Products(1)" })]
        public void ParsePathIgnoresQueryAndFragment(string baseUriString, string uriString, string[] expected)
        {
            // Arrange
            var baseUri = new Uri(baseUriString, UriKind.RelativeOrAbsolute);
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);

            // Assert
            Assert.Equal(expected, segments);
        }

        [Theory]
        [InlineData("api/", "api/People()", "People()")] // both relative, empty parens
        [InlineData("api/", "api/People('X')", "People('X')")] // both relative
        [InlineData("api", "api/People('X')", "People('X')")] // both relative, base no trailing slash
        [InlineData("api/", "http://host/api/People('X')", "People('X')")] // base rel, full abs
        [InlineData("http://host/api/", "api/People('X')", "People('X')")] // base abs, full rel
        [InlineData("/", "People('X')", "People('X')")] // base root + relative full -> mock base
        [InlineData("/", "./People('X')", "People('X')")]
        [InlineData("/", "../People('X')", "People('X')")]
        [InlineData("./", "People('X')", "People('X')")]
        [InlineData("./", "./People('X')", "People('X')")]
        [InlineData("./", "../People('X')", "People('X')")]
        [InlineData("../", "People('X')", "People('X')")]
        [InlineData("../", "./People('X')", "People('X')")]
        [InlineData("../", "../People('X')", "People('X')")]
        [InlineData(null, "People('X')", "People('X')")] // null base + relative full -> mock base
        public void ParsePathMockingCombinations(string baseUriString, string uriString, string expectedSegment)
        {
            // Arrange
            var baseUri = baseUriString is null ? null : new Uri(baseUriString, UriKind.RelativeOrAbsolute);
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);
            var segment = Assert.Single(segments);

            // Assert
            Assert.Equal(expectedSegment, segment);
        }

        [Theory]
        [InlineData("http://localhost/api/", "http://localhost/api/Files('name;v%3D1.1')", "Files('name;v=1.1')")]
        [InlineData("http://localhost/api/", "http://localhost/api/Files('a;b;c')", "Files('a;b;c')")]
        public void ParsePathSemicolonParamsInsideLiteral(string baseUriString, string uriString, string expected)
        {
            // Arrange
            var baseUri = new Uri(baseUriString, UriKind.RelativeOrAbsolute);
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);
            var segment = Assert.Single(segments);

            // Assert
            Assert.Equal(expected, segment);
        }

        [Theory]
        [InlineData("http://localhost/api/", "http://localhost/api/%50roducts(1)", "Products(1)")] // %50 = 'P' at segment start
        [InlineData("http://localhost/api/", "http://localhost/api/Products(1)%2F", "Products(1)/")] // encoded slash at end of segment (retained within same segment)
        [InlineData("http://localhost/api/", "http://localhost/api/%27A%27", "'A'")] // entire segment is an encoded quoted literal
        public void ParsePathEdgeEncodings(string baseUriString, string uriString, string expectedSegment)
        {
            // Arrange
            var baseUri = new Uri(baseUriString, UriKind.RelativeOrAbsolute);
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);
            var segment = Assert.Single(segments);

            // Assert
            Assert.Equal(expectedSegment, segment);
        }

        [Fact]
        public void ParsePathBaseCaseInsensitivity()
        {
            // Arrange
            var baseUri = new Uri("http://localhost/API/", UriKind.Absolute);
            var uri = new Uri("http://localhost/api/Products", UriKind.Absolute);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);
            var segment = Assert.Single(segments);

            // Assert
            Assert.Equal("Products", segment);
        }

        [Theory]
        [InlineData("http://localhost:80/api/", "https://localhost:443/api/People('X')", "People('X')")] // scheme and default port difference ignored
        [InlineData("http://localhost:80/api/", "http://localhost/api/People('X')", "People('X')")] // default port omitted
        [InlineData("https://LOCALHOST/API/", "http://localhost/api/People('X')", "People('X')")] // scheme and case difference ignored
        public void ParsePathSchemePortHostVariance(string baseUriString, string uriString, string expected)
        {
            // Arrange
            var baseUri = new Uri(baseUriString, UriKind.Absolute);
            var uri = new Uri(uriString, UriKind.Absolute);

            // Act
            var segment = Assert.Single(this.pathParser.ParsePathIntoSegments(uri, baseUri));

            // Assert
            Assert.Equal(expected, segment);
        }

        [Fact]
        public void ParsePathLongSegmentWithManyEscapes()
        {
            // Arrange
            var baseUri = new Uri("http://localhost/api/", UriKind.Absolute);
            var repeated = string.Concat(Enumerable.Repeat("%2F%27%32%30", 50)); // "/'20" repeated (odd but valid)
            var uri = new Uri($"http://localhost/api/Doc('{repeated}')", UriKind.Absolute);

            // Act
            var segment = Assert.Single(this.pathParser.ParsePathIntoSegments(uri, baseUri));

            // Assert
            Assert.StartsWith("Doc('", segment);
            Assert.EndsWith("')", segment);
            Assert.Contains("/'20", segment); // verifies decode
        }

        [Theory]
        [InlineData("http://localhost/api/?Foo=Bar", "http://localhost/api/People('Foo/Bar')")]
        [InlineData("http://localhost/api/#Foo", "http://localhost/api/People('Foo/Bar')")]
        public void ParsePathBaseUriWithQueryOrFragment(string baseUriString, string uriString)
        {
            // Arrange
            var baseUri = new Uri(baseUriString);
            var uri = new Uri(uriString);

            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri);

            // Assert
            var segment = Assert.Single(segments);
            Assert.Equal("People('Foo/Bar')", segment);
        }

        [Theory]
        [InlineData("http://localhost/api/People('Foo%2')", "People('Foo%2')")]
        [InlineData("http://localhost/api/People('Foo%GG')", "People('Foo%GG')")]
        [InlineData("http://localhost/api/People('Foo%C3')", "People('Foo%C3')")]
        [InlineData("http://localhost/api/People('Foo%C3%28')", "People('Foo%C3(')")]
        public void ParsePathPercentEncodingNotValidOrUtf8SequenceMalformed(string uriString, string expected)
        {
            // Arrange
            var baseUri = new Uri("http://localhost/api/");
            var uri = new Uri(uriString);
            
            // Act
            var segments = this.pathParser.ParsePathIntoSegments(uri, baseUri); // Uri.UnescapeDataString is lenient - does not throw

            // Assert
            var segment = Assert.Single(segments);
            Assert.Equal(expected, segment);
        }

        [Fact]
        public void ParsePathSegmentsAtPathLimit()
        {
            // Arrange
            var localPathParser = new UriPathParser(new ODataUriParserSettings { PathLimit = 2 });
            var baseUri = new Uri("http://localhost/api/");
            var uri = new Uri("http://localhost/api/People('Foo/Bar')/Orders");

            // Act
            var segments = localPathParser.ParsePathIntoSegments(uri, baseUri);

            // Assert
            this.VerifyPath(segments,
            [
                s => Assert.Equal("People('Foo/Bar')", s),
                s => Assert.Equal("Orders", s),
            ]);
        }

        [Fact]
        public void ParsePathThrowsExceptionForMaxPathLimitExceeded()
        {
            // Arrange
            var localPathParser = new UriPathParser(new ODataUriParserSettings { PathLimit = 2 });
            var baseUri = new Uri("http://localhost/api/");
            var uri = new Uri("http://localhost/api/Customers(5)/Orders(3)/Items");

            // Act & Assert
            var exception = Assert.Throws<ODataException>(() => localPathParser.ParsePathIntoSegments(uri, baseUri));
            Assert.Equal("Too many segments in URI.", exception.Message);
        }

        /// <summary>
        /// Enumerates the segments in a path and calls a corresponding delegate verifier on each segment.
        /// </summary>
        private void VerifyPath(IEnumerable<string> path, Action<string>[] segmentVerifiers)
        {
            Assert.Equal(path.Count(), segmentVerifiers.Count());

            var i = 0;
            foreach (var segment in path)
            {
                segmentVerifiers[i++](segment);
            }
        }
    }
}
