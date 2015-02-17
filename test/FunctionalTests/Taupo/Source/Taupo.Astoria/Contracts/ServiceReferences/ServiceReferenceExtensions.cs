//---------------------------------------------------------------------
// <copyright file="ServiceReferenceExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ServiceReferences
{
    using System;
    using System.ServiceModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Extension methods to simplify registration of injected service references
    /// </summary>
    public static class ServiceReferenceExtensions
    {
        /// <summary>
        /// Registers a custom resolver for the given service reference client type.
        /// Uses a resolver for specific uris as well as a resolver for a backup general uri plus suffix.
        /// If the specific uri resolver returns null, then the general one will be used with the given relative uri.
        /// If they both return null, then a null value will be returned for the dependency.
        /// </summary>
        /// <typeparam name="TClient">The service reference client type contract</typeparam>
        /// <param name="container">The dependency injection container</param>
        /// <param name="specificUriResolver">The resolver for uris specific to this service</param>
        /// <param name="generalBaseUriResolver">The resolver for general uris for test services</param>
        /// <param name="relativePath">The uri for this service relative to the general base test services uri</param>
        public static void RegisterServiceReference<TClient>(this DependencyInjectionContainer container, Func<Uri> specificUriResolver, Func<Uri> generalBaseUriResolver, string relativePath)
        {
            container.RegisterServiceReference<TClient>(
                specificUriResolver,
                () =>
                {
                    var baseUri = generalBaseUriResolver();
                    if (baseUri == null)
                    {
                        return null;
                    }

                    return new Uri(baseUri, relativePath);
                });
        }

        /// <summary>
        /// Registers a custom resolver for the given service reference client type using a sequence of uri resolvers.
        /// If a resolver returns null, the next one will be used. If they all return null, then a null value will be returned for the dependency.
        /// </summary>
        /// <typeparam name="TClient">The service reference client type contract</typeparam>
        /// <param name="container">The dependency injection container</param>
        /// <param name="serviceUriResolvers">The uri resolvers to try</param>
        public static void RegisterServiceReference<TClient>(this DependencyInjectionContainer container, params Func<Uri>[] serviceUriResolvers)
        {
            ExceptionUtilities.CheckArgumentNotNull(container, "container");
            ExceptionUtilities.CheckCollectionNotEmpty(serviceUriResolvers, "serviceUriResolvers");
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(serviceUriResolvers, "serviceUriResolvers");

            var options = container.RegisterCustomResolver(
                typeof(TClient),
                t =>
                {
                    Uri serviceUri = null;
                    foreach (var uriResolver in serviceUriResolvers)
                    {
                        serviceUri = uriResolver();
                        if (serviceUri != null)
                        {
                            break;
                        }
                    }

                    if (serviceUri == null)
                    {
                        return null;
                    }

                    var factory = container.Resolve<IServiceReferenceFactory>();
                    return factory.CreateInstance<TClient>(serviceUri);
                });

            options.IsTransient = true;
        }
    }
}
