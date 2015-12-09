//---------------------------------------------------------------------
// <copyright file="SingleEntityCastNodeTests.cs" company="Microsoft">
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
    /// Unit tests for the SingleEntityCastNode class
    /// </summary>
    public class SingleEntityCastNodeTests
    {
        private readonly EntityRangeVariableReferenceNode singleEntityNode = new EntityRangeVariableReferenceNode("a", new EntityRangeVariable("a", new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false), HardCodedTestModel.GetPeopleSet()));

        [Fact]
        public void EntityTypeCannotBeNull()
        {
            Action createWithNullEntityType = () => new SingleEntityCastNode(this.singleEntityNode, null);
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void SourceIsSetCorrectly()
        {
            SingleEntityCastNode singleEntityCast = new SingleEntityCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.Source.Should().Be(this.singleEntityNode);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            SingleEntityCastNode singleEntityCast = new SingleEntityCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.TypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonType().FullName());
        }

        [Fact]
        public void EntityTypeReferenceIsSameAsTypeReference()
        {
            SingleEntityCastNode singleEntityCast = new SingleEntityCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.EntityTypeReference.Should().BeSameAs(singleEntityCast.TypeReference);
        }

        [Fact]
        public void KindIsSingleEntityCastNode()
        {
            SingleEntityCastNode singleEntityCast = new SingleEntityCastNode(this.singleEntityNode, HardCodedTestModel.GetDogType());
            singleEntityCast.InternalKind.Should().Be(InternalQueryNodeKind.SingleEntityCast);
        }
    }
}
