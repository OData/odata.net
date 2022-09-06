//---------------------------------------------------------------------
// <copyright file="ODataJsonLightErrorDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightErrorDeserializerTests
    {
        [Fact]
        public void ReadTopLevelError_Works()
        {
            // Arrange
            const string payload =
                @"{""error"":{""code"":"""",""message"":"""",""target"":""any target"","
                + @"""details"":[{""code"":""500"",""target"":""another target"",""message"":""any msg""}]}}";
            var context = CreateJsonLightInputContext(payload);
            var deserializer = new ODataJsonLightErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();

            // Assert
            Assert.Equal("any target", error.Target);
            Assert.Equal(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.Equal("500", detail.ErrorCode);
            Assert.Equal("another target", detail.Target);
            Assert.Equal("any msg", detail.Message);
        }

        [Fact]
        public void ReadTopLevelErrorWithResourceValueInstanceAnnotation_Works()
        {
            // Arrange
            const string payload =
                @"{""error"":{""code"":"""",""message"":"""","
                + @"""@sample.error"":{""@odata.type"":""#NS.Entity"",""Sample"":""sample value""}}}";
            var context = CreateJsonLightInputContext(payload, GetEdmModel());
            var deserializer = new ODataJsonLightErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();

            // Assert
            Assert.NotNull(error.InstanceAnnotations);
            var annotation = Assert.Single(error.InstanceAnnotations);
            Assert.Equal("sample.error", annotation.Name);
            Assert.NotNull(annotation.Value);
            var value = Assert.IsType<ODataResourceValue>(annotation.Value);
            Assert.Equal("NS.Entity", value.TypeName);
            Assert.NotNull(value.Properties);
            var property = Assert.Single(value.Properties);
            Assert.Equal("Sample", property.Name);
            Assert.Equal("sample value", property.Value);
        }

        [Fact]
        public void ReadTopLevelErrorWithCollectionResourceValueInstanceAnnotation_Works()
        {
            // Arrange
            const string payload =
                @"{""error"":{""code"":"""",""message"":"""","
                + @"""sample.error@odata.type"":""Collection(NS.Entity)"",""@sample.error"":[{""@odata.type"":""#NS.Entity"",""Sample"":""sample value""}]}}";
            var context = CreateJsonLightInputContext(payload, GetEdmModel());
            var deserializer = new ODataJsonLightErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();

            // Assert
            Assert.NotNull(error.InstanceAnnotations);
            var annotation = Assert.Single(error.InstanceAnnotations);
            Assert.Equal("sample.error", annotation.Name);
            Assert.NotNull(annotation.Value);
            var collection = Assert.IsType<ODataCollectionValue>(annotation.Value);
            Assert.Equal("Collection(NS.Entity)", collection.TypeName);
            Assert.NotNull(collection.Items);
            var item = Assert.Single(collection.Items);
            var value = Assert.IsType<ODataResourceValue>(item);
            Assert.Equal("NS.Entity", value.TypeName);
            Assert.NotNull(value.Properties);
            var property = Assert.Single(value.Properties);
            Assert.Equal("Sample", property.Name);
            Assert.Equal("sample value", property.Value);
        }

        [Fact]
        public void ReadExtendedInnerError()
        {
            // Arrange
            const string payload =
                    "{\"error\":{" +
                        "\"code\":\"\",\"message\":\"\"," +
                        "\"target\":\"any target\"," +
                        "\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}]," +
                        "\"innererror\":{" +
                            "\"stacktrace\":\"NormalString\"," +
                            "\"message\":\"\"," +
                            "\"type\":\"\"," +
                            "\"innererror\":{" +
                                    "\"stacktrace\":\"InnerError\"," +
                                    "\"MyNewObject\":{" +
                                        "\"StringProperty\":\"newProperty\"," +
                                        "\"IntProperty\": 1," +
                                        "\"BooleanProperty\": true," +
                                        "\"type\":\"\"," +
                                        "\"NestedNull\":{" +
                                           "\"InnerMostPropertyName\":\"InnerMostPropertyValue\"," +
                                           "\"InnerMostNull\":null" +
                                         "}" +
                                     "}" +
                                 "}" +
                            "}" +
                       "}}";

            var context = CreateJsonLightInputContext(payload, GetEdmModel());
            var deserializer = new ODataJsonLightErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();

            // Assert Top Level properties
            Assert.Equal("any target", error.Target);
            Assert.Equal(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.Equal("500", detail.ErrorCode);
            Assert.Equal("any target", detail.Target);
            Assert.Equal("any msg", detail.Message);

            //Assert Inner Error properties
            Assert.NotNull(error.InnerError);
            Assert.True(error.InnerError.Properties.ContainsKey("innererror"));
            var objectValue =
                (error.InnerError.Properties["innererror"] as ODataResourceValue).Properties
                .FirstOrDefault(p => p.Name == "MyNewObject").ODataValue as ODataResourceValue;
            Assert.NotNull(objectValue);

            ODataResourceValue nestedInnerObject = error.InnerError.Properties["innererror"] as ODataResourceValue;
            ODataResourceValue nestedMyNewObject = nestedInnerObject.Properties.FirstOrDefault(p => p.Name == "MyNewObject").ODataValue as ODataResourceValue;

            Assert.Equal(5, nestedMyNewObject.Properties.Count());
            Assert.Equal("StringProperty", nestedMyNewObject.Properties.ElementAt(0).Name);
            Assert.Equal("newProperty", nestedMyNewObject.Properties.ElementAt(0).Value);

            Assert.Equal("IntProperty", nestedMyNewObject.Properties.ElementAt(1).Name);
            Assert.Equal(1, nestedMyNewObject.Properties.ElementAt(1).Value);

            Assert.Equal("BooleanProperty", nestedMyNewObject.Properties.ElementAt(2).Name);
            Assert.Equal(true, nestedMyNewObject.Properties.ElementAt(2).Value);

            Assert.Equal("type", nestedMyNewObject.Properties.ElementAt(3).Name);
            Assert.Equal("", nestedMyNewObject.Properties.ElementAt(3).Value);

            Assert.Equal("NestedNull", nestedMyNewObject.Properties.ElementAt(4).Name);

            ODataResourceValue innerValue = nestedMyNewObject.Properties.ElementAt(4).Value as ODataResourceValue;

            Assert.NotNull(innerValue);

            Assert.Equal(2, innerValue.Properties.Count());
            Assert.Equal("InnerMostPropertyName", innerValue.Properties.ElementAt(0).Name);
            Assert.Equal("InnerMostPropertyValue", innerValue.Properties.ElementAt(0).Value);

            Assert.Equal("InnerMostNull", innerValue.Properties.ElementAt(1).Name);
            Assert.Null(innerValue.Properties.ElementAt(1).Value);
        }

        [Fact]
        public void ReadErrorWithCollectionInInnerError()
        {
            // Arrange
            const string payload = "{\"error\":{\"code\":\"\",\"message\":\"\",\"target\":\"any target\",\"details\":[{\"code\":\"500\",\"target\":\"any target\",\"message\":\"any msg\"}],\"innererror\":{\"message\":\"\",\"type\":\"\",\"stacktrace\":\"\",\"ResourceValue\":{\"PropertyName\":\"PropertyValue\",\"NullProperty\":null},\"NullProperty\":null,\"CollectionValue\":[null,\"CollectionValue\",1]}}}";

            var context = CreateJsonLightInputContext(payload, GetEdmModel());
            var deserializer = new ODataJsonLightErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();

            //Assert Inner Error properties
            Assert.NotNull(error.InnerError);
            Assert.True(error.InnerError.Properties.ContainsKey("CollectionValue"));
            Assert.Equal(3, (error.InnerError.Properties["CollectionValue"] as ODataCollectionValue).Items.Count());
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync()
        {
            var payload = "{\"error\":{" +
                "\"code\":\"forbidden\"," +
                "\"message\":\"Access to the resource is forbidden\"," +
                "\"target\":\"Resource\"," +
                "\"details\":[{\"code\":\"insufficientPrivileges\",\"message\":\"You don't have the required privileges\",\"target\":\"\",\"ignore\":true}]," +
                "\"innererror\":{" +
                    "\"message\":\"Contact administrator\"," +
                    "\"type\":\"\"," +
                    "\"stacktrace\":\"\"," +
                    "\"correlationId\":\"4784efae-d1c4-4f1f-baba-e811b3b0826c\"," +
                    "\"internalexception\":{}}," +
                "\"ns.workloadId@odata.type\":\"#Guid\"," +
                "\"@ns.workloadId\":\"5a3c4b92-f401-416f-bf88-106cb03efaf4\"}}";

            await SetupJsonLightErrorDeserializerAndRunTestAsync(
                payload,
                async (jsonLightErrorDeserializer) =>
                {
                    var error = await jsonLightErrorDeserializer.ReadTopLevelErrorAsync();

                    Assert.NotNull(error);
                    Assert.Equal("forbidden", error.ErrorCode);
                    Assert.Equal("Access to the resource is forbidden", error.Message);
                    Assert.Equal("Resource", error.Target);
                    Assert.NotNull(error.InnerError);
                    Assert.Equal("Contact administrator", error.InnerError.Message);
                    Assert.Equal("", error.InnerError.TypeName);
                    Assert.Equal("", error.InnerError.StackTrace);
                    Assert.NotNull(error.InnerError.InnerError);
                    Assert.True(error.InnerError.Properties.TryGetValue("correlationId", out ODataValue correlationIdValue));
                    var correlationId = Assert.IsType<ODataPrimitiveValue>(correlationIdValue);
                    Assert.Equal("4784efae-d1c4-4f1f-baba-e811b3b0826c", correlationId.Value);
                    Assert.NotNull(error.Details);
                    var errorDetail = Assert.Single(error.Details);
                    Assert.Equal("insufficientPrivileges", errorDetail.ErrorCode);
                    Assert.Equal("You don't have the required privileges", errorDetail.Message);
                    Assert.Equal("", errorDetail.Target);
                    var nsWorkloadIdAnnotation = Assert.Single(error.GetInstanceAnnotations());
                    var nsWorkloadId = Assert.IsType<ODataPrimitiveValue>(nsWorkloadIdAnnotation.Value);
                    Assert.Equal(Guid.Parse("5a3c4b92-f401-416f-bf88-106cb03efaf4"), nsWorkloadId.Value);
                });
        }

        [Fact]
        public async Task ReadTopLevelErrorWithResourceValueInstanceAnnotationAsync()
        {
            var model = new EdmModel();
            var errorTokenComplexType = new EdmComplexType("NS", "ErrorToken");
            errorTokenComplexType.AddStructuralProperty("Severity", EdmPrimitiveTypeKind.String);
            model.AddElement(errorTokenComplexType);

            var payload = "{\"error\":{\"@custom.annotation\":{\"@odata.type\":\"#NS.ErrorToken\",\"Severity\":\"Critical\"}}}";

            await SetupJsonLightErrorDeserializerAndRunTestAsync(
                payload,
                async (jsonLightErrorDeserializer) =>
                {
                    var error = await jsonLightErrorDeserializer.ReadTopLevelErrorAsync();

                    Assert.NotNull(error);
                    var errorTokenInstanceAnnotation = Assert.Single(error.GetInstanceAnnotations());
                    var errorTokenResourceValue = Assert.IsType<ODataResourceValue>(errorTokenInstanceAnnotation.Value);
                    var severityProperty = Assert.Single(errorTokenResourceValue.Properties);
                    Assert.Equal("Severity", severityProperty.Name);
                    Assert.Equal("Critical", severityProperty.Value);
                },
                model);
        }

        [Fact]
        public async Task ReadTopLevelErrorWithResourceValueInInnerErrorObjectAsync()
        {
            var payload = "{\"error\":{\"innererror\":{\"errorToken\":{\"Severity\":\"Critical\"}}}}";

            await SetupJsonLightErrorDeserializerAndRunTestAsync(
                payload,
                async (jsonLightErrorDeserializer) =>
                {
                    var error = await jsonLightErrorDeserializer.ReadTopLevelErrorAsync();

                    Assert.NotNull(error);
                    Assert.NotNull(error.InnerError);
                    Assert.True(error.InnerError.Properties.TryGetValue("errorToken", out ODataValue resourceValue));
                    var errorToken = Assert.IsType<ODataResourceValue>(resourceValue);
                    var severityProperty = Assert.Single(errorToken.Properties);
                    Assert.Equal("Severity", severityProperty.Name);
                    Assert.Equal("Critical", severityProperty.Value);
                });
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForMultipleErrorProperties()
        {
            var payload = "{\"error\":{" +
                "\"code\":\"forbidden\"," +
                "\"message\":\"Access to the resource is forbidden\"," +
                "\"target\":\"Resource\"}," +
                "\"error\":{}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightErrorDeserializer) => jsonLightErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName("error"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForUnexpectedTopLevelProperty()
        {
            var payload = "{\"UnexpectedProp\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightErrorDeserializer) => jsonLightErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty("UnexpectedProp"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForODataAnnotationInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"@odata.type\":\"#NS.ODataError\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightErrorDeserializer) => jsonLightErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload("odata.type"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForPropertyWithoutValueInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"target@odata.type\":\"#Edm.String\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightErrorDeserializer) => jsonLightErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError("target"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForUnexpectedMetadataReferencePropertyInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"#NS.ResolveError\":{\"title\":\"ResolveError\",\"target\":\"http://tempuri.org/ResolveError\"}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightErrorDeserializer) => jsonLightErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.ResolveError"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForUnexpectedODataPropertyAnnotationInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"message@odata.etag\":\"etag\"," +
                "\"message\":\"Access to the resource is forbidden\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightErrorDeserializer) => jsonLightErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload("odata.etag"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForInvalidODataTypeInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"message@odata.type\":null," +
                "\"message\":\"Access to the resource is forbidden\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightErrorDeserializer) => jsonLightErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName("odata.type"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForUnexpectedPropertyInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"UnexpectedProp\":\"foobar\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightErrorDeserializer) => jsonLightErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty("UnexpectedProp"),
                exception.Message);
        }

        private async Task SetupJsonLightErrorDeserializerAndRunTestAsync(
            string payload,
            Func<ODataJsonLightErrorDeserializer, Task> func,
            IEdmModel model = null)
        {
            using (var jsonLightInputContext = CreateJsonLightInputContext(payload, model, isAsync: true))
            {
                var jsonLightErrorDeserializer = new ODataJsonLightErrorDeserializer(jsonLightInputContext);

                await func(jsonLightErrorDeserializer);
            }
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, IEdmModel model = null, bool isAsync = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = JsonLightUtils.JsonLightStreamingMediaType,
                IsAsync = isAsync,
                Model = model ?? EdmCoreModel.Instance,
            };

            return new ODataJsonLightInputContext(
                new StringReader(payload),
                messageInfo,
                new ODataMessageReaderSettings());
        }

        private static IEdmModel GetEdmModel()
        {
            EdmModel model = new EdmModel();
            var complexType = new EdmComplexType("NS", "Entity");
            var property = new EdmStructuralProperty(complexType, "Sample", EdmCoreModel.Instance.GetString(false));
            complexType.AddProperty(property);
            model.AddElement(complexType);
            return model;
        }
    }
}
