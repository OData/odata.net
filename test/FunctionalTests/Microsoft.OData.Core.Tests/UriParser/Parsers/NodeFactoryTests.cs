//---------------------------------------------------------------------
// <copyright file="NodeFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class NodeFactoryTests
    {
        private static readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        [Fact]
        public void CreateParameterNodeShouldReturnEntityParameterQueryNodeForEntityType()
        {
            var nodeToIterationOver = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var resultNode = NodeFactory.CreateParameterNode("a", nodeToIterationOver);
            var ns = resultNode.ShouldBeResourceRangeVariable(HardCodedTestModel.GetPersonTypeReference())
                .CollectionResourceNode.NavigationSource;
            Assert.Same(ns, HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void CreateParameterNodeShouldKeepParameterNameExactlyTheSame()
        {
            var nodeToIterationOver = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var resultNode = NodeFactory.CreateParameterNode("PARAM_name!@#", nodeToIterationOver);
            Assert.Equal("PARAM_name!@#", resultNode.Name);
        }

        [Fact]
        public void CreateImplicitParameterNodeShouldReturnEntityParameterQueryNodeForEntityType()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.ShouldBeResourceRangeVariable(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void CreateImplicitParameterNodeShouldReturnNonEntityParameterQueryNodeForPrimitiveEntityType()
        {
            var type = EdmCoreModel.Instance.GetString(false);
            var resultNode = NodeFactory.CreateImplicitRangeVariable(type, null);
            var typeReference = resultNode.ShouldBeNonentityRangeVariable(ExpressionConstants.It).TypeReference;
            Assert.True(typeReference.IsEquivalentTo(type));
        }

        [Fact]
        public void CreateDollarThisRangeVariableShouldReturnEntityParameterQueryNodeForEntityType()
        {
            var resultNode = NodeFactory.CreateDollarThisRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.ShouldBeResourceRangeVariable(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void CreateDollarThisRangeVariableShouldReturnNonEntityParameterQueryNodeForPrimitiveEntityType()
        {
            var type = EdmCoreModel.Instance.GetString(false);
            var resultNode = NodeFactory.CreateDollarThisRangeVariable(type, null);
            var typeReference = resultNode.ShouldBeNonentityRangeVariable(ExpressionConstants.This).TypeReference;
            Assert.True(typeReference.IsEquivalentTo(type));
        }

        [Fact]
        public void CreateImplicitParameterNodeUsesEntitySetIfProvidedAndTypeWasEntity()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.ShouldBeResourceRangeVariable(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void CreateImplicitParameterNodeShouldHaveRightName()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            Assert.Equal(ExpressionConstants.It, resultNode.Name);
        }

        [Fact]
        public void CreateImplicitParameterNodeFromEntityCollectionNodeShouldCreateEntityParameterNode()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(new ODataPath(new EntitySetSegment(HardCodedTestModel.GetLionSet())));
            resultNode.ShouldBeResourceRangeVariable(HardCodedTestModel.GetLionTypeReference());
        }

        [Fact]
        public void CreateImplicitParameterNodeFromNonEntityCollectionShouldCreateNonResourceRangeVariableReferenceNode()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(new ODataPath(new[] {new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp())}));
            Assert.Same(resultNode.ShouldBeResourceRangeVariable(ExpressionConstants.It).TypeReference.Definition, HardCodedTestModel.GetAddressType());
            Assert.Equal(ExpressionConstants.It, resultNode.Name);
        }

        [Fact]
        public void CreateLambdaNodeForAnyTokenShouldCreateAnyNode()
        {
            BindingState bindingState = new BindingState(configuration);
            CollectionResourceNode parent = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            SingleValueNode expression = new ConstantNode(true);
            RangeVariable rangeVariable = new ResourceRangeVariable("bob", HardCodedTestModel.GetPersonTypeReference(), parent);
            var resultNode = NodeFactory.CreateLambdaNode(bindingState, parent, expression, rangeVariable, QueryTokenKind.Any);
            var node = resultNode.ShouldBeAnyQueryNode();
            Assert.Same(expression, node.Body);
            Assert.Same(parent, node.Source);
        }

        [Fact]
        public void CreateLambdaNodeForAllTokenShouldCreateAllNode()
        {
            BindingState bindingState = new BindingState(configuration);
            CollectionResourceNode parent = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            SingleValueNode expression = new ConstantNode(true);
            RangeVariable rangeVariable = new ResourceRangeVariable("bob", HardCodedTestModel.GetPersonTypeReference(), parent);
            var resultNode = NodeFactory.CreateLambdaNode(bindingState, parent, expression, rangeVariable, QueryTokenKind.All);

            var node = resultNode.ShouldBeAllQueryNode();
            Assert.Same(expression, node.Body);
            Assert.Same(parent, node.Source);
        }
    }
}
