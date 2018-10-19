//------------------------------------------------------------------
// <copyright file="IODataClientHandler.cs" company="Microsoft Corporation">
//     Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    /// <summary>
    /// A single handler that can alter the behavior of odataclient
    /// </summary>
    public interface IODataClientHandler
    {
        /// <summary>
        /// Called after IODataProxyFactory{T}.CreateProxy(string, string)
        /// If multiple handlers are registered, then they are called in reverse order of registration.
        /// </summary>
        /// <param name="args">the output proxy</param>
        void OnClientCreated(ClientCreatedArgs args);
    }
}
