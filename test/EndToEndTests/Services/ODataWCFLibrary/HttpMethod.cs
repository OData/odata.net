//---------------------------------------------------------------------
// <copyright file="HttpMethod.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    public enum HttpMethod
    {
        /// <summary>Default value of HttpMethod, represents a invalid method.</summary>
        Unknown,

        /// <summary>Represents the GET http method.</summary>
        GET,

        /// <summary>Represents the PUT http method.</summary>
        PUT,

        /// <summary>Represents the POST http method.</summary>
        POST,

        /// <summary>Represents the DELETE http method.</summary>
        DELETE,

        /// <summary>Represents the PATCH http method.</summary>
        PATCH,
    }
}