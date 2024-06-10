//-----------------------------------------------------------------------------
// <copyright file="TestBaseWebApplicationFactoryOfT.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Microsoft.OData.E2E.TestCommon
{
    public class TestBaseWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public string HostUrl { get; set; } = "https://localhost:5097"; // Ensure this port is open and accessible
        private IHost? _host;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>()
                              .UseKestrel()
                              .UseUrls(HostUrl)
                              .UseContentRoot(Directory.GetCurrentDirectory());
                });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Create the initial host for TestServer
            var testHost = builder.Build();

            // Create a Kestrel host
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseKestrel();
                    webHostBuilder.UseUrls(HostUrl);
                    webHostBuilder.UseStartup<TStartup>();
                    webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                })
                .Build();

            // Start the Kestrel server
            _host.Start();

            ClientOptions.BaseAddress = new Uri(HostUrl);

            // Start the TestServer and return it
            testHost.Start();
            return testHost;
        }

        public string ServerAddress
        {
            get
            {
                EnsureServer();
                return ClientOptions.BaseAddress.ToString();
            }
        }

        protected override void Dispose(bool disposing)
        {
            _host?.Dispose();
            base.Dispose(disposing);
        }

        private void EnsureServer()
        {
            if (_host == null)
            {
                // Force WebApplicationFactory to bootstrap the server
                using var _ = CreateDefaultClient();
            }
        }
    }
}


