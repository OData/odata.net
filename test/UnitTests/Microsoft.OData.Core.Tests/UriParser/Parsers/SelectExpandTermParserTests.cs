//---------------------------------------------------------------------
// <copyright file="SelectExpandTermParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
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
            Assert.Null(result.ShouldBeNonSystemToken("foo").NextToken);
        }

        [Fact]
        public void WhitespaceShouldBeTrimmedInSelect()
        {
            var result = ParseSelectTerm("  foo ");
            Assert.Null(result.ShouldBeNonSystemToken("foo").NextToken);
        }

        [Fact]
        public void TermWithSlashCreatesParentNonRootSegmentTokenInSelect()
        {
            var result = ParseSelectTerm("navprop/foo");
            result.ShouldBeNonSystemToken("foo").NextToken.ShouldBeNonSystemToken("navprop");
        }

        [Fact]
        public void TermWithManySlashesCreatesManyNonRootSegmentTokensInSelect()
        {
            var result = ParseSelectTerm("one/two/three/four/five/foo");
            result.ShouldBeNonSystemToken("foo")
                  .NextToken.ShouldBeNonSystemToken("five")
                  .NextToken.ShouldBeNonSystemToken("four")
                  .NextToken.ShouldBeNonSystemToken("three")
                  .NextToken.ShouldBeNonSystemToken("two")
                  .NextToken.ShouldBeNonSystemToken("one");
        }

        [Fact]
        public void ATermWithJustSpaceAndSlashesShouldThrowInSelect()
        {
            Action parse = () => ParseSelectTerm(" /  // /");
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, 1)); // TODO: better error message
        }

        [Fact]
        public void JustOneSlashShouldThrowInSelect()
        {
            Action parse = () => ParseSelectTerm("/");
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, "0"));
        }

        [Fact]
        public void StarThenSlashShouldThrowInSelect()
        {
            Action parse = () => ParseSelectTerm("*/");
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, "0"));
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
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, "4"));
        }

        [Fact]
        public void StarAfterNavPropIsOkInSelect()
        {
            var result = ParseSelectTerm("navprop/*");
            result.ShouldBeNonSystemToken("*")
                 .NextToken.ShouldBeNonSystemToken("navprop");
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
                Assert.Throws<ODataException>(parse); // TODO: Better error message
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
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, 8));
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
            result.ShouldBeNonSystemToken("foo").NextToken.ShouldBeNonSystemToken("navprop");
        }

        [Fact]
        public void TermWithManySlashesCreatesManyNonRootSegmentTokensInExpand()
        {
            var result = this.ParseExpandTerm("one/two/three/four/five/foo");
            result.ShouldBeNonSystemToken("foo").NextToken.ShouldBeNonSystemToken("five")
                  .NextToken.ShouldBeNonSystemToken("four")
                  .NextToken.ShouldBeNonSystemToken("three")
                  .NextToken.ShouldBeNonSystemToken("two")
                  .NextToken.ShouldBeNonSystemToken("one");
        }

        [Fact]
        public void ATermWithJustSpaceAndSlashesShouldThrowInExpand()
        {
            Action parse = () => this.ParseExpandTerm(" /  // /");
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, 1)); // TODO: better error message
        }

        [Fact]
        public void JustOneSlashShouldThrowInExpand()
        {
            Action parse = () => this.ParseExpandTerm("/");
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, "0"));
        }

        [Fact]
        public void StarThenSlashShouldThrowInExpand()
        {
            Action parse = () => this.ParseExpandTerm("*/");
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, "2"));
        }

        [Fact]
        public void StarResultsInStarTokenInExpand()
        {
            var result = this.ParseExpandTerm("*");
            result.ShouldBeNonSystemToken("*"); 
        }

        [Fact]
        public void SpaceAroundStarIsOkInExpand()
        {
            var result = this.ParseExpandTerm("   * ");
            result.ShouldBeNonSystemToken("*"); 
        }

        [Fact]
        public void StarCannotBeInMiddleOfPathInExpand()
        {
            Action parse = () => this.ParseExpandTerm("foo/*/bar");
            parse.Throws<ODataException>(SRResources.ExpressionToken_NoSegmentAllowedBeforeStarInExpand);
        }

        [Fact]
        public void StarWithRefIsOkInExpand()
        {
            var result = this.ParseExpandTerm("*/$ref");
            result.ShouldBeNonSystemToken("$ref").NextToken.ShouldBeNonSystemToken("*");
        }

        [Fact]
        public void PropertyAfterStarWithRefIsOkInExpand()
        {
            Action parse = () => this.ParseExpandTerm("*/$ref/prop");
            parse.Throws<ODataException>(SRResources.ExpressionToken_NoPropAllowedAfterRef);
        } 

        [Fact]
        public void StarAfterNavPropIsOkInExpand()
        {
            Action parse = () => this.ParseExpandTerm("navprop/*");
            parse.Throws<ODataException>(SRResources.ExpressionToken_NoSegmentAllowedBeforeStarInExpand);
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
                Assert.Throws<ODataException>(parse); // TODO: Better error message
            }
        }

        [Fact]
        public void ContainerQualifiedWildcardNotAllowedInExpand()
        {
            Action parseWithContainerQualfiedWildcard = () => this.ParseExpandTerm("container.qualified.*");
            parseWithContainerQualfiedWildcard.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_SyntaxError, "21", "container.qualified.*"));
        }

        [Fact]
        public void TrailingSlashShouldBeIgnoredInExpand()
        {
            var result = this.ParseExpandTerm("path/to/a/navprop/");
            Assert.NotNull(result.ShouldBeNonSystemToken("navprop").NextToken);
        }

        [Fact]
        public void DoubleTrailingSlashShouldFailInExpand()
        {
            Action parse = () => this.ParseExpandTerm("navprop//");
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, 8));
        }

        [Fact]
        public void TermParserStopsParsingAtOpenParenthesisForOptions()
        {
            ExpressionLexer lexer;
            var result = this.ParseExpandTerm("NavProp($filter=true)", out lexer);
            result.ShouldBeNonSystemToken("NavProp");
            Assert.Equal(ExpressionTokenKind.OpenParen, lexer.CurrentToken.Kind);
        }

        // TODO: maybe need verification of where the lexer is left off?

        #endregion

        #region Helpers

        /// <summary>
        /// Runs a unit test on SelectExpandTermParser.ParseTerm() for a $select term.
        /// </summary>
        private static PathSegmentToken ParseSelectTerm(string term)
        {
            var lexer = new ExpressionLexer(HardCodedTestModel.TestModel, expression: term, moveToFirstToken: true, useSemicolonDelimiter: true);
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
            lexer = new ExpressionLexer(HardCodedTestModel.TestModel, expression: term, moveToFirstToken: true, useSemicolonDelimiter: true);
            var parser = new SelectExpandTermParser(lexer, 100 /*maxPathLength*/, false /*isSelect*/);
            return parser.ParseTerm(allowRef: true);
        }

        #endregion
    }
}
