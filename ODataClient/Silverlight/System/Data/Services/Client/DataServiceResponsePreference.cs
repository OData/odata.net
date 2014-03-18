//---------------------------------------------------------------------
// <copyright file="DataServiceResponsePreference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Preference for response behavior of the data service.
// </summary>
//---------------------------------------------------------------------

namespace System.Data.Services.Client
{
    /// <summary>Determines whether the client requests that the data service return inserted or updated entity data as an entry in the response message.</summary>
    public enum DataServiceResponsePreference
    {
        /// <summary>default option, no Prefer header is sent.</summary>
        None = 0,

        /// <summary>Prefer header with value return-content is sent with all PUT/MERGE/POST requests to entities.</summary>
        IncludeContent,

        /// <summary>Prefer header with value return-no-content is sent with all PUT/MERGE/POST requests to entities.</summary>
        NoContent,
    }
}
