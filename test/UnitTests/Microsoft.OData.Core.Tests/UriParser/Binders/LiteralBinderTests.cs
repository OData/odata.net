//---------------------------------------------------------------------
// <copyright file="LiteralBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class LiteralBinderTests
    {
        [Fact]
        public void BindLiteralShouldReturnIntValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(5)) as ConstantNode;
            Assert.Equal(5, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnDateTimeOffsetValue()
        {
            var value = new DateTimeOffset(2012, 12, 2, 3, 34, 20, 0, new TimeSpan(2, 0, 0));
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnDateValue()
        {
            var value = new DateOnly(2012, 12, 2);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnTimeOfDayValue()
        {
            var value = new TimeOnly(10, 15, 5, 20);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldReturnQueryNode()
        {
            var value = GeometryPoint.Create(5, 2);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value)) as ConstantNode;
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public void BindLiteralShouldSetLiteralTextFromToken()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(1, "originalText")) as ConstantNode;
            Assert.Equal("originalText", result.LiteralText);
        }
    }
}
