//---------------------------------------------------------------------
// <copyright file="HttpVerb.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    /// <summary>
    /// Represents commonly used HTTP methods like GET, POST, etc
    /// </summary>
    public enum HttpVerb
    {
        /// <summary>
        /// Represents the GET http method, considered the default
        /// </summary>
        Get = 0,

        /// <summary>
        /// Represents the POST http method
        /// </summary>
        Post,

        /// <summary>
        /// Represents the PUT http method
        /// </summary>
        Put,

        /// <summary>
        /// Represents the DELETE http method
        /// </summary>
        Delete,

        /// <summary>
        /// Represents the HEAD http method
        /// </summary>
        Head,

        /// <summary>
        /// Represents the TRACE http method
        /// </summary>
        Trace,

        /// <summary>
        /// Represents the OPTIONS http method
        /// </summary>
        Options,

        /// <summary>
        /// Represents the PATCH http method
        /// </summary>
        Patch
    }
}
