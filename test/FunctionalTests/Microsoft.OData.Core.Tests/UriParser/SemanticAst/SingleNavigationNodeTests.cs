//---------------------------------------------------------------------
// <copyright file="SingleNavigationNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class SingleNavigationNodeTests
    {
        [Fact]
        public void SourceIsSet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.Source.Should().BeSameAs(source);
        }

        [Fact]
        public void TypeReferenceIsExactlyFromProperty()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.TypeReference.Should().BeSameAs(HardCodedTestModel.GetPersonMyDogNavProp().Type);
        }

        [Fact]
        public void EntityTypeIsSameAsType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.EntityTypeReference.Should().BeSameAs(node.TypeReference);
        }

        [Fact]
        public void EntitySetIsCalculatedCorrectly()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void SingleNavigationNodeHandlesNullEntitySetOnParentNode()
        {
            var source = new FakeSingleEntityNode(HardCodedTestModel.GetPersonTypeReference(), null);
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.NavigationSource.Should().BeNull();
        }

        [Fact]
        public void SingleNavigationNodeHandlesNullSourceSetParameter()
        {
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), (IEdmEntitySet)null);
            node.NavigationSource.Should().BeNull();
        }

        [Fact]
        public void KindIsEntitySet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.InternalKind.Should().Be(InternalQueryNodeKind.SingleNavigationNode);
        }
    }
}
