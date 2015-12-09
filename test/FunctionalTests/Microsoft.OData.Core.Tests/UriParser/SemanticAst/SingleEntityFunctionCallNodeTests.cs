//---------------------------------------------------------------------
// <copyright file="SingleEntityFunctionCallNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the SingleEntityFunctionCallNode class
    /// </summary>
    public class SingleEntityFunctionCallNodeTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new SingleEntityFunctionCallNode(null, new QueryNode[] { }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }
        
        [Fact]
        public void EntityTypeReferenceCannotBeNull()
        {
            Action createWithNullEntityType = () => new SingleEntityFunctionCallNode("stuff", new QueryNode[] { }, null, HardCodedTestModel.GetPeopleSet());
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("entityTypeReference").ToString());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", new QueryNode[] { }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.ShouldBeSingleEntityFunctionCallNode("stuff");
        }

        [Fact]
        public void ArgumentsSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.Parameters.Should().HaveCount(1);
            singleEntityFunctionCall.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void EntityTypeReferenceSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.EntityTypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonTypeReference().FullName());
            singleEntityFunctionCall.TypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonTypeReference().FullName());
        }

        [Fact]
        public void KindIsSingleEntityFunction()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.Kind.Should().Be(QueryNodeKind.SingleEntityFunctionCall);
        }

        [Fact]
        public void FunctionImportsAreSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("HasDog", HardCodedTestModel.GetHasDogOverloads(), null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), ModelBuildingHelpers.BuildValidEntitySet(), null);
            singleEntityFunctionCall.Functions.Should().ContainExactly(HardCodedTestModel.GetHasDogOverloads());
        }

        [Fact]
        public void ParametersSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", null, null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), null, null);
            singleEntityFunctionCall.Parameters.Should().NotBeNull();
            singleEntityFunctionCall.Parameters.Should().BeEmpty();
        }
    }
}
