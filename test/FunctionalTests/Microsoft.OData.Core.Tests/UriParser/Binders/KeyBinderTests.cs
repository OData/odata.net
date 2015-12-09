//---------------------------------------------------------------------
// <copyright file="KeyBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit tests for the KeyBinder class.
    /// </summary>
    public class KeyBinderTests
    {
        private readonly IEdmModel model = HardCodedTestModel.TestModel;
        private KeyBinder keyBinder;

        public KeyBinderTests()
        {
            this.keyBinder = new KeyBinder(FakeBindMethods.KeyValueBindMethod);
        }

        [Fact]
        public void KeyLookupOnEntitySetReturnsKeyLookupQueryNode()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var namedValues = new[] { new NamedValue(null, new LiteralToken(123)) };

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues, model);
            results.ShouldBeKeyLookupQueryNode().And.KeyPropertyValues.Single().KeyValue.Should().Be(FakeBindMethods.KeyBinderConstantToken);
        }

        [Fact]
        public void KeyLookupWithNamedKeyReturnsKeyLookupQueryNode()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var namedValues = new[] { new NamedValue("ID", new LiteralToken(123)) };

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues, model);
            results.ShouldBeKeyLookupQueryNode().And.KeyPropertyValues.Single().KeyValue.Should().Be(FakeBindMethods.KeyBinderConstantToken);
        }

        [Fact]
        public void KeyLookupWithMultipleUnnamedKeysShouldThrow()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetLionSet());
            var namedValues = new[] { new NamedValue(null, new LiteralToken(123)), new NamedValue(null, new LiteralToken(456)), };

            Action bind = () => this.keyBinder.BindKeyValues(collectionNode, namedValues, model);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(HardCodedTestModel.GetLionSet().EntityType().FullTypeName()));
        }

        [Fact]
        public void KeyLookupWithOneOfTwoKeysMissingShouldThrow()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetLionSet());
            var namedValues = new[] { new NamedValue("ID1", new LiteralToken(123)) };

            Action bind = () => this.keyBinder.BindKeyValues(collectionNode, namedValues, model);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(collectionNode.ItemType.FullName()));
        }

        [Fact]
        public void EntitySetWithNoKeysShouldBeJustBeEntitySetQueryNode()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var namedValues = new NamedValue[0];

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues, model);
            results.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void KeyLookupWithTwoKeysReturnsKeyLookupQueryNodeWithTwoKeyProperties()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetLionSet());
            var namedValues = new[] { new NamedValue("ID1", new LiteralToken(123)), new NamedValue("ID2", new LiteralToken(456)) };

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues, model);
            results.ShouldBeKeyLookupQueryNode().And.KeyPropertyValues.Count().Should().Be(2);
        }

        [Fact]
        public void KeyLookupOnEntitySetWithNamedKeyReturnsKeyLookupQueryNode()
        {
            var collectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());
            var namedValues = new[] { new NamedValue("ID", new LiteralToken(123)) };

            var results = this.keyBinder.BindKeyValues(collectionNode, namedValues, model);
            results.ShouldBeKeyLookupQueryNode().And.KeyPropertyValues.Single().KeyValue.Should().Be(FakeBindMethods.KeyBinderConstantToken);
        }

        // TODO: Clearly CollectionNode is too broad for BindKeyValues. Consider change the object model and/or the paramter type for BindKeyValues.

        /*
        // TODO: Collection of Primitive should throw
        [Fact]
        public void KeyLookupOnCollectionOfPrimitivePropertyShouldThrow()
        {
            // TODO: What is the Node for a regular property that is a collection of primitive or complex?
            var collectionNode = new PropertyAccessQueryNode();//(HardCodedTestModel.GetDogNicknamesProperty());
            var namedValues = new[] { new NamedValue(null, new LiteralToken(123)) };

            Action bind = () => this.keyBinder.BindKeyValues(collectionNode, namedValues);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_KeyValueApplicableOnlyToEntityType(collectionNode.ItemType.FullTypeName()));
        }

        // TODO: Collection of Complex should throw
        [Fact]
        public void KeyLookupOnCollectionOfComplexPropertyShouldThrow()
        {
            var collectionNode = new PropertyAccessQueryNode();//(HardCodedTestModel.GetPersonPreviousAddressesProp());
            var namedValues = new[] { new NamedValue(null, new LiteralToken(123)) };

            Action bind = () => this.keyBinder.BindKeyValues(collectionNode, namedValues);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_KeyValueApplicableOnlyToEntityType(collectionNode.ItemType.FullTypeName()));
        }
        */
    }
}
