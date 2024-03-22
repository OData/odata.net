//---------------------------------------------------------------------
// <copyright file="ServiceProviderHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.OData.Core.Tests.DependencyInjection
{
    public static class ServiceProviderHelper
    {
        public static IServiceProvider BuildServiceProvider(Action<IServiceCollection> action)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDefaultODataServices();

            action?.Invoke(services);

            return services.BuildServiceProvider();
        }
    }
}
