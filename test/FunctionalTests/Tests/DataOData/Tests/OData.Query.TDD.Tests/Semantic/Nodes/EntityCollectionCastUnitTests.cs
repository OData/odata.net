//---------------------------------------------------------------------
// <copyright file="EntityCollectionCastUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the EntityCollectionCastNode class
    /// </summary>
    [TestClass]
    public class EntityCollectionCastUnitTests
    {
        [TestMethod]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new EntityCollectionCastNode(null, HardCodedTestModel.GetDogType());
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }

        [TestMethod]
        public void EntityTypeCannotBeNull()
        {
            EntityCollectionNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            Action createWithNullEntityType = () => new EntityCollectionCastNode(source, null);
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("entityType").ToString());
        }

        [TestMethod]
        public void SourceIsSetCorrectly()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.Source.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void ItemTypeReturnsEdmEntityTypeReference()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.ItemType.Should().BeOfType<EdmEntityTypeReference>();
        }

        [TestMethod]
        public void EntityItemTypeIsSameAsItemType()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.EntityItemType.Should().BeSameAs(entityCollectionCastNode.ItemType);
        }

        [TestMethod]
        public void EntitySetComesFromSource()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void KindIsEntityCollectionCastNode()
        {
            EntitySetNode source = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionCastNode entityCollectionCastNode = new EntityCollectionCastNode(source, HardCodedTestModel.GetDogType());
            entityCollectionCastNode.InternalKind.Should().Be(InternalQueryNodeKind.EntityCollectionCast);
        }
    }
}
