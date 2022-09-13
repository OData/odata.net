//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntryAndFeedDeSerializerUndeclaredTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
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

            // enum type
            var enumType = new EdmEnumType("Server.NS", "EnumType");
            enumType.AddMember(new EdmEnumMember(enumType, "Member", new EdmEnumMemberValue(1)));
            this.serverModel.AddElement(enumType);

            EdmEntityContainer container = new EdmEntityContainer("Server.NS", "container1");
            this.serverEntitySet = container.AddEntitySet("serverEntitySet", this.serverEntityType);
            this.serverOpenEntitySet = container.AddEntitySet("serverOpenEntitySet", this.serverOpenEntityType);
            this.serverModel.AddElement(container);

            this.writerSettings.SetContentType(ODataFormat.Json);
            this.writerSettings.SetServiceDocumentUri(new Uri("http://www.sampletest.com/"));
        }
        // ----------- end of edm for entry reader ----------- 

        private void ReadEntryPayload(string payload, EdmEntitySet entitySet, EdmEntityType entityType, Action<ODataReader> action, bool readUntypedAsValue = false, bool readRequest = false)
        {
            ODataMessageReaderSettings readerSettings = readUntypedAsValue ? UntypedAsValueReaderSettings : UntypedAsStringReaderSettings;
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/json");

            using (var msgReader = readRequest ?
                new ODataMessageReader((IODataRequestMessage)message, readerSettings, this.serverModel) :
                new ODataMessageReader((IODataResponseMessage)message, readerSettings, this.serverModel))
            {
                var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private void ReadCollectionPayload(string payload, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/json");
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, UntypedAsValueReaderSettings, this.serverModel))
            {
                var reader = msgReader.CreateODataResourceSetReader();
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private void ReadResourcePayload(string payload, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/json");
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, UntypedAsValueReaderSettings, this.serverModel))
            {
                var reader = msgReader.CreateODataResourceReader();
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("null", (entry.Properties.Last().ODataValue as ODataUntypedValue).RawValue);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            Assert.Equal(payload, result);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal(false, complex1.Properties.First(s => string.Equals("UndeclaredBool", s.Name, StringComparison.Ordinal)).Value);

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

            Assert.Equal(payload, result);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal("No.10000000999,Zixing Rd Minhang", complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal(12d, complex1.Properties
                .First(s => string.Equals("UndeclaredStreetNo", s.Name, StringComparison.Ordinal)).Value);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreetNo@odata.type\":\"#Double\",\"UndeclaredStreetNo\":12.0}}", result);
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

            Assert.Single(entry.Properties);
            Assert.Equal("Server.NS.Address", complex1.TypeName);
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""",
                (complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1\":{\"@odata.type\":\"#Server.NS.Address\",\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(3, (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).ODataValue as ODataCollectionValue).Items.Count());
            Assert.Equal(2, complex1.Properties.Count());

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"UndeclaredCollection1@odata.type\":\"#Collection(String)\",\"UndeclaredCollection1\":[\"email1@163.com\",\"email2@gmail.com\",\"email3@gmail2.com\"],\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("null", (entry.Properties.Last().Value as ODataUntypedValue).RawValue);

            entry.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(entry);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#Server.NS.UndefComplex1\",\"UndeclaredAddress1\":null}", result);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("12.3", (entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name, StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue); // numeric
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""",
                (complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value as ODataUntypedValue) // string
                .RawValue);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(@"{""@odata.type"":""Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}",
                (entry.Properties.Last().Value as ODataUntypedValue).RawValue);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(@"{""@odata.type"":""Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""innerComplex1"":{""innerProp1"":null,""inerProp2"":""abc""},""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}",
                (entry.Properties.Last().Value as ODataUntypedValue).RawValue);
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]",
                (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(2, complex1.Properties.Count());
        }

        #endregion

        #region non-open entity's property unknown name + unknown value type - readAsValue

        [Fact]
        public void ReadNonOpenUnknownNullAsValueTest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,"
                +@"""UndeclaredAddress1@odata.type"":""#Server.NS.UndefComplex1"",""UndeclaredAddress1"":null}";
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

            Assert.Single(entry.Properties);
            Assert.Equal("Server.NS.UndefComplex1", undeclaredAddress1NestedInfo.TypeAnnotation.TypeName);
            Assert.Null(undeclaredAddress1);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredAddress1@odata.type\":\"#Server.NS.UndefComplex1\",\"UndeclaredAddress1\":null}", result);
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

            Assert.Equal(2, entry.Properties.Count());
            ODataProperty decimalProperty = entry.Properties.First(s => string.Equals("UndeclaredDecimal", s.Name, StringComparison.Ordinal));
            Assert.Equal(12.3M, decimalProperty.Value);
            Assert.Equal("Server.NS.UndeclaredDecimalType", decimalProperty.TypeAnnotation.TypeName);
            Assert.Equal(2, complex1.Properties.Count());
            ODataProperty addressProperty = complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal));
            Assert.Equal("No.10000000999,Zixing Rd Minhang", addressProperty.Value);
            Assert.Equal("Server.NS.UndeclaredStreetType", addressProperty.TypeAnnotation.TypeName);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(12.3M, entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name, StringComparison.Ordinal)).Value);
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal("No.10000000999,Zixing Rd Minhang", complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value);
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

            Assert.Single(entry.Properties);
            Assert.Equal("Server.NS.AddressInValid", undeclaredAddress1.TypeName);
            Assert.Equal(2, undeclaredAddress1.Properties.Count());
            Assert.Equal("No.10000000999,Zixing Rd Minhang", undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value);
        }

        [Fact]
        public void ReadNonOpenWithoutTypeComplexAsValueTest_InRequest()
        {
            // non-open entity's unknown property type including string & numeric values
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
                + @"{'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
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
            },
            /*readUntypedAsValue*/ true,
            /*readRequest*/ true);

            Assert.Single(entry.Properties);
            Assert.Equal("Edm.Untyped", undeclaredAddress1.TypeName);
            Assert.Equal(2, undeclaredAddress1.Properties.Count());
            Assert.Equal("No.10000000999,Zixing Rd Minhang", undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value);
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

            Assert.Single(entry.Properties);
            Assert.Equal("Server.NS.AddressInValid", undeclaredAddress1.TypeName);
            Assert.Equal(2, undeclaredAddress1.Properties.Count());
            Assert.Equal("No.10000000999,Zixing Rd Minhang", undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value);
            Assert.Equal("No.999,Zixing Rd Minhang", undeclaredAddress1.Properties.First(s => string.Equals("Street", s.Name, StringComparison.Ordinal)).Value);
            Assert.Equal(2, innerComplex1.Properties.Count());
            Assert.Null(innerComplex1.Properties.First(s => string.Equals("innerProp1", s.Name, StringComparison.Ordinal)).Value);
            Assert.Equal("abc", innerComplex1.Properties.First(s => string.Equals("innerProp2", s.Name, StringComparison.Ordinal)).Value);
        }

        [Fact]
        public void ReadNonOpenUnknownTypeCollectionAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                UndeclaredCollection1:[null,""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource address = null;
            ODataNestedResourceInfo undeclaredCollection1 = null;
            ODataNestedResourceInfo addressNestedInfo = null;
            List<object> undeclaredCollection1Items = new List<object>();
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
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    if (undeclaredCollection1 == null)
                    {
                        undeclaredCollection1 = (reader.Item as ODataNestedResourceInfo);
                    }
                    else
                    {
                        addressNestedInfo = (reader.Item as ODataNestedResourceInfo);
                    }
                }
                else if (reader.State == ODataReaderState.Primitive)
                {
                    undeclaredCollection1Items.Add(reader.Item == null ? null : ((ODataPrimitiveValue)(reader.Item)).Value);
                }
            }, /*readUntypedAsValue*/ true);

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("UndeclaredCollection1", undeclaredCollection1.Name);
            Assert.Equal(3, undeclaredCollection1Items.Count());
            Assert.Equal("email1@163.com", undeclaredCollection1Items.First());
            Assert.Equal("email1@163.comemail2@gmail.comemail3@gmail2.com", String.Concat(undeclaredCollection1Items));
            Assert.Equal("Address", addressNestedInfo.Name);
            Assert.Equal("Server.NS.Address", address.TypeName);
            Assert.Equal(2, address.Properties.Count());
            Assert.Equal("No.999,Zixing Rd Minhang", address.Properties.First(s => string.Equals("Street", s.Name, StringComparison.Ordinal)).Value);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", address.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value);
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
                else if (reader.State == ODataReaderState.ResourceSetStart)
                {
                    undeclaredCollection1.TypeAnnotation = new ODataTypeAnnotation((reader.Item as ODataResourceSet).TypeName);
                }
            }, /*readUntypedAsValue*/ true);

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("Collection(Server.NS.UnknownCollectionType)", undeclaredCollection1.TypeAnnotation.TypeName);
            Assert.Equal(3, untypedCollection.Count());
            Assert.Equal("email1@163.comemail2@gmail.comemail3@gmail2.com", String.Concat(untypedCollection.Select(c=>((ODataResource)c).Properties.Single(p => string.Equals(p.Name, "email", StringComparison.Ordinal)).Value)));
            Assert.Equal(2, address.Properties.Count());
            Assert.Equal("No.999,Zixing Rd Minhang", address.Properties.First(s => string.Equals("Street", s.Name, StringComparison.Ordinal)).Value);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", address.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value);
        }

        [Fact]
        public void ReadNonOpenUnknownComplexTypeCollectionWithInvalidTypeAsValueTest_InRequest()
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
                else if (reader.State == ODataReaderState.ResourceSetStart)
                {
                    undeclaredCollection1.TypeAnnotation = new ODataTypeAnnotation((reader.Item as ODataResourceSet).TypeName);
                }
            },
            /*readUntypedAsValue*/ true,
            /*readRequest*/ true);

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("Collection(Server.NS.UnknownCollectionType)", undeclaredCollection1.TypeAnnotation.TypeName);
            Assert.Equal(3, untypedCollection.Count());
            Assert.Equal("email1@163.comemail2@gmail.comemail3@gmail2.com", String.Concat(untypedCollection.Select(c => ((ODataResource)c).Properties.Single(p => string.Equals(p.Name, "email", StringComparison.Ordinal)).Value)));
            Assert.Equal(2, address.Properties.Count());
            Assert.Equal("No.999,Zixing Rd Minhang", address.Properties.First(s => string.Equals("Street", s.Name, StringComparison.Ordinal)).Value);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", address.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)).Value);
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal("null", (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredType1", StringComparison.Ordinal)).ODataValue as ODataUntypedValue).RawValue);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, complex1.Properties.Count());
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(3, (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).ODataValue as ODataCollectionValue).Items
                .Count());
            Assert.Equal(2, complex1.Properties.Count());
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}", (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
            Assert.Equal(2, complex1.Properties.Count());
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
            Assert.Equal(@"{""@odata.type"":""Server.NS.AddressUndeclared"",""Street"":""No.999,Zixing Rd Minhang""}",
                (entry.Properties.Last().Value as ODataUntypedValue).RawValue);
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"{}", (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue)
                .RawValue);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredPrimitiveCollectionPropertiesWithoutODataTypeTest()
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]",
                (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(2, complex1.Properties.Count());
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"[]", (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal)).Value as ODataUntypedValue)
                .RawValue);
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""", (complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, undeclaredComplex.Properties.Count());
            Assert.Equal("aaaaaaaaa", undeclaredComplex.Properties.Single(p => p.Name == "MyProp1").Value);
            Assert.Equal("bbbbbbb", undeclaredComplex.Properties.Single(p => p.Name == "UndeclaredProp1").Value);
            Assert.Equal(2, address.Properties.Count());
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("Server.NS.AddressUndeclared", undeclaredComplex1.TypeName);
            Assert.Single(undeclaredComplex1.Properties);
            Assert.Equal("No.999,Zixing Rd Minhang", undeclaredComplex1.Properties.Single(p => p.Name == "Street").Value);
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

            Assert.Equal(2, entry.Properties.Count());
            Assert.Empty(undeclaredComplex1.Properties);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredPrimitiveCollectionPropertiesWithoutODataTypeAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                        UndeclaredCollection1:[""email1@163.com"",null,""email2@gmail.com"",""email3@gmail2.com"",null],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource address = null;
            ODataNestedResourceInfo undeclaredCollection1 = null;
            ODataNestedResourceInfo addressNestedInfo = null;
            bool populatingCollection = false;
            List<object> undeclaredCollection1Items = new List<object>();
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (populatingCollection)
                    {
                        undeclaredCollection1Items.Add(reader.Item as ODataResource);
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
                else if (reader.State == ODataReaderState.ResourceSetStart)
                {
                    populatingCollection = true;
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    populatingCollection = false;
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    if (undeclaredCollection1 == null)
                    {
                        undeclaredCollection1 = (reader.Item as ODataNestedResourceInfo);
                    }
                    else
                    {
                        addressNestedInfo = (reader.Item as ODataNestedResourceInfo);
                    }
                }
                else if (reader.State == ODataReaderState.Primitive)
                {
                    Assert.True(populatingCollection, "Found primitive type outside of collection");
                    undeclaredCollection1Items.Add(((ODataPrimitiveValue)(reader.Item)).Value);
                }
            }, /*readUntypedAsValue*/ true);

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("UndeclaredCollection1", undeclaredCollection1.Name);
            Assert.Equal(5, undeclaredCollection1Items.Count());
            Assert.Null(undeclaredCollection1Items.Last());
            Assert.Equal("email1@163.comemail2@gmail.comemail3@gmail2.com", String.Concat(undeclaredCollection1Items));
            Assert.Equal("Address", addressNestedInfo.Name);
            Assert.Equal(2, address.Properties.Count());
            Assert.Equal("Server.NS.Address", address.TypeName);
        }

        [Fact]
        public void ReadUndeclaredNonPrimitiveCollectionPropertyAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",
                                        ""Id"":61880128,
                                        ""UndeclaredFloatId"":12.3,
                                        ""UndeclaredCollection1"":[
                                            {""@odata.type"":""Server.NS.Address"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
                                            ""email1@163.com"",
                                            ""email2@gmail.com"",
                                            [ 
                                                ""email1@163.com"",
                                                {""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""},
                                                null,
                                                ""email3@gmail2.com""
                                            ],
                                            ""email3@gmail2.com""
                                        ],
                                        ""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}
                                    }";
            ODataResource entry = null;
            ODataResource undeclaredCollection1Address = null;
            ODataPrimitiveValue undeclaredCollection1Email1 = null;
            ODataPrimitiveValue undeclaredCollection1Email2 = null;
            ODataPrimitiveValue undeclaredCollection1NestedEmail1 = null;
            ODataResource undeclaredCollection1NestedCollectionAddress = null;
            ODataResource undeclaredCollection1NestedNull = null;
            bool setNestedNull = false;
            ODataPrimitiveValue undeclaredCollection1NestedEmail2 = null;
            ODataResource address = null;
            ODataNestedResourceInfo undeclaredCollection1 = null;
            ODataNestedResourceInfo undeclaredNestedCollection = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredCollection1Address == null)
                    {
                        undeclaredCollection1Address = (reader.Item as ODataResource);
                    }
                    else if (undeclaredCollection1NestedCollectionAddress == null)
                    {
                        undeclaredCollection1NestedCollectionAddress = (reader.Item as ODataResource);
                    }
                    else if (!setNestedNull)
                    {
                        setNestedNull = true;
                        undeclaredCollection1NestedNull = (reader.Item as ODataResource);
                    }
                    else if (address == null)
                    {
                        address = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.Primitive)
                {
                    if (undeclaredCollection1Email1 == null)
                    {
                        undeclaredCollection1Email1 = (reader.Item as ODataPrimitiveValue);
                    }
                    else if (undeclaredCollection1Email2 == null)
                    {
                        undeclaredCollection1Email2 = (reader.Item as ODataPrimitiveValue);
                    }
                    else if (undeclaredCollection1NestedEmail1 == null)
                    {
                        undeclaredCollection1NestedEmail1 = (reader.Item as ODataPrimitiveValue);
                    }

                    else if (undeclaredCollection1NestedEmail2 == null)
                    {
                        undeclaredCollection1NestedEmail2 = (reader.Item as ODataPrimitiveValue);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    if (undeclaredCollection1 == null)
                    {
                        undeclaredCollection1 = (reader.Item as ODataNestedResourceInfo);
                    }
                    else if (undeclaredNestedCollection == null)
                    {
                        undeclaredNestedCollection = (reader.Item as ODataNestedResourceInfo);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal(2, undeclaredCollection1Address.Properties.Count());
            Assert.Equal("Server.NS.Address", undeclaredCollection1Address.TypeAnnotation.TypeName);
            Assert.Equal("email1@163.com", undeclaredCollection1Email1.Value);
            Assert.Equal("email2@gmail.com", undeclaredCollection1Email2.Value);
            Assert.Equal("email1@163.com", undeclaredCollection1NestedEmail1.Value);
            Assert.Equal(2, undeclaredCollection1NestedCollectionAddress.Properties.Count());
            Assert.Null(undeclaredCollection1NestedNull);
            Assert.Equal("email3@gmail2.com", undeclaredCollection1NestedEmail2.Value);
            Assert.Equal(2, address.Properties.Count());
            Assert.Equal("UndeclaredCollection1", undeclaredCollection1.Name);
            Assert.Equal("Address", undeclaredNestedCollection.Name);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyCollectionPropertiesWithoutODataTypeAsValueTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource address = null;
            ODataNestedResourceInfo undeclaredCollection1 = null;
            ODataNestedResourceInfo addressNestedInfo = null;
            List<object> undeclaredCollection1Items = new List<object>();
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
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    if (undeclaredCollection1 == null)
                    {
                        undeclaredCollection1 = (reader.Item as ODataNestedResourceInfo);
                    }
                    else
                    {
                        addressNestedInfo = (reader.Item as ODataNestedResourceInfo);
                    }
                }
                else if (reader.State == ODataReaderState.Primitive)
                {
                    undeclaredCollection1Items.Add(reader.Item == null ? null : ((ODataPrimitiveValue)(reader.Item)).Value);
                }
            }, /*readUntypedAsValue*/ true);

            Assert.Equal(2, entry.Properties.Count());
            Assert.Equal("UndeclaredCollection1", undeclaredCollection1.Name);
            Assert.Empty(undeclaredCollection1Items);
            Assert.Equal("Server.NS.Address", address.TypeName);
            Assert.Equal("Address", addressNestedInfo.Name);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", address.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet", StringComparison.Ordinal)).Value);
        }

        #endregion

        #region untyped single value tests
        [Theory]
        [InlineData("Edm.Untyped")]
        [InlineData("Server.NS.UndefinedType")]
        public void ReadUntypedResource(string fragment)
        {
            string payload = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#" + fragment + "\",\"id\":1}";
            ODataResource entry = null;
            this.ReadResourcePayload(payload, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    entry = (reader.Item as ODataResource);
                }
            });

            Assert.Single(entry.Properties);
            Assert.Equal(1m, entry.Properties.First(p => p.Name == "id").Value);
        }

        #endregion

        #region untyped collection tests

        [Theory]
        [InlineData("Edm.Untyped")]
        [InlineData("Collection(Edm.Untyped)")]
        [InlineData("Collection(Server.NS.UndefinedType)")]
        public void ReadUntypedCollectionEmpty(string fragment)
        {
            string payload = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#" + fragment + "\",\"value\":[]}";
            bool readEntry = false;
            this.ReadCollectionPayload(payload, reader =>
             {
                 if (reader.State == ODataReaderState.ResourceStart || reader.State == ODataReaderState.Primitive)
                 {
                     readEntry = true;
                 }
             });

            Assert.False(readEntry);
        }

        [Theory]
        [InlineData("Edm.Untyped")]
        [InlineData("Collection(Edm.Untyped)")]
        [InlineData("Collection(Server.NS.UndefinedType)")]
        public void ReadUntypedCollectionContainingNull(string fragment)
        {
            string payload = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#" + fragment + "\",\"value\":[null]}";
            object entry = null;
            bool readEntry = false;
            this.ReadCollectionPayload(payload, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    entry = reader.Item;
                    readEntry = true;
                }
            });

            Assert.True(readEntry);
            Assert.Null(entry);
        }

        [Theory]
        [InlineData("Edm.Untyped")]
        [InlineData("Collection(Edm.Untyped)")]
        [InlineData("Collection(Server.NS.UndefinedTypeDefinition)")]
        public void ReadUntypedCollectionContainingPrimitive(string fragment)
        {
            string payload = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#" + fragment + "\",\"value\":[\"primitiveString\"]}";
            ODataPrimitiveValue entry = null;
            this.ReadCollectionPayload(payload, reader =>
            {
                if (reader.State == ODataReaderState.Primitive)
                {
                    entry = (reader.Item as ODataPrimitiveValue);
                }
            });

            Assert.Equal("primitiveString", entry.Value);
        }

        [Theory]
        [InlineData("Edm.Untyped")]
        [InlineData("Collection(Edm.Untyped)")]
        [InlineData("Collection(Server.NS.UndefinedType)")]
        public void ReadUntypedCollectionContainingResource(string fragment)
        {
            string payload = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#" + fragment + "\",\"value\":[{\"id\":1}]}";
            ODataResource entry = null;
            this.ReadCollectionPayload(payload, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    entry = (reader.Item as ODataResource);
                }
            });

            Assert.Single(entry.Properties);
            Assert.Equal(1m, entry.Properties.First(p=>p.Name == "id").Value);
        }

        [Theory]
        [InlineData("Edm.Untyped")]
        [InlineData("Collection(Edm.Untyped)")]
        public void ReadUntypedCollectionContainingCollection(string fragment)
        {
            string payload = "{\"@odata.context\":\"http://www.sampletest.com/$metadata#" + fragment +"\",\"value\":[[\"primitiveString\",{\"id\":1}]]}";
            ODataPrimitiveValue primitiveMember = null;
            ODataResource resourceMember = null;
            int level = 0;
            this.ReadCollectionPayload(payload, reader =>
            {
                if (reader.State == ODataReaderState.ResourceSetStart)
                {
                    level++;
                }
                else if (reader.State == ODataReaderState.ResourceStart && level == 2)
                {
                    resourceMember = (reader.Item as ODataResource);
                }
                else if (reader.State == ODataReaderState.Primitive && level == 2)
                {
                    primitiveMember = (reader.Item as ODataPrimitiveValue);
                }
            });

            Assert.Single(resourceMember.Properties);
            Assert.Equal(1m, resourceMember.Properties.First(p => p.Name == "id").Value);
            Assert.Equal("primitiveString", primitiveMember.Value);
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

            Assert.Equal(4, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp1\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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

            Assert.Equal(4, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp2", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"MyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (complex1.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp3", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\",\"MyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null}}}", result);
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

            Assert.Equal(4, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}", (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))
                .Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}", (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1", StringComparison.Ordinal)).
                Value as ODataUntypedValue).RawValue);
            Assert.Equal("Edm.Untyped", entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1", StringComparison.Ordinal)).TypeAnnotation.TypeName);
            Assert.Null(complex2);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp1@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp1\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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

            Assert.Equal(4, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal("Edm.Untyped", entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2", StringComparison.Ordinal)).TypeAnnotation.TypeName);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"UndeclaredMyEdmUntypedProp2@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp2\":{\"MyProp12\":\"bbb222\",\"abc\":null},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\"}}", result);
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

            Assert.Equal(3, entry.Properties.Count());
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                (entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                (complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3", StringComparison.Ordinal)).Value as ODataUntypedValue).RawValue);
            Assert.Equal("Edm.Untyped", complex1.Properties
                .Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3", StringComparison.Ordinal)).TypeAnnotation.TypeName);

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

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Id\":61880128,\"UndeclaredFloatId\":12.3,\"undeclaredComplex1\":{\"MyProp1\":\"aaaaaaaaa\",\"UndeclaredProp1\":\"bbbbbbb\"},\"Address\":{\"Street\":\"No.999,Zixing Rd Minhang\",\"UndeclaredStreet\":\"No.10000000999,Zixing Rd Minhang\",\"UndeclaredMyEdmUntypedProp3@odata.type\":\"#Untyped\",\"UndeclaredMyEdmUntypedProp3\":{\"MyProp12\":\"bbb222\",\"abc\":null}}}", result);
        }
        #endregion

        #region undeclared type tests

        [Fact]
        public void ReadResourceWithUndeclaredResourceTypeShouldSucceed()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,
                ""UndeclaredProperty"":{""@odata.type"":""#Server.NS.Undeclared"", ""Id"":""123""}}";
            ODataResource entry = null;
            ODataResource undeclaredProperty = null;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (entry == null)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (undeclaredProperty == null)
                    {
                        undeclaredProperty = (reader.Item as ODataResource);
                    }
                }
            }, /*readUntypedAsValue*/ true);

            Assert.Single(entry.Properties);
            Assert.Equal("Server.NS.Undeclared", undeclaredProperty.TypeName);
            Assert.Single(undeclaredProperty.Properties);
            Assert.Equal("123", undeclaredProperty.Properties.Single(p => p.Name == "Id").Value);
        }

        [Fact]
        public void ReadResourceWithUndeclaredCollectionTypeShouldFail()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,
                ""UndeclaredProperty"":{""@odata.type"":""#Collection(Server.NS.Undeclared)"", ""Id"":""123""}}";
            Action test = () => this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader => { }, true);
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_CollectionTypeNotExpected("Collection(Server.NS.Undeclared)"));
        }

        [Fact]
        public void ReadCollectionWithUndeclaredCollectionTypeShouldSucceed()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,
                ""UndeclaredProperty@odata.type"":""#Collection(Server.NS.Undeclared)"",""UndeclaredProperty"":[{""Id"":""123""}]}";
            ODataResource entry = null;
            ODataResource undeclaredProperty = null;
            int level = 0;
            this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (level == 0)
                    {
                        entry = (reader.Item as ODataResource);
                    }
                    else if (level == 1)
                    {
                        undeclaredProperty = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    level++;
                }
            }, /*readUntypedAsValue*/ true);

            Assert.Single(entry.Properties);
            Assert.Equal("Server.NS.Undeclared", undeclaredProperty.TypeName);
            Assert.Single(undeclaredProperty.Properties);
            Assert.Equal("123", undeclaredProperty.Properties.Single(p => p.Name == "Id").Value);
        }

        [Fact]
        public void ReadCollectionWithUndeclaredSingleTypeShouldFail()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,
                ""UndeclaredProperty@odata.type"":""#Server.NS.Undeclared"",""UndeclaredProperty"":[{""Id"":""123""}]}";
            Action test = () => this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader => { }, true);
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_CollectionTypeExpected("Server.NS.Undeclared"));
        }

        [Fact]
        public void ReadResourceExpectingCollectionShouldFail()
        {
            string contextUrl = @"http://www.sampletest.com/$metadata#Collection(Server.NS.Undefined)";
            string payload = "{\"@odata.context\":\"" + contextUrl + "\",\"id\":1}";
            Action test = () => this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader => { }, true);
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(contextUrl, "Resource"));
        }

        [Fact]
        public void ReadCollectionExpectingResourceShouldFail()
        {
            string contextUrl = @"http://www.sampletest.com/$metadata#Server.NS.Undefined";
            string payload = "{\"@odata.context\":\"" + contextUrl + "\",\"value\":[{\"id\":1}]}";
            Action test = () => this.ReadCollectionPayload(payload, reader => { });
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(contextUrl, "ResourceSet"));
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
