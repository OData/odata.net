//---------------------------------------------------------------------
// <copyright file="CollectionFunctionCallNodeTests.cs" company="Microsoft">
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
            createWithNullName.ShouldThrow<ArgumentNullException>().WithMessage("name", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void CollectionTypeReferenceCannotBeNull()
        {
            Action createWithNullType = () => new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { }, null, null);
            createWithNullType.ShouldThrow<ArgumentNullException>().WithMessage("collectionType", ComparisonMode.EquivalentSubstring);
        }

        [Fact]
        public void FunctionImportsAreSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", HardCodedTestModel.GetHasDogOverloads(), null, collectionTypeReference, null);
            collectionFunctionCallNode.Functions.Should().ContainExactly(HardCodedTestModel.GetHasDogOverloads());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", HardCodedTestModel.GetHasDogOverloads(), null, collectionTypeReference, null);
            collectionFunctionCallNode.Name.Should().Be("stuff");
        }

        [Fact]
        public void ArgumentsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            collectionFunctionCallNode.Parameters.Should().HaveCount(1);
            collectionFunctionCallNode.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [Fact]
        public void ItemTypeSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            collectionFunctionCallNode.ItemType.Should().BeSameAs(this.itemTypeReference);
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            collectionFunctionCallNode.Kind.Should().Be(QueryNodeKind.CollectionFunctionCall);
        }

        [Fact]
        public void ItemTypeMustBePrimitiveOrComplex()
        {
            Action createWithEntityCollectionType = () => new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, new EdmCollectionTypeReference(new EdmCollectionType(ModelBuildingHelpers.BuildValidEntityType().ToTypeReference())), null);
            createWithEntityCollectionType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }
    }
}
