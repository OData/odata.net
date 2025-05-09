//---------------------------------------------------------------------
// <copyright file="CustomHttpClientRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Globalization;
using Microsoft.OData.E2E.TestCommon.Common;

namespace Microsoft.OData.Core.E2E.Tests.AsyncRequestTests;

internal class CustomHttpClientRequestMessage : TestHttpClientRequestMessage
{
    private readonly HttpClient _httpClient;

    public CustomHttpClientRequestMessage(Uri uri, HttpClient httpClient) : base(uri, httpClient)
    {
        this._httpClient = httpClient;
    }

    public override async Task<IODataResponseMessageAsync> GetResponseAsync(string mimeType, TestHttpClientRequestMessage? requestMessage = null)
    {
        string? asyncToken = null;
        if (this.Url.Query.Contains("$asyncToken"))
        {
            asyncToken = this.Url.Query.Split("=")[^1];
        }

        if (asyncToken == null || AsyncTask.GetTask(asyncToken) == null)
        {
            var now = DateTime.Now;
            asyncToken = now.Ticks.ToString(CultureInfo.InvariantCulture);

            var newAsyncTask = new AsyncTask(this.Url, DateTime.Now.AddSeconds(5), requestMessage);
            AsyncTask.AddTask(asyncToken, newAsyncTask);
        }

        var asyncTask = AsyncTask.GetTask(asyncToken);

        if (!asyncTask.Ready)
        {
            return new CustomODataResponseMessageAsync(new Uri(this.Url, $"async?$asyncToken={asyncToken}"), mimeType);
        }

        // Get the RequestMessage from asyncTask
        var request = asyncTask.GetRequestMessage();

        if (request == null)
        {
            var requestUrl = asyncTask.GetOriginalUrl();
            request = new TestHttpClientRequestMessage(requestUrl, _httpClient);
            request.SetHeader("Accept", mimeType);
            request.SetHeader("Content-Type", mimeType);
            request.SetHeader("Prefer", "respond-async");
        }

        return await request.GetResponseAsync();
    }
}

public class CustomODataResponseMessageAsync : IODataResponseMessageAsync
{
    private readonly Dictionary<string, string> _headers = new();
    private readonly MemoryStream _stream = new();

    public CustomODataResponseMessageAsync(Uri requestUri, string mimeType)
    {
        StatusCode = 202; // Accepted
        SetHeader("Location", requestUri.OriginalString);
        SetHeader("Prefer", "respond-async");
        SetHeader("OData-Version", "4.0");
        SetHeader("Accept", mimeType);
    }

    public IEnumerable<KeyValuePair<string, string>> Headers => _headers;

    public int StatusCode { get; set; }

    public string GetHeader(string headerName)
    {
        _headers.TryGetValue(headerName, out var value);
        return value;
    }

    public void SetHeader(string headerName, string headerValue)
    {
        _headers[headerName] = headerValue;
    }

    public Stream GetStream()
    {
        return _stream;
    }

    public Task<Stream> GetStreamAsync()
    {
        return Task.FromResult<Stream>(_stream);
    }
}
