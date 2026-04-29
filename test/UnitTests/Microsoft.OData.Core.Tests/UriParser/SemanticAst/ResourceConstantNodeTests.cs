//---------------------------------------------------------------------
// <copyright file="ResourceConstantNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the ResourceConstantNode class
    /// </summary>
    public class ResourceConstantNodeTests
    {
        private static readonly IEdmModel Model = HardCodedTestModel.TestModel;

        #region Constructor Tests

        [Fact]
        public void ConstructorWithNullTypeReferenceShouldSucceed()
        {
            // Act
            var node = new ResourceConstantNode(null);

            // Assert
            Assert.Null(node.ExpectedStructuredType);
            Assert.Null(node.TypeReference);
        }

        [Fact]
        public void ConstructorWithValidTypeReferenceShouldSucceed()
        {
            // Arrange
            var personType = HardCodedTestModel.GetPersonTypeReference();

            // Act
            var node = new ResourceConstantNode(personType);

            // Assert
            Assert.NotNull(node.ExpectedStructuredType);
            Assert.Equal(personType, node.ExpectedStructuredType);
            Assert.Equal(personType, node.TypeReference);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void PropertiesCollectionShouldBeEmptyByDefault()
        {
            // Arrange & Act
            var node = new ResourceConstantNode(null);

            // Assert
            Assert.NotNull(node.Properties);
            Assert.Empty(node.Properties);
        }

        [Fact]
        public void AddPropertyShouldAddToPropertiesCollection()
        {
            // Arrange
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var node = new ResourceConstantNode(personType);
            var nameNode = new ConstantNode("John Doe");

            // Act
            node.Properties.Add(new KeyValuePair<string, QueryNode>("Name", nameNode));

            // Assert
            Assert.Single(node.Properties);
            Assert.Equal("Name", node.Properties[0].Key);
            Assert.Equal(nameNode, node.Properties[0].Value);
        }

        [Fact]
        public void AddMultiplePropertiesShouldMaintainOrder()
        {
            // Arrange
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var node = new ResourceConstantNode(personType);

            // Act
            node.Properties.Add(new KeyValuePair<string, QueryNode>("Name", new ConstantNode("John")));
            node.Properties.Add(new KeyValuePair<string, QueryNode>("Shoe", new ConstantNode(10)));
            node.Properties.Add(new KeyValuePair<string, QueryNode>("FavoriteDate", new ConstantNode(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))));

            // Assert
            Assert.Equal(3, node.Properties.Count);
            Assert.Equal("Name", node.Properties[0].Key);
            Assert.Equal("Shoe", node.Properties[1].Key);
            Assert.Equal("FavoriteDate", node.Properties[2].Key);
        }

        [Fact]
        public void PropertiesShouldSupportNestedResourceConstantNodes()
        {
            // Arrange
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var addressType = Model.FindType("Fully.Qualified.Namespace.Address") as IEdmComplexType;
            var addressTypeRef = new EdmComplexTypeReference(addressType, true);

            var addressNode = new ResourceConstantNode(addressTypeRef);
            addressNode.Properties.Add(new KeyValuePair<string, QueryNode>("Street", new ConstantNode("123 Main St")));
            addressNode.Properties.Add(new KeyValuePair<string, QueryNode>("City", new ConstantNode("Redmond")));

            var personNode = new ResourceConstantNode(personType);
            personNode.Properties.Add(new KeyValuePair<string, QueryNode>("Name", new ConstantNode("Jane")));
            personNode.Properties.Add(new KeyValuePair<string, QueryNode>("HomeAddress", addressNode));

            // Assert
            Assert.Equal(2, personNode.Properties.Count);
            Assert.Equal("HomeAddress", personNode.Properties[1].Key);
            Assert.IsType<ResourceConstantNode>(personNode.Properties[1].Value);

            var nestedAddress = personNode.Properties[1].Value as ResourceConstantNode;
            Assert.Equal(2, nestedAddress.Properties.Count);
        }

        #endregion

        #region LiteralText Tests

        [Fact]
        public void LiteralTextShouldBeEmptyByDefault()
        {
            // Arrange & Act
            var node = new ResourceConstantNode(null);

            // Assert
            Assert.True(node.LiteralText.IsEmpty);
        }

        [Fact]
        public void LiteralTextShouldBeSettable()
        {
            // Arrange
            var node = new ResourceConstantNode(null);
            var literalText = "{\"Name\":\"John\",\"Age\":30}".AsMemory();

            // Act
            node.LiteralText = literalText;

            // Assert
            Assert.Equal("{\"Name\":\"John\",\"Age\":30}", node.LiteralText.ToString());
        }

        [Fact]
        public void LiteralTextShouldBeOverwritable()
        {
            // Arrange
            var node = new ResourceConstantNode(null);
            node.LiteralText = "original".AsMemory();

            // Act
            node.LiteralText = "updated".AsMemory();

            // Assert
            Assert.Equal("updated", node.LiteralText.ToString());
        }

        #endregion

        #region TypeReference Tests

        [Fact]
        public void TypeReferenceShouldReturnExpectedStructuredType()
        {
            // Arrange
            var personType = HardCodedTestModel.GetPersonTypeReference();

            // Act
            var node = new ResourceConstantNode(personType);

            // Assert
            Assert.Equal(personType, node.TypeReference);
            Assert.Same(node.ExpectedStructuredType, node.TypeReference);
        }

        [Fact]
        public void TypeReferenceShouldBeNullWhenExpectedTypeIsNull()
        {
            // Arrange & Act
            var node = new ResourceConstantNode(null);

            // Assert
            Assert.Null(node.TypeReference);
        }

        #endregion

        #region InternalKind Tests

        [Fact]
        public void InternalKindShouldBeResourceConstant()
        {
            // Arrange & Act
            var node = new ResourceConstantNode(null);

            // Assert
            Assert.Equal(InternalQueryNodeKind.ResourceConstant, node.InternalKind);
        }

        #endregion

        #region Accept Tests

        [Fact]
        public void AcceptShouldCallVisitOnVisitor()
        {
            // Arrange
            var node = new ResourceConstantNode(HardCodedTestModel.GetPersonTypeReference());
            var visitor = new TestQueryNodeVisitor();

            // Act
            var result = node.Accept(visitor);

            // Assert
            Assert.True(visitor.VisitResourceConstantNodeCalled);
            Assert.Equal(node, result);
        }

        [Fact]
        public void AcceptShouldThrowWhenVisitorIsNull()
        {
            // Arrange
            var node = new ResourceConstantNode(null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => node.Accept<QueryNode>(null));
        }

        #endregion

        #region Complex Scenarios

        [Fact]
        public void ResourceConstantNodeWithMixedPropertyTypes()
        {
            // Arrange
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var node = new ResourceConstantNode(personType);

            // Act
            node.Properties.Add(new KeyValuePair<string, QueryNode>("Name", new ConstantNode("Alice")));
            node.Properties.Add(new KeyValuePair<string, QueryNode>("Shoe", new ConstantNode(7)));
            node.Properties.Add(new KeyValuePair<string, QueryNode>("FavoriteDate", new ConstantNode(new DateTimeOffset(2024, 12, 25, 0, 0, 0, TimeSpan.Zero))));

            var addressType = Model.FindType("Fully.Qualified.Namespace.Address") as IEdmComplexType;
            var addressTypeRef = new EdmComplexTypeReference(addressType, true);
            var addressNode = new ResourceConstantNode(addressTypeRef);
            addressNode.Properties.Add(new KeyValuePair<string, QueryNode>("City", new ConstantNode("Seattle")));
            node.Properties.Add(new KeyValuePair<string, QueryNode>("HomeAddress", addressNode));

            // Assert
            Assert.Equal(4, node.Properties.Count);
            Assert.IsType<ConstantNode>(node.Properties[0].Value);
            Assert.IsType<ConstantNode>(node.Properties[1].Value);
            Assert.IsType<ConstantNode>(node.Properties[2].Value);
            Assert.IsType<ResourceConstantNode>(node.Properties[3].Value);
        }

        [Fact]
        public void ResourceConstantNodeWithEmptyPropertiesAndLiteralText()
        {
            // Arrange
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var node = new ResourceConstantNode(personType);

            // Act
            node.LiteralText = "{}".AsMemory();

            // Assert
            Assert.Empty(node.Properties);
            Assert.Equal("{}", node.LiteralText.ToString());
            Assert.NotNull(node.TypeReference);
        }

        [Fact]
        public void ResourceConstantNodeCanHaveDuplicatePropertyNames()
        {
            // Arrange
            var node = new ResourceConstantNode(null);

            // Act - This simulates a potentially malformed input
            node.Properties.Add(new KeyValuePair<string, QueryNode>("Name", new ConstantNode("First")));
            node.Properties.Add(new KeyValuePair<string, QueryNode>("Name", new ConstantNode("Second")));

            // Assert - The collection allows duplicates (validation would happen elsewhere)
            Assert.Equal(2, node.Properties.Count);
            Assert.Equal("Name", node.Properties[0].Key);
            Assert.Equal("Name", node.Properties[1].Key);
        }

        [Fact]
        public void ResourceConstantNodeWithDerivedType()
        {
            // Arrange
            var employeeType = HardCodedTestModel.GetEmployeeTypeReference();
            var node = new ResourceConstantNode(employeeType);

            // Act
            node.Properties.Add(new KeyValuePair<string, QueryNode>("Name", new ConstantNode("Bob")));
            node.Properties.Add(new KeyValuePair<string, QueryNode>("WorkEmail", new ConstantNode("bob@contoso.com")));

            // Assert
            Assert.Equal(employeeType, node.TypeReference);
            Assert.Equal("Fully.Qualified.Namespace.Employee", node.TypeReference.FullName());
        }

        #endregion

        #region Helper Classes

        private class TestQueryNodeVisitor : QueryNodeVisitor<QueryNode>
        {
            public bool VisitResourceConstantNodeCalled { get; private set; }

            public override QueryNode Visit(ResourceConstantNode nodeIn)
            {
                VisitResourceConstantNodeCalled = true;
                return nodeIn;
            }
        }

        #endregion
    }
}
