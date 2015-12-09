//---------------------------------------------------------------------
// <copyright file="LiteralBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm.Library;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    public class LiteralBinderTests
    {
        [Fact]
        public void BindLiteralShouldReturnIntValue()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(5));
            result.Value.Should().Be(5);
        }

        [Fact]
        public void BindLiteralShouldReturnDateTimeOffsetValue()
        {
            var value = new DateTimeOffset(2012, 12, 2, 3, 34, 20, 0, new TimeSpan(2, 0, 0));
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value));
            result.Value.Should().Be(value);
        }

        [Fact]
        public void BindLiteralShouldReturnDateValue()
        {
            var value = new Date(2012, 12, 2);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value));
            result.Value.Should().Be(value);
        }

        [Fact]
        public void BindLiteralShouldReturnTimeOfDayValue()
        {
            var value = new TimeOfDay(10, 15, 5, 20);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value));
            result.Value.Should().Be(value);
        }

        [Fact]
        public void BindLiteralShouldReturnQueryNode()
        {
            var value = GeometryPoint.Create(5, 2);
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(value));
            result.Value.Should().Be(value);
        }

        [Fact]
        public void BindLiteralShouldSetLiteralTextFromToken()
        {
            ConstantNode result = LiteralBinder.BindLiteral(new LiteralToken(1, "originalText"));
            result.LiteralText.Should().Be("originalText");
        }
    }
}
