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
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Action createWithNullName = () => new ResourceRangeVariableReferenceNode(null, rangeVariable);
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void RangeVariableIsSetCorrectly()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.RangeVariable.ShouldBeResourceRangeVariable(HardCodedTestModel.GetDogTypeReference()).And.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void EntitySetComesFromRangeVariable()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void TypeReferenceComesFromRangeVariable()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
        }

        [Fact]
        public void TypeReferenceIsEdmEntityTypeReference()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.TypeReference.Should().BeOfType<EdmEntityTypeReference>();
        }

        [Fact]
        public void EntityTypeReferenceIsSameAsTypeReference()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.StructuredTypeReference.Should().BeSameAs(referenceNode.TypeReference);
        }

        [Fact]
        public void KindIsResourceRangeVariableReferenceNode()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            referenceNode.InternalKind.Should().Be(InternalQueryNodeKind.ResourceRangeVariableReference);
        }
    }
}
