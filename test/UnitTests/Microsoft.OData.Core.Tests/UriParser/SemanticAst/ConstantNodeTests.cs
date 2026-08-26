//---------------------------------------------------------------------
// <copyright file="ConstantNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
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
            Assert.Equal(1, Assert.IsType<int>(constantNode.Value));
        }

        [Fact]
        public void TypeIsSetFromTheTypeOfTheValue()
        {
            ConstantNode constantNode = new ConstantNode(1);
            Assert.Equal("Edm.Int32", constantNode.TypeReference.FullName());
        }

        [Fact]
        public void KindIsConstantNode()
        {
            ConstantNode constantNode = new ConstantNode(1);
            Assert.Equal(InternalQueryNodeKind.Constant, constantNode.InternalKind);
        }

        [Fact]
        public void NullValueShouldNotThrow()
        {
            Action target = () => new ConstantNode(null);
            target.DoesNotThrow();
        }

        [Fact]
        public void NullLiteralTextShouldThrow()
        {
            Action target = () => new ConstantNode(null, null);
            Assert.Throws<ArgumentNullException>("literalText", target);
        }

        [Fact]
        public void EmptyLiteralTextShouldThrow()
        {
            Action createWithoutType = () => new ConstantNode(string.Empty, string.Empty);
            Action createWithType = () => new ConstantNode(string.Empty, string.Empty, EdmCoreModel.Instance.GetString(false));

            Assert.Throws<ArgumentNullException>("literalText", createWithoutType);
            Assert.Throws<ArgumentNullException>("literalText", createWithType);
        }

        [Fact]
        public void LiteralTextPropertyShouldBeSet()
        {
            ConstantNode constantNode = new ConstantNode(null, "foo");
            Assert.Equal("foo", constantNode.LiteralText);
        }
    }
}
