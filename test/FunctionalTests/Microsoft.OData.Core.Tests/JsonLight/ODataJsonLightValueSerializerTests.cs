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
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
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
            settings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false, Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://example.com"));
        }

        // Regression for: Validates when no metadata is specified WriteCollection will fail
        [Fact]
        public void WritingCollectionValueShouldFailWhenNoTypeSpecified()
        {
            var serializer = CreateODataJsonLightValueSerializer(false);

            var collectionValue = new ODataCollectionValue() { Items = new List<string>() };

            Action test = () => serializer.WriteCollectionValue(collectionValue, null, null, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest);
        }

        [Fact]
        public void WritingCollectionValueDifferingFromExpectedTypeShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue() { Items = new List<string>(), TypeName = "Collection(Edm.Int32)" };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, null, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(Edm.Int32)", "Collection(Edm.String)"));
        }

        [Fact]
        public void WritingCollectionValueDifferingFromExpectedTypeShouldFail_WithoutEdmPrefix()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue() { Items = new List<string>(), TypeName = "Collection(Int32)" };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, null, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(Edm.Int32)", "Collection(Edm.String)"));
        }

        [Fact]
        public void WritingCollectionValueElementWithNullValue()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue()
                {
                    Items = new object[] { null },
                    TypeName = "Collection(Edm.Int32)"
                };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
            serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, null, false, false, false);
        }

        [Fact]
        public void WritingCollectionValueWithNullValueForNonNullableElementTypeShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue()
            {
                Items = new object[] { null },
                TypeName = "Collection(Edm.Int32)"
            };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, null, false, false, false);

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
        public void WritingDateTimeOffsetWithCustomFormat()
        {
            var df = EdmCoreModel.Instance.GetDateTimeOffset(false);

            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, TimeSpan.FromHours(8));
                serializer.WritePrimitiveValue(value, df);
            },
            ContainerBuilderHelper.BuildContainer(
                builder => builder.AddService<ODataPayloadValueConverter, DateTimeOffsetCustomFormatPrimitivePayloadValueConverter>(ServiceLifetime.Singleton)));

            result.Should().Be("\"Thu, 12 Apr 2012 18:43:10 GMT\"");
        }

        [Fact]
        public void WritingResourceValueWithoutTypeNameInTopLevelShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var resourceValue = new ODataResourceValue();

            Action test = () => serializer.WriteResourceValue(resourceValue, null, true /*isTopLevel*/, false, null);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightValueSerializer_MissingTypeNameOnResource);
        }

        [Fact]
        public void WritingResourceValueWithoutMetadataTypeAndWithoutTypeNameInRequestShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(false);

            var resourceValue = new ODataResourceValue();

            Action test = () => serializer.WriteResourceValue(resourceValue, null, false /*isTopLevel*/, false, null);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForResourceValueRequest);
        }

        [Fact]
        public void WritingResourceValueWithPropertiesShouldWrite()
        {
            var complexType = new EdmComplexType("NS", "Address");
            complexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);

            var entityType = new EdmEntityType("NS", "Customer");
            entityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            entityType.AddStructuralProperty("Location", new EdmComplexTypeReference(complexType, false));
            model.AddElement(entityType);

            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var resourceValue = new ODataResourceValue
                {
                    TypeName = "NS.Customer",
                    Properties = new []
                    {
                        new ODataProperty { Name = "Name", Value = "MyName" },
                        new ODataProperty { Name = "Location", Value = new ODataResourceValue
                            {
                                TypeName = "NS.Address",
                                Properties = new [] { new ODataProperty {  Name = "City", Value = "MyCity" } }
                            }
                        }
                    }
                };

                var entityTypeRef = new EdmEntityTypeReference(entityType, false);
                serializer.WriteResourceValue(resourceValue, entityTypeRef, false, false, serializer.CreateDuplicatePropertyNameChecker());
            });

            Assert.Equal(@"{""Name"":""MyName"",""Location"":{""City"":""MyCity""}}", result);
        }

        [Fact]
        public void WritingCollectionResourceValueWithPropertiesShouldWrite()
        {
            var complexType = new EdmComplexType("NS", "Address");
            complexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);

            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var resourceValue1 = new ODataResourceValue
                {
                    TypeName = "NS.Address",
                    Properties = new[] { new ODataProperty { Name = "City", Value = "MyCity1" } }
                };

                var resourceValue2 = new ODataResourceValue
                {
                    TypeName = "NS.Address",
                    Properties = new[] { new ODataProperty { Name = "City", Value = "MyCity2" } }
                };

                var collectionValue = new ODataCollectionValue
                {
                    TypeName = "Collection(NS.Address)",
                    Items = new[] { resourceValue1, resourceValue2 }
                };

                var collectionTypeRef = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complexType, false)));
                serializer.WriteCollectionValue(collectionValue, collectionTypeRef, null, false, false, false);
            });

            Assert.Equal(@"[{""City"":""MyCity1""},{""City"":""MyCity2""}]", result);
        }

        [Fact]
        public void WritingInstanceAnnotationInResourceValueShouldWrite()
        {
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var resourceValue = new ODataResourceValue
                {
                    TypeName = "TestNamespace.Address",
                    InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(true))
                    }
                };

                var complexTypeRef = new EdmComplexTypeReference(complexType, false);
                serializer.WriteResourceValue(resourceValue, complexTypeRef, false, false, serializer.CreateDuplicatePropertyNameChecker());
            });

            Assert.Equal(@"{""@Is.ReadOnly"":true}", result);
        }

        [Theory]
        [InlineData("*", @"{""@Custom.Bool"":true,""@Custom.Int"":123,""@My.String"":""annotation"",""@My.Bool"":false}")]
        [InlineData("-*", @"{}")]
        [InlineData("Custom.*", @"{""@Custom.Bool"":true,""@Custom.Int"":123}")]
        [InlineData("My.*", @"{""@My.String"":""annotation"",""@My.Bool"":false}")]
        [InlineData("My.Bool", @"{""@My.Bool"":false}")]
        public void WritingMultipleInstanceAnnotationInResourceValueShouldWrite(string filter, string expect)
        {
            var complexType = new EdmComplexType("NS", "Address");
            model.AddElement(complexType);
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter(filter);
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var resourceValue = new ODataResourceValue
                {
                    TypeName = "NS.Address",
                    InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Custom.Bool", new ODataPrimitiveValue(true)),
                        new ODataInstanceAnnotation("Custom.Int", new ODataPrimitiveValue(123)),
                        new ODataInstanceAnnotation("My.String", new ODataPrimitiveValue("annotation")),
                        new ODataInstanceAnnotation("My.Bool", new ODataPrimitiveValue(false))
                    }
                };

                var complexTypeRef = new EdmComplexTypeReference(complexType, false);
                serializer.WriteResourceValue(resourceValue, complexTypeRef, false, false, serializer.CreateDuplicatePropertyNameChecker());
            });

            Assert.Equal(expect, result);
        }

        private ODataJsonLightValueSerializer CreateODataJsonLightValueSerializer(bool writingResponse, IServiceProvider container = null)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = stream,
                MediaType = new ODataMediaType("application", "json"),
#if NETCOREAPP1_0
                Encoding = Encoding.GetEncoding(0),
#else
                Encoding = Encoding.Default,
#endif
                IsResponse = writingResponse,
                IsAsync = false,
                Model = model,
                Container = container
            };
            var context = new ODataJsonLightOutputContext(messageInfo, settings);
            var serializer = new ODataJsonLightValueSerializer(context);
            return serializer;
        }

        /// <summary>
        /// Sets up a ODataJsonLightSerializer, runs the given test code, and then flushes and reads the stream back as a string for
        /// customized verification.
        /// </summary>
        private string SetupSerializerAndRunTest(Action<ODataJsonLightValueSerializer> action, IServiceProvider container = null)
        {
            var serializer = CreateODataJsonLightValueSerializer(true, container);
            action(serializer);
            serializer.JsonWriter.Flush();
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}
