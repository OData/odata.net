//---------------------------------------------------------------------
// <copyright file="WindowsStoreUriGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using Microsoft.Test.OData.Services.TestServices;

    public class WindowsStoreUriGenerator : IServiceUriGenerator
    {
        /// <summary>
        /// Gets the uri of the data service from the app.settings file inside the app package or the Documents library
        /// </summary>
        /// <param name="path">A unique path segment to identify the service.</param>
        /// <returns>Uri for the locally hosted service.</returns>
        public Uri GenerateServiceUri(string path)
        {
            return new Uri(ConfigurationManager.AppSettings[path]);
        }
    }
}
