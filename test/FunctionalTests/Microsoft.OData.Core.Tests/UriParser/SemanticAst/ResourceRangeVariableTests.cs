//---------------------------------------------------------------------
// <copyright file="EntityRangeVariableTests.cs" company="Microsoft">
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
    /// Unit tests for the ResourceRangeVariable class
    /// </summary>
    public class ResourceRangeVariableTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new ResourceRangeVariable(null, HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Assert.Throws<ArgumentNullException>("name", createWithNullName);
        }

        [Fact]
        public void StructuredTypeCannotBeNull()
        {
            Action createWithNullEntityType = () => new ResourceRangeVariable("dogs", null, HardCodedTestModel.GetDogsSet());
            Assert.Throws<ArgumentNullException>("structuredType", createWithNullEntityType);
        }

        [Fact]
        public void CanCreateFromEntitySetOrEntityCollectionNode()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            Action createWithEntityset = () => new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Action createWithCollectionNode = () => new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            createWithEntityset.DoesNotThrow();
            createWithCollectionNode.DoesNotThrow();
        }

        [Fact]
        public void EntityCollectionNodeIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            ResourceRangeVariable entityRangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            entityRangeVariable.CollectionResourceNode.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void EntitySetIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            ResourceRangeVariable entityRangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            Assert.Same(HardCodedTestModel.GetDogsSet(), entityRangeVariable.NavigationSource);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            ResourceRangeVariable entityRangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            Assert.Equal(HardCodedTestModel.GetDogTypeReference().FullName(), entityRangeVariable.TypeReference.FullName());
        }

        [Fact]
        public void TypeReferenceReturnsEdmEntityTypeReference()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            ResourceRangeVariable entityRangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            Assert.IsType<EdmEntityTypeReference>(entityRangeVariable.TypeReference);
        }

        [Fact]
        public void EntityTypeReferenceReturnsEdmEntityTypeReference()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            ResourceRangeVariable entityRangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            Assert.IsType<EdmEntityTypeReference>(entityRangeVariable.StructuredTypeReference);
        }

        [Fact]
        public void KindShouldBeEntityRangeVariable()
        {
            EntitySetNode entitySetNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            ResourceRangeVariable entityRangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), entitySetNode);
            Assert.Equal(RangeVariableKind.Resource, entityRangeVariable.Kind);
        }
    }
}
