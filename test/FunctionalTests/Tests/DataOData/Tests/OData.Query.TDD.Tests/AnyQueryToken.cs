//---------------------------------------------------------------------
// <copyright file="AnyQueryToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Core;
using Microsoft.OData.Core.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    [TestClass]
    public class AnyQueryTest
    {
        [TestMethod]
        public void ShouldMapStructure()
        {
            var expressionParser = new UriQueryExpressionParser(800);
            var tok = expressionParser.ParseFilter("any(ol: ol/Ratings gt 42)");

            tok.Should().BeOfType<FunctionCallQueryToken>();
            var anyTok = tok.As<FunctionCallQueryToken>();

            anyTok.Kind.Should().Be(QueryTokenKind.FunctionCall);
            anyTok.Name.Should().Be("any");

            anyTok.Arguments.Count().Should().Be(2);
            anyTok.Arguments.ElementAt(0).As<PropertyAccessQueryToken>().Name.Should().Be("ol");
            
            var binTok = anyTok.Arguments.ElementAt(1).As<BinaryOperatorQueryToken>();
            binTok.OperatorKind.Should().Be(BinaryOperatorKind.GreaterThan);
            binTok.Left.As<PropertyAccessQueryToken>().Name.Should().Be("Ratings");
            binTok.Left.As<PropertyAccessQueryToken>().Parent.As<PropertyAccessQueryToken>().Name.Should().Be("ol");
            binTok.Right.As<LiteralQueryToken>().Value.Should().Be(42);
        }

        [TestMethod]
        public void ShouldHandleEmptyArgument()
        {
            var expressionParser = new UriQueryExpressionParser(800);
            var tok = expressionParser.ParseFilter("any()");

            tok.Should().BeOfType<FunctionCallQueryToken>();
            var anyTok = tok.As<FunctionCallQueryToken>();

            anyTok.Kind.Should().Be(QueryTokenKind.FunctionCall);
            anyTok.Name.Should().Be("any");

            anyTok.Arguments.Count().Should().Be(0);
        }

        [TestMethod]
        public void ShouldCatchMissingColon()
        {
            var expressionParser = new UriQueryExpressionParser(800);
            try
            {
                var tok = expressionParser.ParseFilter("any(ol ol/SomeBool)");
            }
            catch (ODataException ex)
            {
                Debug.Assert(ex.ToString().StartsWith("Microsoft.OData.Core.ODataException: ')' or ',' expected"),ex.ToString());
            }
        }  
    }
}
