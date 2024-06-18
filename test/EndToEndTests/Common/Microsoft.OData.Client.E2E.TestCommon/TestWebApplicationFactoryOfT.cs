//-----------------------------------------------------------------------------
// <copyright file="TestWebApplicationFactoryOfT.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.OData.Client.E2E.TestCommon
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseStartup<TStartup>()
                .ConfigureServices(services => {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHttpClientFactory));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Register the custom IHttpClientFactory
                    services.AddSingleton<IHttpClientFactory>(sp => new TestHttpClientFactory<TStartup>(this));
                })
                .UseContentRoot("");
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder();
        }
    }
}
