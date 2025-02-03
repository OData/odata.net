//---------------------------------------------------------------------
// <copyright file="SingleResourceFunctionCallNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;

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
            Assert.Throws<ArgumentNullException>("name", createWithNullName);
        }
        
        [Fact]
        public void ReturnedStructuredTypeReferenceCannotBeNull()
        {
            Action createWithNullEntityType = () => new SingleResourceFunctionCallNode("stuff", new QueryNode[] { }, null, HardCodedTestModel.GetPeopleSet());
            Assert.Throws<ArgumentNullException>("returnedStructuredTypeReference", createWithNullEntityType);
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
            Assert.Single(singleEntityFunctionCall.Parameters);
            singleEntityFunctionCall.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void EntityTypeReferenceSetCorrectly()
        {
            string name = HardCodedTestModel.GetPersonTypeReference().FullName();
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            Assert.Equal(name, singleEntityFunctionCall.StructuredTypeReference.FullName());
            Assert.Equal(name, singleEntityFunctionCall.TypeReference.FullName());
        }

        [Fact]
        public void KindIsSingleEntityFunction()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            Assert.Equal(QueryNodeKind.SingleResourceFunctionCall, singleEntityFunctionCall.Kind);
        }

        [Fact]
        public void FunctionImportsAreSetCorrectly()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("HasDog", HardCodedTestModel.GetHasDogOverloads(), null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), ModelBuildingHelpers.BuildValidEntitySet(), null);
            singleEntityFunctionCall.Functions.ContainExactly(HardCodedTestModel.GetHasDogOverloads());
        }

        [Fact]
        public void ParametersSetCorrectly()
        {
            SingleResourceFunctionCallNode singleEntityFunctionCall = new SingleResourceFunctionCallNode("stuff", null, null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), null, null);
            Assert.NotNull(singleEntityFunctionCall.Parameters);
            Assert.Empty(singleEntityFunctionCall.Parameters);
        }
    }
}
