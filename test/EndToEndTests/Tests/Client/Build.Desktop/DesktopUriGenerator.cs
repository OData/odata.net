//---------------------------------------------------------------------
// <copyright file="DesktopUriGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using System.Globalization;
    using Microsoft.Test.OData.Services.TestServices;

    public class DesktopUriGenerator : IServiceUriGenerator
    {
        public int ServicePort = 9090;

        /// <summary>
        /// Generates a Uri for a test service hosted on the local machine.
        /// </summary>
        /// <param name="port">The port for this hosted service.</param>
        /// <param name="path">A unique path segment to identify the service.</param>
        /// <returns>Uri for the locally hosted service.</returns>
        public Uri GenerateServiceUri(string path)
        {
            return new Uri("http://" + Environment.MachineName + ":" + this.ServicePort + "/" + path + DateTimeOffset.Now.Ticks.ToString(CultureInfo.InvariantCulture) + "/");
        }
    }
}
