//---------------------------------------------------------------------
// <copyright file="PropertyAndValueJsonLightWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.PrimitiveValueConverters;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Writer.JsonLight
{
    public class PropertyAndValueJsonLightWriterIntegrationTests
    {
        [Fact]
        public void WritingPayloadInt64SingleDoubleDecimalWithoutSuffix()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);
            entityType.AddStructuralProperty("FloatId", EdmPrimitiveTypeKind.Single, false);
            entityType.AddStructuralProperty("DoubleId", EdmPrimitiveTypeKind.Double, false);
            entityType.AddStructuralProperty("DecimalId", EdmPrimitiveTypeKind.Decimal, false);
            entityType.AddStructuralProperty("BoolValue1", EdmPrimitiveTypeKind.Boolean, false);
            entityType.AddStructuralProperty("BoolValue2", EdmPrimitiveTypeKind.Boolean, false);

            EdmComplexType complexType = new EdmComplexType("NS", "MyTestComplexType");
            complexType.AddStructuralProperty("CLongId", EdmPrimitiveTypeKind.Int64, false);
            complexType.AddStructuralProperty("CFloatId", EdmPrimitiveTypeKind.Single, false);
            complexType.AddStructuralProperty("CDoubleId", EdmPrimitiveTypeKind.Double, false);
            complexType.AddStructuralProperty("CDecimalId", EdmPrimitiveTypeKind.Decimal, false);
            model.AddElement(complexType);
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);
            entityType.AddStructuralProperty("ComplexProperty", complexTypeRef);

            entityType.AddStructuralProperty("LongNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt64(false))));
            entityType.AddStructuralProperty("FloatNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSingle(false))));
            entityType.AddStructuralProperty("DoubleNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDouble(false))));
            entityType.AddStructuralProperty("DecimalNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDecimal(false))));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = new EdmEntitySet(container, "MyTestEntitySet", entityType);
            model.AddElement(container);

            ODataEntry entry = new ODataEntry()
            {
                Id = new Uri("http://mytest"),
                TypeName = "NS.MyTestEntity",
                Properties = new[]
                {
                    new ODataProperty {Name = "LongId", Value = 12L},
                    new ODataProperty {Name = "FloatId", Value = 34.98f},
                    new ODataProperty {Name = "DoubleId", Value = 56.010},
                    new ODataProperty {Name = "DecimalId", Value = 78.62m},
                    new ODataProperty {Name = "BoolValue1", Value = true},
                    new ODataProperty {Name = "BoolValue2", Value = false},
                    new ODataProperty {Name = "LongNumbers", Value = new ODataCollectionValue {Items = new[] {0L, long.MinValue, long.MaxValue}, TypeName = "Collection(Int64)" }},
                    new ODataProperty {Name = "FloatNumbers", Value = new ODataCollectionValue {Items = new[] {1F, float.MinValue, float.MaxValue, float.PositiveInfinity, float.NegativeInfinity, float.NaN}, TypeName = "Collection(Single)" }},
                    new ODataProperty {Name = "DoubleNumbers", Value = new ODataCollectionValue {Items = new[] {-1D, double.MinValue, double.MaxValue, double.PositiveInfinity, double.NegativeInfinity, double.NaN}, TypeName = "Collection(Double)" }},
                    new ODataProperty {Name = "DecimalNumbers", Value = new ODataCollectionValue {Items = new[] {0M, decimal.MinValue, decimal.MaxValue}, TypeName = "Collection(Decimal)" }},
                    new ODataProperty
                    {
                        Name = "ComplexProperty",
                        Value = new ODataComplexValue
                        {
                            Properties = new[]
                            {
                                new ODataProperty { Name = "CLongId", Value = 1L},
                                new ODataProperty { Name = "CFloatId", Value = -1.0F},
                                new ODataProperty { Name = "CDoubleId", Value = 1.0D},
                                new ODataProperty { Name = "CDecimalId", Value = 1.0M},
                            }
                        }
                    }
                },

            };

            string outputPayload = this.WriterEntry(TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model), entry, entitySet, entityType);
            string expectedPayload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://mytest/\"," +
                "\"LongId\":12," +
                "\"FloatId\":34.98," +
                "\"DoubleId\":56.01," +
                "\"DecimalId\":78.62," +
                "\"BoolValue1\":true," +
                "\"BoolValue2\":false," +
                "\"LongNumbers\":[" +
                "0," +
                "-9223372036854775808," +
                "9223372036854775807" +
                "]," +
                "\"FloatNumbers\":[" +
                "1," +
                "-3.40282347E+38," +
                "3.40282347E+38," +
                "\"INF\"," +
                "\"-INF\"," +
                "\"NaN\"" +
                "]," +
                "\"DoubleNumbers\":[" +
                "-1.0," +
                "-1.7976931348623157E+308," +
                "1.7976931348623157E+308," +
                "\"INF\"," +
                "\"-INF\"," +
                "\"NaN\"" +
                "]," +
                "\"DecimalNumbers\":[" +
                "0," +
                "-79228162514264337593543950335," +
                "79228162514264337593543950335" +
                "]," +
                "\"ComplexProperty\":{" +
                "\"CLongId\":1," +
                "\"CFloatId\":-1," +
                "\"CDoubleId\":1.0," +
                "\"CDecimalId\":1.0}" +
                "}";

            outputPayload.Should().Be(expectedPayload);
        }

        [Fact]
        public void WriteTypeDefinitionPayloadShouldWork()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Double);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);

            EdmComplexType complexType = new EdmComplexType("NS", "Address");
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            EdmTypeDefinition addressType = new EdmTypeDefinition("NS", "Address", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference addressTypeRef = new EdmTypeDefinitionReference(addressType, false);
            complexType.AddStructuralProperty("CountryRegion", addressTypeRef);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(weightType);
            model.AddElement(addressType);
            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            ODataEntry entry = new ODataEntry()
            {
                TypeName = "NS.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Weight", Value = 60.5 },
                    new ODataProperty
                    {
                        Name = "Address",
                        Value = new ODataComplexValue
                        {
                            Properties = new[]
                            {
                                new ODataProperty { Name = "CountryRegion", Value = "China" }
                            }
                        }
                    }
                }
            };

            string outputPayload = this.WriterEntry(model, entry, entitySet, entityType);

            const string expectedMinimalPayload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#People/$entity\"," +
                "\"Id\":1," +
                "\"Weight\":60.5," +
                "\"Address\":{\"CountryRegion\":\"China\"}" +
                "}";

            outputPayload.Should().Be(expectedMinimalPayload);

            outputPayload = this.WriterEntry(model, entry, entitySet, entityType, true);

            const string expectedFullPayload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#People/$entity\"," +
                "\"@odata.type\":\"#NS.Person\"," +
                "\"@odata.id\":\"People(1)\"," +
                "\"@odata.editLink\":\"People(1)\"," +
                "\"Id\":1," +
                "\"Weight\":60.5," +
                "\"Address\":{\"CountryRegion\":\"China\"}" +
                "}";

            outputPayload.Should().Be(expectedFullPayload);
        }

        [Fact]
        public void WriteTypeDefinitionAsKeyPayloadShouldWork()
        {
            EdmModel model = new EdmModel();

            EdmTypeDefinition uint32Type = new EdmTypeDefinition("NS", "UInt32", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference uint32Reference = new EdmTypeDefinitionReference(uint32Type, false);
            model.AddElement(uint32Type);

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id1", uint32Reference), entityType.AddStructuralProperty("Id2", EdmPrimitiveTypeKind.Int32));
            model.SetPrimitiveValueConverter(uint32Reference, new MyUInt32Converter());

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Double);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);

            EdmComplexType complexType = new EdmComplexType("NS", "Address");
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            EdmTypeDefinition addressType = new EdmTypeDefinition("NS", "Address", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference addressTypeRef = new EdmTypeDefinitionReference(addressType, false);
            complexType.AddStructuralProperty("CountryRegion", addressTypeRef);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(weightType);
            model.AddElement(addressType);
            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            ODataEntry entry = new ODataEntry()
            {
                TypeName = "NS.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "Id1", Value = (UInt32)1 },
                    new ODataProperty { Name = "Id2", Value = 2 },
                    new ODataProperty { Name = "Weight", Value = 60.5 },
                    new ODataProperty
                    {
                        Name = "Address",
                        Value = new ODataComplexValue
                        {
                            Properties = new[]
                            {
                                new ODataProperty { Name = "CountryRegion", Value = "China" }
                            }
                        }
                    }
                }
            };

            string outputPayload = this.WriterEntry(model, entry, entitySet, entityType);

            const string expectedMinimalPayload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#People/$entity\"," +
                "\"Id1\":\"1\"," +
                "\"Id2\":2," +
                "\"Weight\":60.5," +
                "\"Address\":{\"CountryRegion\":\"China\"}" +
                "}";

            outputPayload.Should().Be(expectedMinimalPayload);

            outputPayload = this.WriterEntry(model, entry, entitySet, entityType, true);

            const string expectedFullPayload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#People/$entity\"," +
                "\"@odata.type\":\"#NS.Person\"," +
                "\"@odata.id\":\"People(Id1='1',Id2=2)\"," +
                "\"@odata.editLink\":\"People(Id1='1',Id2=2)\"," +
                "\"Id1\":\"1\"," +
                "\"Id2\":2," +
                "\"Weight\":60.5," +
                "\"Address\":{\"CountryRegion\":\"China\"}" +
                "}";

            outputPayload.Should().Be(expectedFullPayload);
        }

        [Fact]
        public void WriteTypeDefinitionPayloadWithIncompatibleTypeShouldFail()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            EdmTypeDefinition weightType = new EdmTypeDefinition("NS", "Weight", EdmPrimitiveTypeKind.Double);
            EdmTypeDefinitionReference weightTypeRef = new EdmTypeDefinitionReference(weightType, false);
            entityType.AddStructuralProperty("Weight", weightTypeRef);

            model.AddElement(weightType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            ODataEntry entry = new ODataEntry()
            {
                TypeName = "NS.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Weight", Value = "abc" },
                }
            };

            Action write = () => this.WriterEntry(model, entry, entitySet, entityType);
            write.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatiblePrimitiveItemType("Edm.String", "True", "NS.Weight", "False"));
        }

        [Fact]
        public void WriteUIntPayloadShouldWork()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Guid", model.GetUInt64("NS", false));

            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("Ns", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            ODataEntry entry = new ODataEntry()
            {
                TypeName = "NS.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Guid", Value = UInt64.MaxValue }
                }
            };

            string outputPayload = this.WriterEntry(model, entry, entitySet, entityType);

            const string expectedMinimalPayload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#People/$entity\"," +
                "\"Id\":1," +
                "\"Guid\":18446744073709551615" +
                "}";

            outputPayload.Should().Be(expectedMinimalPayload);

            outputPayload = this.WriterEntry(model, entry, entitySet, entityType, true);

            const string expectedFullPayload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#People/$entity\"," +
                "\"@odata.type\":\"#NS.Person\"," +
                "\"@odata.id\":\"People(1)\"," +
                "\"@odata.editLink\":\"People(1)\"," +
                "\"Id\":1," +
                "\"Guid\":18446744073709551615" +
                "}";

            outputPayload.Should().Be(expectedFullPayload);
        }

        [Fact]
        public void WriteDynamicPropertyOfUIntIsNotSupported()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person", null, false, true);
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("Ns", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            ODataEntry entry = new ODataEntry()
            {
                TypeName = "NS.Person",
                Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Guid", Value = UInt64.MaxValue }
                }
            };

            Action write = () => this.WriterEntry(model, entry, entitySet, entityType);
            write.ShouldThrow<ODataException>().WithMessage("The value of type 'System.UInt64' is not supported and cannot be converted to a JSON representation.");
        }

        private string WriterEntry(IEdmModel userModel, ODataEntry entry, EdmEntitySet entitySet, IEdmEntityType entityType, bool fullMetadata = false)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream() };

            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true, AutoComputePayloadMetadataInJson = true };
            writerSettings.SetContentType(ODataFormat.Json);
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com"));
            writerSettings.SetContentType(fullMetadata ?
                "application/json;odata.metadata=full;odata.streaming=false" :
                "application/json;odata.metadata=minimal;odata.streaming=false", "utf-8");

            using (var msgReader = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, userModel))
            {
                var writer = msgReader.CreateODataEntryWriter(entitySet, entityType);
                writer.WriteStart(entry);
                writer.WriteEnd();
            }

            message.Stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(message.Stream))
            {
                return reader.ReadToEnd();
            }
        }

        private class MyUInt32Converter : IPrimitiveValueConverter
        {
            public object ConvertToUnderlyingType(object value)
            {
                return Convert.ToString(value);
            }

            public object ConvertFromUnderlyingType(object value)
            {
                return Convert.ToUInt32(value);
            }
        }
    }
}
