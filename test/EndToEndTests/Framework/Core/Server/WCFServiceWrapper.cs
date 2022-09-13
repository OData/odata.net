//---------------------------------------------------------------------
// <copyright file="WCFServiceWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Server
{
    using System;
    using System.ServiceModel.Web;

    /// <summary>
    /// Implementation of the IServiceWrapper, using WebServiceHost to host the service.
    /// </summary>
    public class WCFServiceWrapper : IServiceWrapper
    {
        private readonly WebServiceHost webServiceHost;

        /// <summary>
        /// Initializes a new instance of the WCFServiceWrapper class.
        /// </summary>
        /// <param name="descriptor">Descriptor for the service to wrap.</param>
        public WCFServiceWrapper(ServiceDescriptor descriptor)
        {
            this.ServiceUri = descriptor.CreateServiceUri();
            this.webServiceHost = new WebServiceHost(descriptor.ServiceType, new[] { this.ServiceUri });
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
            this.webServiceHost.Open();
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void StopService()
        {
            this.webServiceHost.Close();
        }
    }
}
