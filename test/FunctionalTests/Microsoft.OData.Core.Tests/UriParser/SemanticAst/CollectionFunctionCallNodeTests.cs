//---------------------------------------------------------------------
// <copyright file="CollectionFunctionCallNodeTests.cs" company="Microsoft">
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
    /// Unit tests for the CollectionFunctionCallNode class
    /// </summary>
    public class CollectionFunctionCallNodeTests
    {
        private readonly IEdmTypeReference itemTypeReference;
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        public CollectionFunctionCallNodeTests()
        {
            itemTypeReference = EdmCoreModel.Instance.GetString(true);
            collectionTypeReference = new EdmCollectionType(itemTypeReference).ToTypeReference().AsCollection();
        }

        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new CollectionFunctionCallNode(null, null, new QueryNode[] { }, collectionTypeReference, null);
            Assert.Throws<ArgumentNullException>("name", createWithNullName);
        }

        [Fact]
        public void CollectionTypeReferenceCannotBeNull()
        {
            Action createWithNullType = () => new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { }, null, null);
            Assert.Throws<ArgumentNullException>("returnedCollectionType", createWithNullType);
        }

        [Fact]
        public void FunctionImportsAreSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", HardCodedTestModel.GetHasDogOverloads(), null, collectionTypeReference, null);
            collectionFunctionCallNode.Functions.ContainExactly(HardCodedTestModel.GetHasDogOverloads());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", HardCodedTestModel.GetHasDogOverloads(), null, collectionTypeReference, null);
            Assert.Equal("stuff", collectionFunctionCallNode.Name);
        }

        [Fact]
        public void ArgumentsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            var parameter = Assert.Single(collectionFunctionCallNode.Parameters);
            parameter.ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void ItemTypeSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            Assert.Same(this.itemTypeReference, collectionFunctionCallNode.ItemType);
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            Assert.Equal(QueryNodeKind.CollectionFunctionCall, collectionFunctionCallNode.Kind);
        }

        [Fact]
        public void ItemTypeMustBePrimitiveOrComplex()
        {
            Action createWithEntityCollectionType = () => new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, new EdmCollectionTypeReference(new EdmCollectionType(ModelBuildingHelpers.BuildValidEntityType().ToTypeReference())), null);
            createWithEntityCollectionType.Throws<ArgumentException>(ODataErrorStrings.Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }
    }
}
