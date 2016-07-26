//---------------------------------------------------------------------
// <copyright file="ODataBatchPayloadUriOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Indicates the format of Request-URI in each sub request in the batch operation.
    /// </summary>
    public enum BatchPayloadUriOption
    {
        /// <summary>
        /// Absolute URI with schema, host, port, and absolute resource path.
        /// </summary>
        /// Example:
        /// GET https://host:1234/path/service/People(1) HTTP/1.1
        AbsoluteUri,

        /// <summary>
        /// Absolute resource path and separate Host header.
        /// </summary>
        /// Example:
        /// GET /path/service/People(1) HTTP/1.1
        /// Host: myserver.mydomain.org:1234
        AbsoluteUriUsingHostHeader,

        /// <summary>
        /// Resource path relative to the batch request URI.
        /// </summary>
        /// Example:
        /// GET People(1) HTTP/1.1
        RelativeUri
    }
}
