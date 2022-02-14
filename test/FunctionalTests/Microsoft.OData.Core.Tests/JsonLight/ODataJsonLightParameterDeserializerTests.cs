﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightParameterDeserializerTests.cs" company="Microsoft">
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
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    /// <summary>
    /// Unit tests for ODataJsonLightParameterDeserializer class.
    /// </summary>
    public class ODataJsonLightParameterDeserializerTests
    {
        private EdmModel model;
        private ODataMessageReaderSettings messageReaderSettings;
        PropertyAndAnnotationCollector propertyAndAnnotationCollector;

        public ODataJsonLightParameterDeserializerTests()
        {
            this.messageReaderSettings = new ODataMessageReaderSettings();

            this.InitializeModel();
            this.propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
        }

        [Fact]
        public async Task ReadPrimitiveParameterAsync()
        {
            var setRatingAction = new EdmAction("NS", "SetRating", null);
            setRatingAction.AddParameter("rating", EdmCoreModel.Instance.GetInt32(false));
            this.model.AddElement(setRatingAction);

            var payload = "{\"rating\":4}";

            await SetupJsonLightParameterDeserializerAndRunTestAsync(
                payload,
                async (jsonLightParameterDeserializer) =>
                {
                    var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                    Assert.True(parameterRead);
                },
                setRatingAction);
        }

        [Fact]
        public async Task ReadEnumParameterAsync()
        {
            var colorEnumType = this.model.SchemaElements.Single(d => d.Name.Equals("Color")) as EdmEnumType;
            var setColorAction = new EdmAction("NS", "SetColor", null);
            setColorAction.AddParameter("color", new EdmEnumTypeReference(colorEnumType, false));
            this.model.AddElement(setColorAction);

            var payload = "{\"color\":\"Black\"}";

            await SetupJsonLightParameterDeserializerAndRunTestAsync(
                payload,
                async (jsonLightParameterDeserializer) =>
                {
                    var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                    Assert.True(parameterRead);
                },
                setColorAction);
        }

        [Fact]
        public async Task ReadTypeDefinitionParameterAsync()
        {
            var moneyTypeDefinition = new EdmTypeDefinition("NS", "Money", EdmPrimitiveTypeKind.Decimal);
            this.model.AddElement(moneyTypeDefinition);

            var setPriceAction = new EdmAction("NS", "SetPrice", null);
            setPriceAction.AddParameter("price", new EdmTypeDefinitionReference(moneyTypeDefinition, false));
            this.model.AddElement(setPriceAction);

            var payload = "{\"price\":17.30}";

            await SetupJsonLightParameterDeserializerAndRunTestAsync(
                payload,
                async (jsonLightParameterDeserializer) =>
                {
                    var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                    Assert.True(parameterRead);
                },
                setPriceAction);
        }

        [Fact]
        public async Task ReadStructuredTypeParameterAsync()
        {
            var productEntityType = this.model.SchemaElements.Single(d => d.Name.Equals("Product")) as EdmEntityType;

            var transferProductAction = new EdmAction("NS", "TransferProduct", null);
            transferProductAction.AddParameter("product", new EdmEntityTypeReference(productEntityType, false));
            this.model.AddElement(transferProductAction);

            var payload = "{\"product\":{\"Id\":1,\"Name\":\"Pencil\"}}";

            await SetupJsonLightParameterDeserializerAndRunTestAsync(
                payload,
                async (jsonLightParameterDeserializer) =>
                {
                    var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                    Assert.True(parameterRead);
                },
                transferProductAction);
        }

        [Fact]
        public async Task ReadPrimitiveCollectionParameterAsync()
        {
            var setAttributesAction = new EdmAction("NS", "SetAttributes", null);
            setAttributesAction.AddParameter(
                "attributes",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
            this.model.AddElement(setAttributesAction);

            var payload = "{\"attributes\":[\"Perishable\",\"Bulky\"]}";

            await SetupJsonLightParameterDeserializerAndRunTestAsync(
                payload,
                async (jsonLightParameterDeserializer) =>
                {
                    var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                    Assert.True(parameterRead);
                },
                setAttributesAction);
        }

        [Fact]
        public async Task ReadNullPrimitiveCollectionParameterAsync()
        {
            var setAttributesAction = new EdmAction("NS", "SetAttributes", null);
            setAttributesAction.AddParameter(
                "attributes",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
            this.model.AddElement(setAttributesAction);

            var payload = "{\"attributes\":null}";

            await SetupJsonLightParameterDeserializerAndRunTestAsync(
                payload,
                async (jsonLightParameterDeserializer) =>
                {
                    var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                    Assert.True(parameterRead);
                },
                setAttributesAction);
        }

        [Fact]
        public async Task ReadStructureTypeCollectionParameterAsync()
        {
            var productEntityType = this.model.SchemaElements.Single(d => d.Name.Equals("Product")) as EdmEntityType;

            var rateProductsAction = new EdmAction("NS", "RateProducts", null);
            rateProductsAction.AddParameter(
                "products",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(productEntityType, false))));
            this.model.AddElement(rateProductsAction);

            var payload = "{\"products\":[{\"Id\":1,\"Name\":\"Tea\"},{\"Id\":2,\"Name\":\"Coffee\"}]}";

            await SetupJsonLightParameterDeserializerAndRunTestAsync(
                payload,
                async (jsonLightParameterDeserializer) =>
                {
                    var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                    Assert.True(parameterRead);
                },
                rateProductsAction);
        }

        [Fact]
        public async Task ReadParameterWithCustomInstanceAnnotationAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var setDiscountAction = new EdmAction("NS", "SetDiscount", null);
            setDiscountAction.AddParameter("discount", EdmCoreModel.Instance.GetDouble(false));
            this.model.AddElement(setDiscountAction);

            var payload = "{\"@custom.annotation\":\"foobar\",\"discount\":1.70}";

            await SetupJsonLightParameterDeserializerAndRunTestAsync(
                payload,
                async (jsonLightParameterDeserializer) =>
                {
                    var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                    Assert.True(parameterRead);
                },
                setDiscountAction);
        }

        [Fact]
        public async Task ReadParameterAsync_ThrowsExceptionForMissingParameter()
        {
            var transferProductAction = new EdmAction("NS", "TransferProduct", null);
            transferProductAction.AddParameter("productId", EdmCoreModel.Instance.GetInt32(false));
            transferProductAction.AddParameter("categoryId", EdmCoreModel.Instance.GetInt32(false));
            this.model.AddElement(transferProductAction);

            var payload = "{\"productId\":13}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterDeserializerAndRunTestAsync(
                    payload,
                    async (jsonLightParameterDeserializer) =>
                    {
                        var parameterRead = await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);

                        Assert.True(parameterRead);
                        // Read next parameter
                        await jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector);
                    },
                    transferProductAction));

            Assert.Equal(
                ErrorStrings.ODataParameterReaderCore_ParametersMissingInPayload("TransferProduct", "categoryId"),
                exception.Message);
        }

        [Fact]
        public async Task ReadParameterAsync_ThrowsExceptionForUnexpectedMetadataReferenceProperty()
        {
            var getRatingFunction = this.model.SchemaElements.Single(d => d.Name.Equals("GetRating")) as EdmFunction;

            var payload = "{\"#NS.GetRating\":{\"title\":\"GetRating\",\"target\":\"http://tempuri.org/GetRating\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightParameterDeserializer) => jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector),
                    getRatingFunction));

            Assert.Equal(
                ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.GetRating"),
                exception.Message);
        }

        [Fact]
        public async Task ReadParameterAsync_ThrowsExceptionForUnexpectedODataInstanceAnnotation()
        {
            var getRatingFunction = this.model.SchemaElements.Single(d => d.Name.Equals("GetRating")) as EdmFunction;

            var payload = "{\"@odata.id\":\"http://tempuri.org/Products(1)\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightParameterDeserializer) => jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector),
                    getRatingFunction));

            Assert.Equal(
                ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.id"),
                exception.Message);
        }

        [Fact]
        public async Task ReadParameterAsync_ThrowsExceptionForUnexpectedPropertyWithoutValue()
        {
            var getRatingFunction = this.model.SchemaElements.Single(d => d.Name.Equals("GetRating")) as EdmFunction;

            var payload = "{\"productId@custom.annotation\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightParameterDeserializer) => jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector),
                    getRatingFunction));

            Assert.Equal(
                ErrorStrings.ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters("productId"),
                exception.Message);
        }

        [Fact]
        public async Task ReadParameterAsync_ThrowsExceptionForUnsupportedPrimitiveParameterType()
        {
            var setPhotoAction = new EdmAction("NS", "SetPhoto", null);
            setPhotoAction.AddParameter("photo", EdmCoreModel.Instance.GetStream(false));
            this.model.AddElement(setPhotoAction);

            var payload = "{\"photo\":\"AQIDBAUGBwgJAA==\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightParameterDeserializer) => jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector),
                    setPhotoAction));

            Assert.Equal(
                ErrorStrings.ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType("photo", EdmPrimitiveTypeKind.Stream),
                exception.Message);
        }

        [Fact]
        public async Task ReadParameterAsync_ThrowsExceptionForUnexpectedPrimitiveCollectionParameterValue()
        {
            var setAttributesAction = new EdmAction("NS", "SetAttributes", null);
            setAttributesAction.AddParameter(
                "attributes",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
            this.model.AddElement(setAttributesAction);

            var payload = "{\"attributes\":\"attrs\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterDeserializerAndRunTestAsync(
                    payload,
                    (jsonLightParameterDeserializer) => jsonLightParameterDeserializer.ReadNextParameterAsync(this.propertyAndAnnotationCollector),
                    setAttributesAction));

            Assert.Equal(
                ErrorStrings.ODataJsonLightParameterDeserializer_NullCollectionExpected("PrimitiveValue", "attrs"),
                exception.Message);
        }

        private async Task SetupJsonLightParameterDeserializerAndRunTestAsync(
            string payload,
            Func<ODataJsonLightParameterDeserializer, Task> func,
            IEdmOperation edmOperation = null)
        {
            using (var jsonLightInputContext = CreateJsonLightInputContext(
                payload,
                isAsync: true,
                isResponse: false))
            {
                var jsonLightParameterReader = new ODataJsonLightParameterReader(jsonLightInputContext, edmOperation);
                var jsonLightParameterDeserializer = new ODataJsonLightParameterDeserializer(jsonLightParameterReader, jsonLightInputContext);

                await jsonLightParameterDeserializer.JsonReader.ReadAsync();
                await jsonLightParameterDeserializer.JsonReader.ReadAsync();
                await func(jsonLightParameterDeserializer);
            }
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(
            string payload,
            bool isAsync = false,
            bool isResponse = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                IsResponse = isResponse,
                MediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>("odata.streaming", "true")),
                IsAsync = isAsync,
                Model = this.model,
            };

            return new ODataJsonLightInputContext(
                new StringReader(payload),
                messageInfo,
                this.messageReaderSettings);
        }

        private void InitializeModel()
        {
            this.model = new EdmModel();

            var colorEnumType = new EdmEnumType("NS", "Color");
            colorEnumType.AddMember("Black", new EdmEnumMemberValue(1));
            colorEnumType.AddMember("White", new EdmEnumMemberValue(2));
            this.model.AddElement(colorEnumType);

            var productEntityType = new EdmEntityType("NS", "Product");
            productEntityType.AddKeys(productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.model.AddElement(productEntityType);

            var getRatingFunction = new EdmFunction("NS", "GetRating", EdmCoreModel.Instance.GetInt32(true));
            getRatingFunction.AddParameter("productId", EdmCoreModel.Instance.GetInt32(false));
            this.model.AddElement(getRatingFunction);
        }
    }
}
