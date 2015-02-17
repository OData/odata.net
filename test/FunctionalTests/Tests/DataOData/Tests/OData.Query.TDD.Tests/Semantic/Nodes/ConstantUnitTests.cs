//---------------------------------------------------------------------
// <copyright file="ConstantUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the ConstantNode class
    /// </summary>
    [TestClass]
    public class ConstantUnitTests
    {
        [TestMethod]
        public void ValueIsSetCorrectly()
        {
            ConstantNode constantNode = new ConstantNode(1);
            constantNode.Value.As<int>().Should().Be(1);
        }

        [TestMethod]
        public void TypeIsSetFromTheTypeOfTheValue()
        {
            ConstantNode constantNode = new ConstantNode(1);
            constantNode.TypeReference.FullName().Should().Be("Edm.Int32");
        }

        [TestMethod]
        public void KindIsConstantNode()
        {
            ConstantNode constantNode = new ConstantNode(1);
            constantNode.InternalKind.Should().Be(InternalQueryNodeKind.Constant);
        }

        [TestMethod]
        public void NullValueShouldNotThrow()
        {
            Action target = () => new ConstantNode(null);
            target.ShouldNotThrow();
        }

        [TestMethod]
        public void NullLiteralTextShouldThrow()
        {
            Action target = () => new ConstantNode(null, null);
            target.ShouldThrow<ArgumentNullException>().WithMessage("literalText", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void LiteralTextPropertyShouldBeSet()
        {
            new ConstantNode(null, "foo").LiteralText.Should().Be("foo");
        }
    }
}
