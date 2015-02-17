//---------------------------------------------------------------------
// <copyright file="CollectionNavigationNodeUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Nodes
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CollectionNavigationNodeUnitTests
    {
        [TestMethod]
        public void SourceShouldBeSet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.Source.Should().BeSameAs(source);
        }

        [TestMethod]
        public void ItemTypeShouldBeExactlyFromPropertyType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.ItemType.Should().BeSameAs(HardCodedTestModel.GetDogMyPeopleNavProp().Type.AsCollection().CollectionDefinition().ElementType);
        }

        [TestMethod]
        public void CollectionTypeShouldBeExactlyFromPropertyType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.CollectionType.Should().BeSameAs(HardCodedTestModel.GetDogMyPeopleNavProp().Type.AsCollection());
        }

        [TestMethod]
        public void EntityItemTypeShouldBeSameAsItemType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.EntityItemType.Should().BeSameAs(node.ItemType);
        }

        [TestMethod]
        public void EntitySetShouldBeSet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            node.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void CollectionNavigationNodeHandlesNullEntitySetOnParentNode()
        {
            var source = new FakeSingleEntityNode(HardCodedTestModel.GetDogTypeReference(), null);
            var collectionCastNode = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), source);

            collectionCastNode.NavigationSource.Should().BeNull();
        }

        [TestMethod]
        public void CollectionNavigationNodeHandlesNullSourceSetParameter()
        {
            var collectionCastNode = new CollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp(), (IEdmNavigationSource)null);

            collectionCastNode.NavigationSource.Should().BeNull();
        }

        [TestMethod]
        public void CollectionNavigationNodeConstructorRequiresManyMultiplicity()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            Action create = () => new CollectionNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            create.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveManyMultiplicity);
        }

        [TestMethod]
        public void CollectionNavigationNodeOnSingletonShouldWork()
        {
            var collectionCastNode = new CollectionNavigationNode(HardCodedTestModel.GetPersonMyPaintingsNavProp(), HardCodedTestModel.GetBossSingleton());
            collectionCastNode.NavigationSource.Should().Be(HardCodedTestModel.GetPaintingsSet());
        }
    }
}
