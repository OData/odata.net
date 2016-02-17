namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    // ReSharper disable RedundantUsingDirective
    using Microsoft.OData.Core.Tests.JsonLight;
    using Xunit;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Tests;
    // ReSharper restore RedundantUsingDirective

    public class ODataJsonLightEntryAndFeedDeserializerUndeclaredTests
    {
        private ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
        {
            UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty
        };

        private ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
        {
            UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty
        };

        // ----------- begin of edm for entry reader -----------
        private EdmModel serverModel;
        private EdmEntityType serverEntityType;
        private EdmEntityType serverOpenEntityType;
        private EdmEntitySet serverEntitySet;
        private EdmEntitySet serverOpenEntitySet;

        public ODataJsonLightEntryAndFeedDeserializerUndeclaredTests()
        {
            this.serverModel = new EdmModel();
            var addressType = new EdmComplexType("Server.NS", "Address");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            this.serverModel.AddElement(addressType);

            // non-open entity type
            this.serverEntityType = new EdmEntityType("Server.NS", "ServerEntityType");
            this.serverModel.AddElement(this.serverEntityType);
            this.serverEntityType.AddKeys(this.serverEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));

            // open entity type
            this.serverOpenEntityType = new EdmEntityType("Server.NS", "ServerOpenEntityType",
                baseType: null, isAbstract: false, isOpen: true);
            this.serverModel.AddElement(this.serverOpenEntityType);
            this.serverOpenEntityType.AddKeys(this.serverOpenEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverOpenEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));

            EdmEntityContainer container = new EdmEntityContainer("Server.NS", "container1");
            this.serverEntitySet = container.AddEntitySet("serverEntitySet", this.serverEntityType);
            this.serverOpenEntitySet = container.AddEntitySet("serverOpenEntitySet", this.serverOpenEntityType);
            this.serverModel.AddElement(container);

            this.writerSettings.SetContentType(ODataFormat.Json);
            this.writerSettings.SetServiceDocumentUri(new Uri("http://www.sampletest.com/"));
        }
        // ----------- end of edm for entry reader ----------- 

        private void ReadEntryPayload(string payload, EdmEntitySet entitySet, EdmEntityType entityType, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/json");
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, this.serverModel))
            {
                var reader = msgReader.CreateODataEntryReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        #region non-open entity's property unknown name + known value type

        [Fact]
        public void ReadNonOpenNullTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"null}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().ODataValue.As<ODataNullValue>().Should().NotBeNull();

            // roundtrip test: writer
            entry.MetadataBuilder = new Microsoft.OData.Core.Evaluation.NoOpEntityMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(payload);
        }

        [Fact]
        public void ReadNonOpenknownTypeBoolTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredBool"":false}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties
                .First(s => string.Equals("UndeclaredBool", s.Name)).Value.Should().Be(false);

            // roundtrip test: writer
            entry.MetadataBuilder = new Microsoft.OData.Core.Evaluation.NoOpEntityMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(payload);
        }

        [Fact]
        public void ReadNonOpenknownTypeStringTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet@odata.type"":""Edm.String"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name)).Value.Should().Be("No.10000000999,Zixing Rd Minhang");

            // roundtrip test: writer
            entry.MetadataBuilder = new Microsoft.OData.Core.Evaluation.NoOpEntityMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet@odata.type\":\"Edm.String\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }

        [Fact]
        public void ReadNonOpenknownTypeNumericTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreetNo@odata.type"":""Edm.Double"",""UndeclaredStreetNo"":""12""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties
                .First(s => string.Equals("UndeclaredStreetNo", s.Name)).Value.Should().Be(12d);

            // roundtrip test: writer
            entry.MetadataBuilder = new Microsoft.OData.Core.Evaluation.NoOpEntityMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreetNo@odata.type\":\"Edm.Double\",\"UndeclaredStreetNo\":12.0}}");
        }

        [Fact]
        public void ReadNonOpenKnownTypeComplexTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":{""@odata.type"":""Server.NS.Address"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(2);
            ODataComplexValue UndeclaredAddress1Value = entry.Properties.Last().Value.As<ODataComplexValue>();
            UndeclaredAddress1Value.TypeName.Should().Be("Server.NS.Address");
            UndeclaredAddress1Value.Properties.Count().Should().Be(2);
            UndeclaredAddress1Value.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name)).Value.As<ODataUndeclaredPropertyValue>()
                .RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");

            // roundtrip test: writer
            entry.MetadataBuilder = new Microsoft.OData.Core.Evaluation.NoOpEntityMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }

        [Fact]
        public void ReadNonOpenKnownTypeCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1@odata.type:""Collection(Edm.String)"",UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).ODataValue.As<ODataCollectionValue>().Items
                .Cast<string>().Count().Should().Be(3);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);

            // roundtrip test: writer
            entry.MetadataBuilder = new Microsoft.OData.Core.Evaluation.NoOpEntityMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"UndeclaredCollection1@odata.type\":\"Collection(Edm.String)\",\"UndeclaredCollection1\":[\"email1@163.com\",\"email2@gmail.com\",\"email3@gmail2.com\"],\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }
        #endregion

        #region non-open entity's property unknown name + unknown value type

        [Fact]
        public void ReadNonOpenUnknownNullTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"null,""UndeclaredAddress1@odata.type"":""Server.NS.UndefComplex1""}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUndeclaredPropertyValue>().RawValue.Should().Be("null");

            // roundtrip test: writer
            entry.MetadataBuilder = new Microsoft.OData.Core.Evaluation.NoOpEntityMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"Server.NS.UndefComplex1\",\"UndeclaredAddress1\":null}");
        }

        [Fact]
        public void ReadNonOpenUnknownTypePrimitiveTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name)).Value.As<ODataUndeclaredPropertyValue>().RawValue.Should().Be("12.3"); // numeric
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name)).Value.As<ODataUndeclaredPropertyValue>() // string
                .RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUndeclaredPropertyValue>().RawValue
                .Should().Be(@"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexNestedTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""inerProp2"":'abc'},""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUndeclaredPropertyValue>().RawValue
                .Should().Be(@"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""inerProp2"":'abc'},""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUndeclaredPropertyValue>().RawValue
              .Should().Be(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]");
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
        }

        #endregion

        #region open entity's property unknown name + known value type

        [Fact]
        public void ReadOpenEntryUndeclaredPropertiesWithNullValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""UndeclaredType1"":null}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(3);
            Assert.True(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredType1")).ODataValue.IsNullValue);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredComplexPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{""@odata.type"":""Server.NS.Address"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(4);
            var tmp = entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"));
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1")).ODataValue.As<ODataComplexValue>()
                .Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredCollectionPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1@odata.type:""Collection(Edm.String)"",UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).ODataValue.As<ODataCollectionValue>().Items
                .Cast<string>().Count().Should().Be(3);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
        }
        #endregion

        #region open entity's property unknown name + unknown value type

        [Fact]
        public void ReadOpenEntryUndeclaredComplexPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUndeclaredPropertyValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredComplexInvalidTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{""@odata.type"":""Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });
            entry.Properties.Last().Value.As<ODataUndeclaredPropertyValue>().RawValue
                .Should().Be(@"{""@odata.type"":""Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}");
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyComplexPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1")).Value.As<ODataUndeclaredPropertyValue>()
                .RawValue.Should().Be(@"{}");
        }

        [Fact]
        public void ReadOpenEntryUndeclaredCollectionPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUndeclaredPropertyValue>().RawValue
                .Should().Be(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]");
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyCollectionPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataEntry entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataEntry;
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUndeclaredPropertyValue>()
                .RawValue.Should().Be(@"[]");
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet"))
                .Value.As<ODataUndeclaredPropertyValue>().RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");
        }

        #endregion

        #region writer methods for roundtrip testing

        private string WriteEntryPayload(EdmEntitySet entitySet, EdmEntityType entityType, Action<ODataWriter> action)
        {
            MemoryStream stream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using (var msgReader = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, this.serverModel))
            {
                var writer = msgReader.CreateODataEntryWriter(entitySet, entityType);
                action(writer);

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                return payload;
            }
        }

        #endregion
    }
}
