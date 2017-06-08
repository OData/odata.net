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
    using Microsoft.OData.Tests.JsonLight;
    using Xunit;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Tests;
    // ReSharper restore RedundantUsingDirective

    public class ODataJsonLightEntryAndFeedDeserializerUndeclaredTests
    {
        private static ODataMessageReaderSettings UntypedAsStringReaderSettings = new ODataMessageReaderSettings
        {
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
        };

        private static ODataMessageReaderSettings UntypedAsValueReaderSettings = new ODataMessageReaderSettings
        {
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType & ~ValidationKinds.ThrowIfTypeConflictsWithMetadata,
            ReadUntypedAsString = false
        };

        private ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
        {
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
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
            addressType.AddProperty(new EdmStructuralProperty(addressType, "MyEdmUntypedProp3", EdmCoreModel.Instance.GetUntyped()));
            this.serverModel.AddElement(addressType);

            // non-open entity type
            this.serverEntityType = new EdmEntityType("Server.NS", "ServerEntityType");
            this.serverModel.AddElement(this.serverEntityType);
            this.serverEntityType.AddKeys(this.serverEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));
            this.serverEntityType.AddStructuralProperty("MyEdmUntypedProp1", EdmCoreModel.Instance.GetUntyped());

            // open entity type
            this.serverOpenEntityType = new EdmEntityType("Server.NS", "ServerOpenEntityType",
                baseType: null, isAbstract: false, isOpen: true);
            this.serverModel.AddElement(this.serverOpenEntityType);
            this.serverOpenEntityType.AddKeys(this.serverOpenEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverOpenEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));
            this.serverOpenEntityType.AddStructuralProperty("MyEdmUntypedProp2", EdmCoreModel.Instance.GetUntyped());

            EdmEntityContainer container = new EdmEntityContainer("Server.NS", "container1");
            this.serverEntitySet = container.AddEntitySet("serverEntitySet", this.serverEntityType);
            this.serverOpenEntitySet = container.AddEntitySet("serverOpenEntitySet", this.serverOpenEntityType);
            this.serverModel.AddElement(container);

            this.writerSettings.SetContentType(ODataFormat.Json);
            this.writerSettings.SetServiceDocumentUri(new Uri("http://www.sampletest.com/"));
        }
        // ----------- end of edm for entry reader ----------- 

        private void ReadEntryPayload(string payload, EdmEntitySet entitySet, EdmEntityType entityType, Action<ODataReader> action, bool readUntypedAsValue = false)
        {
            ODataMessageReaderSettings readerSettings = readUntypedAsValue ? UntypedAsValueReaderSettings : UntypedAsStringReaderSettings;
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/json");
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, this.serverModel))
            {
                var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
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
            ODataResource entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataResource;
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().ODataValue.As<ODataUntypedValue>().RawValue.Should().Be("null");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
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
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
            complex1.Properties.First(s => string.Equals("UndeclaredBool", s.Name)).Value.Should().Be(false);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be(payload);
        }

        [Fact]
        public void ReadNonOpenknownTypeStringTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet@odata.type"":""Edm.String"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
            complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name)).Value.Should().Be("No.10000000999,Zixing Rd Minhang");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }

        [Fact]
        public void ReadNonOpenknownTypeNumericTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreetNo@odata.type"":""#Double"",""UndeclaredStreetNo"":""12""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
            complex1.Properties
                .First(s => string.Equals("UndeclaredStreetNo", s.Name)).Value.Should().Be(12d);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreetNo@odata.type\":\"#Double\",\"UndeclaredStreetNo\":12.0}}");
        }

        [Fact]
        public void ReadNonOpenKnownTypeComplexTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":{""@odata.type"":""Server.NS.Address"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(1);
            complex1.TypeName.Should().Be("Server.NS.Address");
            complex1.Properties.Count().Should().Be(2);
            complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name)).Value.As<ODataUntypedValue>()
                .RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "UndeclaredAddress1" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1\":{\"@odata.type\":\"#Server.NS.Address\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }

        [Fact]
        public void ReadNonOpenKnownTypeCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1@odata.type:""#Collection(String)"",UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).ODataValue.As<ODataCollectionValue>().Items
                .Cast<string>().Count().Should().Be(3);
            complex1.Properties.Count().Should().Be(2);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"UndeclaredCollection1@odata.type\":\"#Collection(String)\",\"UndeclaredCollection1\":[\"email1@163.com\",\"email2@gmail.com\",\"email3@gmail2.com\"],\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }
        #endregion

        #region non-open entity's property unknown name + unknown value type

        [Fact]
        public void ReadNonOpenUnknownNullTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"null,""UndeclaredAddress1@odata.type"":""#Server.NS.UndefComplex1""}";
            ODataResource entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataResource;
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUntypedValue>().RawValue.Should().Be("null");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#Server.NS.UndefComplex1\",\"UndeclaredAddress1\":null}");
        }

        [Fact]
        public void ReadNonOpenUnknownTypePrimitiveTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name)).Value.As<ODataUntypedValue>().RawValue.Should().Be("12.3"); // numeric
            complex1.Properties.Count().Should().Be(2);
            complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name)).Value.As<ODataUntypedValue>() // string
                .RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataResource entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataResource;
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"{""@odata.type"":""Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexNestedTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""inerProp2"":'abc'},""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"{""@odata.type"":""Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""inerProp2"":""abc""},""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUntypedValue>().RawValue
              .Should().Be(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]");
            complex1.Properties.Count().Should().Be(2);
        }

        #endregion

        #region non-open entity's property unknown name + unknown value type - readAsValue

        [Fact]
        public void ReadNonOpenUnknownNullAsValueTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"null,""UndeclaredAddress1@odata.type"":""#Server.NS.UndefComplex1""}";
            ODataResource entry = null;
            ODataResource undeclaredAddress1 = null;
            ODataNestedResourceInfo undeclaredAddress1NestedInfo = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredAddress1 == null)
                    {
                        undeclaredAddress1 = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    undeclaredAddress1NestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(1);
            undeclaredAddress1NestedInfo.TypeAnnotation.TypeName.Should().Be("Server.NS.UndefComplex1");
            undeclaredAddress1.Should().Be(null);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(undeclaredAddress1NestedInfo);
                writer.WriteStart(undeclaredAddress1);
                writer.WriteEnd(); // undeclaredAddress
                writer.WriteEnd(); // nested info
                writer.WriteEnd(); // entry
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#Server.NS.UndefComplex1\",\"UndeclaredAddress1\":null}");
        }

        [Fact]
        public void ReadNonOpenInvalidTypePrimitiveAsValueTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredDecimal@odata.type"":""#Server.NS.UndeclaredDecimalType"",""UndeclaredDecimal"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet@odata.type"":""Server.NS.UndeclaredStreetType"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(2);
            ODataProperty decimalProperty = entry.Properties.First(s => string.Equals("UndeclaredDecimal", s.Name));
            decimalProperty.Value.Should().Be(12.3M);
            decimalProperty.TypeAnnotation.TypeName.Should().Be("Server.NS.UndeclaredDecimalType");
            complex1.Properties.Count().Should().Be(2);
            ODataProperty addressProperty = complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name));
            addressProperty.Value.Should().Be("No.10000000999,Zixing Rd Minhang");
            addressProperty.TypeAnnotation.TypeName.Should().Be("Server.NS.UndeclaredStreetType");
        }

        [Fact]
        public void ReadNonOpenUnknownTypePrimitiveAsValueTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(2);
            entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name)).Value.Should().Be(12.3M);
            complex1.Properties.Count().Should().Be(2);
            complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name)).Value.Should().Be("No.10000000999,Zixing Rd Minhang");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexAsValueTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataResource entry = null;
            ODataResource undeclaredAddress1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredAddress1 == null)
                    {
                        undeclaredAddress1 = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(1);
            undeclaredAddress1.TypeName.Should().Be("Server.NS.AddressInValid");
            undeclaredAddress1.Properties.Count().Should().Be(2);
            undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name)).Value.Should().Be("No.10000000999,Zixing Rd Minhang");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeInvalidComplexNestedAsValueTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""innerProp2"":'abc'},""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataResource entry = null;
            ODataResource undeclaredAddress1 = null;
            ODataResource innerComplex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredAddress1 == null)
                    {
                        undeclaredAddress1 = (reader.Item as ODataResource);
                    }
                    else if (innerComplex1 == null)
                    {
                        innerComplex1 = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(1);
            undeclaredAddress1.TypeName.Should().Be("Server.NS.AddressInValid");
            undeclaredAddress1.Properties.Count().Should().Be(2);
            undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name)).Value.Should().Be("No.10000000999,Zixing Rd Minhang");
            undeclaredAddress1.Properties.First(s => string.Equals("Street", s.Name)).Value.Should().Be("No.999,Zixing Rd Minhang");
            innerComplex1.Properties.Count().Should().Be(2);
            innerComplex1.Properties.First(s => string.Equals("innerProp1", s.Name)).Value.Should().Be(null);
            innerComplex1.Properties.First(s => string.Equals("innerProp2", s.Name)).Value.Should().Be("abc");
        }

        [Fact]
        public void ReadNonOpenUnknownTypeCollectionAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource address = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (address == null)
                    {
                        address = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(3);
            ODataCollectionValue untypedCollection = entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataCollectionValue>();
            untypedCollection.Items.Count().Should().Be(3);
            String.Concat(untypedCollection.Items).Should().Be("email1@163.comemail2@gmail.comemail3@gmail2.com");
            address.Properties.Count().Should().Be(2);
            address.Properties.First(s => string.Equals("Street", s.Name)).Value.Should().Be("No.999,Zixing Rd Minhang");
            address.Properties.First(s => string.Equals("UndeclaredStreet", s.Name)).Value.Should().Be("No.10000000999,Zixing Rd Minhang");
        }

        [Fact]
        public void ReadNonOpenUnknownComplexTypeCollectionWithInvalidTypeAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""UndeclaredCollection1@odata.type"":""Collection(Server.NS.UnknownCollectionType)"",""UndeclaredCollection1"":[{""email"":""email1@163.com""},{""email"":""email2@gmail.com""},{""email"":""email3@gmail2.com""}],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource address = null;
            ODataNestedResourceInfo undeclaredCollection1 = null;
            List<ODataResource> untypedCollection = new List<ODataResource>();
            bool insideCollection = false;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (insideCollection)
                    {
                        untypedCollection.Add(reader.Item as ODataResource);
                    }
                    else if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (address == null)
                    {
                        address = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    ODataNestedResourceInfo nestedInfo = (reader.Item as ODataNestedResourceInfo);
                    if (insideCollection = nestedInfo.IsCollection == true)
                    {
                        undeclaredCollection1 = nestedInfo;
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    ODataNestedResourceInfo nestedInfo = (reader.Item as ODataNestedResourceInfo);
                    insideCollection = !(nestedInfo.IsCollection == true);
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(2);
//            ODataCollectionValue untypedCollectionProperty = entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataCollectionValue>();
//            untypedCollectionProperty.TypeName.Should().Be("Collection(Server.NS.UnkonwnCollectionType)");
//            String.Concat(untypedCollectionProperty.Items.Select(c => ((ODataResource)c).Properties.Single(p => string.Equals(p.Name, "email")).Value)).Should().Be("email1@163.comemail2@gmail.comemail3@gmail2.com");
            untypedCollection.Count().Should().Be(3);
            String.Concat(untypedCollection.Select(c=>((ODataResource)c).Properties.Single(p => string.Equals(p.Name, "email")).Value)).Should().Be("email1@163.comemail2@gmail.comemail3@gmail2.com");
            address.Properties.Count().Should().Be(2);
            address.Properties.First(s => string.Equals("Street", s.Name)).Value.Should().Be("No.999,Zixing Rd Minhang");
            address.Properties.First(s => string.Equals("UndeclaredStreet", s.Name)).Value.Should().Be("No.10000000999,Zixing Rd Minhang");
        }
        #endregion

        #region open entity's property unknown name + known value type

        [Fact]
        public void ReadOpenEntryUndeclaredPropertiesWithNullValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""UndeclaredType1"":null}";
            ODataResource entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataResource;
            });

            entry.Properties.Count().Should().Be(3);
            Assert.Equal("null", entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredType1")).ODataValue.As<ODataUntypedValue>().RawValue);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredComplexPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{""@odata.type"":""Server.NS.Address"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(2);
            complex1.Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredCollectionPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1@odata.type:""Collection(Edm.String)"",UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).ODataValue.As<ODataCollectionValue>().Items
                .Cast<string>().Count().Should().Be(3);
            complex1.Properties.Count().Should().Be(2);
        }
        #endregion

        #region open entity's property unknown name + unknown value type

        [Fact]
        public void ReadOpenEntryUndeclaredComplexPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            complex1.Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredComplexInvalidTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{""@odata.type"":""Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });
            entry.Properties.Last().Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"{""@odata.type"":""Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}");
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyComplexPropertiesTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{}}";
            ODataResource entry = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                entry = reader.Item as ODataResource;
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1")).Value.As<ODataUntypedValue>()
                .RawValue.Should().Be(@"{}");
        }

        [Fact]
        public void ReadOpenEntryUndeclaredCollectionPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]");
            complex1.Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyCollectionPropertiesWithoutODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataUntypedValue>()
                .RawValue.Should().Be(@"[]");
            complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"""No.10000000999,Zixing Rd Minhang""");
        }

        #endregion

        #region open entity's property unknown name + unknown value type - readAsValue

        [Fact]
        public void ReadOpenEntryUndeclaredComplexPropertiesWithoutODataTypeAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource undeclaredComplex = null;
            ODataResource address = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredComplex == null)
                    {
                        undeclaredComplex = (reader.Item as ODataResource);
                    }
                    else if (address == null)
                    {
                        address = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(2);
            undeclaredComplex.Properties.Count().Should().Be(2);
            undeclaredComplex.Properties.Single(p => p.Name == "MyProp1").Value.Should().Be("aaaaaaaaa");
            undeclaredComplex.Properties.Single(p => p.Name == "UndeclaredProp1").Value.Should().Be("bbbbbbb");
            address.Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredComplexInvalidTypeAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{""@odata.type"":""Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource undeclaredComplex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredComplex1 == null)
                    {
                        undeclaredComplex1 = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);
            entry.Properties.Count().Should().Be(2);
            undeclaredComplex1.TypeName.Should().Be("Server.NS.AddressUndeclared");
            undeclaredComplex1.Properties.Count().Should().Be(1);
            undeclaredComplex1.Properties.Single(p => p.Name == "Street").Value.Should().Be("No.999,Zixing Rd Minhang");
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyComplexPropertiesAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                ""undeclaredComplex1"":{}}";
            ODataResource entry = null;
            ODataResource undeclaredComplex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredComplex1 == null)
                    {
                        undeclaredComplex1 = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(2);
            undeclaredComplex1.Properties.Count().Should().Be(0);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredCollectionPropertiesWithoutODataTypeAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource address = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (address == null)
                    {
                        address = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(3);
            ODataCollectionValue untypedCollection = entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataCollectionValue>();
            untypedCollection.Items.Count().Should().Be(3);
            String.Concat(untypedCollection.Items).Should().Be("email1@163.comemail2@gmail.comemail3@gmail2.com");
            address.Properties.Count().Should().Be(2);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyCollectionPropertiesWithoutODataTypeAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1")).Value.As<ODataCollectionValue>().Items.Count().Should().Be(0);
            complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet")).Value.Should().Be("No.10000000999,Zixing Rd Minhang");
        }

        #endregion

        #region declared Edm.Untyped property
        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
                                   MyEdmUntypedProp1:{""MyProp12"":""bbb222"",abc:null}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp1\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }

        [Fact]
        public void ReadOpenEntryEdmUntypedPropertyTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
                                   MyEdmUntypedProp2:{""MyProp12"":""bbb222"",abc:null}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp2"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }

        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyInComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang"",MyEdmUntypedProp3:{""MyProp12"":""bbb222"",abc:null}}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            complex1.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp3"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\",\"MyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null}}}");
        }
        #endregion

        #region undeclared Edm.Untyped property with odata.type
        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
            ""UndeclaredMyEdmUntypedProp1@odata.type"":""Edm.Untyped"",UndeclaredMyEdmUntypedProp1:{""MyProp12"":""bbb222"",abc:null}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            ODataResource complex2 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                    else if (complex2 == null)
                    {
                        complex2 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1")).
                Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1")).TypeAnnotation.TypeName.Should().Be("Edm.Untyped");
            complex2.Should().BeNull();

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp1@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp1\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }

        [Fact]
        public void ReadOpenEntryEdmUntypedPropertyODataTypeTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
            ""UndeclaredMyEdmUntypedProp2@odata.type"":""Edm.Untyped"",UndeclaredMyEdmUntypedProp2:{""MyProp12"":""bbb222"",""abc"":null}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(4);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2")).
                Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2")).TypeAnnotation.TypeName.Should().Be("Edm.Untyped");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp2@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}");
        }

        [Fact]
        public void ReadNonOpenEntryEdmUntypedPropertyODataTypeInComplexTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                  ""undeclaredComplex1"":{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang"",""UndeclaredMyEdmUntypedProp3@odata.type"":""Edm.Untyped"",UndeclaredMyEdmUntypedProp3:{""MyProp12"":""bbb222"",abc:null}}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (complex1 == null)
                    {
                        complex1 = (reader.Item as ODataResource);
                    }
                }
            });

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}");
            complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3"))
                .Value.As<ODataUntypedValue>().RawValue.Should().Be(@"{""MyProp12"":""bbb222"",""abc"":null}");
            complex1.Properties
                .Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3")).TypeAnnotation.TypeName.Should().Be("Edm.Untyped");

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(new ODataNestedResourceInfo() { Name = "Address" });
                writer.WriteStart(complex1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            result.Should().Be("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\",\"UndeclaredMyEdmUntypedProp3@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null}}}");
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
                var writer = msgReader.CreateODataResourceWriter(entitySet, entityType);
                action(writer);

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                return payload;
            }
        }

        #endregion

    }
}
