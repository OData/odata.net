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
        
        private DataServiceClientRequestMessage OnMessageCreating(DataServiceClientRequestMessageArgs args, string clientName)
        {
            var message = new HttpClientRequestMessage(this.HttpClientFactory, args.ActualMethod, clientName) { Url = args.RequestUri, Method = args.Method, };
            foreach (var header in args.Headers)
            {
                message.SetHeader(header.Key, header.Value);
            }

            return message;
        }

        public void OnClientCreated(ClientCreatedArgs clientCreatedArgs)
        {
            clientCreatedArgs.ODataClient.Configurations.RequestPipeline.OnMessageCreating = (args) => this.OnMessageCreating(args, clientCreatedArgs.Name);
        }
    }
}
