//---------------------------------------------------------------------
// <copyright file="ParameterReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Tests the use of ODataParameterReader class when the payload is JSON.
    /// 
    /// TODO: For error tests, see Microsoft.Test.Taupo.OData.Reader.Tests.Reader.ParameterReaderTests. 
    /// These should eventually be migrated here.
    /// </summary>
    [TestClass]
    public class ParameterReaderTests
    {
        private EdmModel referencedModel;
        private IEdmModel model;
        private EdmAction action;

        [TestInitialize]
        public void Init()
        {
            referencedModel = new EdmModel();
            referencedModel.Fixup();
            this.action = new EdmAction("FQ.NS", "ActionImport", null);
            referencedModel.AddElement(this.action);
            this.model = TestUtils.WrapReferencedModelsToMainModel("TestModel", "DefaultContainer", referencedModel);
        }

        [TestMethod]
        public void ParameterReaderShouldReadEmptyJson()
        {
            string payload = "{}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().BeEmpty();
            result.Collections.Should().BeEmpty();
        }

        [TestMethod]
        public void ParameterReaderShouldReadSinglePrimitiveValue()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            const string payload = "{\"days\":4}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("days") && keyValuePair.Value.Equals(4));
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void ParameterReaderShouldReadSingleComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String).Property("StreetNumber", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("address", new EdmComplexTypeReference(complexType, false));
            string payload = "{\"address\" : { \"StreetName\": \"Bla\", \"StreetNumber\" : 61 } }";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("address"));
            var complexValue = result.Values.Single().Value;
            complexValue.Should().BeOfType<ODataComplexValue>();
        }

        [TestMethod]
        public void ParameterReaderShouldReadSingleDerivedComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String);
            var derivedComplexType = new EdmComplexType("TestModel", "derivedAddress", complexType, false);
            derivedComplexType.AddStructuralProperty("StreetNumber", EdmPrimitiveTypeKind.Int32, false);
            this.referencedModel.AddElement(derivedComplexType);
            
            this.action.AddParameter("address", new EdmComplexTypeReference(complexType, false));
            string payload = "{\"address\" : { \"StreetName\": \"Bla\", \"StreetNumber\" : 61, \"@odata.type\":\"TestModel.derivedAddress\" } }";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("address"));
            var complexValue = result.Values.Single().Value;
            complexValue.Should().BeOfType<ODataComplexValue>();
        }

        [TestMethod]
        public void ParameterReaderShouldReadCollectionOfDerivedComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String);
            var derivedComplexType = new EdmComplexType("TestModel", "derivedAddress", complexType, false);
            derivedComplexType.AddStructuralProperty("StreetNumber", EdmPrimitiveTypeKind.Int32, false);
            this.referencedModel.AddElement(derivedComplexType);
            
            this.action.AddParameter("addresses", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, false)));
            string payload = "{\"addresses\" : [{ \"StreetName\": \"Bla\", \"StreetNumber\" : 61, \"@odata.type\":\"TestModel.derivedAddress\" }, { \"StreetName\": \"Bla2\" }]}";

            var result = this.RunParameterReaderTest(payload);
            result.Collections.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("addresses"));
            var collectioItems = result.Collections.Single().Value.Items;
            collectioItems.Should().HaveCount(2);
            collectioItems.Should().OnlyContain(item => item is ODataComplexValue);
        }

        [TestMethod]
        public void ParameterReaderShouldReadCollectionOfComplexValue()
        {
            var complexType = this.referencedModel.ComplexType("address").Property("StreetName", EdmPrimitiveTypeKind.String).Property("StreetNumber", EdmPrimitiveTypeKind.Int32);
            this.action.AddParameter("addresses", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, false)));
            string payload = "{\"addresses\" : [{ \"StreetName\": \"Bla\", \"StreetNumber\" : 61 }, { \"StreetName\": \"Bla2\", \"StreetNumber\" : 64 }]}";

            var result = this.RunParameterReaderTest(payload);
            result.Collections.Should().OnlyContain(keyValuePair => keyValuePair.Key.Equals("addresses"));
            var collectioItems = result.Collections.Single().Value.Items;
            collectioItems.Should().HaveCount(2);
            collectioItems.Should().OnlyContain(item => item is ODataComplexValue);
        }

        [TestMethod]
        public void ParameterReaderShouldReadMultiplePrimitiveValue()
        {
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("thirdParameter", EdmCoreModel.Instance.GetInt32(false));

            string payload = "{\"days\":4, \"name\":\"john\", \"thirdParameter\": 90}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().HaveCount(3);
        }

        [TestMethod]
        public void ParameterReaderShouldReadBeOrderInsensitive()
        {
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("days", EdmCoreModel.Instance.GetInt32(false));

            string payload = "{\"days\":4, \"name\":\"john\"}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().HaveCount(2);
        }

        [TestMethod]
        public void ParameterReaderShouldReadPrimitiveValueAndPrimitiveCollection()
        {
            this.action.AddParameter("name", EdmCoreModel.Instance.GetString(false));
            this.action.AddParameter("hobbies", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));

            string payload = "{\"name\" : \"john\", \"hobbies\": [\"football\", \"basketball\"]}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.Should().HaveCount(1);
            result.Collections.Should().HaveCount(1);
        }

        [TestMethod]
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

        [TestMethod]
        public void ParameterReaderShouldConvertTypesAccordingToModel()
        {
            this.action.AddParameter("length", EdmCoreModel.Instance.GetDouble(false));

            string payload = "{\"length\":\"4.5\"}";

            var result = this.RunParameterReaderTest(payload);
            result.Values.First().Value.Should().BeOfType<Double>();
        }

        [TestMethod]
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

        [TestMethod]
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
            pair.Value.Count().Should().Be(1);
            var entry = pair.Value.First();
            entry.Properties.Count().Should().Be(2);
            entry.Properties.First().Value.Should().Be(1);
            entry.Properties.ElementAt(1).Value.Should().BeOfType<ODataComplexValue>();
            result.Values.Count().Should().Be(1);
            result.Values.First().Value.Should().BeOfType<ODataComplexValue>();
        }

        [TestMethod]
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
            entry.Properties.ElementAt(1).Value.Should().Be("DynamicValue");
        }

        [TestMethod]
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

        [TestMethod]
        [Ignore]
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
            entry.Properties.Count().Should().Be(2);
            entry.Properties.First().Value.Should().Be(1);
            entry.Properties.ElementAt(1).Value.Should().Be("TestName");
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void ReadContainedFeed()
        {
            var entityType = this.referencedModel.EntityType("EntityType").Property("ID", EdmPrimitiveTypeKind.Int32);
            var expandEntityType = this.referencedModel.EntityType("ExpandEntityType", "NS").Property("ID", EdmPrimitiveTypeKind.Int32).Property("Name", EdmPrimitiveTypeKind.String);
            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() {ContainsTarget = true, Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.Many });
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
                        case ODataParameterReaderState.Entry:
                        {
                            var entryReader = parameterReader.CreateEntryReader();

                            var entryList = new List<ODataEntry>();
                            parameterReaderResult.Entries.Add(new KeyValuePair<string, IList<ODataEntry>>(parameterReader.Name, entryList));
                            while (entryReader.Read())
                            {
                                switch (entryReader.State)
                                {
                                    case ODataReaderState.EntryEnd:
                                        entryList.Add((ODataEntry)entryReader.Item);
                                        break;

                                }
                            }
                            break;
                        }
                        case ODataParameterReaderState.Feed:
                        {
                            var entryReader = parameterReader.CreateFeedReader();
                            
                            var entryList = new List<ODataEntry>();
                            parameterReaderResult.Entries.Add(new KeyValuePair<string, IList<ODataEntry>>(parameterReader.Name, entryList));

                            var feedList = new List<ODataFeed>();
                            parameterReaderResult.Feeds.Add(new KeyValuePair<string, IList<ODataFeed>>(parameterReader.Name, feedList));

                            while (entryReader.Read())
                            {
                                switch (entryReader.State)
                                {
                                    case ODataReaderState.EntryEnd:
                                        entryList.Add((ODataEntry)entryReader.Item);
                                        break;
                                    case ODataReaderState.FeedEnd:
                                        feedList.Add((ODataFeed)entryReader.Item);
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
            public IList<KeyValuePair<string, IList<ODataEntry>>> Entries { get; set; }
            public IList<KeyValuePair<string, IList<ODataFeed>>> Feeds { get; set; } 

            public ParameterReaderResult()
            {
                Values = new List<KeyValuePair<string, object>>();
                Collections = new List<KeyValuePair<string, ParameterReaderCollection>>();
                Entries = new List<KeyValuePair<string, IList<ODataEntry>>>();
                Feeds = new List<KeyValuePair<string, IList<ODataFeed>>>();
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
