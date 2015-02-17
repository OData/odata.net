//---------------------------------------------------------------------
// <copyright file="SilverlightUriGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using System.Windows;
    using Microsoft.Test.OData.Services.TestServices;

    public class SilverlightUriGenerator : IServiceUriGenerator
    {
        /// <summary>
        /// Gets uri from the Silverlight app init params that corresponds to the given path
        /// </summary>
        /// <param name="path">A unique path segment to identify the service.</param>
        /// <returns>Uri for the locally hosted service.</returns>
        public Uri GenerateServiceUri(string path)
        {
            var uriInitParam = Application.Current.Host.InitParams[path];
            return new Uri(uriInitParam);
        }
    }
}