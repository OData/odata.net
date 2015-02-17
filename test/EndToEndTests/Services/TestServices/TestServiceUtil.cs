//---------------------------------------------------------------------
// <copyright file="TestServiceUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Static helper methods for interacting with OData test services.
    /// </summary>
    public static class TestServiceUtil
    {
        public static IServiceUriGenerator ServiceUriGenerator { get; set; }

        /// <summary>
        /// Generates a Uri for a test service hosted on the local machine.
        /// </summary>
        /// <param name="path">A unique path segment to identify the service.</param>
        /// <returns>Uri for the locally hosted service.</returns>
        public static Uri GenerateServiceUri(string path)
        {
            return ServiceUriGenerator.GenerateServiceUri(path);
        }
    }
}
