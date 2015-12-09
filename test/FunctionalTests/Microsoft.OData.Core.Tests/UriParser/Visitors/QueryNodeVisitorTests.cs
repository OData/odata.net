//---------------------------------------------------------------------
// <copyright file="QueryNodeVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Core.UriParser.Visitors;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Visitors
{
    public class QueryNodeVisitorTests
    {
        private class FakeVisitor : QueryNodeVisitor<string>
        {
        }
        
        [Fact]
        public void AllNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitAllNode = () => visitor.Visit(new AllNode(null, null));
            visitAllNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void AnyNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitAnyNode = () => visitor.Visit(new AnyNode(null, null));
            visitAnyNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void BinaryOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitBinaryOperatorNode = () => visitor.Visit(new BinaryOperatorNode(BinaryOperatorKind.Equal, new ConstantNode(1), new ConstantNode(2)));
            visitBinaryOperatorNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void CollectionNavigationNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitCollectionNavigationNode = () => visitor.Visit(new CollectionNavigationNode(ModelBuildingHelpers.BuildCollectionNavigationProperty(), HardCodedTestModel.GetPeopleSet()));
            visitCollectionNavigationNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void CollectionPropertyAccessNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitCollectionPropertyAccessNode = () => visitor.Visit(new CollectionPropertyAccessNode(new ConstantNode(1), HardCodedTestModel.GetPersonGeographyCollectionProp()));
            visitCollectionPropertyAccessNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void ConstantNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitConstantNode = () => visitor.Visit(new ConstantNode(null));
            visitConstantNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void ConvertNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitConvertNode = () => visitor.Visit(new ConvertNode(new ConstantNode(1), EdmCoreModel.Instance.GetBinary(true)));
            visitConvertNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void EntityCollectionCastNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitEntityCollectionCastNode = () => visitor.Visit(new EntityCollectionCastNode(new EntitySetNode(HardCodedTestModel.GetPeopleSet()), HardCodedTestModel.GetPersonType()));
            visitEntityCollectionCastNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void EntityRangeVariableReferenceNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitEntityRangeVariableReferenceNode = () => visitor.Visit(new EntityRangeVariableReferenceNode("stuff", new EntityRangeVariable("stuff", HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet())));
            visitEntityRangeVariableReferenceNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void NonentityRangeVariableReferenceNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            ConstantNode constNode = new ConstantNode("stuff");
            NonentityRangeVariable nonentityRangeVariable = new NonentityRangeVariable("dummy", constNode.TypeReference, null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode(nonentityRangeVariable.Name, nonentityRangeVariable);
            Action visitNonEntityRangeVariableReferenceNode = () => visitor.Visit(nonentityRangeVariableReferenceNode);
            visitNonEntityRangeVariableReferenceNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleEntityCastNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleEntityCastNode = () => visitor.Visit(new SingleEntityCastNode(null, ModelBuildingHelpers.BuildValidEntityType()));
            visitSingleEntityCastNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleNavigationNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleNavigationNode = () => visitor.Visit(new SingleNavigationNode(ModelBuildingHelpers.BuildValidNavigationProperty(), HardCodedTestModel.GetPeopleSet()));
            visitSingleNavigationNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleEntityFunctionCallNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleEntityFunctionCallNode = () => visitor.Visit(new SingleEntityFunctionCallNode("stuff", null, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet()));
            visitSingleEntityFunctionCallNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleValueFunctionCallNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleValueFunctionCallNode = () => visitor.Visit(new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true)));
            visitSingleValueFunctionCallNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleValueOpenPropertyAccessNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleValueOpenPropertyAccessNode = () => visitor.Visit(new SingleValueOpenPropertyAccessNode(new ConstantNode(1), "stuff"));
            visitSingleValueOpenPropertyAccessNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleValuePropertyAccessNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleValuePropertyAccessNode = () => visitor.Visit(new SingleValuePropertyAccessNode(new ConstantNode(1), ModelBuildingHelpers.BuildValidPrimitiveProperty()));
            visitSingleValuePropertyAccessNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void UnaryOperatorNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorNode = () => visitor.Visit(new UnaryOperatorNode(UnaryOperatorKind.Not, new ConstantNode(1)));
            visitUnaryOperatorNode.ShouldThrow<NotImplementedException>();
        }
    }
}
