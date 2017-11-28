//---------------------------------------------------------------------
// <copyright file="DataServiceProtocolVersion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// Used in place of Microsoft.OData.Service.Common.DataServiceProtocolVersion to avoid a direct dependency on the product
    /// Should never be converted by value, because they explicitly do not match due to the addition of the 'Unspecified' value
    /// </summary>
    public enum DataServiceProtocolVersion
    {
        /// <summary>Unspecified data service protocol version</summary>
        Unspecified = 0,

        /// <summary>Version 4 of the Server</summary>
        V4,

        /// <summary>Version 4.01 of the Server</summary>
        V4_01,

        /// <summary>Version 5 of the Server</summary>
        LatestVersionPlusOne
    }
}