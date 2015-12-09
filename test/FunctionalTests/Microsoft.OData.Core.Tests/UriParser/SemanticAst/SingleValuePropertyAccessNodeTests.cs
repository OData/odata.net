//---------------------------------------------------------------------
// <copyright file="SingleValuePropertyAccessNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the SingleValuePropertyAccessNode class
    /// </summary>
    public class SingleValuePropertyAccessNodeTests
    {
        [Fact]
        public void SourceIsSet()
        {
            var source = new ConstantNode(null);
            var node = new SingleValuePropertyAccessNode(source, HardCodedTestModel.GetDogColorProp());
            node.Source.Should().BeSameAs(source);
        }

        [Fact]
        public void PropertyIsSet()
        {
            var node = new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogColorProp());
            node.Property.Should().BeSameAs(HardCodedTestModel.GetDogColorProp());
        }

        [Fact]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new SingleValuePropertyAccessNode(null, HardCodedTestModel.GetDogColorProp());
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }

        [Fact]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new SingleValuePropertyAccessNode(new ConstantNode(1), null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [Fact]
        public void SingleValuePropertyAccessNodesCanUseGeography()
        {
            ConstantNode constant = new ConstantNode(2);
            SingleValuePropertyAccessNode accessNode = new SingleValuePropertyAccessNode(constant, HardCodedTestModel.GetPersonGeographyPointProp());
            accessNode.Property.Should().Be(HardCodedTestModel.GetPersonGeographyPointProp());
        }

        [Fact]
        public void SingleValuePropertyAccessNodesCanUseGeometry()
        {
            ConstantNode constant = new ConstantNode(2);
            SingleValuePropertyAccessNode accessNode = new SingleValuePropertyAccessNode(constant, HardCodedTestModel.GetPersonGeometryPointProp());
            accessNode.Property.Should().Be(HardCodedTestModel.GetPersonGeometryPointProp());
        }

        [Fact]
        public void SingleValuePropertyAccessCannotTakeANavigationProperty()
        {
            Action create = () => new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetPersonMyDogNavProp());
            create.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_PropertyAccessShouldBeNonEntityProperty("MyDog"));
        }

        [Fact]
        public void SingleValuePropertyAccessCannotTakeACollectionProperty()
        {
            Action create = () => new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogNicknamesProperty());
            create.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_PropertyAccessTypeShouldNotBeCollection("Nicknames"));
        }
    }
}
