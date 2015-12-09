//---------------------------------------------------------------------
// <copyright file="EntityRangeVariableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the EntityRangeVariable class
    /// </summary>
    public class EntityRangeVariableTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new EntityRangeVariable(null, HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void EntityTypeCannotBeNull()
        {
            Action createWithNullEntityType = () => new EntityRangeVariable("dogs", null, HardCodedTestModel.GetDogsSet());
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("entityType").ToString());
        }

        [Fact]
        public void CanCreateFromEntitySetOrEntityCollectionNode()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            Action createWithEntityset = () => new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Action createWithCollectionNode = () => new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            createWithEntityset.ShouldNotThrow();
            createWithCollectionNode.ShouldNotThrow();
        }

        [Fact]
        public void EntityCollectionNodeIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.EntityCollectionNode.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void EntitySetIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
        }

        [Fact]
        public void TypeReferenceReturnsEdmEntityTypeReference()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.TypeReference.Should().BeOfType<EdmEntityTypeReference>();
        }

        [Fact]
        public void EntityTypeReferenceReturnsEdmEntityTypeReference()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.EntityTypeReference.Should().BeOfType<EdmEntityTypeReference>();
        }

        [Fact]
        public void KindShouldBeEntityRangeVariable()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable entityRangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.Kind.Should().Be(RangeVariableKind.Entity);
        }
    }
}
