//---------------------------------------------------------------------
// <copyright file="BinaryOperatorNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the BinaryOperatorNode class
    /// </summary>
    public class BinaryOperatorNodeTests
    {
        [Fact]
        public void OperatorKindShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            Assert.Equal(BinaryOperatorKind.Add, operatorNode.OperatorKind);
        }

        [Fact]
        public void LeftShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            Assert.Same(left, operatorNode.Left);
        }

        [Fact]
        public void RightShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            Assert.Same(right, operatorNode.Right);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectlyFromOperands()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            Assert.Equal("Edm.Int32", operatorNode.TypeReference.FullName());
        }

        [Fact]
        public void KindIsBinaryOperatorNode()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            Assert.Equal(InternalQueryNodeKind.BinaryOperator, operatorNode.InternalKind);
        }
    }
}
