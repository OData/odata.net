//-----------------------------------------------------------------------------
// <copyright file="EndToEndTestBaseOfT.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Client.E2E.TestCommon
{
    public class EndToEndTestBase<T> : IClassFixture<TestWebApplicationFactory<T>> where T : class
    {
        private HttpClient _client;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndToEndTestBase{T}"/> class.
        /// </summary>
        /// <param name="factory">The factory used to initialize the web service client.</param>
        protected EndToEndTestBase(TestWebApplicationFactory<T> factory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _httpClientFactory = Factory.Services.GetRequiredService<IHttpClientFactory>();
        }

        /// <summary>
        /// An HttpClient to use with the server.
        /// </summary>
        public virtual HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = Factory.CreateClient();
                }
                return _client;
            }
        }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        public TestWebApplicationFactory<T> Factory { get; }

        /// <summary>
        /// Gets the HttpClientFactory.
        /// </summary>
        public IHttpClientFactory HttpClientFactory => _httpClientFactory;
    }
}
