//---------------------------------------------------------------------
// <copyright file="CollectionConstantNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using System.Collections.Generic;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the CollectionConstantNode class
    /// </summary>
    public class CollectionConstantNodeTests
    {
        [Fact]
        public void NumberCollectionThroughLiteralTokenIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("[1,2,3]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                (literalToken.Value as ODataCollectionValue)?.Items, text, expectedType);

            var expectedList = new ConstantNode[] {
                new ConstantNode(1, "1", EdmCoreModel.Instance.GetInt32(false)),
                new ConstantNode(2, "2", EdmCoreModel.Instance.GetInt32(false)),
                new ConstantNode(3, "3", EdmCoreModel.Instance.GetInt32(false)),
            };

            VerifyCollectionConstantNode(collectionConstantNode.Collection, expectedList);
        }

        [Fact]
        public void StringCollectionThroughLiteralTokenIsSetCorrectly()
        {
            const string text = "('abc','def','ghi')";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("['abc','def','ghi']", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                (literalToken.Value as ODataCollectionValue)?.Items, text, expectedType);

            var expectedList = new ConstantNode[] {
                new ConstantNode("abc", "abc", EdmCoreModel.Instance.GetString(true)),
                new ConstantNode("def", "def", EdmCoreModel.Instance.GetString(true)),
                new ConstantNode("ghi", "ghi", EdmCoreModel.Instance.GetString(true)),
            };

            VerifyCollectionConstantNode(collectionConstantNode.Collection, expectedList);
        }

        [Fact]
        public void NullableCollectionTypeSetsConstantNodeCorrectly()
        {
            const string text = "('abc','def', null)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("['abc','def', null]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                (literalToken.Value as ODataCollectionValue)?.Items, text, expectedType);

            var expectedList = new ConstantNode[] {
                new ConstantNode("abc", "abc", EdmCoreModel.Instance.GetString(true)),
                new ConstantNode("def", "def", EdmCoreModel.Instance.GetString(true)),
                new ConstantNode(null, "null", EdmCoreModel.Instance.GetString(true)),
            };

            VerifyCollectionConstantNode(collectionConstantNode.Collection, expectedList);
        }

        [Fact]
        public void TextIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("[1,2,3]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                (literalToken.Value as ODataCollectionValue)?.Items, text, expectedType);

            Assert.Equal(text, collectionConstantNode.LiteralText);
        }

        [Fact]
        public void ItemTypeIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("[1,2,3]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                (literalToken.Value as ODataCollectionValue)?.Items, text, expectedType);

            Assert.Equal("Edm.Int32", collectionConstantNode.ItemType.FullName());
        }

        [Fact]
        public void CollectionTypeIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("[1,2,3]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                (literalToken.Value as ODataCollectionValue)?.Items, text, expectedType);

            Assert.Equal("Collection(Edm.Int32)", collectionConstantNode.CollectionType.FullName());
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("[1,2,3]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                (literalToken.Value as ODataCollectionValue)?.Items, text, expectedType);

            Assert.Equal(InternalQueryNodeKind.CollectionConstant, collectionConstantNode.InternalKind);
        }

        [Fact]
        public void NullValueShouldThrow()
        {
            const string text = "(1,2,3)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("[1,2,3]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            Action target = () => new CollectionConstantNode(null, text, expectedType);
            Assert.Throws<ArgumentNullException>("objectCollection", target);
        }

        [Fact]
        public void NullLiteralTextShouldThrow()
        {
            const string text = "(1,2,3)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("[1,2,3]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            Action target = () => new CollectionConstantNode((literalToken.Value as ODataCollectionValue)?.Items, null, expectedType);
            Assert.Throws<ArgumentNullException>("literalText", target);
        }

        [Fact]
        public void NullCollectionTypeShouldThrow()
        {
            const string text = "(1,2,3)";
            var expectedType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            object collection = ODataUriConversionUtils.ConvertFromCollectionValue("[1,2,3]", HardCodedTestModel.TestModel, expectedType);
            LiteralToken literalToken = new LiteralToken(collection, text, expectedType);

            Action target = () => new CollectionConstantNode((literalToken.Value as ODataCollectionValue)?.Items, text, null);
            Assert.Throws<ArgumentNullException>("collectionType", target);
        }

        private static void VerifyCollectionConstantNode(IList<ConstantNode> actual, ConstantNode[] expected)
        {
            Assert.NotNull(actual);
            Assert.Equal(actual.Count, expected.Length);

            int index = 0;
            foreach (var node in actual)
            {
                Assert.Equal(expected[index].LiteralText, node.LiteralText);
                Assert.Equal(expected[index].Value, node.Value);
                Assert.True(node.TypeReference.IsEquivalentTo(expected[index].TypeReference));
                index++;
            }
        }
    }
}
