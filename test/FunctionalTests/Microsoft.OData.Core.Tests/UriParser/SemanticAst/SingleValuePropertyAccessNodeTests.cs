//---------------------------------------------------------------------
// <copyright file="SingleValuePropertyAccessNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
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
            Assert.Same(source, node.Source);
        }

        [Fact]
        public void PropertyIsSet()
        {
            var node = new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogColorProp());
            Assert.Same(HardCodedTestModel.GetDogColorProp(), node.Property);
        }

        [Fact]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new SingleValuePropertyAccessNode(null, HardCodedTestModel.GetDogColorProp());
            Assert.Throws<ArgumentNullException>("source", createWithNullSource);
        }

        [Fact]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new SingleValuePropertyAccessNode(new ConstantNode(1), null);
            Assert.Throws<ArgumentNullException>("property", createWithNullProperty);
        }

        [Fact]
        public void SingleValuePropertyAccessNodesCanUseGeography()
        {
            ConstantNode constant = new ConstantNode(2);
            SingleValuePropertyAccessNode accessNode = new SingleValuePropertyAccessNode(constant, HardCodedTestModel.GetPersonGeographyPointProp());
            Assert.Same(HardCodedTestModel.GetPersonGeographyPointProp(), accessNode.Property);
        }

        [Fact]
        public void SingleValuePropertyAccessNodesCanUseGeometry()
        {
            ConstantNode constant = new ConstantNode(2);
            SingleValuePropertyAccessNode accessNode = new SingleValuePropertyAccessNode(constant, HardCodedTestModel.GetPersonGeometryPointProp());
            Assert.Same(HardCodedTestModel.GetPersonGeometryPointProp(), accessNode.Property);
        }

        [Fact]
        public void SingleValuePropertyAccessCannotTakeANavigationProperty()
        {
            Action create = () => new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetPersonMyDogNavProp());
            create.Throws<ArgumentException>(Strings.Nodes_PropertyAccessShouldBeNonEntityProperty("MyDog"));
        }

        [Fact]
        public void SingleValuePropertyAccessCannotTakeACollectionProperty()
        {
            Action create = () => new SingleValuePropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogNicknamesProperty());
            create.Throws<ArgumentException>(Strings.Nodes_PropertyAccessTypeShouldNotBeCollection("Nicknames"));
        }
    }
}
