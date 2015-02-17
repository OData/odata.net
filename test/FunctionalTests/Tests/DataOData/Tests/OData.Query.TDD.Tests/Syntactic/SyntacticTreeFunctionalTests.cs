//---------------------------------------------------------------------
// <copyright file="SyntacticTreeFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core;
using Microsoft.OData.Core.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    /// <summary>
    /// Various functional tests for the syntactic parser.
    /// TODO: Ideally some of these should be unit tests deeper down in the syntactic parsing.
    /// </summary>
    [TestClass]
    public class SyntacticTreeFunctionalTests
    {
        [TestMethod]
        public void AnyNotAllowedInTopLevelFilter()
        {
            Action parse = () => HardCodedTestModel.GetSyntacticTree("People?$filter=any(a: true");
            parse.ShouldThrow<ODataException>().WithMessage("')' or ',' expected at position 5 in 'any(a: true'.");
        }

        [TestMethod]
        public void AllNotAllowedInTopLevelFilter()
        {
            Action parse = () => HardCodedTestModel.GetSyntacticTree("People?$filter=all(a: true");
            parse.ShouldThrow<ODataException>().WithMessage("')' or ',' expected at position 5 in 'all(a: true'.");
        }

        [TestMethod]
        public void FunctionWithLamdbaVariableNotAllowedInTopLevelFilter()
        {
            Action parse = () => HardCodedTestModel.GetSyntacticTree("People?$filter=foo(a: true");
            parse.ShouldThrow<ODataException>().WithMessage("')' or ',' expected at position 5 in 'foo(a: true'.");
        }

        [TestMethod]
        public void AnyNotAllowedInTopLevelOrderby()
        {
            Action parse = () => HardCodedTestModel.GetSyntacticTree("People?$orderby=any(a: true");
            parse.ShouldThrow<ODataException>().WithMessage("')' or ',' expected at position 5 in 'any(a: true'.");
        }

        [TestMethod]
        public void AllNotAllowedInTopLevelOrderby()
        {
            Action parse = () => HardCodedTestModel.GetSyntacticTree("People?$orderby=all(a: true");
            parse.ShouldThrow<ODataException>().WithMessage("')' or ',' expected at position 5 in 'all(a: true'.");
        }

        [TestMethod]
        public void FunctionWithLamdbaVariableNotAllowedInTopLevelOrderby()
        {
            Action parse = () => HardCodedTestModel.GetSyntacticTree("People?$orderby=foo(a: true");
            parse.ShouldThrow<ODataException>().WithMessage("')' or ',' expected at position 5 in 'foo(a: true'.");
        }

        [TestMethod]
        public void FilterWithBasicAnyExpression()
        {
            var tree = HardCodedTestModel.GetSyntacticTree("People?$filter=MyFriends/any(a: a/Name eq 'Mike')");
            
            // any(...)
            var any = tree.Filter.ShouldBeAnyToken("a").And;

            // MyFriends/
            any.Parent.ShouldBeInnerPathToken("MyFriends");

            // a:
            any.Parameter.Should().Be("a");

            // a/Name eq 'Mike'
            var binaryToken = any.Expression.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal).And;
            binaryToken.Left.ShouldBeEndPathToken("Name").And.NextToken.ShouldBeRangeVariableToken("a");
            binaryToken.Right.ShouldBeLiteralQueryToken("Mike");
        }
    }
}
