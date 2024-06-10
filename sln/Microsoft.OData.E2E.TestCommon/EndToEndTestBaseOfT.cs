//-----------------------------------------------------------------------------
// <copyright file="EndToEndTestBaseOfT.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Microsoft.OData.E2E.TestCommon
{
    public class EndToEndTestBase<T> : IClassFixture<TestBaseWebApplicationFactory<T>> where T : class
    {
        private HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebODataTestBase{TStartup}"/> class.
        /// </summary>
        /// <param name="factory">The factory used to initialize the web service client.</param>
        protected EndToEndTestBase(TestBaseWebApplicationFactory<T> factory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
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
                    _client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                    {
                        BaseAddress = new Uri("http://localhost:5097")
                    });
                }
                _client.Timeout = TimeSpan.FromSeconds(10000);

                return _client;
            }
        }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        public TestBaseWebApplicationFactory<T> Factory { get; }

    }
}
