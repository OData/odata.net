//---------------------------------------------------------------------
// <copyright file="ODataJsonEntryAndFeedDeSerializerUndeclaredTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.Test.OData.TDD.Tests.Reader.Json
{
    public class ODataJsonEntryAndFeedDeserializerUndeclaredTests
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

        public ODataJsonEntryAndFeedDeserializerUndeclaredTests()
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
            this.serverEntityType.AddStructuralProperty("Infos",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetUntyped())));

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

        private void ReadEntryPayload(
            string payload, 
            EdmEntitySet entitySet,
            EdmEntityType entityType, 
            Action<ODataReader> action, 
            bool readUntypedAsValue = false,
            bool readRequest = false, 
            ODataMessageReaderSettings settings = null)
        {
            ODataMessageReaderSettings readerSettings = readUntypedAsValue ? UntypedAsValueReaderSettings : UntypedAsStringReaderSettings;
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/json");

            using (var msgReader = readRequest ?
                new ODataMessageReader((IODataRequestMessage)message, settings ?? readerSettings, this.serverModel) :
                new ODataMessageReader((IODataResponseMessage)message, settings ?? readerSettings, this.serverModel))
            {
                var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private async Task ReadEntryPayloadAsync(string payload,
            EdmEntitySet entitySet,
            EdmEntityType entityType,
            Action<ODataReader> action,
            bool readUntypedAsValue = false,
            bool readRequest = false,
            ODataMessageReaderSettings settings = null)
        {
            var message = new InMemoryMessage()
            {
                Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload))
            };
            message.SetHeader("Content-Type", "application/json");

            ODataMessageReaderSettings readerSettings = readUntypedAsValue ? UntypedAsValueReaderSettings : UntypedAsStringReaderSettings;

            using (var msgReader = readRequest ?
                new ODataMessageReader((IODataRequestMessage)message, settings ?? readerSettings, this.serverModel) :
                new ODataMessageReader((IODataResponseMessage)message, settings ?? readerSettings, this.serverModel))
            {
                var reader = await msgReader.CreateODataResourceReaderAsync(entitySet, entityType);
                while (await reader.ReadAsync())
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
            Assert.Equal("null", Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Last()).ODataValue).RawValue);

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
            Assert.Equal(false, Assert.IsType<ODataProperty>(complex1.Properties.First(s => string.Equals("UndeclaredBool", s.Name, StringComparison.Ordinal))).Value);

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
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(complex1.Properties
                .First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);

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
            Assert.Equal(12d, Assert.IsType<ODataProperty>(complex1.Properties
                .First(s => string.Equals("UndeclaredStreetNo", s.Name, StringComparison.Ordinal))).Value);

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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value).RawValue);

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
            Assert.Equal(3, Assert.IsType<ODataCollectionValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal))).ODataValue).Items.Count());
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
            Assert.Equal("null", Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Last()).Value).RawValue);

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
            Assert.Equal("12.3", Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name, StringComparison.Ordinal))).Value).RawValue); // numeric
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value).RawValue);  // string
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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Last()).Value).RawValue);
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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Last()).Value).RawValue);
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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal))).Value).RawValue);
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
            ODataProperty decimalProperty = Assert.IsType<ODataProperty>(entry.Properties.First(s => string.Equals("UndeclaredDecimal", s.Name, StringComparison.Ordinal)));
            Assert.Equal(12.3M, decimalProperty.Value);
            Assert.Equal("Server.NS.UndeclaredDecimalType", decimalProperty.TypeAnnotation.TypeName);
            Assert.Equal(2, complex1.Properties.Count());
            ODataProperty addressProperty = Assert.IsType<ODataProperty>(complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal)));
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
            Assert.Equal(12.3M, Assert.IsType<ODataProperty>(entry.Properties.First(s => string.Equals("UndeclaredFloatId", s.Name, StringComparison.Ordinal))).Value);
            Assert.Equal(2, complex1.Properties.Count());
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(complex1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);
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
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);
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
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);
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
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(undeclaredAddress1.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);
            Assert.Equal("No.999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(undeclaredAddress1.Properties.First(s => string.Equals("Street", s.Name, StringComparison.Ordinal))).Value);
            Assert.Equal(2, innerComplex1.Properties.Count());
            Assert.Null(Assert.IsType<ODataProperty>(innerComplex1.Properties.First(s => string.Equals("innerProp1", s.Name, StringComparison.Ordinal))).Value);
            Assert.Equal("abc", Assert.IsType<ODataProperty>(innerComplex1.Properties.First(s => string.Equals("innerProp2", s.Name, StringComparison.Ordinal))).Value);
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
            Assert.Equal("email1@163.comemail2@gmail.comemail3@gmail2.com", string.Concat(undeclaredCollection1Items));
            Assert.Equal("Address", addressNestedInfo.Name);
            Assert.Equal("Server.NS.Address", address.TypeName);
            Assert.Equal(2, address.Properties.Count());
            Assert.Equal("No.999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Properties.First(s => string.Equals("Street", s.Name, StringComparison.Ordinal))).Value);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);
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
            Assert.Equal("email1@163.comemail2@gmail.comemail3@gmail2.com", string.Concat(untypedCollection.Select(c=> Assert.IsType<ODataProperty>(c.Properties.Single(p => string.Equals(p.Name, "email", StringComparison.Ordinal))).Value)));
            Assert.Equal(2, address.Properties.Count());
            Assert.Equal("No.999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Properties.First(s => string.Equals("Street", s.Name, StringComparison.Ordinal))).Value);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);
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
            Assert.Equal("email1@163.comemail2@gmail.comemail3@gmail2.com", string.Concat(untypedCollection.Select(c => Assert.IsType<ODataProperty>(c.Properties.Single(p => string.Equals(p.Name, "email", StringComparison.Ordinal))).Value)));
            Assert.Equal(2, address.Properties.Count());
            Assert.Equal("No.999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Properties.First(s => string.Equals("Street", s.Name, StringComparison.Ordinal))).Value);
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Properties.First(s => string.Equals("UndeclaredStreet", s.Name, StringComparison.Ordinal))).Value);
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
            Assert.Equal("null", Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredType1", StringComparison.Ordinal))).ODataValue).RawValue);
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
            Assert.Equal(3, Assert.IsType<ODataCollectionValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal))).ODataValue).Items
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
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))).Value).RawValue);
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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Last()).Value).RawValue);
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
            Assert.Equal(@"{}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))).Value).RawValue);
        }

        [Fact]
        public void ReadOpenEntryUndeclaredPrimitiveCollectionPropertiesWithoutODataTypeAsODataCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(
                payload, 
                this.serverOpenEntitySet, 
                this.serverOpenEntityType,
                reader =>
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
                },
                settings: new ODataMessageReaderSettings
                {
                    ShouldIncludeAnnotation = (annotationName) => true,
                    Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
                    EnableUntypedCollections = true,
                });

            Assert.Equal(3, entry.Properties.Count());
            var odataCollection = Assert.IsType<ODataCollectionValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal))).Value);
            Assert.Equal(@"""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""",
                string.Join(",", odataCollection.Items.Cast<ODataUntypedValue>().Select(item => item.RawValue)));
            Assert.Equal(2, complex1.Properties.Count());
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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal))).Value).RawValue);
            Assert.Equal(2, complex1.Properties.Count());
        }

        [Fact]
        public void ReadOpenEntryUndeclaredEmptyCollectionPropertiesWithoutODataTypeAsODataCollectionTest()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,
                                                                          UndeclaredCollection1:[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}";
            ODataResource entry = null;
            ODataResource complex1 = null;
            this.ReadEntryPayload(
                payload, 
                this.serverOpenEntitySet, 
                this.serverOpenEntityType, 
                reader =>
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
                },
                settings: new ODataMessageReaderSettings
                {
                    ShouldIncludeAnnotation = (annotationName) => true,
                    Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
                    EnableUntypedCollections = true,
                });

            Assert.Equal(3, entry.Properties.Count());
            var odataCollection = Assert.IsType<ODataCollectionValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal))).Value);
            Assert.False(odataCollection.Items.Any());
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet", StringComparison.Ordinal))).Value).RawValue);
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
            Assert.Equal(@"[]",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredCollection1", StringComparison.Ordinal))).Value).RawValue);
            Assert.Equal(@"""No.10000000999,Zixing Rd Minhang""",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet", StringComparison.Ordinal))).Value).RawValue);
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
            Assert.Equal("aaaaaaaaa", Assert.IsType<ODataProperty>(undeclaredComplex.Properties.Single(p => p.Name == "MyProp1")).Value);
            Assert.Equal("bbbbbbb", Assert.IsType<ODataProperty>(undeclaredComplex.Properties.Single(p => p.Name == "UndeclaredProp1")).Value);
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
            Assert.Equal("No.999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(undeclaredComplex1.Properties.Single(p => p.Name == "Street")).Value);
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
            Assert.Equal("No.10000000999,Zixing Rd Minhang", Assert.IsType<ODataProperty>(address.Properties.Single(s => string.Equals(s.Name, "UndeclaredStreet", StringComparison.Ordinal))).Value);
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
            Assert.Equal(1m, Assert.IsType<ODataProperty>(entry.Properties.First(p => p.Name == "id")).Value);
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
            Assert.Equal(1m, Assert.IsType<ODataProperty>(entry.Properties.First(p=>p.Name == "id")).Value);
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
            Assert.Equal(1m, Assert.IsType<ODataProperty>(resourceMember.Properties.First(p => p.Name == "id")).Value);
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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))).Value).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp1", StringComparison.Ordinal))).Value).RawValue);

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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))).Value).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp2", StringComparison.Ordinal))).Value).RawValue);

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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))).Value).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(complex1.Properties.Single(s => string.Equals(s.Name, "MyEdmUntypedProp3", StringComparison.Ordinal))).Value).RawValue);

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

        #region declared Edm.Untyped property with odata.type
        [Fact]
        public void ReadSingleValueEdmUntypedPropertyWithODataTypeResourceTest()
        {
            const string payload = @"{
  ""MyEdmUntypedProp1"":{
    ""@odata.type"": ""#Server.NS.Address"",
    ""Street"":""Mars Rd""
  }
}";
            ODataResource topLevelResource = null;
            ODataResource nestedResource = null;
            ODataNestedResourceInfo nestedInfo = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (topLevelResource == null)
                    {
                        topLevelResource = (reader.Item as ODataResource);
                    }
                    else
                    {
                        nestedResource = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
            }, false, true /*reading request*/);

            VerifyResourceAndWrite(topLevelResource, nestedResource, nestedInfo);
        }

        [Fact]
        public async Task ReadSingleValueEdmUntypedPropertyWithODataTypeResourceTestAsync()
        {
            const string payload = @"{
  ""MyEdmUntypedProp1"":{
    ""@odata.type"": ""#Server.NS.Address"",
    ""Street"":""Mars Rd""
  }
}";
            ODataResource topLevelResource = null;
            ODataResource nestedResource = null;
            ODataNestedResourceInfo nestedInfo = null;
            await this.ReadEntryPayloadAsync(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (topLevelResource == null)
                    {
                        topLevelResource = (reader.Item as ODataResource);
                    }
                    else
                    {
                        nestedResource = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
            }, false, true /*reading request*/);

            VerifyResourceAndWrite(topLevelResource, nestedResource, nestedInfo);
        }

        private void VerifyResourceAndWrite(ODataResource topLevelResource, ODataResource nestedResource, ODataNestedResourceInfo nestedInfo)
        {
            Assert.Empty(topLevelResource.Properties);
            ODataProperty property = Assert.IsType<ODataProperty>(Assert.Single(nestedResource.Properties));
            Assert.Equal("Street", property.Name);
            Assert.Equal("Mars Rd", property.Value);
            Assert.Equal("MyEdmUntypedProp1", nestedInfo.Name);

            topLevelResource.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(topLevelResource);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(topLevelResource);
                writer.WriteStart(nestedInfo);
                writer.WriteStart(nestedResource);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\"," +
                "\"MyEdmUntypedProp1\":{" +
                  "\"Street\":\"Mars Rd\"" +
                "}" +
              "}", result);
        }

        [Fact]
        public void ReadSingleValueEdmUntypedPropertyWithODataTypeEnumTest()
        {
            const string payload = @"{
  ""MyEdmUntypedProp1@odata.type"": ""#Server.NS.EnumType"",
  ""MyEdmUntypedProp1"": ""Member""
}";
            ODataResource topLevelResource = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    topLevelResource = (reader.Item as ODataResource);
                }
            }, false, true /*reading request*/);

            VerifyEnum(topLevelResource);
        }

        [Fact]
        public async Task ReadSingleValueEdmUntypedPropertyWithODataTypeEnumTestAsync()
        {
            const string payload = @"{
  ""MyEdmUntypedProp1@odata.type"": ""#Server.NS.EnumType"",
  ""MyEdmUntypedProp1"": ""Member""
}";
            ODataResource topLevelResource = null;
            await this.ReadEntryPayloadAsync(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    topLevelResource = (reader.Item as ODataResource);
                }
            }, false, true /*reading request*/);

            VerifyEnum(topLevelResource);
        }

        private static void VerifyEnum(ODataResource topLevelResource)
        {
            ODataProperty property = Assert.IsType<ODataProperty>(Assert.Single(topLevelResource.Properties));
            Assert.Equal("MyEdmUntypedProp1", property.Name);
            ODataEnumValue enumValue = Assert.IsType<ODataEnumValue>(property.Value);
            Assert.Equal("Server.NS.EnumType", enumValue.TypeName);
            Assert.Equal("Member", enumValue.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadSingleValueEdmUntypedPropertyWithODataTypePrimitiveCollectionTest(bool readUntypedAsValue)
        {
            const string payload = @"{
  ""MyEdmUntypedProp1@odata.type"": ""#Collection(Edm.Int32)"",
  ""MyEdmUntypedProp1"": [5, 3]
}";
            ODataResource topLevelResource = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    topLevelResource = (reader.Item as ODataResource);
                }
            },
            readUntypedAsValue, // it doesn't matter since the value is typed.
            true /*reading request*/);

            ODataProperty property = Assert.IsType<ODataProperty>(Assert.Single(topLevelResource.Properties));
            Assert.Equal("MyEdmUntypedProp1", property.Name);
            ODataCollectionValue collectionValue = Assert.IsType<ODataCollectionValue>(property.Value);

            Assert.Equal("Collection(Edm.Int32)", collectionValue.TypeName);
            Assert.Equal(2, collectionValue.Items.Count());
            Assert.Equal(5, collectionValue.Items.First());
            Assert.Equal(3, collectionValue.Items.Last());
        }

        [Fact]
        public void ReadSingleValueEdmUntypedPropertyWithODataTypeMixCollectionTest()
        {
            const string payload = @"{
  ""MyEdmUntypedProp1"": [
      5,
      {
        ""@odata.type"": ""#Server.NS.Address"",
        ""Street"":""Mars Rd""
      }
  ]
}";
            ODataResource topLevelResource = null;
            ODataResource nestedResource = null;
            ODataNestedResourceInfo nestedInfo = null;
            ODataPrimitiveValue primitiveValue = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (topLevelResource == null)
                    {
                        topLevelResource = (reader.Item as ODataResource);
                    }
                    else
                    {
                        nestedResource = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
                else if (reader.State == ODataReaderState.Primitive)
                {
                    primitiveValue = (reader.Item as ODataPrimitiveValue);
                }
            }, true, true /*reading request*/);

            Assert.Empty(topLevelResource.Properties);
            Assert.Equal("MyEdmUntypedProp1", nestedInfo.Name);
            Assert.Equal(5, primitiveValue.Value);

            ODataProperty property = Assert.IsType<ODataProperty>(Assert.Single(nestedResource.Properties));
            Assert.Equal("Street", property.Name);
            Assert.Equal("Mars Rd", property.Value);
        }

        [Fact]
        public void ReadCollectionEdmUntypedPropertyWithODataTypeMixCollectionTest()
        {
            const string payload = @"{
  ""Infos"":[
    {
      ""@odata.type"": ""#Server.NS.Address"",
      ""Street"":""Mars Rd""
    },
    42
  ]
}";
            ODataResource topLevelResource = null;
            ODataResource nestedResource = null;
            ODataResourceSet resourceSet = null;
            ODataNestedResourceInfo nestedInfo = null;
            ODataPrimitiveValue primitiveValue = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (topLevelResource == null)
                    {
                        topLevelResource = (reader.Item as ODataResource);
                    }
                    else
                    {
                        nestedResource = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetStart)
                {
                    resourceSet = (reader.Item as ODataResourceSet);
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
                else if (reader.State == ODataReaderState.Primitive)
                {
                    primitiveValue = (reader.Item as ODataPrimitiveValue);
                }
            }, false, true /*reading request*/);

            VerifyCollectionAndWrite(topLevelResource, nestedResource, resourceSet, nestedInfo, primitiveValue);
        }

        [Fact]
        public async Task ReadCollectionEdmUntypedPropertyWithODataTypeMixCollectionTestAsync()
        {
            const string payload = @"{
  ""Infos"":[
    {
      ""@odata.type"": ""#Server.NS.Address"",
      ""Street"":""Mars Rd""
    },
    42
  ]
}";
            ODataResource topLevelResource = null;
            ODataResource nestedResource = null;
            ODataResourceSet resourceSet = null;
            ODataNestedResourceInfo nestedInfo = null;
            ODataPrimitiveValue primitiveValue = null;
            await this.ReadEntryPayloadAsync(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    if (topLevelResource == null)
                    {
                        topLevelResource = (reader.Item as ODataResource);
                    }
                    else
                    {
                        nestedResource = (reader.Item as ODataResource);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetStart)
                {
                    resourceSet = (reader.Item as ODataResourceSet);
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    nestedInfo = (reader.Item as ODataNestedResourceInfo);
                }
                else if (reader.State == ODataReaderState.Primitive)
                {
                    primitiveValue = (reader.Item as ODataPrimitiveValue);
                }
            }, false, true /*reading request*/);

            VerifyCollectionAndWrite(topLevelResource, nestedResource, resourceSet, nestedInfo, primitiveValue);
        }

        private void VerifyCollectionAndWrite(ODataResource topLevelResource, ODataResource nestedResource,
            ODataResourceSet resourceSet, ODataNestedResourceInfo nestedInfo, ODataPrimitiveValue primitiveValue)
        {
            Assert.Empty(topLevelResource.Properties);
            ODataProperty property = Assert.IsType<ODataProperty>(Assert.Single(nestedResource.Properties));
            Assert.Equal("Street", property.Name);
            Assert.Equal("Mars Rd", property.Value);
            Assert.Equal(42, primitiveValue.Value);
            Assert.Equal("Infos", nestedInfo.Name);

            topLevelResource.MetadataBuilder = new Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder(topLevelResource);
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(topLevelResource);
                writer.WriteStart(nestedInfo);
                writer.WriteStart(resourceSet);
                writer.WritePrimitive(primitiveValue); // switch the order intentionally
                writer.WriteStart(nestedResource);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\"," +
                "\"Infos\":[" +
                  "42," +
                  "{\"@odata.type\":\"#Server.NS.Address\",\"Street\":\"Mars Rd\"}" +
                "]" +
              "}", result);
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
            Assert.Equal(@"{""MyProp1"":""aaaaaaaaa"",""UndeclaredProp1"":""bbbbbbb""}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))).Value).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp1", StringComparison.Ordinal))).Value).RawValue);
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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))).Value).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp2", StringComparison.Ordinal))).Value).RawValue);
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
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1", StringComparison.Ordinal))).Value).RawValue);
            Assert.Equal(@"{""MyProp12"":""bbb222"",""abc"":null}",
                Assert.IsType<ODataUntypedValue>(Assert.IsType<ODataProperty>(complex1.Properties.Single(s => string.Equals(s.Name, "UndeclaredMyEdmUntypedProp3", StringComparison.Ordinal))).Value).RawValue);
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
            Assert.Equal("123", Assert.IsType<ODataProperty>(undeclaredProperty.Properties.Single(p => p.Name == "Id")).Value);
        }

        [Fact]
        public void ReadResourceWithUndeclaredCollectionTypeShouldFail()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,
                ""UndeclaredProperty"":{""@odata.type"":""#Collection(Server.NS.Undeclared)"", ""Id"":""123""}}";
            Action test = () => this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader => { }, true);
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_CollectionTypeNotExpected("Collection(Server.NS.Undeclared)"));
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
            Assert.Equal("123", Assert.IsType<ODataProperty>(undeclaredProperty.Properties.Single(p => p.Name == "Id")).Value);
        }

        [Fact]
        public void ReadCollectionWithUndeclaredSingleTypeShouldFail()
        {
            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,
                ""UndeclaredProperty@odata.type"":""#Server.NS.Undeclared"",""UndeclaredProperty"":[{""Id"":""123""}]}";
            Action test = () => this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader => { }, true);
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_CollectionTypeExpected("Server.NS.Undeclared"));
        }

        [Fact]
        public void ReadResourceExpectingCollectionShouldFail()
        {
            string contextUrl = @"http://www.sampletest.com/$metadata#Collection(Server.NS.Undefined)";
            string payload = "{\"@odata.context\":\"" + contextUrl + "\",\"id\":1}";
            Action test = () => this.ReadEntryPayload(payload, this.serverOpenEntitySet, this.serverOpenEntityType, reader => { }, true);
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(contextUrl, "Resource"));
        }

        [Fact]
        public void ReadCollectionExpectingResourceShouldFail()
        {
            string contextUrl = @"http://www.sampletest.com/$metadata#Server.NS.Undefined";
            string payload = "{\"@odata.context\":\"" + contextUrl + "\",\"value\":[{\"id\":1}]}";
            Action test = () => this.ReadCollectionPayload(payload, reader => { });
            test.Throws<ODataException>(ODataErrorStrings.ODataJsonContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(contextUrl, "ResourceSet"));
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
