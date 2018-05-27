//---------------------------------------------------------------------
// <copyright file="ODataJsonLightPropertyTypeSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Writer.JsonLight
{
    public class ODataJsonLightPropertyTypeSerializerTests
    {
        private IEdmModel model;
        private IEdmEntityType entityType;
        private IEdmEntitySet entitySet;
        private IEdmComplexType complexType;

        public ODataJsonLightPropertyTypeSerializerTests()
        {
            EdmModel edmModel = new EdmModel();
            EdmEntityType edmEntityType = new EdmEntityType("TestNamespace", "EntityType", baseType: null, isAbstract: false, isOpen: true);
            edmModel.AddElement(edmEntityType);
            EdmComplexType ComplexType = new EdmComplexType("TestNamespace", "TestComplexType");
            ComplexType.AddProperty(new EdmStructuralProperty(ComplexType, "StringProperty", EdmCoreModel.Instance.GetString(false)));
            edmModel.AddElement(ComplexType);
            EdmComplexType derivedComplexType = new EdmComplexType("TestNamespace", "TestDerivedComplexType", ComplexType, true);
            derivedComplexType.AddProperty(new EdmStructuralProperty(derivedComplexType, "DerivedStringProperty", EdmCoreModel.Instance.GetString(false)));
            edmModel.AddElement(derivedComplexType);

            var defaultContainer = new EdmEntityContainer("TestNamespace", "DefaultContainer_sub");
            edmModel.AddElement(defaultContainer);
            this.entitySet = new EdmEntitySet(defaultContainer, "TestEntitySet", edmEntityType);
            defaultContainer.AddElement(this.entitySet);

            this.model = TestUtils.WrapReferencedModelsToMainModel("TestNamespace", "DefaultContainer", edmModel);
            this.entityType = edmEntityType;
            this.complexType = ComplexType;
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeBinary()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = new byte[] { 1, 2, 4 } });
            result.Should().Contain("@odata.type\":\"#Binary");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeBinary_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = new byte[] { 1, 2, 4 } }, ODataVersion.V401);
            result.Should().Contain("@type\":\"Binary");
        }

        [Fact]
        public void ShouldNotWriteODataTypeForPrimitiveTypeBoolean()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = true });
            result.Should().NotContain("@odata.type");
        }

        [Fact]
        public void ShouldNotWriteODataTypeForPrimitiveTypeBoolean_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = true }, ODataVersion.V401);
            result.Should().NotContain("@type");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeByte()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Byte.Parse("12") });
            result.Should().Contain("@odata.type\":\"#Byte");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeByte_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Byte.Parse("12") }, ODataVersion.V401);
            result.Should().Contain("type\":\"Byte");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeDate()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = new Date(2014, 8, 8) });
            result.Should().Contain("@odata.type\":\"#Date");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeDate_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = new Date(2014, 8, 8) }, ODataVersion.V401);
            result.Should().Contain("@type\":\"Date");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeDateTimeOffset()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = new DateTimeOffset(2013, 11, 28, 12, 12, 12, TimeSpan.Zero) });
            result.Should().Contain("@odata.type\":\"#DateTimeOffset");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeDateTimeOffset_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = new DateTimeOffset(2013, 11, 28, 12, 12, 12, TimeSpan.Zero) }, ODataVersion.V401);
            result.Should().Contain("@type\":\"DateTimeOffset");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeDecimal()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Decimal.Parse("111.100") });
            result.Should().Contain("@odata.type\":\"#Decimal");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeDecimal_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Decimal.Parse("111.100") }, ODataVersion.V401);
            result.Should().Contain("@type\":\"Decimal");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeNanDouble()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Double.NaN });
            result.Should().Contain("@odata.type\":\"#Double");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeInfiniteDouble()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Double.NegativeInfinity });
            result.Should().Contain("@odata.type\":\"#Double");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeInfiniteDouble_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Double.NegativeInfinity }, ODataVersion.V401);
            result.Should().Contain("@type\":\"Double");
        }

        [Fact]
        public void ShouldNotWriteODataTypeForPrimitiveTypeNormalDouble()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Double.Parse("111.11") });
            result.Should().NotContain("@odata.type");
        }

        [Fact]
        public void ShouldNotWriteODataTypeForPrimitiveTypeNormalDouble_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Double.Parse("111.11") }, ODataVersion.V401);
            result.Should().NotContain("@type");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeGuid()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Guid.Empty });
            result.Should().Contain("@odata.type\":\"#Guid");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeGuid_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Guid.Empty }, ODataVersion.V401);
            result.Should().Contain("@type\":\"Guid");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeInt16()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Int16.Parse("16") });
            result.Should().Contain("@odata.type\":\"#Int16");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeInt16_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Int16.Parse("16") }, ODataVersion.V401);
            result.Should().Contain("@type\":\"Int16");
        }

        [Fact]
        public void ShouldNotWriteODataTypeForPrimitiveTypeInt32()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Int32.Parse("32") });
            result.Should().NotContain("@odata.type");
        }

        [Fact]
        public void ShouldNotWriteODataTypeForPrimitiveTypeInt32_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Int32.Parse("32") }, ODataVersion.V401);
            result.Should().NotContain("@type");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeInt64()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Int64.Parse("64") });
            result.Should().Contain("@odata.type\":\"#Int64");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeInt64_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = Int64.Parse("64") }, ODataVersion.V401);
            result.Should().Contain("@type\":\"Int64");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeSByte()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = SByte.Parse("1") });
            result.Should().Contain("@odata.type\":\"#SByte");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeSByte_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = SByte.Parse("1") }, ODataVersion.V401);
            result.Should().Contain("@type\":\"SByte");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeTimeOfDay()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = new TimeOfDay(23, 59, 59, 0) });
            result.Should().Contain("@odata.type\":\"#TimeOfDay");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeTimeOfDay_401()
        {
            string result = this.SerializeProperty(new ODataProperty() { Name = "TestProperty", Value = new TimeOfDay(23, 59, 59, 0) }, ODataVersion.V401);
            result.Should().Contain("@type\":\"TimeOfDay");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeGeometry()
        {
            var testCases = new[]
            {
                new
                {
                    Expect = "GeometryProperty@odata.type\":\"#GeometryPoint",
                    Value = new ODataProperty { Name = "GeometryProperty", Value= ODataSpatialTypeUtil.GeometryValue }
                },
                new
                {
                    Expect = "GeometryCollectionProperty@odata.type\":\"#GeometryCollection",
                    Value = new ODataProperty { Name = "GeometryCollectionProperty", Value= ODataSpatialTypeUtil.GeometryCollectionValue }
                },
                new
                {
                    Expect = "GeometryLineStringProperty@odata.type\":\"#GeometryLineString",
                    Value = new ODataProperty { Name = "GeometryLineStringProperty", Value= ODataSpatialTypeUtil.GeometryLineStringValue }
                },
                new
                {
                    Expect = "GeometryMultiLineStringProperty@odata.type\":\"#GeometryMultiLineString",
                    Value = new ODataProperty { Name = "GeometryMultiLineStringProperty", Value= ODataSpatialTypeUtil.GeometryMultiLineStringValue }
                },
                new
                {
                    Expect = "GeometryMultiPointProperty@odata.type\":\"#GeometryMultiPoint",
                    Value = new ODataProperty { Name = "GeometryMultiPointProperty", Value= ODataSpatialTypeUtil.GeometryMultiPointValue }
                },
                new
                {
                    Expect = "GeometryMultiPolygonProperty@odata.type\":\"#GeometryMultiPolygon",
                    Value = new ODataProperty { Name = "GeometryMultiPolygonProperty", Value= ODataSpatialTypeUtil.GeometryMultiPolygonValue }
                },
                new
                {
                    Expect = "GeometryPointProperty@odata.type\":\"#GeometryPoint",
                    Value = new ODataProperty { Name = "GeometryPointProperty", Value= ODataSpatialTypeUtil.GeometryPointValue }
                },
                new
                {
                    Expect = "GeometryPolygonProperty@odata.type\":\"#GeometryPolygon",
                    Value = new ODataProperty { Name = "GeometryPolygonProperty", Value= ODataSpatialTypeUtil.GeometryPolygonValue }
                }
            };

            // test
            foreach (var testCase in testCases)
            {
                string result = this.SerializeProperty(testCase.Value);
                result.Should().Contain(testCase.Expect);
            }
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeGeography()
        {
            var testCases = new[]
            {
                new
                {
                    Expect = "GeometryProperty@odata.type\":\"#GeographyPoint",
                    Value = new ODataProperty { Name = "GeometryProperty", Value= ODataSpatialTypeUtil.GeographyValue }
                },
                new
                {
                    Expect = "GeometryCollectionProperty@odata.type\":\"#GeographyCollection",
                    Value = new ODataProperty { Name = "GeometryCollectionProperty", Value= ODataSpatialTypeUtil.GeographyCollectionValue }
                },
                new
                {
                    Expect = "GeometryLineStringProperty@odata.type\":\"#GeographyLineString",
                    Value = new ODataProperty { Name = "GeometryLineStringProperty", Value= ODataSpatialTypeUtil.GeographyLineStringValue }
                },
                new
                {
                    Expect = "GeometryMultiLineStringProperty@odata.type\":\"#GeographyMultiLineString",
                    Value = new ODataProperty { Name = "GeometryMultiLineStringProperty", Value= ODataSpatialTypeUtil.GeographyMultiLineStringValue }
                },
                new
                {
                    Expect = "GeometryMultiPointProperty@odata.type\":\"#GeographyMultiPoint",
                    Value = new ODataProperty { Name = "GeometryMultiPointProperty", Value= ODataSpatialTypeUtil.GeographyMultiPointValue }
                },
                new
                {
                    Expect = "GeometryMultiPolygonProperty@odata.type\":\"#GeographyMultiPolygon",
                    Value = new ODataProperty { Name = "GeometryMultiPolygonProperty", Value= ODataSpatialTypeUtil.GeographyMultiPolygonValue }
                },
                new
                {
                    Expect = "GeometryPointProperty@odata.type\":\"#GeographyPoint",
                    Value = new ODataProperty { Name = "GeometryPointProperty", Value= ODataSpatialTypeUtil.GeographyPointValue }
                },
                new
                {
                    Expect = "GeometryPolygonProperty@odata.type\":\"#GeographyPolygon",
                    Value = new ODataProperty { Name = "GeometryPolygonProperty", Value= ODataSpatialTypeUtil.GeographyPolygonValue }
                }
            };

            // test
            foreach (var testCase in testCases)
            {
                string result = this.SerializeProperty(testCase.Value);
                result.Should().Contain(testCase.Expect);
            }
        }

        [Fact]
        public void WriteCollectionInstanceAnnotationShouldIncludeType()
        {
            var testCases = new[]
            {
                new
                {
                    Expect = "@odata.type\":\"#Collection(String)\"",
                    InstanceName = "NS.StringCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.String)", Items = new[] { "StringValue1", "StringValue2" } }
                },
                new
                {
                    Expect = "@odata.type\":\"#Collection(Guid)\"",
                    InstanceName = "NS.GuidCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.Guid)", Items = new object[] { Guid.Empty, Guid.Empty } }
                },
                new
                {
                    Expect = "@odata.type\":\"#Collection(Double)\"",
                    InstanceName = "NS.DoubleCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.Double)", Items = new object[] { 11.11, 22.22 } }
                },
                new
                {
                    Expect = "@odata.type\":\"#Collection(Decimal)\"",
                    InstanceName = "NS.DecimalCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.Decimal)", Items = new object[] { 11.11m, 22.22m } }
                },
                new
                {
                    Expect = "@odata.type\":\"#Collection(Int32)\"",
                    InstanceName = "NS.DecimalCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.Int32)", Items = new object[] { 1, 2, 3, 5 } }
                },

                // #625

                //new
                //{
                //    Expect = "@odata.type\":\"#Collection(TestNamespace.TestComplexType)\"",
                //    InstanceName = "NS.CustomComplexCollectionProperty",
                //    CollectionValue = new ODataCollectionValue
                //    {
                //        Items = new[]
                //        {
                //            new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue1" } }, TypeName = "TestNamespace.TestComplexType" },
                //            new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue2" } }, TypeName = "TestNamespace.TestComplexType" }
                //        },
                //        TypeName = "Collection(TestNamespace.TestComplexType)"
                //    }
                //},
                //new
                //{
                //    Expect = "@odata.type\":\"#Collection(TestNamespace.TestComplexType)\"",
                //    InstanceName = "NS.CustomComplexCollectionProperty",
                //    CollectionValue = new ODataCollectionValue
                //    {
                //        Items = new[]
                //        {
                //            new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue1" } }, TypeName = "TestNamespace.TestComplexType" },
                //            new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue2" }, new ODataProperty { Name = "DerivedStringProperty", Value = "DerivedStringProperty2" } }, TypeName = "TestNamespace.TestDerivedComplexType" }
                //        },
                //        TypeName = "Collection(TestNamespace.TestComplexType)"
                //    }
                //}
            };

            foreach (var testCase in testCases)
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "MyProperty", Value = 1 } } };
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation(testCase.InstanceName, testCase.CollectionValue));
                string result = this.WriteODataEntry(entryToWrite);
                result.Should().Contain(testCase.Expect);
            }
        }

        #region assist private functions
        private string SerializeProperty(ODataProperty odataProperty, ODataVersion version = ODataVersion.V4)
        {
            MemoryStream outputStream = new MemoryStream();
            ODataJsonLightOutputContext jsonLightOutputContext = this.CreateJsonLightOutputContext(outputStream, version);
            var serializer = new ODataJsonLightPropertySerializer(jsonLightOutputContext);

            jsonLightOutputContext.JsonWriter.StartObjectScope();
            serializer.WriteProperties(
                this.entityType,
                new[] { odataProperty },
                /*isComplexValue*/ false,
                new NullDuplicatePropertyNameChecker());
            jsonLightOutputContext.JsonWriter.EndObjectScope();

            jsonLightOutputContext.Flush();
            outputStream.Position = 0;
            string result = new StreamReader(outputStream).ReadToEnd();
            return result;
        }

        private string WriteODataEntry(ODataResource entryToWrite)
        {
            var writerSettings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false };
            writerSettings.SetContentType(ODataFormat.Json);
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));
            MemoryStream stream = new MemoryStream();
            IODataRequestMessage requestMessageToWrite = new InMemoryMessage { Method = "GET", Stream = stream };
            using (var messageWriter = new ODataMessageWriter(requestMessageToWrite, writerSettings, this.model))
            {
                ODataWriter odataWriter = messageWriter.CreateODataResourceWriter(this.entitySet, this.entityType);
                odataWriter.WriteStart(entryToWrite);
                odataWriter.WriteEnd();
            }
            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();
            return payload;
        }

        private ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, ODataVersion version = ODataVersion.V4)
        {
            var settings = new ODataMessageWriterSettings { Version = version };
            settings.SetServiceDocumentUri(new Uri("http://example.com/"));

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = true,
                IsAsync = false,
                Model = this.model
            };

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }
        #endregion
    }
}
