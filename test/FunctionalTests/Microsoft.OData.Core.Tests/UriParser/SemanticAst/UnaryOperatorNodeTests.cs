//---------------------------------------------------------------------
// <copyright file="UnaryOperatorNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the UnaryOperator class
    /// </summary>
    public class UnaryOperatorNodeTests
    {
        [Fact]
        public void OperatorKindSetCorrectly()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            unaryOperatorNode.OperatorKind.Should().Be(UnaryOperatorKind.Negate);
        }

        [Fact]
        public void OperandSetCorrectly()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            unaryOperatorNode.Operand.ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void TypeReferenceSetFromOperand()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            unaryOperatorNode.TypeReference.Should().Be(new ConstantNode(1).TypeReference);
        }

        [Fact]
        public void KindIsUnaryOperatorNode()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            unaryOperatorNode.InternalKind.Should().Be(InternalQueryNodeKind.UnaryOperator);
        }
    }
}
