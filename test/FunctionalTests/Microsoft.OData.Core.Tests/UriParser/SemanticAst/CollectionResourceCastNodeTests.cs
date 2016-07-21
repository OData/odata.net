//---------------------------------------------------------------------
// <copyright file="EntityCollectionCastNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// TODO: test the rest of CollectionResourceCastNode
    /// </summary>
    public class CollectionResourceCastNodeTests
    {
        private readonly CollectionResourceNode fakeSource = new EntitySetNode(HardCodedTestModel.GetPeopleSet());

        [Fact]
        public void ItemTypeReferenceShouldBeCreatedFromConstructorParameter()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.ItemType.Definition.Should().BeSameAs(HardCodedTestModel.GetEmployeeType());
            node.ItemType.IsNullable.Should().BeFalse();
        }

        [Fact]
        public void CollectionTypeShouldBeCreatedFromItemType()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.CollectionType.FullName().Should().Be("Collection(Fully.Qualified.Namespace.Employee)");
        }

        [Fact]
        public void CollectionTypeShouldBeSaved()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.CollectionType.Should().BeSameAs(node.CollectionType);
        }

        [Fact]
        public void ItemTypeShouldBeSaved()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.ItemType.Should().BeSameAs(node.ItemType);
        }

        [Fact]
        public void EntityItemTypeShouldBeSameAsItemType()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.ItemStructuredType.Should().BeSameAs(node.ItemType);
        }

        [Fact]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new CollectionResourceCastNode(null, HardCodedTestModel.GetDogType());
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }

        [Fact]
        public void EntityTypeCannotBeNull()
        {
            CollectionResourceNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            Action createWithNullEntityType = () => new CollectionResourceCastNode(source, null);
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("entityType").ToString());
        }

        [Fact]
        public void SourceIsSetCorrectly()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceCastNode collectionResourceCastNode = new CollectionResourceCastNode(source, HardCodedTestModel.GetDogType());
            collectionResourceCastNode.Source.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void ItemTypeReturnsEdmEntityTypeReference()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceCastNode collectionResourceCastNode = new CollectionResourceCastNode(source, HardCodedTestModel.GetDogType());
            collectionResourceCastNode.ItemType.Should().BeOfType<EdmEntityTypeReference>();
        }

        [Fact]
        public void EntityItemTypeIsSameAsItemType()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceCastNode collectionResourceCastNode = new CollectionResourceCastNode(source, HardCodedTestModel.GetDogType());
            collectionResourceCastNode.ItemStructuredType.Should().BeSameAs(collectionResourceCastNode.ItemType);
        }

        [Fact]
        public void EntitySetComesFromSource()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceCastNode collectionResourceCastNode = new CollectionResourceCastNode(source, HardCodedTestModel.GetDogType());
            collectionResourceCastNode.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void KindIsCollectionResourceCastNode()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceCastNode collectionResourceCastNode = new CollectionResourceCastNode(source, HardCodedTestModel.GetDogType());
            collectionResourceCastNode.InternalKind.Should().Be(InternalQueryNodeKind.CollectionResourceCast);
        }
    }
}
