//---------------------------------------------------------------------
// <copyright file="SingletonTestsHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.SingletonTests.Tests;

public class SingletonTestsHelper
{
    private readonly Uri BaseUri;
    private readonly IEdmModel Model;
    private readonly HttpClient Client;
    private const string IncludeAnnotation = "odata.include-annotations";

    public SingletonTestsHelper(Uri baseUri, IEdmModel model, HttpClient client)
    {
        BaseUri = baseUri;
        Model = model;
        Client = client;
    }

    /// <summary>
    /// Queries resource entries asynchronously based on the provided query text and MIME type.
    /// </summary>
    /// <param name="queryText">The query text to append to the base URI.</param>
    /// <param name="mimeType">The MIME type to set in the request header.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="ODataResource"/>.</returns>
    public async Task<List<ODataResource>> QueryResourceEntriesAsync(string queryText, string mimeType)
    {
        ODataMessageReaderSettings readerSettings = new() { BaseUri = BaseUri };
        var requestUrl = new Uri(BaseUri.AbsoluteUri + queryText, UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

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
                    if (reader.State == ODataReaderState.ResourceEnd && reader.Item is ODataResource odataResource)
                    {
                        entries.Add(odataResource);
                    }
                }
                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        return entries;
    }

    /// <summary>
    /// Queries resource sets asynchronously based on the provided query text and MIME type.
    /// </summary>
    /// <param name="queryText">The query text to append to the base URI.</param>
    /// <param name="mimeType">The MIME type to set in the request header.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="ODataResource"/>.</returns>
    public async Task<List<ODataResource>> QueryResourceSetsAsync(string queryText, string mimeType)

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

    /// <summary>
    /// Queries a property asynchronously based on the provided request URI and MIME type.
    /// </summary>
    /// <param name="requestUri">The request URI to append to the base URI.</param>
    /// <param name="mimeType">The MIME type to set in the request header.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ODataProperty"/> if found; otherwise, null.</returns>
    public async Task<ODataProperty?> QueryPropertyAsync(string requestUri, string mimeType)
    {
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = BaseUri };

        var uri = new Uri(BaseUri.AbsoluteUri + requestUri, UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(uri, Client);

        requestMessage.SetHeader("Accept", mimeType);

        var responseMessage = await requestMessage.GetResponseAsync();

        Assert.Equal(200, responseMessage.StatusCode);

        ODataProperty? property = null;

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
