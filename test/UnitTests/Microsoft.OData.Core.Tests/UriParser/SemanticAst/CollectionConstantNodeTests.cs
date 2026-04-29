//---------------------------------------------------------------------
// <copyright file="CollectionConstantNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        #region Constructor and property initialization

        [Fact]
        public void ConstructorSetsCollectionTypeCorrectly()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);

            Assert.Equal("Collection(Edm.Int32)", node.CollectionType.FullName());
        }

        [Fact]
        public void ConstructorSetsItemTypeCorrectly()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)));
            var node = new CollectionConstantNode(collectionType);

            Assert.Equal("Edm.String", node.ItemType.FullName());
        }

        [Fact]
        public void ConstructorWithNullCollectionTypeAllowed()
        {
            var node = new CollectionConstantNode(null);

            Assert.Null(node.CollectionType);
            Assert.Null(node.ItemType);
        }

        [Fact]
        public void ItemsIsEmptyOnConstruction()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);

            Assert.NotNull(node.Items);
            Assert.Empty(node.Items);
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);

            Assert.Equal(InternalQueryNodeKind.CollectionConstant, node.InternalKind);
        }

        #endregion

        #region LiteralText

        [Fact]
        public void LiteralTextDefaultsToEmpty()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);

            Assert.Equal(string.Empty, node.LiteralText.ToString());
        }

        [Fact]
        public void LiteralTextCanBeSet()
        {
            const string text = "(1,2,3)";
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType) { LiteralText = text.AsMemory() };

            Assert.Equal(text, node.LiteralText.ToString());
        }

        [Fact]
        public void LiteralTextRoundTripsForStringCollection()
        {
            const string text = "('abc','def','ghi')";
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)));
            var node = new CollectionConstantNode(collectionType) { LiteralText = text.AsMemory() };

            Assert.Equal(text, node.LiteralText.ToString());
        }

        #endregion

        #region Items

        [Fact]
        public void IntegerItemsCanBeAddedAndRetrieved()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);

            var item1 = new ConstantNode(1, "1", EdmCoreModel.Instance.GetInt32(false));
            var item2 = new ConstantNode(2, "2", EdmCoreModel.Instance.GetInt32(false));
            var item3 = new ConstantNode(3, "3", EdmCoreModel.Instance.GetInt32(false));
            node.Items.Add(item1);
            node.Items.Add(item2);
            node.Items.Add(item3);

            Assert.Equal(3, node.Items.Count);
            Assert.Same(item1, node.Items[0]);
            Assert.Same(item2, node.Items[1]);
            Assert.Same(item3, node.Items[2]);
        }

        [Fact]
        public void StringItemsCanBeAddedAndRetrieved()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)));
            var node = new CollectionConstantNode(collectionType);
            node.Items.Add(new ConstantNode("abc", "abc", EdmCoreModel.Instance.GetString(true)));
            node.Items.Add(new ConstantNode("def", "def", EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(2, node.Items.Count);
            Assert.Equal("abc", ((ConstantNode)node.Items[0]).Value);
            Assert.Equal("def", ((ConstantNode)node.Items[1]).Value);
        }

        [Fact]
        public void NullableStringItemWithNullValueIsAllowed()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)));
            var node = new CollectionConstantNode(collectionType);
            node.Items.Add(new ConstantNode("abc", "abc", EdmCoreModel.Instance.GetString(true)));
            node.Items.Add(new ConstantNode(null, "null", EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(2, node.Items.Count);
            Assert.Null(((ConstantNode)node.Items[1]).Value);
        }

        [Fact]
        public void BooleanItemsCanBeAddedAndRetrieved()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetBoolean(false)));
            var node = new CollectionConstantNode(collectionType);
            node.Items.Add(new ConstantNode(true, "true", EdmCoreModel.Instance.GetBoolean(false)));
            node.Items.Add(new ConstantNode(false, "false", EdmCoreModel.Instance.GetBoolean(false)));

            Assert.Equal(2, node.Items.Count);
            Assert.Equal(true, ((ConstantNode)node.Items[0]).Value);
            Assert.Equal(false, ((ConstantNode)node.Items[1]).Value);
        }

        [Fact]
        public void DoubleItemsCanBeAddedAndRetrieved()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDouble(false)));
            var node = new CollectionConstantNode(collectionType);
            node.Items.Add(new ConstantNode(1.5, "1.5", EdmCoreModel.Instance.GetDouble(false)));
            node.Items.Add(new ConstantNode(2.5, "2.5", EdmCoreModel.Instance.GetDouble(false)));

            Assert.Equal(2, node.Items.Count);
            Assert.Equal(1.5, ((ConstantNode)node.Items[0]).Value);
            Assert.Equal(2.5, ((ConstantNode)node.Items[1]).Value);
        }

        [Fact]
        public void SingleItemCollectionIsCorrect()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);
            node.Items.Add(new ConstantNode(42, "42", EdmCoreModel.Instance.GetInt32(false)));

            Assert.Single(node.Items);
            Assert.Equal(42, ((ConstantNode)node.Items[0]).Value);
        }

        [Fact]
        public void NestedCollectionNodeCanBeAddedAsItem()
        {
            var innerType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var outerType = new EdmCollectionTypeReference(new EdmCollectionType(innerType));

            var innerNode = new CollectionConstantNode(innerType);
            innerNode.Items.Add(new ConstantNode(1, "1", EdmCoreModel.Instance.GetInt32(false)));

            var outerNode = new CollectionConstantNode(outerType);
            outerNode.Items.Add(innerNode);

            Assert.Single(outerNode.Items);
            Assert.IsType<CollectionConstantNode>(outerNode.Items[0]);
        }

        #endregion

        #region Collection (obsolete) property

        [Fact]
        public void CollectionObsoletePropertyReturnsConstantNodesOnly()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);

            var constNode1 = new ConstantNode(1, "1", EdmCoreModel.Instance.GetInt32(false));
            var constNode2 = new ConstantNode(2, "2", EdmCoreModel.Instance.GetInt32(false));
            var innerCollection = new CollectionConstantNode(collectionType);

            node.Items.Add(constNode1);
            node.Items.Add(innerCollection);
            node.Items.Add(constNode2);

#pragma warning disable CS0618
            var collection = node.Collection;
#pragma warning restore CS0618

            Assert.Equal(2, collection.Count);
            Assert.Same(constNode1, collection[0]);
            Assert.Same(constNode2, collection[1]);
        }

        [Fact]
        public void CollectionObsoletePropertyIsReadOnly()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);
            node.Items.Add(new ConstantNode(1, "1", EdmCoreModel.Instance.GetInt32(false)));

#pragma warning disable CS0618
            var collection = node.Collection;
#pragma warning restore CS0618

            Assert.Throws<NotSupportedException>(() => collection.Add(new ConstantNode(2, "2", EdmCoreModel.Instance.GetInt32(false))));
        }

        #endregion

        #region Visitor

        [Fact]
        public void AcceptVisitorCallsVisitMethod()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);

            var visitor = new TestQueryNodeVisitor();
            var result = node.Accept(visitor);

            Assert.Same(node, result);
        }

        [Fact]
        public void AcceptNullVisitorThrows()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            var node = new CollectionConstantNode(collectionType);

            Assert.Throws<ArgumentNullException>(() => node.Accept<QueryNode>(null));
        }

        #endregion

        #region CollectionType for different EDM types

        [Fact]
        public void Int64CollectionTypeIsCorrect()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt64(false)));
            var node = new CollectionConstantNode(collectionType);

            Assert.Equal("Collection(Edm.Int64)", node.CollectionType.FullName());
            Assert.Equal("Edm.Int64", node.ItemType.FullName());
        }

        [Fact]
        public void GuidCollectionTypeIsCorrect()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetGuid(false)));
            var node = new CollectionConstantNode(collectionType);

            Assert.Equal("Collection(Edm.Guid)", node.CollectionType.FullName());
            Assert.Equal("Edm.Guid", node.ItemType.FullName());
        }

        [Fact]
        public void DateTimeOffsetCollectionTypeIsCorrect()
        {
            var collectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDateTimeOffset(false)));
            var node = new CollectionConstantNode(collectionType);

            Assert.Equal("Collection(Edm.DateTimeOffset)", node.CollectionType.FullName());
            Assert.Equal("Edm.DateTimeOffset", node.ItemType.FullName());
        }

        #endregion

        #region Helpers

        private sealed class TestQueryNodeVisitor : QueryNodeVisitor<QueryNode>
        {
            public override QueryNode Visit(CollectionConstantNode nodeIn) => nodeIn;
        }

        #endregion
    }
}