//---------------------------------------------------------------------
// <copyright file="CollectionLiteralBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class CollectionLiteralBinderTests
    {
        private static readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        [Fact]
        public void BindCollectionLiteralTokenShouldThrowWhenTokenIsNull()
        {
            // Arrange
            Func<QueryToken, QueryNode> bindMethod = token => null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CollectionLiteralBinder.BindCollectionLiteralToken(null, bindMethod));
        }

        [Fact]
        public void BindCollectionLiteralTokenShouldThrowWhenBindMethodIsNull()
        {
            // Arrange
            var token = new CollectionLiteralToken
            {
                OriginalText = "[]".AsMemory()
            };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CollectionLiteralBinder.BindCollectionLiteralToken(token, null));
        }

        [Fact]
        public void BindCollectionLiteralTokenWithEmptyCollection()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var token = new CollectionLiteralToken
            {
                OriginalText = "[]".AsMemory()
            };

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CollectionConstantNode>(result);
            var collectionNode = result as CollectionConstantNode;
            Assert.Empty(collectionNode.Items);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithPrimitiveLiterals()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var token = new CollectionLiteralToken
            {
                OriginalText = "[1, 2, 3]".AsMemory()
            };
            token.Items.Add(new LiteralToken(1));
            token.Items.Add(new LiteralToken(2));
            token.Items.Add(new LiteralToken(3));

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CollectionConstantNode>(result);
            var collectionNode = result as CollectionConstantNode;
            Assert.Equal(3, collectionNode.Items.Count);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithStringLiterals()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var token = new CollectionLiteralToken
            {
                OriginalText = "['a', 'b', 'c']".AsMemory()
            };
            token.Items.Add(new LiteralToken("a"));
            token.Items.Add(new LiteralToken("b"));
            token.Items.Add(new LiteralToken("c"));

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CollectionConstantNode>(result);
            var collectionNode = result as CollectionConstantNode;
            Assert.Equal(3, collectionNode.Items.Count);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithResourceLiterals()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var resourceToken1 = new ResourceLiteralToken { OriginalText = "{}".AsMemory() };
            resourceToken1.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("John")));

            var resourceToken2 = new ResourceLiteralToken { OriginalText = "{}".AsMemory() };
            resourceToken2.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("Jane")));

            var token = new CollectionLiteralToken
            {
                OriginalText = "[{...}, {...}]".AsMemory()
            };
            token.Items.Add(resourceToken1);
            token.Items.Add(resourceToken2);

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CollectionConstantNode>(result);
            var collectionNode = result as CollectionConstantNode;
            Assert.Equal(2, collectionNode.Items.Count);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithNestedCollections()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var nestedCollection1 = new CollectionLiteralToken { OriginalText = "[1, 2]".AsMemory() };
            nestedCollection1.Items.Add(new LiteralToken(1));
            nestedCollection1.Items.Add(new LiteralToken(2));

            var nestedCollection2 = new CollectionLiteralToken { OriginalText = "[3, 4]".AsMemory() };
            nestedCollection2.Items.Add(new LiteralToken(3));
            nestedCollection2.Items.Add(new LiteralToken(4));

            var token = new CollectionLiteralToken
            {
                OriginalText = "[[1, 2], [3, 4]]".AsMemory()
            };
            token.Items.Add(nestedCollection1);
            token.Items.Add(nestedCollection2);

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            CollectionConstantNode collectionNode = Assert.IsType<CollectionConstantNode>(result);
            Assert.Equal(2, collectionNode.Items.Count);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithExpectedCollectionType()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var stringCollectionType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            var token = new CollectionLiteralToken
            {
                ExpectedCollectionType = stringCollectionType,
                OriginalText = "['a', 'b']".AsMemory()
            };
            token.Items.Add(new LiteralToken("a"));
            token.Items.Add(new LiteralToken("b"));

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CollectionConstantNode>(result);
            var collectionNode = result as CollectionConstantNode;
            Assert.Equal(2, collectionNode.Items.Count);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithMixedLiteralsAndResourceLiteralsShouldThrow()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var resourceToken = new ResourceLiteralToken { OriginalText = "{}".AsMemory() };
            resourceToken.Properties.Add(new KeyValuePair<string, QueryToken>("Name", new LiteralToken("John")));

            var stringCollectionType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            var token = new CollectionLiteralToken
            {
                ExpectedCollectionType = stringCollectionType,
                OriginalText = "[1, {...}]".AsMemory()
            };
            token.Items.Add(new LiteralToken(1));
            token.Items.Add(resourceToken);

            // Act & Assert
            Assert.Throws<ODataException>(() => CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod));
        }

        [Fact]
        public void BindCollectionLiteralTokenWithMixedLiteralsAndCollectionsShouldThrow()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var nestedCollection = new CollectionLiteralToken { OriginalText = "[1, 2]".AsMemory() };
            nestedCollection.Items.Add(new LiteralToken(1));

            var stringCollectionType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false));
            var token = new CollectionLiteralToken
            {
                ExpectedCollectionType = stringCollectionType,
                OriginalText = "[1, [1, 2]]".AsMemory()
            };
            token.Items.Add(new LiteralToken(1));
            token.Items.Add(nestedCollection);

            // Act & Assert
            Assert.Throws<ODataException>(() => CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod));
        }

        [Fact]
        public void BindCollectionLiteralTokenWithRootPathTokens()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var token = new CollectionLiteralToken
            {
                OriginalText = "[$root, $root]".AsMemory()
            };

            RootPathToken rootPathToken = new RootPathToken();
            rootPathToken.Segments.Add("People(1)");
            token.Items.Add(rootPathToken);
            token.Items.Add(rootPathToken);

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            CollectionRootPathNode collectionRootPathNode = Assert.IsType<CollectionRootPathNode>(result);
            Assert.Equal(2, collectionRootPathNode.Collection.Count);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithMixedRootPathAndLiteralsShouldThrow()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var token = new CollectionLiteralToken
            {
                OriginalText = "[$root, 1]".AsMemory()
            };
            token.Items.Add(new RootPathToken());
            token.Items.Add(new LiteralToken(1));

            // Act & Assert
            Assert.Throws<ODataException>(() => CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod));
        }

        [Fact]
        public void BindCollectionLiteralTokenWithMixedLiteralsAndRootPathShouldThrow()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var token = new CollectionLiteralToken
            {
                OriginalText = "[1, $root]".AsMemory()
            };
            token.Items.Add(new LiteralToken(1));
            token.Items.Add(new RootPathToken());

            // Act & Assert
            Assert.Throws<ODataException>(() => CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod));
        }

        [Fact]
        public void BindCollectionLiteralTokenWithSingleItem()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var token = new CollectionLiteralToken
            {
                OriginalText = "[42]".AsMemory()
            };
            token.Items.Add(new LiteralToken(42));

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CollectionConstantNode>(result);
            var collectionNode = result as CollectionConstantNode;
            Assert.Single(collectionNode.Items);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithDateTimeLiterals()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var date1 = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var date2 = new DateTimeOffset(2024, 12, 31, 23, 59, 59, TimeSpan.Zero);

            var token = new CollectionLiteralToken
            {
                OriginalText = "[date1, date2]".AsMemory()
            };
            token.Items.Add(new LiteralToken(date1));
            token.Items.Add(new LiteralToken(date2));

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CollectionConstantNode>(result);
            var collectionNode = result as CollectionConstantNode;
            Assert.Equal(2, collectionNode.Items.Count);
        }

        [Fact]
        public void BindCollectionLiteralTokenWithUntypedCollection()
        {
            // Arrange
            var bindMethod = CreateBindMethod();
            var token = new CollectionLiteralToken
            {
                OriginalText = "[1, 'text', true]".AsMemory()
            };
            token.Items.Add(new LiteralToken(1));
            token.Items.Add(new LiteralToken("text"));
            token.Items.Add(new LiteralToken(true));

            // Act
            var result = CollectionLiteralBinder.BindCollectionLiteralToken(token, bindMethod);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CollectionConstantNode>(result);
            var collectionNode = result as CollectionConstantNode;
            Assert.Equal(3, collectionNode.Items.Count);
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

        private static Func<QueryToken, QueryNode> CreateBindMethod()
        {
            var bindingState = CreateBindingState();
            var metadataBinder = new MetadataBinder(bindingState);
            return token => metadataBinder.Bind(token);
        }

        #endregion
    }
}
