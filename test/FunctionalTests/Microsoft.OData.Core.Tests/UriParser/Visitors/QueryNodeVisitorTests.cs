//---------------------------------------------------------------------
// <copyright file="QueryNodeVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Visitors
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
            Action visitCollectionNavigationNode = () => visitor.Visit(new CollectionNavigationNode(HardCodedTestModel.GetPeopleSet(), ModelBuildingHelpers.BuildCollectionNavigationProperty(), new EdmPathExpression("Reference")));
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
        public void CollectionResourceCastNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitCollectionResourceCastNode = () => visitor.Visit(new CollectionResourceCastNode(new EntitySetNode(HardCodedTestModel.GetPeopleSet()), HardCodedTestModel.GetPersonType()));
            visitCollectionResourceCastNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void ResourceRangeVariableReferenceNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitResourceRangeVariableReferenceNode = () => visitor.Visit(new ResourceRangeVariableReferenceNode("stuff", new ResourceRangeVariable("stuff", HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet())));
            visitResourceRangeVariableReferenceNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void NonResourceRangeVariableReferenceNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            ConstantNode constNode = new ConstantNode("stuff");
            NonResourceRangeVariable nonentityRangeVariable = new NonResourceRangeVariable("dummy", constNode.TypeReference, null);
            NonResourceRangeVariableReferenceNode nonResourceRangeVariableReferenceNode = new NonResourceRangeVariableReferenceNode(nonentityRangeVariable.Name, nonentityRangeVariable);
            Action visitNonResourceRangeVariableReferenceNode = () => visitor.Visit(nonResourceRangeVariableReferenceNode);
            visitNonResourceRangeVariableReferenceNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleResourceCastNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleResourceCastNode = () => visitor.Visit(new SingleResourceCastNode(null, ModelBuildingHelpers.BuildValidEntityType()));
            visitSingleResourceCastNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleNavigationNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleNavigationNode = () => visitor.Visit(new SingleNavigationNode(HardCodedTestModel.GetPeopleSet(), ModelBuildingHelpers.BuildValidNavigationProperty(), new EdmPathExpression("Reference")));
            visitSingleNavigationNode.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SingleResourceFunctionCallNodeNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSingleResourceFunctionCallNode = () => visitor.Visit(new SingleResourceFunctionCallNode("stuff", null, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet()));
            visitSingleResourceFunctionCallNode.ShouldThrow<NotImplementedException>();
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
