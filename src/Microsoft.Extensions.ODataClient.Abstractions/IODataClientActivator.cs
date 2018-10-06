//---------------------------------------------------------------------
// <copyright file="IODataClientActivator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Extensions.ODataClient
{
    using Microsoft.OData.Client;
    using System.Net.Http;

    /// <summary>
    /// An activator that's responsbile soly to create the raw <see cref="DataServiceContext"/> instances.
    /// </summary>
    public interface IODataClientActivator
    {
        /// <summary>
        /// Creates an <see cref="DataServiceContext"/> instance.
        T CreateClient<T>() where T : DataServiceContext;
    }
}
