//---------------------------------------------------------------------
// <copyright file="JsonInstanceAnnotationWriterAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for JsonInstanceAnnotationWriter asynchronous API.
    /// </summary>
    public class JsonInstanceAnnotationWriterAsyncTests
    {
        private EdmModel model;
        private Stream stream;
        private ODataMessageWriterSettings settings;
        private ODataJsonValueSerializer jsonValueSerializer;
        private EdmEnumType dayEnumType;
        private EdmComplexType mealComplexType;

        public JsonInstanceAnnotationWriterAsyncTests()
        {
            this.model = new EdmModel();

            this.dayEnumType = new EdmEnumType("NS", "Day");
            dayEnumType.AddMember(new EdmEnumMember(dayEnumType, "Monday", new EdmEnumMemberValue(0)));
            dayEnumType.AddMember(new EdmEnumMember(dayEnumType, "Tuesday", new EdmEnumMemberValue(1)));
            dayEnumType.AddMember(new EdmEnumMember(dayEnumType, "Wednesday", new EdmEnumMemberValue(2)));
            dayEnumType.AddMember(new EdmEnumMember(dayEnumType, "Thursday", new EdmEnumMemberValue(3)));
            dayEnumType.AddMember(new EdmEnumMember(dayEnumType, "Friday", new EdmEnumMemberValue(4)));
            dayEnumType.AddMember(new EdmEnumMember(dayEnumType, "Saturday", new EdmEnumMemberValue(5)));
            dayEnumType.AddMember(new EdmEnumMember(dayEnumType, "Sunday", new EdmEnumMemberValue(6)));
            this.model.AddElement(this.dayEnumType);

            mealComplexType = new EdmComplexType("NS", "Meal");
            mealComplexType.AddStructuralProperty("Starter", EdmPrimitiveTypeKind.String);
            mealComplexType.AddStructuralProperty("MainCourse", EdmPrimitiveTypeKind.String);
            mealComplexType.AddStructuralProperty("Dessert", EdmPrimitiveTypeKind.String);
            this.model.AddElement(this.mealComplexType);

            this.stream = new AsyncStream(new MemoryStream());
            this.settings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
                Version = ODataVersion.V4
            };
            this.settings.SetServiceDocumentUri(new Uri("http://tempuri.org"));
        }

        [Fact]
        public async Task WriteInstanceAnnotationsForErrorAsync_WritesInstanceAnnotations()
        {
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Error.Level", new ODataPrimitiveValue("Warning")),
                new ODataInstanceAnnotation("Error.Severity", new ODataPrimitiveValue("Critical"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonInstanceAnnotationWriter) =>
                {
                    return jsonInstanceAnnotationWriter.WriteInstanceAnnotationsForErrorAsync(instanceAnnotations);
                });

            Assert.Equal("{\"@Error.Level\":\"Warning\",\"@Error.Severity\":\"Critical\"", result);
        }

        public static IEnumerable<object[]> GetWriteInstanceAnnotationTestData()
        {
            // ODataNullValue
            yield return new object[]
            {
                new ODataInstanceAnnotation(
                    "favorite.City",
                    new ODataNullValue()),
                "{\"FunFacts@favorite.City\":null"
            };

            // ODataResourceValue
            yield return new object[]
            {
                new ODataInstanceAnnotation(
                    "favorite.Meal",
                    new ODataResourceValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Starter", Value = "Broccoli soup" },
                            new ODataProperty { Name = "MainCourse", Value = "Lamb chops" },
                            new ODataProperty { Name = "Dessert", Value = "Carrot cake" }
                        },
                        TypeName = "NS.Meal"
                    }),
                "{\"FunFacts@favorite.Meal\":{\"@odata.type\":\"#NS.Meal\",\"Starter\":\"Broccoli soup\",\"MainCourse\":\"Lamb chops\",\"Dessert\":\"Carrot cake\"}"
            };

            // ODataCollectionValue
            yield return new object[]
            {
                new ODataInstanceAnnotation(
                    "favorite.Days",
                    new ODataCollectionValue
                    {
                        Items = new List<ODataEnumValue>
                        {
                            new ODataEnumValue("Monday"),
                            new ODataEnumValue("Sunday")
                        },
                        TypeName = "Collection(NS.Day)"
                    }),
                "{\"favorite.Days@odata.type\":\"#Collection(NS.Day)\",\"FunFacts@favorite.Days\":[\"Monday\",\"Sunday\"]"
            };

            // ODataUntypedValue
            yield return new object[]
            {
                new ODataInstanceAnnotation(
                    "favorite.Book",
                    new ODataUntypedValue { RawValue = "A Tail in the Mouth" }),
                "{\"FunFacts@favorite.Book\":A Tail in the Mouth"
            };

            // ODataEnumValue
            yield return new object[]
            {
                new ODataInstanceAnnotation(
                    "favorite.Day",
                    new ODataEnumValue("Monday", "NS.Day")),
                "{\"FunFacts@favorite.Day\":\"Monday\""
            };

            // ODataPrimitiveValue
            yield return new object[]
            {
                new ODataInstanceAnnotation(
                    "favorite.Song",
                    new ODataPrimitiveValue("Short Change Hero")),
                "{\"FunFacts@favorite.Song\":\"Short Change Hero\""
            };

            // ODataPrimitiveValue - written with type name
            yield return new object[]
            {
                new ODataInstanceAnnotation(
                    "favorite.Number",
                    new ODataPrimitiveValue(13L)),
                "{\"favorite.Number@odata.type\":\"#Int64\",\"FunFacts@favorite.Number\":13"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteInstanceAnnotationTestData))]
        public async Task WriteInstanceAnnotationAsync_WritesInstanceAnnotation(ODataInstanceAnnotation instanceAnnotation, string expected)
        {
            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonInstanceAnnotationWriter) =>
                {
                    return jsonInstanceAnnotationWriter.WriteInstanceAnnotationAsync(instanceAnnotation, /* ignoreFilter */ true, "FunFacts");
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationAsync_ForIgnoreFilterEqualsFalse()
        {
            var instanceAnnotation = new ODataInstanceAnnotation(
                    "favorite.Person",
                    new ODataNullValue());

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonInstanceAnnotationWriter) =>
                {
                    return jsonInstanceAnnotationWriter.WriteInstanceAnnotationAsync(instanceAnnotation, /* ignoreFilter */ false, "FunFacts");
                });

            Assert.Equal("{" /* Object scope left paren */, result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_WritesInstanceAnnotations()
        {
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Day", new ODataEnumValue("Monday", "NS.Day"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonInstanceAnnotationWriter) =>
                {
                    return jsonInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        new InstanceAnnotationWriteTracker(), 
                        /* ignoreFilter */ true,
                        "FunFacts");
                });

            Assert.Equal("{\"FunFacts@favorite.Coffee\":\"Cappuccino\",\"FunFacts@favorite.Day\":\"Monday\"", result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_ExceptionThrownForDuplicateInstanceAnnotations()
        {
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
            };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                    (jsonInstanceAnnotationWriter) =>
                    {
                        return jsonInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                            instanceAnnotations,
                            new InstanceAnnotationWriteTracker(),
                            /* ignoreFilter */ true,
                            "FunFacts");
                    }));

            Assert.Equal(
                ODataErrorStrings.JsonInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("favorite.Coffee"),
                exception.Message);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_IgnoresInstanceAnnotationsWithKnownODataAnnotationNames()
        {
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("odata.null", new ODataPrimitiveValue("NULL"), /* isCustomAnnotation */ true),
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonInstanceAnnotationWriter) =>
                {
                    return jsonInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        new InstanceAnnotationWriteTracker(),
                        /* ignoreFilter */ true,
                        "FunFacts");
                });

            Assert.Equal("{" /* Object scope left paren */, result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_ForDeclaredProperty()
        {
            this.settings.ShouldIncludeAnnotationInternal = (annotation) => true; // Allow annotations to be written
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Day", new ODataEnumValue("Monday", "NS.Day"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonInstanceAnnotationWriter) =>
                {
                    return jsonInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        "FunFacts",
                        /* isUndeclaredProperty */ false);
                });

            Assert.Equal("{\"FunFacts@favorite.Coffee\":\"Cappuccino\",\"FunFacts@favorite.Day\":\"Monday\"", result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_ForUndeclaredProperty()
        {
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Day", new ODataEnumValue("Monday", "NS.Day"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonInstanceAnnotationWriter) =>
                {
                    return jsonInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        "FunFacts",
                        /* isUndeclaredProperty */ true);
                });

            Assert.Equal("{\"FunFacts@favorite.Coffee\":\"Cappuccino\",\"FunFacts@favorite.Day\":\"Monday\"", result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_ForUndeclaredProperty_WriteAnnotationsEvenIfShouldIncludeAnnotationReturnsFalse()
        {
            this.settings.ShouldIncludeAnnotation = name => false;
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Day", new ODataEnumValue("Monday", "NS.Day"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonInstanceAnnotationWriter) =>
                {
                    return jsonInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        propertyName: "FunFacts",
                        isUndeclaredProperty: true);
                });

            Assert.Equal("{\"FunFacts@favorite.Coffee\":\"Cappuccino\",\"FunFacts@favorite.Day\":\"Monday\"", result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_ShouldWriteAnnotationIfShouldIncludeAnnotationReturnsTrue()
        {
            this.settings.ShouldIncludeAnnotation = (name) => true;
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Day", new ODataEnumValue("Monday", "NS.Day"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonLightInstanceAnnotationWriter) =>
                {
                    return jsonLightInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        "FunFacts",
                        /* isUndeclaredProperty */ false);
                });

            Assert.Equal("{\"FunFacts@favorite.Coffee\":\"Cappuccino\",\"FunFacts@favorite.Day\":\"Monday\"", result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_ShouldWriteAnnotationThatDoesNotPassTheAnnotationFilterIfShouldIncludeAnnotationReturnsTrue()
        {
            this.settings.ShouldIncludeAnnotationInternal = (name) => false;
            this.settings.ShouldIncludeAnnotation = (name) => name == "favorite.Day";
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Day", new ODataEnumValue("Monday", "NS.Day"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonLightInstanceAnnotationWriter) =>
                {
                    return jsonLightInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        "FunFacts");
                });

            Assert.Equal("{\"FunFacts@favorite.Day\":\"Monday\"", result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_ShouldSkipAnnotationThatDoesNotPassTheAnnotationFilterIfShouldIncludeAnnotationReturnsFalse()
        {
            this.settings.ShouldIncludeAnnotationInternal = (name) => false;
            this.settings.ShouldIncludeAnnotation = (name) => false;
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Day", new ODataEnumValue("Monday", "NS.Day"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonLightInstanceAnnotationWriter) =>
                {
                    return jsonLightInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        "FunFacts");
                });

            Assert.Equal("{", result);
        }

        [Fact]
        public async Task WriteInstanceAnnotationsAsync_ShouldWriteAnnotationThatPassesTheAnnotationFilterIfShouldIncludeAnnotationReturnsFalse()
        {
            this.settings.ShouldIncludeAnnotationInternal = (name) => name == "favorite.Day";
            this.settings.ShouldIncludeAnnotation = (name) => false;
            var instanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("favorite.Coffee", new ODataPrimitiveValue("Cappuccino")),
                new ODataInstanceAnnotation("favorite.Day", new ODataEnumValue("Monday", "NS.Day"))
            };

            var result = await SetupJsonInstanceAnnotationWriterAndRunTestAsync(
                (jsonLightInstanceAnnotationWriter) =>
                {
                    return jsonLightInstanceAnnotationWriter.WriteInstanceAnnotationsAsync(
                        instanceAnnotations,
                        "FunFacts");
                });

            Assert.Equal("{\"FunFacts@favorite.Day\":\"Monday\"", result);
        }

        private JsonInstanceAnnotationWriter CreateJsonInstanceAnnotationWriter(bool writingResponse, IServiceProvider serviceProvider = null, bool isAsync = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.GetEncoding(0),
                IsResponse = writingResponse,
                IsAsync = isAsync,
                Model = model,
                ServiceProvider = serviceProvider
            };
            var context = new ODataJsonOutputContext(messageInfo, this.settings);
            this.jsonValueSerializer = new ODataJsonValueSerializer(context);
            return new JsonInstanceAnnotationWriter(this.jsonValueSerializer, new JsonMinimalMetadataTypeNameOracle());
        }

        /// <summary>
        /// Sets up an ODataJsonValueSerializer,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonInstanceAnnotationWriterAndRunTestAsync(Func<JsonInstanceAnnotationWriter, Task> func, IServiceProvider container = null)
        {
            var jsonInstanceAnnotationWriter = CreateJsonInstanceAnnotationWriter(true, container, true);
            await this.jsonValueSerializer.JsonWriter.StartObjectScopeAsync();
            await func(jsonInstanceAnnotationWriter);
            await this.jsonValueSerializer.JsonOutputContext.FlushAsync();
            await this.jsonValueSerializer.JsonWriter.FlushAsync();

            this.stream.Position = 0;

            return await new StreamReader(this.stream).ReadToEndAsync();
        }
    }
}
