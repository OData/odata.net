//---------------------------------------------------------------------
// <copyright file="ResourceRangeVariableReferenceNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the ResourceRangeVariableReferenceNode class
    /// </summary>
    public class ResourceRangeVariableReferenceNodeTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Action createWithNullName = () => new ResourceRangeVariableReferenceNode(null, rangeVariable);
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void RangeVariableIsSetCorrectly()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.RangeVariable.ShouldBeEntityRangeVariable(HardCodedTestModel.GetDogTypeReference()).And.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void EntitySetComesFromRangeVariable()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void TypeReferenceComesFromRangeVariable()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
        }

        [Fact]
        public void TypeReferenceIsEdmEntityTypeReference()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.TypeReference.Should().BeOfType<EdmEntityTypeReference>();
        }

        [Fact]
        public void EntityTypeReferenceIsSameAsTypeReference()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.StructuredTypeReference.Should().BeSameAs(referenceNode.TypeReference);
        }

        [Fact]
        public void KindIsResourceRangeVariableReferenceNode()
        {
            EntityRangeVariable rangeVariable = new EntityRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.InternalKind.Should().Be(InternalQueryNodeKind.ResourceRangeVariableReference);
        }
    }
}
