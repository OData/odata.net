//---------------------------------------------------------------------
// <copyright file="CollectionResourceFunctionCallNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.Throws<ArgumentNullException>("name", createWithNullName);
        }

        [Fact]
        public void CollectionTypeReferenceCannotBeNull()
        {
            Action createWithNullType = () => new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>(){ HardCodedTestModel.GetFunctionForHasJob()}, null, null, HardCodedTestModel.GetPeopleSet(), null);
            Assert.Throws<ArgumentNullException>("returnedCollectionTypeReference", createWithNullType);
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
            Assert.Equal("stuff", entityCollectionFunctionCallNode.Name);
        }

        [Fact]
        public void ArgumentsSetCorrectly()
        {
            var entityFunctionCallNode = new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            var parameter = Assert.Single(entityFunctionCallNode.Parameters);
            parameter.ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void EntityTypeReferenceSetCorrectly()
        {
            var entityFunctionCallNode = new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            Assert.Same(entityFunctionCallNode.ItemType, this.entityTypeReference);
            Assert.Same(entityFunctionCallNode.ItemStructuredType, this.entityTypeReference);
        }

        [Fact]
        public void KindIsSingleEntityFunction()
        {
            var entityFunctionCallNode = new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, this.entityCollectionTypeReference, HardCodedTestModel.GetPeopleSet(), null);
            Assert.Equal(QueryNodeKind.CollectionResourceFunctionCall, entityFunctionCallNode.Kind);
        }

        [Fact]
        public void ItemTypeMustBeAnEntityType()
        {
            Action createWithPrimitiveCollectionType = () => new CollectionResourceFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))), HardCodedTestModel.GetPeopleSet(), null);
            createWithPrimitiveCollectionType.Throws<ArgumentException>(ODataErrorStrings.Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity);
        }
    }
}
