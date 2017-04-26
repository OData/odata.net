//---------------------------------------------------------------------
// <copyright file="CollectionResourceFunctionCallNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
    /// Unit tests for the CollectionResourceFunctionCallNode class
    /// </summary>
    public class CollectionResourceFunctionCallNodeTests
    {
        private readonly IEdmEntityTypeReference entityTypeReference;
        private readonly IEdmCollectionTypeReference entityCollectionTypeReference;

        public CollectionResourceFunctionCallNodeTests()
        {
            entityTypeReference = ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity();
            entityCollectionTypeReference = new EdmCollectionType(entityTypeReference).ToTypeReference().AsCollection();
        }

        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new CollectionResourceFunctionCallNode(null, null, new QueryNode[] { }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            createWithNullName.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("name"));
        }

        [Fact]
        public void CollectionTypeReferenceCannotBeNull()
        {
            Action createWithNullType = () => new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>(){ HardCodedTestModel.GetFunctionForHasJob()}, null, null, HardCodedTestModel.GetPeopleSet(), null);
            createWithNullType.ShouldThrow<ArgumentNullException>().Where(e => e.Message.IndexOf("collectionTypeReference", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        [Fact]
        public void FunctionImportsAreSetCorrectly()
        {
            var entityCollectionFunctionCallNode = new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, null, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityCollectionFunctionCallNode.ShouldBeCollectionResourceFunctionCallNode(HardCodedTestModel.GetFunctionForHasJob());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            var entityCollectionFunctionCallNode = new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, null, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityCollectionFunctionCallNode.Name.Should().Be("stuff");
        }

        [Fact]
        public void ArgumentsSetCorrectly()
        {
            var entityFunctionCallNode = new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.Parameters.Should().HaveCount(1);
            entityFunctionCallNode.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void EntityTypeReferenceSetCorrectly()
        {
            var entityFunctionCallNode = new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.ItemType.Should().BeSameAs(this.entityTypeReference);
            entityFunctionCallNode.ItemStructuredType.Should().BeSameAs(this.entityTypeReference);
        }

        [Fact]
        public void KindIsSingleEntityFunction()
        {
            var entityFunctionCallNode = new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            entityFunctionCallNode.Kind.Should().Be(QueryNodeKind.CollectionResourceFunctionCall);
        }

        [Fact]
        public void ItemTypeMustBeAnEntityType()
        {
            Action createWithPrimitiveCollectionType = () => new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))), HardCodedTestModel.GetPeopleSet(), null);
            createWithPrimitiveCollectionType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity);
        }
    }
}
