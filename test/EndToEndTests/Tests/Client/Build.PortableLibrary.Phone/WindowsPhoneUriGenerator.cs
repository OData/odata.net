//---------------------------------------------------------------------
// <copyright file="WindowsPhoneUriGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using Microsoft.Test.OData.Services.TestServices;

    public class WindowsPhoneUriGenerator : IServiceUriGenerator
    {
        /// <summary>
        /// Gets the uri of the data service
        /// </summary>
        /// <param name="path">A unique path segment to identify the service.</param>
        /// <returns>Uri for the locally hosted service.</returns>
        public Uri GenerateServiceUri(string path)
        {
            if (path == "ODL")
            {
                return new Uri("http://localhost:19691/DefaultService/");
            }
            return new Uri("http://localhost:6630/DefaultService.svc/");
        }
    }
}
