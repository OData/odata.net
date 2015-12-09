//---------------------------------------------------------------------
// <copyright file="ODataJsonLightCollectionWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightCollectionWriterTests
    {
        private readonly ODataCollectionStart collectionStartWithoutSerializationInfo;
        private readonly ODataCollectionStart collectionStartWithSerializationInfo;
        private readonly ODataComplexValue address;
        private readonly EdmComplexTypeReference addressTypeReference;
        private readonly object[] items;

        public ODataJsonLightCollectionWriterTests()
        {
            collectionStartWithoutSerializationInfo = new ODataCollectionStart();

            collectionStartWithSerializationInfo = new ODataCollectionStart();
            collectionStartWithSerializationInfo.SetSerializationInfo(new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(ns.Address)" });

            address = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "Street", Value = "1 Microsoft Way" }, new ODataProperty { Name = "Zipcode", Value = 98052 }, new ODataProperty { Name = "State", Value = new ODataEnumValue("WA", "ns.StateEnum") } } };
            items = new[] { address };

            EdmComplexType addressType = new EdmComplexType("ns", "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(isNullable: true)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Zipcode", EdmCoreModel.Instance.GetInt32(isNullable: true)));
            var stateEnumType = new EdmEnumType("ns", "StateEnum", isFlags: true);
            stateEnumType.AddMember("IL", new EdmIntegerConstant(1));
            stateEnumType.AddMember("WA", new EdmIntegerConstant(2));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "State", new EdmEnumTypeReference(stateEnumType, true)));

            addressTypeReference = new EdmComplexTypeReference(addressType, isNullable: false);
        }

        #region Writing odata.context
        #region Without model
        [Fact]
        public void ShouldWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithSerializationInfo()
        {
            WriteAndValidate(this.collectionStartWithSerializationInfo, items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State@odata.type\":\"#ns.StateEnum\",\"State\":\"WA\"}]}", writingResponse: false);
        }

        [Fact]
        public void ShouldWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithItemType()
        {
            WriteAndValidate(this.collectionStartWithoutSerializationInfo, this.items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: false, itemTypeReference: this.addressTypeReference);
        }

        [Fact]
        public void ShouldNotWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithoutItemTypeAndWithoutSerializationInfo()
        {
            WriteAndValidate(this.collectionStartWithoutSerializationInfo, this.items, "{\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State@odata.type\":\"#ns.StateEnum\",\"State\":\"WA\"}]}", writingResponse: false);
        }

        [Fact]
        public void ShouldWriteContextUriBasedOnSerializationInfoForComplexCollectionRequestWithoutUserModelWhenBothItemTypeAndSerializationInfoAreGiven()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            collectionStart.SetSerializationInfo(new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(foo.bar)" });
            WriteAndValidate(collectionStart, this.items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(foo.bar)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: false, itemTypeReference: this.addressTypeReference);
        }

        [Fact]
        public void ShouldWriteContextUriForComplexCollectionResponseWithoutUserModelAndWithSerializationInfo()
        {
            WriteAndValidate(this.collectionStartWithSerializationInfo, items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: true);
        }

        [Fact]
        public void ShouldWriteContextUriForComplexCollectionResponseWithoutUserModelAndWithItemType()
        {
            WriteAndValidate(this.collectionStartWithoutSerializationInfo, items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: true, itemTypeReference: this.addressTypeReference);
        }

        [Fact]
        public void ShouldThrowForComplexCollectionResponseWithoutUserModelAndWithoutItemTypeAndWithoutSerializationInfo()
        {
            Action sync = () => WriteAndValidateSync(/*itemTypeReference*/ null, this.collectionStartWithoutSerializationInfo, items, "", writingResponse: true);
            Action async = () => WriteAndValidateAsync(/*itemTypeReference*/ null, this.collectionStartWithoutSerializationInfo, items, "", writingResponse: true);
            sync.ShouldThrow<ODataException>().WithMessage(Strings.ODataContextUriBuilder_TypeNameMissingForTopLevelCollection);
            async.ShouldThrow<AggregateException>();
        }

        [Fact]
        public void ShouldWriteContextUriBasedOnSerializationInfoForComplexCollectionResponseWithoutUserModelWhenBothItemTypeAndSerializationInfoAreGiven()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            collectionStart.SetSerializationInfo(new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(foo.bar)" });
            WriteAndValidate(collectionStart, this.items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(foo.bar)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: true, itemTypeReference: this.addressTypeReference);
        }

        [Fact]
        public void ShouldWriteCountAndNextLinkAnnotationOfComplexCollectionPropertyIfSpecified()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart()
            {
                Count = 3,
                NextPageLink = new Uri("http://next-link")
            };
            collectionStart.SetSerializationInfo(new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(foo.bar)" });
            WriteAndValidate(collectionStart, this.items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(foo.bar)\",\"@odata.count\":3,\"@odata.nextLink\":\"http://next-link/\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: true, itemTypeReference: this.addressTypeReference);
        }
        #endregion Without model
        #endregion Writing odata.context

        [Fact]
        public void ShouldWriteCollectionOfTypeDefinitionItemType()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            WriteAndValidate(collectionStart, new object[] { 123 }, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.Test)\",\"value\":[123]}", true, new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), false));
        }

        private static void WriteAndValidate(ODataCollectionStart collectionStart, IEnumerable<object> items, string expectedPayload, bool writingResponse = true, IEdmTypeReference itemTypeReference = null)
        {
            WriteAndValidateSync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
            WriteAndValidateAsync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
        }

        private static void WriteAndValidateSync(IEdmTypeReference itemTypeReference, ODataCollectionStart collectionStart, IEnumerable<object> items, string expectedPayload, bool writingResponse)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, writingResponse, synchronous: true);
            var collectionWriter = new ODataJsonLightCollectionWriter(outputContext, itemTypeReference);
            collectionWriter.WriteStart(collectionStart);
            foreach (object item in items)
            {
                collectionWriter.WriteItem(item);
            }

            collectionWriter.WriteEnd();
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static void WriteAndValidateAsync(IEdmTypeReference itemTypeReference, ODataCollectionStart collectionStart, IEnumerable<object> items, string expectedPayload, bool writingResponse)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, writingResponse, synchronous: false);
            var collectionWriter = new ODataJsonLightCollectionWriter(outputContext, itemTypeReference);
            collectionWriter.WriteStartAsync(collectionStart).Wait();
            foreach (object item in items)
            {
                collectionWriter.WriteItemAsync(item).Wait();
            }

            collectionWriter.WriteEndAsync().Wait();
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static void ValidateWrittenPayload(MemoryStream stream, string expectedPayload)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(expectedPayload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, bool synchronous = true)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test/"));

            return new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                writingResponse,
                synchronous,
                EdmCoreModel.Instance,
                /*urlResolver*/ null);
        }
    }
}
