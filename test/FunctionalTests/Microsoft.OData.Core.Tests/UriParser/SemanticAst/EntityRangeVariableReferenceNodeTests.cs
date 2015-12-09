//---------------------------------------------------------------------
// <copyright file="EntityRangeVariableReferenceNodeTests.cs" company="Microsoft">
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

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the EntityRangeVariableReferenceNode class
    /// </summary>
    public class EntityRangeVariableReferenceNodeTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Action createWithNullName = () => new EntityRangeVariableReferenceNode(null, rangeVariable);
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void RangeVariableIsSetCorrectly()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            EntityRangeVariableReferenceNode referenceNode = new EntityRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.RangeVariable.ShouldBeEntityRangeVariable(HardCodedTestModel.GetDogTypeReference()).And.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void EntitySetComesFromRangeVariable()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            EntityRangeVariableReferenceNode referenceNode = new EntityRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void TypeReferenceComesFromRangeVariable()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            EntityRangeVariableReferenceNode referenceNode = new EntityRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
        }

        [Fact]
        public void TypeReferenceIsEdmEntityTypeReference()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            EntityRangeVariableReferenceNode referenceNode = new EntityRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.TypeReference.Should().BeOfType<EdmEntityTypeReference>();
        }

        [Fact]
        public void EntityTypeReferenceIsSameAsTypeReference()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            EntityRangeVariableReferenceNode referenceNode = new EntityRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.EntityTypeReference.Should().BeSameAs(referenceNode.TypeReference);
        }

        [Fact]
        public void KindIsEntityRangeVariableReferenceNode()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            EntityRangeVariableReferenceNode referenceNode = new EntityRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.InternalKind.Should().Be(InternalQueryNodeKind.EntityRangeVariableReference);
        }
    }
}
