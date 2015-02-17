//---------------------------------------------------------------------
// <copyright file="KeyBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the KeyBinder class.
    /// </summary>
    [TestClass]
    public class KeyBinderTests
    {
        private readonly IEdmModel model = HardCodedTestModel.TestModel;
        private KeyBinder keyBinder;

        [TestInitialize]
        public void Init()
        {
            this.keyBinder = new KeyBinder(FakeBindMethods.KeyValueBindMethod);
        }

        [TestMethod]
        public void KeyLookupOnEntitySetReturnsKeyLookupQueryNode()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var namedValues = new[] { new NamedValue(null, new LiteralToken(123)) };

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues);
            results.ShouldBeKeyLookupQueryNode().And.KeyPropertyValues.Single().KeyValue.Should().Be(FakeBindMethods.KeyBinderConstantToken);
        }

        [TestMethod]
        public void KeyLookupWithNamedKeyReturnsKeyLookupQueryNode()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var namedValues = new[] { new NamedValue("ID", new LiteralToken(123)) };

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues);
            results.ShouldBeKeyLookupQueryNode().And.KeyPropertyValues.Single().KeyValue.Should().Be(FakeBindMethods.KeyBinderConstantToken);
        }

        [TestMethod]
        public void KeyLookupWithMultipleUnnamedKeysShouldThrow()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetLionSet());
            var namedValues = new[] { new NamedValue(null, new LiteralToken(123)), new NamedValue(null, new LiteralToken(456)), };

            Action bind = () => this.keyBinder.BindKeyValues(collectionNode, namedValues);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(HardCodedTestModel.GetLionSet().EntityType().ODataFullName()));
        }

        [TestMethod]
        public void KeyLookupWithOneOfTwoKeysMissingShouldThrow()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetLionSet());
            var namedValues = new[] { new NamedValue("ID1", new LiteralToken(123)) };

            Action bind = () => this.keyBinder.BindKeyValues(collectionNode, namedValues);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(collectionNode.ItemType.ODataFullName()));
        }

        [TestMethod]
        public void EntitySetWithNoKeysShouldBeJustBeEntitySetQueryNode()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var namedValues = new NamedValue[0];

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues);
            results.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void KeyLookupWithTwoKeysReturnsKeyLookupQueryNodeWithTwoKeyProperties()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetLionSet());
            var namedValues = new[] { new NamedValue("ID1", new LiteralToken(123)), new NamedValue("ID2", new LiteralToken(456)) };

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues);
            results.ShouldBeKeyLookupQueryNode().And.KeyPropertyValues.Count().Should().Be(2);
        }

        [TestMethod]
        public void KeyLookupOnEntitySetWithNamedKeyReturnsKeyLookupQueryNode()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var namedValues = new[] { new NamedValue("ID", new LiteralToken(123)) };

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues);
            results.ShouldBeKeyLookupQueryNode().And.KeyPropertyValues.Single().KeyValue.Should().Be(FakeBindMethods.KeyBinderConstantToken);
        }

        // TODO: Clearly CollectionNode is too broad for BindKeyValues. Consider change the object model and/or the paramter type for BindKeyValues.

        /*
        // TODO: Collection of Primitive should throw
        [TestMethod]
        public void KeyLookupOnCollectionOfPrimitivePropertyShouldThrow()
        {
            // TODO: What is the Node for a regular property that is a collection of primitive or complex?
            var collectionNode = new PropertyAccessQueryNode();//(HardCodedTestModel.GetDogNicknamesProperty());
            var namedValues = new[] { new NamedValue(null, new LiteralToken(123)) };

            Action bind = () => this.keyBinder.BindKeyValues(collectionNode, namedValues);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_KeyValueApplicableOnlyToEntityType(collectionNode.ItemType.ODataFullName()));
        }

        // TODO: Collection of Complex should throw
        [TestMethod]
        public void KeyLookupOnCollectionOfComplexPropertyShouldThrow()
        {
            var collectionNode = new PropertyAccessQueryNode();//(HardCodedTestModel.GetPersonPreviousAddressesProp());
            var namedValues = new[] { new NamedValue(null, new LiteralToken(123)) };

            Action bind = () => this.keyBinder.BindKeyValues(collectionNode, namedValues);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_KeyValueApplicableOnlyToEntityType(collectionNode.ItemType.ODataFullName()));
        }
        */
    }
}
