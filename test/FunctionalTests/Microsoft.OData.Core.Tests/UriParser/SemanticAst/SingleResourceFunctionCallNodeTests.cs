//---------------------------------------------------------------------
// <copyright file="SingleResourceFunctionCallNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the SingleResourceFunctionCallNode class
    /// </summary>
    public class SingleResourceFunctionCallNodeTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new SingleResourceFunctionCallNode(null, new QueryNode[] { }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }
        
        [Fact]
        public void EntityTypeReferenceCannotBeNull()
        {
            Action createWithNullEntityType = () => new SingleResourceFunctionCallNode("stuff", new QueryNode[] { }, null, HardCodedTestModel.GetPeopleSet());
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("entityTypeReference").ToString());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("stuff", new QueryNode[] { }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.ShouldBeSingleResourceFunctionCallNode("stuff");
        }

        [Fact]
        public void ArgumentsSetCorrectly()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.Parameters.Should().HaveCount(1);
            singleEntityFunctionCall.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void EntityTypeReferenceSetCorrectly()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.StructuredTypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonTypeReference().FullName());
            singleEntityFunctionCall.TypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonTypeReference().FullName());
        }

        [Fact]
        public void KindIsSingleEntityFunction()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.Kind.Should().Be(QueryNodeKind.SingleResourceFunctionCall);
        }

        [Fact]
        public void FunctionImportsAreSetCorrectly()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("HasDog", HardCodedTestModel.GetHasDogOverloads(), null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), ModelBuildingHelpers.BuildValidEntitySet(), null);
            singleEntityFunctionCall.Functions.Should().ContainExactly(HardCodedTestModel.GetHasDogOverloads());
        }

        [Fact]
        public void ParametersSetCorrectly()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("stuff", null, null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), null, null);
            singleEntityFunctionCall.Parameters.Should().NotBeNull();
            singleEntityFunctionCall.Parameters.Should().BeEmpty();
        }
    }
}
