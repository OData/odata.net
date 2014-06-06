//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
