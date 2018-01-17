//---------------------------------------------------------------------
// <copyright file="SingleResourceCastNodeTests.cs" company="Microsoft">
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
    /// Unit tests for the SingleResourceCastNode class
    /// </summary>
    public class SingleResourceCastNodeTests
    {
        private readonly ResourceRangeVariableReferenceNode singleEntityNode = new ResourceRangeVariableReferenceNode("a", new ResourceRangeVariable("a", new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false), HardCodedTestModel.GetPeopleSet()));

        [Fact]
        public void EntityTypeCannotBeNull()
        {
            Action createWithNullEntityType = () => new SingleResourceCastNode(this.singleEntityNode, null);
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void SourceIsSetCorrectly()
        {
            SingleResourceCastNode singleEntityCast = new SingleResourceCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.Source.Should().Be(this.singleEntityNode);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            SingleResourceCastNode singleEntityCast = new SingleResourceCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.TypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonType().FullName());
        }

        [Fact]
        public void EntityTypeReferenceIsSameAsTypeReference()
        {
            SingleResourceCastNode singleEntityCast = new SingleResourceCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.StructuredTypeReference.Should().BeSameAs(singleEntityCast.TypeReference);
        }

        [Fact]
        public void KindIsSingleResourceCastNode()
        {
            SingleResourceCastNode singleEntityCast = new SingleResourceCastNode(this.singleEntityNode, HardCodedTestModel.GetDogType());
            singleEntityCast.InternalKind.Should().Be(InternalQueryNodeKind.SingleResourceCast);
        }
    }
}
