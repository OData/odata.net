using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers
{
    class VerificationHttpClientHandler : DelegatingHandler
    {
        private readonly VerificationCounter counter;

        public VerificationHttpClientHandler(HttpMessageHandler innerHandler, VerificationCounter counter)
        {
            this.counter = counter;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.counter.HttpInvokeCount++;

            return base.SendAsync(request, cancellationToken);
        }
    }
}
