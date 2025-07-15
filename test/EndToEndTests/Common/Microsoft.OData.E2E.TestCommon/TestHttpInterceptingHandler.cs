//-----------------------------------------------------------------------------
// <copyright file="TestHttpInterceptingHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Net;
using System.Text;

namespace Microsoft.OData.E2E.TestCommon
{
    /// <summary>
    /// This class is used to intercept HTTP requests and provide a controlled response for testing purposes.
    /// </summary>
    public class TestHttpInterceptingHandler : DelegatingHandler
    {
        private readonly HttpStatusCode httpStatusCode;
        private readonly Uri? location;
        private readonly string responseBody;

        public HttpRequestMessage? InterceptedRequest { get; private set; }

        public string? InterceptedBody { get; private set; }

        public TestHttpInterceptingHandler()
            : this(HttpStatusCode.OK)
        {
        }

        public TestHttpInterceptingHandler(HttpStatusCode httpStatusCode)
            : this(httpStatusCode, location: null)
        {
        }

        public TestHttpInterceptingHandler(HttpStatusCode httpStatusCode, Uri? location)
            : this(httpStatusCode, location, "{}")
        {
        }

        public TestHttpInterceptingHandler(HttpStatusCode httpStatusCode, Uri? location, string responseBody)
            : base(new HttpClientHandler())
        {
            this.httpStatusCode = httpStatusCode;
            this.location = location;
            this.responseBody = responseBody;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            InterceptedRequest = request;

            if (request.Content != null)
            {
                InterceptedBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            // Return dummy response
            var responseMessage = new HttpResponseMessage(httpStatusCode)
            {
                RequestMessage = request,
                Content = new StringContent(this.responseBody, Encoding.UTF8, "application/json")
            };

            if (location != null)
            {
                responseMessage.Headers.Location = location;
            }

            return responseMessage;
        }
    }

}
