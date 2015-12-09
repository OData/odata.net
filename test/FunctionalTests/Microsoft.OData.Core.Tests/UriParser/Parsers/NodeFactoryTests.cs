//---------------------------------------------------------------------
// <copyright file="NodeFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm.Library;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    public class NodeFactoryTests
    {
        private static readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        [Fact]
        public void CreateParameterNodeShouldReturnEntityParameterQueryNodeForEntityType()
        {
            var nodeToIterationOver = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var resultNode = NodeFactory.CreateParameterNode("a", nodeToIterationOver);
            resultNode.ShouldBeEntityRangeVariable(HardCodedTestModel.GetPersonTypeReference())
                .And.EntityCollectionNode.NavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void CreateParameterNodeShouldKeepParameterNameExactlyTheSame()
        {
            var nodeToIterationOver = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var resultNode = NodeFactory.CreateParameterNode("PARAM_name!@#", nodeToIterationOver);
            resultNode.Name.Should().Be("PARAM_name!@#");
        }

        [Fact]
        public void CreateImplicitParameterNodeShouldReturnEntityParameterQueryNodeForEntityType()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.ShouldBeEntityRangeVariable(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void CreateImplicitParameterNodeShouldReturnNonEntityParameterQueryNodeForPrimitiveEntityType()
        {
            var type = EdmCoreModel.Instance.GetString(false);
            var resultNode = NodeFactory.CreateImplicitRangeVariable(type, null);
            resultNode.ShouldBeNonentityRangeVariable(ExpressionConstants.It).And.TypeReference.Should().Be(type);
        }

        [Fact]
        public void CreateImplicitParameterNodeUsesEntitySetIfProvidedAndTypeWasEntity()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.ShouldBeEntityRangeVariable(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void CreateImplicitParameterNodeShouldHaveRightName()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.Name.Should().Be(ExpressionConstants.It);
        }

        [Fact]
        public void CreateImplicitParameterNodeFromEntityCollectionNodeShouldCreateEntityParameterNode()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(new ODataPath(new EntitySetSegment(HardCodedTestModel.GetLionSet())));
            resultNode.ShouldBeEntityRangeVariable(HardCodedTestModel.GetLionTypeReference());
        }

        [Fact]
        public void CreateImplicitParameterNodeFromNonEntityCollectionShouldCreateNonEntityRangeVariableReferenceNode()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(new ODataPath(new[] {new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp())}));
            resultNode.ShouldBeNonentityRangeVariable(ExpressionConstants.It).And.TypeReference.Definition.Should().Be(HardCodedTestModel.GetAddressType());
            resultNode.Name.Should().Be(ExpressionConstants.It);
        }

        [Fact]
        public void CreateLambdaNodeForAnyTokenShouldCreateAnyNode()
        {
            BindingState bindingState = new BindingState(configuration);
            EntityCollectionNode parent = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            SingleValueNode expression = new ConstantNode(true);
            RangeVariable rangeVariable = new EntityRangeVariable("bob", HardCodedTestModel.GetPersonTypeReference(), parent);
            var resultNode = NodeFactory.CreateLambdaNode(bindingState, parent, expression, rangeVariable, QueryTokenKind.Any);
            var node = resultNode.ShouldBeAnyQueryNode().And;
            node.Body.Should().BeSameAs(expression);
            node.Source.Should().BeSameAs(parent);
        }

        [Fact]
        public void CreateLambdaNodeForAllTokenShouldCreateAllNode()
        {
            BindingState bindingState = new BindingState(configuration);
            EntityCollectionNode parent = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            SingleValueNode expression = new ConstantNode(true);
            RangeVariable rangeVariable = new EntityRangeVariable("bob", HardCodedTestModel.GetPersonTypeReference(), parent);
            var resultNode = NodeFactory.CreateLambdaNode(bindingState, parent, expression, rangeVariable, QueryTokenKind.All);

            var node = resultNode.ShouldBeAllQueryNode().And;
            node.Body.Should().BeSameAs(expression);
            node.Source.Should().BeSameAs(parent);
        }
    }
}
