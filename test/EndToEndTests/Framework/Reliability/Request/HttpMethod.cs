//---------------------------------------------------------------------
// <copyright file="HttpMethod.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    /// <summary>
    /// Http method
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        /// Http Get
        /// </summary>
        Get,

        /// <summary>
        /// Http Head
        /// </summary>
        Head,

        /// <summary>
        /// Http Post
        /// </summary>
        Post,

        /// <summary>
        /// Http Put
        /// </summary>
        Put,

        /// <summary>
        /// Http Delete
        /// </summary>
        Delete,

        /// <summary>
        /// Http Trace
        /// </summary>
        Trace,

        /// <summary>
        /// Http Options
        /// </summary>
        Options,

        /// <summary>
        /// Http Patch
        /// -- V4 change Merge to Patch
        /// </summary>
        Patch
    }
}
