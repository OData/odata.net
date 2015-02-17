//---------------------------------------------------------------------
// <copyright file="SingleValuePropertyAccessUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the CollectionPropertyAccessNode class
    /// </summary>
    [TestClass]
    public class SingleValuePropertyAccessUnitTests
    {
        [TestMethod]
        public void SourceIsSet()
        {
            var source = new ConstantNode(null);
            var node = new SingleValuePropertyAccessNode(source, HardCodedTestModel.GetDogColorProp());
            node.Source.Should().BeSameAs(source);
        }

        [TestMethod]
        public void PropertyIsSet()
        {
            var node = new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogColorProp());
            node.Property.Should().BeSameAs(HardCodedTestModel.GetDogColorProp());
        }

        [TestMethod]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new SingleValuePropertyAccessNode(null, HardCodedTestModel.GetDogColorProp());
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }

        [TestMethod]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new SingleValuePropertyAccessNode(new ConstantNode(1), null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [TestMethod]
        public void SingleValuePropertyAccessNodesCanUseGeography()
        {
            ConstantNode constant = new ConstantNode(2);
            SingleValuePropertyAccessNode accessNode = new SingleValuePropertyAccessNode(constant, HardCodedTestModel.GetPersonGeographyPointProp());
            accessNode.Property.Should().Be(HardCodedTestModel.GetPersonGeographyPointProp());
        }

        [TestMethod]
        public void SingleValuePropertyAccessNodesCanUseGeometry()
        {
            ConstantNode constant = new ConstantNode(2);
            SingleValuePropertyAccessNode accessNode = new SingleValuePropertyAccessNode(constant, HardCodedTestModel.GetPersonGeometryPointProp());
            accessNode.Property.Should().Be(HardCodedTestModel.GetPersonGeometryPointProp());
        }

        [TestMethod]
        public void SingleValuePropertyAccessCannotTakeANavigationProperty()
        {
            Action create = () => new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetPersonMyDogNavProp());
            create.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_PropertyAccessShouldBeNonEntityProperty("MyDog"));
        }

        [TestMethod]
        public void SingleValuePropertyAccessCannotTakeACollectionProperty()
        {
            Action create = () => new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogNicknamesProperty());
            create.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_PropertyAccessTypeShouldNotBeCollection("Nicknames"));
        }
    }
}
