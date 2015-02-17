//---------------------------------------------------------------------
// <copyright file="ServiceConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;

    /// <summary>
    /// Service wide constant values.
    /// </summary>
    public static class ServiceConstants
    {
        static ServiceConstants()
        {
            ServiceBaseUri = new Uri(OperationContext.Current.Host.BaseAddresses.First().AbsoluteUri.TrimEnd('/') + "/OData/");
        }

        /// <summary>
        /// Gets the value of the service's base URI.
        /// </summary>
        public static Uri ServiceBaseUri { get; private set; }
    }
}