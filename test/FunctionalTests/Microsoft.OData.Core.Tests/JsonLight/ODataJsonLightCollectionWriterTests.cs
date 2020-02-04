//---------------------------------------------------------------------
// <copyright file="ODataJsonLightCollectionWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightCollectionWriterTests
    {
        private IEdmModel model = EdmCoreModel.Instance;

        [Fact]
        public void ShouldWriteDynamicNullableCollectionValuedProperty()
        {
            // setup model
            var model = new EdmModel();
            var entityType = new EdmEntityType("NS", "EntityType", null, false, true);
            entityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
            var container = new EdmEntityContainer("NS", "Container");
            var entitySet = container.AddEntitySet("EntitySet", entityType);
            var complexType = new EdmComplexType("NS", "ComplexType");
            complexType.AddStructuralProperty("Prop1", EdmPrimitiveTypeKind.Int32);
            complexType.AddStructuralProperty("Prop2", EdmPrimitiveTypeKind.Int32);
            model.AddElements(new IEdmSchemaElement[] { entityType, complexType, container });

            // setup writer
            var stream = new MemoryStream();
            var message = new InMemoryMessage { Stream = stream };
            var settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri
                {
                    ServiceRoot = new Uri("http://svc/")
                }
            };
            var writer = new ODataMessageWriter((IODataResponseMessage)message, settings, model)
                         .CreateODataResourceWriter(entitySet, entityType);

            // write payload
            writer.Write(new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 1 },
                    new ODataProperty
                    {
                        Name = "DynamicPrimitive",
                        Value = new ODataCollectionValue
                        {
                            TypeName = "Collection(Edm.Int64)",
                            Items = new object[] { 1L, 2L, null }
                        }
                    }
                }
            }, () => writer
                .Write(new ODataNestedResourceInfo
                {
                    Name = "DynamicComplex",
                    IsCollection = true,
                }, () => writer
                    .Write(new ODataResourceSet
                    {
                        TypeName = "Collection(NS.ComplexType)"
                    }, () => writer
                        .Write((ODataResource)null)
                        .Write((ODataResource)null)
                        .Write(new ODataResource
                        {
                            Properties = new[]
                            {
                                new ODataProperty { Name = "Prop1", Value = 1 },
                                new ODataProperty { Name = "Prop2", Value = 2 }
                            }
                        }))));
            var str = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal(str,
                "{" +
                    "\"@odata.context\":\"http://svc/$metadata#EntitySet/$entity\"," +
                    "\"ID\":1," +
                    "\"DynamicPrimitive@odata.type\":\"#Collection(Int64)\"," +
                    "\"DynamicPrimitive\":[1,2,null]," +
                    "\"DynamicComplex@odata.type\":\"#Collection(NS.ComplexType)\"," +
                    "\"DynamicComplex\":[null,null,{\"Prop1\":1,\"Prop2\":2}]" +
                "}");
        }

        [Fact]
        public void ShouldWriteCollectionOfTypeDefinitionItemType()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            WriteAndValidate(collectionStart, new object[] { 123 }, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.Test)\",\"value\":[123]}", true, new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), false));
        }

        [Fact]
        public void ShouldWriteCollectionOfResourceValueItem()
        {
            EdmModel currentModel = new EdmModel();
            EdmComplexType addressType = new EdmComplexType("ns", "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(isNullable: true)));
            currentModel.AddElement(addressType);
            this.model = currentModel;
            var address = new ODataResourceValue
            {
                TypeName = "ns.Address",
                Properties = new[] { new ODataProperty { Name = "Street", Value = "1 Microsoft Way" } }
            };

            WriteAndValidate(new ODataCollectionStart(),
                new object[] { address },
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\"}]}",
                true,
                new EdmComplexTypeReference(addressType, false));
        }

        [Fact]
        public void ShouldWriteCollectionOfDerivedResourceValueItem()
        {
            EdmModel currentModel = new EdmModel();
            EdmComplexType addressType = new EdmComplexType("ns", "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(isNullable: true)));
            currentModel.AddElement(addressType);
            EdmComplexType homeAddressType = new EdmComplexType("ns", "HomeAddress", addressType);
            homeAddressType.AddProperty(new EdmStructuralProperty(homeAddressType, "City", EdmCoreModel.Instance.GetString(isNullable: true)));
            currentModel.AddElement(homeAddressType);
            this.model = currentModel;

            var address = new ODataResourceValue
            {
                TypeName = "ns.HomeAddress",
                Properties = new[]
                {
                    new ODataProperty { Name = "Street", Value = "1 Microsoft Way" },
                    new ODataProperty { Name = "City", Value = "Redmond" },
                }
            };

            WriteAndValidate(new ODataCollectionStart(),
                new object[] { address },
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"@odata.type\":\"#ns.HomeAddress\",\"Street\":\"1 Microsoft Way\",\"City\":\"Redmond\"}]}",
                true,
                new EdmComplexTypeReference(addressType, false));
        }

        private void WriteAndValidate(ODataCollectionStart collectionStart, IEnumerable<object> items, string expectedPayload, bool writingResponse = true, IEdmTypeReference itemTypeReference = null)
        {
            WriteAndValidateSync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
            //WriteAndValidateAsync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
        }

        private void WriteAndValidateSync(IEdmTypeReference itemTypeReference, ODataCollectionStart collectionStart, IEnumerable<object> items, string expectedPayload, bool writingResponse)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, this.model, writingResponse, synchronous: true);
            var collectionWriter = new ODataJsonLightCollectionWriter(outputContext, itemTypeReference);
            collectionWriter.WriteStart(collectionStart);
            foreach (object item in items)
            {
                collectionWriter.WriteItem(item);
            }

            collectionWriter.WriteEnd();
            ValidateWrittenPayload(stream, expectedPayload);
        }

        //private void WriteAndValidateAsync(IEdmTypeReference itemTypeReference, ODataCollectionStart collectionStart, IEnumerable<object> items, string expectedPayload, bool writingResponse)
        //{
        //    MemoryStream stream = new MemoryStream();
        //    var outputContext = CreateJsonLightOutputContext(stream, this.model, writingResponse, synchronous: false);
        //    var collectionWriter = new ODataJsonLightCollectionWriter(outputContext, itemTypeReference);
        //    collectionWriter.WriteStartAsync(collectionStart).Wait();
        //    foreach (object item in items)
        //    {
        //        collectionWriter.WriteItemAsync(item).Wait();
        //    }

        //    collectionWriter.WriteEndAsync().Wait();
        //    ValidateWrittenPayload(stream, expectedPayload);
        //}

        private static void ValidateWrittenPayload(MemoryStream stream, string expectedPayload)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            Assert.Equal(payload, expectedPayload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, IEdmModel model, bool writingResponse = true, bool synchronous = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = !synchronous,
                Model = model
            };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test/"));

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }
    }
}
