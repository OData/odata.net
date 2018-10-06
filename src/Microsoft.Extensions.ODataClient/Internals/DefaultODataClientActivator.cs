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

        public T CreateClient<T>() where T : DataServiceContext
        {
            // default to higest protocol version client support.
            var odataVersion = ODataProtocolVersion.V401;

            T container = (T)Activator.CreateInstance(typeof(T), new Object[] { null });

            Log.ContainerCreated(this.logger, odataVersion, null);

            return container;
        }
                
        private static class Log
        {
            public static readonly Action<ILogger, ODataProtocolVersion, Exception> ContainerCreated = LoggerMessage.Define<ODataProtocolVersion>(
                LogLevel.Information,
                new EventId(1003, nameof(ContainerCreated)),
                "Created OData {ODataVersion} container created.");
        }
    }
}
