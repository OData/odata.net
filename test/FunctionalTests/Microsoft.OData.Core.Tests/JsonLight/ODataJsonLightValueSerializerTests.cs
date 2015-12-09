//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValueSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    /// <summary>
    /// Unit tests and short-span integration tests for ODataJsonLightValueSerializer.
    /// </summary>
    public class ODataJsonLightValueSerializerTests
    {
        private EdmModel model;
        private MemoryStream stream;
        private ODataMessageWriterSettings settings;

        public ODataJsonLightValueSerializerTests()
        {
            model = new EdmModel();
            stream = new MemoryStream();
            settings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true, Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://example.com"));
        }

        // Regression for: Validates when no metadata is specified WriteCollection will fail
        [Fact]
        public void WritingCollectionValueShouldFailWhenNoTypeSpecified()
        {
            var serializer = CreateODataJsonLightValueSerializer(false);

            var collectionValue = new ODataCollectionValue() { Items = new List<string>() };

            Action test = () => serializer.WriteCollectionValue(collectionValue, null, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest);
        }

        [Fact]
        public void WritingCollectionValueDifferingFromExpectedTypeShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue() { Items = new List<string>(), TypeName = "Collection(Edm.Int32)" };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(Edm.Int32)", "Collection(Edm.String)"));
        }

        [Fact]
        public void WritingCollectionValueDifferingFromExpectedTypeShouldFail_WithoutEdmPrefix()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue() { Items = new List<string>(), TypeName = "Collection(Int32)" };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(Edm.Int32)", "Collection(Edm.String)"));
        }

        [Fact]
        public void WritingCollectionValueElementWithNullValue()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue()
                {
                    Items = new int?[] { null },
                    TypeName = "Collection(Edm.Int32)"
                };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
            serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, false, false, false);
        }

        [Fact]
        public void WritingCollectionValueWithNullValueForNonNullableElementTypeShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue()
            {
                Items = new int?[] { null },
                TypeName = "Collection(Edm.Int32)"
            };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_NonNullableCollectionElementsMustNotBeNull);
        }

        [Fact]
        public void WritingTypeDefinitionValueWithValueOfIncompatibleTypeShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var uint64 = model.GetUInt64("NS", true);

            Action test = () => serializer.WritePrimitiveValue("123", uint64);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatiblePrimitiveItemType("Edm.String", true, "NS.UInt64", true));
        }

        [Fact]
        public void WritingInstanceAnnotationInComplexValueShouldWrite()
        {
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var complexValue = new ODataComplexValue { TypeName = "TestNamespace.Address", InstanceAnnotations = new Collection<ODataInstanceAnnotation> { new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(true)) } };

                var complexTypeRef = new EdmComplexTypeReference(complexType, false);
                serializer.WriteComplexValue(complexValue, complexTypeRef, false, false, new DuplicatePropertyNamesChecker(false, true));
            });

            result.Should().Contain("\"@Is.ReadOnly\":true");
        }

        [Fact]
        public void WritingMultipleInstanceAnnotationInComplexValueShouldWrite()
        {
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var complexValue = new ODataComplexValue
                {
                    TypeName = "TestNamespace.Address",
                    InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Annotation.1", new ODataPrimitiveValue(true)),
                        new ODataInstanceAnnotation("Annotation.2", new ODataPrimitiveValue(123)),
                        new ODataInstanceAnnotation("Annotation.3", new ODataPrimitiveValue("annotation"))
                    }
                };

                var complexTypeRef = new EdmComplexTypeReference(complexType, false);
                serializer.WriteComplexValue(complexValue, complexTypeRef, false, false, new DuplicatePropertyNamesChecker(false, true));
            });

            result.Should().Contain("\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\"");
        }

        [Fact]
        public void WritingMultipleInstanceAnnotationInComplexValueShouldSkipBaseOnSettings()
        {
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var complexValue = new ODataComplexValue
                {
                    TypeName = "TestNamespace.Address",
                    InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Annotation.1", new ODataPrimitiveValue(true)),
                        new ODataInstanceAnnotation("Annotation.2", new ODataPrimitiveValue(123)),
                        new ODataInstanceAnnotation("Annotation.3", new ODataPrimitiveValue("annotation"))
                    }
                };

                var complexTypeRef = new EdmComplexTypeReference(complexType, false);
                serializer.WriteComplexValue(complexValue, complexTypeRef, false, false, new DuplicatePropertyNamesChecker(false, true));
            });

            result.Should().NotContain("\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\"");
        }

        [Fact]
        public void WritingDateTimeOffsetWithCustomFormat()
        {
            var df = EdmCoreModel.Instance.GetDateTimeOffset(false);
            model.SetPayloadValueConverter(new DateTimeOffsetCustomFormatPrimitivePayloadValueConverter());
            
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, TimeSpan.FromHours(8));
                serializer.WritePrimitiveValue(value, df);
            });

            result.Should().Be("\"Thu, 12 Apr 2012 18:43:10 GMT\"");
        }


        private ODataJsonLightValueSerializer CreateODataJsonLightValueSerializer(bool writingResponse)
        {
            var context = new ODataJsonLightOutputContext(ODataFormat.Json, stream, new ODataMediaType("application", "json"), Encoding.Default, settings, writingResponse, true, model, null);
            var serializer = new ODataJsonLightValueSerializer(context);
            return serializer;
        }

        /// <summary>
        /// Sets up a ODataJsonLightSerializer, runs the given test code, and then flushes and reads the stream back as a string for
        /// customized verification.
        /// </summary>
        private string SetupSerializerAndRunTest(Action<ODataJsonLightValueSerializer> action)
        {
            var serializer = CreateODataJsonLightValueSerializer(true);
            action(serializer);
            serializer.JsonWriter.Flush();
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}
