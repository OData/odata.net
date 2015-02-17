//---------------------------------------------------------------------
// <copyright file="BinaryOperatorUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the BinaryOperatorNode class
    /// </summary>
    [TestClass]
    public class BinaryOperatorUnitTests
    {
        [TestMethod]
        public void OperatorKindShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.OperatorKind.Should().Be(BinaryOperatorKind.Add);
        }

        [TestMethod]
        public void LeftShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.Left.Should().Be(left);
        }

        [TestMethod]
        public void RightShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.Right.Should().Be(right);
        }

        [TestMethod]
        public void TypeReferenceIsSetCorrectlyFromOperands()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.TypeReference.FullName().Should().Be("Edm.Int32");
        }

        [TestMethod]
        public void KindIsBinaryOperatorNode()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.InternalKind.Should().Be(InternalQueryNodeKind.BinaryOperator);
        }
    }
}
