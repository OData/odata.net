//---------------------------------------------------------------------
// <copyright file="EntityCollectionFunctionCallUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

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
    /// Unit tests for the EntityCollectionFunctionCallNode class
    /// </summary>
    [TestClass]
    public class EntityCollectionFunctionCallNodeUnitTests
    {
        private readonly IEdmEntityTypeReference entityTypeReference;
        private readonly IEdmCollectionTypeReference entityCollectionTypeReference;

        public EntityCollectionFunctionCallNodeUnitTests()
        {
            entityTypeReference = ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity();
            entityCollectionTypeReference = new EdmCollectionType(entityTypeReference).ToTypeReference().AsCollection();
        }

        [TestMethod]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new EntityCollectionFunctionCallNode(null, null, new QueryNode[] { }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            createWithNullName.ShouldThrow<ArgumentNullException>().WithMessage("name", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void CollectionTypeReferenceCannotBeNull()
        {
            Action createWithNullType = () => new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>(){ HardCodedTestModel.GetFunctionForHasJob()}, null, null, HardCodedTestModel.GetPeopleSet(), null);
            createWithNullType.ShouldThrow<ArgumentNullException>().WithMessage("collectionTypeReference", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void FunctionImportsAreSetCorrectly()
        {
            var entityCollectionFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, null, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityCollectionFunctionCallNode.ShouldBeEntityCollectionFunctionCallNode(HardCodedTestModel.GetFunctionForHasJob());
        }

        [TestMethod]
        public void NameIsSetCorrectly()
        {
            var entityCollectionFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, null, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityCollectionFunctionCallNode.Name.Should().Be("stuff");
        }

        [TestMethod]
        public void ArgumentsSetCorrectly()
        {
            var entityFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.Parameters.Should().HaveCount(1);
            entityFunctionCallNode.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [TestMethod]
        public void EntityTypeReferenceSetCorrectly()
        {
            var entityFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.ItemType.Should().BeSameAs(this.entityTypeReference);
            entityFunctionCallNode.EntityItemType.Should().BeSameAs(this.entityTypeReference);
        }

        [TestMethod]
        public void KindIsSingleEntityFunction()
        {
            var entityFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.Kind.Should().Be(QueryNodeKind.EntityCollectionFunctionCall);
        }

        [TestMethod]
        public void ItemTypeMustBeAnEntityType()
        {
            Action createWithPrimitiveCollectionType = () => new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))), HardCodedTestModel.GetPeopleSet(), null);
            createWithPrimitiveCollectionType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity);
        }
    }
}
