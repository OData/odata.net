//---------------------------------------------------------------------
// <copyright file="ParameterReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
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
            result.Values.Should().BeEmpty();
            result.Collections.Should().BeEmpty();
        }

        [Fact]
        public void ParameterReaderShouldReadSinglePrimitiveValue()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            const string payload = "{\"days\":4}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("days") && keyValuePair.Value.Equals(4));
        }

        [Fact]
        public void ParameterReaderShouldReadTwoPrimitiveValue()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            string payload = "{\"days\":4, \"name\":\"john\"}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().HaveCount(2);
            result.Values.Should().Contain(keyValuePair => keyValuePair.Key.Equals("days") && keyValuePair.Value.Equals(4));
            result.Values.Should().Contain(keyValuePair => keyValuePair.Key.Equals("name") && keyValuePair.Value.Equals("john"));
        }

        [Fact]
        public void ParameterReaderShouldReadSingleCollectionValue()
        {
            this.action.AddParameter("names", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));
            string payload = "{\"names\": [\"john\", \"suzy\"]}";

            var result = this.RunParameterReaderTest(payload);
            result.Collections.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("names"));
            var collectionItems = result.Collections.Single().Value.Items;
            collectionItems.Should().HaveCount(2);
            collectionItems.Should().ContainInOrder(new[] { "john", "suzy" });
        }

        [Fact]
        public void ParameterReaderShouldReadSingleComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String).Property("StreetNumber", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("address", new EdmComplexTypeReference(complexType, false));
            string payload = "{\"address\" : { \"StreetName\": \"Bla\", \"StreetNumber\" : 61 } }";

            var result = this.RunParameterReaderTest(payload);
            result.Entries.Should().HaveCount(1);
            result.Entries.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("address"));

            result.Entries.SingleOrDefault().Value.Should().OnlyContain(item => item is ODataResource);
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
            result.Entries.Should().HaveCount(1);
            result.Entries.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("address"));

            result.Entries.SingleOrDefault().Value.Should().OnlyContain(item => item is ODataResource); ;
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
            result.Feeds.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("addresses"));
            var collectioItems = result.Entries.First().Value;
            collectioItems.Should().HaveCount(2);
            collectioItems.Should().OnlyContain(item => item is ODataResource);
        }

        [Fact]
        public void ParameterReaderShouldReadCollectionOfComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String).Property("StreetNumber", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("addresses", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, false)));
            string payload = "{\"addresses\" : [{ \"StreetName\": \"Bla\", \"StreetNumber\" : 61 }, { \"StreetName\": \"Bla2\", \"StreetNumber\" : 64 }]}";

            var result = this.RunParameterReaderTest(payload);
            result.Feeds.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("addresses"));
            var collectioItems = result.Entries.First().Value;
            collectioItems.Should().HaveCount(2);
            collectioItems.Should().OnlyContain(item => item is ODataResource);
        }

        [Fact]
        public void ParameterReaderShouldReadMultiplePrimitiveValue()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("thirdParameter", EdmCoreModel.Instance.GetInt32(false));

            string payload = "{\"days\":4, \"name\":\"john\", \"thirdParameter\": 90}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().HaveCount(3);
        }

        [Fact]
        public void ParameterReaderShouldReadBeOrderInsensitive()
        {
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));

            string payload = "{\"days\":4, \"name\":\"john\"}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().HaveCount(2);
        }

        [Fact]
        public void ParameterReaderShouldReadPrimitiveValueAndPrimitiveCollection()
        {
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("hobbies", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));

            string payload = "{\"name\" : \"john\", \"hobbies\": [\"football\", \"basketball\"]}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().HaveCount(1);
            result.Collections.Should().HaveCount(1);
        }

        [Fact]
        public void ParameterReaderShouldReadTypeDefinitionValueAndTypeDefinitionCollection()
        {
            var nameType = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "NameType", EdmPrimitiveTypeKind.String), false);
            this.action.AddParameter("name", nameType);
            this.action.AddParameter("hobbies", EdmCoreModel.GetCollection(nameType));

            string payload = "{\"name\" : \"john\", \"hobbies\": [\"football\", \"basketball\"]}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().HaveCount(1);
            result.Collections.Should().HaveCount(1);
        }

        [Fact]
        public void ParameterReaderShouldConvertTypesAccordingToModel()
        {
            this.action.AddParameter("length", EdmCoreModel.Instance.GetDouble(false));

            string payload = "{\"length\":\"4.5\"}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.First().Value.Should().BeOfType<Double>();
        }

        [Fact]
        public void ReadEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            string payload = "{\"entry\":{\"ID\":1}}";

            var result = this.RunParameterReaderTest(payload);
            var pair = result.Entries.First();
            pair.Key.Should().Be("entry");
            pair.Value.Count().Should().Be(1);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(1);
            entry.Properties.First().Value.Should().Be(1);
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
            var pair = result.Entries.First();
            pair.Key.Should().Be("entry");
            pair.Value.Count().Should().Be(2);
            var complex = pair.Value.ElementAt(0);
            complex.Properties.First().Value.Should().Be("ComplexName");
            var entry = pair.Value.Last();
            entry.Properties.Count().Should().Be(1);
            entry.Properties.First().Value.Should().Be(1);
            var pair2 = result.Entries.Last();
            pair2.Key.Should().Be("complex");
            pair2.Value.Count().Should().Be(1);
            pair2.Value.Single().Properties.Count().Should().Be(1);
        }

        [Fact]
        public void ReadOpenEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType", null, null, false, true).Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            string payload = "{\"entry\":{\"ID\":1,\"DynamicProperty\":\"DynamicValue\"}}";

            var result = this.RunParameterReaderTest(payload);
            var pair = result.Entries.First();
            pair.Key.Should().Be("entry");
            pair.Value.Count().Should().Be(1);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(2);
            entry.Properties.ElementAt(1).Value.As<ODataUntypedValue>().RawValue.Should().Be("\"DynamicValue\"");
        }

        [Fact]
        public void ReadDerivedEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32);
            var dervivedType = this.referencedModel.EntityType("DerivedType", "NS", entityType).Property("Name", EdmPrimitiveTypeKind.String);
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            string payload = "{\"entry\":{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}}";

            var result = this.RunParameterReaderTest(payload);
            var pair = result.Entries.First();
            pair.Key.Should().Be("entry");
            pair.Value.Count().Should().Be(1);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(2);
            entry.Properties.First().Value.Should().Be(1);
            entry.Properties.ElementAt(1).Value.Should().Be("TestName");
        }

        [Fact]
        public void ReadNullEntity()
        {
            var entityType = this.referencedModel.EntityType("EntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            string payload = "{\"entry\":null}";

            var result = this.RunParameterReaderTest(payload);
            var pair = result.Entries.First();
            pair.Key.Should().Be("entry");
            pair.Value.Count().Should().Be(1);
            var entry = pair.Value.First();
            entry.Should().BeNull();
        }

        [Fact]
        public void ReadFeed()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"ID\":1}]}";

            var result = this.RunParameterReaderTest(payload);
            result.Feeds.Count.Should().Be(1);
            var pair = result.Entries.First();
            pair.Key.Should().Be("feed");
            pair.Value.Count().Should().Be(1);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(1);
            entry.Properties.First().Value.Should().Be(1);
        }

        [Fact]
        public void ReadTwoFeedsWithSameType()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feedA", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("feedB", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            string payload = "{\"feedA\":[{\"ID\":1}],\"feedB\":[{\"ID\":2}]}";

            var result = this.RunParameterReaderTest(payload);

            result.Feeds.Count.Should().Be(2);
            result.Entries.Count.Should().Be(2);
            var feedA = result.Feeds.First();
            feedA.Key.Should().Be("feedA");
            feedA.Value.Count().Should().Be(1);
            var entryA = result.Entries.First().Value.First();
            entryA.Properties.Count().Should().Be(1);
            entryA.Properties.First().Value.Should().Be(1);
            var feedB = result.Feeds.ElementAt(1);
            feedB.Key.Should().Be("feedB");
            feedB.Value.Count().Should().Be(1);
            var entryB = result.Entries.ElementAt(1).Value.First();
            entryB.Properties.Count().Should().Be(1);
            entryB.Properties.First().Value.Should().Be(2);
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

            result.Feeds.Count.Should().Be(2);
            result.Entries.Count.Should().Be(2);
            var feedA = result.Feeds.First();
            feedA.Key.Should().Be("feedA");
            feedA.Value.Count().Should().Be(1);
            var entryA = result.Entries.First().Value.First();
            entryA.Properties.Count().Should().Be(1);
            entryA.Properties.First().Value.Should().Be(1);
            var feedB = result.Feeds.ElementAt(1);
            feedB.Key.Should().Be("feedB");
            feedB.Value.Count().Should().Be(1);
            var entryB = result.Entries.ElementAt(1).Value.First();
            entryB.Properties.Count().Should().Be(1);
            entryB.Properties.First().Value.Should().Be(2);
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

            result.Feeds.Count.Should().Be(2);
            result.Entries.Count.Should().Be(2);
            var feedA = result.Feeds.First();
            feedA.Key.Should().Be("feedA");
            feedA.Value.Count().Should().Be(1);
            var entryA = result.Entries.First().Value.First();
            entryA.Properties.Count().Should().Be(1);
            entryA.Properties.First().Value.Should().Be(1);
            var feedB = result.Feeds.ElementAt(1);
            feedB.Key.Should().Be("feedB");
            feedB.Value.Count().Should().Be(1);
            var entryB = result.Entries.ElementAt(1).Value.First();
            entryB.Properties.Count().Should().Be(2);
            entryB.Properties.First().Value.Should().Be(2);
            entryB.Properties.ElementAt(1).Value.Should().Be("testName");
        }


        [Fact]
        public void ReadFeedAndEntry()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feedA", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("entryB", new EdmEntityTypeReference(entityType, false));
            string payload = "{\"feedA\":[{\"ID\":1}],\"entryB\":{\"ID\":2}}";

            var result = this.RunParameterReaderTest(payload);

            result.Feeds.Count.Should().Be(1);
            result.Entries.Count.Should().Be(2);
            var feedA = result.Feeds.First();
            feedA.Key.Should().Be("feedA");
            feedA.Value.Count().Should().Be(1);
            var entryA = result.Entries.First().Value.First();
            entryA.Properties.Count().Should().Be(1);
            entryA.Properties.First().Value.Should().Be(1);
            var pair = result.Entries.ElementAt(1);
            pair.Key.Should().Be("entryB");
            var entryB = pair.Value.First();
            entryB.Properties.Count().Should().Be(1);
            entryB.Properties.First().Value.Should().Be(2);
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

            result.Feeds.Count.Should().Be(1);
            var feedA = result.Feeds.First();
            feedA.Key.Should().Be("feed");
            feedA.Value.Count().Should().Be(1);
            var entryA = result.Entries.First().Value.First();
            entryA.Properties.Count().Should().Be(1);
            entryA.Properties.First().Value.Should().Be(1);

            var entryB = result.Entries.Last().Value.Single();
            entryB.Properties.Count().Should().Be(1);
            entryB.Properties.First().Value.Should().Be("ComplexName");
        }

        [Fact]
        public void ReadFeedAndProperty()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));
            this.action.AddParameter("property", EdmCoreModel.Instance.GetString(false));
            string payload = "{\"feed\":[{\"ID\":1}],\"property\":\"value\"}";

            var result = this.RunParameterReaderTest(payload);

            result.Feeds.Count.Should().Be(1);
            var feedA = result.Feeds.First();
            feedA.Key.Should().Be("feed");
            feedA.Value.Count().Should().Be(1);
            var entryA = result.Entries.First().Value.First();
            entryA.Properties.Count().Should().Be(1);
            entryA.Properties.First().Value.Should().Be(1);
            result.Values.Count().Should().Be(1);
            result.Values.Should().Contain(keyValuePair => keyValuePair.Key.Equals("property") && keyValuePair.Value.Equals("value"));
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

            result.Feeds.Count.Should().Be(1);
            var feedA = result.Feeds.First();
            feedA.Key.Should().Be("feed");
            feedA.Value.Count().Should().Be(1);
            var entryA = result.Entries.First().Value.First();
            entryA.Properties.Count().Should().Be(1);
            entryA.Properties.First().Value.Should().Be(1);
            result.Values.Count().Should().Be(1);
            result.Values.First().Value.Should().BeOfType<ODataEnumValue>();
        }

        [Fact]
        public void ReadFeedWithMultipleEntries()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var dervivedType = this.referencedModel.EntityType("DerivedType", "NS", entityType).Property("Name", EdmPrimitiveTypeKind.String);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"ID\":1},{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}]}";

            var result = this.RunParameterReaderTest(payload);
            result.Feeds.Count.Should().Be(1);
            var pair = result.Entries.First();
            pair.Key.Should().Be("feed");
            pair.Value.Count().Should().Be(2);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(1);
            entry.Properties.First().Value.Should().Be(1);

            entry = pair.Value.ElementAt(1);
            entry.Properties.Count().Should().Be(2);
            entry.Properties.ElementAt(1).Value.Should().Be("TestName");
        }

        [Fact]
        public void ReadDerivedEntryInFeed()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var dervivedType = this.referencedModel.EntityType("DerivedType", "NS", entityType).Property("Name", EdmPrimitiveTypeKind.String);
            this.action.AddParameter("feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityType, false)));

            string payload = "{\"feed\":[{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}]}";

            var result = this.RunParameterReaderTest(payload);
            result.Feeds.Count.Should().Be(1);
            var pair = result.Entries.First();
            pair.Key.Should().Be("feed");
            pair.Value.Count().Should().Be(1);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(2);
            entry.Properties.ElementAt(1).Value.Should().Be("TestName");
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
            result.Feeds.Count.Should().Be(1);
            var pair = result.Entries.First();
            pair.Key.Should().Be("feed");
            pair.Value.Count().Should().Be(2);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(2);
            entry.Properties.ElementAt(1).Value.Should().Be("TestName");
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
            result.Feeds.Count.Should().Be(1);
            var pair = result.Entries.First();
            pair.Key.Should().Be("feed");
            pair.Value.Count().Should().Be(2);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(2);
            entry.Properties.ElementAt(1).Value.Should().Be("TestName");
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
            result.Feeds.Count.Should().Be(1);
            var pair = result.Entries.First();
            pair.Key.Should().Be("feed");
            pair.Value.Count().Should().Be(2);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(2);
            entry.Properties.ElementAt(1).Value.Should().Be("TestName");
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
            result.Feeds.Count.Should().Be(1);
            var pair = result.Entries.First();
            pair.Key.Should().Be("feed");
            pair.Value.Count().Should().Be(2);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(2);
            entry.Properties.ElementAt(1).Value.Should().Be("TestName");
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
