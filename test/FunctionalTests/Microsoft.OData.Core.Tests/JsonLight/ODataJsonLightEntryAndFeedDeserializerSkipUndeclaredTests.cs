namespace Microsoft.OData.Core.Tests.JsonLight
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    // ReSharper disable RedundantUsingDirective
    using ErrorStrings = Microsoft.OData.Core.Strings;
    using Xunit;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Tests;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Core.Tests.JsonLight;
    using Microsoft.OData.Core.Json;
    // ReSharper restore RedundantUsingDirective

    public class ODataJsonLightEntryAndFeedDeserializerSkipUndeclaredTests
    {
        private ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
        {
            UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.DiscardUndeclaredValueProperty
        };

        // ----------- begin of edm for entry reader -----------
        private EdmModel serverModel;
        private EdmEntityType serverEntityType;
        private EdmEntityType serverOpenEntityType;
        private EdmEntitySet serverEntitySet;
        private EdmEntitySet serverOpenEntitySet;

        public ODataJsonLightEntryAndFeedDeserializerSkipUndeclaredTests()
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

            entry.Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties
                .FirstOrDefault(s => string.Equals("UndeclaredStreet", s.Name)).Should().BeNull();
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties
                .FirstOrDefault(s => string.Equals("UndeclaredStreetNo", s.Name)).Should().BeNull();
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

            entry.Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.FirstOrDefault(s => string.Equals("UndeclaredFloatId", s.Name)).Should().BeNull();
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties
                .FirstOrDefault(s => string.Equals("UndeclaredStreet", s.Name)).Should().BeNull();
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

            entry.Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "UndeclaredCollection1")).Should().BeNull();
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
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredFloatId")).ODataValue.FromODataValue().Should().Be(12.3d);
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
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
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
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1")).ODataValue.As<ODataComplexValue>()
                .Properties.Count().Should().Be(1);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1")).ODataValue.As<ODataComplexValue>()
                .TypeName.Should().Be("Server.NS.Address");
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
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
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "undeclaredComplex1")).Should().BeNull();
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
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
            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.Should().Be(12.3d);
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

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.Should().Be(12.3d);
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "UndeclaredCollection1")).Should().BeNull();
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
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

            entry.Properties.Count().Should().Be(3);
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "UndeclaredCollection1")).Should().BeNull();
            entry.Properties.Last().Value.As<ODataComplexValue>().Properties.Count().Should().Be(1);
        }

        #endregion
    }
}
