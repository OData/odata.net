//---------------------------------------------------------------------
// <copyright file="DefaultServiceWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Server
{
    using System;
    using Microsoft.OData.Service;

    /// <summary>
    /// Default realization of the IServiceWrapper, using DataServiceHost to host the service.
    /// </summary>
    public class DefaultServiceWrapper : IServiceWrapper
    {
        private readonly DataServiceHost dataServiceHost;

        /// <summary>
        /// Initializes a new instance of the DefaultServiceWrapper class.
        /// </summary>
        /// <param name="descriptor">Descriptor for the service to wrap.</param>
        public DefaultServiceWrapper(ServiceDescriptor descriptor)
        {
            this.ServiceUri = descriptor.CreateServiceUri();
            this.dataServiceHost = new DataServiceHost(descriptor.ServiceType, new[] { this.ServiceUri });
        }

        /// <summary>
        /// Gets the URI for the service.
        /// </summary>
        public Uri ServiceUri { get; private set; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void StartService()
        {
            this.dataServiceHost.Open();
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void StopService()
        {
            this.dataServiceHost.Close();
        }
    }
}
