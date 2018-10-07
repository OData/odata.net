//---------------------------------------------------------------------
// <copyright file="ClientCreatedArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.OData.Client;
    using System.Collections.Generic;

    /// <summary>
    /// Argument passed to <see cref="IODataClientHandler.OnClientCreated(ClientCreatedArgs)"/>
    /// </summary>
    public sealed class ClientCreatedArgs 
    {
        /// <summary>
        /// Create a new instance of <see cref="ClientCreatedArgs"/>
        /// </summary>
        /// <param name="name">the logical name of the client.</param>
        /// <param name="container">the instance of data service context for client communication.</param>
        public ClientCreatedArgs(string name, DataServiceContext container)
        {
            Name = name;
            this.ODataClient = container;
        }

        /// <summary>
        /// The logic name of the client.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// the instance of data service context for client communication.
        /// </summary>
        public DataServiceContext ODataClient { get; }
    }
}