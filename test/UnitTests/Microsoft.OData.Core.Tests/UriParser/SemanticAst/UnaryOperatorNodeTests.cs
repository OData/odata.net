//---------------------------------------------------------------------
// <copyright file="UnaryOperatorNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
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
            Assert.Equal(UnaryOperatorKind.Negate, unaryOperatorNode.OperatorKind);
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
            Assert.True(unaryOperatorNode.TypeReference.IsEquivalentTo(new ConstantNode(1).TypeReference));
        }

        [Fact]
        public void KindIsUnaryOperatorNode()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            Assert.Equal(InternalQueryNodeKind.UnaryOperator, unaryOperatorNode.InternalKind);
        }
    }
}
