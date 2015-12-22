//---------------------------------------------------------------------
// <copyright file="OpenTypeUnqualifiedExtensionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Metadata;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Metadata
{
    // Select unqualified Function not supported.
    public class OpenTypeUnqualifiedExtensionTests
        : OpenTypeExtensionTestBase
    {
        [Fact]
        public void OpenTypeUnqualifiedFunctionInPathTest()
        {
            this.TestUnqualified(
                "People(1)/TestNS.FindPencil(pid=2)",
                "People(1)/FindPencil(pid=2)",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationSegment(FindPencil2P),
                Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes("FindPencil"));
        }

        [Fact]
        public void OpenTypeUnqualifiedFunctionWithNoParameterInPathTest()
        {
            this.TestUnqualified(
                "People(1)/TestNS.FindPencil",
                "People(1)/FindPencil",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationSegment(FindPencil1P),
                null);
        }

        [Fact]
        public void OpenTypeUnqualifiedFunctionWithNoParameterWithParenthesisInPathTest()
        {
            this.TestUnqualified(
                "People(1)/TestNS.FindPencil()",
                "People(1)/FindPencil()",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationSegment(FindPencil1P),
                Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes("FindPencil"));
        }

        [Fact]
        public void OpenTypeUnqualifiedActionOnComplexTypeInPathTest()
        {
            this.TestUnqualified(
                "People(1)/Addr/TestNS.ChangeZip",
                "People(1)/Addr/ChangeZip",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationSegment(ChangeZip),
                null);
        }

        [Fact]
        public void OpenTypeUnqualifiedActionWithParenthesisOnComplexTypeInPathTest()
        {
            this.TestUnqualified(
                "People(1)/Addr/TestNS.ChangeZip()",
                "People(1)/Addr/ChangeZip()",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationSegment(ChangeZip),
                Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes("ChangeZip"));
        }

        [Fact]
        public void OpenTypeUnqualifiedOperationWithParenthesisNonexistInPath()
        {
            this.TestCaseUnqualifiedNotExist(
                "People(1)/Addr/ChangeZipEE()",
                parser => parser.ParsePath(),
                Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes("ChangeZipEE"));
        }

        [Fact]
        public void OpenTypeUnqualifiedOperationConflictsInPath()
        {
            this.TestCaseUnqualifiedConflict(
                "People(1)/FindPencilsCon",
                parser => parser.ParsePath(),
                Strings.FunctionOverloadResolver_NoSingleMatchFound("FindPencilsCon", ""));
        }

        [Fact]
        public void OpenTypeUnqualifiedOperationWithParenthesisConflictsInPath()
        {
            this.TestCaseUnqualifiedConflict(
                "People(1)/FindPencilsCon()",
                parser => parser.ParsePath(),
                Strings.FunctionOverloadResolver_NoSingleMatchFound("FindPencilsCon", ""));
        }

        [Fact]
        public void OpenTypeUnqualifiedFunctionInQueryTest()
        {
            this.TestUnqualified(
                "People?$orderby=TestNS.FindPencil(pid=2)/Id",
                "People?$orderby=FindPencil(pid=2)/Id",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PencilId).And.Source.ShouldBeSingleEntityFunctionCallNode("TestNS.FindPencil"),
                Strings.MetadataBinder_UnknownFunction("FindPencil"));
        }

        [Fact]
        public void OpenTypeUnqualifiedFunctionWithNoParameterInQueryTest()
        {
            this.TestUnqualified(
                "People?$orderby=TestNS.FindPencil/Id",
                "People?$orderby=FindPencil/Id",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PencilId).And.Source.ShouldBeSingleEntityFunctionCallNode("TestNS.FindPencil"),
                null);
        }

        [Fact]
        public void OpenTypeUnqualifiedFunctionWithNoParameterWithParenthesisInQueryTest()
        {
            this.TestUnqualified(
                "People?$orderby=TestNS.FindPencil()/Id",
                "People?$orderby=FindPencil()/Id",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PencilId).And.Source.ShouldBeSingleEntityFunctionCallNode("TestNS.FindPencil"),
                Strings.MetadataBinder_UnknownFunction("FindPencil"));
        }

        [Fact]
        public void OpenTypeUnqualifiedFunctionOnComplexTypeInQueryTest()
        {
            this.TestUnqualified(
                "People?$orderby=Addr/TestNS.GetZip",
                "People?$orderby=Addr/GetZip",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValueFunctionCallQueryNode("TestNS.GetZip").And.Source.ShouldBeSingleValuePropertyAccessQueryNode(AddrProperty),
                null);
        }

        [Fact]
        public void OpenTypeUnqualifiedFunctionWithParenthesisOnComplexTypeInQueryTest()
        {
            this.TestUnqualified(
                "People?$orderby=Addr/TestNS.GetZip()",
                "People?$orderby=Addr/GetZip()",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValueFunctionCallQueryNode("TestNS.GetZip").And.Source.ShouldBeSingleValuePropertyAccessQueryNode(AddrProperty),
                Strings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent("GetZip"));
        }

        [Fact]
        public void OpenTypeUnqualifiedOperationWithParenthesisNonexistInOrderBy()
        {
            this.TestCaseUnqualifiedNotExist(
                "People?$orderby=FindPencilEE()/Id",
                parser => parser.ParseOrderBy(),
                Strings.MetadataBinder_UnknownFunction("FindPencilEE"));
        }

        [Fact]
        public void OpenTypeUnqualifiedOperationConflictsInOrderby()
        {
            // Actually not a valid orderby cause, but use it to test pre-thrown exceptions.
            this.TestCaseUnqualifiedConflict(
                "People?$orderby=FindPencilsCon",
                parser => parser.ParseOrderBy(),
                Strings.FunctionOverloadResolver_NoSingleMatchFound("FindPencilsCon", ""));
        }

        [Fact]
        public void OpenTypeUnqualifiedOperationWithParenthesisConflictsInOrderby()
        {
            // Actually not a valid orderby cause, but use it to test pre-thrown exceptions.
            this.TestCaseUnqualifiedConflict(
                "People?$orderby=FindPencilsCon()",
                parser => parser.ParseOrderBy(),
                Strings.FunctionOverloadResolver_NoSingleMatchFound("FindPencilsCon", ""));
        }

        private void TestUnqualified<TResult>(
            string originalStr,
            string caseInsensitiveStr,
            Func<ODataUriParser, TResult> parse,
            Action<TResult> verify,
            string errorMessage)
        {
            this.TestUriParserExtension(originalStr, caseInsensitiveStr, parse, verify, errorMessage, Model, parser => parser.Resolver = new UnqualifiedODataUriResolver());
        }

        private void TestCaseUnqualifiedConflict<TResult>(string originalStr, Func<ODataUriParser, TResult> parse, string conflictMessage)
        {
            this.TestConflict(originalStr, parse, null, conflictMessage, Model, new UnqualifiedODataUriResolver());
        }

        private void TestCaseUnqualifiedNotExist<TResult>(string originalStr, Func<ODataUriParser, TResult> parse, string message)
        {
            this.TestNotExist(originalStr, parse, message, Model, parser => parser.Resolver = new UnqualifiedODataUriResolver());
        }
    }
}
