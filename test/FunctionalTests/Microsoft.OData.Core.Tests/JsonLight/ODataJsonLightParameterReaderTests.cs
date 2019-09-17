﻿//---------------------------------------------------------------------
// <copyright file="ParameterReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Utils.ODataLibTest;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    /// <summary>
    /// Tests the use of ODataParameterReader class when the payload is JSON.
    ///
    /// TODO: For error tests, see Microsoft.Test.Taupo.OData.Reader.Tests.Reader.ParameterReaderTests.
    /// These should eventually be migrated here.
    /// </summary>
    public class ODataJsonLightParameterReaderTests
    {
        private EdmModel referencedModel;
        private IEdmModel model;
        private EdmAction action;

        public ODataJsonLightParameterReaderTests()
        {
            referencedModel = new EdmModel();
            referencedModel.Fixup();
            this.action = new EdmAction("FQ.NS", "ActionImport", null);
            referencedModel.AddElement(this.action);
            this.model = TestUtils.WrapReferencedModelsToMainModel("TestModel", "DefaultContainer", referencedModel);
        }

        [Fact]
        public void ParameterReaderShouldReadEmptyJson()
        {
            string payload = "{}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Empty(result.Values);
            Assert.Empty(result.Collections);
        }

        [Fact]
        public void ParameterReaderShouldReadSinglePrimitiveValue()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            const string payload = "{\"days\":4}";

            var result = this.RunParameterReaderTest(payload);
            Assert.NotNull(result);
            var value = Assert.Single(result.Values);
            Assert.Equal("days", value.Key);
            Assert.Equal(4, value.Value);
        }

        [Fact]
        public void ParameterReaderShouldNotThrowIfMissingNullableNonOptionalParameter()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(true));
            const string payload = "{ }";

            var result = this.RunParameterReaderTest(payload);
            Assert.Empty(result.Values);
        }

        [Fact]
        public void ParameterReaderShouldThrowIfMissingNonNullableNonOptionalParameter()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            const string payload = "{ }";

            Action test = () => this.RunParameterReaderTest(payload);
            test.Throws<ODataException>(Strings.ODataParameterReaderCore_ParametersMissingInPayload("ActionImport", "days"));
        }

        [Fact]
        public void ParameterReaderShouldReadOptionalParameter()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddOptionalParameter("optionalDays", EdmCoreModel.Instance.GetInt32(false));
            const string payload = "{\"days\":4, \"optionalDays\":8 }";

            var result = this.RunParameterReaderTest(payload);

            Assert.Equal(2, result.Values.Count);
            Assert.Contains(result.Values, v => v.Key.Equals("days") && v.Value.Equals(4));
            Assert.Contains(result.Values, v => v.Key.Equals("optionalDays") && v.Value.Equals(8));
        }

        [Fact]
        public void ParameterReaderShouldNotThrowIfMissingOptionalParameter()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddOptionalParameter("optionalDays", EdmCoreModel.Instance.GetInt32(false));
            const string payload = "{\"days\":4 }";

            var result = this.RunParameterReaderTest(payload);

            var value = Assert.Single(result.Values);
            Assert.Equal("days", value.Key);
            Assert.Equal(4, value.Value);
        }

        [Fact]
        public void ParameterReaderShouldReadTwoPrimitiveValue()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            string payload = "{\"days\":4, \"name\":\"john\"}";

            var result = this.RunParameterReaderTest(payload);

            Assert.Equal(2, result.Values.Count);
            Assert.Contains(result.Values, v => v.Key.Equals("days") && v.Value.Equals(4));
            Assert.Contains(result.Values, v => v.Key.Equals("name") && v.Value.Equals("john"));
        }

        [Fact]
        public void ParameterReaderShouldReadSingleCollectionValue()
        {
            this.action.AddParameter("names", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));
            string payload = "{\"names\": [\"john\", \"suzy\"]}";

            var result = this.RunParameterReaderTest(payload);
            var item = Assert.Single(result.Collections);
            Assert.Equal("names", item.Key);

            var collectionItems = item.Value.Items;
            Assert.Equal(2, collectionItems.Count());
            Assert.Equal(new[] { "john", "suzy" }, collectionItems.OfType<string>());
        }

        [Fact]
        public void ParameterReaderShouldReadSingleComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String).Property("StreetNumber", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("address", new EdmComplexTypeReference(complexType, false));
            string payload = "{\"address\" : { \"StreetName\": \"Bla\", \"StreetNumber\" : 61 } }";

            var result = this.RunParameterReaderTest(payload);

            var item = Assert.Single(result.Entries);
            Assert.Equal("address", item.Key);
            Assert.Single(item.Value);
        }

        [Fact]
        public void ParameterReaderShouldReadSingleDerivedComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String);
            var derivedComplexType = new EdmComplexType("TestModel", "derivedAddress", complexType, false);
            derivedComplexType.AddStructuralProperty("StreetNumber", EdmPrimitiveTypeKind.Int32, false);
            this.referencedModel.AddElement(derivedComplexType);

            this.action.AddParameter("address", new EdmComplexTypeReference(complexType, false));
            string payload = "{\"address\" : { \"StreetName\": \"Bla\", \"StreetNumber\" : 61, \"@odata.type\":\"TestModel.derivedAddress\" } }";

            var result = this.RunParameterReaderTest(payload);
            var item = Assert.Single(result.Entries);
            Assert.Equal("address", item.Key);
            Assert.Single(item.Value);
        }

        [Fact]
        public void ParameterReaderShouldReadCollectionOfDerivedComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String);
            var derivedComplexType = new EdmComplexType("TestModel", "derivedAddress", complexType, false);
            derivedComplexType.AddStructuralProperty("StreetNumber", EdmPrimitiveTypeKind.Int32, false);
            this.referencedModel.AddElement(derivedComplexType);

            this.action.AddParameter("addresses", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, false)));
            string payload = "{\"addresses\" : [{ \"StreetName\": \"Bla\", \"StreetNumber\" : 61, \"@odata.type\":\"TestModel.derivedAddress\" }, { \"StreetName\": \"Bla2\" }]}";

            var result = this.RunParameterReaderTest(payload);

            var item = Assert.Single(result.Feeds);
            Assert.Equal("addresses", item.Key);

            var collectioItems = Assert.Single(result.Entries).Value;
            Assert.Equal(2, collectioItems.Count());
            Assert.Equal(2, collectioItems.OfType<ODataResource>().Count());
        }

        [Fact]
        public void ParameterReaderShouldReadCollectionOfComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String).Property("StreetNumber", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("addresses", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, false)));
            string payload = "{\"addresses\" : [{ \"StreetName\": \"Bla\", \"StreetNumber\" : 61 }, { \"StreetName\": \"Bla2\", \"StreetNumber\" : 64 }]}";

            var result = this.RunParameterReaderTest(payload);
            var item = Assert.Single(result.Feeds);
            Assert.Equal("addresses", item.Key);

            var collectioItems = Assert.Single(result.Entries).Value;
            Assert.Equal(2, collectioItems.Count());
            Assert.Equal(2, collectioItems.OfType<ODataResource>().Count());
        }

        [Fact]
        public void ParameterReaderShouldReadMultiplePrimitiveValue()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("thirdParameter", EdmCoreModel.Instance.GetInt32(false));

            string payload = "{\"days\":4, \"name\":\"john\", \"thirdParameter\": 90}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Equal(3, result.Values.Count());
        }

        [Fact]
        public void ParameterReaderShouldReadBeOrderInsensitive()
        {
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));

            string payload = "{\"days\":4, \"name\":\"john\"}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Equal(2, result.Values.Count());
        }

        [Fact]
        public void ParameterReaderShouldReadPrimitiveValueAndPrimitiveCollection()
        {
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("hobbies", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));

            string payload = "{\"name\" : \"john\", \"hobbies\": [\"football\", \"basketball\"]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Values);
            Assert.Single(result.Collections);
        }

        [Fact]
        public void ParameterReaderShouldReadTypeDefinitionValueAndTypeDefinitionCollection()
        {
            var nameType = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "NameType", EdmPrimitiveTypeKind.String), false);
            this.action.AddParameter("name", nameType);
            this.action.AddParameter("hobbies", EdmCoreModel.GetCollection(nameType));

            string payload = "{\"name\" : \"john\", \"hobbies\": [\"football\", \"basketball\"]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Values);
            Assert.Single(result.Collections);
        }

        [Fact]
        public void ParameterReaderShouldConvertTypesAccordingToModel()
        {
            this.action.AddParameter("length", EdmCoreModel.Instance.GetDouble(false));

            string payload = "{\"length\":\"4.5\"}";

            var result = this.RunParameterReaderTest(payload);
            Assert.IsType<double>(Assert.Single(result.Values).Value);
        }

        [Fact]
        public void ReadEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            string payload = "{\"entry\":{\"ID\":1}}";

            var result = this.RunParameterReaderTest(payload);
            var pair = Assert.Single(result.Entries);
            Assert.Equal("entry", pair.Key);
            var entry = Assert.Single(pair.Value);
            var property = Assert.Single(entry.Properties);
            Assert.Equal(1, property.Value);
        }

        [Fact]
        public void ReadEntityAndComplex()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var complexType = this.referencedModel.ComplexType("ComplexType").Property("Name", EdmPrimitiveTypeKind.String);
            entityType.AddStructuralProperty("complexProperty", new EdmComplexTypeReference(complexType, true));
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));
            this.action.AddParameter("complex", new EdmComplexTypeReference(complexType, false));

            string payload = "{\"entry\":{\"ID\":1,\"complexProperty\":{\"Name\":\"ComplexName\"}},\"complex\":{\"Name\":\"ComplexName\"}}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Equal(2, result.Entries.Count);
            var pair = result.Entries.First();
            Assert.Equal("entry", pair.Key);
            Assert.Equal(2, pair.Value.Count);
            var complex = pair.Value.ElementAt(0);
            Assert.Equal("ComplexName", Assert.Single(complex.Properties).Value);
            var entry = pair.Value.Last();
            Assert.Equal(1, Assert.Single(entry.Properties).Value);

            var pair2 = result.Entries.Last();
            Assert.Equal("complex", pair2.Key);
            Assert.Single(Assert.Single(pair2.Value).Properties);
        }

        [Fact]
        public void ReadOpenEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType", null, null, false, true).Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            string payload = "{\"entry\":{\"ID\":1,\"DynamicProperty\":\"DynamicValue\"}}";

            var result = this.RunParameterReaderTest(payload);
            var pair = result.Entries.First();
            Assert.Equal("entry", pair.Key);
            var entry = Assert.Single(pair.Value);
            Assert.Equal(2, entry.Properties.Count());
            var untypedValue = Assert.IsType<ODataUntypedValue>(entry.Properties.ElementAt(1).Value);
            Assert.Equal("\"DynamicValue\"", untypedValue.RawValue);
        }

        [Fact]
        public void ReadDerivedEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32);
            var dervivedType = this.referencedModel.EntityType("DerivedType", "NS", entityType).Property("Name", EdmPrimitiveTypeKind.String);
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            string payload = "{\"entry\":{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}}";

            var result = this.RunParameterReaderTest(payload);
            var pair = Assert.Single(result.Entries);
            Assert.Equal("entry", pair.Key);
            var entry = Assert.Single(pair.Value);
            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(1, entry.Properties.First().Value);
            Assert.Equal("TestName", entry.Properties.ElementAt(1).Value);
        }

        [Fact]
        public void ReadNullEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            string payload = "{\"entry\":null}";

            var result = this.RunParameterReaderTest(payload);
            var pair = Assert.Single(result.Entries);
            Assert.Equal("entry", pair.Key);
            var entry = Assert.Single(pair.Value);
            Assert.Null(entry);
        }

        [Fact]
        public void ReadFeed()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"ID\":1}]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Feeds);
            var pair = Assert.Single(result.Entries);
            Assert.Equal("feed", pair.Key);
            var entry = Assert.Single(pair.Value);
            var property = Assert.Single(entry.Properties);
            Assert.Equal(1, property.Value);
        }

        [Fact]
        public void ReadTwoFeedsWithSameType()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feedA", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("feedB", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            string payload = "{\"feedA\":[{\"ID\":1}],\"feedB\":[{\"ID\":2}]}";

            var result = this.RunParameterReaderTest(payload);

            Assert.Equal(2, result.Feeds.Count);
            Assert.Equal(2, result.Entries.Count);
            var feedA = result.Feeds.First();
            Assert.Equal("feedA", feedA.Key);
            Assert.Single(feedA.Value);
            var entryA = result.Entries.First().Value.First();
            var property = Assert.Single(entryA.Properties);
            Assert.Equal(1, property.Value);

            var feedB = result.Feeds.Last();
            Assert.Equal("feedB", feedB.Key);
            Assert.Single(feedB.Value);
            var entryB = result.Entries.ElementAt(1).Value.First();
            property = Assert.Single(entryB.Properties);
            Assert.Equal(2, property.Value);
        }

        [Fact]
        public void ReadTwoFeedsWithDifferentType()
        {
            var entityTypeA = this.referencedModel.EntityType("EntityTypeA").Property("AID", EdmPrimitiveTypeKind.Int32);
            var entityTypeB = this.referencedModel.EntityType("EntityTypeB").Property("BID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feedA", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityTypeA, false)));
            this.action.AddParameter("feedB", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityTypeB, false)));
            string payload = "{\"feedA\":[{\"AID\":1}],\"feedB\":[{\"BID\":2}]}";

            var result = this.RunParameterReaderTest(payload);

            Assert.Equal(2, result.Feeds.Count);
            Assert.Equal(2, result.Entries.Count);
            var feedA = result.Feeds.First();
            Assert.Equal("feedA", feedA.Key);
            Assert.Single(feedA.Value);
            var entryA = result.Entries.First().Value.First();
            var property = Assert.Single(entryA.Properties);
            Assert.Equal(1, property.Value);

            var feedB = result.Feeds.ElementAt(1);
            Assert.Equal("feedB", feedB.Key);
            Assert.Single(feedB.Value);
            var entryB = result.Entries.ElementAt(1).Value.First();
            property = Assert.Single(entryB.Properties);
            Assert.Equal(2, property.Value);
        }

        [Fact]
        public void ReadTwoFeedsWithinheritance()
        {
            var entityType = this.referencedModel.EntityType("EntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32);
            var dervivedType = this.referencedModel.EntityType("DerivedType", "NS", entityType).Property("Name", EdmPrimitiveTypeKind.String);

            this.action.AddParameter("feedA", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("feedB", EdmCoreModel.GetCollection(new EdmEntityTypeReference(dervivedType, false)));
            string payload = "{\"feedA\":[{\"ID\":1}],\"feedB\":[{\"ID\":2,\"Name\":\"testName\"}]}";

            var result = this.RunParameterReaderTest(payload);

            Assert.Equal(2, result.Feeds.Count);
            Assert.Equal(2, result.Entries.Count);
            var feedA = result.Feeds.First();
            Assert.Equal("feedA", feedA.Key);
            var entryA = Assert.Single(result.Entries.First().Value);
            var property = Assert.Single(entryA.Properties);
            Assert.Equal(1, property.Value);

            var feedB = result.Feeds.ElementAt(1);
            Assert.Equal("feedB", feedB.Key);
            Assert.Single(feedB.Value);
            var entryB = result.Entries.ElementAt(1).Value.First();
            Assert.Equal(2, entryB.Properties.Count());
            Assert.Equal(2, entryB.Properties.First().Value);
            Assert.Equal("testName", entryB.Properties.ElementAt(1).Value);
        }

        [Fact]
        public void ReadFeedAndEntry()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feedA", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("entryB", new EdmEntityTypeReference(entityType, false));
            string payload = "{\"feedA\":[{\"ID\":1}],\"entryB\":{\"ID\":2}}";

            var result = this.RunParameterReaderTest(payload);

            var feedA = Assert.Single(result.Feeds);
            Assert.Equal("feedA", feedA.Key);
            Assert.Single(feedA.Value);

            Assert.Equal(2, result.Entries.Count);
            var entryA = result.Entries.First().Value.First();
            var property = Assert.Single(entryA.Properties);
            Assert.Equal(1, property.Value);
            var pair = result.Entries.ElementAt(1);
            Assert.Equal("entryB", pair.Key);
            var entryB = pair.Value.First();
            property = Assert.Single(entryB.Properties);
            Assert.Equal(2, property.Value);
        }

        [Fact]
        public void ReadFeedAndComplexType()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var complexType = this.referencedModel.ComplexType("ComplexType").Property("Name", EdmPrimitiveTypeKind.String);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("complex", new EdmComplexTypeReference(complexType, false));
            string payload = "{\"feed\":[{\"ID\":1}],\"complex\":{\"Name\":\"ComplexName\"}}";

            var result = this.RunParameterReaderTest(payload);

            Assert.Single(result.Feeds);
            var feedA = result.Feeds.First();
            Assert.Equal("feed", feedA.Key);
            Assert.Single(feedA.Value);
            var entryA = result.Entries.First().Value.First();
            Assert.Equal(1, Assert.Single(entryA.Properties).Value);

            var entryB = result.Entries.Last().Value.Single();
            Assert.Equal("ComplexName", Assert.Single(entryB.Properties).Value);
        }

        [Fact]
        public void ReadFeedAndProperty()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("property", EdmCoreModel.Instance.GetString(false));
            string payload = "{\"feed\":[{\"ID\":1}],\"property\":\"value\"}";

            var result = this.RunParameterReaderTest(payload);

            var feedA = Assert.Single(result.Feeds);
            Assert.Equal("feed", feedA.Key);
            Assert.Single(feedA.Value);
            var entryA = result.Entries.First().Value.First();
            Assert.Equal(1, Assert.Single(entryA.Properties).Value);

            var item = Assert.Single(result.Values);
            Assert.Equal("property", item.Key);
            Assert.Equal("value",  item.Value);
        }

        [Fact]
        public void ReadFeedAndEnumValue()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var enumType = this.referencedModel.EnumType("Color");
            enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            enumType.AddMember("Red", new EdmEnumMemberValue(1));
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("enum", new EdmEnumTypeReference(enumType, false));
            string payload = "{\"feed\":[{\"ID\":1}],\"enum\":\"Blue\"}";

            var result = this.RunParameterReaderTest(payload);

            var feedA = Assert.Single(result.Feeds);
            Assert.Equal("feed", feedA.Key);
            Assert.Single(feedA.Value);
            var entryA = result.Entries.First().Value.First();
            Assert.Equal(1, Assert.Single(entryA.Properties).Value);
            Assert.IsType<ODataEnumValue>(Assert.Single(result.Values).Value);
        }

        [Fact]
        public void ReadFeedWithMultipleEntries()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var dervivedType = this.referencedModel.EntityType("DerivedType", "NS", entityType).Property("Name", EdmPrimitiveTypeKind.String);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"ID\":1},{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Feeds);
            var pair = result.Entries.First();
            Assert.Equal("feed", pair.Key);
            Assert.Equal(2, pair.Value.Count());
            var entry = pair.Value.First();
            Assert.Equal(1, Assert.Single(entry.Properties).Value);

            entry = pair.Value.ElementAt(1);
            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("TestName", entry.Properties.ElementAt(1).Value);
        }

        [Fact]
        public void ReadDerivedEntryInFeed()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var dervivedType = this.referencedModel.EntityType("DerivedType", "NS", entityType).Property("Name", EdmPrimitiveTypeKind.String);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Feeds);
            var pair = result.Entries.First();
            Assert.Equal("feed", pair.Key);
            var entry = Assert.Single(pair.Value);
            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("TestName", entry.Properties.ElementAt(1).Value);
        }

        [Fact]
        public void ReadNestedEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var expandEntityType = this.referencedModel.EntityType("ExpandEntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32).Property("Name", EdmPrimitiveTypeKind.String);
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.One });
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"ID\":1,\"Property1\":{\"ID\":1,\"Name\":\"TestName\"}}]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Feeds);
            var pair = result.Entries.First();
            Assert.Equal("feed", pair.Key);
            Assert.Equal(2, pair.Value.Count());
            var entry = pair.Value.First();
            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("TestName", entry.Properties.ElementAt(1).Value);
        }

        [Fact]
        public void ReadContainedEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var expandEntityType = this.referencedModel.EntityType("ExpandEntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32).Property("Name", EdmPrimitiveTypeKind.String);
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { ContainsTarget = true, Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.One });
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"ID\":1,\"Property1\":{\"ID\":1,\"Name\":\"TestName\"}}]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Feeds);
            var pair = result.Entries.First();
            Assert.Equal("feed", pair.Key);
            Assert.Equal(2, pair.Value.Count());
            var entry = pair.Value.First();
            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("TestName", entry.Properties.ElementAt(1).Value);
        }

        [Fact]
        public void ReadNestedFeed()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var expandEntityType = this.referencedModel.EntityType("ExpandEntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32).Property("Name", EdmPrimitiveTypeKind.String);
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.Many });
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"ID\":1,\"Property1\":[{\"ID\":1,\"Name\":\"TestName\"}]}]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Feeds);
            var pair = result.Entries.First();
            Assert.Equal("feed", pair.Key);
            Assert.Equal(2, pair.Value.Count());
            var entry = pair.Value.First();
            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("TestName", entry.Properties.ElementAt(1).Value);
        }

        [Fact]
        public void ReadContainedFeed()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var expandEntityType = this.referencedModel.EntityType("ExpandEntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32).Property("Name", EdmPrimitiveTypeKind.String);
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { ContainsTarget = true, Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.Many });
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"ID\":1,\"Property1\":[{\"ID\":1,\"Name\":\"TestName\"}]}]}";

            var result = this.RunParameterReaderTest(payload);
            Assert.Single(result.Feeds);
            var pair = result.Entries.First();
            Assert.Equal("feed", pair.Key);
            Assert.Equal(2, pair.Value.Count());
            var entry = pair.Value.First();
            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("TestName", entry.Properties.ElementAt(1).Value);
        }

        private ParameterReaderResult RunParameterReaderTest(string payload)
        {
            var message = new InMemoryMessage();

            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            message.SetHeader("Content-Type", "application/json");

            var parameterReaderResult = new ParameterReaderResult();
            using (var messageReader = new ODataMessageReader((IODataRequestMessage)message, null, this.model))
            {
                var parameterReader = messageReader.CreateODataParameterReader(this.action);

                while (parameterReader.Read())
                {
                    switch (parameterReader.State)
                    {
                        case ODataParameterReaderState.Value:
                            {
                                parameterReaderResult.Values.Add(new KeyValuePair<string, object>(parameterReader.Name, parameterReader.Value));
                                break;
                            }
                        case ODataParameterReaderState.Collection:
                            {
                                var collection = new ParameterReaderCollection();
                                parameterReaderResult.Collections.Add(new KeyValuePair<string, ParameterReaderCollection>(parameterReader.Name, collection));
                                var collectionReader = parameterReader.CreateCollectionReader();
                                while (collectionReader.Read())
                                {
                                    switch (collectionReader.State)
                                    {
                                        case ODataCollectionReaderState.Value:
                                            {
                                                collection.Items.Add(collectionReader.Item);
                                                break;
                                            }
                                    }
                                }
                                break;
                            }
                        case ODataParameterReaderState.Resource:
                            {
                                var entryReader = parameterReader.CreateResourceReader();

                                var entryList = new List<ODataResource>();
                                parameterReaderResult.Entries.Add(new KeyValuePair<string, IList<ODataResource>>(parameterReader.Name, entryList));
                                while (entryReader.Read())
                                {
                                    switch (entryReader.State)
                                    {
                                        case ODataReaderState.ResourceEnd:
                                            entryList.Add((ODataResource)entryReader.Item);
                                            break;

                                    }
                                }
                                break;
                            }
                        case ODataParameterReaderState.ResourceSet:
                            {
                                var entryReader = parameterReader.CreateResourceSetReader();

                                var entryList = new List<ODataResource>();
                                parameterReaderResult.Entries.Add(new KeyValuePair<string, IList<ODataResource>>(parameterReader.Name, entryList));

                                var feedList = new List<ODataResourceSet>();
                                parameterReaderResult.Feeds.Add(new KeyValuePair<string, IList<ODataResourceSet>>(parameterReader.Name, feedList));

                                while (entryReader.Read())
                                {
                                    switch (entryReader.State)
                                    {
                                        case ODataReaderState.ResourceEnd:
                                            entryList.Add((ODataResource)entryReader.Item);
                                            break;
                                        case ODataReaderState.ResourceSetEnd:
                                            feedList.Add((ODataResourceSet)entryReader.Item);
                                            break;
                                    }
                                }
                                break;
                            }
                    }
                }
            }

            return parameterReaderResult;
        }

        private class ParameterReaderResult
        {
            public IList<KeyValuePair<string, object>> Values { get; set; }
            public IList<KeyValuePair<string, ParameterReaderCollection>> Collections { get; set; }
            public IList<KeyValuePair<string, IList<ODataResource>>> Entries { get; set; }
            public IList<KeyValuePair<string, IList<ODataResourceSet>>> Feeds { get; set; }

            public ParameterReaderResult()
            {
                Values = new List<KeyValuePair<string, object>>();
                Collections = new List<KeyValuePair<string, ParameterReaderCollection>>();
                Entries = new List<KeyValuePair<string, IList<ODataResource>>>();
                Feeds = new List<KeyValuePair<string, IList<ODataResourceSet>>>();
            }
        }

        private class ParameterReaderCollection
        {
            public IList<object> Items { get; set; }

            public ParameterReaderCollection()
            {
                Items = new List<object>();
            }
        }
    }
}
