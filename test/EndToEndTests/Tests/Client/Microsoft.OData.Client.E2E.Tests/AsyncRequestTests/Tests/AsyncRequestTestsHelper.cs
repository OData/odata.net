//-----------------------------------------------------------------------------
// <copyright file="AsyncRequestTestsHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRequestTests.Tests;

public class AsyncRequestTestsHelper
{
    private readonly Uri BaseUri;
    private readonly IEdmModel Model;
    private readonly HttpClient Client;

    public AsyncRequestTestsHelper(Uri baseUri, IEdmModel model, HttpClient client)
    {
        this.BaseUri = baseUri;
        this.Model = model;
        this.Client = client;
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

    /// Queries an OData resource and OData resource set asynchronously based on the provided query text and MIME type.
    /// </summary>
    /// <param name="queryText">The query text to append to the base URI.</param>
    /// <param name="mimeType">The MIME type to set in the request header.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple of list of <see cref="ODataResource"/> and list of <see cref="ODataResourceSet"/>.</returns>
    public async Task<(List<ODataResource>, List<ODataResourceSet>)> QueryResourceAndResourceSetsAsync(string queryText, string mimeType)
    {
        ODataMessageReaderSettings readerSettings = new() { BaseUri = BaseUri };
        var requestUrl = new Uri(BaseUri.AbsoluteUri + queryText, UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        var responseMessage = await requestMessage.GetResponseAsync();

        Assert.Equal(200, responseMessage.StatusCode);

        var resourceEntries = new List<ODataResource>();
        var resourceSetEntries = new List<ODataResourceSet>();
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var reader = await messageReader.CreateODataResourceSetReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd && reader.Item is ODataResource odataResource)
                    {
                        resourceEntries.Add(odataResource);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd && reader.Item is ODataResourceSet odataResourceSet)
                    {
                        resourceSetEntries.Add(odataResourceSet);
                    }
                }
                Assert.Equal(ODataReaderState.Completed, reader.State);
            }

        }

        return (resourceEntries, resourceSetEntries);
    }

    /// <summary>
    /// Queries an OData resource set asynchronously based on the provided query text and MIME type.
    /// </summary>
    /// <param name="queryText">The query text to append to the base URI.</param>
    /// <param name="mimeType">The MIME type to set in the request header.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ODataResourceSet"/> if found; otherwise, null.</returns>
    public async Task<ODataResourceSet?> QueryODataResourceSetAsync(string queryText, string mimeType)
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

    /// <summary>
    /// Queries a property value in string asynchronously based on the provided request URI.
    /// </summary>
    /// <param name="requestUri">The query text to append to the base URI.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="object"/> if found; otherwise, null.</returns>
    public async Task<object?> QueryPropertyValueInStringAsync(string requestUri)
    {
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = BaseUri };

        var uri = new Uri(BaseUri.AbsoluteUri + requestUri, UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(uri, Client);

        requestMessage.SetHeader("Accept", "*/*");

        var responseMessage = await requestMessage.GetResponseAsync();

        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
        {
            return messageReader.ReadValue(EdmCoreModel.Instance.GetString(false));
        }
    }
}
