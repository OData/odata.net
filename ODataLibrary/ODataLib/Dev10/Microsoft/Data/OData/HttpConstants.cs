//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Constant values used by the HTTP protocol.
    /// </summary>
    internal static class HttpConstants
    {
        /// <summary>
        /// HTTP method name for GET requests.
        /// </summary>
        internal const string HttpMethodGet = "GET";

        /// <summary>
        /// HTTP method name for POST requests.
        /// </summary>
        internal const string HttpMethodPost = "POST";

        /// <summary>
        /// HTTP method name for PUT requests.
        /// </summary>
        internal const string HttpMethodPut = "PUT";

        /// <summary>
        /// HTTP method name for DELETE requests.
        /// </summary>
        internal const string HttpMethodDelete = "DELETE";

        /// <summary>
        /// HTTP method name for PATCH requests.
        /// </summary>
        internal const string HttpMethodPatch = "PATCH";

        /// <summary>
        /// Custom HTTP method name for MERGE requests.
        /// </summary>
        internal const string HttpMethodMerge = "MERGE";

        /// <summary>
        /// 'q' - HTTP q-value parameter name.
        /// </summary>
        internal const string HttpQValueParameter = "q";

        /// <summary>Http Version in batching requests and response.</summary>
        internal const string HttpVersionInBatching = "HTTP/1.1";

        /// <summary>'charset' - HTTP parameter name.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Charset", Justification = "Member name chosen based on HTTP header name.")]
        internal const string Charset = "charset";

        /// <summary>multi-part keyword in content-type to identify batch separator</summary>
        internal const string HttpMultipartBoundary = "boundary";

        /// <summary>Name of the HTTP content transfer encoding header.</summary>
        internal const string ContentTransferEncoding = "Content-Transfer-Encoding";

        /// <summary>Content-Transfer-Encoding value for batch requests.</summary>
        internal const string BatchRequestContentTransferEncoding = "binary";

        /// <summary>Name of the HTTP content-ID header.</summary>
        internal const string ContentId = "Content-ID";
    }
}
