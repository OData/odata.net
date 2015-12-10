//---------------------------------------------------------------------
// <copyright file="PropertyAndValueAtomWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Writer.Atom
{
    public class PropertyAndValueAtomWriterIntegrationTests
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");
        private static readonly ODataFeedAndEntrySerializationInfo MySerializationInfo = new ODataFeedAndEntrySerializationInfo()
        {
            NavigationSourceEntityTypeName = "NS.MyTestEntity",
            NavigationSourceName = "MyTestEntity",
            ExpectedTypeName = "NS.MyTestEntity"
        };

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
            entityType.AddStructuralProperty(
                "IntNumbers",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            entityType.AddStructuralProperty(
                "NullableIntNumbers",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true))));
            model.AddElement(entityType);

            ODataEntry entry = new ODataEntry()
            {
                Id = new Uri("http://test/Id"),
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
                SerializationInfo = MySerializationInfo
            };

            string outputPayload = this.WriterEntry(model, entry, entityType);
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" " +
                    "xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" " +
                    "xmlns:gml=\"http://www.opengis.net/gml\" " +
                    "m:context=\"http://odata.org/$metadata#MyTestEntity/$entity\">" +
                    "<id>http://test/Id</id>" +
                    "<category term=\"#NS.MyTestEntity\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<title />" + // "<updated>2013-10-01T19:35:43Z</updated>" +
                    "<author><name /></author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:LongId m:type=\"Int64\">12</d:LongId>" +
                            "<d:FloatId m:type=\"Single\">34.98</d:FloatId>" +
                            "<d:DoubleId m:type=\"Double\">56.01</d:DoubleId>" +
                            "<d:DecimalId m:type=\"Decimal\">78.62</d:DecimalId>" +
                            "<d:BoolValue1 m:type=\"Boolean\">true</d:BoolValue1>" +
                            "<d:BoolValue2 m:type=\"Boolean\">false</d:BoolValue2>" +
                             "<d:LongNumbers m:type=\"#Collection(Int64)\">" +
                                "<m:element>0</m:element>" +
                                "<m:element>-9223372036854775808</m:element>" +
                                "<m:element>9223372036854775807</m:element>" +
                             "</d:LongNumbers>" +
                             "<d:FloatNumbers m:type=\"#Collection(Single)\">" +
                                "<m:element>1</m:element>" +
                                "<m:element>-3.40282347E+38</m:element>" +
                                "<m:element>3.40282347E+38</m:element>" +
                                "<m:element>INF</m:element>" +
                                "<m:element>-INF</m:element>" +
                                "<m:element>NaN</m:element>" +
                             "</d:FloatNumbers>" +
                             "<d:DoubleNumbers m:type=\"#Collection(Double)\">" +
                                "<m:element>-1</m:element>" +
                                "<m:element>-1.7976931348623157E+308</m:element>" +
                                "<m:element>1.7976931348623157E+308</m:element>" +
                                "<m:element>INF</m:element>" +
                                "<m:element>-INF</m:element>" +
                                "<m:element>NaN</m:element>" +
                             "</d:DoubleNumbers>" +
                             "<d:DecimalNumbers m:type=\"#Collection(Decimal)\">" +
                                "<m:element>0</m:element>" +
                                "<m:element>-79228162514264337593543950335</m:element>" +
                                "<m:element>79228162514264337593543950335</m:element>" +
                             "</d:DecimalNumbers>" +
                             "<d:ComplexProperty m:type=\"#NS.MyTestComplexType\">" +
                               "<d:CLongId m:type=\"Int64\">1</d:CLongId>" +
                               "<d:CFloatId m:type=\"Single\">-1</d:CFloatId>" +
                               "<d:CDoubleId m:type=\"Double\">1</d:CDoubleId>" +
                               "<d:CDecimalId m:type=\"Decimal\">1.0</d:CDecimalId>" +
                             "</d:ComplexProperty>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";

            outputPayload = Regex.Replace(outputPayload, "<updated>[^<]*</updated>", "");
            outputPayload.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingNullableCollectionValue()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key =
                entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            entityType.AddKeys(key);
            entityType.AddStructuralProperty(
                "NullableIntNumbers",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true))));
            model.AddElement(entityType);

            var collectionValue = new ODataCollectionValue()
            {
                Items = new int?[] { null },
                TypeName = "Collection(Edm.Int32)"
            };

            ODataEntry entry = new ODataEntry()
            {
                Id = new Uri("http://test/Id"),
                TypeName = "NS.MyTestEntity",
                Properties = new[]
                {
                    new ODataProperty { Name = "NullableIntNumbers", Value = collectionValue },
                },
                SerializationInfo = MySerializationInfo
            };

            string outputPayload = this.WriterEntry(model, entry, entityType);
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" " +
                    "xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" " +
                    "xmlns:gml=\"http://www.opengis.net/gml\" " +
                    "m:context=\"http://odata.org/$metadata#MyTestEntity/$entity\">" +
                    "<id>http://test/Id</id>" +
                    "<category term=\"#NS.MyTestEntity\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<title />" +
                    "<author><name /></author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                             "<d:NullableIntNumbers m:type=\"#Collection(Int32)\">" +
                                "<m:element m:null=\"true\" />" +
                             "</d:NullableIntNumbers>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";

            outputPayload = Regex.Replace(outputPayload, "<updated>[^<]*</updated>", "");
            outputPayload.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingNonNullableCollectionValueWithNullElementShouldThrow()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key =
                entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            entityType.AddKeys(key);
            entityType.AddStructuralProperty(
                "NonNullableIntNumbers",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            model.AddElement(entityType);

            var collectionValue = new ODataCollectionValue()
            {
                Items = new int?[] { null },
                TypeName = "Collection(Edm.Int32)"
            };

            ODataEntry entry = new ODataEntry()
            {
                Id = new Uri("http://test/Id"),
                TypeName = "NS.MyTestEntity",
                Properties = new[]
                {
                    new ODataProperty { Name = "NonNullableIntNumbers", Value = collectionValue },
                },
                SerializationInfo = PropertyAndValueAtomWriterIntegrationTests.MySerializationInfo
            };

            Action test = () => this.WriterEntry(model, entry, entityType);

            test.ShouldThrow<ODataException>().WithMessage(
                Strings.ValidationUtils_NonNullableCollectionElementsMustNotBeNull);
        }

        private string WriterEntry(EdmModel userModel, ODataEntry entry, IEdmEntityType entityType)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream() };
            message.SetHeader("Content-Type", "application/atom+xml;type=entry");
            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true, EnableAtom = true };
            writerSettings.SetServiceDocumentUri(ServiceDocumentUri);
            using (var msgReader = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, userModel))
            {
                var writer = msgReader.CreateODataEntryWriter();
                writer.WriteStart(entry);
                writer.WriteEnd();
            }

            message.Stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(message.Stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
