//---------------------------------------------------------------------
// <copyright file="ODataJsonCollectionReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonCollectionReaderTests
    {
        private ODataMessageReaderSettings messageReaderSettings;
        private EdmModel model;

        public ODataJsonCollectionReaderTests()
        {
            this.messageReaderSettings = new ODataMessageReaderSettings();
            this.InitializeModel();
        }

        [Fact]
        public async Task ReadPrimitiveCollectionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"value\":[\"Foo\",\"Bar\"]}";
            var itemTypeReference = EdmCoreModel.Instance.GetString(true);

            await SetupJsonCollectionReaderAndRunTestAsync(
                payload,
                itemTypeReference,
                async (JsonCollectionReader) =>
                {
                    var collectionItems = new List<object>();

                    while (await JsonCollectionReader.ReadAsync())
                    {
                        switch (JsonCollectionReader.State)
                        {
                            case ODataCollectionReaderState.Value:
                                collectionItems.Add(JsonCollectionReader.Item);
                                break;
                            default:
                                break;
                        }
                    }

                    Assert.Equal(2, collectionItems.Count);
                    Assert.Equal("Foo", collectionItems[0]);
                    Assert.Equal("Bar", collectionItems[1]);
                });
        }

        [Fact]
        public async Task ReadEmptyCollectionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\"," +
                "\"value\":[]}";
            var itemTypeReference = EdmCoreModel.Instance.GetString(true);

            await SetupJsonCollectionReaderAndRunTestAsync(
                payload,
                itemTypeReference,
                async (JsonCollectionReader) =>
                {
                    var collectionItems = new List<object>();

                    while (await JsonCollectionReader.ReadAsync())
                    {
                        switch (JsonCollectionReader.State)
                        {
                            case ODataCollectionReaderState.Value:
                                collectionItems.Add(JsonCollectionReader.Item);
                                break;
                            default:
                                break;
                        }
                    }

                    Assert.Empty(collectionItems);
                });
        }

        [Fact]
        public async Task ReadEnumCollectionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(NS.Color)\"," +
                "\"value\":[\"Black\",\"White\"]}";
            var colorEnumType = this.model.SchemaElements.SingleOrDefault(d => d.Name.Equals("Color")) as EdmEnumType;

            await SetupJsonCollectionReaderAndRunTestAsync(
                payload,
                new EdmEnumTypeReference(colorEnumType, true),
                async (JsonCollectionReader) =>
                {
                    var collectionItems = new List<object>();

                    while (await JsonCollectionReader.ReadAsync())
                    {
                        switch (JsonCollectionReader.State)
                        {
                            case ODataCollectionReaderState.Value:
                                collectionItems.Add(JsonCollectionReader.Item);
                                break;
                            default:
                                break;
                        }
                    }

                    Assert.Equal(2, collectionItems.Count);
                    var blackColor = Assert.IsType<ODataEnumValue>(collectionItems[0]);
                    var whiteColor = Assert.IsType<ODataEnumValue>(collectionItems[1]);
                    Assert.Equal("Black", blackColor.Value);
                    Assert.Equal("White", whiteColor.Value);
                });
        }

        private ODataJsonInputContext CreateJsonInputContext(string payload, bool isAsync = false, bool isResponse = true)
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
                Model = this.model
            };

            return new ODataJsonInputContext(new StringReader(payload), messageInfo, this.messageReaderSettings);
        }

        /// <summary>
        /// Sets up an ODataJsonCollectionReader,
        /// then runs the given test code asynchronously
        /// </summary>
        private async Task SetupJsonCollectionReaderAndRunTestAsync(
            string payload,
            IEdmTypeReference itemTypeReference,
            Func<ODataJsonCollectionReader, Task> func,
            bool isResponse = true)
        {
            using (var JsonInputContext = CreateJsonInputContext(payload, isAsync: true, isResponse: isResponse))
            {
                var JsonCollectionReader = new ODataJsonCollectionReader(JsonInputContext, itemTypeReference, listener: null);

                await func(JsonCollectionReader);
            }
        }

        private void InitializeModel()
        {
            this.model = new EdmModel();

            var colorEnumType = new EdmEnumType("NS", "Color");
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "Black", new EdmEnumMemberValue(0)));
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "White", new EdmEnumMemberValue(0)));
            this.model.AddElement(colorEnumType);
        }
    }
}
