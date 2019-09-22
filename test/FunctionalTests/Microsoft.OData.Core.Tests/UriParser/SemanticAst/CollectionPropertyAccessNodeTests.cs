//---------------------------------------------------------------------
// <copyright file="CollectionPropertyAccessNodeTests.cs" company="Microsoft">
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
    /// Unit tests for the CollectionPropertyAccessNode class
    /// </summary>
    public class CollectionPropertyAccessNodeTests
    {
        private FakeSingleEntityNode fakeDogSource = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();

        [Fact]
        public void SourceIsSet()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            Assert.Same(node.Source, this.fakeDogSource);
        }

        [Fact]
        public void PropertyIsSet()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            Assert.Same(node.Property, HardCodedTestModel.GetDogNicknamesProperty());
        }

        [Fact]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new CollectionPropertyAccessNode(null, HardCodedTestModel.GetPersonShoeProp());
            Assert.Throws<ArgumentNullException>("source", createWithNullSource);
        }

        [Fact]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new CollectionPropertyAccessNode(new ConstantNode(1), null);
            Assert.Throws<ArgumentNullException>("property", createWithNullProperty);
        }

        [Fact]
        public void CollectionPropertyAccessNodesCanUseGeography()
        {
            CollectionPropertyAccessNode propertyAccessNode = new CollectionPropertyAccessNode(new ConstantNode(2), HardCodedTestModel.GetPersonGeographyCollectionProp());
            Assert.Same(propertyAccessNode.Property, HardCodedTestModel.GetPersonGeographyCollectionProp());
        }

        [Fact]
        public void CollectionPropertyAccessNodesCanUseGeometry()
        {
            ConstantNode constant = new ConstantNode(2);
            CollectionPropertyAccessNode propertyAccessNode = new CollectionPropertyAccessNode(constant, HardCodedTestModel.GetPersonGeometryCollectionProp());
            Assert.Same(propertyAccessNode.Property, HardCodedTestModel.GetPersonGeometryCollectionProp());
        }

        [Fact]
        public void CollectionPropertyAccessCannotTakeANavigationProperty()
        {
            Action create = () => new CollectionPropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogMyPeopleNavProp());
            create.Throws<ArgumentException>(Strings.Nodes_PropertyAccessShouldBeNonEntityProperty("MyPeople"));
        }

        [Fact]
        public void CollectionPropertyAccessCannotTakeANonCollectionProperty()
        {
            Action create = () => new CollectionPropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogColorProp());
            create.Throws<ArgumentException>(Strings.Nodes_PropertyAccessTypeMustBeCollection("Color"));
        }

        [Fact]
        public void CollectionPropertyAccessShouldSetItemTypeFromProperty()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            Assert.Same(node.ItemType, HardCodedTestModel.GetDogNicknamesProperty().Type.AsCollection().CollectionDefinition().ElementType);
        }

        [Fact]
        public void CollectionPropertyAccessShouldSetCollectionTypeFromProperty()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            Assert.Same(node.CollectionType, HardCodedTestModel.GetDogNicknamesProperty().Type.AsCollection());
        }
    }
}
