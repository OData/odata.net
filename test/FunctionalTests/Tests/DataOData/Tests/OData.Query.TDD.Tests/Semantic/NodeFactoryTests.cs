//---------------------------------------------------------------------
// <copyright file="NodeFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NodeFactoryTests
    {
        private static readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        [TestMethod]
        public void CreateParameterNodeShouldReturnEntityParameterQueryNodeForEntityType()
        {
            var nodeToIterationOver = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var resultNode = NodeFactory.CreateParameterNode("a", nodeToIterationOver);
            resultNode.ShouldBeEntityRangeVariable(HardCodedTestModel.GetPersonTypeReference())
                .And.EntityCollectionNode.NavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void CreateParameterNodeShouldKeepParameterNameExactlyTheSame()
        {
            var nodeToIterationOver = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var resultNode = NodeFactory.CreateParameterNode("PARAM_name!@#", nodeToIterationOver);
            resultNode.Name.Should().Be("PARAM_name!@#");
        }

        [TestMethod]
        public void CreateImplicitParameterNodeShouldReturnEntityParameterQueryNodeForEntityType()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.ShouldBeEntityRangeVariable(HardCodedTestModel.GetPersonTypeReference());
        }

        [TestMethod]
        public void CreateImplicitParameterNodeShouldReturnNonEntityParameterQueryNodeForPrimitiveEntityType()
        {
            var type = EdmCoreModel.Instance.GetString(false);
            var resultNode = NodeFactory.CreateImplicitRangeVariable(type, null);
            resultNode.ShouldBeNonentityRangeVariable(ExpressionConstants.It).And.TypeReference.Should().Be(type);
        }

        [TestMethod]
        public void CreateImplicitParameterNodeUsesEntitySetIfProvidedAndTypeWasEntity()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.ShouldBeEntityRangeVariable(HardCodedTestModel.GetPersonTypeReference());
        }

        [TestMethod]
        public void CreateImplicitParameterNodeShouldHaveRightName()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetPersonType().ToTypeReference(), HardCodedTestModel.GetPeopleSet());
            resultNode.Name.Should().Be(ExpressionConstants.It);
        }

        [TestMethod]
        public void CreateImplicitParameterNodeFromEntityCollectionNodeShouldCreateEntityParameterNode()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(new ODataPath(new EntitySetSegment(HardCodedTestModel.GetLionSet())));
            resultNode.ShouldBeEntityRangeVariable(HardCodedTestModel.GetLionTypeReference());
        }

        [TestMethod]
        public void CreateImplicitParameterNodeFromNonEntityCollectionShouldCreateNonEntityRangeVariableReferenceNode()
        {
            var resultNode = NodeFactory.CreateImplicitRangeVariable(new ODataPath(new[] {new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp())}));
            resultNode.ShouldBeNonentityRangeVariable(ExpressionConstants.It).And.TypeReference.Definition.Should().Be(HardCodedTestModel.GetAddressType());
            resultNode.Name.Should().Be(ExpressionConstants.It);
        }

        [TestMethod]
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

        [TestMethod]
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
