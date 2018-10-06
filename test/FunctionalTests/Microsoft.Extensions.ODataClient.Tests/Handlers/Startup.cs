using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers
{
    class Startup
    {
        internal IServiceProvider ConfigureServices(ServiceCollection sc)
        {
            sc.AddSingleton<VerificationCounter>();
            sc.AddTransient<VerificationController>();

            sc
                .AddODataClient("Verification")
                .AddODataClientHandler<VerificationODataClientHandler>()
                .AddHttpClient()
                .AddHttpMessageHandler<VerificationHttpClientHandler>();

            return sc.BuildServiceProvider();
        }
    }
}
