//---------------------------------------------------------------------
// <copyright file="IODataClientBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.OData.Client;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A builder for configuring named OData V4 client instances returned by factory.
    /// </summary>
    public interface IODataClientBuilder
    {
        /// <summary>
        /// Gets the name of the client configured by this builder.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the application service collection.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
