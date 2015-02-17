//---------------------------------------------------------------------
// <copyright file="CollectionPropertyAccessUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the CollectionPropertyAccessNode class
    /// </summary>
    [TestClass]
    public class CollectionPropertyAccessUnitTests
    {
        private FakeSingleEntityNode fakeDogSource = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();

        [TestMethod]
        public void SourceIsSet()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            node.Source.Should().BeSameAs(this.fakeDogSource);
        }

        [TestMethod]
        public void PropertyIsSet()
        {   
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            node.Property.Should().BeSameAs(HardCodedTestModel.GetDogNicknamesProperty());
        }

        [TestMethod]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new CollectionPropertyAccessNode(null, HardCodedTestModel.GetPersonShoeProp());
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }

        [TestMethod]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new CollectionPropertyAccessNode(new ConstantNode(1), null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [TestMethod]
        public void CollectionPropertyAccessNodesCanUseGeography()
        {
            CollectionPropertyAccessNode propertyAccessNode = new CollectionPropertyAccessNode(new ConstantNode(2), HardCodedTestModel.GetPersonGeographyCollectionProp());
            propertyAccessNode.Property.Should().Be(HardCodedTestModel.GetPersonGeographyCollectionProp());
        }

        [TestMethod]
        public void CollectionPropertyAccessNodesCanUseGeometry()
        {
            ConstantNode constant = new ConstantNode(2);
            CollectionPropertyAccessNode propertyAccessNode = new CollectionPropertyAccessNode(constant, HardCodedTestModel.GetPersonGeometryCollectionProp());
            propertyAccessNode.Property.Should().Be(HardCodedTestModel.GetPersonGeometryCollectionProp());
        }

        [TestMethod]
        public void CollectionPropertyAccessCannotTakeANavigationProperty()
        {
            Action create = () => new CollectionPropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogMyPeopleNavProp());
            create.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_PropertyAccessShouldBeNonEntityProperty("MyPeople"));
        }

        [TestMethod]
        public void CollectionPropertyAccessCannotTakeANonCollectionProperty()
        {
            Action create = () => new CollectionPropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogColorProp());
            create.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_PropertyAccessTypeMustBeCollection("Color"));
        }

        [TestMethod]
        public void CollectionPropertyAccessShouldSetItemTypeFromProperty()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            node.ItemType.Should().BeSameAs(HardCodedTestModel.GetDogNicknamesProperty().Type.AsCollection().CollectionDefinition().ElementType);
        }

        [TestMethod]
        public void CollectionPropertyAccessShouldSetCollectionTypeFromProperty()
        {
            var node = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            node.CollectionType.Should().BeSameAs(HardCodedTestModel.GetDogNicknamesProperty().Type.AsCollection());
        }
    }
}
