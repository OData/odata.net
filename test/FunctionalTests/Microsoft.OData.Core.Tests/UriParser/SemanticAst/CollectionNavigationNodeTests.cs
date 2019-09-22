//---------------------------------------------------------------------
// <copyright file="CollectionNavigationNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class CollectionNavigationNodeTests
    {
        [Fact]
        public void SourceShouldBeSet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(source, HardCodedTestModel.GetDogMyPeopleNavProp(), new EdmPathExpression("MyPeople"));

            Assert.Same(source, node.Source);
        }

        [Fact]
        public void ItemTypeShouldBeExactlyFromPropertyType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(source, HardCodedTestModel.GetDogMyPeopleNavProp(), new EdmPathExpression("MyPeople"));

            Assert.Same(node.ItemType, HardCodedTestModel.GetDogMyPeopleNavProp().Type.AsCollection().CollectionDefinition().ElementType);
        }

        [Fact]
        public void CollectionTypeShouldBeExactlyFromPropertyType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(source, HardCodedTestModel.GetDogMyPeopleNavProp(), new EdmPathExpression("MyPeople"));

            Assert.Same(node.CollectionType, HardCodedTestModel.GetDogMyPeopleNavProp().Type.AsCollection());
        }

        [Fact]
        public void EntityItemTypeShouldBeSameAsItemType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(source, HardCodedTestModel.GetDogMyPeopleNavProp(), new EdmPathExpression("MyPeople"));

            Assert.Same(node.EntityItemType, node.ItemType);
        }

        [Fact]
        public void EntitySetShouldBeSet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();
            var node = new CollectionNavigationNode(source, HardCodedTestModel.GetDogMyPeopleNavProp(), new EdmPathExpression("MyPeople"));

            Assert.Same(node.NavigationSource, HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void CollectionNavigationNodeHandlesNullEntitySetOnParentNode()
        {
            var source = new FakeSingleEntityNode(HardCodedTestModel.GetDogTypeReference(), null);
            var collectionCastNode = new CollectionNavigationNode(source, HardCodedTestModel.GetDogMyPeopleNavProp(), new EdmPathExpression("MyPeople"));

            Assert.Null(collectionCastNode.NavigationSource);
        }

        [Fact]
        public void CollectionNavigationNodeHandlesNullSourceSetParameter()
        {
            var collectionCastNode = new CollectionNavigationNode((IEdmNavigationSource)null, HardCodedTestModel.GetDogMyPeopleNavProp(), null);

            Assert.Null(collectionCastNode.NavigationSource);
        }

        [Fact]
        public void CollectionNavigationNodeConstructorRequiresManyMultiplicity()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            Action create = () => new CollectionNavigationNode(source, HardCodedTestModel.GetPersonMyDogNavProp(), new EdmPathExpression("MyDog"));

            create.Throws<ArgumentException>(ODataErrorStrings.Nodes_CollectionNavigationNode_MustHaveManyMultiplicity);
        }

        [Fact]
        public void CollectionNavigationNodeOnSingletonShouldWork()
        {
            var collectionCastNode = new CollectionNavigationNode(HardCodedTestModel.GetBossSingleton(), HardCodedTestModel.GetPersonMyPaintingsNavProp(), new EdmPathExpression("MyPaintings"));
            Assert.Same(collectionCastNode.NavigationSource, HardCodedTestModel.GetPaintingsSet());
        }
    }
}
