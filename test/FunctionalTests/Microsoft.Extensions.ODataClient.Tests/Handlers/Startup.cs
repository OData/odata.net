//---------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers
{
    class Startup
    {
        internal IServiceProvider ConfigureServices(ServiceCollection sc)
        {
            sc.AddSingleton<VerificationCounter>();
            sc.AddTransient<VerificationController>();

            sc.AddTransient<VerificationODataClientHandler>();
            sc.AddTransient<VerificationHttpClientHandler>();

            sc
                .AddODataClient("Verification")
                .AddODataClientHandler<VerificationODataClientHandler>()
                .AddHttpClient()
                .AddHttpMessageHandler<VerificationHttpClientHandler>();

            return sc.BuildServiceProvider();
        }
    }
}
