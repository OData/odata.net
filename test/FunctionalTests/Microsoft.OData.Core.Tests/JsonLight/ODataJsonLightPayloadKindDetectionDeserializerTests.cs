﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightPayloadKindDetectionDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightPayloadKindDetectionDeserializerTests
    {
        private EdmModel model;
        private ODataMessageReaderSettings messageReaderSettings;
        private ODataMessageInfo messageInfo;
        private ODataPayloadKindDetectionInfo payloadKindDetectionInfo;

        public ODataJsonLightPayloadKindDetectionDeserializerTests()
        {
            this.InitializeEdmModel();
        }

        [Fact]
        public void DetectPayloadKindForContextUriInPayload()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Products/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Tea\"}";

            SetupJsonLightPayloadKindDetectionDeserializerAndRunTest(
                payload,
                (jsonLightPayloadKindDetectionDeserializer) =>
                {
                    var payloadKinds = jsonLightPayloadKindDetectionDeserializer.DetectPayloadKind(this.payloadKindDetectionInfo);

                    Assert.Equal(2, payloadKinds.Count());
                    Assert.Single(payloadKinds.Where(d => d.Equals(ODataPayloadKind.Resource)));
                    Assert.Single(payloadKinds.Where(d => d.Equals(ODataPayloadKind.Delta)));
                });
        }

        [Theory]
        [InlineData("{\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"@custom.annotation\":\"foobar\",\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"},\"@custom.annotation\":\"foobar\"}")]
        public void DetectPayloadKindForValidErrorPayload(string payload)
        {
            SetupJsonLightPayloadKindDetectionDeserializerAndRunTest(
                payload,
                (jsonLightPayloadKindDetectionDeserializer) =>
                {
                    var payloadKinds = jsonLightPayloadKindDetectionDeserializer.DetectPayloadKind(this.payloadKindDetectionInfo);

                    Assert.Single(payloadKinds);
                    Assert.Single(payloadKinds.Where(d => d.Equals(ODataPayloadKind.Error)));
                });
        }

        [Theory]
        [InlineData("{\"error\":{\"foo\":\"bar\"}}")]
        [InlineData("{\"error@custom.annotation\":\"foobar\",\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"},\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"any\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"@odata.type\":\"#NS.Error\",\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{}")]
        [InlineData("{\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}")]
        public void DetectPayloadKindForInvalidErrorPayload(string payload)
        {
            SetupJsonLightPayloadKindDetectionDeserializerAndRunTest(
                payload,
                (jsonLightPayloadKindDetectionDeserializer) =>
                {
                    var payloadKinds = jsonLightPayloadKindDetectionDeserializer.DetectPayloadKind(this.payloadKindDetectionInfo);

                    Assert.Empty(payloadKinds);
                });
        }

        [Fact]
        public async Task DetectPayloadKindForContextUriInPayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Products/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Tea\"}";

            await SetupJsonLightPayloadKindDetectionDeserializerAndRunTestAsync(
                payload,
                async (jsonLightPayloadKindDetectionDeserializer) =>
                {
                    var payloadKinds = await jsonLightPayloadKindDetectionDeserializer.DetectPayloadKindAsync(this.payloadKindDetectionInfo);

                    Assert.Equal(2, payloadKinds.Count());
                    Assert.Single(payloadKinds.Where(d => d.Equals(ODataPayloadKind.Resource)));
                    Assert.Single(payloadKinds.Where(d => d.Equals(ODataPayloadKind.Delta)));
                });
        }

        [Theory]
        [InlineData("{\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"@custom.annotation\":\"foobar\",\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"},\"@custom.annotation\":\"foobar\"}")]
        public async Task DetectPayloadKindForValidErrorPayloadAsync(string payload)
        {
            await SetupJsonLightPayloadKindDetectionDeserializerAndRunTestAsync(
                payload,
                async (jsonLightPayloadKindDetectionDeserializer) =>
                {
                    var payloadKinds = await jsonLightPayloadKindDetectionDeserializer.DetectPayloadKindAsync(this.payloadKindDetectionInfo);

                    Assert.Single(payloadKinds);
                    Assert.Single(payloadKinds.Where(d => d.Equals(ODataPayloadKind.Error)));
                });
        }

        [Theory]
        [InlineData("{\"error\":{\"foo\":\"bar\"}}")]
        [InlineData("{\"error@custom.annotation\":\"foobar\",\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"},\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"any\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{\"@odata.type\":\"#NS.Error\",\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}}")]
        [InlineData("{}")]
        [InlineData("{\"error\":{\"code\":\"NRE\",\"message\":\"MSG\",\"target\":\"ConApp\"}")]
        public async Task DetectPayloadKindForInvalidErrorPayloadAsync(string payload)
        {
            await SetupJsonLightPayloadKindDetectionDeserializerAndRunTestAsync(
                payload,
                async (jsonLightPayloadKindDetectionDeserializer) =>
                {
                    var payloadKinds = await jsonLightPayloadKindDetectionDeserializer.DetectPayloadKindAsync(this.payloadKindDetectionInfo);

                    Assert.Empty(payloadKinds);
                });
        }

        private async Task SetupJsonLightPayloadKindDetectionDeserializerAndRunTestAsync(
            string payload,
            Func<ODataJsonLightPayloadKindDetectionDeserializer, Task> func,
            bool isResponse = true)
        {
            using (var jsonInputContext = CreateJsonLightInputContext(
                payload,
                this.model,
                isAsync: true,
                isResponse: isResponse))
            {
                var jsonLightPayloadKindDetectionDeserializer = new ODataJsonLightPayloadKindDetectionDeserializer(jsonInputContext);

                await func(jsonLightPayloadKindDetectionDeserializer);
            }
        }

        private void SetupJsonLightPayloadKindDetectionDeserializerAndRunTest(
            string payload,
            Action<ODataJsonLightPayloadKindDetectionDeserializer> action,
            bool isResponse = true)
        {
            using (var jsonInputContext = CreateJsonLightInputContext(
                payload,
                this.model,
                isAsync: false,
                isResponse: isResponse))
            {
                var jsonLightPayloadKindDetectionDeserializer = new ODataJsonLightPayloadKindDetectionDeserializer(jsonInputContext);

                action(jsonLightPayloadKindDetectionDeserializer);
            }
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(
            string payload,
            IEdmModel model,
            bool isAsync = false,
            bool isResponse = true)
        {
            this.messageInfo = new ODataMessageInfo
            {
                IsResponse = isResponse,
                MediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>("odata.streaming", "true")),
                IsAsync = isAsync,
                Model = this.model,
            };

            this.messageReaderSettings = new ODataMessageReaderSettings();
            messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            payloadKindDetectionInfo = new ODataPayloadKindDetectionInfo(this.messageInfo, this.messageReaderSettings);

            return new ODataJsonLightInputContext(
                new StringReader(payload),
                messageInfo,
                messageReaderSettings);
        }

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();
            var productEntityType = new EdmEntityType("NS", "Product");

            var productIdProperty = productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            productEntityType.AddKeys(productIdProperty);
            productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.model.AddElement(productEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            entityContainer.AddEntitySet("Products", productEntityType);
        }
    }
}
