//---------------------------------------------------------------------
// <copyright file="BinaryOperatorNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
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
            operatorNode.OperatorKind.Should().Be(BinaryOperatorKind.Add);
        }

        [Fact]
        public void LeftShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.Left.Should().Be(left);
        }

        [Fact]
        public void RightShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.Right.Should().Be(right);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectlyFromOperands()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.TypeReference.FullName().Should().Be("Edm.Int32");
        }

        [Fact]
        public void KindIsBinaryOperatorNode()
        {
            ConstantNode left = new ConstantNode(1);
            ConstantNode right = new ConstantNode(2);
            BinaryOperatorNode operatorNode = new BinaryOperatorNode(BinaryOperatorKind.Add, left, right);
            operatorNode.InternalKind.Should().Be(InternalQueryNodeKind.BinaryOperator);
        }
    }
}
