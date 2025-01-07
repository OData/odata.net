//---------------------------------------------------------------------
// <copyright file="ResourceRangeVariableReferenceNodeTests.cs" company="Microsoft">
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
    /// Unit tests for the ResourceRangeVariableReferenceNode class
    /// </summary>
    public class ResourceRangeVariableReferenceNodeTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Action createWithNullName = () => new ResourceRangeVariableReferenceNode(null, rangeVariable);
            Assert.Throws<ArgumentNullException>("name", createWithNullName);
        }

        [Fact]
        public void RangeVariableIsSetCorrectly()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            Assert.Same(referenceNode.RangeVariable.ShouldBeResourceRangeVariable(HardCodedTestModel.GetDogTypeReference()).NavigationSource, HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void EntitySetComesFromRangeVariable()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            Assert.Same(HardCodedTestModel.GetDogsSet(), referenceNode.NavigationSource);
        }

        [Fact]
        public void TypeReferenceComesFromRangeVariable()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            Assert.Equal(HardCodedTestModel.GetDogTypeReference().FullName(), referenceNode.TypeReference.FullName());
        }

        [Fact]
        public void TypeReferenceIsEdmEntityTypeReference()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            Assert.IsType<EdmEntityTypeReference>(referenceNode.TypeReference);
        }

        [Fact]
        public void EntityTypeReferenceIsSameAsTypeReference()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            Assert.Same(referenceNode.TypeReference, referenceNode.StructuredTypeReference);
        }

        [Fact]
        public void KindIsResourceRangeVariableReferenceNode()
        {
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("dogs", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            ResourceRangeVariableReferenceNode referenceNode = new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable);
            Assert.Equal(InternalQueryNodeKind.ResourceRangeVariableReference, referenceNode.InternalKind);
        }
    }
}
