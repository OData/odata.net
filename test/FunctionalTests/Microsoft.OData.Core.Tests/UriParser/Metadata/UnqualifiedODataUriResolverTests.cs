//---------------------------------------------------------------------
// <copyright file="UnqualifiedODataUriResolverTests.cs" company="Microsoft">
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
    public class UnqualifiedODataUriResolverTests
        : ExtensionTestBase
    {
        [Fact]
        public void UnqualifiedFunctionInPathTest()
        {
            this.TestUnqualified(
                "People(1)/TestNS.FindPencil(pid=2)",
                "People(1)/FindPencil(pid=2)",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationSegment(FindPencil2P),
                Strings.RequestUriProcessor_ResourceNotFound("FindPencil"));
        }

        [Fact]
        public void UnqualifiedFunctionWithNoParameterInPathTest()
        {
            this.TestUnqualified(
                "People(1)/TestNS.FindPencil",
                "People(1)/FindPencil",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationSegment(FindPencil1P),
                Strings.RequestUriProcessor_ResourceNotFound("FindPencil"));
        }

        [Fact]
        public void UnqualifiedActionOnComplexTypeInPathTest()
        {
            this.TestUnqualified(
                "People(1)/Addr/TestNS.ChangeZip",
                "People(1)/Addr/ChangeZip",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeOperationSegment(ChangeZip),
                Strings.RequestUriProcessor_ResourceNotFound("ChangeZip"));
        }

        [Fact]
        public void UnqualifiedOperationNonexistInPath()
        {
            this.TestCaseUnqualifiedNotExist(
                "People(1)/Addr/ChangeZipEE",
                parser => parser.ParsePath(),
                Strings.RequestUriProcessor_ResourceNotFound("ChangeZipEE"));
        }

        [Fact]
        public void UnqualifiedOperationConflictsInPath()
        {
            this.TestCaseUnqualifiedConflict(
                "People(1)/FindPencilsCon",
                parser => parser.ParsePath(),
                Strings.FunctionOverloadResolver_NoSingleMatchFound("FindPencilsCon", ""));
        }

        [Fact]
        public void UnqualifiedFunctionInQueryTest()
        {
            this.TestUnqualified(
                "People?$orderby=TestNS.FindPencil(pid=2)/Id",
                "People?$orderby=FindPencil(pid=2)/Id",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PencilId).And.Source.ShouldBeSingleEntityFunctionCallNode("TestNS.FindPencil"),
                Strings.MetadataBinder_UnknownFunction("FindPencil"));
        }

        [Fact]
        public void UnqualifiedFunctionWithNoParameterInQueryTest()
        {
            this.TestUnqualified(
                "People?$orderby=TestNS.FindPencil/Id",
                "People?$orderby=FindPencil/Id",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PencilId).And.Source.ShouldBeSingleEntityFunctionCallNode("TestNS.FindPencil"),
                Strings.MetadataBinder_PropertyNotDeclared("TestNS.Person", "FindPencil"));
        }

        [Fact]
        public void UnqualifiedFunctionOnComplexTypeInQueryTest()
        {
            this.TestUnqualified(
                "People?$orderby=Addr/TestNS.GetZip",
                "People?$orderby=Addr/GetZip",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValueFunctionCallQueryNode("TestNS.GetZip").And.Source.ShouldBeSingleValuePropertyAccessQueryNode(AddrProperty),
                Strings.MetadataBinder_PropertyNotDeclared("TestNS.Address", "GetZip"));
        }

        [Fact]
        public void UnqualifiedOperationNonexistInOrderBy()
        {
            this.TestCaseUnqualifiedNotExist(
                "People?$orderby=FindPencilEE/Id",
                parser => parser.ParseOrderBy(),
                Strings.MetadataBinder_PropertyNotDeclared("TestNS.Person", "FindPencilEE"));
        }

        [Fact]
        public void UnqualifiedOperationConflictsInOrderby()
        {
            // Actually not a valid orderby cause, but use it to test pre-thrown exceptions.
            this.TestCaseUnqualifiedConflict(
                "People?$orderby=FindPencilsCon",
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
