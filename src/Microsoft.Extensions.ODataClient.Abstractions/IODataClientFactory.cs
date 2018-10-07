//---------------------------------------------------------------------
// <copyright file="IODataClientFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.OData.Client;
    using System;
    using System.Net.Http;

    /// <summary>
    /// A factory abstraction for a component that can create <see cref="DataServiceContext"/> instances with custom
    /// configuration for a given logical name.
    /// </summary>
    /// <remarks>
    /// A default <see cref="IODataClientFactory<typeparamref name="T"/>"/> can be registered in an <see cref="IServiceCollection"/>
    /// by calling <see cref="ODataClientFactoryExtensions.AddODataClient(IServiceCollection)"/>.
    /// The default <see cref="IODataClientFactory<typeparamref name="T"/>"/> will be registered in the service collection as a singleton.
    /// </remarks>
    public interface IODataClientFactory<T> where T : DataServiceContext
    {
        /// <summary>
        /// Creates and configures an <see cref="DataServiceContext"/> instance using the configuration that corresponds
        /// to the logical name specified by <paramref name="name"/> with the specified <paramref name="serviceRoot" />
        /// </summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="name">
        /// The logical name of the client to create. the same logic name will be used to create underlying <see cref="HttpClient"/> instance for communication.
        /// </param>
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
        T CreateClient(Uri serviceRoot, string name = ODataClientOptions.DefaultName);
    }
}
