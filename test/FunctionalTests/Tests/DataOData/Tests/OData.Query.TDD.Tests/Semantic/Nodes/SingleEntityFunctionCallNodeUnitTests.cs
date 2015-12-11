//---------------------------------------------------------------------
// <copyright file="SingleEntityFunctionCallNodeUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the SingleValueFunctionCallNode class
    /// </summary>
    [TestClass]
    public class SingleEntityFunctionCallNodeUnitTests
    {
        [TestMethod]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new SingleEntityFunctionCallNode(null, new QueryNode[] { }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }
        
        [TestMethod]
        public void EntityTypeReferenceCannotBeNull()
        {
            Action createWithNullEntityType = () => new SingleEntityFunctionCallNode("stuff", new QueryNode[] { }, null, HardCodedTestModel.GetPeopleSet());
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("entityTypeReference").ToString());
        }

        [TestMethod]
        public void NameIsSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", new QueryNode[] { }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.ShouldBeSingleEntityFunctionCallNode("stuff");
        }

        [TestMethod]
        public void ArgumentsSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.Parameters.Should().HaveCount(1);
            singleEntityFunctionCall.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [TestMethod]
        public void EntityTypeReferenceSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.EntityTypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonTypeReference().FullName());
            singleEntityFunctionCall.TypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonTypeReference().FullName());
        }

        [TestMethod]
        public void KindIsSingleEntityFunction()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", new QueryNode[] { new ConstantNode(1) }, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), HardCodedTestModel.GetPeopleSet());
            singleEntityFunctionCall.Kind.Should().Be(QueryNodeKind.SingleEntityFunctionCall);
        }

        [TestMethod]
        public void FunctionImportsAreSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("HasDog", HardCodedTestModel.GetHasDogOverloads(), null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), ModelBuildingHelpers.BuildValidEntitySet(), null);
            singleEntityFunctionCall.Functions.Should().ContainExactly(HardCodedTestModel.GetHasDogOverloads());
        }

        [TestMethod]
        public void ParametersSetCorrectly()
        {
            SingleEntityFunctionCallNode singleEntityFunctionCall = new SingleEntityFunctionCallNode("stuff", null, null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity(), null, null);
            singleEntityFunctionCall.Parameters.Should().NotBeNull();
            singleEntityFunctionCall.Parameters.Should().BeEmpty();
        }
    }
}
