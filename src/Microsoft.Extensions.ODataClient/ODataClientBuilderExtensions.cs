//---------------------------------------------------------------------
// <copyright file="ODataClientBuilderExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.OData.Client;

    /// <summary>
    /// Extension methods for configuring an <see cref="IODataClientBuilder"/>
    /// </summary>
    public static class ODataClientBuilderExtensions
    {
        /// <summary>
        /// Adds a delegate that will be used to configure a named OData proxy.
        /// </summary>
        /// <param name="builder">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureProxy">A delegate that is used to configure an OData proxy.</param>
        /// <returns>An <see cref="IODataClientBuilder"/> that can be used to configure the client.</returns>
        public static IODataClientBuilder ConfigureODataClient(this IODataClientBuilder builder, Action<DataServiceContext> configureProxy)
        {
            builder.Services.Configure<ODataClientOptions>(
                builder.Name, 
                options => options.ODataHandlers.Add(new DelegatingODataClientHandler(configureProxy)));

            return builder;
        }

        /// <summary>
        /// Adds an additional message handler from the dependency injection container for a named OData proxy.
        /// </summary>
        /// <param name="builder">The <see cref="IODataClientBuilder"/>.</param>
        /// <returns>An <see cref="IODataClientBuilder"/> that can be used to configure the client.</returns>
        /// <typeparam name="THandler">
        /// The type of the <see cref="IODataClientHandler"/>. The handler type will be registered as a transient service.
        /// </typeparam>
        /// <remarks>
        /// <para>
        /// The <typeparamref name="THandler"/> will be resolved from a scoped service provider that shares 
        /// the lifetime of the handler being constructed.
        /// </para>
        /// </remarks>
        public static IODataClientBuilder AddODataClientHandler<THandler>(this IODataClientBuilder builder)
            where THandler : class, IODataClientHandler
        {
            // Use transient as those handler will only be created a few times, to transient is not that expensive.
            // Adding as singleton will need handler to make sure the class is thread safe
            builder.Services.AddTransient<THandler>();

            builder.Services
                .AddOptions<ODataClientOptions>(builder.Name)
                .Configure<THandler>((o, h) => o.ODataHandlers.Add(h));

            return builder;
        }

        /// <summary>
        /// Adds a delegate that will be used to configure the http client for the named OData proxy.
        /// </summary>
        /// <param name="builder">The <see cref="IServiceCollection"/>.</param>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the http client.</returns>
        public static IHttpClientBuilder AddHttpClient(this IODataClientBuilder builder)
        {
            builder.AddODataClientHandler<HttpClientODataClientHandler>();
            return builder.Services.AddHttpClient(builder.Name);
        }
    }
}
