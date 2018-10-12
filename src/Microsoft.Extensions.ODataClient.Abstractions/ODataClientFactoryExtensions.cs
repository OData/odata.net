//---------------------------------------------------------------------
// <copyright file="ODataClientFactoryExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.OData.Client;
    using System;
    using System.Net.Http;

    /// <summary>
    /// Client extensions
    /// </summary>
    public static class ODataClientFactoryExtensions
    {
        /// <summary>
        /// Creates and configures an <see cref="DataServiceContext"/> instance using the configuration that corresponds
        /// to the logical name specified by <paramref name="name"/> with the specified <paramref name="serviceRoot" />
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <returns>A new <see cref="DataServiceContext"/> instance.</returns>
        /// <remarks>
        /// <para>
        /// Each call to <see cref="CreateClient(Uri, string)"/> is guaranteed to return a new <see cref="HttpClient"/>
        /// instance. Callers may cache the returned <see cref="HttpClient"/> instance indefinitely or surround
        /// its use in a <langword>using</langword> block to dispose it when desired.
        /// </para>
        /// <para>
        /// Callers are also free to mutate the returned <see cref="DataServiceContext"/> instance's public properties
        /// as desired.
        /// </para>
        /// </remarks>
        public static T CreateClient<T>(this IODataClientFactory factory, Uri serviceRoot) where T : DataServiceContext
        {
            return factory.CreateClient<T>(serviceRoot, ODataClientOptions.DefaultName);
        }
    }
}
