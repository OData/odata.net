//---------------------------------------------------------------------
// <copyright file="CollectionConstantNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
        public void ValueIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                new LiteralToken(new int[] { 1, 2, 3 }, text),
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            collectionConstantNode.Value.As<LiteralToken>().Should().NotBeNull();
        }

        [Fact]
        public void TextIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                new LiteralToken(new int[] { 1, 2, 3 }, text),
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            collectionConstantNode.LiteralText.Should().Be(text);
        }

        [Fact]
        public void ItemTypeIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                new LiteralToken(new int[] { 1, 2, 3 }, text),
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            collectionConstantNode.ItemType.FullName().Should().Be("Edm.Int32");
        }

        [Fact]
        public void CollectionTypeIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                new LiteralToken(new int[] { 1, 2, 3 }, text),
                text,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));

            collectionConstantNode.CollectionType.FullName().Should().Be("Collection(Edm.Int32)");
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            const string text = "(1,2,3)";
            CollectionConstantNode collectionConstantNode = new CollectionConstantNode(
                new LiteralToken(new int[] { 1, 2, 3 }, text),
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
            target.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("constantValue"));
        }

        [Fact]
        public void NullLiteralTextShouldThrow()
        {
            const string text = "(1,2,3)";
            Action target = () => new CollectionConstantNode(
                new LiteralToken(new int[] { 1, 2, 3 }, text),
                null,
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            target.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("literalText"));
        }

        [Fact]
        public void NullCollectionTypeShouldThrow()
        {
            const string text = "(1,2,3)";
            Action target = () => new CollectionConstantNode(
                new LiteralToken(new int[] { 1, 2, 3 }, text),
                text,
                null);
            target.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("collectionType"));
        }
    }
}
