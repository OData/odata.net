//---------------------------------------------------------------------
// <copyright file="DesktopExternalUriGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using System.Configuration;
    using Microsoft.Test.OData.Services.TestServices;

    public class DesktopExternalUriGenerator : IServiceUriGenerator
    {
        /// <summary>
        /// Read the uri of the service from the application settings
        /// </summary>
        /// <param name="path">A unique path segment to identify the service.</param>
        /// <returns>Uri of the requested service.</returns>
        public Uri GenerateServiceUri(string path)
        {
            return new Uri(ConfigurationManager.AppSettings[path]);
        }
    }
}
