//---------------------------------------------------------------------
// <copyright file="ODataJsonInheritComplexCollectionWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Xunit;
using Microsoft.OData.Core;
using System.Threading.Tasks;

namespace Microsoft.OData.Tests.ScenarioTests.Writer.Json
{
    public class ODataJsonInheritComplexCollectionWriterTests
    {
        private readonly ODataResourceSet collectionStartWithoutSerializationInfo;
        private readonly ODataResourceSet collectionStartWithSerializationInfo;
        private readonly ODataResource address;
        private readonly ODataResource derivedAddress;
        private readonly EdmComplexTypeReference addressTypeReference;
        private readonly EdmComplexTypeReference derivedAddressTypeReference;
        private readonly ODataResource[] derivedItems;
        private readonly ODataResource[] items;

        public ODataJsonInheritComplexCollectionWriterTests()
        {
            collectionStartWithoutSerializationInfo = new ODataResourceSet();

            collectionStartWithSerializationInfo = new ODataResourceSet();
            collectionStartWithSerializationInfo.SetSerializationInfo(new ODataResourceSerializationInfo { ExpectedTypeName = "ns.Address" });

            address = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Street", Value = "1 Microsoft Way" },
                    new ODataProperty { Name = "Zipcode", Value = 98052 },
                    new ODataProperty { Name = "State", Value = new ODataEnumValue("WA", "ns.StateEnum") }
                }
            };
            items = new[] { address };

            derivedAddress = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "Street", Value = "1 Microsoft Way" },
                    new ODataProperty { Name = "Zipcode", Value = 98052 },
                    new ODataProperty { Name = "State", Value = new ODataEnumValue("WA", "ns.StateEnum") },
                    new ODataProperty { Name = "City", Value = "Shanghai" }
                },
                TypeName = "ns.DerivedAddress"
            };

            derivedItems = new[] { derivedAddress };

            EdmComplexType addressType = new EdmComplexType("ns", "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(isNullable: true)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Zipcode", EdmCoreModel.Instance.GetInt32(isNullable: true)));
            var stateEnumType = new EdmEnumType("ns", "StateEnum", isFlags: true);
            stateEnumType.AddMember("IL", new EdmEnumMemberValue(1));
            stateEnumType.AddMember("WA", new EdmEnumMemberValue(2));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "State", new EdmEnumTypeReference(stateEnumType, true)));

            EdmComplexType derivedAddressType = new EdmComplexType("ns", "DerivedAddress", addressType, false);
            derivedAddressType.AddProperty(new EdmStructuralProperty(derivedAddressType, "City", EdmCoreModel.Instance.GetString(isNullable: true)));

            addressTypeReference = new EdmComplexTypeReference(addressType, isNullable: false);
            derivedAddressTypeReference = new EdmComplexTypeReference(derivedAddressType, isNullable: false);
        }

        #region Writing odata.context

        [Fact]
        public async Task ShouldWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithSerializationInfo()
        {
            await WriteAndValidateAsync(this.collectionStartWithSerializationInfo, items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State@odata.type\":\"#ns.StateEnum\",\"State\":\"WA\"}]}", writingResponse: false);
        }

        [Fact]
        public async Task ShouldWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithItemType()
        {
            await WriteAndValidateAsync(this.collectionStartWithoutSerializationInfo, this.items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: false, itemTypeReference: this.addressTypeReference);
        }

        [Fact]
        public async Task ShouldNotWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithoutItemTypeAndWithoutSerializationInfo()
        {
            await WriteAndValidateAsync(this.collectionStartWithoutSerializationInfo, this.items, "{\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State@odata.type\":\"#ns.StateEnum\",\"State\":\"WA\"}]}", writingResponse: false);
        }

        [Fact]
        public async Task ShouldWriteContextUriBasedOnSerializationInfoForComplexCollectionRequestWithoutUserModelWhenBothItemTypeAndSerializationInfoAreGiven()
        {
            ODataResourceSet collectionStart = new ODataResourceSet();
            collectionStart.SetSerializationInfo(new ODataResourceSerializationInfo { ExpectedTypeName = "foo.bar" });
            await WriteAndValidateAsync(collectionStart, this.items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(foo.bar)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: false, itemTypeReference: this.addressTypeReference);
        }

        [Fact]
        public async Task ShouldWriteContextUriForComplexCollectionResponseWithoutUserModelAndWithSerializationInfo()
        {
            await WriteAndValidateAsync(this.collectionStartWithSerializationInfo, items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: true);
        }

        [Fact]
        public async Task ShouldWriteContextUriForComplexCollectionResponseWithoutUserModelAndWithItemType()
        {
            await WriteAndValidateAsync(this.collectionStartWithoutSerializationInfo, items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: true, itemTypeReference: this.addressTypeReference);
        }

        [Fact]
        public void ShouldThrowForComplexCollectionResponseWithoutUserModelAndWithoutItemTypeAndWithoutSerializationInfo()
        {
            Action sync = () => WriteAndValidateSync(/*itemTypeReference*/ null, this.collectionStartWithoutSerializationInfo, items, "", writingResponse: true);
          //  Action async = () => WriteAndValidateAsync(/*itemTypeReference*/ null, this.collectionStartWithoutSerializationInfo, items, "", writingResponse: true);
            sync.Throws<ODataException>(SRResources.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
           // Assert.Throws<AggregateException>(async);
        }

        [Fact]
        public async Task ShouldWriteContextUriBasedOnSerializationInfoForComplexCollectionResponseWithoutUserModelWhenBothItemTypeAndSerializationInfoAreGiven()
        {
            ODataResourceSet collectionStart = new ODataResourceSet();
            collectionStart.SetSerializationInfo(new ODataResourceSerializationInfo { ExpectedTypeName = "foo.bar" });
            await WriteAndValidateAsync(collectionStart, this.items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(foo.bar)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: true, itemTypeReference: this.addressTypeReference);
        }

        [Fact]
        public async Task ShouldWriteCountAndNextLinkAnnotationOfComplexCollectionPropertyIfSpecified()
        {
            ODataResourceSet collectionStart = new ODataResourceSet()
            {
                Count = 3,
                NextPageLink = new Uri("http://next-link")
            };
            collectionStart.SetSerializationInfo(new ODataResourceSerializationInfo { ExpectedTypeName = "foo.bar" });
            await WriteAndValidateAsync(collectionStart, this.items, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(foo.bar)\",\"@odata.count\":3,\"@odata.nextLink\":\"http://next-link/\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\"}]}", writingResponse: true, itemTypeReference: this.addressTypeReference);
        }
        #endregion

        #region Inheritance
        [Fact]
        public async Task ShouldWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithSerializationInfo_Inherit()
        {
            await WriteAndValidateAsync(this.collectionStartWithSerializationInfo, derivedItems, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"@odata.type\":\"#ns.DerivedAddress\",\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State@odata.type\":\"#ns.StateEnum\",\"State\":\"WA\",\"City\":\"Shanghai\"}]}", writingResponse: false);
        }

        [Fact]
        public async Task ShouldWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithItemType_Inherit()
        {
            await WriteAndValidateAsync(this.collectionStartWithoutSerializationInfo, this.derivedItems, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.DerivedAddress)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\",\"City\":\"Shanghai\"}]}", writingResponse: false, itemTypeReference: this.derivedAddressTypeReference);
        }

        [Fact]
        public async Task ShouldNotWriteContextUriForComplexCollectionRequestWithoutUserModelAndWithoutItemTypeAndWithoutSerializationInfo_Inherit()
        {
            await WriteAndValidateAsync(this.collectionStartWithoutSerializationInfo, this.derivedItems, "{\"value\":[{\"@odata.type\":\"#ns.DerivedAddress\",\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State@odata.type\":\"#ns.StateEnum\",\"State\":\"WA\",\"City\":\"Shanghai\"}]}", writingResponse: false);
        }

        [Fact]
        public async Task ShouldWriteContextUriBasedOnSerializationInfoForComplexCollectionRequestWithoutUserModelWhenBothItemTypeAndSerializationInfoAreGiven_Inherit()
        {
            ODataResourceSet collectionStart = new ODataResourceSet();
            collectionStart.SetSerializationInfo(new ODataResourceSerializationInfo { ExpectedTypeName = "foo.bar" });
            await WriteAndValidateAsync(collectionStart, this.derivedItems, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(foo.bar)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\",\"City\":\"Shanghai\"}]}", writingResponse: false, itemTypeReference: this.derivedAddressTypeReference);
        }

        [Fact]
        public async Task ShouldWriteContextUriForComplexCollectionResponseWithoutUserModelAndWithSerializationInfo_Inherit()
        {
            await WriteAndValidateAsync(this.collectionStartWithSerializationInfo, derivedItems, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"@odata.type\":\"#ns.DerivedAddress\",\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\",\"City\":\"Shanghai\"}]}", writingResponse: true);
        }

        [Fact]
        public async Task ShouldWriteContextUriForComplexCollectionResponseWithoutUserModelAndWithItemType_Inherit()
        {
            await WriteAndValidateAsync(this.collectionStartWithoutSerializationInfo, derivedItems, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.DerivedAddress)\",\"value\":[{\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\",\"City\":\"Shanghai\"}]}", writingResponse: true, itemTypeReference: this.derivedAddressTypeReference);
        }

        [Fact]
        public void ShouldThrowForComplexCollectionResponseWithoutUserModelAndWithoutItemTypeAndWithoutSerializationInfo_Inherit()
        {
            Action sync = () => WriteAndValidateSync(/*itemTypeReference*/ null, this.collectionStartWithoutSerializationInfo, derivedItems, "", writingResponse: true);
            //Action async = () => WriteAndValidateAsync(/*itemTypeReference*/ null, this.collectionStartWithoutSerializationInfo, derivedItems, "", writingResponse: true);
            sync.Throws<ODataException>(SRResources.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
            //Assert.Throws<AggregateException>(async);
        }

        [Fact]
        public async Task ShouldWriteContextUriBasedOnSerializationInfoForComplexCollectionResponseWithoutUserModelWhenBothItemTypeAndSerializationInfoAreGiven_Inherit()
        {
            ODataResourceSet collectionStart = new ODataResourceSet();
            collectionStart.SetSerializationInfo(new ODataResourceSerializationInfo { ExpectedTypeName = "foo.bar" });
            await WriteAndValidateAsync(collectionStart, this.derivedItems, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(foo.bar)\",\"value\":[{\"@odata.type\":\"#ns.DerivedAddress\",\"Street\":\"1 Microsoft Way\",\"Zipcode\":98052,\"State\":\"WA\",\"City\":\"Shanghai\"}]}", writingResponse: true, itemTypeReference: this.derivedAddressTypeReference);
        }
        #endregion Without model

        private static async Task WriteAndValidateAsync(ODataResourceSet collectionStart, IEnumerable<ODataResource> items, string expectedPayload, bool writingResponse = true, IEdmTypeReference itemTypeReference = null)
        {
            WriteAndValidateSync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
            await WriteAndValidateResourcesAsync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
            WriteAndValidatePrimitivesSync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
            await WriteAndValidatePrimitivesAsync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
        }

        private static void WriteAndValidateSync(IEdmTypeReference itemTypeReference, ODataResourceSet collectionStart, IEnumerable<ODataResource> items, string expectedPayload, bool writingResponse)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonOutputContext(stream, writingResponse, synchronous: true);
            var odataWriter = outputContext.CreateODataResourceSetWriter(null, itemTypeReference == null ? null : itemTypeReference.ToStructuredType());
            odataWriter.WriteStart(collectionStart);
            foreach (ODataResource item in items)
            {
                odataWriter.WriteStart(item);
                odataWriter.WriteEnd();
            }

            odataWriter.WriteEnd();
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static async Task WriteAndValidateResourcesAsync(IEdmTypeReference itemTypeReference, ODataResourceSet collectionStart, IEnumerable<ODataResource> items, string expectedPayload, bool writingResponse)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonOutputContext(stream, writingResponse, synchronous: false);
            var odataWriter = await outputContext.CreateODataResourceSetWriterAsync(null, itemTypeReference == null ? null : itemTypeReference.ToStructuredType());
            await odataWriter.WriteStartAsync(collectionStart);
            foreach (ODataResource item in items)
            {
                await odataWriter.WriteStartAsync(item);
                await odataWriter.WriteEndAsync();
            }

            await odataWriter.WriteEndAsync();
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static void WriteAndValidatePrimitivesSync(IEdmTypeReference itemTypeReference, ODataResourceSet collectionStart, IEnumerable<ODataResource> items, string expectedPayload, bool writingResponse)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonOutputContext(stream, writingResponse, synchronous: true);
            var odataWriter = outputContext.CreateODataResourceSetWriter(null, itemTypeReference == null ? null : itemTypeReference.ToStructuredType());
            odataWriter.WriteStart(collectionStart);
            foreach (ODataResource item in items)
            {
                odataWriter.WriteStart(new ODataResource
                {
                    Id = item.Id,
                    SerializationInfo = item.SerializationInfo,
                    InstanceAnnotations = item.InstanceAnnotations,
                    TypeAnnotation = item.TypeAnnotation,
                    TypeName = item.TypeName
                });

                foreach(var property in item.Properties)
                {
                    odataWriter.WriteStart(property);
                    odataWriter.WriteEnd();
                }

                odataWriter.WriteEnd(); // resource
            }

            odataWriter.WriteEnd(); // collection
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static async Task WriteAndValidatePrimitivesAsync(IEdmTypeReference itemTypeReference, ODataResourceSet collectionStart, IEnumerable<ODataResource> items, string expectedPayload, bool writingResponse)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonOutputContext(stream, writingResponse, synchronous: false);
            var odataWriter = await outputContext.CreateODataResourceSetWriterAsync(null, itemTypeReference == null ? null : itemTypeReference.ToStructuredType());
            await odataWriter.WriteStartAsync(collectionStart);
            foreach (ODataResource item in items)
            {
                await odataWriter.WriteStartAsync(new ODataResource
                {
                    Id = item.Id,
                    SerializationInfo = item.SerializationInfo,
                    InstanceAnnotations = item.InstanceAnnotations,
                    TypeAnnotation = item.TypeAnnotation,
                    TypeName = item.TypeName
                });

                foreach (var property in item.Properties)
                {
                    await odataWriter.WriteStartAsync(property);
                    await odataWriter.WriteEndAsync();
                }

                await odataWriter.WriteEndAsync(); // resource
            }

            await odataWriter.WriteEndAsync(); // collection
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static void ValidateWrittenPayload(MemoryStream stream, string expectedPayload)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            Assert.Equal(expectedPayload, payload);
        }

        private static ODataJsonOutputContext CreateJsonOutputContext(MemoryStream stream, bool writingResponse = true, bool synchronous = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = !synchronous,
                Model = EdmCoreModel.Instance
            };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test/"));

            return new ODataJsonOutputContext(messageInfo, settings);
        }
    }
}
