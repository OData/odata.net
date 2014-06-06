//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.ServiceModel.Web;

    #endregion Namespaces

    /// <summary>
    /// This structure supports the .NET Framework infrastructure and is 
    /// not intended to be used directly from your code.
    /// </summary>
    /// <internal>
    /// Provides a host for services of type DataService.
    /// </internal>
    [CLSCompliant(false)]
    public class DataServiceHost : WebServiceHost
    {
        /// <summary>Instantiates <see cref="T:Microsoft.OData.Service.DataServiceHost" /> for WCF Data Services.</summary>
        /// <param name="serviceType">Identifies the WCF Data Services to the host.</param>
        /// <param name="baseAddresses">The URI of the host.</param>
        public DataServiceHost(Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
