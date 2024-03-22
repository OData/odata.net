//---------------------------------------------------------------------
// <copyright file="ODataAvroScenarioTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Test.OData.PluggableFormat.Tests
{
    internal class ServiceProviderBuilderHelper
    {
        public static IServiceProvider BuildServiceProvider(Action<IServiceCollection> configureServices)
        {
            ServiceCollection services = new ServiceCollection();
            configureServices.Invoke(services);
            return services.BuildServiceProvider();
        }
    }
}
