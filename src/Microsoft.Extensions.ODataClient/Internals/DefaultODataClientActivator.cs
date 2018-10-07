//---------------------------------------------------------------------
// <copyright file="DefaultODataClientActivator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.OData.Client;
    using Microsoft.Extensions.Logging;
    using System;

    internal sealed class DefaultODataClientActivator : IODataClientActivator
    {
        private readonly ILogger<DefaultODataClientActivator> logger;

        /// <summary>
        /// constructor for default OData client activator.
        /// </summary>
        public DefaultODataClientActivator(ILogger<DefaultODataClientActivator> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public T CreateClient<T>(Uri serviceRoot) where T : DataServiceContext
        {
            // default to higest protocol version client support.
            var odataVersion = ODataProtocolVersion.V401;

            T container = (T)Activator.CreateInstance(typeof(T), new Object[] { serviceRoot });

            Log.ContainerCreated(this.logger, odataVersion, serviceRoot, null);

            return container;
        }
                
        private static class Log
        {
            public static readonly Action<ILogger, ODataProtocolVersion, Uri, Exception> ContainerCreated = LoggerMessage.Define<ODataProtocolVersion, Uri>(
                LogLevel.Information,
                new EventId(1003, nameof(ContainerCreated)),
                "Created OData {ODataVersion} container with root uri:{ServiceRoot}");
        }
    }
}
