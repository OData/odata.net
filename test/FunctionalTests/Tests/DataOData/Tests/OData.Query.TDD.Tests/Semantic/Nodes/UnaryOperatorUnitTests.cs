//---------------------------------------------------------------------
// <copyright file="UnaryOperatorUnitTests.cs" company="Microsoft">
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
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the UnaryOperator class
    /// </summary>
    [TestClass]
    public class UnaryOperatorUnitTests
    {
        [TestMethod]
        public void OperatorKindSetCorrectly()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            unaryOperatorNode.OperatorKind.Should().Be(UnaryOperatorKind.Negate);
        }

        [TestMethod]
        public void OperandSetCorrectly()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            unaryOperatorNode.Operand.ShouldBeConstantQueryNode(1);
        }

        [TestMethod]
        public void TypeReferenceSetFromOperand()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            unaryOperatorNode.TypeReference.Should().Be(new ConstantNode(1).TypeReference);
        }

        [TestMethod]
        public void KindIsUnaryOperatorNode()
        {
            UnaryOperatorNode unaryOperatorNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            unaryOperatorNode.InternalKind.Should().Be(InternalQueryNodeKind.UnaryOperator);
        }
    }
}
