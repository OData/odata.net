//---------------------------------------------------------------------
// <copyright file="WCFServiceWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    /// <summary>
    /// Implementation is mostly in WCFServiceWrapper base class.
    /// </summary>
    public class WCFServiceWrapper : ServiceWrapper
    {
        /// <summary>
        /// Initializes a new instance of the WCFServiceWrapper class.
        /// </summary>
        /// <param name="descriptor">Descriptor for the service to wrap.</param>
        public WCFServiceWrapper(ServiceDescriptor descriptor)
        {
            ServiceProcess.StartInfo.Arguments = string.Format("{0} {1} a",
                (int)descriptor.IPCCommand, (int)IPCCommandMap.ServiceType.WCF);
        }
    }
}