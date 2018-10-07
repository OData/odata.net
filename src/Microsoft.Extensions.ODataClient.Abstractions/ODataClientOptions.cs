//---------------------------------------------------------------------
// <copyright file="ODataClientOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Extensions.ODataClient
{
    /// <summary>
    /// An options class for configuring the default IODataClientFactory.
    /// </summary>
    public class ODataClientOptions
    {
        /// <summary>
        /// The default logic name if no name is specified for the client when creating client from factory.
        /// </summary>
        public const string DefaultName = "";

        /// <summary>
        /// Gets a list of operations used to configure an IODataClientFactory.
        /// </summary>
        public IList<IODataClientHandler> ODataHandlers { get; } = new List<IODataClientHandler>();
    }
}
