//-----------------------------------------------------------------------------
// <copyright file="TestHttpClientFactory.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.TestCommon
{
    internal class TestHttpClientFactory<T> : IHttpClientFactory where T : class
    {
        private readonly TestWebApplicationFactory<T> _factory;

        public TestHttpClientFactory(TestWebApplicationFactory<T> factory)
        {
            _factory = factory;
        }

        public HttpClient CreateClient(string name)
        {
            return _factory.CreateClient();
        }
    }
}
