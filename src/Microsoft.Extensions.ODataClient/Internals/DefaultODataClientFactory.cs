//---------------------------------------------------------------------
// <copyright file="DefaultODataClientFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.OData.Client;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Factory for Containers to an ODATA v4 service. 
    /// </summary>
    /// <remarks>
    ///    
    /// For containers to ODATA v3 services, a separate factory will be used. The interfaces will be similar
    /// but it will rely on basic types from the ODATA v3 client package instead of the v4 package.
    /// </remarks>
    internal sealed class DefaultODataClientFactory<T> : IODataClientFactory<T> where T : DataServiceContext
    {
        private readonly IEnumerable<IODataClientHandler> handlers;
        private readonly ILogger<DefaultODataClientFactory<T>> logger;

        /// <summary>
        /// constructor for default client factory.
        /// </summary>
        public DefaultODataClientFactory(ILoggerFactory loggerFactory, IOptions<ODataClientOptions> options)
        {
            if (options == null || options.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            var config = options.Value;
            this.handlers = config.ODataHandlers;
            this.logger = loggerFactory.CreateLogger<DefaultODataClientFactory<T>>();
        }

        /// <summary>
        /// create a connection to an OData service, specifying a named HTTP client
        /// </summary>
        /// <param name="name">the logic name of the client to use, including both HttpClient and ODataClient.</param>
        /// <returns></returns>
        public T CreateClient(string name)
        {
            // default to higest protocol version client support.
            var odataVersion = ODataProtocolVersion.V401;
            Log.BeforeCreateClient(this.logger, odataVersion, name, null);

            T container = (T)Activator.CreateInstance(typeof(T), new Object[] { });

            Log.ContainerCreated(this.logger, odataVersion, name, null);

            var args = new ClientCreatedArgs(name, container);

            this.OnClientCreated(args);
            return container;
        }

        /// <summary>
        /// call OnProxyCreated for each this.handlers
        /// </summary>
        /// <param name="args">ProxyCreatedArgs</param>
        private void OnClientCreated(ClientCreatedArgs args)
        {
            if (this.handlers == null)
            {
                return;
            }            

            foreach (IODataClientHandler handler in this.handlers)
            {
                var odataVersion = args.ODataClient.MaxProtocolVersion;
                Log.OnClientCreatedHandler(this.logger, odataVersion, handler.GetType().FullName, args.Name, null);
                handler.OnClientCreated(args);
            }
        }

        private static class Log
        {
            public static readonly Action<ILogger, ODataProtocolVersion, string, Exception> BeforeCreateClient = LoggerMessage.Define<ODataProtocolVersion, string>(
                LogLevel.Debug,
                new EventId(1001, nameof(BeforeCreateClient)),
                "Before Creat OData {ODataVersion} client factory for service:{serviceName}, version:{schemaVersion}, logical name:{name}");

            public static readonly Action<ILogger, ODataProtocolVersion, string, string, Exception> OnCreatingClientHandler = LoggerMessage.Define<ODataProtocolVersion, string, string>(
                LogLevel.Information,
                new EventId(1002, nameof(OnCreatingClientHandler)),
                "Calling OnCreatingClient with {ODataVersion} handler {handlerName} for service:{serviceName}, version:{schemaVersion}, logical name:{name}");

            public static readonly Action<ILogger, ODataProtocolVersion, string, Exception> ContainerCreated = LoggerMessage.Define<ODataProtocolVersion, string>(
                LogLevel.Information,
                new EventId(1003, nameof(ContainerCreated)),
                "Created OData {ODataVersion} container created with url {rootUrl} for service:{serviceName}, version:{schemaVersion}, logical name:{name}");

            public static readonly Action<ILogger, ODataProtocolVersion, string, string, Exception> OnClientCreatedHandler = LoggerMessage.Define<ODataProtocolVersion, string, string>(
                LogLevel.Debug,
                new EventId(1004, nameof(OnClientCreatedHandler)),
                "Calling OnClientCreated with {ODataVersion} handler {handlerName} for service:{serviceName}, version:{schemaVersion}, logical name:{name}");
        }
    }
}
