//---------------------------------------------------------------------
// <copyright file="CollectionNavigationNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class CollectionNavigationNodeTests
    {
        [Fact]
        public void SourceShouldBeSet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.Source.Should().BeSameAs(source);
        }

        [Fact]
        public void ItemTypeShouldBeExactlyFromPropertyType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.ItemType.Should().BeSameAs(HardCodedTestModel.GetDogMyPeopleNavProp().Type.AsCollection().CollectionDefinition().ElementType);
        }

        [Fact]
        public void CollectionTypeShouldBeExactlyFromPropertyType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.CollectionType.Should().BeSameAs(HardCodedTestModel.GetDogMyPeopleNavProp().Type.AsCollection());
        }

        [Fact]
        public void EntityItemTypeShouldBeSameAsItemType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.EntityItemType.Should().BeSameAs(node.ItemType);
        }

        [Fact]
        public void EntitySetShouldBeSet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void CollectionNavigationNodeHandlesNullEntitySetOnParentNode()
        {
            var source = new FakeSingleEntityNode(HardCodedTestModel.GetDogTypeReference(), null);
            var collectionCastNode = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            collectionCastNode.NavigationSource.Should().BeNull();
        }

        [Fact]
        public void CollectionNavigationNodeHandlesNullSourceSetParameter()
        {
            var collectionCastNode = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), (IEdmNavigationSource)null);

            collectionCastNode.NavigationSource.Should().BeNull();
        }

        [Fact]
        public void CollectionNavigationNodeConstructorRequiresManyMultiplicity()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            Action create = () => new CollectionNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            create.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveManyMultiplicity);
        }

        [Fact]
        public void CollectionNavigationNodeOnSingletonShouldWork()
        {
            var collectionCastNode = new CollectionNavigationNode(HardCodedTestModel.GetPersonMyPaintingsNavProp(), HardCodedTestModel.GetBossSingleton());
            collectionCastNode.NavigationSource.Should().Be(HardCodedTestModel.GetPaintingsSet());
        }
    }
}
