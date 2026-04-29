//---------------------------------------------------------------------
// <copyright file="ResourceLiteralBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class ResourceLiteralBinderTests
    {
        private static readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        [Fact]
        public void BindResourceLiteralTokenShouldThrowWhenArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>("resourceLiteralToken", () => ResourceLiteralBinder.BindResourceLiteralToken(null, null, null));

            Assert.Throws<ArgumentNullException>("bindMethod", () => ResourceLiteralBinder.BindResourceLiteralToken(new ResourceLiteralToken(), null, null));

            Assert.Throws<ArgumentNullException>("bindingState", () => ResourceLiteralBinder.BindResourceLiteralToken(new ResourceLiteralToken(), token => null, null));
        }

        [Fact]
        public void BindResourceLiteralTokenWithNoExpectedTypeAndNoTypeFromLiteral()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var token = new ResourceLiteralToken
            {
                OriginalText = "{'Name': 'Test'}".AsMemory()
            };
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("Test", "'Test'")));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            ResourceConstantNode resourceConstantNode = Assert.IsType<ResourceConstantNode>(result);
            Assert.Single(resourceConstantNode.Properties);
        }

        [Fact]
        public void BindResourceLiteralTokenWithExpectedTypeOnly()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var token = new ResourceLiteralToken
            {
                ExpectedType = personType,
                OriginalText = "{}".AsMemory()
            };
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("John", "'John'")));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            ResourceConstantNode resourceConstantNode = Assert.IsType<ResourceConstantNode>(result);
            Assert.Single(resourceConstantNode.Properties);
        }

        [Fact]
        public void BindResourceLiteralTokenWithTypeFromLiteralOnly()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var token = new ResourceLiteralToken
            {
                TypeNameFromLiteral = "Fully.Qualified.Namespace.Person",
                OriginalText = "{\"@odata.type\":\"#Fully.Qualified.Namespace.Person\"}".AsMemory()
            };
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("Jane")));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResourceConstantNode>(result);
        }

        [Fact]
        public void BindResourceLiteralTokenWithMatchingExpectedTypeAndTypeFromLiteral()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var token = new ResourceLiteralToken
            {
                ExpectedType = personType,
                TypeNameFromLiteral = "Fully.Qualified.Namespace.Person",
                OriginalText = "{\"@odata.type\":\"#Fully.Qualified.Namespace.Person\"}".AsMemory()
            };
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("Bob")));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResourceConstantNode>(result);
        }

        [Fact]
        public void BindResourceLiteralTokenWithDerivedTypeFromLiteral()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var token = new ResourceLiteralToken
            {
                ExpectedType = personType,
                TypeNameFromLiteral = "Fully.Qualified.Namespace.Employee",
                OriginalText = "{\"@odata.type\":\"#Fully.Qualified.Namespace.Employee\"}".AsMemory()
            };
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("Alice")));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ConvertNode>(result);
            var convertNode = result as ConvertNode;
            Assert.IsType<ResourceConstantNode>(convertNode.Source);
        }

        [Fact]
        public void BindResourceLiteralTokenWithIncompatibleTypesShouldThrow()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var token = new ResourceLiteralToken
            {
                ExpectedType = personType,
                TypeNameFromLiteral = "Fully.Qualified.Namespace.Dog",
                OriginalText = "{\"@odata.type\":\"#Fully.Qualified.Namespace.Dog\"}".AsMemory()
            };

            // Act & Assert
            Assert.Throws<ODataException>(() => ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState));
        }

        [Fact]
        public void BindResourceLiteralTokenWithNestedResourceLiteral()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var nestedToken = new ResourceLiteralToken
            {
                OriginalText = "{}".AsMemory()
            };
            nestedToken.Properties.Add(new KeyValuePair<string, QueryToken>("Street", new LiteralToken("123 Main St")));

            var token = new ResourceLiteralToken
            {
                ExpectedType = personType,
                OriginalText = "{}".AsMemory()
            };
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("Test")));
            token.Properties.Add(new KeyValuePair<string, QueryToken>("HomeAddress", nestedToken));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResourceConstantNode>(result);
            var resourceNode = result as ResourceConstantNode;
            Assert.NotNull(resourceNode);
        }

        [Fact]
        public void BindResourceLiteralTokenWithCollectionProperty()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var collectionToken = new CollectionLiteralToken();
            collectionToken.Items.Add(new LiteralToken("value1"));
            collectionToken.Items.Add(new LiteralToken("value2"));

            var token = new ResourceLiteralToken
            {
                ExpectedType = personType,
                OriginalText = "{}".AsMemory()
            };
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("Test")));
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Numbers", collectionToken));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResourceConstantNode>(result);
        }

        [Fact]
        public void BindResourceLiteralTokenWithMultipleProperties()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var token = new ResourceLiteralToken
            {
                ExpectedType = personType,
                OriginalText = "{}".AsMemory()
            };
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("John Doe")));
            token.Properties.Add(new KeyValuePair<string, QueryToken>("Shoe", new LiteralToken("10")));
            token.Properties.Add(new KeyValuePair<string, QueryToken>("MyGuid", new LiteralToken(Guid.NewGuid())));
            token.Properties.Add(new KeyValuePair<string, QueryToken>("FavoriteDate", new LiteralToken(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            ResourceConstantNode resourceConstantNode = Assert.IsType<ResourceConstantNode>(result);
            Assert.Equal(4, resourceConstantNode.Properties.Count);
        }

        [Fact]
        public void BindResourceLiteralTokenWithUntypedProperty()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var paintingType = HardCodedTestModel.GetPaintingTypeReference(); // Open type
            var token = new ResourceLiteralToken
            {
                ExpectedType = paintingType,
                OriginalText = "{}".AsMemory()
            };
            // Adding an undeclared property to an open type
            token.Properties.Add(new KeyValuePair<string, QueryToken>("CustomProperty", new LiteralToken("CustomValue")));

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResourceConstantNode>(result);
        }

        [Fact]
        public void BindResourceLiteralTokenWithInvalidTypeNameShouldThrow()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var token = new ResourceLiteralToken
            {
                TypeNameFromLiteral = "Invalid.Type.Name",
                OriginalText = "{\"@odata.type\":\"#Invalid.Type.Name\"}".AsMemory()
            };

            // Act & Assert
            Assert.Throws<ODataException>(() => ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState));
        }

        [Fact]
        public void BindResourceLiteralTokenWithEmptyPropertiesList()
        {
            // Arrange
            var (bindMethod, bindingState) = CreateArguments();
            var personType = HardCodedTestModel.GetPersonTypeReference();
            var token = new ResourceLiteralToken
            {
                ExpectedType = personType,
                OriginalText = "{}".AsMemory()
            };

            // Act
            var result = ResourceLiteralBinder.BindResourceLiteralToken(token, bindMethod, bindingState);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResourceConstantNode>(result);
        }

        #region Helper Methods

        private static BindingState CreateBindingState()
        {
            ResourceRangeVariable implicitRangeVariable = new ResourceRangeVariable(
                ExpressionConstants.It,
                HardCodedTestModel.GetPersonTypeReference(),
                HardCodedTestModel.GetPeopleSet());

            var bindingState = new BindingState(configuration)
            {
                ImplicitRangeVariable = implicitRangeVariable
            };

            bindingState.RangeVariables.Push(bindingState.ImplicitRangeVariable);
            return bindingState;
        }

        private static (Func<QueryToken, QueryNode>, BindingState) CreateArguments()
        {
            var bindingState = CreateBindingState();
            var metadataBinder = new MetadataBinder(bindingState);
            Func<QueryToken, QueryNode> bindMethod = token => metadataBinder.Bind(token);
            return (bindMethod, bindingState);
        }

        #endregion
    }
}
