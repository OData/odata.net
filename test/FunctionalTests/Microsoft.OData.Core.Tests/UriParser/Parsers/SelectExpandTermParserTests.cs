//---------------------------------------------------------------------
// <copyright file="SelectExpandTermParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests targeting SelectExpandTermParser.
    /// </summary>
    public class SelectExpandTermParserTests
    {
        #region ParseSingleSelectTerm
        [Fact]
        public void OneTermBecomesPropertyAccessTokenInSelect()
        {
            var result = ParseSelectTerm("foo");
            result.ShouldBeNonSystemToken("foo").And.NextToken.Should().BeNull();
        }

        [Fact]
        public void WhitespaceShouldBeTrimmedInSelect()
        {
            var result = ParseSelectTerm("  foo ");
            result.ShouldBeNonSystemToken("foo").And.NextToken.Should().BeNull();
        }

        [Fact]
        public void TermWithSlashCreatesParentNonRootSegmentTokenInSelect()
        {
            var result = ParseSelectTerm("navprop/foo");
            result.ShouldBeNonSystemToken("foo").And.NextToken.ShouldBeNonSystemToken("navprop");
        }

        [Fact]
        public void TermWithManySlashesCreatesManyNonRootSegmentTokensInSelect()
        {
            var result = ParseSelectTerm("one/two/three/four/five/foo");
            result.ShouldBeNonSystemToken("foo")
                  .And.NextToken.ShouldBeNonSystemToken("five")
                  .And.NextToken.ShouldBeNonSystemToken("four")
                  .And.NextToken.ShouldBeNonSystemToken("three")
                  .And.NextToken.ShouldBeNonSystemToken("two")
                  .And.NextToken.ShouldBeNonSystemToken("one");
        }

        [Fact]
        public void ATermWithJustSpaceAndSlashesShouldThrowInSelect()
        {
            Action parse = () => ParseSelectTerm(" /  // /");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected(1)); // TODO: better error message
        }

        [Fact]
        public void JustOneSlashShouldThrowInSelect()
        {
            Action parse = () => ParseSelectTerm("/");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected("0"));
        }

        [Fact]
        public void StarThenSlashShouldThrowInSelect()
        {
            Action parse = () => ParseSelectTerm("*/");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected("0"));
        }

        [Fact]
        public void StarResultsInStarTokenInSelect()
        {
            var result = ParseSelectTerm("*");
            result.ShouldBeNonSystemToken("*");
        }

        [Fact]
        public void SpaceAroundStarIsOkInSelect()
        {
            var result = ParseSelectTerm("   * ");
            result.ShouldBeNonSystemToken("*");
        }

        [Fact]
        public void StarCannotBeInMiddleOfPathInSelect()
        {
            Action parse = () => ParseSelectTerm("foo/*/bar");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected("4"));
        }

        [Fact]
        public void StarAfterNavPropIsOkInSelect()
        {
            var result = ParseSelectTerm("navprop/*");
            result.ShouldBeNonSystemToken("*")
                 .And.NextToken.ShouldBeNonSystemToken("navprop");
        }

        [Fact]
        public void WeirdFailureCasesInSelect()
        {
            var tests = new[]
                {
                    "'some",
                    "'",
                    "'some''",
                    "^",
                    "",
                    "binary'",
                    "binary'1234",
                    "4.a",
                    "4Ea",
                    "x'1",
                    "x'1z'",
                    "x'z1",
                };

            foreach (var term in tests)
            {
                Action parse = () => ParseSelectTerm(term);
                parse.ShouldThrow<ODataException>(); // TODO: Better error message
            }
        }

        [Fact]
        public void ContainerQualifiedWildcardIsAllowedInSelect()
        {
            var result = ParseSelectTerm("container.qualified.*");
            result.ShouldBeNonSystemToken("container.qualified.*");
        }

        [Fact]
        public void DoubleTrailingSlashShouldFailInSelect()
        {
            Action parse = () => ParseSelectTerm("navprop//");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected(8));
        }

        [Fact]
        public void TrailingSlashShouldBeIgnoredInSelect()
        {
            var result = ParseSelectTerm("path/to/a/navprop/");
            result.ShouldBeNonSystemToken("navprop");
        }

        #endregion

        #region ParseSingleExpandTerm
        [Fact]
        public void OneTermBecomesPropertyAccessTokenInExpand()
        {
            var result = this.ParseExpandTerm("foo");
            result.ShouldBeNonSystemToken("foo");
        }

        [Fact]
        public void WhitespaceShouldBeTrimmedInExpand()
        {
            var result = this.ParseExpandTerm("  foo ");
            result.ShouldBeNonSystemToken("foo");
        }

        [Fact]
        public void TermWithSlashCreatesParentNonRootSegmentTokenInExpand()
        {
            var result = this.ParseExpandTerm("navprop/foo");
            result.ShouldBeNonSystemToken("foo").And.NextToken.ShouldBeNonSystemToken("navprop");
        }

        [Fact]
        public void TermWithManySlashesCreatesManyNonRootSegmentTokensInExpand()
        {
            var result = this.ParseExpandTerm("one/two/three/four/five/foo");
            result.ShouldBeNonSystemToken("foo").And.NextToken.ShouldBeNonSystemToken("five")
                  .And.NextToken.ShouldBeNonSystemToken("four")
                  .And.NextToken.ShouldBeNonSystemToken("three")
                  .And.NextToken.ShouldBeNonSystemToken("two")
                  .And.NextToken.ShouldBeNonSystemToken("one");
        }

        [Fact]
        public void ATermWithJustSpaceAndSlashesShouldThrowInExpand()
        {
            Action parse = () => this.ParseExpandTerm(" /  // /");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected(1)); // TODO: better error message
        }

        [Fact]
        public void JustOneSlashShouldThrowInExpand()
        {
            Action parse = () => this.ParseExpandTerm("/");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected("0"));
        }

        [Fact]
        public void StarThenSlashShouldThrowInExpand()
        {
            Action parse = () => this.ParseExpandTerm("*/");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected("0"));
        }

        [Fact]
        public void StarResultsInStarTokenInExpand()
        {
            var result = this.ParseExpandTerm("*");
            result.ShouldBeNonSystemToken("*"); // TODO: Makes sense to throw here, right?
        }

        [Fact]
        public void SpaceAroundStarIsOkInExpand()
        {
            var result = this.ParseExpandTerm("   * ");
            result.ShouldBeNonSystemToken("*"); // TODO: Makes sense to throw here, right?
        }

        [Fact]
        public void StarCannotBeInMiddleOfPathInExpand()
        {
            Action parse = () => this.ParseExpandTerm("foo/*/bar");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected("4"));
        }

        [Fact]
        public void StarAfterNavPropIsOkInExpand()
        {
            var result = this.ParseExpandTerm("navprop/*");
            result.ShouldBeNonSystemToken("*").And.NextToken.ShouldBeNonSystemToken("navprop"); // TODO: Makes sense to throw here, right?
        } 

        [Fact]
        public void WeirdFailureCasesInExpand()
        {
            var tests = new[]
                {
                    "'some",
                    "'",
                    "'some''",
                    "^",
                    "",
                    "binary'",
                    "binary'1234",
                    "4.a",
                    "4Ea",
                    "x'1",
                    "x'1z'",
                    "x'z1",
                };

            foreach (var term in tests)
            {
                Action parse = () => this.ParseExpandTerm(term);
                parse.ShouldThrow<ODataException>(); // TODO: Better error message
            }
        }

        [Fact]
        public void ContainerQualifiedWildcardNotAllowedInExpand()
        {
            Action parseWithContainerQualfiedWildcard = () => this.ParseExpandTerm("container.qualified.*");
            parseWithContainerQualfiedWildcard.ShouldThrow<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError("10", "container.qualified.*"));
        }

        [Fact]
        public void TrailingSlashShouldBeIgnoredInExpand()
        {
            var result = this.ParseExpandTerm("path/to/a/navprop/");
            result.ShouldBeNonSystemToken("navprop").And.NextToken.Should().NotBeNull();
        }

        [Fact]
        public void DoubleTrailingSlashShouldFailInExpand()
        {
            Action parse = () => this.ParseExpandTerm("navprop//");
            parse.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected(8));
        }

        [Fact]
        public void TermParserStopsParsingAtOpenParenthesisForOptions()
        {
            ExpressionLexer lexer;
            var result = this.ParseExpandTerm("NavProp($filter=true)", out lexer);
            result.ShouldBeNonSystemToken("NavProp");
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.OpenParen);
        }

        // TODO: maybe need verification of where the lexer is left off?

        #endregion

        #region Helpers

        /// <summary>
        /// Runs a unit test on SelectExpandTermParser.ParseTerm() for a $select term.
        /// </summary>
        private static PathSegmentToken ParseSelectTerm(string term)
        {
            var lexer = new ExpressionLexer(term, true /*moveToFirstToken*/, true/*useSemicolonDelimiter*/);
            var parser = new SelectExpandTermParser(lexer, 100 /*maxPathLength*/, true /*isSelect*/);
            return parser.ParseTerm();
        }

        /// <summary>
        /// Runs a unit test on SelectExpandTermParser.ParseTerm() for a $expand term.
        /// </summary>
        private PathSegmentToken ParseExpandTerm(string term)
        {
            ExpressionLexer lexer;
            return ParseExpandTerm(term, out lexer);
        }

        /// <summary>
        /// Runs a unit test on SelectExpandTermParser.ParseTerm() for a $expand term, and gives the lexer back for positional verification.
        /// </summary>
        private PathSegmentToken ParseExpandTerm(string term, out ExpressionLexer lexer)
        {
            lexer = new ExpressionLexer(term, true /*moveToFirstToken*/, true/*useSemicolonDelimiter*/);
            var parser = new SelectExpandTermParser(lexer, 100 /*maxPathLength*/, false /*isSelect*/);
            return parser.ParseTerm();
        }

        #endregion
    }
}
