//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services
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
        /// <summary>Instantiates <see cref="T:System.Data.Services.DataServiceHost" /> for WCF Data Services.</summary>
        /// <param name="serviceType">Identifies the WCF Data Services to the host.</param>
        /// <param name="baseAddresses">The URI of the host.</param>
        public DataServiceHost(Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
