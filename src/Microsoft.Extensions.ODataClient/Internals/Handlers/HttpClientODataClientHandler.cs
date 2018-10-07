//---------------------------------------------------------------------
// <copyright file="HttpClientODataClientHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.OData.Client;
    using System.Net.Http;

    /// <summary>
    /// an OData proxy handler that configures the proxy to use HttpClientFactory 
    /// </summary>
    internal sealed class HttpClientODataClientHandler : IODataClientHandler
    {
        public IHttpClientFactory HttpClientFactory { get; }

        public HttpClientODataClientHandler(IHttpClientFactory httpClientFactory)
        {
            this.HttpClientFactory = httpClientFactory;
        }

        public void OnClientCreated(ClientCreatedArgs clientArgs)
        {
            clientArgs.ODataClient.Configurations.RequestPipeline.OnMessageCreating = (args) => new HttpClientRequestMessage(this.HttpClientFactory.CreateClient(clientArgs.Name), args, clientArgs.ODataClient.Configurations);
        }
    }
}
