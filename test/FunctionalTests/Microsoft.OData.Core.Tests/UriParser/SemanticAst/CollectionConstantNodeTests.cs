//---------------------------------------------------------------------
// <copyright file="CollectionConstantNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

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
            LiteralToken literalToken = new LiteralToken(new object[] { 1, 2, 3 }, text);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                literalToken.Value as IEnumerable<object>,
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            var expectedList = new ConstantNode[] {
                new ConstantNode(1, "1", EdmCoreModel.Instance.GetInt32(false)),
                new ConstantNode(2, "2", EdmCoreModel.Instance.GetInt32(false)),
                new ConstantNode(3, "3", EdmCoreModel.Instance.GetInt32(false)),
            };

            collectionConstantNode.Collection.ShouldBeEquivalentTo(expectedList);
        }

        [Fact]
        public void StringCollectionThroughLiteralTokenIsSetCorrectly()
        {
            const string text = "('abc','def','ghi')";
            LiteralToken literalToken = new LiteralToken(new object[] { "abc", "def", "ghi" }, text);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                literalToken.Value as IEnumerable<object>,
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));

            var expectedList = new ConstantNode[] {
                new ConstantNode("abc", "abc", EdmCoreModel.Instance.GetString(true)),
                new ConstantNode("def", "def", EdmCoreModel.Instance.GetString(true)),
                new ConstantNode("ghi", "ghi", EdmCoreModel.Instance.GetString(true)),
            };

            collectionConstantNode.Collection.ShouldBeEquivalentTo(expectedList);
        }

        [Fact]
        public void NumberCollectionThroughEnumerableIsSetCorrectly()
        {
            const string text = "(1,2,3)";

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                new object[] { 1, 2, 3 },
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            var expectedList = new ConstantNode[] {
                new ConstantNode(1, "1", EdmCoreModel.Instance.GetInt32(false)),
                new ConstantNode(2, "2", EdmCoreModel.Instance.GetInt32(false)),
                new ConstantNode(3, "3", EdmCoreModel.Instance.GetInt32(false)),
            };

            collectionConstantNode.Collection.ShouldBeEquivalentTo(expectedList);
        }

        [Fact]
        public void StringCollectionThroughEnumerableIsSetCorrectly()
        {
            const string text = "('abc','def','ghi')";

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                new object[] { "abc", "def", "ghi" },
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));

            var expectedList = new ConstantNode[] {
                new ConstantNode("abc", "abc", EdmCoreModel.Instance.GetString(true)),
                new ConstantNode("def", "def", EdmCoreModel.Instance.GetString(true)),
                new ConstantNode("ghi", "ghi", EdmCoreModel.Instance.GetString(true)),
            };

            collectionConstantNode.Collection.ShouldBeEquivalentTo(expectedList);
        }

        [Fact]
        public void TextIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            LiteralToken literalToken = new LiteralToken(new object[] { 1, 2, 3 }, text);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                literalToken.Value as IEnumerable<object>,
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            collectionConstantNode.LiteralText.Should().Be(text);
        }

        [Fact]
        public void ItemTypeIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            LiteralToken literalToken = new LiteralToken(new object[] { 1, 2, 3 }, text);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                literalToken.Value as IEnumerable<object>,
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            collectionConstantNode.ItemType.FullName().Should().Be("Edm.Int32");
        }

        [Fact]
        public void CollectionTypeIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            LiteralToken literalToken = new LiteralToken(new object[] { 1, 2, 3 }, text);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                literalToken.Value as IEnumerable<object>,
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            collectionConstantNode.CollectionType.FullName().Should().Be("Collection(Edm.Int32)");
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            LiteralToken literalToken = new LiteralToken(new object[] { 1, 2, 3 }, text);

            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                literalToken.Value as IEnumerable<object>,
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            collectionConstantNode.InternalKind.Should().Be(InternalQueryNodeKind.CollectionConstant);
        }

        [Fact]
        public void NullValueShouldThrow()
        {
            const string text = "(1,2,3)";
            Action target = () => new CollectionConstantNode(
                null,
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            target.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("objectCollection"));
        }

        [Fact]
        public void NullLiteralTextShouldThrow()
        {
            const string text = "(1,2,3)";
            LiteralToken literalToken = new LiteralToken(new object[] { 1, 2, 3 }, text);

            Action target = () => new CollectionConstantNode(
                literalToken.Value as IEnumerable<object>,
                null,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            target.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("literalText"));
        }

        [Fact]
        public void NullCollectionTypeShouldThrow()
        {
            const string text = "(1,2,3)";
            LiteralToken literalToken = new LiteralToken(new object[] { 1, 2, 3 }, text);

            Action target = () => new CollectionConstantNode(
                literalToken.Value as IEnumerable<object>,
                text,
                null);
            target.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("collectionType"));
        }
    }
}
