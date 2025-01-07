//---------------------------------------------------------------------
// <copyright file="SingleResourceCastNodeTests.cs" company="Microsoft">
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
    /// Unit tests for the SingleResourceCastNode class
    /// </summary>
    public class SingleResourceCastNodeTests
    {
        private readonly ResourceRangeVariableReferenceNode singleEntityNode = new ResourceRangeVariableReferenceNode("a", new ResourceRangeVariable("a", new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false), HardCodedTestModel.GetPeopleSet()));

        [Fact]
        public void StructuredTypeCannotBeNull()
        {
            Action createWithNullEntityType = () => new SingleResourceCastNode(this.singleEntityNode, null);
            Assert.Throws<ArgumentNullException>("structuredType", createWithNullEntityType);
        }

        [Fact]
        public void SourceIsSetCorrectly()
        {
            SingleResourceCastNode singleEntityCast = new SingleResourceCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            Assert.Same(this.singleEntityNode, singleEntityCast.Source);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            SingleResourceCastNode singleEntityCast = new SingleResourceCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            Assert.Equal(singleEntityCast.TypeReference.FullName(), HardCodedTestModel.GetPersonType().FullName());
        }

        [Fact]
        public void EntityTypeReferenceIsSameAsTypeReference()
        {
            SingleResourceCastNode singleEntityCast = new SingleResourceCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            Assert.Same(singleEntityCast.StructuredTypeReference, singleEntityCast.TypeReference);
        }

        [Fact]
        public void KindIsSingleResourceCastNode()
        {
            SingleResourceCastNode singleEntityCast = new SingleResourceCastNode(this.singleEntityNode, HardCodedTestModel.GetDogType());
            Assert.Equal(InternalQueryNodeKind.SingleResourceCast, singleEntityCast.InternalKind);
        }
    }
}
