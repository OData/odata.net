//---------------------------------------------------------------------
// <copyright file="QueryOptionTestsHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Reflection;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Tests
{
    public class QueryOptionTestsHelper
    {
        private readonly Uri BaseUri;
        private readonly IEdmModel Model;
        private readonly HttpClient Client;
        private const string IncludeAnnotation = "odata.include-annotations";

        public QueryOptionTestsHelper(Uri baseUri, IEdmModel model, HttpClient client)
        {
            this.BaseUri = baseUri;
            this.Model = model;
            this.Client = client;
        }

        public async Task<List<ODataResource>> QueryResourceEntriesAsync(string queryText, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = BaseUri };
            var requestUrl = new Uri(BaseUri.AbsoluteUri + queryText, UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);

            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            var entries = new List<ODataResource>();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = await messageReader.CreateODataResourceReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            if (reader.Item is ODataResource odataResource)
                            {
                                entries.Add(odataResource);
                            }
                        }
                    }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }

            return entries;
        }

        public async Task<List<ODataResource>> QueryResourceSetFeedAsync(string queryText, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = BaseUri };
            var requestUrl = new Uri(BaseUri.AbsoluteUri + queryText, UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);

            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            var entries = new List<ODataResource>();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = await messageReader.CreateODataResourceSetReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            if (reader.Item is ODataResource odataResource)
                            {
                                entries.Add(odataResource);
                            }
                        }
                    }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
                
            }

            return entries;
        }

        public async Task<ODataResourceSet?> QueryInnerFeedAsync(string queryText, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = BaseUri };
            var requestUrl = new Uri(BaseUri.AbsoluteUri + queryText, UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);

            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = await messageReader.CreateODataResourceReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        if (reader.State == ODataReaderState.ResourceSetEnd && reader.Item is ODataResourceSet oDataResourceSet)
                        {
                            return oDataResourceSet;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<ODataEntityReferenceLink?> QueryReferenceLinkAsync(string queryText, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = BaseUri };
            var requestUrl = new Uri(BaseUri.AbsoluteUri + queryText, UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            ODataEntityReferenceLink item = null;
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    item = messageReader.ReadEntityReferenceLink();
                }
            }

            return item;
        }

        public async Task<ODataEntityReferenceLinks?> QueryReferenceLinksAsync(string queryText, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = BaseUri };
            var requestUrl = new Uri(BaseUri.AbsoluteUri + queryText, UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            ODataEntityReferenceLinks item = null;
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    item = messageReader.ReadEntityReferenceLinks();
                }
            }

            return item;
        }

        public async Task<ODataProperty> QueryPropertyAsync(string queryText, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = BaseUri };
            var requestUrl = new Uri(BaseUri.AbsoluteUri + queryText, UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);

            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            ODataProperty property = null;

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    property = messageReader.ReadProperty();
                }
            }
            return property;
        }
    }
}
