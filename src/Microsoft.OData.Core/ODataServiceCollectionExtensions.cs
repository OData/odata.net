//---------------------------------------------------------------------
// <copyright file="ODataServiceCollectionExtensions.cs" company="Microsoft">
// Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Json;
    using Microsoft.OData.UriParser;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ODataServiceCollectionExtensions
    {

        /// <summary>
        /// Adds the default OData required services to a given <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> instance to add OData's default services to.</param>
        /// <param name="odataVersion">An <see cref="ODataVersion" /> specifying which OData version to support. Defaults to <see cref="ODataVersion.V4" />.</param>
        /// <param name="configureReaderAction">An <see cref="Action{ODataMessageReaderSettings}" /> that allows for configuring the default <see cref="ODataMessageReaderSettings" />.</param>
        /// <param name="configureWriterAction">An <see cref="Action{ODataMessageWriterSettings}" /> that allows for configuring the default <see cref="ODataMessageWriterSettings" />.</param>
        /// <param name="configureUriParserAction">An <see cref="Action{ODataUriParserSettings}" /> that allows for configuring the default <see cref="ODataUriParserSettings" />.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance we added OData services to, for fluent manipulation.</returns>
        /// <remarks>This is usually called by OData platform libraries as a part of initializing runtime components of the OData platform.</remarks>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> parameter is <c>null</c>.</exception>
        public static IServiceCollection AddDefaultODataServices(this IServiceCollection services,
            ODataVersion odataVersion = ODataVersion.V4, 
            Action<ODataMessageReaderSettings> configureReaderAction = default, 
            Action<ODataMessageWriterSettings> configureWriterAction = default,
            Action<ODataUriParserSettings> configureUriParserAction = default)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Start by registering services that do not require execution state.
            services.AddSingleton<IJsonReaderFactory, DefaultJsonReaderFactory>();
            services.AddSingleton<IJsonWriterFactory, ODataUtf8JsonWriterFactory>();
            services.AddSingleton(sp => ODataMediaTypeResolver.GetMediaTypeResolver(null));
            services.AddSingleton(sp => ODataPayloadValueConverter.GetPayloadValueConverter(null));
            services.AddSingleton<IEdmModel>(sp => EdmCoreModel.Instance);
            services.AddSingleton(sp => ODataUriResolver.GetUriResolver(null));

            // Now register services specific to a given request.
            services.AddScoped<ODataMessageInfo>();
            services.AddScoped<UriPathParser>();

            // Finally, register configurable settings.
            var readerSettings = new ODataMessageReaderSettings(odataVersion);
            configureReaderAction?.Invoke(readerSettings);
            services.AddScoped(sp => readerSettings.Clone());

            var writerSettings = new ODataMessageWriterSettings(odataVersion);
            configureWriterAction?.Invoke(writerSettings);
            services.AddScoped(sp => writerSettings.Clone());

            var parserSettings = new ODataUriParserSettings();
            configureUriParserAction?.Invoke(parserSettings);
            services.AddScoped(sp => parserSettings.Clone());

            return services;
        }

    }
}
