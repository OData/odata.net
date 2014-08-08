//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services
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
        /// <summary>Creates a new <see cref="T:System.Data.Services.DataServiceHost" /> from the URI.</summary>
        /// <returns>The new <see cref="T:System.Data.Services.DataServiceHost" />.</returns>
        /// <param name="serviceType">The type of WCF service to host.</param>
        /// <param name="baseAddresses">An array of base addresses for the service. </param>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new DataServiceHost(serviceType, baseAddresses);
        }
    }
}
