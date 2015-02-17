//---------------------------------------------------------------------
// <copyright file="EntityRangeVariableUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the EntityRangeVariable class
    /// </summary>
    [TestClass]
    public class EntityRangeVariableUnitTests
    {
        [TestMethod]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new EntityRangeVariable(null, HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [TestMethod]
        public void EntityTypeCannotBeNull()
        {
            Action createWithNullEntityType = () => new EntityRangeVariable("dogs", null, HardCodedTestModel.GetDogsSet());
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("entityType").ToString());
        }

        [TestMethod]
        public void CanCreateFromEntitySetOrEntityCollectionNode()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            Action createWithEntityset = () => new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Action createWithCollectionNode = () => new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            createWithEntityset.ShouldNotThrow();
            createWithCollectionNode.ShouldNotThrow();
        }

        [TestMethod]
        public void EntityCollectionNodeIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.EntityCollectionNode.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void EntitySetIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void TypeReferenceIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
        }

        [TestMethod]
        public void TypeReferenceReturnsEdmEntityTypeReference()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.TypeReference.Should().BeOfType<EdmEntityTypeReference>();
        }

        [TestMethod]
        public void EntityTypeReferenceReturnsEdmEntityTypeReference()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.EntityTypeReference.Should().BeOfType<EdmEntityTypeReference>();
        }

        [TestMethod]
        public void KindShouldBeEntityRangeVariable()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.Kind.Should().Be(RangeVariableKind.Entity);
        }
    }
}
