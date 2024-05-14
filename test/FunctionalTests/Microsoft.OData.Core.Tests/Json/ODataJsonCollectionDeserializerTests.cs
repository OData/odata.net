//---------------------------------------------------------------------
// <copyright file="ODataJsonCollectionDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
    /// Unit tests for ODataJsonCollectionDeserializer.
    /// </summary>
    public class ODataJsonCollectionDeserializerTests
    {
        [Theory]
        [InlineData("")] // No annotation
        [InlineData("\"@odata.count\":3,")]
        [InlineData("\"@odata.nextLink\":\"http://tempuri.org/nextLink\",")]
        [InlineData("\"@custom.annotation\":\"foobar\",")]
        [InlineData("\"value@odata.type\":\"#Collection(Edm.String)\",")]
        public void ReadCollection(string annotationPart)
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                $"{annotationPart}" +
                "\"value\":[\"Tea\",\"Coffee\"]}";

            SetupJsonCollectionDeserializerAndRunTest(
                payload,
                (JsonCollectionDeserializer) =>
                {
                    JsonCollectionDeserializer.ReadPayloadStart(
                        ODataPayloadKind.Collection,
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload: false,
                        allowEmptyPayload: false);

                    var collectionStart = JsonCollectionDeserializer.ReadCollectionStart(
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload: false,
                        expectedItemTypeReference: expectedItemTypeReference,
                        out IEdmTypeReference actualItemTypeReference);
                    JsonCollectionDeserializer.JsonReader.Read(); // Read StartArray [
                    var collectionItem1 = JsonCollectionDeserializer.ReadCollectionItem(
                        expectedItemTypeReference,
                        collectionValidator: null);
                    var collectionItem2 = JsonCollectionDeserializer.ReadCollectionItem(
                        expectedItemTypeReference,
                        collectionValidator: null);
                    JsonCollectionDeserializer.ReadCollectionEnd(
                        isReadingNestedPayload: false);

                    Assert.NotNull(collectionStart);
                    Assert.Equal("Tea", collectionItem1);
                    Assert.Equal("Coffee", collectionItem2);
                });
        }

        [Theory]
        [InlineData("\"@odata.count\":3")]
        [InlineData("\"@odata.nextLink\":\"http://tempuri.org/nextLink\"")]
        [InlineData("\"@custom.annotation\":\"foobar\"")]
        public void ReadCollectionWithInstanceAnnotationsAtCollectionEnd(string annotationPart)
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"value\":[\"Tea\",\"Coffee\"]," +
                $"{annotationPart}}}";

            SetupJsonCollectionDeserializerAndRunTest(
                payload,
                (JsonCollectionDeserializer) =>
                {
                    JsonCollectionDeserializer.ReadPayloadStart(
                        ODataPayloadKind.Collection,
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload: false,
                        allowEmptyPayload: false);

                    var collectionStart = JsonCollectionDeserializer.ReadCollectionStart(
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload: false,
                        expectedItemTypeReference: expectedItemTypeReference,
                        out IEdmTypeReference actualItemTypeReference);
                    JsonCollectionDeserializer.JsonReader.Read(); // Read StartArray [
                    var collectionItem1 = JsonCollectionDeserializer.ReadCollectionItem(
                        expectedItemTypeReference,
                        collectionValidator: null);
                    var collectionItem2 = JsonCollectionDeserializer.ReadCollectionItem(
                        expectedItemTypeReference,
                        collectionValidator: null);
                    JsonCollectionDeserializer.ReadCollectionEnd(
                        isReadingNestedPayload: false);

                    Assert.NotNull(collectionStart);
                    Assert.Equal("Tea", collectionItem1);
                    Assert.Equal("Coffee", collectionItem2);
                });
        }

        [Theory]
        [InlineData("")] // No annotation
        [InlineData("\"@odata.count\":3,")]
        [InlineData("\"@odata.nextLink\":\"http://tempuri.org/nextLink\",")]
        [InlineData("\"@custom.annotation\":\"foobar\",")]
        [InlineData("\"value@odata.type\":\"#Collection(Edm.String)\",")]
        public async Task ReadCollectionAsync(string annotationPart)
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                $"{annotationPart}" +
                "\"value\":[\"Tea\",\"Coffee\"]}";

            await SetupJsonCollectionDeserializerAndRunTestAsync(
                payload,
                async (JsonCollectionDeserializer) =>
                {
                    await JsonCollectionDeserializer.ReadPayloadStartAsync(
                        ODataPayloadKind.Collection,
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload: false,
                        allowEmptyPayload: false);

                    var readCollectionStartResult = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload: false,
                        expectedItemTypeReference: expectedItemTypeReference);
                    await JsonCollectionDeserializer.JsonReader.ReadAsync(); // Read StartArray [
                    var collectionItem1 = await JsonCollectionDeserializer.ReadCollectionItemAsync(
                        expectedItemTypeReference,
                        collectionValidator: null);
                    var collectionItem2 = await JsonCollectionDeserializer.ReadCollectionItemAsync(
                        expectedItemTypeReference,
                        collectionValidator: null);
                    await JsonCollectionDeserializer.ReadCollectionEndAsync(
                        isReadingNestedPayload: false);

                    Assert.NotNull(readCollectionStartResult);
                    var collectionStart = Assert.IsType<ODataCollectionStart>(readCollectionStartResult.Item1);
                    var actualItemTypeReference = Assert.IsAssignableFrom<IEdmTypeReference>(readCollectionStartResult.Item2);
                    Assert.NotNull(collectionStart);
                    Assert.NotNull(actualItemTypeReference);
                    Assert.Equal("Tea", collectionItem1);
                    Assert.Equal("Coffee", collectionItem2);
                });
        }

        [Theory]
        [InlineData("\"@odata.count\":3")]
        [InlineData("\"@odata.nextLink\":\"http://tempuri.org/nextLink\"")]
        [InlineData("\"@custom.annotation\":\"foobar\"")]
        public async Task ReadCollectionWithInstanceAnnotationsAtCollectionEndAsync(string annotationPart)
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"value\":[\"Tea\",\"Coffee\"]," +
                $"{annotationPart}}}";

            await SetupJsonCollectionDeserializerAndRunTestAsync(
                payload,
                async (JsonCollectionDeserializer) =>
                {
                    await JsonCollectionDeserializer.ReadPayloadStartAsync(
                        ODataPayloadKind.Collection,
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload: false,
                        allowEmptyPayload: false);

                    var readCollectionStartResult = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                        propertyAndAnnotationCollector,
                        isReadingNestedPayload: false,
                        expectedItemTypeReference: expectedItemTypeReference);
                    await JsonCollectionDeserializer.JsonReader.ReadAsync(); // Read StartArray [
                    var collectionItem1 = await JsonCollectionDeserializer.ReadCollectionItemAsync(
                        expectedItemTypeReference,
                        collectionValidator: null);
                    var collectionItem2 = await JsonCollectionDeserializer.ReadCollectionItemAsync(
                        expectedItemTypeReference,
                        collectionValidator: null);
                    await JsonCollectionDeserializer.ReadCollectionEndAsync(
                        isReadingNestedPayload: false);

                    Assert.NotNull(readCollectionStartResult);
                    Assert.Equal("Tea", collectionItem1);
                    Assert.Equal("Coffee", collectionItem2);
                });
        }

        [Fact]
        public async Task ReadCollectionStartAsync_ThrowsExceptionForInvalidTopLevelPropertyTypeName()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"value@odata.type\":\"#Edm.String\"," +
                "\"value\":[\"Tea\",\"Coffee\"]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonCollectionDeserializerAndRunTestAsync(
                    payload,
                    async (JsonCollectionDeserializer) =>
                    {
                        await JsonCollectionDeserializer.ReadPayloadStartAsync(
                            ODataPayloadKind.Collection,
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            allowEmptyPayload: false);

                        var collectionStart = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            expectedItemTypeReference: expectedItemTypeReference);
                    }));

            Assert.Equal(
                ODataErrorStrings.ODataJsonCollectionDeserializer_InvalidCollectionTypeName("Edm.String"),
                exception.Message);
        }

        [Fact]
        public async Task ReadCollectionStartAsync_ThrowsExceptionForUnexpectedODataAnnotations()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/deltaLink\"," +
                "\"value\":[\"Tea\",\"Coffee\"]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonCollectionDeserializerAndRunTestAsync(
                    payload,
                    async (JsonCollectionDeserializer) =>
                    {
                        await JsonCollectionDeserializer.ReadPayloadStartAsync(
                            ODataPayloadKind.Collection,
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            allowEmptyPayload: false);

                        var collectionStart = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            expectedItemTypeReference: expectedItemTypeReference);
                    }));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.deltaLink"),
                exception.Message);
        }

        [Fact]
        public async Task ReadCollectionStartAsync_ThrowsExceptionForInvalidTopLevelPropertyName()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"UnexpectedProp\":[\"foobar\"]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonCollectionDeserializerAndRunTestAsync(
                    payload,
                    async (JsonCollectionDeserializer) =>
                    {
                        await JsonCollectionDeserializer.ReadPayloadStartAsync(
                            ODataPayloadKind.Collection,
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            allowEmptyPayload: false);

                        var collectionStart = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            expectedItemTypeReference: expectedItemTypeReference);
                    }));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyName("UnexpectedProp", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadCollectionStartAsync_ThrowsExceptionForExpectedCollectionPropertyNotFound()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonCollectionDeserializerAndRunTestAsync(
                    payload,
                    async (JsonCollectionDeserializer) =>
                    {
                        await JsonCollectionDeserializer.ReadPayloadStartAsync(
                            ODataPayloadKind.Collection,
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            allowEmptyPayload: false);

                        var collectionStart = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            expectedItemTypeReference: expectedItemTypeReference);
                    }));

            Assert.Equal(
                ODataErrorStrings.ODataJsonCollectionDeserializer_ExpectedCollectionPropertyNotFound("value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadCollectionStartAsync_ThrowsExceptionForInvalidValueForTopLevelProperty()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"value\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonCollectionDeserializerAndRunTestAsync(
                    payload,
                    async (JsonCollectionDeserializer) =>
                    {
                        await JsonCollectionDeserializer.ReadPayloadStartAsync(
                            ODataPayloadKind.Collection,
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            allowEmptyPayload: false);

                        var collectionStart = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            expectedItemTypeReference: expectedItemTypeReference);
                    }));

            Assert.Equal(
                ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionContentStart("PrimitiveValue"),
                exception.Message);
        }

        [Fact]
        public async Task ReadCollectionStartAsync_ThrowsExceptionForTopLevelPropertyAnnotationWithoutProperty()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"value@custom.annotation\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonCollectionDeserializerAndRunTestAsync(
                    payload,
                    async (JsonCollectionDeserializer) =>
                    {
                        await JsonCollectionDeserializer.ReadPayloadStartAsync(
                            ODataPayloadKind.Collection,
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            allowEmptyPayload: false);

                        var collectionStart = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            expectedItemTypeReference: expectedItemTypeReference);
                    }));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty("value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadCollectionStartAsync_ThrowsExceptionForMetadataReferenceProperty()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"#NS.Func\":{\"title\":\"Func\",\"target\":\"http://tempuri.org/Func\"}," +
                "\"value\":[\"Tea\",\"Coffee\"]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonCollectionDeserializerAndRunTestAsync(
                    payload,
                    async (JsonCollectionDeserializer) =>
                    {
                        await JsonCollectionDeserializer.ReadPayloadStartAsync(
                            ODataPayloadKind.Collection,
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            allowEmptyPayload: false);

                        var collectionStart = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            expectedItemTypeReference: expectedItemTypeReference);
                    }));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.Func"),
                exception.Message);
        }

        [Theory]
        [InlineData("\"UnexpectedProp\":\"foobar\"", "UnexpectedProp")]
        [InlineData("\"value@custom.annotation\":\"foobar\"}", "value")]
        [InlineData("\"#NS.Func\":{\"title\":\"Func\",\"target\":\"http://tempuri.org/Func\"}", "#NS.Func")]
        public async Task ReadCollectionEndAsync_ThrowsExceptionForUnexpectedPropertiesAtCollectionEnd(string unexpectedPart, string propertyName)
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            var expectedItemTypeReference = EdmCoreModel.Instance.GetString(false);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"value\":[\"Tea\",\"Coffee\"]," +
                $"{unexpectedPart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonCollectionDeserializerAndRunTestAsync(
                    payload,
                    async (JsonCollectionDeserializer) =>
                    {
                        await JsonCollectionDeserializer.ReadPayloadStartAsync(
                            ODataPayloadKind.Collection,
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            allowEmptyPayload: false);

                        var collectionStart = await JsonCollectionDeserializer.ReadCollectionStartAsync(
                            propertyAndAnnotationCollector,
                            isReadingNestedPayload: false,
                            expectedItemTypeReference: expectedItemTypeReference);
                        await JsonCollectionDeserializer.JsonReader.ReadAsync(); // Read StartArray [

                        var collectionItem1 = await JsonCollectionDeserializer.ReadCollectionItemAsync(
                            expectedItemTypeReference,
                            collectionValidator: null);
                        var collectionItem2 = await JsonCollectionDeserializer.ReadCollectionItemAsync(
                            expectedItemTypeReference,
                            collectionValidator: null);

                        await JsonCollectionDeserializer.ReadCollectionEndAsync(
                            isReadingNestedPayload: false);
                    }));

            Assert.Equal(
                ODataErrorStrings.ODataJsonCollectionDeserializer_CannotReadCollectionEnd(propertyName),
                exception.Message);
        }

        private void SetupJsonCollectionDeserializerAndRunTest(
            string payload,
            Action<ODataJsonCollectionDeserializer> action,
            bool isResponse = true)
        {
            using (var JsonInputContext = CreateJsonInputcontext(payload, isAsync: false, isResponse))
            {
                var JsonCollectionDeserializer = new ODataJsonCollectionDeserializer(JsonInputContext);

                action(JsonCollectionDeserializer);
            }
        }

        private async Task SetupJsonCollectionDeserializerAndRunTestAsync(
            string payload,
            Func<ODataJsonCollectionDeserializer, Task> func,
            bool isResponse = true)
        {
            using (var JsonInputContext = CreateJsonInputcontext(payload, isAsync: true, isResponse))
            {
                var JsonCollectionDeserializer = new ODataJsonCollectionDeserializer(JsonInputContext);

                await func(JsonCollectionDeserializer);
            }
        }

        private ODataJsonInputContext CreateJsonInputcontext(string payload, bool isAsync = false, bool isResponse = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MediaType = new ODataMediaType("application", "json"),
#if NETCOREAPP1_1
                Encoding = Encoding.GetEncoding(0),
#else
                Encoding = Encoding.Default,
#endif
                IsResponse = isResponse,
                IsAsync = isAsync,
                Model = new EdmModel()
            };

            var messageReaderSettings = new ODataMessageReaderSettings();
            messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            return new ODataJsonInputContext(
                new StringReader(payload),
                messageInfo,
                messageReaderSettings);
        }
    }
}
