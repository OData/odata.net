//---------------------------------------------------------------------
// <copyright file="ContainerBuilderExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.UriParser;

namespace Microsoft.OData
{
    /// <summary>
    /// Extension methods for <see cref="IContainerBuilder"/>.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        #region Overloads for IContainerBuilder.AddService

        /// <summary>
        /// Adds a service of <typeparamref name="TService"/> with an <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="builder">The <see cref="IContainerBuilder"/> to add the service to.</param>
        /// <param name="lifetime">The lifetime of the service to register.</param>
        /// <returns>The <see cref="IContainerBuilder"/> instance itself.</returns>
        public static IContainerBuilder AddService<TService, TImplementation>(
            this IContainerBuilder builder,
            ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            Debug.Assert(builder != null, "builder != null");

            return builder.AddService(lifetime, typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a service of <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IContainerBuilder"/> to add the service to.</param>
        /// <param name="lifetime">The lifetime of the service to register.</param>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <returns>The <see cref="IContainerBuilder"/> instance itself.</returns>
        public static IContainerBuilder AddService(
            this IContainerBuilder builder,
            ServiceLifetime lifetime,
            Type serviceType)
        {
            Debug.Assert(builder != null, "builder != null");
            Debug.Assert(serviceType != null, "serviceType != null");

            return builder.AddService(lifetime, serviceType, serviceType);
        }

        /// <summary>
        /// Adds a service of <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="builder">The <see cref="IContainerBuilder"/> to add the service to.</param>
        /// <param name="lifetime">The lifetime of the service to register.</param>
        /// <returns>The <see cref="IContainerBuilder"/> instance itself.</returns>
        public static IContainerBuilder AddService<TService>(
            this IContainerBuilder builder,
            ServiceLifetime lifetime)
            where TService : class
        {
            Debug.Assert(builder != null, "builder != null");

            return builder.AddService(lifetime, typeof(TService));
        }

        /// <summary>
        /// Adds a service of <typeparamref name="TService"/> with an <paramref name="implementationFactory"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="builder">The <see cref="IContainerBuilder"/> to add the service to.</param>
        /// <param name="lifetime">The lifetime of the service to register.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>The <see cref="IContainerBuilder"/> instance itself.</returns>
        public static IContainerBuilder AddService<TService>(
            this IContainerBuilder builder,
            ServiceLifetime lifetime,
            Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            Debug.Assert(builder != null, "builder != null");
            Debug.Assert(implementationFactory != null, "implementationFactory != null");

            return builder.AddService(lifetime, typeof(TService), implementationFactory);
        }

        #endregion

        /// <summary>
        /// Adds a service prototype of type <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service prototype to add.</typeparam>
        /// <param name="builder">The <see cref="IContainerBuilder"/> to add the service to.</param>
        /// <param name="instance">The service prototype to add.</param>
        /// <returns>The <see cref="IContainerBuilder"/> instance itself.</returns>
        public static IContainerBuilder AddServicePrototype<TService>(
            this IContainerBuilder builder,
            TService instance)
        {
            Debug.Assert(builder != null, "builder != null");

            return builder.AddService(ServiceLifetime.Singleton, sp => new ServicePrototype<TService>(instance));
        }

        /// <summary>
        /// Adds the default OData services to the <see cref="IContainerBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IContainerBuilder"/> to add the services to.</param>
        /// <returns>The <see cref="IContainerBuilder"/> instance itself.</returns>
        public static IContainerBuilder AddDefaultODataServices(this IContainerBuilder builder)
        {
            return builder.AddDefaultODataServices(ODataVersion.V4);
        }

        /// <summary>
        /// Adds the default OData services to the <see cref="IContainerBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IContainerBuilder"/> to add the services to.</param>
        /// <param name="odataVersion">ODataVersion for the default services.</param>
        /// <returns>The <see cref="IContainerBuilder"/> instance itself.</returns>
        public static IContainerBuilder AddDefaultODataServices(this IContainerBuilder builder, ODataVersion odataVersion)
        {
            Debug.Assert(builder != null, "builder != null");

            builder.AddService<IJsonReaderFactory, DefaultJsonReaderFactory>(ServiceLifetime.Singleton);
            builder.AddService<IJsonWriterFactory>(ServiceLifetime.Singleton, sp => new DefaultJsonWriterFactory());
            builder.AddService<IJsonWriterFactoryAsync>(ServiceLifetime.Singleton, sp => new DefaultJsonWriterFactory());
            builder.AddService(ServiceLifetime.Singleton, sp => ODataMediaTypeResolver.GetMediaTypeResolver(null));
            builder.AddService<ODataMessageInfo>(ServiceLifetime.Scoped);
            builder.AddServicePrototype(new ODataMessageReaderSettings());
            builder.AddService(ServiceLifetime.Scoped, sp => sp.GetServicePrototype<ODataMessageReaderSettings>().Clone());
            builder.AddServicePrototype(new ODataMessageWriterSettings());
            builder.AddService(ServiceLifetime.Scoped, sp => sp.GetServicePrototype<ODataMessageWriterSettings>().Clone());
            builder.AddService(ServiceLifetime.Singleton, sp => ODataPayloadValueConverter.GetPayloadValueConverter(null));
            builder.AddService<IEdmModel>(ServiceLifetime.Singleton, sp => EdmCoreModel.Instance);
            builder.AddService(ServiceLifetime.Singleton, sp => ODataUriResolver.GetUriResolver(null));
            builder.AddService<ODataUriParserSettings>(ServiceLifetime.Scoped);
            builder.AddService<UriPathParser>(ServiceLifetime.Scoped);
            builder.AddServicePrototype(new ODataSimplifiedOptions(odataVersion));
            builder.AddService(ServiceLifetime.Scoped, sp => sp.GetServicePrototype<ODataSimplifiedOptions>().Clone());

            return builder;
        }
    }
}
