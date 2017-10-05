//---------------------------------------------------------------------
// <copyright file="DefaultServiceWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Server
{
    using System;

    /// <summary>
    /// Implementation of the IServiceWrapper. This class is utilized to communicate with the service via RPC-like calls.
    /// </summary>
    public class DefaultServiceWrapper : IServiceWrapper
    {
        /// <summary>
        /// Initializes a new instance of the DefaultServiceWrapper class.
        /// </summary>
        /// <param name="descriptor">Descriptor for the service to wrap.</param>
        public DefaultServiceWrapper(ServiceDescriptor descriptor)
        {
            // TODO: Save service URI from response.
            // this.ServiceUri = descriptor.CreateServiceUri();
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
            // TODO: Perform RPC call to start service.
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void StopService()
        {
            // TODO: Perform RPC call to stop service.
        }
    }
}