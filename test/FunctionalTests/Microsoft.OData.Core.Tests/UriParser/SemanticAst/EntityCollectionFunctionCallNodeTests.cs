//---------------------------------------------------------------------
// <copyright file="EntityCollectionFunctionCallNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the EntityCollectionFunctionCallNode class
    /// </summary>
    public class EntityCollectionFunctionCallNodeTests
    {
        private readonly IEdmEntityTypeReference entityTypeReference;
        private readonly IEdmCollectionTypeReference entityCollectionTypeReference;

        public EntityCollectionFunctionCallNodeTests()
        {
            entityTypeReference = ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity();
            entityCollectionTypeReference = new EdmCollectionType(entityTypeReference).ToTypeReference().AsCollection();
        }

        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new EntityCollectionFunctionCallNode(null, null, new QueryNode[] { }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            createWithNullName.ShouldThrow<ArgumentNullException>().WithMessage("name", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void CollectionTypeReferenceCannotBeNull()
        {
            Action createWithNullType = () => new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>(){ HardCodedTestModel.GetFunctionForHasJob()}, null, null, HardCodedTestModel.GetPeopleSet(), null);
            createWithNullType.ShouldThrow<ArgumentNullException>().WithMessage("collectionTypeReference", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void FunctionImportsAreSetCorrectly()
        {
            var entityCollectionFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, null, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityCollectionFunctionCallNode.ShouldBeEntityCollectionFunctionCallNode(HardCodedTestModel.GetFunctionForHasJob());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            var entityCollectionFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, null, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityCollectionFunctionCallNode.Name.Should().Be("stuff");
        }

        [Fact]
        public void ArgumentsSetCorrectly()
        {
            var entityFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.Parameters.Should().HaveCount(1);
            entityFunctionCallNode.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void EntityTypeReferenceSetCorrectly()
        {
            var entityFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.ItemType.Should().BeSameAs(this.entityTypeReference);
            entityFunctionCallNode.EntityItemType.Should().BeSameAs(this.entityTypeReference);
        }

        [Fact]
        public void KindIsSingleEntityFunction()
        {
            var entityFunctionCallNode = new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.Kind.Should().Be(QueryNodeKind.EntityCollectionFunctionCall);
        }

        [Fact]
        public void ItemTypeMustBeAnEntityType()
        {
            Action createWithPrimitiveCollectionType = () => new EntityCollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))), HardCodedTestModel.GetPeopleSet(), null);
            createWithPrimitiveCollectionType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity);
        }
    }
}
