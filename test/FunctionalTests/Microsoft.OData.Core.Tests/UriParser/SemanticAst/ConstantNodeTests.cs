//---------------------------------------------------------------------
// <copyright file="ConstantNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the ConstantNode class
    /// </summary>
    public class ConstantNodeTests
    {
        [Fact]
        public void ValueIsSetCorrectly()
        {
            ConstantNode constantNode = new ConstantNode(1);
            constantNode.Value.As<int>().Should().Be(1);
        }

        [Fact]
        public void TypeIsSetFromTheTypeOfTheValue()
        {
            ConstantNode constantNode = new ConstantNode(1);
            constantNode.TypeReference.FullName().Should().Be("Edm.Int32");
        }

        [Fact]
        public void KindIsConstantNode()
        {
            ConstantNode constantNode = new ConstantNode(1);
            constantNode.InternalKind.Should().Be(InternalQueryNodeKind.Constant);
        }

        [Fact]
        public void NullValueShouldNotThrow()
        {
            Action target = () => new ConstantNode(null);
            target.ShouldNotThrow();
        }

        [Fact]
        public void NullLiteralTextShouldThrow()
        {
            Action target = () => new ConstantNode(null, null);
            target.ShouldThrow<ArgumentNullException>().WithMessage("literalText", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void LiteralTextPropertyShouldBeSet()
        {
            new ConstantNode(null, "foo").LiteralText.Should().Be("foo");
        }
    }
}
