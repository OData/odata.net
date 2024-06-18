//-----------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.OData.Client.E2E.TestCommon
{
    /// <summary>
    /// Extension for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Config the controller provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="controllers">The configured controllers.</param>
        /// <returns>The caller.</returns>
        public static IServiceCollection ConfigureControllers(this IServiceCollection services, params Type[] controllers)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddControllers()
                .ConfigureApplicationPartManager(pm =>
                {
                    pm.FeatureProviders.Add(new WebODataControllerFeatureProvider(controllers));
                });

            return services;
        }
    }
}
