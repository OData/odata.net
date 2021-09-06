//---------------------------------------------------------------------
// <copyright file="ExternalHostedServiceWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Server
{
    using System;

    /// <summary>
    /// Client only realization of the IServiceWrapper. The service should be hosted externally.
    /// </summary>
    public class ExternalHostedServiceWrapper : IServiceWrapper
    {
        /// <summary>
        /// Initializes a new instance of the ExternalHostedServiceWrapper class.
        /// </summary>
        /// <param name="descriptor">Descriptor for the service to wrap.</param>
        public ExternalHostedServiceWrapper(ServiceDescriptor descriptor)
        {
            this.ServiceUri = descriptor.CreateServiceUri();
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
            // TODO: Verify service is running or return error
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void StopService()
        {
        }
    }
}
