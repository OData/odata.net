//---------------------------------------------------------------------
// <copyright file="PropertyHttpClientHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers
{
    class PropertyHttpClientHandler : DelegatingHandler
    {
        private readonly VerificationCounter counter;

        public PropertyHttpClientHandler(VerificationCounter counter)
        {
            this.counter = counter;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.counter.HttpInvokeCount++;
            this.counter.HttpRequestProperties = new Dictionary<string, object>(request.Properties);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
