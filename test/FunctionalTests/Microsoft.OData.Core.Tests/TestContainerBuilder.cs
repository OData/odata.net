//---------------------------------------------------------------------
// <copyright file="TestContainerBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.OData.Tests
{
    public class TestContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection services = new ServiceCollection();

        public IContainerBuilder AddService(
            ServiceLifetime lifetime,
            Type serviceType,
            Type implementationType)
        {
            Debug.Assert(serviceType != null, "serviceType != null");
            Debug.Assert(implementationType != null, "implementationType != null");

            services.Add(new ServiceDescriptor(
                serviceType, implementationType, TranslateServiceLifetime(lifetime)));

            return this;
        }

        public IContainerBuilder AddService(
            ServiceLifetime lifetime,
            Type serviceType,
            Func<IServiceProvider, object> implementationFactory)
        {
            Debug.Assert(serviceType != null, "serviceType != null");
            Debug.Assert(implementationFactory != null, "implementationFactory != null");

            services.Add(new ServiceDescriptor(
                serviceType, implementationFactory, TranslateServiceLifetime(lifetime)));

            return this;
        }

        public IServiceProvider BuildContainer()
        {
            return services.BuildServiceProvider();
        }

        private static Extensions.DependencyInjection.ServiceLifetime TranslateServiceLifetime(
            ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    return Extensions.DependencyInjection.ServiceLifetime.Scoped;
                case ServiceLifetime.Singleton:
                    return Extensions.DependencyInjection.ServiceLifetime.Singleton;
                default:
                    return Extensions.DependencyInjection.ServiceLifetime.Transient;
            }
        }
    }
}
