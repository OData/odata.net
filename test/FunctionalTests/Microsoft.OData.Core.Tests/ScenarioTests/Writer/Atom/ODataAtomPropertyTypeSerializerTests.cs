//---------------------------------------------------------------------
// <copyright file="ODataAtomPropertyTypeSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Core.Tests.ScenarioTests.Writer.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class ODataAtomPropertyTypeSerializerTests
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");
        private Stream stream;
        private ODataMessageWriterSettings settings;
        private ODataAtomPropertyAndValueSerializer serializer;
        private InstanceAnnotationWriteTracker instanceAnnotationWriteTracker;
        private EdmModel model;

        public ODataAtomPropertyTypeSerializerTests()
        {
            model = new EdmModel();
            EdmComplexType complexType = new EdmComplexType("ns", "complex");
            complexType.AddProperty(new EdmStructuralProperty(complexType, "StringProperty", EdmCoreModel.Instance.GetString(isNullable: false)));
            model.AddElement(complexType);

            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            this.settings.SetServiceDocumentUri(ServiceDocumentUri);
            this.serializer = new ODataAtomPropertyAndValueSerializer(this.CreateAtomOutputContext(model, this.stream));
            this.instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
        }

        #region Basic Primititive type
        [Fact]
        public void WriteInstanceAnnotationsShouldIncludeTypeBinary()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(new byte[] { 1, 2, 4 })), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldContain("m:type=\"Binary\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldNotIncludeTypeBoolean()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(true)), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldNotContain("m:type=\"Boolean\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldIncludeTypeByte()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(Byte.Parse("12"))), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldContain("m:type=\"Byte\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldIncludeTypeDateTimeOffset()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(new DateTimeOffset(2013, 11, 28, 12, 12, 12, TimeSpan.Zero))), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldContain("m:type=\"DateTimeOffset\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldNotIncludeTypeDecimal()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(Decimal.Parse("111.100"))), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldNotContain("m:type=\"Decimal\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldNotIncludeTypeDoubleForNaN()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(Double.NaN)), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldNotContain("m:type=\"Double\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldNotIncludeTypeDouble()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(Double.Parse("111.11"))), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldNotContain("m:type=\"Double\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldIncludeTypeGuid()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(Guid.Empty)), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldContain("m:type=\"Guid\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldIncludeTypeInt16()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(Int16.Parse("16"))), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldContain("m:type=\"Int16\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldNotIncludeTypeInt32()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(Int32.Parse("32"))), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldNotContain("m:type=\"Int32\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldIncludeTypeInt64()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(Int64.Parse("64"))), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldContain("m:type=\"Int64\"");
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldIncludeTypeSByte()
        {
            var primitive = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", new ODataPrimitiveValue(SByte.Parse("2"))), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { primitive }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldContain("m:type=\"SByte\"");
        }
        #endregion

        [Fact]
        public void WriteInstanceAnnotationsShouldIncludeTypeComplexType()
        {
            ODataComplexValue complexValue = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue1" }  }, TypeName = "ns.complex" };
            var complex = AtomInstanceAnnotation.CreateFrom(new ODataInstanceAnnotation("ns.term", complexValue), /*target*/ null);
            this.serializer.WriteInstanceAnnotations(new[] { complex }, instanceAnnotationWriteTracker);
            this.ValidatePayloadShouldContain("m:type=\"#ns.complex\"");
        }

        [Fact]
        public void ShouldWriteODataTypeForPrimitiveTypeGeometry()
        {
            var testCases = new[]
            {
                new
                {
                    Expect = "m:type=\"GeometryPoint",
                    Value = new ODataProperty { Name = "GeometryProperty", Value= ODataSpatialTypeUtil.GeometryValue }
                },
                new
                {
                    Expect = "m:type=\"GeometryCollection",
                    Value = new ODataProperty { Name = "GeometryCollectionProperty", Value= ODataSpatialTypeUtil.GeometryCollectionValue }
                },
                new
                {
                    Expect = "m:type=\"GeometryLineString",
                    Value = new ODataProperty { Name = "GeometryLineStringProperty", Value= ODataSpatialTypeUtil.GeometryLineStringValue }
                },
                new
                {
                    Expect = "m:type=\"GeometryMultiLineString",
                    Value = new ODataProperty { Name = "GeometryMultiLineStringProperty", Value= ODataSpatialTypeUtil.GeometryMultiLineStringValue }
                },
                new
                {
                    Expect = "m:type=\"GeometryMultiPoint",
                    Value = new ODataProperty { Name = "GeometryMultiPointProperty", Value= ODataSpatialTypeUtil.GeometryMultiPointValue }
                },
                new
                {
                    Expect = "m:type=\"GeometryMultiPolygon",
                    Value = new ODataProperty { Name = "GeometryMultiPolygonProperty", Value= ODataSpatialTypeUtil.GeometryMultiPolygonValue }
                },
                new
                {
                    Expect = "m:type=\"GeometryPoint",
                    Value = new ODataProperty { Name = "GeometryPointProperty", Value= ODataSpatialTypeUtil.GeometryPointValue }
                },
                new
                {
                    Expect = "m:type=\"GeometryPolygon",
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
                    Expect = "m:type=\"GeographyPoint",
                    Value = new ODataProperty { Name = "GeometryProperty", Value= ODataSpatialTypeUtil.GeographyValue }
                },
                new
                {
                    Expect = "m:type=\"GeographyCollection",
                    Value = new ODataProperty { Name = "GeometryCollectionProperty", Value= ODataSpatialTypeUtil.GeographyCollectionValue }
                },
                new
                {
                    Expect = "m:type=\"GeographyLineString",
                    Value = new ODataProperty { Name = "GeometryLineStringProperty", Value= ODataSpatialTypeUtil.GeographyLineStringValue }
                },
                new
                {
                    Expect = "m:type=\"GeographyMultiLineString",
                    Value = new ODataProperty { Name = "GeometryMultiLineStringProperty", Value= ODataSpatialTypeUtil.GeographyMultiLineStringValue }
                },
                new
                {
                    Expect = "m:type=\"GeographyMultiPoint",
                    Value = new ODataProperty { Name = "GeometryMultiPointProperty", Value= ODataSpatialTypeUtil.GeographyMultiPointValue }
                },
                new
                {
                    Expect = "m:type=\"GeographyMultiPolygon",
                    Value = new ODataProperty { Name = "GeometryMultiPolygonProperty", Value= ODataSpatialTypeUtil.GeographyMultiPolygonValue }
                },
                new
                {
                    Expect = "m:type=\"GeographyPoint",
                    Value = new ODataProperty { Name = "GeometryPointProperty", Value= ODataSpatialTypeUtil.GeographyPointValue }
                },
                new
                {
                    Expect = "m:type=\"GeographyPolygon",
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
                    Expect = "m:type=\"#Collection(String)\"", 
                    PropertyName = "StringCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.String)", Items = new[] { "StringValue1", "StringValue2" } }
                },
                new
                {
                    Expect = "m:type=\"#Collection(Guid)\"", 
                    PropertyName = "GuidCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.Guid)", Items = new[] { Guid.Empty, Guid.Empty } }
                },
                new
                {
                    Expect = "m:type=\"#Collection(Double)\"", 
                    PropertyName = "DoubleCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.Double)", Items = new[] { 11.11, 22.22 } }
                },
                new
                {
                    Expect = "m:type=\"#Collection(Decimal)\"", 
                    PropertyName = "DecimalCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.Decimal)", Items = new[] { 11.11m, 22.22m } }
                },
                new
                {
                    Expect = "m:type=\"#Collection(Int32)\"", 
                    PropertyName = "DecimalCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(Edm.Int32)", Items = new[] { 1, 2, 3, 5 } }
                },
                new
                {
                    Expect = "m:type=\"#Collection(ns.complex)\"", 
                    PropertyName = "ComplexCollectionProperty",
                    CollectionValue = new ODataCollectionValue { TypeName = "Collection(ns.complex)", Items = new [] { new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue1" }  }, TypeName = "ns.complex" }}}
                }
            };

            foreach (var testCase in testCases)
            {
                string result = SerializeProperty(new ODataProperty { Name = testCase.PropertyName, Value = testCase.CollectionValue } );
                result.Should().Contain(testCase.Expect);
            }
        }

        #region private helper functions
        private void ValidatePayloadShouldContain(string expectedPayload)
        {
            this.serializer.XmlWriter.Flush();
            this.stream.Position = 0;
            string payload = (new StreamReader(this.stream)).ReadToEnd();
            payload.Should().Contain(expectedPayload);
        }

        private void ValidatePayloadShouldNotContain(string expectedPayload)
        {
            this.serializer.XmlWriter.Flush();
            this.stream.Position = 0;
            string payload = (new StreamReader(this.stream)).ReadToEnd();
            payload.Should().NotContain(expectedPayload);
        }

        private string SerializeProperty(ODataProperty odataProperty)
        {
            MemoryStream outputStream = new MemoryStream();
            ODataAtomOutputContext atomOutputContext = this.CreateAtomOutputContext(this.model, outputStream);
            var serializer = new ODataAtomPropertyAndValueSerializer(atomOutputContext);

            serializer.WriteTopLevelProperty(odataProperty);
            atomOutputContext.Flush();
            outputStream.Position = 0;
            string result = new StreamReader(outputStream).ReadToEnd();

            return result;
        }

        private ODataAtomOutputContext CreateAtomOutputContext(IEdmModel model, Stream stream)
        {
            return new ODataAtomOutputContext(
                ODataFormat.Atom, 
                stream, 
                Encoding.UTF8, 
                this.settings, 
                /*writingResponse*/ true, 
                /*synchronous*/ true, 
                model, 
                /*urlResolver*/ null);
        }
        #endregion
    }
}
