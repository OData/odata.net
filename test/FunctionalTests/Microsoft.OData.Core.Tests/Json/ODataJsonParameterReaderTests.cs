//---------------------------------------------------------------------
// <copyright file="ODataJsonParameterReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.Test.OData.Utils.ODataLibTest;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Tests the use of ODataJsonParameterReader class when the payload is JSON.
    ///
    /// TODO: For error tests, see Microsoft.Test.Taupo.OData.Reader.Tests.Reader.ParameterReaderTests.
    /// These should eventually be migrated here.
    /// </summary>
    public class ODataJsonParameterReaderTests
    {
        private EdmModel referencedModel;
        private IEdmModel model;
        private EdmAction action;

        public ODataJsonParameterReaderTests()
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
            var property = Assert.IsType<ODataProperty>(Assert.Single(entry.Properties));
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
            Assert.Equal("ComplexName", Assert.IsType<ODataProperty>(Assert.Single(complex.Properties)).Value);
            var entry = pair.Value.Last();
            Assert.Equal(1, Assert.IsType<ODataProperty>(Assert.Single(entry.Properties)).Value);

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
            var untypedValue = Assert.IsType<ODataUntypedValue>(entry.Properties.OfType<ODataProperty>().ElementAt(1).Value);
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
            var properties = entry.Properties.OfType<ODataProperty>();
            Assert.Equal(1, properties.ElementAt(0).Value);
            Assert.Equal("TestName", properties.ElementAt(1).Value);
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
            var property = Assert.IsType<ODataProperty>(Assert.Single(entry.Properties));
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
            var property = Assert.IsType<ODataProperty>(Assert.Single(entryA.Properties));
            Assert.Equal(1, property.Value);

            var feedB = result.Feeds.Last();
            Assert.Equal("feedB", feedB.Key);
            Assert.Single(feedB.Value);
            var entryB = result.Entries.ElementAt(1).Value.First();
            property = Assert.IsType<ODataProperty>(Assert.Single(entryB.Properties));
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
            var property = Assert.IsType<ODataProperty>(Assert.Single(entryA.Properties));
            Assert.Equal(1, property.Value);

            var feedB = result.Feeds.ElementAt(1);
            Assert.Equal("feedB", feedB.Key);
            Assert.Single(feedB.Value);
            var entryB = result.Entries.ElementAt(1).Value.First();
            property = Assert.IsType<ODataProperty>(Assert.Single(entryB.Properties));
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
            var property = Assert.IsType<ODataProperty>(Assert.Single(entryA.Properties));
            Assert.Equal(1, property.Value);

            var feedB = result.Feeds.ElementAt(1);
            Assert.Equal("feedB", feedB.Key);
            Assert.Single(feedB.Value);
            var entryB = result.Entries.ElementAt(1).Value.First();
            Assert.Equal(2, entryB.Properties.Count());
            Assert.Equal(2, Assert.IsType<ODataProperty>(entryB.Properties.First()).Value);
            Assert.Equal("testName", Assert.IsType<ODataProperty>(entryB.Properties.ElementAt(1)).Value);
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
            var property = Assert.IsType<ODataProperty>(Assert.Single(entryA.Properties));
            Assert.Equal(1, property.Value);
            var pair = result.Entries.ElementAt(1);
            Assert.Equal("entryB", pair.Key);
            var entryB = pair.Value.First();
            property = Assert.IsType<ODataProperty>(Assert.Single(entryB.Properties));
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
            Assert.Equal(1, Assert.IsType<ODataProperty>(Assert.Single(entryA.Properties)).Value);

            var entryB = result.Entries.Last().Value.Single();
            Assert.Equal("ComplexName", Assert.IsType<ODataProperty>(Assert.Single(entryB.Properties)).Value);
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
            Assert.Equal(1, Assert.IsType<ODataProperty>(Assert.Single(entryA.Properties)).Value);

            var item = Assert.Single(result.Values);
            Assert.Equal("property", item.Key);
            Assert.Equal("value", item.Value);
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
            Assert.Equal(1, Assert.IsType<ODataProperty>(Assert.Single(entryA.Properties)).Value);
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
            Assert.Equal(1, Assert.IsType<ODataProperty>(Assert.Single(entry.Properties)).Value);

            entry = pair.Value.ElementAt(1);
            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("TestName", Assert.IsType<ODataProperty>(entry.Properties.ElementAt(1)).Value);
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
            Assert.Equal("TestName", Assert.IsType<ODataProperty>(entry.Properties.ElementAt(1)).Value);
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
            Assert.Equal("TestName", Assert.IsType<ODataProperty>(entry.Properties.ElementAt(1)).Value);
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
            Assert.Equal("TestName", Assert.IsType<ODataProperty>(entry.Properties.ElementAt(1)).Value);
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
            Assert.Equal("TestName", Assert.IsType<ODataProperty>(entry.Properties.ElementAt(1)).Value);
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
            Assert.Equal("TestName", Assert.IsType<ODataProperty>(entry.Properties.ElementAt(1)).Value);
        }

        [Fact]
        public async Task ReadEmptyParameterPayloadAsync()
        {
            var payload = "{}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                async (jsonParameterReader) =>
                {
                    await DoReadAsync(jsonParameterReader);
                    
                    Assert.Equal(ODataParameterReaderState.Completed, jsonParameterReader.State);
                });
        }

        [Fact]
        public async Task ReadPrimitiveParameterAsync()
        {
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));

            var payload = "{\"rating\":4}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(jsonParameterReader, verifyValueAction: (value) => Assert.Equal(4, value)));
        }

        [Fact]
        public async Task ReadEmptyNullableNonOptionalParameterAsync()
        {
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(true));

            var payload = "{ }";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(jsonParameterReader));
        }

        [Fact]
        public async Task ReadEmptyNonNullableNonOptionalParameterAsync_ThrowsException()
        {
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));

            var payload = "{ }";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonParameterReaderAndRunTestAsync(
                    payload,
                    (jsonParameterReader) => DoReadAsync(jsonParameterReader)));

            Assert.Equal(
                Strings.ODataParameterReaderCore_ParametersMissingInPayload("ActionImport", "rating"),
                exception.Message);
        }

        [Fact]
        public async Task ReadOptionalParameterAsync()
        {
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddOptionalParameter("comment", EdmCoreModel.Instance.GetString(false));

            var payload = "{\"rating\":4,\"comment\":\"Great product!\"}";

            var valueActionStack = new Stack<object>(new object[] { "Great product!", 4 });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyValueAction: (value) =>
                    {
                        Assert.NotEmpty(valueActionStack);
                        Assert.Equal(valueActionStack.Pop(), value);
                    }));

            Assert.Empty(valueActionStack);
        }

        [Fact]
        public async Task ReadParameterPayalodWithMissingOptionalParameterAsync()
        {
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddOptionalParameter("comment", EdmCoreModel.Instance.GetString(false));

            var payload = "{\"rating\":4}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(jsonParameterReader, verifyValueAction: (value) => Assert.Equal(4, value)));
        }

        [Fact]
        public async Task ReadMultiplePrimitiveParametersAsync()
        {
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddParameter("comment", EdmCoreModel.Instance.GetString(false));

            var payload = "{\"rating\":4,\"comment\":\"Great product!\"}";

            var valueActionStack = new Stack<object>(new object[] { "Great product!", 4 });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyValueAction: (value) =>
                    {
                        Assert.NotEmpty(valueActionStack);
                        Assert.Equal(valueActionStack.Pop(), value);
                    }));

            Assert.Empty(valueActionStack);
        }

        [Fact]
        public async Task ReadPrimitiveCollectionParameterAsync()
        {
            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            this.action.AddParameter("ratings", parameterEdmTypeReference);

            var payload = "{\"ratings\":[4,3]}";

            var collectionItemsStack = new Stack<int>(new int[] { 3, 4 });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyCollectionAction: async (collectionReader) =>
                    {
                        while (await collectionReader.ReadAsync())
                        {
                            switch(collectionReader.State)
                            {
                                case ODataCollectionReaderState.Value:
                                    Assert.NotEmpty(collectionItemsStack);
                                    Assert.Equal(collectionItemsStack.Pop(), collectionReader.Item);
                                    break;
                            }
                        }
                    }));
        }

        [Fact]
        public async Task ReadRandomlyOrderedParametersAsync()
        {
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddParameter("comment", EdmCoreModel.Instance.GetString(false));

            var payload = "{\"comment\":\"Great product!\",\"rating\":4}";

            var verifyValueActionStack = new Stack<object>(new object[] { 4, "Great product!" });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyValueAction: (value) =>
                    {
                        Assert.NotEmpty(verifyValueActionStack);
                        Assert.Equal(verifyValueActionStack.Pop(), value);
                    }));

            Assert.Empty(verifyValueActionStack);
        }

        [Fact]
        public async Task ReadPrimitiveAndPrimitiveCollectionParametersAsync()
        {
            this.action.AddParameter("comment", EdmCoreModel.Instance.GetString(false));
            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)));
            this.action.AddParameter("ratings", parameterEdmTypeReference);

            var payload = "{\"comment\":\"Great product!\",\"ratings\":[4,3]}";

            var collectionItemsStack = new Stack<int>(new int[] { 3, 4 });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyValueAction: (value) => Assert.Equal("Great product!", value),
                    verifyCollectionAction: async (collectionReader) =>
                    {
                        while (await collectionReader.ReadAsync())
                        {
                            switch (collectionReader.State)
                            {
                                case ODataCollectionReaderState.Value:
                                    Assert.NotEmpty(collectionItemsStack);
                                    Assert.Equal(collectionItemsStack.Pop(), collectionReader.Item);
                                    break;
                            }
                        }
                    }));
        }

        [Fact]
        public async Task ReadTypeDefinitionAndTypeDefinitionCollectionParametersAsync()
        {
            var moneyTypeDefinition = new EdmTypeDefinition("NS", "Money", EdmPrimitiveTypeKind.Decimal);
            this.referencedModel.AddElement(moneyTypeDefinition);

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmTypeDefinitionReference(moneyTypeDefinition, false)));
            this.action.AddParameter("price", new EdmTypeDefinitionReference(moneyTypeDefinition, false));
            this.action.AddParameter("discounts", parameterEdmTypeReference);

            var payload = "{\"price\":13.5,\"discounts\":[1.5,2.5]}";

            var collectionItemsStack = new Stack<decimal>(new decimal[] { 2.5M, 1.5M });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyValueAction: (value) => Assert.Equal(13.5M, value),
                    verifyCollectionAction: async (collectionReader) =>
                    {
                        while (await collectionReader.ReadAsync())
                        {
                            switch (collectionReader.State)
                            {
                                case ODataCollectionReaderState.Value:
                                    Assert.NotEmpty(collectionItemsStack);
                                    Assert.Equal(collectionItemsStack.Pop(), collectionReader.Item);
                                    break;
                            }
                        }
                    }));
        }

        [Fact]
        public async Task ReadParameterWithTypeDetectionAsync()
        {
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetDouble(false));

            var payload = "{\"rating\":3.5}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyValueAction: (value) =>
                    {
                        Assert.IsType<double>(value);
                        Assert.Equal(3.5D, value);
                    }));
        }

        [Fact]
        public async Task ReadComplexParameterAsync()
        {
            var addressComplexType = new EdmComplexType("NS", "Address");
            addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(addressComplexType);

            this.action.AddParameter("address", new EdmComplexTypeReference(addressComplexType, false));

            var payload = "{\"address\":{\"Street\":\"One Way\"}}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotNull(resource);
                            Assert.Equal("NS.Address", resource.TypeName);
                            var streetProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                            Assert.Equal("Street", streetProperty.Name);
                            Assert.Equal("One Way", streetProperty.Value);
                        })));
        }

        [Fact]
        public async Task ReadDerivedComplexParameterAsync()
        {
            var addressComplexType = new EdmComplexType("NS", "Address");
            addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            var buildingAddressComplexType = new EdmComplexType("NS", "BuildingAddress", addressComplexType);
            buildingAddressComplexType.AddStructuralProperty("Building", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(buildingAddressComplexType);

            this.action.AddParameter("address", new EdmComplexTypeReference(addressComplexType, false));

            var payload = "{\"address\":{\"@odata.type\":\"#NS.BuildingAddress\",\"Building\":\"Studio A\",\"Street\":\"One Way\"}}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotNull(resource);
                            Assert.Equal("NS.BuildingAddress", resource.TypeName);
                            Assert.Equal(2, resource.Properties.Count());
                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                            Assert.Equal("Building", properties[0].Name);
                            Assert.Equal("Studio A", properties[0].Value);
                            Assert.Equal("Street", properties[1].Name);
                            Assert.Equal("One Way", properties[1].Value);
                        })));
        }

        [Fact]
        public async Task ReadComplexCollectionParameterAsync()
        {
            var addressComplexType = new EdmComplexType("NS", "Address");
            addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(addressComplexType);

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressComplexType, false)));
            this.action.AddParameter("addresses", parameterEdmTypeReference);

            var payload = "{\"addresses\":[{\"Street\":\"One Way\"},{\"Street\":\"Two Way\"}]}";
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Address", resource.TypeName);
                var streetProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Street", streetProperty.Name);
                Assert.Equal("Two Way", streetProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Address", resource.TypeName);
                var streetProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Street", streetProperty.Name);
                Assert.Equal("One Way", streetProperty.Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotEmpty(verifyResourceActionStack);
                            var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                            innerVerifyResourceAction(resource);
                        })));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadDerivedComplexCollectionParameterAsync()
        {
            var addressComplexType = new EdmComplexType("NS", "Address");
            addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            var buildingAddressComplexType = new EdmComplexType("NS", "BuildingAddress", addressComplexType);
            buildingAddressComplexType.AddStructuralProperty("Building", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(buildingAddressComplexType);

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressComplexType, false)));
            this.action.AddParameter("addresses", parameterEdmTypeReference);

            var payload = "{\"addresses\":[{\"@odata.type\":\"#NS.BuildingAddress\",\"Building\":\"Studio A\",\"Street\":\"One Way\"},{\"Street\":\"Two Way\"}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Address", resource.TypeName);
                var streetProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Street", streetProperty.Name);
                Assert.Equal("Two Way", streetProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.BuildingAddress", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Building", properties[0].Name);
                Assert.Equal("Studio A", properties[0].Value);
                Assert.Equal("Street", properties[1].Name);
                Assert.Equal("One Way", properties[1].Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotEmpty(verifyResourceActionStack);
                            var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                            innerVerifyResourceAction(resource);
                        })));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadEntityParameterAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(customerEntityType);

            this.action.AddParameter("customer", new EdmEntityTypeReference(customerEntityType, false));

            var payload = "{\"customer\":{\"Id\":1,\"Name\":\"Sue\"}}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotNull(resource);
                            Assert.Equal("NS.Customer", resource.TypeName);
                            Assert.Equal(2, resource.Properties.Count());
                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                            Assert.Equal("Id", properties[0].Name);
                            Assert.Equal(1, properties[0].Value);
                            Assert.Equal("Name", properties[1].Name);
                            Assert.Equal("Sue", properties[1].Value);
                        })));
        }

        [Fact]
        public async Task ReadNullEntityParameterAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.referencedModel.AddElement(customerEntityType);

            this.action.AddParameter("customer", new EdmEntityTypeReference(customerEntityType, false));

            var payload = "{\"customer\":null}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) => Assert.Null(resource))));
        }

        [Fact]
        public async Task ReadDerivedEntityParameterAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var enterpriseCustomerEntityType = new EdmEntityType("NS", "EnterpriseCustomer", customerEntityType);
            enterpriseCustomerEntityType.AddStructuralProperty("CreditLimit", EdmPrimitiveTypeKind.Decimal);
            this.referencedModel.AddElement(enterpriseCustomerEntityType);

            this.action.AddParameter("customer", new EdmEntityTypeReference(customerEntityType, false));

            var payload = "{\"customer\":{\"@odata.type\":\"#NS.EnterpriseCustomer\",\"Id\":1,\"CreditLimit\":170}}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotNull(resource);
                            Assert.Equal("NS.EnterpriseCustomer", resource.TypeName);
                            Assert.Equal(2, resource.Properties.Count());
                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                            Assert.Equal("Id", properties[0].Name);
                            Assert.Equal(1, properties[0].Value);
                            Assert.Equal("CreditLimit", properties[1].Name);
                            Assert.Equal(170M, properties[1].Value);
                        })));
        }

        [Fact]
        public async Task ReadOpenEntityParameterAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer", baseType: null, isAbstract: false, isOpen: true);
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.referencedModel.AddElement(customerEntityType);

            this.action.AddParameter("customer", new EdmEntityTypeReference(customerEntityType, false));

            var payload = "{\"customer\":{\"Id\":1,\"DynamicProp@odata.type\":\"#Edm.Decimal\",\"DynamicProp\":310}}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotNull(resource);
                            Assert.Equal("NS.Customer", resource.TypeName);
                            Assert.Equal(2, resource.Properties.Count());
                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                            Assert.Equal("Id", properties[0].Name);
                            Assert.Equal(1, properties[0].Value);
                            Assert.Equal("DynamicProp", properties[1].Name);
                            Assert.Equal(310M, properties[1].Value);
                        })));
        }

        [Fact]
        public async Task ReadEntityCollectionParameterAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(customerEntityType);

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(customerEntityType, false)));
            this.action.AddParameter("customers", parameterEdmTypeReference);

            var payload = "{\"customers\":[{\"Id\":1,\"Name\":\"Sue\"}]}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotNull(resource);
                            Assert.Equal("NS.Customer", resource.TypeName);
                            Assert.Equal(2, resource.Properties.Count());
                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                            Assert.Equal("Id", properties[0].Name);
                            Assert.Equal(1, properties[0].Value);
                            Assert.Equal("Name", properties[1].Name);
                            Assert.Equal("Sue", properties[1].Value);
                        })));
        }

        [Fact]
        public async Task ReadMultipleEntityCollectionParametersAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.referencedModel.AddElement(customerEntityType);

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(customerEntityType, false)));
            this.action.AddParameter("customer1", parameterEdmTypeReference);
            this.action.AddParameter("customer2", parameterEdmTypeReference);

            var payload = "{\"customer1\":[{\"Id\":1}],\"customer2\":[{\"Id\":2}]}";

            var resourceSetCount = 0;
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(2, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: async (jsonReader) =>
                    {
                        resourceSetCount++;
                        await DoReadAsync(
                            jsonReader,
                            verifyResourceAction: (resource) =>
                            {
                                Assert.NotEmpty(verifyResourceActionStack);
                                var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                                innerVerifyResourceAction(resource);
                            });
                    }));

            Assert.Equal(2, resourceSetCount);
            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadDerivedEntityCollectionParametersAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var enterpriseCustomerEntityType = new EdmEntityType("NS", "EnterpriseCustomer", customerEntityType);
            enterpriseCustomerEntityType.AddStructuralProperty("CreditLimit", EdmPrimitiveTypeKind.Decimal);
            this.referencedModel.AddElement(enterpriseCustomerEntityType);

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(customerEntityType, false)));
            this.action.AddParameter("customer1", parameterEdmTypeReference);
            this.action.AddParameter("customer2", parameterEdmTypeReference);

            var payload = "{\"customer1\":[{\"Id\":1}],\"customer2\":[{\"@odata.type\":\"#NS.EnterpriseCustomer\",\"Id\":2,\"CreditLimit\":130}]}";

            var resourceSetCount = 0;
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.EnterpriseCustomer", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(2, properties[0].Value);
                Assert.Equal("CreditLimit", properties[1].Name);
                Assert.Equal(130M, properties[1].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: async (jsonReader) =>
                    {
                        resourceSetCount++;
                        await DoReadAsync(
                            jsonReader,
                            verifyResourceAction: (resource) =>
                            {
                                Assert.NotEmpty(verifyResourceActionStack);
                                var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                                innerVerifyResourceAction(resource);
                            });
                    }));

            Assert.Equal(2, resourceSetCount);
            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadPrimitiveAndEntityParametersAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.referencedModel.AddElement(customerEntityType);

            this.action.AddParameter("customer", new EdmEntityTypeReference(customerEntityType, false));
            this.action.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));

            var payload = "{\"customer\":{\"Id\":1},\"rating\":4}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyValueAction: (value) => Assert.Equal(4, value),
                    verifyResourceAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotNull(resource);
                            Assert.Equal("NS.Customer", resource.TypeName);
                            var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                            Assert.Equal("Id", idProperty.Name);
                            Assert.Equal(1, idProperty.Value);
                        })));
        }

        [Fact]
        public async Task ReadEnumAndEntityParametersAsync()
        {
            var colorEnumType = new EdmEnumType("NS", "Color");
            colorEnumType.AddMember("Black", new EdmEnumMemberValue(1));
            colorEnumType.AddMember("White", new EdmEnumMemberValue(2));
            this.referencedModel.AddElement(colorEnumType);
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.referencedModel.AddElement(customerEntityType);

            this.action.AddParameter("customer", new EdmEntityTypeReference(customerEntityType, false));
            this.action.AddParameter("favoriteColor", new EdmEnumTypeReference(colorEnumType, false));

            var payload = "{\"customer\":{\"Id\":1},\"favoriteColor\":\"Black\"}";

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyValueAction: (value) =>
                    {
                        var favoriteColor = Assert.IsType<ODataEnumValue>(value);
                        Assert.Equal("Black", favoriteColor.Value);
                    },
                    verifyResourceAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotNull(resource);
                            Assert.Equal("NS.Customer", resource.TypeName);
                            var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                            Assert.Equal("Id", idProperty.Name);
                            Assert.Equal(1, idProperty.Value);
                        })));
        }

        [Fact]
        public async Task ReadComplexAndEntityParametersAsync()
        {
            var addressComplexType = new EdmComplexType("NS", "Address");
            addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(addressComplexType);
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            customerEntityType.AddStructuralProperty("PhysicalAddress", new EdmComplexTypeReference(addressComplexType, false));
            this.referencedModel.AddElement(customerEntityType);

            this.action.AddParameter("customer", new EdmEntityTypeReference(customerEntityType, false));
            this.action.AddParameter("address", new EdmComplexTypeReference(addressComplexType, false));

            var payload = "{\"customer\":{\"Id\":1,\"PhysicalAddress\":{\"Street\":\"One Way\"}},\"address\":{\"Street\":\"Two Way\"}}";
            
            var outerResourceCount = 0;
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Address", resource.TypeName);
                var streetProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Street", streetProperty.Name);
                Assert.Equal("Two Way", streetProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Address", resource.TypeName);
                var streetProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Street", streetProperty.Name);
                Assert.Equal("One Way", streetProperty.Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceAction: async (jsonReader) =>
                    {
                        outerResourceCount++;
                        await DoReadAsync(
                            jsonReader,
                            verifyResourceAction: (resource) =>
                            {
                                Assert.NotEmpty(verifyResourceActionStack);
                                var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                                innerVerifyResourceAction(resource);
                            });
                    }));

            Assert.Equal(2, outerResourceCount);
            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadEntityAndEntityCollectionParametersAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.referencedModel.AddElement(customerEntityType);

            this.action.AddParameter("customer", new EdmEntityTypeReference(customerEntityType, false));
            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(customerEntityType, false)));
            this.action.AddParameter("competitors", parameterEdmTypeReference);

            var payload = "{\"customer\":{\"Id\":1},\"competitors\":[{\"Id\":2}]}";

            var resourceCount = 0;
            var resourceSetCount = 0;
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(2, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: async (jsonReader) =>
                    {
                        resourceSetCount++;
                        await DoReadAsync(
                            jsonReader,
                            verifyResourceAction: (resource) =>
                            {
                                Assert.NotEmpty(verifyResourceActionStack);
                                var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                                innerVerifyResourceAction(resource);
                            });
                    },
                    verifyResourceAction: async (jsonReader) =>
                    {
                        resourceCount++;
                        await DoReadAsync(
                            jsonReader,
                            verifyResourceAction: (resource) =>
                            {
                                Assert.NotEmpty(verifyResourceActionStack);
                                var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                                innerVerifyResourceAction(resource);
                            });
                    }));

            Assert.Equal(1, resourceSetCount);
            Assert.Equal(1, resourceCount);
            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadEntityCollectionParameterWithNestedEntityAsync()
        {
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var orderEntityType = new EdmEntityType("NS", "Order");
            orderEntityType.AddKeys(orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Customer", Target = customerEntityType, TargetMultiplicity = EdmMultiplicity.One });

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(orderEntityType, false)));
            this.action.AddParameter("orders", parameterEdmTypeReference);

            var payload = "{\"orders\":[{\"Id\":7,\"Customer\":{\"Id\":1}}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(7, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotEmpty(verifyResourceActionStack);
                            var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                            innerVerifyResourceAction(resource);
                        })));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadEntityCollectionParameterWithNestedContainedEntityAsync()
        {
            var nextOfKinEntityType = new EdmEntityType("NS", "NextOfKin");
            nextOfKinEntityType.AddKeys(nextOfKinEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            customerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "NextOfKin", Target = nextOfKinEntityType, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(customerEntityType, false)));
            this.action.AddParameter("customers", parameterEdmTypeReference);

            var payload = "{\"customers\":[{\"Id\":1,\"NextOfKin\":{\"Id\":13}}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.NextOfKin", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(13, idProperty.Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotEmpty(verifyResourceActionStack);
                            var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                            innerVerifyResourceAction(resource);
                        })));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadEntityCollectionParameterWithNestedContainedEntityCollectionAsync()
        {
            var nextOfKinEntityType = new EdmEntityType("NS", "Subsidiary");
            nextOfKinEntityType.AddKeys(nextOfKinEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            customerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Subsidiaries", Target = nextOfKinEntityType, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });

            var parameterEdmTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(customerEntityType, false)));
            this.action.AddParameter("customers", parameterEdmTypeReference);

            var payload = "{\"customers\":[{\"Id\":1,\"Subsidiaries\":[{\"Id\":13}]}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Subsidiary", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(13, idProperty.Value);
            });

            await SetupJsonParameterReaderAndRunTestAsync(
                payload,
                (jsonParameterReader) => DoReadAsync(
                    jsonParameterReader,
                    verifyResourceSetAction: (jsonReader) => DoReadAsync(
                        jsonReader,
                        verifyResourceAction: (resource) =>
                        {
                            Assert.NotEmpty(verifyResourceActionStack);
                            var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                            innerVerifyResourceAction(resource);
                        })));

            Assert.Empty(verifyResourceActionStack);
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

        private ODataJsonInputContext CreateJsonInputContext(string payload, bool isAsync = false, bool isResponse = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.Default,
                IsResponse = isResponse,
                IsAsync = isAsync,
                Model = this.model
            };

            return new ODataJsonInputContext(new StringReader(payload), messageInfo, new ODataMessageReaderSettings());
        }

        /// <summary>
        /// Sets up an ODataJsonParameterReader, then runs the given test code asynchronously
        /// </summary>
        private async Task SetupJsonParameterReaderAndRunTestAsync(
            string payload,
            Func<ODataJsonParameterReader, Task> func)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, isAsync: true, isResponse: false))
            {
                var jsonParameterReader = new ODataJsonParameterReader(
                    jsonInputContext,
                    this.action);

                await func(jsonParameterReader);
            }
        }

        private async Task DoReadAsync(
            ODataJsonParameterReader jsonParameterReader,
            Action<object> verifyValueAction = null,
            Func<ODataCollectionReader, Task> verifyCollectionAction = null,
            Func<ODataJsonReader, Task> verifyResourceAction = null,
            Func<ODataJsonReader, Task> verifyResourceSetAction = null)
        {
            while (await jsonParameterReader.ReadAsync())
            {
                switch(jsonParameterReader.State)
                {
                    case ODataParameterReaderState.Value:
                        if (verifyValueAction != null)
                        {
                            verifyValueAction(jsonParameterReader.Value);
                        }

                        break;
                    case ODataParameterReaderState.Collection:
                        if (verifyCollectionAction != null)
                        {
                            await verifyCollectionAction(await jsonParameterReader.CreateCollectionReaderAsync());
                        }

                        break;
                    case ODataParameterReaderState.Resource:
                        if (verifyResourceAction != null)
                        {
                            await verifyResourceAction(await jsonParameterReader.CreateResourceReaderAsync() as ODataJsonReader);
                        }

                        break;
                    case ODataParameterReaderState.ResourceSet:
                        if (verifyResourceSetAction != null)
                        {
                            await verifyResourceSetAction(await jsonParameterReader.CreateResourceSetReaderAsync() as ODataJsonReader);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private async Task DoReadAsync(
           ODataJsonReader jsonReader,
           Action<ODataResourceSet> verifyResourceSetAction = null,
           Action<ODataResource> verifyResourceAction = null)
        {
            while (await jsonReader.ReadAsync())
            {
                switch (jsonReader.State)
                {
                    case ODataReaderState.ResourceSetStart:
                        break;
                    case ODataReaderState.ResourceSetEnd:
                        if (verifyResourceSetAction != null)
                        {
                            verifyResourceSetAction(jsonReader.Item as ODataResourceSet);
                        }

                        break;
                    case ODataReaderState.ResourceStart:
                        break;
                    case ODataReaderState.ResourceEnd:
                        if (verifyResourceAction != null)
                        {
                            verifyResourceAction(jsonReader.Item as ODataResource);
                        }

                        break;
                    default:
                        break;
                }
            }
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
