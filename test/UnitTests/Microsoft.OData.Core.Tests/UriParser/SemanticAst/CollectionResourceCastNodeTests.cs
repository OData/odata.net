//---------------------------------------------------------------------
// <copyright file="EntityCollectionCastNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

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
            Assert.Same(node.ItemType.Definition, HardCodedTestModel.GetEmployeeType());
            Assert.False(node.ItemType.IsNullable);
        }

        [Fact]
        public void CollectionTypeShouldBeCreatedFromItemType()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            Assert.Equal("Collection(Fully.Qualified.Namespace.Employee)", node.CollectionType.FullName());
        }

        [Fact]
        public void CollectionTypeShouldBeSaved()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            Assert.Same(node.CollectionType, node.CollectionType);
        }

        [Fact]
        public void ItemTypeShouldBeSaved()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            Assert.Same(node.ItemType, node.ItemType);
        }

        [Fact]
        public void EntityItemTypeShouldBeSameAsItemType()
        {
            var node = new CollectionResourceCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            Assert.Same(node.ItemStructuredType, node.ItemType);
        }

        [Fact]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new CollectionResourceCastNode(null, HardCodedTestModel.GetDogType());
            Assert.Throws<ArgumentNullException>("source", createWithNullSource);
        }

        [Fact]
        public void StructuredTypeCannotBeNull()
        {
            CollectionResourceNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            Action createWithNullEntityType = () => new CollectionResourceCastNode(source, null);
            Assert.Throws<ArgumentNullException>("structuredType", createWithNullEntityType);
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
            Assert.IsType<EdmEntityTypeReference>(collectionResourceCastNode.ItemType);
        }

        [Fact]
        public void EntityItemTypeIsSameAsItemType()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceCastNode collectionResourceCastNode = new CollectionResourceCastNode(source, HardCodedTestModel.GetDogType());
            Assert.Same(collectionResourceCastNode.ItemStructuredType, collectionResourceCastNode.ItemType);
        }

        [Fact]
        public void EntitySetComesFromSource()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceCastNode collectionResourceCastNode = new CollectionResourceCastNode(source, HardCodedTestModel.GetDogType());
            Assert.Same(collectionResourceCastNode.NavigationSource, HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void KindIsCollectionResourceCastNode()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceCastNode collectionResourceCastNode = new CollectionResourceCastNode(source, HardCodedTestModel.GetDogType());
            Assert.Equal(InternalQueryNodeKind.CollectionResourceCast, collectionResourceCastNode.InternalKind);
        }
    }
}
