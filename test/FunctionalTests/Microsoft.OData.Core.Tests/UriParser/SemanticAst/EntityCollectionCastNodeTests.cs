//---------------------------------------------------------------------
// <copyright file="EntityCollectionCastNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// TODO: test the rest of EntityCollectionCastNode
    /// </summary>
    public class EntityCollectionCastNodeTests
    {
        private readonly EntityCollectionNode fakeSource = new EntitySetNode(HardCodedTestModel.GetPeopleSet());

        [Fact]
        public void ItemTypeReferenceShouldBeCreatedFromConstructorParameter()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.ItemType.Definition.Should().BeSameAs(HardCodedTestModel.GetEmployeeType());
            node.ItemType.IsNullable.Should().BeFalse();
        }

        [Fact]
        public void CollectionTypeShouldBeCreatedFromItemType()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.CollectionType.FullName().Should().Be("Collection(Fully.Qualified.Namespace.Employee)");
        }

        [Fact]
        public void CollectionTypeShouldBeSaved()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.CollectionType.Should().BeSameAs(node.CollectionType);
        }

        [Fact]
        public void ItemTypeShouldBeSaved()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.ItemType.Should().BeSameAs(node.ItemType);
        }

        [Fact]
        public void EntityItemTypeShouldBeSameAsItemType()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.EntityItemType.Should().BeSameAs(node.ItemType);
        }

        [Fact]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new EntityCollectionCastNode(null, HardCodedTestModel.GetDogType());
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }

        [Fact]
        public void EntityTypeCannotBeNull()
        {
            EntityCollectionNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            Action createWithNullEntityType = () => new EntityCollectionCastNode(source, null);
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("entityType").ToString());
        }

        [Fact]
        public void SourceIsSetCorrectly()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.Source.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void ItemTypeReturnsEdmEntityTypeReference()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.ItemType.Should().BeOfType<EdmEntityTypeReference>();
        }

        [Fact]
        public void EntityItemTypeIsSameAsItemType()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.EntityItemType.Should().BeSameAs(entityCollectionCastNode.ItemType);
        }

        [Fact]
        public void EntitySetComesFromSource()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void KindIsEntityCollectionCastNode()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.InternalKind.Should().Be(InternalQueryNodeKind.EntityCollectionCast);
        }
    }
}
