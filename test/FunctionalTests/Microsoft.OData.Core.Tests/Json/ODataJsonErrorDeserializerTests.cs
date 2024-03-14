//---------------------------------------------------------------------
// <copyright file="ODataJsonErrorDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonErrorDeserializerTests
    {
        [Fact]
        public void ReadTopLevelError_Works()
        {
            // Arrange
            const string payload =
                @"{""error"":{""code"":"""",""message"":"""",""target"":""any target"","
                + @"""details"":[{""code"":""500"",""target"":""another target"",""message"":""any msg""}]}}";
            var context = CreateJsonInputContext(payload);
            var deserializer = new ODataJsonErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();

            // Assert
            Assert.Equal("any target", error.Target);
            Assert.Equal(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.Equal("500", detail.Code);
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
            var context = CreateJsonInputContext(payload, GetEdmModel());
            var deserializer = new ODataJsonErrorDeserializer(context);

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
            var context = CreateJsonInputContext(payload, GetEdmModel());
            var deserializer = new ODataJsonErrorDeserializer(context);

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

            var context = CreateJsonInputContext(payload, GetEdmModel());
            var deserializer = new ODataJsonErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();

            // Assert Top Level properties
            Assert.Equal("any target", error.Target);
            Assert.Equal(1, error.Details.Count);
            var detail = error.Details.Single();
            Assert.Equal("500", detail.Code);
            Assert.Equal("any target", detail.Target);
            Assert.Equal("any msg", detail.Message);

            //Assert Inner Error properties
            Assert.NotNull(error.InnerError);
            var nestedInnerError = error.InnerError.InnerError;
            Assert.NotNull(nestedInnerError);
            ODataValue myNewObjectValue;
            Assert.True(nestedInnerError.Properties.TryGetValue("MyNewObject", out myNewObjectValue));
            Assert.NotNull(myNewObjectValue);

            ODataResourceValue nestedMyNewObject = Assert.IsType<ODataResourceValue>(myNewObjectValue);

            Assert.Equal(5, nestedMyNewObject.Properties.Count());
            var properties = nestedMyNewObject.Properties.OfType<ODataProperty>();
            Assert.Equal("StringProperty", properties.ElementAt(0).Name);
            Assert.Equal("newProperty", properties.ElementAt(0).Value);

            Assert.Equal("IntProperty", properties.ElementAt(1).Name);
            Assert.Equal(1, properties.ElementAt(1).Value);

            Assert.Equal("BooleanProperty", properties.ElementAt(2).Name);
            Assert.Equal(true, properties.ElementAt(2).Value);

            Assert.Equal("type", properties.ElementAt(3).Name);
            Assert.Equal("", properties.ElementAt(3).Value);

            Assert.Equal("NestedNull", properties.ElementAt(4).Name);

            ODataResourceValue innerValue = Assert.IsType<ODataResourceValue>(properties.ElementAt(4).Value);

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

            var context = CreateJsonInputContext(payload, GetEdmModel());
            var deserializer = new ODataJsonErrorDeserializer(context);

            // Act
            var error = deserializer.ReadTopLevelError();

            //Assert Inner Error properties
            Assert.NotNull(error.InnerError);
            Assert.True(error.InnerError.Properties.ContainsKey("CollectionValue"));
            Assert.Equal(3, (error.InnerError.Properties["CollectionValue"] as ODataCollectionValue).Items.Count());
        }

        [Theory]
        [InlineData(JsonConstants.ODataErrorInnerErrorInnerErrorName)]
        [InlineData(JsonConstants.ODataErrorInnerErrorName)]
        public async Task ReadTopLevelErrorAsync(string nestedInnerErrorName)
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
                    $"\"{nestedInnerErrorName}\":{{}}}}," +
                "\"ns.workloadId@odata.type\":\"#Guid\"," +
                "\"@ns.workloadId\":\"5a3c4b92-f401-416f-bf88-106cb03efaf4\"}}";

            await SetupJsonErrorDeserializerAndRunTestAsync(
                payload,
                async (jsonErrorDeserializer) =>
                {
                    var error = await jsonErrorDeserializer.ReadTopLevelErrorAsync();

                    Assert.NotNull(error);
                    Assert.Equal("forbidden", error.Code);
                    Assert.Equal("Access to the resource is forbidden", error.Message);
                    Assert.Equal("Resource", error.Target);
                    Assert.NotNull(error.InnerError);
                    Assert.True(error.InnerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorMessageName, out ODataValue innerErrorMessage));
                    Assert.Equal("Contact administrator", Assert.IsType<ODataPrimitiveValue>(innerErrorMessage).Value);
                    Assert.True(error.InnerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorTypeNameName, out ODataValue innerErrorTypeName));
                    Assert.Equal("", Assert.IsType<ODataPrimitiveValue>(innerErrorTypeName).Value);
                    Assert.True(error.InnerError.Properties.TryGetValue(JsonConstants.ODataErrorInnerErrorStackTraceName, out ODataValue innerErrorStackTrace));
                    Assert.Equal("", Assert.IsType<ODataPrimitiveValue>(innerErrorStackTrace).Value);
                    Assert.NotNull(error.InnerError.InnerError);
                    Assert.True(error.InnerError.Properties.TryGetValue("correlationId", out ODataValue correlationIdValue));
                    var correlationId = Assert.IsType<ODataPrimitiveValue>(correlationIdValue);
                    Assert.Equal("4784efae-d1c4-4f1f-baba-e811b3b0826c", correlationId.Value);
                    Assert.NotNull(error.Details);
                    var errorDetail = Assert.Single(error.Details);
                    Assert.Equal("insufficientPrivileges", errorDetail.Code);
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

            await SetupJsonErrorDeserializerAndRunTestAsync(
                payload,
                async (jsonErrorDeserializer) =>
                {
                    var error = await jsonErrorDeserializer.ReadTopLevelErrorAsync();

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

            await SetupJsonErrorDeserializerAndRunTestAsync(
                payload,
                async (jsonErrorDeserializer) =>
                {
                    var error = await jsonErrorDeserializer.ReadTopLevelErrorAsync();

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
                () => SetupJsonErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonErrorDeserializer) => jsonErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName("error"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForUnexpectedTopLevelProperty()
        {
            var payload = "{\"UnexpectedProp\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonErrorDeserializer) => jsonErrorDeserializer.ReadTopLevelErrorAsync()));

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
                () => SetupJsonErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonErrorDeserializer) => jsonErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload("odata.type"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForPropertyWithoutValueInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"target@odata.type\":\"#Edm.String\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonErrorDeserializer) => jsonErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonErrorDeserializer_PropertyAnnotationWithoutPropertyForError("target"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForUnexpectedMetadataReferencePropertyInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"#NS.ResolveError\":{\"title\":\"ResolveError\",\"target\":\"http://tempuri.org/ResolveError\"}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonErrorDeserializer) => jsonErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.ResolveError"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForUnexpectedODataPropertyAnnotationInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"message@odata.etag\":\"etag\"," +
                "\"message\":\"Access to the resource is forbidden\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonErrorDeserializer) => jsonErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload("odata.etag"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForInvalidODataTypeInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"message@odata.type\":null," +
                "\"message\":\"Access to the resource is forbidden\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonErrorDeserializer) => jsonErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTypeName("odata.type"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelErrorAsync_ThrowsExceptionForUnexpectedPropertyInErrorObject()
        {
            var payload = "{\"error\":{" +
                "\"UnexpectedProp\":\"foobar\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonErrorDeserializerAndRunTestAsync(
                    payload,
                    (jsonErrorDeserializer) => jsonErrorDeserializer.ReadTopLevelErrorAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty("UnexpectedProp"),
                exception.Message);
        }

        private async Task SetupJsonErrorDeserializerAndRunTestAsync(
            string payload,
            Func<ODataJsonErrorDeserializer, Task> func,
            IEdmModel model = null)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, model, isAsync: true))
            {
                var jsonErrorDeserializer = new ODataJsonErrorDeserializer(jsonInputContext);

                await func(jsonErrorDeserializer);
            }
        }

        private ODataJsonInputContext CreateJsonInputContext(string payload, IEdmModel model = null, bool isAsync = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = JsonUtils.JsonStreamingMediaType,
                IsAsync = isAsync,
                Model = model ?? EdmCoreModel.Instance,
            };

            return new ODataJsonInputContext(
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
