//---------------------------------------------------------------------
// <copyright file="DataServiceHostFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    /// <summary>
    /// This structure supports the .NET Framework infrastructure and is 
    /// not intended to be used directly from your code.
    /// </summary>
    /// <internal>
    /// This class is used to hook up a WCF as a raw HTTP handler.
    /// </internal>
    public class DataServiceHostFactory : ServiceHostFactory
    {
        /// <summary>Creates a new <see cref="T:Microsoft.OData.Service.DataServiceHost" /> from the URI.</summary>
        /// <returns>The new <see cref="T:Microsoft.OData.Service.DataServiceHost" />.</returns>
        /// <param name="serviceType">The type of WCF service to host.</param>
        /// <param name="baseAddresses">An array of base addresses for the service. </param>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new DataServiceHost(serviceType, baseAddresses);
        }
    }
}
