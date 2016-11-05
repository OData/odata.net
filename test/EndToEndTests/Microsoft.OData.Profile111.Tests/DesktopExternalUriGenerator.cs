//---------------------------------------------------------------------
// <copyright file="DesktopExternalUriGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Test.OData.Services.TestServices;

namespace Microsoft.OData.Profile111.Tests
{
    public class DesktopExternalUriGenerator : IServiceUriGenerator
    {
        /// <summary>
        /// Read the uri of the service from the application settings
        /// </summary>
        /// <param name="path">A unique path segment to identify the service.</param>
        /// <returns>Uri of the requested service.</returns>
        public Uri GenerateServiceUri(string path)
        {
            switch (path)
            {
                case "AstoriaDefault":
                    return new Uri("http://localhost:6630/DefaultService.svc/");
                case "ODL":
                    return new Uri("http://localhost:19691/DefaultService/");
                default:
                    return null;
            }
        }
    }
}
