//---------------------------------------------------------------------
// <copyright file="CollectionPropertyAccessNodeTests.cs" company="Microsoft">
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
            node.Source.Should().BeSameAs(this.fakeDogSource);
        }

        [Fact]
        public void PropertyIsSet()
        {   
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            node.Property.Should().BeSameAs(HardCodedTestModel.GetDogNicknamesProperty());
        }

        [Fact]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new CollectionPropertyAccessNode(null, HardCodedTestModel.GetPersonShoeProp());
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }

        [Fact]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new CollectionPropertyAccessNode(new ConstantNode(1), null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [Fact]
        public void CollectionPropertyAccessNodesCanUseGeography()
        {
            CollectionPropertyAccessNode propertyAccessNode = new CollectionPropertyAccessNode(new ConstantNode(2), HardCodedTestModel.GetPersonGeographyCollectionProp());
            propertyAccessNode.Property.Should().Be(HardCodedTestModel.GetPersonGeographyCollectionProp());
        }

        [Fact]
        public void CollectionPropertyAccessNodesCanUseGeometry()
        {
            ConstantNode constant = new ConstantNode(2);
            CollectionPropertyAccessNode propertyAccessNode = new CollectionPropertyAccessNode(constant, HardCodedTestModel.GetPersonGeometryCollectionProp());
            propertyAccessNode.Property.Should().Be(HardCodedTestModel.GetPersonGeometryCollectionProp());
        }

        [Fact]
        public void CollectionPropertyAccessCannotTakeANavigationProperty()
        {
            Action create = () => new CollectionPropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogMyPeopleNavProp());
            create.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_PropertyAccessShouldBeNonEntityProperty("MyPeople"));
        }

        [Fact]
        public void CollectionPropertyAccessCannotTakeANonCollectionProperty()
        {
            Action create = () => new CollectionPropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogColorProp());
            create.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_PropertyAccessTypeMustBeCollection("Color"));
        }

        [Fact]
        public void CollectionPropertyAccessShouldSetItemTypeFromProperty()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            node.ItemType.Should().BeSameAs(HardCodedTestModel.GetDogNicknamesProperty().Type.AsCollection().CollectionDefinition().ElementType);
        }

        [Fact]
        public void CollectionPropertyAccessShouldSetCollectionTypeFromProperty()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            node.CollectionType.Should().BeSameAs(HardCodedTestModel.GetDogNicknamesProperty().Type.AsCollection());
        }
    }
}
